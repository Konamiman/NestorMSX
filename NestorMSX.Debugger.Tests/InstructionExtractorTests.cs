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
    }
}
