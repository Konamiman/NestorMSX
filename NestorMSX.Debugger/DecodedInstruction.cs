namespace Konamiman.NestorMSX.Debugger
{
    public class DecodedInstruction
    {
        public ushort BaseMemoryAddress { get; set; }
        public string DisplayTemplate { get; set; }
        public int ImmediateOperand { get; set; }
        public ImmediateOperandType ImmediateOperandType { get; set; }
        public byte[] InstructionBytes { get; set; }
        public bool IsUndocumentedInstruction { get; set; }
        public bool IsUndefinedInstruction { get; set; }
        public bool ModifiesRegisters { get; set; }
        public bool WritesToMemory { get; set; }
        public bool WritesToPort { get; set; }
    }
}
