using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DALSA.SaperaLT.Demos.NET.CSharp.SeqGrabDemo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SeqGrabDemoDlg SeqGD = new SeqGrabDemoDlg();
            if (!SeqGD.IsDisposed)
                Application.Run(SeqGD);
        }
    }
}