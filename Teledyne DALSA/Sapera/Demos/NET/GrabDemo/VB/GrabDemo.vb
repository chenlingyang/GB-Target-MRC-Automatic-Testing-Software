Public Class GrabDemo
   'The main entry point for the application.
   <STAThread()> _
   Shared Sub Main()
      Application.EnableVisualStyles()
      Application.SetCompatibleTextRenderingDefault(False)
      Dim GrabDemo As GrabDemoDlg = New GrabDemoDlg
      If Not GrabDemo.IsDisposed Then
         Application.Run(GrabDemo)
      End If
   End Sub
End Class
