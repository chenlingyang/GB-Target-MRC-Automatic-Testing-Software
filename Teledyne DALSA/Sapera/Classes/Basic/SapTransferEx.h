#ifndef _SAPTRANSFEREX_H_
#define _SAPTRANSFEREX_H_

//	SapTransferEx.h : header file
//

#include "SapClassBasicDef.h"

#define SAPCLASS_REF(type,a) (type&)((const type&)(a))

//
// SapAcqToBuf class declaration
//
class SAPCLASSBASIC_CLASS SapAcqToBuf : public SapTransfer
{
public:
   // Default Constructor
   SapAcqToBuf(SapAcquisition *pAcq, SapBuffer *pBuf, SapXferCallback pCallback = NULL, void *pContext = NULL)
      : SapTransfer(pCallback, pContext)
   {
      m_pAcquisition = pAcq;
      m_pBuf = pBuf;

      AddPair(SAPCLASS_REF(SapXferPair, SapXferPair(pAcq, pBuf)));
   }

   static BOOL UpdateFrameRateStatistics(SapTransfer *pXfer, SapAcquisition *pAcq, SapBuffer *pBuf);
   virtual BOOL UpdateFrameRateStatistics();

protected:
   SapAcquisition *m_pAcquisition;
   SapBuffer *m_pBuf;
};

#if COR_WIN32 && !COR_WIN64 && !defined(SAPERA_DOT_NET)
//
// SapAcqToBufThruPixPro class declaration
//
class SAPCLASSBASIC_CLASS SapAcqToBufThruPixPro : public SapTransfer
{
public:
   // Default Constructor
   SapAcqToBufThruPixPro(SapAcquisition *pAcq, SapPixPro *pPixPro, SapBuffer *pBuf, SapXferCallback pCallback = NULL, void *pContext = NULL)
      : SapTransfer(pCallback, pContext)
   {
      AddPair(SAPCLASS_REF(SapXferPair,SapXferPair(pAcq, pPixPro, FALSE)));
      AddPair(SAPCLASS_REF(SapXferPair,SapXferPair(pPixPro, pBuf)));
   }
};
#endif

//
// SapMultiAcqToBuf class declaration
//
class SAPCLASSBASIC_CLASS SapMultiAcqToBuf : public SapTransfer
{
public:
   // Default Constructor
   SapMultiAcqToBuf(SapAcquisition *pAcq[], SapBuffer *pBuf[], int numPairs, SapXferCallback pCallback = NULL, void *pContext = NULL)
      : SapTransfer(pCallback, pContext)
   {
      AddPair(SAPCLASS_REF(SapXferPair, SapXferPair(pAcq[0], pBuf[0])));
      for (int i = 1; i < numPairs; i++)
         AddPair(SAPCLASS_REF(SapXferPair, SapXferPair(pAcq[i], pBuf[i])));
   }
};


//
// SapAcqDeviceToBuf class declaration
//
class SAPCLASSBASIC_CLASS SapAcqDeviceToBuf : public SapTransfer
{
public:
   // Default Constructor
   SapAcqDeviceToBuf(SapAcqDevice *pAcqDevice, SapBuffer *pBuf, SapXferCallback pCallback = NULL, void *pContext = NULL)
      : SapTransfer(pCallback, pContext)
   {
      m_pAcqDevice = pAcqDevice;
      m_pBuf = pBuf;

      AddPair(SAPCLASS_REF(SapXferPair, SapXferPair(pAcqDevice, pBuf)));
      pBuf->SetBufferParamsNode(pAcqDevice);
      pAcqDevice->SetDestBuffer(pBuf);
   }

   static BOOL UpdateFrameRateStatistics(SapTransfer *pXfer, SapAcqDevice *pAcqDevice, SapBuffer *pBuf);
   virtual BOOL UpdateFrameRateStatistics();

protected:
   SapAcqDevice *m_pAcqDevice;
   SapBuffer *m_pBuf;
};

#endif	// _SAPTRANSFEREX_H_

