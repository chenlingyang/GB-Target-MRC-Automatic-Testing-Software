using System;

using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.Examples.NET.Utils;

namespace DALSA.SaperaLT.Examples.NET.CSharp.GrabConsole3DWorld
{
    class GrabConsole3DWorld
    {
        //Constants
        static int CallbackNumber = 0;

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

        static void Main(string[] args)
        {
            Console.WriteLine("Sapera Console 3D Grab Example with world coordinates calculations (C#)");
            Console.WriteLine("The Acquisition can come in 4 formats : C AC CR ACRW.");
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

            // End of frame event
            AcqDeviceToBuf.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            AcqDeviceToBuf.XferNotify += new SapXferNotifyHandler(TransferCallback);

            AcqDeviceToBuf.XferNotifyContext = AcqBuffers;

            // Create acquisition object
            if (!AcqDevice.Create())
            {
                DestroysObjects(AcqDevice, AcqBuffers, AcqDeviceToBuf);
                return;
            }

            // Create buffer object
            if (!AcqBuffers.Create())
            {
                DestroysObjects(AcqDevice, AcqBuffers, AcqDeviceToBuf);
                return;
            }

            // Create transfer object
            if (!AcqDeviceToBuf.Create())
            {
                DestroysObjects(AcqDevice, AcqBuffers, AcqDeviceToBuf);
                return;
            }

            //Get Non 3D Camera Features
            if (!GrabConsole3DWorld.InitializeCameraFeatures(AcqDevice, AcqBuffers))
            {
                DestroysObjects(AcqDevice, AcqBuffers, AcqDeviceToBuf);
                return;
            }

            //Get 3D Camera Features
            if (!ExampleUtils.GetParametersFrom3DBuffer(AcqBuffers, ref Invalid_Flag_C_Parameter, ref Invalid_Value_C_Parameter,
                ref Scale_A_Parameter, ref Offset_A_Parameter, ref Scale_C_Parameter, ref Offset_C_Parameter, 
                ref Buffer_Layout_Parameter, ref Output_Mode_Parameter))
            {
                DestroysObjects(AcqDevice, AcqBuffers, AcqDeviceToBuf);
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

            //Grab
            AcqDeviceToBuf.Grab();
            Console.WriteLine();
            Console.WriteLine("Any Key to stop Grabbing.");
            Console.ReadKey(true);

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

            Console.WriteLine("Camera features:");
            Console.WriteLine("PixelFormat = {0}", PixelFormat_FeatureText);
            Console.WriteLine("DeviceScanType = {0}", DeviceScan_FeatureText);
            Console.WriteLine("LineRate = {0}", Line_Rate_Feature);
            Console.WriteLine("Height = {0}", Image_Height_Feature);

            //Calculate callback rate (which could be quite slow), and display as information
            double CallbackFrequency = (Image_Height_Feature / Line_Rate_Feature) * 1000.0;
            Console.WriteLine();
            Console.WriteLine("Transfer callbacks will occur every {0:F1} milliseconds", CallbackFrequency);

            return true;
        }

        static public unsafe void ReadPoint(SapBuffer Buffer, int xIndex, int yIndex, SapFormat Pixel_Format,
            double Scale_A, double Offset_A, double Scale_C, double Offset_C,
            SapBuffer.Val Output_Mode, ref int X, ref int Z, ref int R)
        {
            SapDataCoord3D value = new SapDataCoord3D();

            // Read native coordinates from buffer
            Buffer.ReadElement(xIndex, yIndex, value);

            switch (Pixel_Format)
            {
                case SapFormat.Coord3D_C16:
                {
                    Z = value.C * (int)Scale_C + (int)Offset_C;      // Real Z
                    if (Output_Mode == SapBuffer.Val.SCAN3D_OUTPUT_MODE_RECTIFIED_C)
                    {
                        // Real X (can be calculated from X index only in Rectified output mode)
                        X = xIndex * (int)Scale_A + (int)Offset_A;
                    }
                }
                break;
                case SapFormat.Coord3D_AC16:
                {
                    X = value.A * (int)Scale_A + (int)Offset_A;      // Real X
                    Z = value.C * (int)Scale_C + (int)Offset_C;      // Real Z
                }
                break;
                case SapFormat.Coord3D_ACRW16:
                {
                    X = value.A * (int)Scale_A + (int)Offset_A;      // Real X
                    Z = value.C * (int)Scale_C + (int)Offset_C;      // Real Z
                    R = value.R;                                     // Real R = Native R
                }
                break;
            }
        }

        static void TransferCallback(object sender, SapXferNotifyEventArgs args)
        {
            object contextContent = args.Context as object;
            SapBuffer BufferAcq = contextContent as SapBuffer;
            int X = 0, Z = 0, R = 0;

            if (Buffer_Layout_Parameter == ExampleUtils.BUFFER_LAYOUT.Profile)
            {
                //Read middle point of the profile
                int xIndex = BufferAcq.Width / 2;
                ReadPoint(BufferAcq, xIndex, 0, Pixel_Format_Feature, Scale_A_Parameter, Offset_A_Parameter,
                          Scale_C_Parameter, Offset_C_Parameter, Output_Mode_Parameter, ref X, ref Z, ref R);
            }
            else
            {
                //Read middle point of the range image
                int xIndex = BufferAcq.Width / 2;
                int yIndex = BufferAcq.Height / 2;
                ReadPoint(BufferAcq, xIndex, yIndex, Pixel_Format_Feature, Scale_A_Parameter, Offset_A_Parameter,
                          Scale_C_Parameter, Offset_C_Parameter, Output_Mode_Parameter, ref X, ref Z, ref R);
            }

            Console.Write("\rCallback number = {0}, X = {1}, Z = {2}, R = {3}      ", CallbackNumber++, X, Z, R);
        }

        static void SetParameters(SapBuffer buffer, int count, int width, int height, SapFormat format, SapBuffer.MemoryType type)
        {
            buffer.Width = width;
            buffer.Count = count;
            buffer.Height = height;
            buffer.Format = format;
            buffer.Type = type;
        }

        static void DestroysObjects(SapAcqDevice AcqDevice, SapBuffer Buffer, SapTransfer Transfer)
        {
            Transfer.Destroy();
            Transfer.Dispose();

            Buffer.Destroy();
            Buffer.Dispose();

            AcqDevice.Destroy();
            AcqDevice.Dispose();

            Console.WriteLine();
            Console.WriteLine("Press any key to terminate.");
            Console.ReadKey(true);
        }
    }
}
