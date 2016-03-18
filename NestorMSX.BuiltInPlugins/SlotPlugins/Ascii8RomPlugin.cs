using System;
using System.Collections.Generic;
using System.IO;
using Konamiman.NestorMSX.Memories;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Plugins
{
    [NestorMSXPlugin("Ascii8")]
    public class Ascii8RomPlugin
    {
        private readonly string fileName;

        public Ascii8RomPlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            if(!pluginConfig.ContainsKey("file"))
                throw new InvalidOperationException("No 'file' key in config file");

            fileName = pluginConfig.GetMachineOrDataFilePath(pluginConfig.GetValue<string>("file"));
        }

        public IMemory GetMemory()
        {
            return new Ascii8Rom(File.ReadAllBytes(fileName));
        }
    }
}
