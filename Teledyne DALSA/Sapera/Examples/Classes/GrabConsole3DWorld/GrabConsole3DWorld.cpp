// -----------------------------------------------------------------------------------------
// Sapera++ console grab 3D example with world coordinates calculations
// 
//    This program shows how to grab images from a 3D camera into a buffer in the host
//    computer's memory, using Sapera++ Acquisition and Buffer objects, and a Transfer 
//    object to link them.  Also, real world values are computed at the midpoint of the
//    profile or range.
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
double Offset_A_Parameter;            // Offset from Pixel to World according to the X (X) component
double Scale_C_Parameter;             // Scale from Pixel to World according to the C (Z) component
double Offset_C_Parameter;            // Offset from Pixel to World according to the C (Z) component
int Output_Mode_Parameter;            // Output Mode of the device


int main(int argc, char* argv[])
{
	UINT32   acqDeviceNumber;
	char*    acqServerName = new char[CORSERVER_MAX_STRLEN];

	printf("Sapera 3D Console Grab Example with world coordinates calculations (C++)\n");
	printf("The Acquisition can come in 3 formats : C AC ACRW \n");
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

	// Create acquisition object
	if (!AcqDevice.Create())
		goto FreeHandles;

	// Create buffer object
	if (!AcqBuffers.Create())
		goto FreeHandles;

	// Create transfer object
	AcqDeviceToBuf.SetCallbackInfo(TransferCallback);
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

    //Grab
	AcqDeviceToBuf.Grab();
   printf("\nAny Key to stop Grabbing.\n");
   CorGetch();

	//Freezing
	AcqDeviceToBuf.Freeze();
	if (!AcqDeviceToBuf.Wait(5000))
		printf("\nGrab could not stop properly.\n");

FreeHandles:
	printf("\nPress any key to terminate\n");
	CorGetch();

	// Destroy transfer object
	AcqDeviceToBuf.Destroy();

	// Destroy buffer object
	AcqBuffers.Destroy();

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
   double CallbackFrequency = (Image_Height_Feature / Line_Rate_Feature ) * 1000.0;
   printf("\nTransfer callbacks will occur every %.1f milliseconds\n", CallbackFrequency);

	return true;
}

void ReadPoint(SapBuffer* Buffer, int xIndex, int yIndex, SapFormat Pixel_Format,
   double Scale_A, double Offset_A, double Scale_C, double Offset_C,
   int Output_Mode, int* X, int* Z, int* R)
{
   SapDataCoord3D value;

   // Read native coordinates from buffer
   Buffer->ReadElement(xIndex, yIndex, &value);

   switch (Pixel_Format)
   {
      case SapFormatCoord3D_C16:
         *Z = value.C() * (int)Scale_C + (int)Offset_C;     // Real Z
         if (Output_Mode == CORBUFFER_VAL_SCAN3D_OUTPUT_MODE_RECTIFIED_C)
         {
            // Real X (can be calculated from X index only in Rectified output mode)
            *X = xIndex * (int)Scale_A + (int)Offset_A;
         }
         break;

      case SapFormatCoord3D_AC16:
         *X = value.A() * (int)Scale_A + (int)Offset_A;     // Real X
         *Z = value.C() * (int)Scale_C + (int)Offset_C;     // Real Z
         break;

      case SapFormatCoord3D_ACRW16:
         *X = value.A() * (int)Scale_A + (int)Offset_A;     // Real X
         *Z = value.C() * (int)Scale_C + (int)Offset_C;     // Real Z
         *R = value.R();                                    // Real R = Native R
         break;
   }
}

void TransferCallback(SapXferCallbackInfo *pInfo)
{
	//We get the acquisition Buffer from the transfer.
	SapBuffer *pBufferAcq = (SapBuffer *)pInfo->GetTransfer()->GetPair(0)->GetDst(); 

   int X = 0, Z = 0, R = 0;
   static int CallbackNumber = 0;

	if (Buffer_Layout_Parameter == Profile)
	{
      //Read middle point of the profile
      int xIndex = pBufferAcq->GetWidth() / 2;
      ReadPoint(pBufferAcq, xIndex, 0, Pixel_Format_Feature, Scale_A_Parameter, Offset_A_Parameter,
                Scale_C_Parameter, Offset_C_Parameter, Output_Mode_Parameter, &X, &Z, &R);
	}
	else
	{
      //Read middle point of the range image
      int xIndex = pBufferAcq->GetWidth() / 2;
      int yIndex = pBufferAcq->GetHeight() / 2;
      ReadPoint(pBufferAcq, xIndex, yIndex, Pixel_Format_Feature, Scale_A_Parameter, Offset_A_Parameter,
                Scale_C_Parameter, Offset_C_Parameter, Output_Mode_Parameter, &X, &Z, &R);
	}
	printf("\rCallback number = %d, X = %d, Z = %d, R = %d      ", CallbackNumber++, X, Z, R);
}
