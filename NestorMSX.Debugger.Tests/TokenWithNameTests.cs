using Konamiman.NestorMSX.Z80Debugger.Console;
using NUnit.Framework;

namespace NestorMSX.Debugger.Tests
{
    public class TokenWithNameTests
    {
        private class TestClass : TokenWithName
        {
            public TestClass(string fullName) : base(fullName)
            {
            }
        }

        [Test]
        public void ValidNameTest()
        {
            var sut = new TestClass("  abc.def._12AB  ");
            Assert.IsTrue(sut.HasValidName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("1ab")]
        [TestCase("abc.def.123")]
        [TestCase(",")]
        [TestCase("ab c")]
        public void InValidNamesTest(string name)
        {
            var sut = new TestClass(name);
            Assert.IsFalse(sut.HasValidName);
        }

        [Test]
        [TestCase("abcde.fghij.klmno.pqrst")]
        [TestCase("fghij.klmno.pqrst")]
        [TestCase("klmno.pqrst")]
        [TestCase("pqrst")]
        [TestCase("ab.fg.kl.pq")]
        [TestCase("fg.kl.pq")]
        [TestCase("kl.pq")]
        [TestCase("pq")]
        public void StringEqualityTest(string testedName)
        {
            var sut=new TestClass("Abcde.fghij.klmno.pqrsT");
            Assert.True(sut == testedName);
            Assert.AreEqual(sut, testedName);
        }

        [Test]
        public void ObjectEqualityTest()
        {
            var sut1 = new TestClass("abc");
            var sut2 = new TestClass("abc");
            Assert.AreEqual(sut1, sut2);
        }

        [Test]
        public void ObjectInequalityTest()
        {
            var sut1 = new TestClass("abc");
            var sut2 = new TestClass("xyz");
            Assert.AreNotEqual(sut1, sut2);
        }
    }
}
