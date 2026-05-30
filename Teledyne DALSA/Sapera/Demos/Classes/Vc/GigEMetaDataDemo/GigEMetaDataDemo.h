//==============================================================================
// Module : GigEMetaDataDemo.h
// Project: Sapera demos.
// Purpose: Main header file for the GigEMetaDataDemo application.
//==============================================================================
// Include files.

#ifndef _GigEMetaDataDemoH
#define _GigEMetaDataDemoH

#if _MSC_VER >= 1000
   #pragma once
#endif // _MSC_VER >= 1000

#ifndef __AFXWIN_H__
   #error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"      // main symbols

//==============================================================================
// CGigEMetaDataDemoApp:

class CGigEMetaDataDemoApp : public CWinApp
{
   public:
      CGigEMetaDataDemoApp(void);

   // Overrides
   // ClassWizard generated virtual function overrides
   //{{AFX_VIRTUAL(CGigEMetaDataDemoApp)
   public:
      virtual BOOL InitInstance(void);
   //}}AFX_VIRTUAL

   // Implementation
   //{{AFX_MSG(CGigEMetaDataDemoApp)
   //}}AFX_MSG
   DECLARE_MESSAGE_MAP()
};

//==============================================================================
//{{AFX_INSERT_LOCATION}}
#endif
