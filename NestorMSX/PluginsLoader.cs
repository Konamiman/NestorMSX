using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX
{
    class PluginsLoader
    {
        private static IDictionary<string, Type> pluginTypes;
        private object[] loadedPlugins;

        private readonly PluginContext context;
        private Action<string, object[]> tell;

        public PluginsLoader(PluginContext context, Action<string, object[]> tell)
        {
            this.context = context;
            this.tell = tell;
        }

        private IDictionary<string, Type> GetPluginTypes()
        {
            if(pluginTypes != null)
                return pluginTypes;

            var pluginsDirectory = new DirectoryInfo("plugins");

            Func<DirectoryInfo, string[]> getDllFilenames =
                dir => dir.GetFiles("*.dll").Select(f => f.FullName).ToArray();

            var pluginAssemblyFileNames =
                getDllFilenames(pluginsDirectory)
                .Union(pluginsDirectory.GetDirectories().SelectMany(d => getDllFilenames(d)))
                .ToArray();

            pluginTypes =
                pluginAssemblyFileNames.SelectMany(n => GetPluginsInAssembly(n))
                    .ToDictionary(x => x.Key, x => x.Value);

            return pluginTypes;
        }

        /// <summary>
        /// Gets an instance of a plugin to be inserted in a slot.
        /// </summary>
        /// <param name="pluginName"></param>
        /// <param name="pluginConfig"></param>
        /// <returns></returns>
        public object GetPluginInstanceForSlot(string pluginName, IDictionary<string, object> pluginConfig)
        {
            return LoadPlugin(pluginName, pluginConfig, requireGetMemory: true);
        }

        /// <summary>
        /// Creates an instance of each of the available plugins that have
        /// an entry in plugins.config and don't have active=false.
        /// </summary>
        public void LoadGlobalPlugins()
        {
            var configFileText = File.ReadAllText("plugins.config");
            IDictionary<string, object> allConfigValues;
            var loadedPluginsList = new List<object>();

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

            var allAvailablePluginsByName = GetPluginTypes();

            foreach(var pluginName in namesOfPluginsConfiguredAsActive)
            {
                try
                {
                    var pluginConfig = validPluginConfigs[pluginName];
                    foreach(var sharedConfigKey in commonConfigValues.Keys)
                        if(!pluginConfig.ContainsKey(sharedConfigKey))
                            pluginConfig[sharedConfigKey] = commonConfigValues[sharedConfigKey];

                    var pluginInstance = LoadPlugin(pluginName, pluginConfig, requireGetMemory: false);
                    loadedPluginsList.Add(pluginInstance);
                }
                catch(Exception ex)
                {
                    tell("Could not load plugin '{0}': {1}", new[] {pluginName, ex.Message});
                }
            }

            loadedPlugins = loadedPluginsList.ToArray();
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

        private object LoadPlugin(
            string pluginName, 
            IDictionary<string, object> pluginConfig,
            bool requireGetMemory)
        {
            var pluginTypes = GetPluginTypes();

            if(!pluginTypes.ContainsKey(pluginName))
                throw new InvalidOperationException($"No plugin with name '{pluginName}' found");

            var type = pluginTypes[pluginName];

            var constructor = type
                .GetConstructor(new[] {typeof(PluginContext), typeof(IDictionary<string, object>)});

            if(constructor == null)
                throw new InvalidOperationException("No suitable constructor found for " + type.FullName);

            if(requireGetMemory)
            {
                var getMemoryMethod = type.GetMethod("GetMemory");
                if(getMemoryMethod == null || getMemoryMethod.GetParameters().Length > 0 || getMemoryMethod.ReturnType != typeof(IMemory))
                    throw new InvalidOperationException(type.FullName + " has no suitable GetMemory method");
            }

            return Activator.CreateInstance(type, new object[] {context, pluginConfig});
        }
    }
}
