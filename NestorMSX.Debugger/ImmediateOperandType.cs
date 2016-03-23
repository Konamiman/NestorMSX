namespace Konamiman.NestorMSX.Debugger
{
    public enum ImmediateOperandType
    {
        None,
        ByteValue,
        WordValue,
        MemoryAddress,
        PortAddress,
        RelativeJumpAddress,
        IndexRegisterOffset,
        BitNumber
    }
}
