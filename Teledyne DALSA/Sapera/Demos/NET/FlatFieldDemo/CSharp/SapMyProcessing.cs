using System;
using System.Collections.Generic;
using System.Text;
using DALSA.SaperaLT.SapClassBasic;

namespace DALSA.SaperaLT.Demos.NET.CSharp.FlatFieldDemo
{
   class SapMyProcessing :  SapProcessing
   {
      private SapFlatField m_pFlatField;
     // Constructor

     public SapMyProcessing(SapBuffer pBuffers, SapFlatField pFlatField, SapProcessingDoneHandler  pCallback, Object pContext)
        : base(pBuffers)
     {
        base.ProcessingDoneEnable = true;
        base.ProcessingDone += pCallback;
        base.ProcessingDoneContext = pContext;
        m_pFlatField = pFlatField;
     }

 

     public override bool Run()
     {
         // Check if flat field correction is enabled and if it's to be done by software
         if (m_pFlatField.Enabled && m_pFlatField.SoftwareCorrection)
         {
            // Do software flat field correction
            m_pFlatField.Execute(base.Buffer, base.Index);
         }
         return true;
     }

   
   }
}
