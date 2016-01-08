using System;
using Konamiman.NestorMSX.Exceptions;
using Konamiman.Z80dotNet;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using Konamiman.NestorMSX.Misc;

namespace Konamiman.NestorMSX.Hardware
{
    /// <summary>
    /// Represents a TMS9918 video display processor (text mode only supported)
    /// </summary>
    public class Tms9918 : IExternallyControlledTms9918, IDisposable
    {
        private const int colorTableLength = 32;
        private const int patternGeneratorTableLength = 2048;

        private readonly ITms9918DisplayRenderer displayRenderer;
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

        private int _patternGeneratorTableAddress;
        public int PatternGeneratorTableAddress
        {
            get
            {
                return _patternGeneratorTableAddress;
            }
            private set
            {
                _patternGeneratorTableAddress = value;
                for(var position = 0; position < patternGeneratorTableLength; position++)
                    displayRenderer.WriteToPatternGeneratorTable(position, Vram[PatternGeneratorTableAddress + position]);
            }
        }

        private byte? valueWrittenToPort1;
        private byte readAheadBuffer;
        private byte statusRegisterValue;
        private int vramPointer;
        private Bit[] modeBits;
        private int[] PatternNameTableSizes = { 768, 960 };
        
        private int screenMode = 0;

        private int _PatternNameTableAddress;
        public int PatternNameTableAddress
        {
            get
            {
                return _PatternNameTableAddress;
            }
            private set
            {
                _PatternNameTableAddress = value;
                ReprintAll();
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
                _colorTableAddress = value;
                for(int i = 0; i < colorTableLength; i++)
                    displayRenderer.WriteToColourTable(i, Vram[_colorTableAddress + i]);
            }
        }

        private static int[] allowedVramSizes = new[] {16, 64, 128};

        public Tms9918(ITms9918DisplayRenderer displayRenderer, Configuration config, int vramSizeInKB = 16)
        {
            if(!allowedVramSizes.Contains(vramSizeInKB))
                throw new ArgumentException("VRAM size must be one of: " + 
                    string.Join(", ", allowedVramSizes.Select(s => s.ToString()).ToArray()));

            vramSize = vramSizeInKB * 1024;
            vramAddressMask = vramSize - 1;

            _PatternNameTableAddress = 0x1800;
            _colorTableAddress = 0x2000;

            Vram = new PlainMemory(vramSize);
            modeBits = new Bit[] {0, 0, 0};

            this.displayRenderer = displayRenderer;
            displayRenderer.BlankScreen();
            SetScreenMode(0);

            if(config.VdpFrequencyMultiplier < 0.01M || config.VdpFrequencyMultiplier > 100)
                throw new ConfigurationException("The VDP frequency multiplier must be a number between 0.01 and 100.");

            var interruptGenerationInterval = TimeSpan.FromSeconds(((double)1)/60).TotalMilliseconds/(double)config.VdpFrequencyMultiplier;
            interruptGenerationTask = Task.Factory.StartNew(InterruptGenerationTaskProcess, interruptGenerationInterval);
        }

        private void InterruptGenerationTaskProcess(object interruptGenerationInterval)
        {
            var interval = (double)interruptGenerationInterval;
            var sw = new Stopwatch();
            sw.Start();

            while(true)
            {
                if(sw.ElapsedMilliseconds < interval)
                    continue;
                
                statusRegisterValue |= 0x80;
                if(generateInterrupts)
                    IntLineIsActive = true;
                sw.Restart();
            }
        }

        private void SetScreenMode(int mode)
        {
            screenMode = mode;
            displayRenderer.SetScreenMode((byte)mode);
            PatternNameTableSize = PatternNameTableSizes[mode & 1];
        }

        void ReprintAll()
        {
            for(int i = 0; i < PatternNameTableSize; i++)
                displayRenderer.WriteToNameTable(i, Vram[PatternNameTableAddress + i]);
        }

        public event EventHandler NmiInterruptPulse;
        public event EventHandler<VdpRegisterWrittenEventArgs> ControlRegisterWritten;

        public bool IntLineIsActive { get; private set; }
        public byte? ValueOnDataBus { get; private set; }

        public void WriteToPort(TwinBit portNumber, byte value)
        {
            if(portNumber == 0) {
                WriteVram(vramPointer, value);
                vramPointer = (vramPointer+1) & vramAddressMask;
                readAheadBuffer = value;
                valueWrittenToPort1 = null;
                return;
            }

            if (portNumber == 3) {
                WriteControlRegister(registerNumberForIndirectAccess, value);
                if(autoIncrementRegisterNumberForIndirectAccess)
                    registerNumberForIndirectAccess = (byte)((registerNumberForIndirectAccess++) & 0x3F);
                return;
            }
            
            if (valueWrittenToPort1 == null) {
                valueWrittenToPort1 = value;
                return;
            }

            if ((value & 0x80) == 0) {
                SetVramAccess(valueWrittenToPort1.Value, value, vramAddressThreeHighBits);
            } else {
                WriteControlRegister(valueWrittenToPort1.Value, value);
            }

            valueWrittenToPort1 = null;
        }

        private void SetVramAccess(byte firstByte, byte secondByte, byte vramAddressThreeHighBits)
        {
            vramPointer = (firstByte | ((secondByte & 0x3F) << 8) | (vramAddressThreeHighBits << 14)) & vramAddressMask;

            if((secondByte & 0x40) == 0) {
                readAheadBuffer = Vram[vramPointer];
                vramPointer = (vramPointer++) & vramAddressMask;
            }
        }

        private void WriteControlRegister(byte value, byte register)
        {
            ControlRegisterWritten?.Invoke(this, new VdpRegisterWrittenEventArgs(register, value));

            register &= 63;

            switch(register) {
                case 0:
                    SetModeBit(2, value.GetBit(1), true);
                    break;

                case 1:
                    SetModeBit(1, value.GetBit(4), false);
                    SetModeBit(3, value.GetBit(3), true);

                    generateInterrupts = value.GetBit(5);
                    if(generateInterrupts && statusRegisterValue.GetBit(7))
                        IntLineIsActive = true;
                    else
                        IntLineIsActive = false;

                    if(value.GetBit(6))
                        displayRenderer.ActivateScreen();
                    else
                        displayRenderer.BlankScreen();

                    break;

                case 2:
                    PatternNameTableAddress = (value << 10) & vramAddressMask;
                    break;

                case 3:
                    colorTableLow = value;
                    SetColorTableAddress();
                    break;

                case 4:
                    PatternGeneratorTableAddress = (value << 11) & vramAddressMask;
                    break;

                case 7:
                    displayRenderer.SetBackdropColor((byte)(value & 0x0F));
                    displayRenderer.SetTextColor((byte)(value >> 4));
                    break;

                case 10:
                    colorTableHigh = (byte)(value & 7);
                    SetColorTableAddress();
                    break;

                case 14:
                    vramAddressThreeHighBits = (byte)(value & 7);
                    break;

                case 15:
                    statusRegisterNumberToRead = (byte)(value & 0x0F);
                    break;

                case 17:
                    registerNumberForIndirectAccess = (byte)(value & 0x3F);
                    autoIncrementRegisterNumberForIndirectAccess = ((value & 0x80) == 0);
                    break;
            }
        }

        private void SetColorTableAddress()
        {
            ColorTableAddress = ((colorTableLow << 6) + (colorTableHigh << 14)) & vramAddressMask;
        }

        private void SetModeBit(int mode, Bit value, bool changeScreenMode)
        {
            modeBits[mode - 1] = value;
            if(!changeScreenMode)
                return;

            for(byte i = 0; i <= 2; i++) {
                if(modeBits[i]) {
                    SetScreenMode((byte)(i + 1));
                    return;
                }
            }

            SetScreenMode(0);
        }

        public byte ReadFromPort(TwinBit portNumber)
        {
            valueWrittenToPort1 = null;

            if(portNumber == 0) {
                var value = readAheadBuffer;
                vramPointer = (vramPointer+1) & vramAddressMask;
                readAheadBuffer = Vram[vramPointer];
                return value;
            }
            else if(statusRegisterNumberToRead == 0) {
                var value = statusRegisterValue;
                statusRegisterValue &= 0x7F;
                IntLineIsActive = false;
                return value;
            }
            else {
                return 0;
            }
        }

        public void WriteVram(int address, byte value)
        {
            address &= vramAddressMask;

            Vram[address] = value;
            if(address >= PatternNameTableAddress && address < PatternNameTableAddress + PatternNameTableSize) {
                displayRenderer.WriteToNameTable(address - PatternNameTableAddress, value);
            }
            if(address >= PatternGeneratorTableAddress && address < PatternGeneratorTableAddress + patternGeneratorTableLength) {
                displayRenderer.WriteToPatternGeneratorTable(address - PatternGeneratorTableAddress, value);
            }
            if(screenMode != 1 && address >= ColorTableAddress && address < ColorTableAddress + colorTableLength) {
                displayRenderer.WriteToColourTable(address - ColorTableAddress, value);
            }
        }

        public byte ReadVram(int address)
        {
            return Vram[address & vramAddressMask];
        }

        public byte[] GetVramContents(int startAddress, int length)
        {
            return Vram.GetContents(startAddress, length);
        }

        public int PatternNameTableSize { get; private set; }

        public void SetVramContents(int startAddress, byte[] contents, int startIndex = 0, int? length = null)
        {
            var actualLength = length.GetValueOrDefault(contents.Length);
            for(int i = 0; i < actualLength; i++)
                WriteVram(startAddress + i, contents[startIndex + i]);
        }

        public void Dispose()
        {
            interruptGenerationTask.Dispose();
        }
    }
}
