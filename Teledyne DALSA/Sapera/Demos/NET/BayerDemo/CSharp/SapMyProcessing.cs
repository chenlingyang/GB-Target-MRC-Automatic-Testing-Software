using System;
using System.Collections.Generic;
using System.Text;
using DALSA.SaperaLT.SapClassBasic;

namespace DALSA.SaperaLT.Demos.NET.CSharp.BayerDemo
{
   class SapMyProcessing : SapProcessing   
   {
      private SapBayer m_Bayer;
     // Constructor

      public SapMyProcessing(SapBuffer pBuffers, SapBayer pBayer, SapProcessingDoneHandler pCallback, Object pContext)
        : base(pBuffers)
     {
        base.ProcessingDoneEnable = true;
        base.ProcessingDone += pCallback;
        base.ProcessingDoneContext = pContext;
        m_Bayer = pBayer;
     }

     public override bool Run()
     {
        if (m_Bayer.Enabled && m_Bayer.SoftwareConversion)
        {
           m_Bayer.Convert(base.Index);
        }

        return true;
     }
 
   }
}
