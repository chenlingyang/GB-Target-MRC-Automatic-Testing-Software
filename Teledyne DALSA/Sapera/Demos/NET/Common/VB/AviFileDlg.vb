Public Class AviFileDlg

    Private m_FirstFrame As Integer
    Private m_CompressionLevel As Integer
    Private m_CompressionType As Integer

    Public Sub New(ByVal bLoad As Boolean, ByVal firstFrame As Integer, ByVal compressionLevel As Integer, ByVal compressionType As Integer)
        InitializeComponent()

        ' Read file name

        Dim keyPath As String = "Software\Teledyne DALSA\" + Application.ProductName + "\SapFile"
        Dim regkey As RegistryKey = Registry.CurrentUser.OpenSubKey(keyPath)
        If regkey IsNot Nothing Then
            UpDown_FirstFrame.Value = Convert.ToDecimal(regkey.GetValue("Avi First Frame", firstFrame))
            UpDown_Compression_level.Value = Convert.ToDecimal(regkey.GetValue("Avi Compression Level", compressionLevel))
            comboBox_Compression_type.SelectedIndex = Convert.ToInt32(regkey.GetValue("Avi Compression Type", compressionType))
        End If
        UpDown_FirstFrame.Enabled = bLoad
        UpDown_Compression_level.Enabled = Not bLoad
        comboBox_Compression_type.Enabled = Not bLoad
    End Sub

    Private Sub button_OK_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_OK.Click
        m_FirstFrame = CInt(UpDown_FirstFrame.Value)
        m_CompressionLevel = CInt(UpDown_Compression_level.Value)
        m_CompressionType = comboBox_Compression_type.SelectedIndex

        ' Write AVI compression type and level values
        Dim keyPath As String = "Software\Teledyne DALSA\" + Application.ProductName + "\SapFile"
        Dim regKey As RegistryKey = Registry.CurrentUser.CreateSubKey(keyPath)
        regKey.SetValue("Avi First Frame", m_FirstFrame)
        regKey.SetValue("Avi Compression Level", m_CompressionLevel)
        regKey.SetValue("Avi Compression Type", m_CompressionType)
    End Sub
    Public ReadOnly Property FirstFrame() As Integer

        Get
            Return m_FirstFrame
        End Get
    End Property
    Public ReadOnly Property CompressionLevel() As Integer

        Get
            Return m_CompressionLevel
        End Get
    End Property
    Public ReadOnly Property CompressionType() As Integer

        Get
            Return m_CompressionType
        End Get
    End Property
End Class