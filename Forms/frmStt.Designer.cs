namespace BMS
{
	partial class frmStt
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
			this.label1 = new System.Windows.Forms.Label();
			this.nmrStt = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.nmrStt)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Blue;
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(527, 78);
			this.label1.TabIndex = 0;
			this.label1.Text = "SỐ THỨ TỰ BẮT ĐẦU SẢN PHẨM";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// nmrStt
			// 
			this.nmrStt.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nmrStt.Location = new System.Drawing.Point(12, 103);
			this.nmrStt.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.nmrStt.Name = "nmrStt";
			this.nmrStt.Size = new System.Drawing.Size(503, 44);
			this.nmrStt.TabIndex = 3;
			this.nmrStt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.nmrStt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.nmrStt_KeyDown);
			// 
			// frmStt
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(527, 181);
			this.Controls.Add(this.nmrStt);
			this.Controls.Add(this.label1);
			this.Name = "frmStt";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "START INDEX";
			((System.ComponentModel.ISupportInitialize)(this.nmrStt)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown nmrStt;
	}
}