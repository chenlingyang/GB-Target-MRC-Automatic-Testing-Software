// FlatFieldDemo.h : main header file for the FLATFIELDDEMO application
//

#if !defined(AFX_FLATFIELDDEMO_H__E6DE8FEE_766E_49C3_90B2_69D0D8F42675__INCLUDED_)
#define AFX_FLATFIELDDEMO_H__E6DE8FEE_766E_49C3_90B2_69D0D8F42675__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

/////////////////////////////////////////////////////////////////////////////
// CFlatFieldDemoApp:
// See FlatFieldDemo.cpp for the implementation of this class
//

class CFlatFieldDemoApp : public CWinApp
{
public:
	CFlatFieldDemoApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CFlatFieldDemoApp)
	public:
	virtual BOOL InitInstance();
	//}}AFX_VIRTUAL

// Implementation

	//{{AFX_MSG(CFlatFieldDemoApp)
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_FLATFIELDDEMO_H__E6DE8FEE_766E_49C3_90B2_69D0D8F42675__INCLUDED_)
