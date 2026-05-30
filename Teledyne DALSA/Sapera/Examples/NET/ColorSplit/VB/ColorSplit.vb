Module ColorSplit

    Const INPUT_FILE_NAME As String = "rgb8888.bmp"
    Const OUTPUT_FILE_NAME As String = "rgb8888_inverted.bmp"

    Sub Main()
        Dim inBuf As New SapBuffer()
        Dim outBuf As New SapBuffer()
        Dim compBuf As New SapBuffer()
        Dim inView As New SapView(inBuf)
        Dim outView As New SapView(outBuf)

        Console.WriteLine("Sapera Color Split Example (VB.NET version)" & Chr(10) & "")

        ' Get image location from environment variable
        Dim inputFileName As String = Environment.GetEnvironmentVariable("SAPERADIR")
        If inputFileName Is Nothing Then
            Console.WriteLine("Error: cannot obtain image location, environment variable not present.")
            Return
        End If
        inputFileName += "\Images\Display\" + INPUT_FILE_NAME

        ' Allocate input buffer with parameters compatible to file (does not load it)
        inBuf.SetParametersFromFile(inputFileName, SapBuffer.MemoryType.[Default])
        If Not inBuf.Create() Then
            DestroysObjects(inBuf, outBuf, compBuf, inView, outView)
            Return
        End If

        ' Allocate output buffer with parameters compatible to input buffer
        outBuf.Count = inBuf.Count
        outBuf.Width = inBuf.Width
        outBuf.Height = inBuf.Height
        outBuf.Format = inBuf.Format
        outBuf.Type = inBuf.Type

        If Not outBuf.Create() Then
            DestroysObjects(inBuf, outBuf, compBuf, inView, outView)
            Return
        End If

        ' Create view objects     
        If Not inView.Create() Then
            DestroysObjects(inBuf, outBuf, compBuf, inView, outView)
            Return
        End If

        If Not outView.Create() Then
            DestroysObjects(inBuf, outBuf, compBuf, inView, outView)
            Return
        End If

        inView.WindowTitle = "Input Image"
        outView.WindowTitle = "Output Image"

        ' Load input image
        If Not inBuf.Load(inputFileName, -1) Then
            DestroysObjects(inBuf, outBuf, compBuf, inView, outView)
            Return
        End If

        ' Allocate the three component buffers
        compBuf.Count = 3
        compBuf.Width = inBuf.Width
        compBuf.Height = inBuf.Height
        compBuf.Format = SapFormat.Mono8
        compBuf.Type = inBuf.Type

        If Not compBuf.Create() Then
            DestroysObjects(inBuf, outBuf, compBuf, inView, outView)
            Return
        End If

        ' Apply simple processing in the monochrome components (pixel inversion)
        Console.WriteLine("Start Color Split processing... Wait" & Chr(10) & "")

        ' Split input image in three monochrome components
        compBuf.SplitComponents(inBuf)

        Dim numPix As Integer = compBuf.Width * compBuf.Height
        Dim dataBuf(numPix) As Byte

        Dim dataBufHandle As GCHandle = GCHandle.Alloc(dataBuf, GCHandleType.Pinned)
        Dim dataBufAddress As IntPtr = dataBufHandle.AddrOfPinnedObject()

        ' Process the components using a temporary array
        For compIndex As Integer = 0 To 2
            compBuf.Read(compIndex, 0, numPix, dataBufAddress)
            For pixIndex As Integer = 0 To numPix - 1
                dataBuf(pixIndex) = 255 - dataBuf(pixIndex)
            Next
            compBuf.Write(compIndex, 0, numPix, dataBufAddress)
        Next

        dataBufHandle.Free()
        Console.WriteLine("End processing" & Chr(10) & "")

        'Merge the three monochrome components into output image
        outBuf.MergeComponents(compBuf)

        ' Display input and output buffers
        inView.Show()
        outView.Show()

        Console.WriteLine("Press any key to terminate.")
        Console.ReadKey()

        DestroysObjects(inBuf, outBuf, compBuf, inView, outView)

    End Sub

    Sub DestroysObjects(ByVal inBuffer As SapBuffer, ByVal outBuffer As SapBuffer, ByVal compBuffer As SapBuffer, ByVal inView As SapView, ByVal outView As SapView)
        If inView IsNot Nothing Then
            inView.Destroy()
            inView.Dispose()
        End If

        If outView IsNot Nothing Then
            outView.Destroy()
            outView.Dispose()
        End If

        If inBuffer IsNot Nothing Then
            inBuffer.Destroy()
            inBuffer.Dispose()
        End If

        If outBuffer IsNot Nothing Then
            outBuffer.Destroy()
            outBuffer.Dispose()
        End If

        If compBuffer IsNot Nothing Then
            compBuffer.Destroy()
            compBuffer.Dispose()
        End If
    End Sub

End Module
