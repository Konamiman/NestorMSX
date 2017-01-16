namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    partial class CommandPromptControl
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.lblPrompt = new System.Windows.Forms.Label();
            this.rtbMessages = new System.Windows.Forms.RichTextBox();
            this.toolTipCommand = new System.Windows.Forms.ToolTip(this.components);
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.SystemColors.Window;
            this.panelBottom.Controls.Add(this.txtInput);
            this.panelBottom.Controls.Add(this.lblPrompt);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBottom.Location = new System.Drawing.Point(0, 120);
            this.panelBottom.Margin = new System.Windows.Forms.Padding(0);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(300, 26);
            this.panelBottom.TabIndex = 0;
            // 
            // txtInput
            // 
            this.txtInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInput.Location = new System.Drawing.Point(31, 0);
            this.txtInput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(269, 31);
            this.txtInput.TabIndex = 1;
            this.txtInput.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
            this.txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
            this.txtInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInput_KeyPress);
            // 
            // lblPrompt
            // 
            this.lblPrompt.AutoSize = true;
            this.lblPrompt.BackColor = System.Drawing.SystemColors.Window;
            this.lblPrompt.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblPrompt.Location = new System.Drawing.Point(0, 0);
            this.lblPrompt.Margin = new System.Windows.Forms.Padding(0);
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Size = new System.Drawing.Size(31, 32);
            this.lblPrompt.TabIndex = 0;
            this.lblPrompt.Text = ">";
            // 
            // rtbMessages
            // 
            this.rtbMessages.BackColor = System.Drawing.SystemColors.Window;
            this.rtbMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbMessages.Dock = System.Windows.Forms.DockStyle.Top;
            this.rtbMessages.Location = new System.Drawing.Point(0, 0);
            this.rtbMessages.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rtbMessages.Name = "rtbMessages";
            this.rtbMessages.ReadOnly = true;
            this.rtbMessages.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.rtbMessages.Size = new System.Drawing.Size(300, 120);
            this.rtbMessages.TabIndex = 2;
            this.rtbMessages.TabStop = false;
            this.rtbMessages.Text = "";
            this.rtbMessages.Click += new System.EventHandler(this.rtbMessages_Click);
            this.rtbMessages.TextChanged += new System.EventHandler(this.rtbMessages_TextChanged);
            // 
            // toolTipCommand
            // 
            this.toolTipCommand.AutoPopDelay = 5000;
            this.toolTipCommand.InitialDelay = 0;
            this.toolTipCommand.ReshowDelay = 100;
            // 
            // CommandPromptControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.rtbMessages);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(0, 26);
            this.Name = "CommandPromptControl";
            this.Size = new System.Drawing.Size(300, 146);
            this.Load += new System.EventHandler(this.Prompt_Load);
            this.BackColorChanged += new System.EventHandler(this.Prompt_BackColorChanged);
            this.FontChanged += new System.EventHandler(this.Prompt_FontChanged);
            this.ForeColorChanged += new System.EventHandler(this.Prompt_ForeColorChanged);
            this.Resize += new System.EventHandler(this.CommandPrompt_Resize);
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Label lblPrompt;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.RichTextBox rtbMessages;
        private System.Windows.Forms.ToolTip toolTipCommand;




    }
}
