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
            if(pluginConfig.ContainsKey("page"))
                page = (int)(long)pluginConfig["key"];

            var pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var sharedPath = pluginPath + "..\\Shared";

            if(!pluginConfig.ContainsKey("file"))
                throw new InvalidOperationException("No 'file' key in config file");

            fileName = Path.Combine(pluginPath, (string)pluginConfig["file"]);
            if(!File.Exists(fileName))
            {
                fileName = Path.Combine(sharedPath, (string)pluginConfig["file"]);
                if(!File.Exists(fileName))
                {
                    throw new InvalidOperationException($"File not found: {pluginConfig["file"]}");
                }
            }
        }

        public IMemory GetMemory()
        {
            return new PlainRom(File.ReadAllBytes(fileName), page);
        }
    }
}
