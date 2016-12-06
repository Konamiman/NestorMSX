using System;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter
{
    public abstract class Variable : TokenWithName
    {
        public bool CanRead { get; protected set; }
        public bool CanWrite { get; protected set; }

        protected Variable(string fullName) : base(fullName)
        {
        }

        public object GetValue()
        {
            if(!CanRead)
                throw new CommandExecutionException($"Can't get the value of '{FullName}' - it's write-only");

            try {
                return GetValueImpl();
            }
            catch (Exception ex) {
                throw new CommandExecutionException($"Error getting the value of '{FullName}': {ex.Message}", ex);
            }
        }

        public abstract object GetValueImpl();

        public void SetValue(object value)
        {
            if(!CanWrite)
                throw new CommandExecutionException($"Can't set the value of '{FullName}' - it's read-only");

            try {
                SetValueImpl(value);
            }
            catch (Exception ex) {
                throw new CommandExecutionException($"Error setting the value of '{FullName}': {ex.Message}", ex);
            }
        }

        public abstract void SetValueImpl(object value);
    }
}
