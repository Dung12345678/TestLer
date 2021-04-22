using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BMS
{
	public partial class frmStt : Form
	{
		public frmStt()
		{
			InitializeComponent();
			
			
		}
		public int _SttStart = 0;

		private void nmrStt_KeyDown(object sender, KeyEventArgs e)
		{
			
			if (e.KeyCode == Keys.Enter)
			{
				_SttStart = TextUtils.ToInt(nmrStt.Value);
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}
	}
}
