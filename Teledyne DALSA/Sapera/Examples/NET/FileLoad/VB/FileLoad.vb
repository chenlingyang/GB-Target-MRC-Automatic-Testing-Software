Module FileLoad

    Dim FileList As String() = {"mono8.bmp", "mono16.tif", "rgb565.bmp", "rgb888.bmp", "rgb8888.bmp", "rgb101010.bmp", _
 "yuy2.crc"}

    Sub Main(ByVal args As String())

        Dim Buffers As SapBuffer = Nothing
        Dim View As SapView = Nothing
        Dim filename As String = ""

        Console.WriteLine("Sapera Console File Load Example (VB.NET version)")

        ' Call GetOptions to determine which file to load
        If Not GetOptions(args, filename) Then
            Console.WriteLine("Press any key to terminate.")
            Console.ReadKey()
            Return
        End If

        ' Allocate buffer with parameters compatible to file (does not load it)
        Buffers = New SapBuffer(filename, SapBuffer.MemoryType.[Default])
        ' Allocate view with buffer
        View = New SapView(Buffers)

        ' Create buffer object
        If Not Buffers.Create() Then
            DestroysObjects(Buffers, View)
            Return
        End If

        ' Create view object
        If Not View.Create() Then
            DestroysObjects(Buffers, View)
            Return
        End If

        ' Load file
        If Not Buffers.Load(filename, -1) Then
            DestroysObjects(Buffers, View)
            Return
        End If

        ' Display buffer
        View.Show()


        Console.WriteLine("Press any key to terminate.")
        Console.ReadKey()
    End Sub

    Private Function GetOptions(ByVal args As String(), ByRef filename As String) As Boolean
        ' Check the command line for user commands
        If args.Length > 1 Then
            If (String.Compare(args(1), "/?", StringComparison.Ordinal) = 0 OrElse String.Compare(args(1), "-?", StringComparison.Ordinal) = 0) Then
                ' print help
                Console.WriteLine("Usage:" & Chr(10) & "")
                Console.WriteLine("FileLoadCPP [<image filename>]")
                Return False
            End If

            ' Check if user specified filename
            If args.Length = 2 Then
                ' Verify that the specified file exists
                If Not File.Exists(args(1)) Then
                    Console.WriteLine("Specified image file {0} is invalid!" & Chr(10) & "", args(1))
                    Return False
                End If
                filename = args(1)
                Return True
            End If

            Console.WriteLine("Invalid command line!" & Chr(10) & "")
            Return False
        End If

        ' Ask user to select image file 

        Console.WriteLine("Choose one image file: ")
        For i As Integer = 0 To FileList.Length - 1
            Console.WriteLine("{0}. {1}", i + 1, FileList(i))
        Next


        Dim info As ConsoleKeyInfo = Console.ReadKey(True)
        Dim key As Char = info.KeyChar
        Dim numkey As Integer = CInt(Val(key))

        If numkey < 1 OrElse numkey > FileList.Length Then
            Console.WriteLine("Invalid selection!" & Chr(10) & "")
            Return False
        End If

        filename = Environment.GetEnvironmentVariable("SAPERADIR")
        If filename Is Nothing Then
            Return False
        End If
        filename += "\Images\Display\" + FileList(numkey - 1)
        Return True
    End Function

    Sub DestroysObjects(ByVal Buffers As SapBuffer, ByVal View As SapView)

        If View IsNot Nothing Then
            View.Destroy()
            View.Dispose()
        End If
        If Buffers IsNot Nothing Then
            Buffers.Destroy()
            Buffers.Dispose()
        End If

        Console.WriteLine("Press any key to terminate.")
        Console.ReadKey()

    End Sub


End Module
