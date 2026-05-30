Public Class BayerDemo
   'The main entry point for the application.
   <STAThread()> _
   Shared Sub Main()
      Application.EnableVisualStyles()
      Application.SetCompatibleTextRenderingDefault(False)
      Dim BD As BayerDemoDlg = New BayerDemoDlg()
      If Not BD.IsDisposed Then
         Application.Run(BD)
      End If
   End Sub
End Class
