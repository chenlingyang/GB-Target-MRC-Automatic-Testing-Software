Public Class SeqGrabDemo
    'The main entry point for the application.
    <STAThread()> _
    Shared Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Dim SeqGD As SeqGrabDemoDlg = New SeqGrabDemoDlg()
        If Not SeqGD.IsDisposed Then
            Application.Run(SeqGD)
        End If
    End Sub
End Class
