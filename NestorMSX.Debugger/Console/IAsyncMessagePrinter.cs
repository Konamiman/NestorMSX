using System;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    public interface IAsyncMessagePrinter
    {
        event EventHandler<PrintMessageRequestEventArgs> PrintMessageRequest;
    }

    public class PrintMessageRequestEventArgs : EventArgs
    {
        public object Message { get; }

        public PrintMessageRequestEventArgs(object message)
        {
            Message = message;
        }
    }
}
