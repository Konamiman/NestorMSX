using System;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public class IgnoreAttribute : Attribute
    {
    }
}
