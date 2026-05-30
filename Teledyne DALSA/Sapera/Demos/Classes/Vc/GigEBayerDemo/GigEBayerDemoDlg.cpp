// GigEBayerDemoDlg.cpp : implementation file
//

#include "stdafx.h"
#include "GigEBayerDemo.h"
#include "GigEBayerDemoDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

SapFormatInfo CGigEBayerDemoDlg::m_BayerOutputFormatValues[] =
{
   { SapFormatRGB8888, _T("RGB 8888") },
   { SapFormatRGB101010, _T("RGB 101010") },
};

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
// CGigEBayerDemoDlg dialog

CGigEBayerDemoDlg::CGigEBayerDemoDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CGigEBayerDemoDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CGigEBayerDemoDlg)
	m_BayerEnabled = TRUE;
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);

	m_AcqDevice = NULL;
	m_Buffers = NULL;
	m_Xfer = NULL;
	m_View = NULL;
	m_ImageWnd = NULL;
	m_Conv = NULL;
	m_Pro = NULL;

	m_ConvOptionsDlg = NULL;

	m_IsSignalDetected = TRUE;
}

void CGigEBayerDemoDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CGigEBayerDemoDlg)
	DDX_Control(pDX, IDC_BAYER_OUTPUT_FORMAT, m_BayerOutputFormatCtrl);
	DDX_Control(pDX, IDC_VERT_SCROLLBAR, m_verticalScr);
	DDX_Control(pDX, IDC_HORZ_SCROLLBAR, m_horizontalScr);
	DDX_Control(pDX, IDC_VIEW_WND, m_viewWnd);
	DDX_Control(pDX, IDC_STATUS, m_statusWnd);
	DDX_Check(pDX, IDC_BAYER_ENABLE, m_BayerEnabled);
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CGigEBayerDemoDlg, CDialog)
	//{{AFX_MSG_MAP(CGigEBayerDemoDlg)
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
	ON_BN_CLICKED(IDC_LOAD_ACQ_CONFIG, OnLoadAcqConfig)
	ON_BN_CLICKED(IDC_BUFFER_OPTIONS, OnBufferOptions)
	ON_BN_CLICKED(IDC_FILE_LOAD, OnFileLoad)
	ON_BN_CLICKED(IDC_FILE_NEW, OnFileNew)
	ON_BN_CLICKED(IDC_FILE_SAVE, OnFileSave)
	ON_BN_CLICKED(IDC_FILE_SAVE_CONVERTED, OnFileSaveConverted)
	ON_BN_CLICKED(IDC_EXIT, OnExit)
	ON_BN_CLICKED(IDC_BAYER_ENABLE, OnBayerEnable)
	ON_BN_CLICKED(IDC_BAYER_OPTIONS, OnBayerOptions)
	ON_WM_SETCURSOR()
	ON_WM_LBUTTONDOWN()
	ON_BN_CLICKED(IDC_VIEW_OPTIONS, OnViewOptions)
	ON_CBN_SELCHANGE(IDC_BAYER_OUTPUT_FORMAT, OnBayerOutputFormat)
	ON_WM_ENDSESSION()
	ON_WM_QUERYENDSESSION()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CGigEBayerDemoDlg message handlers

//
// This function is called each time an image has been transferred into system memory by the transfer object
//
void CGigEBayerDemoDlg::XferCallback(SapXferCallbackInfo *pInfo)
{
	CGigEBayerDemoDlg *pDlg = (CGigEBayerDemoDlg *)pInfo->GetContext();

	// If grabbing in trash buffer, do not display the image, update the
	// appropriate number of frames on the status bar instead
	if (pInfo->IsTrash())
	{
		CString str;
		str.Format(_T("Frames acquired in trash buffer: %03d"), pInfo->GetEventCount());

		pDlg->m_statusWnd.SetWindowText(str);
	}

	else
	{
		// Process current buffer (see Run member function into the SapMyProcessing.cpp file)
		pDlg->m_Pro->Execute();
		pDlg->UpdateTitleBar();
	}
}

//
// This function is called each time a buffer has been processed by the processing object
//
void CGigEBayerDemoDlg::ProCallback(SapProCallbackInfo *pInfo)
{
	CGigEBayerDemoDlg* pDlg = (CGigEBayerDemoDlg *)pInfo->GetContext();
	CString			str;

	// Check if bayer conversion was enabled and if it's been done by software
	if (pDlg->m_Conv->IsEnabled() && pDlg->m_Conv->IsSoftwareEnabled())
	{
		// Show current buffer index and execution time in millisecond
		str.Format(_T("Color conversion = %5.2f ms"), pDlg->m_Pro->GetTime());
	}

	// Refresh view
	pDlg->m_statusWnd.SetWindowText(str);
	pDlg->m_View->Show();
}

//
// This function is called each time a buffer has been shown by the view object
//
void CGigEBayerDemoDlg::ViewCallback(SapViewCallbackInfo *pInfo)
{
	CGigEBayerDemoDlg *pDlg = (CGigEBayerDemoDlg *)pInfo->GetContext();

	if (pDlg->m_ImageWnd->IsRoiTrackerActive())
		pDlg->m_ImageWnd->DisplayRoiTracker();
}

//
// This function is called each time the input signal connection status change
//
void CGigEBayerDemoDlg::SignalCallback(SapAcqCallbackInfo *pInfo)
{
	CGigEBayerDemoDlg *pDlg = (CGigEBayerDemoDlg *)pInfo->GetContext();
	pDlg->GetSignalStatus(pInfo->GetSignalStatus());
}

BOOL CGigEBayerDemoDlg::OnInitDialog()
{
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
	SetIcon(m_hIcon, FALSE);   // Set small icon
	SetIcon(m_hIcon, TRUE);      // Set big icon

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
	m_AcqDevice = new SapAcqDevice(dlg.GetLocation(), dlg.GetConfigFile());
	m_Buffers = new SapBufferWithTrash(2, m_AcqDevice);
	m_Xfer = new SapAcqDeviceToBuf(m_AcqDevice, m_Buffers, XferCallback, this);
	m_Conv = new SapColorConversion(m_AcqDevice, m_Buffers);
	m_Pro = new SapMyProcessing(m_Buffers, m_Conv, ProCallback, this);
	m_View = new SapView(m_Buffers, m_viewWnd.GetSafeHwnd(), ViewCallback, this);

	// Create all objects
	if (!CreateObjects())
	{
		EndDialog(TRUE);
		return FALSE;
	}

	// Fill Bayer output format control
	m_BayerOutputFormatCtrl.ResetContent();
	for (int i = 0; i < sizeof(m_BayerOutputFormatValues) / sizeof(m_BayerOutputFormatValues[0]); i++)
	{
		m_BayerOutputFormatCtrl.AddString(CString(m_BayerOutputFormatValues[i].m_Name));
		m_BayerOutputFormatCtrl.SetItemData(i, m_BayerOutputFormatValues[i].m_Value);
	}

	// Create image window
	m_ImageWnd = new CImageWnd(m_View, &m_viewWnd, &m_horizontalScr, &m_verticalScr, this);

	// Get current input signal connection status
	GetSignalStatus();

	// Create Bayer Options dialog (modeless)
	m_ConvOptionsDlg = new CColorConversionOptionsDlg(this, m_Conv, m_Xfer, m_ImageWnd, m_Pro);
	if (m_ConvOptionsDlg == NULL || !m_ConvOptionsDlg->Create(CColorConversionOptionsDlg::IDD, this))
	{
		EndDialog(TRUE);
		return FALSE;
	}

	UpdateMenu();

	return TRUE;  // return TRUE  unless you set the focus to a control
}

BOOL CGigEBayerDemoDlg::CreateObjects()
{

	// Create acquisition object
	if (m_AcqDevice && !*m_AcqDevice && !m_AcqDevice->Create())
	{
		DestroyObjects();
		return FALSE;
	}

	// Check if software color conversion is supported
	if (m_AcqDevice)
	{
		if (m_AcqDevice->IsRawBayerOutput() == FALSE)
		{
			MessageBox(_T("Software color conversion is not supported on this camera!"));
			DestroyObjects();
			EndDialog(TRUE);
			return FALSE;
		}
	}

	// Enable/Disable bayer conversion
	// This call may require to modify the acquisition output format.
	// For this reason, it has to be done after creating the acquisition object but before
	// creating the output buffer object.
	if (m_Conv && !m_Conv->Enable(m_BayerEnabled, FALSE))
	{
		m_BayerEnabled = FALSE;
	}

	// Create buffer objects
	if (m_Buffers && !*m_Buffers)
	{
		if (!m_Buffers->Create())
		{
			DestroyObjects();
			return FALSE;
		}
		// Clear all buffers
		m_Buffers->Clear();
	}

	// Create color converter object
	if (m_Conv && !*m_Conv && !m_Conv->Create())
	{
		DestroyObjects();
		return FALSE;
	}

	// Create view object
	if (m_View && !*m_View)
	{
		if (m_Conv && *m_Conv)
		{
			// Set buffer to be viewed
			// When using hardware bayer decoder, view the acquired RGB buffer,
			// otherwise, for software bayer conversion, view the converted RGB buffer.
			SapBuffer* convBuffer = m_Conv->GetOutputBuffer();
			if (convBuffer && *convBuffer)
				m_View->SetBuffer(convBuffer);
			else
				m_View->SetBuffer(m_Buffers);
		}

		if (!m_View->Create())
		{
			DestroyObjects();
			return FALSE;
		}
	}

	// Create transfer object
	if (m_Xfer && !*m_Xfer)
	{
		if (!m_Xfer->Create())
		{
			DestroyObjects();
			return FALSE;
		}

		m_Xfer->SetAutoEmpty(FALSE);
	}

	// Create processing object
	if (m_Pro && !*m_Pro)
	{
		if (!m_Pro->Create())
		{
			DestroyObjects();
			return FALSE;
		}

		m_Pro->SetAutoEmpty(TRUE);
	}

	return TRUE;
}

BOOL CGigEBayerDemoDlg::DestroyObjects()
{
	// Destroy processing object
	if (m_Pro && *m_Pro) m_Pro->Destroy();

	// Destroy transfer object
	if (m_Xfer && *m_Xfer) m_Xfer->Destroy();

	// Destroy view object
	if (m_View && *m_View) m_View->Destroy();

	// Destroy bayer object
	if (m_Conv && *m_Conv) m_Conv->Destroy();

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
void CGigEBayerDemoDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
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
void CGigEBayerDemoDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, (WPARAM)dc.GetSafeHdc(), 0);

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

		if (m_ImageWnd)
		{
			m_ImageWnd->OnPaint();
		}
	}
}

void CGigEBayerDemoDlg::OnDestroy()
{
	// Delete Bayer dialog
	if (m_ConvOptionsDlg != NULL)
	{
		delete m_ConvOptionsDlg;
		m_ConvOptionsDlg = NULL;
	}

	// Destroy all objects
	DestroyObjects();

	// Delete all objects
	if (m_Conv)		delete m_Conv;
	if (m_Xfer)		delete m_Xfer;
	if (m_ImageWnd)	delete m_ImageWnd;
	if (m_Pro)		delete m_Pro;
	if (m_View)		delete m_View;
	if (m_Buffers)	delete m_Buffers;
	if (m_AcqDevice)	delete m_AcqDevice;

	CDialog::OnDestroy();
}

void CGigEBayerDemoDlg::OnMouseMove(UINT nFlags, CPoint point)
{
	if (m_IsSignalDetected)
	{
		CString str = m_appTitle;

		if (!nFlags && m_ImageWnd)
			str += "  " + m_ImageWnd->GetPixelString(point);

		SetWindowText(str);
	}

	CDialog::OnMouseMove(nFlags, point);
}

void CGigEBayerDemoDlg::OnMove(int x, int y)
{
	CDialog::OnMove(x, y);

	if (m_ImageWnd)
		m_ImageWnd->OnMove();
}

void CGigEBayerDemoDlg::OnSize(UINT nType, int cx, int cy)
{
	CDialog::OnSize(nType, cx, cy);

	if (m_ImageWnd)
		m_ImageWnd->OnSize();
}

void CGigEBayerDemoDlg::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
	if (pScrollBar->GetDlgCtrlID() == IDC_HORZ_SCROLLBAR)
	{
		// Adjust source's horizontal origin
		if (m_ImageWnd)
			m_ImageWnd->OnHScroll(nSBCode, nPos);

		OnPaint();
	}

	CDialog::OnHScroll(nSBCode, nPos, pScrollBar);
}

void CGigEBayerDemoDlg::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
	if (pScrollBar->GetDlgCtrlID() == IDC_VERT_SCROLLBAR)
	{
		// Adjust source's vertical origin
		if (m_ImageWnd)
			m_ImageWnd->OnVScroll(nSBCode, nPos);

		OnPaint();
	}

	CDialog::OnVScroll(nSBCode, nPos, pScrollBar);
}

// The system calls this to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CGigEBayerDemoDlg::OnQueryDragIcon()
{
	return (HCURSOR)m_hIcon;
}

void CGigEBayerDemoDlg::OnExit()
{
	EndDialog(TRUE);
}

void CGigEBayerDemoDlg::OnEndSession(BOOL bEnding)
{
	CDialog::OnEndSession(bEnding);

	if (bEnding)
	{
		// If ending the session, free the resources.
		OnDestroy();
	}
}

BOOL CGigEBayerDemoDlg::OnQueryEndSession()
{
	if (!CDialog::OnQueryEndSession())
		return FALSE;

	return TRUE;
}

//**************************************************************************************
// Updates the menu items enabling/disabling the proper items depending on the state
//  of the application
//**************************************************************************************
void CGigEBayerDemoDlg::UpdateMenu(void)
{
	BOOL bAcqNoGrab = m_Xfer && *m_Xfer && !m_Xfer->IsGrabbing();
	BOOL bAcqGrab = m_Xfer && *m_Xfer && m_Xfer->IsGrabbing();
	BOOL bNoGrab = !m_Xfer || !m_Xfer->IsGrabbing();
	BOOL isBayerEnabled = m_Conv && *m_Conv && m_Conv->IsEnabled();
	BOOL isBayerSoftware = m_Conv && *m_Conv && m_Conv->IsSoftwareEnabled();

	// Acquisition Control
	GetDlgItem(IDC_GRAB)->EnableWindow(bAcqNoGrab);
	GetDlgItem(IDC_SNAP)->EnableWindow(bAcqNoGrab);
	GetDlgItem(IDC_FREEZE)->EnableWindow(bAcqGrab);

	// Options
	GetDlgItem(IDC_LOAD_ACQ_CONFIG)->EnableWindow(m_Xfer && !m_Xfer->IsGrabbing());
	GetDlgItem(IDC_BUFFER_OPTIONS)->EnableWindow(bNoGrab);
	GetDlgItem(IDC_VIEW_OPTIONS)->EnableWindow(bNoGrab);

	// File Options
	GetDlgItem(IDC_FILE_NEW)->EnableWindow(bNoGrab);
	GetDlgItem(IDC_FILE_LOAD)->EnableWindow(bNoGrab && (isBayerSoftware || !isBayerEnabled));
	GetDlgItem(IDC_FILE_SAVE)->EnableWindow(bNoGrab && (isBayerSoftware || !isBayerEnabled));
	GetDlgItem(IDC_FILE_SAVE_CONVERTED)->EnableWindow(bNoGrab && isBayerEnabled);

	// Bayer Options
	((CButton *)GetDlgItem(IDC_BAYER_ENABLE))->SetCheck(isBayerEnabled);
	GetDlgItem(IDC_BAYER_ENABLE)->EnableWindow(bNoGrab);

	// Set selection for Bayer output format control
   if (m_Conv)
   {
	   for (int i = 0; i < sizeof(m_BayerOutputFormatValues) / sizeof(m_BayerOutputFormatValues[0]); i++)
	   {
		   if (m_BayerOutputFormatValues[i].m_Value == m_Conv->GetOutputFormat())
			   m_BayerOutputFormatCtrl.SetCurSel(i);
	   }
   }

	GetDlgItem(IDC_BAYER_OUTPUT_FORMAT)->EnableWindow(bNoGrab);

	// If last control was disabled, set default focus
	if (!GetFocus())
		GetDlgItem(IDC_EXIT)->SetFocus();

	// Update Bayer dialog user interface
	if (m_ConvOptionsDlg)
		m_ConvOptionsDlg->UpdateInterface();
}

void CGigEBayerDemoDlg::UpdateTitleBar()
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
//               Acquisition Control
//
//*****************************************************************************************

void CGigEBayerDemoDlg::OnFreeze()
{
	if (m_Xfer->Freeze())
	{
		if (CAbortDlg(this, m_Xfer).DoModal() != IDOK)
			m_Xfer->Abort();

		UpdateMenu();
	}
}

void CGigEBayerDemoDlg::OnGrab()
{
	m_statusWnd.SetWindowText(_T(""));

	if (m_Xfer->Grab())
	{
		UpdateMenu();
	}
}

void CGigEBayerDemoDlg::OnSnap()
{
	m_statusWnd.SetWindowText(_T(""));

	if (m_Xfer->Snap())
	{
		if (CAbortDlg(this, m_Xfer).DoModal() != IDOK)
			m_Xfer->Abort();

		UpdateMenu();
	}
}

//*****************************************************************************************
//
//               Options
//
//*****************************************************************************************

void CGigEBayerDemoDlg::OnLoadAcqConfig()
{
	// Set acquisition parameters
	CAcqConfigDlg dlg(this, CAcqConfigDlg::ServerAcqDevice);
	if (dlg.DoModal() == IDOK)
	{
		// Destroy objects
		DestroyObjects();

		// Update acquisition object
		SapAcqDevice acqDevice = *m_AcqDevice;
		m_AcqDevice = new SapAcqDevice(dlg.GetLocation(), dlg.GetConfigFile());

		// Set bayer output format to unknown.
		// The bayer object will retrieve the output format from the acquisition object.
		m_Conv->SetOutputFormat(SapFormatUnknown);

		// Recreate objects
		if (!CreateObjects())
		{
			*m_AcqDevice = acqDevice;
			CreateObjects();
		}

		GetSignalStatus();

		m_ImageWnd->OnSize();

		InvalidateRect(NULL);
		UpdateWindow();

		UpdateMenu();
	}
}

void CGigEBayerDemoDlg::OnBufferOptions()
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

		m_ImageWnd->OnSize();

		InvalidateRect(NULL);
		UpdateWindow();

		UpdateMenu();
	}
}

void CGigEBayerDemoDlg::OnViewOptions()
{
	CViewDlg dlg(this, m_View);

	if (dlg.DoModal() == IDOK)
	{
		m_ImageWnd->Invalidate();
		m_ImageWnd->OnSize();
	}
}

//*****************************************************************************************
//
//               File Options
//
//*****************************************************************************************

void CGigEBayerDemoDlg::OnFileNew()
{
	m_Buffers->Clear();

	SapBuffer *pBayerBuffer = m_Conv->GetOutputBuffer();
	if (pBayerBuffer && pBayerBuffer != m_Buffers)
	{
		pBayerBuffer->Clear();
	}

	InvalidateRect(NULL, FALSE);
}

void CGigEBayerDemoDlg::OnFileLoad()
{
	CLoadSaveDlg dlg(this, m_Buffers, TRUE);

	if (dlg.DoModal() == IDOK)
	{
		if (m_Conv->IsEnabled() && m_Conv->IsSoftwareEnabled())
		{
			m_Pro->Execute();
		}

		InvalidateRect(NULL, FALSE);
		UpdateWindow();
	}
}

void CGigEBayerDemoDlg::OnFileSave()
{
	CLoadSaveDlg dlg(this, m_Conv->GetInputBuffer(), FALSE);
	dlg.DoModal();
}

void CGigEBayerDemoDlg::OnFileSaveConverted()
{
	CLoadSaveDlg dlg(this, m_Conv->GetOutputBuffer(), FALSE);
	dlg.DoModal();
}


//**************************************************************************************
//
//         Bayer Options
//
//**************************************************************************************

void CGigEBayerDemoDlg::OnBayerEnable()
{
	UpdateData();

	DestroyObjects();

	// Recreate objects
	if (!CreateObjects())
	{
		// If ever it fail, try to go back to previous state
		m_BayerEnabled = !m_BayerEnabled;
		UpdateData(FALSE);

		CreateObjects();
	}

	if (m_Conv->IsEnabled() && m_Conv->IsSoftwareEnabled())
	{
		m_Pro->Execute();
	}

	m_ImageWnd->OnSize();

	InvalidateRect(NULL, FALSE);
	UpdateWindow();

	UpdateMenu();
}

void CGigEBayerDemoDlg::OnBayerOptions()
{
	// Show Bayer Options dialog
	if (m_ConvOptionsDlg)
		m_ConvOptionsDlg->ShowWindow(SW_SHOW);
}


void CGigEBayerDemoDlg::OnBayerOutputFormat()
{
	UpdateData();

	// Retrieve current Bayer decoder output format
	SapFormat bayerOutputFormat = m_Conv->GetOutputFormat();

	// Try to change bayer output format to new value
	int index = m_BayerOutputFormatCtrl.GetCurSel();

	if (index == CB_ERR)
		return;

	// Before changing Bayer decoder output format, we have to destroy Sapera objects
	DestroyObjects();

	// Set new Bayer decoder output format
	m_Conv->SetOutputFormat((SapFormat)m_BayerOutputFormatCtrl.GetItemData(index));

	// Recreate objects
	if (!CreateObjects())
	{
		// If ever it fail, try to go back to previous state

		// Set Bayer decoder output format to what it was before
		m_Conv->SetOutputFormat(bayerOutputFormat);

		CreateObjects();
	}

	m_ImageWnd->OnSize();

	InvalidateRect(NULL, FALSE);
	UpdateWindow();

	UpdateMenu();
}
//**************************************************************************************
//
//         ROI tracking
//
//**************************************************************************************

BOOL CGigEBayerDemoDlg::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message)
{
	if (m_ConvOptionsDlg != NULL && m_ConvOptionsDlg->IsWindowVisible())
	{
		if (m_ImageWnd && m_ImageWnd->OnSetCursor(nHitTest))
			return TRUE;
	}

	return CDialog::OnSetCursor(pWnd, nHitTest, message);
}

void CGigEBayerDemoDlg::OnLButtonDown(UINT nFlags, CPoint point)
{
	if (m_ConvOptionsDlg != NULL && m_ConvOptionsDlg->IsWindowVisible())
	{
		if (m_ImageWnd && m_ImageWnd->OnLButtonDown(point))
		{
			InvalidateRect(NULL, FALSE);
			UpdateMenu();
		}
	}

	CDialog::OnLButtonDown(nFlags, point);
}

//*****************************************************************************************
//
//					Status signal 
//
//*****************************************************************************************

void CGigEBayerDemoDlg::GetSignalStatus()
{

}

void CGigEBayerDemoDlg::GetSignalStatus(SapAcquisition::SignalStatus signalStatus)
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
