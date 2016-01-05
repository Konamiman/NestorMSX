using System.Collections.Generic;
using Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Plugins
{
    [NestorMSXPlugin("RAM")]
    public class PlainRamPlugin
    {
        private readonly int baseAddress;
        private readonly int size;

        public PlainRamPlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            baseAddress = pluginConfig.GetValueOrDefault<int>("baseAddress", -1);
            size = pluginConfig.GetValueOrDefault<int>("size", -1);

            if(baseAddress == -1 && size != -1)
            {
                baseAddress = ushort.MaxValue - size;
            }
            else if(baseAddress != -1 && size == -1)
            {
                size = ushort.MaxValue - baseAddress;
            }
            else if(baseAddress == -1 && size == -1)
            {
                baseAddress = 0;
                size = ushort.MaxValue;
            }
        }

        public IMemory GetMemory()
        {
            return new PlainRam(baseAddress, size);
        }
    }
}
