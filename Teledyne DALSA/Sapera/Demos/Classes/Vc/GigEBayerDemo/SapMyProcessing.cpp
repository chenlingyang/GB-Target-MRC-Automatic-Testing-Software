//	SapMyProcessing.cpp : implementation file
//

#include "SapMyProcessing.h"


//
// Constructor/Destructor
//
SapMyProcessing::SapMyProcessing(SapBuffer *pBuffers, SapColorConversion* pConv, SapProCallback pCallback, void *pContext)
: SapProcessing(pBuffers, pCallback, pContext)
{
	m_Conv = pConv;
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
   if (m_Conv->IsEnabled() && m_Conv->IsSoftwareEnabled())
	{
      m_Conv->Convert(GetIndex());
	}

	return TRUE;
}

