using System;
using System.Collections.Generic;
using System.Linq;
using Konamiman.NestorMSX.Misc;

namespace Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes
{
    public class MappedRam : IMappedRam
    {
        private const int segmentSize = 16*1024;
        private static readonly int[] allowedSizes = {4, 8, 16, 32, 64, 128, 256};

        private readonly byte[] memory;
        private readonly byte segmentMask;

        private readonly IDictionary<ushort, int> addressOffsetsPerEachPage;
        private readonly byte[] segmentsInEachPage = new byte[] {3, 2, 1, 0};

        public event EventHandler<BankValueChangedEventArgs> BankValueChanged;

        public MappedRam(int sizeInSegments)
        {
            if(!allowedSizes.Contains(sizeInSegments))
                throw new ArgumentException("Size in segments must be a power of two between 4 and 256",
                    nameof(sizeInSegments));

            memory = new byte[sizeInSegments * segmentSize];
            segmentMask = (byte)(sizeInSegments - 1);

            addressOffsetsPerEachPage = new Dictionary<ushort, int> {
                { 0x0000, 0xC000 },
                { 0x4000, 0x8000 },
                { 0x8000, 0x4000 },
                { 0xC000, 0x0000 }
            };
        }

        public byte this[int address]
        {
            get
            {
                return memory[(address & 0x3FFF) + addressOffsetsPerEachPage[(ushort)(address & 0xC000)]];
            }

            set
            {
                memory[(address & 0x3FFF) + addressOffsetsPerEachPage[(ushort)(address & 0xC000)]] = value;
            }
        }

        public int Size
        {
            get
            {
                return ushort.MaxValue + 1;
            }
        }

        public int AddressOfFirstBank => 0;

        public int NumberOfBanks => 4;

        public int BankSize => 16384;

        public byte[] GetContents(int startAddress, int length)
        {
            return memory.Skip(startAddress).Take(length).ToArray();
        }

        public void SetContents(int startAddress, byte[] contents, int startIndex = 0, int? length = default(int?))
        {
            Array.Copy(contents, 0, memory, startIndex, length.GetValueOrDefault(contents.Length));
        }

        public void WriteToSegmentSelectionRegister(Z80Page page, byte segmentNumber)
        {
            segmentNumber &= segmentMask;
            if(segmentsInEachPage[page] == segmentNumber)
                return;

            addressOffsetsPerEachPage[page.AddressMask] = segmentNumber * segmentSize;
            segmentsInEachPage[page] = segmentNumber;
            BankValueChanged?.Invoke(this, new BankValueChangedEventArgs(page, segmentNumber));
        }

        public void SetBankValue(int bankNumber, byte value)
        {
            CheckBankNumber(bankNumber);
            WriteToSegmentSelectionRegister(bankNumber, value);
        }

        private static void CheckBankNumber(int bankNumber)
        {
            if(bankNumber < 0 || bankNumber > 3)
                throw new InvalidOperationException("Bank number must be a value between 0 and 3");
        }

        public int GetBlockInBank(int bankNumber)
        {
            CheckBankNumber(bankNumber);
            return segmentsInEachPage[bankNumber];
        }
    }
}
