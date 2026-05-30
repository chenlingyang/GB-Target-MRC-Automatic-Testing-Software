// JPEGProcessing.cpp : implementation file
//

#include "SapClassBasic.h"
#include "JpegProcessing.h"


JpegProcessing::JpegProcessing(SapAcqDevice* device)
{
   m_ImageCompressionQuality = 84;
   m_AcqDevice = device;
   m_InitOK = FALSE;
   m_JpegAvailable = FALSE;
   m_LastImageJpegDecodable = FALSE;
   m_ImageCompressionQualityMin = 0;
   m_ImageCompressionQualityMax = 0;
   m_PixelFormat = 0;
   m_JpegLib = NULL;
   m_pReadJpegMemFunc = NULL;
   m_ImageCompressionMode_EnumString_Jpeg[0] = NULL;
}

JpegProcessing::~JpegProcessing()
{
   if (m_InitOK)
      Destroy();
}

BOOL32 JpegProcessing::Create()
{
   m_InitOK = FALSE;

   // Augment feature InterPacketTimeout to 100 ms to be sure to not ask for resend packet when there can be much time between each packet:
   // - if compression quality is high, the camera engine is slower to encode packets
   // - if compression quality is low and the number of compressed packets is small, the engine must use enough source image data before outputing each packet
   BOOL isAvailable = FALSE;
   m_AcqDevice->IsFeatureAvailable("InterPacketTimeout", &isAvailable);
   if (isAvailable)
   {
      m_AcqDevice->GetFeatureValue("InterPacketTimeout", &m_OldInterPacketTimeout);
      m_AcqDevice->SetFeatureValue("InterPacketTimeout", 0.1);
   }

   //
   // Need to have feature ImageCompressionMode with Jpeg value accessible,
   // and ImageCompressionQuality to work with this demo. Else, JPEG won't be checkable.
   //
   isAvailable = FALSE;
   m_AcqDevice->IsFeatureAvailable("ImageCompressionMode", &isAvailable);
   if (isAvailable)
   {
      m_JpegLib = LoadLibrary("jpegfiles.dll");
      if (m_JpegLib)
         m_pReadJpegMemFunc = (ReadJpegMemFunc)GetProcAddress(m_JpegLib, "readJPEGmem");

      if (m_JpegLib && m_pReadJpegMemFunc)
      {
         SapFeature* featureImageCompressionMode;
         featureImageCompressionMode = new SapFeature(m_AcqDevice->GetLocation());
         featureImageCompressionMode->Create();

         if ( m_AcqDevice->GetFeatureInfo("ImageCompressionMode", featureImageCompressionMode))
         {
            int i;
            int enumCount;
            featureImageCompressionMode->GetEnumCount(&enumCount);
            for (i = 0; i < enumCount; i++)
            {
               char enumString[8];
               featureImageCompressionMode->GetEnumString(i, enumString, sizeof(enumString));
               if (!strcmp(enumString, "JPEG")) // If enumString equals "Jpeg", it is time to update the camera firmware!
               {
                  strcpy_s(m_ImageCompressionMode_EnumString_Jpeg, sizeof(m_ImageCompressionMode_EnumString_Jpeg), enumString);

                  SapFeature* featureImageCompressionQuality = new SapFeature(m_AcqDevice->GetLocation());
                  featureImageCompressionQuality->Create();

                  if (m_AcqDevice->GetFeatureInfo("ImageCompressionQuality", featureImageCompressionQuality))
                  {
                     featureImageCompressionQuality->GetMin(&m_ImageCompressionQualityMin);
                     featureImageCompressionQuality->GetMax(&m_ImageCompressionQualityMax);

                     m_InitOK = TRUE;
                     m_JpegAvailable = TRUE;
                  }

                  featureImageCompressionQuality->Destroy();
                  delete featureImageCompressionQuality;

                  if (m_JpegAvailable == TRUE)
                     break;
               }
            }
         }

         featureImageCompressionMode->Destroy();
         delete featureImageCompressionMode;
      }
   }
   else
   {
      m_InitOK = TRUE;
   }

   // Identify current GigE pixel format to later know if it changes by enabling JPEG
   GetImagePixelFormat();

   return m_InitOK;
}

BOOL32 JpegProcessing::Destroy()
{
   if (m_InitOK)
   {
      m_InitOK = FALSE;
      m_JpegAvailable = FALSE;
   }

   BOOL isAvailable = FALSE;
   m_AcqDevice->IsFeatureAvailable("InterPacketTimeout", &isAvailable);
   if (isAvailable)
      m_AcqDevice->SetFeatureValue("InterPacketTimeout", m_OldInterPacketTimeout);

   if (m_JpegLib)
   {
      m_pReadJpegMemFunc = NULL;
      FreeLibrary(m_JpegLib);
      m_JpegLib = NULL;
   }

   return TRUE;
}

BOOL32 JpegProcessing::SetImageCompressionQuality(UINT32 imageCompressionQuality)
{
   if (imageCompressionQuality > m_ImageCompressionQualityMax)
      imageCompressionQuality = m_ImageCompressionQualityMax;

   if (imageCompressionQuality < m_ImageCompressionQualityMin)
      imageCompressionQuality = m_ImageCompressionQualityMin;

   if ( m_AcqDevice->SetFeatureValue("ImageCompressionQuality", imageCompressionQuality))
   {
      m_ImageCompressionQuality = imageCompressionQuality;
      return TRUE;
   }

   return FALSE;
}

UINT32 JpegProcessing::GetImageCompressionQuality()
{
   UINT32 imageCompressionQuality = 0;
   if (m_AcqDevice->GetFeatureValue("ImageCompressionQuality", &imageCompressionQuality))
      m_ImageCompressionQuality = imageCompressionQuality;

   return m_ImageCompressionQuality;
}

BOOL JpegProcessing::GetImagePixelFormat()
{
   UINT32 pixelFormat = 0;
   if (m_AcqDevice->GetFeatureValue("PixelFormat", &pixelFormat))
   {
      if (pixelFormat != m_PixelFormat)
      {
         m_PixelFormat = pixelFormat;
         return TRUE;
      }
   }

   return FALSE;
}


BOOL32 JpegProcessing::IsImageCompressionModeJpeg()
{
   char featureString[16];
   if (m_JpegAvailable && m_AcqDevice->GetFeatureValue("ImageCompressionMode", featureString, sizeof(featureString)))
   {
      return (strncmp(featureString, "JPEG", sizeof(featureString))) // If featureString equals "Jpeg", it is time to update the camera firmware!
         ? false : true;
   }

   return false;
}

BOOL32 JpegProcessing::IsLastImageJpegDecodable()
{
   return (IsImageCompressionModeJpeg() && m_LastImageJpegDecodable);
}


BOOL32 JpegProcessing::EnableJpegProcessing(BOOL32 enable, BOOL& rPixelFormatChanged)
{
   BOOL retCode = m_AcqDevice->SetFeatureValue("ImageCompressionMode", enable ? m_ImageCompressionMode_EnumString_Jpeg : "Off");
   if (enable)
      GetImageCompressionQuality(); // Get image compression quality (in case it is updated by the camera).

   // Test if image pixel format changed (in case it is updated by the camera, like in color cameras where JPEG supports only color format)
   // We need only to know if it changed since we will use Sapera constructor with acquisition as a parameter to convert the GigE format to
   // Sapera format.
   rPixelFormatChanged = GetImagePixelFormat();

   return retCode;
}

BOOL32 JpegProcessing::DecompressImageData(void* encodedData, size_t encodedDataSize, void* decodedData, int w, int h, int mw, int type)
{
   if ((encodedData != NULL) && (decodedData != NULL))
   {
      int status = 0;
      JPEGDECOMPRINFO jpeginfo;

      // In the future, this call could be replaced by a new class implementation in Sapera LT.
      // In the meantime, here are the values to pass to the function:
      // void* o0, int w, int h, int mw, int type, void* iHeader, size_t iHeaderSize, void* iData, size_t iDataSize, PJPEGDECOMPRINFO pcinfo
      // o0: pointer to the start of the data where to put the decoded image. In the case of an ROI, start of the ROI data.
      // w: destination image width or ROI width (not in bytes but in pixels).
      // h: destination image height or ROI height.
      // mw: destination image width (different from w only if an ROI is used in destination image).
      // type: IMAGE_FORMAT_UINT8 or IMAGE_FORMAT_RGB8888.
      // iHeader: Set to NULL. In the case that the JPEG header is separated from the rest, set it to the start of the header. With Genie TS: set to NULL. Could be removed in the future.
      // iHeaderSize: Set to 0 if iHeader is set to NULL. Could be removed in the future.
      // iData: pointer to the start of encoded JPEG data, including header (Genie TS always encode the JPEG header with the rest of the data).
      // iDataSize: size of the encoded JPEG data. Could be bigger than the encoded data (it won't all be read if the decoder finds the end of the JPEG image).
      //            If it is too small, it won't decode properly; be sure that the size does not overlap inaccessible memory.
      //            You can use the full size of the buffer if you don't want to read the SpaceUsed by the data for each frame. The only case when it could loose time is if
      //            the data is corrupted, so using SpaceUsed is optimal in all cases.
      // pcinfo: pointer to a JPEGDECOMPRINFO structure. You have to put one (uninitialized). It will stay empty but you must put one for a possible future implementation.
      // return value: data decoded correctly if != 0.
      status = m_pReadJpegMemFunc(decodedData, w, h, mw, type, NULL, 0, encodedData, encodedDataSize, &jpeginfo);
      BOOL32 bStatus = (status != 0);

      m_LastImageJpegDecodable = bStatus;

      return bStatus;
   }

   return FALSE;
}
