using NUnit.Framework;

namespace NestorMSX.Debugger.Tests
{
    public partial class CommandInterpreterTests
    {
        [Test]
        [TestCase("rwprop=")]
        [TestCase("  rwprop  =  ")]
        [TestCase("ReadandwriteProperty=")]
        public void CanAssignValueToReadAndWriteProperty(string commandStart)
        {
            Sut.ExecuteCommand(commandStart + "2+2");
            Assert.AreEqual(4, CommandsObject.ReadAndWriteProperty);
        }

        [Test]
        public void CanAssignValueToPropertyWithNoGetter()
        {
            Sut.ExecuteCommand("woprop = 1234");
            Assert.AreEqual(1234, CommandsObject.WriteOnlyPropertyValue);
        }

        [Test]
        public void CanAssignValueToPropertyWithPrivateGetter()
        {
            Sut.ExecuteCommand("woprop_priv = 5678");
            Assert.AreEqual(5678, CommandsObject.PrivateGetterPropertyValue);
        }
    }
}
