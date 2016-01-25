using System;

namespace Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes
{
    public class BankValueChangedEventArgs : EventArgs
    {
        public int BankNumber { get; }
        public byte Value { get; }

        public BankValueChangedEventArgs(int bankNumber, byte value)
        {
            BankNumber = bankNumber;
            Value = value;
        }
    }
}