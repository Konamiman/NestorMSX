using System;
using System.Linq;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes
{
    public class MsxDos2Rom : IMemory
    {
        private const int bankSize = 16*1024;

        private readonly byte[] contents;
        private readonly int maxBankNumber;

        private int contentsOffset;
        
        public MsxDos2Rom(byte[] contents)
        {
            var remainingSizeToFullBank = (bankSize - (contents.Length & 0x3FFF)) & 0x3FFF;
            if(remainingSizeToFullBank > 0)
                contents = contents.Concat(Enumerable.Repeat((byte)0xFF, remainingSizeToFullBank)).ToArray();

            this.contents = contents;
            this.maxBankNumber = (contents.Length/bankSize) - 1;

            contentsOffset = 0;
        }

        public int CurrentBlockSelected
        {
            get { return contentsOffset >> 14; }
        }

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

                this.contentsOffset = value*bankSize;
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
