using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DALSA.SaperaLT.Demos.NET.CSharp.GigEFlatFieldDemo
{
    static class GigEFlatFieldDemo
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            GigEFlatFieldDemoDlg dlg = new GigEFlatFieldDemoDlg();
            if(!dlg.IsDisposed)
                Application.Run(dlg);
        }
    }
}