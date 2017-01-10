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

        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool EqualsCI(this string value, string compared)
        {
            return value.Equals(compared, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsCommandCI(this string value, string command)
        {
            if(value.IsEmpty()) return command.IsEmpty();
            return command.StartsWith(value, StringComparison.InvariantCultureIgnoreCase);
        }

        public static int AsInt(this object value)
        {
            if (value is int)
                return (int) value;
            else if (value is decimal)
                return (int) (decimal) value;
            else
                throw new InvalidCastException($"can't cast from {value.GetType().Name} to int");
        }
    }
}
