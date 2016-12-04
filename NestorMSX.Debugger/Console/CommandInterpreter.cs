using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Konamiman.NestorMSX.Z80Debugger.Console.ExpressionEvaluator;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    public class CommandInterpreter
    {
        private readonly IExpressionEvaluator expressionEvaluator;
        private readonly Command[] commands;
        private readonly Tuple<string, Func<string, object>>[] commandExecutors;

        public CommandInterpreter(IExpressionEvaluator expressionEvaluator, object[] commandsProviders)
        {
            this.expressionEvaluator = expressionEvaluator;
            
            var commandsList = new List<Command>();
            foreach(var obj in commandsProviders)
                AddCommandsFrom(obj, commandsList);
            commands = commandsList.ToArray();
            
            expressionEvaluator.EvaluateFunction += EvaluateFunction;

            commandExecutors = new[]
            {
                new Tuple<string, Func<string, object>>("^[A-Za-z_][A-Za-z_0-9.]*$", ExecuteSingleWord),
                new Tuple<string, Func<string, object>>("^[A-Za-z_][A-Za-z_0-9.]* +.+$", ExecuteCommandStyle)
            };
        }

        private void AddCommandsFrom(object commandsProvider, List<Command> commandsList)
        {
            var type = commandsProvider.GetType();
            var typeName = (type
                .GetCustomAttributes(typeof(NameAttribute), false)
                .SingleOrDefault() as NameAttribute)?.Name ?? type.Name;

            if(!typeName.Contains("."))
                typeName = (type.Namespace + "." + typeName).ToLower();

            var commandMethods = commandsProvider
                .GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(IgnoreAttribute), false).Length == 0);
            
            foreach(var method in commandMethods) {
                var methodAliases = GetAliases(method);
                
                foreach (var methodAlias in methodAliases) {
                    var commandFullName = $"{typeName}.{methodAlias}";
                    if (commandsList.Any(c => c == commandFullName))
                        throw new Exception($"Duplicate command: {commandFullName}");

                    var command = new MethodCommand(commandFullName, method, commandsProvider);
                    commandsList.Add(command);
                }
            }
        }

        private string[] GetAliases(MemberInfo member)
        {
            var aliases = new List<string>();
            aliases.Add(member.Name.ToLower());
            var aliasesAttribute =
                member.GetCustomAttributes(typeof(AliasAttribute), false).SingleOrDefault() as AliasAttribute;
            if(aliasesAttribute != null)
                aliases.AddRange(aliasesAttribute.Aliases);
            return aliases.ToArray();
        }

        private void EvaluateFunction(object sender, EvaluateFunctionEventArgs args)
        {
            var name = args.Name;
            var command = GetCommand(ref name);
            var arguments = args.Parameters.Select(a => new CommandArgument(null, a)).ToArray();
            args.Result = command.Execute(name, arguments);
        }

        private Command GetCommand(ref string name)
        {
            name = name.ToLower().Trim('.');
            var name2 = name; //can't use ref param inside anonymous method (I didn't know that!)
            var matchingCommands = commands.Where(c => c == name2).ToArray();
            
            if(matchingCommands.Length == 0)
                throw new CommandExecutionException($"Unknown command: {name}");

            if(matchingCommands.Length > 1)
                    throw new CommandExecutionException($"Ambiguous command '{name}', possible matches: {string.Join(",", matchingCommands.Select(c => c.FullName))}");

            return matchingCommands[0];
        }

        public object ExecuteCommand(string commandLine)
        {
            commandLine = commandLine.Trim();
            var executor =
                commandExecutors.FirstOrDefault(e => Regex.IsMatch(commandLine, e.Item1))?.Item2 ?? ExecuteFunctionStyle;

            return executor(commandLine);
        }

        private object ExecuteSingleWord(string commandLine)
        {
            var command = GetCommand(ref commandLine);
            return command.Execute(commandLine, CommandArgument.NoArguments);
        }

        private object ExecuteCommandStyle(string commandLine)
        {
            var tokens = Regex.Split(commandLine, 
                "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            var commandName = tokens[0];
            var command = GetCommand(ref commandName);

            var arguments = new List<CommandArgument>();
            foreach(var token in tokens.Skip(1)) {
                if(Regex.IsMatch(token, "^([A-Za-z_][A-Za-z_0-9]*)=")) {
                    var indexOfEquals = token.IndexOf("=");
                    var argName = token.Substring(0, indexOfEquals).ToLower();
                    var argValue = expressionEvaluator.Evaluate(token.Substring(indexOfEquals + 1));
                    arguments.Add(new CommandArgument(argName, argValue));
                }
                else {
                    arguments.Add(new CommandArgument(null, expressionEvaluator.Evaluate(token)));
                }
            }

            return command.Execute(commandName, arguments.ToArray());
        }

        private object ExecuteFunctionStyle(string commandLine)
        {
            return expressionEvaluator.Evaluate(commandLine);
        }
    }
}
