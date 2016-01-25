using System;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes
{
    /// <summary>
    /// Represents a banked memory, that is, a memory whose content is larger than 64K
    /// and has the addressing space divided in banks whose values can be changed
    /// to selectively access memory contents.
    /// </summary>
    /// <remarks>
    /// The memory addresses before AddressOfFirstBank and after
    /// (AddressOfFirstBank + NumberOfBanks*BankSize) are assumed to be unusable.
    /// </remarks>
    public interface IBankedMemory : IMemory
    {
        /// <summary>
        /// Memory address of the first bank.
        /// </summary>
        int AddressOfFirstBank { get; }

        /// <summary>
        /// Number of available banks.
        /// </summary>
        int NumberOfBanks { get; }

        /// <summary>
        /// Size of one bank in bytes.
        /// </summary>
        int BankSize { get; }

        /// <summary>
        /// Sets the value of a bank, so that the offset of the memory contents
        /// visible at this bank becomes value*BankSize.
        /// </summary>
        /// <param name="bankNumber"></param>
        /// <param name="value"></param>
        void SetBankValue(int bankNumber, byte value);

        /// <summary>
        /// Gets the current value of the bank (from which the offset of the memory contents
        /// visible at the bank can be calculated)
        /// </summary>
        /// <param name="bankNumber"></param>
        /// <returns></returns>
        int GetBlockInBank(int bankNumber);

        /// <summary>
        /// Event fired when the value of a certain bank (and thus the visible contents at that bank) changes.
        /// </summary>
        event EventHandler<BankValueChangedEventArgs> BankValueChanged;
    }
}
