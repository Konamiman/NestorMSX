using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Z80Debugger
{
    public interface IInstructionExtractor
    {
        IMemory Memory { get; set; }
        ushort NextInstructionAddress { get; set; }
        Z80Instruction ExtractInstruction();
    }
}
