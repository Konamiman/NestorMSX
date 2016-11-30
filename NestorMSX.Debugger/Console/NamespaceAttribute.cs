using System;
using System.Text.RegularExpressions;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NamespaceAttribute : Attribute
    {
        public NamespaceAttribute(string @namespace)
        {
            if (!Regex.IsMatch(
                @namespace,
                @"^[A-Za-z_][A-Za-z_0-9]*$")) {
                throw new ArgumentException(
                    $"{nameof(@namespace)} must be a identifier consisting of letters, numbers and underscores; first character can't be a number");
            }

            Namespace = @namespace;
        }

        public string Namespace { get; }
    }
}
