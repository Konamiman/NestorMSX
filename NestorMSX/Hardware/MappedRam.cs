using System;
using System.Collections.Generic;
using System.Linq;
using Konamiman.NestorMSX.Misc;

namespace Konamiman.NestorMSX.Hardware
{
    public class MappedRam : IMappedRam
    {
        private const int segmentSize = 16*1024;
        private static int[] allowedSizes = new[] {4, 8, 16, 32, 64, 128, 256};

        private readonly byte[] memory;
        private readonly int sizeInSegments;
        private readonly byte segmentMask;

        private readonly Dictionary<ushort, int> addressOffsetsPerEachPage;
        
        public MappedRam(int sizeInSegments)
        {
            if(!allowedSizes.Contains(sizeInSegments))
                throw new ArgumentException("Size in segments must be a power of two between 4 and 256", nameof(sizeInSegments));

            this.sizeInSegments = sizeInSegments;
            memory = new byte[sizeInSegments * segmentSize];
            segmentMask = (byte)(sizeInSegments - 1);

            addressOffsetsPerEachPage = new Dictionary<ushort, int> {
                { 0x0000, 0 },
                { 0x4000, 0 },
                { 0x8000, 0 },
                { 0xC000, 0 }
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
            addressOffsetsPerEachPage[page.AddressMask] = segmentNumber * segmentSize;
        }
    }
}
