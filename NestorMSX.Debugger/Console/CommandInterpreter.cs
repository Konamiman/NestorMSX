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
        private readonly Dictionary<string, Command> commandsByFullName;
        private readonly Dictionary<string, List<string>> commandFullNamesBySimpleName;
        private readonly Dictionary<string, List<Command>> commandsBySimpleName;

        private readonly Tuple<string, Func<string, object>>[] commandExecutors;

        public CommandInterpreter(IExpressionEvaluator expressionEvaluator, object[] commandsProviders)
        {
            this.expressionEvaluator = expressionEvaluator;
            
            commandsByFullName = new Dictionary<string, Command>();
            commandsBySimpleName = new Dictionary<string, List<Command>>();
            commandFullNamesBySimpleName = new Dictionary<string, List<string>>();
            foreach(var obj in commandsProviders)
                AddCommandsFrom(obj);
            
            expressionEvaluator.EvaluateFunction += EvaluateFunction;

            commandExecutors = new[]
            {
                new Tuple<string, Func<string, object>>("^[A-Za-z_][A-Za-z_0-9.]*$", ExecuteSingleWord),
                new Tuple<string, Func<string, object>>("^[A-Za-z_][A-Za-z_0-9.]* +[^ ]+$", ExecuteCommandStyle),
                new Tuple<string, Func<string, object>>("^[A-Za-z_][A-Za-z_0-9.]*\\(.*\\)$", ExecuteFunctionStyle)
            };
        }

        private void AddCommandsFrom(object commandsProvider)
        {
            var type = commandsProvider.GetType();
            var equivalencyId = Guid.NewGuid();

            var typeAliases = GetAliases(type);

            var commandMethods = commandsProvider
                .GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(IgnoreAttribute), false).Length == 0);
            
            foreach(var method in commandMethods) {
                var methodAliases = GetAliases(method);

                var command = new MethodCommand(method, commandsProvider, equivalencyId);
                foreach (var typeAlias in typeAliases) {
                    foreach (var methodAlias in methodAliases) {
                        var fullName = $"{typeAlias}.{methodAlias}";
                        if (commandsByFullName.ContainsKey(fullName))
                            throw new Exception($"Duplicate command: {fullName}");

                        commandsByFullName.Add(fullName, command);

                        if (!commandFullNamesBySimpleName.ContainsKey(methodAlias))
                            commandFullNamesBySimpleName.Add(methodAlias, new List<string>());
                        commandFullNamesBySimpleName[methodAlias].Add(fullName);

                        if (!commandsBySimpleName.ContainsKey(methodAlias))
                            commandsBySimpleName.Add(methodAlias, new List<Command>());
                        commandsBySimpleName[methodAlias].Add(command);
                    }
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
            if(!name.Contains(".")) {
                if(!commandFullNamesBySimpleName.ContainsKey(name))
                    throw new CommandExecutionException($"Unknown command: {name}");
                var commands = commandsBySimpleName[name];
                if(commands.Count > 1 && commands.Select(c => c.EquivalencyId).Distinct().Count() > 1)
                    throw new CommandExecutionException($"Ambiguous command {name}, possible matches: {string.Join(",", commandFullNamesBySimpleName[name].ToArray())}");
                return commands[0];
            }

            if(!commandsByFullName.ContainsKey(name))
                throw new CommandExecutionException($"Unknown command: {name}");

            return commandsByFullName[name];
        }

        public object ExecuteCommand(string commandLine)
        {
            commandLine = commandLine.Trim();
            var executor = commandExecutors.FirstOrDefault(e => Regex.IsMatch(commandLine, e.Item1));
            if(executor == null)
                throw new CommandExecutionException("Syntax error");

            return executor.Item2(commandLine);
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
                    var argValue = token.Substring(indexOfEquals + 1);
                    arguments.Add(new CommandArgument(argName, argValue));
                }
                else {
                    arguments.Add(new CommandArgument(null, token));
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
