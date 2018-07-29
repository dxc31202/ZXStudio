namespace ZXStudio
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.machine1 = new ZXStudio.Machine();
            this.SuspendLayout();
            // 
            // machine1
            // 
            this.machine1.A = 255;
            this.machine1.AF = 65535;
            this.machine1.AF_ = 65535;
            this.machine1.AllowDrop = true;
            this.machine1.B = 255;
            this.machine1.BC = 65535;
            this.machine1.BC_ = 65535;
            this.machine1.C = 255;
            this.machine1.D = 255;
            this.machine1.DE = 65535;
            this.machine1.DE_ = 65535;
            this.machine1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.machine1.E = 255;
            this.machine1.F = 255;
            this.machine1.H = 255;
            this.machine1.HL = 65535;
            this.machine1.HL_ = 65535;
            this.machine1.I = 0;
            this.machine1.IFF1 = 0;
            this.machine1.IFF2 = 0;
            this.machine1.IM = 0;
            this.machine1.IR = 0;
            this.machine1.IsTZXPlaying = false;
            this.machine1.IX = 65535;
            this.machine1.IXH = 255;
            this.machine1.IXL = 255;
            this.machine1.IY = 65535;
            this.machine1.IYH = 255;
            this.machine1.IYL = 255;
            this.machine1.L = 255;
            this.machine1.LastOut = 7;
            this.machine1.Location = new System.Drawing.Point(0, 0);
            this.machine1.Name = "machine1";
            this.machine1.PC = 0;
            this.machine1.R = 0;
            this.machine1.R7 = 0;
            this.machine1.Size = new System.Drawing.Size(474, 390);
            this.machine1.SP = 65535;
            this.machine1.TabIndex = 0;
            this.machine1.TapeBlock = null;
            this.machine1.TapeBlockProgress = null;
            this.machine1.TapeCounter = null;
            this.machine1.TapeOverallProgress = null;
            this.machine1.TotalCycles = 0;
            this.machine1.WZ = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 390);
            this.Controls.Add(this.machine1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Machine machine1;
    }
}