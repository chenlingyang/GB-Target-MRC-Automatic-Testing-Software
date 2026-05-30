// CameraFiles.cs 
// This example gives the user an insight on how to access files on a camera.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.Examples.NET.Utils;

namespace DALSA.SaperaLT.Examples.NET.CSharp.CameraFiles
{

    class Program
    {
        public const int SELECT_FILE = 0;
        public const int SELECT_OPERATION = 1;
        public const int PERFORM_DELETE = 2;
        public const int PERFORM_UPLOAD = 3;
        public const int PERFORM_DOWNLOAD = 4;
        public const int MAX_FILE_NAME = 64;
             

        static void Main(string[] args)
        {
            SapManager.CommandTimeout = 1200000;
            MyAcquisitionParams acqParams = new MyAcquisitionParams();
            SapFeature feature;
            bool status;
            char key;
            int nextMenu = 0;
            int fileNumber = 0;
            String currentFileName = null;
            
            Console.Write("Sapera Console Camera Files Example (C# version)\n");

            if (!GetOptions(args, acqParams))
            {
                Console.Write("\nPress any key to terminate");
                Console.ReadKey();
                return;
            }

            // Create a camera object and assign delegate
            SapLocation location = new SapLocation(acqParams.ServerName, acqParams.ResourceIndex);
            SapAcqDevice device = new SapAcqDevice(location, false);
            feature = new SapFeature(location);
            if (!device.Create())
            {
                DestroysObjects(device,feature);
                return;
            }

            if (!device.FileAccessAvailable || device.FileCount == 0)
            {
                Console.WriteLine("\nFile access is not available for this device");
                Console.WriteLine("Press any key to terminate");
                Console.ReadKey();
                DestroysObjects(device, feature);
                return;
            }

            if (!feature.Create())
            {
                DestroysObjects(device,feature);
                return;
            }

            if (device.FileCount == 0)
                return;

            while(true)
            {
                

                //Menus
                switch(nextMenu)
                {
                    //Select File menu will list all the files present on the device.
                    case SELECT_FILE:
                    {
                        Console.Write("\nPlease select the file you want to access or 'q' to quit:\n\n");

                        //list all files. After indexes 1-9 use lowercase letters a,b,c..etc
                        for (int i = 0; i < device.FileCount;i++)
                        {
                            if (i < 9)
                                Console.Write("{0}. {1}\n", i + 1, device.FileNames[i]);
		                    else
                                Console.Write("{0}. {1}\n", Convert.ToChar(i + 1 - 10 + 'a'), device.FileNames[i]);
                        }

                        //read the user's choice
                        do 
                        {
                            ConsoleKeyInfo info = Console.ReadKey(true);
                            key = info.KeyChar;
                            if (key != 0)
                            {
                                //quit
	                            if (key == 'q')
                                {
                                    // Destroy acquisition object
	                                if (!device.Destroy()) return;

	                                // Destroy feature object
	                                if (!feature.Destroy()) return;

                                    return;
                                }

		                        // Use numbers 0 to 9, then lowercase letters if there are more than 10 files
                  
                                if (key >= '1' && key <= '9')
                                    fileNumber = key - '1'; // char-to-int conversion
                                else
                                    fileNumber = key -'a' +9;

                                if ((fileNumber >= 0) && (fileNumber <= device.FileCount))
                                {
                                    Console.Write("\nYou selected {0}\n\n", device.FileNames[fileNumber]);
                                    currentFileName = device.FileNames[fileNumber];
                                    break;
                                }
                                else
                                {
                                    Console.Write("Invalid selection!\n"); 
                                    continue;
                                }
                            }
                            else
                            {
                                Console.Write("Invalid selection!\n");
                                return;
                            }
                        }while(true);

                        nextMenu = SELECT_OPERATION;
                        break;
                    }
                     //Select Operation  menu displays all the possible operations that can be performed on the selected file
                    case SELECT_OPERATION:
                    {
                        SapAcqDevice.FilePropertyVal prop;
                        Console.Write("Please select an operation or 'q' to return to file selection:\n\n");
                        bool canDownload = false, canUpload = false, canDelete = false;
                        //Based on the Access mode of the file, a user can upload or/and download it.
                        device.GetFileProperty(fileNumber, SapAcqDevice.FileProperty.AccessMode, out prop);
                        switch(prop)
                        {
                           case SapAcqDevice.FilePropertyVal.AccessModeNone:
                              break;
                           case SapAcqDevice.FilePropertyVal.AccessModeReadOnly:
                              Console.Write("d. Download file from the device.\n");
                              canDownload = true;
                              break;
                           case SapAcqDevice.FilePropertyVal.AccessModeWriteOnly:
                              Console.Write("u. Upload file to the device.\n");
                              canUpload = true;
                              break;
                           case SapAcqDevice.FilePropertyVal.AccessModeReadWrite:
                              Console.Write("d. Download file from the device.\n");
                              Console.Write("u. Upload file to the device.\n");
                              canUpload = true;
                              canDownload = true;
                              break;
                           default:
                              break;
                        };

                        //check to see if the file can be deleted
                        SapFeature.AccessMode accessMode = 0;
                        status = device.IsFeatureAvailable("##FileOperationSelector.Delete");
                        if (status)
                        {
                            device.GetFeatureInfo("##FileOperationSelector.Delete", feature);
                            accessMode = feature.DataAccessMode;
                        }
                        if (accessMode == SapFeature.AccessMode.RO)
                        {
                            Console.Write("x. Delete file from the device.\n");
                            canDelete = true;
                        }

                        Console.Write("q. Return to file selection.\n\n");

                        //read the user's choice
                        do 
                        {
                           
                           ConsoleKeyInfo info = Console.ReadKey(true);
                           key = info.KeyChar;
                           if (key != 0)
                           {
                              //quit
	                           if (key == 'q')
                              {
                                 nextMenu = SELECT_FILE;
                                 break;
                              }
                              //download
		                        else if (key == 'd' && canDownload)
                              {
                                 nextMenu = PERFORM_DOWNLOAD;
                                 break;
                              }
                              //upload
                              else if (key == 'u' && canUpload)
                              {
                                 nextMenu = PERFORM_UPLOAD;
                                 break;
                              }
                              //delete
                              else if (key == 'x' && canDelete)
                              {
                                 nextMenu = PERFORM_DELETE;
                                 break;
                              }
                              else
                              {
                                 Console.Write("Invalid selection!\n"); 
                                 continue;
                              }
                           }
                           else
                           {
                              Console.Write("Invalid selection!\n");
                              continue;
                           }
                        }while(true);
                        break;
                    } 
                     //Perform a file delete
                    case PERFORM_DELETE:
                    {
                        if (device.DeleteDeviceFile(fileNumber) == true)
                           Console.Write("The file delete completed successfully.\n");
                        else
                           Console.Write("The file could not be deleted.\n");

                        Console.Write("Press any key to continue or 'q' to quit.\n");
                        ConsoleKeyInfo info = Console.ReadKey(true);
                        key = info.KeyChar;
                        if (key == 'q')
                        {
                           // Destroy acquisition object
	                        if (!device.Destroy()) return;

                           // Destroy feature object
                           if (!feature.Destroy()) return;

                           return;
                        }
                        nextMenu = SELECT_FILE;
                        break;
                     }
                    //perform a file upload
                    case PERFORM_UPLOAD:
                    {
                        bool writeReturn;
                        String loadFileName;

                        //get the name of the file the user is trying to upload. 
                        Console.Write ("Please type the name of the file to load, including the extension\n");
                        Console.Write ("Note: The file must exist in the current directory or the upload will fail.\n\n");

                        loadFileName = Console.ReadLine();
                        Console.Write ("\nUploading...\n");
                        Console.Write ("Note: Depending on the file size and communication speed, the transfer could take many minutes, but must not be aborted. Please wait.\n\n");
                        
                        //upload file to the device
                        writeReturn = device.WriteFile(".\\" + loadFileName, currentFileName);
                        if (!writeReturn)
                        {
                           Console.Write ("The selected file write access to the device failed!\n\n");
                        }
                        else
                        {
                           Console.Write("\nThe file upload completed successfully.\n\n");
                        }

                        Console.Write("Press any key to continue or 'q' to quit.\n");
                        ConsoleKeyInfo info = Console.ReadKey(true);
                        key = info.KeyChar;
                        if (key == 'q')
                        {
                           // Destroy acquisition object
	                        if (!device.Destroy()) return;

                           // Destroy feature object
                           if (!feature.Destroy()) return;

                           return;
                        }
                        nextMenu = SELECT_FILE;
                        break;
                    }
                    //perform a file download
                    case PERFORM_DOWNLOAD:
                    {
                        bool readReturn;
                        String saveFileName;

                        //get the name of the file to which to save.
                        Console.Write ("Please type the name of the file to save to, including the extension:\n");
                        Console.Write ("Note: The file will be downloaded in the current directory.\n\n");
                        saveFileName = Console.ReadLine();
                        Console.Write ("\nDownloading...\n");
                        Console.Write ("Note: Depending on the file size and communication speed, the transfer could take many minutes, but must not be aborted. Please wait.\n\n");
                        
                        //download file to current directory
                        readReturn = device.ReadFile(currentFileName,".\\"+saveFileName);
                       
                        if (!readReturn)
                        {
                          Console.Write("\nThe selected file read access from the device failed!\n\n");
                        }
                        else
                        {
                           Console.Write("\nThe file download completed successfully.\n");
                        }

                        Console.Write("Press any key to continue or 'q' to quit.\n");
                        ConsoleKeyInfo info = Console.ReadKey(true);
                        key = info.KeyChar;
                        if (key == 'q')
                        {
                           // Destroy acquisition object
	                        if (!device.Destroy()) return;

                           // Destroy feature object
                           if (!feature.Destroy()) return;

                           return;
                        }
                        nextMenu = SELECT_FILE;
                        break;
                    }
                } //end switch
            }//end while(true)
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
    }//end Program
}//end namespace

