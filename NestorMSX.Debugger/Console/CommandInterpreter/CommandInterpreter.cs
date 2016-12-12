using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Konamiman.NestorMSX.Z80Debugger.Console.ExpressionEvaluator;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter
{
    public class CommandInterpreter
    {
        private readonly IExpressionEvaluator expressionEvaluator;
        private readonly Command[] commands;
        private readonly Variable[] variables;
        private readonly Tuple<string, Func<string, object>>[] commandExecutors;
        private readonly Tuple<object, MethodInfo>[] getUnknownVariableValueHandlers;
        private readonly Tuple<object, MethodInfo>[] setUnknownVariableValueHandlers;

        public CommandInterpreter(IExpressionEvaluator expressionEvaluator, object[] commandsProviders)
        {
            this.expressionEvaluator = expressionEvaluator;
            
            var commandsList = new List<Command>();
            var propertiesList = new List<Variable>();
            var getUnknownVariableValueHandlersList = new List<Tuple<object, MethodInfo>>();
            var setUnknownVariableValueHandlersList = new List<Tuple<object, MethodInfo>>();
            foreach (var obj in commandsProviders) {
                if ((obj as Type)?.IsEnum == true)
                    AddEnumValuesFrom((Type) obj, propertiesList);
                else
                    AddCommandsAndPropertiesFrom(obj, commandsList, propertiesList, getUnknownVariableValueHandlersList, setUnknownVariableValueHandlersList);
            }
            commands = commandsList.ToArray();
            variables = propertiesList.ToArray();
            getUnknownVariableValueHandlers = getUnknownVariableValueHandlersList.ToArray();
            setUnknownVariableValueHandlers = setUnknownVariableValueHandlersList.ToArray();
            expressionEvaluator.EvaluateFunction += EvaluateFunction;
            expressionEvaluator.EvaluateName += EvaluateName;

            commandExecutors = new[]
            {
                new Tuple<string, Func<string, object>>("^[A-Za-z_][A-Za-z_0-9.]*$", ExecuteSingleWord),
                new Tuple<string, Func<string, object>>("^[A-Za-z_][A-Za-z_0-9.]*(?> +)[^=].*$", ExecuteCommandStyle),
                new Tuple<string, Func<string, object>>("^[A-Za-z_][A-Za-z_0-9.]* *=.+$", ExecuteAssignmentStyle),
            };
        }

        private void AddEnumValuesFrom(Type type, List<Variable> propertiesList)
        {
            var typeName = GetTypeName(type);
            var names = Enum.GetNames(type);
            var values = Enum.GetValues(type).Cast<object>().Select(v => (int)v).ToArray();
            for(int i=0; i<names.Length; i++) {
                var fullName = $"{typeName}.{names[i]}";
                propertiesList.Add(new ConstantValueVariable(fullName, values[i]));
            }
        }

        private void AddCommandsAndPropertiesFrom(
            object commandsProvider,
            List<Command> commandsList,
            List<Variable> propertiesList, List<Tuple<object, MethodInfo>> getUnknownVariableValueHandlersList, 
            List<Tuple<object, MethodInfo>> setUnknownVariableValueHandlersList)
        {
            Type type;
            BindingFlags bindingFlags;
            if(commandsProvider is Type) {
                type = (Type)commandsProvider;
                commandsProvider = null;
                bindingFlags = BindingFlags.Public | BindingFlags.Static;
            }
            else {
                type = commandsProvider.GetType();
                bindingFlags = BindingFlags.Public | BindingFlags.Instance;
            }

            // command methods

            var typeName = GetTypeName(type);

            var commandMethods = type
                .GetMethods(bindingFlags)
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

            // variable properties

            var propertyVariables = type
                .GetProperties(bindingFlags)
                .Where(p => p.GetCustomAttributes(typeof(IgnoreAttribute), false).Length == 0)
                .ToArray();

            foreach(var property in propertyVariables) {
                var propertyAliases = GetAliases(property);
                
                foreach (var propertyAlias in propertyAliases) {
                    var propertyFullName = $"{typeName}.{propertyAlias}";
                    if (propertiesList.Any(c => c == propertyFullName))
                        throw new Exception($"Duplicate variable: {propertyFullName}");

                    var variable = new PropertyVariable(propertyFullName, property, commandsProvider);
                    propertiesList.Add(variable);
                }
            }

            // variable fallbacks

            var getFallback = type.GetMethod("TryGetVariableValue", new[] { typeof(string), typeof(object).MakeByRefType() });
            if(getFallback!=null && getFallback.GetParameters()[1].IsOut && getFallback.ReturnType == typeof(bool))
                getUnknownVariableValueHandlersList.Add(new Tuple<object, MethodInfo>(commandsProvider, getFallback));

            var setFallback = type.GetMethod("TrySetVariableValue", new[] { typeof(string), typeof(object) });
            if(setFallback!=null && setFallback.ReturnType == typeof(bool))
                setUnknownVariableValueHandlersList.Add(new Tuple<object, MethodInfo>(commandsProvider, setFallback));

            // expression evaluation property

            var expressionEvaluatorProperty =
                propertyVariables.SingleOrDefault(p => p.Name == "EvaluateExpression" && p.PropertyType == typeof(Func<string, object>) && p.GetSetMethod()?.IsPublic == true);
            expressionEvaluatorProperty?.SetValue(commandsProvider, (Func<string, object>)expressionEvaluator.Evaluate, null);
        }

        private static string GetTypeName(Type type)
        {
            var typeName = (type
                .GetCustomAttributes(typeof(NameAttribute), false)
                .SingleOrDefault() as NameAttribute)?.Name ?? type.Name;

            if (!typeName.Contains("."))
                typeName = (type.Namespace + "." + typeName).ToLower();
            return typeName;
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

        private void EvaluateName(object sender, EvaluateNameEventArgs args)
        {
            args.Result = GetVariableValue(args.Name, "Unknown variable");
        }

        private object GetVariableValue(string name, string errorMessage)
        {
            string name2 = name;
            var variable = GetVariable(ref name2);
            if(variable != null) {
                return variable.GetValue();
            }

            var parameters = new object[] {name2, null};
            foreach(var handler in getUnknownVariableValueHandlers) {
                try {
                    if((bool)handler.Item2.Invoke(handler.Item1, parameters))
                        return parameters[1];
                }
                catch (TargetInvocationException ex) {
                    throw new CommandExecutionException($"Error obtaining the value of variable '{name2}': {ex.InnerException.Message}", ex);
                }
            }

            throw new CommandExecutionException($"{errorMessage}: '{name2}'" );
        }

        private Command GetCommand(ref string name, bool throwIfNotFound = true)
        {
            var name2 = name.ToLower().Trim('.'); //can't use ref param inside anonymous method (I didn't know that!)
            var matchingCommands = commands.Where(c => c == name2).ToArray();

            if(matchingCommands.Length == 0) {
                if(throwIfNotFound)
                    throw new CommandExecutionException($"Unknown command: {name}");
                else
                    return null;
            }

            if(matchingCommands.Length > 1)
                throw new CommandExecutionException($"Ambiguous command '{name}', possible matches: {string.Join(",", matchingCommands.Select(c => c.FullName))}");

            name = matchingCommands[0].FullName;
            return matchingCommands[0];
        }

        private Variable GetVariable(ref string name)
        {
            var name2 = name.ToLower().Trim('.'); //can't use ref param inside anonymous method (I didn't know that!)
            var matchingVariables = variables.Where(v => v == name2).ToArray();

            if(matchingVariables.Length == 0) {
                name = name2;
                return null;
            }

            if (matchingVariables.Length > 1) {
                var exactMatch = matchingVariables.Where(v => v.FullName.EndsWith(name2)).ToArray();
                if(exactMatch.Length == 1) {
                    name = exactMatch[0].FullName;
                    return exactMatch[0];
                }

                throw new CommandExecutionException(
                    $"Ambiguous variable '{name}', possible matches: {string.Join(",", matchingVariables.Select(c => c.FullName))}");
            }

            name = matchingVariables[0].FullName;
            return matchingVariables[0];
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
            var command = GetCommand(ref commandLine, false);
            if(command != null)
                return command.Execute(commandLine, CommandArgument.NoArguments);

            return GetVariableValue(commandLine, "Unknown command or variable");
        }

        private object ExecuteCommandStyle(string commandLine)
        {
            var tokens = Regex.Split(commandLine, 
                "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            var commandName = tokens[0];
            var command = GetCommand(ref commandName);

            var arguments = new List<CommandArgument>();
            var parameterIndex = 0;
            foreach(var token in tokens.Skip(1)) {
                if(Regex.IsMatch(token, "^([A-Za-z_][A-Za-z_0-9]*)=")) {
                    var indexOfEquals = token.IndexOf("=");
                    var argName = token.Substring(0, indexOfEquals).ToLower();
                    var argValue = token.Substring(indexOfEquals + 1);
                    var skipEvaluation = command.Parameters.Any(p => p.Name.Equals(argName, StringComparison.InvariantCultureIgnoreCase) && p.IsRawExpression);
                    arguments.Add(new CommandArgument(argName, skipEvaluation ? argValue : expressionEvaluator.Evaluate(argValue)));
                }
                else {
                    var skipEvaluation = command.Parameters[parameterIndex].IsRawExpression;
                    var value = skipEvaluation ? token : expressionEvaluator.Evaluate(token);
                    arguments.Add(new CommandArgument(null, value));
                }
                parameterIndex++;
            }

            return command.Execute(commandName, arguments.ToArray());
        }

        private object ExecuteFunctionStyle(string commandLine)
        {
            return expressionEvaluator.Evaluate(commandLine);
        }

        private object ExecuteAssignmentStyle(string commandLine)
        {
            var indexOfEquals = commandLine.IndexOf("=");
            var variableName = commandLine.Substring(0, indexOfEquals).Trim();
            var expression = commandLine.Substring(indexOfEquals + 1);
            var value = expressionEvaluator.Evaluate(expression);
            SetVariableValue(variableName, value);
            return value;
        }

        private void SetVariableValue(string name, object value)
        {
            string name2 = name;
            var variable = GetVariable(ref name2);
            if(variable != null) {
                variable.SetValue(value);
                return;
            }

            foreach(var handler in setUnknownVariableValueHandlers) {
                try {
                    if((bool)handler.Item2.Invoke(handler.Item1, new object[] {name, value}))
                        return;
                }
                catch (TargetInvocationException ex) {
                    throw new CommandExecutionException($"Error setting the value of variable '{name2}': {ex.InnerException.Message}", ex);
                }
            }

            throw new CommandExecutionException($"Unknown variable: '{name2}'" );
        }
    }
}
