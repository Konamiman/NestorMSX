using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Konamiman.NestorMSX.Exceptions;
using Konamiman.NestorMSX.Hardware;
using Konamiman.NestorMSX.Host;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;
using System.Linq;
using System.Reflection;

namespace Konamiman.NestorMSX.Emulator
{
    /// <summary>
    /// Full environment for running a MSX emulation.
    /// </summary>
    public class MsxEmulationEnvironment
    {
        private MsxEmulator emulator;
        private IDictionary<string, object> machineConfig;
        private IDictionary<string, object> injectedConfig;
        private IDictionary<string, object> globalSharedPluginsConfig;
        private IDictionary<string, object> machineSharedPluginsConfig;
        private Action<string, object[]> tell;
        private Configuration globalConfig;
        private PluginContext pluginContext;
        private string machineName;
        private List<object> loadedPlugins = new List<object>();
        private IDictionary<string, object> configDictionary;
        private IDictionary<string, object> defaultEmulationParameters;
        private IClockSynchronizer originalClockSynchronizer;
        
        public IKeyEventSource KeyboardEventSource { get; private set; }
        public EmulatorHostForm HostForm { get; }
        public IExternallyControlledSlotsSystem SlotsSystem { get; private set; }
        public IExternallyControlledTms9918 Vdp { get; set; }
        public IZ80Processor Z80 { get; private set; }
        public IKeyboardController KeyboardController { get; private set; }
        public PluginsLoader PluginsLoader { get; private set; }

        public IEnumerable<object> GetLoadedPlugins()
        {
            return loadedPlugins.ToArray();
        }

        public MsxEmulationEnvironment(IDictionary<string, object> configDictionary, Action<string, object[]> tell, string machineName)
        {
            this.configDictionary = configDictionary;
            defaultEmulationParameters = configDictionary.GetValue<IDictionary<string, object>>("defaultEmulationParameters");
            this.tell = tell;
            HostForm = new EmulatorHostForm(this);
            Z80 = new Z80Processor();
            this.originalClockSynchronizer = Z80.ClockSynchronizer;

            this.machineName = machineName;

            LoadMachineConfig();
            GenerateInjectedConfig();

            var machineEmulationParameters = machineConfig.GetDictionaryOrDefault("emulationParameters");
            defaultEmulationParameters.MergeInto(machineEmulationParameters);

            this.globalConfig = ConvertConfigDictionaryToObject(machineEmulationParameters);
            globalConfig.GlobalPluginsConfig = configDictionary.GetDictionaryOrDefault("plugins");
            globalConfig.SharedPluginsConfig = configDictionary.GetDictionaryOrDefault("sharedPluginsConfig");

            HostForm.ApplyConfig(globalConfig);

            ConfigureCpu();
            SlotsSystem = CreateEmptySlotsSystem();

            KeyboardEventSource = HostForm;
            HostForm.SetFormTitle(this.machineName);
            Vdp = CreateVdp(HostForm);
            KeyboardController = CreateKeyboardController(HostForm);

            pluginContext = new PluginContext
            {
                Cpu = Z80,
                HostForm = HostForm,
                SlotsSystem = SlotsSystem,
                Vdp = Vdp,
                KeyEventSource = KeyboardEventSource,
                LoadedPlugins = null
            };
            PluginsLoader = new PluginsLoader(pluginContext, tell);
            LoadGlobalPlugins();
            LoadMachinePlugins();

            CreateSlotsSystem();

            var hardware = new MsxHardwareSet
            {
                Cpu = Z80,
                KeyboardController = KeyboardController,
                SlotsSystem = SlotsSystem,
                Vdp = Vdp
            };

            emulator = new MsxEmulator(hardware);
        }

        private static Configuration ConvertConfigDictionaryToObject(IDictionary<string, object> configDictionary)
        {
            var config = new Configuration();

            config.ColorsFile = configDictionary.GetValue<string>("colorsFile").AsAbsolutePath();
            config.CpuSpeedInMHz = configDictionary.GetValueOrDefault<decimal>("cpuSpeedInMHz", 0);
            config.DisplayZoomLevel = configDictionary.GetValueOrDefault("displayZoomLevel", 2);
            config.HorizontalMarginInPixels = configDictionary.GetValueOrDefault("horizontalMarginInPixels", 8);
            config.VerticalMarginInPixels = configDictionary.GetValueOrDefault("verticalMarginInPixels", 16);
            config.KeymapFile = configDictionary.GetValue<string>("keymapFile").AsAbsolutePath();
            config.VdpFrequencyMultiplier = configDictionary.GetValueOrDefault<decimal>("vdpFrequencyMultiplier", 1);

            return config;
        }

        /// <summary>
        /// Creates an instance of each of the available plugins that have
        /// an entry in plugins.config and don't have active=false.
        /// </summary>
        private void LoadGlobalPlugins()
        {
            try
            {
                var globalPlugins = globalConfig.GlobalPluginsConfig;
                globalSharedPluginsConfig = globalConfig.SharedPluginsConfig;
                var plugins = PluginsLoader.LoadPlugins(globalPlugins, machineSharedPluginsConfig, globalSharedPluginsConfig, injectedConfig);
                foreach(var plugin in plugins)
                    RegisterPlugin(plugin);
            }
            catch(Exception ex)
            {
                throw new Exception("Error when loading global plugins: " + ex.Message);
            }
        }

        private void LoadMachinePlugins()
        {
            var machinePlugins = machineConfig.GetDictionaryOrDefault("plugins");

            try
            {
                var plugins = PluginsLoader.LoadPlugins(machinePlugins, machineSharedPluginsConfig, globalSharedPluginsConfig, injectedConfig);
                foreach (var plugin in plugins)
                    RegisterPlugin(plugin);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when loading machine plugins: " + ex.Message);
            }
        }

        private void RegisterPlugin(object plugin)
        {
            if(plugin != null && !loadedPlugins.Contains(plugin)) {
                loadedPlugins.Add(plugin);
            }
        }

        private void LoadMachineConfig()
        {
            var folder = Path.Combine("machines", machineName).AsAbsolutePath();
            if(!Directory.Exists(folder))
                throw new ConfigurationException($"Machine folder not found for '{machineName}'");

            var configFilePath = Path.Combine(folder, "machine.config");
            if(!File.Exists(configFilePath))
                throw new ConfigurationException($"machine.config file not found for '{machineName}'");

            var json = File.ReadAllText(configFilePath);
            try
            {
                machineConfig = (IDictionary<string, object>)JsonParser.Parse(json);
            }
            catch(Exception ex)
            {
                throw new ConfigurationException($"Error when reading machine.config file for '{machineName}': {ex.Message}");
            }

            machineSharedPluginsConfig = machineConfig.GetDictionaryOrDefault("sharedPluginsConfig");
        }

        private IExternallyControlledSlotsSystem CreateEmptySlotsSystem()
        {
            var expandedSlots = machineConfig
                .GetValueOrDefault<int[]>("expandedSlots", new int[0])
                .Select(s =>(TwinBit)s).ToArray();
            return new SlotsSystem(expandedSlots);
        }

        private KeyboardController CreateKeyboardController(IKeyEventSource keyEventSource)
        {
            return new KeyboardController(keyEventSource, FileUtils.ReadAllText(globalConfig.KeymapFile));
        }

        private IExternallyControlledTms9918 CreateVdp(IDrawingSurface drawingSurface)
        {
            return new Tms9918(new DisplayRenderer(new GraphicsBasedDisplay(drawingSurface, globalConfig), globalConfig), globalConfig);
        }

        private void CreateSlotsSystem()
        {
            foreach (var slotConfig in machineConfig["slots"] as IDictionary<string, object>)
            {
                SlotNumber slotNumber;
                if (!SlotNumber.TryParse(slotConfig.Key, out slotNumber))
                    continue;

                var pluginConfig = (IDictionary<string, object>)slotConfig.Value;
                pluginConfig["slotNumber"] = slotNumber.EncodedByte;

                var typeName = (string)pluginConfig["type"];

                injectedConfig.MergeInto(pluginConfig);

                object pluginInstance;
                try
                {
                    pluginInstance = PluginsLoader.GetPluginInstanceForSlot(typeName, pluginConfig, machineSharedPluginsConfig, globalSharedPluginsConfig, injectedConfig);
                    RegisterPlugin(pluginInstance);
                }
                catch(Exception ex)
                {
                    var message = ex.InnerException?.Message ?? ex.Message;
                    tell($"Could not load plugin {typeName} in slot {slotConfig.Key}: {message}", new object[0]);
                    continue;
                }

                var getMemoryMethod = pluginInstance.GetType().GetMethod("GetMemory");

                IMemory memory;
                try
                {
                    memory = (IMemory)getMemoryMethod.Invoke(pluginInstance, null);
                }
                catch (TargetInvocationException ex)
                {
                    tell($"Error when invoking GetMemory for plugin {typeName} in slot {slotConfig.Key}: {ex.InnerException.Message}", new object[0]);
                    continue;
                }

                try
                {
                    SlotsSystem.SetSlotContents(slotNumber, memory);
                }
                catch(Exception ex)
                {
                    tell($"Error when setting contents for slot {slotConfig.Key}: {ex.Message}", new object[0]);
                }
            }
        }

        private void GenerateInjectedConfig()
        {
            injectedConfig = new Dictionary<string, object>
            {
                { "NestorMSX.machineName", machineName },
                { "NestorMSX.machineDirectory", Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    @"machines/" + machineName) },
                { "NestorMSX.sharedDirectory", Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    @"machines/Shared") }
            };
        }

        private void ConfigureCpu()
        {
            if(globalConfig.CpuSpeedInMHz == 0)
                Z80.ClockSynchronizer = null;
            else if(globalConfig.CpuSpeedInMHz < 0.001M || globalConfig.CpuSpeedInMHz > 100)
                throw new ConfigurationException("CPU speed must be either zero or a value between 0.001 and 100");
            else {
                Z80.ClockSynchronizer = originalClockSynchronizer;
                Z80.ClockFrequencyInMHz = globalConfig.CpuSpeedInMHz;
            }
        }

        public void DisposePlugins()
        {
            var disposablePlugins = GetLoadedPlugins().Where(p => p is IDisposable).Cast<IDisposable>();

            foreach (var plugin in disposablePlugins)
                try
                {
                    plugin.Dispose();
                }
                catch
                {

                }

            loadedPlugins.Clear();
        }
        
        public void Run()
        {
            pluginContext.LoadedPlugins = loadedPlugins.ToArray();
            pluginContext.FireInitializationCompleteEvent();

            KeyboardEventSource.StartGeneratingKeyEvents();
            new Task(() => emulator.Run()).Start();
            Application.Run(HostForm);
        }
    }
}
