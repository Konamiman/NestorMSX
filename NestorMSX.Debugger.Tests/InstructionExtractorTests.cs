using System.Linq;
using Konamiman.NestorMSX.Z80Debugger;
using Konamiman.Z80dotNet;
using NUnit.Framework;

namespace NestorMSX.Debugger.Tests
{
    public class InstructionExtractorTests
    {
        [Test]
        public void ExtractsLdBcNn()
        {
            var memory = new PlainMemory(65536);
            memory.SetContents(0, new byte[] {0x01, 0x33, 0x22});
            var sut = new InstructionExtractor(memory);
            var instruction = sut.ExtractInstruction();

            Assert.AreEqual(3, sut.NextInstructionAddress);
            Assert.AreEqual("ld bc,{0}", instruction.FormatString);
            CollectionAssert.AreEqual(new byte[] {0x01, 0x33, 0x22}, instruction.RawBytes);
            Assert.AreEqual(0x2233, instruction.Operands.Single().Value);
        }

        [Test]
        public void ExtractsRlcC()
        {
            var memory = new PlainMemory(65536);
            memory.SetContents(0, new byte[] {0xCB, 0x01});
            var sut = new InstructionExtractor(memory);
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
            var memory = new PlainMemory(65536);
            memory.SetContents(0, new byte[] {0xED, (byte)secondByte});
            var sut = new InstructionExtractor(memory);
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
            var memory = new PlainMemory(65536);
            memory.SetContents(0, new byte[] {0xED, (byte)secondByte});
            var sut = new InstructionExtractor(memory);
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
            var memory = new PlainMemory(65536);
            memory.SetContents(0, new byte[] {(byte)prefix, 0x09});
            var sut = new InstructionExtractor(memory);
            var instruction = sut.ExtractInstruction();

            Assert.AreEqual(2, sut.NextInstructionAddress);
            Assert.AreEqual($"add {registerName},bc", instruction.FormatString);
            CollectionAssert.AreEqual(new byte[] {(byte)prefix, 0x09}, instruction.RawBytes);
        }

        [Test]
        [TestCase(0xDD, "ix")]
        [TestCase(0xFD, "iy")]
        public void ExtractsMirroredLdDeNn(int prefix, string registerName)
        {
            var memory = new PlainMemory(65536);
            memory.SetContents(0, new byte[] {(byte)prefix, 0x11, 0x34, 0x12});
            var sut = new InstructionExtractor(memory);
            var instruction = sut.ExtractInstruction();

            Assert.AreEqual(4, sut.NextInstructionAddress);
            Assert.AreEqual("ld de,{0}", instruction.FormatString);
            Assert.AreEqual(InstructionType.SafeMirror, instruction.InstructionType);
            CollectionAssert.AreEqual(new byte[] {(byte)prefix, 0x11, 0x34, 0x12}, instruction.RawBytes);
            Assert.AreEqual(0x1234, instruction.Operands.Single().Value);
        }
    }
}
