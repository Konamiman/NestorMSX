using NUnit.Framework;

namespace NestorMSX.Debugger.Tests
{
    public partial class CommandInterpreterTests
    {
        [Test]
        [TestCase("simplekommand()")]
        [TestCase("Simplekommand()")]
        [TestCase("   SimpleKommand(   )  ")]
        public void CanRunSimpleCommandInFunctionMode(string command)
        {
            var result = Sut.ExecuteCommand(command);
            Assert.AreEqual(ClassWithCommands.SimpleCommandResult, result);
        }

        [Test]
        [TestCase("SomeCommands.simplekommand()")]
        [TestCase("somecommands.SimpleKommand()")]
        [TestCase("   some.SimpleKommand(   )  ")]
        public void CanRunSimpleCommandInFunctionMode_UsingClassName(string command)
        {
            var result = Sut.ExecuteCommand(command);
            Assert.AreEqual(ClassWithCommands.SimpleCommandResult, result);
        }

        [Test]
        [TestCase("simpleandnice()")]
        [TestCase("SimpleAndNice()")]
        [TestCase("   san(   )  ")]
        [TestCase("   SAN(   )  ")]
        public void CanRunSimpleCommandInFunctionMode_UsingCommandAlias(string command)
        {
            var result = Sut.ExecuteCommand(command);
            Assert.AreEqual(ClassWithCommands.SimpleCommandWithAliasesResult, result);
        }

        [Test]
        [TestCase("SomeCommands.simpleandnice()")]
        [TestCase("some.SimpleAndNice()")]
        [TestCase("   some.san(   )  ")]
        [TestCase("   s.SAN(   )  ")]
        public void CanRunSimpleCommandInFunctionMode_UsingClassAlias_UsingCommandAlias(string command)
        {
            var result = Sut.ExecuteCommand(command);
            Assert.AreEqual(ClassWithCommands.SimpleCommandWithAliasesResult, result);
        }

        [Test]
        [TestCase("addition")]
        [TestCase("  Addition")]
        [TestCase("sum")]
        [TestCase(" Sum")]
        [TestCase("somecommands.sum")]
        public void CanRunCommandWithParametersSpecifyingOnlyMandatoryParameters(string commandName)
        {
            var command = $"{commandName}( 10, 20,30 )";
            var result = (int)Sut.ExecuteCommand(command);
            Assert.AreEqual(10+20+30+1+2+3, result);
        }

        [Test]
        [TestCase("addition")]
        [TestCase("  Addition")]
        [TestCase("sum")]
        [TestCase(" Sum")]
        [TestCase("some.sum")]
        public void CanRunCommandWithParametersSpecifyingAllParameters(string commandName)
        {
            var command = $"{commandName}( 10, 20,30,0x40,%1111,7 )";
            var result = (int)Sut.ExecuteCommand(command);
            Assert.AreEqual(10+20+30+0x40+15+7, result);
        }

        [Test]
        public void CanRunFunctionsRecursively()
        {
            var command = "sum(10,20,Addition(30,40,50,60))";
            var result = (int)Sut.ExecuteCommand(command);
            Assert.AreEqual(10+20+1+2+3+30+40+50+60+2+3, result);
        }

        [Test]
        public void CanActAsCalculator()
        {
            var command = "(2+3*4)*3";
            var result = Sut.ExecuteCommand(command);
            Assert.AreEqual(42, result);
        }
    }
}
