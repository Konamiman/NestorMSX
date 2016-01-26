using System;
using System.Collections.Generic;
using System.IO;
using Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Plugins
{
    [NestorMSXPlugin("Ascii16")]
    public class Ascii16RomPlugin
    {
        private readonly string fileName;

        public Ascii16RomPlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            if(!pluginConfig.ContainsKey("file"))
                throw new InvalidOperationException("No 'file' key in config file");

            fileName = pluginConfig.GetMachineOrDataFilePath(pluginConfig.GetValue<string>("file"));
        }

        public IMemory GetMemory()
        {
            return new Ascii16Rom_old(File.ReadAllBytes(fileName));
        }
    }
}
