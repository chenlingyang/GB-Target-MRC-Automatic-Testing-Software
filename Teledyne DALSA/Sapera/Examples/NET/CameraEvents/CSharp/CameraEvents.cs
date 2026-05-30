using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.Examples.NET.Utils;

namespace DALSA.SaperaLT.Examples.NET.CSharp.CameraEvents
{
    class Program
    {
        static void AcqDeviceCallback(Object sender, SapAcqDeviceNotifyEventArgs args)
        {
            SapAcqDevice acqDevice = sender as SapAcqDevice;
            Console.WriteLine("AcqDeviceNotify event \"{0}\", Feature = \"{1}\"",
                acqDevice.EventNames[args.EventIndex], acqDevice.FeatureNames[args.FeatureIndex]);
        }

        static void Main(string[] args)
        {
            SapFeature feature = null;
            SapAcqDevice device = null;
            MyAcquisitionParams acqParams = new MyAcquisitionParams();
            
            Console.WriteLine("Sapera Console Cameras Events Example (C# version)\n");

            if (!GetOptions(args, acqParams))
            {
                Console.WriteLine("\nPress any key to terminate\n");
                Console.ReadKey();
                return;
            }

            SapLocation location = new SapLocation(acqParams.ServerName, acqParams.ResourceIndex);
            // Create a camera object and assign delegate
            device = new SapAcqDevice(location, acqParams.ConfigFileName);
            device.AcqDeviceNotify += new SapAcqDeviceNotifyHandler(AcqDeviceCallback);
            if (!device.Create())
            {
                DestroysObjects(device,feature);
                return;
            }
            // Create an empty feature object (to receive information)
            feature = new SapFeature(location);
            if (!feature.Create())
            {
                DestroysObjects(device,feature);
                return;
            }
            // Get the number of features provided by the camera
            int featureCount = device.FeatureCount;         
            // Browse event list
            int numEvents = device.EventCount;       
            Console.WriteLine("Available events are:\n");
            for (int eventIndex = 0; eventIndex < numEvents; eventIndex++)
            {             
                Console.WriteLine("  " + device.EventNames[eventIndex]);
            }
            Console.WriteLine();

            // Register event by name
            if (!device.IsEventEnabled("Feature Value Changed"))
                 device.EnableEvent("Feature Value Changed");

            // Modified a feature (Will trigger callback function)
            double currentGainDouble =0;
            int currentGainInt=0;
            Boolean isGenie = false, isAvailable = false, isSFNCDeprecated = false, status = false;
            string modelName;

            


            status = device.IsFeatureAvailable("DeviceModelName");
            if (status)
            {
                status = device.IsFeatureAvailable("FrameRate");
                device.GetFeatureValue("DeviceModelName", out modelName);
                if (status && modelName.Contains("Genie"))
                {
                    isGenie = true;
                }
            }

            status = false;
            isAvailable = device.IsFeatureAvailable("Gain");
            if (isAvailable)
                device.GetFeatureInfo("Gain", feature);
            else if (isAvailable = device.IsFeatureAvailable("GainRaw"))
            {
                device.GetFeatureInfo("GainRaw", feature);
                isSFNCDeprecated = true;
            }
            else
            {
                goto Feature_Not_Available;
            }

            if (isGenie || isSFNCDeprecated)
            {
                int gainMin = 0, gainExponent = 0, gainMax = 0, gainValue = 0;
                double powValue = 0;
                feature.GetValueMax(out gainMax);
                feature.GetValueMin(out gainMin);
                gainExponent = feature.SiToNativeExp10;
                powValue = Convert.ToDouble(Math.Pow((float)10, -gainExponent));
                
                // Get current Gain value in camera
                if (isSFNCDeprecated)
                    device.GetFeatureValue("GainRaw", out currentGainInt);
                else
                    device.GetFeatureValue("Gain", out currentGainInt);

                Console.WriteLine("\nCurrent gain value is " + Convert.ToString(currentGainInt) + "\n");

                gainValue = Convert.ToInt32(gainMax * powValue);
                if(currentGainInt == gainValue)
                    gainValue = Convert.ToInt32(gainMin * powValue);

                if(isSFNCDeprecated)
                    device.SetFeatureValue("GainRaw", gainMax);
                else
                    device.SetFeatureValue("Gain", gainMax);
                Console.WriteLine("\nSet Gain value to " + Convert.ToString(gainValue) + "\n");
                
            }
            else 
            {
                double gainMin = 0, gainMax = 0, gainValue = 0, powValue = 0;
                int gainExponent = 0;
                feature.GetValueMax(out gainMax);
                feature.GetValueMin(out gainMin);
                gainExponent = feature.SiToNativeExp10;
                powValue = Convert.ToDouble(Math.Pow((float)10, -gainExponent));
                
                // Get current Gain value in camera
                device.GetFeatureValue("Gain", out currentGainDouble);
                Console.WriteLine("\nCurrent gain value is " + String.Format("{0:0.00}",currentGainDouble) + "\n");

                //Set gain to max. if it's already at max, set it to min.
                //The 0.001 adjustment is to avoid rounded numbers larger/lower
                //than max/min which would trigger errors on the camera.
                gainValue = gainMax * powValue - 0.001;
                if (currentGainDouble == gainValue)
                    gainValue = gainMin * powValue + 0.001;

                device.SetFeatureValue("Gain", gainValue);
                Console.WriteLine("\nSet Gain value to " + String.Format("{0:0.00}", gainValue) + "\n");
            }
            
Feature_Not_Available:
            // Unregister event by name
            if (device.IsEventEnabled("Feature Value Changed"))
               device.DisableEvent("Feature Value Changed");

            Console.WriteLine("Press any key to terminate.");
            Console.ReadKey();

            if (isAvailable)
            {
                // Set Gain value to old value
                if (isGenie)
                    device.SetFeatureValue("Gain", currentGainInt);
                else if (isSFNCDeprecated)
                    device.SetFeatureValue("GainRaw", currentGainInt);
                else
                    device.SetFeatureValue("Gain", currentGainDouble);
                Console.WriteLine("\nSet Gain to old value.");
            }

            DestroysObjects(device, feature);
        }

        static bool GetOptions(string[] args, MyAcquisitionParams acparams)
        {
            // Check if arguments were passed
            if (args.Length > 1)
                return ExampleUtils.GetAcqDeviceOptionsFromCommandLine(args, acparams);
            else
                return ExampleUtils.GetCorAcqDeviceOptionsFromQuestions(acparams, false);
        }

        static void DestroysObjects(SapAcqDevice acq, SapFeature feat)
        {
            if (acq != null)
            {
                acq.Destroy();
                acq.Dispose();
            }
            if (feat != null)
            {
                feat.Destroy();
                feat.Dispose();
            }       
           
        }

    }
}
