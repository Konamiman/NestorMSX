using System;

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
        public abstract Guid EquivalencyId { get; }
    }
}
