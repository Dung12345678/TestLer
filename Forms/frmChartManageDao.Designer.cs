namespace BMS
{
	partial class frmChartManageDao
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.chartDao = new DevExpress.XtraCharts.ChartControl();
			this.chartDaoDetail = new DevExpress.XtraCharts.ChartControl();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartDao)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartDaoDetail)).BeginInit();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.chartDao);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.chartDaoDetail);
			this.splitContainer1.Size = new System.Drawing.Size(1387, 702);
			this.splitContainer1.SplitterDistance = 336;
			this.splitContainer1.TabIndex = 0;
			// 
			// chartDao
			// 
			this.chartDao.Dock = System.Windows.Forms.DockStyle.Fill;
			this.chartDao.Location = new System.Drawing.Point(0, 0);
			this.chartDao.Name = "chartDao";
			this.chartDao.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
			this.chartDao.Size = new System.Drawing.Size(1387, 336);
			this.chartDao.TabIndex = 0;
			// 
			// chartDaoDetail
			// 
			this.chartDaoDetail.Dock = System.Windows.Forms.DockStyle.Fill;
			this.chartDaoDetail.Location = new System.Drawing.Point(0, 0);
			this.chartDaoDetail.Name = "chartDaoDetail";
			this.chartDaoDetail.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
			this.chartDaoDetail.Size = new System.Drawing.Size(1387, 362);
			this.chartDaoDetail.TabIndex = 0;
			// 
			// frmChartManageDao
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1387, 702);
			this.Controls.Add(this.splitContainer1);
			this.Name = "frmChartManageDao";
			this.Text = "frmChartManageDao";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.frmChartManageDao_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chartDao)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartDaoDetail)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private DevExpress.XtraCharts.ChartControl chartDao;
		private DevExpress.XtraCharts.ChartControl chartDaoDetail;
	}
}