using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using DALSA.SaperaLT.SapClassBasic;

namespace DALSA.SaperaLT.Demos.NET.CSharp.CameraCompressionDemo
{
   class JpegProcessing
   {
      private bool m_Initialized;
      private bool m_JpegAvailable;
      private bool m_LastImageJpegDecodable;
      private SapAcqDevice m_AcqDevice;
      private UInt32 m_ImageCompressionQualityMin;
      private UInt32 m_ImageCompressionQualityMax;
      private UInt32 m_ImageCompressionQuality;
      private UInt32 m_PixelFormat;
      private double m_OldInterPacketTimeout;
      private string m_ImageCompressionMode_EnumString_Jpeg;
      public static int IMAGE_FORMAT_UINT8 = 0;
      public static int IMAGE_FORMAT_RGB8888 = 9;

      [DllImport("jpegfiles.dll")]
      public static extern int readJPEGmem(IntPtr outData, int w, int h, int mw, int type, IntPtr iHeader, uint iHeaderSize, IntPtr inData, int iDataSize, IntPtr pcinfo);

      // Constructor

      public JpegProcessing(SapAcqDevice device)
      {
         m_Initialized = false;
         m_JpegAvailable = false;
         m_LastImageJpegDecodable = false;
         m_AcqDevice = device;
         m_ImageCompressionMode_EnumString_Jpeg = null;
      }

      public void Dispose()
      {
      }

      public bool Initialized()
      {
         return m_Initialized;
      }

      public bool JpegAvailable()
      {
         return m_JpegAvailable;
      }

      public bool Create()
      {
         m_Initialized = false;

         // Augment feature InterPacketTimeout to 100 ms to be sure to not ask for resend packet when there can be much time between each packet:
         // - if compression quality is high, the camera engine is slower to encode packets
         // - if compression quality is low and the number of compressed packets is small, the engine must use enough source image data before outputing each packet
         if (m_AcqDevice.IsFeatureAvailable("InterPacketTimeout"))
         {
            m_AcqDevice.GetFeatureValue("InterPacketTimeout", out m_OldInterPacketTimeout);
            m_AcqDevice.SetFeatureValue("InterPacketTimeout", 0.1);
         }

         //
         // Need to have feature ImageCompressionMode with Jpeg value accessible,
         // and ImageCompressionQuality to work with this demo. Else, JPEG won't be checkable.
         //
         bool isAvailable = m_AcqDevice.IsFeatureAvailable("ImageCompressionMode");
         if (isAvailable == true)
         {
            SapFeature featureImageCompressionMode;
            featureImageCompressionMode = new SapFeature(m_AcqDevice.Location);
            featureImageCompressionMode.Create();

            if (m_AcqDevice.GetFeatureInfo("ImageCompressionMode", featureImageCompressionMode))
            {
               int i;
               for (i = 0; i < featureImageCompressionMode.EnumCount; i++)
               {
                  if (featureImageCompressionMode.EnumText[i].CompareTo("JPEG") == 0)
                  {
                     m_ImageCompressionMode_EnumString_Jpeg = featureImageCompressionMode.EnumText[i];

                     SapFeature featureImageCompressionQuality = new SapFeature(m_AcqDevice.Location);
                     featureImageCompressionQuality.Create();

                     if (m_AcqDevice.GetFeatureInfo("ImageCompressionQuality", featureImageCompressionQuality))
                     {
                        featureImageCompressionQuality.GetValueMin(out m_ImageCompressionQualityMin);
                        featureImageCompressionQuality.GetValueMax(out m_ImageCompressionQualityMax);

                        m_Initialized = true;
                        m_JpegAvailable = true;
                     }

                     featureImageCompressionQuality.Destroy();
                     featureImageCompressionQuality.Dispose();

                     if (m_JpegAvailable == true)
                        break;
                  }
               }
            }

            featureImageCompressionMode.Destroy();
            featureImageCompressionMode.Dispose();
         }
         else
         {
            m_Initialized = true;
         }

         // Identify current GigE pixel format to later know if it changes by enabling JPEG
         GetImagePixelFormat();

         return m_Initialized;
      }

      public bool Destroy()
      {
         if (m_Initialized == true)
         {
            m_Initialized = false;
            m_JpegAvailable = false;
         }

         if (m_AcqDevice.IsFeatureAvailable("InterPacketTimeout"))
            m_AcqDevice.SetFeatureValue("InterPacketTimeout", m_OldInterPacketTimeout);

         return true;
      }

      public bool SetImageCompressionQuality(UInt32 imageCompressionQuality)
      {
         if (imageCompressionQuality < m_ImageCompressionQualityMin)
            imageCompressionQuality = m_ImageCompressionQualityMin;

         if (imageCompressionQuality > m_ImageCompressionQualityMax)
            imageCompressionQuality = m_ImageCompressionQualityMax;

         if (m_AcqDevice.SetFeatureValue("ImageCompressionQuality", imageCompressionQuality))
         {
            m_ImageCompressionQuality = imageCompressionQuality;
            return true;
         }

         return false;
      }

      public UInt32 GetImageCompressionQuality()
      {
         uint imageCompressionQuality = 0;
         if (m_AcqDevice.GetFeatureValue("ImageCompressionQuality", out imageCompressionQuality))
            m_ImageCompressionQuality = imageCompressionQuality;

         return m_ImageCompressionQuality;
      }

      public UInt32 GetImageCompressionQualityMin()
      {
         return m_ImageCompressionQualityMin;
      }

      public UInt32 GetImageCompressionQualityMax()
      {
         return m_ImageCompressionQualityMax;
      }

      public bool GetImagePixelFormat()
      {
         uint pixelFormat = 0;
         if (m_AcqDevice.GetFeatureValue("PixelFormat", out pixelFormat))
         {
            if (pixelFormat != m_PixelFormat)
            {
               m_PixelFormat = pixelFormat;
               return true;
            }
         }

         return false;
      }

      public bool IsImageCompressionModeJpeg()
      {
         string featureString;
         if (m_JpegAvailable == true && m_AcqDevice.GetFeatureValue("ImageCompressionMode", out featureString))
            return featureString.Equals("JPEG", StringComparison.Ordinal);

         return false;
      }

      public bool IsLastImageJpegDecodable()
      {
         return (IsImageCompressionModeJpeg() && m_LastImageJpegDecodable);
      }

      public bool EnableJpegProcessing(bool enable, out bool rPixelFormatChanged)
      {
         bool retCode = m_AcqDevice.SetFeatureValue("ImageCompressionMode", enable == true ? m_ImageCompressionMode_EnumString_Jpeg : "Off");
         if (enable == true)
            GetImageCompressionQuality(); // Get image compression quality (in case it is updated by the camera).

         // Test if image pixel format changed (in case it is updated by the camera, like in color cameras where JPEG supports only color format)
         // We need only to know if it changed since we will use Sapera constructor with acquisition as a parameter to convert the GigE format to
         // Sapera format.
         rPixelFormatChanged = GetImagePixelFormat();

         return retCode;
      }

      public bool DecompressImageData(IntPtr encodedData, int encodedDataSize, IntPtr decodedData, int w, int h, int mw, int type)
      {
         // In the future, this call could be replaced by a new class implementation in Sapera LT.
         // In the meantime, here are the values to pass to the function:
         // IntPtr outData, int w, int h, int mw, int type, IntPtr iHeader, uint iHeaderSize, IntPtr iData, int iDataSize, IntPtr pcinfo
         // outData: pointer to the start of the data where to put the decoded image. In the case of an ROI, start of the ROI data.
         // w: destination image width or ROI width (not in bytes but in pixels).
         // h: destination image height or ROI height.
         // mw: destination image width (different from w only if an ROI is used in destination image).
         // type: IMAGE_FORMAT_UINT8 or IMAGE_FORMAT_RGB8888.
         // iHeader: Set to IntPtr.Zero. In the case that the JPEG header is separated from the rest, set it to the start of the header. With Genie TS: set to IntPtr.Zero. Could be removed in the future.
         // iHeaderSize: Set to 0 if iHeader is set to IntPtr.Zero. Could be removed in the future.
         // iData: pointer to the start of encoded JPEG data, including header (Genie TS always encode the JPEG header with the rest of the data).
         // iDataSize: size of the encoded JPEG data. Could be bigger than the encoded data (it won't all be read if the decoder finds the end of the JPEG image).
         //            If it is too small, it won't decode properly; be sure that the size does not overlap inaccessible memory.
         //            You can use the full size of the buffer if you don't want to read the SpaceUsed by the data for each frame. The only case when it could loose time is if
         //            the data is corrupted, so using SpaceUsed is optimal in all cases.
         // pcinfo: Set to IntPtr.Zero. Pointer to a JPEGDECOMPRINFO structure. You may have to put one for a possible future implementation.
         // return value: data decoded correctly if != 0.
         int status = readJPEGmem(decodedData, w, h, mw, type, IntPtr.Zero, 0, encodedData, encodedDataSize, IntPtr.Zero);
         bool bStatus = (status != 0);

         m_LastImageJpegDecodable = bStatus;

         return bStatus;
      }
   }
}
