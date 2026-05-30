using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DALSA.SaperaLT.Demos.NET.CSharp.BayerDemo
{
    static class BayerDemo
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            BayerDemoDlg dlg = new BayerDemoDlg();
            if(!dlg.IsDisposed)
                Application.Run(dlg);
        }
    }
}