using System.Collections.Generic;
using Konamiman.NestorMSX.Menus;
using Konamiman.Z80dotNet;
using KeyEventArgs = Konamiman.NestorMSX.Hardware.KeyEventArgs;

namespace Konamiman.NestorMSX.BuiltInPlugins
{
    [NestorMSXPlugin("Debug form")]
    public class DebugFormPlugin
    {
        private readonly IZ80Processor z80;
        private readonly DebugForm debugForm;
        private MenuEntry menuEntry;
        private PluginContext context;
        private int i = 1;

        private DebugFormPlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            this.context = context;
            this.z80 = context.Cpu;
            this.debugForm = new DebugForm();
            this.debugForm.Show(context.HostForm);
            context.KeyEventSource.KeyPressed += KeyEventSourceOnKeyPressed;
            SetMenu();
        }

        private void SetMenu()
        {
            menuEntry = new MenuEntry("Debug form", new[]
            {
                new MenuEntry("Option 1", Option1),
                new MenuEntry("Option 2 - " + i++, SetMenu)
            });
            context.SetMenuEntry(this, menuEntry);
        }

        private void Option1()
        {
            menuEntry.ChildEntries[0].Title += "!";
            menuEntry.ChildEntries[1].IsVisible = !menuEntry.ChildEntries[1].IsVisible;
        }

        public static DebugFormPlugin GetInstance(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            return new DebugFormPlugin(context, pluginConfig);
        }

        private void KeyEventSourceOnKeyPressed(object sender, KeyEventArgs keyEventArgs)
        {
            debugForm.Append(keyEventArgs.Value.ToString());
        }
    }
}
