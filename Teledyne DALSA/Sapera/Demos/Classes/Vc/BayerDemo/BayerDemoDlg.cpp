// BayerDemoDlg.cpp : implementation file
//

#include "stdafx.h"
#include "BayerDemo.h"
#include "BayerDemoDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

SapFormatInfo CBayerDemoDlg::m_BayerOutputFormatValues[]=
{
   { SapFormatRGB8888,  _T("RGB 8888") },
   { SapFormatRGB101010, _T("RGB 101010") },
   { SapFormatRGB16161616, _T("RGB 16161616") }
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
// CBayerDemoDlg dialog

CBayerDemoDlg::CBayerDemoDlg(CWnd* pParent /*=NULL*/)
   : CDialog(CBayerDemoDlg::IDD, pParent)
{
   //{{AFX_DATA_INIT(CBayerDemoDlg)
   m_BayerEnabled = TRUE;
   m_BayerUseHardware = TRUE;
   //}}AFX_DATA_INIT
   // Note that LoadIcon does not require a subsequent DestroyIcon in Win32
   m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);

   m_Acq         = NULL;
   m_Buffers      = NULL;
   m_Xfer        = NULL;
   m_View        = NULL;
   m_ImageWnd    = NULL;
   m_Bayer       = NULL;
   m_Pro			  = NULL;

   m_pBayerOptionsDlg   = NULL;

   m_IsSignalDetected = TRUE;
}

void CBayerDemoDlg::DoDataExchange(CDataExchange* pDX)
{
   CDialog::DoDataExchange(pDX);
   //{{AFX_DATA_MAP(CBayerDemoDlg)
   DDX_Control(pDX, IDC_BAYER_OUTPUT_FORMAT, m_BayerOutputFormatCtrl);
   DDX_Control(pDX, IDC_VERT_SCROLLBAR, m_verticalScr);
   DDX_Control(pDX, IDC_HORZ_SCROLLBAR, m_horizontalScr);
   DDX_Control(pDX, IDC_VIEW_WND, m_viewWnd);
   DDX_Control(pDX, IDC_STATUS, m_statusWnd);
   DDX_Check(pDX, IDC_BAYER_ENABLE, m_BayerEnabled);
   DDX_Check(pDX, IDC_BAYER_USE_HARDWARE, m_BayerUseHardware);
   //}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CBayerDemoDlg, CDialog)
   //{{AFX_MSG_MAP(CBayerDemoDlg)
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
   ON_BN_CLICKED(IDC_BAYER_USE_HARDWARE, OnBayerUseHardware)
   ON_CBN_SELCHANGE(IDC_BAYER_OUTPUT_FORMAT, OnBayerOutputFormat)
   ON_WM_ENDSESSION()
   ON_WM_QUERYENDSESSION()
   //}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CBayerDemoDlg message handlers

//
// This function is called each time an image has been transferred into system memory by the transfer object
//
void CBayerDemoDlg::XferCallback(SapXferCallbackInfo *pInfo)
{
   CBayerDemoDlg *pDlg= (CBayerDemoDlg *) pInfo->GetContext();

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
void CBayerDemoDlg::ProCallback(SapProCallbackInfo *pInfo)
{
   CBayerDemoDlg* pDlg= (CBayerDemoDlg *) pInfo->GetContext();
   CString			str;

   // Check if bayer conversion was enabled and if it's been done by software
   if (pDlg->m_Bayer->IsEnabled() && pDlg->m_Bayer->IsSoftware())
   {
      // Show current buffer index and execution time in millisecond
      str.Format(_T("Bayer conversion = %5.2f ms"), pDlg->m_Pro->GetTime());
   }

   // Refresh view
   pDlg->m_statusWnd.SetWindowText(str);
   pDlg->m_View->Show();
}

//
// This function is called each time a buffer has been shown by the view object
//
void CBayerDemoDlg::ViewCallback(SapViewCallbackInfo *pInfo)
{
   CBayerDemoDlg *pDlg= (CBayerDemoDlg *) pInfo->GetContext();

   if( pDlg->m_ImageWnd->IsRoiTrackerActive())
      pDlg->m_ImageWnd->DisplayRoiTracker();
}

//
// This function is called each time the input signal connection status change
//
void CBayerDemoDlg::SignalCallback(SapAcqCallbackInfo *pInfo)
{
   CBayerDemoDlg *pDlg = (CBayerDemoDlg *) pInfo->GetContext();
   pDlg->GetSignalStatus(pInfo->GetSignalStatus());
}

BOOL CBayerDemoDlg::OnInitDialog()
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

   // With the introduction of the new(est) Color Conversion Demo
   // this is now deprecated and not maintained. 
   if(MessageBox(
      _T("Please note that the Bayer Demo is now deprecated.\nYou should use the Color Conversion Demo instead\nDo you want to continue?"),
      m_appTitle, MB_ICONWARNING | MB_YESNO) == IDNO)
   {
      EndDialog(TRUE);
      return FALSE;
   }

   // Are we operating on-line?
   CAcqConfigDlg dlg(this, NULL);
   if (dlg.DoModal() == IDOK)
   {
      // Define on-line objects
      m_Acq    = new SapAcquisition(dlg.GetAcquisition());
      m_Buffers = new SapBufferWithTrash(2, m_Acq);
      m_Xfer   = new SapAcqToBuf(m_Acq, m_Buffers, XferCallback, this);
      m_Bayer  = new SapBayer(m_Acq, m_Buffers);
   }
   else
   {
      // Define off-line objects
      m_Buffers = new SapBuffer();
      m_Bayer  = new SapBayer(m_Buffers);
   }

   // Define other objects
   m_Pro  = new SapMyProcessing( m_Buffers, m_Bayer, ProCallback, this);
   m_View = new SapView( m_Buffers, m_viewWnd.GetSafeHwnd(), ViewCallback, this);

   // Create all objects
   if (!CreateObjects()) 
   { 
      EndDialog(TRUE); 
      return FALSE; 
   }

   // Fill Bayer output format control
   m_BayerOutputFormatCtrl.ResetContent();
   for (int i= 0; i < sizeof(m_BayerOutputFormatValues) / sizeof(m_BayerOutputFormatValues[0]); i++)
   {
      m_BayerOutputFormatCtrl.AddString(m_BayerOutputFormatValues[i].m_Name);
      m_BayerOutputFormatCtrl.SetItemData(i, m_BayerOutputFormatValues[i].m_Value);
   }

   // Create image window
   m_ImageWnd = new CImageWnd(m_View, &m_viewWnd, &m_horizontalScr, &m_verticalScr, this);

   // Get current input signal connection status
   GetSignalStatus();

   // Create Bayer Options dialog (modeless)
   m_pBayerOptionsDlg= new CBayerOptionsDlg (this, m_Bayer, m_Xfer, m_ImageWnd, m_Pro);
   if( m_pBayerOptionsDlg == NULL || !m_pBayerOptionsDlg->Create( CBayerOptionsDlg::IDD, this))
   {
      EndDialog(TRUE); 
      return FALSE; 
   }

   UpdateMenu();

   return TRUE;  // return TRUE  unless you set the focus to a control
}

BOOL CBayerDemoDlg::CreateObjects()
{
   // Create acquisition object
   if (m_Acq && !*m_Acq && !m_Acq->Create()) 
   {
      DestroyObjects();
      return FALSE;
   }

   // Validate the video format of the configuration file
   if (m_Acq)
   {
      UINT32 videoType = 0;

      if (!m_Acq->GetParameter(CORACQ_PRM_VIDEO, &videoType))
      {
         DestroyObjects();
         return FALSE;
      }

      if (videoType != CORACQ_VAL_VIDEO_BAYER)
      {
         MessageBox(_T("The specified configuration file does not correspond to a Bayer camera"));
         DestroyObjects();
         return FALSE;
      }
   }

   // Enable/Disable bayer conversion
   // This call may require to modify the acquisition output format.
   // For this reason, it has to be done after creating the acquisition object but before
   // creating the output buffer object.
   if( m_Bayer && !m_Bayer->Enable( m_BayerEnabled, m_BayerUseHardware))
   {
      m_BayerEnabled= FALSE;
   }

   // Create buffer objects
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

   // Create bayer object
   if (m_Bayer && !*m_Bayer && !m_Bayer->Create())
   {
      DestroyObjects();
      return FALSE;
   }

   // Create view object
   if (m_View && !*m_View)
   {
      if (m_Bayer && *m_Bayer)
      {
         // Set buffer to be viewed
         // When using hardware bayer decoder, view the acquired RGB buffer,
         // otherwise, for software bayer conversion, view the converted RGB buffer.
         SapBuffer* bayerBuffer = m_Bayer->GetBayerBuffer();
         if (bayerBuffer && *bayerBuffer)
            m_View->SetBuffer(bayerBuffer);
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
      if( !m_Xfer->Create())
      {	
         DestroyObjects();
         return FALSE;
      }

      m_Xfer->SetAutoEmpty( FALSE);
   }

   // Create processing object
   if( m_Pro && !*m_Pro)
   {
      if( !m_Pro->Create())
      {
         DestroyObjects();
         return FALSE;
      }

      m_Pro->SetAutoEmpty( TRUE);
   }

   return TRUE;
}

BOOL CBayerDemoDlg::DestroyObjects()
{
   // Destroy transfer object
   if (m_Xfer && *m_Xfer) m_Xfer->Destroy();

   // Destroy processing object
   if (m_Pro && *m_Pro) m_Pro->Destroy();

   // Destroy view object
   if (m_View && *m_View) m_View->Destroy();

   // Destroy bayer object
   if (m_Bayer && *m_Bayer) m_Bayer->Destroy();

   // Destroy buffer object
   if (m_Buffers && *m_Buffers) m_Buffers->Destroy();

   // Destroy acquisition object
   if (m_Acq && *m_Acq) m_Acq->Destroy();

   return TRUE;
}

//**********************************************************************************
//
//				Window related functions
//
//**********************************************************************************
void CBayerDemoDlg::OnSysCommand(UINT nID, LPARAM lParam)
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
void CBayerDemoDlg::OnPaint() 
{
   if (IsIconic())
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

      if (m_ImageWnd)
      {
         m_ImageWnd->OnPaint();
      }
   }
}

void CBayerDemoDlg::OnDestroy() 
{
   CDialog::OnDestroy();

   // Delete Bayer dialog
   if( m_pBayerOptionsDlg != NULL)
   {
      delete m_pBayerOptionsDlg;
      m_pBayerOptionsDlg= NULL;
   }

   // Destroy all objects
   DestroyObjects();

   // Delete all objects
   if (m_Bayer)       delete m_Bayer;
   if (m_Xfer)        delete m_Xfer; 
   if (m_ImageWnd)    delete m_ImageWnd;
   if (m_Pro)			 delete m_Pro; 
   if (m_View)        delete m_View; 
   if (m_Buffers)     delete m_Buffers; 
   if (m_Acq)         delete m_Acq; 
}

void CBayerDemoDlg::OnMouseMove(UINT nFlags, CPoint point) 
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

void CBayerDemoDlg::OnMove(int x, int y) 
{
   CDialog::OnMove(x, y);

   if (m_ImageWnd) 
      m_ImageWnd->OnMove();
}

void CBayerDemoDlg::OnSize(UINT nType, int cx, int cy) 
{
   CDialog::OnSize(nType, cx, cy);

   if (m_ImageWnd) 
      m_ImageWnd->OnSize();
}

void CBayerDemoDlg::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar) 
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

void CBayerDemoDlg::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar) 
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
HCURSOR CBayerDemoDlg::OnQueryDragIcon()
{
   return (HCURSOR) m_hIcon;
}

void CBayerDemoDlg::OnExit() 
{
   EndDialog(TRUE);
}

void CBayerDemoDlg::OnEndSession(BOOL bEnding)
{
   CDialog::OnEndSession(bEnding);

   if( bEnding)
   {
      // If ending the session, free the resources.
      OnDestroy(); 
   }
}

BOOL CBayerDemoDlg::OnQueryEndSession()
{
   if (!CDialog::OnQueryEndSession())
      return FALSE;

   return TRUE;
}

//**************************************************************************************
// Updates the menu items enabling/disabling the proper items depending on the state
//  of the application
//**************************************************************************************
void CBayerDemoDlg::UpdateMenu( void)
{
   BOOL bAcqNoGrab			= m_Xfer && *m_Xfer && !m_Xfer->IsGrabbing();
   BOOL bAcqGrab				= m_Xfer && *m_Xfer && m_Xfer->IsGrabbing();
   BOOL bNoGrab				= !m_Xfer || !m_Xfer->IsGrabbing();
   BOOL isBayerEnabled		= m_Bayer && *m_Bayer && m_Bayer->IsEnabled();
   BOOL isBayerAvailable	= m_Acq && *m_Acq && m_Acq->IsBayerAvailable();
   BOOL isBayerSoftware		= m_Bayer && *m_Bayer && m_Bayer->IsSoftware();

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
   ((CButton *) GetDlgItem(IDC_BAYER_ENABLE))->SetCheck(isBayerEnabled);
   GetDlgItem(IDC_BAYER_ENABLE)->EnableWindow(bNoGrab);

   ((CButton *) GetDlgItem(IDC_BAYER_USE_HARDWARE))->SetCheck( isBayerAvailable && !isBayerSoftware);
   GetDlgItem(IDC_BAYER_USE_HARDWARE)->EnableWindow( isBayerAvailable && isBayerEnabled && bNoGrab);

   // Set selection for Bayer output format control
   if (m_Bayer)
   {
      for (int i= 0; i < sizeof(m_BayerOutputFormatValues) / sizeof(m_BayerOutputFormatValues[0]); i++)
      {
         if (m_BayerOutputFormatValues[i].m_Value == m_Bayer->GetOutputFormat())
            m_BayerOutputFormatCtrl.SetCurSel(i);
      }
   }

   GetDlgItem(IDC_BAYER_OUTPUT_FORMAT)->EnableWindow(bNoGrab);

   // If last control was disabled, set default focus
   if (!GetFocus())
      GetDlgItem(IDC_EXIT)->SetFocus();

   // Update Bayer dialog user interface
   if( m_pBayerOptionsDlg)
      m_pBayerOptionsDlg->UpdateInterface();
}

void CBayerDemoDlg::UpdateTitleBar()
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

void CBayerDemoDlg::OnFreeze( ) 
{
   if( m_Xfer->Freeze())
   {
      if (CAbortDlg(this, m_Xfer).DoModal() != IDOK) 
         m_Xfer->Abort();

      UpdateMenu();
   }
}

void CBayerDemoDlg::OnGrab() 
{
   m_statusWnd.SetWindowText(_T(""));

   if( m_Xfer->Grab())
   {
      UpdateMenu();
   }
}

void CBayerDemoDlg::OnSnap() 
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
//               Options
//
//*****************************************************************************************

void CBayerDemoDlg::OnLoadAcqConfig() 
{
   // Set acquisition parameters
   CAcqConfigDlg dlg(this, m_Acq);
   if (dlg.DoModal() == IDOK)
   {
      // Destroy objects
      DestroyObjects();

      // Update acquisition object
      SapAcquisition acq = *m_Acq;
      *m_Acq = dlg.GetAcquisition();

      // Set bayer output format to unknown.
      // The bayer object will retrieve the output format from the acquisition object.
      m_Bayer->SetOutputFormat( SapFormatUnknown);

      // Recreate objects
      if (!CreateObjects())
      {
         *m_Acq = acq;
         CreateObjects();
      }

      GetSignalStatus();

      m_ImageWnd->OnSize();

      InvalidateRect(NULL);
      UpdateWindow();

      UpdateMenu();
   }
}

void CBayerDemoDlg::OnBufferOptions() 
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

void CBayerDemoDlg::OnViewOptions() 
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

void CBayerDemoDlg::OnFileNew() 
{
   m_Buffers->Clear();

   SapBuffer *pBayerBuffer = m_Bayer->GetBayerBuffer();
   if (pBayerBuffer && pBayerBuffer != m_Buffers)
   {
      pBayerBuffer->Clear();
   }

   InvalidateRect(NULL, FALSE);
}

void CBayerDemoDlg::OnFileLoad() 
{
   CLoadSaveDlg dlg(this, m_Buffers, TRUE);

   if (dlg.DoModal() == IDOK)
   {
      if( m_Bayer->IsEnabled() && m_Bayer->IsSoftware())
      {
         m_Pro->Execute();
      }

      InvalidateRect(NULL, FALSE);
      UpdateWindow();
   }
}

void CBayerDemoDlg::OnFileSave() 
{
   CLoadSaveDlg dlg(this, m_Bayer->GetBuffer(), FALSE);
   dlg.DoModal();
}

void CBayerDemoDlg::OnFileSaveConverted()
{
   CLoadSaveDlg dlg(this, m_Bayer->GetBayerBuffer(), FALSE);
   dlg.DoModal();
}


//**************************************************************************************
//
//         Bayer Options
//
//**************************************************************************************

void CBayerDemoDlg::OnBayerEnable() 
{
   UpdateData();

   DestroyObjects();

   // Recreate objects
   if (!CreateObjects())
   {	
      // If ever it fail, try to go back to previous state
      m_BayerEnabled= !m_BayerEnabled;
      UpdateData( FALSE);

      CreateObjects();
   }

   if( m_Bayer->IsEnabled() && m_Bayer->IsSoftware())
   {
      m_Pro->Execute();
   }

   m_ImageWnd->OnSize();

   InvalidateRect(NULL, FALSE);
   UpdateWindow();

   UpdateMenu();
}

void CBayerDemoDlg::OnBayerOptions() 
{
   // Show Bayer Options dialog
   if (m_pBayerOptionsDlg)
      m_pBayerOptionsDlg->ShowWindow( SW_SHOW);
}

void CBayerDemoDlg::OnBayerUseHardware() 
{
   UpdateData();

   // Destroy objects
   DestroyObjects();

   // Recreate objects
   if (!CreateObjects())
   {	
      // If ever it fail, try to go back to previous state
      m_BayerUseHardware= !m_BayerUseHardware;
      UpdateData( FALSE);

      CreateObjects();
   }

   m_ImageWnd->OnSize();

   InvalidateRect(NULL, FALSE);
   UpdateWindow();

   UpdateMenu();	
}

void CBayerDemoDlg::OnBayerOutputFormat() 
{
   UpdateData();

   // Retrieve current Bayer decoder output format
   SapFormat bayerOutputFormat= m_Bayer->GetOutputFormat();

   // Try to change bayer output format to new value
   int index= m_BayerOutputFormatCtrl.GetCurSel();

   if( index == CB_ERR)
      return;

   // Before changing Bayer decoder output format, we have to destroy Sapera objects
   DestroyObjects();

   // Set new Bayer decoder output format
   m_Bayer->SetOutputFormat( (SapFormat) m_BayerOutputFormatCtrl.GetItemData( index));

   // Recreate objects
   if (!CreateObjects())
   {	
      // If ever it fail, try to go back to previous state

      // Set Bayer decoder output format to what it was before
      m_Bayer->SetOutputFormat( bayerOutputFormat);

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

BOOL CBayerDemoDlg::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
{
   if( m_pBayerOptionsDlg != NULL && m_pBayerOptionsDlg->IsWindowVisible())
   { 
      if (m_ImageWnd && m_ImageWnd->OnSetCursor(nHitTest)) 
         return TRUE;
   }

   return CDialog::OnSetCursor(pWnd, nHitTest, message);
}

void CBayerDemoDlg::OnLButtonDown(UINT nFlags, CPoint point) 
{
   if( m_pBayerOptionsDlg != NULL && m_pBayerOptionsDlg->IsWindowVisible())
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

void CBayerDemoDlg::GetSignalStatus()
{
   SapAcquisition::SignalStatus signalStatus;

   if (m_Acq && m_Acq->IsSignalStatusAvailable())
   {
      if (m_Acq->GetSignalStatus(&signalStatus, SignalCallback, this))
         GetSignalStatus(signalStatus);
   }
}

void CBayerDemoDlg::GetSignalStatus(SapAcquisition::SignalStatus signalStatus)
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
