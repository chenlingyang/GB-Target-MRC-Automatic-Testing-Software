Public Class TiffFileDlg

    Private m_Compression_Level As Integer
    Private m_Compression_Type As Integer

    Public Sub New(ByVal compressionLevel As Integer, ByVal compressionType As Integer)
        InitializeComponent()
        ' Read file name
        Dim keyPath As String = "Software\Teledyne DALSA\" + Application.ProductName + "\SapFile"
        Dim regkey As RegistryKey = Registry.CurrentUser.OpenSubKey(keyPath)
        If regkey IsNot Nothing Then
            UpDown_compression_level.Value = Convert.ToDecimal(regkey.GetValue("Tiff Compression Level", compressionLevel))
            comboBox_compression_type.SelectedIndex = Convert.ToInt32(regkey.GetValue("Tiff Compression Type", compressionType))
        End If
    End Sub

    Private Sub button_OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_OK.Click
        m_Compression_Level = CInt(UpDown_compression_level.Value)
        m_Compression_Type = comboBox_compression_type.SelectedIndex
        ' Write compression type and level values
        Dim keyPath As String = "Software\Teledyne DALSA\" + Application.ProductName + "\SapFile"
        Dim regKey As RegistryKey = Registry.CurrentUser.CreateSubKey(keyPath)
        regKey.SetValue("Tiff Compression Level", m_Compression_Level)
        regKey.SetValue("Tiff Compression Type", m_Compression_Type)
    End Sub

    Public ReadOnly Property CompressionLevel() As Integer
        Get
            Return m_Compression_Level
        End Get
    End Property

    Public ReadOnly Property CompressionType() As Integer
        Get
            Return m_Compression_Type
        End Get
    End Property
End Class