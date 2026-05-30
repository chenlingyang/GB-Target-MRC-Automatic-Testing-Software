#ifndef _CMYPROCESSING_H_
#define _CMYPROCESSING_H_

// SapMyProcessing.h : header file
//
#include "stdafx.h"
#include "SapClassBasic.h"
#include "JpegProcessing.h"

//
// SapMyProcessing class declaration
//
class SapMyProcessing : public SapProcessing
{
public:
   // Constructor/Destructor
   SapMyProcessing(SapBuffer* pBuffer, SapBuffer* pBufferDecode, JpegProcessing* processing, SapProCallback pCallback, void* pContext);
   virtual ~SapMyProcessing();
   BOOL Create();

   void SetQuality(UINT32 imageCompressionQuality);
   void SetDecompressionEnabled(BOOL bDecompress);
   BOOL GetDecompressionEnabled() const { return m_bDecompress; }

   void InitProSpaceUsedStatistics();
   unsigned int GetProCurSpaceUsed() const { return m_ProCurSpaceUsed; }
   unsigned int GetProMinSpaceUsed() const { return m_ProMinSpaceUsed; }
   unsigned int GetProMaxSpaceUsed() const { return m_ProMaxSpaceUsed; }

   void InitProTimeStatistics();
   void UpdateProTimeStatistics(bool goodContent);
   float GetProTime() const { return m_ProTime; }
   float GetProTimeAvg() const { return m_ProTimeAvg; }
   int GetProFrameRate() const { return m_ProFrameRate; }

   void InitProImageStatistics();
   void UpdateProImageStatistics(bool goodContent);
   float GetProBandwidth() const { return m_ProBandwidth; }
   float GetProBandwidthAvg() const { return m_ProBandwidthAvg; }
   int GetProIncompleteFrameCount() const { return m_ProIncompleteFrameCount; }

   CString m_StringSkippedFramesCount;
   CString m_StringIncompleteFrameCount;

protected:
   unsigned int m_ProCurSpaceUsed;
   unsigned int m_ProMinSpaceUsed;
   unsigned int m_ProMaxSpaceUsed;
   float m_ProTime;
   float m_ProTimeAvg;
   unsigned int m_ProTimeAvgCount;
   double m_ProTimeAvgSum;
   int m_ProFrameRate;
   int m_ProImageStatisticsCount;
   UINT64 m_ProCurSpaceUsedSum;
   UINT64 m_baseDiffCounterAvg;
   unsigned int m_curMaxDiffCounterAvg;
   float m_ProBandwidth;
   float m_ProBandwidthAvg;
   int m_ProFirstImageCounterStamp;
   int m_ProPreviousImageCounterStamp;
   int m_ProIncompleteFrameCount;

   virtual BOOL Run();
   SapBuffer* m_pBuffersDecode;
   UINT32 m_ImageCompressionQuality;
   JpegProcessing* m_JpegProcessing;
   BOOL m_bDecompress;
};

#endif // _CMYPROCESSING_H_
