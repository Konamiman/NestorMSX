using System.Drawing;

namespace Konamiman.NestorMSX.Hardware
{
    /// <summary>
    /// Represents a device that can be connected to a V9938 processor
    /// and render the video data that it produces.
    /// </summary>
    public interface IV9938DisplayRenderer
    {
        /// <summary>
        /// Enables the screen, displaying again all the data in the pattern name table.
        /// </summary>
        void ActivateScreen();

        /// <summary>
        /// Disables the screen, showing just the backdrop color.
        /// </summary>
        void BlankScreen();

        /// <summary>
        /// Notifies of a change of the screen mode.
        /// </summary>
        /// <param name="mode">New screen mode, a number from 0 to 3.</param>
        /// <param name="columns">Screen width in columns</param>
        void SetScreenMode(byte mode, byte columns);

        /// <summary>
        /// Notifies a change of the number of rows for the TEXT1 mode.
        /// </summary>
        /// <param name="numberOfRows">New number of rows, either 24 or 27.</param>
        void SetNumberOfRows(int numberOfRows);

        /// <summary>
        /// Notifies a change of palette for a given color index.
        /// </summary>
        /// <param name="colorIndex">Color index to change</param>
        /// <param name="value">New color for the index</param>
        void SetPalette(byte colorIndex, Color value);

        /// <summary>
        /// Notifies of a byte written in the pattern name table.
        /// </summary>
        /// <param name="position">Offset in the table.</param>
        /// <param name="value">Value written.</param>
        void WriteToNameTable(int position, byte value);

        /// <summary>
        /// Notifies of a byte written in the pattern generator table.
        /// </summary>
        /// <param name="position">Offset in the table.</param>
        /// <param name="value">Value to write.</param>
        void WriteToPatternGeneratorTable(int position, byte value);

        /// <summary>
        /// Notifies of a byte written in the color generator table.
        /// </summary>
        /// <param name="position">Offset in the table.</param>
        /// <param name="value">Value to write.</param>
        void WriteToColourTable(int position, byte value);

        /// <summary>
        /// Notifies of a change of the text color.
        /// </summary>
        /// <param name="colorIndex">Color index of the new text color.</param>
        void SetTextColor(byte colorIndex);
        
        /// <summary>
        /// Notifies of a change of the backdrop color.
        /// </summary>
        /// <param name="colorIndex">Color index of the new backdrop color.</param>
        void SetBackdropColor(byte colorIndex);

        /// <summary>
        /// Notifies a change of the blink text color.
        /// </summary>
        /// <param name="colorIndex">Color index of the new blink text color.</param>
        void SetBlinkTextColor(byte colorIndex);

        /// <summary>
        /// Notifies a change of the blink backdrop color.
        /// </summary>
        /// <param name="colorIndex">Color index of the new blink backdrop color.</param>
        void SetBlinkBackdropColor(byte colorIndex);

        /// <summary>
        /// Notifies a change of the blink times
        /// </summary>
        /// <param name="onTimeInMs">Blink ON time in milliseconds</param>
        /// <param name="offTimeInMs">Blink OFF time in milliseconds</param>
        void SetBlinkTimes(decimal onTimeInMs, decimal offTimeInMs);
    }
}
