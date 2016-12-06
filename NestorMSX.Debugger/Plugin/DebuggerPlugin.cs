using System.Collections.Generic;
using System.Windows.Forms;
using Konamiman.NestorMSX.Menus;
using Konamiman.NestorMSX.Z80Debugger.Console;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Z80Debugger.Plugin
{
    [NestorMSXPlugin("Debugger")]
    public class DebuggerPlugin
    {
        private readonly IZ80Processor z80;
        private readonly Form hostForm;

        public DebuggerPlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            this.z80 = context.Cpu;
            this.hostForm = context.HostForm;
            var menuEntry = new MenuEntry("Show console", ShowConsole);
            context.SetMenuEntry(this, menuEntry);
        }

        private void ShowConsole()
        {
            var console = new ConsoleTestForm(z80);
            console.Show(hostForm);
        }
    }
}
