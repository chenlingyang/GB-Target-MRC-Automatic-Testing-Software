// -----------------------------------------------------------------------------------------
// Sapera Find Camera Example
// 
//		This program shows how to list all cameras detected when more
//		than one cameras are present, listing them by user name, serial number, model name or 
//		server name.
//
// -----------------------------------------------------------------------------------------

// Disable deprecated function warnings with Visual Studio 2005
#if defined(_MSC_VER) && _MSC_VER >= 1400
#pragma warning(disable: 4995)
#endif

#include <iostream>
#include <string>
#include <sstream>

#include "stdio.h"
#include "conio.h"
#include "sapclassbasic.h"
#include "ExampleUtils.h"

#include <vector>

// Restore deprecated function warnings with Visual Studio 2005
#if defined(_MSC_VER) && _MSC_VER >= 1400
#pragma warning(default: 4995)
#endif


static BOOL GetStartOptionsFromQuestions();

void ServerCallback(SapManCallbackInfo *pInfo)
{
   int serverIndex = pInfo->GetServerIndex();
   char serverName[64];
	SapManager::GetServerName(serverIndex, serverName, sizeof(serverName));

   switch (pInfo->GetEventType())
   {
      case SapManager::EventServerNew:
         printf("\n==> Camera %s was connected for the first time\n", serverName);
         break;

      case SapManager::EventServerDisconnected:
         printf("\n==> Camera %s was disconnected\n", serverName);
         break;

      case SapManager::EventServerConnected:
         printf("\n==> Camera %s was reconnected\n", serverName);
         break;
   }
}

int main(int argc, char* argv[])
{
   printf("Sapera Console Find Camera Example (C++ version)\n");

   // This prevents unnecessary unloading and reloading of the Sapera LT libraries
   // during execution of this program
   SapManager::Open();

   // Register callback to be notified of camera connection and disconnection events
	SapManager::RegisterServerCallback(SapManager::EventServerNew | SapManager::EventServerDisconnected | SapManager::EventServerConnected | SapManager::EventResourceInfoChanged, ServerCallback, NULL);

   if (!GetStartOptionsFromQuestions())
   {
      printf("\nPress any key to terminate\n");
      CorGetch(); 
      SapManager::Close();
      return 0;
   }

	SapManager::UnregisterServerCallback();
   SapManager::Close();
   return 1;	
}


static BOOL GetStartOptionsFromQuestions()
{
   int serverCount = 0;
   int cameraIndex = 0;
   char serverName[CORSERVER_MAX_STRLEN];
   char userDefinedName[STRING_LENGTH];
   int featureCount = 0;

   SapManager::SetDisplayStatusMode(SapManager::StatusLog);

   for (;;)
   {
      printf("\nPress 1 to 4 to show detected camera(s)\n");
      printf("    1: By User defined Name\n");
      printf("    2: By Serial Number\n");
      printf("    3: By Server Name\n");
      printf("    4: By Model Name\n\n");
      printf("Press 'u' to find a GigE-Vision camera server name from its user defined name\n\n");
      printf("Press 'd' to detect new CameraLink cameras (not needed for other types of cameras)\n\n");
      printf("Press 'q' to quit\n");
      printf("\n........................................\n");

      char key = (char)CorGetch();

      serverCount = SapManager::GetServerCount();
      cameraIndex = 0;
      featureCount = 0;

      if (key != 0)
      {
         int choice = key - '0'; // char-to-int conversion

         // quit
         if (key == 'q')
            return FALSE;

         // detect
         if (key == 'd')
         {
            printf("\nDetecting new CameraLink camera servers... ");
            if (SapManager::DetectAllServers(SapManager::DetectServerAll))
            {
               // let time for the detection to execute
               Sleep(5000);
            
               // get the new server count
               serverCount = SapManager::GetServerCount();
            
               printf("complete\n\n");
            }
            else
            {
               printf("failed\n\n");
            }
         }

         // Find server name from user defined name
         if (key == 'u')
         {
            printf("Please type the user defined name:\n\n");
            gets_s(userDefinedName);
	         if (SapManager::GetServerName(userDefinedName, serverName))
               printf("\nServer name for %s is %s\n", userDefinedName, serverName);
            else
               printf("\nNo server found for %s\n", userDefinedName);
         }

         else if ((choice >= 1) && (choice <= 4))
         {
            switch(choice)
            {
            case 1:
               {
                  printf("\nCameras listed by User Defined Name:\n\n");

                  for (int serverIndex = 0; serverIndex < serverCount; serverIndex++)
                  {
                     if (SapManager::GetResourceCount(serverIndex, SapManager::ResourceAcqDevice) != 0)
                     {
                        SapManager::GetServerName(serverIndex, serverName, sizeof(serverName));

                        SapAcqDevice camera(serverName);
                        BOOL status = camera.Create();
                        if (status)
                           status = camera.GetFeatureCount(&featureCount);
                        if (status && featureCount > 0)
                        {
                           // Get user defined name
                           status = camera.GetFeatureValue("DeviceUserID", userDefinedName, sizeof(userDefinedName));
                           printf("%d/ %s\n", cameraIndex+1, status ? userDefinedName : "N/A");
                           cameraIndex++;
                        }

                        // Destroy acquisition device object
                        if (!camera.Destroy()) return FALSE;
                     }
                  }
                  break;
               }
            case 2:
               {
                  printf("\nCameras listed by Serial Number:\n\n");
                  char serialNumberName[STRING_LENGTH];
                  for (int serverIndex = 0; serverIndex < serverCount; serverIndex++)
                  {
                     if (SapManager::GetResourceCount(serverIndex, SapManager::ResourceAcqDevice) != 0)
                     {
                        SapManager::GetServerName(serverIndex, serverName, sizeof(serverName));

                        SapAcqDevice camera(serverName);
                        BOOL status = camera.Create();
                        if (status)
                           status = camera.GetFeatureCount(&featureCount);
                        if (status && featureCount > 0)
                        {
                           // Get Serial Number Feature Value
                           status = camera.GetFeatureValue("DeviceID", serialNumberName, sizeof(serialNumberName));
                           printf("%d/ %s\n", cameraIndex+1, status ? serialNumberName : "N/A");
                           cameraIndex++;
                        }

                        // Destroy acquisition device object
                        if (!camera.Destroy()) return FALSE;

                     }
                  }

                  break;
               }
            case 3:
               {
                  printf("\nCameras listed by Server Name:\n\n");
                  for (int serverIndex = 0; serverIndex < serverCount; serverIndex++)
                  {
                     if (SapManager::GetResourceCount(serverIndex, SapManager::ResourceAcqDevice) != 0)
                     {
                        // Get Server Name Value
                        SapManager::GetServerName(serverIndex, serverName, sizeof(serverName));
                        SapAcqDevice camera(serverName);
                        BOOL status = camera.Create();
                        if (status)
                           status = camera.GetFeatureCount(&featureCount);
                        if (status && featureCount > 0)
                        {
                           printf("%d/ %s\n", cameraIndex+1, serverName);
                           cameraIndex++;
                        }
                        // Destroy acquisition device object
                        if (!camera.Destroy()) return FALSE;
                     }
                  }
                  break;
               }
            case 4:
               {
                  printf("\nCameras listed by Model Name:\n\n");
                  char deviceModelName[STRING_LENGTH];
                  for (int serverIndex = 0; serverIndex < serverCount; serverIndex++)
                  {
                     if (SapManager::GetResourceCount(serverIndex, SapManager::ResourceAcqDevice) != 0)
                     {
                        SapManager::GetServerName(serverIndex, serverName, sizeof(serverName));

                        SapAcqDevice camera(serverName);
                        BOOL status = camera.Create();
                        if (status)
                           status = camera.GetFeatureCount(&featureCount);
                        if (status && featureCount > 0)
                        {
                           // Get Model Name
                           status = camera.GetFeatureValue("DeviceModelName", deviceModelName, sizeof(deviceModelName));
                           printf("%d/ %s\n", cameraIndex+1, status ? deviceModelName : "N/A");
                           cameraIndex++;
                        }
                        // Destroy acquisition device object
                        if (!camera.Destroy()) return FALSE;
                     }
                  }
                  break;
               }
            }

            if (cameraIndex == 0)
               printf("No camera found !\n");
         }
         else
         {
            printf("\nInvalid selection!\n");
         }
      }
      else
      {
         printf("\nInvalid selection!\n");
      }
   }
}
