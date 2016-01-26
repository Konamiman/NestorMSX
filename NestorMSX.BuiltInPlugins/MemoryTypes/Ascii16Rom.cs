using System;
using System.Collections.Generic;
using System.Linq;

namespace Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes
{
    public class Ascii16Rom : IBankedMemory
    {
        private static readonly IDictionary<int, int> bankStartAddressBySelectionAddress =
            new Dictionary<int, int>
        {
            {0x6000, 0x4000},
            {0x7000, 0x8000},
        };

        private static int[] bankStartAddresses = { 0x4000, 0x8000 };
        private byte[] blockNumbersInEachBank = new byte[] { 0, 1 };

        private const int bankSize = 16*1024;

        private readonly byte[] contents;
        private readonly int maxBlockNumber;

        private readonly Dictionary<int, int> contentsOffsetsPerEachBank;
        
        public Ascii16Rom(byte[] contents)
        {
            var remainingSizeToFullBank = (bankSize - (contents.Length & 0x3FFF)) & 0x3FFF;
            if(remainingSizeToFullBank > 0)
                contents = contents.Concat(Enumerable.Repeat((byte)0xFF, remainingSizeToFullBank)).ToArray();

            this.contents = contents;
            this.maxBlockNumber = (contents.Length/bankSize) - 1;

            contentsOffsetsPerEachBank = new Dictionary<int, int> {
                { 0x4000, 0x0000 },
                { 0x8000, 0x4000 }
            };
        }

        public event EventHandler<BankValueChangedEventArgs> BankValueChanged;

        public int GetBlockInBank(int bankNumber)
        {
            CheckBankNumber(bankNumber);
            return blockNumbersInEachBank[bankNumber];
        }

        public byte this[int address]
        {
            get
            {
                if(address < 0x4000 || address > 0xBFFF)
                    return 0xFF;

                return contents[(address & 0x3FFF) + contentsOffsetsPerEachBank[(ushort)(address & 0xC000)]];
            }

            set
            {
                if(value > maxBlockNumber)
                    return;

                address = address & 0xF000;
                if(address < 0x6000 || address > 0x7000)
                    return;

                var bankStartAddress = bankStartAddressBySelectionAddress[address];
                var contentsOffset = value*bankSize;

                var bankNumber = (bankStartAddress-AddressOfFirstBank) >> 13;
                SetBankValueCore(bankNumber, value);
            }
        }

        public int Size
        {
            get
            {
                return ushort.MaxValue + 1;
            }
        }

        public int AddressOfFirstBank => 0x4000;

        public int NumberOfBanks => 2;

        public int BankSize => bankSize;

        public byte[] GetContents(int startAddress, int length)
        {
            return contents.Skip(startAddress).Take(length).ToArray();
        }

        public void SetContents(int startAddress, byte[] contents, int startIndex = 0, int? length = default(int?))
        {
            Array.Copy(contents, 0, contents, startIndex, length.GetValueOrDefault(contents.Length));
        }

        public void SetBankValue(int bankNumber, byte value)
        {
            CheckBankNumber(bankNumber);

            SetBankValueCore(bankNumber, value);
        }

        private void SetBankValueCore(int bankNumber, byte value)
        {
            var oldValue = blockNumbersInEachBank[bankNumber];
            if(oldValue == value)
                return;

            var bankStartAddress = bankStartAddresses[bankNumber];
            var contentsOffset = value * bankSize;
            
            contentsOffsetsPerEachBank[bankStartAddress] = contentsOffset;
            blockNumbersInEachBank[bankNumber] = value;
            BankValueChanged?.Invoke(this, new BankValueChangedEventArgs(bankNumber, value));
        }

        private static void CheckBankNumber(int bankNumber)
        {
            if(bankNumber < 0 || bankNumber > 1)
                throw new InvalidOperationException("Bank number must be a value between 0 and 1");
        }
    }
}
