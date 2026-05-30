//	SapMyProcessing.cpp : implementation file
//

#include "SapMyProcessing.h"


//
// Constructor/Destructor
//
SapMyProcessing::SapMyProcessing(SapBuffer *pBuffers, SapBayer* pBayer, SapProCallback pCallback, void *pContext)
: SapProcessing(pBuffers, pCallback, pContext)
{
	m_Bayer= pBayer;
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
	if( m_Bayer->IsEnabled() && m_Bayer->IsSoftware())	
	{
		m_Bayer->Convert( GetIndex());
	}

	return TRUE;
}

