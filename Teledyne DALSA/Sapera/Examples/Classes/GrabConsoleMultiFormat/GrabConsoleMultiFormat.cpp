// -----------------------------------------------------------------------------------------
// Sapera++ Grab Console MultiFormat example
// 
//    This program illustrates how to work with multiformat buffers in Sapera++.
//    First we verify that the selected configuration file defines a multiformat acquisition
//    source and display the buffer types contained in the multiformat acquisition buffer.
//    Then we setup a live grab during which the user can dynamically change the active page 
//    of the multiformat buffer to select the component to view.
//    It is also possible to extract a specific buffer component from the multiformat buffer
//    and save it to a bitmap file on disk.
//
// -----------------------------------------------------------------------------------------

// Disable deprecated function warnings with Visual Studio 2005
#if defined(_MSC_VER) && _MSC_VER >= 1400
#pragma warning(disable: 4995)
#endif

#include "stdio.h"
#include "conio.h"
#include "sapclassbasic.h"
#include "ExampleUtils.h"

// Restore deprecated function warnings with Visual Studio 2005
#if defined(_MSC_VER) && _MSC_VER >= 1400
#pragma warning(default: 4995)
#endif

// Static Functions
static void AcqCallback(SapXferCallbackInfo *pInfo);
static BOOL GetOptions(int argc, char *argv[], char *acqServerName, UINT32 *pAcqDeviceIndex, char *configFileName);
static BOOL GetOptionsFromCommandLine(int argc, char *argv[], char *acqServerName, UINT32 *pAcqDeviceIndex, char *configFileName);

// Entry point
int main(int argc, char* argv[])
{
   SapAcquisition	*Acq = NULL;
   SapBuffer      *Buffers = NULL;
   SapTransfer    *Xfer = NULL;
   SapView        *View = NULL;


   char acqServerName[CORSERVER_MAX_STRLEN];
   char configFilename[MAX_PATH];
   UINT32 acqDeviceNumber;

   printf("Sapera Grab Console MultiFormat Example (C++ version)\n");

   // Call GetOptions to determine which acquisition device to use and which config
   // file (CCF) should be loaded to configure it.
   // Note: if this were an MFC-enabled application, we could have replaced the lengthy GetOptions 
   // function with the CAcqConfigDlg dialog of the Sapera++ GUI Classes (see GrabMFC example)
   if (!GetOptions(argc, argv, acqServerName, &acqDeviceNumber, configFilename))
   {
      printf("\nPress any key to terminate\n");
      CorGetch();
      return 0;
   }

   SapLocation loc(acqServerName, acqDeviceNumber);

   Acq = new SapAcquisition(loc, configFilename);
   Buffers = new SapBuffer(1, Acq);
   View = new SapView(Buffers, SapHwndAutomatic);
   Xfer = new SapAcqToBuf(Acq, Buffers, AcqCallback, View);

   // Create acquisition object
   if (Acq && !*Acq && !Acq->Create())
      goto FreeHandles;

   // Create buffer object
   if (Buffers && !*Buffers && !Buffers->Create())
      goto FreeHandles;

   // Validate buffer object (must be multiformat)
   if (!Buffers->IsMultiFormat())
   {
      printf("\nThe buffer is not a multiformat one. Please configure your acquisition properly.\n");
      goto FreeHandles;
   }

   // Display buffer format and other descriptive information
   SapFormat pageFormatArray[6] = { SapFormatUnknown };
   int numPages = -1;
   if (!Buffers->GetPageFormat(pageFormatArray) || !Buffers->GetNumPages(&numPages))
   {
      printf("\nFailed to retrieve buffer info. Abort.\n");
      goto FreeHandles;
   }

   SapFormat format = Buffers->GetFormat();
   char sFormat[128] = { 0 };
   SapManager::GetStringFromFormat(format, sFormat);

   printf("\n");
   printf("**************    BUFFER INFO    **************");
   printf("\nformat: %s", sFormat);
   printf("\nlist of subformats:");
   for (int iPage = 0; iPage < numPages; iPage++)
   {
      if (pageFormatArray[iPage] == SapFormatUnknown)
         break;

      if (SapManager::GetStringFromFormat(pageFormatArray[iPage], sFormat))
         printf("\n  page %d contains the %s component", iPage + 1, sFormat);
   }
   printf("\n");
   printf("***********************************************");
   printf("\n");


   // Create transfer object
   if (Xfer && !*Xfer && !Xfer->Create())
      goto FreeHandles;

   // Create view object
   if (View && !*View && !View->Create())
      goto FreeHandles;

   // interactive menu commands
   bool bContinue = true;
   while (bContinue)
   {
      // present menu options
      printf("\n");
      printf("*************************    LIST OF COMMANDS    *************************");
      printf("\n start:  start live grab");
      printf("\n  stop:  abort/stop live grab");

      printf("\n showN:  display the N-th page");
      printf("\n         (with N in 1..%d)", numPages);

      printf("\n  save:  save the buffer to a CRC file on disk");
      printf("\n saveN:  save the N-th page of the buffer to a BMP file on disk");
      printf("\n         (with N in 1..%d)", numPages);

      printf("\n  quit:  close the example");
      printf("\n");
      printf("**************************************************************************");

      // read user-provided menu
      printf("\n> Type in a menu option: ");
      char menu[6] = { 0 };
      scanf_s("%s", menu, (int)_countof(menu));

      // execute command from user
      if (strcmp(menu, "start") == 0)
      {
         printf("\n> Starting continuous grab ...");
         if (Xfer->Grab())
            printf(" Succeeded\n");
         else
         {
            printf(" Failed. Abort.\n");
            bContinue = false;
         }
      }
      else if (strcmp(menu, "stop") == 0)
      {
         printf("\n> Stopping continuous grab ...");
         Xfer->Freeze();
         if (!Xfer->Wait(5000))
            printf(" Failed\n");
      }
      else if (strcmp(menu, "quit") == 0)
      {
         if (Xfer->IsGrabbing())
            printf("\n> Please stop the grab first!");
         else
         {
            printf("\n> Come back again!");
            bContinue = false;
         }
      }
      else if (strcmp(menu, "save") == 0)
      {
         printf("\n> Saving current buffert to \"multiformat.crc\" ...");
         if (Buffers->Save(".\\multiformat.crc", "-format crc"))
            printf(" Succeeded\n");
         else
            printf(" Failed\n");
      }
      else
      {
         for (int iPage = 0; iPage < numPages; iPage++)
         {
            char dyna[6] = { 0 };

            // showN
            CorSnprintf(dyna, _countof(dyna), "show%d", iPage + 1);
            if (strcmp(menu, dyna) == 0)
            {
               printf("\n> Setting active page for display to %d ...", iPage + 1);
               if (Buffers->SetAllPage(iPage))
               {
                  // when not grabbing we need to manually refresh the view
                  if (!Xfer->IsGrabbing())
                     View->Show();

                  printf(" Succeeded\n");
               }
               else
                  printf(" Failed\n");

               break;
            }

            // saveN
            CorSnprintf(dyna, _countof(dyna), "save%d", iPage + 1);
            if (strcmp(menu, dyna) == 0)
            {
               memset(sFormat, 0, sizeof(sFormat));
               SapManager::GetStringFromFormat(pageFormatArray[iPage], sFormat);

               char sFileName[MAX_PATH] = { 0 };
               CorSnprintf(sFileName, MAX_PATH, ".\\subformat-%s.bmp", sFormat);

               printf("\n> Saving page %d with subformat %s to \"%s\" ...", iPage + 1, sFormat, sFileName);

               // use SapBuffer::Copy to extract the N-th page
               // (could have used SapBuffer::SplitComponents as well)

               SapBuffer bufferExtract(1, Buffers->GetWidth(), Buffers->GetHeight(), pageFormatArray[iPage]);
               if (bufferExtract.Create() && bufferExtract.Copy(Buffers) && bufferExtract.Save(sFileName, "-format bmp"))
                  printf(" Succeeded\n");
               else
                  printf(" Failed\n");

               if (bufferExtract)
                  bufferExtract.Destroy();

               break;
            }
         }
      }

      printf("\n\n\n");
   }

FreeHandles:

   printf("\nPress any key to terminate ...\n");
   CorGetch();

   // Destroy view object
   if (View && *View && !View->Destroy()) return FALSE;

   // Destroy transfer object
   if (Xfer && *Xfer && !Xfer->Destroy()) return FALSE;

   // Destroy buffer object
   if (Buffers && *Buffers && !Buffers->Destroy()) return FALSE;

   // Destroy acquisition object
   if (Acq && *Acq && !Acq->Destroy()) return FALSE;

   // Delete all objects
   if (View)     delete View;
   if (Xfer)     delete Xfer;
   if (Buffers)  delete Buffers;
   if (Acq)      delete Acq;

   return 0;
}

static void AcqCallback(SapXferCallbackInfo *pInfo)
{
   SapView *pView = (SapView *)pInfo->GetContext();

   // Resfresh view
   pView->Show();
}

static BOOL GetOptions(int argc, char *argv[], char *acqServerName, UINT32 *pAcqDeviceIndex, char *configFileName)
{
   // Check if arguments were passed
   if (argc > 1)
      return GetOptionsFromCommandLine(argc, argv, acqServerName, pAcqDeviceIndex, configFileName);
   else
      return GetCorAcquisitionOptionsFromQuestions(acqServerName, pAcqDeviceIndex, configFileName);
}

static BOOL GetOptionsFromCommandLine(int argc, char *argv[], char *acqServerName, UINT32 *pAcqDeviceIndex, char *configFileName)
{
   // Check the command line for user commands
   if ((strcmp(argv[1], "/?") == 0) || (strcmp(argv[1], "-?") == 0))
   {
      // print help
      printf("Usage:\n");
      printf("GrabConsoleMultiFormat [<acquisition server name> <acquisition device index> <config filename>]\n");
      return FALSE;
   }

   // Check if enough arguments were passed
   if (argc < 4)
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
   int deviceCount = SapManager::GetResourceCount(argv[1], SapManager::ResourceAcq);

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

   // Verify that the specified config file exist
   OFSTRUCT of = { 0 };
   if (OpenFile(argv[3], &of, OF_EXIST) == HFILE_ERROR)
   {
      printf("The specified config file (%s) is invalid!\n", argv[3]);
      return FALSE;
   }

   // Fill-in output variables
   CorStrncpy(acqServerName, argv[1], CORSERVER_MAX_STRLEN);
   *pAcqDeviceIndex = atoi(argv[2]);
   CorStrncpy(configFileName, argv[3], MAX_PATH);

   return TRUE;
}

