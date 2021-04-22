using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading;
using System.IO;
using ActUtlTypeLib;
using DocumentFormat.OpenXml.Drawing;

namespace BMS
{
    public partial class Capture : Form
    {
        public delegate void saveImageCompleteDelegate(string link, bool SaveImgSucc);
        public event saveImageCompleteDelegate saveImageComplete;
        ActUtlType plcFX3G;
        bool conditionRunCam = true;
        Mat m = new Mat(), mm = new Mat();
        VideoCapture captureV;
        string _pathLocal = "";
        string _pathFileName;
        Thread newThread;
        private int buttonRead;
        public string _FileName = "";
        public bool SaveImgSucc = true;

        public Capture(ref ActUtlType PLC) : this("Unknow", ref PLC)
        {
        }

        public Capture(string pathLocal, ref ActUtlType PLC)
        {
            InitializeComponent();
            plcFX3G = PLC;
            _pathLocal = pathLocal;
            newThread = new Thread(runCamera);
            newThread.IsBackground = true;
            newThread.Start();
        }

        private void runCamera()
        {
            while (true)
            {
                try
                {
                    if (conditionRunCam)
                    {
                        if (captureV == null) captureV = new VideoCapture(0);
                        captureV.Read(m);
                        Mat tempImage = new Mat();
                        CvInvoke.Resize(m, tempImage, new Size(imageBox1.Width, imageBox1.Height), 0, 0, Emgu.CV.CvEnum.Inter.Linear);
                        if (!m.IsEmpty) imageBox1.Image = tempImage;
                        Thread.Sleep(50);
                    }
                    //if (!conditionRunCam)
                    //    if (captureV != null) captureV.Dispose();
                    plcFX3G.GetDevice("X13", out buttonRead);
                    if (buttonRead == 1)
                    {
                        Invoke(new MethodInvoker(delegate { btnCapture.PerformClick(); }));
                        break;
                    }
                }
                catch
                {
                    return;
                }

            }
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            conditionRunCam = false;
            if (captureV == null) MessageBox.Show("Không có Camera kết nối");
            else
            {
               
                captureV.Read(mm);
                Mat tempImage = new Mat();
                CvInvoke.Resize(mm, tempImage, new Size(imageBox1.Width, imageBox1.Height), 0, 0, Emgu.CV.CvEnum.Inter.Linear);
                if (!mm.IsEmpty) imageBox1.Image = tempImage;

                // Tạo thư mục lưu
                //string tempDirection = posSaveImage.Substring(0, posSaveImage.LastIndexOf("\\") + 1);
                if (!Directory.Exists(_pathLocal)) Directory.CreateDirectory(_pathLocal);
               
                // Tên file
                //fileName = posSaveImage + DateTime.Now.ToString("_yyyyMMdd_hhmmss") + ".jpg";
                _pathFileName = System.IO.Path.Combine(_pathLocal, _FileName);
                if (File.Exists(_pathFileName)) File.Delete(_pathFileName);
                mm.Save(_pathFileName);
                if (!File.Exists(_pathFileName)) SaveImgSucc = false;

                
                // Fire Event - Gửi đường dẫn ảnh theo Event
                saveImageComplete?.Invoke(_pathFileName, SaveImgSucc);

            }
            this.Close();
        }
       
        private void Capture_FormClosed(object sender, FormClosedEventArgs e)
        {
            captureV.Dispose();
        }

        private void Capture_Load(object sender, EventArgs e)
        {

        }

        private void btnCloseCapture_Click(object sender, EventArgs e)
        {
            newThread.Abort();
            Form.ActiveForm.Close();
        }
    }
}
