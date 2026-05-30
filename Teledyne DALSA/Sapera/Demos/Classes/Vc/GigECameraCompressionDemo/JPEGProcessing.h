#ifndef _JPEGPROCESSING_H_
#define _JPEGPROCESSING_H_

// JpegMyProcessing.h : header file
//
extern "C"
{
   //#undef FAR
   //#include "jinclude.h"
   //#include "jpeglib.h"
   //#include "jerror.h"
   #include "jpegdef.h"

   enum
   {
      IMAGE_FORMAT_UINT8 = 0,
      IMAGE_FORMAT_RGB8888 = 9
   };

   typedef struct jpeg_decompress_struct JPEGDECOMPRINFO, *PJPEGDECOMPRINFO;
   typedef int (*ReadJpegMemFunc)(void* o0, int w, int h, int mw, int type, void* iHeader, size_t iHeaderSize, void* iData, size_t iDataSize, PJPEGDECOMPRINFO pcinfo);
}


//
// JpegProcessing class declaration
//
class JpegProcessing
{
public:
   // Constructor/Destructor
   JpegProcessing(SapAcqDevice* device);
   ~JpegProcessing();

   JpegProcessing& operator=(const JpegProcessing& jpegPro);
   operator BOOL() const { return m_InitOK; }

   BOOL32 Create();
   BOOL32 Destroy();
   BOOL32 SetImageCompressionQuality(UINT32 imageCompressionQuality);
   UINT32 GetImageCompressionQuality();
   UINT32 GetImageCompressionQualityMin() { return m_ImageCompressionQualityMin; };
   UINT32 GetImageCompressionQualityMax() { return m_ImageCompressionQualityMax; };
   BOOL   GetImagePixelFormat();
   BOOL32 IsJpegAvailable() { return m_JpegAvailable; };
   BOOL32 IsImageCompressionModeJpeg();
   BOOL32 IsLastImageJpegDecodable();
   BOOL32 EnableJpegProcessing(BOOL32 enable, BOOL& rPixelFormatChanged);
   BOOL32 DecompressImageData(void* encodedData, size_t encodedDataSize, void* decodedData, int w, int h, int mw, int type);

protected:
   SapAcqDevice* m_AcqDevice;
   BOOL32 m_InitOK;
   double m_OldInterPacketTimeout;
   BOOL32 m_JpegAvailable;
   char m_ImageCompressionMode_EnumString_Jpeg[8];
   BOOL32 m_LastImageJpegDecodable;
   UINT32 m_ImageCompressionQuality;
   UINT32 m_PixelFormat;
   struct jpeg_decompress_struct m_Cinfo;
   UINT32 m_ImageCompressionQualityMin;
   UINT32 m_ImageCompressionQualityMax;
   HMODULE m_JpegLib;
   ReadJpegMemFunc m_pReadJpegMemFunc;
};

#endif // _JPEGPROCESSING_H_
