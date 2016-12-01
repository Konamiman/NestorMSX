using Konamiman.NestorMSX.Z80Debugger.Console;
using Konamiman.NestorMSX.Z80Debugger.Console.ExpressionEvaluator;
using NUnit.Framework;

namespace NestorMSX.Debugger.Tests
{
    public partial class CommandInterpreterTests
    {
        [Alias("  niceClass,  classy  ")]
        class ClassWithCommands
        {
            public static readonly string SimpleCommandResult = "SimpleCommand result";

            public string SimpleCommand()
            {
                return SimpleCommandResult;
            }

            public static readonly string SimpleCommandWithAliasesResult = "SimpleCommandWithAliases result";

            [Alias("   SimpleAndNice,  san   ")]
            public string SimpleCommandWithAliases()
            {
                return SimpleCommandWithAliasesResult;
            }

            [Alias("sum")]
            public int Addition(int num1, int num2, int num3, int num4 = 1, int num5 = 2, int num6 = 3)
            {
                return num1 + num2 + num3 + num4 + num5 + num6;
            }
        }

        private CommandInterpreter Sut;

        [SetUp]
        public void Setup()
        {
            Sut = new CommandInterpreter(
                new EvaluantExpressionEvaluatorWrapper(),
                new object[] { new ClassWithCommands() });
        }
    }
}
