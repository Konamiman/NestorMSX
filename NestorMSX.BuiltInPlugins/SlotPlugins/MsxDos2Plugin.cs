using System.Collections.Generic;
using System.IO;
using Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Plugins
{
    [NestorMSXPlugin("Deviceless MSX-DOS 2")]
    public class DevicelessMsxDos2RomPlugin
    {
        private readonly string kernelFilePath;

        public DevicelessMsxDos2RomPlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            var kernelFileName = pluginConfig.GetValueOrDefault("kernelFile", "MsxDos2Kernel.rom");
            kernelFilePath = pluginConfig.GetMachineOrDataFilePath(kernelFileName);
        }

        public IMemory GetMemory()
        {
            return new MsxDos2Rom(File.ReadAllBytes(kernelFilePath));
        }
    }
}
