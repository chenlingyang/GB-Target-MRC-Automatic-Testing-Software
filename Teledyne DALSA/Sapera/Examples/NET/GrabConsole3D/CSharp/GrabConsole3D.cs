using System;

using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.Examples.NET.Utils;

namespace DALSA.SaperaLT.Examples.NET.CSharp.GrabConsole3D
{
    class GrabConsole3D
    {
        //Constants
        static int CallbackNumber = 0;

        //Array containing 4 buffers, 1 for each component A(X), C(Z), R(Reflectance), W(unused)
        static SapBuffer[] Buffer_Components_Split = new SapBuffer[4];

        //Camera Features
        static string PixelFormat_FeatureText; // contains the feature Pixel Format enum text value
        static string DeviceScan_FeatureText;  // contains the feature Device Scan enum text value
        static SapFormat Pixel_Format_Feature;  // AC, C, CR or ACRW
        static double Line_Rate_Feature;        // contains the feature line rate value
        static int Image_Height_Feature;        // contains the feature image height value

        //Buffer Parameters
        static ExampleUtils.BUFFER_LAYOUT Buffer_Layout_Parameter;     //Range or Profile
        static int Invalid_Flag_C_Parameter;         // Flag (0 or else) expressing if invalid values must be taking into account
        static double Invalid_Value_C_Parameter;     // The Z value flagged as invalid
        static double Scale_A_Parameter;             // Scale from Pixel to World according to the A (X) component
        static double Offset_A_Parameter;            // Offset from Pixel to World according to the A (X) component
        static double Scale_C_Parameter;             // Scale from Pixel to World according to the C (Z) component
        static double Offset_C_Parameter;            // Offset from Pixel to World according to the C (Z) component
        static SapBuffer.Val Output_Mode_Parameter;  // Output Mode of the device

        static int Index_DisplayedComponent = 1;  //Index selected by user input : A = 0, C = 1, R = 2 , W = 3

        static void Main(string[] args)
        {
            Console.WriteLine("Sapera Console Grab Example (C# version)");
            Console.WriteLine("Show Acquisition and Display by Sapera LT.");
            Console.WriteLine("The Acquisition can come in 4 formats : C AC CR ACRW.");
            Console.WriteLine("The Demo display the chosen component: A(X) C(Z) R(Reflectance) W(unused).");
            Console.WriteLine("The Demo can handle profiles (1 line) or ranges (X height of 1 line).");
            Console.WriteLine("The selected Camera must be in LineScan3D and not use a Mono pixel format.");

            MyAcquisitionParams acqParams = new MyAcquisitionParams();
            // Call GetCorAcqDeviceOptionsFromQuestions to determine which acquisition device to use
            if (!ExampleUtils.GetCorAcqDeviceOptionsFromQuestions(acqParams, true))
            {
                Console.WriteLine();
                Console.WriteLine("Press any key to terminate.");
                Console.ReadKey(true);
                return;
            }

            //The acquision device location
            SapLocation loc = new SapLocation(acqParams.ServerName, acqParams.ResourceIndex);
            //The acquision device
            SapAcqDevice AcqDevice = new SapAcqDevice(loc);

            //Setting up buffers and transfer
            SapBuffer AcqBuffers = new SapBufferWithTrash(2, AcqDevice, SapBuffer.MemoryType.ScatterGather);
            SapTransfer AcqDeviceToBuf = new SapAcqDeviceToBuf(AcqDevice, AcqBuffers);

            //Viewing tool buffer
            SapBuffer ViewBuffer = new SapBuffer();

            //Viewing tool
            SapView View = new SapView(ViewBuffer);
            // End of frame event
            AcqDeviceToBuf.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            AcqDeviceToBuf.XferNotify += new SapXferNotifyHandler(TransferCallback);

            object[] context = new object[2];
            context[0] = View;
            context[1] = AcqBuffers;
            AcqDeviceToBuf.XferNotifyContext = context;

            // Create acquisition object
            if (!AcqDevice.Create())
            {
                DestroysObjects(AcqDevice, AcqBuffers, ViewBuffer, AcqDeviceToBuf, View);
                return;
            }

            // Create buffer object
            if (!AcqBuffers.Create())
            {
                DestroysObjects(AcqDevice, AcqBuffers, ViewBuffer, AcqDeviceToBuf, View);
                return;
            }

            // Create transfer object
            if (!AcqDeviceToBuf.Create())
            {
                DestroysObjects(AcqDevice, AcqBuffers, ViewBuffer, AcqDeviceToBuf, View);
                return;
            }

            //Get Non 3D Camera Features
            if (!GrabConsole3D.InitializeCameraFeatures(AcqDevice, AcqBuffers))
            {
                DestroysObjects(AcqDevice, AcqBuffers, ViewBuffer, AcqDeviceToBuf, View);
                return;
            }

            //Get 3D Camera Features
            if (!ExampleUtils.GetParametersFrom3DBuffer(AcqBuffers, ref Invalid_Flag_C_Parameter, ref Invalid_Value_C_Parameter,
                ref Scale_A_Parameter, ref Offset_A_Parameter, ref Scale_C_Parameter, ref Offset_C_Parameter, 
                ref Buffer_Layout_Parameter, ref Output_Mode_Parameter))
            {
                DestroysObjects(AcqDevice, AcqBuffers, ViewBuffer, AcqDeviceToBuf, View);
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Buffer Parameters:");
            Console.WriteLine("InvalidFlag = {0}", Invalid_Flag_C_Parameter);
            Console.WriteLine("InvalidValue = {0}", Invalid_Value_C_Parameter);
            Console.WriteLine("Scale_A (X) = {0}", Scale_A_Parameter);
            Console.WriteLine("Offset_A (X) = {0}", Offset_A_Parameter);
            Console.WriteLine("Scale_C (Z) = {0}", Scale_C_Parameter);
            Console.WriteLine("Offset_C (Z) = {0}", Offset_C_Parameter);
            Console.WriteLine("BufferLayout = {0}", Buffer_Layout_Parameter.ToString());

            //Contains 1 buffer for each possible component
            //  Buffer 0 = A (X)
            //  Buffer 1 = C (Z)
            //  Buffer 2 = R (Reflectance)
            //  Buffer 3 = W (unused)
            for (int bufIndex = 0; bufIndex < 4; bufIndex++)
            {
               Buffer_Components_Split[bufIndex] = new SapBuffer();
               SetParameters(Buffer_Components_Split[bufIndex], 2, AcqBuffers.Width, AcqBuffers.Height, SapFormat.Mono16, SapBuffer.MemoryType.Default);
               if (!Buffer_Components_Split[bufIndex].Create())
               {
                   DestroysObjects(AcqDevice, AcqBuffers, ViewBuffer, AcqDeviceToBuf, View);
                   return;
               }
            }

            //Buffer of the Viewer. Created empty and manually modified
            //Buffer #1 = A (X)
            SetParameters(ViewBuffer, 1, AcqBuffers.Width, ExampleUtils.PROFILE_WINDOW_HEIGHT, SapFormat.Mono16, SapBuffer.MemoryType.Default);
            if (!ViewBuffer.Create())
            {
                DestroysObjects(AcqDevice, AcqBuffers, ViewBuffer, AcqDeviceToBuf, View);
                return;
            }

            if (!View.Create())
            {
                DestroysObjects(AcqDevice, AcqBuffers, ViewBuffer, AcqDeviceToBuf, View);
                return;
            }

            //Grab
            bool Grabbing = true;
            AcqDeviceToBuf.Grab();
            //During Grab, if PixelFormat has more than one component, you can toggle the displayed component.
            while (Grabbing)
            {
                switch (Pixel_Format_Feature)
                {
                    case SapFormat.Coord3D_C16:
                        Console.WriteLine();
                        Console.WriteLine("Any Key to stop Grabbing.");
                        Console.ReadKey(true);
                        Grabbing = false;
                        break;
                    case SapFormat.Coord3D_AC16:
                        Grabbing = ExampleUtils.GetSelectionFromConsoleAC(ref Index_DisplayedComponent);
                        break;
                    case SapFormat.Coord3D_ACRW16:
                        Grabbing = ExampleUtils.GetSelectionFromConsoleACRW(ref Index_DisplayedComponent);
                        break;
                }
            }
            //Freezing
            AcqDeviceToBuf.Freeze();
            if (!AcqDeviceToBuf.Wait(5000))
            {
                Console.WriteLine();
                Console.WriteLine("Grab could not stop properly.");
            }

            Console.WriteLine();
        }

        //Return the enum text value of a feature
        static bool GetTextFromEnumFeatureName(string FeatureName, SapAcqDevice AcqDevice, ref string FeatureText)
        {
            SapFeature Feature = new SapFeature(AcqDevice.Location);
            if (AcqDevice.IsFeatureAvailable(FeatureName) && Feature.Create())
            {
               if (AcqDevice.GetFeatureInfo(FeatureName, Feature))
               {
                  int enumvalue;
                  if (AcqDevice.GetFeatureValue(FeatureName, out enumvalue))
                     Feature.GetEnumTextFromValue(enumvalue, out FeatureText);
               }

               Feature.Destroy();
               return true;
            }

            return false;
        }

        //Get Non 3D Camera Features
        static bool InitializeCameraFeatures(SapAcqDevice AcqDevice, SapBuffer AcqBuffer)
        {
            //Get DeviceScanType Feature
            bool success = GetTextFromEnumFeatureName("DeviceScanType", AcqDevice, ref DeviceScan_FeatureText);

            SapBuffer.Val DeviceScanType = SapBuffer.Val.DEVICE_SCAN_TYPE_UNKNOWN;
            AcqBuffer.GetParameter(SapBuffer.Prm.DEVICE_SCAN_TYPE, out DeviceScanType);
            if (!success || DeviceScanType != SapBuffer.Val.DEVICE_SCAN_TYPE_LINESCAN3D)
            {
                Console.WriteLine();
                Console.WriteLine("This application only support cameras with Linescan3D as the Device Scan Type.");
                return false;
            }

            //Get PixelFormat Feature Value
            Pixel_Format_Feature = AcqBuffer.Format;
            PixelFormat_FeatureText = Pixel_Format_Feature.ToString();

            if (Pixel_Format_Feature != SapFormat.Coord3D_C16 &&
                Pixel_Format_Feature != SapFormat.Coord3D_AC16 &&
                Pixel_Format_Feature != SapFormat.Coord3D_ACRW16)
            {
               Console.WriteLine("This application doensn't support {0} Format.", PixelFormat_FeatureText);
               return false;
            }

            //Get AcquisitionFrameRate Feature
            //For LineScan3D, this is actually the line rate
            if (!AcqDevice.GetFeatureValue("AcquisitionFrameRate", out Line_Rate_Feature))
                return false;

            //Get Height Feature
            if (!AcqDevice.GetFeatureValue("Height", out Image_Height_Feature))
                return false;

            Console.WriteLine();
            Console.WriteLine("Camera features:");
            Console.WriteLine("PixelFormat = {0}", PixelFormat_FeatureText);
            Console.WriteLine("DeviceScanType = {0}", DeviceScan_FeatureText);
            Console.WriteLine("LineRate = {0}", Line_Rate_Feature);
            Console.WriteLine("Height = {0}", Image_Height_Feature);

            //Calculate callback rate (which could be quite slow), and display as information
            double CallbackFrequency = Image_Height_Feature / Line_Rate_Feature;
            Console.WriteLine();
            Console.WriteLine("Transfer callbacks will occur every {0} seconds", CallbackFrequency);

            return true;
        }

        //Called Each time a Buffer is available in the transfer. Splitting, Display and Processing
        static void TransferCallback(object sender, SapXferNotifyEventArgs args)
        {
            object[] contextContent = args.Context as object[];
            SapView pView = contextContent[0] as SapView;
            SapBuffer pBufferAcq = contextContent[1] as SapBuffer;
            int Pixel_Mean = 0;
            int World_Mean = 0;

            switch (Pixel_Format_Feature)  //Split the components, in the corresponding container,depending on the format
            {
                case SapFormat.Coord3D_C16:
                    Buffer_Components_Split[1].Copy(pBufferAcq);
                    break;
                case SapFormat.Coord3D_AC16:
                    pBufferAcq.SplitComponents(Buffer_Components_Split[0], Buffer_Components_Split[1], null);
                    break;
                case SapFormat.Coord3D_ACRW16:
                    pBufferAcq.SplitComponents(Buffer_Components_Split[0], Buffer_Components_Split[1], Buffer_Components_Split[2]);
                    break;
            }

            if (Buffer_Layout_Parameter == ExampleUtils.BUFFER_LAYOUT.Profile)
            {
                //Display a buffer, as a profile, chosen by the input
                ExampleUtilsUnsafe.DisplayProfile(pView, Buffer_Components_Split[Index_DisplayedComponent]); 
                //Process the mean of the pixels C(Z) with respect for the invalids (value and flag) parameters
                ExampleUtilsUnsafe.ProcessingProfile(Buffer_Components_Split[1], Invalid_Flag_C_Parameter, Invalid_Value_C_Parameter,
                Scale_C_Parameter, Offset_C_Parameter, ref Pixel_Mean, ref World_Mean);
            }
            else
            {
                ExampleUtils.DisplayRange(pView, Buffer_Components_Split[Index_DisplayedComponent]);
                //Pass the Component C to be processed
                ExampleUtilsUnsafe.ProcessingRange(Buffer_Components_Split[1], Invalid_Flag_C_Parameter, Invalid_Value_C_Parameter,
                Scale_C_Parameter, Offset_C_Parameter, ref Pixel_Mean, ref World_Mean);
            }

            Console.Write("\rCallback number = {0}, Pixel_Mean = {1}, World_Mean = {2}        ", CallbackNumber++, Pixel_Mean, World_Mean);
        }

        static void SetParameters(SapBuffer buffer, int count, int width, int height, SapFormat format, SapBuffer.MemoryType type)
        {
            buffer.Width = width;
            buffer.Count = count;
            buffer.Height = height;
            buffer.Format = format;
            buffer.Type = type;
        }

        static void DestroysObjects(SapAcqDevice AcqDevice, SapBuffer Buffer, SapBuffer ViewBuffer, SapTransfer Transfer, SapView View)
        {
            View.Destroy();
            View.Dispose();

            ViewBuffer.Destroy();
            ViewBuffer.Dispose();

            Transfer.Destroy();
            Transfer.Dispose();

            Buffer.Destroy();
            Buffer.Dispose();

            AcqDevice.Destroy();
            AcqDevice.Dispose();

            for (int bufIndex = 0; bufIndex < 4; bufIndex++)
            {
                if (Buffer_Components_Split[bufIndex] != null)
                {
                    Buffer_Components_Split[bufIndex].Destroy();
                    Buffer_Components_Split[bufIndex].Dispose();
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to terminate.");
            Console.ReadKey(true);
        }
    }
}
