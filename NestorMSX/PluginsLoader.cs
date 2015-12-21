using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Konamiman.NestorMSX.Misc;

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

        /// <summary>
        /// Parses a JSON string into an object. Arrays are converted to object[],
        /// objects are converted to Dictionary&lt;string, object&gt;.
        /// </summary>
        public void LoadPlugins()
        {
            var configFileText = File.ReadAllText("plugins.config");
            IDictionary<string, object> allConfigValues;

            try
            {
                allConfigValues = JsonParser.Parse(configFileText) as IDictionary<string, object>;
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("Error when parsing plugins.config file: " + ex.Message);
            }

            if (allConfigValues == null)
                ThrowNotValidJson();

            var commonConfigValues = allConfigValues["sharedPluginsConfig"] as IDictionary<string, object>;
            var plugins = allConfigValues["plugins"] as IDictionary<string, object>;
            if(commonConfigValues == null || plugins == null)
                ThrowNotValidJson();

            var validPluginConfigs =
                plugins
                    .Where(p => p.Value is IDictionary<string, object>)
                    .ToDictionary(p => p.Key, p => (IDictionary<string, object>)p.Value);

            var activePluginConfigs =
                validPluginConfigs.Keys
                    .Where(k => !validPluginConfigs[k].ContainsKey("active") || (validPluginConfigs[k]["active"] as bool?) == true)
                    .ToDictionary(k => k, k => validPluginConfigs[k]);

            var namesOfPluginsConfiguredAsActive = activePluginConfigs.Keys;

            var pluginsDirectory = new DirectoryInfo("plugins");
            
            Func<DirectoryInfo, string[]> getDllFilenames =
                dir => dir.GetFiles("*.dll").Select(f => f.FullName).ToArray();

            var pluginAssemblyFileNames =
                getDllFilenames(pluginsDirectory)
                .Union(pluginsDirectory.GetDirectories().SelectMany(d => getDllFilenames(d)))
                .ToArray();

            var allAvailablePluginsByName =
                pluginAssemblyFileNames.SelectMany(n => GetPluginsInAssembly(n))
                    .ToDictionary(x => x.Key, x => x.Value);

            foreach(var pluginName in namesOfPluginsConfiguredAsActive)
            {
                try
                {
                    var pluginConfig = validPluginConfigs[pluginName];
                    foreach(var sharedConfigKey in commonConfigValues.Keys)
                        if(!pluginConfig.ContainsKey(sharedConfigKey))
                            pluginConfig[sharedConfigKey] = commonConfigValues[sharedConfigKey];

                    var pluginInstance = LoadPlugin(pluginName, allAvailablePluginsByName, pluginConfig);
                }
                catch(Exception ex)
                {
                    tell("Could not load plugin '{0}': {1}", new[] {pluginName, ex.Message});
                }
            }
        }

        private static void ThrowNotValidJson()
        {
            throw new InvalidOperationException("plugins.config is not a valid json file");
        }

        private IDictionary<string, Type> GetPluginsInAssembly(string assemblyFileName)
        {
            var assembly = Assembly.LoadFile(assemblyFileName);

            Func<Type, NestorMSXPluginAttribute> getPluginAttribute =
                t => (NestorMSXPluginAttribute)t.GetCustomAttributes(typeof(NestorMSXPluginAttribute), false).SingleOrDefault();

            var pluginTypes =
                assembly.GetTypes()
                    .Where(t => getPluginAttribute(t) != null)
                    .ToArray();

            var pluginTypesByName = pluginTypes.ToDictionary(t => t.FullName);

            foreach(var type in pluginTypes)
            {
                var pluginAttribute = getPluginAttribute(type);
                if(pluginAttribute.Name != null)
                    pluginTypesByName[pluginAttribute.Name] = type;
            }

            return pluginTypesByName;
        }

        private object LoadPlugin(string pluginName, IDictionary<string, Type> pluginsByName, IDictionary<string, object> pluginConfig)
        {
            if(!pluginsByName.ContainsKey(pluginName))
                throw new InvalidOperationException($"No plugin with name '{pluginName}' found");

            var type = pluginsByName[pluginName];

            var constructor = type
                .GetConstructor(new[] {typeof(PluginContext), typeof(IDictionary<string, object>)});

            if(constructor == null)
                throw new InvalidOperationException("No suitable constructor found for " + type.FullName);

            return Activator.CreateInstance(type, new object[] {context, pluginConfig});
        }
    }
}
