using System;
using Konamiman.NestorMSX.Z80Debugger.Console;
using Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter;
using Konamiman.NestorMSX.Z80Debugger.Console.ExpressionEvaluator;
using NUnit.Framework;

namespace NestorMSX.Debugger.Tests
{
    public partial class CommandInterpreterTests
    {
        [Name("somecommands")]
        class ClassWithCommands_Duplicate
        {
            public string san()
            {
                return null;
            }
        }

        [Test]
        public void ThrowsOnDuplicateCommands()
        {
            TestDelegate createInterpreter = () => new CommandInterpreter(new EvaluantExpressionEvaluatorWrapper(), new object[] {new ClassWithCommands(), new ClassWithCommands_Duplicate() });
            AssertThrows<Exception>(createInterpreter, "Duplicate", "san");
        }

        [Name("morecommands")]
        class ClassWithCommands_Ambiguous
        {
            public string san()
            {
                return null;
            }
        }

        [Test]
        public void ThrowsOnAmbiguousCommands()
        {
            var sut = new CommandInterpreter(new EvaluantExpressionEvaluatorWrapper(), new object[] {new ClassWithCommands(), new ClassWithCommands_Ambiguous() });
            TestDelegate runCommand = () => sut.ExecuteCommand("somecommands.simple");
            AssertThrows(runCommand, "simplekommand", "simplecommandwithaliases");
        }

        [Test]
        [TestCase("notknown")]
        [TestCase("someclass.notknown")]
        [TestCase("notknown()")]
        [TestCase("someclass.notknown()")]
        [TestCase("notknown 1")]
        [TestCase("someclass.notknown 1")]
        public void ThrowsOnUnknownCommands(string name)
        {
            TestDelegate runCommand = () => Sut.ExecuteCommand(name);
            AssertThrows(runCommand, "Unknown", "notknown");
        }

//Commented out because it pops a "Test runner failed" dialog
#if false
        [Alias("999")]
        class ClassWithCommands_BadAlias
        {
            public string san()
            {
                return null;
            }
        }

        [Test]
        public void ThrowsOnBadAlias()
        {
            TestDelegate createInterpreter = () => new CommandInterpreter(new EvaluantExpressionEvaluatorWrapper(), new object[] {new ClassWithCommands_BadAlias() });
            AssertThrows<ArgumentException>(createInterpreter, "aliases must be", "999");
        }
#endif

        [Test]
        public void ThrowsOnErrorEvaluatingExpression()
        {
            TestDelegate runCommand = () => Sut.ExecuteCommand("san(");
            AssertThrows(runCommand, "evaluating expression");
        }

        [Test]
        public void ThrowsOnErrorParsingExpression()
        {
            TestDelegate runCommand = () => Sut.ExecuteCommand("sum(0xGGG)");
            AssertThrows(runCommand, "parsing command", "hexadecimal");
        }

        [Test]
        public void ThrowsOnErrorConvertingType()
        {
            TestDelegate runCommand = () => Sut.ExecuteCommand("sum(\"foo\",2,3,4,5,6)");
            AssertThrows(runCommand, "Error converting", "foo");
        }

        [Test]
        public void ThrowsOnCantConvertType()
        {
            TestDelegate runCommand = () => Sut.ExecuteCommand("datez(1234)");
            AssertThrows(runCommand, "Can't convert", "1234");
        }

        [Test]
        public void ThrowsOnPositionalParametersAfterNamed()
        {
            TestDelegate runCommand = () => Sut.ExecuteCommand("sum 1 2 num3=3 4 5 6");
            AssertThrows(runCommand, "Only named argument");
        }

        [Test]
        public void ThrowsOnConflictingNamedAndPositionalArguments()
        {
            TestDelegate runCommand = () => Sut.ExecuteCommand("sum 1 2 num1=3");
            AssertThrows(runCommand, "num1", "for which a positional");
        }

        [Test]
        public void ThrowsOnDuplicateNamedArgument()
        {
            TestDelegate runCommand = () => Sut.ExecuteCommand("sum 1 2 3 num4=3 num4=4");
            AssertThrows(runCommand, "num4", "multiple times");
        }

        [Test]
        public void ThrowsOnMissingArgument()
        {
            TestDelegate runCommand = () => Sut.ExecuteCommand("sum 1 2");
            AssertThrows(runCommand, "num3", "There is no argument");
        }

        [Test]
        public void ThrowsOnUnknownParameter()
        {
            TestDelegate runCommand = () => Sut.ExecuteCommand("sum 1 2 3 xxx=4");
            AssertThrows(runCommand, "xxx", "Unknown parameter");
        }

        [Test]
        public void ThrowsOnMethodthrowing()
        {
            TestDelegate runCommand = () => Sut.ExecuteCommand("throws");
            AssertThrows(runCommand, "Exception thrown", "Throws", "Booo!");
        }
    }
}
