// CameraCompressionDemoDlg.cpp : implementation file
//

#include "stdafx.h"
#include "GigECameraCompressionDemo.h"
#include "GigECameraCompressionDemoDlg.h"

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
   virtual void DoDataExchange(CDataExchange* pDX);   // DDX/DDV support
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
// CCameraCompressionDemoDlg dialog

CCameraCompressionDemoDlg::CCameraCompressionDemoDlg(CWnd* pParent)
   : CDialog(CCameraCompressionDemoDlg::IDD, pParent)
{
   //{{AFX_DATA_INIT(CCameraCompressionDemoDlg)
   // NOTE: the ClassWizard will add member initialization here
   //}}AFX_DATA_INIT
   // Note that LoadIcon does not require a subsequent DestroyIcon in Win32
   m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);

   m_ImageWnd = NULL;
   m_AcqDevice = NULL;
   m_Buffers = NULL;
   m_BuffersView = NULL;
   m_Xfer = NULL;
   m_View = NULL;
   m_Pro = NULL;
   m_JpegProcessing = NULL;
   m_CompressedImageAvailable = FALSE;
   m_JpegEnabled = false;
   m_EditImageCompressionQualityValue = 0;
   m_TemperatureLogFilename[0] = 0;
   m_temperatureLog = NULL;
}

CCameraCompressionDemoDlg::~CCameraCompressionDemoDlg()
{
   if (m_temperatureLog)
      delete m_temperatureLog;
}

void CCameraCompressionDemoDlg::DoDataExchange(CDataExchange* pDX)
{
   CDialog::DoDataExchange(pDX);
   //{{AFX_DATA_MAP(CCameraCompressionDemoDlg)
   DDX_Control(pDX, IDC_STATUS, m_statusWnd);
   DDX_Control(pDX, IDC_VERT_SCROLLBAR, m_verticalScr);
   DDX_Control(pDX, IDC_HORZ_SCROLLBAR, m_horizontalScr);
   DDX_Control(pDX, IDC_VIEW_WND, m_viewWnd);
   DDX_Control(pDX, IDC_CHECK_JPEG_ENABLE, m_JpegEnableButton);
   DDX_Control(pDX, IDC_SPIN_JPEG_QUALITY, m_SpinImageCompressionQuality);
   DDX_Control(pDX, IDC_EDIT_JPEG_QUALITY, m_EditImageCompressionQuality);
   DDX_Text(pDX, IDC_EDIT_JPEG_QUALITY, m_EditImageCompressionQualityValue);
   //}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CCameraCompressionDemoDlg, CDialog)
   //{{AFX_MSG_MAP(CCameraCompressionDemoDlg)
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
   ON_BN_CLICKED(IDC_VIEW_OPTIONS, OnViewOptions)
   ON_BN_CLICKED(IDC_FILE_LOAD, OnFileLoad)
   ON_BN_CLICKED(IDC_FILE_SAVE, OnFileSave)
   ON_WM_ENDSESSION()
   ON_WM_QUERYENDSESSION()
   ON_BN_CLICKED(IDC_CHECK_JPEG_ENABLE, &CCameraCompressionDemoDlg::OnBnClickedCheckJpegEnable)
   ON_EN_KILLFOCUS(IDC_EDIT_JPEG_QUALITY, &CCameraCompressionDemoDlg::OnEnKillfocusEditImageCompressionQuality)
   //}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// CCameraCompressionDemoDlg message handlers

void CCameraCompressionDemoDlg::XferCallback(SapXferCallbackInfo* pInfo)
{
   CCameraCompressionDemoDlg* pDlg = (CCameraCompressionDemoDlg*)pInfo->GetContext();

   // If grabbing in trash buffer, do not display the image, update the
   // appropriate number of frames on the status bar instead
   if (pInfo->IsTrash())
   {
      // Update skipped frames count
      pDlg->m_Pro->m_StringSkippedFramesCount.Format(_T("Skipped frames count (processing queue full): %d"), pInfo->GetEventCount());

      // Display skipped frames count and incomplete frames count
      pDlg->m_statusWnd.SetWindowText(pDlg->m_Pro->m_StringSkippedFramesCount + _T(" ") + pDlg->m_Pro->m_StringIncompleteFrameCount);
   }
   else
   {
      // Converts to normal image in the view buffer
      // Can't use CycleNextEmpty and CycleNextWithTrash in conjunction with ExecuteNext, use CycleWithTrash instead.
      pDlg->m_Pro->ExecuteNext(); // Process next image (we do not want to miss a frame for statistics accuracy)
      pDlg->UpdateTitleBar();
   }
}

void CCameraCompressionDemoDlg::ProCallback(SapProCallbackInfo* pInfo)
{
   // While in ProCallback, you cannot access m_pBuffers information since it has been set as empty
   // and may already have been updated by a newer frame. If you need this info, put the code in
   // SapMyProcessing::Run() function.

   CCameraCompressionDemoDlg* pDlg = (CCameraCompressionDemoDlg*)pInfo->GetContext();

   pDlg->m_CompressedImageAvailable = pDlg->m_JpegProcessing ? pDlg->m_JpegProcessing->IsLastImageJpegDecodable() : FALSE;

   // Display last processed image
   pDlg->m_View->Show(pDlg->m_BuffersView->GetIndex());

   // Update string for incomplete frame count
   int incompleteFrameCount = pDlg->m_Pro->GetProIncompleteFrameCount();
   if (incompleteFrameCount > 0)
      pDlg->m_Pro->m_StringIncompleteFrameCount.Format(_T("   Incomplete buffer frames count: %d"), incompleteFrameCount);

   // Display Skipped frames count and incomplete frames count
   pDlg->m_statusWnd.SetWindowText(pDlg->m_Pro->m_StringSkippedFramesCount + pDlg->m_Pro->m_StringIncompleteFrameCount);

   // Display execution time

   CString str;
   str.Format(_T("%.2f"), pDlg->m_Pro->GetProTime());
   pDlg->GetDlgItem(IDC_EDIT_JPEG_DECODE_TIME)->SetWindowText(str);

   str.Format(_T("%.2f"), pDlg->m_Pro->GetProTimeAvg());
   pDlg->GetDlgItem(IDC_EDIT_JPEG_DECODE_TIME_AVG)->SetWindowText(str);

   int numFramesInCamera;
   pDlg->m_AcqDevice->GetFeatureValue("transferQueueCurrentBlockCount", &numFramesInCamera);
   str.Format(_T("%d"), numFramesInCamera);
   pDlg->GetDlgItem(IDC_EDIT_NUM_FRAMES_IN_CAMERA)->SetWindowText(str);

   int numImagesLostInCamera;
   pDlg->m_AcqDevice->GetFeatureValue("eventStatisticCount", &numImagesLostInCamera);
   str.Format(_T("%d"), numImagesLostInCamera);
   pDlg->GetDlgItem(IDC_EDIT_NUM_IMAGES_LOST_IN_CAMERA)->SetWindowText(str);

   float minSpaceUsedKB = (float)pDlg->m_Pro->GetProMinSpaceUsed() / 1024;
   float minSpaceUsedMB = minSpaceUsedKB / 1024;
   if ((int)minSpaceUsedMB > 0)
      str.Format(_T("%.01fMB"), minSpaceUsedMB);
   else
      str.Format(_T("%dKB"), (int)minSpaceUsedKB);
   pDlg->GetDlgItem(IDC_EDIT_MIN_SPACE_USED)->SetWindowText(str);

   float maxSpaceUsedKB = (float)pDlg->m_Pro->GetProMaxSpaceUsed() / 1024;
   float maxSpaceUsedMB = maxSpaceUsedKB / 1024;
   if ((int)maxSpaceUsedMB > 0)
      str.Format(_T("%.01fMB"), maxSpaceUsedMB);
   else
      str.Format(_T("%dKB"), (int)maxSpaceUsedKB);
   pDlg->GetDlgItem(IDC_EDIT_MAX_SPACE_USED)->SetWindowText(str);

   // Display frame's JPEG encoded size
   float curSpaceUsedKB = (float)pDlg->m_Pro->GetProCurSpaceUsed() / 1024;
   float curSpaceUsedMB = curSpaceUsedKB / 1024;
   if ((int)curSpaceUsedMB > 0)
      str.Format(_T("%.01fMB"), curSpaceUsedMB);
   else
      str.Format(_T("%dKB"), (int)curSpaceUsedKB);
   pDlg->GetDlgItem(IDC_EDIT_CURRENT_SPACE_USED)->SetWindowText(str);

   // Display data bandwidth
   if (pDlg->m_Pro->GetProBandwidth() == 0)
   {
      pDlg->GetDlgItem(IDC_EDIT_MB_PER_SECOND_AVG)->SetWindowText(_T(""));
      pDlg->GetDlgItem(IDC_EDIT_POSSIBLE_FRAMES_PER_SECOND)->SetWindowText(_T(""));
      pDlg->GetDlgItem(IDC_EDIT_CURRENT_FRAMES_PER_SECOND)->SetWindowText(_T(""));
   }
   else
   {
      float bandwidthAvgKB = pDlg->m_Pro->GetProBandwidthAvg() / 1024;
      float bandwidthAvgMB = bandwidthAvgKB / 1024;
      if ((int)bandwidthAvgMB > 0)
         str.Format(_T("%.01fMB/s"), (float)bandwidthAvgMB);
      else
         str.Format(_T("%dKB/s"), (int)bandwidthAvgKB);
      pDlg->GetDlgItem(IDC_EDIT_MB_PER_SECOND_AVG)->SetWindowText(str);

      str.Format(_T("%.01f"), 1000 / pDlg->m_Pro->GetProTimeAvg());
      pDlg->GetDlgItem(IDC_EDIT_POSSIBLE_FRAMES_PER_SECOND)->SetWindowText(str);

      str.Format(_T("%d"), pDlg->m_Pro->GetProFrameRate());
      pDlg->GetDlgItem(IDC_EDIT_CURRENT_FRAMES_PER_SECOND)->SetWindowText(str);
   }
}

void CCameraCompressionDemoDlg::ViewCallback(SapViewCallbackInfo* pInfo)
{
   CCameraCompressionDemoDlg* pContext = (CCameraCompressionDemoDlg*)pInfo->GetContext();
   if (pContext->m_temperatureLog)
   {
      pContext->m_temperatureLog->LogIfElapsed();
   }
}

//***********************************************************************************
// Initialize Demo Dialog based application
//***********************************************************************************
BOOL CCameraCompressionDemoDlg::OnInitDialog()
{
   CRect rect;

   CDialog::OnInitDialog();
   CDialog::SetDefID(IDC_EDIT_JPEG_QUALITY);

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
   SetIcon(m_hIcon, TRUE);    // Set big icon

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
   m_Buffers = new SapBufferWithTrash(4, m_AcqDevice);
   m_BuffersView = new SapBuffer(1); // w, h and format will be set in CreateObjects(TRUE); No need to have more than 1 view buffer
   m_JpegProcessing = new JpegProcessing(m_AcqDevice);
   m_Xfer = new SapAcqDeviceToBuf(m_AcqDevice, m_Buffers, XferCallback, this);
   m_Pro = new SapMyProcessing(m_Buffers, m_BuffersView, m_JpegProcessing, ProCallback, this);
   m_View = new SapView(m_BuffersView, m_viewWnd.GetSafeHwnd(), ViewCallback, this);

   // Create all objects
   if (!CreateObjects(TRUE))
   {
      EndDialog(TRUE);
      return FALSE;
   }

   m_JpegEnabled = m_JpegProcessing->IsImageCompressionModeJpeg();

   if (m_JpegEnabled && (m_Buffers->GetFormat() == SapFormatYUY2))
      UpdateViewBuffersViewFormat(SapFormatRGB8888);

   m_JpegEnableButton.SetCheck(m_JpegEnabled);
   m_EditImageCompressionQualityValue = m_JpegEnabled ? m_JpegProcessing->GetImageCompressionQuality() : 0;
   m_EditImageCompressionQuality.EnableWindow(TRUE);
   UpdateData(FALSE);

   UINT32 min = m_JpegProcessing->GetImageCompressionQualityMin();
   UINT32 max = m_JpegProcessing->GetImageCompressionQualityMax();
   m_SpinImageCompressionQuality.SetRange((short)min, (short)max);

   m_Pro->SetDecompressionEnabled(m_JpegEnabled);

   // Create an image window object
   m_ImageWnd = new CImageWnd(m_View, &m_viewWnd, &m_horizontalScr, &m_verticalScr, this);
   UpdateMenu();

   // We want each buffer frames be set to empty after processing
   m_Xfer->SetAutoEmpty(FALSE);
   m_Pro->SetAutoEmpty(TRUE);
   m_Pro->Init(); // Set SapProcessing index to current buffer index (required if buffer info changed)

   m_Pro->InitProTimeStatistics();
   m_Pro->InitProImageStatistics();

   if (m_TemperatureLogFilename[0])
   {
      m_temperatureLog = new TemperatureLog(m_TemperatureLogFilename, 1, m_AcqDevice);

      // First log to temperature log file:
      m_temperatureLog->Log();
   }

   return TRUE;  // return TRUE  unless you set the focus to a control
}

BOOL CCameraCompressionDemoDlg::CreateObjects(BOOL updateViewFormat, SapFormat viewFormat)
{
   CWaitCursor wait;

   // Create acquisition object
   if (m_AcqDevice && !*m_AcqDevice && !m_AcqDevice->Create())
   {
      DestroyObjects();
      return FALSE;
   }

   // Create the Jpeg processing object (uses the acq object).
   if (m_JpegProcessing && !*m_JpegProcessing && !m_JpegProcessing->Create())
   {
      DestroyObjects();
      return FALSE;
   }

   // Check the device supports compression. If not, tell the user and terminate.
   if (!m_JpegProcessing->IsJpegAvailable())
   {
      MessageBox(_T("This demo only supports image compression enabled devices."), _T("Incompatible device"), MB_ICONERROR);
      DestroyObjects();
      return FALSE;
   }

   // Create buffer object
   if (m_Buffers && !*m_Buffers)
   {
      if (!m_Buffers->Create())
      {
         DestroyObjects();
         return FALSE;
      }

      m_Buffers->Clear(); // Clear all buffers
   }

   if (updateViewFormat == TRUE)
   {
      SapFormat newViewFormat = viewFormat == SapFormatUnknown ? m_Buffers->GetFormat() : viewFormat;

      m_BuffersView->SetWidth(m_Buffers->GetWidth());
      m_BuffersView->SetHeight(m_Buffers->GetHeight());
      m_BuffersView->SetFormat(newViewFormat);
   }

   if (m_BuffersView && !*m_BuffersView)
   {
      if (!m_BuffersView->Create())
      {
         DestroyObjects();
         return FALSE;
      }

      m_BuffersView->Clear(); // Clear all buffers
   }

   // Create processing object
   if (m_Pro && !*m_Pro)
   {
      if (!m_Pro->Create())
      {
         DestroyObjects();
         return FALSE;
      }
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
      // Can't use CycleNextEmpty and CycleNextWithTrash in conjunction with ExecuteNext, use CycleWithTrash instead.
      if (!m_Xfer->GetPair(0)->SetCycleMode(SapXferPair::CycleWithTrash))
      {
         DestroyObjects();
         return FALSE;
      }
   }

   if (m_Xfer && !*m_Xfer && !m_Xfer->Create())
   {
      DestroyObjects();
      return FALSE;
   }

   return TRUE;
}

BOOL CCameraCompressionDemoDlg::DestroyObjects()
{
   // Destroy transfer object
   if (m_Xfer && *m_Xfer)
      m_Xfer->Destroy();

   // Destroy JPEG object
   if (m_JpegProcessing && *m_JpegProcessing)
      m_JpegProcessing->Destroy();

   // Destroy view object
   if (m_View && *m_View)
      m_View->Destroy();

   // Destroy buffer object
   if (m_Buffers && *m_Buffers)
      m_Buffers->Destroy();
   if (m_BuffersView && *m_BuffersView)
      m_BuffersView->Destroy();

   // Destroy acquisition object
   if (m_AcqDevice && *m_AcqDevice)
      m_AcqDevice->Destroy();

   return TRUE;
}

//**********************************************************************************
//
//       Window related functions
//
//**********************************************************************************
void CCameraCompressionDemoDlg::OnSysCommand(UINT nID, LPARAM lParam)
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
void CCameraCompressionDemoDlg::OnPaint()
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

      // Display last acquired image
      m_ImageWnd->OnPaint();
   }
}

void CCameraCompressionDemoDlg::OnDestroy()
{
   CDialog::OnDestroy();

   // Destroy all objects
   DestroyObjects();

   // Delete all objects
   if (m_Xfer)
      delete m_Xfer;
   if (m_ImageWnd)
      delete m_ImageWnd;
   if (m_View)
      delete m_View;
   if (m_Buffers)
      delete m_Buffers;
   if (m_BuffersView)
      delete m_BuffersView;
   if (m_AcqDevice)
      delete m_AcqDevice;
   if (m_JpegProcessing)
      delete m_JpegProcessing;
   if (m_Pro)
      delete m_Pro;
}

void CCameraCompressionDemoDlg::OnMouseMove(UINT nFlags, CPoint point)
{
   CString str = m_appTitle;

   if (!nFlags)
      str += "  " + m_ImageWnd->GetPixelString(point);

   SetWindowText(str);

   CDialog::OnMouseMove(nFlags, point);
}

void CCameraCompressionDemoDlg::OnMove(int x, int y)
{
   CDialog::OnMove(x, y);

   if (m_ImageWnd)
      m_ImageWnd->OnMove();
}


void CCameraCompressionDemoDlg::OnSize(UINT nType, int cx, int cy)
{
   CDialog::OnSize(nType, cx, cy);

   if (m_ImageWnd)
      m_ImageWnd->OnSize();
}


void CCameraCompressionDemoDlg::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
   if (pScrollBar->GetDlgCtrlID() == IDC_HORZ_SCROLLBAR)
   {
      // Adjust source's horizontal origin
      m_ImageWnd->OnHScroll(nSBCode, nPos);
      OnPaint();
   }

   CDialog::OnHScroll(nSBCode, nPos, pScrollBar);
}

void CCameraCompressionDemoDlg::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
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
HCURSOR CCameraCompressionDemoDlg::OnQueryDragIcon()
{
   return (HCURSOR)m_hIcon;
}


void CCameraCompressionDemoDlg::OnEndSession(BOOL bEnding)
{
   CDialog::OnEndSession(bEnding);

   if (bEnding)
   {
      // If ending the session, free the resources.
      OnDestroy();
   }
}

BOOL CCameraCompressionDemoDlg::OnQueryEndSession()
{
   if (!CDialog::OnQueryEndSession())
      return FALSE;

   return TRUE;
}

//**************************************************************************************
// Updates the menu items enabling/disabling the proper items depending on the state
//  of the application
//**************************************************************************************
void CCameraCompressionDemoDlg::UpdateMenu(void)
{
   BOOL bAcqNoGrab = m_Xfer && *m_Xfer && !m_Xfer->IsGrabbing();
   BOOL bAcqGrab = m_Xfer && *m_Xfer && m_Xfer->IsGrabbing();
   BOOL bNoGrab = !m_Xfer || !m_Xfer->IsGrabbing();

   // Acquisition Control
   GetDlgItem(IDC_GRAB)->EnableWindow(bAcqNoGrab);
   GetDlgItem(IDC_SNAP)->EnableWindow(bAcqNoGrab);
   GetDlgItem(IDC_FREEZE)->EnableWindow(bAcqGrab);

   // Acquisition Options
   GetDlgItem(IDC_LOAD_ACQ_CONFIG)->EnableWindow(m_Xfer && !m_Xfer->IsGrabbing());

   // File Options
   GetDlgItem(IDC_FILE_LOAD)->EnableWindow(bNoGrab);
   GetDlgItem(IDC_FILE_SAVE)->EnableWindow(bNoGrab);

   // General Options
   GetDlgItem(IDC_BUFFER_OPTIONS)->EnableWindow(bNoGrab);

   // Processing
   GetDlgItem(IDC_CHECK_JPEG_ENABLE)->EnableWindow(bNoGrab && m_JpegPresent);
   m_EditImageCompressionQuality.EnableWindow(bNoGrab && m_JpegEnabled && m_JpegPresent);

   // If last control was disabled, set default focus
   if (!GetFocus())
      GetDlgItem(IDC_VIEW_OPTIONS)->SetFocus();
}

void CCameraCompressionDemoDlg::UpdateTitleBar()
{
   // Update pixel information
   CString str = m_appTitle;
   CPoint point;
   GetCursorPos(&point);
   ScreenToClient(&point);
   str += "  " + m_ImageWnd->GetPixelString(point);

   SetWindowText(str);
}

void CCameraCompressionDemoDlg::UpdateViewBuffersViewFormat(SapFormat viewFormat)
{
   m_View->Destroy();
   m_BuffersView->Destroy();

   m_BuffersView->SetFormat(viewFormat);

   m_BuffersView->Create();
   m_View->Create();

   m_BuffersView->Clear();
}


//*****************************************************************************************
//
//       Acquisition Control
//
//*****************************************************************************************

void CCameraCompressionDemoDlg::OnFreeze()
{
   if (m_Xfer->Freeze())
   {
      if (CAbortDlg(this, m_Xfer).DoModal() != IDOK)
         m_Xfer->Abort();

      UpdateMenu();
   }
}

void CCameraCompressionDemoDlg::OnGrab()
{
   m_statusWnd.SetWindowText(_T(""));

   if (m_Xfer->Grab())
   {
      m_Pro->InitProTimeStatistics();
      m_Pro->InitProImageStatistics();
      UpdateMenu();
   }
}

void CCameraCompressionDemoDlg::OnSnap()
{
   m_statusWnd.SetWindowText(_T(""));

   m_Pro->InitProImageStatistics();

   if (m_Xfer->Snap())
   {
      if (CAbortDlg(this, m_Xfer).DoModal() != IDOK)
         m_Xfer->Abort();

      UpdateMenu();
   }
}


//*****************************************************************************************
//
//       Acquisition Options
//
//*****************************************************************************************
void CCameraCompressionDemoDlg::OnLoadAcqConfig()
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
         // Restore backup
         m_AcqDevice->SetLocation(loc);
         m_AcqDevice->SetConfigFile(configFile);
         CreateObjects();
      }

      m_ImageWnd->OnSize();
      InvalidateRect(NULL);
      UpdateWindow();
      UpdateMenu();
   }
   else
   {
      if (m_Xfer)
         m_Xfer->Connect();
   }
}

//*****************************************************************************************
//
//       General Options
//
//*****************************************************************************************

void CCameraCompressionDemoDlg::OnBufferOptions()
{
   CBufDlg dlg(this, m_Buffers, m_View->GetDisplay());
   if (dlg.DoModal() == IDOK)
   {
      // Destroy objects
      DestroyObjects();

      //
      // Update grab buffer, view buffer, transfer, processing and view objects
      //

      SapBuffer oldBuf = *m_Buffers;
      *m_Buffers = dlg.GetBuffer();

      // Delete image window object
      delete m_ImageWnd;

      // Recreate objects
      if (!CreateObjects())
      {
         *m_Buffers = oldBuf;

         CreateObjects();
      }
      else
         m_CompressedImageAvailable = FALSE;

      m_Pro->Init(); // Set SapProcessing index to current buffer index (required if buffer info changed)

      // Re-create an image window object
      m_ImageWnd = new CImageWnd(m_View, &m_viewWnd, &m_horizontalScr, &m_verticalScr, this);

      m_ImageWnd->OnSize();
      InvalidateRect(NULL);
      UpdateWindow();
      UpdateMenu();
   }
}

void CCameraCompressionDemoDlg::OnViewOptions()
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
//       File Options
//
//*****************************************************************************************

void CCameraCompressionDemoDlg::OnFileLoad()
{
   CLoadSaveDlg dlg(this, m_BuffersView, TRUE);
   m_CompressedImageAvailable = FALSE;
   if (dlg.DoModal() == IDOK)
   {
      InvalidateRect(NULL);
      UpdateWindow();
   }
}

void CCameraCompressionDemoDlg::OnFileSave()
{

   if (m_CompressedImageAvailable)
   {
      // If JPG is selected, must not show the Quality dialog (data already encoded by the hardware),
      // so use this overloaded class:
      CLoadSaveHwJpegDlg dlg(this, m_BuffersView, FALSE, 0);
      dlg.DoModal();
      if (dlg.GetType() == 4)
      {
         // If "Jpeg" was selected, nothing will have been saved
         // since we must save as a raw with the .jpg extension

         CString pathName = dlg.GetPathName();

         int dataPrmSize;
         if (m_Buffers->GetSpaceUsed(&dataPrmSize))
         {
            // Allocate temp buffer for saving
            SapBuffer saveJpegBuffers(1, dataPrmSize, 1);
            saveJpegBuffers.Create();

            unsigned char* saveJpegBuffer;
            saveJpegBuffers.GetAddress((void**)&saveJpegBuffer);

            unsigned char* bufferAddress;
            m_Buffers->GetAddress((void**)&bufferAddress);

            memcpy_s(saveJpegBuffer, dataPrmSize, bufferAddress, dataPrmSize);

            saveJpegBuffers.Save(CStringA(pathName), "-format raw");
         }
         else
         {
            // Save whole buffer
            m_Buffers->Save(CStringA(pathName), "-format raw");
         }
      }
   }
   else
   {
      CLoadSaveDlg dlg(this, m_BuffersView, FALSE);
      dlg.DoModal();
   }
}

void CCameraCompressionDemoDlg::OnBnClickedCheckJpegEnable()
{
   HCURSOR hNormal = GetCursor();
   HCURSOR hBusy = LoadCursor(NULL, IDC_WAIT);
   SetCursor(hBusy);

   m_JpegEnabled = !m_JpegProcessing->IsImageCompressionModeJpeg();
   m_JpegEnableButton.SetCheck(m_JpegEnabled);

   // Disconnect the transfer (in order to enable/disable Jpeg).
   if (m_Xfer)
      m_Xfer->Disconnect();

   // Initialize acquisition and processing
   m_Pro->SetDecompressionEnabled(m_JpegEnabled);

   // Force repaint of display
   m_ImageWnd->Invalidate(FALSE);

   // Enable/disable the Jpeg procesing. (Xfer must be disconnected).
   BOOL pixelFormatChanged = FALSE;
   m_JpegProcessing->EnableJpegProcessing(m_JpegEnabled, pixelFormatChanged);
   if (pixelFormatChanged)
   {
      // Destroy objects
      DestroyObjects();

      // Update object
      m_AcqDevice->SetConfigFile(NULL);

      // Recreate objects
      CreateObjects();

      m_CompressedImageAvailable = FALSE;

      m_ImageWnd->OnSize();
      InvalidateRect(NULL);
      UpdateWindow();
      UpdateMenu();
   }

   if (m_JpegEnabled)
      m_EditImageCompressionQualityValue = m_JpegProcessing->GetImageCompressionQuality();
   else
      m_EditImageCompressionQualityValue = 0;
   UpdateData(FALSE);

   SapFormat newViewFormat = SapFormatUnknown;
   SapFormat acqFormat = m_Buffers->GetFormat();
   if (m_JpegEnabled)
   {
      // If the acq buffer is YUY2, color JPEG decompression can only be done to RGB8888, so
      // we change it to this (one less conversion to make)
      if (acqFormat == SapFormatYUY2)
         newViewFormat = SapFormatRGB8888;
   }
   else
   {
      // If the acq buffer format is different from the view buffer, make it the same
      if (m_BuffersView->GetFormat() != acqFormat)
         newViewFormat = acqFormat;
   }

   if (newViewFormat != SapFormatUnknown)
   {
      delete m_ImageWnd; // Delete image window object

      UpdateViewBuffersViewFormat(newViewFormat);

      // Re-create an image window object
      m_ImageWnd = new CImageWnd(m_View, &m_viewWnd, &m_horizontalScr, &m_verticalScr, this);

      m_ImageWnd->OnSize();
      InvalidateRect(NULL);
      UpdateWindow();
      UpdateMenu();
   }

   // Force repaint of display
   m_ImageWnd->Invalidate(FALSE);

   if (m_Xfer)
      m_Xfer->Connect();

   m_Pro->InitProTimeStatistics();

   UpdateMenu();
   SetCursor(hNormal);
}

void CCameraCompressionDemoDlg::OnEnKillfocusEditImageCompressionQuality()
{
   UINT32 imageCompressionQuality;
   HCURSOR hNormal = GetCursor();
   HCURSOR hBusy = LoadCursor(NULL, IDC_WAIT);
   SetCursor(hBusy);

   UpdateData(TRUE);

   imageCompressionQuality = m_EditImageCompressionQualityValue;
   m_JpegProcessing->SetImageCompressionQuality(imageCompressionQuality);

   // Update the dialog.
   m_EditImageCompressionQualityValue = m_JpegProcessing->GetImageCompressionQuality();

   UpdateData(FALSE);
   UpdateMenu();

   SetCursor(hNormal);
}
