namespace Konamiman.NestorMSX.Host
{
    partial class EmulatorHostForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmulatorHostForm));
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.pluginsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.emulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetCPUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.restartEmulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartAsADifferentMachineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.canvas = new Konamiman.NestorMSX.Host.DoubleBufferedPictureBox();
            this.mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pluginsMenu,
            this.emulationToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(669, 28);
            this.mainMenu.TabIndex = 1;
            this.mainMenu.Text = "menuStrip1";
            // 
            // pluginsMenu
            // 
            this.pluginsMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.pluginsMenu.Name = "pluginsMenu";
            this.pluginsMenu.Size = new System.Drawing.Size(68, 24);
            this.pluginsMenu.Text = "Plugins";
            // 
            // emulationToolStripMenuItem
            // 
            this.emulationToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.emulationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetCPUToolStripMenuItem,
            this.toolStripSeparator1,
            this.restartEmulationToolStripMenuItem,
            this.restartAsADifferentMachineToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.emulationToolStripMenuItem.Name = "emulationToolStripMenuItem";
            this.emulationToolStripMenuItem.Size = new System.Drawing.Size(88, 24);
            this.emulationToolStripMenuItem.Text = "Emulation";
            this.emulationToolStripMenuItem.Click += new System.EventHandler(this.emulationToolStripMenuItem_Click);
            // 
            // resetCPUToolStripMenuItem
            // 
            this.resetCPUToolStripMenuItem.Name = "resetCPUToolStripMenuItem";
            this.resetCPUToolStripMenuItem.Size = new System.Drawing.Size(290, 26);
            this.resetCPUToolStripMenuItem.Text = "Reset";
            this.resetCPUToolStripMenuItem.Click += new System.EventHandler(this.resetCPUToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(287, 6);
            // 
            // restartEmulationToolStripMenuItem
            // 
            this.restartEmulationToolStripMenuItem.Name = "restartEmulationToolStripMenuItem";
            this.restartEmulationToolStripMenuItem.Size = new System.Drawing.Size(290, 26);
            this.restartEmulationToolStripMenuItem.Text = "Restart emulator";
            this.restartEmulationToolStripMenuItem.Click += new System.EventHandler(this.restartEmulationToolStripMenuItem_Click);
            // 
            // restartAsADifferentMachineToolStripMenuItem
            // 
            this.restartAsADifferentMachineToolStripMenuItem.Name = "restartAsADifferentMachineToolStripMenuItem";
            this.restartAsADifferentMachineToolStripMenuItem.Size = new System.Drawing.Size(290, 26);
            this.restartAsADifferentMachineToolStripMenuItem.Text = "Restart as a different machine...";
            this.restartAsADifferentMachineToolStripMenuItem.Click += new System.EventHandler(this.restartAsADifferentMachineToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(287, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(290, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // canvas
            // 
            this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvas.Location = new System.Drawing.Point(0, 28);
            this.canvas.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(669, 408);
            this.canvas.TabIndex = 0;
            this.canvas.TabStop = false;
            // 
            // EmulatorHostForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 436);
            this.Controls.Add(this.canvas);
            this.Controls.Add(this.mainMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenu;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "EmulatorHostForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NestorMSX";
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DoubleBufferedPictureBox canvas;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem emulationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetCPUToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restartEmulationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restartAsADifferentMachineToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pluginsMenu;
    }
}

