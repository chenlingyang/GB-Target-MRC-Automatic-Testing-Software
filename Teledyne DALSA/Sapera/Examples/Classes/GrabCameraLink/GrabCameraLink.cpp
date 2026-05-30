// -----------------------------------------------------------------------------------------
// Sapera++ console Grab Camera Link example
// 
//    
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
#include <math.h>



// Restore deprecated function warnings with Visual Studio 2005
#if defined(_MSC_VER) && _MSC_VER >= 1400
#pragma warning(default: 4995)
#endif

// Static Functions
static void AcqCallback(SapXferCallbackInfo *pInfo);


int main(int argc, char* argv[])
{
   SapAcquisition	*Acq=NULL;
   SapAcqDevice	*AcqDevice=NULL;
   SapFeature     *Feature=NULL;
   SapBuffer		*Buffers=NULL;
   SapTransfer		*Xfer=NULL;
   SapView			*View=NULL;

   UINT32   acqDeviceNumber;
   char*    acqServerName=new char[CORSERVER_MAX_STRLEN];
	char*    configFilename=new char[MAX_PATH];
   BOOL isNotSupported = FALSE,status = FALSE,acquisitionCreated = TRUE,acqDeviceCreated = TRUE;

   printf("Sapera Console Grab Camera Link Example (C++ version)\n");

   //get the SapAquisition (frame grabber)
   if (!GetCorAcquisitionOptionsFromQuestions(acqServerName, &acqDeviceNumber, configFilename))
   {
      printf("\nPress any key to terminate\n");
      CorGetch(); 
      return 0;
   }
   SapLocation loc(acqServerName, acqDeviceNumber);
   if (SapManager::GetResourceCount(acqServerName, SapManager::ResourceAcq) > 0)
   {
	   Acq		= new SapAcquisition(loc, configFilename);
	   Buffers	= new SapBuffer(1, Acq);
	   View		= new SapView(Buffers, SapHwndAutomatic);
	   Xfer		= new SapAcqToBuf(Acq, Buffers, AcqCallback, View);

      // Create acquisition object
      if (Acq && !*Acq && !Acq->Create())
         acquisitionCreated = FALSE;   
   }  

   //get the SapAcqDevice (camera)
   if (!GetCorAcqDeviceOptionsFromQuestions(acqServerName, &acqDeviceNumber))
   {
      printf("\nPress any key to terminate\n");
      CorGetch(); 
      return 0;
   }
   SapLocation loc2(acqServerName, acqDeviceNumber);
   AcqDevice = new SapAcqDevice(loc2,FALSE);
   Feature = new SapFeature(acqServerName);
   status = Feature->Create();
   
   //create SapAcqDevice
   if (AcqDevice && !*AcqDevice && !AcqDevice->Create())
        acqDeviceCreated = FALSE;

   //if any of the frame grabber or camera failed to initialize, end program
   if (!acquisitionCreated || !acqDeviceCreated)
      goto FreeHandles;

   //check to see if this is a DFNC camera. If it is not, end program
   //Program checks for both current and legacy naming
   char modelName[64];
   AcqDevice->IsFeatureAvailable("DeviceModelName",&status);
   if (status)
   {
      AcqDevice->GetFeatureValue("DeviceModelName",modelName,sizeof(modelName));
      if(strstr(modelName,"Genie")!= 0)
      {
         isNotSupported = TRUE;
      }
   }
   else
   {
      isNotSupported = TRUE;
   }
   if (isNotSupported)
   {
      printf("Current camera model is not supported.\n");
      goto FreeHandles;
   }
   
   // Create buffer object
   if (Buffers && !*Buffers && !Buffers->Create())
      goto FreeHandles;

   // Create transfer object
   if (Xfer && !*Xfer && !Xfer->Create())
      goto FreeHandles;

   // Create view object
   if (View && !*View && !View->Create())
      goto FreeHandles;

   // Start continous grab
   Xfer->Grab();

   printf("Press any key to stop grab\n");
   CorGetch();

   // Stop grab
   Xfer->Freeze();
   if (!Xfer->Wait(5000))
   {
      printf("Grab could not stop properly.\n");
      Xfer->Abort();
   }
      

//end program
FreeHandles:   
   printf("Press any key to terminate\n");
   CorGetch();

	// Destroy view object
	if (View && *View && !View->Destroy()) return FALSE;

	// Destroy transfer object
	if (Xfer && *Xfer && !Xfer->Destroy()) return FALSE;

	// Destroy buffer object
	if (Buffers && *Buffers && !Buffers->Destroy()) return FALSE;

	// Destroy acquisition object
	if (Acq && *Acq && !Acq->Destroy()) return FALSE;

	// Destroy feature object
	if (Feature && *Feature && !Feature->Destroy()) return FALSE;

	// Destroy acquisition object
	if (AcqDevice && *AcqDevice && !AcqDevice->Destroy()) return FALSE;


	// Delete all objects
	if (View)		delete View; 
	if (Xfer)		delete Xfer; 
	if (Buffers)	delete Buffers; 
	if (Acq)		delete Acq; 
	if (Feature)		delete Feature; 
	if (AcqDevice)	delete AcqDevice; 

   return 0;
}

static void AcqCallback(SapXferCallbackInfo *pInfo)
{
	SapView *pView= (SapView *) pInfo->GetContext();

	// Resfresh view
	pView->Show();
}



