//==============================================================================
// Module : GigESeqAcqDemo.h
// Project: Sapera demos.
// Purpose: Main header file for the GigESeqAcqDemo application.
//==============================================================================
// Include files.

#ifndef _GigESeqAcqDemoH
#define _GigESeqAcqDemoH

#if _MSC_VER >= 1000
   #pragma once
#endif // _MSC_VER >= 1000

#ifndef __AFXWIN_H__
   #error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"      // main symbols

//==============================================================================
// CGigESeqGrabDemoApp:

class CGigESeqGrabDemoApp : public CWinApp
{
   public:
      CGigESeqGrabDemoApp (void);

   // Overrides
   // ClassWizard generated virtual function overrides
   //{{AFX_VIRTUAL(CGigESeqGrabDemoApp)
   public:
      virtual BOOL InitInstance (void);
   //}}AFX_VIRTUAL

   // Implementation
   //{{AFX_MSG(CGigESeqGrabDemoApp)
   //}}AFX_MSG
   DECLARE_MESSAGE_MAP()
};

//==============================================================================
//{{AFX_INSERT_LOCATION}}
#endif
