using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace DALSA.SaperaLT.Demos.NET.CSharp.MultiBoardSyncGrabDemo
{
    static class MultiBoardSyncGrabDemo
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MultiBoardSyncGrabDemoDlg form = new MultiBoardSyncGrabDemoDlg();
            if (!form.IsDisposed)
                Application.Run(form);
        }
    }
}