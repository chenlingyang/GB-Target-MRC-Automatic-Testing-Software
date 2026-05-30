
// -----------------------------------------------------------------------------------------
// Sapera AcqDevice Grab Multicast Example
// 
//    This program shows how to enable multicast as a Master or a receiver
//		with CorAcqDevice.
//
// -----------------------------------------------------------------------------------------

// Disable the following Visual C++ compiler warning
//    4786 -> 'identifier' : identifier was truncated to 'number' characters in the debug information
#ifdef _MSC_VER
   #pragma warning(disable: 4786)
#endif

// Disable deprecated function warnings with Visual Studio 2005
#if defined(_MSC_VER) && _MSC_VER >= 1400
   #pragma warning(disable: 4995)
#endif

#include "stdio.h"
#include "conio.h"
#include "sapclassbasic.h"
#include "ExampleUtils.h"
#include <iostream>
#include <vector>
#include <string>

// Restore deprecated function warnings with Visual Studio 2005
#if defined(_MSC_VER) && _MSC_VER >= 1400
   #pragma warning(default: 4995)
#endif

// Static Functions
static void AcqCallback(SapXferCallbackInfo *pInfo);
static BOOL GetOptions(int argc, char *argv[], char *acqServerName, UINT32 *pAcqDeviceIndex);
static BOOL GetOptionsFromCommandLine(int argc, char *argv[], char *acqServerName, UINT32 *pAcqDeviceIndex);

int main(int argc, char* argv[])
{
   BOOL     master = FALSE;
   BOOL     selectionOk = FALSE;
   UINT32   acqDeviceNumber;
   char     acqServerName[CORSERVER_MAX_STRLEN];

   printf("Sapera Console GigE Camera Multicast Example (C++ version)\n");

   printf("\nIMPORTANT\n=========\n\n");
   printf("Even though the multicast features are available to all GigE-Vision cameras,\n");
   printf("this functionality is only supported for Genie Nano. Before proceeding, please\n");
   printf("make sure that you have carefully read the following application note:\n\n");
   printf("    G3-AN0008 Genie Nano Multicasting\n\n");
   printf("This document is available on the Support -> Documentation -> Application Notes\n");
   printf("section of www.teledynedalsa.com, or you can use the following direct link:\n\n");
   printf("https://info.teledynedalsa.com/acton/attachment/14932/f-09b4/1/-/-/-/-/G3-AN0008%%20-%%20Genie-Nano-How_to_Multicast_with_SaperaLT.pdf\n\n");
   printf("Unless you have correctly set up your cameras for multicast, press 'q' instead\n");
   printf("of selecting a camera\n\n");

   if (!GetOptions(argc, argv, acqServerName, &acqDeviceNumber))
   {
      printf("\nPress any key to terminate\n");
      CorGetch(); 
      return 0;
   }

   printf("M or m: Start as a master\n");
   printf("R or r: Start as a receiver\n");

   do
   {
      char key = (char)_getch();

      if (key != 0)
      {
         if (key == 'q')
            return FALSE;

         if ((key == 'm') || (key == 'M'))
         {
            printf("\n---> Starting as a MASTER <---\n\n");
            selectionOk = TRUE;
            master = TRUE;
         }

         if ((key == 'r') || (key == 'R'))
         {
            printf("\n---> Starting as a RECEIVER <---\n\n");
            selectionOk = TRUE;
         }
      }

   } while (!selectionOk);

   SapAcqDevice   AcqDevice;
   SapBuffer      Buffers;
   SapTransfer    Xfer;
   SapView        View;

   SapLocation loc(acqServerName);

   if (SapManager::GetResourceCount(acqServerName, SapManager::ResourceAcqDevice) > 0)
   {
      if (master)
      {
         // Create the Acquisition Device in read/write mode.
         // You must have set the "Camera Default Write Mode" to "Control"
         // with Network Configuration Tool.
         AcqDevice = SapAcqDevice(loc, FALSE);
      }
      else
      {
         // Create the Acquisition Device in read-only mode.
         // Make sure the Camera is properly set by the MASTER before
         // the Acquisition Device Creation.
         AcqDevice = SapAcqDevice(loc, TRUE);
         // No change must be done by the MASTER to the Camera configuration after the
         // Acquisition Device creation by a RECEIVER... except enabling multicast.

      }

      Buffers = SapBufferWithTrash(10, &AcqDevice);
      View = SapView(&Buffers, SapHwndAutomatic);
      Xfer = SapAcqDeviceToBuf(&AcqDevice, &Buffers, AcqCallback, &View);
   }

   // Create acquisition object
   if (!AcqDevice.Create())
      goto FreeHandles;

   if (master)
   {
      INT64 multicastAddress = 0xef0b0b0b; //239.11.11.11

      // Set the multicast IP address used by the Camera (optional... only if
      // you don't want to use the default value of 239.10.10.10.
      AcqDevice.SetFeatureValue("multiCastStreamingIPAddress", multicastAddress);

      // Enable the multicast.
      AcqDevice.SetFeatureValue("multiCastStreamingEnable", TRUE);
   }
   else
   {
      unsigned int streamingEnable = 0;

      do
      {
         Sleep(1000);

         // Loop until the multicast is enabled by the MASTER.
         if (AcqDevice.GetFeatureValue("multiCastStreamingEnable", &streamingEnable) == FALSE)
            goto FreeHandles;

      } while (!streamingEnable);

      Sleep(1000);
   }

   // Create buffer object
   if (!Buffers.Create())
      goto FreeHandles;

   // Create transfer object
   if (!Xfer.Create())
      goto FreeHandles;

   // Create view object
   if (!View.Create())
      goto FreeHandles;

   // Start continous grab
   Xfer.Grab();

   printf("Press any key to stop grab\n");
   CorGetch();

   // Stop grab
   Xfer.Freeze();
   if (!Xfer.Wait(5000))
      printf("Grab could not stop properly.\n");

FreeHandles:
   printf("Press any key to terminate\n");
   CorGetch();

   // Destroy view object
   if (!View.Destroy()) return FALSE;

   // Destroy transfer object
   if (!Xfer.Destroy()) return FALSE;

   // Destroy buffer object
   if (!Buffers.Destroy()) return FALSE;

   // Destroy acquisition object
   if (!AcqDevice.Destroy()) return FALSE;

   return 0;
}

static void AcqCallback(SapXferCallbackInfo *pInfo)
{
   SapView *pView= (SapView *) pInfo->GetContext();

   // Refresh view
   pView->Show();
}

static BOOL GetOptions(int argc, char *argv[], char *acqServerName, UINT32 *pAcqDeviceIndex)
{
   // Check if arguments were passed
   if (argc > 1)
      return GetOptionsFromCommandLine(argc, argv, acqServerName, pAcqDeviceIndex);
   else
      return GetCorAcqDeviceOptionsFromQuestions(acqServerName, pAcqDeviceIndex, TRUE);
}

static BOOL GetOptionsFromCommandLine(int argc, char *argv[], char *acqServerName, UINT32 *pAcqDeviceIndex)
{
   // Check the command line for user commands
   if ((strcmp(argv[1], "/?") == 0) || (strcmp(argv[1], "-?") == 0))
   {
      // print help
      printf("Usage:\n");
      printf("GigECameraMulticast [<acquisition server name> <acquisition device index>]\n");
      return FALSE;
   }

   // Check if enough arguments were passed
   if (argc < 3)
   {
      printf("Invalid command line!\n");
      return FALSE;
   }

   // Validate server name
   if (SapManager::GetServerIndex(argv[1]) < 0)
   {
      printf("Invalid acquisition server name!\n");
      return FALSE;
   }

   // Does the server support acquisition?
   int deviceCount = SapManager::GetResourceCount(argv[1], SapManager::ResourceAcqDevice);

   if (deviceCount == 0)
   {
      printf("This server does not support acquisition!\n");
      return FALSE;
   }

   // Validate device index
   if (atoi(argv[2]) < 0 || atoi(argv[2]) >= deviceCount)
   {
      printf("Invalid acquisition device index!\n");
      return FALSE;
   }

   // Fill-in output variables
   CorStrncpy(acqServerName, argv[1], CORSERVER_MAX_STRLEN);
   *pAcqDeviceIndex = atoi(argv[2]);

   return TRUE;
}
