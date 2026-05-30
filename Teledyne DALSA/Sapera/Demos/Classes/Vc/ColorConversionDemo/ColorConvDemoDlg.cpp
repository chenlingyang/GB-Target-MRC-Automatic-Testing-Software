// ColorConvDemoDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ColorConvDemo.h"
#include "ColorConvDemoDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

SapFormatInfo CColorConvDemoDlg::m_ColorConvOutputFormatValues[] =
{
   { SapFormatRGB888, _T("RGB 888") },
   { SapFormatRGB8888, _T("RGB 8888") },
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
// CColorConvDemoDlg dialog

CColorConvDemoDlg::CColorConvDemoDlg(CWnd* pParent /*=NULL*/)
   : CDialog(CColorConvDemoDlg::IDD, pParent)
{
   //{{AFX_DATA_INIT(CColorConvDemoDlg)
   m_ColorConvChoice = ColorConvChoiceNone;
   //}}AFX_DATA_INIT
   // Note that LoadIcon does not require a subsequent DestroyIcon in Win32
   m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);

   m_Acq = NULL;
   m_Buffers = NULL;
   m_Xfer = NULL;
   m_View = NULL;
   m_ImageWnd = NULL;
   m_ColorConv = NULL;
   m_Pro = NULL;

   m_pColorConvOptionsDlg = NULL;

   m_IsSignalDetected = TRUE;
}

void CColorConvDemoDlg::DoDataExchange(CDataExchange* pDX)
{
   CDialog::DoDataExchange(pDX);
   //{{AFX_DATA_MAP(CColorConvDemoDlg)
   DDX_Control(pDX, IDC_COLOR_CONV_OUTPUT_FORMAT, m_ColorConvOutputFormatCtrl);
   DDX_Control(pDX, IDC_VERT_SCROLLBAR, m_verticalScr);
   DDX_Control(pDX, IDC_HORZ_SCROLLBAR, m_horizontalScr);
   DDX_Control(pDX, IDC_VIEW_WND, m_viewWnd);
   DDX_Control(pDX, IDC_STATUS, m_statusWnd);
   DDX_Radio(pDX, IDC_COLOR_CONV_DISABLE, m_ColorConvChoice);
   //}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CColorConvDemoDlg, CDialog)
   //{{AFX_MSG_MAP(CColorConvDemoDlg)
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
   ON_BN_CLICKED(IDC_COLOR_CONV_DISABLE, OnColorConvChoiceChange)
   ON_BN_CLICKED(IDC_COLOR_CONV_ENABLE_HW, OnColorConvChoiceChange)
   ON_BN_CLICKED(IDC_COLOR_CONV_ENABLE_SW, OnColorConvChoiceChange)
   ON_BN_CLICKED(IDC_COLOR_CONV_OPTIONS, OnColorConvOptions)
   ON_WM_SETCURSOR()
   ON_WM_LBUTTONDOWN()
   ON_BN_CLICKED(IDC_VIEW_OPTIONS, OnViewOptions)
   ON_CBN_SELCHANGE(IDC_COLOR_CONV_OUTPUT_FORMAT, OnColorConvOutputFormat)
   ON_WM_ENDSESSION()
   ON_WM_QUERYENDSESSION()
   //}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CColorConvDemoDlg message handlers

//
// This function is called each time an image has been transferred into system memory by the transfer object
//
void CColorConvDemoDlg::XferCallback(SapXferCallbackInfo *pInfo)
{
   CColorConvDemoDlg *pDlg = (CColorConvDemoDlg *)pInfo->GetContext();

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
   }
}

//
// This function is called each time a buffer has been processed by the processing object
//
void CColorConvDemoDlg::ProCallback(SapProCallbackInfo *pInfo)
{
   CColorConvDemoDlg* pDlg = (CColorConvDemoDlg *)pInfo->GetContext();
   CString			str;

   // Check if color conversion was enabled and if it's been done by software
   if (pDlg->m_ColorConv->IsSoftwareEnabled())
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
void CColorConvDemoDlg::ViewCallback(SapViewCallbackInfo *pInfo)
{
   CColorConvDemoDlg *pDlg = (CColorConvDemoDlg *)pInfo->GetContext();

   if (pDlg->m_ImageWnd->IsRoiTrackerActive())
      pDlg->m_ImageWnd->DisplayRoiTracker();
}

//
// This function is called each time the input signal connection status change
//
void CColorConvDemoDlg::SignalCallback(SapAcqCallbackInfo *pInfo)
{
   CColorConvDemoDlg *pDlg = (CColorConvDemoDlg *)pInfo->GetContext();
   pDlg->GetSignalStatus(pInfo->GetSignalStatus());
}

BOOL CColorConvDemoDlg::OnInitDialog()
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

   // Are we operating on-line?
   CAcqConfigDlg dlg(this, NULL);
   if (dlg.DoModal() == IDOK)
   {
      // Define on-line objects
      m_Acq = new SapAcquisition(dlg.GetAcquisition());
      m_Buffers = new SapBufferWithTrash(2, m_Acq);
      m_Xfer = new SapAcqToBuf(m_Acq, m_Buffers, XferCallback, this);
      m_ColorConv = new SapColorConversion(m_Acq, m_Buffers);
   }
   else
   {
      // Define off-line objects
      m_Buffers = new SapBuffer();
      m_ColorConv = new SapColorConversion(m_Buffers);
   }

   // Define other objects
   m_Pro = new SapMyProcessing(m_Buffers, m_ColorConv, ProCallback, this);
   m_View = new SapView(m_Buffers, m_viewWnd.GetSafeHwnd(), ViewCallback, this);

   // Create all objects
   if (!CreateObjects())
   {
      EndDialog(TRUE);
      return FALSE;
   }

   // Fill color output format control
   UpdateOutputFormatList();

   // Create image window
   m_ImageWnd = new CImageWnd(m_View, &m_viewWnd, &m_horizontalScr, &m_verticalScr, this);

   // Get current input signal connection status
   GetSignalStatus();

   // Create color Options dialog (modeless)
   m_pColorConvOptionsDlg = new CColorConversionOptionsDlg(this, m_ColorConv, m_Xfer, m_ImageWnd, m_Pro);
   if (m_pColorConvOptionsDlg == NULL || !m_pColorConvOptionsDlg->Create(CColorConversionOptionsDlg::IDD, this))
   {
      EndDialog(TRUE);
      return FALSE;
   }

   UpdateMenu();

   return TRUE;  // return TRUE  unless you set the focus to a control
}

BOOL CColorConvDemoDlg::CreateObjects()
{
   // Create acquisition object
   if (m_Acq && !*m_Acq && !m_Acq->Create())
   {
      DestroyObjects();
      return FALSE;
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

   // Create color conversion object
   if (m_ColorConv && !*m_ColorConv && !m_ColorConv->Create())
   {
      DestroyObjects();
      return FALSE;
   }

   UpdateColorConvChoiceFromData();

   // Create view object
   if (m_View && !*m_View)
   {
      if (m_ColorConv && *m_ColorConv)
      {
         // Set buffer to be viewed
         // When using hardware color conversion decoder, view the acquired RGB buffer,
         // otherwise, for software color conversion, view the converted RGB buffer.
         SapBuffer* colorConvBuffer = m_ColorConv->GetOutputBuffer();
         if (colorConvBuffer && *colorConvBuffer)
            m_View->SetBuffer(colorConvBuffer);
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

BOOL CColorConvDemoDlg::DestroyObjects()
{
   // Destroy transfer object
   if (m_Xfer && *m_Xfer) m_Xfer->Destroy();

   // Destroy processing object
   if (m_Pro && *m_Pro) m_Pro->Destroy();

   // Destroy view object
   if (m_View && *m_View) m_View->Destroy();

   // Destroy color conversion object
   if (m_ColorConv && *m_ColorConv) m_ColorConv->Destroy();

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
void CColorConvDemoDlg::OnSysCommand(UINT nID, LPARAM lParam)
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
void CColorConvDemoDlg::OnPaint()
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

void CColorConvDemoDlg::OnDestroy()
{
   CDialog::OnDestroy();

   // Delete color conversion dialog
   if (m_pColorConvOptionsDlg != NULL)
   {
      delete m_pColorConvOptionsDlg;
      m_pColorConvOptionsDlg = NULL;
   }

   // Destroy all objects
   DestroyObjects();

   // Delete all objects
   if (m_ColorConv)   delete m_ColorConv;
   if (m_Xfer)        delete m_Xfer;
   if (m_ImageWnd)    delete m_ImageWnd;
   if (m_Pro)			 delete m_Pro;
   if (m_View)        delete m_View;
   if (m_Buffers)     delete m_Buffers;
   if (m_Acq)         delete m_Acq;
}

void CColorConvDemoDlg::OnMouseMove(UINT nFlags, CPoint point)
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

void CColorConvDemoDlg::OnMove(int x, int y)
{
   CDialog::OnMove(x, y);

   if (m_ImageWnd)
      m_ImageWnd->OnMove();
}

void CColorConvDemoDlg::OnSize(UINT nType, int cx, int cy)
{
   CDialog::OnSize(nType, cx, cy);

   if (m_ImageWnd)
      m_ImageWnd->OnSize();
}

void CColorConvDemoDlg::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
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

void CColorConvDemoDlg::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
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
HCURSOR CColorConvDemoDlg::OnQueryDragIcon()
{
   return (HCURSOR)m_hIcon;
}

void CColorConvDemoDlg::OnExit()
{
   EndDialog(TRUE);
}

void CColorConvDemoDlg::OnEndSession(BOOL bEnding)
{
   CDialog::OnEndSession(bEnding);

   if (bEnding)
   {
      // If ending the session, free the resources.
      OnDestroy();
   }
}

BOOL CColorConvDemoDlg::OnQueryEndSession()
{
   if (!CDialog::OnQueryEndSession())
      return FALSE;

   return TRUE;
}

//**************************************************************************************
// Updates the menu items enabling/disabling the proper items depending on the state
//  of the application
//**************************************************************************************
void CColorConvDemoDlg::UpdateMenu(void)
{
   BOOL bAcqNoGrab = m_Xfer && *m_Xfer && !m_Xfer->IsGrabbing();
   BOOL bAcqGrab = m_Xfer && *m_Xfer && m_Xfer->IsGrabbing();
   BOOL bNoGrab = !m_Xfer || !m_Xfer->IsGrabbing();
   BOOL isColorConvEnabled = m_ColorConv && *m_ColorConv && m_ColorConv->IsEnabled();
   BOOL isColorConvHWsupported = m_ColorConv && *m_ColorConv && m_ColorConv->IsHardwareSupported();
   BOOL isColorConvSW = isColorConvEnabled && m_ColorConv && *m_ColorConv && m_ColorConv->IsSoftwareEnabled();
   BOOL isColorConvSWsupported = m_ColorConv && *m_ColorConv && m_ColorConv->IsSoftwareSupported();

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
   GetDlgItem(IDC_FILE_LOAD)->EnableWindow(bNoGrab && (isColorConvSW || !isColorConvEnabled));
   GetDlgItem(IDC_FILE_SAVE)->EnableWindow(bNoGrab && (isColorConvSW || !isColorConvEnabled));
   GetDlgItem(IDC_FILE_SAVE_CONVERTED)->EnableWindow(bNoGrab && isColorConvEnabled);

   // Color Conversion Options
   GetDlgItem(IDC_COLOR_CONV_DISABLE)->EnableWindow(bNoGrab && !isColorConvHWsupported);
   GetDlgItem(IDC_COLOR_CONV_ENABLE_HW)->EnableWindow(bNoGrab && isColorConvHWsupported);
   GetDlgItem(IDC_COLOR_CONV_ENABLE_SW)->EnableWindow(bNoGrab && isColorConvSWsupported);

   // Set selection for Color Conversion output format control
   if (m_ColorConv)
   {
      for (int i = 0; i < sizeof(m_ColorConvOutputFormatValues) / sizeof(m_ColorConvOutputFormatValues[0]); i++)
      {
         if (m_ColorConvOutputFormatValues[i].m_Value == m_ColorConv->GetOutputFormat())
            m_ColorConvOutputFormatCtrl.SetCurSel(i);
      }
   }

   UpdateColorConvChoiceFromData();

   GetDlgItem(IDC_COLOR_CONV_OUTPUT_FORMAT)->EnableWindow(bNoGrab && (m_ColorConvChoice == ColorConvChoiceSW));
   GetDlgItem(IDC_COLOR_CONV_OPTIONS)->EnableWindow(bNoGrab && (m_ColorConvChoice != ColorConvChoiceNone));

   // If last control was disabled, set default focus
   if (!GetFocus())
      GetDlgItem(IDC_EXIT)->SetFocus();

   // Update Color Conversion dialog user interface
   if (m_pColorConvOptionsDlg)
      m_pColorConvOptionsDlg->UpdateInterface();
}


//*****************************************************************************************
//
//               Acquisition Control
//
//*****************************************************************************************

void CColorConvDemoDlg::OnFreeze()
{
   if (m_Xfer->Freeze())
   {
      if (CAbortDlg(this, m_Xfer).DoModal() != IDOK)
         m_Xfer->Abort();

      UpdateMenu();
   }
}

void CColorConvDemoDlg::OnGrab()
{
   m_statusWnd.SetWindowText(_T(""));

   if (m_Xfer->Grab())
   {
      UpdateMenu();
   }
}

void CColorConvDemoDlg::OnSnap()
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

void CColorConvDemoDlg::OnLoadAcqConfig()
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

      // Set Color conversion output format to unknown.
      // The color conversion object will retrieve the output format from the acquisition object.
      m_ColorConv->SetOutputFormat(SapFormatUnknown);

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

void CColorConvDemoDlg::OnBufferOptions()
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

      UpdateOutputFormatList();
      UpdateMenu();
   }
}

void CColorConvDemoDlg::OnViewOptions()
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

void CColorConvDemoDlg::OnFileNew()
{
   m_Buffers->Clear();

   SapBuffer *pBuffer = m_ColorConv->GetOutputBuffer();
   if (pBuffer && pBuffer != m_Buffers)
   {
      pBuffer->Clear();
   }

   InvalidateRect(NULL, FALSE);
}

void CColorConvDemoDlg::OnFileLoad()
{
   CLoadSaveDlg dlg(this, m_Buffers, TRUE);

   if (dlg.DoModal() == IDOK)
   {
      if (m_ColorConv->IsSoftwareEnabled())
      {
         m_Pro->Execute();
      }

      InvalidateRect(NULL, FALSE);
      UpdateWindow();
   }
}

void CColorConvDemoDlg::OnFileSave()
{
   CLoadSaveDlg dlg(this, m_ColorConv->GetInputBuffer(), FALSE);
   dlg.DoModal();
}

void CColorConvDemoDlg::OnFileSaveConverted()
{
   CLoadSaveDlg dlg(this, m_ColorConv->GetOutputBuffer(), FALSE);
   dlg.DoModal();
}


//**************************************************************************************
//
//         Color conversion Options
//
//**************************************************************************************

void CColorConvDemoDlg::OnColorConvChoiceChange()
{
   UpdateData();
   BOOL bEnabled = FALSE;

   // disable / enable the color conversion
   if (m_ColorConvChoice == ColorConvChoiceNone)
      bEnabled = m_ColorConv->Enable(FALSE, FALSE);
   else if (m_ColorConvChoice == ColorConvChoiceHW)
      bEnabled = m_ColorConv->Enable(TRUE, TRUE);
   else if (m_ColorConvChoice == ColorConvChoiceSW)
      bEnabled = m_ColorConv->Enable(TRUE, FALSE);

   if (bEnabled)
   {
      // need to tell the view about the right buffer to render
      UpdateViewFormat();
      UpdateOutputFormatList();

      if (m_ColorConv->IsSoftwareEnabled())
         m_Pro->Execute();

      m_ImageWnd->OnSize();
   }
   else
   {
      ::MessageBoxA(m_hWnd, SapManager::GetLastStatus(), "Sapera Runtime Error", MB_OK | MB_ICONEXCLAMATION);
   }

   InvalidateRect(NULL, FALSE);
   UpdateWindow();

   UpdateMenu();
}

void CColorConvDemoDlg::UpdateColorConvChoiceFromData()
{
   if (m_ColorConv->IsSoftwareEnabled())
      m_ColorConvChoice = ColorConvChoiceSW;
   if (m_ColorConv->IsHardwareEnabled())
      m_ColorConvChoice = ColorConvChoiceHW;
   if (!m_ColorConv->IsEnabled())
      m_ColorConvChoice = ColorConvChoiceNone;

   UpdateData(FALSE);
}

void CColorConvDemoDlg::UpdateOutputFormatList()
{
   m_ColorConvOutputFormatCtrl.ResetContent();
   for (int i = 0; i < sizeof(m_ColorConvOutputFormatValues) / sizeof(m_ColorConvOutputFormatValues[0]); i++)
   {
      if (m_ColorConv->IsSoftwareEnabled())
         if (!m_ColorConv->IsOutputFormatSupported(m_ColorConvOutputFormatValues[i].m_Value))
            continue;

      m_ColorConvOutputFormatCtrl.AddString(m_ColorConvOutputFormatValues[i].m_Name);
      m_ColorConvOutputFormatCtrl.SetItemData(i, m_ColorConvOutputFormatValues[i].m_Value);
   }
}

void CColorConvDemoDlg::UpdateViewFormat()
{
   SapBuffer* colorConvBuffer = m_ColorConv->GetOutputBuffer();

   if (!m_View->Destroy())
      return;

   m_View->SetBuffer(colorConvBuffer);

   if (!m_View->Create())
      return;
}

void CColorConvDemoDlg::OnColorConvOptions()
{
   // Show color conversion Options dialog
   if (m_pColorConvOptionsDlg)
      m_pColorConvOptionsDlg->ShowWindow(SW_SHOW);
}

void CColorConvDemoDlg::OnColorConvOutputFormat()
{
   UpdateData();

   // Try to change color conversion output format to new value
   int index = m_ColorConvOutputFormatCtrl.GetCurSel();

   if (index == CB_ERR)
      return;

   // Set new color conversion decoder output format
   m_ColorConv->SetOutputFormat((SapFormat)m_ColorConvOutputFormatCtrl.GetItemData(index));

   // Set the view with the correct format
   UpdateViewFormat();

   if (m_ColorConv->IsSoftwareEnabled())
      m_Pro->Execute();

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

BOOL CColorConvDemoDlg::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message)
{
   if (m_pColorConvOptionsDlg != NULL && m_pColorConvOptionsDlg->IsWindowVisible())
   {
      if (m_ImageWnd && m_ImageWnd->OnSetCursor(nHitTest))
         return TRUE;
   }

   return CDialog::OnSetCursor(pWnd, nHitTest, message);
}

void CColorConvDemoDlg::OnLButtonDown(UINT nFlags, CPoint point)
{
   if (m_pColorConvOptionsDlg != NULL && m_pColorConvOptionsDlg->IsWindowVisible())
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

void CColorConvDemoDlg::GetSignalStatus()
{
   SapAcquisition::SignalStatus signalStatus;

   if (m_Acq && m_Acq->IsSignalStatusAvailable())
   {
      if (m_Acq->GetSignalStatus(&signalStatus, SignalCallback, this))
         GetSignalStatus(signalStatus);
   }
}

void CColorConvDemoDlg::GetSignalStatus(SapAcquisition::SignalStatus signalStatus)
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
