using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Debugger
{
    public interface IInstructionDecoder
    {
        IMemory InstructionsSource { get; set; }
        ushort NextInstructionAddress { get; set; }
        DecodedInstruction GetNextInstruction();
    }
}
