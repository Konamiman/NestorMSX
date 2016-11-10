using System.Linq;

namespace Konamiman.NestorMSX.Z80Debugger.InstructionExtraction
{
    public class Z80Instruction
    {
        public ushort BaseMemoryAddress { get; set; }
        public string FormatString { get; set; }
        public byte[] RawBytes { get; set; }
        public Operand[] Operands { get; set; }
        public InstructionType InstructionType { get; set; }
        public bool WritesToMemory { get; set; }
        public bool WritesToPort { get; set; }
        public bool ChangesPc { get; set; }
        public bool ChangesSp { get; set; }

        public Z80Instruction Clone()
        {
            return new Z80Instruction
            {
                FormatString = FormatString,
                RawBytes = RawBytes,
                Operands = Operands.Select(o => o.Clone()).ToArray(),
                InstructionType = InstructionType,
                WritesToMemory = WritesToMemory,
                WritesToPort = WritesToPort,
                ChangesPc = ChangesPc,
                ChangesSp = ChangesSp
            };
        }
    }
}
