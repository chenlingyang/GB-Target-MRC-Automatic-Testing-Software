// TemperatureLog.h : header file
//

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

typedef struct
{
   int enumValue;
   char enumString[256];
} *P_TEMPERATURE_ENUM, TEMPERATURE_ENUM;

class TemperatureLog
{
public:
   TemperatureLog(char* filename, float seconds, SapAcqDevice* pAcqDevice)
   {
      strcpy_s(m_filename, sizeof(m_filename), filename);

      m_seconds = seconds;

      m_pAcqDevice = pAcqDevice;

      // Initialize feature enums for temperature reading
      BOOL isAvailable = FALSE;
      m_numTemperatures = 0;
      m_temperatureEnum = NULL;
      m_pAcqDevice->IsFeatureAvailable("DeviceTemperatureSelector", &isAvailable);
      if (isAvailable)
      {
         SapFeature* featureDeviceTemperatureSelector;
         featureDeviceTemperatureSelector = new SapFeature(pAcqDevice->GetLocation());
         if (featureDeviceTemperatureSelector)
         {
            featureDeviceTemperatureSelector->Create();

            if ( m_pAcqDevice->GetFeatureInfo("DeviceTemperatureSelector", featureDeviceTemperatureSelector))
            {
               int numTemperatures;
               featureDeviceTemperatureSelector->GetEnumCount(&numTemperatures);

               m_temperatureEnum = new TEMPERATURE_ENUM[numTemperatures];

               int i;
               for (i = 0; i < numTemperatures; i++)
               {
                  int value;

                  featureDeviceTemperatureSelector->GetEnumValue(i, &value);

                  // Test for duplicated entry
                  int j;
                  for ( j = i - 1; j >= 0; j--)
                  {
                     if (value == m_temperatureEnum[j].enumValue)
                        break; // Found a duplicate: j won't reach -1 so we won't put this enum in the list
                  }

                  if (j == -1)
                  {
                     // Add temperature sensor to the list
                     m_temperatureEnum[m_numTemperatures].enumValue = value;

                     featureDeviceTemperatureSelector->GetEnumStringFromValue(value, m_temperatureEnum[m_numTemperatures].enumString, sizeof(m_temperatureEnum[m_numTemperatures].enumString));

                     m_numTemperatures++;
                  }
               }
            }

            delete featureDeviceTemperatureSelector;
         }
      }

      m_perf.Reset();
   };
   ~TemperatureLog()
   {
      if (m_temperatureEnum)
         delete m_temperatureEnum;
   };

   void Log()
   {
      LogAndReset(TRUE);
   };
   BOOL LogIfElapsed()
   {
      // Test if the lapse is reached
      if (m_perf.GetTime(FALSE) >= m_seconds)
      {
         LogAndReset();
         return TRUE;
      }
      else
         return FALSE;
   };

private:
   void LogAndReset(BOOL firstLog = FALSE)
   {
      SYSTEMTIME sysTime;

      // Reset for next LogIfElapsed()
      m_perf.Reset();

      GetLocalTime(&sysTime);

      // Output log to file
      FILE* myFile;
      errno_t myErrno_t = fopen_s(&myFile, m_filename, "a+");

      if (!myErrno_t)
      {
         if (firstLog)
         {
            char deviceIdString[256];
            deviceIdString[0] = 0;
            m_pAcqDevice->GetFeatureValue("DeviceID", deviceIdString, sizeof(deviceIdString));
            fprintf(myFile, "\nStarting temperature log for DeviceID %s\n", deviceIdString);
         }

         fprintf(myFile, "%04d-%02d-%02d %02dh%02dm%02d.%03ds :",
            sysTime.wYear, sysTime.wMonth, sysTime.wDay, sysTime.wHour, sysTime.wMinute, sysTime.wSecond, sysTime.wMilliseconds);

         int i;
         for (i = 0; i < m_numTemperatures; i++)
         {
            // Read temperature from sensors in the camera
            m_pAcqDevice->SetFeatureValue("DeviceTemperatureSelector", m_temperatureEnum[i].enumValue);
            double temperature;
            m_pAcqDevice->GetFeatureValue("DeviceTemperature", &temperature);
            fprintf(myFile, " %s=% 2.0f deg,", m_temperatureEnum[i].enumString, temperature);
         }

         fprintf(myFile, "\n");

         fclose(myFile);
      }
   };

   char m_filename[512];
   float m_seconds;
   SapAcqDevice* m_pAcqDevice;
   SapPerformance m_perf;
   int m_numTemperatures;
   P_TEMPERATURE_ENUM m_temperatureEnum;
};
