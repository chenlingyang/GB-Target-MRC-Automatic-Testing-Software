Public Class FlatFieldDemo
   'The main entry point for the application.
   <STAThread()> _
   Shared Sub Main()
      Application.EnableVisualStyles()
      Application.SetCompatibleTextRenderingDefault(False)
      Dim FF As FlatFieldDemoDlg = New FlatFieldDemoDlg()
      If Not FF.IsDisposed Then
         Application.Run(FF)
      End If
   End Sub
End Class
