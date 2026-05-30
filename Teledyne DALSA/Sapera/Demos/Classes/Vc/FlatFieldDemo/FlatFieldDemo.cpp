// FlatFieldDemo.cpp : Defines the class behaviors for the application.
//

#include "stdafx.h"
#include "FlatFieldDemo.h"
#include "FlatFieldDemoDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CFlatFieldDemoApp

BEGIN_MESSAGE_MAP(CFlatFieldDemoApp, CWinApp)
	//{{AFX_MSG_MAP(CFlatFieldDemoApp)
	//}}AFX_MSG
	ON_COMMAND(ID_HELP, CWinApp::OnHelp)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CFlatFieldDemoApp construction

CFlatFieldDemoApp::CFlatFieldDemoApp()
{
}

/////////////////////////////////////////////////////////////////////////////
// The one and only CFlatFieldDemoApp object

CFlatFieldDemoApp theApp;

/////////////////////////////////////////////////////////////////////////////
// CFlatFieldDemoApp initialization

BOOL CFlatFieldDemoApp::InitInstance()
{
	// Standard initialization

#if _MFC_VER < 0x0700
#ifdef _AFXDLL
	Enable3dControls();			// Call this when using MFC in a shared DLL
#else
	Enable3dControlsStatic();	// Call this when linking to MFC statically
#endif
#endif

	SetRegistryKey(_T("Teledyne DALSA"));

	CFlatFieldDemoDlg dlg;
	m_pMainWnd = &dlg;
   INT_PTR nResponse = dlg.DoModal();
	if (nResponse == IDOK)
	{
	}
	else if (nResponse == IDCANCEL)
	{
	}

	// Since the dialog has been closed, return FALSE so that we exit the
	//  application, rather than start the application's message pump.
	return FALSE;
}
