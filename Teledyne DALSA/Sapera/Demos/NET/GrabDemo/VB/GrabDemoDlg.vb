Public Class GrabDemoDlg
    Private m_Acquisition As SapAcquisition
    Private m_Buffers As SapBuffer
    Private m_Xfer As SapAcqToBuf
    Private m_View As SapView
    Private m_ServerLocation As SapLocation
    Private m_ConfigFileName As String
    Private m_IsSignalDetected As Boolean
    Private m_online As Boolean

    Private m_SystemMenu As SystemMenu
    'index for "about this.." item im system menu
    Const m_AboutID As Integer = &H100


   ' Delegate to display number of frame acquired 
   ' Delegate is needed because .NEt framework does not support cross thread control modification
   Private Delegate Sub DisplayFrameAcquired(ByVal number As Integer, ByVal trash As Boolean)

   Private Shared Sub xfer_XferNotify(ByVal sender As Object, ByVal argsNotify As SapXferNotifyEventArgs)
      Dim GrabDlg As GrabDemoDlg = TryCast(argsNotify.Context, GrabDemoDlg)
      ' If grabbing in trash buffer, do not display the image, update the
      ' appropriate number of frames on the status bar instead
      If argsNotify.Trash Then
         GrabDlg.Invoke(New DisplayFrameAcquired(AddressOf GrabDlg.ShowFrameNumber), argsNotify.EventCount, True)
      Else
         ' Refresh view
         GrabDlg.Invoke(New DisplayFrameAcquired(AddressOf GrabDlg.ShowFrameNumber), argsNotify.EventCount, False)
         GrabDlg.m_View.Show()
      End If
   End Sub

   Private Shared Sub GetSignalStatus(ByVal sender As Object, ByVal argsSignal As SapSignalNotifyEventArgs)
      Dim GrabDlg As GrabDemoDlg = TryCast(argsSignal.Context, GrabDemoDlg)
      Dim signalStatus As SapAcquisition.AcqSignalStatus = argsSignal.SignalStatus

      GrabDlg.m_IsSignalDetected = (signalStatus <> SapAcquisition.AcqSignalStatus.None)
      If GrabDlg.m_IsSignalDetected = False Then
         GrabDlg.StatusLabelInfo.Text = "Online... No camera signal detected"
      Else
         GrabDlg.StatusLabelInfo.Text = "Online... Camera signal detected"
      End If
   End Sub

    Private Sub SystemEvents_SessionEnded(ByVal sender As Object, ByVal args As SessionEndedEventArgs)
        ' The FormClosed event is not invoked when logging off or shutting down,
        ' so we need to clean up here too.
        DestroyObjects()
        DisposeObjects()
    End Sub
    ' Constructor
   Public Sub New()
      m_Acquisition = Nothing
      m_Buffers = Nothing
      m_Xfer = Nothing
      m_View = Nothing
      m_IsSignalDetected = True

      InitializeComponent()

      ' Note:
      '  The code to initialize m_ImageBox was originally in the InitializeComponent function
      '  called above. However, it has been moved to the dialog constructor as a workaround
      '  to a Visual Studio Designer error when loading the DALSA.SaperaLT.SapClassBasic
      '  assembly under 64-bit Windows.
      '  As a consequence, it is not possible to adjust the m_ImageBox properties
      '  automatically using the Designer anymore, this has to be done manually.
      ' 
      Me.m_ImageBox = New DALSA.SaperaLT.Demos.NET.VB.GrabDemo.ImageBox
      Me.m_ImageBox.Location = New System.Drawing.Point(214, 4)
      Me.m_ImageBox.Name = "m_ImageBox"
      Me.m_ImageBox.PixelValueDisplay = Me.PixelDataValue
      Me.m_ImageBox.Size = New System.Drawing.Size(386, 406)
      Me.m_ImageBox.SliderEnable = True
      Me.m_ImageBox.SliderMaximum = 10
      Me.m_ImageBox.SliderMinimum = 0
      Me.m_ImageBox.SliderValue = 0
      Me.m_ImageBox.SliderVisible = False
      Me.m_ImageBox.TabIndex = 10
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
   End Sub

   Private Sub ShowFrameNumber(ByVal number As Integer, ByVal trash As Boolean)
      Dim str As String
      If trash Then
         str = [String].Format("Frames acquired in trash buffer: {0}", number * m_Buffers.Count)
         Me.StatusLabelInfoTrash.Text = str
      Else
         str = [String].Format("Frames acquired :{0}", number * m_Buffers.Count)
         Me.StatusLabelInfo.Text = str
      End If
   End Sub


    '*****************************************************************************************
    '
    '					Create and Destroy Object
    '
    '*****************************************************************************************

    ' Create new object with acquisition information 
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
         m_View = New SapView(m_Buffers)


            'event for view
            m_Xfer.Pairs(0).EventType = SapXferPair.XferEventType.EndOfFrame
            AddHandler m_Xfer.XferNotify, AddressOf xfer_XferNotify
            m_Xfer.XferNotifyContext = Me

            'event for signal status
            AddHandler m_Acquisition.SignalNotify, AddressOf GetSignalStatus
            m_Acquisition.SignalNotifyContext = Me
        Else
            'define off-line object
            m_Buffers = New SapBuffer()
         m_View = New SapView(m_Buffers)
            StatusLabelInfo.Text = "offline... Load images"
      End If

      m_ImageBox.View = m_View

        If Not CreateObjects() Then
            DisposeObjects()
            Return False
        End If

      m_ImageBox.OnSize()
        EnableSignalStatus()
        UpdateControls()
        Return True
    End Function

    ' Create new object with Buffer information
    Public Function CreateNewObjects(ByVal BufDlg As BufDlg) As Boolean
        If m_online Then
            ' define on-line object
            m_Acquisition = New SapAcquisition(m_ServerLocation, m_ConfigFileName)
            If BufDlg.Count > 1 Then
                m_Buffers = New SapBufferWithTrash(BufDlg.Count, m_Acquisition, BufDlg.Type)
            Else
                m_Buffers = New SapBuffer(1, m_Acquisition, BufDlg.Type)
            End If

            m_Xfer = New SapAcqToBuf(m_Acquisition, m_Buffers)
         m_View = New SapView(m_Buffers)

            'event for view
            m_Xfer.Pairs(0).EventType = SapXferPair.XferEventType.EndOfFrame
            AddHandler m_Xfer.XferNotify, AddressOf xfer_XferNotify
            m_Xfer.XferNotifyContext = Me
        Else
            'define off-line object
            m_Buffers = New SapBuffer(BufDlg.Count, BufDlg.BWidth, BufDlg.BHeight, BufDlg.Format, BufDlg.Type)
         m_View = New SapView(m_Buffers)
            StatusLabelInfo.Text = "offline... Load images"
        End If
      m_ImageBox.View = m_View

        If Not CreateObjects() Then
            DisposeObjects()
            Return False
        End If

      m_ImageBox.OnSize()
      EnableSignalStatus()
      Return True
    End Function

    ' Call Create method  
    Private Function CreateObjects() As Boolean
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
            If m_Xfer.Create() = False Then
                DestroyObjects()
                Return False
            End If
        End If
        Return True
    End Function

    Private Sub DestroyObjects()
        'Call Destroy method
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
    ' 				Window related functions
    ' 
    '**********************************************************************************

    Protected Overloads Overrides Sub OnResize(ByVal argsPaint As EventArgs)
      If m_ImageBox IsNot Nothing Then
         m_ImageBox.OnSize()
      End If
        MyBase.OnResize(argsPaint)
    End Sub

    Private Sub GrabDemoDlg_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        DestroyObjects()
        DisposeObjects()
    End Sub

    '*****************************************************************************************
    ' 
    '					File Control
    '
    '*****************************************************************************************

    Private Sub Button_New_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_New.Click
        m_Buffers.Clear()
      m_ImageBox.Refresh()
    End Sub

    Private Sub Button_Load_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Load.Click
        Dim newDialogLoad As LoadSaveDlg = New LoadSaveDlg(m_Buffers, True, False)
        ' Show the dialog and process the result
        If newDialogLoad.ShowDialog() = Windows.Forms.DialogResult.OK Then
            m_View.Show()
        End If
        newDialogLoad.Dispose()
        m_ImageBox.Refresh()
    End Sub

    Private Sub Button_Save_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Save.Click
        Dim newDialogSave As LoadSaveDlg = New LoadSaveDlg(m_Buffers, False, False)
        ' Show the dialog and process the result
        newDialogSave.ShowDialog()
        newDialogSave.Dispose()
      m_ImageBox.Refresh()
    End Sub


    '*****************************************************************************************
    '  
    ' 					General Options
    ' 
    '*****************************************************************************************

    Private Sub Button_Buffer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Buffer.Click
        ' Set new buffer parameters
        Dim dlg As New BufDlg(m_Buffers, m_View.Display, m_online)
        If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            DestroyObjects()          
            DisposeObjects()

            ' Update objects with new buffer
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

    Private Sub Button_View_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_View.Click
      Dim viewDialog As New ViewDlg(m_View, m_ImageBox.ViewRectangle)
        If viewDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
         m_ImageBox.OnSize()
        End If
        m_ImageBox.Refresh()
    End Sub

    Private Sub Button_Load_Config_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Load_Config.Click
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
    '				General Function
    ' 
    '*****************************************************************************************

    Private Sub UpdateControls()
        Dim bAcqNoGrab As Boolean = (m_Xfer IsNot Nothing) AndAlso (m_Xfer.Grabbing = False)
        Dim bAcqGrab As Boolean = (m_Xfer IsNot Nothing) AndAlso (m_Xfer.Grabbing = True)
        Dim bNoGrab As Boolean = (m_Xfer Is Nothing) OrElse (m_Xfer.Grabbing = False)
        ' Acquisition Control
        Button_Grab.Enabled = bAcqNoGrab AndAlso m_online
        Button_Snap.Enabled = bAcqNoGrab AndAlso m_online
        Button_Freeze.Enabled = bAcqGrab AndAlso m_online

        ' File Options
        Button_New.Enabled = bNoGrab
        Button_Load.Enabled = bNoGrab
        Button_Save.Enabled = bNoGrab

        Button_Load_Config.Enabled = bAcqNoGrab OrElse bNoGrab
        Button_Buffer.Enabled = bNoGrab
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

    Private Sub Button_Exit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Exit.Click
        DestroyObjects()
        DisposeObjects()
        Application.Exit()
    End Sub

    '*****************************************************************************************
    '          
    '				Acquisition Control
    ' 
    '*****************************************************************************************

    Private Sub Button_Snap_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Snap.Click
        Dim abort As AbortDlg = New AbortDlg(m_Xfer)

        If m_Xfer.Snap() Then
            If abort.ShowDialog() <> Windows.Forms.DialogResult.OK Then
                m_Xfer.Abort()
            End If
            UpdateControls()
        End If
    End Sub

    Private Sub Button_Grab_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Grab.Click
        If m_Xfer.Grab() Then
            UpdateControls()
        End If
    End Sub

    Private Sub Button_Freeze_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Freeze.Click
        Dim abort As AbortDlg = New AbortDlg(m_Xfer)

        If m_Xfer.Freeze Then
            If abort.ShowDialog() <> Windows.Forms.DialogResult.OK Then
                m_Xfer.Abort()
            End If
            UpdateControls()
        End If
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

    Private Sub GrabDemoDlg_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
