// CameraFirmwareUpdate.cpp 
// This example demonstrates how to update firmware on a camera.


// Disable deprecated function warnings with Visual Studio 2005
#if defined(_MSC_VER) && _MSC_VER >= 1400
#pragma warning(disable: 4995)
#pragma warning(disable: 4127)
#endif

#include "SapClassBasic.h"
#include "ExampleUtils.h"
#include <iostream>

using namespace std;

// Restore deprecated function warnings with Visual Studio 2005
#if defined(_MSC_VER) && _MSC_VER >= 1400
#pragma warning(default: 4995)
#endif

class SaperaObjects
{
public:
   SaperaObjects()
   {
      pAcqDevice = NULL;
   }

   ~SaperaObjects()
   {
      if (pAcqDevice != NULL && *pAcqDevice)
      {
         pAcqDevice->Destroy();
         delete pAcqDevice;
         pAcqDevice = NULL;
      }
   }

   SapAcqDevice* pAcqDevice;
};

void PrintBlockingMessage(const char* msg)
{
   if (msg != NULL)
      cout << msg << endl;

   cout << "Press any key to continue..." << endl;
   _getch();

   cout << endl;
}

BOOL GetFirmwareFileFromDisk(char* pBuffer, UINT32 size)
{
   // adapted from https://msdn.microsoft.com/en-us/library/windows/desktop/ms646829(v=vs.85).aspx#open_file

   OPENFILENAME ofn;

   ZeroMemory(&ofn, sizeof(ofn));
   ofn.lStructSize = sizeof(ofn);
   ofn.lpstrFile = pBuffer;
   ofn.lpstrFile[0] = '\0';
   ofn.nMaxFile = size;
   ofn.lpstrFilter = "Firmware file (*.cbf)\0*.cbf\0";
   ofn.Flags = OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST;

   return GetOpenFileName(&ofn);
}


int main(int argc, char* argv[])
{
   cout << "Sapera Console Camera Firmware Update Example (C++ version)" << endl;

   // mitigate transfer errors due to command timeouts (since file transfer can require an extended time to complete)
   SapManager::SetCommandTimeout(1200000);

   // find the device with interactive questions
   char acqServerName[CORSERVER_MAX_STRLEN] = "";
   UINT32 acqDeviceNumber = 0;
   if (!GetCorAcqDeviceOptionsFromQuestions(acqServerName, &acqDeviceNumber))
   {
      PrintBlockingMessage("No acquisition device was selected. Operation aborted.");
      return 0;
   }

   SaperaObjects manager;
   BOOL status;

   // create an instance of the SapAcqDevice class to interact with the camera
   manager.pAcqDevice = new SapAcqDevice(acqServerName);
   if (!manager.pAcqDevice->Create())
   {
      PrintBlockingMessage("Failed to create the SapAcqDevice object. Operation aborted.");
      return 0;
   }

   if (!manager.pAcqDevice->IsFileAccessAvailable())
   {
      PrintBlockingMessage("File access is not available for this device. Operation aborted.");
      return 0;
   }

   // Temporarily suspend reporting of Sapera API errors through popups (use the Debug
   // window if available instead), since the GetFileIndexByName function reports an
   // error if its 'fileName' argument is not available on the acquisition device.
   SapManager::StatusMode savedStatusMode = SapManager::GetDisplayStatusMode();
   SapManager::SetDisplayStatusMode(SapManager::StatusDebug);

   // Make sure the device has a supported firmware selectors
   char* supportedFileSelectors[] = {"Firmware1", "Firmware", "Firmware2"};
   int numSupportedFileSelectors = 3;
   int selectorIndex = 0;
   char* deviceFileSelector = NULL;
   int fileSelectorIndex = -1;

   for (selectorIndex = 0; selectorIndex < numSupportedFileSelectors; selectorIndex++)
   {
      if (manager.pAcqDevice->GetFileIndexByName(supportedFileSelectors[selectorIndex], &fileSelectorIndex) && fileSelectorIndex >= 0)
      {
         deviceFileSelector = supportedFileSelectors[selectorIndex];
         break;
      }
      fileSelectorIndex = -1;
   }

   // Restore the original error reporting mode
   SapManager::SetDisplayStatusMode(savedStatusMode);

   if (fileSelectorIndex < 0)
   {
      cout << "Device supports none of the following firmware selectors:" << endl;
      for (selectorIndex = 0; selectorIndex < numSupportedFileSelectors; selectorIndex++)
         cout << "   " << supportedFileSelectors[selectorIndex] << endl;
      PrintBlockingMessage("Firmware cannot be updated. Operation aborted.");
      return 0;
   }

   cout << "Device firmware selector \"" << deviceFileSelector << "\" is available for updating" << endl;

   // get the version of the firmware the device is currently running (via the DeviceFirmwareVersion feature)
   if (!manager.pAcqDevice->IsFeatureAvailable("DeviceFirmwareVersion", &status))
   {
      PrintBlockingMessage("Failed to access the DeviceFirmwareVersion feature on the device. Operation aborted.");
      return 0;
   }

   char version[MAX_PATH] = { 0 };
   if (!manager.pAcqDevice->GetFeatureValue("DeviceFirmwareVersion", version, MAX_PATH))
   {
      PrintBlockingMessage("Failed to read the DeviceFirmwareVersion feature value from the device. Operation aborted.");
      return 0;
   }

   cout << "Device is running firmware version \"" << version << "\"\n" << endl;

   // retrieve path to the firmware file (with a .cbf file extension)
   PrintBlockingMessage("Select the file with the new firmware using the following open file dialog");
   const UINT32 LOCAL_FILE_PATH_LENGTH = 256;
   char localFilePath[LOCAL_FILE_PATH_LENGTH] = { 0 };
   if (!GetFirmwareFileFromDisk(localFilePath, LOCAL_FILE_PATH_LENGTH))
   {
      PrintBlockingMessage("Failed to get a path to the .cbf file. Operation aborted.");
      return 0;
   }

   // upload the firmware to the device
   cout << "Selected firmware file will now upload to \"" << deviceFileSelector << "\"" << endl;
   PrintBlockingMessage("This operation may take a few minutes to complete and must not be interrupted.");
   cout << "File transfer in progress..." << endl;
   if (!manager.pAcqDevice->WriteFile(localFilePath, deviceFileSelector))
   {
      PrintBlockingMessage("Failed to upload the .cbf file. Operation aborted.");
      return 0;
   }

   cout << "File transfer complete!\n" << endl;

   // reset the device?
   cout << "Changes are pending until next device reset." << endl;
   if (manager.pAcqDevice->IsFeatureAvailable("DeviceReset", &status) && status == TRUE)
   {
      cout << "Do you want to reset your device now? (y/n)" << endl;
      if ('y' == _getch())
      {
         cout << "Resetting..." << endl;
         manager.pAcqDevice->SetFeatureValue("DeviceReset", TRUE);
      }
   }

   // end 
   PrintBlockingMessage("Operation successfully completed!");
   return 0;
}

