using Konamiman.NestorMSX.Misc;

namespace Konamiman.NestorMSX.Memories
{
    public interface IMappedRam : IBankedMemory
    {
        void WriteToSegmentSelectionRegister(Z80Page page, byte segmentNumber);
    }
}
