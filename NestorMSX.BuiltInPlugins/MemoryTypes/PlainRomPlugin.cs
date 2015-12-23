using System;
using System.Collections.Generic;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;
using System.Reflection;
using System.IO;
using Konamiman.NestorMSX.Hardware;

namespace Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes
{
    [NestorMSXPlugin("ROM")]
    public class PlainRomPlugin
    {
        private readonly PluginContext context;
        private readonly IDictionary<string, object> pluginConfig;
        private readonly Z80Page page = 0;
        private readonly string fileName;

        public PlainRomPlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            page = pluginConfig.GetValueOrDefault<int>("page", 0);

            if(!pluginConfig.ContainsKey("file"))
                throw new InvalidOperationException("No 'file' key in config file");

            fileName = pluginConfig.GetPluginFilePath(pluginConfig.GetValue<string>("file"));
        }

        public IMemory GetMemory()
        {
            return new PlainRom(File.ReadAllBytes(fileName), page);
        }
    }
}
