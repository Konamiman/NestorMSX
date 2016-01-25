using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Konamiman.NestorMSX.Exceptions;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Host
{
    /// <summary>
    /// An implementation of ICharacterBasedDisplay that writes on a Graphics object.
    /// </summary>
    public class GraphicsBasedDisplay : ICharacterBasedDisplay
    {
        private readonly object syncObject = new object();
        private readonly IDrawingSurface drawingSurface;
        private IDictionary<Point, byte> screenBuffer;
        private Graphics defaultGraphics;
        private Color BackdropColor;
        private int characterWidth = 8;
        private bool screenIsActive = false;
        private IDictionary<byte, byte[]> characterPatterns = new Dictionary<byte, byte[]>();
        private IDictionary<byte, Tuple<Color, Color>> characterColors = new Dictionary<byte, Tuple<Color, Color>>();
        private IDictionary<byte, Tuple<Brush, Brush>> characterBrushes = new Dictionary<byte, Tuple<Brush, Brush>>();
        private readonly Configuration config;
        private Tuple<Brush, Brush> blinkBrushes = new Tuple<Brush, Brush>(new SolidBrush(Color.Black), new SolidBrush(Color.Black));

        public GraphicsBasedDisplay(IDrawingSurface drawingSurface, Configuration config)
        {
            ValidateConfiguration(config);

            this.config = config;
            this.drawingSurface = drawingSurface;

            BackdropColor = Color.Blue;

            for(int i = 0; i < 256; i++) {
                characterColors[(byte)i] = new Tuple<Color, Color>(Color.Black, Color.Black);
                characterBrushes[(byte)i] = new Tuple<Brush, Brush>(new SolidBrush(Color.Black), new SolidBrush(Color.Black));
            }

            defaultGraphics = drawingSurface.GetGraphics();
            defaultGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            Transform(defaultGraphics);

            drawingSurface.RequiresPaint += DrawingSurfaceOnRequiresPaint;
        }

        private void ValidateConfiguration(Configuration configuration)
        {
            if(configuration.DisplayZoomLevel < 0.1M || configuration.DisplayZoomLevel > 1000)
                throw new ConfigurationException("Display zoom level must be a number between 0.1 and 1000.");
        }

        private void DrawingSurfaceOnRequiresPaint(object sender, PaintEventArgs eventArgs)
        {
            lock(syncObject) {
                Transform(eventArgs.Graphics);
                RepaintAll(eventArgs.Graphics);
            }
        }

        private int trX, trY;
        private float scX, scY;
        private Func<int, int> transfX, transfY;
        private void Transform(Graphics graphics)
        {
            var horizontalStretch = doubleColumns ? 2 : 1;
            var horizontalMarginMultiplier = doubleColumns ? 2 : 1;
            scX = (float)config.DisplayZoomLevel / horizontalStretch;
            scY = (float)config.DisplayZoomLevel;
            trX = config.HorizontalMarginInPixels * horizontalMarginMultiplier;
            trY = config.VerticalMarginInPixels;

            transfX = x => (int)((x + trX) * scX);
            transfY = y => (int)((y + trY) * scY);
        }

        private void RepaintAll(Graphics graphics)
        {
            ClearScreen(graphics);
            foreach(var item in screenBuffer)
                DrawCharacter(item.Key, item.Value, graphics);
        }

        private Dictionary<Point, BufferedGraphics> bufferedGraphicsWidth6 = new Dictionary<Point, BufferedGraphics>();
        private Dictionary<Point, BufferedGraphics> bufferedGraphicsWidth8 = new Dictionary<Point, BufferedGraphics>();
        private Dictionary<Point, BufferedGraphics> bufferedGraphics80columns = new Dictionary<Point, BufferedGraphics>();

        private void DrawCharacter(Point coordinates, byte charIndex, Graphics graphics)
        {
            if(!screenIsActive || coordinates.Y >= numberOfRows)
                return;

            lock(syncObject) {
                var baseX = (coordinates.X*characterWidth) + (characterWidth == 8 ? 0 : doubleColumns ? 16 : 8);
                var X = baseX;
                var Y = coordinates.Y*8;
                var point = new Point(X, Y);

                var bufferedGraphics = characterWidth == 8 ? bufferedGraphicsWidth8 :
                    doubleColumns ? bufferedGraphics80columns : bufferedGraphicsWidth6;
                
                if(!bufferedGraphics.ContainsKey(point))
                    bufferedGraphics[point] = BufferedGraphicsManager.Current.Allocate(graphics,
                        new Rectangle(transfX(X), transfY(Y), (int)(characterWidth*scX), (int)(8*scY)));
                var bg = bufferedGraphics[point];
                var g = bg.Graphics;

                var brushes = blinkEnabled && blinkPositions.Contains(coordinates) ? blinkBrushes : characterBrushes[charIndex];
                var pattern = characterPatterns[charIndex];
                g.FillRectangle(brushes.Item2, transfX(baseX), transfY(Y), (int)(characterWidth * scX), (int)(8 * scY));
                for(int row = 0; row < 8; row++) {
                    for(int column = 7; column >= 8 - characterWidth; column--) {
                        if(pattern[row].GetBit(column)) {
                            g.FillRectangle(brushes.Item1,transfX(X + (7 - column)), transfY(Y), scX, scY);
                        }
                    }
                    X = baseX;
                    Y++;
                }

                bg.Render(graphics);
            }
        }

        public void SetScreenBufer(IDictionary<Point, byte> value)
        {
            screenBuffer = value;
        }

        public void SetBackdropColor(Color color)
        {
            BackdropColor = color;
            RepaintAll(defaultGraphics);
        }

        public void ClearScreen(Graphics graphics)
        {
            lock(syncObject)
                graphics.Clear(BackdropColor);
        }

        public void SetCharacterPattern(byte charIndex, byte[] pattern)
        {
            characterPatterns[charIndex] = pattern;
            ReprintAllInstancesOf(charIndex);
        }

        public void SetCharacterColors(byte charIndex, Color foreground, Color background)
        {
            characterColors[charIndex] = new Tuple<Color, Color>(foreground, background);

            var oldBrushes = characterBrushes[charIndex];
            oldBrushes.Item1.Dispose();
            oldBrushes.Item2.Dispose();

            var newBrushes = new Tuple<Brush, Brush>(
                new SolidBrush(foreground),
                new SolidBrush(background));
            characterBrushes[charIndex] = newBrushes;

            ReprintAllInstancesOf(charIndex);
        }

        private void ReprintAllInstancesOf(byte charIndex)
        {
            foreach(var item in screenBuffer.Where(i => i.Value == charIndex))
                DrawCharacter(item.Key, item.Value, defaultGraphics);
        }

        public void SetCharacterWidth(int width)
        {
            characterWidth = width;
        }

        private bool doubleColumns = false;
        private int oldColumns;
        public void SetColumns(int columns)
        {
            if(columns == oldColumns)
                return;

            oldColumns = columns;
            doubleColumns = (columns > 40);
            lock(syncObject) {
                defaultGraphics.ResetTransform();
                Transform(defaultGraphics);
                ClearScreen(defaultGraphics);
                RepaintAll(defaultGraphics);
            }
        }

        public void NotifyScreenBufferContentsAddedOrChanged(Point coordinates)
        {
            DrawCharacter(coordinates, screenBuffer[coordinates], defaultGraphics);
        }

        public void NotifyScreenBufferContentsRemoved(Point coordinates)
        {
        }

        public void BlankScreen()
        {
            ClearScreen(defaultGraphics);
            screenIsActive = false;
        }

        public void ActivateScreen()
        {
            screenIsActive = true;
            RepaintAll(defaultGraphics);
        }

        private int numberOfRows = 24;
        public void SetNumberOfRows(int numberOfRows)
        {
            this.numberOfRows = numberOfRows;
        }

        public void SetBlinkColors(Color foreground, Color background)
        {
            if(blinkBrushes != null) {
                blinkBrushes.Item1.Dispose();
                blinkBrushes.Item2.Dispose();
            }
            blinkBrushes = new Tuple<Brush, Brush>(new SolidBrush(foreground), new SolidBrush(background));
        }

        private void ReprintAllBlinks()
        {
            foreach(var position in blinkPositions)
                ReprintPosition(position);
        }

        private void ReprintPosition(Point position)
        {
            if(screenBuffer.ContainsKey(position))
                DrawCharacter(position, screenBuffer[position], defaultGraphics);
        }

        private bool blinkEnabled;
        public void EnableBlink()
        {
            blinkEnabled = true;
            ReprintAllBlinks();
        }

        public void DisableBlink()
        {
            blinkEnabled = false;
            ReprintAllBlinks();
        }

        private List<Point> blinkPositions = new List<Point>();

        public void SetBlink(Point position, bool isBlink)
        {
            if(isBlink && !blinkPositions.Contains(position))
            {
                blinkPositions.Add(position);
                ReprintPosition(position);
            }
            else if(!isBlink && blinkPositions.Contains(position))
            {
                blinkPositions.Remove(position);
                ReprintPosition(position);
            }
        }
    }
}
