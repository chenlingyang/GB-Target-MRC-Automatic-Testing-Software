Module CameraEvents

    Sub AcqDeviceCallback(ByVal sender As Object, ByVal args As SapAcqDeviceNotifyEventArgs)
        Dim acqDevice As SapAcqDevice = TryCast(sender, SapAcqDevice)
        Console.WriteLine("AcqDeviceNotify event ""{0}"", Feature = ""{1}""", acqDevice.EventNames(args.EventIndex), acqDevice.FeatureNames(args.FeatureIndex))
    End Sub

    Sub Main(ByVal args As String())
        Dim feature As SapFeature = Nothing
        Dim device As SapAcqDevice = Nothing
        Dim acqParams As New MyAcquisitionParams()
        Dim currentGainDouble As Double = 0
        Dim currentGainInt As Integer = 0
        Dim isGenie As Boolean
        Dim isAvailable As Boolean = False
        Dim isSFNCDeprecated As Boolean = False
        Dim status As Boolean = False

        Console.WriteLine("Sapera Console Cameras Events Example (VB.NET version)" & Chr(10) & "")

        If Not GetOptions(args, acqParams) Then
            Console.WriteLine("" & Chr(10) & "Press any key to terminate" & Chr(10) & "")
            Console.ReadKey()
            Return
        End If

        Dim location As New SapLocation(acqParams.ServerName, acqParams.ResourceIndex)
        ' Create a device object and assign delegate
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
        ' Browse event list
        Dim numEvents As Integer = device.EventCount
        Console.WriteLine("Available events are:" & Chr(10) & "")
        For eventIndex As Integer = 0 To numEvents - 1
            Console.WriteLine("  " + device.EventNames(eventIndex))
        Next
        Console.WriteLine()

        ' Register event by name
        If Not device.IsEventEnabled("Feature Value Changed") Then
            device.EnableEvent("Feature Value Changed")
        End If

        Dim modelName As String = ""
        status = device.IsFeatureAvailable("DeviceModelName")
        If (status) Then
            status = device.IsFeatureAvailable("FrameRate")
            device.GetFeatureValue("DeviceModelName", modelName)
            If (status And modelName.Contains("Genie")) Then
                isGenie = True
            End If
        End If

        isAvailable = device.IsFeatureAvailable("Gain")
        If (isAvailable) Then
            device.GetFeatureInfo("Gain", feature)
            GoTo Gain_available
        End If
        isAvailable = device.IsFeatureAvailable("GainRaw")
        If (isAvailable) Then
            device.GetFeatureInfo("GainRaw", feature)
            isSFNCDeprecated = True
        Else
            GoTo Feature_Not_Available
        End If

Gain_Available:
        If (isGenie Or isSFNCDeprecated) Then
            Dim gainMin As Integer = 0, gainExponent As Integer = 0, gainMax As Integer = 0, gainValue As Integer = 0, powValue As Double = 0
            feature.GetValueMax(gainMax)
            feature.GetValueMin(gainMin)
            gainExponent = feature.SiToNativeExp10
            powValue = Convert.ToDouble(Math.Pow(CSng(10), -gainExponent))

            ' Get current Gain value in camera
            If (isSFNCDeprecated) Then
                device.GetFeatureValue("GainRaw", currentGainInt)
            Else
                device.GetFeatureValue("Gain", currentGainInt)
            End If

            Console.WriteLine("" & Chr(10) & "Current gain value is " + Convert.ToString(currentGainInt) + "" & Chr(10) & "")

            gainValue = Convert.ToInt32(gainMax * powValue)
            If (gainValue = currentGainInt) Then
                gainValue = Convert.ToInt32(gainMin * powValue)
            End If

            If (isSFNCDeprecated) Then
                device.SetFeatureValue("GainRaw", gainMax)
            Else
                device.SetFeatureValue("Gain", gainMax)
            End If
            Console.WriteLine("" & Chr(10) & "Set Gain value to " + Convert.ToString(gainValue) + "" & Chr(10) & "")


        Else
            Dim gainMin As Double = 0, gainExponent As Integer = 0, gainMax As Double = 0, gainValue As Double = 0, powValue As Double = 0
            feature.GetValueMax(gainMax)
            feature.GetValueMin(gainMin)
            gainExponent = feature.SiToNativeExp10
            powValue = Convert.ToDouble(Math.Pow(CSng(10), -gainExponent))

            ' Get current Gain value in camera
            device.GetFeatureValue("Gain", currentGainDouble)
            Console.WriteLine("" & Chr(10) & "Current gain value is " + String.Format("{0:n2}", currentGainDouble) + "" & Chr(10) & "")

            'Set gain to max. if it's already at max, set it to min.
            'The 0.001 adjustment is to avoid rounded numbers larger/lower
            'than max/min which would trigger errors on the camera.
            gainValue = gainMax * powValue - 0.001
            If (gainValue = currentGainDouble) Then
                gainValue = gainMin * powValue + 0.001
            End If

            
            device.SetFeatureValue("Gain", gainValue)
            Console.WriteLine("" & Chr(10) & "Set Gain value to " + String.Format("{0:n2}", gainValue) + "" & Chr(10) & "")
        End If

        ' Unregister event by name
        If device.IsEventEnabled("Feature Value Changed") Then
            device.DisableEvent("Feature Value Changed")
        End If
Feature_Not_Available:

        Console.WriteLine("Press any key to terminate.")
        Console.ReadKey()

        ' Set Gain value to old value
        If (isAvailable) Then
            If (isGenie) Then
                device.SetFeatureValue("Gain", currentGainInt)
            ElseIf (isSFNCDeprecated) Then
                device.SetFeatureValue("GainRaw", currentGainInt)
            Else
                device.SetFeatureValue("Gain", currentGainDouble)
            End If
            Console.WriteLine("" & Chr(10) & "Set Gain to old value.")
        End If

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
