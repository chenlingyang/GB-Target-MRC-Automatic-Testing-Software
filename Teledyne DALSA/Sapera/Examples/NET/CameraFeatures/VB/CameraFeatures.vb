Module CameraFeatures
    Const TABLE1_LIMIT As Integer = 13
    Const TABLE2_LIMIT As Integer = 4


    Sub AcqDeviceCallback(ByVal sender As Object, ByVal args As SapAcqDeviceNotifyEventArgs)
        Dim acqDevice As SapAcqDevice = TryCast(sender, SapAcqDevice)
        Console.WriteLine("AcqDeviceNotify event ""{0}"", Feature = ""{1}""", acqDevice.EventNames(args.EventIndex), acqDevice.FeatureNames(args.FeatureIndex))
    End Sub

    Sub Main(ByVal args As String())
        Dim feature As SapFeature = Nothing
        Dim device As SapAcqDevice = Nothing
        Dim acqParams As New MyAcquisitionParams()



        'list of features based on SFNC
        Dim featuresNamesSFNCTable1() As String = _
        {"DeviceVendorName", _
        "DeviceModelName", _
        "DeviceVersion", _
        "DeviceFirmwareVersion", _
        "DeviceID", _
        "DeviceUserID", _
        "DeviceScanType", _
        "sensorColorType", _
        "SensorWidth", _
        "SensorHeight", _
        "AcquisitionFrameRate", _
        "Gain", _
        "BinningHorizontal"}

        'list of features based on Genie naming convention
        Dim featuresNamesGenieTable1() As String = _
        {"DeviceVendorName", _
        "DeviceModelName", _
        "DeviceVersion", _
        "FirmwareVersion", _
        "DeviceID", _
        "DeviceUserID", _
        "DeviceScanType", _
        "ColorType", _
        "SensorWidth", _
        "SensorHeight", _
        "FrameRate", _
        "Gain"}

        'list of features based on SFNC table 2
        Dim featuresNamesSFNCTable2() As String = _
        {"DeviceVendorName", _
        "sensorColorType", _
        "Gain", _
        "BinningHorizontal"}

        'list of features based on Genie naming convention table 2
        Dim featuresNamesGenieTable2() As String = _
        {"DeviceVendorName", _
        "DeviceScanType", _
        "Gain"}

        Dim g_featuresNames() As String

        Console.WriteLine("Sapera Console Camera Features Example (VB.NET version)" & Chr(10) & "")

        If Not GetOptions(args, acqParams) Then
            Console.WriteLine("" & Chr(10) & "Press any key to terminate" & Chr(10) & "")
            Console.ReadKey()
            Return
        End If

        Dim location As New SapLocation(acqParams.ServerName, acqParams.ResourceIndex)
        ' Create a camera object and assign delegate
        device = New SapAcqDevice(location, acqParams.ConfigFileName)
        AddHandler device.AcqDeviceNotify, AddressOf AcqDeviceCallback
        If Not device.Create() Then
            DestroysObjects(device, feature)
            Return
        End If
        ' Create an empty feature object (to receive information)
        feature = New SapFeature(location)
        If Not feature.Create() Then
            DestroysObjects(device, feature)
            Return
        End If


        Dim numberOfFeatures As Integer
        numberOfFeatures = device.FeatureCount
        Console.WriteLine("Total number of features present on the camera: {0}", numberOfFeatures)

        If (numberOfFeatures = 0) Then
            Console.WriteLine("Press any key to terminate.")
            Console.ReadKey()
            DestroysObjects(device, feature)
            Return
        End If

        Dim status As Boolean
        Dim isGenie As Boolean

        Do While (True)
            Console.WriteLine()
            Console.WriteLine("Please select one of the following options:")
            Console.WriteLine()
            Console.WriteLine("1. Display all {0} feature names.", numberOfFeatures)
            Console.WriteLine("2. Display detailed list of selected features.")
            Console.WriteLine("3. Exit program")

            Dim first As Boolean = True, isAvailable As Boolean = False
            Dim key As Char = "0"
            Do While (key <> "1" And key <> "2" And key <> "3")
                If (first) Then

                    Dim info As ConsoleKeyInfo = Console.ReadKey(True)
                    key = info.KeyChar
                    first = False

                Else

                    Console.WriteLine("Invalid choice ! Please try again !")
                    Dim info As ConsoleKeyInfo = Console.ReadKey(True)
                    key = info.KeyChar

                End If
            Loop
            Console.WriteLine()

            If (key = "1") Then
                Console.WriteLine("{0,-60}", "NAME")
                Console.WriteLine()
                For featureIndex As Integer = 0 To numberOfFeatures - 1
                    Console.WriteLine("{0}. {1,-60}", featureIndex, device.FeatureNames(featureIndex))
                Next
            ElseIf (key = "2") Then
                Console.WriteLine("{0,-25}{1,-15}{2,-15}{3}", "NAME", "ACCESS", "TYPE", "VALUE")
                Console.WriteLine()

                Dim modelName As String = Nothing
                status = device.IsFeatureAvailable("DeviceModelName")
                If (status) Then
                    status = device.IsFeatureAvailable("FrameRate")
                    device.GetFeatureValue("DeviceModelName", modelName)
                    If (status And modelName.Contains("Genie")) Then
                        isGenie = True
                        g_featuresNames = featuresNamesGenieTable1
                    Else
                        g_featuresNames = featuresNamesSFNCTable1
                    End If
                Else
                    g_featuresNames = featuresNamesSFNCTable1
                End If

                Dim limit As Integer = TABLE1_LIMIT
                If (isGenie) Then
                    limit = limit - 1
                End If

                For featureIndex As Integer = 0 To limit - 1
                    Dim enumString As String = Nothing
                    Dim featureStringValue As String = Nothing
                    Dim featureIntValue32, enumValue As Integer
                    Dim featureIntValue64 As Int64
                    Dim featureDoubleValue As Double

                    'display feature name
                    status = False
                    isAvailable = device.IsFeatureAvailable(g_featuresNames(featureIndex))
                    If (isAvailable) Then
                        status = device.GetFeatureInfo(g_featuresNames(featureIndex), feature)
                    End If

                    If (status) Then
                        Console.Write("{0,-25}", feature.DisplayName)

                        Dim MyIncrement As SapFeature.IncrementType = feature.ValueIncrementType
                        Dim MyAccess As SapFeature.AccessMode = feature.DataAccessMode
                        Select Case MyAccess
                            Case SapFeature.AccessMode.Undefined
                                Console.Write("{0,-15}", "Undefined")
                            Case SapFeature.AccessMode.RW
                                Console.Write("{0,-15}", "ReadWrite")
                            Case SapFeature.AccessMode.RO
                                Console.Write("{0,-15}", "ReadOnly")
                            Case SapFeature.AccessMode.WO
                                Console.Write("{0,-15}", "WriteOnly")
                            Case SapFeature.AccessMode.NP
                                Console.Write("{0,-15}", "NotPresent")
                            Case SapFeature.AccessMode.NE
                                Console.Write("{0,-15}", "NotEnabled")
                                Continue For
                            Case Else
                                Console.Write("{0,-15}", "N/A")
                        End Select


                        Dim MyType As SapFeature.Type = feature.DataType
                        Select Case MyType
                            Case SapFeature.Type.String
                                Console.Write("{0,-15}", feature.DataType)
                                status = False
                                isAvailable = device.IsFeatureAvailable(g_featuresNames(featureIndex))
                                If (isAvailable) Then
                                    status = device.GetFeatureValue(g_featuresNames(featureIndex), featureStringValue)
                                    If (status) Then
                                        Console.Write("{0}", featureStringValue)
                                    End If
                                End If
                            Case SapFeature.Type.Int32, SapFeature.Type.Int64, SapFeature.Type.Bool
                                Console.Write("{0,-15}", feature.DataType)
                                status = False
                                isAvailable = device.IsFeatureAvailable(g_featuresNames(featureIndex))
                                If (isAvailable) Then
                                    If (feature.DataType = SapFeature.Type.Int64) Then
                                        status = device.GetFeatureValue(g_featuresNames(featureIndex), featureIntValue64)
                                    Else
                                        status = device.GetFeatureValue(g_featuresNames(featureIndex), featureIntValue32)
                                    End If
                                    If (status) Then
                                        Console.Write("{0}", IIf(feature.DataType = SapFeature.Type.Int64, featureIntValue64, featureIntValue32))
                                    End If
                                End If
                            Case SapFeature.Type.Enum
                                Console.Write("{0,-15}", feature.DataType)

                                status = False
                                isAvailable = device.IsFeatureAvailable(g_featuresNames(featureIndex))
                                If (isAvailable) Then
                                    status = device.GetFeatureValue(g_featuresNames(featureIndex), enumValue)
                                    If (status) Then
                                        status = feature.GetEnumTextFromValue(enumValue, enumString)
                                        Console.Write("{0}", enumString)
                                    End If
                                End If

                            Case SapFeature.Type.Double
                                Console.Write("{0,-15}", feature.DataType)
                                status = False
                                isAvailable = device.IsFeatureAvailable(g_featuresNames(featureIndex))
                                If (isAvailable) Then
                                    status = device.GetFeatureValue(g_featuresNames(featureIndex), featureDoubleValue)
                                    If (status) Then
                                        Console.Write("{0:f}", featureDoubleValue)
                                    End If
                                End If
                            Case Else
                                Console.Write("{0}", "N/A")
                        End Select
                        Console.WriteLine("")
                    End If

                Next
                Console.WriteLine("")
                Console.WriteLine("")

                'select the 2nd table of features
                If (isGenie) Then
                    g_featuresNames = featuresNamesGenieTable2
                Else
                    g_featuresNames = featuresNamesSFNCTable2
                End If


                'display table 2
                Console.WriteLine("{0,-25}{1,-15}{2,-15}{3}", "NAME", "TYPE", "RANGE", "POSSIBLE VALUE(S)")
                Console.WriteLine()

                limit = TABLE2_LIMIT
                If (isGenie) Then
                    limit = limit - 1
                End If

                For featureIndex As Integer = 0 To limit - 1
                    Dim enumString As String = Nothing
                    Dim featureStringValue As String = Nothing
                    Dim featureIntValue32, enumValue As Integer
                    Dim featureIntValue64 As Int64
                    Dim featureDoubleValue As Double

                    'display feature name
                    status = False
                    isAvailable = device.IsFeatureAvailable(g_featuresNames(featureIndex))
                    If (isAvailable) Then
                        status = device.GetFeatureInfo(g_featuresNames(featureIndex), feature)
                    End If

                    If (status) Then
                        Console.Write("{0,-25}", feature.DisplayName)

                        Dim MyIncrement As SapFeature.IncrementType = feature.ValueIncrementType
                        Dim MyType As SapFeature.Type = feature.DataType

                        Select Case MyType
                            Case SapFeature.Type.String
                                Console.Write("{0,-15}", feature.DataType)
                                status = False
                                Dim min, max As Integer

                                status = feature.GetValueMin(min)
                                status = feature.GetValueMax(max)
                                status = device.GetFeatureValue(g_featuresNames(featureIndex), featureStringValue)

                                If status Then
                                    Console.Write("{0} - {1,-11}", min, max)
                                    Console.Write("{0}", featureStringValue)
                                End If

                            Case SapFeature.Type.Int32
                                status = False
                                Console.Write(String.Format("{0,-15}", feature.DataType))
                                Dim min, max As Integer

                                status = feature.GetValueMin(min)
                                status = feature.GetValueMax(max)
                                status = device.GetFeatureValue(g_featuresNames(featureIndex), featureIntValue32)
                                If (status) Then

                                    Console.Write(String.Format("{0} - {1,-11}", min, max))
                                    Console.Write(String.Format("{0}", featureIntValue32))
                                End If

                            Case SapFeature.Type.Int64
                                Console.Write("{0,-15}", feature.DataType)
                                status = False
                                Dim min, max As Int64

                                status = feature.GetValueMin(min)
                                status = feature.GetValueMax(max)
                                If (status) Then
                                    Console.Write("{0} - {1,-11}", min, max)
                                End If

                                If (MyIncrement = SapFeature.IncrementType.List) Then
                                    For i As Integer = 0 To feature.ValidValueCount - 1
                                        status = feature.GetValidValue(i, featureIntValue64)
                                        If (i < feature.ValidValueCount - 1) Then
                                            Console.Write(String.Format("{0}, ", featureIntValue64))
                                        Else
                                            Console.Write(String.Format("{0}", featureIntValue64))
                                        End If
                                    Next

                                End If

                            Case SapFeature.Type.Enum
                                Console.Write(String.Format("{0,-15}", feature.DataType))
                                Console.Write(String.Format("{0,-15}", "N/A"))
                                status = device.GetFeatureValue(g_featuresNames(featureIndex), enumValue)
                                For i As Integer = 0 To feature.EnumCount - 1
                                    If (i < feature.EnumCount - 1) Then
                                        Console.Write(String.Format("{0}, ", feature.EnumText(i)))
                                    Else
                                        Console.Write(String.Format("{0}", feature.EnumText(i)))
                                    End If
                                Next
                            Case SapFeature.Type.Double
                                status = False
                                Console.Write(String.Format("{0,-15}", feature.DataType))
                                Dim min, max As Double

                                status = feature.GetValueMin(min)
                                status = feature.GetValueMax(max)
                                status = device.GetFeatureValue(g_featuresNames(featureIndex), featureDoubleValue)
                                If (status) Then
                                    Console.Write(String.Format("{0:f} - {1,-8:f}", min, max))
                                    Console.Write(String.Format("{0:f}", featureDoubleValue))
                                End If
                            Case Else
                                Console.Write("{0}", "N/A")
                        End Select
                        Console.WriteLine("")
                    End If

                Next


                Console.WriteLine("")

                ' Modify a feature (Will trigger callback function)
                ' Register event by name and a callback function will be performed when the event occurs
                status = False

                If device.IsFeatureAvailable("DeviceUserID") Then
                    isAvailable = True
                    If device.GetFeatureInfo("DeviceUserID", feature) Then
                        status = True
                    End If
                End If

                If (status And isAvailable And feature.DataAccessMode = SapFeature.AccessMode.RW) Then
                    Dim currentUserId As String = Nothing
                    If Not device.IsEventEnabled("Feature Value Changed") Then
                        device.EnableEvent("Feature Value Changed")
                    End If
                    ' get DeviceUserID value from the camera using the name of the feature
                    device.GetFeatureValue("DeviceUserID", currentUserId)

                    ' Set new value for DeviceUserID feature in camera
                    device.SetFeatureValue("DeviceUserID", "Testdevice")

                    Console.WriteLine("Set DeviceUserID to Testdevice")

                    ' Unregister event by name
                    If device.IsEventEnabled("Feature Value Changed") Then
                        device.DisableEvent("Feature Value Changed")
                    End If

                    ' Restore DeviceUserID value to old value
                    device.SetFeatureValue("DeviceUserID", currentUserId)
                    Console.WriteLine("Set DeviceUserID to old value: {0}", currentUserId)
                End If

                Else
            Exit Do
                End If
            Console.WriteLine("")
        Loop



        Console.WriteLine("Press any key to terminate.")
        Console.ReadKey()

        DestroysObjects(device, feature)
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
