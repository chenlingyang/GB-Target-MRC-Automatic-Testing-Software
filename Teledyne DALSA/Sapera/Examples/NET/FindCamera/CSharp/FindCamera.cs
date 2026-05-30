using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using DALSA.SaperaLT.SapClassBasic;
using System.Threading;

namespace DALSA.SaperaLT.Examples.NET.CSharp.FindCamera
{
   class Program
   {

      public static void SapManager_ServerNotify(Object sender, SapServerNotifyEventArgs args)
      {
         string serverName = SapManager.GetServerName(args.ServerIndex);

         switch (args.EventType)
         {
            case SapManager.EventType.ServerNew:
               Console.WriteLine("\n==> Camera {0} was connected for the first time", serverName);
               break;

            case SapManager.EventType.ServerDisconnected:
               Console.WriteLine("\n==> Camera {0} was disconnected", serverName);
               break;

            case SapManager.EventType.ServerConnected:
               Console.WriteLine("\n==> Camera {0} was reconnected", serverName);
               break;
         }
      }

      static void Main(string[] args)
      {
         Console.WriteLine("Sapera Console Find Cameras Example (C# version)");

         SapManager.ServerNotify += new SapServerNotifyHandler(SapManager_ServerNotify);
         SapManager.EventType eventType = SapManager.EventType.ServerNew |
            SapManager.EventType.ServerConnected | SapManager.EventType.ServerDisconnected;
         SapManager.ServerEventType = eventType;

         if (!GetStartOptionsFromQuestions())
         {
            Console.WriteLine("\nPress any key to terminate");
            Console.ReadKey();
            return;
         }

         SapManager.ServerEventType = SapManager.EventType.None;
         SapManager.ServerNotify -= new SapServerNotifyHandler(SapManager_ServerNotify);
      }

      static bool GetStartOptionsFromQuestions()
      {
         int serverCount = 0;
         int cameraIndex = 0;
         string serverName = "";
         string userDefinedName = "";
         SapAcqDevice acqDevice = null;

         SapManager.DisplayStatusMode = SapManager.StatusMode.Log;

         while (true)
         {
            Console.WriteLine("\nPress 1 to 4 to show detected camera(s)");
            Console.WriteLine("    1: By User defined Name");
            Console.WriteLine("    2: By Serial Number");
            Console.WriteLine("    3: By Server Name");
            Console.WriteLine("    4: By Model Name\n");
            Console.WriteLine("Press 'u' to find a GigE-Vision camera server name from its user defined name\n");
            Console.WriteLine("Press 'd' to detect new CameraLink cameras (not needed for other types of cameras)\n");
            Console.WriteLine("Press 'q' to quit");
            Console.WriteLine("\n........................................");

            ConsoleKeyInfo keypress = Console.ReadKey(true);
            Char key = keypress.KeyChar;

            serverCount = SapManager.GetServerCount();
            cameraIndex = 0;

            if (key != 0)
            {
               int choice = key - '0'; // char-to-int conversion

               // quit
               if (key == 'q')
                  return false;

               // detect
               else if (key == 'd')
               {
                  Console.Write("\nDetecting new CameraLink camera servers... ");
                  if (SapManager.DetectAllServers(SapManager.DetectServerType.All))
                  {
                     // let time for the detection to execute
                     Thread.Sleep(5000);
               
                     // get the new server count
                     serverCount = SapManager.GetServerCount();
               
                     Console.WriteLine("complete\n");
                  }
                  else
                  {
                     Console.WriteLine("failed\n");
                  }
               }

               // Find server name from user defined name
               else if (key == 'u')
               {
                  Console.WriteLine("Please type the user defined name:\n");
                  userDefinedName = Console.ReadLine();
	              serverName = SapManager.GetServerName(userDefinedName);
                  if (serverName.Length > 0)
                     Console.WriteLine("\nServer name for {0} is {1}", userDefinedName, serverName);
                  else
                     Console.WriteLine("\nNo server found for {0}", userDefinedName);
               }

               else if ((choice >= 1) && (choice <= 4))
               {
                  switch (choice)
                  {
                     case 1:
                        {
                           Console.WriteLine("\n\nCameras listed by User Defined Name:\n");

                           for (int serverIndex = 0; serverIndex < serverCount; serverIndex++)
                           {
                              if (SapManager.GetResourceCount(serverIndex, SapManager.ResourceType.AcqDevice) != 0)
                              {
                                 SapLocation location = new SapLocation(SapManager.GetServerName(serverIndex), 0);
                                 acqDevice = new SapAcqDevice(location);

                                 // Create acquisition device object
                                 bool status = acqDevice.Create();
                                 if (status && acqDevice.FeatureCount > 0)
                                 {
                                    // Get User Defined Name Feature Value
                                    status = acqDevice.GetFeatureValue("DeviceUserID", out userDefinedName);
                                    Console.WriteLine("{0}/ {1}", cameraIndex + 1, status ? userDefinedName : "N/A");
                                    cameraIndex++;
                                 }

                                 // Destroy acquisition device object
                                 if (!acqDevice.Destroy())
                                    return false;
                              }
                           }
                           break;
                        }
                     case 2:
                        {
                           Console.WriteLine("\n\nCameras listed by Serial Number:\n");
                           string serialNumberName = "";

                           for (int serverIndex = 0; serverIndex < serverCount; serverIndex++)
                           {
                              if (SapManager.GetResourceCount(serverIndex, SapManager.ResourceType.AcqDevice) != 0)
                              {
                                 SapLocation location = new SapLocation(SapManager.GetServerName(serverIndex), 0);
                                 acqDevice = new SapAcqDevice(location);

                                 // Create acquisition device object
                                 bool status = acqDevice.Create();
                                 if (status && acqDevice.FeatureCount > 0)
                                 {
                                    // Get Serial Number Feature Value
                                    status = acqDevice.GetFeatureValue("DeviceID", out serialNumberName);
                                    Console.WriteLine("{0}/ {1}", cameraIndex + 1, status ? serialNumberName : "N/A");
                                    cameraIndex++;
                                 }

                                 // Destroy acquisition device object
                                 if (!acqDevice.Destroy())
                                    return false;
                              }
                           }
                           break;
                        }
                     case 3:
                        {
                           Console.WriteLine("\n\nCameras listed by Server Name:\n");

                           for (int serverIndex = 0; serverIndex < serverCount; serverIndex++)
                           {
                              if (SapManager.GetResourceCount(serverIndex, SapManager.ResourceType.AcqDevice) != 0)
                              {
                                 SapLocation location = new SapLocation(SapManager.GetServerName(serverIndex), 0);
                                 acqDevice = new SapAcqDevice(location);

                                 // Create acquisition device object
                                 bool status = acqDevice.Create();
                                 if (status && acqDevice.FeatureCount > 0)
                                 {
                                    // Get Server Name Value
                                    Console.WriteLine("{0}/ {1}", cameraIndex + 1, SapManager.GetServerName(serverIndex));
                                    cameraIndex++;
                                 }

                                 // Destroy acquisition device object
                                 if (!acqDevice.Destroy())
                                    return false;
                              }
                           }
                           break;
                        }
                     case 4:
                        {
                           Console.WriteLine("\n\nCameras listed by Model Name:\n");
                           string deviceModelName = "";

                           for (int serverIndex = 0; serverIndex < serverCount; serverIndex++)
                           {
                              if (SapManager.GetResourceCount(serverIndex, SapManager.ResourceType.AcqDevice) != 0)
                              {
                                 SapLocation location = new SapLocation(SapManager.GetServerName(serverIndex), 0);
                                 acqDevice = new SapAcqDevice(location);

                                 // Create acquisition device object
                                 bool status = acqDevice.Create();
                                 if (status && acqDevice.FeatureCount > 0)
                                 {
                                    // Get Model Name Feature Value
                                    status = acqDevice.GetFeatureValue("DeviceModelName", out deviceModelName);
                                    Console.WriteLine("{0}/ {1}", cameraIndex + 1, status ? deviceModelName : "N/A");
                                    cameraIndex++;
                                 }

                                 // Destroy acquisition device object
                                 if (!acqDevice.Destroy())
                                    return false;
                              }
                           }
                           break;
                        }
                  }


                  if (cameraIndex == 0)
                     Console.WriteLine("No camera found !");
                  if (acqDevice != null)
                     acqDevice.Dispose();
               }
               else
               {
                  Console.WriteLine("Invalid selection!");
                  if (acqDevice != null)
                     acqDevice.Dispose();
               }
            }
            else
            {
               Console.WriteLine("Invalid selection!");
               if (acqDevice != null)
                  acqDevice.Dispose();
            }
         }
      }
   }
}
