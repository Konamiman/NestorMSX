using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    public interface IConsoleWindow
    {
        string Title { get; set; }

        event EventHandler<CommandExecutionRequestedEventArgs> CommandExecutionRequested;
    }
}
