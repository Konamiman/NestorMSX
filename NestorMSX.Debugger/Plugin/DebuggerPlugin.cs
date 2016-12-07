using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Konamiman.NestorMSX.Menus;
using Konamiman.NestorMSX.Z80Debugger.Console;
using Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter;
using Konamiman.NestorMSX.Z80Debugger.Console.ExpressionEvaluator;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Z80Debugger.Plugin
{
    [NestorMSXPlugin("Debugger")]
    public class DebuggerPlugin
    {
        private readonly IZ80Processor z80;
        private readonly Form hostForm;
        private readonly CommandInterpreter commandInterpreter;

        public DebuggerPlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            this.z80 = context.Cpu;
            this.hostForm = context.HostForm;
            var menuEntry = new MenuEntry("Show console", ShowConsole);
            context.SetMenuEntry(this, menuEntry);

            commandInterpreter = new CommandInterpreter(new EvaluantExpressionEvaluatorWrapper(), new object[0]);
        }

        private void ShowConsole()
        {
            var console = new ConsoleWindow();
            console.CommandExecutionRequested += Console_CommandExecutionRequested;
            console.Show(hostForm);
        }

        private void Console_CommandExecutionRequested(object sender, CommandExecutionRequestedEventArgs e)
        {
            try {
                var result = commandInterpreter.ExecuteCommand(e.Command);
                e.Result = result;
            }
            catch (CommandExecutionException ex) {
                e.Result = $"*** {ex.Message}";
            }
            catch(Exception ex) {
                e.Result = $"*** Unexpected exception: {ex}";
            }
        }
    }
}
