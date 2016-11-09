using System;

namespace Konamiman.NestorMSX.Z80Debugger
{
    public partial class InstructionExtractor : IInstructionExtractor
    {
        public InstructionExtractor(Func<int, byte> getByteAtAddress)
        {
            this.GetByteAtAddress = getByteAtAddress;
        }

        public InstructionExtractor(byte[] memoryBytes)
            : this(address => memoryBytes[address])
        {
        }

        public Func<int, byte> GetByteAtAddress { get; }

        public ushort NextInstructionAddress { get; set; }

        public Z80Instruction ExtractInstruction()
        {
            var opcode = GetByteAtAddress(NextInstructionAddress);
            Z80Instruction instruction;

            if(opcode == 0xCB)
                instruction = ExtractCBInstruction(GetByteAtAddress(NextInstructionAddress + 1));
            else if(opcode == 0xED)
                instruction = ExtractEDInstruction(GetByteAtAddress(NextInstructionAddress + 1));
            else if (opcode == 0xDD || opcode == 0xFD) {
                var nextByte = GetByteAtAddress(NextInstructionAddress + 1);
                instruction = nextByte == 0xCB ? 
                    ExtractDDCBorFDCBInstruction(opcode) :
                    ExtractDDorFDInstruction(opcode, nextByte);
            }
            else
                instruction = ExtractSingleByteInstruction(opcode);

            if(instruction.InstructionType != InstructionType.Undefined) {
                foreach (var operand in instruction.Operands) {
                    if(operand.Type == OperandType.ImmediateWord || operand.Type == OperandType.AbsoluteMemoryAddress) {
                        var valueLow = GetByteAtAddress(NextInstructionAddress + operand.OffsetWithinInstruction);
                        var valueHigh = GetByteAtAddress(NextInstructionAddress + operand.OffsetWithinInstruction + 1);
                        operand.Value = valueLow | (valueHigh << 8);
                        instruction.RawBytes[operand.OffsetWithinInstruction] = valueLow;
                        instruction.RawBytes[operand.OffsetWithinInstruction + 1] = valueHigh;
                    }
                    else {
                        operand.Value = GetByteAtAddress(NextInstructionAddress + operand.OffsetWithinInstruction);
                        instruction.RawBytes[operand.OffsetWithinInstruction] = (byte)operand.Value;
                    }
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

        private Z80Instruction ExtractCBInstruction(byte nextByte)
        {
            return cbInstructionPrototypes[nextByte].Clone();
        }

        private Z80Instruction ExtractEDInstruction(byte nextByte)
        {
            Z80Instruction instruction = null;

            if (nextByte >= 0x40 && nextByte < 0x80)
                instruction = EDInstructionPrototypes[nextByte - 0x40];
            
            if (nextByte >= 0xA0 && nextByte < 0xBC)
                instruction = EDBlockInstructionPrototypes[nextByte - 0xA0];

            if(instruction != null)
                return instruction.Clone();

            return new Z80Instruction
            {
                FormatString = "db {0},{1}",
                RawBytes = new byte[] {0xED, nextByte},
                Operands = new[]
                {
                    new Operand
                    {
                        Type = OperandType.ImmediateByte,
                        OffsetWithinInstruction = 0,
                        Value = 0xED
                    },
                    new Operand
                    {
                        Type = OperandType.ImmediateByte,
                        OffsetWithinInstruction = 1,
                        Value = nextByte
                    }
                },
                InstructionType = InstructionType.Undefined,
                ChangesSp = false,
                ChangesPc = false,
                WritesToMemory = false,
                WritesToPort = false
            };
        }

        private Z80Instruction ExtractDDorFDInstruction(byte prefix, byte nextByte)
        {
            var instructions = prefix == 0xDD ? DDInstructionPrototypes : FDInstructionPrototypes;
            if(instructions.ContainsKey(nextByte))
                return instructions[nextByte].Clone();

            return new Z80Instruction
            {
                FormatString = "db {0}",
                RawBytes = new byte[] {prefix},
                Operands = new[]
                {
                    new Operand
                    {
                        Type = OperandType.ImmediateByte,
                        OffsetWithinInstruction = 0,
                        Value = prefix
                    }
                },
                InstructionType = InstructionType.Undefined,
                ChangesSp = false,
                ChangesPc = false,
                WritesToMemory = false,
                WritesToPort = false
            };
        }

        private Z80Instruction ExtractDDCBorFDCBInstruction(byte prefix)
        {
            var instructions = prefix == 0xDD ? DDCBInstructionPrototypes : FDCBInstructionPrototypes;
            var opcode = GetByteAtAddress(NextInstructionAddress + 3);
            return instructions[opcode].Clone();
        }
    }
}
