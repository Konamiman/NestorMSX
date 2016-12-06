namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter
{
    public class CommandArgument
    {
        public CommandArgument(string name, object value)
        {
            Name = name?.ToLower();
            Value = value;
        }

        public string Name { get; }
        public object Value { get; }
        public static readonly CommandArgument[] NoArguments = new CommandArgument[0];
    }
}
