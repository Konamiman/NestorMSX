namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter
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
