namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter
{
    public class CommandArgument
    {
        public CommandArgument(string name, object value, bool skipEvaluation = false)
        {
            Name = name?.ToLower();
            Value = value;
            SkipEvaluation = skipEvaluation;
        }

        public string Name { get; }
        public object Value { get; }
        public bool SkipEvaluation { get; }
        public static readonly CommandArgument[] NoArguments = new CommandArgument[0];
    }
}
