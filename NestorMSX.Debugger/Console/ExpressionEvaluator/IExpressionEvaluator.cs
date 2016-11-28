using System;

namespace Konamiman.NestorMSX.Z80Debugger.Console.ExpressionEvaluator
{
    public interface IExpressionEvaluator
    {
        object Evaluate(string expression);

	    event EventHandler<EvaluateFunctionEventArgs> EvaluateFunction;

	    event EventHandler<EvaluateNameEventArgs> EvaluateName;
    }
}
