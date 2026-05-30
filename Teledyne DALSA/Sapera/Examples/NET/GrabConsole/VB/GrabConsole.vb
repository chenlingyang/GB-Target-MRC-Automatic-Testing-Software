Module GrabConsole

   Dim lastFrameRate As Single = 0.0F


   Sub xfer_XferNotify(ByVal sender As Object, ByVal args As SapXferNotifyEventArgs)

      ' refresh view
      Dim View As SapView = TryCast(args.Context, SapView)
      View.Show()

      ' refresh frame rate
      Dim Transfer As SapTransfer = TryCast(sender, SapTransfer)
      If Transfer.UpdateFrameRateStatistics() Then
         Dim Stats As SapXferFrameRateInfo = Transfer.FrameRateStatistics
         Dim framerate As Single = 0.0F

         If Stats.IsLiveFrameRateAvailable Then
            framerate = Stats.LiveFrameRate
         End If

         If Stats.IsLiveFrameRateStalled Then
            ' check if frame rate is stalled
            Console.WriteLine("Live Frame rate is stalled.")
         ElseIf (framerate > 0.0F) And (Math.Abs(lastFrameRate - framerate) > 0.1F) Then
            ' update FPS only if the value changed by +/- 0.1
            Console.WriteLine("Grabbing at {0} frames/sec", framerate)
            lastFrameRate = framerate
         End If

      End If

   End Sub

   Sub Main(ByVal args As String())

      Dim Acq As SapAcquisition = Nothing
      Dim AcqDevice As SapAcqDevice = Nothing
      Dim Buffers As SapBuffer = Nothing
      Dim Xfer As SapTransfer = Nothing
      Dim View As SapView = Nothing

      Console.WriteLine("Sapera Console Grab Example (VB.NET version)" & Chr(10) & "")

      Dim acqParams As New MyAcquisitionParams()

      ' Call GetOptions to determine which acquisition device to use and which config
      ' file (CCF) should be loaded to configure it.
      If Not GetOptions(args, acqParams) Then
         Console.WriteLine("" & Chr(10) & "Press any key to terminate" & Chr(10) & "")
         Console.ReadKey(True)
         Return
      End If

      Dim loc As New SapLocation(acqParams.ServerName, acqParams.ResourceIndex)

      If SapManager.GetResourceCount(acqParams.ServerName, SapManager.ResourceType.Acq) > 0 Then
         Acq = New SapAcquisition(loc, acqParams.ConfigFileName)
         Buffers = New SapBufferWithTrash(2, Acq, SapBuffer.MemoryType.ScatterGather)
         Xfer = New SapAcqToBuf(Acq, Buffers)

         ' Create acquisition object
         If Not Acq.Create() Then
            Console.WriteLine("Error during SapAcquisition creation!" & Chr(10) & "")
            DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View)
            Return
         End If
         Acq.EnableEvent(SapAcquisition.AcqEventType.StartOfFrame)

        ElseIf SapManager.GetResourceCount(acqParams.ServerName, SapManager.ResourceType.AcqDevice) > 0 Then
            AcqDevice = New SapAcqDevice(loc, acqParams.ConfigFileName)
            Buffers = New SapBufferWithTrash(2, AcqDevice, SapBuffer.MemoryType.ScatterGather)
            Xfer = New SapAcqDeviceToBuf(AcqDevice, Buffers)

            ' Create acquisition object
            If Not AcqDevice.Create() Then
                Console.WriteLine("Error during SapAcqDevice creation!" & Chr(10) & "")
                DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View)
                Return
            End If
        End If

        View = New SapView(Buffers)
        ' End of frame event
        Xfer.Pairs(0).EventType = SapXferPair.XferEventType.EndOfFrame
        AddHandler Xfer.XferNotify, AddressOf xfer_XferNotify
        Xfer.XferNotifyContext = View

        ' Create buffer object
        If Not Buffers.Create() Then
            Console.WriteLine("Error during SapBuffer creation!" & Chr(10) & "")
            DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View)
            Return
        End If

        ' Create buffer object
        If Not Xfer.Create() Then
            Console.WriteLine("Error during SapTransfer creation!" & Chr(10) & "")
            DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View)
            Return
        End If

        ' Create buffer object
        If Not View.Create() Then
            Console.WriteLine("Error during SapView creation!" & Chr(10) & "")
            DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View)
            Return
        End If


        Xfer.Grab()
        Console.WriteLine("" & Chr(10) & "" & Chr(10) & "Grab started, press a key to freeze")
        Console.ReadKey(True)
        Xfer.Freeze()
        Xfer.Wait(1000)

        DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View)
        loc.Dispose()
   End Sub


   Function GetOptions(ByVal args As String(), ByVal acqParams As MyAcquisitionParams) As Boolean
      ' Check if arguments were passed
      If args.Length > 1 Then
         Return ExampleUtils.GetOptionsFromCommandLine(args, acqParams)
      Else
         Return ExampleUtils.GetOptionsFromQuestions(acqParams)
      End If
   End Function


   Sub DestroysObjects(ByVal acq As SapAcquisition, ByVal camera As SapAcqDevice, ByVal buf As SapBuffer, ByVal xfer As SapTransfer, ByVal view As SapView)

      If xfer IsNot Nothing Then
         xfer.Destroy()
         xfer.Dispose()
      End If

      If camera IsNot Nothing Then
         camera.Destroy()
         camera.Dispose()
      End If

      If acq IsNot Nothing Then
         acq.Destroy()
         acq.Dispose()
      End If

      If buf IsNot Nothing Then
         buf.Destroy()
         buf.Dispose()
      End If

      If view IsNot Nothing Then
         view.Destroy()
         view.Dispose()
      End If

      Console.WriteLine("" & Chr(10) & "Press any key to terminate" & Chr(10) & "")
      Console.ReadKey(True)
   End Sub


End Module
