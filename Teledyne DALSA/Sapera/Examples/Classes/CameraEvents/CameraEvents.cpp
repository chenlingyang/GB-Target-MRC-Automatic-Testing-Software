// -----------------------------------------------------------------------------------------
// Sapera AcqDevice Events Example
// 
//    This program shows how to get all the available events with CorAcqDevice.
//	  With the registering and unregistering callback mechanism, it shows also 
//	  how to track when a specific event occurs.
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
#include <vector>
#include <string>

// Restore deprecated function warnings with Visual Studio 2005
#if defined(_MSC_VER) && _MSC_VER >= 1400
#pragma warning(default: 4995)
#endif

static void CameraCallback(SapAcqDeviceCallbackInfo* pInfo);

//
// Callback Function
//
void CameraCallback(SapAcqDeviceCallbackInfo* pInfo)
{
   BOOL status;
   int eventCount;
   int eventIndex;
   char eventName[STRING_LENGTH];
	char message[STRING_LENGTH];

   // Retrieve count, index and name of the received event
   status = pInfo->GetEventCount(&eventCount);
   status = pInfo->GetEventIndex(&eventIndex);
   status = pInfo->GetAcqDevice()->GetEventNameByIndex(eventIndex, eventName, sizeof(eventName));

	int featureIndex;
   char featureName[STRING_LENGTH];

   // Check for "Feature Value Changed" event
   //Note: The callback events listed could be different from camera to camera as a feature change 
   //could trigger multiple internal feature changes
	if (CorStricmp(eventName, "Feature Value Changed") == 0)
   {
      // Retrieve index and name of the feature that has changed
      status = pInfo->GetFeatureIndex(&featureIndex);
      status = pInfo->GetAcqDevice()->GetFeatureNameByIndex(featureIndex, featureName, sizeof(featureName));
      
      CorSnprintf(message, sizeof(message), "\n%s%s%s",  "Get Callback Event: ",featureName, " value has changed.\n");
		printf("%s", message);
   }
}

int main(int argc, char* argv[])
{
   char acqServerName[CORSERVER_MAX_STRLEN]="";
   UINT32   acqDeviceNumber=0;
	char message[STRING_LENGTH];


   printf("Sapera Console Camera Events Example (C++ version)\n");
   
	if (!GetCorAcqDeviceOptionsFromQuestions(acqServerName, &acqDeviceNumber))
   {
      printf("\nPress any key to terminate\n");
      _getch(); 
      return 0;
   }

   BOOL status;

   // Create a camera object
   SapAcqDevice device(acqServerName);
   status = device.Create();


   // Get the number of features provided by the camera
   int featureCount;
   status = device.GetFeatureCount(&featureCount);

   // Create an empty feature object (to receive information)
   SapFeature feature(acqServerName);
   status = feature.Create();
 
   //
   // Callback management
   //
   // Browse event list

   int numEvents;
   status = device.GetEventCount(&numEvents);

   int eventIndex;
	printf("Available events are:\n");
   for (eventIndex = 0; eventIndex < numEvents; eventIndex++)
   {
      char eventName[64];
      status = device.GetEventNameByIndex(eventIndex, eventName, sizeof(eventName));
		printf("  %s\n", eventName);
   }
   
   // Register event by name
	BOOL bIsRegistered=FALSE;

	status = device.IsCallbackRegistered("Feature Value Changed", &bIsRegistered);

	if (!bIsRegistered)
	 	status = device.RegisterCallback("Feature Value Changed", CameraCallback, NULL);

   // Modified a feature (Will trigger callback function)
	BOOL isGenie = FALSE,isAvailable = FALSE,isSFNCDeprecated = FALSE;
	double currentGainDouble=0;
	int currentGainInt=0;
   char modelName[64];

	device.IsFeatureAvailable("DeviceModelName", &status);
   if (status)
   {
      device.IsFeatureAvailable("FrameRate", &status);
      device.GetFeatureValue("DeviceModelName",modelName,sizeof(modelName));
      if (strstr(modelName,"Genie")!=0 && status)
      {
         isGenie = TRUE;   //using Genie Proprietary Driver
      }
   }

   status = false;
   if (device.IsFeatureAvailable("Gain", &isAvailable) && isAvailable)
   {
      status = device.GetFeatureInfo("Gain", &feature);
   }
   else if (device.IsFeatureAvailable("GainRaw", &isAvailable) && isAvailable)
   {
      status = device.GetFeatureInfo("GainRaw", &feature);
      isSFNCDeprecated = TRUE;
   }
   else
   {
      goto Feature_Not_Available;
   }

   float powValue;

   //using Genie Proprietary Driver or XML with depracated feature names
	if (isGenie || isSFNCDeprecated)		
	{
      INT32 gainMax=0, gainMin=0, gainValue=0;
      int gainExponent=0;
		status = feature.GetMax(&gainMax);
      status = feature.GetMin(&gainMin);
      status = feature.GetSiToNativeExp10(&gainExponent);
		powValue = pow((float)10, -gainExponent);
		

		// Get current Gain value in camera
      if (isSFNCDeprecated)
		   status = device.GetFeatureValue("GainRaw", &currentGainInt);
      else
         status = device.GetFeatureValue("Gain", &currentGainInt);

      CorSnprintf(message, sizeof(message), "%s%d%s","\nCurrent gain value is ", currentGainInt, "\n"); 
      printf("%s", message);

      //if Gain is already set at max, change it to minimum, else set it to max
      gainValue = (UINT32)(gainMax * powValue);
      if (currentGainInt == gainValue)
         gainValue = (UINT32)(gainMin * powValue);
         

      if (isSFNCDeprecated)
		   status = device.SetFeatureValue("GainRaw", gainValue);
      else
         status = device.SetFeatureValue("Gain", gainValue);
	   CorSnprintf(message, sizeof(message), "%s%d%s","\nSet Gain value to ", gainValue, "\n"); 
		printf("%s", message);


	}
	else	//any other camera
	{
      double gainMax=0, gainValue=0, gainMin=0; 
		int gainExponent=0;
		status = feature.GetMax(&gainMax);
      status = feature.GetMin(&gainMin);
		status = feature.GetSiToNativeExp10(&gainExponent);
		powValue = pow((float)10, -gainExponent);
		
		// Get current Gain value in camera
		status = device.GetFeatureValue("Gain", &currentGainDouble);
      CorSnprintf(message, sizeof(message), "%s%.2f%s","\nCurrent gain value is ", currentGainDouble, "\n"); 
      printf("%s", message);

      //Set gain to max. if it's already at max, set it to min.
      //The 0.001 adjustment is to avoid rounded numbers larger/lower
      //than max/min which would trigger errors on the camera.
      gainValue = (double)(gainMax * powValue - 0.001);
      if (currentGainDouble == gainValue)
         gainValue = (double)(gainMin * powValue + 0.001);

		// Set new Gain value in camera
		status = device.SetFeatureValue("Gain", gainValue);
		CorSnprintf(message, sizeof(message), "%s%.2f%s","\nSet Gain value to ", gainValue, "\n"); 
		printf("%s", message);		
	}
	
Feature_Not_Available:

   // Unregister event by name
	status = device.IsCallbackRegistered("Feature Value Changed", &bIsRegistered);

	if (bIsRegistered)
   status = device.UnregisterCallback("Feature Value Changed");

   // Set Gain value to old value
   if (isAvailable)
   {
      if (isGenie)   //using Genie Proprietary Driver
         status = device.SetFeatureValue("Gain", currentGainInt);
      else if (isSFNCDeprecated)    //other cameras using depracted SFNC feature names
	      status = device.SetFeatureValue("GainRaw", currentGainInt);
      else                       //other cameras using SFNC
   	   status = device.SetFeatureValue("Gain", currentGainDouble);
	   printf("Set Gain to old value.\n");

   }
   printf("\nPress any key to terminate\n");
   _getch();

	// Destroy acquisition object
	if (!device.Destroy()) return FALSE;

	// Destroy feature object
	if (!feature.Destroy()) return FALSE;

return 0;
}


