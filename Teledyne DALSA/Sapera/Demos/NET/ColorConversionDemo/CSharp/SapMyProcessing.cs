using System;
using System.Collections.Generic;
using System.Text;
using DALSA.SaperaLT.SapClassBasic;

namespace DALSA.SaperaLT.Demos.NET.CSharp.ColorConvDemo
{
   class SapMyProcessing : SapProcessing   
   {
      private SapColorConversion m_ColorConv;
     // Constructor

      public SapMyProcessing(SapBuffer pBuffers, SapColorConversion pColorConv, SapProcessingDoneHandler pCallback, Object pContext)
        : base(pBuffers)
     {
        base.ProcessingDoneEnable = true;
        base.ProcessingDone += pCallback;
        base.ProcessingDoneContext = pContext;
        m_ColorConv = pColorConv;
     }

     public override bool Run()
     {
        if (m_ColorConv != null && m_ColorConv.Initialized && m_ColorConv.SoftwareEnabled)
        {
           m_ColorConv.Convert(base.Index);
        }

        return true;
     }
 
   }
}
