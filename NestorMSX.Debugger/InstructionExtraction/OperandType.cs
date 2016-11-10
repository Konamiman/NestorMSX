namespace Konamiman.NestorMSX.Z80Debugger.InstructionExtraction
{
    public enum OperandType
    {
        ImmediateByte,
        ImmediateWord,
        RelativeJumpAddress,
        IndexRegisterOffset,
        AbsoluteMemoryAddress
    }
}
