using System;
using System.Linq;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes
{
    /// <summary>
    /// Represents a 64K ROM memory.
    /// </summary>
    public class PlainRom : IMemory
    {
        private readonly byte[] memory;

        public PlainRom()
        {
            memory = Enumerable.Repeat<byte>(0xFF, Size).ToArray();
        }

        public PlainRom(byte[] contents, Z80Page? page = null) : this()
        {
            if(contents.Length == 0)
                throw new Exception(
                    "The ROM file is empty.");

            var startAddress = page == null ? 0 : page.Value.AddressMask;

            var maxSize = 65536 - startAddress;
            if(contents.Length > maxSize)
                throw new InvalidOperationException(
                    $"The ROM file is too big. The maximum size is {maxSize} bytes.");

            Array.Copy(contents, 0, memory, startAddress, contents.Length);
        }

        public int Size
        {
            get
            {
                return ushort.MaxValue + 1;
            }
        }

        public byte this[int address]
        {
            get { return memory[address]; }
            set { }
        }

        public void SetContents(int startAddress, byte[] contents, int startIndex = 0, int? length = null)
        {
        }

        public byte[] GetContents(int startAddress, int length)
        {
            return memory.Skip(startAddress).Take(length).ToArray();
        }
    }
}
