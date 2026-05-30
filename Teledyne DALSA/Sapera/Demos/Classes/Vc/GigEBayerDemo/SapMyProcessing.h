#ifndef _SAPMYPROCESSING_H_
#define _SAPMYPROCESSING_H_

//	SapMyProcessing.h : header file
//

#include "SapClassBasic.h"

//
// SapMyProcessing class declaration
//
class SapMyProcessing : public SapProcessing
{
public:
	// Constructor/Destructor
	SapMyProcessing(SapBuffer *pBuffers, SapColorConversion* pConv, SapProCallback pCallback, void *pContext);
	virtual ~SapMyProcessing();


protected:
	virtual BOOL Run();

protected:
   SapColorConversion*	m_Conv;
}; 

#endif	// _SAPMYPROCESSING_H_

