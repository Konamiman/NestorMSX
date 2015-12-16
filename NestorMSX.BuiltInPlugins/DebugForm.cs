using System;
using System.CodeDom;
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
            Invoke(new Action(() => { txtDebug.Text += text; } ));
        }
    }
}
