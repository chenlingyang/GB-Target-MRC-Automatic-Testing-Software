using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DALSA.SaperaLT.Demos.NET.CSharp.GigEMetadataDemo
{
   static class GigEMetadataDemo
   {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main()
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         GigEMetadataDemoDlg GigEMetadata = new GigEMetadataDemoDlg();
         if (!GigEMetadata.IsDisposed)
            Application.Run(GigEMetadata);
      }
   }
}
