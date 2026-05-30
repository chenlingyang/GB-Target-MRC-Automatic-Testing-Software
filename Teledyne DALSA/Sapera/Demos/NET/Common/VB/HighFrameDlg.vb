Public Class HighFrameDlg

    Private m_Frame_per_Callback As Integer = 1
    Private m_Frame_on_Board As Integer = 2

    Public Sub New(ByVal Frame As Integer, ByVal Frame_Onboard As Integer, ByVal pXfer As SapTransfer)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        UpDown_Frame.Value = CDec(Frame)

        If pXfer IsNot Nothing Then
            UpDown_Frame_Onboard.Value = CDec(Frame_Onboard)
            ' Check if on-board buffers is supported
            Dim capIntBuffers As Integer = 0

            If pXfer.IsCapabilityAvailable(SapTransfer.Cap.NB_INT_BUFFERS) Then
                pXfer.GetCapability(SapTransfer.Cap.NB_INT_BUFFERS, capIntBuffers)
            End If

            If Not (capIntBuffers = CInt(SapTransfer.Val.NB_INT_BUFFERS_AUTO)) Then
                ' This feature is not supported; disable control
                UpDown_Frame_Onboard.Value = 2
                UpDown_Frame_Onboard.Enabled = False
            End If
        Else
            UpDown_Frame_Onboard.Enabled = False
            UpDown_Frame_Onboard.Hide()
            label2.Hide()
        End If

    End Sub

    Private Sub button_OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_OK.Click
        m_Frame_per_Callback = CInt(UpDown_Frame.Value)
        m_Frame_on_Board = CInt(UpDown_Frame_Onboard.Value)
    End Sub

    Public ReadOnly Property Frame_Per_Callback() As Integer
        Get
            Return m_Frame_per_Callback
        End Get
    End Property

    Public ReadOnly Property Frame_On_Board() As Integer
        Get
            Return m_Frame_on_Board
        End Get
    End Property
End Class