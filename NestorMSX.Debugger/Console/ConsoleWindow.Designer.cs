namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    partial class ConsoleWindow
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
            this.cmdControl = new Konamiman.NestorMSX.Z80Debugger.Console.CommandPromptControl();
            this.SuspendLayout();
            // 
            // cmdControl
            // 
            this.cmdControl.AutoSize = true;
            this.cmdControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.cmdControl.Delimiters = new char[] {
        ' '};
            this.cmdControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdControl.ForeColor = System.Drawing.Color.White;
            this.cmdControl.Location = new System.Drawing.Point(0, 0);
            this.cmdControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmdControl.MessageColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.cmdControl.MinimumSize = new System.Drawing.Size(0, 26);
            this.cmdControl.Name = "cmdControl";
            this.cmdControl.PromptColor = System.Drawing.Color.White;
            this.cmdControl.Size = new System.Drawing.Size(649, 429);
            this.cmdControl.TabIndex = 0;
            // 
            // ConsoleWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(649, 417);
            this.Controls.Add(this.cmdControl);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ConsoleWindow";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NestorMSX Debugger - Console";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CommandPromptControl cmdControl;
    }
}