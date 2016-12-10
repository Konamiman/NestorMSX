using Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter;
using Konamiman.NestorMSX.Z80Debugger.Console.ExpressionEvaluator;
using NUnit.Framework;

namespace NestorMSX.Debugger.Tests
{
    public partial class CommandInterpreterTests
    {
        [Name("theenum")]
        enum SomeEnum
        {
            One = 1,
            Two = 2,
            Three = 3
        }

        [Test]
        public void CanUseEnumValuesAsConstants()
        {
            var sut = new CommandInterpreter(new EvaluantExpressionEvaluatorWrapper(), new object[] {typeof(SomeEnum)});
            var result = sut.ExecuteCommand("theenum.one+theenum.two+theenum.three");
            Assert.AreEqual(1+2+3, result);
        }

        class CommandsUsingEnum
        {
            public string DoWithEnum(SomeEnum value)
            {
                return "Value: " + value;
            }

            public string DoWithInt(int value)
            {
                return "Value: " + value;
            }
        }

        [Test]
        [TestCase("dowithenum(theenum.one)")]
        [TestCase("dowithenum(1)")]
        public void CanUseEnumValuesAsEnumInputParameters(string expression)
        {
            var sut = new CommandInterpreter(new EvaluantExpressionEvaluatorWrapper(), new object[] {typeof(SomeEnum), new CommandsUsingEnum()});
            var result = sut.ExecuteCommand(expression);
            Assert.AreEqual("Value: One", result);
        }

        [Test]
        [TestCase("dowithint(theenum.one)")]
        [TestCase("dowithint(1)")]
        public void CanUseEnumValuesAsIntInputParameters(string expression)
        {
            var sut = new CommandInterpreter(new EvaluantExpressionEvaluatorWrapper(), new object[] {typeof(SomeEnum), new CommandsUsingEnum()});
            var result = sut.ExecuteCommand(expression);
            Assert.AreEqual("Value: 1", result);
        }
    }
}
