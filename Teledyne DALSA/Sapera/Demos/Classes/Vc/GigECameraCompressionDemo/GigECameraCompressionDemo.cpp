// CameraCompressionDemo.cpp : Defines the class behaviors for the application.
//

#include "stdafx.h"
#include "GigECameraCompressionDemo.h"
#include "GigECameraCompressionDemoDlg.h"
#include "io.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CCameraCompressionDemoApp

BEGIN_MESSAGE_MAP(CCameraCompressionDemoApp, CWinApp)
   //{{AFX_MSG_MAP(CCameraCompressionDemoApp)
      // NOTE - the ClassWizard will add and remove mapping macros here.
      //    DO NOT EDIT what you see in these blocks of generated code!
   //}}AFX_MSG
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CCameraCompressionDemoApp construction

CCameraCompressionDemoApp::CCameraCompressionDemoApp()
{
   // TODO: add construction code here,
   // Place all significant initialization in InitInstance
}

/////////////////////////////////////////////////////////////////////////////
// The one and only CCameraCompressionDemoApp object

CCameraCompressionDemoApp theApp;

/////////////////////////////////////////////////////////////////////////////
// CCameraCompressionDemoApp initialization

BOOL CCameraCompressionDemoApp::InitInstance()
{
   AfxEnableControlContainer();

   // Standard initialization
   // If you are not using these features and wish to reduce the size
   //  of your final executable, you should remove from the following
   //  the specific initialization routines you do not need.

#if _MFC_VER < 0x0700
#ifdef _AFXDLL
   Enable3dControls();        // Call this when using MFC in a shared DLL
#else
   Enable3dControlsStatic();  // Call this when linking to MFC statically
#endif
#endif

   CDemoCommandLineInfo cmdInfo;
   ParseCommandLine(cmdInfo);

   SetRegistryKey(_T("Teledyne DALSA"));

   CCameraCompressionDemoDlg dlg;
   strcpy_s(dlg.m_TemperatureLogFilename, sizeof(dlg.m_TemperatureLogFilename), m_TemperatureLogFilename);
   m_pMainWnd = &dlg;
   INT_PTR nResponse = dlg.DoModal();
   if (nResponse == IDOK)
   {
      // TODO: Place code here to handle when the dialog is
      //  dismissed with OK
   }
   else if (nResponse == IDCANCEL)
   {
      // TODO: Place code here to handle when the dialog is
      //  dismissed with Cancel
   }

   // Since the dialog has been closed, return FALSE so that we exit the
   //  application, rather than start the application's message pump.
   return FALSE;
}

void CDemoCommandLineInfo::ParseParam(LPCTSTR lpszParam, BOOL bFlag, BOOL bLast)
{
   if (!bFlag && (m_TemperatureLogFilenameIncoming == TRUE))
   {
      m_TemperatureLogFilenameIncoming = FALSE;

      char chDrive[_MAX_DRIVE];
      char chDir[_MAX_DIR];
      char chFname[_MAX_FNAME];
      char chExt[_MAX_EXT];

      _splitpath_s(CStringA(lpszParam), chDrive, _MAX_DRIVE, chDir, _MAX_DIR, chFname, _MAX_FNAME, chExt, _MAX_EXT);

      // Initialize filename to NULL
      theApp.m_TemperatureLogFilename[0] = 0;

      // Put the drive letter if one is passed
      if (strnlen(chDrive, _MAX_DRIVE))
         strcat_s(theApp.m_TemperatureLogFilename, sizeof(theApp.m_TemperatureLogFilename), chDrive);

      // Put the directory if one is passed
      if (strnlen(chDir, _MAX_DIR))
         strcat_s(theApp.m_TemperatureLogFilename, sizeof(theApp.m_TemperatureLogFilename), chDir);

      // Put the passed filename or a default one
      strcat_s(theApp.m_TemperatureLogFilename, sizeof(theApp.m_TemperatureLogFilename), strnlen(chFname, _MAX_FNAME) ? chFname : "templog");

      // Put the passed extension or a default one
      strcat_s(theApp.m_TemperatureLogFilename, sizeof(theApp.m_TemperatureLogFilename), strnlen(chExt, _MAX_EXT) ? chExt : ".txt");
   }
   else if (bFlag && !_tcscmp (lpszParam, _T("templog")))
   {
      //next parameter is the log of the temperatures into a file
      m_TemperatureLogFilenameIncoming = TRUE;
   }

   if (bLast)
   {
      if (m_TemperatureLogFilenameIncoming)
      {
         // Nothing to come, put a default filename
         strcpy_s(theApp.m_TemperatureLogFilename, sizeof(theApp.m_TemperatureLogFilename), "templog.txt");
      }
   }
}
