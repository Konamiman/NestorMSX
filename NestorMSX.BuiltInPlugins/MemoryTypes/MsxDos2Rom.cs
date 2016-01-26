using System;
using System.Linq;

namespace Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes
{
    public class MsxDos2Rom : IBankedMemory
    {
        private const int bankSize = 16*1024;

        private readonly byte[] contents;
        private readonly int maxBankNumber;

        private int contentsOffset;
        private int currentBlock;

        public event EventHandler<BankValueChangedEventArgs> BankValueChanged;

        public MsxDos2Rom(byte[] contents)
        {
            var remainingSizeToFullBank = (bankSize - (contents.Length & 0x3FFF)) & 0x3FFF;
            if(remainingSizeToFullBank > 0)
                contents = contents.Concat(Enumerable.Repeat((byte)0xFF, remainingSizeToFullBank)).ToArray();

            this.contents = contents;
            this.maxBankNumber = (contents.Length/bankSize) - 1;

            contentsOffset = 0;
            currentBlock = 0;
        }

        public int CurrentBlockSelected => currentBlock;

        public byte this[int address]
        {
            get
            {
                if(address < 0x4000 || address > 0xBFFF)
                    return 0xFF;

                return contents[(address & 0x3FFF) + contentsOffset];
            }

            set
            {
                if(value > maxBankNumber)
                    return;

                address = address & 0xE000;
                if(address != 0x6000)
                    return;

                SetBankValueCore(value);
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

        public int NumberOfBanks => 1;

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
            SetBankValueCore(value);
        }

        private void SetBankValueCore(byte value)
        {
            if(value == currentBlock)
                return;

            contentsOffset = value*bankSize;
            currentBlock = value;

            BankValueChanged?.Invoke(this, new BankValueChangedEventArgs(0, value));
        }

        public int GetBlockInBank(int bankNumber)
        {
            CheckBankNumber(bankNumber);
            return currentBlock;
        }

        private static void CheckBankNumber(int bankNumber)
        {
            if (bankNumber != 0)
                throw new InvalidOperationException("Bank number must be 0");
        }
    }
}
