using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DALSA.SaperaLT.Demos.NET.CSharp.GigESeqGrabDemo
{
    static class GigESeqGrabDemo
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            GigESeqGrabDemoDlg GigESeq = new GigESeqGrabDemoDlg();
            if (!GigESeq.IsDisposed)
                Application.Run(GigESeq);
        }
    }
}