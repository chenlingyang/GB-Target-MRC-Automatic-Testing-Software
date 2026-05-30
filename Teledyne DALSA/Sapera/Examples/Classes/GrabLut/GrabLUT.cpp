// -----------------------------------------------------------------------------------------
// Sapera Grab and LUT Example
// 
//    This program shows how to grab images from a camera into a buffer in the host
//    computer's memory.
//	  This program also shows how to enable, save and load  a lookup-table (LUT) 
//	  in hardware processing engine.
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
static BOOL GetOptions(int argc, char *argv[], char *acqServerName, UINT32 *pAcqDeviceIndex, char *configFileName);
static BOOL GetOptionsFromCommandLine(int argc, char *argv[], char *acqServerName, UINT32 *pAcqDeviceIndex, char *configFileName);


int main(int argc, char* argv[])
{
   std::vector<std::string> plistLutFiles;
   BOOL bLutLoaded=FALSE;
   char key;
   int lutEntries=0;
   int numberOfLuts = 0;

   char*  acqServerName = new char[CORSERVER_MAX_STRLEN];
	char*  configFilename = new char[MAX_PATH];
	char*  acqLutName=new char[MAX_CONFIG_FILES];
   UINT32   acqDeviceNumber;

   printf("Sapera Console Grab Lut Example (C++ version)\n");
   
   // Call GetOptions to determine which acquisition device to use 
   // Note: if this were an MFC-enabled application, we could have replaced the lengthy GetOptions 
   // function with the CAcqConfigDlg dialog of the Sapera++ GUI Classes (see GrabMFC example)
   if (!GetOptions(argc, argv, acqServerName, &acqDeviceNumber, configFilename))
   {
      printf("\nPress any key to terminate\n");
      CorGetch();
      return 0;
   }

   SapLocation loc(acqServerName, acqDeviceNumber);
   SapAcquisition Acq(loc, configFilename);
   SapBuffer Buffers(1, &Acq);
   SapView View(&Buffers, SapHwndAutomatic);
   SapTransfer Xfer = SapAcqToBuf(&Acq, &Buffers, AcqCallback, &View);
	SapLut *Lut = NULL;

   // Create acquisition object
   if (!Acq.Create())
      goto FreeHandles;

   // Create buffer object
   if (!Buffers.Create())
      goto FreeHandles;

   // Create transfer object
   if (!Xfer.Create())
      goto FreeHandles;

   // Create a LUT compatible with the acquisition
	lutEntries = 1 << Buffers.GetPixelDepth();
	SapFormat lutFormat;

			switch( Buffers.GetFormat())
			{
				case SapFormatMono8:
					lutFormat = SapFormatColorNI8;
					break;

				case SapFormatMono16:
					lutFormat = SapFormatColorNI16;
					break;
				default:
					printf("Input buffer format must be 8 or 16-bit monochrome.\n");
					goto FreeHandles;
			}

   numberOfLuts = Acq.GetNumLut();
   
   if (numberOfLuts == 0)
   {
      goto exit;
   }
   else
      Lut = Acq.GetLut();

   if (!Lut)
   {
exit:
      printf("The selected acquisition device has no LUTs\n");
      goto FreeHandles;
   }

StartLutOperations:   
   printf("\nPress any key to perform LUT operation. Press 'q' to quit.\n");
   key = (char)CorGetch();
   if (key != 0)
	{
	   if (key == 'q')
			goto FreeHandles;
	}

   // Enable LUT
   Acq.EnableLut(TRUE);

bLutLoaded=FALSE;
	
// Load one of the LUT previously saved in the demo
 if (!GetLoadLUTFiles(Lut, plistLutFiles, &bLutLoaded))
		goto StartLutOperations;

if (!bLutLoaded)
{
 if (!GetLUTOptionsFromQuestions(&Buffers, Lut, acqLutName))
		goto StartLutOperations;
}

// Apply the LUT you selected 
	Acq.ApplyLut(TRUE);	

if (!bLutLoaded)
{
   printf("Showing %s ... ", acqLutName);
}
   printf("Press any key to start acquisition\n");
   CorGetch();

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
	else
		goto StartLutOperations;

FreeHandles:   
   printf("Press any key to terminate\n");
   CorGetch();

   if (acqServerName)
		delete[] acqServerName;

   if (configFilename)
		delete[] configFilename;

   if (acqLutName)
		delete[] acqLutName;

	// Destroy view object
	if (!View.Destroy()) return FALSE;

	// Destroy transfer object
	if (!Xfer.Destroy()) return FALSE;

	// Destroy buffer object
	if (!Buffers.Destroy()) return FALSE;

	// Disable LUT
   Acq.EnableLut(FALSE);

	// Destroy acquisition object
	if (!Acq.Destroy()) return FALSE;

   return 0;
}

static void AcqCallback(SapXferCallbackInfo *pInfo)
{
	SapView *pView= (SapView *) pInfo->GetContext();

	// Refresh view
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
      printf("GrabLUT [<acquisition server name> <acquisition device index> <config filename>]\n");
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

   // Fill-in output variables
   CorStrncpy(acqServerName, argv[1], CORSERVER_MAX_STRLEN);
   *pAcqDeviceIndex = atoi(argv[2]);

   return TRUE;
}



