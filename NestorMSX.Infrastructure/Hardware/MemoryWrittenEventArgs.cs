using System;

namespace Konamiman.NestorMSX.Hardware
{
    public class MemoryWrittenEventArgs : EventArgs
    {
        public MemoryWrittenEventArgs(int address, byte value)
        {
            this.Address = address;
            this.Value = value;
        }

        public int Address { get; }

        public byte Value { get; }
    }
}
