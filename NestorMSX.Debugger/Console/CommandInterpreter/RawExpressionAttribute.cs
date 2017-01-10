using System;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class RawExpressionAttribute : Attribute
    {
    }
}
