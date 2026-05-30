Module GrabCameraLink

    Sub xfer_XferNotify(ByVal sender As Object, ByVal args As SapXferNotifyEventArgs)
        Dim View As SapView = TryCast(args.Context, SapView)
        View.Show()
    End Sub

    Sub Main(ByVal args As String())
        Dim Acq As SapAcquisition = Nothing
        Dim AcqDevice As SapAcqDevice = Nothing
        Dim Feature As SapFeature = Nothing
        Dim Buffers As SapBuffer = Nothing
        Dim Xfer As SapTransfer = Nothing
        Dim View As SapView = Nothing
        Dim isNotSupported, status, acquisitionCreated, acqDeviceCreated As Boolean

        isNotSupported = False
        status = False
        acquisitionCreated = True
        acqDeviceCreated = True

        Console.WriteLine("Sapera Console Grab Camera Link Example (VB.NET version)")

        Dim acqParams As New MyAcquisitionParams()

        'get the SapAquisition (frame grabber)
        If Not ExampleUtils.GetCorAcquisitionOptionsFromQuestions(acqParams) Then
            Console.WriteLine("" & Chr(10) & "Press any key to terminate" & Chr(10) & "")
            Console.ReadKey(True)
            Return
        End If

        Dim loc As New SapLocation(acqParams.ServerName, acqParams.ResourceIndex)

        If SapManager.GetResourceCount(acqParams.ServerName, SapManager.ResourceType.Acq) > 0 Then
            Acq = New SapAcquisition(loc, acqParams.ConfigFileName)
            Buffers = New SapBuffer(1, Acq, SapBuffer.MemoryType.ScatterGather)
            Xfer = New SapAcqToBuf(Acq, Buffers)
            View = New SapView(Buffers)

            ' Create acquisition object
            If Not Acq.Create() Then
                acquisitionCreated = False
            End If

        End If

        'get the SapAcqDevice (camera)
        If Not ExampleUtils.GetCorAcqDeviceOptionsFromQuestions(acqParams, False) Then
            Console.WriteLine("" & Chr(10) & "Press any key to terminate" & Chr(10) & "")
            Console.ReadKey(True)
            Return
        End If

        Dim loc2 As New SapLocation(acqParams.ServerName, acqParams.ResourceIndex)
        AcqDevice = New SapAcqDevice(loc2, False)
        Feature = New SapFeature(loc2)
        Feature.Create()

        ' Create acquisition object
        If Not AcqDevice.Create() Then
            acqDeviceCreated = False
        End If


        'check to see if this is a DFNC camera. If it is not, end program
        'Program checks for both current and legacy naming
        Dim modelName As String = ""
        status = AcqDevice.IsFeatureAvailable("DeviceModelName")
        If (status) Then
            AcqDevice.GetFeatureValue("DeviceModelName", modelName)
            If (modelName.Contains("Genie")) Then
                isNotSupported = True
            End If
        Else
            isNotSupported = True
        End If

        If (isNotSupported) Then
            Console.WriteLine("Current camera model is not supported.")
            DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer, View)
            Return
        End If

        ' End of frame event
        Xfer.Pairs(0).EventType = SapXferPair.XferEventType.EndOfFrame
        AddHandler Xfer.XferNotify, AddressOf xfer_XferNotify
        Xfer.XferNotifyContext = View


        ' Create buffer object
        If Not Buffers.Create() Then
            Console.WriteLine("Error during SapBuffer creation!" & Chr(10) & "")
            DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer, View)
            Return
        End If

        ' Create Xfer object
        If Not Xfer.Create() Then
            Console.WriteLine("Error during SapTransfer creation!" & Chr(10) & "")
            DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer, View)
            Return
        End If

        ' Create View object
        If Not View.Create() Then
            Console.WriteLine("Error during SapView creation!" & Chr(10) & "")
            DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer, View)
            Return
        End If

        Xfer.Grab()

        Console.WriteLine("Press any key to stop grab.")
        Console.ReadKey(True)

        'Stop grab
        Xfer.Freeze()
        If (Not Xfer.Wait(5000)) Then
            Console.WriteLine("Grab could not stop properly.")
            Console.WriteLine()
            Xfer.Abort()
        End If

        DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer, View)
        loc.Dispose()
        loc2.Dispose()
    End Sub

    Sub DestroysObjects(ByVal acq As SapAcquisition, ByVal feature As SapFeature, ByVal camera As SapAcqDevice, ByVal buf As SapBuffer, ByVal xfer As SapTransfer, ByVal view As SapView)

        If view IsNot Nothing Then
            view.Destroy()
            view.Dispose()
        End If

        If xfer IsNot Nothing Then
            xfer.Destroy()
            xfer.Dispose()
        End If

        If buf IsNot Nothing Then
            buf.Destroy()
            buf.Dispose()
        End If

        If acq IsNot Nothing Then
            acq.Destroy()
            acq.Dispose()
        End If

        If feature IsNot Nothing Then
            feature.Destroy()
            feature.Dispose()
        End If

        If camera IsNot Nothing Then
            camera.Destroy()
            camera.Dispose()
        End If

        Console.WriteLine("" & Chr(10) & "Press any key to terminate" & Chr(10) & "")
        Console.ReadKey(True)
    End Sub


End Module
