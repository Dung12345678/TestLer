namespace BMS
{
    partial class wfDodaoSylvac
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
            try
            {
                base.Dispose(disposing);
            }
            catch { }
            
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblDodao03 = new System.Windows.Forms.Label();
            this.lblDodao02 = new System.Windows.Forms.Label();
            this.lblDodao01 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblProcess = new System.Windows.Forms.Label();
            this.lblClose = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(208)))), ((int)(((byte)(240)))));
            this.panel1.Controls.Add(this.lblDodao03);
            this.panel1.Controls.Add(this.lblDodao02);
            this.panel1.Controls.Add(this.lblDodao01);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblProcess);
            this.panel1.Location = new System.Drawing.Point(12, 15);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(586, 163);
            this.panel1.TabIndex = 3;
            // 
            // lblDodao03
            // 
            this.lblDodao03.BackColor = System.Drawing.Color.White;
            this.lblDodao03.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDodao03.ForeColor = System.Drawing.Color.Black;
            this.lblDodao03.Location = new System.Drawing.Point(393, 28);
            this.lblDodao03.Name = "lblDodao03";
            this.lblDodao03.Size = new System.Drawing.Size(185, 74);
            this.lblDodao03.TabIndex = 3;
            this.lblDodao03.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDodao02
            // 
            this.lblDodao02.BackColor = System.Drawing.Color.White;
            this.lblDodao02.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDodao02.ForeColor = System.Drawing.Color.Black;
            this.lblDodao02.Location = new System.Drawing.Point(202, 28);
            this.lblDodao02.Name = "lblDodao02";
            this.lblDodao02.Size = new System.Drawing.Size(185, 74);
            this.lblDodao02.TabIndex = 3;
            this.lblDodao02.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDodao01
            // 
            this.lblDodao01.BackColor = System.Drawing.Color.White;
            this.lblDodao01.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDodao01.ForeColor = System.Drawing.Color.Black;
            this.lblDodao01.Location = new System.Drawing.Point(11, 28);
            this.lblDodao01.Name = "lblDodao01";
            this.lblDodao01.Size = new System.Drawing.Size(185, 74);
            this.lblDodao01.TabIndex = 3;
            this.lblDodao01.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(195, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(199, 22);
            this.label1.TabIndex = 1;
            this.label1.Text = "Đang lấy dữ liệu độ đảo";
            // 
            // lblProcess
            // 
            this.lblProcess.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProcess.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.lblProcess.Location = new System.Drawing.Point(214, 112);
            this.lblProcess.Name = "lblProcess";
            this.lblProcess.Size = new System.Drawing.Size(159, 49);
            this.lblProcess.TabIndex = 3;
            this.lblProcess.Text = "________";
            // 
            // lblClose
            // 
            this.lblClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblClose.AutoSize = true;
            this.lblClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClose.ForeColor = System.Drawing.Color.Gray;
            this.lblClose.Location = new System.Drawing.Point(570, 1);
            this.lblClose.Name = "lblClose";
            this.lblClose.Size = new System.Drawing.Size(38, 13);
            this.lblClose.TabIndex = 4;
            this.lblClose.Text = "Close";
            // 
            // wfDodaoSylvac
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 190);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(80, 80);
            this.Name = "wfDodaoSylvac";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "wfDodaoSylvac";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.wfDodaoSylvac_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblDodao03;
        private System.Windows.Forms.Label lblDodao02;
        private System.Windows.Forms.Label lblDodao01;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblProcess;
        private System.Windows.Forms.Label lblClose;
    }
}