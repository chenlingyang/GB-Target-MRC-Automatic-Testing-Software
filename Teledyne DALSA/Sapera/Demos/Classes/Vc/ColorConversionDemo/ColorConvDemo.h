// ColorConvDemo.h : main header file for the COLOR CONVERSION DEMO application
//

#if !defined(AFX_COLORCONVDEMO_H__A8BE5972_F1B3_11D1_AF85_00A0C91AC0FB__INCLUDED_)
#define AFX_COLORCONVDEMO_H__A8BE5972_F1B3_11D1_AF85_00A0C91AC0FB__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#ifndef __AFXWIN_H__
#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

/////////////////////////////////////////////////////////////////////////////
// CColorConvDemoApp:
// See ColorConvDemo.cpp for the implementation of this class
//

class CColorConvDemoApp : public CWinApp
{
public:
   CColorConvDemoApp();

   // Overrides
   // ClassWizard generated virtual function overrides
   //{{AFX_VIRTUAL(CColorConvDemoApp)
public:
   virtual BOOL InitInstance();
   //}}AFX_VIRTUAL

   // Implementation

   //{{AFX_MSG(CColorConvDemoApp)
   // NOTE - the ClassWizard will add and remove member functions here.
   //    DO NOT EDIT what you see in these blocks of generated code !
   //}}AFX_MSG
   DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_COLORCONVDEMO_H__A8BE5972_F1B3_11D1_AF85_00A0C91AC0FB__INCLUDED_)
