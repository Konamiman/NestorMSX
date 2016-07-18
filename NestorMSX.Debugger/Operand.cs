namespace Konamiman.NestorMSX.Z80Debugger
{
    public class Operand
    {
        public int Value { get; set; }
        public OperandType Type { get; set; }
        public int OffsetWithinInstruction { get; set; }
    }
}
