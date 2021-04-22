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
using System.Threading;
//using System.Web.UI.DataVisualization.Charting;
using System.Windows.Forms;

namespace BMS
{
	public partial class frmChartManageDao : Form
	{
		public frmChartManageDao()
		{
			InitializeComponent();
		}
		
		private Thread _threadLoadDaoDetail;
		ArrayList _lstDao;
		Series _series1;
		Series _series2;
		Series _series3;

		private void frmChartManageDao_Load(object sender, EventArgs e)
		{
			
			// gắn title Dao
			ChartTitle chartTitle = new ChartTitle();
			chartTitle.Text = "QUAN LY DAO HOB (TOTAL)";
			chartDao.Titles.Add(chartTitle);
			

			//khởi tạo series Dao
			_series1 = new Series("QtyProduct", ViewType.Bar);
			_series1.ArgumentDataMember = "ProductCode";
			_series1.ValueDataMembers[0] = "QtyProduct";
			_series1.ArgumentScaleType = ScaleType.Qualitative;
			_series2 = new Series("QtyProductMax", ViewType.Line);
			_series2.ArgumentDataMember = "ProductCode";
			_series2.ValueDataMembers[0] = "QtyProductMax";
			_series2.ArgumentScaleType = ScaleType.Qualitative;
			_series3 = new Series("QtyMai", ViewType.Line);
			_series3.ArgumentDataMember = "ProductCode";
			_series3.ValueDataMembers[0] = "QtyMai";
			_series3.ArgumentScaleType = ScaleType.Qualitative;

			_series1.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
			_series2.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
			_series3.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;

			chartDao.Series.AddRange(new Series[] { _series1, _series2, _series3 });

			//Khởi tạo series Dao Detail
			Series seriesDetail1 = new Series("QtyProduct", ViewType.Bar);
			seriesDetail1.ArgumentDataMember = "CreatedAt";
			seriesDetail1.ValueDataMembers[0] = "QtyProduct";
			seriesDetail1.ArgumentScaleType = ScaleType.Qualitative;
			Series seriesDetail2 = new Series("QtyProductMax", ViewType.Line);
			seriesDetail2.ArgumentDataMember = "CreatedAt";
			seriesDetail2.ValueDataMembers[0] = "QtyProductMax";
			seriesDetail2.ArgumentScaleType = ScaleType.Qualitative;

			Series seriesDetail3 = new Series("TotalProduct", ViewType.Line);
			seriesDetail3.ArgumentDataMember = "CreatedAt";
			seriesDetail3.ValueDataMembers[0] = "TotalProduct";
			seriesDetail3.ArgumentScaleType = ScaleType.Qualitative;
			chartDaoDetail.Series.AddRange(new Series[] { seriesDetail1, seriesDetail2, seriesDetail3 });

			seriesDetail1.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
			seriesDetail2.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
			seriesDetail3.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;

			// format Axis
			XYDiagram diagram = chartDaoDetail.Diagram as XYDiagram;
			//diagram.AxisX.DateTimeScaleOptions.;
			//diagram.AxisX.DateTimeScaleOptions.GridAlignment = DateTimeGridAlignment.Millisecond;
			diagram.AxisY.Title.Alignment = StringAlignment.Center;
			diagram.AxisY.Title.Text = "Cột 1";
			diagram.AxisY.Title.Visible = true;
			diagram.AxisY.Title.TextColor = Color.Red;
			diagram.AxisY.Label.TextColor = Color.Red;

			
			XYDiagram diaDao = chartDao.Diagram as XYDiagram;
			diaDao.AxisY.Title.Alignment = StringAlignment.Center;
			diaDao.AxisY.Title.Text = "Cột 1";
			diaDao.AxisY.Title.Visible = true;
			diaDao.AxisY.Title.TextColor = Color.Red;
			diaDao.AxisY.Label.TextColor = Color.Red;

			SecondaryAxisY myAxisY = new SecondaryAxisY("my Y-Axis");
			((XYDiagram)chartDao.Diagram).SecondaryAxesY.Add(myAxisY);
			((XYDiagram)chartDaoDetail.Diagram).SecondaryAxesY.Add(myAxisY);
			((LineSeriesView)_series3.View).AxisY = myAxisY;
			((LineSeriesView)seriesDetail3.View).AxisY = myAxisY;
			myAxisY.Title.Alignment = StringAlignment.Center;

			myAxisY.Title.Text = "Cột 2";
			myAxisY.Title.Visible = true;
			myAxisY.Title.TextColor = Color.Green;
			myAxisY.Label.TextColor = Color.Green;
			myAxisY.Color = Color.Green;

			// Series Label
			SeriesPoint s = new SeriesPoint();
			
			PointSeriesLabel barSeries =  _series1.Label as PointSeriesLabel;
     		SideBySideBarSeriesLabel l = chartDao.Series[0].Label as SideBySideBarSeriesLabel;
			SideBySideBarSeriesView v = chartDao.Series[0].View as SideBySideBarSeriesView;
			BarSeriesLabel seriesLabel = chartDao.Series[0].Label as BarSeriesLabel;

			PointSeriesLabel p = _series1.Label as PointSeriesLabel;
			
			
			
			//seriesLabel.BackColor
			seriesLabel.Position = BarSeriesLabelPosition.TopInside;
			seriesLabel.TextOrientation = TextOrientation.Horizontal;
			 
			v.EqualBarWidth = true;
			
			//seriesLabel.MaxWidth = (int)barSeries.BarWidth;

			_lstDao = ManageDaoHOBBO.Instance.FindAll();
			if (_lstDao.Count>0)
			{
				chartDao.DataSource = _lstDao;
				//start thread
				_threadLoadDaoDetail = new Thread(new ThreadStart(loadDaoDetails));
				_threadLoadDaoDetail.IsBackground = true;
				_threadLoadDaoDetail.Start();
			}
		}
		private void loadDaoDetails()
		{
			int i = 0;
			while (true)
			{
				if (i == _lstDao.Count - 1)
					i = 0;
				
				ManageDaoHOBModel daoHOBModel = _lstDao[i] as ManageDaoHOBModel;
				Expression exp = new Expression("ProductID", daoHOBModel.ID);
				ArrayList arrDaoDetails = DaodetailBO.Instance.FindByExpressionWithOrder(exp, "CreatedAt", "DESC");
				// gắn title DaoDetail
				ChartTitle chartTitleDetail = new ChartTitle();
				chartTitleDetail.Text = string.Format("QUAN LY DAO HOB (DETAIL). MA SP: {0}", daoHOBModel.ProductCode);
				if (chartDaoDetail.Titles.Count > 0)
				{
					chartDaoDetail.Titles.RemoveAt(0);
				}				
				chartDaoDetail.Titles.Add(chartTitleDetail);
				if (arrDaoDetails.Count > 0)
				{
					DaodetailModel daoRecent = arrDaoDetails[0] as DaodetailModel;
					DaodetailModel daodetail = new DaodetailModel();
					daodetail.QtyProduct = TextUtils.ToInt(_series1.Points[i].Values[0]);
					daodetail.QtyProductMax = TextUtils.ToInt(_series2.Points[i].Values[0]);
					daodetail.TotalProduct = daodetail.QtyProduct + daoRecent.QtyProduct;
					daodetail.CreatedAt = DateTime.Now.Date;
					arrDaoDetails.Add(daodetail);
					chartDaoDetail.DataSource = arrDaoDetails;
				}
				else
				{
					ArrayList arr = new ArrayList();
					DaodetailModel daodetail = new DaodetailModel();
					daodetail.QtyProduct = TextUtils.ToInt(_series1.Points[i].Values[0]);
					daodetail.QtyProductMax = TextUtils.ToInt(_series2.Points[i].Values[0]);
					daodetail.CreatedAt = DateTime.Now.Date;
					daodetail.TotalProduct = daodetail.QtyProduct;
					arr.Add(daodetail);
					chartDaoDetail.DataSource = arr;
				}
				i++;
				Thread.Sleep(5000);

			}
		}
	}
}
