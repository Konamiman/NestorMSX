using System;
using System.Linq;
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

            public string Concat(int num1, int num2, int num3, int num4 = 1, int num5 = 2, int num6 = 3)
            {
                return $"({num1},{num2},{num3},{num4},{num5},{num6})";
            }

            public string Datez(DateTime d)
            {
                return null;
            }

            public string Throws()
            {
                throw new Exception("Booo!");
            }

            public void TheVoid()
            {
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

        private void AssertThrows(TestDelegate action, params string[] messageParts)
        {
            AssertThrows<CommandExecutionException>(action, messageParts);
        }

        private void AssertThrows<T>(TestDelegate action, params string[] messageParts) where T : Exception
        {
            var ex = Assert.Throws<T>(action);
            Assert.IsInstanceOf<T>(ex);
            Assert.True(messageParts.All(m => ex.Message.Contains(m)), $"Exception message should contain '{string.Join(",", messageParts)}', but it is '{ex.Message}'");
            Console.WriteLine(ex.Message);
        }
    }
}
