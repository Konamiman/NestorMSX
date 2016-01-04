using System.Collections.Generic;
using Konamiman.Z80dotNet;
using KeyEventArgs = Konamiman.NestorMSX.Hardware.KeyEventArgs;

namespace Konamiman.NestorMSX.BuiltInPlugins
{
    [NestorMSXPlugin("Debug form")]
    public class DebugFormPlugin
    {
        private readonly IZ80Processor z80;
        private readonly DebugForm debugForm;

        private DebugFormPlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            this.z80 = context.Cpu;
            this.debugForm = new DebugForm();
            this.debugForm.Show(context.HostForm);
            context.KeyEventSource.KeyPressed += KeyEventSourceOnKeyPressed;
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
