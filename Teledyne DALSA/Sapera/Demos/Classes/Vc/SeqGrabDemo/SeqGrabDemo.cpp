//==============================================================================
// Module : SeqGrabDemo.cpp
// Project: Sapera samples.
// Purpose: This module contains the methods that define the CSeqGrabDemoApp class.
//==============================================================================
// Include files.

#include "stdafx.h"
#include "SeqGrabDemo.h"
#include "SeqGrabDemoDlg.h"

#ifdef _DEBUG
   #define new DEBUG_NEW
   #undef THIS_FILE
   static char THIS_FILE[] = __FILE__;
#endif

//==============================================================================
// CSeqGrabDemoApp

BEGIN_MESSAGE_MAP(CSeqGrabDemoApp, CWinApp)
   //{{AFX_MSG_MAP(CSeqGrabDemoApp)
   //}}AFX_MSG
END_MESSAGE_MAP()

//==============================================================================
// Name      : CSeqGrabDemoApp::CSeqGrabDemoApp
// Purpose   : Class constructor.
// Parameters: None
//==============================================================================
CSeqGrabDemoApp::CSeqGrabDemoApp (void)
{
} // End of the CSeqGrabDemoApp::CSeqGrabDemoApp method.

//==============================================================================
// The one and only CSeqGrabDemoApp object

CSeqGrabDemoApp theApp;

//==============================================================================
// Name      : CSeqGrabDemoApp::InitInstance
// Purpose   : Initialization.
// Parameters: None
//==============================================================================
BOOL CSeqGrabDemoApp::InitInstance (void)
{
   // Standard initialization
#if _MFC_VER < 0x0700
   Enable3dControls ();
#endif
   AfxEnableControlContainer ();

   SetRegistryKey (_T("Teledyne DALSA"));

   CSeqGrabDemoDlg dlg;
   m_pMainWnd = &dlg;
   dlg.DoModal ();

   // Since the dialog has been closed, return FALSE so that we exit the
   //  application, rather than start the application's message pump.
   return FALSE;
} // End of the CSeqGrabDemoApp::InitInstance method.

//==============================================================================
