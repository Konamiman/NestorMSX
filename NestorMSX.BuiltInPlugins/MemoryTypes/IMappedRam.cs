using Konamiman.NestorMSX.Misc;

namespace Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes
{
    public interface IMappedRam : IBankedMemory
    {
        void WriteToSegmentSelectionRegister(Z80Page page, byte segmentNumber);
    }
}
