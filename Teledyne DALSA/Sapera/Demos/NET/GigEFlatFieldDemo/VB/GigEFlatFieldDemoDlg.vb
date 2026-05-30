Partial Public Class GigEFlatFieldDemoDlg
	Inherits Form

	Private Const g_ValueChangeEventName As String = "Feature Value Changed"
	Private Const DEFAULT_FFC_FILENAME As String = "FFC.tif"
	Private Const STANDARD_FILTER As String = "TIFF Files (*.tif)|*.tif||"

	Private g_CalibEnableFeatureName As String = ""
	Private m_AcqDevice As SapAcqDevice
	Private m_Buffers As SapBuffer
	Private m_Xfer As SapAcqDeviceToBuf
	Private m_View As SapView
	Private m_FlatField As SapFlatField
	Private m_Pro As SapMyProcessing
	Private m_ServerLocation As SapLocation
	Private m_ConfigFileName As String, m_FlatField_ExecuteTime As String
	Private m_CalibEnableAvailable As Boolean, m_CalibEnable As Boolean, m_ValueChangeEventAvailable As Boolean, m_isGenie As Boolean
	Private m_bUseROI As Boolean

	'System menu
	Private m_SystemMenu As SystemMenu
	'index for "about this.." item im system menu
	Private Const m_AboutID As Integer = &H100

	' Delegate to display number of frame acquired 
	' Delegate is needed because .NEt framework does not support  cross thread control modification
	Private Delegate Sub DisplayFrameAcquired(ByVal number As Integer, ByVal trash As Boolean)


	Private Shared Sub xfer_XferNotify(ByVal sender As Object, ByVal argsNotify As SapXferNotifyEventArgs)
		Dim GigeDlg As GigEFlatFieldDemoDlg = TryCast(argsNotify.Context, GigEFlatFieldDemoDlg)

		' Set the ROI if needed
		If GigeDlg.m_bUseROI Then
			Dim roi = GigeDlg.m_ImageBox.Tracker
			GigeDlg.m_FlatField.SetRegionOfInterest(roi.Left, roi.Top, roi.Width, roi.Height)
		End If

		' If grabbing in trash buffer, do not display the image, update the
		' appropriate number of frames on the status bar instead
		If argsNotify.Trash Then
			GigeDlg.Invoke(New DisplayFrameAcquired(AddressOf GigeDlg.ShowFrameNumber), argsNotify.EventCount, True)
		Else
			' Refresh view
			GigeDlg.Invoke(New DisplayFrameAcquired(AddressOf GigeDlg.ShowFrameNumber), argsNotify.EventCount, False)
			GigeDlg.m_Pro.ExecuteNext()
		End If
	End Sub

	'
	' This function is called each time a buffer has been processed by the processing object
	'
	Private Shared Sub ProCallback(ByVal sender As Object, ByVal pInfo As SapProcessingDoneEventArgs)
		Dim GigeDlg As GigEFlatFieldDemoDlg = TryCast(pInfo.Context, GigEFlatFieldDemoDlg)

		' Check if flat field correction was enabled and if it's been done by software
		If GigeDlg.m_FlatField.Enabled AndAlso GigeDlg.m_FlatField.SoftwareCorrection Then
			' Show current buffer index and execution time in milliseconds
			GigeDlg.m_FlatField_ExecuteTime = [String].Format("Flat field correction= {0:0.00} ms", GigeDlg.m_Pro.Time)
		Else
			GigeDlg.m_FlatField_ExecuteTime = ""
		End If

		' Refresh view
		GigeDlg.m_View.Show()
	End Sub

	Private Shared Sub ViewCallback(ByVal sender As Object, ByVal pInfo As SapDisplayDoneEventArgs)
		Dim GigeDlg As GigEFlatFieldDemoDlg = TryCast(pInfo.Context, GigEFlatFieldDemoDlg)
		If GigeDlg.m_bUseROI AndAlso Not GigeDlg.m_ImageBox.IsTrackerEmpty Then
			GigeDlg.m_ImageBox.DisplayTracker()
		End If
	End Sub

	'
	' This function is called each time an acquisition device specific event happens,
	' for example, when the value of a feature changes
	'
	Private Shared Sub AcqDeviceCallback(ByVal sender As Object, ByVal pInfo As SapAcqDeviceNotifyEventArgs)
		' Uncomment to display events received from the camera
		'SapAcqDevice acq = pInfo.Context as SapAcqDevice;

		'int eventIndex, featureIndex;
		'String [] allEvent;
		'String [] allFeature;

		'eventIndex = pInfo.EventIndex;
		'allEvent = acq.EventNames;
		'String message = "Acquisition device event: ";
		'message += allEvent[eventIndex];
		'message += "\n";

		'featureIndex = pInfo.FeatureIndex;
		'if (allEvent[eventIndex] == g_ValueChangeEventName)
		'{
		'   allFeature = acq.FeatureNames;
		'   message += "Feature name: ";
		'   message += allFeature[featureIndex];
		'   message += "\n";
		'}

		'MessageBox.Show(message);
	End Sub

	Private Sub SystemEvents_SessionEnded(ByVal sender As Object, ByVal args As SessionEndedEventArgs)
		' The FormClosed event is not invoked when logging off or shutting down,
		' so we need to clean up here too.
		DestroyObjects(True, True)
		DisposeObjects()
	End Sub
	Public Sub New()
		m_AcqDevice = Nothing
		m_Buffers = Nothing
		m_Xfer = Nothing
		m_View = Nothing
		m_FlatField = Nothing
		m_Pro = Nothing

		m_CalibEnableAvailable = False
		m_CalibEnable = False
		m_ValueChangeEventAvailable = False
		m_FlatField_ExecuteTime = ""
		m_bUseROI = False

		Dim acConfigDlg As New AcqConfigDlg(Nothing, "", AcqConfigDlg.ServerCategory.ServerAcqDevice)
		If acConfigDlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
			InitializeComponent()

			' Note:
			'  The code to initialize m_ImageBox was originally in the InitializeComponent function
			'  called above. However, it has been moved to the dialog constructor as a workaround
			'  to a Visual Studio Designer error when loading the DALSA.SaperaLT.SapClassBasic
			'  assembly under 64-bit Windows.
			'  As a consequence, it is not possible to adjust the m_ImageBox properties
			'  automatically using the Designer anymore, this has to be done manually.
			' 
			Me.m_ImageBox = New DALSA.SaperaLT.Demos.NET.VB.GigEFlatFieldDemo.ImageBox
			Me.m_ImageBox.Location = New System.Drawing.Point(261, 73)
			Me.m_ImageBox.Name = "m_ImageBox"
			Me.m_ImageBox.PixelValueDisplay = Me.PixelDataValue
			Me.m_ImageBox.Size = New System.Drawing.Size(386, 357)
			Me.m_ImageBox.SliderEnable = True
			Me.m_ImageBox.SliderMaximum = 10
			Me.m_ImageBox.SliderMinimum = 0
			Me.m_ImageBox.SliderValue = 0
			Me.m_ImageBox.SliderVisible = False
			Me.m_ImageBox.TabIndex = 37
			Me.m_ImageBox.TrackerEnable = False
			Me.m_ImageBox.View = Nothing
			Me.Controls.Add(Me.m_ImageBox)

			If Not CreateNewObjects(acConfigDlg, False) Then
				Me.Close()
			ElseIf Not m_isGenie and Not m_CalibEnableAvailable Then
				MessageBox.Show("Hardware based flat-field correction is not supported on this camera.")
				Me.Close()
			End If
		Else
			MessageBox.Show("No cameras found or selected")
			Me.Close()
		End If

		CheckBox_Use_ROI.Checked = m_bUseROI

	End Sub

	Private Sub ShowFrameNumber(ByVal number As Integer, ByVal trash As Boolean)
		Dim str As String
		If trash Then
			If m_FlatField_ExecuteTime = "" Then
				str = [String].Format("Frames acquired in trash buffer: {0}", number * m_Buffers.Count)
				Me.StatusLabelInfoTrash.Text = str
			End If
		Else

			If m_FlatField_ExecuteTime <> "" Then
				Me.StatusLabelInfo.Text = m_FlatField_ExecuteTime
			Else
				str = [String].Format("Frames acquired :{0}", number * m_Buffers.Count)
				Me.StatusLabelInfo.Text = str
			End If
		End If
	End Sub

	'*****************************************************************************************
	'
	'					Create and Destroy Object
	'
	'*****************************************************************************************

	' Create new objects with acquisition information
	Public Function CreateNewObjects(ByVal acConfigDlg As AcqConfigDlg, ByVal Restore As Boolean) As Boolean
		If Not Restore Then
			m_ServerLocation = acConfigDlg.ServerLocation
			m_ConfigFileName = acConfigDlg.ConfigFile
		End If
		m_AcqDevice = New SapAcqDevice(m_ServerLocation, m_ConfigFileName)
		If SapBuffer.IsBufferTypeSupported(m_ServerLocation, SapBuffer.MemoryType.ScatterGather) Then
			m_Buffers = New SapBufferWithTrash(2, m_AcqDevice, SapBuffer.MemoryType.ScatterGather)
		Else
			m_Buffers = New SapBufferWithTrash(2, m_AcqDevice, SapBuffer.MemoryType.ScatterGatherPhysical)
		End If
		m_Xfer = New SapAcqDeviceToBuf(m_AcqDevice, m_Buffers)

		m_View = New SapView(m_Buffers)
		'event for view  
		AddHandler m_View.DisplayDone, AddressOf ViewCallback
		m_View.DisplayDoneContext = Me
		m_View.DisplayDoneEnable = True

		m_FlatField = New SapFlatField(m_AcqDevice)
		m_Pro = New SapMyProcessing(m_Buffers, m_FlatField, New SapProcessingDoneHandler(AddressOf ProCallback), Me)
		m_ImageBox.View = m_View

		m_Xfer.Pairs(0).EventType = SapXferPair.XferEventType.EndOfFrame
		AddHandler m_Xfer.XferNotify, AddressOf xfer_XferNotify
		m_Xfer.XferNotifyContext = Me
		StatusLabelInfo.Text = "Online... Waiting grabbed images"

		If Not CreateObjects(True, True) Then
			DisposeObjects()
			Return False
		End If

		' Resize ImagBox to take into account the size of created sapview
		m_ImageBox.OnSize()
		UpdateControls()
		Return True
	End Function


	' Create new objects with acquisition information
	Public Function CreateNewObjects(ByVal dlg As BufDlg) As Boolean
		m_AcqDevice = New SapAcqDevice(m_ServerLocation, m_ConfigFileName)
		If dlg.Count > 1 Then
			m_Buffers = New SapBufferWithTrash(dlg.Count, m_AcqDevice, dlg.Type)
		Else
			m_Buffers = New SapBuffer(1, m_AcqDevice, dlg.Type)
		End If
		m_Xfer = New SapAcqDeviceToBuf(m_AcqDevice, m_Buffers)

		m_View = New SapView(m_Buffers)
		'event for view  
		AddHandler m_View.DisplayDone, AddressOf ViewCallback
		m_View.DisplayDoneContext = Me
		m_View.DisplayDoneEnable = True

		m_FlatField = New SapFlatField(m_AcqDevice)
		m_Pro = New SapMyProcessing(m_Buffers, m_FlatField, New SapProcessingDoneHandler(AddressOf ProCallback), Me)
		m_ImageBox.View = m_View

		m_Xfer.Pairs(0).EventType = SapXferPair.XferEventType.EndOfFrame
		AddHandler m_Xfer.XferNotify, AddressOf xfer_XferNotify
		m_Xfer.XferNotifyContext = Me
		StatusLabelInfo.Text = "Online... Waiting grabbed images"

		If Not CreateObjects(True, True) Then
			DisposeObjects()
			Return False
		End If

		' Resize ImagBox to take into account the size of created sapview
		m_ImageBox.OnSize()
		UpdateControls()
		Return True
	End Function

	' Call Create Object 
	Private Function CreateObjects(ByVal createAcq As Boolean, ByVal createFlatField As Boolean) As Boolean
		If createAcq Then
			' Create acquisition object
			If m_AcqDevice IsNot Nothing AndAlso Not m_AcqDevice.Initialized Then
				If Not m_AcqDevice.Create() Then
					DestroyObjects(True, True)
					Return False
				End If
			End If
			'GigE
			m_CalibEnableAvailable = m_AcqDevice.IsFeatureAvailable("flatfieldCorrectionMode")
			If (m_CalibEnableAvailable) Then
				m_isGenie = False
				g_CalibEnableFeatureName = "flatfieldCorrectionMode"
			Else
				'Genie
				m_CalibEnableAvailable = m_AcqDevice.IsFeatureAvailable("FlatFieldCalibrationEnable")
				If (m_CalibEnableAvailable) Then
					m_isGenie = True
					g_CalibEnableFeatureName = "FlatFieldCalibrationEnable"
				End If
			End If


			m_ValueChangeEventAvailable = m_AcqDevice.IsEventAvailable(g_ValueChangeEventName)

			If m_ValueChangeEventAvailable Then
				m_AcqDevice.EnableEvent(g_ValueChangeEventName)
				AddHandler m_AcqDevice.AcqDeviceNotify, AddressOf AcqDeviceCallback
				m_AcqDevice.AcqDeviceNotifyContext = m_AcqDevice

			End If
		End If

		' Create buffer object
		If m_Buffers IsNot Nothing AndAlso Not m_Buffers.Initialized Then
			If m_Buffers.Create() = False Then
				DestroyObjects(True, True)
				Return False
			End If
			m_Buffers.Clear()
		End If
		' Create view object
		If m_View IsNot Nothing AndAlso Not m_View.Initialized Then
			If m_View.Create() = False Then
				DestroyObjects(True, True)
				Return False
			End If
		End If
		' Create Xfer object
		If m_Xfer IsNot Nothing AndAlso Not m_Xfer.Initialized Then
			If m_Xfer.Create() = False Then
				DestroyObjects(True, True)
				Return False
			End If
		End If

		' Create flat field object
		If createFlatField Then
			If m_FlatField IsNot Nothing AndAlso Not m_FlatField.Initialized Then
				If Not m_FlatField.Create() Then
					DestroyObjects(True, True)
					Return False
				End If
			End If
		End If

		If m_Pro IsNot Nothing AndAlso Not m_Pro.Initialized Then
			If Not m_Pro.Create() Then
				DestroyObjects(True, True)
				Return False
			End If
			m_Pro.AutoEmpty = True
			If m_Xfer IsNot Nothing Then
				m_Xfer.AutoEmpty = False
			End If
		End If

		Return True
	End Function

	Private Sub DestroyObjects(ByVal destroyAcq As Boolean, ByVal destroyFlatField As Boolean)
		' Destroy processing object
		If m_Pro IsNot Nothing AndAlso m_Pro.Initialized Then
			m_Pro.Destroy()
		End If

		' Destroy flat field object
		If destroyFlatField Then
			If m_FlatField IsNot Nothing AndAlso m_FlatField.Initialized Then
				m_FlatField.Destroy()
			End If
			' Disable flat field correction
			checkBox_FlatField.Checked = False
		End If

		If m_Xfer IsNot Nothing AndAlso m_Xfer.Initialized Then
			m_Xfer.Destroy()
		End If
		If m_View IsNot Nothing AndAlso m_View.Initialized Then
			m_View.Destroy()
		End If
		If m_Buffers IsNot Nothing AndAlso m_Buffers.Initialized Then
			m_Buffers.Destroy()
		End If

		If destroyAcq Then
			If m_AcqDevice IsNot Nothing AndAlso m_AcqDevice.Initialized Then
				' Unregister acquisition callback
				If m_ValueChangeEventAvailable Then
					m_AcqDevice.DisableEvent(g_ValueChangeEventName)
					RemoveHandler m_AcqDevice.AcqDeviceNotify, AddressOf AcqDeviceCallback
					m_AcqDevice.AcqDeviceNotifyContext = Nothing
				End If

				' Disable flat-field calibration mode on the camera
				If m_CalibEnableAvailable AndAlso m_CalibEnable Then
					If (m_isGenie) Then
						m_AcqDevice.SetFeatureValue(g_CalibEnableFeatureName, False)
					Else
						m_AcqDevice.SetFeatureValue(g_CalibEnableFeatureName, "Off")
					End If
					m_CalibEnable = False
				End If

				' Destroy acquisition object
				m_AcqDevice.Destroy()
			End If
		End If
	End Sub

	Private Sub DisposeObjects()
		If m_Pro IsNot Nothing Then
			m_Pro.Dispose()
			m_Pro = Nothing
		End If
		If m_FlatField IsNot Nothing Then
			m_FlatField.Dispose()
			m_FlatField = Nothing
		End If
		If m_Xfer IsNot Nothing Then
			m_Xfer.Dispose()
			m_Xfer = Nothing
		End If
		If m_View IsNot Nothing Then
			m_View.Dispose()
			m_View = Nothing
			m_ImageBox.View = Nothing
		End If
		If m_Buffers IsNot Nothing Then
			m_Buffers.Dispose()
			m_Buffers = Nothing
		End If
		If m_AcqDevice IsNot Nothing Then
			m_AcqDevice.Dispose()
			m_AcqDevice = Nothing
		End If
	End Sub

	'**********************************************************************************
	'
	'				Window related functions
	'
	'**********************************************************************************


	Protected Overloads Overrides Sub OnResize(ByVal argsPaint As EventArgs)
		If m_ImageBox IsNot Nothing Then
			m_ImageBox.OnSize()
		End If
		MyBase.OnResize(argsPaint)
	End Sub

	Private Sub GigEFlatFieldDemoDlg_FormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs) Handles MyBase.FormClosed
		DestroyObjects(True, True)
		DisposeObjects()
	End Sub

	'**************************************************************************************
	' Updates the menu items enabling/disabling the proper items depending on the state
	'  of the application
	'**************************************************************************************
	Private Sub UpdateControls()
		Dim acqNoGrab As Boolean = m_Xfer IsNot Nothing AndAlso m_Xfer.Initialized AndAlso Not m_Xfer.Grabbing
		Dim acqGrab As Boolean = m_Xfer IsNot Nothing AndAlso m_Xfer.Initialized AndAlso m_Xfer.Grabbing
		Dim noGrab As Boolean = m_Xfer Is Nothing OrElse Not m_Xfer.Grabbing

		Dim isFlatFieldEnabled As Boolean = m_FlatField IsNot Nothing AndAlso m_FlatField.Initialized AndAlso m_FlatField.Enabled
		Dim isFlatFieldAvailable As Boolean = m_AcqDevice IsNot Nothing AndAlso m_AcqDevice.Initialized AndAlso m_AcqDevice.FlatFieldAvailable
		Dim isFlatFieldSoftware As Boolean = m_FlatField IsNot Nothing AndAlso m_FlatField.Initialized AndAlso m_FlatField.SoftwareCorrection
		Dim isFlatFieldPixelReplacement As Boolean = m_FlatField IsNot Nothing AndAlso m_FlatField.Initialized AndAlso m_FlatField.PixelReplacement

		' Acquisition Control
		button_Grab.Enabled = acqNoGrab
		button_Freeze.Enabled = acqGrab
		button_Snap.Enabled = acqNoGrab

		' Acquisition Options
		button_LoadConfig.Enabled = acqNoGrab

		' Flat field correction
		checkBox_FlatField.Enabled = noGrab
		button_Calibrate.Enabled = noGrab AndAlso Not isFlatFieldEnabled
		button_Load_FF.Enabled = noGrab AndAlso Not isFlatFieldEnabled
		button_Save_FF.Enabled = noGrab

		checkBox_FlatField.Checked = isFlatFieldEnabled
		checkBox_Hardware_Correction.Checked = isFlatFieldAvailable AndAlso Not isFlatFieldSoftware
		checkBox_Hardware_Correction.Enabled = isFlatFieldAvailable AndAlso isFlatFieldEnabled AndAlso noGrab

		checkBox_Pixels_Replacement.Checked = isFlatFieldPixelReplacement
		checkBox_Pixels_Replacement.Enabled = isFlatFieldSoftware AndAlso isFlatFieldEnabled AndAlso noGrab

		' File Options
		button_New.Enabled = noGrab
		button_Load.Enabled = noGrab
		button_Save.Enabled = noGrab

		' General Options
		button_Buffers.Enabled = noGrab
		button_View.Enabled = noGrab

	End Sub

	' Set the flat-field calibration mode of the camera
	Private Function SetCalibEnable(ByVal newCalibEnable As Boolean) As Boolean
        Dim calibFeature As SapFeature = New SapFeature(m_AcqDevice.Location)
        calibFeature.Create()
        m_AcqDevice.GetFeatureInfo(g_CalibEnableFeatureName, calibFeature)
        Dim enumCount As Integer = calibFeature.EnumCount
        Dim enumText As String() = calibFeature.EnumText
        Dim calibModeValueAvailable As Boolean = False

        Dim enumIndex As Integer
        For enumIndex = 0 To enumCount - 1
            Dim enumEnabled As Boolean = calibFeature.EnumEnabled(enumIndex)
            If (enumText(enumIndex) = "Calibration" And enumEnabled) Then
                calibModeValueAvailable = True
            End If
        Next

        calibFeature.Destroy()
        calibFeature.Dispose()

        If (newCalibEnable <> m_CalibEnable) AndAlso m_CalibEnableAvailable Then
            DestroyObjects(False, False)
            Dim success As Boolean = False
            If (m_isGenie) Then
                success = m_AcqDevice.SetFeatureValue(g_CalibEnableFeatureName, newCalibEnable)
            Else
                If (newCalibEnable And calibModeValueAvailable) Then
                    success = m_AcqDevice.SetFeatureValue(g_CalibEnableFeatureName, "Calibration")
                Else
                    success = m_AcqDevice.SetFeatureValue(g_CalibEnableFeatureName, "Off")
                End If
            End If

            If (success) Then
                If Not CreateObjects(False, False) Then
                    If (m_isGenie) Then
                        success = m_AcqDevice.SetFeatureValue(g_CalibEnableFeatureName, m_CalibEnable)
                    Else
                        If (m_CalibEnable And calibModeValueAvailable) Then
                            success = m_AcqDevice.SetFeatureValue(g_CalibEnableFeatureName, "Calibration")
                        Else
                            success = m_AcqDevice.SetFeatureValue(g_CalibEnableFeatureName, "Off")
                        End If
                    End If
                    CreateObjects(False, False)
                    Return False
                End If
                m_CalibEnable = newCalibEnable
            End If
            If m_ImageBox IsNot Nothing Then
                m_ImageBox.OnSize()
            End If
        End If
        Return True
    End Function

	Private Function CheckPixelFormat(ByVal mode As String) As Boolean
		Dim format As SapFormat = m_Buffers.Format

		If format <> SapFormat.Mono8 AndAlso format <> SapFormat.Mono16 Then
			Dim message As String
			If mode = "warning" Then
				message = "Pixel Format will be automatically changed to Raw Bayer for flat field calibration."
			Else
				message = [String].Format("Pixel format must be 8-bit or 16-bit monochrome for {0}" & vbLf, mode)
				message += "(for Genie color, this corresponds to Bayer Raw8 in CamExpert)"
			End If
			MessageBox.Show(message)
			Return False
		End If

		Return True
	End Function


	'*****************************************************************************************
	'
	'               Acquisition Control
	'
	'*****************************************************************************************

	Private Sub button_Snap_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Snap.Click
		Dim abort As New AbortDlg(m_Xfer)

		If m_Xfer.Snap() Then
			If abort.ShowDialog() <> Windows.Forms.DialogResult.OK Then
				m_Xfer.Abort()
			End If
			UpdateControls()
		End If
	End Sub

	Private Sub button_Grab_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Grab.Click
		Me.StatusLabelInfo.Text = ""
		Me.StatusLabelInfoTrash.Text = ""
		If m_Xfer.Grab() Then
			UpdateControls()
		End If
	End Sub

	Private Sub button_Freeze_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Freeze.Click
		Dim abort As New AbortDlg(m_Xfer)

		If m_Xfer.Freeze() Then
			If abort.ShowDialog() <> Windows.Forms.DialogResult.OK Then
				m_Xfer.Abort()
			End If
			UpdateControls()
		End If
	End Sub


	'*****************************************************************************************
	'
	'               Flat field Options
	'
	'*****************************************************************************************

	Private Sub button_Calibrate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Calibrate.Click
        If Not m_isGenie And m_AcqDevice.FlatFieldAvailable Then
            Dim width, height, sensorWidth, sensorHeight As ULong
            Dim isInvalidSize as Boolean
            If Not m_AcqDevice.GetFeatureValue("Width", width) Then
                Exit Sub
            End If
            If Not m_AcqDevice.GetFeatureValue("Height", height) Then
                Exit Sub
            End If
            If Not m_AcqDevice.GetFeatureValue("SensorWidth", sensorWidth) Then
                Exit Sub
            End If
            If Not m_AcqDevice.GetFeatureValue("SensorHeight", sensorHeight) Then
                Exit Sub
            End If
            if (sensorHeight = 1) Then
               isInvalidSize = (width <> sensorWidth)
            Else
               isInvalidSize = (width <> sensorWidth Or height <> sensorHeight)
            End If
            If (isInvalidSize) Then
                MessageBox.Show("Flat-field calibration can only be performed on the full sensor size.\nMake sure that image cropping and binning are disabled.")
                Exit Sub
            End If
        End If

        CheckPixelFormat("warning")

        ' Flat-field calibration mode must be enabled on the camera
        If SetCalibEnable(True) Then
            Dim dlg As New FlatFieldDlg(m_FlatField, m_Xfer, m_Buffers)
            dlg.ShowDialog()
            SetCalibEnable(False)
            UpdateControls()
        End If
    End Sub

    Private Sub checkBox_FlatField_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles checkBox_FlatField.CheckedChanged
        Cursor = Cursors.WaitCursor

        If m_FlatField IsNot Nothing AndAlso m_FlatField.Initialized Then
            ' To enable/disable flat field correction, the transfer object must first be disconnected from the hardware
            If m_Xfer IsNot Nothing AndAlso m_Xfer.Initialized Then
                m_Xfer.Destroy()
            End If

            Dim success As Boolean = True

            ' Check for invalid pixel format when using software correction
            If checkBox_FlatField.Checked AndAlso Not checkBox_Hardware_Correction.Checked Then
                success = CheckPixelFormat("software correction")
            End If

            If success Then
                m_FlatField.Enable(checkBox_FlatField.Checked, checkBox_Hardware_Correction.Checked)
            End If


            If m_Xfer IsNot Nothing AndAlso Not m_Xfer.Initialized Then
                ' Recreate the transfer object to reconnect it to the hardware
                m_Xfer.Create()
                m_Xfer.Init(True)
                m_Pro.Init()
            End If
            If checkBox_FlatField.Checked Then
                StatusLabelInfoTrash.Text = ""
            End If
        End If
        UpdateControls()
        Cursor = Cursors.[Default]
    End Sub

    Private Sub button_Load_FF_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Load_FF.Click

        Dim dlg As New OpenFileDialog()
        dlg.Title = "Open Flat Field Correction"
        dlg.FileName = DEFAULT_FFC_FILENAME
        dlg.Filter = STANDARD_FILTER

        If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Dim PathName As String = dlg.FileName
            ' Load flat field correction file
            If Not m_FlatField.Load(PathName) Then
                Exit Sub
            End If
        End If

        UpdateControls()
    End Sub

    Private Sub button_Save_FF_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Save_FF.Click
        Dim dlg As New SaveFileDialog()
        dlg.Title = "Save Flat Field Correction As"
        dlg.FileName = DEFAULT_FFC_FILENAME
        dlg.Filter = STANDARD_FILTER


        If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Dim PathName As String = dlg.FileName
            ' Save flat field correction file
            m_FlatField.Save(PathName)
        End If

        UpdateControls()
    End Sub

    Private Sub checkBox_Hardware_Correction_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles checkBox_Hardware_Correction.CheckedChanged
        Cursor = Cursors.WaitCursor

        If m_FlatField IsNot Nothing AndAlso m_FlatField.Initialized Then
            ' To enable/disable flat field correction, the transfer object must first be disconnected from the hardware
            If m_Xfer IsNot Nothing AndAlso m_Xfer.Initialized Then
                m_Xfer.Destroy()
            End If

            Dim success As Boolean = True

            ' Check for invalid pixel format when using software correction
            If checkBox_FlatField.Checked AndAlso Not checkBox_Hardware_Correction.Checked Then
                success = CheckPixelFormat("software correction")
            End If

            ' Enable/disable flat field correction
            If success Then
                m_FlatField.Enable(checkBox_FlatField.Checked, checkBox_Hardware_Correction.Checked)
            End If

            If m_Xfer IsNot Nothing AndAlso Not m_Xfer.Initialized Then
                ' Recreate the transfer object to reconnect it to the hardware
                m_Xfer.Create()
                m_Xfer.Init(True)
                m_Pro.Init()
            End If
        End If
        UpdateControls()
        Cursor = Cursors.[Default]
    End Sub

    Private Sub CheckBox_Use_ROI_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox_Use_ROI.CheckedChanged
        m_bUseROI = CheckBox_Use_ROI.Checked
        m_ImageBox.TrackerEnable = m_bUseROI
        If m_bUseROI Then
            Dim roi = m_ImageBox.Tracker
            m_FlatField.SetRegionOfInterest(roi.Left, roi.Top, roi.Width, roi.Height)
        Else
            m_FlatField.ResetRegionOfInterest()
        End If
    End Sub

    Private Sub checkBox_Pixels_Replacement_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles checkBox_Pixels_Replacement.CheckedChanged
        ' Enable/disable pixel replacement flat field correction
        m_FlatField.PixelReplacement = checkBox_Pixels_Replacement.Checked
        UpdateControls()
    End Sub

    Private Sub button_Exit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Exit.Click
        Application.[Exit]()
    End Sub

    Private Sub button_Buffers_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Buffers.Click
        ' Set new buffer parameters
        Dim dlg As New BufDlg(m_Buffers, m_View.Display, True)
        If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            DestroyObjects(True, True)
            DisposeObjects()

            'Update objects with new buffer
            If Not CreateNewObjects(dlg) Then
                MessageBox.Show("New objects creation has failed. Restoring original object ")
                ' Recreate original objects
                If Not CreateNewObjects(Nothing, True) Then
                    MessageBox.Show("Original object creation has failed. Closing application ")
                    Application.[Exit]()
                End If
            End If
        End If
        m_ImageBox.Refresh()
    End Sub

    Private Sub button_View_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_View.Click
        Dim viewDialog As New ViewDlg(m_View, m_ImageBox.ViewRectangle)

        If viewDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
            m_ImageBox.OnSize()
        End If
        m_ImageBox.Refresh()

    End Sub

    Private Sub button_LoadConfig_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_LoadConfig.Click
        ' Set new acquisition parameters
        Dim acConfigDlg As New AcqConfigDlg(Nothing, "", AcqConfigDlg.ServerCategory.ServerAcqDevice)
        If acConfigDlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            DestroyObjects(True, True)
            DisposeObjects()

            ' Update objects with new acquisition
            If Not CreateNewObjects(acConfigDlg, False) Then
                MessageBox.Show("New objects creation has failed. Restoring original object ")
                ' Recreate original objects
                If Not CreateNewObjects(Nothing, True) Then
                    MessageBox.Show("Original object creation has failed. Closing application ")
                    Application.[Exit]()
                End If
            End If
        Else
            MessageBox.Show("No Modification in Acquisition")
        End If
        m_ImageBox.Refresh()

    End Sub

    Private Sub button_New_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_New.Click
        m_Buffers.Clear()
        m_ImageBox.Refresh()
    End Sub

    Private Sub button_Load_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Load.Click
        Dim newDialogLoad As New LoadSaveDlg(m_Buffers, True, False)
        ' Show the dialog and process the result
        newDialogLoad.ShowDialog()
        newDialogLoad.Dispose()
        m_ImageBox.Refresh()
    End Sub

    Private Sub button_Save_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Save.Click
        Dim newDialogSave As New LoadSaveDlg(m_Buffers, False, False)
        ' Show the dialog and process the result
        newDialogSave.ShowDialog()
        newDialogSave.Dispose()
        m_ImageBox.Refresh()
    End Sub

    '*****************************************************************************************
    '
    '					System menu
    '
    '*****************************************************************************************

    Private Sub GigEFlatFieldDemoDlg_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ' Add about this to the system menu
        m_SystemMenu = SystemMenu.FromForm(Me)
        If m_SystemMenu IsNot Nothing Then
            m_SystemMenu.AppendSeparator()
            m_SystemMenu.AppendMenu(m_AboutID, "About this...")
        End If

        ' Register event handler which is invoked when logging off or shutting down
        AddHandler SystemEvents.SessionEnded, AddressOf SystemEvents_SessionEnded
    End Sub


    'Catch the WM_SYSCOMMAND message and process it.
    Protected Overloads Overrides Sub WndProc(ByRef msg As Message)
        If msg.Msg = CInt(WindowMessages.wmSysCommand) Then
            If msg.WParam.ToInt32() = m_AboutID Then
                Dim dlg As New AboutBox()
                dlg.ShowDialog()
                Invalidate()
                Update()
            End If
        End If
        ' Call base class function
        MyBase.WndProc(msg)
    End Sub

End Class



