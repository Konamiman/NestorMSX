using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Konamiman.NestorMSX.Menus;
using Konamiman.NestorMSX.Z80Debugger.Console;
using Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter;
using Konamiman.NestorMSX.Z80Debugger.Console.CommandProviders;
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

            var commandProviders = new object[]
            {
                new EmulatorCommandsProvider(context),
                typeof(UtilsCommandsProvider)
            };

            var messagePrinters = commandProviders.OfType<IAsyncMessagePrinter>();
            foreach(var printer in messagePrinters) {
                printer.PrintMessageRequest += OnPrintMessageRequest;
            }

            commandInterpreter = new CommandInterpreter(
                new EvaluantExpressionEvaluatorWrapper(), 
                commandProviders);
        }

        private void OnPrintMessageRequest(object sender, PrintMessageRequestEventArgs args)
        {
            if (hostForm.InvokeRequired)
                hostForm.Invoke(new Action(() => { PrintInConsole(args.Message); }));
            else
                PrintInConsole(args.Message);
        }

        private void PrintInConsole(object message)
        {
            var formFocused = hostForm.Focused;
            console?.Print(ResultsFormatter(message));
            if (formFocused) hostForm.Focus();
        }

        

        private ConsoleWindow console;
        private void ShowConsole()
        {
            console = new ConsoleWindow();
            console.CommandExecutionRequested += Console_CommandExecutionRequested;
            console.ResultsFormatter = ResultsFormatter;
            console.Show(hostForm);
        }

        private string ResultsFormatter(object arg)
        {
            if(arg is byte) {
                return $"{arg} - &H{arg:X2} - &B{Convert.ToString((byte)arg, 2).PadLeft(8,'0')} - \"{Convert.ToChar((byte)arg)}\"";
            }

            if(arg is ushort) {
                return $"{arg} - &H{arg:X4} - &B{Convert.ToString((ushort)arg, 4).PadLeft(4,'0')}";
            }

            if(arg is short) {
                return $"{arg} - &H{arg:X4} - &B{Convert.ToString((short)arg, 4).PadLeft(4,'0')}";
            }

            return arg?.ToString();
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
