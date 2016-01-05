using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes
{
    public interface IMappedRam : IMemory
    {
        void WriteToSegmentSelectionRegister(Z80Page page, byte segmentNumber);
    }
}
