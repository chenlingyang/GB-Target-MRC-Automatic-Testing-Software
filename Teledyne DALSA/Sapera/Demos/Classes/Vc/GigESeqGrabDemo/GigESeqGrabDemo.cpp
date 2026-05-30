//==============================================================================
// Module : GigESeqGrabDemo.cpp
// Project: Sapera samples.
// Purpose: This module contains the methods that define the CGigESeqGrabDemoApp class.
//==============================================================================
// Include files.

#include "stdafx.h"
#include "GigESeqGrabDemo.h"
#include "GigESeqGrabDemoDlg.h"

#ifdef _DEBUG
   #define new DEBUG_NEW
   #undef THIS_FILE
   static char THIS_FILE[] = __FILE__;
#endif

//==============================================================================
// CGigESeqGrabDemoApp

BEGIN_MESSAGE_MAP(CGigESeqGrabDemoApp, CWinApp)
   //{{AFX_MSG_MAP(CGigESeqGrabDemoApp)
   //}}AFX_MSG
END_MESSAGE_MAP()

//==============================================================================
// Name      : CGigESeqGrabDemoApp::CGigESeqGrabDemoApp
// Purpose   : Class constructor.
// Parameters: None
//==============================================================================
CGigESeqGrabDemoApp::CGigESeqGrabDemoApp (void)
{
} // End of the CGigESeqGrabDemoApp::CGigESeqGrabDemoApp method.

//==============================================================================
// The one and only CGigESeqGrabDemoApp object

CGigESeqGrabDemoApp theApp;

//==============================================================================
// Name      : CGigESeqGrabDemoApp::InitInstance
// Purpose   : Initialization.
// Parameters: None
//==============================================================================
BOOL CGigESeqGrabDemoApp::InitInstance (void)
{
   // Standard initialization
#if _MFC_VER < 0x0700
   Enable3dControls ();
#endif
   AfxEnableControlContainer ();

   SetRegistryKey (_T("Teledyne DALSA"));

   CGigESeqGrabDemoDlg dlg;
   m_pMainWnd = &dlg;
   dlg.DoModal ();

   // Since the dialog has been closed, return FALSE so that we exit the
   //  application, rather than start the application's message pump.
   return FALSE;
} // End of the CGigESeqGrabDemoApp::InitInstance method.

//==============================================================================
