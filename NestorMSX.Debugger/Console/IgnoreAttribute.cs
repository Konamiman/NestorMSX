using System;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public class IgnoreAttribute : Attribute
    {
    }
}
