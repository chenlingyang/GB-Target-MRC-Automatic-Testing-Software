// GigEFlatFieldDemo.h : main header file for the GIGEFLATFIELDDEMO application
//

#if !defined(AFX_GIGEFLATFIELDDEMO_H__E6DE8FEE_766E_49C3_90B2_69D0D8F42675__INCLUDED_)
#define AFX_GIGEFLATFIELDDEMO_H__E6DE8FEE_766E_49C3_90B2_69D0D8F42675__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
   #error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"      // main symbols

/////////////////////////////////////////////////////////////////////////////
// CGigEFlatFieldDemoApp:
// See GigEFlatFieldDemo.cpp for the implementation of this class
//

class CGigEFlatFieldDemoApp : public CWinApp
{
public:
   CGigEFlatFieldDemoApp();

// Overrides
   // ClassWizard generated virtual function overrides
   //{{AFX_VIRTUAL(CGigEFlatFieldDemoApp)
   public:
   virtual BOOL InitInstance();
   //}}AFX_VIRTUAL

// Implementation

   //{{AFX_MSG(CGigEFlatFieldDemoApp)
   //}}AFX_MSG
   DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_GIGEFLATFIELDDEMO_H__E6DE8FEE_766E_49C3_90B2_69D0D8F42675__INCLUDED_)
