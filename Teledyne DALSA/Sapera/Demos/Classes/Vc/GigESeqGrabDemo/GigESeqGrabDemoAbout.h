//==============================================================================
// Module : GigESeqGrabDemoAbout.h
// Project: Sapera demos.
// Purpose: About box header file.
//==============================================================================
// Include files.

#ifndef _GigESeqGrabDemoAboutH
#define _GigESeqGrabDemoAboutH

#if _MSC_VER >= 1000
   #pragma once
#endif // _MSC_VER >= 1000

//==============================================================================
// CAboutDlg dialog used for App About

class CAboutDlg : public CDialog
{
   public:
	   CAboutDlg (void);

      // Dialog Data
	   //{{AFX_DATA(CAboutDlg)
	   enum { IDD = IDD_ABOUTBOX };
	   //}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CAboutDlg)
	protected:
	   virtual void DoDataExchange (CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

   // Implementation.
   protected:
	   //{{AFX_MSG(CAboutDlg)
	   //}}AFX_MSG
	   DECLARE_MESSAGE_MAP()
};
//==============================================================================

CAboutDlg::CAboutDlg (void) : CDialog (CAboutDlg::IDD)
{
	//{{AFX_DATA_INIT(CAboutDlg)
	//}}AFX_DATA_INIT
} // End of the CAboutDlg::CAboutDlg method.

void CAboutDlg::DoDataExchange (CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CAboutDlg)
	//}}AFX_DATA_MAP
} // End of the CAboutDlg::DoDataExchange method.

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
	//{{AFX_MSG_MAP(CAboutDlg)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

//==============================================================================
#endif
