using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using DALSA.SaperaLT.SapClassBasic;

namespace DALSA.SaperaLT.Demos.NET.CSharp.CameraCompressionDemo
{
   class SapMyProcessing : SapProcessing
   {
      //private SapBayer m_Bayer;
      private uint m_ProCurSpaceUsed;
      private uint m_ProMinSpaceUsed;
      private uint m_ProMaxSpaceUsed;
      private float m_ProTime;
      private float m_ProTimeAvg;
      private uint m_ProTimeAvgCount;
      private double m_ProTimeAvgSum;
      private int m_ProFrameRate;
      private int m_ProImageStatisticsCount;
      private UInt64 m_ProCurSpaceUsedSum;
      private UInt64 m_baseDiffCounterAvg;
      private uint m_curMaxDiffCounterAvg;
      private float m_ProBandwidth;
      private float m_ProBandwidthAvg;
      private uint m_ProFirstImageCounterStamp;
      private uint m_ProPreviousImageCounterStamp;
      private int m_ProIncompleteFrameCount;

      private SapBuffer m_pBuffersDecode;
      private UInt32 m_ImageCompressionQuality;
      private JpegProcessing m_JpegProcessing;
      private bool m_bDecompress;


      // Constructor

      public SapMyProcessing(SapBuffer pBuffers, SapBuffer pBuffersDecode, JpegProcessing pJpegProcessing, SapProcessingDoneHandler pCallback, Object pContext)
        : base(pBuffers)
      {
         base.ProcessingDoneEnable = true;
         base.ProcessingDone += new SapProcessingDoneHandler(pCallback);
         base.ProcessingDoneContext = pContext;

         // Set up the externally allocated buffers for local access.
         base.Buffer = pBuffers;
         m_pBuffersDecode = pBuffersDecode;
         m_JpegProcessing = pJpegProcessing;
         m_ImageCompressionQuality = 84;
      }

      public new void Dispose()
      {
         if (Initialized == true)
            Destroy();

         base.Dispose();
      }

      public new bool Create()
      {
         return base.Create();
      }

      public void SetQuality(UInt32 imageCompressionQuality)
      {
         m_ImageCompressionQuality = imageCompressionQuality;
      }

      public void SetDecompressionEnabled(bool bDecompress)
      {
         m_bDecompress = bDecompress;
      }

      public bool GetDecompressionEnabled() { return m_bDecompress; }

      public void InitProSpaceUsedStatistics()
      {
         m_ProMinSpaceUsed = uint.MaxValue;
         m_ProMaxSpaceUsed = uint.MinValue;
      }

      public uint GetProCurSpaceUsed() { return m_ProCurSpaceUsed; }
      public uint GetProMinSpaceUsed() { return m_ProMinSpaceUsed; }
      public uint GetProMaxSpaceUsed() { return m_ProMaxSpaceUsed; }

      public void InitProTimeStatistics()
      {
         m_ProTimeAvgCount = 0;
         m_ProTimeAvgSum = 0.0;
      }

      public void UpdateProTimeStatistics(bool goodContent)
      {
         m_ProTime = base.Time;
         if (goodContent)
         {
            m_ProTimeAvgCount++;
            m_ProTimeAvgSum += m_ProTime;
            m_ProTimeAvg = (float)(m_ProTimeAvgSum / m_ProTimeAvgCount);
         }
      }

      public float GetProTime() { return m_ProTime; }
      public float GetProTimeAvg() { return m_ProTimeAvg; }
      public int GetProFrameRate() { return m_ProFrameRate; }

      public void InitProImageStatistics()
      {
         m_ProImageStatisticsCount = 0;
         m_ProCurSpaceUsedSum = 0;
         m_baseDiffCounterAvg = 0;
         m_curMaxDiffCounterAvg = 0;
         m_ProIncompleteFrameCount = 0;
         InitProSpaceUsedStatistics();
      }

      public void UpdateProImageStatistics(bool goodContent)
      {
         if (goodContent)
         {
            int proIndex = base.Index;

            uint imageCounterStamp = Buffer.get_CounterStamp(proIndex);
            if (m_ProImageStatisticsCount == 0)
            {
               m_ProFirstImageCounterStamp = imageCounterStamp;
               m_ProPreviousImageCounterStamp = imageCounterStamp;
            }

            m_ProImageStatisticsCount++;

            uint diffCounter = (uint)imageCounterStamp - (uint)m_ProPreviousImageCounterStamp;

            m_ProPreviousImageCounterStamp = imageCounterStamp;

            m_ProCurSpaceUsed = (uint)Buffer.get_SpaceUsed(proIndex);

            if (m_ProCurSpaceUsed < m_ProMinSpaceUsed)
               m_ProMinSpaceUsed = m_ProCurSpaceUsed;
            if (m_ProCurSpaceUsed > m_ProMaxSpaceUsed)
               m_ProMaxSpaceUsed = m_ProCurSpaceUsed;

            m_ProBandwidth = (diffCounter == 0) ? 0 : (float)m_ProCurSpaceUsed / ((float)diffCounter / 1000000);

            m_ProCurSpaceUsedSum += m_ProCurSpaceUsed;

            uint diffCounterAvg = (uint)imageCounterStamp - (uint)m_ProFirstImageCounterStamp;

            if (diffCounterAvg < m_curMaxDiffCounterAvg)
            {
               // After about 71 minutes, the 32-bit counter will wrap around so we have to add a 33rd bit to the divisor
               m_baseDiffCounterAvg += 0x100000000;
            }
            m_curMaxDiffCounterAvg = diffCounterAvg;

            if (diffCounter != 0)
            {
               m_ProBandwidthAvg = (float)m_ProCurSpaceUsedSum / ((float)(m_baseDiffCounterAvg + diffCounterAvg) / 1000000);
               m_ProFrameRate = (int)((float)1000000 / (float)diffCounter);
            }
         }
      }

      public float GetProBandwidth() { return m_ProBandwidth; }
      public float GetProBandwidthAvg() { return m_ProBandwidthAvg; }
      public int GetProIncompleteFrameCount() { return m_ProIncompleteFrameCount; }

      public override bool Run()
      {
         // StartTime() is not needed here because it is called just before starting Run() for each frame received.
         // You would need it only if some processing need not be timed.
         StartTime();

         // Get the next image buffer (contains variable length, compressed data).
         // Pay attention to not use m_pBuffers current index while in Run (or in any function you may call);
         // use index return by GetIndex() instead:
         // the current index of m_pBuffers can change during a Run if a new image is transfered before Run has ended.
         int proIndex = base.Index;

         bool goodContent = (Buffer.get_State(proIndex) == SapBuffer.DataState.Full);

         // The only possible buffer states that get here could be:
         // SapBuffer.DataState.Full or
         // SapBuffer.DataState.Full | SapBuffer.DataState.Overflow.
         // So we set goodContent if the buffer contains good data (not overflow).

         if (!goodContent)
            m_ProIncompleteFrameCount++;

         // If goodContent is false, the viewBuffer won't be updated so the last good image will be shown.

         if (GetDecompressionEnabled())
         {
            if (goodContent)
            {
               IntPtr inAddress;    // From received buffer object - compressed data
               IntPtr outAddress;   // From output buffer object - decompressed image data

               int w = m_pBuffersDecode.Width;
               int h = m_pBuffersDecode.Height;
               m_pBuffersDecode.GetAddress(out outAddress);

               // Get information on the the input compressed data (ptr and size).
               Buffer.GetAddress(proIndex, out inAddress);
               int inSize = Buffer.get_SpaceUsed(proIndex);

               SapFormat sapFormat = Buffer.Format;
               bool status;

               if (sapFormat == SapFormat.Mono8)
               {
                  // Decompress the data into the view buffer
                  status = m_JpegProcessing.DecompressImageData(inAddress, inSize, outAddress, w, h, w, JpegProcessing.IMAGE_FORMAT_UINT8);
               }
               else if (sapFormat == SapFormat.YUY2)
               {
                  // No need for a temporary buffer, but m_BuffersView must already be set to SapFormatRGB8888
                  status = m_JpegProcessing.DecompressImageData(inAddress, inSize, outAddress, w, h, w, JpegProcessing.IMAGE_FORMAT_RGB8888);
               }
               else
                  status = false;

               if (status == false)
               {
                  goodContent = false;

                  // We do not want to update the view buffer
               }
            }
         }
         else
         {
            if (goodContent)
               m_pBuffersDecode.Copy(Buffer, proIndex, 0);
         }

         // StopTime() needed here because we will read the time in here (we would not need
         // it if we'd read it in ProCallback() since StopTime() will be called after exiting Run()).
         StopTime();

         UpdateProTimeStatistics(goodContent);

         UpdateProImageStatistics(goodContent);

         // Next, ProCallback() will be called, unless another frame has already been
         // transfered in which case it will be Run() immediately.
         return true;
      }
   }
}
