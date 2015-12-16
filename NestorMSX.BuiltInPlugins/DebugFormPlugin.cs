using System.Collections.Generic;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.BuiltInPlugins
{
    [NestorMSXPlugin("Debug form")]
    public class DebugFormPlugin
    {
        private readonly IZ80Processor z80;
        private readonly DebugForm debugForm;

        public DebugFormPlugin(PluginContext context, IDictionary<string, string> pluginConfig)
        {
            this.z80 = context.Cpu;
            this.debugForm = new DebugForm();
            this.debugForm.Show(context.HostForm);
            context.KeyEventSource.KeyPressed += KeyEventSourceOnKeyPressed;
        }

        private void KeyEventSourceOnKeyPressed(object sender, Hardware.KeyEventArgs keyEventArgs)
        {
            debugForm.Append(keyEventArgs.Value.ToString());
        }
    }
}
