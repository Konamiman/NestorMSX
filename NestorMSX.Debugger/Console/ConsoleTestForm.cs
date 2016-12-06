using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    public partial class ConsoleTestForm : Form
    {
        private readonly IZ80Processor z80;
        private bool mustPause;

        public ConsoleTestForm(IZ80Processor z80) : this()
        {
            this.z80 = z80;
            z80.BeforeInstructionFetch += Z80_BeforeInstructionFetch;
        }

        private void Z80_BeforeInstructionFetch(object sender, BeforeInstructionFetchEventArgs e)
        {
            if(mustPause) {
                mustPause = false;
                e.ExecutionStopper.Stop(true);
                Do(() => button1.Text = "Continue");
                Do(() => button1.Enabled = true);
            }
        }

        public ConsoleTestForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(z80.State != ProcessorState.Running) {
                Do(() => button1.Text = "Pause");
                new Task(() => z80.Continue()).Start();
            }
            else {
                mustPause = true;
                Do(() => button1.Enabled = false);
            }
        }

        private void Do(Action action, bool alwaysInvoke = false)
        {
            if (this.InvokeRequired || alwaysInvoke)
                this.Invoke(action);
            else
                action();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lblPC.Text = $"0x{z80.Registers.PC:X4}";
        }

        private void lblPC_Click(object sender, EventArgs e)
        {

        }
    }
}
