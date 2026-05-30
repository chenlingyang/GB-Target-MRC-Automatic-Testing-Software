Module FindCamera
    Sub SapManager_ServerNotify(ByVal sender As Object, ByVal args As SapServerNotifyEventArgs)
        Dim serverName As String = SapManager.GetServerName(args.ServerIndex)

        Select Case args.EventType
            Case SapManager.EventType.ServerNew
                Console.WriteLine(Environment.NewLine & "==> Camera {0} was connected for the first time", serverName)

            Case SapManager.EventType.ServerDisconnected
                Console.WriteLine(Environment.NewLine & "==> Camera {0} was disconnected", serverName)

            Case SapManager.EventType.ServerConnected
                Console.WriteLine(Environment.NewLine & "==> Camera {0} was reconnected", serverName)
        End Select
    End Sub

   Sub Main(ByVal args As String())
        Console.WriteLine("Sapera Console Find Camera Example (VB.NET version)")

        AddHandler SapManager.ServerNotify, AddressOf SapManager_ServerNotify
        Dim eventType As SapManager.EventType = SapManager.EventType.ServerNew Or _
            SapManager.EventType.ServerConnected Or SapManager.EventType.ServerDisconnected
        SapManager.ServerEventType = eventType

        If Not GetStartOptionsFromQuestions() Then
            Console.WriteLine(Environment.NewLine & "Press any key to terminate")
            Console.ReadKey()
            Return
        End If

        SapManager.ServerEventType = SapManager.EventType.None
        RemoveHandler SapManager.ServerNotify, AddressOf SapManager_ServerNotify
    End Sub

   Private Function GetStartOptionsFromQuestions() As Boolean
        Dim serverCount As Integer
        Dim cameraIndex As Integer = 0
        Dim serverName As String = ""
        Dim userDefinedName As String = ""
        Dim acqDevice As SapAcqDevice = Nothing

        SapManager.DisplayStatusMode = SapManager.StatusMode.Log

        While True
            Console.WriteLine(Environment.NewLine & "Press 1 to 4 to show detected camera(s)")
            Console.WriteLine("    1: By User defined Name")
            Console.WriteLine("    2: By Serial Number")
            Console.WriteLine("    3: By Server Name")
            Console.WriteLine("    4: By Model Name" & Environment.NewLine)
            Console.WriteLine("Press 'u' to find a GigE-Vision camera server name from its user defined name" & Environment.NewLine)
            Console.WriteLine("Press 'd' to detect new CameraLink cameras (not needed for other types of cameras)" & Environment.NewLine)
            Console.WriteLine("Press 'q' to quit")
            Console.WriteLine(Environment.NewLine & "........................................")

            Dim keypress As ConsoleKeyInfo = Console.ReadKey(True)
            Dim key As Char = keypress.KeyChar

            serverCount = SapManager.GetServerCount()
            cameraIndex = 0

            If key <> Chr(0) Then
                Dim choice As Integer = Val(key)    ' char-to-int conversion

                ' quit
                If keypress.KeyChar = "q"c Then
                    Return False

                    ' detect
                ElseIf keypress.KeyChar = "d"c Then
                    Console.Write(Environment.NewLine & "Detecting new CameraLink camera servers... ")
                    If SapManager.DetectAllServers(SapManager.DetectServerType.All) Then
                        ' let time for the detection to execute
                        Threading.Thread.Sleep(5000)
                        ' get the new server count
                        serverCount = SapManager.GetServerCount()
                        Console.WriteLine("complete" & Environment.NewLine)
                    Else
                        Console.WriteLine("failed" & Environment.NewLine)
                    End If
                    GetStartOptionsFromQuestions()
                ElseIf keypress.KeyChar = "u"c Then
                    Console.WriteLine("Please type the user defined name:" & Environment.NewLine)
                    userDefinedName = Console.ReadLine()
                    serverName = SapManager.GetServerName(userDefinedName)
                    If (serverName.Length > 0) Then
                        Console.WriteLine(Environment.NewLine & "Server name for {0} is {1}", userDefinedName, serverName)
                    Else
                        Console.WriteLine(Environment.NewLine & "No server found for {0}", userDefinedName)
                    End If
                ElseIf (choice >= 1) AndAlso (choice <= 4) Then
                        Select Case choice
                            Case 1
                                Console.WriteLine(Environment.NewLine & Environment.NewLine & "Cameras listed by User Defined Name:" & Environment.NewLine)
                                For serverIndex As Integer = 0 To serverCount - 1
                                    If SapManager.GetResourceCount(serverIndex, SapManager.ResourceType.AcqDevice) <> 0 Then
                                        Dim location As New SapLocation(SapManager.GetServerName(serverIndex), 0)
                                        acqDevice = New SapAcqDevice(location)

                                        ' Create acquisition device object
                                        Dim status As Boolean = acqDevice.Create()
                                        If (status And acqDevice.FeatureCount > 0) Then
                                            ' Get User Defined Name Feature Value
                                            status = acqDevice.GetFeatureValue("DeviceUserID", userDefinedName)
                                            Console.WriteLine("{0}/ {1}", cameraIndex + 1, IIf(status, userDefinedName, "N/A"))
                                            cameraIndex += 1
                                        End If

                                        ' Destroy acquisition device object
                                        If (Not acqDevice.Destroy()) Then
                                            Return False
                                        End If
                                    End If
                                Next
                                Exit Select
                            Case 2
                                Console.WriteLine(Environment.NewLine & Environment.NewLine & "Cameras listed by Serial Number:" & Environment.NewLine)
                                Dim serialNumberName As String = ""

                                For serverIndex As Integer = 0 To serverCount - 1
                                    If SapManager.GetResourceCount(serverIndex, SapManager.ResourceType.AcqDevice) <> 0 Then
                                        Dim location As New SapLocation(SapManager.GetServerName(serverIndex), 0)
                                        acqDevice = New SapAcqDevice(location)

                                        ' Create acquisition device object
                                        Dim status As Boolean = acqDevice.Create()
                                        If (status And acqDevice.FeatureCount > 0) Then
                                            ' Get Serial Number Feature Value
                                            status = acqDevice.GetFeatureValue("DeviceID", serialNumberName)
                                            Console.WriteLine("{0}/ {1}", cameraIndex + 1, IIf(status, serialNumberName, "N/A"))
                                            cameraIndex += 1
                                        End If

                                        ' Destroy acquisition device object
                                        If (Not acqDevice.Destroy()) Then
                                            Return False
                                        End If
                                    End If
                                Next
                                Exit Select
                            Case 3
                                Console.WriteLine(Environment.NewLine & Environment.NewLine & "Cameras listed by Server Name:" & Environment.NewLine)

                                For serverIndex As Integer = 0 To serverCount - 1
                                    If SapManager.GetResourceCount(serverIndex, SapManager.ResourceType.AcqDevice) <> 0 Then
                                        Dim location As New SapLocation(SapManager.GetServerName(serverIndex), 0)
                                        acqDevice = New SapAcqDevice(location)

                                        ' Create acquisition device object
                                        Dim status As Boolean = acqDevice.Create()
                                        If (status And acqDevice.FeatureCount > 0) Then
                                            ' Get Server Name Value
                                            Console.WriteLine("{0}/ {1}", cameraIndex + 1, SapManager.GetServerName(serverIndex))
                                            cameraIndex += 1
                                        End If

                                        ' Destroy acquisition device object
                                        If (Not acqDevice.Destroy()) Then
                                            Return False
                                        End If
                                    End If
                                Next
                                Exit Select
                            Case 4
                                Console.WriteLine(Environment.NewLine & Environment.NewLine & "Cameras listed by Model Name:" & Environment.NewLine)
                                Dim deviceModelName As String = ""

                                For serverIndex As Integer = 0 To serverCount - 1
                                    If SapManager.GetResourceCount(serverIndex, SapManager.ResourceType.AcqDevice) <> 0 Then
                                        Dim location As New SapLocation(SapManager.GetServerName(serverIndex), 0)
                                        acqDevice = New SapAcqDevice(location)

                                        ' Create acquisition device object
                                        Dim status As Boolean = acqDevice.Create()
                                        If (status And acqDevice.FeatureCount > 0) Then
                                            ' Get Serial Number Feature Value
                                            status = acqDevice.GetFeatureValue("DeviceModelName", deviceModelName)
                                            Console.WriteLine("{0}/ {1}", cameraIndex + 1, IIf(status, deviceModelName, "N/A"))
                                            cameraIndex += 1
                                        End If

                                        ' Destroy acquisition device object
                                        If (Not acqDevice.Destroy()) Then
                                            Return False
                                        End If
                                    End If
                                Next
                                Exit Select
                            Case Else
                                Return False
                        End Select
                        If cameraIndex = 0 Then
                            Console.WriteLine("No camera found !")
                        End If
                        If acqDevice IsNot Nothing Then
                            acqDevice.Dispose()
                        End If
                Else
                        Console.WriteLine("Invalid selection!")
                        If acqDevice IsNot Nothing Then
                            acqDevice.Dispose()
                        End If
                    End If
            Else
                Console.WriteLine("Invalid selection!")
                If acqDevice IsNot Nothing Then
                    acqDevice.Dispose()
                End If
            End If
        End While
        Return True
    End Function

   Private Sub DestroyObjects(ByVal pAcqDevice As SapAcqDevice)
      If pAcqDevice IsNot Nothing Then
         pAcqDevice.Destroy()
         pAcqDevice.Dispose()
      End If
   End Sub


End Module
