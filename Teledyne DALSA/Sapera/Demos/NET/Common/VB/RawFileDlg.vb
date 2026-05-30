Public Class RawFileDlg

    Private m_Height As Integer
    Private m_Width As Integer
    Private m_Offset As Integer
    Private m_Format As SapFormat
    Private m_FormatInfo() As SapFormatInfo = _
    { _
        New SapFormatInfo(SapFormat.Mono1, "Monochrome 1-bit"), _
            New SapFormatInfo(SapFormat.Mono8, "Monochrome 8-bit"), _
            New SapFormatInfo(SapFormat.Mono16, "Monochrome 16-bit"), _
            New SapFormatInfo(SapFormat.RGB5551, "RGB 5551"), _
            New SapFormatInfo(SapFormat.RGB565, "RGB 565"), _
            New SapFormatInfo(SapFormat.RGB888, "RGB 888 (blue first)"), _
            New SapFormatInfo(SapFormat.RGBR888, "RGBR 888 (red first)"), _
            New SapFormatInfo(SapFormat.RGB8888, "RGB 8888"), _
            New SapFormatInfo(SapFormat.RGB101010, "RGB 101010"), _
            New SapFormatInfo(SapFormat.RGB161616, "RGB 161616"), _
            New SapFormatInfo(SapFormat.RGB16161616, "RGB 16161616"), _
            New SapFormatInfo(SapFormat.RGBP8, "RGB Planar 8-bit"), _
            New SapFormatInfo(SapFormat.RGBP16, "RGB Planar 16-bit"), _
            New SapFormatInfo(SapFormat.RGBAP8, "RGB Alpha Planar 8-bit"), _
            New SapFormatInfo(SapFormat.RGBAP16, "RGB Alpha Planar 16-bit"), _
            New SapFormatInfo(SapFormat.HSI, "HSI"), _
            New SapFormatInfo(SapFormat.HSIP8, "HSI Planar 8-bit"), _
            New SapFormatInfo(SapFormat.HSV, "HSV"), _
            New SapFormatInfo(SapFormat.UYVY, "UYVY"), _
            New SapFormatInfo(SapFormat.YUY2, "YUY2"), _
            New SapFormatInfo(SapFormat.YUYV, "YUYV"), _
            New SapFormatInfo(SapFormat.LAB, "LAB"), _
            New SapFormatInfo(SapFormat.LABP8, "LAB Planar 8-bit"), _
            New SapFormatInfo(SapFormat.LAB101010, "LAB 101010"), _
            New SapFormatInfo(SapFormat.LABP16, "LAB Planar 16-bit"), _
            New SapFormatInfo(SapFormat.BICOLOR88, "BICOLOR 8-bit"), _
            New SapFormatInfo(SapFormat.BICOLOR1616, "BICOLOR 16-bit"), _
            New SapFormatInfo(SapFormat.BICOLOR1212, "BICOLOR 12-bit"), _
            New SapFormatInfo(SapFormat.RGB888_MONO8, "RGB-IR 8-bit"), _
            New SapFormatInfo(SapFormat.RGB161616_MONO16, "RGB-IR 16-bit"), _
            New SapFormatInfo(SapFormat.Mono8P2, "Monochrome 8-bit (2 planes)"), _
            New SapFormatInfo(SapFormat.Mono8P3, "Monochrome 8-bit (3 planes)"), _
            New SapFormatInfo(SapFormat.Mono8P4, "Monochrome 8-bit (4 planes)"), _
            New SapFormatInfo(SapFormat.Mono8P5, "Monochrome 8-bit (5 planes)"), _
            New SapFormatInfo(SapFormat.Mono8P6, "Monochrome 8-bit (6 planes)"), _
            New SapFormatInfo(SapFormat.Mono8P7, "Monochrome 8-bit (7 planes)"), _
            New SapFormatInfo(SapFormat.Mono8P8, "Monochrome 8-bit (8 planes)"), _
            New SapFormatInfo(SapFormat.Mono8P9, "Monochrome 8-bit (9 planes)"), _
            New SapFormatInfo(SapFormat.Mono8P10, "Monochrome 8-bit (10 planes)"), _
            New SapFormatInfo(SapFormat.Mono16P2, "Monochrome 16-bit (2 planes)"), _
            New SapFormatInfo(SapFormat.Mono16P3, "Monochrome 16-bit (3 planes)"), _
            New SapFormatInfo(SapFormat.Mono16P4, "Monochrome 16-bit (4 planes)"), _
            New SapFormatInfo(SapFormat.Mono16P5, "Monochrome 16-bit (5 planes)"), _
            New SapFormatInfo(SapFormat.Mono16P6, "Monochrome 16-bit (6 planes)"), _
            New SapFormatInfo(SapFormat.Mono16P7, "Monochrome 16-bit (7 planes)"), _
            New SapFormatInfo(SapFormat.Mono16P8, "Monochrome 16-bit (8 planes)"), _
            New SapFormatInfo(SapFormat.Mono16P9, "Monochrome 16-bit (9 planes)"), _
            New SapFormatInfo(SapFormat.Mono16P10, "Monochrome 16-bit (10 planes)") _
    }

    Public Sub New(ByVal width As Integer, ByVal height As Integer, ByVal offset As Integer, ByVal format As SapFormat)
        InitializeComponent()

        ' Read file name
        Dim keyPath As String = "Software\Teledyne DALSA\" + Application.ProductName + "\SapFile"

        Dim regkey As RegistryKey = Registry.CurrentUser.OpenSubKey(keyPath)
        If regkey IsNot Nothing Then
            textBox_Width.Text = regkey.GetValue("Raw Width", width).ToString()
            textBox_Height.Text = regkey.GetValue("Raw Height", height).ToString()
            textBox_Offset.Text = regkey.GetValue("Raw Offset", offset).ToString()
            m_Format = [Enum].Parse(GetType(SapFormat), regkey.GetValue("Raw Format", format).ToString(), True)
        End If

        For i As Integer = 0 To m_FormatInfo.Length - 1
            comboBox_RawFormat.Items.Add(m_FormatInfo(i).ToString())
            If (m_FormatInfo(i).Value = m_Format) Or ((m_Format = 0) AndAlso (m_FormatInfo(i).Value = SapFormat.Mono8)) Then
                comboBox_RawFormat.SelectedIndex = i
            End If
        Next

        If m_Format <> 0 Then
            comboBox_RawFormat.Enabled = False
        End If
    End Sub

    Private Sub button_OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_OK.Click
        m_Width = Integer.Parse(textBox_Width.Text)
        m_Height = Integer.Parse(textBox_Height.Text)
        m_Offset = Integer.Parse(textBox_Offset.Text)
        If comboBox_RawFormat.SelectedIndex <> -1 Then
            m_Format = m_FormatInfo(comboBox_RawFormat.SelectedIndex).Value
        End If

        ' Write compression type and level values
        Dim keyPath As String = "Software\Teledyne DALSA\" + Application.ProductName + "\SapFile"
        Dim regKey As RegistryKey = Registry.CurrentUser.CreateSubKey(keyPath)
        regKey.SetValue("Raw Width", m_Width)
        regKey.SetValue("Raw Height", m_Height)
        regKey.SetValue("Raw Offset", m_Offset)
        regKey.SetValue("Raw Format", m_Format)
    End Sub

    Public ReadOnly Property RawFileHeight() As Integer
        Get
            Return m_Height
        End Get
    End Property
    Public ReadOnly Property RawFileWidth() As Integer
        Get
            Return m_Width
        End Get
    End Property
    Public ReadOnly Property RawFileOffset() As Integer
        Get
            Return m_Offset
        End Get
    End Property
    Public ReadOnly Property RawFileFormat() As SapFormat
        Get
            Return m_Format
        End Get
    End Property
End Class