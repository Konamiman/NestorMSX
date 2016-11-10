namespace Konamiman.NestorMSX.Z80Debugger.InstructionExtraction
{
    public class Operand
    {
        public int Value { get; set; }
        public OperandType Type { get; set; }
        public int OffsetWithinInstruction { get; set; }

        public Operand Clone()
        {
            return new Operand
            {
                Type = Type,
                OffsetWithinInstruction = OffsetWithinInstruction,
                Value = Value
            };
        }
    }
}
