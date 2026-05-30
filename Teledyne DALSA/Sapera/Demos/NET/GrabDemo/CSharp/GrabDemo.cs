using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace DALSA.SaperaLT.Demos.NET.CSharp.GrabDemo
{
    static class GrabDemo
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            GrabDemoDlg form = new GrabDemoDlg();
            if (!form.IsDisposed)
                Application.Run(form);
        }
    }
}