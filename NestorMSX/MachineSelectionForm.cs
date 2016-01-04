using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Konamiman.NestorMSX
{
    public partial class MachineSelectionForm : Form
    {
        public MachineSelectionForm()
        {
            InitializeComponent();
        }

        public void SetMachinesListItems(IEnumerable<string> machines)
        {
            machinesList.Items.AddRange(machines.OrderBy(m => m).ToArray());
        }

        public string SelectedMachine
        {
            get
            {
                return (string)machinesList.SelectedItem;
            }
        }

        private void machinesList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(machinesList.SelectedItem != null)
                this.DialogResult = DialogResult.OK;
        }

        private void machinesList_SelectedValueChanged(object sender, System.EventArgs e)
        {
            btnSelect.Enabled = (machinesList.SelectedItem != null);
        }
    }
}
