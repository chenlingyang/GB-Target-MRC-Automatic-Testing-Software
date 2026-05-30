Class SapMyProcessing
   Inherits SapProcessing
   Private m_pFlatField As SapFlatField
   ' Constructor

   Public Sub New(ByVal pBuffers As SapBuffer, ByVal pFlatField As SapFlatField, ByVal pCallback As SapProcessingDoneHandler, ByVal pContext As Object)
      MyBase.New(pBuffers)
      MyBase.ProcessingDoneEnable = True
      AddHandler MyBase.ProcessingDone, pCallback
      MyBase.ProcessingDoneContext = pContext
      m_pFlatField = pFlatField
   End Sub



   Public Overloads Overrides Function Run() As Boolean
      ' Check if flat field correction is enabled and if it's to be done by software
      If m_pFlatField.Enabled AndAlso m_pFlatField.SoftwareCorrection Then
         ' Do software flat field correction
         m_pFlatField.Execute(MyBase.Buffer, MyBase.Index)
      End If
      Return True
   End Function
End Class
