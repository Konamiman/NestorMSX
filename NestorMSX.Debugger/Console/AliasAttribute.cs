using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public class AliasAttribute : Attribute
    {
        private static readonly char[] comma = {','};

        public AliasAttribute(string aliases)
        {
            if(!Regex.IsMatch(
                aliases,
                @"^ *[A-Za-z_][A-Za-z_0-9]*(, *[A-Za-z_][A-Za-z_0-9]*)* *$")) {
                throw new ArgumentException($"{nameof(aliases)} must be a comma-separated list of identifier, each consisting of letters, numbers and underscores; first character can't be a number");
            }

            Aliases = aliases
                .Split(comma, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.ToLower().Trim())
                .Distinct()
                .ToArray();
        }

        public string[] Aliases { get; }
    }
}
