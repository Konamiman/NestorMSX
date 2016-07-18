namespace Konamiman.NestorMSX.Z80Debugger
{
    public class Z80Instruction
    {
        public ushort BaseMemoryAddress { get; set; }
        public string FormatString { get; set; }
        public byte[] RawBytes { get; set; }
        public Operand[] Operands { get; set; }
        public bool IsUndocumentedInstruction { get; set; }
        public bool IsUndefinedInstruction { get; set; }
    }
}
