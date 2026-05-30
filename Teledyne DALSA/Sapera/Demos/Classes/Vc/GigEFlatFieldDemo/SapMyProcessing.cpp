// SapMyProcessing.cpp : implementation file
//

#include "SapMyProcessing.h"


//
// Constructor/Destructor
//
SapMyProcessing::SapMyProcessing(SapBuffer* pBuffers, SapFlatField *pFlatField, SapProCallback pCallback, void* pContext)
   : SapProcessing(pBuffers, pCallback, pContext)
{
   m_pFlatField = pFlatField;
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
   // Check if flat field correction is enabled and if it's to be done by software
   if (m_pFlatField->IsEnabled() && m_pFlatField->IsSoftware())
   {
      // Do software flat field correction
      m_pFlatField->Execute(m_pBuffers, GetIndex());
   }

   return TRUE;
}

