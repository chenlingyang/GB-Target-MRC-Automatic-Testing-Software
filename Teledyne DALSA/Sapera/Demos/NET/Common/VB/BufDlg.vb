Public Class BufDlg

    Private m_isXfer As Boolean
    Private m_Count As Integer
    Private m_Width As Integer
    Private m_Height As Integer
    Private m_pixelDepth As Integer
    Private m_format As SapFormat
    Private m_Type As SapBuffer.MemoryType
    Private m_ScatterGatherType As SapBuffer.MemoryType
    Private m_sapDisplay As SapDisplay



    Public Sub New(ByVal pBuffer As SapBuffer, ByVal pDisplay As SapDisplay, ByVal Online As Boolean)

        InitializeComponent()
        m_isXfer = Online
        m_sapDisplay = pDisplay

        m_Count = pBuffer.Count
        m_Width = pBuffer.Width
        m_Height = pBuffer.Height
        m_pixelDepth = pBuffer.PixelDepth
        m_format = pBuffer.Format
        m_Type = pBuffer.Type
        m_ScatterGatherType = SapBuffer.MemoryType.ScatterGather

        ' For acquisition hardware with DMA transfers using 32-bit addresses in 64-bit Windows (e.g., X64-CL iPro)
        If pBuffer.SrcNode IsNot Nothing Then
            If Not SapBuffer.IsBufferTypeSupported(pBuffer.SrcNode.Location, SapBuffer.MemoryType.ScatterGather) Then
                m_ScatterGatherType = SapBuffer.MemoryType.ScatterGatherPhysical
            End If
        End If

        textBox1_Count.Text = m_Count.ToString()
        textBox1_Width.Text = m_Width.ToString()
        textBox1_Height.Text = m_Height.ToString()
        textBox1_PixelDepth.Text = m_pixelDepth.ToString()

        Initialize_Format_Combo()
        Initialize_Type_Flags()
        EnableControls()
    End Sub

    Private Sub Initialize_Format_Combo()
        Dim Index As Integer = 0
        Dim formatInfo As New SapFormatInfo(SapFormat.Mono1, "Monochrome 1-bit")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 0
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono8, "Monochrome 8-bit")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 1
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono16, "Monochrome 16-bit")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 2
        End If
        formatInfo = New SapFormatInfo(SapFormat.RGB5551, "RGB 5551")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 3
        End If
        formatInfo = New SapFormatInfo(SapFormat.RGB565, "RGB 565")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 4
        End If
        formatInfo = New SapFormatInfo(SapFormat.RGB888, "RGB 888 (blue first)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 5
        End If
        formatInfo = New SapFormatInfo(SapFormat.RGBR888, "RGBR 888 (red first)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 6
        End If
        formatInfo = New SapFormatInfo(SapFormat.RGB8888, "RGB 8888")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 7
        End If
        formatInfo = New SapFormatInfo(SapFormat.RGB101010, "RGB 101010")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 8
        End If
        formatInfo = New SapFormatInfo(SapFormat.RGB161616, "RGB 161616")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 9
        End If
        formatInfo = New SapFormatInfo(SapFormat.RGB16161616, "RGB 16161616")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 10
        End If
        formatInfo = New SapFormatInfo(SapFormat.RGBP8, "RGB Planar 8-bit")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 11
        End If
        formatInfo = New SapFormatInfo(SapFormat.RGBP16, "RGB Planar 16-bit")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 12
        End If
        formatInfo = New SapFormatInfo(SapFormat.HSI, "HSI")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 13
        End If
        formatInfo = New SapFormatInfo(SapFormat.HSIP8, "HSI Planar 8-bit")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 14
        End If
        formatInfo = New SapFormatInfo(SapFormat.HSV, "HSV")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 15
        End If
        formatInfo = New SapFormatInfo(SapFormat.UYVY, "UYVY")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 16
        End If
        formatInfo = New SapFormatInfo(SapFormat.YUY2, "YUY2")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 17
        End If
        formatInfo = New SapFormatInfo(SapFormat.YUYV, "YUYV")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 18
        End If
        formatInfo = New SapFormatInfo(SapFormat.LAB, "LAB")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 19
        End If
        formatInfo = New SapFormatInfo(SapFormat.LABP8, "LAB Planar 8-bit")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 20
        End If
        formatInfo = New SapFormatInfo(SapFormat.LAB101010, "LAB 101010")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 21
        End If
        formatInfo = New SapFormatInfo(SapFormat.LABP16, "LAB Planar 16-bit")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 22
        End If
        formatInfo = New SapFormatInfo(SapFormat.BICOLOR88, "BICOLOR 8-bit")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 23
        End If
        formatInfo = New SapFormatInfo(SapFormat.BICOLOR1212, "BICOLOR 12-bit packed")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 24
        End If
        formatInfo = New SapFormatInfo(SapFormat.BICOLOR1616, "BICOLOR 16-bit")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 25
        End If
        formatInfo = New SapFormatInfo(SapFormat.RGB888_MONO8, "RGB-IR 8-bit")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 26
        End If
        formatInfo = New SapFormatInfo(SapFormat.RGB161616_MONO16, "RGB-IR 16-bit")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 27
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono8P2, "Monochrome 8-bit (2 planes)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 28
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono8P3, "Monochrome 8-bit (3 planes)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 29
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono8P4, "Monochrome 8-bit (4 planes)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 30
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono8P5, "Monochrome 8-bit (5 planes)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 31
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono8P6, "Monochrome 8-bit (6 planes)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 32
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono8P7, "Monochrome 8-bit (7 planes)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 33
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono8P8, "Monochrome 8-bit (8 planes)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 34
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono8P9, "Monochrome 8-bit (9 planes)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 35
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono8P10, "Monochrome 8-bit (10 planes)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 36
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono16P2, "Monochrome 16-bit (2 planes)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 37
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono16P3, "Monochrome 16-bit (3 planes)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 38
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono16P4, "Monochrome 16-bit (4 planes)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 39
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono16P5, "Monochrome 16-bit (5 planes)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 40
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono16P6, "Monochrome 16-bit (6 planes)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 41
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono16P7, "Monochrome 16-bit (7 planes)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 42
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono16P8, "Monochrome 16-bit (8 planes)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 43
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono16P9, "Monochrome 16-bit (9 planes)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 44
        End If
        formatInfo = New SapFormatInfo(SapFormat.Mono16P10, "Monochrome 16-bit (10 planes)")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 45
        End If
        formatInfo = New SapFormatInfo(SapFormat.RGBAP8, "RGB Alpha Planar 8-bit")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 46
        End If
        formatInfo = New SapFormatInfo(SapFormat.RGBAP16, "RGB Alpha Planar 16-bit")
        comboBox1_Format.Items.Add(formatInfo)
        If m_format = formatInfo.Value Then
            Index = 47
        End If

        comboBox1_Format.SelectedIndex = Index
    End Sub

    Private Sub Initialize_Type_Flags()
        radioButton1_Contiguous.Checked = (m_Type = SapBuffer.MemoryType.Contiguous)
        radioButton1_ScatterGather.Checked = (m_Type = m_ScatterGatherType)
        radioButton1_Offscreen_Video.Checked = (m_Type = SapBuffer.MemoryType.OffscreenVideo)
        radioButton1_Overlay.Checked = (m_Type = SapBuffer.MemoryType.Overlay)
        radioButton1_Virtual.Checked = (m_Type = SapBuffer.MemoryType.Virtual)
    End Sub

    Private Sub EnableControls()
        Dim isOffscreenAvailable As Boolean = IIf((m_sapDisplay IsNot Nothing), m_sapDisplay.IsOffscreenAvailable(m_format), True)
        Dim isOverLayAvailable As Boolean = IIf((m_sapDisplay IsNot Nothing), m_sapDisplay.IsOverlayAvailable(m_format), True)
        radioButton1_Offscreen_Video.Enabled = isOffscreenAvailable
        radioButton1_Overlay.Enabled = isOverLayAvailable
        radioButton1_Virtual.Enabled = Not m_isXfer
        textBox1_Width.Enabled = Not m_isXfer
        textBox1_Height.Enabled = Not m_isXfer
        comboBox1_Format.Enabled = Not m_isXfer

        ' Is pixel depth adjustable?
        If SapManager.GetPixelDepthMin(m_format) <> SapManager.GetPixelDepthMax(m_format) Then
            textBox1_PixelDepth.Enabled = True
            textBox1_PixelDepth.Text = m_pixelDepth.ToString()
        Else
            textBox1_PixelDepth.Enabled = False
        End If
    End Sub

    Private Sub comboBox1_Format_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles comboBox1_Format.SelectedIndexChanged
        Dim FormatInfo As SapFormatInfo = comboBox1_Format.SelectedItem
        m_format = FormatInfo.Value
        EnableControls()
    End Sub

    Private Sub radioButton1_Contiguous_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radioButton1_Contiguous.CheckedChanged
        If radioButton1_Contiguous.Checked Then
            m_Type = SapBuffer.MemoryType.Contiguous
        End If
    End Sub

    Private Sub radioButton1_ScatterGather_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radioButton1_ScatterGather.CheckedChanged
        If radioButton1_ScatterGather.Checked Then
            m_Type = m_ScatterGatherType
        End If
    End Sub

    Private Sub radioButton1_Offscreen_Video_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radioButton1_Offscreen_Video.CheckedChanged
        If radioButton1_Offscreen_Video.Checked Then
            m_Type = SapBuffer.MemoryType.OffscreenVideo
        End If
    End Sub

    Private Sub radioButton1_Overlay_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radioButton1_Overlay.CheckedChanged
        If radioButton1_Overlay.Checked Then
            m_Type = SapBuffer.MemoryType.Overlay
        End If
    End Sub

    Private Sub radioButton1_Virtual_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radioButton1_Virtual.CheckedChanged
        If radioButton1_Virtual.Checked Then
            m_Type = SapBuffer.MemoryType.Virtual
        End If
    End Sub

    Private Sub button1_OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button1_OK.Click
        m_Count = Integer.Parse(textBox1_Count.Text)
        m_Width = Integer.Parse(textBox1_Width.Text)
        m_Height = Integer.Parse(textBox1_Height.Text)
        m_pixelDepth = Integer.Parse(textBox1_PixelDepth.Text)
    End Sub

    Public ReadOnly Property BWidth() As Integer
        Get
            Return m_Width
        End Get
    End Property
    Public ReadOnly Property BHeight() As Integer
        Get
            Return m_Height
        End Get
    End Property
    Public ReadOnly Property Count() As Integer
        Get
            Return m_Count
        End Get
    End Property
    Public ReadOnly Property Format() As SapFormat
        Get
            Return m_format
        End Get
    End Property
    Public ReadOnly Property Type() As SapBuffer.MemoryType
        Get
            Return m_Type
        End Get
    End Property
End Class
' Format table
Public Class SapFormatInfo
    Public Sub New(ByVal format As SapFormat, ByVal name As String)
        m_Value = format
        m_Name = name
    End Sub

    Public Overloads Overrides Function ToString() As String
        Return m_Name
    End Function

    Public Property Value() As SapFormat
        Get
            Return m_Value
        End Get
        Set(ByVal value As SapFormat)
            m_Value = value
        End Set
    End Property

    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(ByVal value As String)
            m_Name = value
        End Set
    End Property
    Private m_Value As SapFormat
    Private m_Name As String
End Class
