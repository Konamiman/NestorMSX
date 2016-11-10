#if false
namespace Konamiman.NestorMSX.Z80Debugger
{
    public partial class InstructionExtractor
    {
        private readonly Z80Instruction[] sinlgeByteInstructionPrototypes =
        {
            new Z80Instruction
            {
                FormatString = "nop",
                RawBytes = new byte[] {0x00},
                Operands = new Operand[0],
                InstructionType = InstructionType.Standard,
                ChangesSp = false,
                ChangesPc = false,
                WritesToMemory = false,
                WritesToPort = false
            },
            new Z80Instruction
            {
                FormatString = "ld bc,{0}",
                RawBytes = new byte[] {0x01, 0x00, 0x00},
                Operands = new[]
                {
                    new Operand
                    {
                        Type = OperandType.ImmediateWord,
                        OffsetWithinInstruction = 1
                    }
                },
                InstructionType = InstructionType.Standard,
                ChangesSp = false,
                ChangesPc = false,
                WritesToMemory = false,
                WritesToPort = false
            },
        };
    }
}
#endif