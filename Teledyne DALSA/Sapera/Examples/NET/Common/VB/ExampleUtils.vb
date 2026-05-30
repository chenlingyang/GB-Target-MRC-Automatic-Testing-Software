Namespace DALSA.SaperaLT.Examples.NET.Utils
    Public Class ExampleUtils
        ' Static Variables
        Const GAMMA_FACTOR As Integer = 10000
        Const MAX_CONFIG_FILES As Integer = 36
        ' 10 numbers + 26 lowercase letters
        'static char configFileNames[MAX_CONFIG_FILES][MAX_PATH] = {0};

        Public Shared Function GetOptionsFromCommandLine(ByVal argv As String(), ByVal acqParams As MyAcquisitionParams) As Boolean
            ' Check the command line for user commands
            If argv(1).Equals("/?") OrElse argv(1).Equals("-?") Then
                ' print help
                Console.WriteLine("Usage:" & Chr(10) & "")
                Console.WriteLine("Grab  [<acquisition server name> <acquisition device index> <config filename>]" & Chr(10) & "")
                Return False
            End If

            ' Check if enough arguments were passed
            If argv.Length < 4 Then
                Console.WriteLine("Invalid command line!" & Chr(10) & "")
                Return False
            End If

            ' Validate server name
            If SapManager.GetServerIndex(argv(1)) < 0 Then
                Console.WriteLine("Invalid acquisition server name!" & Chr(10) & "")
                Return False
            End If

            ' Does the server support acquisition?
            Dim deviceCount As Integer = SapManager.GetResourceCount(argv(1), SapManager.ResourceType.Acq)
            Dim cameraCount As Integer = IIf(deviceCount = 0, SapManager.GetResourceCount(argv(1), SapManager.ResourceType.AcqDevice), 0)

            If deviceCount + cameraCount = 0 Then
                Console.WriteLine("This server does not support acquisition!" & Chr(10) & "")
                Return False
            End If

            ' Validate device index
            If Integer.Parse(argv(2)) < 0 OrElse Integer.Parse(argv(2)) >= deviceCount + cameraCount Then
                Console.WriteLine("Invalid acquisition device index!" & Chr(10) & "")
                Return False
            End If

            If Not File.Exists(argv(3)) Then
                Console.WriteLine("The specified config file (" + argv(3) + "is invalid!" & Chr(10) & "")
                Return False
            End If

            ' Fill-in output variables
            acqParams.ServerName = argv(1)
            acqParams.ResourceIndex = Integer.Parse(argv(2))
            acqParams.ConfigFileName = argv(3)

            Return True
        End Function

        '''///// Ask questions to user to select acquisition board/device and config file ////////
        Public Shared Function GetOptionsFromQuestions(ByVal acqParams As MyAcquisitionParams) As Boolean

            ' Get total number of boards in the system
            Dim serverCount As Integer = SapManager.GetServerCount()
            'string configFileIndexToPrint;

            If serverCount = 0 Then
                Console.WriteLine("No device found!" & Chr(10) & "")
                Return False
            End If

            Console.WriteLine("" & Chr(10) & "Select the acquisition server (or 'q' to quit)")
            Console.WriteLine(".............................................." & Chr(10) & "")

            ' Scan the boards to find those that support acquisition
            Dim serverIndex As Integer
            Dim serverFound As Boolean = False
            Dim cameraFound As Boolean = False
            For serverIndex = 0 To serverCount - 1
                If SapManager.GetResourceCount(serverIndex, SapManager.ResourceType.Acq) <> 0 Then
                    Dim serverName As String = SapManager.GetServerName(serverIndex)
                    Console.WriteLine(serverIndex.ToString() + ": " + serverName)
                    serverFound = True
                ElseIf SapManager.GetResourceCount(serverIndex, SapManager.ResourceType.AcqDevice) <> 0 Then
                    Dim serverName As String = SapManager.GetServerName(serverIndex)
                    Console.WriteLine(serverIndex.ToString() + ": " + serverName)
                    cameraFound = True
                End If
            Next

            ' At least one acquisition server must be available
            If Not serverFound AndAlso Not cameraFound Then
                Console.WriteLine("No acquisition server found!" & Chr(10) & "")
                Return False
            End If

            Dim info As ConsoleKeyInfo = Console.ReadKey(True)
            Dim key As Char = info.KeyChar


            If key = "q"c Then
                Return False
            End If
            Dim serverNum As Integer = CInt(Val(key))
            ' char-to-int conversion     
            If (serverNum >= 1) AndAlso (serverNum < serverCount) Then
                acqParams.ServerName = SapManager.GetServerName(serverNum)
            Else
                Console.WriteLine("Invalid selection!" & Chr(10) & "")
                Return False
            End If


            ' Scan all the acquisition devices on that server and show menu to user
            Dim deviceCount As Integer = SapManager.GetResourceCount(acqParams.ServerName, SapManager.ResourceType.Acq)
            Dim cameraCount As Integer = IIf(deviceCount = 0, SapManager.GetResourceCount(acqParams.ServerName, SapManager.ResourceType.AcqDevice), 0)
            Dim allDeviceCount As Integer = deviceCount + cameraCount

            Console.WriteLine("" & Chr(10) & "Select the acquisition device (or 'q' to quit)")
            Console.WriteLine(".............................................." & Chr(10) & "")

            For deviceIndex As Integer = 0 To deviceCount - 1
                Dim deviceName As String = SapManager.GetResourceName(acqParams.ServerName, SapManager.ResourceType.Acq, deviceIndex)
                Console.WriteLine(CInt((deviceIndex + 1)).ToString() + ": " + deviceName)
            Next

            If deviceCount = 0 Then
                For cameraIndex As Integer = 0 To cameraCount - 1
                    Dim cameraName As String = SapManager.GetResourceName(acqParams.ServerName, SapManager.ResourceType.AcqDevice, cameraIndex)
                    Console.WriteLine(CInt((cameraIndex + 1)).ToString() + ": " + cameraName)
                Next
            End If

            info = Console.ReadKey(True)
            key = info.KeyChar
            If key = "q"c Then
                Return False
            End If
            Dim deviceNum As Integer = CInt(Val(key))
            If (deviceNum >= 1) AndAlso (deviceNum <= allDeviceCount) Then
                acqParams.ResourceIndex = deviceNum - 1
            Else
                Console.WriteLine("Invalid selection!" & Chr(10) & "")
                Return False
            End If

            ' List all files in the config directory
            Dim configPath As String = Environment.GetEnvironmentVariable("SAPERADIR") + "\CamFiles\User\"
            If Not Directory.Exists(configPath) Then
                Console.WriteLine("Directory : {0} Does not exist", configPath)
                Return False
            End If

            Dim ccffiles As String() = Directory.GetFiles(configPath, "*.ccf")
            Dim configFileCount As Integer = ccffiles.Length
            Dim configFileNames As String() = New String(configFileCount + 1) {}

            If configFileCount = 0 Then
                If cameraCount = 0 Then
                    Console.WriteLine("No config file found." & Chr(10) & "Use CamExpert to generate a config file before running this example." & Chr(10) & "")
                    Return False
                Else
                    Console.WriteLine("" & Chr(10) & "Select the config file (or 'q' to quit)")
                    Console.WriteLine("......................................." & Chr(10) & "")
                    Console.WriteLine("1: No config File." & Chr(10) & "")
                    configFileCount = 1
                End If
            Else

                Console.WriteLine("" & Chr(10) & "Select the config file (or 'q' to quit)")
                Console.WriteLine("......................................." & Chr(10) & "")
                configFileCount = 0
                If (deviceCount = 0 And cameraCount <> 0) Then
                    configFileCount = 1
                    Console.WriteLine("1: No config File.")
                End If

                Dim configFileShow As Integer = 0
                For Each ccfFileName As String In ccffiles
                    Dim fileName As String = ccfFileName.Replace(configPath, "")
                    If configFileCount < 9 Then
                        configFileShow = configFileCount + 1
                        Console.WriteLine(configFileShow.ToString() + ": " + fileName)
                    ElseIf (configFileCount < MAX_CONFIG_FILES - 1) Then
                        '97 = integer value of a
                        configFileShow = configFileCount - 9 + 97
                        Console.WriteLine(Convert.ToChar(configFileShow) + ": " + fileName)
                    End If
                    configFileNames(configFileCount) = ccfFileName
                    configFileCount += 1
                Next
            End If

            info = Console.ReadKey(True)
            key = info.KeyChar
            If key = "q"c Then
                Return False
            End If
            Dim configNum As Integer = 0
            ' Use numbers 0 to 9, then lowercase letters if there are more than 10 files
            If key >= "1"c AndAlso key <= "9"c Then
                configNum = Val(key)
            Else
                configNum = Convert.ToInt32(key) + 10 - 97
                ' char-to-int conversion
            End If

            If (configNum >= 1) AndAlso (configNum <= configFileCount) Then
                acqParams.ConfigFileName = configFileNames(configNum - 1)
            Else
                Console.WriteLine("" & Chr(10) & "Invalid selection!" & Chr(10) & "")
                Return False
            End If

            Console.WriteLine("" & Chr(10) & "")
            Return True
        End Function

        Public Shared Function GetAcqDeviceOptionsFromCommandLine(ByVal args As String(), ByVal acparams As MyAcquisitionParams) As Boolean
            ' Check the command line for user commands
            If args(1).Equals("/?") OrElse args(1).Equals("-?") Then
                ' print help
                Console.WriteLine("Usage:" & Chr(10) & "")
                Console.WriteLine("GigECameraLut [<acquisition server name> <acquisition device index>]" & Chr(10) & "")
                Return False
            End If

            ' Check if enough arguments were passed
            If args.Length < 3 Then
                Console.WriteLine("Invalid command line!" & Chr(10) & "")
                Return False
            End If

            ' Validate server name
            If SapManager.GetServerIndex(args(1)) < 0 Then
                Console.WriteLine("Invalid acquisition server name!" & Chr(10) & "")
                Return False
            End If

            ' Does the server support acquisition?
            Dim deviceCount As Integer = SapManager.GetResourceCount(args(1), SapManager.ResourceType.AcqDevice)
            If deviceCount = 0 Then
                Console.WriteLine("This server does not support acquisition!" & Chr(10) & "")
                Return False
            End If

            ' Validate device index
            If Integer.Parse(args(2)) < 0 OrElse Integer.Parse(args(2)) >= deviceCount Then
                Console.WriteLine("Invalid acquisition device index!" & Chr(10) & "")
                Return False
            End If

            ' Fill-in output variables
            acparams.ServerName = args(1)
            acparams.ResourceIndex = Integer.Parse(args(2))

            Return True
        End Function


        Public Shared Function GetCorAcqDeviceOptionsFromQuestions(ByVal acqParams As MyAcquisitionParams, ByVal showGigEOnly As Boolean) As Boolean
            Dim serverCount As Integer = SapManager.GetServerCount()
            Dim GenieIndex As Integer = 0
            Dim listServerNames As ArrayList = New System.Collections.ArrayList()

            If serverCount = 0 Then
                Console.WriteLine("No device found!" & Chr(10) & "")
                Return False
            End If

            Dim cameraFound As Boolean = False
            For serverIndex As Integer = 0 To serverCount - 1
                If SapManager.GetResourceCount(serverIndex, SapManager.ResourceType.AcqDevice) <> 0 Then
                    Dim serverName As String = SapManager.GetServerName(serverIndex)
                    If (showGigEOnly = False Or (showGigEOnly = True And SapManager.GetResourceCount(serverIndex, SapManager.ResourceType.Acq) = 0)) Then
                        listServerNames.Add(serverName)
                        GenieIndex += 1
                        cameraFound = True
                    End If
                End If
            Next

            ' At least one acquisition server must be available
            If Not cameraFound Then
                Console.WriteLine("No camera found!" & Chr(10) & "")
                Return False
            End If

            Console.WriteLine("" & Chr(10) & "Select one of the camera(s) detected (or 'q' to quit)")
            Dim count As Integer = 1
            For Each serverName As String In listServerNames
                Console.WriteLine("" & Chr(10) & "........................................" & Chr(10) & "")
                Console.WriteLine(Convert.ToString(count) + ": " + serverName)

                Dim deviceName As String = SapManager.GetResourceName(serverName, SapManager.ResourceType.AcqDevice, 0)
                Console.WriteLine("User defined Name : " + deviceName + "" & Chr(10) & "")
                Console.WriteLine("........................................" & Chr(10) & "")
                count += 1
            Next

            Dim info As ConsoleKeyInfo = Console.ReadKey(True)
            Dim key As Char = info.KeyChar

            If key = "q"c Then
                Return False
            End If
            Dim serverNum As Integer = CInt(Val(key))
            ' char-to-int conversion
            If (serverNum >= 1) AndAlso (serverNum <= GenieIndex) Then
                acqParams.ServerName = Convert.ToString(listServerNames(serverNum - 1))
                acqParams.ResourceIndex = 0
            Else
                Console.WriteLine("Invalid selection!" & Chr(10) & "")
                Return False
            End If
            Console.WriteLine("" & Chr(10) & "")
            Return True
        End Function

        Public Shared Function GetCorAcquisitionOptionsFromQuestions(ByVal acqParams As MyAcquisitionParams) As Boolean

            ' Get total number of boards in the system
            Dim configFileNames As String() = New String(MAX_CONFIG_FILES - 1) {}
            Dim serverCount As Integer = SapManager.GetServerCount()
            'string configFileIndexToPrint;

            If serverCount = 0 Then
                Console.WriteLine("No device found!" & Chr(10) & "")
                Return False
            End If

            Console.WriteLine("" & Chr(10) & "Select the acquisition server (or 'q' to quit)")
            Console.WriteLine(".............................................." & Chr(10) & "")

            ' Scan the boards to find those that support acquisition
            Dim serverFound As Boolean = False
            For serverIndex As Integer = 0 To serverCount - 1
                If SapManager.GetResourceCount(serverIndex, SapManager.ResourceType.Acq) <> 0 Then
                    Dim serverName As String = SapManager.GetServerName(serverIndex)
                    Console.WriteLine(serverIndex.ToString() + ": " + serverName)
                    serverFound = True
                End If
            Next

            ' At least one acquisition server must be available
            If Not serverFound Then
                Console.WriteLine("No acquisition server found!" & Chr(10) & "")
                Return False
            End If

            Dim info As ConsoleKeyInfo = Console.ReadKey(True)
            Dim key As Char = info.KeyChar

            If key = "q"c Then
                Return False
            End If
            Dim serverNum As Integer = CInt(Val(key))
            ' char-to-int conversion     
            If (serverNum >= 1) AndAlso (serverNum < serverCount) Then
                acqParams.ServerName = SapManager.GetServerName(serverNum)
            Else
                Console.WriteLine("Invalid selection!" & Chr(10) & "")
                Return False
            End If
           
            ' Scan all the acquisition devices on that server and show menu to user
            Dim deviceCount As Integer = SapManager.GetResourceCount(acqParams.ServerName, SapManager.ResourceType.Acq)

#If GRAB_CAMERA_LINK Then
            Console.WriteLine("" & Chr(10) & "Select the device you wish to use on this server (or 'q' to quit)")
            Console.WriteLine("................................................................." & Chr(10) & "")
#Else
            Console.WriteLine("" & Chr(10) & "Select the acquisition device (or 'q' to quit)")
            Console.WriteLine(".............................................." & Chr(10) & "")
#End If

            Dim deviceName As String = ""
            For deviceIndex As Integer = 0 To deviceCount - 1
                deviceName = SapManager.GetResourceName(acqParams.ServerName, SapManager.ResourceType.Acq, deviceIndex)
                Console.WriteLine(CInt((deviceIndex + 1)).ToString() + ": " + deviceName)
            Next

            info = Console.ReadKey(True)
            key = info.KeyChar
            If key = "q"c Then
                Return False
            End If
            Dim deviceNum As Integer = CInt(Val(key))
            If (deviceNum >= 1) AndAlso (deviceNum <= deviceCount) Then
                acqParams.ResourceIndex = deviceNum - 1
            Else
                Console.WriteLine("Invalid selection!" & Chr(10) & "")
                Return False
            End If

            ' List all files in the config directory

            Dim configPath As String = Environment.GetEnvironmentVariable("SAPERADIR") + "\CamFiles\User\"
            If Not Directory.Exists(configPath) Then
                Console.WriteLine("Directory : {0} Does not exist", configPath)
                Return False
            End If

            Dim ccffiles As String() = Directory.GetFiles(configPath, "*.ccf")

            If ccffiles.Length = 0 Then
                Console.WriteLine("No config file found." & Chr(10) & "Use CamExpert to generate a config file before running this example." & Chr(10) & "")
                Return False
            Else
                Console.WriteLine("" & Chr(10) & "Select the config file (or 'q' to quit)")
                Console.WriteLine("......................................." & Chr(10) & "")

                Dim configFileCount As Integer = 0
                Dim configFileShow As Integer = 0
                For Each ccfFileName As String In ccffiles
                    Dim fileName As String = ccfFileName.Replace(configPath, "")
                    If configFileCount < 9 Then
                        configFileShow = configFileCount + 1
                        Console.WriteLine(configFileShow.ToString() + ": " + fileName)
                    Else
                        '97 = integer value of a
                        configFileShow = configFileCount - 9 + 97
                        Console.WriteLine(Convert.ToChar(configFileShow) + ": " + fileName)
                    End If
                    configFileNames(configFileCount) = ccfFileName
                    configFileCount += 1
                Next

                info = Console.ReadKey(True)
                key = info.KeyChar
                If key = "q"c Then
                    Return False
                End If
                Dim configNum As Integer = 0
                ' Use numbers 0 to 9, then lowercase letters if there are more than 10 files
                If key >= "1"c AndAlso key <= "9"c Then
                    configNum = Val(key)
                Else
                    configNum = Convert.ToInt32(key) + 10 - 97
                    ' char-to-int conversion
                End If

                If (configNum >= 1) AndAlso (configNum <= configFileCount) Then
                    acqParams.ConfigFileName = configFileNames(configNum - 1)
                Else
                    Console.WriteLine("" & Chr(10) & "Invalid selection!" & Chr(10) & "")
                    Return False
                End If
            End If
            Console.WriteLine("" & Chr(10) & "")
            Return True
        End Function

        Private Shared Function IsMonoBuffer(ByVal pBuffers As SapBuffer) As Boolean
            Dim format As SapFormat = pBuffers.Format

            If format = SapFormat.Uint8 OrElse format = SapFormat.Int8 OrElse format = SapFormat.Int16 OrElse format = SapFormat.Uint16 OrElse format = SapFormat.Int24 OrElse format = SapFormat.Uint24 OrElse format = SapFormat.Int32 OrElse format = SapFormat.Uint32 OrElse format = SapFormat.Int64 OrElse format = SapFormat.Uint64 Then
                Return True
            Else
                Return False
            End If
        End Function

        Private Shared Function SetDataValue(ByVal pBuffers As SapBuffer, ByVal pPrmIndex As UInt32) As SapData
            If IsMonoBuffer(pBuffers) Then
                Dim mono As New SapDataMono(128)
                Return mono
            Else
                Dim rgb As New SapDataRGB()
                pPrmIndex = pPrmIndex + 3
                Return rgb
            End If
        End Function

        Public Shared Function GetLUTOptionsFromQuestions(ByVal pBuffers As SapBuffer, ByVal pLut As SapLut) As String
            'Ask questions to user to select LUT mode
            Dim prmIndex As UInt32 = 1
            Dim acqLutFileName As String = ""
            Dim chAcqLutName As String = ""

            While chAcqLutName = ""
                Console.WriteLine("" & Chr(10) & "Select the LookUpTable mode you want to apply: " & Chr(10) & "")

                Console.WriteLine("a : Normal mode")
                Console.WriteLine("b : Arithmetic mode")
                Console.WriteLine("c : Binary mode")
                Console.WriteLine("d : Boolean mode")
                Console.WriteLine("e : Gamma mode")
                Console.WriteLine("f : Reverse mode")
                Console.WriteLine("g : Roll mode")
                Console.WriteLine("h : Shift mode")
                Console.WriteLine("i : Slope mode")
                Console.WriteLine("j : Threshold single mode")
                Console.WriteLine("k : Threshold double mode")

                Dim info As ConsoleKeyInfo = Console.ReadKey(True)
                Dim key As Char = info.KeyChar

                Select Case key
                    Case "a"c
                        pLut.Normal()
                        acqLutFileName = "Normal_Lut_Mode.lut"
                        chAcqLutName = "Normal Lut"
                        Exit Select
                    Case "b"c
                        Dim operationMode As Integer = 0
                        'Linear plus offset with clip
                        '
                        '					            Others operations available
                        '				            
                        'int operation = 1;//Linear minus offset(absolute)
                        'int operation = 2;//Linear minus offset(with clip)
                        'int operation = 3;//Linear with lower clip
                        'int operation = 4;//Linear with upper clip
                        'int operation = 5;//Scale to maximum limit

                        Dim offSet As SapData
                        offSet = SetDataValue(pBuffers, prmIndex)
                        pLut.Arithmetic(DirectCast(operationMode, SapLut.ArithmeticOp), offSet)
                        acqLutFileName = "Arithmetic_Lut_Mode.lut"
                        chAcqLutName = "Arithmetic Lut"
                        Exit Select
                    Case "c"c
                        Dim clipValue As SapData
                        clipValue = SetDataValue(pBuffers, prmIndex)
                        pLut.BinaryPattern(0, clipValue)
                        acqLutFileName = "Binary_Lut_Mode.lut"
                        chAcqLutName = "Binary Lut"
                        Exit Select
                    Case "d"c
                        Dim booleanFunction As SapData
                        booleanFunction = SetDataValue(pBuffers, prmIndex)
                        pLut.[Boolean](DirectCast(0, SapLut.BooleanOp), booleanFunction)
                        '
                        '					            Others operations available
                        '				            

                        ' AND
                        'pLut->Boolean((SapLut::BooleanOp)1, booleanFunction);
                        ' OR
                        'pLut->Boolean((SapLut::BooleanOp)2, booleanFunction);
                        ' XOR
                        acqLutFileName = "Boolean_Lut_Mode.lut"
                        chAcqLutName = "Boolean Lut"
                        Exit Select
                    Case "e"c
                        Dim gammaFactor As Integer = CInt((2 * GAMMA_FACTOR))
                        pLut.Gamma(CSng(gammaFactor) / GAMMA_FACTOR)
                        acqLutFileName = "Gamma_Lut_Mode.lut"
                        chAcqLutName = "Gamma Lut"
                        Exit Select
                    Case "f"c
                        pLut.Reverse()
                        acqLutFileName = "Reverse_Lut_Mode.lut"
                        chAcqLutName = "Reverse Lut"
                        Exit Select
                    Case "g"c
                        Dim numEntries As Integer = 128
                        pLut.Roll(numEntries)
                        acqLutFileName = "Roll_Lut_Mode.lut"
                        chAcqLutName = "Roll Lut"
                        Exit Select
                    Case "h"c
                        Dim bitsToShift As Integer = 3
                        pLut.Shift(bitsToShift)
                        acqLutFileName = "Shift_Lut_Mode.lut"
                        chAcqLutName = "Shift Lut"
                        Exit Select
                    Case "i"c
                        Dim startIndex1 As Integer = 76
                        Dim endIndex1 As Integer = 179
                        Dim clipOutSide As Boolean = False
                        'TRUE
                        Dim minValue As SapData
                        Dim maxValue As SapData
                        minValue = SetDataValue(pBuffers, prmIndex)
                        maxValue = SetDataValue(pBuffers, prmIndex)
                        pLut.Slope(startIndex1, endIndex1, minValue, maxValue, clipOutSide)
                        acqLutFileName = "Slope_With_Range_Lut_Mode.lut"
                        chAcqLutName = "Slope With Range Lut"
                        Exit Select
                    Case "j"c
                        Dim treshValue As SapData
                        treshValue = SetDataValue(pBuffers, prmIndex)
                        pLut.Threshold(treshValue)
                        acqLutFileName = "Threshold_Single_Mode.lut"
                        chAcqLutName = "Threshold Single Lut"
                        Exit Select
                    Case "k"c
                        Dim treshValue1 As SapData
                        Dim treshValue2 As SapData
                        treshValue1 = SetDataValue(pBuffers, prmIndex)
                        treshValue2 = SetDataValue(pBuffers, prmIndex)
                        pLut.Threshold(treshValue1, treshValue2)
                        acqLutFileName = "Threshold_Double_Mode.lut"
                        chAcqLutName = "Threshold Double Lut"
                        Exit Select
                    Case Else
                        Console.WriteLine("" & Chr(10) & "Invalid selection!" & Chr(10) & "")
                        Exit Select
                End Select             
            End While

            pLut.Save(acqLutFileName)
            ' Save LUT to file (can be reloaded in the main demo)
            Console.WriteLine("" & Chr(10) & "")
            Return chAcqLutName
        End Function

        Public Shared Function GetLoadLUTFiles(ByVal pLut As SapLut, ByVal FileSave_list As ArrayList) As Boolean
            ' Ask questions to user to select LUT file 
            Dim fileName As String = ""
            Console.WriteLine("" & Chr(10) & "Do you want to load an existing LUT file? y/n (Yes/No)  " & Chr(10) & "")
            Dim keyQuestion As ConsoleKeyInfo = Console.ReadKey(True)
            Dim key As Char = keyQuestion.KeyChar

            If key = "n"c Then
                Return False
            ElseIf key = "y"c Then
                Console.WriteLine("" & Chr(10) & "Select the LUT file available: " & Chr(10) & "")
                GetLUTFilesSaved(".", FileSave_list)
                If FileSave_list.Count <> 0 Then
                    keyQuestion = Console.ReadKey(True)
                    key = keyQuestion.KeyChar
                    Dim fileNum As Integer = CInt(Val(key))
                    If (fileNum >= 1) AndAlso (fileNum <= FileSave_list.Count) Then
                        fileName = DirectCast(FileSave_list(fileNum - 1), String)
                        ' Load LUT (saved before in the main demo)
                        If pLut.Load(fileName) Then
                            Console.WriteLine("" & Chr(10) & "................")
                            Console.WriteLine(fileName + " loaded.")
                            Console.WriteLine(".................." & Chr(10) & "")
                            Return True
                        End If
                    Else
                        Console.WriteLine("" & Chr(10) & "Invalid selection!" & Chr(10) & "")
                        Return False
                    End If
                Else
                    Console.WriteLine("" & Chr(10) & "No Lut File Avaible. Please load an existing one" & Chr(10) & "")
                    Return False
                End If
            End If          
            Return False
        End Function

        Private Shared Sub GetLUTFilesSaved(ByVal Path As String, ByVal FileSave_list As ArrayList)

            Dim lutfiles As String() = Directory.GetFiles(Path, "*.lut")
            Dim configFileCount As Integer = 1

            For Each lutFileName As String In lutfiles
                FileSave_list.Add(lutFileName)
                Console.WriteLine(configFileCount.ToString() + ": " + lutFileName)
                configFileCount += 1
            Next
        End Sub
    End Class


    Public Class MyAcquisitionParams
        Public Sub New()
            m_serverName = ""
            m_resourceIndex = 0
            m_configFileName = ""
        End Sub

        Public Sub New(ByVal ServerName As String, ByVal ResourceIndex As Integer)
            m_serverName = ServerName
            m_resourceIndex = ResourceIndex
            m_configFileName = ""
        End Sub

        Public Sub New(ByVal ServerName As String, ByVal ResourceIndex As Integer, ByVal ConfigFileName As String)
            m_serverName = ServerName
            m_resourceIndex = ResourceIndex
            m_configFileName = ConfigFileName
        End Sub

        Public Property ConfigFileName() As String
            Get
                Return m_configFileName
            End Get
            Set(ByVal value As String)
                m_configFileName = value
            End Set
        End Property

        Public Property ServerName() As String
            Get
                Return m_serverName
            End Get
            Set(ByVal value As String)
                m_serverName = value
            End Set
        End Property

        Public Property ResourceIndex() As Integer
            Get
                Return m_resourceIndex
            End Get
            Set(ByVal value As Integer)
                m_resourceIndex = value
            End Set
        End Property

        Protected m_serverName As String
        Protected m_resourceIndex As Integer
        Protected m_configFileName As String
    End Class

End Namespace

