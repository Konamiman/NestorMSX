using System.Linq;
using Konamiman.NestorMSX.Z80Debugger;
using Konamiman.NestorMSX.Z80Debugger.InstructionExtraction;
using NUnit.Framework;

namespace NestorMSX.Debugger.Tests
{
    public class InstructionExtractorTests
    {
        private InstructionExtractor GetExtractor(params byte[] data)
        {
            return new InstructionExtractor(data);
        }

        [Test]
        public void ExtractsLdBcNn()
        {
            var sut = GetExtractor(0x01, 0x33, 0x22);
            var instruction = sut.ExtractInstruction();

            Assert.AreEqual(3, sut.NextInstructionAddress);
            Assert.AreEqual("ld bc,{0}", instruction.FormatString);
            CollectionAssert.AreEqual(new byte[] {0x01, 0x33, 0x22}, instruction.RawBytes);
            Assert.AreEqual(0x2233, instruction.Operands.Single().Value);
        }

        [Test]
        public void ExtractsRlcC()
        {
            var sut = GetExtractor(0xCB, 0x01);
            var instruction = sut.ExtractInstruction();

            Assert.AreEqual(2, sut.NextInstructionAddress);
            Assert.AreEqual("rlc c", instruction.FormatString);
            CollectionAssert.AreEqual(new byte[] {0xCB, 0x01}, instruction.RawBytes);
        }

        [Test]
        [TestCase(0x40, "in b,(c)")]
        [TestCase(0x7E, "im 2")]
        [TestCase(0xA0, "ldi")]
        [TestCase(0xBB, "otdr")]
        public void ExtractsEdInstruction(int secondByte, string text)
        {
            var sut = GetExtractor(0xED, (byte)secondByte);
            var instruction = sut.ExtractInstruction();

            Assert.AreEqual(2, sut.NextInstructionAddress);
            Assert.AreEqual(text, instruction.FormatString);
            CollectionAssert.AreEqual(new byte[] {0xED, (byte)secondByte}, instruction.RawBytes);
        }

        [Test]
        [TestCase(0x77)]
        [TestCase(0x7F)]
        [TestCase(0xA4)]
        [TestCase(0xA7)]
        [TestCase(0xAC)]
        [TestCase(0xAF)]
        [TestCase(0xB4)]
        [TestCase(0xB7)]
        [TestCase(0xBC)]
        [TestCase(0xBF)]
        public void ExtractsUndefinedEdsAsUndefinedInstructions(int secondByte)
        {
            var sut = GetExtractor(0xED, (byte)secondByte);
            var instruction = sut.ExtractInstruction();

            Assert.AreEqual(2, sut.NextInstructionAddress);
            Assert.AreEqual("db {0},{1}", instruction.FormatString);
            CollectionAssert.AreEqual(new byte[] {0xED, (byte)secondByte}, instruction.RawBytes);
            Assert.AreEqual(InstructionType.Undefined, instruction.InstructionType);
            Assert.AreEqual(0xED, instruction.Operands[0].Value);
            Assert.AreEqual((byte)secondByte, instruction.Operands[1].Value);
        }

        [Test]
        [TestCase(0xDD, "ix")]
        [TestCase(0xFD, "iy")]
        public void ExtractsAddIxyBc(int prefix, string registerName)
        {
            var sut = GetExtractor((byte)prefix, 0x09);
            var instruction = sut.ExtractInstruction();

            Assert.AreEqual(2, sut.NextInstructionAddress);
            Assert.AreEqual($"add {registerName},bc", instruction.FormatString);
            CollectionAssert.AreEqual(new byte[] {(byte)prefix, 0x09}, instruction.RawBytes);
        }

        [Test]
        [TestCase(0xDD, 0xDD)]
        [TestCase(0xDD, 0xFD)]
        [TestCase(0xDD, 0xED)]
        [TestCase(0xDD, 0x00)]
        [TestCase(0xFD, 0xDD)]
        [TestCase(0xFD, 0xFD)]
        [TestCase(0xFD, 0xED)]
        [TestCase(0xFD, 0x00)]
        public void ExtractsDdsFdsFollowedByUnrecognizedsAsUndefinedInstructions(int prefix, int secondByte)
        {
            var sut = GetExtractor((byte)prefix, (byte)secondByte);
            var instruction = sut.ExtractInstruction();

            Assert.AreEqual(1, sut.NextInstructionAddress);
            Assert.AreEqual("db {0}", instruction.FormatString);
            CollectionAssert.AreEqual(new byte[] {(byte)prefix}, instruction.RawBytes);
            Assert.AreEqual(InstructionType.Undefined, instruction.InstructionType);
            Assert.AreEqual((byte)prefix, instruction.Operands[0].Value);
        }

        [Test]
        [TestCase(0xDD, "ix")]
        [TestCase(0xFD, "iy")]
        public void ExtractsRlIxyN(int prefix, string registerName)
        {
            var sut = GetExtractor((byte)prefix, 0xCB, 0x34, 0x16);
            var instruction = sut.ExtractInstruction();

            Assert.AreEqual(4, sut.NextInstructionAddress);
            Assert.AreEqual($"rl ({registerName}+{{0}})", instruction.FormatString);
            CollectionAssert.AreEqual(new byte[] {(byte)prefix, 0xCB, 0x34, 0x16}, instruction.RawBytes);
            Assert.AreEqual(0x34, instruction.Operands[0].Value);
        }
    }
}
