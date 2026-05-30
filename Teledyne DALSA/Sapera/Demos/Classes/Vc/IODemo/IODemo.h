// IODemo.h : main header file for the IODEMO application
//

#if !defined(AFX_IODEMO_H__DDEF1779_08BF_4668_9CFE_697B23D7F95E__INCLUDED_)
#define AFX_IODEMO_H__DDEF1779_08BF_4668_9CFE_697B23D7F95E__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

/////////////////////////////////////////////////////////////////////////////
// CIODemoApp:
// See IODemo.cpp for the implementation of this class
//

class CIODemoApp : public CWinApp
{
public:
	CIODemoApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CIODemoApp)
	public:
	virtual BOOL InitInstance();
	//}}AFX_VIRTUAL

// Implementation

	//{{AFX_MSG(CIODemoApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_IODEMO_H__DDEF1779_08BF_4668_9CFE_697B23D7F95E__INCLUDED_)
