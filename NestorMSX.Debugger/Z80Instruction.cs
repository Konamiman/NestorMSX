namespace Konamiman.NestorMSX.Z80Debugger
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
    }
}
