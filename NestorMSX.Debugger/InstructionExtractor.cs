using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Z80Debugger
{
    public partial class InstructionExtractor : IInstructionExtractor
    {
        public InstructionExtractor(IMemory memory)
        {
            this.Memory = memory;
        }

        public IMemory Memory { get; }

        public ushort NextInstructionAddress { get; set; }

        public Z80Instruction ExtractInstruction()
        {
            var nextByte = Memory[NextInstructionAddress];
            Z80Instruction instruction;

            if (nextByte == 0xCB)
                instruction = ExtractCBInstruction();
            else if (nextByte == 0xED)
                instruction = ExtractEDInstruction();
            else if (nextByte == 0xDD)
                instruction = ExtractDDInstruction();
            else if (nextByte == 0xFD)
                instruction = ExtractFDInstruction();
            else
                instruction = ExtractSingleByteInstruction(nextByte);

            foreach (var operand in instruction.Operands) {
                if(operand.Type == OperandType.ImmediateWord || operand.Type == OperandType.AbsoluteMemoryAddress) {
                    var valueLow = Memory[NextInstructionAddress + operand.OffsetWithinInstruction];
                    var valueHigh = Memory[NextInstructionAddress + operand.OffsetWithinInstruction + 1];
                    operand.Value = valueLow | (valueHigh << 8);
                    instruction.RawBytes[operand.OffsetWithinInstruction] = valueLow;
                    instruction.RawBytes[operand.OffsetWithinInstruction + 1] = valueHigh;
                }
                else {
                    operand.Value = Memory[NextInstructionAddress];
                    instruction.RawBytes[operand.OffsetWithinInstruction] = (byte)operand.Value;
                }
            }

            instruction.BaseMemoryAddress = NextInstructionAddress;
            NextInstructionAddress += (ushort)instruction.RawBytes.Length;
            return instruction;
        }

        private Z80Instruction ExtractSingleByteInstruction(byte instructionByte)
        {
            return singleByteInstructionPrototypes[instructionByte].Clone();
        }

        private Z80Instruction ExtractFDInstruction()
        {
            throw new NotImplementedException();
        }

        private Z80Instruction ExtractDDInstruction()
        {
            throw new NotImplementedException();
        }

        private Z80Instruction ExtractEDInstruction()
        {
            throw new NotImplementedException();
        }

        private Z80Instruction ExtractCBInstruction()
        {
            throw new NotImplementedException();
        }
    }
}
