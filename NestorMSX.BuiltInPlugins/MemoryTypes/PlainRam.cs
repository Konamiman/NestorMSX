using System;
using System.Linq;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Hardware
{
    /// <summary>
    /// Represents a 64K ROM memory.
    /// </summary>
    public class PlainRam : IMemory
    {
        private readonly byte[] memory;
        private readonly int baseAddress;
        private readonly int maxAddress;

        public PlainRam()
        {
            memory = Enumerable.Repeat<byte>(0xFF, Size).ToArray();
        }

        public PlainRam(int baseAddress, int size) : this()
        {
            var maxSize = 65536 - baseAddress;
            if(size > maxSize)
                throw new InvalidOperationException(
                    $"The memory size is too big. For the start address 0x{baseAddress:X} the maximum size is {maxSize} bytes.");

            this.baseAddress = baseAddress;
            maxAddress = baseAddress + size - 1;
            memory = new byte[size];
        }

        public int Size
        {
            get { return ushort.MaxValue - 1; }
        }

        public byte this[int address]
        {
            get
            {
                if(address < baseAddress || address > maxAddress)
                    return 0xFF;
                else
                    return memory[address - baseAddress];
            }
            set
            {
                if(address >= baseAddress && address <= maxAddress)
                    memory[address - baseAddress] = value;
            }
        }

        public void SetContents(int startAddress, byte[] contents, int startIndex = 0, int? length = null)
        {
            Array.Copy(contents, 0, memory, baseAddress + startIndex, length.GetValueOrDefault(contents.Length));
        }

        public byte[] GetContents(int startAddress, int length)
        {
            return memory.Skip(baseAddress + startAddress).Take(length).ToArray();
        }
    }
}
