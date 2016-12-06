namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter
{
    public abstract class Command : TokenWithName
    {
        protected Command(string name, CommandParameter[] parameters = null) : base(name)
        {
            Parameters = parameters;
        }

        protected object HostingObject { get; }
        public CommandParameter[] Parameters { get; protected set; }
        public abstract object Execute(string name, CommandArgument[] arguments);
    }
}
