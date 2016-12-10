using System;
using System.Text.RegularExpressions;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, AllowMultiple = false)]
    public class NameAttribute : Attribute
    {
        public string Name { get; }

        public NameAttribute(string name)
        {
            if(!Regex.IsMatch(
                name,
                @"^ *[A-Za-z_.][A-Za-z_0-9.]* *$")) {
                throw new ArgumentException($"{nameof(name)} must be a identifier consisting of letters, dots, numbers and underscores; first character can't be a number. Got '{name}' instead.");
            }

            Name = name.Trim(' ', '.').ToLower();
            while (name.Contains(".."))
                name = name.Replace("..", ".");
        }
    }
}
