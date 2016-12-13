using System;
using System.Text;
using System.Threading;
using Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandProviders
{
    [Name("utils")]
    public class UtilsCommandsProvider
    {
        public static byte Asc(string value)
        {
            return Encoding.ASCII.GetBytes(value)[0];
        }
    }
}
