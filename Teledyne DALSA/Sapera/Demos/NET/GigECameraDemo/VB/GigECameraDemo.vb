Class GigECameraDemo

    'The main entry point for the application.
    <STAThread()> _
    Shared Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Dim GigeDemo As GigECAmeraDemoDlg = New GigECAmeraDemoDlg
        If Not GigeDemo.IsDisposed Then
            Application.Run(GigeDemo)
        End If
    End Sub
End Class
