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
        private List<object> loadedPlugins = new List<object>();
        
        public IKeyEventSource KeyboardEventSource { get; }
        public EmulatorHostForm HostForm { get; }
        public IExternallyControlledSlotsSystem SlotsSystem { get; }
        public IExternallyControlledTms9918 Vdp { get; set; }
        public IZ80Processor Z80 { get; }
        public IKeyboardController KeyboardController { get; }
        public PluginsLoader PluginsLoader { get; }

        public IEnumerable<object> GetLoadedPlugins()
        {
            return loadedPlugins.ToArray();
        }

        public MsxEmulationEnvironment(Configuration config, Action<string, object[]> tell)
        {
            this.globalConfig = config;
            this.tell = tell;

            LoadMachineConfig();
            GenerateInjectedConfig();

            Z80 = CreateCpu();
            SlotsSystem = CreateEmptySlotsSystem();

            HostForm = CreateHostForm(Z80);
            KeyboardEventSource = HostForm;
            HostForm.SetFormTitle(config.MachineName);
            Vdp = CreateVdp(HostForm);
            HostForm.Vdp = Vdp;
            KeyboardController = CreateKeyboardController(HostForm);

            var pluginContext = new PluginContext
            {
                Cpu = Z80,
                HostForm = HostForm,
                SlotsSystem = SlotsSystem,
                Vdp = Vdp,
                KeyEventSource = KeyboardEventSource
            };
            PluginsLoader = new PluginsLoader(pluginContext, tell);
            LoadGlobalPlugins();
            LoadMachinePlugins();

            CreateSlotsSystem();

            var hardware = new MsxHardwareSet {
                Cpu = Z80,
                KeyboardController = KeyboardController,
                SlotsSystem = SlotsSystem,
                Vdp = Vdp
            };

            emulator = new MsxEmulator(hardware);
        }

        /// <summary>
        /// Creates an instance of each of the available plugins that have
        /// an entry in plugins.config and don't have active=false.
        /// </summary>
        private void LoadGlobalPlugins()
        {
            var configFileText = File.ReadAllText("plugins.config");
            IDictionary<string, object> allConfigValues;

            try
            {
                allConfigValues = JsonParser.Parse(configFileText) as IDictionary<string, object>;
            }
            catch (Exception ex)
            {
                throw new ConfigurationException("Error when parsing plugins.config file: " + ex.Message);
            }

            try
            {
                var globalPlugins = allConfigValues.GetDictionaryOrDefault("plugins");
                globalSharedPluginsConfig = allConfigValues.GetDictionaryOrDefault("sharedPluginsConfig");
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
            if(plugin != null && !loadedPlugins.Contains(plugin))
                loadedPlugins.Add(plugin);
        }

        private void LoadMachineConfig()
        {
            var machineName = globalConfig.MachineName;
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

        private EmulatorHostForm CreateHostForm(IZ80Processor cpu)
        {
            return new EmulatorHostForm(cpu, globalConfig);
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
                { "NestorMSX.machineName", globalConfig.MachineName },
                { "NestorMSX.machineDirectory", Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    @"machines/" + globalConfig.MachineName) },
                { "NestorMSX.sharedDirectory", Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    @"machines/Shared") }
            };
        }

        private IZ80Processor CreateCpu()
        {
            var z80 = new Z80Processor();

            if(globalConfig.CpuSpeedInMHz == 0)
                z80.ClockSynchronizer = null;
            else if(globalConfig.CpuSpeedInMHz < 0.001M || globalConfig.CpuSpeedInMHz > 100)
                throw new ConfigurationException("CPU speed must be either zero or a value between 0.001 and 100");
            else
                z80.ClockFrequencyInMHz = globalConfig.CpuSpeedInMHz;

            return z80;
        }

        public void Run()
        {
            KeyboardEventSource.StartGeneratingKeyEvents();
            new Task(() => emulator.Run()).Start();
            Application.Run(HostForm);
        }
    }
}
