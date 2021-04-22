using BMS.Business;
using BMS.Model;
using BMS.Utils;
using DevExpress.XtraCharts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BMS
{
	public partial class frmChart : Form
	{
		public frmChart()
		{
			InitializeComponent();
		}

		private void frmChart_Load(object sender, EventArgs e)
		{
			ArrayList gears = GearBO.Instance.FindAll();
			cbxGear.DisplayMember = "HYP";
			cbxGear.ValueMember = "ID";
			cbxGear.DataSource = gears;			
			cbxGear.SelectedIndex = 0;
		}

		private void cbxGear_SelectedIndexChanged(object sender, EventArgs e)
		{
			int ID = TextUtils.ToInt(cbxGear.SelectedValue);
			Expression exp = new Expression("GearID", ID);
			ArrayList lstGearWk = GearWorkingBO.Instance.FindByExpression(exp);
			ChartTitle chartTitle = new ChartTitle();
			chartTitle.Text = "RTC";
			chart.Titles.Add(chartTitle);
			//
			/*SideBySideBarSeriesView view1 = chart.Series[0].View as SideBySideBarSeriesView;
			view1.BarDistance = 0;
			view1.BarDistanceFixed = 0;
			view1.EqualBarWidth = true;
			view1.Color = Color.MediumSeaGreen;
			if (lstGearWk.Count>0)
			{
				chart.DataSource = lstGearWk;
				chart.Series[1].LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
				chart.Series[2].LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
				chart.Series[0].LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;

				XYDiagram diagram = (XYDiagram)chart.Diagram;
				diagram.AxisY.Title.Visible = true;
				diagram.AxisY.Title.Alignment = StringAlignment.Center;
				diagram.AxisY.Title.Text = "Cột 1";

				SecondaryAxisY myAxisY = new SecondaryAxisY("my Y-Axis");
				((XYDiagram)chart.Diagram).SecondaryAxesY.Add(myAxisY);
				((LineSeriesView)chart.Series[2].View).AxisY = myAxisY;
				myAxisY.Title.Alignment = StringAlignment.Center;

				myAxisY.Title.Text = "Cột 2";
				myAxisY.Title.Visible = true;
				myAxisY.Title.TextColor = Color.Red;
				myAxisY.Label.TextColor = Color.Red;
				myAxisY.Color = Color.Red;

				

				BarSeriesLabel seriesLabel = chart.Series[0].Label as BarSeriesLabel;
				seriesLabel.Position = BarSeriesLabelPosition.TopInside;
				seriesLabel.TextOrientation = TextOrientation.Horizontal;
				
				chart.Series[0].ArgumentDataMember = "WorkingName";
				chart.Series[0].ValueDataMembers[0] = "DefaultValue";
				//chart.Series[0]
				chart.Series[1].ArgumentDataMember = "WorkingName";
				chart.Series[1].ValueDataMembers[0] = "MaxValue";
				chart.Series[2].ArgumentDataMember = "WorkingName";
				chart.Series[2].ValueDataMembers[0] = "MinValue";
			}		*/	
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (chart.Visible == false)
			{
				chart.Visible = true;
			}
			else
			{
				chart.Visible = false;
			}
		}
	}
}
