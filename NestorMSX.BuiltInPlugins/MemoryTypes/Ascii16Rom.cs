using System;
using System.Collections.Generic;
using System.Linq;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes
{
    public class Ascii16Rom : IMemory
    {
        private static readonly IDictionary<int, int> bankStartAddressBySelectionAddress =
            new Dictionary<int, int>
        {
            {0x6000, 0x4000},
            {0x7000, 0x8000}
        };

        private const int bankSize = 16*1024;

        private readonly byte[] contents;
        private readonly int maxBankNumber;

        private readonly Dictionary<int, int> contentsOffsetsPerEachBank;
        
        public Ascii16Rom(byte[] contents)
        {
            var remainingSizeToFullBank = (bankSize - (contents.Length & 0x3FFF)) & 0x3FFF;
            if(remainingSizeToFullBank > 0)
                contents = contents.Concat(Enumerable.Repeat((byte)0xFF, remainingSizeToFullBank)).ToArray();

            this.contents = contents;
            this.maxBankNumber = (contents.Length/bankSize) - 1;

            contentsOffsetsPerEachBank = new Dictionary<int, int> {
                { 0x4000, 0x0000 },
                { 0x8000, 0x4000 }
            };
        }

        private static int[] bankStartAddresses = { 0, 0x4000, 0x8000 };

        public int CurrentBlockInBank(int bankNumber)
        {
            return contentsOffsetsPerEachBank[bankStartAddresses[bankNumber]] >> 14;
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
                if (value > maxBankNumber)
                    return;

                address = address & 0xF000;
                if (address < 0x6000 || address > 0x7000)
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
