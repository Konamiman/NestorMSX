using System;
using System.Linq;
using Konamiman.NestorMSX.Z80Debugger.Console;
using Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter;
using Konamiman.NestorMSX.Z80Debugger.Console.ExpressionEvaluator;
using NUnit.Framework;

namespace NestorMSX.Debugger.Tests
{
    public partial class CommandInterpreterTests
    {
        [Name("SomeCommands")]
        class ClassWithCommands
        {
            public static readonly string SimpleCommandResult = "SimpleCommand result";

            public string SimpleKommand()
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

            [Alias("rwprop")]
            public int ReadAndWriteProperty { get; set; }

            [Alias("roprop")]
            public int ReadOnlyProperty => 34;

            [Alias("roprop_priv")]
            public int ReadAndWritePropertyButPrivateSetter { get; private set; }

            public int WriteOnlyPropertyValue;
            [Alias("woprop")]
            public int WriteOnlyProperty
            {
                set { WriteOnlyPropertyValue = value; }
            }

            public int PrivateGetterPropertyValue;
            [Alias("woprop_priv")]
            public int ReadAndWritePropertyButPrivateGetter
            {
                private get
                {
                    return PrivateGetterPropertyValue;
                }
                set
                {
                    PrivateGetterPropertyValue = value;
                }
            }
        }

        private CommandInterpreter Sut;
        private ClassWithCommands CommandsObject;

        [SetUp]
        public void Setup()
        {
            CommandsObject = new ClassWithCommands();
            Sut = new CommandInterpreter(
                new EvaluantExpressionEvaluatorWrapper(),
                new object[] { CommandsObject });
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

        class ClassWithoutNameAttribute
        {
            public void Cmd()
            {
            }
        }

        [Test]
        [TestCase("cmd")]
        [TestCase("ClassWithoutNameAttribute.cmd")]
        [TestCase("NestorMSX.Debugger.Tests.ClassWithoutNameAttribute.cmd")]
        [TestCase("Nes.Deb.Tes.Cla.c")]
        public void CommandsInClassWithoutNameAttributeTest(string commandLine)
        {
            var sut = new CommandInterpreter(new EvaluantExpressionEvaluatorWrapper(), new object[] { new ClassWithoutNameAttribute() });
            sut.ExecuteCommand(commandLine);
        }

        [Name("one.two.three.class")]
        class ClassWithNameWithDotsAttribute
        {
            public void Cmd()
            {
            }
        }

        [Test]
        [TestCase("cmd")]
        [TestCase("class.cmd")]
        [TestCase("three.class.cmd")]
        [TestCase("one.two.three.class.cmd")]
        [TestCase("o.t.t.c.c")]
        public void CommandsInClassWithDottedNameAttributeTest(string commandLine)
        {
            var sut = new CommandInterpreter(new EvaluantExpressionEvaluatorWrapper(), new object[] { new ClassWithNameWithDotsAttribute() });
            sut.ExecuteCommand(commandLine);
        }

        [Test]
        public void CommandsInClassWithDottedNameAttributeTest_NoOriginalNamespace()
        {
            var sut = new CommandInterpreter(new EvaluantExpressionEvaluatorWrapper(), new object[] { new ClassWithNameWithDotsAttribute() });
            AssertThrows(() => sut.ExecuteCommand("NestorMSX.Debugger.Tests.one.two.three.class.cmd"), "Unknown");
        }
    }
}
