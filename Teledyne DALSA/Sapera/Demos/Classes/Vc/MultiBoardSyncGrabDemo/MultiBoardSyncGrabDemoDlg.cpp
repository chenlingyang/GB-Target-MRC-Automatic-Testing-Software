// MultiBoardSyncGrabDemoDlg.cpp : implementation file
//

#include "stdafx.h"
#include "MultiBoardSyncGrabDemo.h"
#include "MultiBoardSyncGrabDemoDlg.h"

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
// CMultiBoardSyncGrabDemoDlg dialog

CMultiBoardSyncGrabDemoDlg::CMultiBoardSyncGrabDemoDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CMultiBoardSyncGrabDemoDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CMultiBoardSyncGrabDemoDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);

	m_ImageWnd			= NULL;
	m_Acq[0]				= NULL;
   m_Acq[1]          = NULL;
	m_Buffers[0]	   = NULL;
   m_Buffers[1]	   = NULL;
	m_Xfer[0]	      = NULL;
   m_Xfer[1]          = NULL;
	m_View            = NULL;

   m_IsSignalDetected = TRUE;
}

void CMultiBoardSyncGrabDemoDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CMultiBoardSyncGrabDemoDlg)
	DDX_Control(pDX, IDC_STATUS, m_statusWnd);
	DDX_Control(pDX, IDC_VERT_SCROLLBAR, m_verticalScr);
	DDX_Control(pDX, IDC_HORZ_SCROLLBAR, m_horizontalScr);
	DDX_Control(pDX, IDC_VIEW_WND, m_viewWnd);
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CMultiBoardSyncGrabDemoDlg, CDialog)
	//{{AFX_MSG_MAP(CMultiBoardSyncGrabDemoDlg)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_WM_DESTROY()
	ON_WM_MOUSEMOVE()
	ON_WM_HSCROLL()
	ON_WM_VSCROLL()
	ON_WM_MOVE()
	ON_WM_SIZE()
	ON_BN_CLICKED(IDC_SNAP, OnSnap)
	ON_BN_CLICKED(IDC_GRAB, OnGrab)
	ON_BN_CLICKED(IDC_FREEZE, OnFreeze)
	ON_BN_CLICKED(IDC_VIEW_OPTIONS, OnViewOptions)
	ON_BN_CLICKED(IDC_FILE_NEW, OnFileNew)
	ON_BN_CLICKED(IDC_EXIT, OnExit)
   ON_WM_ENDSESSION()
   ON_WM_QUERYENDSESSION()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CMultiBoardSyncGrabDemoDlg message handlers

void CMultiBoardSyncGrabDemoDlg::XferCallback(SapXferCallbackInfo *pInfo)
{
	CMultiBoardSyncGrabDemoDlg *pDlg= (CMultiBoardSyncGrabDemoDlg *) pInfo->GetContext();

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
      pDlg->UpdateTitleBar();
   }
}

void CMultiBoardSyncGrabDemoDlg::SignalCallback(SapAcqCallbackInfo *pInfo)
{
	CMultiBoardSyncGrabDemoDlg *pDlg = (CMultiBoardSyncGrabDemoDlg *) pInfo->GetContext();
   pDlg->GetSignalStatus(pInfo->GetSignalStatus());
}

//***********************************************************************************
// Initialize Demo Dialog based application
//***********************************************************************************
BOOL CMultiBoardSyncGrabDemoDlg::OnInitDialog()
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
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, FALSE);	// Set small icon
	SetIcon(m_hIcon, TRUE);		// Set big icon
	
	// Initialize variables
	GetWindowText(m_appTitle);

   m_Buffer	      = new SapBufferWithTrash();

	// Are we operating on-line?
	CAcqConfigDlg dlg(this, NULL);
	if (dlg.DoModal() == IDOK)
	{
		// Define on-line objects
		m_Acq[0]			= new SapAcquisition(dlg.GetAcquisition());
      m_Buffers[0]	= new MySapBufferRoi(m_Buffer);
      m_Xfer[0]      = new SapAcqToBuf(m_Acq[0], m_Buffers[0], XferCallback, this);
      // Create acquisition object
	   if (m_Acq[0] && !*m_Acq[0] && !m_Acq[0]->Create())
      {
         DestroyObjects();
         EndDialog(TRUE);
         return FALSE;
      }
	}
   else
   {
      CString message;
      message.Format(_T("No board found or selected !"));
      MessageBox(message,_T("No Board"),MB_ICONEXCLAMATION);
      EndDialog(TRUE);
      return FALSE;
   }

   // Are we operating on-line?
	CAcqConfigDlg dlg2(this, NULL);
	if (dlg2.DoModal() == IDOK)
	{
		// Define on-line objects
		m_Acq[1]			= new SapAcquisition(dlg2.GetAcquisition());
      m_Buffers[1]	= new MySapBufferRoi(m_Buffer);
      m_Xfer[1]      = new SapAcqToBuf(m_Acq[1], m_Buffers[1], XferCallback, this);

      // Create acquisition object
	   if (m_Acq[1] && !*m_Acq[1] && !m_Acq[1]->Create())
      {
         DestroyObjects();
         EndDialog(TRUE);
         return FALSE;
      }
	}
   else
   {
      CString message;
      message.Format(_T("No board found or selected !"));
      MessageBox(message,_T("No Board"),MB_ICONEXCLAMATION);
      EndDialog(TRUE);
      return FALSE;
   }


   //check to see if both acquision devices support scatter gather.
   BOOL acq0SupportSG = SapBuffer::IsBufferTypeSupported(m_Acq[0]->GetLocation(),SapBuffer::TypeScatterGather);
   BOOL acq1SupportSG = SapBuffer::IsBufferTypeSupported(m_Acq[1]->GetLocation(),SapBuffer::TypeScatterGather);
   

   if (!acq0SupportSG || !acq1SupportSG)
   {
      // check if they support scatter gather physical
      BOOL acq0SupportSGP = SapBuffer::IsBufferTypeSupported(m_Acq[0]->GetLocation(),SapBuffer::TypeScatterGatherPhysical);
      BOOL acq1SupportSGP = SapBuffer::IsBufferTypeSupported(m_Acq[1]->GetLocation(),SapBuffer::TypeScatterGatherPhysical);

      if (!(!acq0SupportSG && !acq1SupportSG && acq0SupportSGP && acq1SupportSGP))
      {
         CString message;
         message.Format(_T("The chosen acquisition devices\n\n-%s\n-%s\n\ndo not support similar buffer types.\nThe demo will now close."),
                        (LPCTSTR)CString(m_Acq[0]->GetLocation().GetServerName()),
                        (LPCTSTR)CString(m_Acq[1]->GetLocation().GetServerName()));
         MessageBox(message,_T("Buffer Type Error"),MB_ICONEXCLAMATION);
         EndDialog(TRUE);
         return 0;
      }
   }


   
	// Define other objects
	m_View = new SapView( m_Buffer, m_viewWnd.GetSafeHwnd());

	// Create all objects
	if (!CreateObjects()) 
   { 
      EndDialog(TRUE); 
      return FALSE; 
   }

   m_View->SetScalingMode( SapView::ScalingNone, TRUE);

	// Create an image window object
	m_ImageWnd = new CImageWnd(m_View, &m_viewWnd, &m_horizontalScr, &m_verticalScr, this);
	UpdateMenu();

   // Get current input signal connection status
   GetSignalStatus();

	return TRUE;  // return TRUE  unless you set the focus to a control
}

BOOL CMultiBoardSyncGrabDemoDlg::CreateObjects()
{
	CWaitCursor wait;

	
   

	m_bufferWidth  = m_Acq[0]->GetXferParams().GetWidth();
   m_bufferHeight = m_Acq[0]->GetXferParams().GetHeight();
   m_bufferFormat = m_Acq[0]->GetXferParams().GetFormat();

   // Create buffer object
   if (SapBuffer::IsBufferTypeSupported(m_Acq[0]->GetLocation(),SapBuffer::TypeScatterGather))
      m_Buffer->SetParameters(1, 2 * m_bufferWidth, m_bufferHeight, m_bufferFormat, SapBuffer::TypeScatterGather);
   else if (SapBuffer::IsBufferTypeSupported(m_Acq[0]->GetLocation(),SapBuffer::TypeScatterGatherPhysical))
      m_Buffer->SetParameters(1, 2 * m_bufferWidth, m_bufferHeight, m_bufferFormat, SapBuffer::TypeScatterGatherPhysical);
   
   m_Buffer->SetPixelDepth( m_Acq[ 0]->GetXferParams().GetPixelDepth());

   if (m_Buffer && !*m_Buffer && !m_Buffer->Create()) 
   {
      DestroyObjects();
      return FALSE;
   }

   // Create buffer object
   m_Buffers[0]->SetRoi( 0, 0, m_bufferWidth, m_bufferHeight);
   if (m_Buffers[0] && !*m_Buffers[0] && !m_Buffers[0]->Create()) 
   {
      DestroyObjects();
      return FALSE;
   }
   
   m_Buffers[1]->SetRoi( m_bufferWidth, 0, m_bufferWidth, m_bufferHeight);
   if (m_Buffers[1] && !*m_Buffers[1] && !m_Buffers[1]->Create()) 
   {
      DestroyObjects();
      return FALSE;
   }
   
   // Create view object
   if (m_View && !*m_View && !m_View->Create()) 
   {
      DestroyObjects();
      return FALSE;
   }

   // Create transfer object
   if (m_Xfer[0] && !*m_Xfer[0] && !m_Xfer[0]->Create()) 
   {
      DestroyObjects();
      return FALSE;
   }

    if (m_Xfer[1] && !*m_Xfer[1] && !m_Xfer[1]->Create()) 
   {
      DestroyObjects();
      return FALSE;
   }


   return TRUE;
}

BOOL CMultiBoardSyncGrabDemoDlg::DestroyObjects()
{
	BOOL bResult= TRUE;

   if (m_Xfer[1] && *m_Xfer[1])
      bResult&= m_Xfer[1]->Destroy();

   if (m_Xfer[0] && *m_Xfer[0])
      bResult&= m_Xfer[0]->Destroy();

   // Destroy view object
   if (m_View && *m_View)
      bResult&= m_View->Destroy();

   // Destroy buffer object
   if (m_Buffers[0] && *m_Buffers[0])
      bResult&= m_Buffers[0]->Destroy();

   if (m_Buffers[1] && *m_Buffers[1])
      bResult&= m_Buffers[1]->Destroy();

   if (m_Buffer && *m_Buffer )
      bResult&=  m_Buffer->Destroy();

   // Destroy acquisition object
   if (m_Acq[0] && *m_Acq[0] )
      bResult&= m_Acq[0]->Destroy();

   if (m_Acq[1] && *m_Acq[1] )
      bResult&= m_Acq[1]->Destroy();

   return bResult;
}

//**********************************************************************************
//
//				Window related functions
//
//**********************************************************************************
void CMultiBoardSyncGrabDemoDlg::OnSysCommand(UINT nID, LPARAM lParam)
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
void CMultiBoardSyncGrabDemoDlg::OnPaint() 
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

		// Display last acquired image
      m_ImageWnd->OnPaint();
	}
}

void CMultiBoardSyncGrabDemoDlg::OnDestroy() 
{
	CDialog::OnDestroy();

	// Destroy all objects
	DestroyObjects();

	// Delete all objects
	if (m_ImageWnd)   delete m_ImageWnd;
   if (m_View)       delete m_View; 
   if (m_Xfer[0])    delete m_Xfer[0]; 
   if (m_Xfer[1])    delete m_Xfer[1];
   if (m_Buffers[0]) delete m_Buffers[0]; 
   if (m_Buffers[1]) delete m_Buffers[1]; 
   if (m_Buffer)	   delete m_Buffer; 
   if (m_Acq[0])		delete m_Acq[0]; 
   if (m_Acq[1])		delete m_Acq[1]; 
}

void CMultiBoardSyncGrabDemoDlg::OnMouseMove(UINT nFlags, CPoint point) 
{
   if (m_IsSignalDetected)
   {
	   CString str = m_appTitle;

	   if (!nFlags) 
		   str += "  " + m_ImageWnd->GetPixelString(point);

	   SetWindowText(str);
   }

	CDialog::OnMouseMove(nFlags, point);
}

void CMultiBoardSyncGrabDemoDlg::OnMove(int x, int y) 
{
	CDialog::OnMove(x, y);

	if (m_ImageWnd) m_ImageWnd->OnMove();
}


void CMultiBoardSyncGrabDemoDlg::OnSize(UINT nType, int cx, int cy) 
{
	CDialog::OnSize(nType, cx, cy);

	if (m_ImageWnd) m_ImageWnd->OnSize();
}


void CMultiBoardSyncGrabDemoDlg::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar) 
{
	if (pScrollBar->GetDlgCtrlID() == IDC_HORZ_SCROLLBAR)
	{
		// Adjust source's horizontal origin
		m_ImageWnd->OnHScroll(nSBCode, nPos);
		OnPaint();
	}

	CDialog::OnHScroll(nSBCode, nPos, pScrollBar);
}

void CMultiBoardSyncGrabDemoDlg::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar) 
{
	if (pScrollBar->GetDlgCtrlID() == IDC_VERT_SCROLLBAR)
	{
		// Adjust source's vertical origin
		m_ImageWnd->OnVScroll(nSBCode, nPos);
		OnPaint();
	}

	CDialog::OnVScroll(nSBCode, nPos, pScrollBar);
}


// The system calls this to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CMultiBoardSyncGrabDemoDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}


void CMultiBoardSyncGrabDemoDlg::OnExit() 
{
	EndDialog(TRUE);
}

void CMultiBoardSyncGrabDemoDlg::OnEndSession(BOOL bEnding)
{
   CDialog::OnEndSession(bEnding);

   if( bEnding)
   {
      // If ending the session, free the resources.
      OnDestroy(); 
   }
}

BOOL CMultiBoardSyncGrabDemoDlg::OnQueryEndSession()
{
   if (!CDialog::OnQueryEndSession())
      return FALSE;

   return TRUE;
}

//**************************************************************************************
// Updates the menu items enabling/disabling the proper items depending on the state
//  of the application
//**************************************************************************************
void CMultiBoardSyncGrabDemoDlg::UpdateMenu( void)
{
	BOOL bAcqNoGrab	= m_Xfer[0] && *m_Xfer[0] && !m_Xfer[0]->IsGrabbing();
	BOOL bAcqGrab		= m_Xfer[0] && *m_Xfer[0] && m_Xfer[0]->IsGrabbing();

	// Acquisition Control
	GetDlgItem(IDC_GRAB)->EnableWindow(bAcqNoGrab);
	GetDlgItem(IDC_SNAP)->EnableWindow(bAcqNoGrab);
	GetDlgItem(IDC_FREEZE)->EnableWindow(bAcqGrab);

	
	

	// File Options
	GetDlgItem(IDC_FILE_NEW)->EnableWindow(bAcqNoGrab);
	
	// If last control was disabled, set default focus
	if (!GetFocus())
		GetDlgItem(IDC_EXIT)->SetFocus();
}


void CMultiBoardSyncGrabDemoDlg::UpdateTitleBar()
{
   // Update pixel information
   CString str = m_appTitle;
   CPoint point;
   GetCursorPos(&point);
   ScreenToClient(&point);
   str += "  " + m_ImageWnd->GetPixelString(point);

   SetWindowText(str);
}


//*****************************************************************************************
//
//					Acquisition Control
//
//*****************************************************************************************

void CMultiBoardSyncGrabDemoDlg::OnFreeze( ) 
{
	if( m_Xfer[0]->Freeze() && m_Xfer[1]->Freeze())
	{
		if (CAbortDlg(this, m_Xfer[0]).DoModal() != IDOK) 
			m_Xfer[0]->Abort();
      if (CAbortDlg(this, m_Xfer[1]).DoModal() != IDOK) 
			m_Xfer[1]->Abort();

		UpdateMenu();
	}
}

void CMultiBoardSyncGrabDemoDlg::OnGrab() 
{
   m_statusWnd.SetWindowText(_T(""));

	if( m_Xfer[0]->Grab() && m_Xfer[1]->Grab())
	{
		UpdateMenu();	
	}
}

void CMultiBoardSyncGrabDemoDlg::OnSnap() 
{
   
   m_statusWnd.SetWindowText(_T(""));
   if (m_Xfer[0]->Snap() && m_Xfer[1]->Snap())
   {
      if (CAbortDlg(this, m_Xfer[0]).DoModal() != IDOK) 
      m_Xfer[0]->Abort();

      if (CAbortDlg(this, m_Xfer[1]).DoModal() != IDOK) 
      m_Xfer[1]->Abort();

      UpdateMenu();	
   }
}



void CMultiBoardSyncGrabDemoDlg::OnViewOptions() 
{
	CViewDlg dlg(this, m_View);
	if( dlg.DoModal() == IDOK)
	{
		m_ImageWnd->Invalidate();
		m_ImageWnd->OnSize();
	}
}

//*****************************************************************************************
//
//					File Options
//
//*****************************************************************************************

void CMultiBoardSyncGrabDemoDlg::OnFileNew() 
{
	m_Buffers[0]->Clear();
   m_Buffers[1]->Clear();
	InvalidateRect(NULL, FALSE);
}


void CMultiBoardSyncGrabDemoDlg::GetSignalStatus()
{
   SapAcquisition::SignalStatus signalStatus;

   if (m_Acq[0] && m_Acq[0]->IsSignalStatusAvailable())
   {
      if (m_Acq[0]->GetSignalStatus(&signalStatus, SignalCallback, this))
         GetSignalStatus(signalStatus);
   }
}

void CMultiBoardSyncGrabDemoDlg::GetSignalStatus(SapAcquisition::SignalStatus signalStatus)
{
   m_IsSignalDetected = (signalStatus != SapAcquisition::SignalNone);

   if (m_IsSignalDetected)
      SetWindowText(m_appTitle);
   else
   {
      CString newTitle = m_appTitle;
      newTitle += " (No camera signal detected)";
      SetWindowText(newTitle);
   }
}

