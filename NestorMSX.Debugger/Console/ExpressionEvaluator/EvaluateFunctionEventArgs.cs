using System;

namespace Konamiman.NestorMSX.Z80Debugger.Console.ExpressionEvaluator
{
    public class EvaluateFunctionEventArgs : EventArgs
    {
        public string Name { get; set; }
        public object[] Parameters { get; set; }
        public object Result { get; set;}
    }
}