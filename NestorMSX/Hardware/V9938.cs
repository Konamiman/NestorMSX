using System;
using Konamiman.NestorMSX.Exceptions;
using Konamiman.Z80dotNet;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using Konamiman.NestorMSX.Misc;
using System.Drawing;
using System.Threading;

namespace Konamiman.NestorMSX.Hardware
{
    /// <summary>
    /// Represents a V9938 video display processor (text mode only supported)
    /// </summary>
    public class V9938 : IExternallyControlledV9938, IDisposable
    {
        private static decimal[] blinkTimes =
        {
            0M, 166.9M, 333.8M, 500.6M, 667.5M, 834.4M, 1001.3M, 1168.2M,
            1335.1M, 1509.9M, 1668.8M, 1835.7M, 2002.6M, 2169.5M, 2336.3M, 2503.2M
        };

        private int colorTableLength = 32;
        private const int patternGeneratorTableLength = 2048;

        private readonly IV9938DisplayRenderer displayRenderer;
        private PlainMemory Vram;
        private bool generateInterrupts;
        private Task interruptGenerationTask;

        private readonly int vramSize;
        private readonly int vramAddressMask;

        private byte registerNumberForIndirectAccess;
        private bool autoIncrementRegisterNumberForIndirectAccess;
        private byte statusRegisterNumberToRead;
        private byte vramAddressThreeHighBits;
        private byte colorTableLow;
        private byte colorTableHigh;
        private bool expansionVramSelected;
        private bool in27RowsMode;
        private byte paletteRegisterValue;
        private byte? valueWrittenToPort2;
        private byte textColor;
        private byte backdropColor;
        private byte blinkTextColor;
        private byte blinkBackdropColor;
        private bool screenIsActive;
        private bool currentlyIn80ColumnsMode = false;
        private int currentColumnsCount;

        private int _patternGeneratorTableAddress;
        public int PatternGeneratorTableAddress
        {
            get
            {
                return _patternGeneratorTableAddress;
            }
            private set
            {
                if(_patternGeneratorTableAddress != value) {
                    _patternGeneratorTableAddress = value;
                    for(var position = 0; position < patternGeneratorTableLength; position++)
                        displayRenderer.WriteToPatternGeneratorTable(position,
                            Vram[PatternGeneratorTableAddress + position]);
                }
            }
        }

        private byte? valueWrittenToPort1;
        private byte readAheadBuffer;
        private byte statusRegisterValue;
        private int vramPointer;
        private Bit[] modeBits;
        
        private int _PatternNameTableAddress;
        public int PatternNameTableAddress
        {
            get
            {
                return _PatternNameTableAddress;
            }
            private set
            {
                if(_PatternNameTableAddress != value) {
                    _PatternNameTableAddress = value;
                    ReprintAll();
                }
            }
        }

        private int _colorTableAddress;
        public int ColorTableAddress
        {
            get
            {
                return _colorTableAddress;
            }
            private set
            {
                if(_colorTableAddress != value) {
                    _colorTableAddress = value;
                    for(int i = 0; i < colorTableLength; i++)
                        displayRenderer.WriteToColourTable(i, Vram[_colorTableAddress + i]);
                }
            }
        }

        private static int[] allowedVramSizes = new[] {16, 64, 128};

        public V9938(IV9938DisplayRenderer displayRenderer, Configuration config, int vramSizeInKB = 16)
        {
            if(!allowedVramSizes.Contains(vramSizeInKB))
                throw new ArgumentException("VRAM size must be one of: " + 
                    string.Join(", ", allowedVramSizes.Select(s => s.ToString()).ToArray()));

            vramSize = vramSizeInKB * 1024;
            vramAddressMask = vramSize - 1;

            _PatternNameTableAddress = 0x1800;
            _colorTableAddress = 0x2000;
            _patternGeneratorTableAddress = 0x800;

            Vram = new PlainMemory(vramSize);
            modeBits = new Bit[] {0, 0, 0, 0, 0};
            
            this.displayRenderer = displayRenderer;
            displayRenderer.BlankScreen();

            if(config.VdpFrequencyMultiplier < 0.01M || config.VdpFrequencyMultiplier > 100)
                throw new ConfigurationException("The VDP frequency multiplier must be a number between 0.01 and 100.");

            var interruptGenerationInterval = TimeSpan.FromSeconds(((double)1)/60).TotalMilliseconds/(double)config.VdpFrequencyMultiplier;
            interruptGenerationTaskCancellatonTokenSource = new CancellationTokenSource();
            interruptGenerationTask = Task.Factory.StartNew(InterruptGenerationTaskProcess, interruptGenerationInterval, interruptGenerationTaskCancellatonTokenSource.Token);

            SetScreenMode(ScreenMode.Graphic1);
        }

        private CancellationTokenSource interruptGenerationTaskCancellatonTokenSource;
        private void InterruptGenerationTaskProcess(object interruptGenerationInterval)
        {
            var interval = (double)interruptGenerationInterval;
            var sw = new Stopwatch();
            sw.Start();

            while(!interruptGenerationTaskCancellatonTokenSource.IsCancellationRequested)
            {
                if(sw.ElapsedMilliseconds < interval)
                    continue;
                
                statusRegisterValue |= 0x80;
                if(generateInterrupts)
                    IntLineIsActive = true;
                sw.Restart();
            }
        }

        private void SetScreenMode(ScreenMode screenMode)
        {
            CurrentScreenMode = screenMode;

            previousModeBits = modeBits.ToArray();

            currentColumnsCount = screenMode == ScreenMode.Text1 ? 40 : screenMode == ScreenMode.Text2 ? 80 : 32;
            currentlyIn80ColumnsMode = (screenMode == ScreenMode.Text2);
            colorTableLength = screenMode == ScreenMode.Text2 ? 270 : 32;
            displayRenderer.SetScreenMode(screenMode);

            UpdateNumberOfRows();

            ScreenModeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateNumberOfRows()
        {
            var currentlyIn27RowsMode = currentlyIn80ColumnsMode && in27RowsMode;
            displayRenderer.SetNumberOfRows(currentlyIn27RowsMode ? 27 : 24);
            PatternNameTableSize = currentColumnsCount * (currentlyIn27RowsMode ? 27 : 24);
        }

        void ReprintAll()
        {
            for(int i = 0; i < PatternNameTableSize; i++)
                displayRenderer.WriteToNameTable(i, Vram[PatternNameTableAddress + i]);
        }

        public event EventHandler NmiInterruptPulse;
        public event EventHandler<VdpRegisterWrittenEventArgs> ControlRegisterWritten;
        public event EventHandler ScreenModeChanged;
        public event EventHandler<MemoryWrittenEventArgs> VramWritten;

        public bool IntLineIsActive { get; private set; }
        public byte? ValueOnDataBus { get; private set; }

        public void WriteToPort(TwinBit portNumber, byte value)
        {
            var oldValueWrittenToPort1 = valueWrittenToPort1;
            valueWrittenToPort1 = null;

            if(portNumber == 0) {
                WriteVram(vramPointer, value);
                vramPointer = (vramPointer+1) & vramAddressMask;
                readAheadBuffer = value;
                return;
            }

            if(portNumber == 3) {
                if(registerNumberForIndirectAccess != 17)
                    WriteControlRegister(value, registerNumberForIndirectAccess);
                if(autoIncrementRegisterNumberForIndirectAccess)
                    registerNumberForIndirectAccess = (byte)((registerNumberForIndirectAccess+1) & 0x3F);
                return;
            }

            if(portNumber == 2) {
                if(valueWrittenToPort2 == null) {
                    valueWrittenToPort2 = value;
                }
                else {
                    var r = (valueWrittenToPort2.Value >> 4) & 7;
                    var b = (valueWrittenToPort2.Value) & 7;
                    var g = value & 7;
                    var color = Color.FromArgb(r, g, b);
                    displayRenderer.SetPalette(paletteRegisterValue, color);
                    valueWrittenToPort2 = null;
                    paletteRegisterValue = (byte)((paletteRegisterValue + 1) & 0x0F);
                }
                return;
            }
                
            if(oldValueWrittenToPort1 == null) {
                valueWrittenToPort1 = value;
                return;
            }

            if ((value & 0x80) == 0) {
                SetVramAccess(oldValueWrittenToPort1.Value, value, vramAddressThreeHighBits);
            } else {
                WriteControlRegister(oldValueWrittenToPort1.Value, value);
            }
        }

        private void SetVramAccess(byte firstByte, byte secondByte, byte vramAddressThreeHighBits)
        {
            vramPointer = (firstByte | ((secondByte & 0x3F) << 8) | (vramAddressThreeHighBits << 14)) & vramAddressMask;

            if((secondByte & 0x40) == 0) {
                readAheadBuffer = Vram[vramPointer];
            }
        }

        private void WriteControlRegister(byte value, byte register)
        {
            register &= 63;

            ControlRegisterWritten?.Invoke(this, new VdpRegisterWrittenEventArgs(register, value));

            switch(register) {
                case 0:
                    SetModeBit(3, value.GetBit(1), false);
                    SetModeBit(4, value.GetBit(2), false);
                    SetModeBit(5, value.GetBit(3), true);
                    break;

                case 1:
                    SetModeBit(1, value.GetBit(4), false);
                    SetModeBit(2, value.GetBit(3), true);

                    generateInterrupts = value.GetBit(5);
                    if(generateInterrupts && statusRegisterValue.GetBit(7))
                        IntLineIsActive = true;
                    else
                        IntLineIsActive = false;

                    var newScreenIsActive = value.GetBit(6);
                    if(newScreenIsActive && !screenIsActive)
                        displayRenderer.ActivateScreen();
                    else if(!newScreenIsActive && screenIsActive)
                        displayRenderer.BlankScreen();
                    screenIsActive = newScreenIsActive;

                    break;

                case 2:
                    if(currentlyIn80ColumnsMode)
                        value &= 0xFC;
                    PatternNameTableAddress = (value << 10) & vramAddressMask;
                    break;

                case 3:
                    if (currentlyIn80ColumnsMode)
                        value &= 0xF8;
                    colorTableLow = value;
                    SetColorTableAddress();
                    break;

                case 4:
                    PatternGeneratorTableAddress = (value << 11) & vramAddressMask;
                    break;

                case 7:
                    var newTextColor = (byte)(value >> 4);
                    var newBackdropColor = (byte)(value & 0x0F);

                    if (newBackdropColor != backdropColor)
                        displayRenderer.SetBackdropColor(newBackdropColor);
                    if(newTextColor != textColor)
                        displayRenderer.SetTextColor(newTextColor);

                    backdropColor = newBackdropColor;
                    textColor = newTextColor;
                    break;

                case 9:
                    var oldIn27RowsMode = in27RowsMode;
                    in27RowsMode = (value & 0x80) != 0;
                    if(in27RowsMode != oldIn27RowsMode) {
                        UpdateNumberOfRows();
                        ReprintAll();
                    }
                    break;

                case 10:
                    colorTableHigh = (byte)(value & 7);
                    SetColorTableAddress();
                    break;

                case 12:
                    var newBlinkTextColor = (byte)(value >> 4);
                    var newBlinkBackdropColor = (byte)(value & 0x0F);

                    if (newBlinkBackdropColor != blinkBackdropColor)
                        displayRenderer.SetBlinkBackdropColor(newBlinkBackdropColor);
                    if (newBlinkTextColor != blinkTextColor)
                        displayRenderer.SetBlinkTextColor(newBlinkTextColor);

                    blinkBackdropColor = newBlinkBackdropColor;
                    blinkTextColor = newBlinkTextColor;
                    break;

                case 13:
                    var onTime = blinkTimes[(value >> 4) & 0xF];
                    var offTime = blinkTimes[value & 0xF];
                    displayRenderer.SetBlinkTimes(onTime, offTime);
                    break;

                case 14:
                    vramAddressThreeHighBits = (byte)(value & 7);
                    break;

                case 15:
                    statusRegisterNumberToRead = (byte)(value & 0x0F);
                    break;

                case 16:
                    paletteRegisterValue = (byte)(value & 0x0F);
                    break;

                case 17:
                    registerNumberForIndirectAccess = (byte)(value & 0x3F);
                    autoIncrementRegisterNumberForIndirectAccess = ((value & 0x80) == 0);
                    break;

                case 45:
                    expansionVramSelected = (value & 64) != 0;
                    break;
            }
        }

        private void SetColorTableAddress()
        {
            ColorTableAddress = ((colorTableLow << 6) + (colorTableHigh << 14)) & vramAddressMask;
        }

        private static Bit[] width80Bits = {1, 0, 0, 1, 0};

        Bit[] previousModeBits = null;
        private void SetModeBit(int modeBitNumber, Bit value, bool changeScreenMode)
        {
            modeBits[modeBitNumber - 1] = value;
            if(!changeScreenMode)
                return;

            if(previousModeBits == null)
                previousModeBits = new Bit[] { 0, 0, 0, 0, 0 };
            else if(modeBits.SequenceEqual(previousModeBits))
                return;

            var newScreenMode = 
                modeBits[0] + 
                2*modeBits[1] + 
                4*modeBits[2] + 
                8*modeBits[3] + 
                16*modeBits[4];

            SetScreenMode((ScreenMode)newScreenMode);
        }

        private byte lastValueOfS2;

        public byte ReadFromPort(TwinBit portNumber)
        {
            valueWrittenToPort1 = null;

            if(portNumber == 0)
            {
                if(expansionVramSelected)
                    return 0xFF;

                var value = readAheadBuffer;
                vramPointer = (vramPointer + 1) & vramAddressMask;
                readAheadBuffer = Vram[vramPointer];
                return value;
            }

            if (statusRegisterNumberToRead == 0) {
                var value = statusRegisterValue;
                statusRegisterValue &= 0x7F;
                IntLineIsActive = false;
                return value;
            } else if(statusRegisterNumberToRead == 2) {
                var value = lastValueOfS2;
                lastValueOfS2 ^= 0xE0;
                return value;
            }
            else {
                return 0;
            }
        }

        public void WriteVram(int address, byte value)
        {
            if (expansionVramSelected)
                return;

            address &= vramAddressMask;

            Vram[address] = value;
            if(address >= PatternNameTableAddress && address < PatternNameTableAddress + PatternNameTableSize) {
                displayRenderer.WriteToNameTable(address - PatternNameTableAddress, value);
            }
            if(address >= PatternGeneratorTableAddress && address < PatternGeneratorTableAddress + patternGeneratorTableLength) {
                displayRenderer.WriteToPatternGeneratorTable(address - PatternGeneratorTableAddress, value);
            }
            if(address >= ColorTableAddress && address < ColorTableAddress + colorTableLength) {
                displayRenderer.WriteToColourTable(address - ColorTableAddress, value);
            }

            VramWritten?.Invoke(this, new MemoryWrittenEventArgs(address, value));
        }

        public byte ReadVram(int address)
        {
            return expansionVramSelected ? (byte)0xFF : Vram[address & vramAddressMask];
        }

        public byte[] GetVramContents(int startAddress, int length)
        {
            return Vram.GetContents(startAddress, length);
        }

        public int PatternNameTableSize { get; private set; }

        public ScreenMode CurrentScreenMode { get; private set; }

        public void SetVramContents(int startAddress, byte[] contents, int startIndex = 0, int? length = null)
        {
            var actualLength = length.GetValueOrDefault(contents.Length);
            for(int i = 0; i < actualLength; i++)
                WriteVram(startAddress + i, contents[startIndex + i]);
        }

        public void Dispose()
        {
            interruptGenerationTaskCancellatonTokenSource.Cancel();
            interruptGenerationTask.ContinueWith(t => t.Dispose());
        }
    }
}
