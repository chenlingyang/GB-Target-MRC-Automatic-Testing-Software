//	SapMyProcessing.cpp : implementation file
//

#include "SapMyProcessing.h"


//
// Constructor/Destructor
//
SapMyProcessing::SapMyProcessing(SapBuffer *pBuffers, SapColorConversion* pColorConv, SapProCallback pCallback, void *pContext)
   : SapProcessing(pBuffers, pCallback, pContext)
{
   m_ColorConv = pColorConv;
}

SapMyProcessing::~SapMyProcessing()
{
   if (m_bInitOK) 
      Destroy();
}


//
// Processing Control
//
BOOL SapMyProcessing::Run()
{
   if (m_ColorConv->IsSoftwareEnabled())
   {
      m_ColorConv->Convert( GetIndex());
   }

   return TRUE;
}

