using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Konamiman.NestorMSX.Emulator;
using Konamiman.NestorMSX.Exceptions;
using Konamiman.NestorMSX.Menus;
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
        private readonly IDictionary<object, ToolStripItem> menuItemsByPlugin = new Dictionary<object, ToolStripItem>();
        private readonly IDictionary<MenuEntry, ToolStripItem> menuItemsByMenuEntry = new Dictionary<MenuEntry, ToolStripItem>();
        private readonly IDictionary<ToolStripItem, MenuEntry> menuEntriesByMenuItem = new Dictionary<ToolStripItem, MenuEntry>();

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
            pluginsMenu.Enabled = false;
            canvas.Paint += CanvasOnPaint;
        }

        public void ApplyConfig(Configuration config)
        {
            if (config == null)
                return;

            ValidateConfiguration(config);

            var width = (int)(((32 * 8) + config.HorizontalMarginInPixels * 2) * config.DisplayZoomLevel);
            var height = (int)(((26.5M * 8) + config.VerticalMarginInPixels * 2) * config.DisplayZoomLevel);
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

        public void SuspendCanvasLayout()
        {
            //this.Invoke(new Action(() => canvas.SuspendDrawing(this)));
        }

        public void ResumeCanvasLayout()
        {
            //if(this.InvokeRequired)
            //{
            //    this.Invoke(new Action(() => canvas.ResumeDrawing(this)));
            //}
            //else
            //{
            //    canvas.ResumeDrawing(this);
            //}
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

        #region Form menu

        private void emulationToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void resetCPUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            emulationEnvironment.SlotsSystem.EnableSlot(0, 0);
            emulationEnvironment.SlotsSystem.EnableSlot(1, 0);
            emulationEnvironment.SlotsSystem.EnableSlot(2, 0);
            emulationEnvironment.SlotsSystem.EnableSlot(3, 0);
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

        #endregion

        #region Plugin menu entries

        public void SetPluginMenuEntry(object sourcePlugin, MenuEntry entry)
        {
            if(sourcePlugin == null)
                return;

            if(menuItemsByPlugin.ContainsKey(sourcePlugin))
            {
                var oldMenuItem = menuItemsByPlugin[sourcePlugin];
                UnsubscribeFromEvents(oldMenuItem);
                pluginsMenu.DropDown.Items.Remove(oldMenuItem);
                menuItemsByPlugin.Remove(sourcePlugin);
            }

            if(entry != null)
            {
                var newMenuItem = GenerateMenuItem(entry);
                menuItemsByPlugin[sourcePlugin] = newMenuItem;
                pluginsMenu.DropDown.Items.Add(newMenuItem);
            }

            if(pluginsMenu.DropDown.Items.Count > 0) {
                var items = pluginsMenu.DropDown.Items.Cast<ToolStripMenuItem>();

                pluginsMenu.Enabled = true;

                var sortedMenuItems = items.OrderBy(m => m.Text).ToArray();
                pluginsMenu.DropDown.Items.Clear();
                pluginsMenu.DropDown.Items.AddRange(sortedMenuItems);
            }
            else {
                pluginsMenu.Enabled = false;
            }
        }

        private void UnsubscribeFromEvents(ToolStripItem oldMenuItem)
        {
            var entry = menuEntriesByMenuItem[oldMenuItem];
            entry.EnableChanged -= Entry_EnableChanged;
            entry.CheckedChanged -= Entry_CheckedChanged;
            entry.VisibleChanged -= Entry_VisibleChanged;
            entry.TitleChanged -= Entry_TitleChanged;

            menuItemsByMenuEntry.Remove(entry);
            menuEntriesByMenuItem.Remove(oldMenuItem);

            if(oldMenuItem is ToolStripMenuItem)
                foreach(var subMenu in ((ToolStripMenuItem)oldMenuItem).DropDown.Items)
                    UnsubscribeFromEvents((ToolStripMenuItem)subMenu);
        }

        private ToolStripItem GenerateMenuItem(MenuEntry entry)
        {
            ToolStripItem newMenuItem;

            if(entry.Title == "-") {
                newMenuItem = new ToolStripSeparator() {
                    Visible = entry.IsVisible,
                    Enabled = entry.IsEnabled
                };

                entry.EnableChanged += Entry_EnableChanged;
                entry.VisibleChanged += Entry_VisibleChanged;
            }
            else {
                newMenuItem = new ToolStripMenuItem(entry.Title) {
                    Visible = entry.IsVisible,
                    Enabled = entry.IsEnabled,
                    Checked = entry.IsChecked
                };

                entry.EnableChanged += Entry_EnableChanged;
                entry.VisibleChanged += Entry_VisibleChanged;
                entry.TitleChanged += Entry_TitleChanged;

                if(entry.Callback != null) {
                    newMenuItem.Click += (sender, args) => entry.Callback();
                    entry.CheckedChanged += Entry_CheckedChanged;
                }
                else {
                    var subMenuItems = entry.ChildEntries.Select(GenerateMenuItem).ToArray();
                    ((ToolStripMenuItem)newMenuItem).DropDown.Items.AddRange(subMenuItems);
                }
            }

            menuItemsByMenuEntry[entry] = newMenuItem;
            menuEntriesByMenuItem[newMenuItem] = entry;
            return newMenuItem;
        }

        private void Entry_TitleChanged(object sender, EventArgs eventArgs)
        {
            var menuEntry = (MenuEntry)sender;
            menuItemsByMenuEntry[menuEntry].Text = menuEntry.Title;
        }

        private void Entry_VisibleChanged(object sender, EventArgs eventArgs)
        {
            var menuEntry = (MenuEntry)sender;
            menuItemsByMenuEntry[menuEntry].Visible = menuEntry.IsVisible;
        }

        private void Entry_CheckedChanged(object sender, EventArgs e)
        {
            var menuEntry = (MenuEntry)sender;
            ((ToolStripMenuItem)menuItemsByMenuEntry[menuEntry]).Checked = menuEntry.IsChecked;
        }

        private void Entry_EnableChanged(object sender, EventArgs e)
        {
            var menuEntry = (MenuEntry)sender;
            menuItemsByMenuEntry[menuEntry].Enabled = menuEntry.IsEnabled;
        }

        #endregion
    }
}
