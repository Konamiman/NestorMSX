using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Konamiman.NestorMSX.Z80Debugger.Console.ExpressionEvaluator;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    public class CommandInterpreter
    {
        private readonly IExpressionEvaluator expressionEvaluator;
        private readonly Dictionary<string, Command> commandsByFullName;
        private readonly Dictionary<string, string[]> commandFullNamesBySimpleName;

        public CommandInterpreter(IExpressionEvaluator expressionEvaluator, object commandsProvider)
        {
            this.expressionEvaluator = expressionEvaluator;
            
            commandsByFullName = new Dictionary<string, Command>();
            AddCommandsFrom(commandsProvider);
        }

        private void AddCommandsFrom(object commandsProvider)
        {
            var commandFullNamesBySimpleNameTemp = new Dictionary<string, List<string>>();

            var type = commandsProvider.GetType();

            var @namespace = (type
                .GetCustomAttributes(typeof(NamespaceAttribute), false)
                .SingleOrDefault() as NamespaceAttribute)?.Namespace ?? type.Name.ToLower();

            var commandMethods = commandsProvider
                .GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(IgnoreAttribute), false).Length == 0);

            foreach(var method in commandMethods)
            {
                var aliases = new List<string>();
                aliases.Add(method.Name.ToLower());
                var aliasesAttribute =
                    method.GetCustomAttributes(typeof(AliasAttribute), false).SingleOrDefault() as AliasAttribute;
                if(aliasesAttribute != null)
                    aliases.AddRange(aliasesAttribute.Aliases);

                var command = new MethodCommand(method, commandsProvider);
                foreach (var alias in aliases) {
                    var fullName = $"{@namespace}.{alias}";
                    if(commandsByFullName.ContainsKey(fullName))
                        throw new Exception($"Duplicate command: {fullName}");

                    commandsByFullName.Add(alias, command);

                    if(!commandFullNamesBySimpleNameTemp.ContainsKey(alias))
                        commandFullNamesBySimpleNameTemp.Add(alias, new List<string>());
                    commandFullNamesBySimpleNameTemp[alias].Add(fullName);
                }
            }
        }
    }
}
