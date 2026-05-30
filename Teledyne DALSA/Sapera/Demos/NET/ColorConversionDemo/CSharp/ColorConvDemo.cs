using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DALSA.SaperaLT.Demos.NET.CSharp.ColorConvDemo
{
   static class ColorConvDemo
   {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main()
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         ColorConvDemoDlg dlg = new ColorConvDemoDlg();
         if (!dlg.IsDisposed)
            Application.Run(dlg);
      }
   }
}