using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    public class CommandParameter
    {
        public CommandParameter(string name, bool isMandatory = true, object defaultValue = null)
        {
            Name = name.ToLower();
            IsMandatory = isMandatory;
            DefaultValue = defaultValue;
        }

        public string Name { get; }
        public bool IsMandatory { get; }
        public object DefaultValue { get; }
    }
}
