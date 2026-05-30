Partial Public Class FlatFieldDemoDlg
	Inherits Form

	Private Const DEFAULT_FFC_FILENAME As String = "FFC.tif"
	Private Const STANDARD_FILTER As String = "TIFF Files (*.tif)|*.tif||"

	Private m_Acquisition As SapAcquisition
	Private m_Buffers As SapBuffer
	Private m_Xfer As SapTransfer
	Private m_View As SapView
	Private m_FlatField As SapFlatField
	Private m_Pro As SapMyProcessing
	Private m_ServerLocation As SapLocation
	Private m_FlatField_ExecuteTime As String
	Private m_ConfigFileName As String

	Private m_IsSignalDetected As Boolean
	Private m_online As Boolean
	Private m_bUseROI As Boolean

	'System menu
	Private m_SystemMenu As SystemMenu
	'index for "about this.." item im system menu
	Private Const m_AboutID As Integer = &H100

	' Delegate to display number of frame acquired 
	' Delegate is needed because .NEt framework does not support  cross thread control modification
	Private Delegate Sub DisplayFrameAcquired(ByVal number As Integer, ByVal trash As Boolean)

	Private Shared Sub xfer_XferNotify(ByVal sender As Object, ByVal argsNotify As SapXferNotifyEventArgs)
		Dim FlatDlg As FlatFieldDemoDlg = TryCast(argsNotify.Context, FlatFieldDemoDlg)

		' Set the ROI if needed
		If FlatDlg.m_bUseROI Then
			Dim roi = FlatDlg.m_ImageBox.Tracker
			FlatDlg.m_FlatField.SetRegionOfInterest(roi.Left, roi.Top, roi.Width, roi.Height)
		End If


		' If grabbing in trash buffer, do not display the image, update the
		' appropriate number of frames on the status bar instead
		If argsNotify.Trash Then
			FlatDlg.Invoke(New DisplayFrameAcquired(AddressOf FlatDlg.ShowFrameNumber), argsNotify.EventCount, True)
		Else
			' Refresh view
			FlatDlg.Invoke(New DisplayFrameAcquired(AddressOf FlatDlg.ShowFrameNumber), argsNotify.EventCount, False)
			FlatDlg.m_Pro.ExecuteNext()
		End If
	End Sub

	'
	' This function is called each time a buffer has been processed by the processing object
	'
	Private Shared Sub ProCallback(ByVal sender As Object, ByVal pInfo As SapProcessingDoneEventArgs)
		Dim FlatDlg As FlatFieldDemoDlg = TryCast(pInfo.Context, FlatFieldDemoDlg)

		' Check if flat field correction was enabled and if it's been done by software
		If FlatDlg.m_FlatField.Enabled AndAlso FlatDlg.m_FlatField.SoftwareCorrection Then
			' Show current buffer index and execution time in milliseconds
			FlatDlg.m_FlatField_ExecuteTime = [String].Format("Flat field correction= {0:0.00} ms", FlatDlg.m_Pro.Time)
		Else
			FlatDlg.m_FlatField_ExecuteTime = ""
		End If

		' Refresh view
		FlatDlg.m_View.Show()
	End Sub

	Private Shared Sub ViewCallback(ByVal sender As Object, ByVal pInfo As SapDisplayDoneEventArgs)
		Dim FlatDlg As FlatFieldDemoDlg = TryCast(pInfo.Context, FlatFieldDemoDlg)
		If FlatDlg.m_bUseROI AndAlso Not FlatDlg.m_ImageBox.IsTrackerEmpty Then
			FlatDlg.m_ImageBox.DisplayTracker()
		End If
	End Sub


	Private Shared Sub GetSignalStatus(ByVal sender As Object, ByVal argsSignal As SapSignalNotifyEventArgs)
		Dim FlatDlg As FlatFieldDemoDlg = TryCast(argsSignal.Context, FlatFieldDemoDlg)
		Dim signalStatus As SapAcquisition.AcqSignalStatus = argsSignal.SignalStatus

		FlatDlg.m_IsSignalDetected = (signalStatus <> SapAcquisition.AcqSignalStatus.None)
		If FlatDlg.m_IsSignalDetected = False Then
			FlatDlg.StatusLabelInfo.Text = "Online... No camera signal detected"
		Else
			FlatDlg.StatusLabelInfo.Text = "Online... Camera signal detected"
		End If
	End Sub

	Private Sub SystemEvents_SessionEnded(ByVal sender As Object, ByVal args As SessionEndedEventArgs)
		' The FormClosed event is not invoked when logging off or shutting down,
		' so we need to clean up here too.
		DestroyObjects()
		DisposeObjects()
	End Sub
	Public Sub New()
		m_Acquisition = Nothing
		m_Buffers = Nothing
		m_Xfer = Nothing
		m_View = Nothing
		m_IsSignalDetected = True
		m_bUseROI = False

		InitializeComponent()

		' Note:
		'  The code to initialize m_ImageBox was originally in the InitializeComponent function
		'  called above. However, it has been moved to the dialog constructor as a workaround
		'  to a Visual Studio Designer error when loading the DALSA.SaperaLT.SapClassBasic
		'  assembly under 64-bit Windows.
		'  As a consequence, it is not possible to adjust the m_ImageBox properties
		'  automatically using the Designer anymore, this has to be done manually.
		' 
		Me.m_ImageBox = New DALSA.SaperaLT.Demos.NET.VB.FlatFieldDemo.ImageBox
		Me.m_ImageBox.Location = New System.Drawing.Point(262, 74)
		Me.m_ImageBox.Name = "m_ImageBox"
		Me.m_ImageBox.PixelValueDisplay = Me.PixelDataValue
		Me.m_ImageBox.Size = New System.Drawing.Size(386, 357)
		Me.m_ImageBox.SliderEnable = True
		Me.m_ImageBox.SliderMaximum = 10
		Me.m_ImageBox.SliderMinimum = 0
		Me.m_ImageBox.SliderValue = 0
		Me.m_ImageBox.SliderVisible = False
		Me.m_ImageBox.TabIndex = 30
		Me.m_ImageBox.TrackerEnable = False
		Me.m_ImageBox.View = Nothing
		Me.Controls.Add(Me.m_ImageBox)

		Dim acConfigDlg As New AcqConfigDlg(Nothing, "", AcqConfigDlg.ServerCategory.ServerAcq)
		If acConfigDlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
			m_online = True
		Else
			m_online = False
		End If

		If Not CreateNewObjects(acConfigDlg, False) Then
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
		If m_online Then
			If Not Restore Then
				m_ServerLocation = acConfigDlg.ServerLocation
				m_ConfigFileName = acConfigDlg.ConfigFile
			End If
			' define on-line object
			m_Acquisition = New SapAcquisition(m_ServerLocation, m_ConfigFileName)
			If SapBuffer.IsBufferTypeSupported(m_ServerLocation, SapBuffer.MemoryType.ScatterGather) Then
				m_Buffers = New SapBufferWithTrash(2, m_Acquisition, SapBuffer.MemoryType.ScatterGather)
			Else
				m_Buffers = New SapBufferWithTrash(2, m_Acquisition, SapBuffer.MemoryType.ScatterGatherPhysical)
			End If
			m_Xfer = New SapAcqToBuf(m_Acquisition, m_Buffers)
			m_FlatField = New SapFlatField(m_Acquisition)


			'event for transfer
			m_Xfer.Pairs(0).EventType = SapXferPair.XferEventType.EndOfFrame
			AddHandler m_Xfer.XferNotify, AddressOf xfer_XferNotify
			m_Xfer.XferNotifyContext = Me

			' event for signal status
			AddHandler m_Acquisition.SignalNotify, AddressOf GetSignalStatus
			m_Acquisition.SignalNotifyContext = Me
		Else
			'define off-line object
			m_Buffers = New SapBuffer()
			m_FlatField = New SapFlatField(m_Buffers)
			StatusLabelInfo.Text = "offline... Load images"
		End If

		m_View = New SapView(m_Buffers)
		'event for view  
		AddHandler m_View.DisplayDone, AddressOf ViewCallback
		m_View.DisplayDoneContext = Me
		m_View.DisplayDoneEnable = True

		m_Pro = New SapMyProcessing(m_Buffers, m_FlatField, New SapProcessingDoneHandler(AddressOf ProCallback), Me)
		'Set SaperaView to imagebox control 
		m_ImageBox.View = m_View

		If Not CreateObjects() Then
			DisposeObjects()
			Return False
		End If

		' Resize ImagBox to take into account the size of created sapview
		m_ImageBox.OnSize()
		EnableSignalStatus()
		UpdateControls()
		Return True
	End Function

	' Create new objects with acquisition information
	Public Function CreateNewObjects() As Boolean
		If m_online Then
			' define on-line object
			m_Acquisition = New SapAcquisition(m_ServerLocation, m_ConfigFileName)
			If SapBuffer.IsBufferTypeSupported(m_ServerLocation, SapBuffer.MemoryType.ScatterGather) Then
				m_Buffers = New SapBufferWithTrash(2, m_Acquisition, SapBuffer.MemoryType.ScatterGather)
			Else
				m_Buffers = New SapBufferWithTrash(2, m_Acquisition, SapBuffer.MemoryType.ScatterGatherPhysical)
			End If
			m_Xfer = New SapAcqToBuf(m_Acquisition, m_Buffers)
			m_FlatField = New SapFlatField(m_Acquisition)


			'event for view
			m_Xfer.Pairs(0).EventType = SapXferPair.XferEventType.EndOfFrame
			AddHandler m_Xfer.XferNotify, AddressOf xfer_XferNotify
			m_Xfer.XferNotifyContext = Me

			' event for signal status
			AddHandler m_Acquisition.SignalNotify, AddressOf GetSignalStatus
			m_Acquisition.SignalNotifyContext = Me
		Else
			'define off-line object
			m_Buffers = New SapBuffer()
			m_FlatField = New SapFlatField(m_Buffers)
			StatusLabelInfo.Text = "offline... Load images"
		End If

		m_View = New SapView(m_Buffers)
		'event for view  
		AddHandler m_View.DisplayDone, AddressOf ViewCallback
		m_View.DisplayDoneContext = Me
		m_View.DisplayDoneEnable = True

		m_Pro = New SapMyProcessing(m_Buffers, m_FlatField, New SapProcessingDoneHandler(AddressOf ProCallback), Me)
		'Set SaperaView to imagebox control 
		m_ImageBox.View = m_View

		If Not CreateObjects() Then
			DisposeObjects()
			Return False
		End If

		' Resize ImagBox to take into account the size of created sapview
		m_ImageBox.OnSize()
		EnableSignalStatus()
		UpdateControls()
		Return True
	End Function


	' Call Create Object 
	Private Function CreateObjects() As Boolean
		' Create acquisition object
		If m_Acquisition IsNot Nothing AndAlso Not m_Acquisition.Initialized Then
			If Not m_Acquisition.Create() Then
				DestroyObjects()
				Return False
			End If
		End If

		' Create buffer object
		If m_Buffers IsNot Nothing AndAlso Not m_Buffers.Initialized Then
			If m_Buffers.Create() = False Then
				DestroyObjects()
				Return False
			End If
			m_Buffers.Clear()
		End If

		' Create flat field object
		If m_FlatField IsNot Nothing AndAlso Not m_FlatField.Initialized Then
			If Not m_FlatField.Create() Then
				DestroyObjects()
				Return False
			End If
		End If
		' Create view object
		If m_View IsNot Nothing AndAlso Not m_View.Initialized Then
			If m_View.Create() = False Then
				DestroyObjects()
				Return False
			End If
		End If
		' Create Xfer object
		If m_Xfer IsNot Nothing AndAlso Not m_Xfer.Initialized Then
			If m_Xfer.Create() = False Then
				DestroyObjects()
				Return False
			End If
		End If

		If m_Pro IsNot Nothing AndAlso Not m_Pro.Initialized Then
			If Not m_Pro.Create() Then
				DestroyObjects()
				Return False
			End If
			m_Pro.AutoEmpty = True
			If m_Xfer IsNot Nothing Then
				m_Xfer.AutoEmpty = False
			End If
		End If
		Return True
	End Function

	Private Sub DestroyObjects()
		If m_Xfer IsNot Nothing AndAlso m_Xfer.Initialized Then
			m_Xfer.Destroy()
		End If

		' Destroy processing object
		If m_Pro IsNot Nothing AndAlso m_Pro.Initialized Then
			m_Pro.Destroy()
		End If

		If m_View IsNot Nothing AndAlso m_View.Initialized Then
			m_View.Destroy()
		End If

		If m_FlatField IsNot Nothing AndAlso m_FlatField.Initialized Then
			m_FlatField.Destroy()
			' Disable flat field correction
			checkBox_FaltField.Checked = False
			checkBox_Hardware_Correction.Checked = False
		End If

		If m_Buffers IsNot Nothing AndAlso m_Buffers.Initialized Then
			m_Buffers.Destroy()
		End If

		If m_Acquisition IsNot Nothing AndAlso m_Acquisition.Initialized Then
			m_Acquisition.Destroy()
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
		If m_Acquisition IsNot Nothing Then
			m_Acquisition.Dispose()
			m_Acquisition = Nothing
		End If
	End Sub


	'**************************************************************************************
	' Updates the menu items enabling/disabling the proper items depending on the state
	'  of the application
	'**************************************************************************************
	Private Sub UpdateControls()
		Dim bAcqNoGrab As Boolean = m_Xfer IsNot Nothing AndAlso m_Xfer.Initialized AndAlso Not m_Xfer.Grabbing
		Dim bAcqGrab As Boolean = m_Xfer IsNot Nothing AndAlso m_Xfer.Initialized AndAlso m_Xfer.Grabbing
		Dim bNoGrab As Boolean = m_Xfer Is Nothing OrElse Not m_Xfer.Grabbing

		Dim isFlatFieldEnabled As Boolean = m_FlatField IsNot Nothing AndAlso m_FlatField.Initialized AndAlso m_FlatField.Enabled
		Dim isFlatFieldAvailable As Boolean = m_Acquisition IsNot Nothing AndAlso m_Acquisition.Initialized AndAlso m_Acquisition.FlatFieldAvailable
		Dim isFlatFieldSoftware As Boolean = m_FlatField IsNot Nothing AndAlso m_FlatField.Initialized AndAlso m_FlatField.SoftwareCorrection
		Dim isFlatFieldPixelReplacement As Boolean = m_FlatField IsNot Nothing AndAlso m_FlatField.Initialized AndAlso m_FlatField.PixelReplacement

		Dim isBayerAvailable As Boolean = m_Acquisition IsNot Nothing AndAlso m_Acquisition.Initialized AndAlso m_Acquisition.BayerAvailable

		' Acquisition Control
		button_Grab.Enabled = bAcqNoGrab
		button_Snap.Enabled = bAcqNoGrab
		button_Freeze.Enabled = bAcqGrab

		' Acquisition Options
		button_LoadConfig.Enabled = bAcqNoGrab

		' Flat field correction
		checkBox_FaltField.Enabled = bNoGrab
		button_Calibrate.Enabled = bNoGrab AndAlso Not isFlatFieldEnabled
		button_Load_FF.Enabled = bNoGrab AndAlso Not isFlatFieldEnabled
		button_Save_FF.Enabled = bNoGrab

		checkBox_FaltField.Checked = isFlatFieldEnabled
		checkBox_Hardware_Correction.Checked = isFlatFieldAvailable AndAlso Not isFlatFieldSoftware

		Dim bayerDecoder As Boolean = False
		If m_Acquisition IsNot Nothing AndAlso m_Acquisition.Initialized AndAlso m_Acquisition.IsParameterAvailable(SapAcquisition.Prm.BAYER_DECODER_ENABLE) Then
			' Get Parameter of Bayer Decoder
			Dim value As Integer
			m_Acquisition.GetParameter(SapAcquisition.Prm.BAYER_DECODER_ENABLE, value)
			If value > 0 Then
				bayerDecoder = True
			End If
			If isBayerAvailable AndAlso bayerDecoder AndAlso isFlatFieldAvailable Then
				checkBox_Hardware_Correction.Checked = True
			End If
		End If

		checkBox_Pixels_Replacement.Checked = isFlatFieldPixelReplacement

		If (m_Acquisition IsNot Nothing AndAlso m_Acquisition.Initialized) AndAlso (isBayerAvailable AndAlso bayerDecoder) Then
			checkBox_Hardware_Correction.Enabled = isBayerAvailable AndAlso bayerDecoder AndAlso isFlatFieldAvailable
		Else
			checkBox_Hardware_Correction.Enabled = isFlatFieldAvailable AndAlso isFlatFieldEnabled AndAlso bNoGrab
		End If

		checkBox_Pixels_Replacement.Enabled = isFlatFieldSoftware And isFlatFieldEnabled

		' File Options
		button_New.Enabled = bNoGrab
		button_Load.Enabled = bNoGrab
		button_Save.Enabled = bNoGrab

		' General Options
		button_Buffers.Enabled = bNoGrab
		button_View.Enabled = bNoGrab
	End Sub


	Private Function CheckPixelFormat(ByVal mode As String) As Boolean
		Dim format As SapFormat = m_Buffers.Format

		If format <> SapFormat.Mono8 AndAlso format <> SapFormat.Mono16 Then
			Dim message As String = [String].Format("Pixel format must be 8-bit or 16-bit monochrome for {0}" & vbLf, mode)


			MessageBox.Show(message)
			Return False
		End If

		Return True
	End Function

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

	'*****************************************************************************************
	'
	'					General Function
	'
	'*****************************************************************************************

	Private Sub button_Exit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Exit.Click
		Application.[Exit]()
	End Sub

	Private Sub FlatFieldDemoDlg_FormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs) Handles MyBase.FormClosed
		DestroyObjects()
		DisposeObjects()
	End Sub

	Private Sub EnableSignalStatus()
		If m_Acquisition IsNot Nothing Then
			m_IsSignalDetected = (m_Acquisition.SignalStatus <> SapAcquisition.AcqSignalStatus.None)
			If m_IsSignalDetected = False Then
				StatusLabelInfo.Text = "Online... No camera signal detected"
			Else
				StatusLabelInfo.Text = "Online... Camera signal detected"
			End If
			m_Acquisition.SignalNotifyEnable = True
		End If
	End Sub

	Private Sub FlatFieldDemoDlg_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
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

	'*****************************************************************************************
	'
	'					Acquisition Control
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
	'					Flat field Options
	'
	'*****************************************************************************************

	Private Sub button_Calibrate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Calibrate.Click
		If Not CheckPixelFormat("calibration") Then
			Exit Sub
		End If

		Dim dlg As New FlatFieldDlg(m_FlatField, m_Xfer, m_Buffers)
		dlg.ShowDialog()
		UpdateControls()
	End Sub

	Private Sub checkBox_FaltField_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles checkBox_FaltField.Click
		Cursor = Cursors.WaitCursor

		If m_FlatField IsNot Nothing AndAlso m_FlatField.Initialized Then
			' To enable/disable flat field correction, the transfer object must first be disconnected from the hardware
			If m_Xfer IsNot Nothing AndAlso m_Xfer.Initialized Then
				m_Xfer.Destroy()
			End If

			Dim success As Boolean = True

			' Check for invalid pixel format
			If checkBox_FaltField.Checked AndAlso Not checkBox_Hardware_Correction.Checked Then
				success = CheckPixelFormat("software correction")
			End If

			' Enable/disable flat field correction
			If success Then
				m_FlatField.Enable(checkBox_FaltField.Checked, checkBox_Hardware_Correction.Checked)
			End If

			If m_Xfer IsNot Nothing AndAlso Not m_Xfer.Initialized Then
				' Recreate the transfer object to reconnect it to the hardware
				m_Xfer.Create()
				m_Xfer.Init(True)
				m_Pro.Init()
			End If

			UpdateControls()
		End If
		Cursor = Cursors.[Default]
	End Sub

	Private Sub checkBox_Hardware_Correction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles checkBox_Hardware_Correction.Click
		Cursor = Cursors.WaitCursor
		If m_FlatField IsNot Nothing AndAlso m_FlatField.Initialized Then
			' To enable/disable flat field correction, the transfer object must first be disconnected from the hardware
			If m_Xfer IsNot Nothing AndAlso m_Xfer.Initialized Then
				m_Xfer.Destroy()
			End If

			' Get Parameter of Bayer Decoder
			Dim bayerDecoder As Integer
			If m_Acquisition IsNot Nothing AndAlso m_Acquisition.Initialized AndAlso m_Acquisition.BayerAvailable Then
				m_Acquisition.GetParameter(SapAcquisition.Prm.BAYER_DECODER_ENABLE, bayerDecoder)
				If bayerDecoder > 0 Then
					checkBox_Hardware_Correction.Checked = True
					MessageBox.Show("Hardware correction is always used when Bayer decoder is enabled.")
				End If
			End If

			Dim success As Boolean = True

			' Check for invalid pixel format when using software correction
			If checkBox_FaltField.Checked AndAlso Not checkBox_Hardware_Correction.Checked Then
				success = CheckPixelFormat("software correction")
			End If

			' Enable/disable flat field correction
			If success Then
				m_FlatField.Enable(checkBox_FaltField.Checked, checkBox_Hardware_Correction.Checked)
			End If

			If m_Xfer IsNot Nothing AndAlso Not m_Xfer.Initialized Then
				' Recreate the transfer object to reconnect it to the hardware
				m_Xfer.Create()
				m_Xfer.Init(True)
				m_Pro.Init()
			End If

			UpdateControls()
		End If
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

	Private Sub checkBox_Pixels_Replacement_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles checkBox_Pixels_Replacement.Click
		' Enable/disable pixel replacement flat field correction
		m_FlatField.PixelReplacement = checkBox_Pixels_Replacement.Checked

		UpdateControls()
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

	'*****************************************************************************************
	'
	'					General Options
	'
	'*****************************************************************************************

	Private Sub button_Buffers_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Buffers.Click
		' Set new buffer parameters   
		Dim dlg As New BufDlg(m_Buffers, m_View.Display, m_online)
		If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
			DestroyObjects()
			DisposeObjects()

			' Update objects with new buffer
			' Update objects with new acquisition
			If Not CreateNewObjects() Then
				MessageBox.Show("New objects creation has failed. Restoring original object ")
				' Recreate original objects
				If Not CreateNewObjects(Nothing, True) Then
					MessageBox.Show("Original object creation has failed. Closing application ")
					Application.[Exit]()
				End If
			End If
		End If
	End Sub

	Private Sub button_View_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_View.Click
		Dim viewDialog As New ViewDlg(m_View, m_ImageBox.ViewRectangle)

		If viewDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
			m_ImageBox.OnSize()
		End If
	End Sub

	Private Sub button_LoadConfig_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_LoadConfig.Click
		' Set new acquisition parameters     
		Dim acConfigDlg As New AcqConfigDlg(Nothing, "", AcqConfigDlg.ServerCategory.ServerAcq)
		If acConfigDlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
			Dim m_onlineSave As Boolean = m_online
			m_online = True
			' Save device location
			Dim m_ServerLocationSave As SapLocation = m_ServerLocation
			Dim m_ConfigFileNameSave As String = m_ConfigFileName
			'Destroy Object
			DestroyObjects()
			DisposeObjects()
			' Update objects with new acquisition
			If Not CreateNewObjects(acConfigDlg, False) Then
				MessageBox.Show("New objects creation has failed. Restoring original object ")
				m_ServerLocation = m_ServerLocationSave
				m_ConfigFileName = m_ConfigFileNameSave
				m_online = m_onlineSave
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

	'*****************************************************************************************
	'
	'					File Options
	'
	'*****************************************************************************************

	Private Sub button_New_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_New.Click
		m_Buffers.Clear()
		m_ImageBox.Refresh()
	End Sub

	Private Sub button_Load_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Load.Click
		Dim newDialogLoad As New LoadSaveDlg(m_Buffers, True, False)
		' Show the dialog and process the result    
		If newDialogLoad.ShowDialog() = Windows.Forms.DialogResult.OK Then
			If m_FlatField.Enabled AndAlso m_FlatField.SoftwareCorrection Then
				m_Pro.Execute()
			End If
		End If
		newDialogLoad.Dispose()
		m_ImageBox.OnSize()
	End Sub

	Private Sub button_Save_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Save.Click
		Dim newDialogSave As New LoadSaveDlg(m_Buffers, False, False)
		' Show the dialog and process the result
		newDialogSave.ShowDialog()
		newDialogSave.Dispose()
	End Sub

End Class






