Public Class GigEFlatFieldDemo
   'The main entry point for the application.
   <STAThread()> _
   Shared Sub Main()
      Application.EnableVisualStyles()
      Application.SetCompatibleTextRenderingDefault(False)
      Dim GigeFF As GigEFlatFieldDemoDlg = New GigEFlatFieldDemoDlg()
      If Not GigeFF.IsDisposed Then
         Application.Run(GigeFF)
      End If
   End Sub
End Class
