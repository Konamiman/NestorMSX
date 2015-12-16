using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Konamiman.NestorMSX
{
    class PluginsLoader
    {
        private readonly PluginContext context;
        private Action<string, object[]> tell;

        public PluginsLoader(PluginContext context, Action<string, object[]> tell)
        {
            this.context = context;
            this.tell = tell;
        }

        public void LoadPlugins()
        {
            var configFileText = File.ReadAllText("plugins.config");
            var allConfigValues = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(configFileText);
            var commonConfigValues = allConfigValues["sharedPluginsConfig"];

            var activePluginNames =
                allConfigValues.Keys
                .Where(k => k != "sharedPluginsConfig")
                .Where(k => !allConfigValues[k].ContainsKey("active") || allConfigValues[k]["active"].Equals("true", StringComparison.InvariantCultureIgnoreCase))
                .ToArray();

            var pluginsDirectory = new DirectoryInfo("plugins");
            
            Func<DirectoryInfo, string[]> getDllFilenames =
                dir => dir.GetFiles("*.dll").Select(f => f.FullName).ToArray();

            var pluginAssemblyFileNames =
                getDllFilenames(pluginsDirectory)
                .Union(pluginsDirectory.GetDirectories().SelectMany(d => getDllFilenames(d)))
                .ToArray();

            var allPluginsByName =
                pluginAssemblyFileNames.SelectMany(n => GetPluginsInAssembly(n))
                    .ToDictionary(x => x.Key, x => x.Value);

            foreach(var pluginName in activePluginNames)
            {
                try
                {
                    var pluginConfig = allConfigValues[pluginName];
                    foreach(var sharedConfigKey in commonConfigValues.Keys)
                        if(!pluginConfig.ContainsKey(sharedConfigKey))
                            pluginConfig[sharedConfigKey] = commonConfigValues[sharedConfigKey];

                    LoadPlugin(pluginName, allPluginsByName, pluginConfig);
                }
                catch(Exception ex)
                {
                    tell("Could not load plugin '{0}': {1}", new[] {pluginName, ex.Message});
                }
            }
        }

        private IDictionary<string, Type> GetPluginsInAssembly(string assemblyFileName)
        {
            var assembly = Assembly.LoadFile(assemblyFileName);

            var pluginTypes =
                assembly.GetTypes()
                    .Where(t => t.GetCustomAttributes(typeof(NestorMSXPluginAttribute), false).Length > 0)
                    .ToArray();

            var pluginTypesByName =
                pluginTypes
                    .ToDictionary(t =>
                        ((NestorMSXPluginAttribute)t.GetCustomAttributes(typeof(NestorMSXPluginAttribute), false)
                            .Single())
                            .Name ?? t.FullName);

            return pluginTypesByName;
        }

        private void LoadPlugin(string pluginName, IDictionary<string, Type> pluginsByName, IDictionary<string, string> pluginConfig)
        {
            if(!pluginsByName.ContainsKey(pluginName))
                throw new InvalidOperationException($"No plugin with name '{pluginName}' found");

            var type = pluginsByName[pluginName];

            var constructor = type
                .GetConstructor(new[] {typeof(PluginContext), typeof(IDictionary<string, string>)});

            if(constructor == null)
                throw new InvalidOperationException("No suitable constructor found for " + type.FullName);

            Activator.CreateInstance(type, new object[] {context, pluginConfig});
        }
    }
}
