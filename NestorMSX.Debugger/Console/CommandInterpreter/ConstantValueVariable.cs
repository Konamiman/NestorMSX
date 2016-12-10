using System;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter
{
    public class ConstantValueVariable : Variable
    {
        private readonly object value;

        public ConstantValueVariable(string fullName, object value) : base(fullName)
        {
            this.value = value;
            CanRead = true;
            CanWrite = false;
        }

        public override object GetValueImpl()
        {
            return value;
        }

        public override void SetValueImpl(object value)
        {
            throw new NotImplementedException();
        }
    }
}
