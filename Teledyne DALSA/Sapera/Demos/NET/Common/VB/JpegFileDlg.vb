Public Class JPEGFileDlg

    Private m_Compression_Level As Integer

    Public Sub New(ByVal quality As Integer)
        InitializeComponent()
        ' Read file name
        Dim keyPath As String = "Software\Teledyne DALSA\" + Application.ProductName + "\SapFile"
        Dim regkey As RegistryKey = Registry.CurrentUser.OpenSubKey(keyPath)
        If regkey IsNot Nothing Then
            UpDown_compression_level.Value = Convert.ToDecimal(regkey.GetValue("Jpeg Quality", quality))
        End If
    End Sub

    Private Sub button_OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_OK.Click
        m_Compression_Level = CInt(UpDown_compression_level.Value)
        ' Write compression type and level values
        Dim keyPath As String = "Software\Teledyne DALSA\" + Application.ProductName + "\SapFile"
        Dim regKey As RegistryKey = Registry.CurrentUser.CreateSubKey(keyPath)
        regKey.SetValue("Jpeg Quality", m_Compression_Level)
    End Sub

    Public Property JpegFileQuality() As Integer
        Get
            Return m_Compression_Level
        End Get
        Private Set(ByVal value As Integer)
            m_Compression_Level = value
        End Set
    End Property

   
End Class