using System;

namespace Konamiman.NestorMSX.Hardware
{
    /// <summary>
    /// Represents a V9938 video display processor that can be controlled externally,
    /// that is, by explicit screen mode selection and VRAM access methods.
    /// </summary>
    public interface IExternallyControlledV9938 : IV9938
    {
        /// <summary>
        /// Gets the current screen mode.
        /// </summary>
        ScreenMode CurrentScreenMode { get; }

        /// <summary>
        /// Writes one byte to VRAM.
        /// </summary>
        /// <param name="address">VRAM address</param>
        /// <param name="value">Value to write</param>
        void WriteVram(int address, byte value);

        /// <summary>
        /// Reads one byte from VRAM.
        /// </summary>
        /// <param name="address">VRAM address</param>
        /// <returns>Value read from VRAM</returns>
        byte ReadVram(int address);

        /// <summary>
        /// Gets the contents of a portion of VRAM
        /// </summary>
        /// <param name="startAddress">First address</param>
        /// <param name="length">Amount of bytes to read</param>
        /// <returns></returns>
        byte[] GetVramContents(int startAddress, int length);

        /// <summary>
        /// Returns the current base address for the pattern name table.
        /// </summary>
        int PatternNameTableAddress { get; }

        /// <summary>
        /// Returns the current size of the pattern name table.
        /// </summary>
        int PatternNameTableSize { get; }

        /// <summary>
        /// Returns the current size of the pattern generator table.
        /// </summary>
        int PatternGeneratorTableAddress { get; }

        /// <summary>
        /// Returns the current size of the color table.
        /// </summary>
        int ColorTableAddress { get; }

        /// <summary>
        /// Sets the contents of a portion of VRAM
        /// </summary>
        /// <param name="startAddress">First VRAM address that will be set</param>
        /// <param name="contents">New contents of VRAM</param>
        /// <param name="startIndex">StartGeneratingKeyEvents index for starting copying within the contens array</param>
        /// <param name="length">Length of the contents array that will be copied. If null,
        /// the whole array is copied.</param>
        void SetVramContents(int startAddress, byte[] contents, int startIndex = 0, int? length = null);

        /// <summary>
        /// Event fired when a register is written to, either directly or indirectly.
        /// </summary>
        event EventHandler<VdpRegisterWrittenEventArgs> ControlRegisterWritten;

        /// <summary>
        /// Event fired when the screen mode changes.
        /// </summary>
        event EventHandler ScreenModeChanged;

        /// <summary>
        /// Event fired when the VRAM is written to.
        /// </summary>
        event EventHandler<MemoryWrittenEventArgs> VramWritten;
    }
}
