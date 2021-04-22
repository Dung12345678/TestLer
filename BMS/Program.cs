using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualBasic.ApplicationServices;
using System.Security.AccessControl;
using System.IO;
using DevExpress.XtraGrid.Localization;

namespace BMS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static List<int> RequestIDList = new List<int>();

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();

            GridLocalizer.Active = new NVGridLocalizer();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new frmTestta2());
        }
    }
}