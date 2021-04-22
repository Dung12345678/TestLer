using BMS.Business;
using BMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using BMS.Utils;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using DevExpress.XtraCharts;

namespace BMS
{
	public delegate void LapLai(DataGridViewRow currentRow);
	public delegate void SendFileName(string filename, string colName);
	public delegate void TransformLap(string value, bool rd);


	public partial class frmTestta2 : Form
	{


		public long _WorkerID = 0;
		private Color _mauHongNhat = Color.FromArgb(255, 204, 255);
		private Color _mauXam = Color.FromArgb(191, 191, 191);
		private Color _mauCarot = Color.FromArgb(244, 177, 131);
		private Color _mauTanSuat = Color.FromArgb(0, 176, 80);
		public SendFileName _sendFileName;

		string _pathImage = @"D:\Testler\Image";
		string _pathAudio = @"D:\Testler\Audio";
		string _pathSeverImage = @"Testler\Image";
		string _pathSeverAudio = @"Testler\Audio";

		double _maxValueForward;
		double _maxValueBackward;
		bool _deleteTestler = false;
		int _countGearWorking = 0;
		FMain _fMain;
		Series _seriesForward1;
		Series _seriesForward2;
		Series _seriesForward3;
		Series _seriesForwardMax;

		Series _seriesBackward1;
		Series _seriesBackward2;
		Series _seriesBackward3;
		Series _seriesBackwardMax;


		string _pathFileConfigUpdate = Path.Combine(Application.StartupPath, "ConfigUpdate.txt");
		string _pathFolderUpdate = "";
		string _pathUpdateServer = "";
		string _pathFileVersion = "";
		string _language = "VN";
		string _pathError = Path.Combine(Application.StartupPath, "Errors");
		Dictionary<string, string> _dicLanguageControls;
		Dictionary<string, string> _dicLanguageGrv;
		Dictionary<string, string> _dicHeader;

		string[] _arrLangGrv;
		string[] _arrLangLabel;
		ChartTitle _chartTitleF;
		ChartTitle _chartTitleB;

		DateTime[] _arrDateLR;


		public frmTestta2()
		{
			InitializeComponent();
		}

		private void loadFileLanguage(string lang, string[] arrLangLabel, string[] arrLangGrv)
		{
		
			_dicLanguageControls = new Dictionary<string, string>();
			_dicLanguageGrv = new Dictionary<string, string>();
			for (int i = 0; i < arrLangGrv.Length; i++)
			{
				if (string.IsNullOrWhiteSpace(arrLangGrv[i])) continue;
				
				if (lang == "VN")
				{
					_dicLanguageGrv.Add(arrLangGrv[i].Split(';')[0].Trim(), arrLangGrv[i].Split(';')[1].Trim());
				}
				else
				{
					_dicLanguageGrv.Add(arrLangGrv[i].Split(';')[0].Trim(), arrLangGrv[i].Split(';')[2].Trim());
				}
			}

			for (int i = 0; i < arrLangLabel.Length; i++)
			{
				if (string.IsNullOrWhiteSpace(arrLangLabel[i])) continue;

				if (lang == "VN")
				{
					_dicLanguageControls.Add(arrLangLabel[i].Split(';')[0].Trim(), arrLangLabel[i].Split(';')[1].Trim());

				}
				else
				{
					_dicLanguageControls.Add(arrLangLabel[i].Split(';')[0].Trim(), arrLangLabel[i].Split(';')[2].Trim());

				}
			}
		}
		private void changeLanguage()
		{
			loadFileLanguage(_language, _arrLangLabel, _arrLangGrv);
			// thay đổi ngôn ngữ cho các control label
			foreach (var item in _dicLanguageControls)
			{
				Control[] ctr = this.Controls.Find(item.Key, true);
				if (ctr.Length > 0) ctr[0].Text = item.Value;
			}
			// thay đổi ngôn ngữ cho collumn trong grv
			foreach (var item in _dicLanguageGrv)
			{
				if (grvData.Columns[item.Key] == null) continue;
				
				grvData.Columns[item.Key].HeaderText = item.Value;
			}
			if (_language == "JP")
			{
				_chartTitleF.Text = "正転のグラフ";
				_chartTitleB.Text = "逆転のグラフ";

			}
			else
			{
				_chartTitleF.Text = "Biểu đồ chiều thuận";
				//chartVibrationForward.Titles.Add(_chartTitleF);

				_chartTitleB.Text = "Biểu đồ chiều nghịch";
				//chartVibrationBackward.Titles.Add(_chartTitleB);
			}
			

		}
        private void createSeriesVibration(double value, int bit)
		{
			SeriesPoint seriesPoints;
			if (bit == 0)
			{
				forwardVibration1.Add(value);
				int count = _seriesForward1.Points.Count;
				seriesPoints = new SeriesPoint(count + 1, value);
				this.Invoke(new MethodInvoker(delegate { _seriesForward1.Points.Add(seriesPoints); }));
			}
			else
			{
				backwardVibration1.Add(value);
				int count = _seriesBackward1.Points.Count;
				seriesPoints = new SeriesPoint(count + 1, value);
				this.Invoke(new MethodInvoker(delegate { _seriesBackward1.Points.Add(seriesPoints); }));

			}

		}

		private void addPointToSeries(List<double> lst, Series sr)
		{
			SeriesPoint seriesPoints;
			this.Invoke(new MethodInvoker(delegate { sr.Points.Clear(); }));

			for (int i = 0; i < 50; i++)
			{
				seriesPoints = new SeriesPoint(i, lst[i]);
				this.Invoke(new MethodInvoker(delegate { sr.Points.Add(seriesPoints); }));
			}
		}
		void updateVersion()
		{
			try
			{				
				if (!Directory.Exists(_pathFolderUpdate))
				{
					Directory.CreateDirectory(_pathFolderUpdate);
				}
				if (!File.Exists(_pathFileVersion))
				{
					File.Create(_pathFileVersion);
					File.WriteAllText(_pathFileVersion, "1");
				}
				int currentVerion = TextUtils.ToInt(File.ReadAllText(_pathFileVersion).Trim());
				string[] listFileSv = DocUtils.GetFilesList(_pathUpdateServer);
				listFileSv = listFileSv.ToList().Where(o => o.Contains(".zip")).ToArray();
				Array.Reverse(listFileSv);
				if (listFileSv == null)
				{
					MessageBox.Show("Can't connect to server!");
					return;
				}

				string fileName = listFileSv[0];
				if (!fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
				{
					return;
				}
				int newVersion = TextUtils.ToInt(fileName.Split('.')[0]);
				if (newVersion > currentVerion)
				{
					Process.Start(Path.Combine(Application.StartupPath, "UpdateVersion.exe"));
				}
			}
			catch { }


		}

		private void frmTestta2_Load(object sender, EventArgs e)
		{
			DocUtils.InitFTPQLSX();
			// load tên máy tester
			if (File.Exists(Path.Combine(Application.StartupPath, "TesterName.txt")))
			{
				string name = File.ReadAllText(Path.Combine(Application.StartupPath, "TesterName.txt"));
				lbltester.Text = name.Trim();
			}

			// kiểm tra update version
			if (File.Exists(_pathFileConfigUpdate))
			{
				string[] lines = File.ReadAllLines(_pathFileConfigUpdate);
				string[] stringSeparators = new string[] { "||" };
				string[] arr = lines[1].Split(stringSeparators, 4, StringSplitOptions.RemoveEmptyEntries);
				_pathFolderUpdate = Path.Combine(Application.StartupPath, arr[1].Trim());
				_pathUpdateServer = arr[2].Trim();
				_pathFileVersion = Path.Combine(Application.StartupPath, arr[3].Trim());
				//Check update version
				updateVersion();
			}
			

			if (!Directory.Exists(_pathError))
			{
				Directory.CreateDirectory(_pathError);
			}

			//_fMain = new FMain();
			//_fMain._PathError = _pathError;
			//_fMain._transfer = new FMain.TransferData(transferData);
			//_fMain._transferRDBackward = new FMain.TransferDataRDBackward(transferDataRDBackward);
			//_fMain._transferRDForward = new FMain.TransferDataRDForward(transferDataRDForward);
			//_fMain._transferStatusStart = new FMain.TransferStatusRD(transferStatusRD);
			//_sendFileName = new SendFileName(_fMain.ReceiveFileName);
			//_fMain.Show();


			_chartTitleF = new ChartTitle();
			_chartTitleF.Text = "Biểu đồ chiều thuận";
			chartVibrationForward.Titles.Add(_chartTitleF);

			_chartTitleB = new ChartTitle();
			_chartTitleB.Text = "Biểu đồ chiều nghịch";
			chartVibrationBackward.Titles.Add(_chartTitleB);

            //chartVibration.n
           
            _seriesForward1 = new Series("Current", ViewType.Line);
            _seriesForward1.ValueScaleType = ScaleType.Numerical;
            _seriesForward1.ArgumentScaleType = ScaleType.Numerical;
			_seriesForward1.View.Color = Color.Red;


			_seriesForward2 = new Series("Before", ViewType.Line);
            _seriesForward2.ValueScaleType = ScaleType.Numerical;
            _seriesForward2.ArgumentScaleType = ScaleType.Numerical;
			_seriesForward2.View.Color = Color.Yellow;


			_seriesForward3 = new Series("Before...", ViewType.Line);
            _seriesForward3.ValueScaleType = ScaleType.Numerical;
            _seriesForward3.ArgumentScaleType = ScaleType.Numerical;
			_seriesForward3.View.Color = Color.Lime;

			_seriesForwardMax = new Series("", ViewType.Line);
			_seriesForwardMax.ValueScaleType = ScaleType.Numerical;
			_seriesForwardMax.ArgumentScaleType = ScaleType.Numerical;
			_seriesForwardMax.View.Color = Color.Black;
			((LineSeriesView)_seriesForwardMax.View).LineStyle.Thickness = 1;



			_seriesBackward1 = new Series("Current", ViewType.Line);
			_seriesBackward1.ArgumentScaleType = ScaleType.Numerical;
            _seriesBackward1.ValueScaleType = ScaleType.Numerical;
			_seriesBackward1.View.Color = Color.Red;

            _seriesBackward2 = new Series("Before", ViewType.Line);
            _seriesBackward2.ArgumentScaleType = ScaleType.Numerical;
            _seriesBackward2.ValueScaleType = ScaleType.Numerical;
			_seriesBackward2.View.Color = Color.Yellow;

            _seriesBackward3 = new Series("Before...", ViewType.Line);
            _seriesBackward3.ArgumentScaleType = ScaleType.Numerical;
            _seriesBackward3.ValueScaleType = ScaleType.Numerical;
			_seriesBackward3.View.Color = Color.Lime;

			_seriesBackwardMax = new Series("", ViewType.Line);
			_seriesBackwardMax.ArgumentScaleType = ScaleType.Numerical;
			_seriesBackwardMax.ValueScaleType = ScaleType.Numerical;
			_seriesBackwardMax.View.Color = Color.Black;
			((LineSeriesView)_seriesBackwardMax.View).LineStyle.Thickness = 1;


			// biểu đồ theo chiều thuận
			chartVibrationForward.Series.AddRange(new Series[] { _seriesForward1, _seriesForward2, _seriesForward3, _seriesForwardMax });

			// biểu đồ theo chiều nghịch
			chartVibrationBackward.Series.AddRange(new Series[] { _seriesBackward1, _seriesBackward2, _seriesBackward3, _seriesBackwardMax });
            XYDiagram diagramF = (XYDiagram)chartVibrationForward.Diagram;
            XYDiagram diagramB = (XYDiagram)chartVibrationBackward.Diagram;

            diagramF.AxisX.WholeRange.Auto = false;
            diagramF.AxisX.WholeRange.SetMinMaxValues(1, 50);
            diagramF.AxisY.WholeRange.Auto = false;
            diagramF.AxisY.WholeRange.SetMinMaxValues(0, 0.4);

            diagramB.AxisX.WholeRange.Auto = false;
            diagramB.AxisX.WholeRange.SetMinMaxValues(1, 50);
            diagramB.AxisY.WholeRange.Auto = false;
            diagramB.AxisY.WholeRange.SetMinMaxValues(0, 0.4);
            if (!Directory.Exists(_pathAudio))
			{
				Directory.CreateDirectory(_pathAudio);
			}
			if (!Directory.Exists(_pathImage))
			{
				Directory.CreateDirectory(_pathImage);
			}
			// load file language
			if (!File.Exists(Path.Combine(Application.StartupPath, "LanguageGrv.txt"))) return;
			if (!File.Exists(Path.Combine(Application.StartupPath, "Language.txt"))) return;
			_arrLangGrv = File.ReadAllLines(Path.Combine(Application.StartupPath, "LanguageGrv.txt"));
			_arrLangLabel = File.ReadAllLines(Path.Combine(Application.StartupPath, "Language.txt"));
			cboLanguage.SelectedIndex = 0;
			//loadFileLanguage(_language, _arrLangLabel, _arrLangGrv);
			DocUtils.InitFTPQLSX();
			colGear.Frozen = true;
			txtWorkerName.Focus();


		}
		public void transferStatusRD(int type, bool isStart)
		{

            if (type == 0 && isStart)
            {
				forwardVibration3 = new List<double>(forwardVibration2);
				forwardVibration2 = new List<double>(forwardVibration1);

				if (forwardVibration3.Count > 0)
					addPointToSeries(forwardVibration3, _seriesForward3);

				if (forwardVibration2.Count > 0)
					addPointToSeries(forwardVibration2, _seriesForward2);

				forwardVibration1.Clear();
				this.Invoke(new MethodInvoker(delegate
				{
					_seriesForward1.Points.Clear();

				}));


			}
			else if (type == 1 && isStart)
			{
				/// backward chart

				backwardVibration3 = new List<double>(backwardVibration2);
				backwardVibration2 = new List<double>(backwardVibration1);

				if (backwardVibration3.Count > 0)
					addPointToSeries(backwardVibration3, _seriesBackward3);

				if (backwardVibration2.Count > 0)
					addPointToSeries(backwardVibration2, _seriesBackward2);

				backwardVibration1.Clear();
				this.Invoke(new MethodInvoker(delegate
				{
					_seriesBackward1.Points.Clear();

				}));
			}
			
		}


        public void transferData(string[] value, string type)
		{
			if (type == "RD")
			{
				string str = getPathFile(grvData.CurrentCell);
				string[] arr = str.Split('_');
				string fileForward = arr[0] + "_" + arr[1] + "_" + arr[2] + "_1.wav";
				string fileBackward = arr[0] + "_" + arr[1] + "_" + arr[2] + "_2.wav";

				if (!File.Exists(Path.Combine(_pathAudio, fileForward)) || !File.Exists(Path.Combine(_pathAudio, fileBackward)))
				{
					MessageBox.Show("Lỗi file âm thanh. Vui lòng thao tác lại!");
					setFocusCell(grvData.CurrentCell.RowIndex);
					return;
				}
				grvData[colResult1.Index + 1, grvData.CurrentCell.RowIndex].Value = "Play";
				grvData[colResult1.Index + 2, grvData.CurrentCell.RowIndex].Value = "Play";
			}
			// gán giá trị cho các ô cell
			for (int i = 0; i < value.Length; i++)
			{
				grvData.CurrentCell.Value = value[i];
				int col = grvData.CurrentCell.ColumnIndex;
				int row = grvData.CurrentCell.RowIndex;
				bool NG = false;
				string nameworking = grvData.CurrentCell.OwningColumn.Name;

				if (nameworking == "col3" )
				{
					if (grvData.CurrentCell.Style.BackColor == Color.Red)
					{
						lapLaiConfirm();
					}
				}
				else if (nameworking == "col8")
				{
					for (int j = 0; j < 3; j++)
					{
						if (grvData.Rows[row].Cells[col - i].Style.BackColor == Color.Red)
						{
							NG = true;
							break;
						}
					}
					if (NG)
					{
						lapLaiConfirm();
					}
				}
				else if (nameworking == "col5")
				{
					for (int j = 0; j < 2; j++)
					{
						if (grvData.Rows[row].Cells[col - i].Style.BackColor == Color.Red)
						{
							NG = true;
							break;
						}
					}
					if (NG)
					{
						lapLaiConfirm();
					}
				}
				setFocusCell(grvData.CurrentCell.RowIndex);
			
			}     
		}

		List<double> backwardVibration1 = new List<double>();
		List<double> backwardVibration2 = new List<double>();
		List<double> backwardVibration3 = new List<double>();
		List<double> backwardVibrationMax;


		List<double> forwardVibration1 = new List<double>();
		List<double> forwardVibration2 = new List<double>();
		List<double> forwardVibration3 = new List<double>();
		List<double> forwardVibrationMax;


		public void transferDataRDBackward(double value)
		{
			//Thêm các giá trị đo động rung vào chart
			createSeriesVibration(value, 1);
		}


		public void transferDataRDForward(double value)
		{
			createSeriesVibration(value, 0);
		}
		int[] getFocusRowTarget(int rowIndex)
		{
			//int row = 4;
			int[] indexRowCol = new int[2];
			int countRow = grvData.Rows.Count;
			for (int i = rowIndex + 1; i < countRow; i++)
			{
				for (int j = 3; j <= (colResult1.Index - 1); j++)
				{
					//string value = TextUtils.ToString(grvData.Rows[i].Cells[j].Value);
					if (grvData.Rows[i].Cells[j].Style.BackColor != _mauXam)
					{
						indexRowCol[0] = i;
						indexRowCol[1] = j;
						return indexRowCol;
					}
				}
			}
			indexRowCol[0] = 4;
			indexRowCol[1] = 3;
			return indexRowCol;
			// return row;
		}
		string _fileName = "";
		string _colName = "";
		void setFocusCell(int rowIndex)
		{
			try
			{
				int colCurrent = grvData.CurrentCell.ColumnIndex;

				for (int j = colCurrent + 1; j < colQtyLap.Index; j++)
				{
					//if (j >= colResult1.Index - 1) continue;                   
					//string value = TextUtils.ToString(grvData.Rows[rowIndex].Cells[j].Value);
					if (grvData[j, rowIndex].Style.BackColor != _mauXam)
					{
						

						grvData.CurrentCell = grvData[j, rowIndex];
						_fileName = "";_colName = "";
						_fileName = getPathFile(grvData.CurrentCell);
						_colName = grvData.CurrentCell.OwningColumn.Name;
						grvData.CurrentCell.ReadOnly = false;

						if (_colName ==  "col4" || _colName == "col5")
							grvData.CurrentCell.ReadOnly = true;

						if (j > colGear.Index && j < colQtyLap.Index)
						{
							
							if (!string.IsNullOrWhiteSpace(_fileName))
							{
								_sendFileName(_fileName, _colName);
							}

						}

						if (j == colResult1.Index)
						{
							grvData.CurrentCell = grvData[grvData.CurrentCell.ColumnIndex + _qtyAudio + 1, rowIndex];
							grvData.CurrentCell.ReadOnly = false;
							_fileName = ""; _colName = "";
							_fileName = getPathFile(grvData.CurrentCell);
							_colName = grvData.CurrentCell.OwningColumn.Name;
							if (!string.IsNullOrWhiteSpace(_fileName))
							{								
								_sendFileName(_fileName, _colName);
							}
						}

						return;
					}
				}
				int[] rowIndex1 = getFocusRowTarget(rowIndex);
				for (int i = 1; i <= rowIndex1[0] - rowIndex; i++)
				{
					_isCatchKey = false;
					SendKeys.Send("{DOWN}");
				}
				grvData.CurrentCell = grvData[rowIndex1[1], rowIndex1[0]];
				grvData.CurrentCell.ReadOnly = false;

			}
			catch (Exception)
			{
			}
		}
		bool _isCatchKey = false;
		private void grvData_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 4) return;   //Bỏ 4 dòng đầu đi không nhảy nhót gì cả

			if (e.ColumnIndex == colResult1.Index) return;

			if (e.RowIndex < grvData.Rows.Count - 1)
			{
				_isCatchKey = false;
				SendKeys.Send("{Up}");

			}

			if (e.ColumnIndex >= 1 && e.ColumnIndex < colQtyLap.Index)
			{
                if (!_isLogin)
                {
                    //if (grvData.CurrentCell.OwningColumn.HeaderText.StartsWith("Độ rung động"))
					if (grvData.CurrentCell.OwningColumn.Name == "col4" || grvData.CurrentCell.OwningColumn.Name == "col5")
						grvData.CurrentCell.ReadOnly = true;
                }
				
               
                if (grvData.CurrentCell.Style.BackColor == Color.Red)
                {
                    lapLaiConfirm();
                }
				//grvData.CurrentCell.Value = grvData.CurrentCell.Valu
				//Xong thì nhảy đến cột đánh giá tiếp theo
				if (e.RowIndex == colResult1.Index -1)
				{
					_arrDateLR[e.RowIndex] = DateTime.Now;
				}
                setFocusCell(e.RowIndex);
                
            }
		}

	
		private void txtOrderNo_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				_deleteTestler = false;
				// load data từ store
				DataSet dts = TextUtils.GetListDataFromSP("spGetResultData_ByOrderCode", "A"
						, new string[1] { "@OrderCode" }
						, new object[1] { txtOrderNo.Text });
				DataTable dataTable0 = new DataTable();
				DataTable dataTable1 = new DataTable();
				if (dts.Tables.Count > 1)
				{
					dataTable0 = dts.Tables[0];
					dataTable1 = dts.Tables[1];
				}

				// kieeemr tra table 0 
				if (dataTable0.Rows.Count > 0)
				{
					//nếu có thì binding dữ liệu vào các control dạng text
					txtWorkerName.Text = TextUtils.ToString(dataTable0.Rows[0]["WorkerCode"]);
					txtHYP.Text = TextUtils.ToString(dataTable0.Rows[0]["HypCode"]);
					txtQty.Text = TextUtils.ToString(dataTable0.Rows[0]["Qty"]);
					dteNgayGiaCong.Value = TextUtils.ToDate3(dataTable0.Rows[0]["DateLR"]);
					txtBatch.Text = TextUtils.ToString(dataTable0.Rows[0]["Batch"]);
					txtConfirmer.Text = TextUtils.ToString(dataTable0.Rows[0]["Confirmer"]);
					_sttStart = (TextUtils.ToInt(dataTable0.Rows[0]["SttStart"]));
					_hypCode = txtHYP.Text.Trim();
					loadGear(_hypCode);
					GenerateByQty();

					//binding table 1 vào grid
					if (dataTable1.Rows.Count > 0)
					{
						_deleteTestler = true;
						bindingDataRow(dataTable1);
                        checkAudioImageExist();

                    }
				}
				else
				{
					// focus khi không load đc order trong db
					txtConfirmer.Focus();
				}


			}
		}

        void checkAudioImageExist()
        {
            for (int i = 0; i < grvData.Rows.Count; i++)
            {
                if (i < 4 || grvData.Rows[i].Cells[colResult1.Index].Style.BackColor == _mauXam) continue;
                string stt = TextUtils.ToString(grvData.Rows[i].Cells[colNumber.Index].Value);
                string order = TextUtils.ToString(txtOrderNo.Text.Trim());
                string qtyLap = TextUtils.ToString(grvData.Rows[i].Cells[colQtyLap.Index].Value);
                for (int h = 1; h <= _qtyImage; h++)
                {
                    string vtriAnh = TextUtils.ToString(grvData.Columns["colImage" + h].Tag);
                    string filename = Path.Combine(_pathImage, order + "_" + stt + "_" + qtyLap + "_" + vtriAnh + ".jpg");
                    if (File.Exists(filename))
                    {
                        grvData.Rows[i].Cells["colImage" + h].Value = "View";
                    }

                }

                for (int h = 1; h <= _qtyAudio; h++)
                {
                    string vtriAnh = TextUtils.ToString(grvData.Columns["colAudio" + h].Tag);
                    string filename = Path.Combine(_pathAudio, order + "_" + stt + "_" + qtyLap + "_" + vtriAnh + ".wav");
                    if (File.Exists(filename))
                    {
                        grvData.Rows[i].Cells["colAudio" + h].Value = "Play";
                    }

                }
            }
        }

		void bindingDataRow(DataTable dtGridData)
		{
			for (int i = 0; i < grvData.Rows.Count; i++)
			{
				int stt = TextUtils.ToInt(grvData.Rows[i].Cells[colNumber.Index].Value);
				if (i < 4) continue;
				DataRow[] arrRow = dtGridData.Select("STT = " + stt);
				if (arrRow.Length == 0) continue;
				for (int j = 0; j < grvData.Columns.Count; j++)
				{
					DataGridViewColumn col = grvData.Columns[j];
					if (col.DataPropertyName == "image" || col.DataPropertyName == "audio")
					{
                       
                        continue;
					}
					string valueRow = TextUtils.ToString(arrRow[0][col.DataPropertyName]);
					if (string.IsNullOrWhiteSpace(valueRow)) continue;
					grvData.Rows[i].Cells[j].Value = valueRow;

				}

			}
			//Set focus vào ô khi load lại dữ liệu
			grvData.Focus();
			setFocusByLoadOrder();
		}

		private void setFocusByLoadOrder()
		{
			bool _isFind = false;
			for (int i = 0; i < grvData.Rows.Count; i++)
			{
				if (i < 4) continue;
				for (int j = colGear.Index + 1; j < colResult1.Index; j++)
				{
					if (grvData.Rows[i].Cells[j].Style.BackColor == _mauHongNhat)
					{
						grvData.CurrentCell = grvData.Rows[i].Cells[j];
						grvData.CurrentCell.ReadOnly = false;
						_isFind = true;
						break;
					}
				}
				if (_isFind)
					break;
			}
		}
		private void GenerateByQty()
		{
			int qty = TextUtils.ToInt(txtQty.Text.Trim());
			if (qty == 0)
			{
				return;
			}
			// trừ đi 4 dòng đầu để stt bắt đầu từ 0
			if (qty == 5) _tuneParam = -4;
			else if (qty != 5 && qty >= 0)
			{
                _tuneParam = (_sttStart - 4);
            }
			


			if (!GenerateByHpy(qty)) return;


			//remove các dòng đã tạo nếu có
			if (grvData.Rows.Count > 4)
			{
				for (int i = grvData.Rows.Count - 1; i > 3; i--)
				{
					grvData.Rows.RemoveAt(i);
				}
			}
			if (qty == 5)
			{
				qty += 1;
			}
			_arrDateLR = new DateTime[qty+4];
			for (int i = 0; i < qty+4; i++)
			{
				_arrDateLR[i] = new DateTime();
			}
			grvData.Rows.Add(qty);
			
			//Thêm số thứ tự + Bôi màu cho các ô theo tần suất check
			for (int i = 0; i < grvData.Rows.Count; i++)
			{
				if (i < 4) continue;
				grvData.Rows[i].Cells[0].Value = i + _tuneParam;
				grvData.Rows[i].Cells[colQtyLap.Index].Value = 0;

				int countXam = 0;
				for (int j = 1; j < grvData.Columns.Count; j++)
				{
					// locked các cell lại. Các cell được mở khi các cell trước đó đã có giá trị.
					grvData.Rows[i].Cells[j].ReadOnly = true;
					string tagValue = TextUtils.ToString(grvData.Columns[j].Tag);
					//nếu là những cột tự sinh, là mục kiểm tra
					if (!string.IsNullOrEmpty(tagValue))
					{
						if (qty <= 6)
						{

							if (j <= colResult1.Index)
							{
								grvData.Rows[i].Cells[j].Style.BackColor = _mauHongNhat;
								if (_isLogin) grvData.Rows[i].Cells[j].ReadOnly = false;

							}
						}
						else
						{
							string tansuat = TextUtils.ToString(grvData.Rows[3].Cells[j].Value);
							if (tansuat.Contains("/"))
							{
								string[] arr = tansuat.Split('/');
								int moc = TextUtils.ToInt(arr[1]);
								// nếu vị trí hàng chia hết cho tần suất. thực hiện đổi màu nền tại hàng đó là LightBlue
								if ((i - 4) % moc == 0)
								{
									grvData.Rows[i].Cells[j].Style.BackColor = _mauHongNhat;
									if (_isLogin) grvData.Rows[i].Cells[j].ReadOnly = false;

								}
								else
								{
									grvData.Rows[i].Cells[j].Value = "N";
									grvData.Rows[i].Cells[j].Style.BackColor = _mauXam;
									grvData.Rows[i].Cells[j].Style.ForeColor = _mauXam;
									grvData.Rows[i].Cells[j].Selected = false;
									grvData.Rows[i].Cells[j].ReadOnly = true;
									countXam++;
								}
							}
						}

					}
					else
					{
						if (j == colHyp.Index || j == colGear.Index || j == colResult1.Index)
						{
							grvData.Rows[i].Cells[j].Style.BackColor = _mauHongNhat;
							if (_isLogin) grvData.Rows[i].Cells[j].ReadOnly = false;

						}

					}

				}
				if (countXam == _countGearWorking)
				{
					grvData.Rows[i].Cells[1].Style.BackColor = _mauXam;
					grvData.Rows[i].Cells[1].Selected = false;
					grvData.Rows[i].Cells[1].ReadOnly = true;

					grvData.Rows[i].Cells[2].Style.BackColor = _mauXam;
					grvData.Rows[i].Cells[2].Selected = false;
					grvData.Rows[i].Cells[2].ReadOnly = true;

					grvData.Rows[i].Cells[colResult1.Index].Style.BackColor = _mauXam;
					grvData.Rows[i].Cells[colResult1.Index].Selected = false;
					grvData.Rows[i].Cells[colResult1.Index].ReadOnly = true;

					for (int h = 1; h <= _qtyImage; h++)
					{

						grvData.Rows[i].Cells["colImage" + h].Style.BackColor = _mauXam;
					}

					for (int h = 1; h <= _qtyAudio; h++)
					{
						grvData.Rows[i].Cells["colAudio" + h].Style.BackColor = _mauXam;
					}


				}
				else
				{
					
				}
			}


		}

		private bool GenerateByHpy(int qty)
		{
			if (_hypCode == "")
			{
				MessageBox.Show("HYP không được để trống");
				return false;
			}
			
			DataTable dtGearWorking = TextUtils.LoadDataFromSP("spGetGearWorking_ByHypCode", "A", new string[] { "@HYP" }, new object[] { _hypCode });
			Expression exp = new Expression("HYP", _hypCode);
			ArrayList arr = GearBO.Instance.FindByExpression(exp);


			// Từ các hạng mục kiểm tra sinh ra các cột
			_countGearWorking = dtGearWorking.Rows.Count;

			//Đầu tiên bỏ các dòng hiện tại đang có trên danh sách đi
			grvData.Rows.Clear();
			//Xóa các cột có tag Value
			List<int> lstColumnTag = new List<int>();
			for (int i = 0; i < grvData.Columns.Count; i++)
			{
				DataGridViewColumn col = grvData.Columns[i];
				if (col.Tag != null && col.Tag.ToString() != "")
				{
					lstColumnTag.Add(col.Index);
				}
			}
			for (int i = lstColumnTag.Count - 1; i >= 0; i--)
			{
				grvData.Columns.Remove(grvData.Columns[lstColumnTag[i]]);
			}
			//add tiếp 4 dòng
			for (int i = 0; i < 4; i++)
			{
				grvData.Rows.Add();
			}

			if (_language == "VN")
			{
				grvData.Rows[0].Cells[0].Value = "Tiêu chuẩn";
				grvData.Rows[3].Cells[0].Value = "Tần suất";
			}
			else
			{
				grvData.Rows[0].Cells[0].Value = "基準";
				grvData.Rows[3].Cells[0].Value = "頻度";
			}
			

			grvData.Rows[1].Cells[0].Value = "MIN";
			grvData.Rows[2].Cells[0].Value = "MAX";


			if (_countGearWorking == 0 || arr.Count <= 0)
			{
				MessageBox.Show("Bạn phải nhập đúng HYP!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			GearModel gear = arr[0] as GearModel;
			_qtyImage = gear.QtyImage;
			_qtyAudio = gear.QtyAudio;
			if (qty <= 5)
			{
				_qtyImage += 2;
			}
			_dicHeader = new Dictionary<string, string>();
			for (int i = _countGearWorking - 1; i >= 0; i--)
			{
				DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
				/*_colName = TextUtils.ToString(dtGearWorking.Rows[i]["WorkingName"]).Replace("\n", " ").Replace("\r", " ");
				switch (_colName.Trim())
				{
					case "Độ đảo jig lap (đối với jig không bắt được 4 ốc) 冶具振れ度（４本ボルトを締めできない冶具に運用) ":
						_colName = "Độ đảo jig lap (đối với jig không bắt được 4 ốc)";
						break;					
					case "Độ rung động 振動さ Quay thuận 晴天":
						_colName = "Độ rung động Quay thuận";
						break;
					case "Độ rung động 振動さ Quay ngược 逆転":
						_colName = "Độ rung động Quay ngược";
						break;
					case "Đánh giá OK/NG 評価良否":
						_colName = "Đánh giá OK/NG";
						break;						

					default:
						break;
				}
				col.HeaderText = _colName;*/
				col.Name = "col" + i;
				_dicHeader.Add(col.Name.Trim(), TextUtils.ToString(dtGearWorking.Rows[i]["WorkingName"]));
				col.HeaderText = _dicLanguageGrv[col.Name.Trim()];//Tên của GearWorking
				
				//col.HeaderText = TextUtils.ToString(dtGearWorking.Rows[i]["WorkingName"]);//Tên của GearWorking
				col.Tag = TextUtils.ToString(dtGearWorking.Rows[i]["ID"]) + ";" + TextUtils.ToString(dtGearWorking.Rows[i]["SortOrder"]);//Cái này để lưu lại ID của GearWorking
				col.DataPropertyName = TextUtils.ToString(dtGearWorking.Rows[i]["SortOrder"]);//cho stt vào
				col.Width = 70;

				col.SortMode = DataGridViewColumnSortMode.NotSortable;
				grvData.Columns.Insert(3, col);
				// Cập nhật thông tin dữ liệu 4 dòng dữ liệu với giá trị tiêu chuẩn tương ứng với mỗi cột hạng mục
				grvData.Rows[0].Cells[3].Value = TextUtils.ToDecimal(dtGearWorking.Rows[i]["DefaultValue"]);
				grvData.Rows[1].Cells[3].Value = TextUtils.ToDecimal(dtGearWorking.Rows[i]["MinValue"]);
				grvData.Rows[2].Cells[3].Value = TextUtils.ToDecimal(dtGearWorking.Rows[i]["MaxValue"]);
				grvData.Rows[3].Cells[3].Value = TextUtils.ToString(dtGearWorking.Rows[i]["TanSuat"]);

				grvData.Rows[0].Cells[3].Style.BackColor = _mauCarot;
				grvData.Rows[1].Cells[3].Style.BackColor = _mauCarot;
				grvData.Rows[2].Cells[3].Style.BackColor = _mauCarot;
				grvData.Rows[3].Cells[3].Style.BackColor = _mauTanSuat;

					//lấy giá trị max rung động thuận nghịch
				if (TextUtils.ToString(dtGearWorking.Rows[i]["WorkingName"]) == "Độ rung động\n振動さ\nQuay thuận\n晴天")
				{
					_maxValueForward = TextUtils.ToDouble(dtGearWorking.Rows[i]["MaxValue"]);

				}

				if (TextUtils.ToString(dtGearWorking.Rows[i]["WorkingName"]) == "Độ rung động\n振動さ\nQuay ngược\n逆転")
				{
					_maxValueBackward = TextUtils.ToDouble(dtGearWorking.Rows[i]["MaxValue"]);
				}

			}

			
			for (int i = _qtyAudio; i >= 1; i--)
			{
				DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
				col.Name = "colAudio" + i;
				col.HeaderText = _dicLanguageGrv[col.Name.Trim()];//Tên của GearWorking

				//col.HeaderText = "Âm thanh " + i;//Tên của GearWorking
												 //col.Tag = TextUtils.ToString(dtGearWorking.Rows[i]["ID"]) + ";" + TextUtils.ToString(dtGearWorking.Rows[i]["SortOrder"]);//Cái này để lưu lại ID của GearWorking
												 //col.DataPropertyName = i.ToString();//cho stt vào
				col.DataPropertyName = "audio";
				col.Tag = i;
				col.Width = 50;
				col.SortMode = DataGridViewColumnSortMode.NotSortable;
				col.DefaultCellStyle.ForeColor = Color.Blue;
				grvData.Columns.Insert(colResult1.Index + 1, col);
			}

			for (int i = _qtyImage; i >= 1; i--)
			{
				DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
				col.Name = "colImage" + i;
				col.HeaderText = _dicLanguageGrv[col.Name.Trim()];//Tên của GearWorking

				//col.HeaderText = "Ảnh " + i;//Tên của GearWorking
											//col.Tag = TextUtils.ToString(dtGearWorking.Rows[i]["ID"]) + ";" + TextUtils.ToString(dtGearWorking.Rows[i]["SortOrder"]);//Cái này để lưu lại ID của GearWorking
				col.DataPropertyName = "image";//cho stt vào
				col.Tag = i;
				col.Width = 50;
				col.SortMode = DataGridViewColumnSortMode.NotSortable;
				col.DefaultCellStyle.ForeColor = Color.Blue;
				grvData.Columns.Insert(colResult1.Index + 1 + _qtyAudio, col);

			}

			colResult1.HeaderText = _dicLanguageGrv[colResult1.Name.Trim()];
			colQtyLap.HeaderText = _dicLanguageGrv[colQtyLap.Name.Trim()];
			// vẽ đường max rung động thuận nghịch
			forwardVibrationMax = new List<double>();
			for (int i = 0; i < 50; i++)
			{
				forwardVibrationMax.Add(_maxValueForward);
			}

			// vẽ đường max rung động thuận nghịch
			backwardVibrationMax = new List<double>();
			for (int i = 0; i < 50; i++)
			{
				backwardVibrationMax.Add(_maxValueBackward);
			}
			_seriesForwardMax.Points.Clear();
			_seriesBackwardMax.Points.Clear();

			if (forwardVibrationMax.Count > 0)
				addPointToSeries(forwardVibrationMax, _seriesForwardMax);
			if (backwardVibrationMax.Count > 0)
				addPointToSeries(backwardVibrationMax, _seriesBackwardMax);


			grvData.Rows[0].ReadOnly = true;
			grvData.Rows[1].ReadOnly = true;
			grvData.Rows[2].ReadOnly = true;
			grvData.Rows[3].ReadOnly = true;

			return true;

		}


		private void txt_TextChanged(object sender, EventArgs e)
		{
			TextBox txt = (TextBox)sender;
			if (string.IsNullOrWhiteSpace(txt.Text.Trim()))
			{
				txt.BackColor = _mauHongNhat;
			}
			else
			{
				txt.BackColor = Color.White;
			}
		}
		/// <summary>
		/// F12 is Save
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// 


		private bool ValidateData()
		{
			if (txtConfirmer.Text.Trim() == "")
			{
				MessageBox.Show("Bạn chưa nhập người xác nhận", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			if (txtBatch.Text.Trim() == "")
			{
				MessageBox.Show("Bạn chưa nhập Lô lot", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			if (txtOrderNo.Text.Trim() == "")
			{
				MessageBox.Show("Bạn chưa nhập Order", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			if (txtHYP.Text.Trim() == "")
			{
				MessageBox.Show("Bạn chưa nhập HYP", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			if (cboGear.SelectedIndex == -1)
			{
				MessageBox.Show("Bạn chưa nhập Gear", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			if (TextUtils.ToInt(txtQty.Text.Trim()) == 0)
			{
				MessageBox.Show("Bạn chưa nhập Số lượng", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			if (txtWorkerName.Text.Trim() == "")
			{
				MessageBox.Show("Bạn chưa nhập Người gia công", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}
		private void saveDataChecking()
		{

			Cursor = Cursors.WaitCursor;
			// thực hiện lấy giá trị trên form
			int rowCount = grvData.Rows.Count;
			if (_deleteTestler)
			{
				Expression exp = new Expression("OrderCode", txtOrderNo.Text.Trim());
				TestlerBO.Instance.DeleteByExpression(exp);
			}
			for (int i = 0; i < _countGearWorking; i++)
			{
				Thread.Sleep(20);
				DataGridViewColumn col = grvData.Columns[i + 3];
				string[] arr = col.Tag.ToString().Split(';');

				for (int j = 0; j < rowCount; j++)
				{
					if (j < 4) continue;
					string hyp = TextUtils.ToString(grvData.Rows[j].Cells[colHyp.Index].Value);
					string realValue = TextUtils.ToString(grvData.Rows[j].Cells[col.Index].Value);
					//if (string.IsNullOrWhiteSpace(hyp)) continue;
					if (realValue.Trim() == "N" || realValue.Trim() == "") continue;

					TestlerModel obj = new TestlerModel();
					obj.OrderCode = txtOrderNo.Text.Trim();
					obj.HypCode = txtHYP.Text.Trim();
					obj.GearCode = cboGear.Text.Trim();
					obj.Qty = TextUtils.ToInt(txtQty.Text.Trim());
					obj.WorkerCode = txtWorkerName.Text.Trim();
					if (_arrDateLR[j] == new DateTime())
					{
						_arrDateLR[j] = DateTime.Now;
					}
					obj.DateLR = _arrDateLR[j];
					obj.Batch = txtBatch.Text.Trim();
					obj.Confirmer = txtConfirmer.Text.Trim();
					obj.SttStart = _sttStart;
					obj.TesterName = lbltester.Text.Trim();

					obj.GearWorkingID = TextUtils.ToInt(arr[0]);
					obj.SortOrder = TextUtils.ToInt(arr[1]);
					//obj.GearWorkingName = TextUtils.ToString(col.HeaderText);
					obj.GearWorkingName = TextUtils.ToString(_dicHeader[col.Name.Trim()]);
					obj.MinValue = TextUtils.ToDecimal(grvData.Rows[2].Cells[col.Index].Value);
					obj.MaxValue = TextUtils.ToDecimal(grvData.Rows[1].Cells[col.Index].Value);
					obj.DefaultValue = TextUtils.ToDecimal(grvData.Rows[0].Cells[col.Index].Value);
					obj.TanSuat = TextUtils.ToString(grvData.Rows[3].Cells[col.Index].Value);

					obj.Hyp = hyp;
					obj.Gear = TextUtils.ToString(grvData.Rows[j].Cells[colGear.Index].Value);
					obj.RealValue = TextUtils.ToDecimal(grvData.Rows[j].Cells[col.Index].Value);

					obj.Result = TextUtils.ToString(grvData.Rows[j].Cells[colResult1.Index].Value);
					obj.QtyLap = TextUtils.ToInt(grvData.Rows[j].Cells[colQtyLap.Index].Value);
					obj.STT = TextUtils.ToInt(grvData.Rows[j].Cells[colNumber.Index].Value);
					TestlerBO.Instance.Insert(obj);
				}
			}
			Cursor = Cursors.Default;
		}
		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grvData.CurrentCell = grvData[0, 0];
			if (!ValidateData()) return;

			// hiện thị messagebox về việc lưu dữ liệu
			DialogResult rs = MessageBox.Show("Bạn muốn lưu dữ liệu?", "Lưu dữ liệu", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (rs == DialogResult.No) return;
			saveDataChecking();

			txtOrderNo.Text = "";
			cboGear.SelectedIndex = -1;
			txtHYP.Text = "";
			txtQty.Text = "";
			txtConfirmer.Text = "";
			txtBatch.Text = "";

			grvData.Rows.Clear();

			txtOrderNo.Focus();
		}

		/// <summary>
		/// Sự kiện khi next ô tiếp theo khi đã điền dữ liệu vào ô trên grid
		/// created by: nguyenvan.thao
		/// created date: 25.12.2019
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>

		private void grvData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 4) return;   //Bỏ 4 dòng đầu đi không nhảy nhót gì cả
										  //if (e.ColumnIndex == colCheckEye11.Index || e.ColumnIndex == colCheckEye12.Index || e.ColumnIndex == colQtyLap.Index) return;

			string value = TextUtils.ToString(grvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);

			if (string.IsNullOrWhiteSpace(value))
			{
				grvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = _mauHongNhat;
			}
			else
			{
				if (value == "N") return;
				if (e.ColumnIndex == colHyp.Index)
				{
					if (!value.ToUpper().Contains(txtHYP.Text.ToUpper().Trim()))
					{
						grvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Red;
						grvData.Rows[e.RowIndex].Cells[colResult1.Index].Style.BackColor = Color.Red;
						grvData.Rows[e.RowIndex].Cells[colResult1.Index].Value = "NG";
					}
					else
					{
						grvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
					}
				}
				if (e.ColumnIndex == colGear.Index)
				{
					if (!value.ToUpper().Contains(cboGear.Text.ToUpper().Trim()))
					{
						grvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Red;
						grvData.Rows[e.RowIndex].Cells[colResult1.Index].Style.BackColor = Color.Red;
						grvData.Rows[e.RowIndex].Cells[colResult1.Index].Value = "NG";
					}
					else
					{
						grvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
					}
				}
				if (e.ColumnIndex > 2 && e.ColumnIndex < colResult1.Index)
				{
					decimal min = TextUtils.ToDecimal(grvData.Rows[1].Cells[e.ColumnIndex].Value);
					decimal max = TextUtils.ToDecimal(grvData.Rows[2].Cells[e.ColumnIndex].Value);
					decimal currentValue = TextUtils.ToDecimal(value);
					if (currentValue > max || currentValue < min)
					{
						grvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Red;
						grvData.Rows[e.RowIndex].Cells[colResult1.Index].Style.BackColor = Color.Red;
						grvData.Rows[e.RowIndex].Cells[colResult1.Index].Value = "NG";
					}
					else
					{
						grvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;

					}
				}
				if (e.ColumnIndex == colResult1.Index)
				{
					if (value == "OK")
					{
						grvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
					}
					else
					{
						grvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Red;
					}
				}
			}
			// gọi hàm check ok/ng, đổi màu ô kết quả
			checkOKNG(e.RowIndex, e.ColumnIndex);

		}

		private void checkOKNG(int rowIndex, int colIndex)
		{
			if (rowIndex < 4 || colIndex < 3 || colIndex >= colResult1.Index) return;
			int countColorRed = 0;
			int countColorPink = 0;
			int countColorWhite = 0;
			for (int i = colGear.Index + 1; i < colResult1.Index - 1; i++)
			{
				if (grvData.Rows[rowIndex].Cells[i].Style.BackColor == Color.Red) countColorRed++;
				if (grvData.Rows[rowIndex].Cells[i].Style.BackColor == _mauHongNhat) countColorPink++;
				if (grvData.Rows[rowIndex].Cells[i].Style.BackColor == Color.White) countColorWhite++;
			}
			if (countColorRed > 0)
			{
				grvData.Rows[rowIndex].Cells[colResult1.Index].Style.BackColor = Color.Red;
				grvData.Rows[rowIndex].Cells[colResult1.Index].Value = "NG";
                return;
			}
			if (countColorPink > 0)
			{
				grvData.Rows[rowIndex].Cells[colResult1.Index].Style.BackColor = _mauHongNhat;
				grvData.Rows[rowIndex].Cells[colResult1.Index].Value = "";
				return;
			}
			grvData.Rows[rowIndex].Cells[colResult1.Index].Style.BackColor = Color.White;
			grvData.Rows[rowIndex].Cells[colResult1.Index].Value = "OK";

		}

		/// <summary>
		/// Sự kiện dùng để xem lại ảnh khi muốn check bằng mắt thường
		/// created by: nvthao
		/// created date: 25.12.2019
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// 

		private void grvData_CellClick(object sender, DataGridViewCellEventArgs e)
		{						
			try
			{
				_fileName = "";
				_colName = "";
				_fileName = getPathFile(grvData.CurrentCell);
				_colName = grvData.CurrentCell.OwningColumn.Name;
				if (_fileName == "") return;
				_sendFileName(_fileName, _colName);
				if (e.ColumnIndex > colResult1.Index + _qtyAudio && e.ColumnIndex < colQtyLap.Index)
				{
					string pfm = Path.Combine(_pathImage, _fileName+".jpg");
					if (File.Exists(pfm))
					{
						Process.Start(pfm);
					}
				}
				else if (e.ColumnIndex > (colResult1.Index) && e.ColumnIndex <= colResult1.Index + _qtyAudio)
				{
					string pfo = "";
					pfo = Path.Combine(_pathAudio, _fileName + ".wav");

					
					if (File.Exists(pfo))
					{
						Process.Start(pfo);
					}
				}

			}
			catch
			{

			}
		}


		private void button1_Click(object sender, EventArgs e)
		{
			DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
			col.HeaderText = "Hi there";//Tên của GearWorking
			col.Tag = 0;//Cái này để lưu lại ID của GearWorking
			col.DataPropertyName = "";//cho stt vào
			col.Width = 90;
			col.SortMode = DataGridViewColumnSortMode.NotSortable;
			grvData.Columns.Insert(3, col);
		}

		ArrayList _arrGear;
		int _qtyImage;
		int _qtyAudio;
		string _hypCode = "";
		private void txtHYP_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Enter) return;
			// Lẩy ra danh sách hạng mục cần kiểm tra

			grvData.Rows.Clear();

			_hypCode = txtHYP.Text.Trim();
			if (_hypCode == "")
			{
				MessageBox.Show("HYP không được để trống");
				return;
			}
			loadGear(_hypCode);
			txtQty.Focus();

		}

		private void loadGear(string hypCode)
		{
			Expression exp = new Expression("HYP", hypCode);
			ArrayList arr = GearBO.Instance.FindByExpression(exp);
			if (arr.Count > 0)
			{
				GearModel gear = arr[0] as GearModel;
				_arrGear = new ArrayList();
				_arrGear.Add(TextUtils.ToString(gear.Gear1));
				_arrGear.Add(TextUtils.ToString(gear.Gear2));
				_arrGear.Add(TextUtils.ToString(gear.Gear3));

				if (_arrGear.Count > 0)
				{
					cboGear.DataSource = _arrGear;
					cboGear.SelectedIndex = 0;
				}
				else
				{
					cboGear.SelectedIndex = -1;
				}
			}
			

		}

		int _tuneParam = 0;
		int _sttStart = 0;
		/// <summary>
		/// Sinh số lượng sản phẩm cần kiểm tra
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtQty_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Enter) return;


			int qty = TextUtils.ToInt(txtQty.Text.Trim());
			if (qty == 0)
			{
				return;
			}
			// trừ đi 4 dòng đầu để stt bắt đầu từ 0
			if (qty == 5) _tuneParam = -4;
			else if(qty != 5 && qty >= 0)
			{
                frmStt frm = new frmStt();
                frm.ShowDialog();
                if (frm.DialogResult == DialogResult.OK)
                {
                    _sttStart = frm._SttStart;
                    _tuneParam = (_sttStart - 4);
                }
                else
                {
                    _tuneParam = -4;
                }
            }
			
			
			if (!GenerateByHpy(qty)) return;


			//remove các dòng đã tạo nếu có
			if (grvData.Rows.Count > 4)
			{
				for (int i = grvData.Rows.Count - 1; i > 3; i--)
				{
					grvData.Rows.RemoveAt(i);
				}
			}

			// Tạo ra tương ứng các dòng trên grid bằng giá trị số lượng của ô nhập
			//grvData.Rows.Add(qty);
			if (qty == 5)
			{
				qty += 1;
			}
			_arrDateLR = new DateTime[qty+4];
			for (int i = 0; i < qty+4; i++)
			{
				_arrDateLR[i] = new DateTime();
			}
			grvData.Rows.Add(qty);

			//Thêm số thứ tự + Bôi màu cho các ô theo tần suất check
			for (int i = 0; i < grvData.Rows.Count; i++)
			{
				if (i < 4) continue;
				grvData.Rows[i].Cells[0].Value = i + _tuneParam;
				grvData.Rows[i].Cells[colQtyLap.Index].Value = 0;

				int countXam = 0;
				for (int j = 1; j < grvData.Columns.Count; j++)
				{
					
					grvData.Rows[i].Cells[j].ReadOnly = true;

					string tagValue = TextUtils.ToString(grvData.Columns[j].Tag);
					//nếu là những cột tự sinh, là mục kiểm tra
					if (!string.IsNullOrEmpty(tagValue))
					{
						if (qty <= 6)
						{

							if (j <= colResult1.Index)
							{
								grvData.Rows[i].Cells[j].Style.BackColor = _mauHongNhat;
								if (_isLogin) grvData.Rows[i].Cells[j].ReadOnly = false;

							}

						}
						else
						{
							string tansuat = TextUtils.ToString(grvData.Rows[3].Cells[j].Value);
							if (tansuat.Contains("/"))
							{
								string[] arr = tansuat.Split('/');
								int moc = TextUtils.ToInt(arr[1]);
								// nếu vị trí hàng chia hết cho tần suất. thực hiện đổi màu nền tại hàng đó là LightBlue
								if ((i - 4) % moc == 0)
								{
									grvData.Rows[i].Cells[j].Style.BackColor = _mauHongNhat;
									if (_isLogin) grvData.Rows[i].Cells[j].ReadOnly = false;

								}
								else
								{
									grvData.Rows[i].Cells[j].Value = "N";
									grvData.Rows[i].Cells[j].Style.BackColor = _mauXam;
									grvData.Rows[i].Cells[j].Style.ForeColor = _mauXam;
									grvData.Rows[i].Cells[j].Selected = false;
									countXam++;
								}
							}
						}

					}
					else
					{
						if (j == colHyp.Index || j == colGear.Index || j == colResult1.Index)
						{
							grvData.Rows[i].Cells[j].Style.BackColor = _mauHongNhat;
							if (_isLogin) grvData.Rows[i].Cells[j].ReadOnly = false;

						}
					}
				}
				if (countXam == _countGearWorking)
				{
					grvData.Rows[i].Cells[1].Style.BackColor = _mauXam;
					grvData.Rows[i].Cells[1].Selected = false;

					grvData.Rows[i].Cells[2].Style.BackColor = _mauXam;
					grvData.Rows[i].Cells[2].Selected = false;

					grvData.Rows[i].Cells[colResult1.Index].Style.BackColor = _mauXam;
					grvData.Rows[i].Cells[colResult1.Index].Selected = false;

					for (int h = 1; h <= _qtyImage; h++)
					{
						grvData.Rows[i].Cells["colImage" + h].Style.BackColor = _mauXam;
					}

					for (int h = 1; h <= _qtyAudio; h++)
					{
						grvData.Rows[i].Cells["colAudio" + h].Style.BackColor = _mauXam;
					}


				}
				else
				{

				}
			}

			grvData.Focus();
			setFocusByLoadOrder();
		}

		private void txtGear_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				txtQty.Focus();
			}
		}

		private void txtWorkerName_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				txtOrderNo.Focus();
			}
		}


		
        //F8 Lap lai
        bool _isLapLai = false;
        DataGridViewRow _SelectedRow = new DataGridViewRow();

        private void lapLaiConfirm()
        {
			try
			{
				DataGridViewRow currentRow = grvData.CurrentRow;
				int rowIndex = grvData.CurrentRow.Index;
				_SelectedRow = currentRow;
				if (!_isLapLai)
				{
					_isLapLai = true;
					DialogResult rs = MessageBox.Show("Bạn có muốn LAP lại không ?", "Xác nhận", MessageBoxButtons.YesNo);
					if (rs == DialogResult.No)
					{
						_isLapLai = false;
						SendKeys.Send("{Up}");
						return;
					}
					saveLapLai(rowIndex);
					removeValueLap(rowIndex);
					grvData.CurrentCell = grvData.Rows[rowIndex].Cells[colGear.Index];
					_isLapLai = false;
					SendKeys.Send("{Up}");
				}
			}
			catch { }
           
        }

        private void removeValueLap(int rowIndex)
        {
            for (int i = colGear.Index+1; i < colQtyLap.Index; i++)
            {
                if (grvData.Rows[rowIndex].Cells[i].Style.BackColor != _mauXam)
                {
                    grvData.Rows[rowIndex].Cells[i].Value = "";

                }
            }
            int qtylap = TextUtils.ToInt(grvData.Rows[rowIndex].Cells[colQtyLap.Index].Value);
            grvData.Rows[rowIndex].Cells[colQtyLap.Index].Value = TextUtils.ToString((qtylap + 1));
        }
        private void saveLapLai(int rowIndex)
        {
            if (txtConfirmer.Text.Trim() == "")
            {
                MessageBox.Show("Bạn chưa nhập người xác nhận", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtBatch.Text.Trim() == "")
            {
                MessageBox.Show("Bạn chưa nhập Lô lot", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtOrderNo.Text.Trim() == "")
            {
                MessageBox.Show("Bạn chưa nhập Order", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtHYP.Text.Trim() == "")
            {
                MessageBox.Show("Bạn chưa nhập HYP", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cboGear.Text.Trim() == "")
            {
                MessageBox.Show("Bạn chưa nhập Gear", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (TextUtils.ToInt(txtQty.Text.Trim()) == 0)
            {
                MessageBox.Show("Bạn chưa nhập Số lượng", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtWorkerName.Text.Trim() == "")
            {
                MessageBox.Show("Bạn chưa nhập Người gia công", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // thực hiện lấy giá trị trên form
            for (int i = 0; i < _countGearWorking; i++)
            {
                DataGridViewColumn col = grvData.Columns[i + 3];
                TestlerLapModel obj = new TestlerLapModel();
                obj.OrderCode = txtOrderNo.Text.Trim();
                obj.HypCode = txtHYP.Text.Trim();
                obj.GearCode = cboGear.Text.Trim();
                obj.Qty = TextUtils.ToInt(txtQty.Text.Trim());
                obj.WorkerCode = txtWorkerName.Text.Trim();
                obj.Batch = txtBatch.Text.Trim();
                obj.Confirmer = txtConfirmer.Text.Trim();
                obj.SttStart = _sttStart;
				if (_arrDateLR[rowIndex] == new DateTime())
				{
					_arrDateLR[rowIndex] = DateTime.Now;
				}
				obj.DateLR = _arrDateLR[rowIndex];
				obj.TesterName = lbltester.Text.Trim();
				

                string[] arr = col.Tag.ToString().Split(';');
                obj.GearWorkingID = TextUtils.ToInt(arr[0]);
                obj.SortOrder = TextUtils.ToInt(arr[1]);
                //obj.GearWorkingName = TextUtils.ToString(col.HeaderText);
				obj.GearWorkingName = TextUtils.ToString(_dicHeader[col.Name.Trim()]);
				obj.MinValue = TextUtils.ToDecimal(grvData.Rows[2].Cells[col.Index].Value);
                obj.MaxValue = TextUtils.ToDecimal(grvData.Rows[1].Cells[col.Index].Value);
                obj.DefaultValue = TextUtils.ToDecimal(grvData.Rows[0].Cells[col.Index].Value);
                obj.TanSuat = TextUtils.ToString(grvData.Rows[3].Cells[col.Index].Value);

                obj.Hyp = TextUtils.ToString(grvData.Rows[rowIndex].Cells[colHyp.Index].Value);
                obj.Gear = TextUtils.ToString(grvData.Rows[rowIndex].Cells[colGear.Index].Value);
                obj.RealValue = TextUtils.ToDecimal(_SelectedRow.Cells[col.Index].Value);
                obj.STT = TextUtils.ToInt(_SelectedRow.Cells["colQtyLap"].Value);
                obj.Result = TextUtils.ToString(_SelectedRow.Cells["colResult1"].Value);
                TestlerLapBO.Instance.Insert(obj);
            }
        }

        private void lapToolStripMenuItem_Click(object sender, EventArgs e)
		{

            lapLaiConfirm();

        }

		private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
		}

		private string getPathFile(DataGridViewCell e)
		{
			try
			{
				if (e.RowIndex < 4) return "";
				if (e.ColumnIndex <= colNumber.Index && e.ColumnIndex >= colQtyLap.Index) return "";
				string order = txtOrderNo.Text.Trim();
				string filename = "";
				if (order == "")
				{
					MessageBox.Show("Bạn chưa nhập order");
					return "";
				}
				string stt = TextUtils.ToString(grvData.Rows[e.RowIndex].Cells[colNumber.Index].Value);
				string qtyLap = TextUtils.ToString(grvData.Rows[e.RowIndex].Cells[colQtyLap.Index].Value);
				DataGridViewColumn col = grvData.Columns[e.ColumnIndex];
				string vtri = TextUtils.ToString(col.Tag);
				filename = order + "_" + stt + "_" + qtyLap + "_" + vtri;
				/*if (e.ColumnIndex > colResult1.Index + _qtyAudio && e.ColumnIndex < colQtyLap.Index)
				{
					filename = order + "_" + stt + "_" + qtyLap + "_" + vtri + ".jpg";
					return filename;
				}

				if (e.OwningColumn.HeaderText.StartsWith("Độ rung động"))
					filename = order + "_" + stt + "_" + qtyLap;*/
				return filename;
			}
			catch
			{
				return "";
			}
		}


		private void cboGear_TextChanged(object sender, EventArgs e)
		{
			ComboBox cbo = (ComboBox)sender;
			if (string.IsNullOrWhiteSpace(cbo.Text.Trim()))
			{
				cbo.BackColor = _mauHongNhat;
			}
			else
			{
				cbo.BackColor = Color.White;
			}
		}


		private void grvData_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				if (!_isCatchKey)
				{
					_isCatchKey = true;
					return;
				}

				// get filename in cell
				_fileName = ""; _colName = "";


				int col = grvData.CurrentCell.ColumnIndex;
				int row = grvData.CurrentCell.RowIndex;
				if (col <= colNumber.Index || col >= colQtyLap.Index || row < 4)
				{
					return;
				}
				if (e.KeyCode == Keys.Left)
				{
					_fileName = getPathFile(grvData[col - 1, row]);
					_colName = grvData[col - 1, row].OwningColumn.Name;

				}
				if (e.KeyCode == Keys.Right)
				{
					_fileName = getPathFile(grvData[col + 1, row]);
					_colName = grvData[col + 1, row].OwningColumn.Name;

				}
				if (e.KeyCode == Keys.Up)
				{
					_fileName = getPathFile(grvData[col, row + 1]);
					_colName = grvData[col, row +1].OwningColumn.Name;

				}
				if (e.KeyCode == Keys.Down)
				{
					_fileName = getPathFile(grvData[col, row - 1]);
					_colName = grvData[col, row-1].OwningColumn.Name;

				}

				if (_fileName == "") return;
				_sendFileName(_fileName, _colName);
			}
			catch { }
			
		}

		private void txtBatch_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) txtHYP.Focus();

		}

		private void txtConfirmer_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) txtBatch.Focus();
		}

		private void txtConfirmer_TextChanged(object sender, EventArgs e)
		{
			TextBox txt = (TextBox)sender;
			if (string.IsNullOrWhiteSpace(txt.Text.Trim()))
			{
				txt.BackColor = _mauHongNhat;
			}
			else
			{
				txt.BackColor = Color.White;
			}
		}

		private void txtBatch_TextChanged(object sender, EventArgs e)
		{
			TextBox txt = (TextBox)sender;
			if (string.IsNullOrWhiteSpace(txt.Text.Trim()))
			{
				txt.BackColor = _mauHongNhat;
			}
			else
			{
				txt.BackColor = Color.White;
			}
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			_deleteTestler = true;

			if (!ValidateData()) return;
			saveDataChecking();
		}

		private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

		}

		bool _isLogin = false;
		private void loginToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!_isLogin)
			{
				frmLogin frm = new frmLogin();
				frm.ShowDialog();
				if (frm.DialogResult == DialogResult.OK)
				{
					_isLogin = true;
					lblLogin.Text = "F9: Đăng xuất";
					unlockToEdit();
				}
			}
			else
			{
				DialogResult rs = MessageBox.Show("Bạn muốn đăng xuất?", "Thông báo", MessageBoxButtons.YesNo);
				if (rs == DialogResult.No) return;
				_isLogin = false;
				lblLogin.Text = "F9: Đăng nhập";
				lockCellInGrid();

			}
		}

		private void unlockToEdit()
		{
			try
			{
				for (int i = 0; i < grvData.Rows.Count; i++)
				{
					if (i < 4) continue;
					for (int j = colGear.Index + 1; j < colResult1.Index; j++)
					{
						if (grvData.Rows[i].Cells[j].Style.BackColor != _mauXam)
						{
							grvData.Rows[i].Cells[j].ReadOnly = false;
						}
					}

				}
			}
			catch
			{

			}
		}

		private void lockCellInGrid()
		{
			try
			{
				for (int i = 0; i < grvData.Rows.Count; i++)
				{
					if (i < 4) continue;
					for (int j = colGear.Index + 1; j < colResult1.Index; j++)
					{
						if (grvData.Rows[i].Cells[j].Style.BackColor != _mauXam)
						{
							grvData.Rows[i].Cells[j].ReadOnly = true;
						}
					}

				}
				setFocusByLoadOrder();
			}
			catch
			{

			}
		}
		private void chartToolStripMenuItem_Click(object sender, EventArgs e)
		{

			if (tableLayoutPanel2.Visible == true)
			{
				tableLayoutPanel2.Visible = false;
                grvData.Dock = DockStyle.Fill;
			}
			else
			{
				tableLayoutPanel2.Visible = true;
                grvData.Dock = DockStyle.Top;
			}
		}

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void grvData_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            
        }
        //DataGridViewCellStyle st = new DataGridViewCellStyle()
        private void grvData_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
           
        }

		private void changeLanguageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			
		}

		private void cboLanguage_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cboLanguage.SelectedIndex == -1) return;

			if (cboLanguage.SelectedIndex ==0) _language = "VN";
			else _language = "JP";
			changeLanguage();

		}

		
	}
	public class GeneralObject
	{
		public string Confirmer { get; set; }
		public string Batch { get; set; }
		public string OrderCode { get; set; }
		public string HypCode { get; set; }
		public string GearCode { get; set; }
		public string WorkerName { get; set; }
		public int SttStart { get; set; }
		public string Qty { get; set; }
	}
}

