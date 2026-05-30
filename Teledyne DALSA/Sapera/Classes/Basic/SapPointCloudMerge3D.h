#ifndef _SAP_POINT_CLOUD_MERGE_3D_H_
#define _SAP_POINT_CLOUD_MERGE_3D_H_

#include "SapClassBasic.h"

class SAPCLASSBASIC_CLASS SapPointCloudMerge3D
{
public:
   SapPointCloudMerge3D();
   ~SapPointCloudMerge3D();

   struct PointRemovalMode
   {
      enum Enum
      {
         KeepAll,      // Keep all points as-is.
         RemovePoints, // Remove some points that are close to some other points.
         VoxelCentroid // Replace all points within a voxel with their centroid.
      };
   };

   // Default value is PointRemovalMode::KeepAll.
   PointRemovalMode::Enum GetPointRemovalMode() const;
   void SetPointRemovalMode(PointRemovalMode::Enum new_mode);

   /* Points closer to each other than the given threshold are replaced with one point.
    *
    * Default value is 1.0.
    */
   double GetDistanceThreshold() const;
   void SetDistanceThreshold(double new_threshold);

   /* Adds an input point cloud to merge with other inputs.
    *
    * Call this for each of your inputs, then call Execute().
    * The pointed-to SapBuffer objects must be in SapFormatCoord3D_PC_XYZ format,
    * and those objects must stay alive by the time the Execute() method is called.
    * Returns FALSE if the input is invalid (e.g., unsupported format).
    * Returns TRUE if the input has been accepted for the next merge operation.
    */
   BOOL AddInput(const SapBuffer * input);

   /* Forgets about all previously added inputs.
    *
    * The previously added SapBuffer objects are not modified in any way;
    * the only effect of this method is that those objects
    * won't be merged upon the next call to Execute().
    */
   BOOL ClearInputs();

   /* Merges input point clouds and optionally subsamples the result.
    *
    * Before calling Execute(), first add the input SapBuffer objects by calling
    * AddInput() for each input. All inputs must be in SapFormatCoord3D_PC_XYZ format.
    * If no inputs have been added with AddInput() before calling Execute(),
    * or if inputs were cleared with ClearInputs() and no further AddInput() calls have been made,
    * Execute() returns FALSE to protect from inadvertent failures to call AddInput() first.
    * 
    * The Execute() function operates only on the current buffers
    * of the input and output SapBuffer objects.
    *
    * When PointRemovalMode option is KeepAll (the default value),
    * all inputs are simply merged together without removing any points, even invalid ones.
    * When PointRemovalMode is RemovePoints or VoxelCentroid,
    * invalid points are removed, and the output may have fewer points
    * than the total number of points in all inputs.
    * At the same time, the output must have at least one point
    * because creating an empty SapBuffer is impossible.
    * When all input points are invalid, the output is a single invalid point.
    *
    * The output may refer to the same object as one of the inputs.
    * In this case, a temporary copy of that input is made
    * before updating the output, so that no input data is lost.
    *
    * The output is usually an unorganized point cloud (its height is 1).
    * However, when there is only one input and PointRemovalMode is KeepAll,
    * the output preserves the width and height of the input.
    */
   BOOL Execute(SapBuffer * output);

protected:
   // Disable copying and assignment.
   SapPointCloudMerge3D(const SapPointCloudMerge3D &);
   SapPointCloudMerge3D & operator=(const SapPointCloudMerge3D &);

   // Hide implementation details.
   struct Impl;
   Impl * impl;
};

#endif
