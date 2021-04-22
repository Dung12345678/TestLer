using ActUtlTypeLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BMS
{
    public partial class wfDodaoSylvac : Form
    {
        public delegate void StringCap(string x1, string x2, string x3);
        public event StringCap stringDoneDodao;
        int countDisplayProcessBar = 0;
        int countTimeToAlowFinish = 0;
        Timer timerProcess = new Timer();
        private static float valueMax1, valueMin1;
        private static float valueMax2, valueMin2;
        private static float valueMax3, valueMin3;

        private int countDataInCom1, countDataInCom2, countDataInCom3;

        private bool btnTEST_Finish;

        SerialPort ComDodao01, ComDodao02, ComDodao03;
        ActUtlType plcRef;


        private string bufferString;

        private void wfDodaoSylvac_FormClosing(object sender, FormClosingEventArgs e)
        {
            ComDodao01.Write("OUT0\r\n");
            ComDodao02.Write("OUT0\r\n");
            ComDodao03.Write("OUT0\r\n");

            ComDodao01.DataReceived -= ProcessComMessage;
            ComDodao02.DataReceived -= ProcessComMessage;
            ComDodao03.DataReceived -= ProcessComMessage;

            timerProcess.Stop();

        }

        public wfDodaoSylvac()
        {
            InitializeComponent();
            timerProcess.Interval = 100;
            timerProcess.Tick += incProcess;
            timerProcess.Start();
            btnTEST_Finish = false;

            this.KeyDown += CheckKeydown;

            // Giá trị mặc định
            valueMax1 = (float)0.0001;
            valueMin1 = 0;
            valueMax2 = (float)0.0001;
            valueMin2 = 0;
            valueMax3 = (float)0.0001;
            valueMin3 = 0;

            countDataInCom1 = 0;
            countDataInCom2 = 0;
            countDataInCom3 = 0;


            //// Khai báo cổng COM mới cho thiết bị Sylvac đo độ đảo
            //ComDodao01 = new SerialPort("COM21", 4800, Parity.Even, 7, StopBits.Two);
            //ComDodao01.DtrEnable = true;
            //ComDodao01.Open();

            //ComDodao02 = new SerialPort("COM22", 4800, Parity.Even, 7, StopBits.Two);
            //ComDodao02.DtrEnable = true;
            //ComDodao02.Open();

            //ComDodao03 = new SerialPort("COM23", 4800, Parity.Even, 7, StopBits.Two);
            //ComDodao03.DtrEnable = true;
            //ComDodao03.Open();

            //ComDodao01.DataReceived -= ProcessComMessage;
            //ComDodao01.DataReceived += ProcessComMessage;
            //ComDodao02.DataReceived -= ProcessComMessage;
            //ComDodao02.DataReceived += ProcessComMessage;
            //ComDodao03.DataReceived -= ProcessComMessage;
            //ComDodao03.DataReceived += ProcessComMessage;

            //ComDodao01.WriteLine("PRE +0\r\n");
            //ComDodao01.Write("OUT1\r\n");
            //ComDodao02.WriteLine("PRE +0\r\n");
            //ComDodao02.Write("OUT1\r\n");
            //ComDodao03.WriteLine("PRE +0\r\n");
            //ComDodao03.Write("OUT1\r\n");

        }

        private void CheckKeydown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Space:
                    btnTEST_Finish = true;
                    break;
                default:
                    break;
            }
        }

        public wfDodaoSylvac(ref ActUtlType PLC, ref SerialPort COM1, ref SerialPort COM2, ref SerialPort COM3) : this()
        {
            plcRef = PLC;

            ComDodao01 = COM1;
            ComDodao02 = COM2;
            ComDodao03 = COM3;
            ComDodao01.DataReceived -= ProcessComMessage;
            ComDodao01.DataReceived += ProcessComMessage;
            ComDodao02.DataReceived -= ProcessComMessage;
            ComDodao02.DataReceived += ProcessComMessage;
            ComDodao03.DataReceived -= ProcessComMessage;
            ComDodao03.DataReceived += ProcessComMessage;

            // Sent CMD Set Value Message
            Task.Delay(100);
            ComDodao01.WriteLine("PRE +0\r\n");
            ComDodao01.Write("OUT1\r\n");
            ComDodao02.WriteLine("PRE +0\r\n");
            ComDodao02.Write("OUT1\r\n");
            ComDodao03.WriteLine("PRE +0\r\n");
            ComDodao03.Write("OUT1\r\n");
        }

        /// <summary>
        /// Xử lý dữ liệu nhận về từ đầu đo Sylvac
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessComMessage(object sender, SerialDataReceivedEventArgs e)
        {
            // Chuyển đổi từ String thành Float 2 chữ số sau dấu phẩy
            bufferString += (sender as SerialPort).ReadExisting();
            if (bufferString.IndexOf("\r") >= 0)
            {
                string tempStringReceive = bufferString;
                bufferString = "";
                Console.WriteLine(tempStringReceive + "--------");
                // TEST Fast Update 17/04
                float tempF = (float)0.000;
                string[] arrValueBuffer = tempStringReceive.Split('\r');
                foreach (string item in arrValueBuffer)
                {
                    try
                    {
                        if (item.IndexOf('.') == -1) continue;
                        tempF = float.Parse(item);
                        if ((sender as SerialPort).PortName == ComDodao01.PortName)
                        {
                            if (valueMax1 < tempF) valueMax1 = tempF;
                            if (valueMin1 > tempF) valueMin1 = tempF;
                            countDataInCom1 += 1;
                        }
                        if ((sender as SerialPort).PortName == ComDodao02.PortName)
                        {
                            if (valueMax2 < tempF) valueMax2 = tempF;
                            if (valueMin2 > tempF) valueMin2 = tempF;
                            countDataInCom2 += 1;
                            //Invoke(new MethodInvoker(delegate { lblDodao02.Text = tempF.ToString("0.000") + " " + countDataInCom2.ToString(); }));
                        }
                        if ((sender as SerialPort).PortName == ComDodao03.PortName)
                        {
                            if (valueMax3 < tempF) valueMax3 = tempF;
                            if (valueMin3 > tempF) valueMin3 = tempF;
                            countDataInCom3 += 1;
                            //Invoke(new MethodInvoker(delegate { lblDodao03.Text = tempF.ToString("0.000") + " " + countDataInCom3.ToString(); }));
                        }
                    }
                    catch { } 
                }
                try
                {
                    if ((sender as SerialPort).PortName == ComDodao01.PortName)
                    {
                        Invoke(new MethodInvoker(delegate { lblDodao01.Text = tempF.ToString("0.000") 
                            + " "; }));
                    }
                    if ((sender as SerialPort).PortName == ComDodao02.PortName)
                    {
                        Invoke(new MethodInvoker(delegate { lblDodao02.Text = tempF.ToString("0.000") 
                            + " "; }));
                    }
                    if ((sender as SerialPort).PortName == ComDodao03.PortName)
                    {
                        Invoke(new MethodInvoker(delegate { lblDodao03.Text = tempF.ToString("0.000") 
                            + " "; }));
                    }
                }
                catch { }

            }
        }

        /// <summary>
        /// Xử lý hiển thị thanh Process, đồng thời đợi tín hiệu kết thúc từ PLC
        /// Cần phải điều chỉnh chuối gửi ra để phù hợp với hoạt động 3 thiết bị đo cùng lúc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void incProcess(object sender, EventArgs e)
        {
            countDisplayProcessBar += 1;
            countTimeToAlowFinish += 1;
            if (countDisplayProcessBar > 8) countDisplayProcessBar = 0;
            lblProcess.Text = "";
            for (int i = 0; i < countDisplayProcessBar; i++)
            {
                lblProcess.Text += "_";
            }

            if (btnTEST_Finish)
            {
                btnTEST_Finish = false;
                MessageBox.Show((valueMax1 - valueMin1).ToString("0.000") +
                                (valueMax2 - valueMin2).ToString("0.000") +
                                (valueMax3 - valueMin3).ToString("0.000"));

                ComDodao01.Write("OUT0\r\n");
                ComDodao02.Write("OUT0\r\n");
                ComDodao03.Write("OUT0\r\n");
                this.Close();
            }
            // Check Button ReClick => Cần đổi giá trị X12??
            if ((plcRef != null) && (countTimeToAlowFinish > 20))
            {
                int buttonRead;
                var iret = plcRef.GetDevice("X12", out buttonRead);
                if (buttonRead == 1)
                {         

                    if (stringDoneDodao != null) stringDoneDodao((valueMax1 - valueMin1).ToString("0.000"),
                                                                 (valueMax2 - valueMin2).ToString("0.000"),
                                                                 (valueMax3 - valueMin3).ToString("0.000"));
                    this.Close();
                }
            }
        }

    }
}
