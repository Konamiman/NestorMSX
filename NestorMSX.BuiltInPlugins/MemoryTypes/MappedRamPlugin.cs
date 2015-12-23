using System.Collections.Generic;
using System.IO;
using Konamiman.NestorMSX.Hardware;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes
{
    [NestorMSXPlugin("Mapped RAM")]
    public class MappedRamPlugin
    {
        private readonly int sizeInSegments;

        public MappedRamPlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            var sizeInKb = pluginConfig.GetValue<int>("sizeInKb");
            sizeInSegments = sizeInKb/16;
        }

        public IMemory GetMemory()
        {
            return new MappedRam(sizeInSegments);
        }
    }
}
