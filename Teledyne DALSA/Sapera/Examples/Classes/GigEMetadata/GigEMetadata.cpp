// -----------------------------------------------------------------------------------------
// Sapera AcqDevice Metadata Example
// 
//    This program shows how to enable, select, extract and save metadata.
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

#include "SapClassBasic.h"
#include "ExampleUtils.h"

#include <iostream>

// Restore deprecated function warnings with Visual Studio 2005
#if defined(_MSC_VER) && _MSC_VER >= 1400
#pragma warning(default: 4995)
#endif

static BOOL GetOptions(int argc, char *argv[], char *acqServerName, UINT32 *pAcqDeviceIndex);
static BOOL GetOptionsFromCommandLine(int argc, char *argv[], char *acqServerName, UINT32 *pAcqDeviceIndex);

int main(int argc, char* argv[])
{
   SapAcqDevice   *AcqDevice = NULL;
	SapFeature     *Feature = NULL;
   SapBuffer      *Buffers = NULL;
   SapTransfer    *Xfer = NULL;
   SapMetadata    *Metadata = NULL;

   char     acqServerName[CORSERVER_MAX_STRLEN];
   UINT32   acqDeviceNumber;

   printf("Sapera Console GigE Metadata Example (C++ version)\n");

   if (!GetOptions(argc, argv, acqServerName, &acqDeviceNumber))
   {
      printf("\nPress any key to terminate\n");
      CorGetch();
      return 0;
   }

   if (SapManager::GetResourceCount(acqServerName, SapManager::ResourceAcqDevice) == 0)
   {
      printf("\nNo acquisition device connected\n");
      return 0;
   }

   SapLocation loc(acqServerName);

   AcqDevice = new SapAcqDevice(loc);
   Feature = new SapFeature(loc);
   Buffers = new SapBufferWithTrash(5);
   Metadata = new SapMetadata(AcqDevice, Buffers);
   Xfer = new SapAcqDeviceToBuf(AcqDevice, Buffers);
   Xfer->SetStartMode(SapTransfer::StartSynchronous);

   // Create acquisition object
   if (AcqDevice && !*AcqDevice && !AcqDevice->Create())
      goto FreeHandles;

   // Create feature object
   if (Feature && !*Feature && !Feature->Create())
      goto FreeHandles;

   // Create metadata object (if supported)
   if (!SapMetadata::IsMetadataSupported(AcqDevice))
   {
      printf("This program only supports Teledyne Dalsa and Teledyne Lumenera cameras with metadata features\n\n");
      goto FreeHandles;
   }

   if (Metadata && !*Metadata && !Metadata->Create())
      goto FreeHandles;

   // Create buffer object
   if (Buffers && !*Buffers && !Buffers->Create())
      goto FreeHandles;

   // The transfer will not be connected automatically when calling the Create function,
   // this will be done only when the metadata setup is complete
   // Check if the transfer needs to remain disconnected when enabling / disabling / selecting metadata
   BOOL xferDisconnectDuringSetup = FALSE;
   BOOL isAreaScan = (Metadata->GetMetadataType() == SapMetadata::MetadataPerFrame);
   char* featurename = isAreaScan ? "ChunkModeActive" : "endOfLineMetadataMode";
   if (AcqDevice->GetFeatureInfo(featurename, Feature))
   {
      SapFeature::WriteMode writeMode;
      if (Feature->GetWriteMode(&writeMode))
         xferDisconnectDuringSetup = (writeMode == SapFeature::WriteNotConnected);
   }

   if (xferDisconnectDuringSetup)
      Xfer->SetAutoConnect(FALSE);

   // Create transfer object
   if (Xfer && !*Xfer && !Xfer->Create())
      goto FreeHandles;

   if (isAreaScan)
      printf("Metadata for an area-scan device.");
   else
      printf("Metadata for a line-scan device.");

   printf("\nPress any key to go to the next step\n");
   CorGetch();

   // show all available selectors
   unsigned int nSelectorCount = Metadata->GetSelectorCount();
   char sSelectorName[MAX_PATH];
   printf("\nThis device exposes the following %d selectors:\n", nSelectorCount);
   for (unsigned int iSelector = 0; iSelector < nSelectorCount; iSelector++)
   {
      if (Metadata->GetSelectorName(iSelector, sSelectorName, MAX_PATH))
         printf("%s (%d)\n", sSelectorName, iSelector);
      else
         printf("Failed to get the selector name at index(%d)\n", iSelector);
   }

   printf("\nPress any key to go to the next step\n");
   CorGetch();

   printf("\nEnable metadata\n");
   if (!Metadata->Enable())
      printf("Failed to enable the metadata!\n");

   printf("\nPress any key to go to the next step\n");
   CorGetch();

   // enable all selectors (if not available, then all selectors will be enabled
   // automatically when calling the SapMetadata::Enable function)
   // Check if metadata selectors can be enabled through application code
   BOOL isSelectAvailable = FALSE;
   featurename = isAreaScan ? "ChunkEnable" : "endOfLineMetadataContentActivationMode";
   if (AcqDevice->GetFeatureInfo(featurename, Feature))
   {
      SapFeature::AccessMode accessMode;
      if (Feature->GetAccessMode(&accessMode))
         isSelectAvailable = (accessMode == SapFeature::AccessRW);
   }

   if (isSelectAvailable)
   {
      printf("\nSelect all selectors:\n");
      for (unsigned int iSelector = 0; iSelector < nSelectorCount; iSelector++)
      {
         if (Metadata->Select(iSelector))
            printf("Selector (%d) is selected.\n", iSelector);
         else
            printf("Failed to select selector(%d)\n", iSelector);
      }

      printf("Press any key to go to the next step\n");
      CorGetch();
   }

   printf("\nAcquire images buffers\n");
   if (xferDisconnectDuringSetup)
      Xfer->Connect();  // Need to do this if the transfer was not connected when calling Create
   Xfer->Snap(Buffers->GetCount());
   Xfer->Wait(5000);

   printf("\nPress any key to go to the next step\n");
   CorGetch();

   // extract and list chunks from the first buffer (and first line when using a linescan camera)
   BOOL bExtractSuccess = FALSE;
   if (isAreaScan)
   {
      printf("\nExtract and list chunks from the first buffer:\n");
      bExtractSuccess = Metadata->Extract(0);
   }
   else
   {
      printf("\nExtract and list chunks from the first buffer and first line:\n");
      bExtractSuccess = Metadata->Extract(0, 0);
   }
   if (bExtractSuccess)
   {
      unsigned int nResultCount = Metadata->GetExtractedResultCount();
      char sChunkName[MAX_PATH];
      char sChunkValue[MAX_PATH];
      for (unsigned int iResult = 0; iResult < nResultCount; iResult++)
      {
         if (Metadata->GetExtractedResult(iResult, sChunkName, MAX_PATH, sChunkValue, MAX_PATH))
            printf("Result (%d) : %s = %s\n", iResult, sChunkName, sChunkValue);
         else
            printf("Failed to extract result (%d)\n", iResult);
      }
   }

   printf("\nPress any key to go to the next step\n");
   CorGetch();

   // save to CSV (auto-extract)
   char* csvFile = "metadata.csv";
   char currentDir[MAX_PATH];

   GetCurrentDirectory(MAX_PATH, currentDir);

   printf("\nAutomatically extract all buffers and save to CSV file\n");
   if (Metadata->SaveToCSV(csvFile))
      printf("Metadata saved to file %s\\%s\n", currentDir, csvFile);
   else
      printf("Failed to save the metadata to file!\n");

FreeHandles:

   printf("Press any key to terminate\n");
   CorGetch();

   // Destroy metadata object
   if (Metadata && *Metadata)
      Metadata->Destroy();

   // Destroy transfer object
   if (Xfer && *Xfer)
      Xfer->Destroy();

   // Destroy buffer object
   if (Buffers && *Buffers)
      Buffers->Destroy();

   // Destroy feature object
   if (Feature && *Feature)
      Feature->Destroy();

   // Destroy acquisition object
   if (AcqDevice && *AcqDevice)
      AcqDevice->Destroy();

   // Delete all objects
   if (Metadata)  delete Metadata;
   if (Xfer)      delete Xfer;
   if (Buffers)   delete Buffers;
   if (Feature)   delete Feature;
   if (AcqDevice) delete AcqDevice;

   return 0;
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
      printf("GigECameraLut [<acquisition server name> <acquisition device index>]\n");
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

