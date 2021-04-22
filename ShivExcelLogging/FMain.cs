using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ActUtlTypeLib;
//using Excel = Microsoft.Office.Interop.Excel;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;

namespace BMS
{
    
    public partial class FMain : Form
    {
        #region // Khai báo
        ActUtlType plcFX3G;
        //Excel.Application myExcel;
        short D100;
        int countPulse = 0, countNumberOfReadData = 0;
        Thread plcThread, dataThread;
        bool readLeft = false, readRight = false, dataLogging = false;
        string readDoneConfirm;
        int indexRow, indexCol, numberRead = 0;
        int EXCEL_CHART_START_ROW = 4;
        int EXCEL_CHART_START_COLUUM = 2;
        string productName;
        const int LIMIT_NUMBER = 25;
        private Capture formCap;
        private int tempDirection;

        short[] convertIndex = new short[50];
        short[] X0toX5 = new short[10];
        short tempCountPulse = 0;

        object misValue = System.Reflection.Missing.Value;
        private bool conditionRunCam;

        public MessageBoxVisual MessageOKForm = new MessageBoxVisual();
        private bool bitForward;
        private bool bitBackward;
        private bool bitManual = false;
        private short countPulsetemp;
        private bool bitCaptureOpen;
        private string firstString;
        private string secondString;
        private string thirdString;
        private string stringKhehoTinh;
        Dictionary<string, bool> currentPLCBit = new Dictionary<string, bool>();
        private SerialPort COMSylvacKheho;
        private SerialPort COMSylvacDodao01, COMSylvacDodao02, COMSylvacDodao03;
        private bool excelUsing;
        private bool formLock;
        private string imageCaptureLink;
        public string _PathError;
        public double _factorVibration;
        bool _isFinishRD = false;
        public string _FileName = "";
        public string _ColName = "";
        private string _pathTempAudio = "";
        private string _pathTempImg = "";



        string _pathImage = @"D:\Testler\Image";
        string _pathAudio = @"D:\Testler\Audio";
        string _pathSeverImage = @"Testler\Image";
        string _pathSeverAudio = @"Testler\Audio";
        #endregion
        /// <summary>
        /// Hàm khởi tạo FMain
        /// </summary>
        /// 
        
        public delegate void TransferData(string[] value, string type);
        public delegate void TransferDataRDBackward(double value);
        public delegate void TransferDataRDForward(double value);

        public delegate void TransferStatusRD(int type, bool isStartDraw);

        public TransferData _transfer;
        public TransferDataRDBackward _transferRDBackward;
        public TransferDataRDForward _transferRDForward;

        public TransferStatusRD _transferStatusStart;
        string[] _com = { };

        private bool SaveImgSucc;


        public FMain()
        {
            
            InitializeComponent();
            InitializeValue();

        }

        private void FMain_Load(object sender, EventArgs e)
        {
            // load com
            string[] arr = File.ReadAllLines(Path.Combine(Application.StartupPath, "comport.txt"));
            _com = arr[1].Split('|');
            LoadSylvacCOM();
            StartPLCThread();

            if (!bitManual)
            {
                Thread.Sleep(500);
                RecheckPlcConnection();
                StartThreadToCollectionData();
            }
            else
            {
                btnStart.BackColor = Color.MidnightBlue;
                btnStart.ForeColor = Color.White;
                bitManual = false;
            }
           

            //load hệ số rung động
            string strFactor = File.ReadAllText(Path.Combine(Application.StartupPath, "factorVibration.txt"));
            _factorVibration = TextUtils.ToDouble(strFactor);
            // Khởi tạo lại hiển thị nút nhấn Manual
            btnManual.BackColor = Color.White;
            btnManual.ForeColor = Color.Black;
            // Tắt trạng thái Manual
            bitManual = false;
            // Load form Tester
        }


        // nhận file name ảnh và audio từ bên giao diện
        public void ReceiveFileName(string filename, string colName)
        {
            _FileName = filename;
            _ColName = colName;

            if (!string.IsNullOrWhiteSpace(_FileName) && _ColName == "col4")
                _isFinishRD = false;


        }

        /// <summary>
        /// Tạo và chạy Thread lấy dữ liệu PLC
        /// </summary>
        private void StartPLCThread()
        {
            plcThread = new Thread(plcCycleReadAndWriteValue);
            plcThread.IsBackground = true;
            plcThread.Start();
        }

        /// <summary>
        /// Khởi tạo giá trị khi mở ứng dụng
        /// - Đóng App Excel cũ
        /// - Kết nối COM Sylvac
        /// - Tạo mới ứng dụng Excle
        /// - Khởi tạo giao diện
        /// - Khởi tạo mảng vị trí lưu trữ
        /// - Tạo thư mục lưu file Log
        /// </summary>
        private void InitializeValue()
        {
            LoadDefaultDisplay();
        }

        /// <summary>
        /// Tạo kết nối COM đến thiết bị Sylvac
        /// </summary>
        private void LoadSylvacCOM()
        {
            COMSylvacKheho = new SerialPort(_com[0].Trim(), 4800, Parity.Even, 7, StopBits.Two);
            COMSylvacKheho.DtrEnable = true;
            COMSylvacKheho.Open();

            // Khai báo cổng COM mới cho thiết bị Sylvac đo độ đảo
            COMSylvacDodao01 = new SerialPort(_com[1].Trim(), 4800, Parity.Even, 7, StopBits.Two);
            COMSylvacDodao01.DtrEnable = true;
            COMSylvacDodao01.Open();

            COMSylvacDodao02 = new SerialPort(_com[2].Trim(), 4800, Parity.Even, 7, StopBits.Two);
            COMSylvacDodao02.DtrEnable = true;
            COMSylvacDodao02.Open();

            COMSylvacDodao03 = new SerialPort(_com[3].Trim(), 4800, Parity.Even, 7, StopBits.Two);
            COMSylvacDodao03.DtrEnable = true;
            COMSylvacDodao03.Open();
        }

        /// <summary>
        /// Khởi tạo các giá trị trong giao diện hiển thị về mặc định -Load những giá trị sẵn có trong Setting
        /// </summary>
        private void LoadDefaultDisplay()
        {
           
            pnlMainMenu.BringToFront();
            lblNumber.Text = "    Ready!";
            txtNumberRun.Text = "";
        }

        /// <summary>
        /// Tạo mảng giá trị 0,1,2,3,4,5, 10, 20, 30, 40, ... 200
        /// </summary>

        /// <summary>
        /// Tính toán giá trị hàng, cột của ô Excel, dựa trên giá trị cài đặt
        /// Ví dụ: Giá trị cài đặt là "A1" =>> hàng 1, cột 1
        /// </summary>


        /// <summary>
        /// Chu trình lấy dữ liệu PLC, update mỗi 10ms
        /// </summary>
        private void plcCycleReadAndWriteValue()
        {
           
            // Khai báo kết nối đến PLC, với cổng kết nối plcStationNumber (cài đặt qua Mitsubishi Communication Setup Utility)
            plcFX3G = new ActUtlType();
            plcFX3G.ActLogicalStationNumber = TextUtils.ToInt(_com[4].Trim());
            currentPLCBit.Add("X10", false);
            currentPLCBit.Add("X11", false);
            currentPLCBit.Add("X12", false);
            currentPLCBit.Add("X13", false);
            while (true)
            {
                //CheckPlcOnlineOrNOt();              // Kiểm tra xem có đọc được dữ liệu từ PLC không
                ReadD100AndSaveToArray(); // Lấy dữ liệu D100 lưu vào mảng 20pt

                ReadX0ToX5AndProcess(); // Đọc các giá trị Input của PLC -> PC
                CountNumberOfPulseX0(); // Đếm xung số vòng quay
                ReadNewPLCButtonStatus(); // Đọc giá trị 3 nút nhấn - Chụp ảnh, Đo khe hở, Đo độ đảo
                Thread.Sleep(10);
            }   
        }
        /// <summary>
        /// Kiểm tra điều kiện đang lấy dữ liệu không?
        /// Lấy giá trị độ rung (D100) từ PLC và lưu vào mảng dữ liệu (theo chiều chạy của động cơ được chọn - thuận/ nghịch)
        /// </summary>
        private void ReadD100AndSaveToArray()
        {
            try
            {
                // Có thể thêm Count vào để giảm thời gian lấy mẫu D100
                string zDevice = "D100";
                plcFX3G.ReadDeviceRandom2(zDevice, 1, out D100);
 
                Thread.Sleep(20);
            }
            catch(Exception ex)
            {
                File.AppendAllText(_PathError + "/Error_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt",
                        DateTime.Now.ToString("HH:mm:ss") + ":ReadD100AndSaveToArray(): " + ex.ToString() + Environment.NewLine);
            }
          
        }

        /// <summary>
        /// Đọc số xung từ PLC (D10)
        /// Mỗi xung là 1 vòng quay của động cơ =>> Kiểm soát điều kiện vòng quay của chương trình
        /// Ví dụ: Quay 5 vòng bắt đầu lấy dữ liệu, ...
        /// </summary>
        private void CountNumberOfPulseX0()
        {
            try
            {
                // D10 là số xung vòng quay, được đếm trong PLC
                string zDevice = "D10";
                plcFX3G.ReadDeviceRandom2(zDevice, 1, out countPulsetemp);
                if (tempCountPulse != countPulsetemp)
                {
                    tempCountPulse = countPulsetemp;
                    Invoke(new MethodInvoker(delegate { btnLampP.BackColor = Color.MidnightBlue; }));
                }
                countPulse = (int)countPulsetemp;
                if ((!bitBackward) && (!bitForward)) Invoke(new MethodInvoker(delegate { txtNumberPulse.Text = ""; }));
            }
            catch (Exception ex)
            {
                File.AppendAllText(_PathError + "/Error_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt",
                        DateTime.Now.ToString("HH:mm:ss") + ":CountNumberOfPulseX0(): " + ex.ToString() + Environment.NewLine);
            }
            
        }

        /// <summary>
        /// Đọc các giá trị trạng thái nút nhấn từ PLC =>> thay đổi trạng thái các bit điều khiển của chương trình
        /// </summary>
        private void ReadX0ToX5AndProcess()
        {
            try
            {
                plcFX3G.ReadDeviceRandom2("M10\nM11\nM12\nM13\nM14\nM15", 6, out X0toX5[0]);
                //MessageBox.Show(X0toX5[0].ToString());
                if (X0toX5[1] == 1)
                {
                    bitForward = true;
                    btnLampF.BackColor = Color.MidnightBlue;
                }
                else
                {
                    bitForward = false;
                    if (!readLeft) btnLampF.BackColor = Color.Transparent;
                }
                if (X0toX5[3] == 1)
                {
                    bitBackward = true;
                    btnLampB.BackColor = Color.MidnightBlue;
                }
                else
                {
                    bitBackward = false;
                    if (!readRight) btnLampB.BackColor = Color.Transparent;
                }
                if (X0toX5[4] == 1)
                {
                    btnLampG.BackColor = Color.MidnightBlue;
                }
                else
                {
                    btnLampG.BackColor = Color.Transparent;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(_PathError + "/Error_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt",
                        DateTime.Now.ToString("HH:mm:ss") + ":ReadX0ToX5AndProcess(): " + ex.ToString() + Environment.NewLine);
            }
           
        }

        /// <summary>
        /// Đọc giá trị 3 nút nhấn - Chụp ảnh, Đo khe hở, Đo độ đảo
        /// </summary>
        /// 
        //public delegate void BringToFrontMain();
        //BringToFrontMain _bring;
        //private void bringToFrontMain()
        //{
        //    this.BringToFront();
        //}
        private void ReadNewPLCButtonStatus()
        {
            int buttonRead;
            try
            {
                var iret = plcFX3G.GetDevice("X10", out buttonRead);
                if (buttonRead == 1)
                {
                    Console.WriteLine("Nhan nut X10!");
                    if (!currentPLCBit["X10"])
                    {
                        this.Invoke(new MethodInvoker(delegate
                        {
                            this.StartPosition = FormStartPosition.Manual;
                            this.Top = 650;
                            this.Left = 800;
                        }));
                        currentPLCBit["X10"] = true;

                        wfDodao wftemp1 = new wfDodao();
                        wftemp1.stringDoneDodao += InputDodaoToExcel;
                        wftemp1.ShowDialog();
                        formLock = false;
                    }
                }
                else
                {
                    currentPLCBit["X10"] = false;
                }
                iret = plcFX3G.GetDevice("X11", out buttonRead);
                // Kiểm tra nhấn nút đo Khe hở
                if (buttonRead == 1)
                {

                    Console.WriteLine("Nhan nut X11!");
                    if (!currentPLCBit["X11"])
                    {
                        this.Invoke(new MethodInvoker(delegate
                        {
                            this.StartPosition = FormStartPosition.Manual;
                            this.Top = 650;
                            this.Left = 800;
                        }));
                        currentPLCBit["X11"] = true;
                        wfKheho wftemp = new wfKheho(ref plcFX3G, ref COMSylvacKheho);
                        wftemp.stringDoneKheho += InputKhehoToExcel;
                        wftemp.ShowDialog();
                    }
                }
                else
                {
                    currentPLCBit["X11"] = false;
                }
                // Kiểm tra nhấn nút đo độ đảo
                plcFX3G.GetDevice("X12", out buttonRead);
                if (buttonRead == 1)
                {

                    Console.WriteLine("Nhan nut X12!");
                    if (!currentPLCBit["X12"])
                    {


                        this.Invoke(new MethodInvoker(delegate
                        {

                            this.StartPosition = FormStartPosition.Manual;
                            this.Top = 650;
                            this.Left = 800;
                            // this.BringToFront();
                        }));

                        currentPLCBit["X12"] = true;


                        wfDodaoSylvac wftemp1 = new wfDodaoSylvac(ref plcFX3G, ref COMSylvacDodao01, ref COMSylvacDodao02, ref COMSylvacDodao03);
                        wftemp1.stringDoneDodao += InputDodaoSylvacToExcel;

                        wftemp1.ShowDialog();
                        wftemp1.stringDoneDodao -= InputDodaoSylvacToExcel;
                        wftemp1.Dispose();
                        formLock = false;
                    }
                }
                else
                {
                    currentPLCBit["X12"] = false;
                }
                // Kiểm tra nhấn nút chụp ảnh
                plcFX3G.GetDevice("X13", out buttonRead);
                if (buttonRead == 1)
                {
                    if (!currentPLCBit["X13"])
                    {
                        this.Invoke(new MethodInvoker(delegate
                        {
                            this.StartPosition = FormStartPosition.Manual;
                            this.Top = 650;
                            this.Left = 800;
                        }));

                        currentPLCBit["X13"] = true;
                        // Mở form chụp ảnh, gửi kèm địa chỉ lưu ảnh
                        //string tempDic = @txtLoggingFolder.Text + "\\" + DateTime.Now.ToString("ddMMyyyy") + @"Image\" + productName + "_" + (numberRead + 1).ToString();
                        if (!string.IsNullOrWhiteSpace(_FileName) && _ColName.StartsWith("colImage"))
                        {
                            Capture wftemp2 = new Capture(_pathImage, ref plcFX3G);
                            wftemp2._FileName = _FileName + ".jpg";
                            wftemp2.saveImageComplete += ProcessImageSavedLink;
                            wftemp2.ShowDialog();
                            Console.WriteLine("Nhan nut X13!");
                        }
                        
                    }
                }
                else
                {
                    currentPLCBit["X13"] = false;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(_PathError + "/Error_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt",
                        DateTime.Now.ToString("HH:mm:ss") + ":ReadNewPLCButtonStatus(): " + ex.ToString() + Environment.NewLine);
            }
        }

        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int record(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);
        private void RecordStart()
        {
            try
            {
                //record("close recsound", "", 0, 0);
                record("open new Type waveaudio Alias recsound", "", 0, 0);
                record("record recsound", "", 0, 0);
                
            }
            catch
            {                
            }
        }

        private void RecordStopAndSave(string fileName)
        {
            try
            {
                record("save recsound " + @fileName + ".wav", "", 0, 0);
                record("close recsound", "", 0, 0);
            }catch
            {          
            }
            
        }

        /// <summary>
        /// Ghi du lieu do dao 3 vi tri ra keyboard
        /// </summary>
        /// <param name="doDaoViTri01"></param>
        /// <param name="doDaoViTri02"></param>
        /// <param name="doDaoViTri03"></param>
        /// 

        private void InputDodaoSylvacToExcel(string doDaoViTri01, string doDaoViTri02, string doDaoViTri03)
        {
            string[] data = { doDaoViTri01, doDaoViTri02, doDaoViTri03 };
            this.Invoke(new MethodInvoker(delegate { _transfer(data, ""); }));
        }

        /// <summary>
        /// Điền link ảnh đã lưu vào ô Excel
        /// </summary>
        /// <param name="link"></param>
        /// 
        void uploadFile(string pathFile, string pathServer)
        {
            DocUtils.UploadFile(pathFile, pathServer);
        }
        async void uploadFileToServer(string pathFile, string pathServer)
        {
            await Task.Factory.StartNew(() => uploadFile(pathFile, pathServer));
        }
        private void ProcessImageSavedLink(string link, bool SaveImgSucc)
        {
            if (!SaveImgSucc) return;
            imageCaptureLink = link;
            try
            {               
                string[] data = { "View" };
                this.Invoke(new MethodInvoker(delegate { _transfer(data, ""); }));
                uploadFileToServer(link, _pathSeverImage);             
            }
            catch
            {
            }

        }


        /// <summary>
        /// Kiểm tra PLC có được kết nối hay không bằng cách lấy dữ liệu M8000 từ PLC, nếu mất kết nối =>> gửi message thông báo
        /// </summary>
        private void btnCloseApp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            pnlMainMenu.BringToFront();
            conditionRunCam = false;
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            pnlSetting.BringToFront();
            conditionRunCam = false;
            //threadRunCamera.Abort();
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            //pnlCapture.BringToFront();
            formCap = new Capture(ref plcFX3G);
            formCap.Show();
            
        }

        /// <summary>
        /// Khi nhấn nút Start, chu trình lấy dữ liệu bắt đầu
        /// 1. Mở lại kết nối PLC
        /// 2. Mở ứng dụng Excel
        /// 3. Chạy Thread tổng hợp dữ liệu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Kiểm tra kết nối PLC bằng cách đọc giá trị M8000
        /// Nếu mất kết nối thì thông báo PLC Error
        /// </summary>
        private void RecheckPlcConnection()
        {
            try
            {
                short tempData;
                if (plcFX3G.ReadDeviceRandom2("SM400", 1, out tempData) != 0)
                {
                    var iRet = plcFX3G.Open();
                    if (iRet != 0)
                    {
                        txtNumberRun.Text = "";
                    }
                    else ChageStatusLogging();
                }
                else ChageStatusLogging();
            }
            catch
            {
                MessageBox.Show("PLC Error!. Hãy khởi động lại");
            }
           
        }

        /// <summary>
        /// Đổi màu nút nhấn Start khi quá trình lấy dữ liệu bắt đầu
        /// </summary>
        private void ChageStatusLogging()
        {
            btnStart.BackColor = Color.MidnightBlue;
            btnStart.ForeColor = Color.White;
            dataLogging = true;
        }

        /// <summary>
        /// Khai báo và chạy Thread tổng hợp dữ liệu
        /// </summary>
        private void StartThreadToCollectionData()
        {
            dataThread = new Thread(DataCollection); // chương trình tổng hợp dữ liệu "DataCollection"
            dataThread.Name = "ThData";
            dataThread.IsBackground = true;
            dataThread.Start();
            txtNumberRun.Text = "1";
        }

        /// <summary>
        /// Nút nhấn dừng lấy dữ liệu
        /// Dừng quá trình lấy dữ liệu bằng cách đặt giá trji dataLogging sang OFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            StopDataLoggingIfRunning();
        }

        /// <summary>
        /// Đặt dataLoging sang OFF và reset Trạng thái hiển thị nút Start
        /// </summary>
        private void StopDataLoggingIfRunning()
        {
            if (dataLogging)
            {
                btnStart.BackColor = Color.White;
                btnStart.ForeColor = Color.Black;
                dataLogging = false;
            }
        }

        /// <summary>
        /// Khi nhấn nút Brower thì mở giao điện để tìm đến file Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowser_Click(object sender, EventArgs e)
        {
            // Browser file and save fileIndex to Textbox
            if (openFileDialogExcel.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txtFileIndex.Text = openFileDialogExcel.FileName.ToString();
        }

        /// <summary>
        /// Khi đường dẫn file Excel thay đổi thì Update vào trong Setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFileIndex_TextChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Nếu giá trị nhập trong StartPos thayd dổi, thì tính toán lại hàng và cột trong file Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtStartPos_TextChanged(object sender, EventArgs e)
        {
            if (txtStartPos.Text.Length > 1)
            {
            }
        }

        /// <summary>
        /// Nếu giá trị nhập trong NamePos thay đổi, thì Update lại tên sản phẩm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtNamePos_TextChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Lấy tên mã sản phẩm từ file Excel =>> lưu vào productName
        /// </summary>

        private void FMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //myExcel.ActiveWorkbook.Close(false, misValue, misValue);
            }
            catch { }
        }

        /// <summary>
        /// Chương trình test lưu ảnh Excel - chưa sử dụng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        /// <summary>
        /// Khi nhấn nút Reset => hiển thị thông báo có cho phép reset không
        /// Nếu có, Reset tất cả về mặc định
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Đóng các Thread khi tắt chương trình
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            plcThread.Abort();
            try { dataThread.Abort(); } catch { }
            plcFX3G.Close();
        }

        #region<region> nút nhấn Test chương trình, đã làm ẩn đi
        private void chkTest02_CheckedChanged(object sender, EventArgs e)
        {
            // Change Color of Button Forward Lamp if ReadDone Or Not
            if (chkTestForward.Checked) btnLampF.BackColor = Color.MidnightBlue;
            else if (!readLeft)
                btnLampF.BackColor = Color.Transparent;
        }

        private void chkTest01_CheckedChanged(object sender, EventArgs e)
        {
            // Change Color of Button Backward Lamp if ReadDone Or Not
            if (chkTestBackward.Checked) btnLampB.BackColor = Color.MidnightBlue;
            else if (!readRight)
                btnLampB.BackColor = Color.Transparent;
        }

        #endregion

        /// <summary>
        /// Tổng hợp dữ liệu D100. Số đếm numberRead. Xử lý hiển thị Forward, Backward.
        /// </summary>
        private void DataCollection()
        {
          
            while (true)
            {
                // dataLogging = true sau khi nhấn nút Start, và PLC đang được kết nối
                if (dataLogging)
                {
                    //IfCompleteOneReadCyleProcessData(); // Xử lý khi xong 1 chu kỳ kiểm tra
                    IfReadingDataBackward(); // Xử lý nếu bắt đầu kiểm tra chiều thuận
                    IfReadingDataForward(); // Xử lý nếu bắt đầu kiểm tra chiều nghịch
                }
                Thread.Sleep(500);
            }
           
            
        }

        /// <summary>
        /// Kiểm tra trạng thái đang lấy dữ liệu chiều Nghịch (bitBackward)
        /// Chạy chu trình lấy dữ liệu, khi hoàn thành thì cập nhật readRight = true
        /// </summary>
        /// 
        bool _isRecordingBackward = false;
        bool _isRecordingForward = false;
        private void IfReadingDataBackward()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_FileName) && _ColName == "col4")
                {
                    if (bitBackward && !readRight)
                    {
                        if (readDoneConfirmCheckRight() == "True")
                        {
							readRight = true;
							//readLeft = true;
							// kiểm tra hoàn thành 1 chu kì quay thuận nghịch
						
							// kết thúc ghi âm
							string[] arr = _FileName.Split('_');
                            string fname = arr[0] + "_" + arr[1] + "_" + arr[2] + "_2";
                            _pathTempAudio = Path.Combine(_pathAudio, fname);

                            if (File.Exists(_pathTempAudio+".wav")) File.Delete(_pathTempAudio+".wav");

                            RecordStopAndSave(_pathTempAudio);

                            if (readLeft && readRight)
                            {
                                _lstForward.Sort((a, b) => b.CompareTo(a));
                                _lstBackward.Sort((a, b) => b.CompareTo(a));
                                string[] data = { _lstForward[0].ToString(), _lstBackward[0].ToString() };
                                this.Invoke(new MethodInvoker(delegate { _transfer(data, "RD"); }));
                                _isFinishRD = true;

                                readLeft = false;
                                readRight = false;
                            }
                            try
                            {
								if (File.Exists(_pathTempAudio + ".wav"))
									uploadFileToServer(_pathTempAudio + ".wav", _pathSeverAudio);
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.ToString());
                            }
                       
                            Invoke(new MethodInvoker(delegate () { MessageOKForm.Show(); }));
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                File.AppendAllText(_PathError + "/Error_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt",
                        DateTime.Now.ToString("HH:mm:ss") + ":IfReadingDataBackward(): " + ex.ToString() + Environment.NewLine);
            }
            
        }

        /// <summary>
        /// Thực hiện lấy dữ liệu chiều Nghịch
        /// </summary>
        /// <returns></returns>
        private string readDoneConfirmCheckLeft()
        {
            readDoneConfirm = "False";
            // bắt đầu ghi âm
            RecordStart();
            while (bitForward && readDoneConfirm == "False")
            {
                WaitTo4PulseComplete(); // Đợi động cơ quay 4 vòng
                WaitRead20DataToArray(50, 0); // Đợi lấy đủ 20 giá trị
                
            }

            if (readDoneConfirm == "True") return ("True");
            else return ("False");
        }

        /// <summary>
        /// Kiểm tra trạng thái đang lấy dữ liệu chiều Thuận (bitBackward)
        /// Chạy chu trình lấy dữ liệu, khi hoàn thành thì cập nhật readLeft = true
        /// </summary>
        private void IfReadingDataForward()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_FileName) && _ColName == "col4")
                {
                    if (bitForward && !readLeft)
                    {

                        if (readDoneConfirmCheckLeft() == "True")
                        {
							readLeft = true;
							//readRight = true;
							// kiểm tra 1 chu kì thuận nghịch
							if (readLeft && readRight)
							{
								_lstForward.Sort((a, b) => b.CompareTo(a));
								_lstBackward.Sort((a, b) => b.CompareTo(a));
								string[] data = { _lstForward[0].ToString(), _lstBackward[0].ToString() };
								this.Invoke(new MethodInvoker(delegate { _transfer(data, "RD"); }));
								_isFinishRD = true;

								readLeft = false;
								readRight = false;
							}
							// kết thúc ghi âm
							string[] arr = _FileName.Split('_');
                            string fname = arr[0] + "_" + arr[1] + "_" + arr[2] + "_1";
                            _pathTempAudio = Path.Combine(_pathAudio, fname);

                            if (File.Exists(_pathTempAudio+".wav")) File.Delete(_pathTempAudio+".wav");
                            RecordStopAndSave(_pathTempAudio);                             

                            try
                            {
								if (!File.Exists(_pathTempAudio + ".wav"))
									uploadFileToServer(_pathTempAudio + ".wav", _pathSeverAudio);
                            }
                            catch
                            {

                            }
                        
                           Invoke(new MethodInvoker(delegate () { MessageOKForm.Show(); }));

                        }
                    }

                }

            }
            catch (Exception ex)
            {
                File.AppendAllText(_PathError + "/Error_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt",
                        DateTime.Now.ToString("HH:mm:ss") + ":IfReadingDataForward(): " + ex.ToString() + Environment.NewLine);
            }
            
        }

        /// <summary>
        /// Thực hiện lấy dữ liệu chiều Thuận
        /// </summary>
        /// <returns></returns>
        private string readDoneConfirmCheckRight()
        {
            readDoneConfirm = "False";
            // bắt đầu ghi âm
            RecordStart();
            while (bitBackward && readDoneConfirm == "False")
            {
                WaitTo4PulseComplete(); // Đợi động cơ quay 4 vòng
                WaitRead20DataToArray(50, 1); // Đợi lấy đủ 20 giá trị              
            }

            if (readDoneConfirm == "True") return ("True");
            else return ("False");
        }

        /// <summary>
        /// Cập nhật trạng thái Auto/Manual khi nhấn nút Manual trên giao diện
        /// Chế độ Manual cho phép lựa chọn ô điền dữ liệu bằng cách chỉ chuột vào ô bất kỳ trong file Excel đang mở
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnManual_Click(object sender, EventArgs e)
        {
            if (dataLogging)
            {
                btnStart.BackColor = Color.White;
                btnStart.ForeColor = Color.Black;
            }
            btnManual.BackColor = Color.MidnightBlue;
            btnManual.ForeColor = Color.White;
            bitManual = true;
        }

        /// <summary>
        /// Đường đến thư mục lưu file Log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBrowser2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                txtLoggingFolder.Text = folderBrowserDialog.SelectedPath.ToString();
        }

        /// <summary>
        /// Update vào Setting nếu thay đổi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtLoggingFolder_TextChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Chờ đến khi lấy đủ 20 giá trị vào mảng dữ liệu
        /// </summary>
        /// <param name="number"></param>
        /// 


        List<double> _lstBackward;
        List<double> _lstForward;

        private void WaitRead20DataToArray(int number, int bit)
        {
            countNumberOfReadData = 0;
            double tempRd;

            if (bit == 0)
            {
                _transferStatusStart(0, true);
                Thread.Sleep(50);
                _lstForward = new List<double>();
                while (countNumberOfReadData < number)
                {
                    Thread.Sleep(50);
                    tempRd = (double)D100 * _factorVibration;
                    if (!_isFinishRD && (tempRd != 0))
                    {
                        _lstForward.Add(tempRd);
						Console.WriteLine("StartPlot");
                        _transferRDForward(tempRd);
						Console.WriteLine("StopPlot");
                        countNumberOfReadData++;
						Console.WriteLine(countNumberOfReadData.ToString()+"\n");
                    }
                }
				_transferStatusStart(0, false);

			}
			else
            {
                _transferStatusStart(1, true);
                Thread.Sleep(50);
                _lstBackward = new List<double>();

                while (countNumberOfReadData < number)
                {
                    Thread.Sleep(50);
                    tempRd = (double)D100 * _factorVibration;
                    if (!_isFinishRD && (tempRd != 0))
                    {
                        _lstBackward.Add(tempRd);
						Console.WriteLine("StartPlot");
                        _transferRDBackward(tempRd);
						Console.WriteLine("StopPlot");
                        countNumberOfReadData++;
						Console.WriteLine(countNumberOfReadData.ToString() + "\n");
					}
                }
				_transferStatusStart(1, false);

			}

			readDoneConfirm = "True";

        }

        private void btnTest001_Click(object sender, EventArgs e)
        {
            //Task.Delay(100);
            //Console.WriteLine("Nhan nut X5!");
            //wfKheho wftemp = new wfKheho(ref plcFX3G, ref COMSylvac);
            //wftemp.stringDoneKheho += InputKhehoToExcel;
            //wftemp.ShowDialog();
            //wftemp.Dispose();

            // Test
            if (!currentPLCBit["X11"]) plcFX3G.SetDevice("X11", 1);
            else plcFX3G.SetDevice("X11", 0);
        }

        private void InputKhehoToExcel(string x)
        {
            stringKhehoTinh = x;
            string[] data = { stringKhehoTinh };
            this.Invoke(new MethodInvoker(delegate { _transfer(data, ""); }));

        }

        private void btnTest002_Click(object sender, EventArgs e)
        {
            // Test
            if (!currentPLCBit["X12"]) plcFX3G.SetDevice("X12", 1);
            else plcFX3G.SetDevice("X12", 0);
        }

        private void InputDodaoToExcel(string x)
        {
            // Nhập giá trị độ đảo vào Excel

            // Xử lý chuỗi độ đảo nhận về
            // Lay gia tri dau tien
            if (x.IndexOf("DT10000") >= 0)
            {
                string temp = x.Substring(x.IndexOf("DT10000"));
                temp = temp.Substring(7, temp.IndexOf("M") - 7);
                float tempF = float.Parse(temp);
                firstString = tempF.ToString("0.000");
            }
            else firstString = "";

            if (x.IndexOf("DT10001") >= 0)
            {
                string temp = x.Substring(x.IndexOf("DT10001"));
                temp = temp.Substring(7, temp.IndexOf("M") - 7);
                float tempF = float.Parse(temp);
                secondString = tempF.ToString("0.000");
            }
            else secondString = "";

            if (x.IndexOf("DT10002") >= 0)
            {
                string temp = x.Substring(x.IndexOf("DT10002"));
                temp = temp.Substring(7, temp.IndexOf("M") - 7);
                float tempF = float.Parse(temp);
                thirdString = tempF.ToString("0.000");
            }
            else thirdString = "";

            string[] data = { firstString, secondString, thirdString };
            this.Invoke(new MethodInvoker(delegate { _transfer(data, ""); }));
        }

        private async void btnTest003_Click(object sender, EventArgs e)
        {
            await Task.Delay(100);
            if (!bitCaptureOpen)
            {
                Capture wftemp2 = new Capture(ref plcFX3G);
                wftemp2.Show();
                Console.WriteLine("Nhan nut X7!");
            }
        }

        private void pnlTop_Paint(object sender, PaintEventArgs e)
        {

        }

        /// <summary>
        /// Cập nhật giá trị lớn nhất theo chiều quay hiện tại
        /// </summary>

        /// <summary>
        /// Chờ đến cho đến khi động cơ quay 4 vòng (countPulse - oldCountPulse > 4)
        /// </summary>
        private void WaitTo4PulseComplete()
        {
            try
            {
                int tempPulse = countPulse;
                while ((tempPulse + 3) > countPulse)
                {
                    if (bitBackward || bitForward) Invoke(new MethodInvoker(delegate { txtNumberPulse.Text = (countPulse - tempPulse).ToString(); }));
                    else tempPulse = countPulse;
                    Thread.Sleep(20);
                }

                Invoke(new MethodInvoker(delegate { txtNumberPulse.Text = (countPulse - tempPulse).ToString(); }));
            }
            catch { }
            
        }

        /// <summary>
        /// Xử lý dữ liệu khi kết thúc 1 chu trình lấy dữ cả 2 chiều thuận và nghịch
        /// </summary>
        private void IfCompleteOneReadCyleProcessData()
        {
            try
            {
                // giá trị readLeft ON sau khi đọc xong chiều Thuận, readRight ON sau khi đọc chiều Nghịch
                // giá trị bitBackward và bitForward ON khi đang lấy dữ liệu theo chiều tương ứng
                // phải đạt đủ điều kiện thì mới coi như kết thúc 1 chu trình lấy dữ liệu
                if (readLeft && readRight && !bitBackward && !bitForward)
                {
                    UpdateStatusOfRead(); // Khởi tạo lại các giá trị hiển thị về mặc định, chuẩn bị cho chu trình đọc tiếp theo
                    if (!bitManual) // Nếu đang ở chế độ Auto thì cập nhật vào ô tương ứng theo giá trị currentNumberRead
                    {
                        // Tăng giá trị numberRead lên 1, cập nhật hiển thị
                        UpdateDataToAutoCellExcel("doRung");
                        _isFinishRD = true;
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(_PathError + "/Error_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt",
                        DateTime.Now.ToString("HH:mm:ss") + ":IfCompleteOneReadCyleProcessData(): " + ex.ToString() + Environment.NewLine);
            }           
        }



        /// <summary>
        /// Lưu file Excel theo ngày, tháng, năm - Khi đã điền hết dữ liệu vào bảng Excel - tương ứng với numberRead > 24
        /// Sau khi Update, tự nhấn Nút Reset để các giá trị được đặt về mặc định
        /// </summary>
        private void SaveFileAndResetIfRaise200()
        {
            if (numberRead > 24)
            {
                try
                {
                  //  myExcel.ActiveWorkbook.SaveCopyAs(@txtLoggingFolder.Text + productName + DateTime.Now.ToString("_yy_MM_dd_HH_mm_ss") + ".xlsm");

                    // Điều khiển nút Reset tự động nhấn
                    Invoke(new MethodInvoker(delegate () { btnReset.PerformClick(); }));
                }
                catch
                {
                    MessageBox.Show("Save Excel File Error!");
                }
            }
        }

        /// <summary>
        /// Điền dữ liệu ở chế độ Auto
        /// Điền dữ liệu vào ô tương ứng tiếp theo trong bảng
        /// Điền dữ liệu vào ô giá trị đề hiển thị đồ thị - trong Sheet 5
        /// </summary>

         
        private void UpdateDataToAutoCellExcel(string options)
        {
            switch (options)
            {

                /// gửi giá trị độ rung và kết quả record
                case "doRung":
                    _lstForward.Sort((a, b) => b.CompareTo(a));
                    _lstBackward.Sort((a, b) => b.CompareTo(a));
                    string[] data = { _lstForward[0].ToString("0.000"), _lstBackward[0].ToString("0.000") };
                    this.Invoke(new MethodInvoker(delegate { _transfer(data, "RD"); }));
                    break;
                
                default:
                    break;
            }
        }



        /// <summary>
        /// Cập nhật giá trị kiểm tra hiện tại lên giao diện
        /// </summary>
        private void ChangeDisplayOfCurrentNumberRead()
        {
            if (InvokeRequired) Invoke(new MethodInvoker(delegate { txtNumberRun.Text = (convertIndex[numberRead + 1]).ToString(); }));
        }

        /// <summary>
        /// Tăng numberRead lên 1 để nhảy đến ô lấy dữ liệu tiếp theo/ trừ khi đang ở chế độ Manual
        /// Khởi tạo lại các giá trị (đã đọc) về OFF
        /// </summary>
        private void UpdateStatusOfRead()
        {
            //if (!bitManual) numberRead += 1;
            readRight = false;
            readLeft = false;
            btnLampF.BackColor = Color.Transparent;
            btnLampB.BackColor = Color.Transparent;
        }

    }
}