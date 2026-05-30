Public Class GigECAmeraDemoDlg

    Private m_AcqDevice As SapAcqDevice
    Private m_Buffers As SapBuffer
    Private m_Xfer As SapAcqDeviceToBuf
    Private m_View As SapView
    Private m_ServerLocation As SapLocation
    Private M_ConfigFileName As String
    Private m_SystemMenu As SystemMenu
    'index for "about this.." item im system menu
    Const m_AboutID As Integer = &H100

   ' Delegate to display number of frame acquired 
   ' Delegate is needed because .NEt framework does not support cross thread control modification
   Private Delegate Sub DisplayFrameAcquired(ByVal number As Integer, ByVal trash As Boolean)
   '
   ' This function is called each time an image has been transferred into system memory by the transfer object
   '
   Private Shared Sub xfer_XferNotify(ByVal sender As Object, ByVal argsNotify As SapXferNotifyEventArgs)
      Dim GigeDlg As GigECameraDemoDlg = TryCast(argsNotify.Context, GigECameraDemoDlg)
      ' If grabbing in trash buffer, do not display the image, update the
      ' appropriate number of frames on the status bar instead
      If argsNotify.Trash Then
         GigeDlg.Invoke(New DisplayFrameAcquired(AddressOf GigeDlg.ShowFrameNumber), argsNotify.EventCount, True)
      Else
         GigeDlg.Invoke(New DisplayFrameAcquired(AddressOf GigeDlg.ShowFrameNumber), argsNotify.EventCount, False)
         GigeDlg.m_View.Show()
      End If
   End Sub

    Private Sub SystemEvents_SessionEnded(ByVal sender As Object, ByVal args As SessionEndedEventArgs)
        ' The FormClosed event is not invoked when logging off or shutting down,
        ' so we need to clean up here too.
        DestroyObjects()
        DisposeObjects()
    End Sub
    Public Sub New()
        m_AcqDevice = Nothing
        m_Buffers = Nothing
        m_Xfer = Nothing
        m_View = Nothing

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
            Me.m_ImageBox = New DALSA.SaperaLT.Demos.NET.VB.GigECameraDemo.ImageBox
            Me.m_ImageBox.Location = New System.Drawing.Point(211, 3)
            Me.m_ImageBox.Name = "m_ImageBox"
            Me.m_ImageBox.PixelValueDisplay = Me.PixelDataValue
            Me.m_ImageBox.Size = New System.Drawing.Size(386, 406)
            Me.m_ImageBox.SliderEnable = False
            Me.m_ImageBox.SliderMaximum = 10
            Me.m_ImageBox.SliderMinimum = 0
            Me.m_ImageBox.SliderValue = 0
            Me.m_ImageBox.SliderVisible = False
            Me.m_ImageBox.TabIndex = 4
            Me.m_ImageBox.TrackerEnable = False
            Me.m_ImageBox.View = Nothing
            Me.Controls.Add(Me.m_ImageBox)

            If Not CreateNewObjects(acConfigDlg, False) Then
                Me.Close()
            End If
        Else
            MessageBox.Show("No cameras found or selected")
            Me.Close()
        End If
    End Sub

   Private Sub ShowFrameNumber(ByVal number As Integer, ByVal trash As Boolean)
      Dim str As String
      If trash Then
         str = [String].Format("Frames acquired in trash buffer: {0}", number * Me.m_Buffers.Count)
         Me.StatusLabelInfoTrash.Text = str
      Else
         str = [String].Format("Frames acquired :{0}", number * Me.m_Buffers.Count)
         Me.StatusLabelInfo.Text = str
      End If
   End Sub


    '*****************************************************************************************
    '
    '					Create and Destroy Object
    '
    '*****************************************************************************************

    Private Function CreateNewObjects(ByVal acConfigDlg As AcqConfigDlg, ByVal Restore As Boolean) As Boolean

        If Not Restore Then
            m_ServerLocation = acConfigDlg.ServerLocation
            M_ConfigFileName = acConfigDlg.ConfigFile
        End If

        m_AcqDevice = New SapAcqDevice(m_ServerLocation, M_ConfigFileName)
        If SapBuffer.IsBufferTypeSupported(m_ServerLocation, SapBuffer.MemoryType.ScatterGather) Then
            m_Buffers = New SapBufferWithTrash(2, m_AcqDevice, SapBuffer.MemoryType.ScatterGather)
        Else
            m_Buffers = New SapBufferWithTrash(2, m_AcqDevice, SapBuffer.MemoryType.ScatterGatherPhysical)
        End If
        m_Xfer = New SapAcqDeviceToBuf(m_AcqDevice, m_Buffers)
      m_View = New SapView(m_Buffers)
      m_ImageBox.View = m_View



        m_Xfer.Pairs(0).EventType = SapXferPair.XferEventType.EndOfFrame
        AddHandler m_Xfer.XferNotify, AddressOf xfer_XferNotify
        m_Xfer.XferNotifyContext = Me
        StatusLabelInfo.Text = "Online... Waiting grabbed images"

        If Not CreateObjects() Then
            DisposeObjects()
            Return False
        End If

      m_ImageBox.OnSize()
        UpdateControls()
        Return True
    End Function

    Private Function CreateNewObjects(ByVal dlg As BufDlg) As Boolean
        m_AcqDevice = New SapAcqDevice(m_ServerLocation, M_ConfigFileName)
        If dlg.Count > 1 Then
            m_Buffers = New SapBufferWithTrash(dlg.Count, m_AcqDevice, dlg.Type)
        Else
            m_Buffers = New SapBuffer(1, m_AcqDevice, dlg.Type)
        End If

        m_Xfer = New SapAcqDeviceToBuf(m_AcqDevice, m_Buffers)
      m_View = New SapView(m_Buffers)
      m_ImageBox.View = m_View

        m_Xfer.Pairs(0).EventType = SapXferPair.XferEventType.EndOfFrame
        AddHandler m_Xfer.XferNotify, AddressOf xfer_XferNotify
        m_Xfer.XferNotifyContext = Me
        StatusLabelInfo.Text = "Online... Waiting grabbed images"

        If CreateObjects() = False Then
            DisposeObjects()
            Return False
        End If

      m_ImageBox.OnSize()
        UpdateControls()
        Return True
    End Function

    Private Function CreateObjects() As Boolean
        ' Create acquisition object
        If m_AcqDevice IsNot Nothing AndAlso Not m_AcqDevice.Initialized Then
            If m_AcqDevice.Create() = False Then
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

        If m_Xfer IsNot Nothing AndAlso m_Xfer.Pairs(0) IsNot Nothing Then
            m_Xfer.Pairs(0).Cycle = SapXferPair.CycleMode.NextWithTrash
            If m_Xfer.Pairs(0).Cycle <> SapXferPair.CycleMode.NextWithTrash Then
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
        If m_Xfer IsNot Nothing AndAlso m_Xfer.Initialized Then
            m_Xfer.Destroy()
        End If
        If m_View IsNot Nothing AndAlso m_View.Initialized Then
            m_View.Destroy()
        End If
        If m_Buffers IsNot Nothing AndAlso m_Buffers.Initialized Then
            m_Buffers.Destroy()
        End If
        If m_AcqDevice IsNot Nothing AndAlso m_AcqDevice.Initialized Then
            m_AcqDevice.Destroy()
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
        If m_AcqDevice IsNot Nothing Then
            m_AcqDevice.Dispose()
            m_AcqDevice = Nothing
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

    Private Sub GigECAmeraDemoDlg_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
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

    Private Sub Button_Load_Config_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Load_Config.Click
        ' Set new acquisition parameters
        Dim acConfigDlg As New AcqConfigDlg(Nothing, "", AcqConfigDlg.ServerCategory.ServerAcqDevice)
        If acConfigDlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            DestroyObjects()
            DisposeObjects()

            ' Update objects with new acquisition
            If Not CreateNewObjects(acConfigDlg,False) Then
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

    Private Sub Button_Buffer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Buffer.Click
        Dim dlg As BufDlg = New BufDlg(m_Buffers, m_View.Display, True)
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
    End Sub

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
        Dim bAcqNoGrab As Boolean = (m_Xfer IsNot Nothing) AndAlso (m_Xfer.Grabbing = False)
        Dim bAcqGrab As Boolean = (m_Xfer IsNot Nothing) AndAlso (m_Xfer.Grabbing = True)
        Dim bNoGrab As Boolean = (m_Xfer Is Nothing) OrElse (m_Xfer.Grabbing = False)

        ' Acquisition Control
        Button_Grab.Enabled = bAcqNoGrab
        Button_Snap.Enabled = bAcqNoGrab
        Button_Freeze.Enabled = bAcqGrab

        ' File Options
        Button_New.Enabled = bNoGrab
        Button_Load.Enabled = bNoGrab
        Button_Save.Enabled = bNoGrab

        Button_Load_Config.Enabled = bAcqNoGrab
        Button_Buffer.Enabled = bNoGrab
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

    Private Sub GigECAmeraDemoDlg_Load_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
