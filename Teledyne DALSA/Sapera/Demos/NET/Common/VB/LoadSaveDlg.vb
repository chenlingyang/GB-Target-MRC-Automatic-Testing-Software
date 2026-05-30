Class LoadSaveDlg

    ' General definitions
    Const SEQUENCE_FILTER As String = "AVI Files (*.avi)|*.avi||"

    ' File Formats 

    ' Type table definition
    Shared m_StandardTypes As SapBuffer.FileFormat()
    Shared m_SequenceTypes As SapBuffer.FileFormat() = {SapBuffer.FileFormat.AVI}

    Private m_TiffCompressionTypes As String() = {"corfile_val_compression_none", "corfile_val_compression_rle", "corfile_val_compression_lzw", "corfile_val_compression_jpg"}

    ' TIFF file definitions
    Const TIFF_DEFAULT_COMPRESSION_LEVEL As Integer = 90
    Const TIFF_DEFAULT_COMPRESSION_TYPE As Integer = 0
    ' JPEG file definitions
    Const JPEG_DEFAULT_QUALITY As Integer = 90

    ' RAW File defintions
    Const RAW_DEFAULT_WIDTH As Integer = 640
    Const RAW_DEFAULT_HEIGHT As Integer = 480
    Const RAW_DEFAULT_OFFSET As Integer = 0

    ' AVI file definitions
    Const AVI_DEFAULT_START_FRAME As Integer = 0
    Const AVI_DEFAULT_COMPRESSION_LEVEL As Integer = 90
    Const AVI_DEFAULT_COMPRESSION_TYPE As Integer = 0

    Public Class ConversionTable
        Public m_BufferFormat As SapFormat
        ' Support for saving respectively in BMP, TIFF, CRC, RAW, JPEG and JPEG2000
        Public m_FileFormat() As Integer

        Sub New(ByVal bufferFormat As SapFormat, ByRef fileFormat() As Integer)
            m_BufferFormat = bufferFormat
            m_FileFormat = fileFormat
        End Sub
    End Class

    Const UNSUPPORTED As Integer = -1       ' Not supported
    Const NO_CONVERSION As Integer = 0      ' Supported without conversion
    Const CONV_TO_MONO8 As Integer = 1      ' Supported but converted to MONO8
    Const CONV_TO_RGB8 As Integer = 2       ' Supported but converted to RGB888
    Const CONV_TO_RGB10 As Integer = 3      ' Supported but converted to RGB101010
    Const CONV_TO_RGB16 As Integer = 4      ' Supported but converted to RGB161616

    Private m_ConversionTable() As ConversionTable = _
    { _
        New ConversionTable(SapFormat.Mono8, New Integer() {NO_CONVERSION, NO_CONVERSION, NO_CONVERSION, NO_CONVERSION, NO_CONVERSION, NO_CONVERSION}), _
        New ConversionTable(SapFormat.Int8, New Integer() {CONV_TO_MONO8, NO_CONVERSION, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, NO_CONVERSION}), _
        New ConversionTable(SapFormat.Uint8, New Integer() {NO_CONVERSION, NO_CONVERSION, NO_CONVERSION, NO_CONVERSION, NO_CONVERSION, NO_CONVERSION}), _
        New ConversionTable(SapFormat.Mono16, New Integer() {CONV_TO_MONO8, NO_CONVERSION, NO_CONVERSION, NO_CONVERSION, CONV_TO_MONO8, NO_CONVERSION}), _
        New ConversionTable(SapFormat.Int16, New Integer() {CONV_TO_MONO8, NO_CONVERSION, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, NO_CONVERSION}), _
        New ConversionTable(SapFormat.Uint16, New Integer() {CONV_TO_MONO8, NO_CONVERSION, NO_CONVERSION, NO_CONVERSION, CONV_TO_MONO8, NO_CONVERSION}), _
        New ConversionTable(SapFormat.Mono32, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Int32, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Uint32, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono64, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Int64, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Uint64, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.RGB5551, New Integer() {NO_CONVERSION, CONV_TO_RGB8, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.RGB565, New Integer() {NO_CONVERSION, CONV_TO_RGB8, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, NO_CONVERSION}), _
        New ConversionTable(SapFormat.RGB888, New Integer() {NO_CONVERSION, NO_CONVERSION, NO_CONVERSION, NO_CONVERSION, NO_CONVERSION, NO_CONVERSION}), _
        New ConversionTable(SapFormat.RGBR888, New Integer() {CONV_TO_RGB8, CONV_TO_RGB8, NO_CONVERSION, NO_CONVERSION, CONV_TO_RGB8, CONV_TO_RGB8}), _
        New ConversionTable(SapFormat.RGB8888, New Integer() {NO_CONVERSION, CONV_TO_RGB8, NO_CONVERSION, NO_CONVERSION, NO_CONVERSION, NO_CONVERSION}), _
        New ConversionTable(SapFormat.RGB101010, New Integer() {NO_CONVERSION, CONV_TO_RGB16, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, NO_CONVERSION}), _
        New ConversionTable(SapFormat.RGB161616, New Integer() {CONV_TO_RGB10, NO_CONVERSION, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, NO_CONVERSION}), _
        New ConversionTable(SapFormat.RGB16161616, New Integer() {CONV_TO_RGB10, CONV_TO_RGB16, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.HSV, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.UYVY, New Integer() {CONV_TO_RGB8, CONV_TO_RGB8, NO_CONVERSION, NO_CONVERSION, CONV_TO_RGB8, CONV_TO_RGB8}), _
        New ConversionTable(SapFormat.YUY2, New Integer() {CONV_TO_RGB8, CONV_TO_RGB8, NO_CONVERSION, NO_CONVERSION, CONV_TO_RGB8, CONV_TO_RGB8}), _
        New ConversionTable(SapFormat.YVYU, New Integer() {CONV_TO_RGB8, CONV_TO_RGB8, NO_CONVERSION, NO_CONVERSION, CONV_TO_RGB8, CONV_TO_RGB8}), _
        New ConversionTable(SapFormat.YUYV, New Integer() {CONV_TO_RGB8, CONV_TO_RGB8, NO_CONVERSION, NO_CONVERSION, CONV_TO_RGB8, CONV_TO_RGB8}), _
        New ConversionTable(SapFormat.Y411, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Y211, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.YUV, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Float, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Complex, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Point, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.FPoint, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono1, New Integer() {NO_CONVERSION, NO_CONVERSION, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.HSI, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.RGBP8, New Integer() {CONV_TO_RGB8, CONV_TO_RGB8, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, NO_CONVERSION}), _
        New ConversionTable(SapFormat.RGBP16, New Integer() {CONV_TO_RGB10, CONV_TO_RGB16, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.RGBAP8, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.RGBAP16, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.BICOLOR88, New Integer() {CONV_TO_RGB8, CONV_TO_RGB8, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.BICOLOR1616, New Integer() {CONV_TO_RGB8, CONV_TO_RGB8, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.BICOLOR1212, New Integer() {CONV_TO_RGB8, CONV_TO_RGB8, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.RGB888_MONO8, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.RGB161616_MONO16, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono8P2, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono8P3, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono8P4, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono8P5, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono8P6, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono8P7, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono8P8, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono8P9, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono8P10, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono16P2, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono16P3, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono16P4, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono16P5, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono16P6, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono16P7, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono16P8, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono16P9, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}), _
        New ConversionTable(SapFormat.Mono16P10, New Integer() {UNSUPPORTED, UNSUPPORTED, NO_CONVERSION, NO_CONVERSION, UNSUPPORTED, UNSUPPORTED}) _
    }

    Private m_FileDialog As FileDialog
    Private m_pBuffer As SapBuffer
    Private m_PathName As String = ""
    Private m_Type As SapBuffer.FileFormat = SapBuffer.FileFormat.BMP
    Private m_Options As String
    Private m_bSequence As Boolean
    Private m_isOpen As Boolean

    Public Sub New(ByVal pBuffer As SapBuffer, ByVal isOpen As Boolean, ByVal isSequence As Boolean)
        m_pBuffer = pBuffer
        m_isOpen = isOpen
        m_bSequence = isSequence

        If IntPtr.Size = 8 Then
            ' 64-bit
            m_StandardTypes = New SapBuffer.FileFormat(4) {}
        Else
            ' 32-bit
            m_StandardTypes = New SapBuffer.FileFormat(5) {}
        End If

        m_StandardTypes(0) = SapBuffer.FileFormat.BMP
        m_StandardTypes(1) = SapBuffer.FileFormat.TIFF
        m_StandardTypes(2) = SapBuffer.FileFormat.CRC
        m_StandardTypes(3) = SapBuffer.FileFormat.RAW
        m_StandardTypes(4) = SapBuffer.FileFormat.JPEG

        ' 32-bit
        If IntPtr.Size = 4 Then
            m_StandardTypes(5) = SapBuffer.FileFormat.JPEG2000
        End If

        Initialize()
        ' Load parameters from registry
        LoadSettings()
        SetFilterIndex()
    End Sub

    Private Sub Initialize()
        'Create Open or Save Dialog
        If m_isOpen Then
            m_FileDialog = New OpenFileDialog()
            m_FileDialog.Title = "Loading a file..."
        Else
            m_FileDialog = New SaveFileDialog()
            m_FileDialog.Title = "Saving a file..."
        End If

        ' Set filter for image or sequence
        If m_bSequence Then
            m_FileDialog.Filter = SEQUENCE_FILTER
        Else
            If IntPtr.Size = 8 Then
                ' 64-bit
                m_FileDialog.Filter = "BMP Files (*.bmp)|*.bmp|TIF Files (*.tif)|*.tif|Teledyne DALSA Files (*.crc)|*.crc|RAW data Files (*.raw)|*.raw|JPEG Files (*.jpg)|*.jpg||"
            Else
                ' 32-bit
                m_FileDialog.Filter = "BMP Files (*.bmp)|*.bmp|TIF Files (*.tif)|*.tif|Teledyne DALSA Files (*.crc)|*.crc|RAW data Files (*.raw)|*.raw|JPEG Files (*.jpg)|*.jpg|JPEG 2000 Files (*.jp2)|*.jp2||"
            End If
        End If

        ' Set options and type
        m_FileDialog.AddExtension = True
        m_FileDialog.FileName = m_PathName
        m_FileDialog.RestoreDirectory = True
    End Sub

    Public Sub Dispose()
        m_FileDialog.Dispose()
        m_FileDialog = Nothing
    End Sub

    Private Sub SetFilterIndex()
        m_FileDialog.FilterIndex = 1

        If m_bSequence Then
            For i As Integer = 0 To m_SequenceTypes.GetLength(0) - 1
                If m_Type = m_SequenceTypes(i) Then
                    m_FileDialog.FilterIndex = i + 1
                    Return
                End If
            Next
        Else
            For i As Integer = 0 To m_StandardTypes.GetLength(0) - 1
                If m_Type = m_StandardTypes(i) Then
                    m_FileDialog.FilterIndex = i + 1
                    Return
                End If
            Next
        End If
    End Sub

    Private Function SetOptions() As Boolean
        Dim success As Boolean = True
        Select Case m_Type
            Case SapBuffer.FileFormat.BMP
                m_Options = "-format bmp"
                Exit Select
            Case SapBuffer.FileFormat.TIFF
                m_Options = "-format tif"
                If Not m_isOpen Then
                    success = AddTiffOptions()
                End If
                Exit Select
            Case SapBuffer.FileFormat.CRC
                m_Options = "-format crc"
                Exit Select
            Case SapBuffer.FileFormat.RAW
                m_Options = "-format raw"
                If m_isOpen Then
                    success = AddRawOptions()
                End If
                Exit Select
            Case SapBuffer.FileFormat.JPEG
                m_Options = "-format jpg"
                If Not m_isOpen Then
                    success = AddJpegOptions()
                End If
                Exit Select
            Case SapBuffer.FileFormat.JPEG2000
                m_Options = "-format jp2"
                If Not m_isOpen Then
                    success = AddJpegOptions()
                End If
                Exit Select
            Case SapBuffer.FileFormat.AVI
                m_Options = "-format avi"
                If m_isOpen Then
                    success = AddAviOptions()
                End If
                Exit Select
        End Select
        Return success
    End Function

    Private Sub LoadSettings()
        ' Read file name
        Dim keyPath As String = "Software\Teledyne DALSA\" + Application.ProductName + "\SapFile"
        Dim regkey As RegistryKey = Registry.CurrentUser.OpenSubKey(keyPath)
        If regkey IsNot Nothing Then
            m_PathName = regkey.GetValue("Name", "").ToString()
            m_Type = DirectCast((regkey.GetValue("Type", 0)), SapBuffer.FileFormat)
        End If
    End Sub

    Private Sub SaveSettings()
        ' Write file name
        Dim keyPath As String = "Software\Teledyne DALSA\" + Application.ProductName + "\SapFile"
        Dim regKey As RegistryKey = Registry.CurrentUser.CreateSubKey(keyPath)
        regKey.SetValue("Name", m_PathName)
        regKey.SetValue("Type", CInt(m_Type))
    End Sub

    Private Function AddTiffOptions() As Boolean
        Dim dlg As New TiffFileDlg(TIFF_DEFAULT_COMPRESSION_LEVEL, TIFF_DEFAULT_COMPRESSION_TYPE)

        If dlg.ShowDialog() <> DialogResult.OK Then
            Return False
        End If

        Dim strType As String = " -c " + m_TiffCompressionTypes(dlg.CompressionType)
        m_Options += strType
        Dim strLevel As String = " -q " + dlg.CompressionLevel.ToString()
        m_Options += strLevel
        Return True
    End Function

    Private Function AddJpegOptions() As Boolean
        Dim dlg As New JPEGFileDlg(JPEG_DEFAULT_QUALITY)

        If dlg.ShowDialog() <> DialogResult.OK Then
            Return False
        End If

        Dim strQuality As String = " -q " + dlg.JpegFileQuality.ToString()
        m_Options += strQuality
        Return True
    End Function

    Private Function AddAviOptions() As Boolean
        Dim dlg As New AviFileDlg(True, AVI_DEFAULT_START_FRAME, AVI_DEFAULT_COMPRESSION_LEVEL, AVI_DEFAULT_COMPRESSION_TYPE)

        If dlg.ShowDialog() <> DialogResult.OK Then
            Return False
        End If

        Return True
    End Function

    Private Function AddRawOptions() As Boolean
        Dim width As Integer = IIf(m_pBuffer IsNot Nothing, m_pBuffer.Width, RAW_DEFAULT_WIDTH)
        Dim height As Integer = IIf(m_pBuffer IsNot Nothing, m_pBuffer.Height, RAW_DEFAULT_HEIGHT)
        Dim format As SapFormat = IIf(m_pBuffer IsNot Nothing, m_pBuffer.Format, 0)
        Dim dlg As New RawFileDlg(width, height, RAW_DEFAULT_OFFSET, format)

        If dlg.ShowDialog() <> DialogResult.OK Then
            Return False
        End If

        Dim strQuality As String = " -width " + dlg.RawFileWidth.ToString() + " -height " + dlg.RawFileHeight.ToString() + " -offset " + dlg.RawFileOffset.ToString()
        m_Options += strQuality
        Return True

    End Function

    Public Function UpdateBuffer() As Boolean
        ' Set load/save options
        If Not SetOptions() Then
            Return False
        End If
        m_PathName = m_FileDialog.FileName
        If m_pBuffer IsNot Nothing Then
            If m_isOpen Then
                'Load File in the current buffer
                Return m_pBuffer.Load(m_PathName, -1, m_Options)
            Else
                Dim conversionType As Integer = 0
                Dim format As SapFormat = m_pBuffer.Format

                For i As Integer = 0 To m_ConversionTable.Length - 1
                    If m_ConversionTable(i).m_BufferFormat = format Then
                        Select Case m_Type
                            Case SapBuffer.FileFormat.BMP
                                conversionType = m_ConversionTable(i).m_FileFormat(0)
                            Case SapBuffer.FileFormat.TIFF
                                conversionType = m_ConversionTable(i).m_FileFormat(1)
                            Case SapBuffer.FileFormat.CRC
                                conversionType = m_ConversionTable(i).m_FileFormat(2)
                            Case SapBuffer.FileFormat.RAW
                                conversionType = m_ConversionTable(i).m_FileFormat(3)
                            Case SapBuffer.FileFormat.JPEG
                                conversionType = m_ConversionTable(i).m_FileFormat(4)
                            Case SapBuffer.FileFormat.JPEG2000
                                conversionType = m_ConversionTable(i).m_FileFormat(5)
                        End Select
                    End If
                Next

                If conversionType > 0 Then
                    Dim message As String = ""

                    Select Case conversionType
                        Case CONV_TO_MONO8
                            message = "Warning: data conversion to MONO8 has taken place to save the image in this file format"
                        Case CONV_TO_RGB8
                            message = "Warning: data conversion to RGB888 has taken place to save the image in this file format"
                        Case CONV_TO_RGB10
                            message = "Warning: data conversion to RGB101010 has taken place to save the image in this file format"
                        Case CONV_TO_RGB16
                            message = "Warning: data conversion to RGB161616 has taken place to save the image in this file format"
                    End Select

                    MessageBox.Show(message)
                End If

                Return m_pBuffer.Save(m_PathName, m_Options)
            End If
        End If
        Return True
    End Function

    Public Function ShowDialog() As DialogResult
        ' Set name and type
        m_FileDialog.FileName = m_PathName

        SetFilterIndex()

        Dim ret As DialogResult = m_FileDialog.ShowDialog()
        If ret <> DialogResult.OK Then
            Return ret
        End If

        ' Update type from filter index
        If m_bSequence Then
            m_Type = m_SequenceTypes(m_FileDialog.FilterIndex - 1)
        Else
            m_Type = m_StandardTypes(m_FileDialog.FilterIndex - 1)
        End If

        ' Update file object
        If Not UpdateBuffer() Then
            Return DialogResult.Abort
        End If

        ' Save parameters to registry
        SaveSettings()
        Return DialogResult.OK
    End Function

    Public ReadOnly Property PathName() As String
        Get
            Return m_FileDialog.FileName
        End Get
    End Property
End Class

