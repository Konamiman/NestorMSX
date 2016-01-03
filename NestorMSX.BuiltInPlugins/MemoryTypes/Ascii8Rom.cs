using System;
using System.Collections.Generic;
using System.Linq;
using Konamiman.Z80dotNet;
using System.Diagnostics;

namespace Konamiman.NestorMSX.Hardware
{
    public class Ascii8Rom : IMemory
    {
        private static readonly IDictionary<int, int> bankStartAddressBySelectionAddress =
            new Dictionary<int, int>
        {
            {0x6000, 0x4000},
            {0x6800, 0x6000},
            {0x7000, 0x8000},
            {0x7800, 0xA000}
        };

        private const int bankSize = 8*1024;

        private readonly byte[] contents;
        private readonly int maxBankNumber;

        private readonly Dictionary<int, int> contentsOffsetsPerEachBank;
        
        public Ascii8Rom(byte[] contents)
        {
            var remainingSizeToFullBank = (bankSize - (contents.Length & 0x1FFF)) & 0x1FFF;
            if(remainingSizeToFullBank > 0)
                contents = contents.Concat(Enumerable.Repeat((byte)0xFF, remainingSizeToFullBank)).ToArray();

            this.contents = contents;
            this.maxBankNumber = (contents.Length/bankSize) - 1;

            contentsOffsetsPerEachBank = new Dictionary<int, int> {
                { 0x4000, 0x0000 },
                { 0x6000, 0x2000 },
                { 0x8000, 0x4000 },
                { 0xA000, 0x6000 }
            };
        }

        private static int[] bankStartAddresses = { 0, 0x4000, 0x6000, 0x8000, 0xA000 };

        public int CurrentBlockInBank(int bankNumber)
        {
            return contentsOffsetsPerEachBank[bankStartAddresses[bankNumber]] >> 13;
        }

        public byte this[int address]
        {
            get
            {
                if(address < 0x4000 || address > 0xBFFF)
                    return 0xFF;

                return contents[(address & 0x1FFF) + contentsOffsetsPerEachBank[(ushort)(address & 0xE000)]];
            }

            set
            {
                if (value > maxBankNumber)
                    return;

                address = address & 0xF800;
                if (address < 0x6000 || address > 0x7800)
                    return;

                var bankStartAddress = bankStartAddressBySelectionAddress[address];
                var contentsOffset = value*bankSize;

                contentsOffsetsPerEachBank[bankStartAddress] = contentsOffset;
            }
        }

        public int Size
        {
            get
            {
                return ushort.MaxValue + 1;
            }
        }

        public byte[] GetContents(int startAddress, int length)
        {
            return contents.Skip(startAddress).Take(length).ToArray();
        }

        public void SetContents(int startAddress, byte[] contents, int startIndex = 0, int? length = default(int?))
        {
            Array.Copy(contents, 0, contents, startIndex, length.GetValueOrDefault(contents.Length));
        }
    }
}
