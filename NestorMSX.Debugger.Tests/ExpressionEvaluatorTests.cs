using Konamiman.NestorMSX.Z80Debugger.Console.ExpressionEvaluator;
using NUnit.Framework;

namespace NestorMSX.Debugger.Tests
{
    public class ExpressionEvaluatorTests
    {
        private IExpressionEvaluator Sut { get; set; }

        [SetUp]
        public void Setup()
        {
            Sut = new EvaluantExpressionEvaluatorWrapper();
        }

        [Test]
        public void EvaluateHexNumbers()
        {
            Assert.AreEqual(0x12AB, Sut.Evaluate("0x12AB"));
            Assert.AreEqual(0x12AB, Sut.Evaluate("&H12AB"));
            Assert.AreEqual(0x12AB, Sut.Evaluate("&h12AB"));
            Assert.AreEqual(0x12AB, Sut.Evaluate("#12AB"));
            Assert.AreEqual(0x12AB, Sut.Evaluate("12ABH"));
            Assert.AreEqual(0x12AB, Sut.Evaluate("12ABh"));
        }

        [Test]
        public void EvaluateBinNumbers()
        {
            Assert.AreEqual(170, Sut.Evaluate("&b10101010"));
            Assert.AreEqual(170, Sut.Evaluate("&B10101010"));
            Assert.AreEqual(170, Sut.Evaluate("%10101010"));
            Assert.AreEqual(170, Sut.Evaluate("10101010B"));
            Assert.AreEqual(170, Sut.Evaluate("10101010b"));
        }

        [Test]
        public void EvaluateHexAndBinNumbersInsideStringLiterals()
        {
            Assert.AreEqual(@"0x12AB", Sut.Evaluate(@"""0x12AB"""));
            Assert.AreEqual(@"&H12AB", Sut.Evaluate(@"""&H12AB"""));
            Assert.AreEqual(@"&h12AB", Sut.Evaluate(@"""&h12AB"""));
            Assert.AreEqual(@"#12AB", Sut.Evaluate(@"""#12AB"""));
            Assert.AreEqual(@"12ABH", Sut.Evaluate(@"""12ABH"""));
            Assert.AreEqual(@"12ABh", Sut.Evaluate(@"""12ABh"""));

            Assert.AreEqual("&b10101010", Sut.Evaluate(@"""&b10101010"""));
            Assert.AreEqual("&B10101010", Sut.Evaluate(@"""&B10101010"""));
            Assert.AreEqual("%10101010", Sut.Evaluate(@"""%10101010"""));
            Assert.AreEqual("10101010B", Sut.Evaluate(@"""10101010B"""));
            Assert.AreEqual("10101010b", Sut.Evaluate(@"""10101010b"""));
        }

        [Test]
        public void EvaluateFunctionsParamsAndStringLiterals()
        {
            var expression = @"foo(bar,""Hello, \""world\"" 'again'"")";
            Sut.EvaluateName += 
                (sender, args) =>
                args.Result = args.Name == "bar" ? "I say: " : null;
            Sut.EvaluateFunction +=
                (sender, args) =>
                    args.Result = args.Name == "foo" ? (string) args.Parameters[0] + (string) args.Parameters[1] : null;

            var result = Sut.Evaluate(expression);

            Assert.AreEqual(@"I say: Hello, ""world"" 'again'", result);
        }
    }
}
