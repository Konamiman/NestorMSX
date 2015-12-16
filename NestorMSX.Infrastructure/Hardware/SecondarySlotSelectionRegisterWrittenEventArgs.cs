using System;
using Konamiman.NestorMSX.Misc;

namespace Konamiman.NestorMSX.Hardware
{
    public class SecondarySlotSelectionRegisterWrittenEventArgs : EventArgs
    {
        public byte Value { get; private set; }

        public TwinBit PrimarySlotNumber { get; set; }

        public SecondarySlotSelectionRegisterWrittenEventArgs(byte value, TwinBit primarySlotNumber)
        {
            this.Value = value;
            this.PrimarySlotNumber = primarySlotNumber;
        }
    }
}
