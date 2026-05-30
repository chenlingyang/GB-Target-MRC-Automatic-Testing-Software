//==============================================================================
// Module : GigEMetaDataDemo.cpp
// Project: Sapera samples.
// Purpose: This module contains the methods that define the CGigEMetaDataDemoApp class.
//==============================================================================
// Include files.

#include "stdafx.h"
#include "GigEMetaDataDemo.h"
#include "GigEMetaDataDemoDlg.h"

#ifdef _DEBUG
   #define new DEBUG_NEW
   #undef THIS_FILE
   static char THIS_FILE[] = __FILE__;
#endif

//==============================================================================
// CGigEMetaDataDemoApp

BEGIN_MESSAGE_MAP(CGigEMetaDataDemoApp, CWinApp)
   //{{AFX_MSG_MAP(CGigEMetaDataDemoApp)
   //}}AFX_MSG
END_MESSAGE_MAP()

//==============================================================================
// Name      : CGigEMetaDataDemoApp::CGigEMetaDataDemoApp
// Purpose   : Class constructor.
// Parameters: None
//==============================================================================
CGigEMetaDataDemoApp::CGigEMetaDataDemoApp(void)
{
} // End of the CGigEMetaDataDemoApp::CGigEMetaDataDemoApp method.

//==============================================================================
// The one and only CGigEMetaDataDemoApp object

CGigEMetaDataDemoApp theApp;

//==============================================================================
// Name      : CGigEMetaDataDemoApp::InitInstance
// Purpose   : Initialization.
// Parameters: None
//==============================================================================
BOOL CGigEMetaDataDemoApp::InitInstance(void)
{
   // Standard initialization
#if _MFC_VER < 0x0700
   Enable3dControls ();
#endif
   AfxEnableControlContainer();

   SetRegistryKey(_T("Teledyne DALSA"));

   CGigEMetaDataDemoDlg dlg;
   m_pMainWnd = &dlg;
   dlg.DoModal();

   // Since the dialog has been closed, return FALSE so that we exit the
   //  application, rather than start the application's message pump.
   return FALSE;
} // End of the CGigEMetaDataDemoApp::InitInstance method.

//==============================================================================
