using System;

namespace Konamiman.NestorMSX.Hardware
{
    public class VdpRegisterWrittenEventArgs : EventArgs
    {
        public VdpRegisterWrittenEventArgs(byte registerNumber, byte value)
        {
            this.RegisterNumber = registerNumber;
            this.Value = value;
        }

        public byte RegisterNumber { get; }

        public byte Value { get; }
    }
}
