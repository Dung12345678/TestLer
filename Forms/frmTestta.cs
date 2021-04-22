using BMS.Business;
using BMS.Model;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data.SqlClient;

namespace BMS
{
    public partial class frmTestta : Form
    {
        public long _WorkerID = 0;
        int Qty;
        DataTable _dtData = new DataTable();
        public decimal SlitMax;
        private Color tempColor = new Color();
        private Color tempColor1 = new Color();

        public frmTestta()
        {
            InitializeComponent();
        }

        private void txtOrderNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadData();
            }
        }
        /// <summary>
        /// LoadDataInTextBox
        /// </summary>
        private void LoadData()
        {
            if (!string.IsNullOrEmpty(txtOrderNo.Text.Trim()))
            {
                
                try
                {
                    DataTable dt = TextUtils.LoadDataFromSP("spGetGearInfo_ByOrderCode", "A"
                      , new string[1] { "@OrderCode" }
                      , new object[1] { txtOrderNo.Text.Trim() });
                    Qty = TextUtils.ToInt(dt.Rows[0]["Qty"]);
                    txtQty.Text = Qty.ToString("n0");
                    txtLAP.Text = TextUtils.ToString(dt.Rows[0]["ProductCode"]);
                    txtSlitMax.Text = TextUtils.ToDecimal(dt.Rows[0]["SlitMax"]).ToString("n3");
                    txtSlitMin.Text = TextUtils.ToDecimal(dt.Rows[0]["SlitMin"]).ToString("n3");
                    txtVibrateMaxT.Text = TextUtils.ToDecimal(dt.Rows[0]["VibrateMax"]).ToString("n3");
                    txtVibrateMaxN.Text = txtVibrateMaxT.Text;
                    txtVibrateMinT.Text = TextUtils.ToDecimal(dt.Rows[0]["VibrateMin"]).ToString("n3");
                    txtVibrateMinN.Text = txtVibrateMinT.Text;
                    SlitMax = Decimal.Parse(txtSlitMax.Text.ToString());
                }
                catch (Exception)
                {
                    MessageBox.Show("OrderNo is not correct");
                }
            }  
        }
        /// <summary>
        /// ShowDataInGrid
        /// </summary>
        /// <summary>
        /// F12 is Save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Check_OK_NG
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvData_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            
        }

    }
}

