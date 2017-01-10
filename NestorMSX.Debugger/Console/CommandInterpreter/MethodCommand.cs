using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter
{
    public class MethodCommand : Command
    {
        private readonly MethodInfo method;
        private readonly object hostingObject;
        private readonly string[] mandatoryParameterNames;
        private readonly string[] parameterNames;
        private readonly Dictionary<CommandParameter, TypeConverterWrapper> converters;

        public MethodCommand(string name, MethodInfo method, object hostingObject)
            :base(name)
        {
            this.method = method;
            this.hostingObject = hostingObject;

            var methodParameters = method.GetParameters();
            this.Parameters =
                methodParameters
                    .Select(p => new CommandParameter(
                        p.Name,
                        !p.IsOptional, 
                        p.DefaultValue,
                        p.HasAttribute<RawExpressionAttribute>() && p.ParameterType == typeof(string)))
                    .ToArray();

            converters = new Dictionary<CommandParameter, TypeConverterWrapper>();
            for(var i=0; i < methodParameters.Length; i++)
                converters.Add(Parameters[i], new TypeConverterWrapper(TypeDescriptor.GetConverter(methodParameters[i].ParameterType), methodParameters[i].ParameterType));

            mandatoryParameterNames = Parameters.Where(p => p.IsMandatory).Select(p => p.Name).ToArray();
            parameterNames = Parameters.Select(p => p.Name).ToArray();
        }

        public override object Execute(string name, CommandArgument[] arguments)
        {
            var fixedArguments =
                arguments.TakeWhile(a => a.Name == null).ToArray();

            var namedArguments =
                arguments.Skip(fixedArguments.Length).ToArray();

            if(namedArguments.Any(a => a.Name == null))
                throw new CommandExecutionException($"{name}: Only named argument specifications must appear after all fixed arguments have been specified");

            var fixedParameterNames = Parameters.Take(fixedArguments.Length).Select(p => p.Name);
            var conflictingArgumentNames = fixedParameterNames.Intersect(namedArguments.Select(a => a.Name.ToLower())).ToArray();
            if(conflictingArgumentNames.Any())
                throw new CommandExecutionException($"{name}: Named argument '{conflictingArgumentNames.First()}' specifies a parameter for which a positional argument has already been given");

            var argumentNamesSpecifiedMoreThanOnce =
                namedArguments.GroupBy(a => a.Name).Where(g => g.Count() > 1).ToArray();
            if(argumentNamesSpecifiedMoreThanOnce.Any())
                throw new CommandExecutionException($"{name}: Named argument '{argumentNamesSpecifiedMoreThanOnce.First().Key}' cannot be specified multiple times");

            var mandatoryParameterNamesAfterUnnamed =
                mandatoryParameterNames.Skip(fixedArguments.Length).ToArray();
            var unespecifiedMandatoryParameterNames =
                mandatoryParameterNamesAfterUnnamed.Except(namedArguments.Select(a => a.Name)).ToArray();
            if(unespecifiedMandatoryParameterNames.Any())
                throw new CommandExecutionException($"{name}: There is no argument given that corresponds to parameter '{unespecifiedMandatoryParameterNames.First()}'");

            var unknownArgumentNames =
                namedArguments.Select(a => a.Name).Except(parameterNames).ToArray();
            if(unknownArgumentNames.Any())
                throw new CommandExecutionException($"{name}: Unknown parameter '{unknownArgumentNames.First()}'");

            var argumentValues = fixedArguments.Zip(Parameters, (a, p) => converters[p].ConvertFrom(a.Value)).ToList();
            foreach(var parameter in Parameters.Skip(fixedArguments.Length)) {
                var matchingArgument = arguments.SingleOrDefault(a => a.Name != null && a.Name == parameter.Name);
                argumentValues.Add(matchingArgument == null ? parameter.DefaultValue : converters[parameter].ConvertFrom(matchingArgument.Value));
            }

            try
            {
                return method.Invoke(hostingObject, argumentValues.ToArray());
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException is CommandExecutionException)
                    return "*** " + ex.InnerException.Message;
                throw new CommandExecutionException($"Exception thrown by method '{hostingObject.GetType().Name}.{method.Name}': {ex.InnerException.Message}", ex.InnerException);
            }
        }
    }
}
