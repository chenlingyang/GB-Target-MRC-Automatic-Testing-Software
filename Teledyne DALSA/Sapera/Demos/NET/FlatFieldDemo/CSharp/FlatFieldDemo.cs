using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DALSA.SaperaLT.Demos.NET.CSharp.FlatFieldDemo
{
    static class FlatFieldDemo
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FlatFieldDemoDlg dlg = new FlatFieldDemoDlg();
            if (!dlg.IsDisposed)
               Application.Run(dlg);
        }
    }
}