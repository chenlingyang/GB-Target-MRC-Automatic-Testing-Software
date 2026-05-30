// CameraFeatures.cpp 
// This example gives the user an insight on how to access features from a camera.
// The program will retrieve a list of features and display their access mode, data type and value.
// There is also an example of how to change the value of a feature as well as an example of how 
// to trigger a callback once a value has been changed.
//
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.Examples.NET.Utils;

namespace DALSA.SaperaLT.Examples.NET.CSharp.CameraFeatures
{

    class Program
    {
        public const int TABLE1_LIMIT = 13;
        public const int TABLE2_LIMIT = 4;

        static void AcqDeviceCallback(Object sender, SapAcqDeviceNotifyEventArgs args)
        {
            SapAcqDevice acqDevice = sender as SapAcqDevice;
            Console.WriteLine("AcqDeviceNotify event \"{0}\", Feature = \"{1}\"",
                acqDevice.EventNames[args.EventIndex], acqDevice.FeatureNames[args.FeatureIndex]);

        }

        static void Main(string[] args)
        {
            MyAcquisitionParams acqParams = new MyAcquisitionParams();
            Boolean isGenie = false, status;
            SapFeature feature;
            
            //list of features based on SFNC
            string[] featuresNamesSFNCTable1 = new string[] 
	        {
	            "DeviceVendorName",
                "DeviceModelName",
                "DeviceVersion",
		        "DeviceFirmwareVersion",
		        "DeviceID",
		        "DeviceUserID",
		        "DeviceScanType",
		        "sensorColorType",
		        "SensorWidth",
		        "SensorHeight",
		        "AcquisitionFrameRate",
		        "Gain",
                "BinningHorizontal"
	        };

            //list of features based on Genie naming convention
	        string[] featuresNamesGenieTable1 = new string[] 
	        {
		        "DeviceVendorName",
		        "DeviceModelName",
		        "DeviceVersion",
		        "FirmwareVersion",
		        "DeviceID",
		        "DeviceUserID",
		        "DeviceScanType",
		        "ColorType",
		        "SensorWidth",
		        "SensorHeight",
		        "FrameRate",
		        "Gain"
	        };


            //list of features based on SFNC - Table 2
            string[] featuresNamesSFNCTable2 = new string[] 
	        {
	            "DeviceVendorName",
                "sensorColorType",
		        "Gain",
                "BinningHorizontal"
	        };

            //list of features based on Genie - Table 2
            string[] featuresNamesGenieTable2 = new string[] 
	        {
		        "DeviceVendorName",
		        "DeviceScanType",
		        "Gain"
	        };

            string[] g_featuresNames = null;
            Console.WriteLine("Sapera Console Cameras Features Example (C# version)\n");

            if (!GetOptions(args, acqParams))
            {
                Console.WriteLine("\nPress any key to terminate");
                Console.ReadKey();
                return;
            }

            // Create a camera object and assign delegate
            SapLocation location = new SapLocation(acqParams.ServerName, acqParams.ResourceIndex);
            SapAcqDevice device = new SapAcqDevice(location, false);
            device.AcqDeviceNotify += new SapAcqDeviceNotifyHandler(AcqDeviceCallback);
            feature = new SapFeature(location);
            if (!device.Create())
            {
                DestroysObjects(device,feature);
                return;
            }

            
            if (!feature.Create())
            {
                DestroysObjects(device,feature);
                return;
            }

            int numberOfFeatures = device.FeatureCount;
            Console.Write("Total number of features present on the camera: {0}\n\n", numberOfFeatures);

            if (numberOfFeatures == 0)
            {
                Console.WriteLine("Press any key to terminate.");
                Console.ReadKey();
                DestroysObjects(device, feature);
                return;
            }

            do
            {
                Console.Write("\nPlease select one of the following options:\n\n");
                Console.Write("1. Display all {0} features names.\n", numberOfFeatures);
                Console.Write("2. Display detailed list of selected features.\n");
                Console.WriteLine("3. Exit program.");

                Boolean first = true, isAvailable = false;
                char key;
                do
                {
                    if (first)
                    {
                        ConsoleKeyInfo info = Console.ReadKey(true);
                        key = info.KeyChar;     
                        first = false;
                    }
                    else
                    {
                        Console.Write("Invalid choice ! Please try again ! \n");
                        ConsoleKeyInfo info = Console.ReadKey(true);
                        key = info.KeyChar;
                    }
                } while (key != '1' && key != '2' && key != '3');
                Console.WriteLine();
                if (key == '1')
                {
                    Console.Write("{0,-60}\n","NAME\n");
                    for (int featureIndex = 0; featureIndex < numberOfFeatures; featureIndex++)
                    {
                       Console.Write("{0}. {1,-60}\n", featureIndex, device.FeatureNames[featureIndex]);
                    }
                }
                else if (key == '2')
                {
                    Console.WriteLine(String.Format("{0,-20}\t{1,-10}\t{2,-10}\t{3}\n", "NAME", "ACCESS", "TYPE", "VALUE"));
                    string modelName;

                    status = device.IsFeatureAvailable("DeviceModelName");
                    if (status)
                    {
                        status = device.IsFeatureAvailable("FrameRate");
                        device.GetFeatureValue("DeviceModelName", out modelName);
                        if (status && modelName.Contains("Genie"))
                        {
                            g_featuresNames = featuresNamesGenieTable1;
                            isGenie = true;
                        }
                        else
                        {
                            g_featuresNames = featuresNamesSFNCTable1;
                        }
                    }
                    else
                    {
                        g_featuresNames = featuresNamesSFNCTable1;
                    }

                    int limit = TABLE1_LIMIT;
                    if (isGenie)
                        limit--;

                    for (int featureIndex = 0; featureIndex < limit; featureIndex++)
                    {
                        string enumString, featureStringValue = "";
                        int featureIntValue32 = 0 , enumValue = 0;
                        Int64 featureIntValue64 = 0;
                        double featureDoubleValue = 0.0;



                        //display feature name
                        status = false;
                        if (isAvailable = device.IsFeatureAvailable(g_featuresNames[featureIndex]))
                            status = device.GetFeatureInfo(g_featuresNames[featureIndex], feature);

                        if (status)
                        {
                            SapFeature.AccessMode myAccess = feature.DataAccessMode;
                            SapFeature.Type myType = feature.DataType;
                            SapFeature.IncrementType myIncrement = feature.ValueIncrementType;
                            Console.Write(String.Format("{0,-20}\t", feature.DisplayName));

                            switch (myAccess)
                            {
                                case SapFeature.AccessMode.Undefined:
                                    {
                                        Console.Write(String.Format("{0,-10}\t", "Undefined"));
                                        break;
                                    }
                                case SapFeature.AccessMode.RW:
                                    {
                                        Console.Write(String.Format("{0,-10}\t", "ReadWrite"));
                                        break;
                                    }
                                case SapFeature.AccessMode.RO:
                                    {
                                        Console.Write(String.Format("{0,-10}\t", "ReadOnly"));
                                        break;
                                    }
                                case SapFeature.AccessMode.WO:
                                    {
                                        Console.Write(String.Format("{0,-10}\t", "WriteOnly"));
                                        break;
                                    }
                                case SapFeature.AccessMode.NP:
                                    {
                                        Console.Write(String.Format("{0,-10}\t", "NotPresent"));
                                        break;
                                    }
                                case SapFeature.AccessMode.NE:
                                    {
                                        Console.Write(String.Format("{0,-10}\t", "NotEnabled"));
                                        continue;
                                    }

                            }

                            
                            switch (myType)
                            {
                                case SapFeature.Type.String:
                                    {
                                        Console.Write(String.Format("{0,-10}\t", feature.DataType));
                                        status = false;

                                        status = device.GetFeatureValue(g_featuresNames[featureIndex], out featureStringValue);
                                        if (status)
                                            Console.Write(String.Format("{0}\n", featureStringValue));
                                        break;
                                    }
                                case SapFeature.Type.Int32:
                                case SapFeature.Type.Int64:
                                case SapFeature.Type.Bool:
                                    {
                                        Console.Write(String.Format("{0,-10}\t", feature.DataType));
                                        if (feature.DataType == SapFeature.Type.Int64)
                                            status = device.GetFeatureValue(g_featuresNames[featureIndex], out featureIntValue64);
                                        else
                                            status = device.GetFeatureValue(g_featuresNames[featureIndex], out featureIntValue32);

                                        if (status)
                                            Console.Write(String.Format("{0}\n", feature.DataType == SapFeature.Type.Int64 ? featureIntValue64 : featureIntValue32));
                                        break;
                                    }
                                case SapFeature.Type.Enum:
                                    {
                                        Console.Write(String.Format("{0,-10}\t", feature.DataType));

                                        status = device.GetFeatureValue(g_featuresNames[featureIndex], out enumValue);
                                        if (status)
                                        {
                                            status = feature.GetEnumTextFromValue(enumValue, out enumString);
                                            Console.Write(String.Format("{0}\n", enumString));
                                        }

                                        break;
                                    }
                                case SapFeature.Type.Double:
                                    {
                                        Console.Write(String.Format("{0,-10}\t", feature.DataType));

                                        status = device.GetFeatureValue(g_featuresNames[featureIndex], out featureDoubleValue);
                                        if (status)
                                            Console.Write(String.Format("{0:f}\n", featureDoubleValue));
                                        break;
                                    }
                                default:
                                    {
                                        Console.Write(String.Format("{0}\n", "N/A"));
                                        break;
                                    }
                            }
                        }//end if
                    }//end for
                    Console.WriteLine("");
                    Console.WriteLine("");


                    //select the 2nd table of features
                    if (isGenie)
                        g_featuresNames = featuresNamesGenieTable2;
                    else
                        g_featuresNames = featuresNamesSFNCTable2;


                    //display table 2
                    Console.WriteLine(String.Format("{0,-20}\t{1,-10}\t{2,-10}\t{3}\n", "NAME", "TYPE", "RANGE", "POSSIBLE VALUE(S)"));

                    limit = TABLE2_LIMIT;
                    if (isGenie)
                        limit--;


                    for (int featureIndex = 0; featureIndex < limit; featureIndex++)
                    {
                        string featureStringValue = "";
                        int featureIntValue32 = 0, enumValue = 0;
                        Int64 featureIntValue64 = 0;
                        double featureDoubleValue = 0.0;
                        


                        //display feature name
                        status = false;
                        if (isAvailable = device.IsFeatureAvailable(g_featuresNames[featureIndex]))
                            status = device.GetFeatureInfo(g_featuresNames[featureIndex], feature);

                        if (status)
                        {
                            SapFeature.Type myType = feature.DataType;
                            SapFeature.IncrementType myIncrement = feature.ValueIncrementType;

                            Console.Write(String.Format("{0,-20}\t", feature.DisplayName));

                            switch (myType)
                            {
                                case SapFeature.Type.String:
                                    {
                                        Console.Write(String.Format("{0,-10}\t", feature.DataType));
                                        status = false;
                                        int min, max;
                                        status = feature.GetValueMin(out min);
                                        status = feature.GetValueMax(out max);
                                        status = device.GetFeatureValue(g_featuresNames[featureIndex], out featureStringValue);
                                        if (status)
                                        {
                                            Console.Write(String.Format("{0} - {1,-10}\t", min,max));
                                            Console.Write(String.Format("{0}\n", featureStringValue));
                                        }
                                            
                                        break;
                                    }
                                case SapFeature.Type.Int32:
                                    {
                                        status = false;
                                        Console.Write(String.Format("{0,-10}\t", feature.DataType));
                                        Int32 min, max;

                                        status = feature.GetValueMin(out min);
                                        status = feature.GetValueMax(out max);
                                        status = device.GetFeatureValue(g_featuresNames[featureIndex], out featureIntValue32);
                                        if (status)
                                        {
                                            Console.Write(String.Format("{0} - {1,-10}\t", min, max));
                                            Console.Write(String.Format("{0}\n", featureIntValue32));
                                        }

                                        break;
                                    }
                                case SapFeature.Type.Int64:
                                    {
                                        status = false;
                                        Console.Write(String.Format("{0,-10}\t", feature.DataType));
                                        Int64 min, max;

                                        status = feature.GetValueMin(out min);
                                        status = feature.GetValueMax(out max);

                                        if (status)
                                        {
                                            Console.Write(String.Format("{0} - {1,-10}\t", min, max));
                                        }

                                        if (myIncrement == SapFeature.IncrementType.List)
                                        {
                                            for (int i = 0; i < feature.ValidValueCount; i++)
                                            {
                                                status = feature.GetValidValue(i, out featureIntValue64);
                                                if (i < feature.ValidValueCount - 1)
                                                    Console.Write(String.Format("{0}, ", featureIntValue64));
                                                else
                                                    Console.Write(String.Format("{0}\t", featureIntValue64));
                                            }
                                        }
                                        break;
                                    }
                                case SapFeature.Type.Enum:
                                    {
                                        Console.Write(String.Format("{0,-10}\t", feature.DataType));
                                        Console.Write(String.Format("{0,-10}\t", "N/A"));
                                        status = device.GetFeatureValue(g_featuresNames[featureIndex], out enumValue);
                                        for (int i = 0; i < feature.EnumCount; i++)
                                        {
                                            if (i < feature.EnumCount - 1)
                                                Console.Write(String.Format("{0}, ", feature.EnumText[i]));
                                            else
                                                Console.Write(String.Format("{0}\t", feature.EnumText[i]));
                                        }
                                        break;
                                    }
                                case SapFeature.Type.Double:
                                    {
                                        status = false;
                                        Console.Write(String.Format("{0,-10}\t", feature.DataType));
                                        double min, max;

                                        status = feature.GetValueMin(out min);
                                        status = feature.GetValueMax(out max);
                                        status = device.GetFeatureValue(g_featuresNames[featureIndex], out featureDoubleValue);
                                        if (status)
                                        {
                                            Console.Write(String.Format("{0:f} - {1:f}\t", min, max));
                                            Console.Write(String.Format("{0:f}\n", featureDoubleValue));
                                        }

                                        break;
                                    }
                                default:
                                    {
                                        Console.Write(String.Format("{0}\n", "N/A"));
                                        break;
                                    }
                            }
                        }//end if
                    }//end for
                    Console.WriteLine("");
                    Console.WriteLine("");


                    // Modify a feature (Will trigger callback function)
                    // Register event by name and a callback function will be performed when the event occurs
                    status = false;

                    if (device.IsFeatureAvailable("DeviceUserID"))
                    {
                        isAvailable = true;
                        if (device.GetFeatureInfo("DeviceUserID", feature))
                            status = true;
                    }

                    if (status && isAvailable && feature.DataAccessMode == SapFeature.AccessMode.RW)
                    {
                        String currentUserId;
                        if (!device.IsEventEnabled("Feature Value Changed"))
                            device.EnableEvent("Feature Value Changed");

                        // get DeviceUserID value from the device using the name of the feature
                        device.GetFeatureValue("DeviceUserID", out currentUserId);

                        // Set new value for DeviceUserID feature in device
                        device.SetFeatureValue("DeviceUserID", "Testdevice");

                        Console.WriteLine("Set DeviceUserID to Testdevice");

                        // Unregister event by name
                        if (device.IsEventEnabled("Feature Value Changed"))
                            device.DisableEvent("Feature Value Changed");

                        // Restore DeviceUserID value to old value
                        device.SetFeatureValue("DeviceUserID", currentUserId);
                        Console.WriteLine("Set DeviceUserID to old value: {0}", currentUserId);
                    }

                }
                else
                {
                    break;
                }
            
            }while(true); 
            Console.WriteLine("\nPress any key to terminate.");
            Console.ReadKey();

            DestroysObjects(device, feature);
        }//end main

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
