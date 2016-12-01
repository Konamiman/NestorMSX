using System;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    public class CommandExecutionException : Exception
    {
        public CommandExecutionException(string message) : base(message)
        {
        }

        public CommandExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
