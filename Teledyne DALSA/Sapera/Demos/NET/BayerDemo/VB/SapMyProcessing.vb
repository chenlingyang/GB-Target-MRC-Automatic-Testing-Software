Public Class SapMyProcessing
   Inherits SapProcessing
   Private m_Bayer As SapBayer
   ' Constructor

   Public Sub New(ByVal pBuffers As SapBuffer, ByVal pBayer As SapBayer, ByVal pCallback As SapProcessingDoneHandler, ByVal pContext As Object)
      MyBase.New(pBuffers)
      MyBase.ProcessingDoneEnable = True
      AddHandler MyBase.ProcessingDone, pCallback
      MyBase.ProcessingDoneContext = pContext
      m_Bayer = pBayer
   End Sub

   Public Overloads Overrides Function Run() As Boolean
      If m_Bayer.Enabled AndAlso m_Bayer.SoftwareConversion Then
         m_Bayer.Convert(MyBase.Index)
      End If
      Return True
   End Function
End Class
