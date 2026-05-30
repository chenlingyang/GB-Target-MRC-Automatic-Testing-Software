Public Class MultiBoardSyncGrabDemoDlg
    Private m_Acquisition(2) As SapAcquisition
    Private m_Buffers(2) As SapBufferRoi
    Private m_Xfer(2) As SapAcqToBuf
    Private m_Buffer As SapBufferWithTrash
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
      Dim DualAcqDlg As MultiBoardSyncGrabDemoDlg = TryCast(argsNotify.Context, MultiBoardSyncGrabDemoDlg)
      ' If grabbing in trash buffer, do not display the image, update the
      ' appropriate number of frames on the status bar instead
      If argsNotify.Trash Then
         DualAcqDlg.Invoke(New DisplayFrameAcquired(AddressOf DualAcqDlg.ShowFrameNumber), argsNotify.EventCount, True)
      Else
         ' Refresh view
         DualAcqDlg.Invoke(New DisplayFrameAcquired(AddressOf DualAcqDlg.ShowFrameNumber), argsNotify.EventCount, False)
         DualAcqDlg.m_View.Show()
      End If
   End Sub

   Private Shared Sub GetSignalStatus(ByVal sender As Object, ByVal argsSignal As SapSignalNotifyEventArgs)
      Dim DualAcqDlg As MultiBoardSyncGrabDemoDlg = TryCast(argsSignal.Context, MultiBoardSyncGrabDemoDlg)
      Dim signalStatus As SapAcquisition.AcqSignalStatus = argsSignal.SignalStatus

      DualAcqDlg.m_IsSignalDetected = (signalStatus <> SapAcquisition.AcqSignalStatus.None)
      If DualAcqDlg.m_IsSignalDetected = False Then
         DualAcqDlg.StatusLabelInfo.Text = "Online... No camera signal detected"
      Else
         DualAcqDlg.StatusLabelInfo.Text = "Online... Camera signal detected"
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
      Me.m_ImageBox = New DALSA.SaperaLT.Demos.NET.VB.MultiBoardSyncGrabDemo.ImageBox
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

        Dim acConfigDlg0 As New AcqConfigDlg(Nothing, "", AcqConfigDlg.ServerCategory.ServerAcq)
        If acConfigDlg0.ShowDialog() = Windows.Forms.DialogResult.OK Then
            m_Acquisition(0) = New SapAcquisition(acConfigDlg0.ServerLocation, acConfigDlg0.ConfigFile)
            ' Create acquisition object
            If m_Acquisition(0) IsNot Nothing AndAlso Not m_Acquisition(0).Initialized Then
                If m_Acquisition(0).Create() = False Then
                    DestroyObjects()
                End If
            End If
        Else
            MessageBox.Show("No board found or selected !")
            Me.Close()
            Exit Sub
        End If

        Dim acConfigDlg1 As New AcqConfigDlg(Nothing, "", AcqConfigDlg.ServerCategory.ServerAcq)
        If acConfigDlg1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            m_Acquisition(1) = New SapAcquisition(acConfigDlg1.ServerLocation, acConfigDlg1.ConfigFile)
            If m_Acquisition(1) IsNot Nothing AndAlso Not m_Acquisition(1).Initialized Then
                If m_Acquisition(1).Create() = False Then
                    DestroyObjects()
                End If
            End If
        Else
            MessageBox.Show("No board found or selected !")
            Me.Close()
            Exit Sub
        End If

        m_online = True

        Dim acq0SupportSG, acq1SupportSG, acq0SupportSGP, acq1SupportSGP As Boolean
        acq0SupportSG = SapBuffer.IsBufferTypeSupported(m_Acquisition(0).Location, SapBuffer.MemoryType.ScatterGather)
        acq1SupportSG = SapBuffer.IsBufferTypeSupported(m_Acquisition(1).Location, SapBuffer.MemoryType.ScatterGather)

        If (Not acq0SupportSG Or Not acq1SupportSG) Then
            acq0SupportSGP = SapBuffer.IsBufferTypeSupported(m_Acquisition(0).Location, SapBuffer.MemoryType.ScatterGatherPhysical)
            acq1SupportSGP = SapBuffer.IsBufferTypeSupported(m_Acquisition(1).Location, SapBuffer.MemoryType.ScatterGatherPhysical)
            If (Not (Not acq0SupportSG And Not acq1SupportSG And acq0SupportSGP And acq1SupportSGP)) Then
                Dim message As String
                message = [String].Format("The chosen acquisition devices{0}{0}-{1}{0}-{2}{0}{0}do not support similar buffer types.", System.Environment.NewLine, m_Acquisition(0).Location.ServerName, m_Acquisition(1).Location.ServerName)
                MessageBox.Show(message, "Buffer Type Error")
                m_online = False
            End If
        End If



        If Not CreateNewObjects(acConfigDlg1, False) Then
            Me.Close()
        End If
    End Sub

   Private Sub ShowFrameNumber(ByVal number As Integer, ByVal trash As Boolean)
      Dim str As String
      If trash Then
            str = [String].Format("Frames acquired in trash buffer: {0}", number * m_Buffer.Count)
         Me.StatusLabelInfoTrash.Text = str
      Else
            str = [String].Format("Frames acquired :{0}", number * m_Buffer.Count)
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

            
         If SapBuffer.IsBufferTypeSupported(m_ServerLocation, SapBuffer.MemoryType.ScatterGather) Then
                m_Buffer = New SapBufferWithTrash()
                m_Buffers(0) = New SapBufferRoi(m_Buffer)
                m_Buffers(1) = New SapBufferRoi(m_Buffer)
         Else
                m_Buffer = New SapBufferWithTrash()
                m_Buffers(0) = New SapBufferRoi(m_Buffer)
                m_Buffers(1) = New SapBufferRoi(m_Buffer)
            End If

            m_Xfer(0) = New SapAcqToBuf(m_Acquisition(0), m_Buffers(0))
            m_Xfer(1) = New SapAcqToBuf(m_Acquisition(1), m_Buffers(1))

            m_View = New SapView(m_Buffer)


            'event for view
            m_Xfer(0).Pairs(0).EventType = SapXferPair.XferEventType.EndOfFrame
            AddHandler m_Xfer(0).XferNotify, AddressOf xfer_XferNotify
            m_Xfer(0).XferNotifyContext = Me

            m_Xfer(1).Pairs(0).EventType = SapXferPair.XferEventType.EndOfFrame
            AddHandler m_Xfer(1).XferNotify, AddressOf xfer_XferNotify
            m_Xfer(1).XferNotifyContext = Me


            'event for signal status
            AddHandler m_Acquisition(1).SignalNotify, AddressOf GetSignalStatus
            m_Acquisition(1).SignalNotifyContext = Me
        Else
            m_Buffer = New SapBufferWithTrash()
            m_View = New SapView(m_Buffer)
            StatusLabelInfo.Text = "Offline...Please load 2 acquisition devices"
        End If



      m_ImageBox.View = m_View
        m_View.SetScalingMode(0.6F, 0.6F)
        If Not CreateObjects() Then
            DisposeObjects()
            Return False
        End If


        m_ImageBox.OnSize()
        EnableSignalStatus()
        UpdateControls()
        Return True
    End Function

    ' Call Create method  
    Private Function CreateObjects() As Boolean
        If (m_online) Then
            m_Buffer.Count = 1
            m_Buffer.Width = 2 * m_Acquisition(0).XferParams.Width
            m_Buffer.Height = m_Acquisition(0).XferParams.Height
            m_Buffer.Format = m_Acquisition(0).XferParams.Format
            m_Buffer.PixelDepth = m_Acquisition(0).XferParams.PixelDepth
        End If
        ' Create buffer object
        If m_Buffer IsNot Nothing AndAlso Not m_Buffer.Initialized Then
            If m_Buffer.Create() = False Then
                DestroyObjects()
                Return False
            End If
            m_Buffer.Clear()
        End If

        If (m_online) Then
            m_Buffers(0).SetRoi(0, 0, m_Acquisition(0).XferParams.Width, m_Acquisition(0).XferParams.Height)
            If m_Buffers(0) IsNot Nothing AndAlso Not m_Buffers(0).Initialized Then
                If m_Buffers(0).Create() = False Then
                    DestroyObjects()
                    Return False
                End If
            End If

            m_Buffers(1).SetRoi(m_Acquisition(0).XferParams.Width, 0, m_Acquisition(0).XferParams.Width, m_Acquisition(0).XferParams.Height)
            If m_Buffers(1) IsNot Nothing AndAlso Not m_Buffers(1).Initialized Then
                If m_Buffers(1).Create() = False Then
                    DestroyObjects()
                    Return False
                End If
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
        If m_Xfer(0) IsNot Nothing AndAlso Not m_Xfer(0).Initialized Then
            If m_Xfer(0).Create() = False Then
                DestroyObjects()
                Return False
            End If
        End If

        If m_Xfer(1) IsNot Nothing AndAlso Not m_Xfer(1).Initialized Then
            If m_Xfer(1).Create() = False Then
                DestroyObjects()
                Return False
            End If
        End If
        Return True
    End Function

    Private Sub DestroyObjects()
        'stop grabbing before destroying objects
        If m_Xfer(0) IsNot Nothing AndAlso m_Xfer(0).Grabbing Then
            m_Xfer(0).Abort()
        End If

        If m_Xfer(1) IsNot Nothing AndAlso m_Xfer(1).Grabbing Then
            m_Xfer(1).Abort()
        End If


        If m_Xfer(0) IsNot Nothing AndAlso m_Xfer(0).Initialized Then
            m_Xfer(0).Destroy()
        End If
        If m_Xfer(1) IsNot Nothing AndAlso m_Xfer(1).Initialized Then
            m_Xfer(1).Destroy()
        End If

        If m_View IsNot Nothing AndAlso m_View.Initialized Then
            m_View.Destroy()
        End If

        If m_Buffers(0) IsNot Nothing AndAlso m_Buffers(0).Initialized Then
            m_Buffers(0).Destroy()
        End If
        If m_Buffers(1) IsNot Nothing AndAlso m_Buffers(1).Initialized Then
            m_Buffers(1).Destroy()
        End If

        If m_Buffer IsNot Nothing AndAlso m_Buffer.Initialized Then
            m_Buffer.Destroy()
        End If

        If m_Acquisition(0) IsNot Nothing AndAlso m_Acquisition(0).Initialized Then
            m_Acquisition(0).Destroy()
        End If
        If m_Acquisition(1) IsNot Nothing AndAlso m_Acquisition(1).Initialized Then
            m_Acquisition(1).Destroy()
        End If


    End Sub

    Private Sub DisposeObjects()

        If m_Xfer(1) IsNot Nothing Then
            m_Xfer(1).Dispose()
        End If
        If m_Xfer(0) IsNot Nothing Then
            m_Xfer(0).Dispose()
            m_Xfer = Nothing
        End If

        If m_View IsNot Nothing Then
            m_View.Dispose()
         m_View = Nothing
         m_ImageBox.View = Nothing
        End If

        If m_Buffers(1) IsNot Nothing Then
            m_Buffers(1).Dispose()
        End If
        If m_Buffers(0) IsNot Nothing Then
            m_Buffers(0).Dispose()
            m_Buffers = Nothing
        End If
        If m_Buffer IsNot Nothing Then
            m_Buffer.Dispose()
            m_Buffer = Nothing
        End If
        If m_Acquisition(1) IsNot Nothing Then
            m_Acquisition(1).Dispose()
        End If
        If m_Acquisition(0) IsNot Nothing Then
            m_Acquisition(0).Dispose()
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

    Private Sub MultiBoardSyncGrabDemoDlg_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        DestroyObjects()
        DisposeObjects()
    End Sub

    '*****************************************************************************************
    ' 
    '					File Control
    '
    '*****************************************************************************************

    Private Sub Button_New_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_New.Click
        m_Buffer.Clear()
      m_ImageBox.Refresh()
    End Sub

    '*****************************************************************************************
    '  
    ' 					General Options
    ' 
    '*****************************************************************************************

    Private Sub Button_View_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_View.Click
      Dim viewDialog As New ViewDlg(m_View, m_ImageBox.ViewRectangle)
        If viewDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
         m_ImageBox.OnSize()
        End If
        m_ImageBox.Refresh()
    End Sub

    '*****************************************************************************************
    '          
    '				General Function
    ' 
    '*****************************************************************************************

    Private Sub UpdateControls()
        Dim bAcqNoGrab As Boolean = (m_Xfer(0) IsNot Nothing) AndAlso (m_Xfer(0).Grabbing = False)
        Dim bAcqGrab As Boolean = (m_Xfer(0) IsNot Nothing) AndAlso (m_Xfer(0).Grabbing = True)
        Dim bNoGrab As Boolean = (m_Xfer(0) Is Nothing) OrElse (m_Xfer(0).Grabbing = False)
        ' Acquisition Control
        Button_Grab.Enabled = bAcqNoGrab AndAlso m_online
        Button_Snap.Enabled = bAcqNoGrab AndAlso m_online
        Button_Freeze.Enabled = bAcqGrab AndAlso m_online

        ' File Options
        Button_New.Enabled = bNoGrab

    End Sub

    Private Sub EnableSignalStatus()
        If m_Acquisition(0) IsNot Nothing Then
            m_IsSignalDetected = (m_Acquisition(0).SignalStatus <> SapAcquisition.AcqSignalStatus.None)
            If m_IsSignalDetected = False Then
                StatusLabelInfo.Text = "Offline... No camera signal detected"
            Else
                StatusLabelInfo.Text = "Online... Camera signal detected"
            End If
            m_Acquisition(0).SignalNotifyEnable = True
        End If
    End Sub

    Private Sub Button_Exit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Exit.Click
        Application.Exit()
    End Sub

    '*****************************************************************************************
    '          
    '				Acquisition Control
    ' 
    '*****************************************************************************************

    Private Sub Button_Snap_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Snap.Click
        Dim abort0 As AbortDlg = New AbortDlg(m_Xfer(0))
        Dim abort1 As AbortDlg = New AbortDlg(m_Xfer(1))
        If m_Xfer(0).Snap AndAlso m_Xfer(1).Snap Then
            If abort0.ShowDialog() <> Windows.Forms.DialogResult.OK AndAlso abort1.ShowDialog() <> Windows.Forms.DialogResult.OK Then
                m_Xfer(0).Abort()
                m_Xfer(1).Abort()
            End If
            UpdateControls()
        End If
    End Sub

    Private Sub Button_Grab_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Grab.Click
        If m_Xfer(0).Grab() AndAlso m_Xfer(1).Grab() Then
            UpdateControls()
        End If
    End Sub

    Private Sub Button_Freeze_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Freeze.Click
        Dim abort0 As AbortDlg = New AbortDlg(m_Xfer(0))
        Dim abort1 As AbortDlg = New AbortDlg(m_Xfer(1))
        If m_Xfer(0).Freeze AndAlso m_Xfer(1).Freeze Then
            If abort0.ShowDialog() <> Windows.Forms.DialogResult.OK AndAlso abort1.ShowDialog() <> Windows.Forms.DialogResult.OK Then
                m_Xfer(0).Abort()
                m_Xfer(1).Abort()
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

    Private Sub MultiBoardSyncGrabDemoDlg_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
