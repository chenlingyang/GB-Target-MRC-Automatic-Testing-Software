Public Class GigESeqGrabDemo
    'The main entry point for the application.
    <STAThread()> _
    Shared Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Dim GigESeq As GigESeqGrabDemodlg = New GigESeqGrabDemodlg()
        If Not GigESeq.IsDisposed Then
            Application.Run(GigESeq)
        End If
    End Sub
End Class
