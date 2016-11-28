using System;

namespace Konamiman.NestorMSX.Z80Debugger.Console.ExpressionEvaluator
{
    public class EvaluateNameEventArgs : EventArgs
    {
        public string Name { get; set; }
        public object Result { get; set; }
    }
}