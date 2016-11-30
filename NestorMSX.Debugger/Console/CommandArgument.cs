using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    public class CommandArgument
    {
        public CommandArgument(string name, object value)
        {
            Name = name.ToLower();
            Value = value;
        }

        public string Name { get; }
        public object Value { get; }
    }
}
