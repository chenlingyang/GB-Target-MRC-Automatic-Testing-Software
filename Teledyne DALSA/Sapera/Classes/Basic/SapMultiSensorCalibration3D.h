#ifndef _SAP_MULTI_SENSOR_CALIBRATION_3D_H_
#define _SAP_MULTI_SENSOR_CALIBRATION_3D_H_

#include <string>

class SapMultiSensorCalibration3D
{
public:
   SAPCLASSBASIC_CLASS SapMultiSensorCalibration3D();
   SAPCLASSBASIC_CLASS ~SapMultiSensorCalibration3D();

   SAPCLASSBASIC_CLASS bool Load(const char * file_path);

   SAPCLASSBASIC_CLASS int GetObservationsCount() const;

   std::string GetObservationName(int observation_index) const { return GetObservationNameImpl(observation_index); }
   std::string GetObservedTarget(int observation_index) const { return GetObservedTargetImpl(observation_index); }
   std::string GetObservedFeature(int observation_index) const { return GetObservedFeatureImpl(observation_index); }
   std::string GetObservingSensor(int observation_index) const { return GetObservingSensorImpl(observation_index); }

   /* Apply calibration to an acquired buffer
    *
    * The input buffer must be in either C16, AC16, or ACRW16 format.
    * The output buffer is reset to the needed size and Coord3D_PC_XYZ format.
    */
   SAPCLASSBASIC_CLASS bool ApplyCalibration(int observation_index, const SapBuffer & input, SapBuffer * output) const;

protected:
   SAPCLASSBASIC_CLASS const char * GetObservationNameImpl(int observation_index) const;
   SAPCLASSBASIC_CLASS const char * GetObservedTargetImpl(int observation_index) const;
   SAPCLASSBASIC_CLASS const char * GetObservedFeatureImpl(int observation_index) const;
   SAPCLASSBASIC_CLASS const char * GetObservingSensorImpl(int observation_index) const;

   // Disable copying and assignment.
   SapMultiSensorCalibration3D(const SapMultiSensorCalibration3D &);
   SapMultiSensorCalibration3D & operator=(const SapMultiSensorCalibration3D &);

   struct Impl;
   Impl * impl;
};

#endif
