// -----------------------------------------------------------------------------------------
// Sapera++ console grab 3D example
// 
//    This program shows how to grab images from a 3D camera into a buffer in the host
//    computer's memory, using Sapera++ Acquisition and Buffer objects, and a Transfer 
//    object to link them.  Also, a View object is used to display the buffer.
//
// -----------------------------------------------------------------------------------------

#include "stdio.h"
#include "conio.h"
#include "math.h"
#include "sapclassbasic.h"
#include "ExampleUtils.h"
#include <string>

//Constants
const int FEATURE_ENUM_TEXT_SIZE = 32;        //Max Size of feature enum text

// CallBack Function
 void TransferCallback(SapXferCallbackInfo *pInfo);  //called each time a buffer is received

//Utils
bool GetTextFromEnumFeatureName(char* FeatureName, SapAcqDevice* AcqDevice, char* FeatureText); //Get the text value of an enum feature.

//Features and Parameters
bool InitializeCameraFeatures(SapAcqDevice* AcqDevice, SapBuffer* AcqBuffer); //Features: Get Device ScanType and Pixel Format, Set 

//Array containing 4 buffers, 1 for each component A(X), C(Z), R(Reflectance), W(unused)
SapBuffer Buffer_Components_Split[4];

//Camera Features
char PixelFormat_FeatureText[FEATURE_ENUM_TEXT_SIZE]; // contains the feature Pixel Format enum text value
char DeviceScan_FeatureText[FEATURE_ENUM_TEXT_SIZE];  // contains the feature Device Scan enum text value
SapFormat Pixel_Format_Feature;   // AC, C, or ACRW
double Line_Rate_Feature;        // contains the feature line rate value
int Image_Height_Feature;        // contains the feature image height value

//Buffer Parameters
BUFFER_LAYOUT Buffer_Layout_Parameter;     //Range or Profile
int Invalid_Flag_C_Parameter;         // Flag (0 or else) expressing if invalid values must be taking into account
double Invalid_Value_C_Parameter;     // The Z value flagged as invalid
double Scale_A_Parameter;             // Scale from Pixel to World according to the A (X) component
double Offset_A_Parameter;            // Offset from Pixel to World according to the X (Xw) component
double Scale_C_Parameter;             // Scale from Pixel to World according to the C (Z) component
double Offset_C_Parameter;            // Offset from Pixel to World according to the C (Z) component
int Output_Mode_Parameter;            // Output Mode of the device

int Index_DisplayedComponent = 1;  //Index selected by user input : A = 0, C = 1, R = 2 , W = 3

int main(int argc, char* argv[])
{
	UINT32   acqDeviceNumber;
	char*    acqServerName = new char[CORSERVER_MAX_STRLEN];

	printf("Sapera Console Grab Example (C++ version)\n");
	printf("Show Acquisition and Display by Sapera LT.\n");
	printf("The Acquisition can come in 3 formats : C AC ACRW \n");
	printf("The Demo displays the chosen component: A(X) C(Z) R(Reflectance) W(unused) \n");
	printf("The Demo can handle profiles (1 line) or ranges (X height of 1 line) \n");
	printf("The selected Camera must be in LineScan3D and not use a Mono pixel format.\n");

	// Call GetCorAcqDeviceOptionsFromQuestions to determine which acquisition device to use
	if (!GetCorAcqDeviceOptionsFromQuestions(acqServerName, &acqDeviceNumber, TRUE))
	{
		printf("\nPress any key to terminate\n");
		CorGetch();
		return 0;
	}

	//The acquision device
	SapAcqDevice AcqDevice(acqServerName);
	//The buffers filled by the acquision device
	SapBufferWithTrash AcqBuffers = SapBufferWithTrash(2, &AcqDevice);
	//The transfer between the acquisition device and the buffers
	SapTransfer AcqDeviceToBuf = SapAcqDeviceToBuf(&AcqDevice, &AcqBuffers);
	//Viewing tool buffer
	SapBuffer ViewBuffer;
	//Viewing tool
	SapView View;

	// Create acquisition object
	if (!AcqDevice.Create())
		goto FreeHandles;

	// Create buffer object
	if (!AcqBuffers.Create())
		goto FreeHandles;

	// Create transfer object
	AcqDeviceToBuf.SetCallbackInfo(TransferCallback, &View);
	if (!AcqDeviceToBuf.Create())
		goto FreeHandles;

   //Get Non 3D Camera Features
	if (!InitializeCameraFeatures(&AcqDevice, &AcqBuffers))
		goto FreeHandles;

   //Get 3D Camera Features
	if (!GetParametersFrom3DBuffer(&AcqBuffers, &Invalid_Flag_C_Parameter, &Invalid_Value_C_Parameter,
         &Scale_A_Parameter, &Offset_A_Parameter, &Scale_C_Parameter, &Offset_C_Parameter,
         &Buffer_Layout_Parameter, &Output_Mode_Parameter))
   {
		goto FreeHandles;
   }

	printf("\nBuffer Parameters:\n    InvalidFlag = %d\n    InvalidValue = %f\n    Scale_A (X) = %f\n    "
	       "Offset_A (X) = %f\n    Scale_C (Z) = %f\n    Offset_C (Z) = %f\n    BufferLayout = %s    \n",
		    Invalid_Flag_C_Parameter, Invalid_Value_C_Parameter, Scale_A_Parameter, Offset_A_Parameter,
		    Scale_C_Parameter, Offset_C_Parameter, (Buffer_Layout_Parameter == Range) ? "Range" : "Profile");

   //Contains 1 buffer for each possible component
   //  Buffer 0 = A (X)
   //  Buffer 1 = C (Z)
   //  Buffer 2 = R (Reflectance)
   //  Buffer 3 = W (unused)
   for (int bufIndex = 0; bufIndex < 4; bufIndex++)
   {
	   Buffer_Components_Split[bufIndex].SetParameters(2, AcqBuffers.GetWidth(), AcqBuffers.GetHeight(), SapFormatMono16, SapBuffer::TypeDefault);
      if (!Buffer_Components_Split[bufIndex].Create())
         goto FreeHandles;
   }

	//Buffer of the Viewer. Created empty and manually modified.
   //Buffer #1 = A (X)
	ViewBuffer.SetParameters(1, AcqBuffers.GetWidth(), PROFILE_WINDOW_HEIGHT,SapFormatMono16,SapBuffer::TypeDefault);
	if (!ViewBuffer.Create())
		goto FreeHandles;

	//Viewing tool
	View = SapView(&ViewBuffer, SapHwndAutomatic);
	if (!View.Create())
		goto FreeHandles;

    //Grab
	bool Grabbing = true;
	AcqDeviceToBuf.Grab();
	//During Grab, if PixelFormat has more than one component, you can toggle the displayed component.
	while (Grabbing)  
	{
		switch (Pixel_Format_Feature)  
		{
      case SapFormatCoord3D_C16:
	      printf("\nAny Key to stop Grabbing.\n");
	      CorGetch();
			Grabbing = false;
			break;
      case SapFormatCoord3D_AC16:
			Grabbing = GetSelectionFromConsoleAC(&Index_DisplayedComponent);
			break;
      case SapFormatCoord3D_ACRW16:
			Grabbing = GetSelectionFromConsoleACRW(&Index_DisplayedComponent);
			break;
		}
	}
	//Freezing
	AcqDeviceToBuf.Freeze();
	if (!AcqDeviceToBuf.Wait(5000))
		printf("\nGrab could not stop properly.\n");

FreeHandles:
	printf("\nPress any key to terminate\n");
	CorGetch();

	// Destroy view object
	View.Destroy();

	// Destroy transfer object
	AcqDeviceToBuf.Destroy();

	// Destroy buffer object
	AcqBuffers.Destroy();

	// Destroy View Buffer Object
	ViewBuffer.Destroy();

	// Destroy Component A/C/R/W Buffer Objects
   for (int bufIndex = 0; bufIndex < 4; bufIndex++)
	   Buffer_Components_Split[bufIndex].Destroy();

	// Destroy acquisition object
	AcqDevice.Destroy();

	return 0;
}

//Return the enum text value of a feature
bool GetTextFromEnumFeatureName(char* FeatureName, SapAcqDevice * AcqDevice, char* FeatureText)
{
   BOOL isAvailable = FALSE;
   AcqDevice->IsFeatureAvailable(FeatureName, &isAvailable);

	SapFeature Feature = SapFeature(AcqDevice->GetLocation());
   if (isAvailable && Feature.Create())
   {
      if (AcqDevice->GetFeatureInfo(FeatureName, &Feature))
      {
	      int enumvalue ;
         if (AcqDevice->GetFeatureValue(FeatureName, &enumvalue))
	         Feature.GetEnumStringFromValue(enumvalue, FeatureText, FEATURE_ENUM_TEXT_SIZE);
      }

      Feature.Destroy();
      return true;
   }

	return false;
}

//Get Non 3D Camera Features
bool InitializeCameraFeatures(SapAcqDevice* AcqDevice, SapBuffer* AcqBuffer)
{
	memset(PixelFormat_FeatureText, 0, FEATURE_ENUM_TEXT_SIZE);
	memset(DeviceScan_FeatureText, 0, FEATURE_ENUM_TEXT_SIZE);

	//Get DeviceScanType Feature
   bool success = GetTextFromEnumFeatureName("DeviceScanType", AcqDevice, DeviceScan_FeatureText);

	int DeviceScanType = 0;
	AcqBuffer->GetParameter(CORBUFFER_PRM_DEVICE_SCAN_TYPE, &DeviceScanType);
	if (!success || DeviceScanType != CORBUFFER_VAL_DEVICE_SCAN_TYPE_LINESCAN3D)
	{
		printf("\nThis application only support cameras with Linescan3D as the Device Scan Type\n");
		return false;
	}

   //Get PixelFormat Feature Value
   Pixel_Format_Feature = AcqBuffer->GetFormat();
	SapManager::GetStringFromFormat(Pixel_Format_Feature, PixelFormat_FeatureText);

   if (Pixel_Format_Feature != SapFormatCoord3D_C16 &&
       Pixel_Format_Feature != SapFormatCoord3D_AC16 &&
       Pixel_Format_Feature != SapFormatCoord3D_ACRW16)
   {
		printf("This application doensn't support %s Format\n", PixelFormat_FeatureText);
      return false;
   }

	//Get AcquisitionFrameRate Feature
   //For LineScan3D, this is actually the line rate
	if (!AcqDevice->GetFeatureValue("AcquisitionFrameRate", &Line_Rate_Feature))
		return false;

	//Get Height Feature
	if (!AcqDevice->GetFeatureValue("Height", &Image_Height_Feature))
		return false;

	printf("\nCamera features:\n    PixelFormat = %s\n    DeviceScanType = %s\n    LineRate = %f\n    Height = %d\n",
	       PixelFormat_FeatureText, DeviceScan_FeatureText, Line_Rate_Feature, Image_Height_Feature);

   //Calculate callback rate (which could be quite slow), and display as information
   double CallbackFrequency = Image_Height_Feature / Line_Rate_Feature;
   printf("\nTransfer callbacks will occur every %.1f seconds\n", CallbackFrequency);

	return true;
}

//Called Each time a Buffer is available in the transfer. Splitting, Display and Processing
void TransferCallback(SapXferCallbackInfo *pInfo)
{
	SapView *pView = (SapView *)pInfo->GetContext();  
	//We get the acquisition Buffer from the transfer.
	SapBuffer *pBufferAcq = (SapBuffer *)pInfo->GetTransfer()->GetPair(0)->GetDst(); 

	int Pixel_Mean = 0;
	int World_Mean = 0;
   static int CallbackNumber = 0;

	switch (Pixel_Format_Feature)  //Split the components, in the corresponding container,depending on the format
	{
   case SapFormatCoord3D_C16:
		Buffer_Components_Split[1].Copy(pBufferAcq);
		break;
   case SapFormatCoord3D_AC16:
		pBufferAcq->SplitComponents(&Buffer_Components_Split[0], &Buffer_Components_Split[1], NULL);
		break;
   case SapFormatCoord3D_ACRW16:
		pBufferAcq->SplitComponents(&Buffer_Components_Split[0], &Buffer_Components_Split[1], &Buffer_Components_Split[2]);
		break;
	}
	if (Buffer_Layout_Parameter == Profile)
	{
		//Display a buffer, as a profile, chosen by the input
		DisplayProfile(pView, &Buffer_Components_Split[Index_DisplayedComponent]); 
		//Process the mean of the pixels C(Z) with respect for the invalids (value and flag) parameters
		ProcessingProfile(&Buffer_Components_Split[1], Invalid_Flag_C_Parameter, Invalid_Value_C_Parameter,
		   Scale_C_Parameter, Offset_C_Parameter, &Pixel_Mean, &World_Mean);
	}
	else
	{
		DisplayRange(pView, &Buffer_Components_Split[Index_DisplayedComponent]);
		//Pass the Component C to be processed
		ProcessingRange(&Buffer_Components_Split[1], Invalid_Flag_C_Parameter, Invalid_Value_C_Parameter,
		   Scale_C_Parameter, Offset_C_Parameter, &Pixel_Mean, &World_Mean);
	}
	printf("\rCallback number = %d, Pixel_Mean = %d, World_Mean = %d", CallbackNumber++, Pixel_Mean, World_Mean);
}
