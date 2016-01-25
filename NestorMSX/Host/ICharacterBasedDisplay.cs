using System.Collections.Generic;
using System.Drawing;

namespace Konamiman.NestorMSX.Host
{
    /// <summary>
    /// Represents a text mode device that can display character patterns in a character matrix.
    /// </summary>
    public interface ICharacterBasedDisplay
    {
        /// <summary>
        /// Notifies the value of the screen buffer that will be used.
        /// This method is executed only once, when no data is yet present in the buffer.
        /// </summary>
        /// <param name="value"></param>
        void SetScreenBufer(IDictionary<Point, byte> value);

        /// <summary>
        /// Notifies that the backdrop color has changed.
        /// </summary>
        /// <param name="color">New backdrop color</param>
        void SetBackdropColor(Color color);

        /// <summary>
        /// Notifies that the pattern for a certain character has changed.
        /// </summary>
        /// <param name="charIndex">Index of the character that has changed</param>
        /// <param name="pattern">A 8 byte array with the new character pattern</param>
        void SetCharacterPattern(byte charIndex, byte[] pattern);

        /// <summary>
        /// Notifies that the colors for a certain character have changed.
        /// </summary>
        /// <param name="charIndex">Index of the character that has changed</param>
        /// <param name="foreground">New foreground color of the character</param>
        /// <param name="background">New background color of the character</param>
        void SetCharacterColors(byte charIndex, Color foreground, Color background);

        /// <summary>
        /// Notifies that the colors for the blink in TEXT1 mode have changed.
        /// </summary>
        /// <param name="foreground">New blink foreground color</param>
        /// <param name="background">New blink background color</param>
        void SetBlinkColors(Color foreground, Color background);

        /// <summary>
        /// Enables the blink mode in TEXT1
        /// </summary>
        void EnableBlink();

        /// <summary>
        /// Disables the blink mode in TEXT1
        /// </summary>
        void DisableBlink();

        /// <summary>
        /// Sets or resets blink for a certain character position in TEXT1
        /// </summary>
        /// <param name="position">Position to change</param>
        /// <param name="isBlink">True to set the character in that position as blink, false to set it as normal</param>
        void SetBlink(Point position, bool isBlink);

        /// <summary>
        /// Notifies that the character width has changed
        /// </summary>
        /// <param name="width">New character width in pixels, 6 or 8</param>
        void SetCharacterWidth(int width);

        /// <summary>
        /// Sets the number of character columns
        /// </summary>
        /// <param name="columns">Number of character columns: 32, 40 or 80</param>
        void SetColumns(int columns);

        /// <summary>
        /// Sets the number of character rows
        /// </summary>
        /// <param name="numberOfRows">Number of character rows, 24 or 27</param>
        void SetNumberOfRows(int numberOfRows);

        /// <summary>
        /// Notifies that a new value has been added to the screen buffer,
        /// or that the value corresponding to a certain set of coordinates
        /// has changed.
        /// </summary>
        /// <param name="coordinates">Coordinates of the character thas has changed
        /// (key in the screen buffer dictionary)</param>
        void NotifyScreenBufferContentsAddedOrChanged(Point coordinates);

        /// <summary>
        /// Notifies that a value has been removed from the screen buffer
        /// </summary>
        /// <param name="coordinates">Coordinates of the value that has been removed
        /// (key in the screen buffer dictionary)
        /// </param>
        void NotifyScreenBufferContentsRemoved(Point coordinates);

        /// <summary>
        /// Notifies that the screen should be disabled, so that
        /// only the backdrop color should be displayed.
        /// </summary>
        void BlankScreen();

        /// <summary>
        /// Notifies that the screen should be enabled, so that
        /// it displays its normal contents.
        /// </summary>
        void ActivateScreen();
    }
}
