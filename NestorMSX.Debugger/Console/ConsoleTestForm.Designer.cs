namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    partial class ConsoleTestForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.lblPC = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(399, 205);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(180, 113);
            this.button1.TabIndex = 0;
            this.button1.Text = "Pause";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(354, 380);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(119, 69);
            this.button2.TabIndex = 1;
            this.button2.Text = "PC = ";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lblPC
            // 
            this.lblPC.AutoSize = true;
            this.lblPC.Location = new System.Drawing.Point(500, 399);
            this.lblPC.Name = "lblPC";
            this.lblPC.Size = new System.Drawing.Size(109, 32);
            this.lblPC.TabIndex = 2;
            this.lblPC.Text = "0x????";
            this.lblPC.Click += new System.EventHandler(this.lblPC_Click);
            // 
            // ConsoleTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 534);
            this.Controls.Add(this.lblPC);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "ConsoleTestForm";
            this.Text = "ConsoleTestForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label lblPC;
    }
}