using System;
using System.Collections.Generic;
using System.IO;
using Konamiman.NestorMSX.Memories;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Plugins
{
    [NestorMSXPlugin("ROM")]
    public class PlainRomPlugin
    {
        private readonly Z80Page page = 0;
        private readonly string fileName;

        public PlainRomPlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            page = pluginConfig.GetValueOrDefault<int>("page", 0);

            if(!pluginConfig.ContainsKey("file"))
                throw new InvalidOperationException("No 'file' key in config file");

            fileName = pluginConfig.GetMachineOrDataFilePath(pluginConfig.GetValue<string>("file"));
        }

        public IMemory GetMemory()
        {
            return new PlainRom(File.ReadAllBytes(fileName), page);
        }
    }
}
