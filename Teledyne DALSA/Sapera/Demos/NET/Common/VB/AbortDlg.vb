Public Class AbortDlg

    Private m_pXfer As SapTransfer
    Private m_bShow As Boolean

    Public Sub New(ByVal pXfer As SapTransfer)
        InitializeComponent()
        m_pXfer = pXfer
        Timer.Enabled = True
    End Sub

    Private Sub Button_Abort_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Abort.Click
        Timer.Enabled = False
        Me.Close()
    End Sub

    Private Sub Timer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer.Tick
        If m_pXfer.Wait(0) Then
            Timer.Enabled = False
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()
            ' Show window if time is out
        ElseIf Not m_bShow Then
            Me.Opacity = 100
            m_bShow = True
        End If
    End Sub
End Class