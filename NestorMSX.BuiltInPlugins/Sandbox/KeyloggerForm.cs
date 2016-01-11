using System;
using System.Windows.Forms;

namespace Konamiman.NestorMSX.BuiltInPlugins
{
    public partial class DebugForm : Form
    {
        public DebugForm()
        {
            InitializeComponent();
        }

        public void Append(string text)
        {
            if(InvokeRequired)
                Invoke(new Action(() => { txtDebug.Text += text; } ));
            else
                txtDebug.Text += text;
        }
    }
}
