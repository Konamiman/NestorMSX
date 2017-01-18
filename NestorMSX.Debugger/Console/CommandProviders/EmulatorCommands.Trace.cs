using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandProviders
{
    public partial class EmulatorCommandsProvider
    {
        [Alias("print")]
        public string PrintEvaluated(string str)
        {
            return EvaluateInterpolatedString(str);
        }

        private string EvaluateInterpolatedString(string str)
        {
            var sb = new StringBuilder();
            var paramValues = new List<object>();
            int prevStartIndex = 0;
            int paramIndex = 0;
            while (true)
            {
                var startIndex = str.IndexOf("{", prevStartIndex);
                if (startIndex == -1)
                    break;
                var endIndex = str.IndexOf("}", startIndex);
                if (endIndex == -1)
                    break;

                var rawParameter = str.Substring(startIndex + 1, endIndex - startIndex - 1);
                var parameterParts = rawParameter.Split(':');

                paramValues.Add(EvaluateExpression(parameterParts[0]));

                sb.Append(str.Substring(prevStartIndex, startIndex - prevStartIndex));
                if (parameterParts.Length == 1)
                    sb.Append($"{{{paramIndex}}}");
                else
                    sb.Append($"{{{paramIndex}:{parameterParts[1]}}}");

                prevStartIndex = endIndex + 1;
                paramIndex++;
            }

            return string.Format(sb.ToString(), paramValues.ToArray());
        }
    }
}
