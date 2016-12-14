using System;
using System.Reflection;

namespace Konamiman.NestorMSX.Z80Debugger
{
    public static class ExtensionMethods
    {
        public static bool HasAttribute<T>(this ParameterInfo parameter) where T : Attribute
        {
            return parameter.GetCustomAttributes(typeof(T), false).Length > 0;
        }

        public static string Right(this string value, int length)
        {
            return value.Substring(value.Length - length, length);
        }
    }
}
