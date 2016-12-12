using System;
using System.ComponentModel;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter
{
    public class TypeConverterWrapper
    {
        private readonly TypeConverter converter;
        private readonly Type targetType;

        public TypeConverterWrapper(TypeConverter converter, Type targetType)
        {
            this.converter = converter;
            this.targetType = targetType;
        }

        public object ConvertFrom(object value)
        {
            if (value.GetType() == targetType)
                return value;

            if (value is decimal && targetType == typeof(int))
                return (int)(decimal)value;
            if (value is int && targetType == typeof(decimal))
                return (decimal)(int)value;
            if (value is int && targetType == typeof(byte))
                return (byte)(int)value;
            if (value is byte && targetType == typeof(int))
                return (int)(byte)value;

            if (value is int && targetType.IsEnum)
                return Enum.ToObject(targetType, (int) value);

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
