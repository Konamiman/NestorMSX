using NUnit.Framework;

namespace NestorMSX.Debugger.Tests
{
    public partial class CommandInterpreterTests
    {
        [Test]
        public void CanExecuteCommandPassingAllParametersAsPositional()
        {
            var result = Sut.ExecuteCommand("concat 10 20 30 40 50 60");
            Assert.AreEqual("(10,20,30,40,50,60)", result);
        }

        [Test]
        public void CanExecuteCommandPassingAllParametersAsNamed_OriginalOrder()
        {
            var result = Sut.ExecuteCommand("concat num1=10 num2=20 num3=30 num4=40 num5=50 num6=60");
            Assert.AreEqual("(10,20,30,40,50,60)", result);
        }

        [Test]
        public void CanExecuteCommandPassingAllParametersAsNamed_DifferentOrder()
        {
            var result = Sut.ExecuteCommand("concat num6=60 num1=10 num3=30 num2=20 num4=40 num5=50 ");
            Assert.AreEqual("(10,20,30,40,50,60)", result);
        }

        [Test]
        public void CanExecuteCommandPassingOnlyMandatoryParametersAsPositional()
        {
            var result = Sut.ExecuteCommand("concat 10 20 30");
            Assert.AreEqual("(10,20,30,1,2,3)", result);
        }

        [Test]
        public void CanExecuteCommandPassingOnlyMandatoryParametersAsNamed()
        {
            var result = Sut.ExecuteCommand("concat num2=20 num3=30 num1=10");
            Assert.AreEqual("(10,20,30,1,2,3)", result);
        }

        [Test]
        public void CanExecuteCommandPassingMandatoryParametersAsPositionalThenOptionalAsNamed()
        {
            var result = Sut.ExecuteCommand("concat 10 20 30 num6=40");
            Assert.AreEqual("(10,20,30,1,2,40)", result);
        }

        [Test]
        public void CanExecuteCommandPassingFunctionsAsParameters()
        {
            var result = Sut.ExecuteCommand("concat 10 sum(2,4,1) 30 num5=sum(7,1,1)");
            Assert.AreEqual("(10,13,30,1,15,3)", result);
        }
    }
}
