// CameraFiles.cpp 
// This example gives the user an insight on how to access files on the camera.



// Disable deprecated function warnings with Visual Studio 2005
#if defined(_MSC_VER) && _MSC_VER >= 1400
#pragma warning(disable: 4995)
#pragma warning(disable: 4127)
#endif

#include "stdio.h"
#include "conio.h"
#include "sapclassbasic.h"
#include "ExampleUtils.h"
#include <math.h>
#include <vector>
#include <string>

#define SELECT_FILE        0
#define SELECT_OPERATION   1
#define PERFORM_DELETE     2
#define PERFORM_UPLOAD     3
#define PERFORM_DOWNLOAD   4
#define MAX_FILE_NAME      64


// Restore deprecated function warnings with Visual Studio 2005
#if defined(_MSC_VER) && _MSC_VER >= 1400
#pragma warning(default: 4995)
#endif


int main(int argc, char* argv[])
{
   SapManager::SetCommandTimeout(1200000);
	printf("Sapera Console Camera Files Example (C++ version)\n");

	char acqServerName[CORSERVER_MAX_STRLEN]="";
	UINT32   acqDeviceNumber=0;
	if (!GetCorAcqDeviceOptionsFromQuestions(acqServerName, &acqDeviceNumber))
	{
		printf("\nPress any key to terminate\n");
		_getch(); 
		return 0;
	}

	BOOL status;
	int fileCount = 0;
   int nextMenu = 0;
   int fileNumber = 0;
   char currentFileName[MAX_FILE_NAME];

	// Create a SapAcqDevice object
	SapAcqDevice device(acqServerName);
	status = device.Create();

   if (!device.IsFileAccessAvailable() || !device.GetFileCount(&fileCount) || fileCount == 0)
   {
		printf("\nFile access is not available for this device\n");
		printf("Press any key to terminate\n");
		_getch(); 
      device.Destroy();
		return 0;
   }

	// Create an empty feature object (to receive information)
	SapFeature feature(acqServerName);
   
   while(1)
   {
     

      //Menus
      switch (nextMenu)
      {
         //Select File menu will list all the files present on the device.
         case SELECT_FILE:
         {
            char key;
            printf("\nPlease select the file you want to access or 'q' to quit:\n\n");

            //list all files. After indexes 1-9 use lowercase letters a,b,c..etc
            for (int i = 0; i < fileCount;i++)
            {
               memset(currentFileName, 0, sizeof(currentFileName));
               device.GetFileNameByIndex(i, &currentFileName[0], sizeof(currentFileName));
               if (i < 9)
			         printf("%d. %s\n",i+1,currentFileName);
		         else
			         printf("%c. %s\n", i+1 - 10 + 'a', currentFileName);
            }

            //read the user's choice
            do 
            {
               key = (char)_getch();
               if (key != 0)
               {
                  //quit
	               if (key == 'q')
                  {
                     // Destroy acquisition object
	                  if (!device.Destroy()) return FALSE;

	                  // Destroy feature object
	                  if (!feature.Destroy()) return FALSE;

                     return 0;
                  }

		            // Use numbers 0 to 9, then lowercase letters if there are more than 10 files
                  
                  if (key >= '1' && key <= '9')
                     fileNumber = key - '1'; // char-to-int conversion
                  else
                     fileNumber = key -'a' +9;

                  if ((fileNumber >= 0) && (fileNumber <= fileCount))
                  {
                     memset(currentFileName, 0, sizeof(currentFileName));
                     device.GetFileNameByIndex(fileNumber, &currentFileName[0], sizeof(currentFileName));
		               printf("\nYou selected %s\n\n",currentFileName);
                     break;
                  }
                  else
                  {
                     printf("Invalid selection!\n"); 
                     continue;
                  }
               }
               else
               {
                  printf("Invalid selection!\n");
                  return FALSE;
               }
            }while(1);

            nextMenu = SELECT_OPERATION;
            break;
         }
         //Select Operation  menu displays all the possible operations that can be performed on the selected file
         case SELECT_OPERATION:
         {
            UINT64 prop = 0;
            char key;
            status = false;
            BOOL canDownload = FALSE,canUpload = FALSE,canDelete = FALSE;
            printf("Please select an operation or 'q' to return to file selection:\n\n");
            
            //Based on the Access mode of the file, a user can upload or/and download it.
            device.GetFileProperty(fileNumber, SapAcqDevice::FilePropertyAccessMode, &prop);
            switch(prop)
            {
               case SapAcqDevice::FileAccessModeNone:
                  break;
               case SapAcqDevice::FileAccessModeReadOnly:
                  printf("d. Download file from the device.\n");
                  canDownload = TRUE;
                  break;
               case SapAcqDevice::FileAccessModeWriteOnly:
                  printf("u. Upload file to the device.\n");
                  canUpload = TRUE;
                  break;
               case SapAcqDevice::FileAccessModeReadWrite:
                  printf("d. Download file from the device.\n");
                  printf("u. Upload file to the device.\n");
                  canUpload = canDownload = TRUE;
                  break;
               default:
                  break;
            };

            //check to see if the file can be deleted
            SapFeature::AccessMode accessMode = SapFeature::AccessUndefined;
            device.IsFeatureAvailable("##FileOperationSelector.Delete",&status);
            if (status)
            {
               feature.Create();
               device.GetFeatureInfo("##FileOperationSelector.Delete",&feature);
               feature.GetAccessMode(&accessMode);
	            feature.Destroy();
            }
	         
            if (accessMode == SapFeature::AccessRO)
            {
               printf("x. Delete file from the device.\n");
               canDelete = TRUE;
            }

            printf("q. Return to file selection.\n\n");

            //read the user's choice
            do 
            {
               key = (char)_getch();
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
                     printf("Invalid selection!\n"); 
                     continue;
                  }
               }
               else
               {
                  printf("Invalid selection!\n");
                  continue;
               }
            }while(1);
            break;
         } 
         //Perform a file delete
         case PERFORM_DELETE:
         {
            if (device.DeleteDeviceFile(fileNumber) == TRUE)
               printf("The file delete completed successfully.\n");
            else
               printf("The file could not be deleted.\n");

            printf("Press any key to continue or 'q' to quit.\n");
            char key = (char)_getch();
            if (key == 'q')
            {
               // Destroy acquisition object
	            if (!device.Destroy()) return FALSE;

               // Destroy feature object
               if (!feature.Destroy()) return FALSE;

               return 0;
            }
            nextMenu = SELECT_FILE;
            break;
         }
         //perform a file upload
         case PERFORM_UPLOAD:
         {
            int writeReturn;
            char loadFileName[MAX_FILE_NAME];

            //get the name of the file the user is trying to upload. 
            printf ("Please type the name of the file to load, including the extension\n");
            printf ("Note: The file must exist in the current directory or the upload will fail.\n\n");
            
            gets_s (loadFileName);
            printf ("\nUploading...\n");
            printf ("Note: Depending on the file size and communication speed, the transfer could take many minutes, but must not be aborted. Please wait.\n\n");

            //build the path. The file has to be located in the same directory as the executable
            char pathName[MAX_PATH] = "";
            GetModuleFileName( NULL, pathName, MAX_PATH );
            char* slash = strrchr(pathName, '\\'); 
            if(slash)
            {
               *(slash + 1) = '\0';
            }
            CorStrncat(pathName,loadFileName,sizeof(pathName));
            
            //upload file to the device
            writeReturn = device.WriteFile((const char *) pathName, currentFileName);
            if (!writeReturn)
            {
               printf ("The selected file write access to the device failed!\n\n");
            }
            else
            {
               printf("\nThe file upload completed successfully.\n");
            }

            printf("Press any key to continue or 'q' to quit.\n");
            char key = (char)_getch();
            if (key == 'q')
            {
               // Destroy acquisition object
	            if (!device.Destroy()) return FALSE;

               // Destroy feature object
               if (!feature.Destroy()) return FALSE;

               return 0;
            }
            nextMenu = SELECT_FILE;
            break;
         }
         //perform a file download
         case PERFORM_DOWNLOAD:
         {
            int readReturn;
            char saveFileName[MAX_FILE_NAME];

            //get the name of the file to which to save.
            printf ("Please type the name of the file to save to, including the extension:\n");
            printf ("Note: The file will be downloaded in the current directory.\n\n");
         
            gets_s (saveFileName);
            saveFileName[sizeof(saveFileName) - 1] = '\0';
            printf ("\nDownloading...\n");
            printf ("Note: Depending on the file size and communication speed, the transfer could take many minutes, but must not be aborted. Please wait.\n\n");
            
            //Build the path to the downloaded file which will be saved in the same directory as the executable
             //build the path. The file has to be located in the same directory as the executable
            char pathName[MAX_PATH];
            pathName[0] = '\0';
            GetModuleFileName( NULL, pathName, MAX_PATH );
            pathName[sizeof(pathName) - 1] = '\0';
            char* slash = strrchr(pathName, '\\'); 
            if(slash)
            {
               *(slash + 1) = '\0';
            }
            CorStrncat(pathName,saveFileName,sizeof(pathName));

            //download file
            readReturn = device.ReadFile(currentFileName,(const char*)pathName);
           
            if (!readReturn)
            {
              printf("\nThe selected file read access from the device failed!\n\n");
            }
            else
            {
               printf("\nThe file download completed successfully.\n");
            }

            printf("Press any key to continue or 'q' to quit.\n");
            char key = (char)_getch();
            if (key == 'q')
            {
               // Destroy acquisition object
	            if (!device.Destroy()) return FALSE;

               // Destroy feature object
               if (!feature.Destroy()) return FALSE;

               return 0;
            }
            nextMenu = SELECT_FILE;
            break;
         }
      } //end switch
   }
	
}

