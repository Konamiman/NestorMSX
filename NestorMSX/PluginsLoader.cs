using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX
{
    public class PluginsLoader
    {
        private static IDictionary<string, Type> pluginTypes;

        private readonly PluginContext context;
        private readonly Action<string, object[]> tell;

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
                pluginAssemblyFileNames.SelectMany(GetPluginsInAssembly)
                    .ToDictionary(x => x.Key, x => x.Value);

            return pluginTypes;
        }

        /// <summary>
        /// Gets an instance of a plugin to be inserted in a slot.
        /// </summary>
        /// <param name="pluginName"></param>
        /// <param name="pluginConfig"></param>
        /// <returns>The loaded plugin instance</returns>
        public object GetPluginInstanceForSlot(string pluginName, params IDictionary<string, object>[] pluginConfigs)
        {
            var instance = LoadPlugin(pluginName, true, pluginConfigs);
            if(instance == null)
                throw new InvalidOperationException("The plugin factory method returned null");

            return instance;
        }

        public IEnumerable<string> GetPluginNames(IDictionary<string, object> pluginsSectionInConfig, bool activeStatus)
        {
            var validPluginConfigs =
               pluginsSectionInConfig
                   .Where(p => p.Value is IDictionary<string, object>)
                   .ToDictionary(p => p.Key, p => (IDictionary<string, object>)p.Value);

            Func<string, bool> isActive =
                x => !validPluginConfigs[x].ContainsKey("active") || (validPluginConfigs[x]["active"] as bool?) == true;

            var suitablePluginNames =
                validPluginConfigs.Keys
                    .Where(k => activeStatus ? isActive(k) : !isActive(k));

            return suitablePluginNames;
        }

        /// <summary>
        /// Gets all the globally defined plugins (defined in the "plugins" section of a config file)
        /// </summary>
        /// <param name="pluginsSectionInConfig">The "plugins" section of a config file</param>
        /// <param name="pluginConfigs">Plugin configurations to apply, more prioritary first</param>
        /// <returns>All the loaded plugin instances</returns>
        public IEnumerable<object> LoadPlugins(IDictionary<string, object> pluginsSectionInConfig, IEnumerable<string> pluginNamesToExclude, params IDictionary<string, object>[] pluginConfigs)
        {
            var loadedPluginsList = new List<object>();
            
            var validPluginConfigs =
                pluginsSectionInConfig
                    .Where(p => p.Value is IDictionary<string, object>)
                    .ToDictionary(p => p.Key, p => (IDictionary<string, object>)p.Value);

            var namesOfPluginsConfiguredAsActive = GetPluginNames(pluginsSectionInConfig, true);
            if (pluginNamesToExclude != null)
                namesOfPluginsConfiguredAsActive = namesOfPluginsConfiguredAsActive.Except(pluginNamesToExclude);

            foreach(var pluginName in namesOfPluginsConfiguredAsActive)
            {
                try
                {
                    var pluginConfig = validPluginConfigs[pluginName] as IDictionary<string, object>;
                    var allPluginConfigs = new[] {pluginConfig}.Concat(pluginConfigs).ToArray();

                    var pluginInstance = LoadPlugin(pluginName, false, allPluginConfigs);
                    if(pluginInstance != null)
                        loadedPluginsList.Add(pluginInstance);
                }
                catch(Exception ex)
                {
                    if(ex is TargetInvocationException)
                        ex = ex.InnerException;

                    tell("Could not load plugin '{0}': {1}", new[] {pluginName, ex.Message});
                }
            }

            return loadedPluginsList.ToArray();
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

        private static Type[] argumentsForConstruction = 
            {typeof (PluginContext), typeof (IDictionary<string, object>)};


        /// <summary>
        /// Loads one single plugin
        /// </summary>
        /// <param name="pluginName">Name of the plugin</param>
        /// <param name="requireGetMemory">True if "GetMemory" method is required</param>
        /// <param name="pluginConfigs">Plugin configurations to apply, more prioritary first</param>
        /// <returns>The generated plugin instance</returns>
        private object LoadPlugin(
            string pluginName,
            bool requireGetMemory,
            params IDictionary<string, object>[] pluginConfigs)
        {
            var mainPluginConfig = pluginConfigs[0];
            var pluginConfigClone = mainPluginConfig.Keys.ToDictionary(k => k, k => mainPluginConfig[k]);
            foreach(var extraPluginConfig in pluginConfigs.Skip(1))
                extraPluginConfig.MergeInto(pluginConfigClone);

            var pluginTypes = GetPluginTypes();

            if(!pluginTypes.ContainsKey(pluginName))
                throw new InvalidOperationException($"No plugin with name '{pluginName}' found");

            var type = pluginTypes[pluginName];

            var factoryMethod = type
                .GetMethod("GetInstance", BindingFlags.Static | BindingFlags.Public, null,
                    argumentsForConstruction, null);

            if (factoryMethod == null || !factoryMethod.ReturnType.IsAssignableFrom(type))
            {
                var constructor = type.GetConstructor(argumentsForConstruction);
                if(constructor == null)
                    throw new InvalidOperationException("No suitable factory method nor constructor found for " + type.FullName);
            }

            if(requireGetMemory)
            {
                var getMemoryMethod = type.GetMethod("GetMemory");
                if(getMemoryMethod == null || getMemoryMethod.GetParameters().Length > 0 || getMemoryMethod.ReturnType != typeof(IMemory))
                    throw new InvalidOperationException(type.FullName + " has no suitable GetMemory method");
            }

            return 
                factoryMethod == null ? 
                Activator.CreateInstance(type, context, pluginConfigClone) : 
                factoryMethod.Invoke(null, new object[] {context, pluginConfigClone});
        }
    }
}
