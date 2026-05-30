using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace DALSA.SaperaLT.Demos.NET.CSharp.GigECameraDemo
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
            GigECameraDemoDlg form = new GigECameraDemoDlg();
            if (!form.IsDisposed)
                Application.Run(form);
        }
    }
}