using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using Konamiman.NestorMSX.Exceptions;
using Konamiman.NestorMSX.Hardware;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;
using System.Threading.Tasks;

namespace Konamiman.NestorMSX.Host
{
    public class DisplayRenderer : IV9938DisplayRenderer
    {
        private const int SCREEN_1 = 0;
        private const int SCREEN_0 = 1;

        private readonly ICharacterBasedDisplay display;

        private int currentScreenMode = SCREEN_1;
        private int screenWidthInCharacters = 32;
        private readonly Dictionary<Point, byte> screenBuffer = new Dictionary<Point, byte>();
        private Color[] Colors { get; set; }
        private Color BackdropColor { get; set; }
        private Color TextColor { get; set; }
        private Dictionary<byte, byte[]> characterPatterns = new Dictionary<byte, byte[]>();
        private readonly Dictionary<byte, Tuple<byte, byte>> CharacterColorsForScreen1 = new Dictionary<byte, Tuple<byte, byte>>();
        private int numberOfRows = 24;
        private int textColorIndex;
        private int backdropColorIndex;
        private int blinkTextColorIndex;
        private int blinkBackdropColorIndex;

        public DisplayRenderer(ICharacterBasedDisplay display, Configuration config)
        {
            this.display = display;
            display.SetScreenBufer(screenBuffer);

            for(int i = 0; i < 256; i++) {
                characterPatterns[(byte)i] = new byte[8];
                CharacterColorsForScreen1[(byte)i] = new Tuple<byte, byte>(0, 0);
            }

            var colorsLines = FileUtils.ReadAllLines(config.ColorsFile);
            try {
                Colors = ParseColorsFile(colorsLines);
            }
            catch(Exception ex) {
                ThrowParseColorsException(ex);
            }

            TextColor = Colors[0];
            BackdropColor = Colors[0];
        }

        private Color[] ParseColorsFile(string[] colorsLines)
        {
            var colors = new Color[16];
            for(int i = 0; i < 16; i++) {
                var line = colorsLines[i];
                var tokens = line.Split(new[] {" ", "\t"}, StringSplitOptions.RemoveEmptyEntries);
                colors[i] = Color.FromArgb(255, int.Parse(tokens[0]), int.Parse(tokens[1]), int.Parse(tokens[2]));
            }
            return colors;
        }

        protected virtual void SetScreenWidth(int width)
        {
            screenWidthInCharacters = width;
            var itemsToRemove = screenBuffer.Keys.Where(x => x.X >= width).ToArray();
            for(int i = 0; i < itemsToRemove.Length; i++) {
                screenBuffer.Remove(itemsToRemove[i]);
                display.NotifyScreenBufferContentsRemoved(itemsToRemove[i]);
            }
        }

        public void ActivateScreen()
        {
            display.ActivateScreen();
        }

        public void BlankScreen()
        {
            display.BlankScreen();
        }

        public void SetScreenMode(byte mode, byte columns)
        {
            if(mode > 1)
                return;

            currentScreenMode = mode;
            SetScreenWidth(columns);
            display.SetCharacterWidth(mode == SCREEN_0 ? 6 : 8);
            display.SetColumns(columns);

            SetAllCharsColors();

            if(mode == SCREEN_0 && columns == 80 && blinkTimesAvailable)
                EnableBlink();
            else
                DisableBlink();
        }

        public void WriteToNameTable(int position, byte value)
        {
            var coordinates = new Point(position%screenWidthInCharacters, position/screenWidthInCharacters);
            if(coordinates.X >= screenWidthInCharacters || coordinates.Y >= numberOfRows)
                return;

            screenBuffer[coordinates] = value;

            display.NotifyScreenBufferContentsAddedOrChanged(coordinates);
        }

        public void WriteToPatternGeneratorTable(int position, byte value)
        {
            var charIndex = (byte)(position/8);
            var pattern = characterPatterns[charIndex];
            var offset = position%8;
            pattern[offset] = value;

            display.SetCharacterPattern(charIndex, pattern);
        }

        public void WriteToColourTable(int position, byte value)
        {
            if(currentScreenMode == SCREEN_1)
                WriteToColourTableForScreen1(position, value);
            else if(currentScreenMode == SCREEN_0 && screenWidthInCharacters == 80)
                WriteToColourTableForScreen0(position, value);
        }

        private void WriteToColourTableForScreen1(int position, byte value)
        {
            var foregroundColorIndex = value >> 4;
            var backgroundColorIndex = value & 0x0F;

            var firstCharIndex = (position*8);
            for(byte i = 0; i < 8; i++) {
                var charIndex = (byte)(firstCharIndex + i);
                CharacterColorsForScreen1[charIndex] = new Tuple<byte, byte>((byte)foregroundColorIndex, (byte)backgroundColorIndex);
                if(currentScreenMode == SCREEN_1)
                    display.SetCharacterColors((byte)(firstCharIndex + i), Colors[foregroundColorIndex], Colors[backgroundColorIndex]);
            }
        }

        private void WriteToColourTableForScreen0(int position, byte value)
        {
            var y = position / 10;
            var x = (position - y * 10) * 8;
            
            for(var bitIndex = 7; bitIndex >=0; bitIndex--) {
                display.SetBlink(new Point(x, y), value.GetBit(bitIndex));
                x++;
            }
        }

        public void SetTextColor(byte colorIndex)
        {
            textColorIndex = colorIndex;
            TextColor = Colors[colorIndex];
            if(currentScreenMode == SCREEN_0)
                SetAllCharsColorsForScreen0();
        }

        private void SetAllCharsColors()
        {
            if(currentScreenMode == SCREEN_0)
                SetAllCharsColorsForScreen0();
            else
                SetAllCharsColorsForScreen1();
        }

        private void SetAllCharsColorsForScreen0()
        {
            for(int i = 0; i < 256; i++)
                display.SetCharacterColors((byte)i, TextColor, BackdropColor);
        }

        private void SetAllCharsColorsForScreen1()
        {
            for(int i = 0; i < 256; i++)
                display.SetCharacterColors((byte)i,
                    Colors[CharacterColorsForScreen1[(byte)i].Item1],
                    Colors[CharacterColorsForScreen1[(byte)i].Item2]);    
        }

        public void SetBackdropColor(byte colorIndex)
        {
            backdropColorIndex = colorIndex;
            display.SetBackdropColor(Colors[colorIndex]);
            BackdropColor = Colors[colorIndex];
            if(currentScreenMode == SCREEN_0)
                SetAllCharsColorsForScreen0();
        }
        
        private void ThrowParseColorsException(Exception exception)
        {
            throw new EmulationEnvironmentCreationException(
 @"I couldn't parse the colors palette file. Make sure that:

- It is a text file containing extactly 16 text lines.
- Each line contains 3 color components, separated by spaces.
- Each color component is an integer number in the range 0-255."
            ,exception);
        }

        public void SetNumberOfRows(int numberOfRows)
        {
            this.numberOfRows = numberOfRows;
            display.SetNumberOfRows(numberOfRows);
        }

        public void SetPalette(byte colorIndex, Color value)
        {
            Func<int, int> convertComponent = c => (int)(c * 36.5);

            var color = Color.FromArgb(convertComponent(value.R), convertComponent(value.G), convertComponent(value.B));
            Colors[colorIndex] = color;

            if(currentScreenMode == SCREEN_0 && colorIndex == textColorIndex)
                SetTextColor((byte)textColorIndex);
            else if(currentScreenMode == SCREEN_0 && colorIndex == backdropColorIndex)
                SetBackdropColor((byte)backdropColorIndex);
            else if (currentScreenMode == SCREEN_1)
            {
                var affectedChars = CharacterColorsForScreen1.Keys.Where(k =>
                    CharacterColorsForScreen1[k].Item1 == colorIndex || CharacterColorsForScreen1[k].Item2 == colorIndex);

                foreach(var charIndex in affectedChars)
                    display.SetCharacterColors(charIndex, 
                        Colors[CharacterColorsForScreen1[charIndex].Item1],
                        Colors[CharacterColorsForScreen1[charIndex].Item2]);
            }

            if (currentScreenMode == SCREEN_0 && screenWidthInCharacters == 80 && (
                colorIndex == blinkTextColorIndex || colorIndex == blinkBackdropColorIndex))
            {
                display.SetBlinkColors(Colors[blinkTextColorIndex], Colors[blinkBackdropColorIndex]);
            }
        }

        public void SetBlinkTextColor(byte colorIndex)
        {
            this.blinkTextColorIndex = colorIndex;
            RestartBlink();
        }

        public void SetBlinkBackdropColor(byte colorIndex)
        {
            this.blinkBackdropColorIndex = colorIndex;
            RestartBlink();
        }

        private void RestartBlink()
        {
            display.SetBlinkColors(Colors[blinkTextColorIndex], Colors[blinkBackdropColorIndex]);
            SetBlinkTimes(blinkOnTimeInMs, blinkOffTimeInMs);
        }

        private bool blinkTimesAvailable;
        private decimal blinkOnTimeInMs, blinkOffTimeInMs;
        public void SetBlinkTimes(decimal onTimeInMs, decimal offTimeInMs)
        {
            blinkTimesAvailable = onTimeInMs != 0;
            blinkOnTimeInMs = onTimeInMs;
            blinkOffTimeInMs = offTimeInMs;

            if(!blinkTimesAvailable) {
                DisableBlink();
            }
            else if(screenWidthInCharacters == 80)
                EnableBlink();
        }

        private void EnableBlink()
        {
            DisableBlink();

            if(blinkOffTimeInMs == 0) {
                display.EnableBlink();
                return;
            }

            blinkTaskCancellationTokenSource = new CancellationTokenSource();
            blinkTask = Task.Factory.StartNew(
                blinkTaskProcess,
                new decimal[] {blinkOnTimeInMs, blinkOffTimeInMs}, 
                blinkTaskCancellationTokenSource.Token);
        }

        private void DisableBlink()
        {
            display.DisableBlink();
            KillBlinkTask();           
        }

        private void KillBlinkTask()
        {
            if (blinkTask != null) {
                blinkTaskCancellationTokenSource.Cancel();
                blinkTask.ContinueWith(t => t.Dispose());
                blinkTask = null;
            }
        }

        private CancellationTokenSource blinkTaskCancellationTokenSource;
        private Task blinkTask;

        private void blinkTaskProcess(object timesInMs)
        {
            var times = (decimal[])timesInMs;
            var onTime = times[0];
            var offTime = times[1];
            var currentlyOn = false;
            var currentTime = offTime;

            var sw = new Stopwatch();
            sw.Start();

            while(!blinkTaskCancellationTokenSource.IsCancellationRequested) {
                if(sw.ElapsedMilliseconds < currentTime)
                    continue;

                if(currentlyOn) {
                    display.DisableBlink();
                    currentlyOn = false;
                    currentTime = offTime;
                }
                else {
                    display.EnableBlink();
                    currentlyOn = true;
                    currentTime = onTime;
                }

                sw.Restart();
            }
        }
    }
}
