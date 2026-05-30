Partial Public Class FlatFieldDlg
    Inherits Form

   Private Const DEFAULT_FFC_FILENAME As String = "FFC.tif"
   Private Const STANDARD_FILTER As String = "TIFF Files (*.tif)|*.tif||"
   Private Const DEFAULT_FFC_CLUSTER_MAP_FILENAME As String = "clustermap.csv"
   Private Const FFC_CLUSTER_MAP_FILTER As String = "CSV (Comma delimited) (*.csv)|*.csv||"
    Dim m_isGenie As Boolean = False
    Dim m_UserFlatFieldCount As Integer = 0
    Dim flatFieldIndexes As Integer()
    Const SapDefFlatFieldAvgFactorBlack As Single = 0.25F
    Const SapDefFlatFieldAvgFactorWhite As Single = 0.8F
    Const SapDefFlatFieldDefectRatio As Single = 1.0F
    Const SapDefFlatFieldPixelRatio As Single = 99.0F
    ' Various constants
    Const DefNumFramesAverage As Integer = 10


    ' Log message types
    Private Enum LogTypes
        [Error]
        Warning
        Info
    End Enum


    Private m_pFlatField As SapFlatField
    Private m_pXfer As SapTransfer
    Private m_pBuffer As SapBuffer
    Private m_pLocalBuffer As SapBuffer
    Private m_CorrectionType As SapFlatField.ScanCorrectionType
    Private m_VideoType As SapAcquisition.VideoType
    Private m_RecommendedDark As Integer
    Private m_RecommendedBright As Integer


    Public Sub New(ByVal pFlatField As SapFlatField, ByVal pXfer As SapTransfer, ByVal pBuffer As SapBuffer)
        InitializeComponent()
        m_pBuffer = pBuffer
        m_pFlatField = pFlatField
        m_pXfer = pXfer

        m_RecommendedDark = 0
        m_RecommendedBright = 0
    End Sub

    Private Sub FlatFieldDlg_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If m_pBuffer Is Nothing OrElse Not m_pBuffer.Initialized Then
            MessageBox.Show("Invalid buffer object")
            Me.Close()
            Exit Sub
        End If

        If m_pFlatField Is Nothing Then
            MessageBox.Show("Invalid flat-field object")
            Me.Close()
            Exit Sub
        End If

        Dim str As String
        Dim isOffLine As Boolean = (m_pXfer Is Nothing)

        If isOffLine Then
            label_darkImage1.Text = "Step 1: Load a Dark image"
            button_Acq_Dark.Text = "Load a Dark Image"
            label_brightImage1.Text = "Load a non-saturated bright image"
            button_Acq_Bright.Text = "Load Bright Image"
        Else
            label_darkImage1.Text = "Step 1: Acquire a Dark image"
            button_Acq_Dark.Text = "Acquire a Dark Image"
            label_brightImage1.Text = "Acquire a non-saturated bright image"
            button_Acq_Bright.Text = "Acquire Bright Image"
        End If

        ' Calculate recommended values for dark and bright images
        Dim maxPixelValue as Integer = CInt(Math.Pow(2.0R, m_pBuffer.PixelDepth))
        m_RecommendedDark = CInt(maxPixelValue * SapDefFlatFieldAvgFactorBlack)
        m_RecommendedBright = CInt(maxPixelValue * SapDefFlatFieldAvgFactorWhite)

        ' Adjust the recommended dark value according to the hardware limit for offset coefficients
        Dim offsetMax As Integer = m_pFlatField.OffsetMax

        If m_RecommendedDark > offsetMax Then
            m_RecommendedDark = offsetMax
        End If

        str = [String].Format("(recommended average pixel" & vbLf & "value below {0})", m_RecommendedDark)
        label_darkImage2.Text = str

        str = [String].Format("(recommended average pixel" & vbLf & "value around {0})" & vbLf, m_RecommendedBright)
        label_brightImage2.Text = str

        ' Adjust maximum black deviation from the adjusted recommended dark value
        If m_pFlatField.DeviationMaxBlack > m_RecommendedDark Then
            m_pFlatField.DeviationMaxBlack = m_RecommendedDark
        End If

        ' Query correction type, video type, number of lines to average (line scan),
        ' vertical offset, gain divisor, maximum deviation, and calibration index
        ' for the dark image from flat field object
        m_CorrectionType = m_pFlatField.CorrectionType
        m_VideoType = m_pFlatField.AcqVideoType
        textBox_Line_Avg.Text = m_pFlatField.NumLinesAverage.ToString()
        textBox_Vert_Offset.Text = m_pFlatField.VerticalOffset.ToString()
        textBox_Max_Dev.Text = m_pFlatField.DeviationMaxBlack.ToString()
        'm_CalibrationIndex = m_pFlatField->GetIndex();
        textBox_Frame_Avg.Text = DefNumFramesAverage.ToString()
        If m_pFlatField.ClippedGainOffsetDefects Then
            ClippedCoefsDefects_checkbox.Checked = True
        Else
            ClippedCoefsDefects_checkbox.Checked = False
        End If

        ' Set correction type
        comboBox_Correction_Type.Items.Add("Flat Field")
        comboBox_Correction_Type.Items.Add("Flat Line")
        If m_CorrectionType = SapFlatField.ScanCorrectionType.Field Then
            comboBox_Correction_Type.SelectedIndex = 0
        Else
            comboBox_Correction_Type.SelectedIndex = 1
        End If

        ' Set video type
        comboBox_Video_Type.Items.Add("Monochrome")
        comboBox_Video_Type.Items.Add("Bayer")
        If m_VideoType = SapAcquisition.VideoType.Mono Then
            comboBox_Video_Type.SelectedIndex = 0
        Else
            comboBox_Video_Type.SelectedIndex = 1
        End If

        '
        'Multi flat-field not implemented in .NET
        '
        ' Set calibration index
        ' String indexStr;

        'for (int index = 0; index < m_pFlatFieldGetNumFlatField(); index++)
        '{
        '   indexStr.Format("%d", index);
        '   m_CalibrationIndexCtrl.AddString( indexStr);
        '}

        comboBox_Calibration_Index.Items.Add(0)
        comboBox_Calibration_Index.SelectedIndex = 0

        If (m_pFlatField.AcqDevice IsNot Nothing) Then
            Dim feature As SapFeature = New SapFeature(m_pFlatField.AcqDevice.Location)
            feature.Create()

            Dim status As Boolean = False
            Dim deviceModel As String = ""

            'for all Genies except Genie TS, do not try to do a file access
            status = m_pFlatField.AcqDevice.IsFeatureAvailable("DeviceModelName")
            If (status) Then
                m_pFlatField.AcqDevice.GetFeatureValue("DeviceModelName", deviceModel)
                If (deviceModel.Contains("Genie") And Not deviceModel.Contains("TS")) Then

                    'device is a Genie but not Genie TS
                    m_isGenie = True
                    button_Save_And_Upload.Text = "Save"
                    comboBox_FlatField_Selector.Visible = False
                    label_flatField.Visible = False
                    GoTo nextPart
                End If
            End If

            For i As Integer = 0 To m_pFlatField.AcqDevice.FileCount - 1
                If (m_pFlatField.AcqDevice.FileNames(i).Contains("FlatField")) Then
                    Dim featureName As String = "##FileSelector." + m_pFlatField.AcqDevice.FileNames(i)
                    If (Not m_pFlatField.AcqDevice.FileNames(i).Contains("0")) Then
                        status = m_pFlatField.AcqDevice.GetFeatureInfo(featureName, feature)
                        If (status) Then
                            comboBox_FlatField_Selector.Items.Add(feature.DisplayName)
                        Else
                            comboBox_FlatField_Selector.Items.Add(m_pFlatField.AcqDevice.FileNames(i))
                        End If
                        ReDim Preserve flatFieldIndexes(m_UserFlatFieldCount + 1)
                        flatFieldIndexes(m_UserFlatFieldCount) = i
                        m_UserFlatFieldCount = m_UserFlatFieldCount + 1
                    End If
                End If
            Next
            comboBox_FlatField_Selector.SelectedIndex = 0
            feature.Destroy()
            saveLabel.Text = "Save Calibration offset and gain files"
        Else

            button_Save_And_Upload.Text = "Save"
            comboBox_FlatField_Selector.Visible = False
            label_flatField.Visible = False

        End If
nextPart:

        ' Enable/disable controls according to current properties
        ' Changing the correction type (flat-field vs flat-line) is only relevant when operating
        ' offline, that is, when this information is not available from the acquisition hardware.
        Dim isOnline As Boolean = (m_pXfer IsNot Nothing AndAlso m_pXfer.Initialized)
        comboBox_Correction_Type.Enabled = Not isOnline AndAlso m_VideoType <> SapAcquisition.VideoType.Bayer
        comboBox_Video_Type.Enabled = m_pXfer Is Nothing

        textBox_Frame_Avg.Enabled = m_pXfer IsNot Nothing
        textBox_Line_Avg.Enabled = m_CorrectionType = SapFlatField.ScanCorrectionType.Line
        textBox_Vert_Offset.Enabled = m_CorrectionType = SapFlatField.ScanCorrectionType.Line
        textBox_Max_Dev.Enabled = True

        'Multi flat-field not implemented in .NET
        comboBox_Calibration_Index.Enabled = False
        'm_pFlatField->GetNumFlatField() > 1);
        button_Acq_Dark.Enabled = True
        button_Acq_Bright.Enabled = False
        button_OK.Enabled = False
        button_Save_And_Upload.Enabled = False
        comboBox_FlatField_Selector.Enabled = False
    End Sub

    Private Sub button_Acq_Dark_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Acq_Dark.Click

        Dim nbImagesUsed As Integer
        If m_pFlatField.CorrectionType = SapFlatField.ScanCorrectionType.Field Then
            nbImagesUsed = Integer.Parse(textBox_Frame_Avg.Text)
        Else
            nbImagesUsed = 1
        End If

        ' Set correction type
        m_pFlatField.CorrectionType = m_CorrectionType

        ' Set video type
        m_pFlatField.SetVideoType(m_VideoType, SapBayer.AlignMode.BGGR)

        ' Set maximum deviation from average pixel value for dark image
        m_pFlatField.DeviationMaxBlack = Integer.Parse(textBox_Max_Dev.Text)

        ' Set number of lines to average and vertical offset
        m_pFlatField.NumLinesAverage = Integer.Parse(textBox_Line_Avg.Text)
        m_pFlatField.VerticalOffset = Integer.Parse(textBox_Vert_Offset.Text)

        ' Set wether to declare pixels with clipped coefficient as defective
        m_pFlatField.ClippedGainOffsetDefects = ClippedCoefsDefects_checkbox.Checked

        'Multi flat-field not implemented in .NET
        ' Set calibration index
        'm_pFlatField->SetIndex(m_CalibrationIndex);

        LogMessageBox.ResetText()

        If m_pXfer IsNot Nothing AndAlso m_pXfer.Initialized Then
            m_pLocalBuffer = New SapBuffer(nbImagesUsed, m_pBuffer, SapBuffer.MemoryType.[Default])
            m_pLocalBuffer.Create()

            ' Acquire an image
            If Not Snap() Then
                LogMessage(LogTypes.[Error], "Unable to acquire an image")
                If m_pLocalBuffer IsNot Nothing Then
                    m_pLocalBuffer.Destroy()
                    m_pLocalBuffer.Dispose()
                    m_pLocalBuffer = Nothing
                End If
                Exit Sub
            End If
        Else
            ' Load an image

            m_pLocalBuffer = New SapBuffer(1, m_pBuffer, SapBuffer.MemoryType.[Default])
            m_pLocalBuffer.Create()

            Dim dlg As New LoadSaveDlg(Nothing, True, False)
            If dlg.ShowDialog() <> Windows.Forms.DialogResult.OK Then
                If m_pLocalBuffer IsNot Nothing Then
                    m_pLocalBuffer.Destroy()
                    m_pLocalBuffer.Dispose()
                    m_pLocalBuffer = Nothing
                End If
                Exit Sub
            End If

            Dim path As String = dlg.PathName

            ' Create a temporary buffer in order to know the selected file's native format and pixel depth

            Dim loadBuffer As New SapBuffer(path, SapBuffer.MemoryType.[Default])
            loadBuffer.Create()

            If loadBuffer.Format <> m_pBuffer.Format OrElse loadBuffer.PixelDepth <> m_pBuffer.PixelDepth Then
                LogMessage(LogTypes.Warning, "Image file has a different format than expected.  Pixel values may get shifted.")
            End If

            If loadBuffer.Width <> m_pBuffer.Width OrElse loadBuffer.Height <> m_pBuffer.Height Then
                LogMessage(LogTypes.[Error], "Image file selected doesn't have same dimensions as buffer.")
                If m_pLocalBuffer IsNot Nothing Then
                    m_pLocalBuffer.Destroy()
                    m_pLocalBuffer.Dispose()
                    m_pLocalBuffer = Nothing
                End If
                Exit Sub
            End If

            'loadBuffer.Load(path,1);
            m_pLocalBuffer.Copy(loadBuffer)

            Dim str As String
            str = [String].Format("Loaded dark image: {}", path)
            LogMessage(LogTypes.Info, str)
        End If
        DarkImage()
    End Sub


    '
    ' Step 2: Snap a bright image to calculate the gain coefficients
    '
    Private Sub button_Acq_Bright_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Acq_Bright.Click
        Dim nbImagesUsed As Integer
        If m_pFlatField.CorrectionType = SapFlatField.ScanCorrectionType.Field Then
            nbImagesUsed = Integer.Parse(textBox_Frame_Avg.Text)
        Else
            nbImagesUsed = 1
        End If

        ' Set maximum deviation from average pixel value for bright image
        m_pFlatField.DeviationMaxWhite = Integer.Parse(textBox_Max_Dev.Text)

        ' Set number of lines to average and vertical offset
        m_pFlatField.NumLinesAverage = Integer.Parse(textBox_Line_Avg.Text)
        m_pFlatField.VerticalOffset = Integer.Parse(textBox_Vert_Offset.Text)

        ' Set wether to declare pixels with clipped coefficient as defective
        m_pFlatField.ClippedGainOffsetDefects = ClippedCoefsDefects_checkbox.Checked

        If m_pXfer IsNot Nothing AndAlso m_pXfer.Initialized Then
            m_pLocalBuffer = New SapBuffer(nbImagesUsed, m_pBuffer, SapBuffer.MemoryType.[Default])
            m_pLocalBuffer.Create()

            ' Acquire an image
            If Not Snap() Then
                LogMessage(LogTypes.[Error], "Unable to acquire an image")
                If m_pLocalBuffer IsNot Nothing Then
                    m_pLocalBuffer.Destroy()
                    m_pLocalBuffer.Dispose()
                    m_pLocalBuffer = Nothing
                End If
                Exit Sub
            End If
        Else
            ' Load an image
            m_pLocalBuffer = New SapBuffer(1, m_pBuffer, SapBuffer.MemoryType.[Default])
            m_pLocalBuffer.Create()


            Dim dlg As New LoadSaveDlg(Nothing, True, False)
            If dlg.ShowDialog() <> Windows.Forms.DialogResult.OK Then
                If m_pLocalBuffer IsNot Nothing Then
                    m_pLocalBuffer.Destroy()
                    m_pLocalBuffer.Dispose()
                    m_pLocalBuffer = Nothing
                End If
                Exit Sub
            End If

            Dim path As String = dlg.PathName

            ' Create a temporary buffer in order to know the selected file's native format and pixel depth

            Dim loadBuffer As New SapBuffer(path, SapBuffer.MemoryType.[Default])
            loadBuffer.Create()

            If loadBuffer.Format <> m_pBuffer.Format OrElse loadBuffer.PixelDepth <> m_pBuffer.PixelDepth Then
                LogMessage(LogTypes.Warning, "Image file has a different format than expected.  Pixel values may get shifted.")
            End If

            If loadBuffer.Width <> m_pBuffer.Width OrElse loadBuffer.Height <> m_pBuffer.Height Then
                LogMessage(LogTypes.[Error], "Image file selected doesn't have same dimensions as buffer.")
                If m_pLocalBuffer IsNot Nothing Then
                    m_pLocalBuffer.Destroy()
                    m_pLocalBuffer.Dispose()
                    m_pLocalBuffer = Nothing
                End If
                Exit Sub
            End If

            loadBuffer.Load(path, 1)
            m_pLocalBuffer.Copy(loadBuffer)

            Dim str As String
            str = [String].Format("Loaded bright image: '{0}'", path)
            LogMessage(LogTypes.Info, str)
        End If

        BrightImage()
    End Sub

    Private Sub button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_OK.Click
        If m_pBuffer IsNot Nothing AndAlso m_pBuffer.Initialized Then
            m_pBuffer.Clear()
        End If
    End Sub

    Private Sub button2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Cancel.Click
        If m_pBuffer IsNot Nothing AndAlso m_pBuffer.Initialized Then
            m_pBuffer.Clear()
        End If
    End Sub

    Private Sub comboBox_Correction_Type_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles comboBox_Correction_Type.SelectedIndexChanged
        If comboBox_Correction_Type.SelectedIndex = 0 Then
            m_CorrectionType = SapFlatField.ScanCorrectionType.Field
        Else
            m_CorrectionType = SapFlatField.ScanCorrectionType.Line
        End If

        textBox_Line_Avg.Enabled = m_CorrectionType = SapFlatField.ScanCorrectionType.Line
        textBox_Vert_Offset.Enabled = m_CorrectionType = SapFlatField.ScanCorrectionType.Line
        comboBox_Video_Type.Enabled = m_CorrectionType = SapFlatField.ScanCorrectionType.Field
    End Sub

    Private Sub comboBox_Video_Type_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles comboBox_Video_Type.SelectedIndexChanged
        If comboBox_Video_Type.SelectedIndex = 0 Then
            m_VideoType = SapAcquisition.VideoType.Mono
        Else
            m_VideoType = SapAcquisition.VideoType.Bayer
        End If
        comboBox_Correction_Type.Enabled = m_VideoType = SapAcquisition.VideoType.Mono
    End Sub

    Private Sub comboBox1_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles comboBox_Calibration_Index.SelectedIndexChanged
        'int curSel = m_CalibrationIndexCtrl.GetCurSel();
        'if (curSel != CB_ERR)
        '   m_CalibrationIndex = curSel;
    End Sub

    Private Sub DarkImage()
        Dim str As String
        Dim stats As New SapFlatFieldStats()

        str = [String].Format("Correction type: {0}", m_CorrectionType)
        LogMessage(LogTypes.Info, str)

        str = [String].Format("Video type: {0}", m_VideoType)
        LogMessage(LogTypes.Info, str)

        If m_pXfer IsNot Nothing Then
            str = [String].Format("Number of frames to average: {0}", textBox_Frame_Avg.Text)
            LogMessage(LogTypes.Info, str)
        End If

        If m_CorrectionType = SapFlatField.ScanCorrectionType.Line Then
            str = [String].Format("Number of lines to average: {0}", textBox_Line_Avg.Text)
            LogMessage(LogTypes.Info, str)

            str = [String].Format("Vertical offset from top: {0}", textBox_Vert_Offset.Text)
            LogMessage(LogTypes.Info, str)
        End If

        LogMessage(LogTypes.Info, "Dark image calibration")

        If Not m_pFlatField.GetStats(m_pLocalBuffer, stats) Then
            LogMessage(LogTypes.[Error], "   Unable to get image statistics")
            Exit Sub
        End If

        Dim tooManyBadPixels As Boolean = False
        Dim numComponents As Integer = stats.NumComponents

        For i As Integer = 0 To numComponents
            If stats.Average(i) > m_RecommendedDark Then
                tooManyBadPixels = True
                Exit For
            End If
        Next

        If tooManyBadPixels And m_pFlatField.ClippedGainOffsetDefects Then
            str = "The following statistics have been computed on the dark image: " & vbLf
            str += [String].Format("The average pixel value is {0}" & vbLf, GetAverageStr(stats))
            str += [String].Format(vbLf & "This yields too many bad pixels above the hardware limit of {0}" & vbLf, m_RecommendedDark)
            str += [String].Format(vbLf & "To disable bad pixels, uncheck the 'Consider as defective ...'" & vbLf)
            str += [String].Format("checkbox, then acquire the dark image again" & vbLf)

            MessageBox.Show(str, "", MessageBoxButtons.OK)
            Exit Sub
        Else
            str = "The following statistics have been computed on the dark image: " & vbLf
            str += [String].Format("The average pixel value is {0}" & vbLf, GetAverageStr(stats))
            str += [String].Format(vbLf & "We recommend an average pixel value of less than {0}" & vbLf, m_RecommendedDark)
            str += [String].Format(vbLf & "Do you want to use this image?")

            If MessageBox.Show(str, "", MessageBoxButtons.YesNo) <> Windows.Forms.DialogResult.Yes Then
                Exit Sub
            End If
        End If

        ' Log pixel statistics
        str = [String].Format("    Average pixel value: {0}", GetAverageStr(stats))
        LogMessage(LogTypes.Info, str)

        str = [String].Format("    Maximum deviation allowed from average pixel value: {0}", textBox_Max_Dev.Text)
        LogMessage(LogTypes.Info, str)

        ' Compute offset coefficients using last acquired image
        m_pFlatField.NumFramesAverage = m_pLocalBuffer.Count
        If m_pFlatField.ComputeOffset(m_pLocalBuffer) Then
            button_Acq_Dark.Enabled = False
            button_Acq_Bright.Enabled = True

            comboBox_Correction_Type.Enabled = False
            comboBox_Video_Type.Enabled = False
            comboBox_Calibration_Index.Enabled = False

            LogMessage(LogTypes.Info, "Calibration with a dark image has been done successfully")
            textBox_Max_Dev.Text = m_pFlatField.DeviationMaxWhite.ToString()

        End If
    End Sub

    Private Sub BrightImage()
        Dim str As String
        Dim stats As New SapFlatFieldStats()

        If m_pXfer IsNot Nothing Then
            str = [String].Format("Number of frames to average: {0}", textBox_Frame_Avg.Text)
            LogMessage(LogTypes.Info, str)
        End If

        If m_CorrectionType = SapFlatField.ScanCorrectionType.Line Then
            str = [String].Format("Number of lines to average: {0}", textBox_Line_Avg.Text)
            LogMessage(LogTypes.Info, str)

            str = [String].Format("Vertical offset from top: {0}", textBox_Vert_Offset.Text)
            LogMessage(LogTypes.Info, str)
        End If

        LogMessage(LogTypes.Info, "Bright image calibration")

        ' Get statistics on the (bright - dark) image
        If Not m_pFlatField.GetStats(m_pLocalBuffer, stats) Then
            LogMessage(LogTypes.[Error], "Unable to get image statistic")
            Exit Sub
        End If

        str = "The following statistics have been computed on the bright image" & vbLf
        str += "after the substraction of the dark image:" & vbLf & vbLf
        str += [String].Format("    The average pixel value is {0}levels." & vbLf, GetAverageStr(stats))
        str += [String].Format("    The highest peak has been detected at {0}." & vbLf, GetPeakPositionStr(stats))
        str += [String].Format("    {0} pixels {1} have a luminance value between {2} to {3}" & vbLf, GetNumPixelsStr(stats), GetPixelRatioStr(stats), GetLowStr(stats), GetHighStr(stats))
        str += [String].Format(vbLf & "We recommend at least {0} levels for the highest peak value" & vbLf, m_RecommendedBright)
        str += [String].Format("with {0}% pixels lying between the lower and the higher bound." & vbLf, SapDefFlatFieldPixelRatio)
        str += vbLf & "Do you want to use this image?"

        If MessageBox.Show(str, "", MessageBoxButtons.YesNo) <> Windows.Forms.DialogResult.Yes Then
            Exit Sub
        End If

        ' Log average pixel value, lower and higher bounds and pixel ratio
        str = [String].Format("    Average pixel value: {0}", GetAverageStr(stats))
        LogMessage(LogTypes.Info, str)
        str = [String].Format("    Maximum deviation allowed from average pixel value: {0}", textBox_Max_Dev.Text)
        LogMessage(LogTypes.Info, str)
        str = [String].Format("    Highest peak position: {0}", GetPeakPositionStr(stats))
        LogMessage(LogTypes.Info, str)
        str = [String].Format("    Lower bound: {0}", GetLowStr(stats))
        LogMessage(LogTypes.Info, str)
        str = [String].Format("    Upper bound: {0}", GetHighStr(stats))
        LogMessage(LogTypes.Info, str)
        str = [String].Format("    Number of pixels inside bounds: {0} ({1})", GetNumPixelsStr(stats), GetPixelRatioStr(stats))
        LogMessage(LogTypes.Info, str)

        Dim defects As New SapFlatFieldDefects()

        ' Compute gain coefficient using last acquired image

        m_pFlatField.NumFramesAverage = m_pLocalBuffer.Count
        If m_pFlatField.ComputeGain(m_pLocalBuffer, defects, True) Then
            ' Check for the presence of cluster (adjacent defective pixels)
            If defects.NumClusters <> 0 Then
                str = [String].Format("{0} pixels ({1} %) have been identified as being defective" & vbLf, defects.NumDefects, defects.DefectRatio)
                str += [String].Format("with {0} clusters." & vbLf, defects.NumClusters)
                str += [String].Format(vbLf & "We recommend less than {0}% of defective pixels with no cluster." & vbLf, SapDefFlatFieldDefectRatio)
                str += [String].Format(vbLf & "Do you still want to use this image?" & vbLf)

                If MessageBox.Show(str, "", MessageBoxButtons.YesNo) <> Windows.Forms.DialogResult.Yes Then
                    Exit Sub
                End If
            End If

            ' Log number of defective pixels detected
            str = [String].Format("    Number of defective pixels detected: {0} ({1})", defects.NumDefects, defects.DefectRatio)
            LogMessage(LogTypes.Info, str)

            ' Log number of cluster detected
            str = [String].Format("    Number of clusters detected: {0}", defects.NumClusters)
            LogMessage(LogTypes.Info, str)

            button_Acq_Dark.Enabled = True
            button_Acq_Bright.Enabled = False
            button_OK.Enabled = True
            Dim isOnline As Boolean = (m_pXfer IsNot Nothing AndAlso m_pXfer.Initialized)
            comboBox_Correction_Type.Enabled = Not isOnline AndAlso m_VideoType = SapAcquisition.VideoType.Mono
            comboBox_Video_Type.Enabled = m_pXfer Is Nothing
            textBox_Frame_Avg.Enabled = m_pXfer IsNot Nothing
            textBox_Line_Avg.Enabled = m_CorrectionType = SapFlatField.ScanCorrectionType.Line
            textBox_Vert_Offset.Enabled = m_CorrectionType = SapFlatField.ScanCorrectionType.Line
            textBox_Max_Dev.Enabled = True
            button_Save_And_Upload.Enabled = True
            comboBox_FlatField_Selector.Enabled = True

            'Multi flat-field not implemented in .NET
            'm_CalibrationIndexCtrl.EnableWindow( m_pFlatField->GetNumFlatField() > 1);

            LogMessage(LogTypes.Info, "Calibration with a bright image has been done successfully")

            textBox_Max_Dev.Text = m_pFlatField.DeviationMaxBlack.ToString()
        End If
    End Sub

    ' Snap
    '    Acquire image(s)
    '
    Private Function Snap() As Boolean
        ' Check if the transfer object is available
        If m_pXfer Is Nothing OrElse Not m_pXfer.Initialized Then
            Return False
        End If

        For iFrame As Integer = 0 To m_pLocalBuffer.Count - 1
            ' Acquire one image
            m_pXfer.Snap()

            ' Wait until the acquired image has been transferred into system memory
            Dim abort As New AbortDlg(m_pXfer)
            If abort.ShowDialog() <> Windows.Forms.DialogResult.OK Then
                m_pXfer.Abort()
                Return False
            End If

            'Add a short delay to ensure the transfer callback has time to arrive
            System.Threading.Thread.Sleep(100)

            If m_pLocalBuffer IsNot Nothing Then
                m_pLocalBuffer.Index = iFrame
                m_pLocalBuffer.Copy(m_pBuffer)
            End If
        Next
        Return True
    End Function

    Private Sub LogMessage(ByVal messageType As LogTypes, ByVal str As String)
        Dim message As String = ""

        ' Message header
        Select Case messageType
            Case LogTypes.[Error]
                message = "[Err] "
                Exit Select
            Case LogTypes.Warning
                message = "[Wrn] "
                Exit Select
            Case LogTypes.Info
                message = "[Msg] "
                Exit Select
        End Select

        message += str
        LogMessageBox.BeginUpdate()
        LogMessageBox.Items.Add(message)
        LogMessageBox.EndUpdate()

    End Sub

    Private Function GetAverageStr(ByVal stats As SapFlatFieldStats) As String
        Dim str As String = ""

        If stats.NumComponents > 1 Then
            str += "[ "
            For iComponent As Integer = 0 To stats.NumComponents - 1
                Dim szComponent As String
                szComponent = [String].Format("{0}", stats.Average(iComponent))
                str += szComponent

                If iComponent <> stats.NumComponents - 1 Then
                    str += ", "
                End If
            Next
            str += " ]"
        Else
            str = [String].Format("{0}", stats.Average)
        End If

        Return str
    End Function

    Private Function GetLowStr(ByVal stats As SapFlatFieldStats) As String
        Dim str As String = ""

        If stats.NumComponents > 1 Then
            str += "[ "
            For iComponent As Integer = 0 To stats.NumComponents - 1
                Dim szComponent As String


                szComponent = [String].Format("{0}", stats.Low(iComponent))
                str += szComponent

                If iComponent <> stats.NumComponents - 1 Then
                    str += ", "
                End If
            Next
            str += " ]"
        Else
            str = [String].Format("{0}", stats.Low)
        End If

        Return str
    End Function

    Private Function GetHighStr(ByVal stats As SapFlatFieldStats) As String
        Dim str As String = ""

        If stats.NumComponents > 1 Then
            str += "[ "
            For iComponent As Integer = 0 To stats.NumComponents - 1
                Dim szComponent As String
                szComponent = [String].Format("{0}", stats.High(iComponent))
                str += szComponent

                If iComponent <> stats.NumComponents - 1 Then
                    str += ", "
                End If
            Next
            str += " ]"
        Else
            str = [String].Format("{0}", stats.High)
        End If

        Return str
    End Function

    Private Function GetPixelRatioStr(ByVal stats As SapFlatFieldStats) As String
        Dim str As String = ""

        If stats.NumComponents > 1 Then
            str += "[ "
            For iComponent As Integer = 0 To stats.NumComponents - 1
                Dim szComponent As String


                szComponent = [String].Format("{0}", stats.PeakPosition(iComponent))
                str += szComponent

                If iComponent <> stats.NumComponents - 1 Then
                    str += ", "
                End If
            Next
            str += " ]"
        Else
            str = [String].Format("{0}", stats.PeakPosition)
        End If

        Return str
    End Function

    Private Function GetNumPixelsStr(ByVal stats As SapFlatFieldStats) As String
        Dim str As String = ""

        If stats.NumComponents > 1 Then
            str += "[ "
            For iComponent As Integer = 0 To stats.NumComponents - 1
                Dim szComponent As String


                szComponent = [String].Format("{0}", stats.PixelRatio(iComponent))
                str += szComponent

                If iComponent <> stats.NumComponents - 1 Then
                    str += ", "
                End If
            Next
            str += " ]"
        Else
            str = [String].Format("{0}", stats.PixelRatio)
        End If

        Return str
    End Function

    Private Function GetPeakPositionStr(ByVal stats As SapFlatFieldStats) As String
        Dim str As String = ""

        If stats.NumComponents > 1 Then
            str += "[ "
            For iComponent As Integer = 0 To stats.NumComponents - 1
                Dim szComponent As String


                szComponent = [String].Format("{0}", stats.NumPixels(iComponent))
                str += szComponent

                If iComponent <> stats.NumComponents - 1 Then
                    str += ", "
                End If
            Next
            str += " ]"
        Else
            str = [String].Format("{0}", stats.NumPixels)
        End If

        Return str
    End Function

    Private Sub button_Save_And_Upload_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_Save_And_Upload.Click
        Dim dlg As New SaveFileDialog()
        dlg.Title = "Save Flat Field Correction"
        dlg.FileName = DEFAULT_FFC_FILENAME
        dlg.Filter = STANDARD_FILTER

        If (dlg.ShowDialog() = DialogResult.OK) Then
            ' Save flat field offset correction file
            m_pFlatField.Save(dlg.FileName)
            LogMessage(LogTypes.Info, "File saved successfully.")
            If (m_UserFlatFieldCount > 1 And Not m_isGenie) Then
                'CWaitCursor waitCursor
                LogMessage(LogTypes.Info, "Uploading file to device... Please wait.")
                Dim index As Integer = flatFieldIndexes(comboBox_FlatField_Selector.SelectedIndex)
                Dim timeout As Integer = SapManager.CommandTimeout
                SapManager.CommandTimeout = 5 * 60 * 1000 'timeout 5 minutes
                If (m_pFlatField.AcqDevice.WriteFile(dlg.FileName, index)) Then
                    LogMessage(LogTypes.Info, "File upload completed successfully.")
                Else
                    LogMessage(LogTypes.Error, "File upload failed.")
                End If
                'restore original timeout
                SapManager.CommandTimeout = timeout
            End If
        End If
    End Sub

   Private Sub button_load_cluster_Click(sender As Object, e As EventArgs) Handles button_load_cluster.Click
      Dim dlg As New OpenFileDialog()
      dlg.Title = "Load defective pixels cluster map"
      dlg.FileName = DEFAULT_FFC_CLUSTER_MAP_FILENAME
      dlg.Filter = FFC_CLUSTER_MAP_FILTER

      If (dlg.ShowDialog() = DialogResult.OK) Then
         If (m_pFlatField.SetClusterMap(dlg.FileName)) Then
            label_load_cluster.Text = [String].Format("Cluster map loaded with {0} defective pixels", m_pFlatField.ClusterMapPixelCount)
         Else
            label_load_cluster.Text = "Invalid cluster map"
         End If
      End If
   End Sub
End Class




