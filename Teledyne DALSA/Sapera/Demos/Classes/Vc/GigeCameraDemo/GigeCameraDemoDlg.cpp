// GigeCameraDemoDlg.cpp : implementation file
//

#include "stdafx.h"
#include "GigeCameraDemo.h"
#include "GigeCameraDemoDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CAboutDlg dialog used for App About

class CAboutDlg : public CDialog
{
public:
	CAboutDlg();

// Dialog Data
	//{{AFX_DATA(CAboutDlg)
	enum { IDD = IDD_ABOUTBOX };
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CAboutDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	//{{AFX_MSG(CAboutDlg)
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
	//{{AFX_DATA_INIT(CAboutDlg)
	//}}AFX_DATA_INIT
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CAboutDlg)
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
	//{{AFX_MSG_MAP(CAboutDlg)
		// No message handlers
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CGigeCameraDemoDlg dialog

CGigeCameraDemoDlg::CGigeCameraDemoDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CGigeCameraDemoDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CGigeCameraDemoDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);

	m_AcqDevice			= NULL;
	m_Buffers			= NULL;
	m_Xfer				= NULL;
	m_View            = NULL;
}

void CGigeCameraDemoDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CGigeCameraDemoDlg)
	DDX_Control(pDX, IDC_STATUS, m_statusWnd);
   DDX_Control(pDX, IDC_VIEW_WND, m_ImageWnd);
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CGigeCameraDemoDlg, CDialog)
	//{{AFX_MSG_MAP(CGigeCameraDemoDlg)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_WM_DESTROY()
	ON_WM_SIZE()
	ON_BN_CLICKED(IDC_SNAP, OnSnap)
	ON_BN_CLICKED(IDC_GRAB, OnGrab)
	ON_BN_CLICKED(IDC_FREEZE, OnFreeze)
	ON_BN_CLICKED(IDC_LOAD_ACQ_CONFIG, OnLoadAcqConfig)
	ON_BN_CLICKED(IDC_BUFFER_OPTIONS, OnBufferOptions)
	ON_BN_CLICKED(IDC_VIEW_OPTIONS, OnViewOptions)
	ON_BN_CLICKED(IDC_FILE_LOAD, OnFileLoad)
	ON_BN_CLICKED(IDC_FILE_NEW, OnFileNew)
	ON_BN_CLICKED(IDC_FILE_SAVE, OnFileSave)
	ON_BN_CLICKED(IDC_EXIT, OnExit)
   ON_WM_ENDSESSION()
   ON_WM_QUERYENDSESSION()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CGigeCameraDemoDlg message handlers

void CGigeCameraDemoDlg::XferCallback(SapXferCallbackInfo *pInfo)
{
	CGigeCameraDemoDlg *pDlg= (CGigeCameraDemoDlg *) pInfo->GetContext();

   // If grabbing in trash buffer, do not display the image, update the
   // appropriate number of frames on the status bar instead
   if (pInfo->IsTrash())
   {
      CString str;
      str.Format(_T("Frames acquired in trash buffer: %d"), pInfo->GetEventCount());
      pDlg->m_statusWnd.SetWindowText(str);
   }

   // Refresh view
   else
   {
      pDlg->m_View->Show();
   }
}

void CGigeCameraDemoDlg::PixelChanged(int x, int y)
{
   CString str = m_appTitle;
   str += "  " + m_ImageWnd.GetPixelString(CPoint(x,y));
   SetWindowText(str);
}

//***********************************************************************************
// Initialize Demo Dialog based application
//***********************************************************************************
BOOL CGigeCameraDemoDlg::OnInitDialog()
{
	CRect rect;

	CDialog::OnInitDialog();
	
	// Add "About..." menu item to system menu.

	// IDM_ABOUTBOX must be in the system command range.
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		CString strAboutMenu;
		strAboutMenu.LoadString(IDS_ABOUTBOX);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}

		pSysMenu->EnableMenuItem(SC_MAXIMIZE, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
		pSysMenu->EnableMenuItem(SC_SIZE, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
	}

	// Set the icon for this dialog.  The framework does this automatically
	// when the application's main window is not a dialog
	SetIcon(m_hIcon, FALSE);	// Set small icon
	SetIcon(m_hIcon, TRUE);		// Set big icon
	
	// Initialize variables
	GetWindowText(m_appTitle);

   // Select acquisition server from dialog
   // Only the servers that support "AcqDevice" resource are listed
   CAcqConfigDlg dlg(this, CAcqConfigDlg::ServerAcqDevice);
	if (dlg.DoModal() != IDOK)
   {
      MessageBox(_T("No cameras found or selected"));
      EndDialog(TRUE);
      return FALSE;
   }

	// Define objects
   m_AcqDevice	= new SapAcqDevice(dlg.GetLocation(), dlg.GetConfigFile());
	m_Buffers	= new SapBufferWithTrash(2, m_AcqDevice);
	m_Xfer		= new SapAcqDeviceToBuf(m_AcqDevice, m_Buffers, XferCallback, this);
	m_View      = new SapView(m_Buffers);

   // Attach sapview to image viewer
   m_ImageWnd.AttachSapView(m_View);

	// Create all objects
	if (!CreateObjects()) { EndDialog(TRUE); return FALSE; }

   m_ImageWnd.AttachEventHandler(this);
   m_ImageWnd.CenterImage();
   m_ImageWnd.Reset();

	UpdateMenu();

	return TRUE;  // return TRUE  unless you set the focus to a control
}

BOOL CGigeCameraDemoDlg::CreateObjects()
{
	CWaitCursor wait;

	// Create acquisition object
	if (m_AcqDevice && !*m_AcqDevice && !m_AcqDevice->Create())
   {
      DestroyObjects();
      return FALSE;
   }

	// Create buffer object
	if (m_Buffers && !*m_Buffers)
	{
		if( !m_Buffers->Create())
		{
			DestroyObjects();
			return FALSE;
		}
		// Clear all buffers
		m_Buffers->Clear();
	}

	// Create view object
	if (m_View && !*m_View && !m_View->Create())
   {
      DestroyObjects();
      return FALSE;
   }

   // Set next empty with trash cycle mode for transfer
   if (m_Xfer && m_Xfer->GetPair(0))
   {
      if (!m_Xfer->GetPair(0)->SetCycleMode(SapXferPair::CycleNextWithTrash))
      {
         DestroyObjects();
         return FALSE;
      }
   }

	// Create transfer object
	if (m_Xfer && !*m_Xfer && !m_Xfer->Create())
   {
      DestroyObjects();
      return FALSE;
   }

	return TRUE;
}

BOOL CGigeCameraDemoDlg::DestroyObjects()
{
	// Destroy transfer object
	if (m_Xfer && *m_Xfer) m_Xfer->Destroy();

	// Destroy view object
	if (m_View && *m_View) m_View->Destroy();

	// Destroy buffer object
	if (m_Buffers && *m_Buffers) m_Buffers->Destroy();

	// Destroy acquisition object
	if (m_AcqDevice && *m_AcqDevice) m_AcqDevice->Destroy();

	return TRUE;
}

//**********************************************************************************
//
//				Window related functions
//
//**********************************************************************************
void CGigeCameraDemoDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if(( nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialog::OnSysCommand(nID, lParam);
	}
}


// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.
void CGigeCameraDemoDlg::OnPaint() 
{
	if( IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, (WPARAM) dc.GetSafeHdc(), 0);

		// Center icon in client rectangle
		INT32 cxIcon = GetSystemMetrics(SM_CXICON);
		INT32 cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		INT32 x = (rect.Width() - cxIcon + 1) / 2;
		INT32 y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialog::OnPaint();
	}
}

void CGigeCameraDemoDlg::OnDestroy() 
{
	CDialog::OnDestroy();

	// Destroy all objects
	DestroyObjects();

	// Delete all objects
   if (m_View)			delete m_View;
	if (m_Xfer)			delete m_Xfer;
	if (m_Buffers)		delete m_Buffers;
	if (m_AcqDevice)	delete m_AcqDevice;
}


void CGigeCameraDemoDlg::OnSize(UINT nType, int cx, int cy) 
{
   CDialog::OnSize(nType, cx, cy); 
   
   CRect rClient;
   GetClientRect(rClient);
   
   // resize image viewer
   if (m_ImageWnd.GetSafeHwnd())
   {
      CRect rWnd; 
      m_ImageWnd.GetWindowRect(rWnd);
      ScreenToClient(rWnd);
      rWnd.right = rClient.right - 5;
      rWnd.bottom = rClient.bottom - 5;
      m_ImageWnd.MoveWindow(rWnd);
   }
}


// The system calls this to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CGigeCameraDemoDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}


void CGigeCameraDemoDlg::OnExit() 
{
	EndDialog(TRUE);
}

void CGigeCameraDemoDlg::OnEndSession(BOOL bEnding)
{
   CDialog::OnEndSession(bEnding);

   if( bEnding)
   {
      // If ending the session, free the resources.
      OnDestroy(); 
   }
}

BOOL CGigeCameraDemoDlg::OnQueryEndSession()
{
   if (!CDialog::OnQueryEndSession())
      return FALSE;

   return TRUE;
}

//**************************************************************************************
// Updates the menu items enabling/disabling the proper items depending on the state
//  of the application
//**************************************************************************************
void CGigeCameraDemoDlg::UpdateMenu( void)
{
	BOOL bAcqNoGrab	= m_Xfer && *m_Xfer && !m_Xfer->IsGrabbing();
	BOOL bAcqGrab		= m_Xfer && *m_Xfer && m_Xfer->IsGrabbing();
	BOOL bNoGrab		= !m_Xfer || !m_Xfer->IsGrabbing();

	// Acquisition Control
	GetDlgItem(IDC_GRAB)->EnableWindow(bAcqNoGrab);
	GetDlgItem(IDC_SNAP)->EnableWindow(bAcqNoGrab);
	GetDlgItem(IDC_FREEZE)->EnableWindow(bAcqGrab);

	// Acquisition Options
	GetDlgItem(IDC_LOAD_ACQ_CONFIG)->EnableWindow(m_Xfer && !m_Xfer->IsGrabbing());

	// File Options
	GetDlgItem(IDC_FILE_NEW)->EnableWindow(bNoGrab);
	GetDlgItem(IDC_FILE_LOAD)->EnableWindow(bNoGrab);
	GetDlgItem(IDC_FILE_SAVE)->EnableWindow(bNoGrab);

	// General Options
	GetDlgItem(IDC_BUFFER_OPTIONS)->EnableWindow(bNoGrab);

	// If last control was disabled, set default focus
	if (!GetFocus())
		GetDlgItem(IDC_EXIT)->SetFocus();
}


//*****************************************************************************************
//
//					Acquisition Control
//
//*****************************************************************************************

void CGigeCameraDemoDlg::OnFreeze( ) 
{
	if( m_Xfer->Freeze())
	{
		if (CAbortDlg(this, m_Xfer).DoModal() != IDOK) 
			m_Xfer->Abort();

		UpdateMenu();
	}
}

void CGigeCameraDemoDlg::OnGrab() 
{
   m_statusWnd.SetWindowText(_T(""));

	if( m_Xfer->Grab())
	{
		UpdateMenu();	
	}
}

void CGigeCameraDemoDlg::OnSnap() 
{
   m_statusWnd.SetWindowText(_T(""));

	if( m_Xfer->Snap())
	{
		if (CAbortDlg(this, m_Xfer).DoModal() != IDOK) 
			m_Xfer->Abort();

		UpdateMenu();	
	}
}


//*****************************************************************************************
//
//					Acquisition Options
//
//*****************************************************************************************
void CGigeCameraDemoDlg::OnLoadAcqConfig() 
{
	// Set acquisition parameters
	CAcqConfigDlg dlg(this, CAcqConfigDlg::ServerAcqDevice);
	if (dlg.DoModal() == IDOK)
	{
		// Destroy objects
		DestroyObjects();

      // Backup
      SapLocation loc = m_AcqDevice->GetLocation();
      const char* configFile = m_AcqDevice->GetConfigFile();

		// Update object
      m_AcqDevice->SetLocation(dlg.GetLocation());
      m_AcqDevice->SetConfigFile(dlg.GetConfigFile());

		// Recreate objects
		if (!CreateObjects())
		{
         m_AcqDevice->SetLocation(loc);
         m_AcqDevice->SetConfigFile(configFile);
			CreateObjects();
		}

      m_ImageWnd.Reset();
		InvalidateRect(NULL);
		UpdateWindow();
		UpdateMenu();
	}
}

//*****************************************************************************************
//
//					General Options
//
//*****************************************************************************************

void CGigeCameraDemoDlg::OnBufferOptions() 
{
	CBufDlg dlg(this, m_Buffers, m_View->GetDisplay());
	if (dlg.DoModal() == IDOK)
	{
		// Destroy objects
		DestroyObjects();

		// Update buffer object
		SapBuffer buf = *m_Buffers;
		*m_Buffers = dlg.GetBuffer();

		// Recreate objects
		if (!CreateObjects())
		{
			*m_Buffers = buf;
			CreateObjects();
		}

		m_ImageWnd.Reset();
		InvalidateRect(NULL);
		UpdateWindow();
		UpdateMenu();
	}
}

void CGigeCameraDemoDlg::OnViewOptions() 
{
	CViewDlg dlg(this, m_View);
	if( dlg.DoModal() == IDOK)
      m_ImageWnd.Refresh();
}

//*****************************************************************************************
//
//					File Options
//
//*****************************************************************************************

void CGigeCameraDemoDlg::OnFileNew() 
{
	m_Buffers->Clear();
	InvalidateRect( NULL, FALSE);
}

void CGigeCameraDemoDlg::OnFileLoad() 
{
	CLoadSaveDlg dlg(this, m_Buffers, TRUE);
	if (dlg.DoModal() == IDOK)
	{
		InvalidateRect(NULL);
		UpdateWindow();
	}
}

void CGigeCameraDemoDlg::OnFileSave() 
{
	CLoadSaveDlg dlg(this, m_Buffers, FALSE);
	dlg.DoModal();
}

