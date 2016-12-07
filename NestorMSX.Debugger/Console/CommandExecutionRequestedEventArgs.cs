using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    public class CommandExecutionRequestedEventArgs : EventArgs
    {
        public string Command { get; }
        public object Result { get; set; }

        public CommandExecutionRequestedEventArgs(string command)
        {
            Command = command;
        }
    }
}
