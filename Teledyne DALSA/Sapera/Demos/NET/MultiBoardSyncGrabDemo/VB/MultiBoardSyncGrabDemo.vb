Public Class MultiBoardSyncGrabDemo
   'The main entry point for the application.
   <STAThread()> _
   Shared Sub Main()
      Application.EnableVisualStyles()
      Application.SetCompatibleTextRenderingDefault(False)
      Dim MultiBoardSyncGrabDemo As MultiBoardSyncGrabDemoDlg = New MultiBoardSyncGrabDemoDlg
      If Not MultiBoardSyncGrabDemo.IsDisposed Then
         Application.Run(MultiBoardSyncGrabDemo)
      End If
   End Sub
End Class
