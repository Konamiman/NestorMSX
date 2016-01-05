using System;
using System.Collections.Generic;
using System.Text;
using Konamiman.NestorMSX.Exceptions;
using Konamiman.NestorMSX.Misc;
using NUnit.Framework;

namespace Konamiman.NestorMSX.Tests
{
    public class ConfigurationUtilsTests
    {
        private IDictionary<string, object> config;

        [SetUp]
        public void Setup()
        {
            config = new Dictionary<string, object>
            {
                { "theLong", (long)1000 },
                { "theLongLong", long.MaxValue },
                { "theDouble", (double)1.234 },
                { "theString" ,"hello" },
                { "theArray", new object[] { 1,2,3,4 } },
                { "theNull", null },
                { "theHex1", "0x1234" },
                { "theHex2", "#ABCD" }
            };
        }

        [Test]
        public void GetValue_gets_unmodified_value_if_requested_for_same_type()
        {
            var theLong = config.GetValue<long>("theLong");
            Assert.AreEqual(1000, theLong);
            Assert.IsInstanceOf<long>(theLong);

            var theDouble = config.GetValue<double>("theDouble");
            Assert.AreEqual(1.234, theDouble);
            Assert.IsInstanceOf<double>(theDouble);

            var theString = config.GetValue<string>("theString");
            Assert.AreEqual("hello", theString);
            Assert.IsInstanceOf<string>(theString);

            var theNull = config.GetValue<object>("theNull");
            Assert.IsNull(theNull);

            var theArray = config.GetValue<object[]>("theArray");
            CollectionAssert.AreEquivalent(theArray, (object[])config["theArray"]);
            Assert.IsInstanceOf<object[]>(theArray);
        }

        [Test]
        public void GetValue_can_get_value_requested_for_parent_type()
        {
            var theString = config.GetValue<object>("theString");
            Assert.AreEqual("hello", theString);
            Assert.IsInstanceOf<string>(theString);
        }

        [Test]
        public void GetValue_throws_exception_if_key_does_not_exist()
        {
            Assert.Throws<ConfigurationException>(() => config.GetValue<object>("XXXX"));
        }

        [Test]
        public void GetValue_can_convert_value_if_convert_method_exists()
        {
            var theLongAsInt = config.GetValue<int>("theLong");
            Assert.AreEqual(1000, theLongAsInt);
            Assert.IsInstanceOf<int>(theLongAsInt);

            var theDoubleAsDecimal = config.GetValue<decimal>("theDouble");
            Assert.AreEqual((decimal)1.234, theDoubleAsDecimal);
            Assert.IsInstanceOf<decimal>(theDoubleAsDecimal);

            var theLongAsString = config.GetValue<string>("theLong");
            Assert.AreEqual("1000", theLongAsString);
            Assert.IsInstanceOf<string>(theLongAsString);
        }

        [Test]
        public void GetValue_throws_exception_if_no_conversion_method_exists()
        {
            Assert.Throws<ConfigurationException>(() => config.GetValue<StringBuilder>("theLong"));
        }

        [Test]
        public void GetValue_can_convert_arrays()
        {
            var theArrayAsInts = config.GetValue<int[]>("theArray");
            CollectionAssert.AreEquivalent(theArrayAsInts, new[] {1,2,3,4});
            Assert.IsInstanceOf<int[]>(theArrayAsInts);
        }

        [Test]
        public void GetValueOrDefault_gets_value_if_key_exists()
        {
            var actual = config.GetValueOrDefault<long>("theLong");
            Assert.AreEqual((long)1000, actual);
        }

        [Test]
        public void GetValueOrDefault_gets_specified_value_if_key_does_not_exist()
        {
            var actual = config.GetValueOrDefault<long>("XXXXX");
            Assert.AreEqual((long)0, actual);

            actual = config.GetValueOrDefault<long>("XXXXX", 1234);
            Assert.AreEqual((long)1234, actual);
        }

        [Test]
        public void GetValueOrDefault_throws_exception_if_conversion_fails()
        {
            var exception = Assert.Throws<ConfigurationException>(() => config.GetValueOrDefault<int>("theLongLong"));
            Assert.IsInstanceOf<OverflowException>(exception.InnerException);
        }

        [Test]
        public void Can_convert_hexadecimal_strings()
        {
            var value = config.GetValue<int>("theHex1");
            Assert.AreEqual(0x1234, value);

            value = config.GetValue<int>("theHex2");
            Assert.AreEqual(0xABCD, value);
        }
    }
}
