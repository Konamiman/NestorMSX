using System;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    public interface IConsoleWindow
    {
        string Title { get; set; }

        event EventHandler<CommandExecutionRequestedEventArgs> CommandExecutionRequested;

        void Clear();

        Func<object, string> ResultsFormatter { get; set; }

        void Print(string text);
    }
}
