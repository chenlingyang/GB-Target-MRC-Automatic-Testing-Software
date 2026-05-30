// CameraFeatures.cpp 
// This example gives the user an insight on how to access features from a camera.
// The program will retrieve a list of features and display their access mode, data type and value.
// There is also an example of how to change the value of a feature as well as an example of how 
// to trigger a callback once a value has been changed.
//


// Disable deprecated function warnings with Visual Studio 2005
#pragma warning(disable: 4127)
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

#define TABLE1_LIMIT    13
#define TABLE2_LIMIT    4
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
		printf(message);
	}
}

int main(int argc, char* argv[])
{
	//redirect error messages to log viewer
	//SapManager::SetDisplayStatusMode(SapManager::StatusLog);

	//list of features based on DFNC - Dalsa Features Naming Convention (Falcon2, Genie TS)
	char *featuresNamesSFNCTable1[] = 
	{
		"DeviceVendorName",
		"DeviceModelName",
		"DeviceVersion",
		"DeviceFirmwareVersion",
		"DeviceID",
		"DeviceUserID",
		"DeviceScanType",
		"sensorColorType",
		"SensorWidth",
		"SensorHeight",
		"AcquisitionFrameRate",
		"Gain",
		"BinningHorizontal"
	};
	//list of features based on Genie naming convention
	char *featuresNamesGenieTable1 [] = 
	{
		"DeviceVendorName",
		"DeviceModelName",
		"DeviceVersion",
		"FirmwareVersion",
		"DeviceID",
		"DeviceUserID",
		"DeviceScanType",
		"ColorType",
		"SensorWidth",
		"SensorHeight",
		"FrameRate",
		"Gain"
	};


   //list of SFNC features for the 2nd table of information
   char *featuresNamesSFNCTable2[] = 
	{
		"DeviceVendorName",
		"sensorColorType",
		"Gain",
		"BinningHorizontal"
	};

   //list of Genie features for the 2nd table of information
   char *featuresNamesGenieTable2 [] = 
	{
		"DeviceVendorName",
		"DeviceScanType",
		"Gain"
	};
   char **g_featuresNames;

	printf("Sapera Console Camera Features Example (C++ version)\n");

	char acqServerName[CORSERVER_MAX_STRLEN]="";
	UINT32   acqDeviceNumber=0;
	if (!GetCorAcqDeviceOptionsFromQuestions(acqServerName, &acqDeviceNumber))
	{
		printf("\nPress any key to terminate\n");
		_getch(); 
		return 0;
	}

	BOOL status, isGenie = FALSE;
	int featureIndex = 0;
   int numberOfFeatures = 0;
	

	// Create a SapAcqDevice object
	SapAcqDevice device(acqServerName);
	status = device.Create();

	// Create an empty feature object (to receive information)
	SapFeature feature(acqServerName);
	status = feature.Create();

   device.GetFeatureCount(&numberOfFeatures);
   printf("Total number of features present on the device: %u\n\n",numberOfFeatures);

   if (numberOfFeatures == 0)
      goto cleanup;

   do
   {
      printf("\nPlease select one of the following options:\n\n");
      printf("1. Display all %u feature names.\n",numberOfFeatures);
      printf("2. Display detailed list of selected features.\n");
      printf("3. Exit program.\n\n");
      BOOL first = TRUE;
      BOOL isAvailable = FALSE;
      char key;
      do 
      {
         if (first)
         {
           key = (char)_getch();
           first = FALSE;
         }
         else
         {
            printf("Invalid choice ! Please try again ! \n");
            key = (char)_getch();
         }
      }while (key != '1' && key != '2' && key != '3');
      
      //display all features
      if (key == '1')
      {
         printf("%-60s\n","NAME\n");
         for (featureIndex = 0; featureIndex < numberOfFeatures; featureIndex++)
         {
            char featureName[60];
            status = device.GetFeatureNameByIndex(featureIndex,featureName,sizeof(featureName));
            if (status)
               printf("%u. %-60s\n",featureIndex,featureName);
            else
               printf("%u. N/A\n",featureIndex);
         }
      }
      //display only a few features with detailed information
      else if( key == '2')
      {
         printf("%-25s%-10s\t%-10s\t%s\n\n","NAME","ACCESS","TYPE","VALUE");

	      
	     //determine if device is Genie or something else
         char modelName[64];
	      device.IsFeatureAvailable("DeviceModelName", &status);
         if (status)
         {
            device.IsFeatureAvailable("FrameRate", &status);
            device.GetFeatureValue("DeviceModelName",modelName,sizeof(modelName));
            if (strstr(modelName,"Genie")!=0 && status)
            {
               g_featuresNames = &featuresNamesGenieTable1[0];
               isGenie = TRUE;
            }
            else
            {
               g_featuresNames = &featuresNamesSFNCTable1[0];
            }
         }
         else
         {
            g_featuresNames = &featuresNamesSFNCTable1[0];
         }

         int limit = TABLE1_LIMIT;
         if (isGenie)
            limit--;
         
         // Browse features list by index (display only certain features)
	      for (featureIndex = 0; featureIndex < limit;  featureIndex++)
	      {
		      char featureStringValue[64];
		      char enumString[64];
		      char featureName[64];
		      INT64 featureIntVal64;
            int enumValue, featureIntVal32;
		      double featureDoubleValue;
		      SapFeature::AccessMode myAccess;
		      SapFeature::Type myType;
            SapFeature::IncrementType myIncrement;
            BOOL isINT64 = FALSE;

		      //display feature name
            status = FALSE;
		      if (device.IsFeatureAvailable(g_featuresNames[featureIndex],&isAvailable) && isAvailable)
			      status = device.GetFeatureInfo(g_featuresNames[featureIndex], &feature);
         
		      if (status)
		      {
			      status = feature.GetAccessMode(&myAccess);
			      status = feature.GetType(&myType);
			      status = feature.GetDisplayName(featureName,sizeof(featureName));
			      status = feature.GetIncrementType(&myIncrement);
			      printf("%-25s",featureName);

			      //display feature access mode
			      switch (myAccess)
			      {
				      case CORFEATURE_VAL_ACCESS_MODE_UNDEFINED:
				      {
					      printf("%-10s\t","Undefined");
					      break;
				      }
				      case CORFEATURE_VAL_ACCESS_MODE_RW:
				      {
					      printf("%-10s\t","ReadWrite");
					      break;
				      }
				      case CORFEATURE_VAL_ACCESS_MODE_RO:
				      {
					      printf("%-10s\t","ReadOnly");
					      break;
				      }
				      case CORFEATURE_VAL_ACCESS_MODE_WO:
				      {
					      printf("%-10s\t","WriteOnly");
					      break;
				      }
				      case CORFEATURE_VAL_ACCESS_MODE_NP:
				      {
					      printf("%-10s\t","NotPresent");
					      break;
				      }
				      case CORFEATURE_VAL_ACCESS_MODE_NE:
				      {
					      printf("%-10s\t","NotEnabled");
                     continue;
				      }

			      }
      	
			      switch (myType)
			      {
				      //display strings
				      case CORFEATURE_VAL_TYPE_STRING:
				      {
                     status = false;
                     status = device.GetFeatureValue(g_featuresNames[featureIndex],featureStringValue,sizeof(featureStringValue));
                     
                     printf("%-10s\t","String");
					      if (status)
					      {
						      printf(featureStringValue);
					      }
					      break;
				      }	
				      //display integers and booleans
				      case CORFEATURE_VAL_TYPE_INT32:
				      {
					      printf("%-10s\t","Int32");
					      goto Print;
				      }
				      case CORFEATURE_VAL_TYPE_INT64:
				      {
					      printf("%-10s\t","Int64");
                     isINT64 = TRUE;
					      goto Print;
				      }
				      case CORFEATURE_VAL_TYPE_BOOL:
				      {
					      printf("%-10s\t","Bool");
      Print:
                     status = false;
                     char *formattingString;

                     if (isINT64)
                     {
                        status = device.GetFeatureValue(g_featuresNames[featureIndex],&featureIntVal64);
                        formattingString = "%I64d";
                     }
                     else
                     {
                        status = device.GetFeatureValue(g_featuresNames[featureIndex],&featureIntVal32);
                        formattingString = "%d";
                     }
				         if (status)
   			         {
                        printf(formattingString,isINT64 ? featureIntVal64 : featureIntVal32);
                     }
					      break;
				      }
				      //display enumerations
				      case CORFEATURE_VAL_TYPE_ENUM:
				      {
                     status = false;
                     status = device.GetFeatureValue(g_featuresNames[featureIndex],&enumValue);
                     
   			         printf("%-10s\t","Enum");
				         if (status)
				         {
      				      feature.GetEnumStringFromValue(enumValue,enumString,sizeof(enumString));
	   				      printf(enumString);
		  			      }
         		      break;
				      }
				      //display doubles
				      case CORFEATURE_VAL_TYPE_DOUBLE:
				      {
                     status = false;
                     status = device.GetFeatureValue(g_featuresNames[featureIndex],&featureDoubleValue);
                     					      
				         printf("%-10s\t","Double");
				         if (status)
				         {
  						      printf("%.3f",featureDoubleValue);
   				      }
					      break;
				      }
				      default:
				      {
					      printf("N/A");
				      }
			      }
			       printf("\n");
		      }
	      }

	      printf("\n\n");


         //select the second table of features
         if (isGenie)
            g_featuresNames = &featuresNamesGenieTable2[0];
         else
            g_featuresNames = &featuresNamesSFNCTable2[0];


         //display table 2 

         printf("%-25s%-10s\t%-10s\t\t%s\n\n","NAME","TYPE","RANGE","POSSIBLE VALUE(S)");

         limit = TABLE2_LIMIT;
         if (isGenie)
            limit--;

	      for (featureIndex = 0; featureIndex < limit;  featureIndex++)
	      {
            char featureStringValue[64];
		      char featureName[64];
		      INT64 enumValue;
		      double featureDoubleValue;
            int featureIntValue;
            SapFeature::IncrementType myIncrement;
            SapFeature::Type myType;

		      //display feature name
            status = FALSE;
		      if (device.IsFeatureAvailable(g_featuresNames[featureIndex],&isAvailable) && isAvailable)
			      status = device.GetFeatureInfo(g_featuresNames[featureIndex], &feature);
         
		      if (status)
		      {
               status = feature.GetDisplayName(featureName,sizeof(featureName));
               status = feature.GetType(&myType);
               status = feature.GetIncrementType(&myIncrement);
			      printf("%-25s",featureName);
               
			      switch (myType)
			      {
				      //display strings
				      case CORFEATURE_VAL_TYPE_STRING:
				      {
                     status = device.GetFeatureValue(g_featuresNames[featureIndex],featureStringValue,sizeof(featureStringValue));
                     printf("%-10s\t","String");
                     INT32 min,max;
                     status = feature.GetMin(&min);
                     status = feature.GetMax(&max);

                     if (status)
					      {
					         printf("%d - %d (string length)\t",min,max);
					         printf("N/A",min,max);
					      }
                     break;
				      }	
				      
				      case CORFEATURE_VAL_TYPE_INT64:
				      {
                     status = false;

                     printf("%-10s\t","INT64");
                     INT64 min,max;
                     
                     status = feature.GetMin(&min);
                     status = feature.GetMax(&max);

                     if (status)
				         {
                        if (myIncrement == SapFeature::IncrementList)
                        {
                           int validValueCount;
                           feature.GetValidValueCount(&validValueCount);
   				            printf("N/A\t\t\t");
                           for (int i = 0; i < validValueCount; i++)
                           {
                              status = feature.GetValidValue(i,&enumValue);
                              if (i < validValueCount - 1)
                                 printf("%I64d, ",enumValue);
                              else
                                 printf("%I64d",enumValue);
                           }
                           break;
                        }
                        else
                        {
   				            printf("%I64d - %-10I64d\t\t",min,max);
                        }
				         }
					      break;
				      }
				      //display enumerations
				      case CORFEATURE_VAL_TYPE_ENUM:
				      {
                     status = false;
                     status = device.GetFeatureValue(g_featuresNames[featureIndex],&enumValue);
                     printf("%-10s\t","Enum");
                     printf("%-10s\t\t","N/A");
                     int enumCount;
                     char enumString[32];

                     feature.GetEnumCount(&enumCount);
                     for (int i = 0; i < enumCount; i++)
                     {
                        feature.GetEnumString(i,enumString,sizeof(enumString));
         			      if (i < enumCount - 1)
                           printf("%s, ",enumString);
                        else
                           printf("%s",enumString);
                     }

					      break;
				      }
                  case CORFEATURE_VAL_TYPE_DOUBLE:
				      {
                     status = FALSE;
                     double min,max;

                     status = device.GetFeatureValue(g_featuresNames[featureIndex],&featureDoubleValue);
                  					      
				         printf("%-10s\t","Double");
                     status = feature.GetMin(&min);
                     status = feature.GetMax(&max);
                     if (status)
				         {
				            printf("%.3f - %-8.3f\t",min,max);
						      printf("N/A");
   				      }
					      break;
				      }
                  case CORFEATURE_VAL_TYPE_INT32:
				      {
					      status = FALSE;
                     int min,max;
                     status = device.GetFeatureValue(g_featuresNames[featureIndex],&featureIntValue);
                  					      
				         printf("%-10s\t","INT32");
                     status = feature.GetMin(&min);
                     status = feature.GetMax(&max);
                     if (status)
				         {
				            printf("%d - %-10d\t\t",min,max);
						      printf("N/A");
   				      }
					      break;
				      }
				      default:
				      {
					      printf("N/A");
				      }

			      }
               
			      printf("\n");
           
		      }
         }

	      printf("\n\n");


	      // get DeviceUserID value from the device using the name of the feature
		   SapFeature::AccessMode accessMode = SapFeature::AccessUndefined;
         status = false;

         if (device.IsFeatureAvailable("DeviceUserID",&isAvailable))
         {
            if (device.GetFeatureInfo("DeviceUserID", &feature) && feature.GetAccessMode(&accessMode))
               status = true;
         }

         if (status && isAvailable && accessMode == SapFeature::AccessRW)
         {
            BOOL bIsRegistered=FALSE;
	         char currentUserID[64];

	         status = device.GetFeatureValue("DeviceUserID", currentUserID, sizeof(currentUserID));

            // Register event by name and a callback function will be performed when the event occurs
	         status = device.IsCallbackRegistered("Feature Value Changed", &bIsRegistered);

	         if (!bIsRegistered)
		         status = device.RegisterCallback("Feature Value Changed", CameraCallback, NULL);

	         // Set new value for DeviceUserID feature in camera
	         status = device.SetFeatureValue("DeviceUserID", "Testdevice");

	         printf("Set DeviceUserID to Testdevice");

	         // Unregister event by name
	          status = device.IsCallbackRegistered("Feature Value Changed", &bIsRegistered);

	         if (bIsRegistered)
		         status = device.UnregisterCallback("Feature Value Changed");

	         // Restore DeviceUserID value to old value
	         status = device.SetFeatureValue("DeviceUserID", currentUserID);
	         printf("Set DeviceUserID to old value: %s\n",currentUserID);
         }
      }
      else
      {
         break;
      }
   }while(1);

cleanup:
	printf("\nPress any key to terminate\n");
	_getch();

	// Destroy acquisition object
	if (!device.Destroy()) return FALSE;

	// Destroy feature object
	if (!feature.Destroy()) return FALSE;

return 0;
}
