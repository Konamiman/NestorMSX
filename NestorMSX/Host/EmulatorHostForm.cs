using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Konamiman.NestorMSX.Emulator;
using Konamiman.NestorMSX.Exceptions;
using Konamiman.Z80dotNet;
using KeyEventArgs = Konamiman.NestorMSX.Hardware.KeyEventArgs;

namespace Konamiman.NestorMSX.Host
{
    /// <summary>
    /// The form that displays the emulated screen.
    /// </summary>
    public partial class EmulatorHostForm : Form, IKeyEventSource, IDrawingSurface
    {
        private readonly MsxEmulationEnvironment emulationEnvironment;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;

        private readonly List<Keys> PressedKeys = new List<Keys>();

        public void SetFormTitle(string title)
        {
            this.Text = "NestorMSX - " + title;
        }

        #region Initialization

        public EmulatorHostForm() : this(null) { }

        public EmulatorHostForm(MsxEmulationEnvironment emulationEnvironment)
        {
            this.emulationEnvironment = emulationEnvironment;
            InitializeComponent();
            canvas.Paint += CanvasOnPaint;
        }

        public void ApplyConfig(Configuration config)
        {
            if (config == null)
                return;

            ValidateConfiguration(config);

            var width = (int)(((32 * 8) + config.HorizontalMarginInPixels * 2) * config.DisplayZoomLevel);
            var height = (int)(((24 * 8) + config.VerticalMarginInPixels * 2) * config.DisplayZoomLevel);
            ClientSize = new Size(width, height + mainMenu.Height);
        }

        private static void ValidateConfiguration(Configuration config)
        {
            if(config.HorizontalMarginInPixels < 0 || config.HorizontalMarginInPixels > 1000) {
                throw new ConfigurationException(
                    "Horizontal margin for display area must be an integer number between 0 and 1000");
            }

            if(config.VerticalMarginInPixels < 0 || config.VerticalMarginInPixels > 1000) {
                throw new ConfigurationException(
                    "Vertical margin for display area must be an integer number between 0 and 1000");
            }
        }

        #endregion

        #region IDrawingSurface

        public event EventHandler<PaintEventArgs> RequiresPaint;

        public Graphics GetGraphics()
        {
            return canvas.CreateGraphics();
        }

        private void CanvasOnPaint(object sender, PaintEventArgs paintEventArgs)
        {
            if(RequiresPaint != null)
                RequiresPaint(this, paintEventArgs);
        }

        #endregion

        #region Keyboard management

        public void StartGeneratingKeyEvents()
        {
        }

        public void StopGeneratingKeyEvents()
        {
        }

        public event EventHandler<KeyEventArgs> KeyPressed;
        public event EventHandler<KeyEventArgs> KeyReleased;

        protected override bool ProcessKeyMessage(ref Message m)
        {
            var key = (Keys)m.WParam;
            if(m.Msg == WM_KEYDOWN)
            {
                if(!PressedKeys.Contains(key))
                {
                    PressedKeys.Add(key);

                    if (KeyPressed != null)
                        KeyPressed(this, new KeyEventArgs(key));
                }
            }
            else if(m.Msg == WM_KEYUP)
            {
                if(PressedKeys.Contains(key))
                    PressedKeys.Remove(key);
                if(KeyReleased != null)
                    KeyReleased(this, new KeyEventArgs(key));
            }

            return base.ProcessKeyMessage(ref m);
        }

        #endregion

        private void emulationToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void resetCPUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            emulationEnvironment.Z80.Reset();
        }

        private void restartEmulationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void restartAsADifferentMachineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newMachineName = Program.ShowMachineSelectionDialog();
            if(newMachineName == null)
                return;

            Program.SaveMachineNameAsState(newMachineName);
            Application.Restart();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
