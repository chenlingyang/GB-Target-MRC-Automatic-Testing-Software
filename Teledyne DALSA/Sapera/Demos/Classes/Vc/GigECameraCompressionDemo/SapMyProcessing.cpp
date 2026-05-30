// SapMyProcessing.cpp : implementation file
//

#include "SapMyProcessing.h"

//
// Constructor/Destructor
//
SapMyProcessing::SapMyProcessing(SapBuffer* pBuffers, SapBuffer* pBufferDecode, JpegProcessing* processing, SapProCallback pCallback, void* pContext)
: SapProcessing(pBuffers, pCallback, pContext)
{
   // Set up the externally allocated buffers for local access.
   m_pBuffers = pBuffers;
   m_pBuffersDecode = pBufferDecode;
   m_JpegProcessing = (JpegProcessing *)processing;
   m_ImageCompressionQuality = 84;
}

SapMyProcessing::~SapMyProcessing()
{
   if (m_bInitOK)
      Destroy();
}


BOOL SapMyProcessing::Create()
{
   return SapProcessing::Create();
}

void SapMyProcessing::SetQuality(UINT32 imageCompressionQuality)
{
   m_ImageCompressionQuality = imageCompressionQuality;
}

void SapMyProcessing::SetDecompressionEnabled(BOOL bDecompress)
{
   m_bDecompress = bDecompress;
}

//
// Processing Control
//
BOOL SapMyProcessing::Run()
{
   // StartTime() is not needed here because it is called just before starting Run() for each frame received.
   // You would need it only if some processing need not be timed.
   StartTime();

   // Get the next image buffer (contains variable length, compressed data).
   // Pay attention to not use m_pBuffers current index while in Run (or in any function you may call);
   // use index return by GetIndex() instead:
   // the current index of m_pBuffers can change during a Run if a new image is transfered before Run has ended.
   int proIndex = GetIndex();

   SapBuffer::State state;
   bool goodContent = true;
   if (m_pBuffers->GetState(proIndex, &state) == FALSE)
      goodContent = false;
   else if (state != SapBuffer::StateFull)
      goodContent = false;

   // The only possible buffer states that get here could be:
   // SapBuffer::StateFull or
   // SapBuffer::StateFull | SapBuffer::StateOverflow.
   // So we set goodContent if the buffer contains good data (not overflow).

   if (!goodContent)
      m_ProIncompleteFrameCount++;

   // If goodContent is false, the viewBuffer won't be updated so the last good image will be shown.

   if (GetDecompressionEnabled())
   {
      if (goodContent)
      {
         void* inAddress = NULL;    // From received buffer object - compressed data
         void* outAddress = NULL;   // From output buffer object - decompressed image data

         int w = m_pBuffersDecode->GetWidth();
         int h = m_pBuffersDecode->GetHeight();
         m_pBuffersDecode->GetAddress(&outAddress);

         // Get information on the input compressed data (ptr and size).
         m_pBuffers->GetAddress(proIndex, &inAddress);
         int inSize = 0;
         m_pBuffers->GetSpaceUsed(proIndex, &inSize);

         SapFormat sapFormat = m_pBuffers->GetFormat();
         int status;

         if (sapFormat == SapFormatMono8)
         {
            // Decompress the data into the view buffer
            status = m_JpegProcessing->DecompressImageData(inAddress, inSize, outAddress, w, h, w, IMAGE_FORMAT_UINT8);
         }
         else if (sapFormat == SapFormatYUY2)
         {
            // No need for a temporary buffer, but m_BuffersView must already be set to SapFormatRGB8888
            status = m_JpegProcessing->DecompressImageData(inAddress, inSize, outAddress, w, h, w, IMAGE_FORMAT_RGB8888);
         }
         else
            status = FALSE;

         if (status == FALSE)
         {
            goodContent = false;

            // We do not want to update the view buffer
         }
      }
   }
   else // !GetDecompressionEnabled()
   {
      if (goodContent)
         m_pBuffersDecode->Copy(m_pBuffers, proIndex, 0);
   }

   // StopTime() needed here because we will read the time in here (we would not need
   // it if we'd read it in ProCallback() since StopTime() will be called after exiting Run()).
   StopTime();

   UpdateProTimeStatistics(goodContent);

   UpdateProImageStatistics(goodContent);

   // Next, ProCallback() will be called, unless another frame has already been
   // transfered in which case it will be Run() immediately.
   return TRUE;
}

void SapMyProcessing::InitProTimeStatistics()
{
   m_ProTimeAvgCount = 0;
   m_ProTimeAvgSum = 0.0;
}

void SapMyProcessing::UpdateProTimeStatistics(bool goodContent)
{
   m_ProTime = GetTime();
   if (goodContent)
   {
      m_ProTimeAvgCount++;
      m_ProTimeAvgSum += m_ProTime;
      m_ProTimeAvg = (float)(m_ProTimeAvgSum / m_ProTimeAvgCount);
   }
}

void SapMyProcessing::InitProSpaceUsedStatistics()
{
   m_ProMinSpaceUsed = (unsigned int)-1;
   m_ProMaxSpaceUsed = 0;
}

void SapMyProcessing::InitProImageStatistics()
{
   m_ProImageStatisticsCount = 0;
   m_ProCurSpaceUsed = 0;
   m_ProCurSpaceUsedSum = 0;
   m_baseDiffCounterAvg = 0;
   m_curMaxDiffCounterAvg = 0;
   m_ProIncompleteFrameCount = 0;
   m_StringSkippedFramesCount.Empty();
   m_StringIncompleteFrameCount.Empty();
   InitProSpaceUsedStatistics();
}

void SapMyProcessing::UpdateProImageStatistics(bool goodContent)
{
   if (goodContent)
   {
      int proIndex = GetIndex();

      int imageCounterStamp;
      m_pBuffers->GetCounterStamp(proIndex, &imageCounterStamp);

      if (!m_ProImageStatisticsCount)
      {
         m_ProFirstImageCounterStamp = imageCounterStamp;
         m_ProPreviousImageCounterStamp = imageCounterStamp;
      }

      m_ProImageStatisticsCount++;

      unsigned int diffCounter = (unsigned int)imageCounterStamp - (unsigned int)m_ProPreviousImageCounterStamp;

      m_ProPreviousImageCounterStamp = imageCounterStamp;

      m_pBuffers->GetSpaceUsed(proIndex, (int*)&m_ProCurSpaceUsed);

      if (m_ProCurSpaceUsed < m_ProMinSpaceUsed)
         m_ProMinSpaceUsed = m_ProCurSpaceUsed;
      if (m_ProCurSpaceUsed > m_ProMaxSpaceUsed)
         m_ProMaxSpaceUsed = m_ProCurSpaceUsed;

      m_ProBandwidth = (diffCounter == 0) ? 0 : (float)m_ProCurSpaceUsed / ((float)diffCounter / 1000000);

      m_ProCurSpaceUsedSum += m_ProCurSpaceUsed;

      unsigned int diffCounterAvg = (unsigned int)imageCounterStamp - (unsigned int)m_ProFirstImageCounterStamp;

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
