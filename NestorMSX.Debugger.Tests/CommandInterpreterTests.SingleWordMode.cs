using NUnit.Framework;

namespace NestorMSX.Debugger.Tests
{
    public partial class CommandInterpreterTests
    {
        [Test]
        [TestCase("simplekommand")]
        [TestCase("Simplekommand")]
        [TestCase("   Simplekommand  ")]
        [TestCase("simplek")]
        public void CanRunSimpleCommandInSingleWordMode(string command)
        {
            var result = Sut.ExecuteCommand(command);
            Assert.AreEqual(ClassWithCommands.SimpleCommandResult, result);
        }

        [Test]
        [TestCase("somecommands.simplekommand")]
        [TestCase("somec.SimpleK")]
        [TestCase("   NestorMSX.Debugger.Tests.somecommands.Simplekommand  ")]
        [TestCase("   Deb.Tes.some.Simplekommand  ")]
        [TestCase("   Te.so.simplek  ")]
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
        public void CanExecuteVoidMethodsAndReturnsNull()
        {
            var result = Sut.ExecuteCommand("thevoid");
            Assert.IsNull(result);
        }
    }
}
