using System;
using System.ComponentModel;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    public static class ExtensionMethods
    {
        public static object TryConvertFrom(this TypeConverter converter, object value, Type targetType)
        {
            if (value.GetType() == targetType)
                return value;

            try {
                if (converter.CanConvertFrom(value.GetType()))
                    return converter.ConvertFrom(value);
            }
            catch (Exception ex) {
                throw new CommandExecutionException($"Error converting value '{value}' from type {value.GetType().Name} to type {targetType.Name}: {ex.Message}", ex);
            }

            throw new CommandExecutionException($"Can't convert value '{value}' from type {value.GetType().Name} to type {targetType.Name}");
        }
    }
}
