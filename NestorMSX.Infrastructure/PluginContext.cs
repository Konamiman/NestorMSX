using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Konamiman.NestorMSX.Hardware;
using Konamiman.NestorMSX.Host;
using Konamiman.NestorMSX.Menus;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX
{
    /// <summary>
    /// Represents the emulation context in which a plugin runs.
    /// </summary>
    public class PluginContext
    {
        /// <summary>
        /// The emulated CPU.
        /// </summary>
        public IZ80Processor Cpu { get; set; }

        /// <summary>
        /// The emulated slots system.
        /// </summary>
        public IExternallyControlledSlotsSystem SlotsSystem { get; set; }

        /// <summary>
        /// The emulated VDP.
        /// </summary>
        public IExternallyControlledV9938 Vdp { get; set; }

        /// <summary>
        /// The form that displays the emulator screen.
        /// </summary>
        public Form HostForm { get; set; }

        /// <summary>
        /// The source of keystrokes used by the emulator.
        /// </summary>
        public IKeyEventSource KeyEventSource { get; set; }

        /// <summary>
        /// The list of loaded plugins. Note that this will be null until
        /// <see cref="EnvironmentInitializationComplete"/> is fired.
        /// </summary>
        public IEnumerable<object> LoadedPlugins {get; set; }

        /// <summary>
        /// Event that fires when the emulation environment initialization has finished
        /// and the emulation is about to start.
        /// </summary>
        public event EventHandler EnvironmentInitializationComplete;

        public void FireInitializationCompleteEvent()
        {
            if(EnvironmentInitializationComplete != null)
                EnvironmentInitializationComplete(this, EventArgs.Empty);
        }

        /// <summary>
        /// Delegate that allows to set the menu entry for the plugin in the host form.
        /// The first argument must be set to the plugin instance.
        /// The second argument is the menu entry to set, or null to remove an existing entry.
        /// </summary>
        public Action<object, MenuEntry> SetMenuEntry { get; set; }
    }
}
