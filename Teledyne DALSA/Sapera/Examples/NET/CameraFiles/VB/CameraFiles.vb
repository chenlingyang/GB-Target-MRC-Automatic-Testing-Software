Module CameraFiles
    Const SELECT_FILE As Integer = 0
    Const SELECT_OPERATION As Integer = 1
    Const PERFORM_DELETE As Integer = 2
    Const PERFORM_UPLOAD As Integer = 3
    Const PERFORM_DOWNLOAD As Integer = 4
    Const MAX_FILE_NAME As Integer = 64


    Sub Main(ByVal args As String())

        Dim status As Boolean
        Dim key As Char
        Dim nextMenu As Integer = 0
        Dim fileNumber As Integer = 0
        Dim currentFileName As String = ""
        Dim feature As SapFeature = Nothing
        Dim device As SapAcqDevice = Nothing
        Dim acqParams As New MyAcquisitionParams()

        SapManager.CommandTimeout = 1200000

        Console.Write("Sapera Console Camera Files Example (VB.NET version)" & Chr(10) & "")

        If Not GetOptions(args, acqParams) Then
            Console.WriteLine("" & Chr(10) & "Press any key to terminate" & Chr(10) & "")
            Console.ReadKey()
            Return
        End If


        Dim location As New SapLocation(acqParams.ServerName, acqParams.ResourceIndex)
        ' Create a camera object and assign delegate
        device = New SapAcqDevice(location, acqParams.ConfigFileName)

        If Not device.Create() Then
            DestroysObjects(device, feature)
            Return
        End If

        If (device.FileAccessAvailable = False) Then
            Console.WriteLine("File access is not available for this device")
            Console.WriteLine("Press any key to terminate")
            Console.ReadKey()
            DestroysObjects(device, feature)
            Return
        End If

        ' Create an empty feature object (to receive information)
        feature = New SapFeature(location)
        If Not feature.Create() Then
            DestroysObjects(device, feature)
            Return
        End If

        While (True)
            Select Case nextMenu
                Case SELECT_FILE
                    Console.WriteLine("Please select the file you want to access or 'q' to quit:")
                    Console.WriteLine()
                    'list all files. After indexes 1-9 use lowercase letters a,b,c..etc
                    For i As Integer = 0 To device.FileCount - 1
                        If (i < 9) Then
                            Console.WriteLine("{0}. {1}", i + 1, device.FileNames(i))
                        Else
                            Console.WriteLine("{0}. {1}", Convert.ToChar(i + 1 - 10 + 97), device.FileNames(i))
                        End If
                    Next

                    'read the user's choice
                    Do While (True)
                        Dim info As ConsoleKeyInfo = Console.ReadKey(True)
                        key = info.KeyChar
                        If (key <> "0"c) Then
                            'quit
                            If (key = "q"c) Then
                                ' Destroy acquisition object
                                If (Not device.Destroy()) Then
                                    Return
                                End If
                                ' Destroy feature object
                                If (Not feature.Destroy()) Then
                                    Return
                                End If
                                Return
                            End If

                            'Use numbers 0 to 9, then lowercase letters if there are more than 10 files
                            If (key >= "1"c And key <= "9"c) Then
                                fileNumber = Val(key) - 1 ' char-to-int conversion
                            Else
                                fileNumber = Convert.ToInt32(key) + 9 - 97
                            End If

                            If ((fileNumber >= 0) And (fileNumber <= device.FileCount)) Then
                                Console.WriteLine()
                                Console.WriteLine("You selected {0}", device.FileNames(fileNumber))
                                Console.WriteLine()
                                currentFileName = device.FileNames(fileNumber)
                                Exit Do
                            Else
                                Console.WriteLine("Invalid selection!")
                                Continue Do
                            End If
                        Else
                            Console.WriteLine("Invalid selection!")
                            Return
                        End If
                    Loop

                    nextMenu = SELECT_OPERATION

                    'Select Operation  menu displays all the possible operations that can be performed on the selected file
                Case SELECT_OPERATION
                    Dim prop As SapAcqDevice.FilePropertyVal
                    Console.WriteLine("Please select an operation or 'q' to return to file selection:")
                    Console.WriteLine()
                    Dim canDownload As Boolean = False
                    Dim canUpload As Boolean = False
                    Dim canDelete As Boolean = False

                    'Based on the Access mode of the file, a user can upload or/and download it.
                    device.GetFileProperty(fileNumber, SapAcqDevice.FileProperty.AccessMode, prop)
                    Select Case prop
                        Case SapAcqDevice.FilePropertyVal.AccessModeNone

                        Case SapAcqDevice.FilePropertyVal.AccessModeReadOnly
                            Console.WriteLine("d. Download file from the device.")
                            canDownload = True
                        Case SapAcqDevice.FilePropertyVal.AccessModeWriteOnly
                            Console.WriteLine("u. Upload file to the device.")
                            canUpload = True

                        Case SapAcqDevice.FilePropertyVal.AccessModeReadWrite
                            Console.WriteLine("d. Download file from the device.")
                            Console.WriteLine("u. Upload file to the device.")
                            canUpload = True
                            canDownload = True
                    End Select

                    'check to see if the file can be deleted
                    Dim accessMode As SapFeature.AccessMode = 0

                    status = device.IsFeatureAvailable("##FileOperationSelector.Delete")
                    If (status) Then
                        device.GetFeatureInfo("##FileOperationSelector.Delete", feature)
                        accessMode = feature.DataAccessMode
                    End If

                    If (accessMode = SapFeature.AccessMode.RO) Then
                        Console.WriteLine("x. Delete file from the device.")
                        canDelete = True
                    End If
                    Console.WriteLine("q. Return to file selection.")
                    Console.WriteLine()

                    'read the user's choice
                    Do While (True)
                        Dim info As ConsoleKeyInfo = Console.ReadKey(True)
                        key = info.KeyChar

                        If (key <> "0") Then

                            'quit
                            If (key = "q") Then
                                nextMenu = SELECT_FILE
                                Exit Do
                                'download
                            ElseIf (key = "d" And canDownload) Then
                                nextMenu = PERFORM_DOWNLOAD
                                Exit Do
                                'upload
                            ElseIf (key = "u" And canUpload) Then
                                nextMenu = PERFORM_UPLOAD
                                Exit Do
                                'delete
                            ElseIf (key = "x" And canDelete) Then
                                nextMenu = PERFORM_DELETE
                                Exit Do
                            Else
                                Console.WriteLine("Invalid selection!")
                                Continue Do
                            End If
                        Else
                            Console.WriteLine("Invalid selection!")
                            Continue Do
                        End If
                    Loop

                    'Perform a file delete
                Case PERFORM_DELETE
                    If (device.DeleteDeviceFile(fileNumber) = True) Then
                        Console.WriteLine("The file delete completed successfully.")
                    Else
                        Console.WriteLine("The file could not be deleted.")
                    End If

                    Console.WriteLine("Press any key to continue or 'q' to quit.")
                    Dim info As ConsoleKeyInfo = Console.ReadKey(True)
                    key = info.KeyChar
                    If (key = "q") Then
                        ' Destroy acquisition object
                        If (Not device.Destroy()) Then
                            Return
                        End If

                        ' Destroy feature object
                        If (Not feature.Destroy()) Then
                            Return
                        End If

                        Return
                    End If
                    nextMenu = SELECT_FILE
                    'perform a file upload
                Case PERFORM_UPLOAD
                    Dim writeReturn As Boolean
                    Dim loadFileName As String

                    'get the name of the file the user is trying to upload. 
                    Console.WriteLine("Please type the name of the file to load, including the extension")
                    Console.WriteLine("Note: The file must exist in the current directory or the upload will fail.")
                    Console.WriteLine()
                    loadFileName = Console.ReadLine()
                    Console.WriteLine("Uploading...")
                    Console.WriteLine("Note: Depending on the file size and communication speed, the transfer could take many minutes, but must not be aborted. Please wait.")
                    Console.WriteLine()
                    'upload file to the device
                    writeReturn = device.WriteFile(".\\" + loadFileName, currentFileName)
                    If (Not writeReturn) Then
                        Console.WriteLine("The selected file write access to the device failed!")
                        Console.WriteLine()
                    Else
                        Console.WriteLine("The file upload completed successfully.")
                        Console.WriteLine()
                    End If

                    Console.WriteLine("Press any key to continue or 'q' to quit.")

                    Dim info As ConsoleKeyInfo = Console.ReadKey(True)
                    key = info.KeyChar
                    If (key = "q") Then
                        ' Destroy acquisition object
                        If (Not device.Destroy()) Then
                            Return
                        End If

                        ' Destroy feature object
                        If (Not feature.Destroy()) Then
                            Return
                        End If

                        Return
                    End If
                    nextMenu = SELECT_FILE
                    'perform a file download
                Case PERFORM_DOWNLOAD
                    Dim readReturn As Boolean
                    Dim saveFileName As String

                    'get the name of the file to which to save.
                    Console.WriteLine("Please type the name of the file to save to, including the extension:")
                    Console.WriteLine("Note: The file will be downloaded in the current directory.")
                    Console.WriteLine()
                    saveFileName = Console.ReadLine()
                    Console.WriteLine("Downloading...")
                    Console.WriteLine("Note: Depending on the file size and communication speed, the transfer could take many minutes, but must not be aborted. Please wait.")
                    Console.WriteLine()

                    'download file to current directory
                    readReturn = device.ReadFile(currentFileName, ".\\" + saveFileName)

                    If (Not readReturn) Then

                        Console.WriteLine("The selected file read access from the device failed!")
                        Console.WriteLine()
                    Else
                        Console.WriteLine("The file download completed successfully.")
                        Console.WriteLine()
                    End If

                    Console.WriteLine("Press any key to continue or 'q' to quit.")
                    Dim info As ConsoleKeyInfo = Console.ReadKey(True)
                    key = info.KeyChar
                    If (key = "q") Then
                        ' Destroy acquisition object
                        If (Not device.Destroy()) Then
                            Return
                        End If

                        ' Destroy feature object
                        If (Not feature.Destroy()) Then
                            Return
                        End If

                        Return
                    End If
                    nextMenu = SELECT_FILE
            End Select
        End While
    End Sub

    Function GetOptions(ByVal args As String(), ByVal acparams As MyAcquisitionParams) As Boolean
        ' Check if arguments were passed
        If args.Length > 1 Then
            Return ExampleUtils.GetAcqDeviceOptionsFromCommandLine(args, acparams)
        Else
            Return ExampleUtils.GetCorAcqDeviceOptionsFromQuestions(acparams, False)
        End If
    End Function

    Sub DestroysObjects(ByVal acq As SapAcqDevice, ByVal feat As SapFeature)
        If acq IsNot Nothing Then
            acq.Destroy()
            acq.Dispose()
        End If
        If feat IsNot Nothing Then
            feat.Destroy()
            feat.Dispose()
        End If

    End Sub
End Module
