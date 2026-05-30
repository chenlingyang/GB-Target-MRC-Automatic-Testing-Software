Partial Public Class BayerDemoDlg
   Inherits Form

   Private Shared m_BayerOutputFormatValues As SapFormatInfo() = {New SapFormatInfo(SapFormat.RGB8888, "RGB 8888"), New SapFormatInfo(SapFormat.RGB101010, "RGB 101010"), New SapFormatInfo(SapFormat.RGB16161616, "RGB 16161616")}

   Private m_Acquisition As SapAcquisition
   Private m_Buffers As SapBuffer
   Private m_Xfer As SapAcqToBuf
   Private m_View As SapView
   Private m_Bayer As SapBayer
   Private m_Pro As SapProcessing
   Private m_ServerLocation As SapLocation
   Private m_ConfigFileName As String
   Private m_Executetime As String
   Private m_IsSignalDetected As Boolean
   Private m_online As Boolean


   'System menu
   Private m_SystemMenu As SystemMenu
   'index for "about this.." item im system menu
   Private Const m_AboutID As Integer = &H100

   ' Delegate to display number of frame acquired 
   ' Delegate is needed because .NEt framework does not support cross thread control modification
   Private Delegate Sub DisplayFrameAcquired(ByVal number As Integer, ByVal trash As Boolean)

   Private Shared Sub xfer_XferNotify(ByVal sender As Object, ByVal argsNotify As SapXferNotifyEventArgs)
      Dim BD As BayerDemoDlg = TryCast(argsNotify.Context, BayerDemoDlg)
      ' If grabbing in trash buffer, do not display the image, update the
      ' appropriate number of frames on the status bar instead
      If argsNotify.Trash Then
         BD.Invoke(New DisplayFrameAcquired(AddressOf BD.ShowFrameNumber), argsNotify.EventCount, True)
      Else
         ' Refresh view
         BD.Invoke(New DisplayFrameAcquired(AddressOf BD.ShowFrameNumber), argsNotify.EventCount, False)
         BD.m_Pro.ExecuteNext()
      End If
   End Sub

   '
   ' This function is called each time a buffer has been processed by the processing object
   '
   Private Shared Sub ProCallback(ByVal sender As Object, ByVal pInfo As SapProcessingDoneEventArgs)
      Dim BD As BayerDemoDlg = TryCast(pInfo.Context, BayerDemoDlg)

      ' Check if bayer conversion was enabled and if it's been done by software
      If BD.m_Bayer.Enabled AndAlso BD.m_Bayer.SoftwareConversion Then
         ' Show current buffer index and execution time in millisecond
         BD.m_Executetime = [String].Format("Bayer conversion = {0:0.00}", BD.m_Pro.Time)
      Else
         BD.m_Executetime = ""
      End If

      ' Refresh view
      BD.m_View.Show()
   End Sub

   '
   ' This function is called each time a buffer has been shown by the view object
   '
   Private Shared Sub ViewCallback(ByVal sender As Object, ByVal pInfo As SapDisplayDoneEventArgs)
      Dim BD As BayerDemoDlg = TryCast(pInfo.Context, BayerDemoDlg)

      If Not BD.m_ImageBox.IsTrackerEmpty Then
         BD.m_ImageBox.DisplayTracker()
      End If
   End Sub

   Private Shared Sub GetSignalStatus(ByVal sender As Object, ByVal argsSignal As SapSignalNotifyEventArgs)
      Dim BD As BayerDemoDlg = TryCast(argsSignal.Context, BayerDemoDlg)
      Dim signalStatus As SapAcquisition.AcqSignalStatus = argsSignal.SignalStatus

      BD.m_IsSignalDetected = (signalStatus <> SapAcquisition.AcqSignalStatus.None)
      If BD.m_IsSignalDetected = False Then
         BD.StatusLabelInfo.Text = "Online... No camera signal detected"
      Else
         BD.StatusLabelInfo.Text = "Online... Camera signal detected"
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
        m_Bayer = Nothing
        m_Pro = Nothing
        m_Executetime = ""

        InitializeComponent()

        ' Note:
        '  The code to initialize m_ImageBox was originally in the InitializeComponent function
        '  called above. However, it has been moved to the dialog constructor as a workaround
        '  to a Visual Studio Designer error when loading the DALSA.SaperaLT.SapClassBasic
        '  assembly under 64-bit Windows.
        '  As a consequence, it is not possible to adjust the m_ImageBox properties
        '  automatically using the Designer anymore, this has to be done manually.
        ' 
        Me.m_ImageBox = New DALSA.SaperaLT.Demos.NET.VB.BayerDemo.ImageBox
        Me.m_ImageBox.Location = New System.Drawing.Point(254, 69)
        Me.m_ImageBox.Name = "m_ImageBox"
        Me.m_ImageBox.PixelValueDisplay = Me.PixelDataValue
        Me.m_ImageBox.Size = New System.Drawing.Size(387, 356)
        Me.m_ImageBox.SliderEnable = True
        Me.m_ImageBox.SliderMaximum = 10
        Me.m_ImageBox.SliderMinimum = 0
        Me.m_ImageBox.SliderValue = 0
        Me.m_ImageBox.SliderVisible = False
        Me.m_ImageBox.TabIndex = 23
        Me.m_ImageBox.TrackerEnable = False
        Me.m_ImageBox.View = Nothing
        Me.Controls.Add(Me.m_ImageBox)

        ' Fill Bayer output format control
        For i As Integer = 0 To m_BayerOutputFormatValues.Length - 1
            comboBox_Output_Conversion.Items.Add(m_BayerOutputFormatValues(i))
        Next

        Dim acConfigDlg As New AcqConfigDlg(Nothing, "", AcqConfigDlg.ServerCategory.ServerAcq)
        If acConfigDlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            m_online = True
        Else
            m_online = False
        End If

        If Not CreateNewObjects(acConfigDlg, False) Then
            Me.Close()
        End If

    End Sub

   Private Sub ShowFrameNumber(ByVal number As Integer, ByVal trash As Boolean)
      Dim str As String
      If trash Then
         If m_Executetime = "" Then
            str = [String].Format("Frames acquired in trash buffer: {0}", number * m_Buffers.Count)
            Me.StatusLabelInfoTrash.Text = str
         End If
      Else

         If m_Executetime <> "" Then
            Me.StatusLabelInfo.Text = m_Executetime
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
         m_Bayer = New SapBayer(m_Acquisition, m_Buffers)


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
         m_Bayer = New SapBayer(m_Buffers)
         StatusLabelInfo.Text = "offline... Load images"
      End If

      m_View = New SapView(m_Buffers)
      'event for view  
      AddHandler m_View.DisplayDone, AddressOf ViewCallback
      m_View.DisplayDoneContext = Me
      m_View.DisplayDoneEnable = True

      m_Pro = New SapMyProcessing(m_Buffers, m_Bayer, New SapProcessingDoneHandler(AddressOf ProCallback), Me)
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
            m_Bayer = New SapBayer(m_Acquisition, m_Buffers)


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
            m_Bayer = New SapBayer(m_Buffers)
            StatusLabelInfo.Text = "offline... Load images"
        End If

        m_View = New SapView(m_Buffers)
        'event for view 
        AddHandler m_View.DisplayDone, AddressOf ViewCallback
        m_View.DisplayDoneContext = Me
        m_View.DisplayDoneEnable = True

        m_Pro = New SapMyProcessing(m_Buffers, m_Bayer, New SapProcessingDoneHandler(AddressOf ProCallback), Me)
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
         If m_Acquisition.Create() = False Then
            DestroyObjects()
            Return False
         End If
      End If

      ' Validate the video format of the configuration file
      If m_Acquisition IsNot Nothing Then
         Dim videoType As Integer = 0

         If Not m_Acquisition.GetParameter(SapAcquisition.Prm.VIDEO, videoType) Then
            DestroyObjects()
            Return False
         End If

         If videoType <> CInt(SapAcquisition.Val.VIDEO_BAYER) Then
            MessageBox.Show("The specified configuration file does not correspond to a Bayer camera")
            DestroyObjects()
            Return False
         End If
      End If

      ' Enable/Disable bayer conversion
      ' This call may require to modify the acquisition output format.
      ' For this reason, it has to be done after creating the acquisition object but before
      ' creating the output buffer object.
      If m_Bayer IsNot Nothing AndAlso Not m_Bayer.Enable(checkBox_Bayer_Conversion.Checked, checkBox_Hard_Conversion.Checked) Then
         checkBox_Bayer_Conversion.Checked = False
      End If

      ' Create buffer object
      If m_Buffers IsNot Nothing AndAlso Not m_Buffers.Initialized Then
         If m_Buffers.Create() = False Then
            DestroyObjects()
            Return False
         End If
         m_Buffers.Clear()
      End If

      ' Create bayer object
      If m_Bayer IsNot Nothing AndAlso Not m_Bayer.Initialized Then
         If m_Bayer.Create() = False Then
            DestroyObjects()
            Return False
         End If
      End If

      ' Create view object
      If m_View IsNot Nothing AndAlso Not m_View.Initialized Then
         If m_Bayer IsNot Nothing AndAlso m_Bayer.Initialized Then
            ' Set buffer to be viewed
            ' When using hardware bayer decoder, view the acquired RGB buffer,
            ' otherwise, for software bayer conversion, view the converted RGB buffer.
            Dim bayerBuffer As SapBuffer = m_Bayer.BayerBuffer
            If bayerBuffer IsNot Nothing AndAlso bayerBuffer.Initialized Then
               m_View.Buffer = bayerBuffer
            Else
               m_View.Buffer = m_Buffers
            End If
         End If

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
         m_Xfer.AutoEmpty = False
      End If

      ' Create processing object
      If m_Pro IsNot Nothing AndAlso Not m_Pro.Initialized Then
         If Not m_Pro.Create() Then
            DestroyObjects()
            Return False
         End If

         m_Pro.AutoEmpty = True
      End If
      Return True
   End Function

   Private Sub DestroyObjects()
      If m_Xfer IsNot Nothing AndAlso m_Xfer.Initialized Then
         m_Xfer.Destroy()
      End If
      If m_Pro IsNot Nothing AndAlso m_Pro.Initialized Then
         m_Pro.Destroy()
      End If
      If m_View IsNot Nothing AndAlso m_View.Initialized Then
         m_View.Destroy()
      End If
      If m_Bayer IsNot Nothing AndAlso m_Bayer.Initialized Then
         m_Bayer.Destroy()
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
      If m_Pro IsNot Nothing Then
         m_Pro.Dispose()
         m_Pro = Nothing
      End If
      If m_View IsNot Nothing Then
         m_View.Dispose()
         m_View = Nothing
         m_ImageBox.View = Nothing
      End If
      If m_Bayer IsNot Nothing Then
         m_Bayer.Dispose()
         m_Bayer = Nothing
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
   '				Window related functions
   '
   '**********************************************************************************
   Protected Overloads Overrides Sub OnResize(ByVal argsPaint As EventArgs)
      If m_ImageBox IsNot Nothing Then
         m_ImageBox.OnSize()
      End If
      MyBase.OnResize(argsPaint)
   End Sub

   Private Sub GigECameraDemoDlg_FormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs)
      DestroyObjects()
      DisposeObjects()
   End Sub

   '*****************************************************************************************
   '
   '					General Function
   '
   '*****************************************************************************************

   ' Updates the menu items enabling/disabling the proper items depending on the stateof the application
   Private Sub UpdateControls()

      Dim bAcqNoGrab As Boolean = m_Xfer IsNot Nothing AndAlso m_Xfer.Initialized AndAlso Not m_Xfer.Grabbing
      Dim bAcqGrab As Boolean = m_Xfer IsNot Nothing AndAlso m_Xfer.Initialized AndAlso m_Xfer.Grabbing
      Dim bNoGrab As Boolean = m_Xfer Is Nothing OrElse Not m_Xfer.Grabbing
      Dim isBayerEnabled As Boolean = m_Bayer IsNot Nothing AndAlso m_Bayer.Initialized AndAlso m_Bayer.Enabled
      Dim isBayerAvailable As Boolean = m_Acquisition IsNot Nothing AndAlso m_Acquisition.Initialized AndAlso m_Acquisition.BayerAvailable
      Dim isBayerSoftware As Boolean = m_Bayer IsNot Nothing AndAlso m_Bayer.Initialized AndAlso m_Bayer.SoftwareConversion

      ' Acquisition Control
      button_Grab.Enabled = bAcqNoGrab
      button_Freeze.Enabled = bAcqGrab
      button_Snap.Enabled = bAcqNoGrab

      ' Options
      button_LoadConfig.Enabled = bAcqNoGrab OrElse bNoGrab
      button_Buffers.Enabled = bNoGrab
      button_View.Enabled = bNoGrab

      ' File Options
      button_New.Enabled = bNoGrab
      button_Load_Raw.Enabled = bNoGrab AndAlso (isBayerSoftware OrElse Not isBayerEnabled)
      button_Save_Raw.Enabled = bNoGrab AndAlso (isBayerSoftware OrElse Not isBayerEnabled)
      button_Save_Converted.Enabled = bNoGrab AndAlso isBayerEnabled

      ' Bayer Options
      checkBox_Bayer_Conversion.Checked = isBayerEnabled
      checkBox_Bayer_Conversion.Enabled = bNoGrab

      checkBox_Hard_Conversion.Checked = isBayerAvailable AndAlso Not isBayerSoftware
      checkBox_Hard_Conversion.Enabled = isBayerAvailable AndAlso isBayerEnabled AndAlso bNoGrab

      ' Set selection for Bayer output format control
      If m_Bayer IsNot Nothing Then
         For i As Integer = 0 To m_BayerOutputFormatValues.Length - 1
            If m_BayerOutputFormatValues(i).Value = m_Bayer.OutputFormat Then
               comboBox_Output_Conversion.SelectedIndex = i
               Exit For
            End If
         Next
      End If
      comboBox_Output_Conversion.Enabled = bNoGrab
   End Sub


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
   '					General Function
   '
   '*****************************************************************************************

   Private Sub button_Exit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Exit.Click
      Application.[Exit]()
   End Sub

   Private Sub BayerDemoDlg_FormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs) Handles MyBase.FormClosed
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

   Private Sub BayerDemoDlg_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
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
   '					General Options
   '
   '*****************************************************************************************

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

   '*****************************************************************************************
   '
   '               File Options
   '
   '*****************************************************************************************

   Private Sub button_New_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_New.Click
      m_Buffers.Clear()
      Dim pBayerBuffer As SapBuffer = m_Bayer.BayerBuffer
      If pBayerBuffer IsNot Nothing AndAlso pBayerBuffer IsNot m_Buffers Then
         pBayerBuffer.Clear()
      End If
      m_ImageBox.Refresh()

   End Sub

   Private Sub button_Load_Raw_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Load_Raw.Click
      Dim newDialogLoad As New LoadSaveDlg(m_Buffers, True, False)
      If newDialogLoad.ShowDialog() = Windows.Forms.DialogResult.OK Then
         If m_Bayer.Enabled AndAlso m_Bayer.SoftwareConversion Then
            m_Pro.Execute()
         End If
      End If
      m_ImageBox.OnSize()
      newDialogLoad.Dispose()
   End Sub

   Private Sub button_Save_Raw_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Save_Raw.Click
      Dim newDialogSave As New LoadSaveDlg(m_Bayer.Buffer, False, False)
      ' Show the dialog and process the result
      newDialogSave.ShowDialog()
      newDialogSave.Dispose()

   End Sub

   Private Sub button_Save_Converted_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Save_Converted.Click
      Dim newDialogSave As New LoadSaveDlg(m_Bayer.BayerBuffer, False, False)
      ' Show the dialog and process the result
      newDialogSave.ShowDialog()
      newDialogSave.Dispose()
   End Sub

   '**************************************************************************************
   '
   '         Bayer Options
   '
   '**************************************************************************************

   Private Sub button_Options_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Options.Click
      Dim dlg As New BayerDlg(m_Bayer, m_Xfer, m_ImageBox, m_Pro)
      dlg.Show()
   End Sub

   Private Sub checkBox_Bayer_Conversion_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles checkBox_Bayer_Conversion.CheckedChanged
      DestroyObjects()
      CreateObjects()
      If m_Bayer IsNot Nothing AndAlso m_Bayer.Enabled AndAlso m_Bayer.SoftwareConversion Then
         m_Pro.Execute()
      End If
      UpdateControls()
   End Sub

   Private Sub checkBox_Hard_Conversion_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles checkBox_Hard_Conversion.CheckedChanged
      ' Destroy objects
      DestroyObjects()
      ' Recreate objects
      CreateObjects()
      UpdateControls()
   End Sub

   Private Sub comboBox_Output_Conversion_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles comboBox_Output_Conversion.SelectedIndexChanged
      ' Retrieve current Bayer decoder output format
      Dim bayerOutputFormat As SapFormat = m_Bayer.OutputFormat

      ' Before changing Bayer decoder output format, we have to destroy Sapera objects
      DestroyObjects()

      ' Set new Bayer decoder output format
      Dim selectFormat As SapFormatInfo = DirectCast(comboBox_Output_Conversion.SelectedItem, SapFormatInfo)
      m_Bayer.OutputFormat = selectFormat.Value

      ' Recreate objects
      If Not CreateObjects() Then
         ' If ever it fail, try to go back to previous state

         ' Set Bayer decoder output format to what it was before
         m_Bayer.OutputFormat = bayerOutputFormat
         CreateObjects()
      End If

      UpdateControls()
   End Sub

   ' Format table
   Public Class SapFormatInfo
      Public Sub New(ByVal format As SapFormat, ByVal name As String)
         m_Value = format
         m_Name = name
      End Sub

      Public Property Value() As SapFormat
         Get
            Return m_Value
         End Get
         Set(ByVal value As SapFormat)
            m_Value = value
         End Set
      End Property

      Public Overloads Overrides Function ToString() As String
         Return m_Name
      End Function

      Private m_Value As SapFormat
      Private m_Name As String
   End Class
End Class


