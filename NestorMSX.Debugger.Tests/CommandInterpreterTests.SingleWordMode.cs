using NUnit.Framework;

namespace NestorMSX.Debugger.Tests
{
    public partial class CommandInterpreterTests
    {
        [Test]
        [TestCase("simplecommand")]
        [TestCase("SimpleCommand")]
        [TestCase("   SimpleCommand  ")]
        public void CanRunSimpleCommandInSingleWordMode(string command)
        {
            var result = Sut.ExecuteCommand(command);
            Assert.AreEqual(ClassWithCommands.SimpleCommandResult, result);
        }

        [Test]
        [TestCase("niceclass.simplecommand")]
        [TestCase("classy.SimpleCommand")]
        [TestCase("   NiceClass.SimpleCommand  ")]
        [TestCase("   Classy.simplecommand  ")]
        public void CanRunSimpleCommandInSingleWordMode_UsingClassAliases(string command)
        {
            var result = Sut.ExecuteCommand(command);
            Assert.AreEqual(ClassWithCommands.SimpleCommandResult, result);
        }

        [Test]
        [TestCase("SimpleCommandWithAliases")]
        [TestCase("simplecommandwithaliases")]
        [TestCase("   SimpleCommandWithAliases  ")]
        [TestCase("simpleandnice")]
        [TestCase("SimpleAndNice")]
        [TestCase("   simpleandnice  ")]
        [TestCase("san")]
        [TestCase("San")]
        [TestCase("   san  ")]
        public void CanRunSimpleCommandInSingleWordMode_UsingCommandAliases(string command)
        {
            var result = Sut.ExecuteCommand(command);
            Assert.AreEqual(ClassWithCommands.SimpleCommandWithAliasesResult, result);
        }

        [Test]
        [TestCase("niceclass.simpleandnice")]
        [TestCase("NiceClass.SimpleAndNice")]
        [TestCase("   Classy.simpleandnice  ")]
        [TestCase("classy.san")]
        [TestCase("classy.San")]
        [TestCase("   niceclass.san  ")]
        public void CanRunSimpleCommandInSingleWordMode_UsingClassAliases_UsingCommandAliases(string command)
        {
            var result = Sut.ExecuteCommand(command);
            Assert.AreEqual(ClassWithCommands.SimpleCommandWithAliasesResult, result);
        }
        
        [Test]
        public void CanExecuteVoidMethodsAndReturnsNull()
        {
            var result = Sut.ExecuteCommand("thevoid");
            Assert.IsNull(result);
        }
    }
}
