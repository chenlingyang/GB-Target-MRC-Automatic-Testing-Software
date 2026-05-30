Module GRabLut

    Sub Xfer_XferNotify(ByVal sender As Object, ByVal args As SapXferNotifyEventArgs)
        Dim view As SapView = TryCast(args.Context, SapView)
        view.Show()
    End Sub

    Sub Main(ByVal args As String())
        Dim acq As SapAcquisition = Nothing
        Dim view As SapView = Nothing
        Dim lut As SapLut = Nothing
        Dim transfer As SapTransfer = Nothing
        Dim buffer As SapBuffer = Nothing

        Console.WriteLine("Sapera Console Grab Lut Example (VB.NET version)" & Chr(10) & "")

        Dim acqParams As New MyAcquisitionParams()


        If Not GetOptions(args, acqParams) Then
            Console.WriteLine("" & Chr(10) & "Press any key to terminate" & Chr(10) & "")
            Console.ReadKey()
            Return
        End If

        Dim listLutFiles As New ArrayList()
        Dim location As New SapLocation(acqParams.ServerName, acqParams.ResourceIndex)
        ' Allocate objects
        acq = New SapAcquisition(location, acqParams.ConfigFileName)
        buffer = New SapBuffer(1, acq, SapBuffer.MemoryType.ScatterGather)
        transfer = New SapAcqToBuf(acq, buffer)
        view = New SapView(buffer)

        ' Event 
        transfer.Pairs(0).EventType = SapXferPair.XferEventType.EndOfFrame
        AddHandler transfer.XferNotify, AddressOf Xfer_XferNotify
        transfer.XferNotifyContext = view

        If Not acq.Create() Then
            Console.WriteLine("Error during SapAcqDevice creation!" & Chr(10) & "")
            DestroyObjects(acq, buffer, transfer, view)
            Return
        End If

        If Not buffer.Create() Then
            Console.WriteLine("Error during SapBuffer creation!" & Chr(10) & "")
            DestroyObjects(acq, buffer, transfer, view)
            Return
        End If

        If Not transfer.Create() Then
            Console.WriteLine("Error during SapAcqDeviceToBuf creation!" & Chr(10) & "")
            DestroyObjects(acq, buffer, transfer, view)
            Return
        End If

        If Not view.Create() Then
            Console.WriteLine("Error during SapView creation!" & Chr(10) & "")
            DestroyObjects(acq, buffer, transfer, view)
            Return
        End If

        ' Create a LUT compatible with the acquisition
        If buffer.Format <> SapFormat.Uint8 AndAlso buffer.Format <> SapFormat.Uint16 Then
            Console.WriteLine("Input buffer format must be 8 or 16-bit monochrome." & Chr(10) & "")
            DestroyObjects(acq, buffer, transfer, view)
            Return
        End If

        If acq.Luts IsNot Nothing Then
            lut = acq.Luts(0)
        Else
            Console.WriteLine("The selected acquisition device has no LUTs")
            DestroyObjects(acq, buffer, transfer, view)
            Return
        End If

        Console.WriteLine("" & Chr(10) & "Press any key to perform LUT operation. Press 'q' to quit." & Chr(10) & "")
        Dim info As ConsoleKeyInfo = Console.ReadKey(True)
        Dim key As Char = info.KeyChar

        While key <> "q"c
            If Not ProcessLut(acq, buffer, lut, listLutFiles) Then
                DestroyObjects(acq, buffer, transfer, view)
                Return
            End If

            transfer.Grab()
            Console.WriteLine("" & Chr(10) & "" & Chr(10) & "Grab started, press a key to freeze")
            Console.ReadKey(True)
            transfer.Freeze()
            transfer.Wait(1000)
            Console.WriteLine("" & Chr(10) & "Press any key to perform LUT operation. Press 'q' to quit." & Chr(10) & "")
            info = Console.ReadKey(True)
            key = info.KeyChar
        End While

        DestroyObjects(acq, buffer, transfer, view)
        location.Dispose()
    End Sub

    Function ProcessLut(ByVal acq As SapAcquisition, ByVal buf As SapBuffer, ByVal lut As SapLut, ByVal plistLutFiles As ArrayList) As Boolean



        ' Enable LUT
        acq.LutEnable = True

        Dim bLutLoaded As Boolean = False

        ' Load one of the LUT previously saved in the demo
        bLutLoaded = ExampleUtils.GetLoadLUTFiles(lut, plistLutFiles)

        If Not bLutLoaded Then
            Dim acqLutName As String = ExampleUtils.GetLUTOptionsFromQuestions(buf, lut)
            Console.WriteLine("" & Chr(10) & "......................... ")
            Console.WriteLine("Showing " + acqLutName)
            Console.WriteLine("........................." & Chr(10) & "")
        End If

        ' Apply the LUT you created
        acq.ApplyLut(True, 0)

        Return True
    End Function

    Function GetOptions(ByVal args As String(), ByVal acparams As MyAcquisitionParams) As Boolean
        ' Check if arguments were passed
        If args.Length > 1 Then
            Return ExampleUtils.GetOptionsFromCommandLine(args, acparams)
        Else
            Return ExampleUtils.GetCorAcquisitionOptionsFromQuestions(acparams)
        End If
    End Function



    Sub DestroyObjects(ByVal acq As SapAcquisition, ByVal buf As SapBuffer, ByVal xfer As SapTransfer, ByVal view As SapView)

        If xfer IsNot Nothing Then
            xfer.Destroy()
            xfer.Dispose()
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
        Console.ReadKey()
    End Sub


End Module
