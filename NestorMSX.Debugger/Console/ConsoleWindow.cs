using System;
using System.Windows.Forms;
using Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    public partial class ConsoleWindow : Form, IConsoleWindow
    {
        private readonly Func<object, string> defaultResultsFormatter = o => o?.ToString();
        private Func<object, string> _resultsFormatter;

        public ConsoleWindow()
        {
            InitializeComponent();
            cmdControl.Command += CmdControlOnCommand;
            cmdControl.SetWholeThingFont(new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));

            ResultsFormatter = null;
        }

        private void CmdControlOnCommand(object sender, CommandEventArgs e)
        {
            if(e.Command.Equals("cls", StringComparison.InvariantCultureIgnoreCase)) {
                Clear();
                e.Cancel = true;
                return;
            }

            var args = new CommandExecutionRequestedEventArgs(e.Command);
            CommandExecutionRequested?.Invoke(this, args);
            e.Message = ResultsFormatter(args.Result);
        }

        public string Title
        {
            get { return this.Text; }

            set { this.Text = value; }
        }

        public event EventHandler<CommandExecutionRequestedEventArgs> CommandExecutionRequested;

        [Alias("cls")]
        public void Clear()
        {
            Do(() => cmdControl.ClearMessages());
        }

        public Func<object, string> ResultsFormatter
        {
            get { return _resultsFormatter; }
            set { _resultsFormatter = value ?? defaultResultsFormatter; }
        }

        public void Print(string text)
        {
            Do(() => cmdControl.AddMessage(text));
        }

        private void Do(Action action, bool alwaysInvoke = false)
        {
            try {
                if (this.InvokeRequired || alwaysInvoke)
                    this.Invoke(action);
                else
                    action();
            }
            catch (ObjectDisposedException) {
            }
        }
    }
}
