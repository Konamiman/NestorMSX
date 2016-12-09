using System;
using System.Drawing;
using System.Windows.Forms;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    public partial class ConsoleWindow : Form, IConsoleWindow
    {
        public ConsoleWindow()
        {
            InitializeComponent();
            txtCommand.Text = "";
            txtResults.Text = "";

            //this.Size = new Size(800, 600);
        }

        public string Title
        {
            get { return this.Text; }

            set { this.Text = value; }
        }

        public event EventHandler<CommandExecutionRequestedEventArgs> CommandExecutionRequested;

        private void txtCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char) Keys.Return)
                return;

            if (CommandExecutionRequested == null)
                return;

            txtResults.Text += $"> {txtCommand.Text.Trim()}\r\n";
            var args = new CommandExecutionRequestedEventArgs(txtCommand.Text);
            CommandExecutionRequested(this, args);
            txtCommand.Text = "";
            txtResults.Text += (args.Result?.ToString() ?? "null") + "\r\n";

            txtCommand.Focus();
        }
    }
}
