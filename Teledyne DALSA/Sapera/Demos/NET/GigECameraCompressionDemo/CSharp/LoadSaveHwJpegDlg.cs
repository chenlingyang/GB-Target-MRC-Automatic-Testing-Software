using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.SapClassGui;

namespace DALSA.SaperaLT.Demos.NET.CSharp.CameraCompressionDemo
{
   class LoadSaveHwJpegDlg : LoadSaveDlg
   {
      public LoadSaveHwJpegDlg(SapBuffer pBuffer, bool isOpen, bool isSequence) : base(null/*pBuffer*/, isOpen, isSequence)
      {
         m_pBufferView = pBuffer;
      }

      public new DialogResult ShowDialog()
      {
         // Set name and type
         m_FileDialog.FileName = m_PathName;

         SetFilterIndex();
         if (m_FileDialog.ShowDialog() != DialogResult.OK)
            return DialogResult.Cancel;

         // Update type from filter index
         UpdateTypeFromFilterIndex();
         //if (m_bSequence)
         //   m_Type = m_SequenceTypes[m_FileDialog.FilterIndex - 1];
         //else
         //   m_Type = m_StandardTypes[m_FileDialog.FilterIndex - 1];

         // Update file object, only if JPEG is not selected
         if (m_Type != SapBuffer.FileFormat.JPEG)
         {
            // Re-add buffer info that was omitted in the overloaded constructor
            m_pBuffer = m_pBufferView;

            // Removing this line in JPEG will not start the JPEG quality dialog
            if (!UpdateBuffer())
               return DialogResult.Abort;
         }

         // Save parameters to registry
         SaveSettings();
         return DialogResult.OK;
      }

      public SapBuffer.FileFormat GetSapBufferFileFormat()
      {
         return m_Type;
      }

      /*public void Dispose()
      {
         m_LoadSaveDialog.Dispose();
         m_LoadSaveDialog = null;
      }*/

      private SapBuffer m_pBufferView;
      //private LoadSaveDlg m_LoadSaveDialog;
   }
}
