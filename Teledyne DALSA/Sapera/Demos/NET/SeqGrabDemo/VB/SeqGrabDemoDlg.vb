Public Class SeqGrabDemoDlg

   Private m_Acquisition As SapAcquisition
   Private m_Buffers As SapBuffer
   Private m_Xfer As SapAcqToBuf
   Private m_View As SapView
   Private m_ServerLocation As SapLocation
   Private m_ConfigFileName As String
   Private m_bRecordOn As Boolean
   Private m_bPlayOn As Boolean
   Private m_bPauseOn As Boolean
   Private m_online As Boolean
   Private m_IsSignalDetected As Boolean
   Private m_nFramesPerCallback As Integer
   Private m_nFramesOnBoard As Integer
   Private m_MinTime As Single
   Private m_MaxTime As Single
   'System menu
   Private m_SystemMenu As SystemMenu
   'index for "about this.." item im system menu
   Private Const m_AboutID As Integer = 256
   Private Const Default_Nbr_buffers As Integer = 15

   ' Delegate to display number of frame acquired 
   ' Delegate is needed because .NEt framework does not support cross thread control modification
   Private Delegate Sub RefreshControlDelegate()
   Private Delegate Sub CheckForLastFrameDelegate()

   Private Shared Sub xfer_XferNotify(ByVal sender As Object, ByVal argsNotify As SapXferNotifyEventArgs)

      Dim SeqGDDlg As SeqGrabDemoDlg = TryCast(argsNotify.Context, SeqGrabDemoDlg)

      ' Check if last frame is reached
      SeqGDDlg.Invoke(New CheckForLastFrameDelegate(AddressOf SeqGDDlg.CheckForLastFrame))

      ' Refresh view
      SeqGDDlg.m_View.Show()

      ' Refresh controls
      SeqGDDlg.Invoke(New RefreshControlDelegate(AddressOf SeqGDDlg.RefreshTextBox))

   End Sub

   Private Shared Sub GetSignalStatus(ByVal sender As Object, ByVal argsSignal As SapSignalNotifyEventArgs)
      Dim SeqGDDlg As SeqGrabDemoDlg = TryCast(argsSignal.Context, SeqGrabDemoDlg)
      Dim signalStatus As SapAcquisition.AcqSignalStatus = argsSignal.SignalStatus

      SeqGDDlg.m_IsSignalDetected = (signalStatus <> SapAcquisition.AcqSignalStatus.None)
      If SeqGDDlg.m_IsSignalDetected = False Then
         SeqGDDlg.StatusLabelInfo.Text = "Online... No camera signal detected"
      Else
         SeqGDDlg.StatusLabelInfo.Text = "Online... Camera signal detected"
      End If
   End Sub

   Private Shared Sub AcqCallback(ByVal sender As Object, ByVal argsSignal As SapAcqNotifyEventArgs)
      Dim SeqGDDlg As SeqGrabDemoDlg = TryCast(argsSignal.Context, SeqGrabDemoDlg)
      SeqGDDlg.FrameLostCount.Text = "Frame Lost : " & argsSignal.EventCount.ToString()
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
      m_bRecordOn = False
      m_bPlayOn = False
      m_bPauseOn = False
      m_nFramesPerCallback = 1
      m_nFramesOnBoard = 4096

      InitializeComponent()

      ' Note:
      '  The code to initialize m_ImageBox was originally in the InitializeComponent function
      '  called above. However, it has been moved to the dialog constructor as a workaround
      '  to a Visual Studio Designer error when loading the DALSA.SaperaLT.SapClassBasic
      '  assembly under 64-bit Windows.
      '  As a consequence, it is not possible to adjust the m_ImageBox properties
      '  automatically using the Designer anymore, this has to be done manually.
      ' 
      Me.m_ImageBox = New DALSA.SaperaLT.Demos.NET.VB.SeqGrabDemo.ImageBox
      Me.m_ImageBox.Location = New System.Drawing.Point(251, 71)
      Me.m_ImageBox.Name = "m_ImageBox"
      Me.m_ImageBox.PixelValueDisplay = Nothing
      Me.m_ImageBox.Size = New System.Drawing.Size(386, 406)
      Me.m_ImageBox.SliderEnable = False
      Me.m_ImageBox.SliderMaximum = 10
      Me.m_ImageBox.SliderMinimum = 0
      Me.m_ImageBox.SliderValue = 0
      Me.m_ImageBox.SliderVisible = True
      Me.m_ImageBox.TabIndex = 22
      Me.m_ImageBox.TrackerEnable = False
      Me.m_ImageBox.View = Nothing
      Me.Controls.Add(Me.m_ImageBox)

      Dim acConfigDlg As New AcqConfigDlg(Nothing, "", AcqConfigDlg.ServerCategory.ServerAcq)
      If acConfigDlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
         m_online = True
      Else
         m_online = False
      End If

      If Not CreateNewObjects(Default_Nbr_buffers, acConfigDlg) Then
         Me.Close()
      End If
   End Sub

   Private Sub RefreshTextBox()
      UpdateTextBox()
      UpdateFrameRate()
   End Sub


   '*****************************************************************************************
   '
   '					Create and Destroy Object
   '
   '*****************************************************************************************

   ' Create new objects with acquisition information
   Public Function CreateNewObjects(ByVal Nbr_Buffers As Integer, ByVal acConfigDlg As AcqConfigDlg) As Boolean

      If m_online Then
         If acConfigDlg IsNot Nothing Then
            m_ServerLocation = acConfigDlg.ServerLocation
            m_ConfigFileName = acConfigDlg.ConfigFile
         End If

         m_Acquisition = New SapAcquisition(m_ServerLocation, m_ConfigFileName)
         If SapBuffer.IsBufferTypeSupported(m_ServerLocation, SapBuffer.MemoryType.ScatterGather) Then
            m_Buffers = New SapBufferWithTrash(Nbr_Buffers, m_Acquisition, SapBuffer.MemoryType.ScatterGather)
         Else
            m_Buffers = New SapBufferWithTrash(Nbr_Buffers, m_Acquisition, SapBuffer.MemoryType.ScatterGatherPhysical)
         End If
         m_Xfer = New SapAcqToBuf(m_Acquisition, m_Buffers)
         m_View = New SapView(m_Buffers)

         m_Xfer.Pairs(0).EventType = SapXferPair.XferEventType.EndOfFrame
         AddHandler m_Xfer.XferNotify, AddressOf xfer_XferNotify
         m_Xfer.XferNotifyContext = Me

         ' event fot Acqcallback (Frame lost)
         m_Acquisition.EventType = SapAcquisition.AcqEventType.FrameLost
         AddHandler m_Acquisition.AcqNotify, AddressOf AcqCallback
         m_Acquisition.AcqNotifyContext = Me

         ' event for signal status
         AddHandler m_Acquisition.SignalNotify, AddressOf GetSignalStatus
         m_Acquisition.SignalNotifyContext = Me
      Else
         m_Buffers = New SapBuffer()
         m_Buffers.Count = Nbr_Buffers
         m_View = New SapView(m_Buffers)
      End If

      m_ImageBox.View = m_View

      If Not CreateObjects() Then
         DisposeObjects()
         Return False
      End If

      m_ImageBox.OnSize()
      UpdateControls()
      UpdateTextBox()
      EnableSignalStatus()
      Return True
   End Function

   ' Create new objects with buffer information
   Public Function CreateNewObjects(ByVal dlg As BufDlg) As Boolean

      If m_online Then
         m_Acquisition = New SapAcquisition(m_ServerLocation, m_ConfigFileName)
         m_Buffers = New SapBufferWithTrash(Math.Max(2, dlg.Count), m_Acquisition, dlg.Type)

         m_Xfer = New SapAcqToBuf(m_Acquisition, m_Buffers)
         m_View = New SapView(m_Buffers)

         m_Xfer.Pairs(0).EventType = SapXferPair.XferEventType.EndOfFrame
         AddHandler m_Xfer.XferNotify, AddressOf xfer_XferNotify
         m_Xfer.XferNotifyContext = Me

         ' event fot Acqcallback (Frame lost)
         m_Acquisition.EventType = SapAcquisition.AcqEventType.FrameLost
         AddHandler m_Acquisition.AcqNotify, AddressOf AcqCallback
         m_Acquisition.AcqNotifyContext = Me

         ' event for signal status
         AddHandler m_Acquisition.SignalNotify, AddressOf GetSignalStatus
         m_Acquisition.SignalNotifyContext = Me
      Else
         m_Buffers = New SapBuffer(dlg.Count, dlg.BWidth, dlg.BHeight, dlg.Format, dlg.Type)
         m_View = New SapView(m_Buffers)
      End If

      m_ImageBox.View = m_View

      If CreateObjects() = False Then
         DisposeObjects()
         Return False
      End If

      m_ImageBox.OnSize()
      UpdateControls()
      UpdateTextBox()
      EnableSignalStatus()
      Return True
   End Function
   Private Function CreateObjects() As Boolean

      ' Call Create Object 
      ' Create acquisition object
      If m_Acquisition IsNot Nothing AndAlso Not m_Acquisition.Initialized Then
         If m_Acquisition.Create() = False Then
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
      ' Create view object
      If m_View IsNot Nothing AndAlso Not m_View.Initialized Then
         If m_View.Create() = False Then
            DestroyObjects()
            Return False
         End If
      End If
      ' Create Xfer object
      If m_Xfer IsNot Nothing AndAlso Not m_Xfer.Initialized Then

         'Number of frames per callback retreived
         Dim nFramesPerCallback As Integer

         ' Set number of frames per callback
         m_Xfer.Pairs(0).FramesPerCallback = m_nFramesPerCallback

         ' If there is a large number of buffers, temporarily boost the command timeout value,
         ' since the call to Create may take a long time to complete.
         ' As a safe rule of thumb, use 100 milliseconds per buffer.
         Dim oldCommandTimeout As Integer = SapManager.CommandTimeout
         Dim newCommandTimeout As Integer = 100 * m_Buffers.Count

         If newCommandTimeout < oldCommandTimeout Then
            newCommandTimeout = oldCommandTimeout
         End If

         SapManager.CommandTimeout = newCommandTimeout

         ' Create transfer object
         If Not m_Xfer.Create() Then
            DestroyObjects()
            Return False
         End If

         ' Restore original command timeout value
         SapManager.CommandTimeout = oldCommandTimeout
         ' initialize tranfer object and reset source/destination index

         m_Xfer.Init(True)

         ' Retrieve number of frames per callback
         ' It may be less than what we have asked for.
         nFramesPerCallback = m_Xfer.Pairs(0).FramesPerCallback
         If m_nFramesPerCallback > nFramesPerCallback Then
            m_nFramesPerCallback = nFramesPerCallback
            MessageBox.Show("No memory")
         End If
      End If
      Return True
   End Function

   Private Sub DestroyObjects()

      If m_Xfer IsNot Nothing AndAlso m_Xfer.Initialized Then
         m_Xfer.Destroy()
      End If
      If m_View IsNot Nothing AndAlso m_View.Initialized Then
         m_View.Destroy()
      End If
      If m_Buffers IsNot Nothing AndAlso m_Buffers.Initialized Then
         m_Buffers.Destroy()
      End If
      If m_Acquisition IsNot Nothing AndAlso m_Acquisition.Initialized Then
         m_Acquisition.Destroy()
      End If
   End Sub

   Private Sub DisposeObjects()

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

   '**********************************************************************************
   '
   ' Window related functions
   '
   '**********************************************************************************

   Protected Overloads Overrides Sub OnResize(ByVal argsPaint As EventArgs)
      If m_ImageBox IsNot Nothing Then
         m_ImageBox.OnSize()
      End If
      MyBase.OnResize(argsPaint)
   End Sub

   Private Sub SeqGrabDemoDlg_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
      DestroyObjects()
      DisposeObjects()
   End Sub

   '**********************************************************************************
   '
   '			Timer related functions
   '
   '**********************************************************************************
   Private Sub timer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles timer.Tick
      ' Increase buffer index
      m_Buffers.Next()
      ' Resfresh display
      m_View.Show()
      ' Refresh controls
      UpdateTextBox()
      ' Check if last frame is reached
      CheckForLastFrame()
   End Sub

   '**********************************************************************************
   '
   '				General functions
   '
   '**********************************************************************************

   Private Sub UpdateControls()

      Dim bAcqNoGrab As Boolean = (m_Xfer IsNot Nothing) AndAlso Not m_bRecordOn AndAlso Not m_bPlayOn
      Dim bNoGrab As Boolean = Not m_bRecordOn AndAlso Not m_bPlayOn

      ' Record Control
      button_Record.Enabled = bAcqNoGrab
      button_Play.Enabled = bNoGrab
      button_Pause.Enabled = Not bNoGrab
      button_Stop.Enabled = Not bNoGrab
      button_Pause.Text = IIf(m_bPauseOn, "Continue", "Pause")

      ' General Options
      button_Buffers.Enabled = bNoGrab
      button_Load_Config.Enabled = bNoGrab
      button_High_Frame_Rate.Enabled = bNoGrab

      ' File Options
      button_Load_Current.Enabled = bNoGrab
      button_Load_Sequence.Enabled = bNoGrab
      button_Save_Current.Enabled = bNoGrab
      button_Save_Sequence.Enabled = bNoGrab

      ' Recording statistics
      textBox_Frame_Rate.Enabled = bNoGrab

      ' Slider
      m_ImageBox.SliderEnable = bNoGrab OrElse (m_bPlayOn AndAlso m_bPauseOn)
      m_ImageBox.SliderMinimum = 0
      m_ImageBox.SliderMaximum = m_Buffers.Count - 1
   End Sub

   Private Sub UpdateTextBox()

      textBox_Displayed.Text = Convert.ToString(m_Buffers.Index + 1)
      textBox_NumberOfBuffers.Text = m_Buffers.Count.ToString()
      textBox_Minimum.Text = m_MinTime.ToString()
      textBox_Maximum.Text = m_MaxTime.ToString()
      m_ImageBox.SliderValue = m_Buffers.Index
      If Not m_bPlayOn Then
         textBox_Frame_Rate.Text = m_Buffers.FrameRate.ToString()
      End If
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


   Private Sub button_Exit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_Exit.Click
      Application.Exit()
   End Sub

   '**********************************************************************************
   '
   '				Frame rate related functions
   '
   '**********************************************************************************

   Private Sub CheckForLastFrame()
      ' Check for last frame
      If m_Buffers.Index = m_Buffers.Count - 1 Then
         If m_bRecordOn Then
            m_bRecordOn = False
         End If
         If m_bPlayOn Then
            m_bPlayOn = False
            timer.Enabled = False
         End If
         UpdateControls()
      End If
   End Sub

   Private Sub UpdateFrameRate()
      If m_Xfer.UpdateFrameRateStatistics() Then
         Dim stats As SapXferFrameRateInfo = m_Xfer.FrameRateStatistics
         Dim framerate As Single = 0.0F

         If stats.IsBufferFrameRateAvailable Then
            framerate = stats.BufferFrameRate
         ElseIf stats.IsLiveFrameRateAvailable And Not stats.IsLiveFrameRateStalled Then
            framerate = stats.LiveFrameRate
         End If

         m_Buffers.FrameRate = framerate
         m_MinTime = stats.MinTimePerFrame
         m_MaxTime = stats.MaxTimePerFrame
      End If
   End Sub


   '*****************************************************************************************
   '
   '					General Options
   '
   '***************************************************************************************
   Private Sub button_Buffers_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_Buffers.Click
      ' Set new buffer parameters
      Dim dlg As New BufDlg(m_Buffers, m_View.Display, True)
      If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
         DestroyObjects()
         DisposeObjects()

         ' Update objects with new buffer
         If Not CreateNewObjects(dlg) Then
            MessageBox.Show("New objects creation has failed. Restoring original object ")
            ' Recreate original objects
            If Not CreateNewObjects(Default_Nbr_buffers, Nothing) Then
               MessageBox.Show("Original object creation has failed. Closing application ")
               Application.[Exit]()
            End If
         End If
         UpdateTextBox()
      End If
      m_ImageBox.Refresh()
   End Sub

   Private Sub button_Load_Config_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_Load_Config.Click
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
         If Not CreateNewObjects(Default_Nbr_buffers, acConfigDlg) Then
            MessageBox.Show("New objects creation has failed. Restoring original object ")
            m_ServerLocation = m_ServerLocationSave
            m_ConfigFileName = m_ConfigFileNameSave
            m_online = m_onlineSave
            ' Recreate original objects
            If Not CreateNewObjects(Default_Nbr_buffers, Nothing) Then
               MessageBox.Show("Original object creation has failed. Closing application ")
               Application.[Exit]()
            End If
         End If
      Else
         MessageBox.Show("No Modification in Acquisition")
      End If
      m_ImageBox.Refresh()
   End Sub

   Private Sub button_High_Frame_Rate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_High_Frame_Rate.Click
      Dim dlg As New HighFrameDlg(m_nFramesPerCallback, m_nFramesOnBoard, m_Xfer)
      If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
         m_nFramesPerCallback = dlg.Frame_Per_Callback
         m_nFramesOnBoard = dlg.Frame_On_Board
         Dim Nbr_Buffer As Integer = m_Buffers.Count
         DestroyObjects()
         DisposeObjects()
         CreateNewObjects(Nbr_Buffer, Nothing)
      End If
      m_ImageBox.Refresh()
   End Sub

   '*****************************************************************************************
   '
   '					Acquisition Control
   '
   '***************************************************************************************
   Private Sub button_Record_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_Record.Click

      ' Reset source and destination indices
      m_Xfer.Init(True)

      ' Reset the frame rate statistics ahead of each transfer stream
      m_Xfer.FrameRateStatistics.Reset()

      ' Acquire all frames
      If m_Xfer.Snap(m_Buffers.Count) Then
         m_bRecordOn = True
         UpdateControls()
      End If
   End Sub

   Private Sub button_Play_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_Play.Click
      ' Initialize buffer index
      m_Buffers.Index = 0

      ' Start playback timer
      timer.Interval = CInt(1000 / Single.Parse(textBox_Frame_Rate.Text))
      timer.Enabled = True

      m_bPlayOn = True
      UpdateControls()
   End Sub

   Private Sub button_Pause_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_Pause.Click
      If Not m_bPauseOn Then
         If m_bRecordOn Then
            ' Stop current acquisition
            If Not m_Xfer.Freeze() Then
               Return
            End If

            Dim dlg As New AbortDlg(m_Xfer)
            If dlg.ShowDialog() <> Windows.Forms.DialogResult.OK Then
               m_Xfer.Abort()
            End If
         ElseIf m_bPlayOn Then
            ' Stop playback timer
            timer.Enabled = False
         End If
      Else
         ' Check if recording or playing
         If m_bRecordOn Then
            'int frameTime = (int)(1000.0 / m_Buffers->GetFrameRate());
            'SetTimer(1, frameTime, NULL);
            ' Acquire remaining frames
            If Not m_Xfer.Snap(m_Buffers.Count - m_Buffers.Index - 1) Then
               Return
            End If
         ElseIf m_bPlayOn Then
            ' Restart playback timer
            timer.Enabled = True
         End If
      End If

      m_bPauseOn = Not m_bPauseOn
      UpdateControls()
   End Sub

   Private Sub button_Stop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_Stop.Click
      ' Check if recording or playing
      If m_bRecordOn Then
         ' Stop current acquisition
         If Not m_Xfer.Freeze() Then
            Return
         End If

         Dim dlg As New AbortDlg(m_Xfer)
         If dlg.ShowDialog() <> Windows.Forms.DialogResult.OK Then
            m_Xfer.Abort()
         End If

         m_bRecordOn = False
      ElseIf m_bPlayOn Then
         ' Stop playback timer
         timer.Enabled = False
         m_bPlayOn = False
      End If
      m_bPauseOn = False
      UpdateControls()
   End Sub

   '*****************************************************************************************
   '
   ' File Control
   '
   '*****************************************************************************************

   Private Sub button_Load_Sequence_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_Load_Sequence.Click
      If m_Buffers.Format = SapFormat.Mono16 Or m_Buffers.Format = SapFormat.RGB101010 Or _
              m_Buffers.Format = SapFormat.RGB161616 Or m_Buffers.Format = SapFormat.RGB16161616 Then
         MessageBox.Show("Sequence images in AVI format are sampled at 8-bit pixel depth." & vbNewLine & "You cannot load a sequence in the current configuration.")
         Return
      End If

      If m_Buffers.Format = SapFormat.RGBR888 Then
         MessageBox.Show("Sequence images acquired in RGBR888 format (red first) were saved as RGB888 (blue first)." & vbNewLine & "You cannot load a sequence in the current configuration.")
         Return
      End If

      Dim dlg As New LoadSaveDlg(m_Buffers, True, True)
      dlg.ShowDialog()
      dlg.Dispose()
      m_ImageBox.Refresh()
   End Sub

   Private Sub button_Save_Sequence_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_Save_Sequence.Click
      If m_Buffers.Format = SapFormat.Mono16 Or m_Buffers.Format = SapFormat.RGB101010 Or _
              m_Buffers.Format = SapFormat.RGB161616 Or m_Buffers.Format = SapFormat.RGB16161616 Then
         MessageBox.Show("Saving images in AVI format requires downsampling them to 8-bit pixel depth." & vbNewLine & "You will not be able to reload this sequence in this application unless you change the buffer format.")
      End If

      If m_Buffers.Format = SapFormat.RGBR888 Then
         MessageBox.Show("Saving images in AVI format requires conversion to RGB888 format (blue first)." & vbNewLine & "You will not be able to reload this sequence in this application unless you change the buffer format.")
      End If

      Dim dlg As New LoadSaveDlg(m_Buffers, False, True)
      dlg.ShowDialog()
      dlg.Dispose()
      m_ImageBox.Refresh()
   End Sub

   Private Sub button_Load_Current_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_Load_Current.Click
      Dim dlg As New LoadSaveDlg(m_Buffers, True, False)
      ' Show the dialog and process the result
      dlg.ShowDialog()
      dlg.Dispose()
      m_ImageBox.Refresh()
   End Sub

   Private Sub button_Save_Current_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_Save_Current.Click
      Dim dlg As New LoadSaveDlg(m_Buffers, False, False)
      ' Show the dialog and process the result
      dlg.ShowDialog()
      dlg.Dispose()
      m_ImageBox.Refresh()
   End Sub

   '*****************************************************************************************
   '          
   '				SystemMenu
   ' 
   '*****************************************************************************************
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

   Private Sub GigESeqGrabDemodlg_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
      ' Add about this to the system menu
      m_SystemMenu = SystemMenu.FromForm(Me)
      If m_SystemMenu IsNot Nothing Then
         m_SystemMenu.AppendSeparator()
         m_SystemMenu.AppendMenu(m_AboutID, "About this...")
      End If

      ' Register event handler which is invoked when logging off or shutting down
      AddHandler SystemEvents.SessionEnded, AddressOf SystemEvents_SessionEnded
   End Sub
End Class
