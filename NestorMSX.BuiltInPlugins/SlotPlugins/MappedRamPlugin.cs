using System.Collections.Generic;
using Konamiman.NestorMSX.Memories;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Plugins
{
    [NestorMSXPlugin("Mapped RAM")]
    public class MappedRamPlugin
    {
        private readonly int sizeInSegments;
        private MappedRam mappedRam;

        public MappedRamPlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            var sizeInKb = pluginConfig.GetValueOrDefault<int>("sizeInKb", 4096);
            sizeInSegments = sizeInKb/16;

            this.mappedRam = new MappedRam(sizeInSegments);

            context.Cpu.MemoryAccess += Cpu_MemoryAccess;
        }

        private void Cpu_MemoryAccess(object sender, MemoryAccessEventArgs e)
        {
            if(e.EventType != MemoryAccessEventType.BeforePortWrite)
                return;

            if(e.Address < 0xFC || e.Address > 0xFF)
                return;

            var page = e.Address - 0xFC;
            mappedRam.WriteToSegmentSelectionRegister(page, e.Value);
        }

        public IMemory GetMemory()
        {
            return mappedRam;
        }
    }
}
