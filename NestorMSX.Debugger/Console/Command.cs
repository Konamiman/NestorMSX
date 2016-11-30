using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    public abstract class Command
    {
        public Command(CommandParameter[] parameters = null)
        {
            Parameters = parameters;
        }

        protected object HostingObject { get; }
        public CommandParameter[] Parameters { get; protected set; }
        public abstract object Execute(string name, CommandArgument[] arguments);
    }
}
