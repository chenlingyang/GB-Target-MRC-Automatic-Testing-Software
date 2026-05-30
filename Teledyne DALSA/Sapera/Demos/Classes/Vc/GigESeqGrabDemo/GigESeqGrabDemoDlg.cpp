//==============================================================================
// Module : GigESeqGrabDemoDlg.cpp
// Project: Sapera samples.
// Purpose: This module contains the methods that define the CGigESeqGrabDemoDlg class.
//==============================================================================
// Include files.

#include "stdafx.h"
#include "GigESeqGrabDemo.h"
#include "GigESeqGrabDemoDlg.h"
#include "GigESeqGrabDemoAbout.h"
#include "float.h"

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::CGigESeqGrabDemoDlg
// Purpose   : Class constructor.
// Parameters:
//    pCParent                Parent window.
//==============================================================================
CGigESeqGrabDemoDlg::CGigESeqGrabDemoDlg(CWnd* pParent) : CDialog(CGigESeqGrabDemoDlg::IDD, pParent)
{
   //{{AFX_DATA_INIT(CGigESeqGrabDemoDlg)
   m_LiveFrameRate = _T("");
   m_BufferFrameRate = 0.0f;
   m_MaxTime = 0.0f;
   m_MinTime = 0.0f;
   m_ActiveBuffer = 0;
   m_BufferCount = 0;
   m_Slider = 0;
   m_TimestampCurrent = _T("");
   m_TimestampBuffer = 0;
   m_TimestampBufferDelta = 0;
   //}}AFX_DATA_INIT

   // Note that LoadIcon does not require a subsequent DestroyIcon in Win32
   m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
   m_AcqDevice = NULL;
   m_Buffers = NULL;
   m_Xfer = NULL;
   m_View = NULL;

   m_IsSignalDetected = TRUE;

   m_bRecordOn = FALSE;
   m_bPlayOn = FALSE;
   m_bPauseOn = FALSE;

   m_nFramesPerCallback = 1;

} // End of the CGigESeqGrabDemoDlg::CGigESeqGrabDemoDlg method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::DoDataExchange
// Purpose   : Data exchange.
// Parameters:
//    pDX
//==============================================================================
void CGigESeqGrabDemoDlg::DoDataExchange(CDataExchange* pDX)
{
   CDialog::DoDataExchange(pDX);
   //{{AFX_DATA_MAP(CGigESeqGrabDemoDlg)
   DDX_Control(pDX, IDC_STOP, m_stopButton);
   DDX_Control(pDX, IDC_PAUSE, m_pauseButton);
   DDX_Control(pDX, IDC_PLAY, m_playButton);
   DDX_Control(pDX, IDC_RECORD, m_recordButton);
   DDX_Control(pDX, IDC_SLIDER, m_SliderCtrl);
   DDX_Control(pDX, IDC_VIEW_WND, m_ImageWnd);
   DDX_Text(pDX, IDC_LIVE_FRAME_RATE, m_LiveFrameRate);
   DDX_Text(pDX, IDC_BUFFER_FRAME_RATE, m_BufferFrameRate);
   DDV_MinMaxFloat(pDX, m_BufferFrameRate, 1.e-3f, 1.e6f);
   DDX_Text(pDX, IDC_MAX_TIME, m_MaxTime);
   DDX_Text(pDX, IDC_MIN_TIME, m_MinTime);
   DDX_Text(pDX, IDC_ACTIVE, m_ActiveBuffer);
   DDX_Text(pDX, IDC_AVAILABLE, m_BufferCount);
   DDX_Slider(pDX, IDC_SLIDER, m_Slider);
   DDX_Text(pDX, IDC_TIMESTAMP_CURRENT, m_TimestampCurrent);
   DDX_Text(pDX, IDC_TIMESTAMP_BUFFER, m_TimestampBuffer);
   DDX_Text(pDX, IDC_TIMESTAMP_BUFFER_DELTA, m_TimestampBufferDelta);
   //}}AFX_DATA_MAP
} // End of the CGigESeqGrabDemoDlg::DoDataExchange method.

//==============================================================================
BEGIN_MESSAGE_MAP(CGigESeqGrabDemoDlg, CDialog)
   //{{AFX_MSG_MAP(CGigESeqGrabDemoDlg)
   ON_WM_SYSCOMMAND()
   ON_WM_PAINT()
   ON_WM_QUERYDRAGICON()
   ON_WM_DESTROY()
   ON_WM_SIZE()
   ON_WM_HSCROLL()
   ON_WM_TIMER()
   ON_BN_CLICKED(IDC_RECORD, OnRecord)
   ON_BN_CLICKED(IDC_PLAY, OnPlay)
   ON_BN_CLICKED(IDC_PAUSE, OnPause)
   ON_BN_CLICKED(IDC_STOP, OnStop)
   ON_BN_CLICKED(IDC_BUFFER_OPTIONS, OnBufferOptions)
   ON_BN_CLICKED(IDC_LOAD_ACQ_CONFIG, OnLoadAcqDeviceConfig)
   ON_BN_CLICKED(IDC_FILE_LOAD, OnFileLoad)
   ON_BN_CLICKED(IDC_FILE_SAVE, OnFileSave)
   ON_EN_KILLFOCUS(IDC_BUFFER_FRAME_RATE, OnKillfocusBufferFrameRate)
   ON_BN_CLICKED(IDC_FILE_LOAD_CURRENT, OnFileLoadCurrent)
   ON_BN_CLICKED(IDC_FILE_SAVE_CURRENT, OnFileSaveCurrent)
   ON_MESSAGE(WM_UPDATE_CONTROLS, OnUpdateControls)
   ON_BN_CLICKED(IDC_HIGH_FRAME_RATE, OnHighFrameRate)
   ON_WM_ENDSESSION()
   ON_WM_QUERYENDSESSION()
   ON_BN_CLICKED(IDC_CAMERA_TIMESTAMP_UPDATE, OnBnClickedCameraTimestampUpdate)
   ON_BN_CLICKED(IDC_CAMERA_TIMESTAMP_RESET, OnBnClickedCameraTimestampReset)
   //}}AFX_MSG_MAP
END_MESSAGE_MAP()

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::XferCallback
// Purpose   : Callback function for the transfer.
// Parameters: None
//==============================================================================
void CGigESeqGrabDemoDlg::XferCallback(SapXferCallbackInfo *pInfo)
{
   CGigESeqGrabDemoDlg *pDlg = (CGigESeqGrabDemoDlg *)pInfo->GetContext();

   // Measure real frame time
   pDlg->UpdateFrameRate();

   // Check if last frame is reached
   pDlg->CheckForLastFrame();

   // Refresh view
   pDlg->m_View->Show();

   // Refresh controls
   pDlg->PostMessage(WM_UPDATE_CONTROLS, 0, 0);

} // End of the CGigESeqGrabDemoDlg::XferCallback method.

void CGigESeqGrabDemoDlg::PixelChanged(int x, int y)
{
   CString str = m_appTitle;
   str += "  " + m_ImageWnd.GetPixelString(CPoint(x, y));
   SetWindowText(str);
}

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnInitDialog
// Purpose   : Dialog initialization.
// Parameters: None
//==============================================================================
BOOL CGigESeqGrabDemoDlg::OnInitDialog(void)
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
      } // End if.

      pSysMenu->EnableMenuItem(SC_MAXIMIZE, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
      pSysMenu->EnableMenuItem(SC_SIZE, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
   } // End if.

   // Set the icon for this dialog.  The framework does this automatically
   //  when the application's main window is not a dialog
   SetIcon(m_hIcon, TRUE);       // Set big icon
   SetIcon(m_hIcon, FALSE);      // Set small icon

   // Initialize variables
   GetWindowText(m_appTitle);

   // Are we operating on-line?
   CAcqConfigDlg dlg(this, CAcqConfigDlg::ServerAcqDevice);
   if (dlg.DoModal() != IDOK)
   {
      MessageBox(_T("No cameras found or selected"));
      EndDialog(TRUE);
      return FALSE;
   }

   // Define objects
   m_AcqDevice = new SapAcqDevice(dlg.GetLocation(), dlg.GetConfigFile());
   m_Buffers = new SapBufferWithTrash(MAX_BUFFER, m_AcqDevice);
   m_Xfer = new SapAcqDeviceToBuf(m_AcqDevice, m_Buffers, XferCallback, this);
   m_View = new SapView(m_Buffers);

   // Attach sapview to image viewer
   m_ImageWnd.AttachSapView(m_View);

   // Create all objects
   if (!CreateObjects())
   {
      EndDialog(TRUE);
      return FALSE;
   }

   ReadCameraTimestamp();

   // Create image window
   m_ImageWnd.AttachEventHandler(this);
   m_ImageWnd.Reset();

   UpdateMenu();

   return TRUE;  // return TRUE  unless you set the focus to a control
} // End of the CGigESeqGrabDemoDlg::OnInitDialog method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::CreateObjects
// Purpose   : Create sapera objects.
// Parameters: None
//==============================================================================
BOOL CGigESeqGrabDemoDlg::CreateObjects(void)
{
   //Number of frames per callback retreived
   int nFramesPerCallback;

   // Create acquisition object
   if (m_AcqDevice && !*m_AcqDevice && !m_AcqDevice->Create())
   {
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
      // Clear all buffers
      m_Buffers->Clear();
   }

   // Create view object
   if (m_View && !*m_View && !m_View->Create())
   {
      DestroyObjects();
      return FALSE;
   }

   if (m_Xfer && !*m_Xfer)
   {

      // Set number of frames per callback
      m_Xfer->GetPair(0)->SetFramesPerCallback(m_nFramesPerCallback);


      // If there is a large number of buffers, temporarily boost the command timeout value,
      // since the call to Create may take a long time to complete.
      // As a safe rule of thumb, use 100 milliseconds per buffer.
      int oldCommandTimeout = SapManager::GetCommandTimeout();
      int newCommandTimeout = 100 * m_Buffers->GetCount();

      if (newCommandTimeout < oldCommandTimeout)
         newCommandTimeout = oldCommandTimeout;

      SapManager::SetCommandTimeout(newCommandTimeout);

      // Create transfer object
      if (!m_Xfer->Create())
      {
         DestroyObjects();
         return FALSE;
      }

      // Restore original command timeout value
      SapManager::SetCommandTimeout(oldCommandTimeout);

      m_Xfer->Init(TRUE); // initialize tranfer object and reset source/destination index

      // Retrieve number of frames per callback
      // It may be less than what we have asked for.
      nFramesPerCallback = m_Xfer->GetPair(0)->GetFramesPerCallback();
      if (m_nFramesPerCallback > nFramesPerCallback)
      {
         m_nFramesPerCallback = nFramesPerCallback;
         AfxMessageBox(_T("No memory"));
      }

   }

   return TRUE;
} // End of the CGigESeqGrabDemoDlg::CreateObjects method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::DestryObjects
// Purpose   : Destroy sapera objects.
// Parameters: None
//==============================================================================
BOOL CGigESeqGrabDemoDlg::DestroyObjects(void)
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
} // End of the CGigESeqGrabDemoDlg::DestryObjects method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::DestryObjects
// Purpose   : Handle commands from system, only handles about box.
// Parameters:
//    nID
//    lParam
//==============================================================================
void CGigESeqGrabDemoDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
   if ((nID & 0xFFF0) == IDM_ABOUTBOX)
   {
      CAboutDlg dlgAbout;
      dlgAbout.DoModal();
   }
   else
   {
      CDialog::OnSysCommand(nID, lParam);
   } // End if, else.
} // End of the CGigESeqGrabDemoDlg::OnSysCommand method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnPaint
// Purpose   : If you add a minimize button to your dialog, you will need the code
//             below to draw the icon.
// Parameters: None
//==============================================================================
void CGigESeqGrabDemoDlg::OnPaint(void)
{
   if (IsIconic())
   {
      CPaintDC dc(this); // device context for painting

      SendMessage(WM_ICONERASEBKGND, (WPARAM)dc.GetSafeHdc(), 0);

      // Center icon in client rectangle
      int cxIcon = GetSystemMetrics(SM_CXICON);
      int cyIcon = GetSystemMetrics(SM_CYICON);
      CRect rect;
      GetClientRect(&rect);
      int x = (rect.Width() - cxIcon + 1) / 2;
      int y = (rect.Height() - cyIcon + 1) / 2;

      // Draw the icon
      dc.DrawIcon(x, y, m_hIcon);
   }
   else
   {
      CDialog::OnPaint();
   }
} // End of the CGigESeqGrabDemoDlg::OnPaint method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnQueryDragIcon
// Purpose   : The system calls this to obtain the cursor to display while the
//             user drags the minimized window.
// Parameters: None
//==============================================================================
HCURSOR CGigESeqGrabDemoDlg::OnQueryDragIcon(void)
{
   return (HCURSOR)m_hIcon;
} // End of the CGigESeqGrabDemoDlg::OnQueryDragIcon method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnDestroy
// Purpose   : Handle the destroy message.
// Parameters: None
//==============================================================================
void CGigESeqGrabDemoDlg::OnDestroy(void)
{
   CDialog::OnDestroy();

   // Destroy all objects
   DestroyObjects();

   // Delete all objects
   if (m_Xfer)       delete m_Xfer;
   if (m_View)       delete m_View;
   if (m_Buffers)    delete m_Buffers;
   if (m_AcqDevice)  delete m_AcqDevice;
} // End of the CGigESeqGrabDemoDlg::OnDestroy method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnSize
// Purpose   : Handle the size event.
// Parameters:
//    cx                      New X size.
//    cy                      New Y size.
//==============================================================================
void CGigESeqGrabDemoDlg::OnSize(UINT nType, int cx, int cy)
{
   CDialog::OnSize(nType, cx, cy);

   CRect rClient;
   GetClientRect(rClient);

   // resize slider
   if (m_SliderCtrl.GetSafeHwnd())
   {
      CRect rWnd;
      m_SliderCtrl.GetWindowRect(rWnd);
      ScreenToClient(rWnd);
      rWnd.right = rClient.right - 5;
      m_SliderCtrl.MoveWindow(rWnd);
   }

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

} // End of the CGigESeqGrabDemoDlg::OnSize method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnHScroll
// Purpose   : Handle the horizontal scroll bar events.
// Parameters:
//    nSBCode                 Scroll bar code.
//    nPos                    New scroll bar position.
//    pScrollBar              Scroll bar object.
//==============================================================================
void CGigESeqGrabDemoDlg::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
   if (pScrollBar->GetDlgCtrlID() == IDC_SLIDER)
   {
      // Get slider position
      UpdateData(TRUE);

      // Update buffer index
      m_Buffers->SetIndex(m_Slider);

      // Refresh controls
      OnUpdateControls(0, 0);

      // Resfresh display
      m_View->Show();
   }

   CDialog::OnHScroll(nSBCode, nPos, pScrollBar);
} // End of the CGigESeqGrabDemoDlg::OnHScroll method.

void CGigESeqGrabDemoDlg::OnEndSession(BOOL bEnding)
{
   CDialog::OnEndSession(bEnding);

   if (bEnding)
   {
      // If ending the session, free the resources.
      OnDestroy();
   }
}

BOOL CGigESeqGrabDemoDlg::OnQueryEndSession()
{
   if (!CDialog::OnQueryEndSession())
      return FALSE;

   return TRUE;
}

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnKillfocusBufferFrameRate
// Purpose   : Handle the
// Parameters: None
//==============================================================================
void CGigESeqGrabDemoDlg::OnKillfocusBufferFrameRate(void)
{
   UpdateData(TRUE);
   m_Buffers->SetFrameRate(m_BufferFrameRate);
} // End of the CGigESeqGrabDemoDlg::OnKillfocusFrameRate method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::UpdateMenu
// Purpose   : Updates the menu items enabling/disabling the proper items
//             depending on the state of the application.
// Parameters: None
//==============================================================================
void CGigESeqGrabDemoDlg::UpdateMenu(void)
{
   BOOL bAcqNoGrab = m_Xfer && *m_Xfer && !m_bRecordOn && !m_bPlayOn;
   BOOL bNoGrab = !m_bRecordOn && !m_bPlayOn;

   // Record Control
   m_recordButton.EnableWindow(bAcqNoGrab);
   m_playButton.EnableWindow(bNoGrab);
   m_pauseButton.EnableWindow(!bNoGrab);
   m_stopButton.EnableWindow(!bNoGrab);

   m_pauseButton.SetWindowText(m_bPauseOn ? _T("Continue") : _T("Pause"));

   // General Options
   GetDlgItem(IDC_BUFFER_OPTIONS)->EnableWindow(bNoGrab);
   GetDlgItem(IDC_LOAD_CAM_VIC)->EnableWindow(m_Xfer && bNoGrab);
   GetDlgItem(IDC_HIGH_FRAME_RATE)->EnableWindow(m_Xfer && bNoGrab);

   // File Options
   GetDlgItem(IDC_FILE_LOAD)->EnableWindow(bNoGrab);
   GetDlgItem(IDC_FILE_SAVE)->EnableWindow(bNoGrab);

   // Slider
   m_SliderCtrl.EnableWindow(bNoGrab || (m_bPlayOn && m_bPauseOn));
   m_SliderCtrl.SetRange(0, m_Buffers->GetCount() - 1, TRUE);

   // If last control was disabled, set default focus
   if (!GetFocus())
      GetDlgItem(IDC_BUFFER_OPTIONS)->SetFocus();

   // Update control values
   PostMessage(WM_UPDATE_CONTROLS, 0, 0);
} // End of the CGigESeqGrabDemoDlg::UpdateMenu method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::UpdateFrameRate
// Purpose   : Calculate statistics on frame rate.
// Parameters: None
//==============================================================================
void CGigESeqGrabDemoDlg::UpdateFrameRate(void)
{
   if (m_Xfer->UpdateFrameRateStatistics())
   {
      SapXferFrameRateInfo* pStats = m_Xfer->GetFrameRateStatistics();

      if (pStats->IsLiveFrameRateAvailable() && !pStats->IsLiveFrameRateStalled())
      {
         CString sLiveFrameRate;
         sLiveFrameRate.Format(_T("%.1f"), pStats->GetLiveFrameRate());
         m_LiveFrameRate = sLiveFrameRate;
      }
      else
      {
         m_LiveFrameRate = _T("N/A");
      }

      m_BufferFrameRate = pStats->GetBufferFrameRate();
      m_MinTime = pStats->GetMinTimePerFrame();
      m_MaxTime = pStats->GetMaxTimePerFrame();

      if (pStats->IsBufferFrameRateAvailable())
         m_Buffers->SetFrameRate(m_BufferFrameRate);
      else
         m_Buffers->SetFrameRate(pStats->GetLiveFrameRate());
   }
} // End of the CGigESeqGrabDemoDlg::UpdateFrameRate method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::CheckForLastFrame
// Purpose   : Check if the last frame needed has been acquired.
// Parameters: None
//==============================================================================
void CGigESeqGrabDemoDlg::CheckForLastFrame(void)
{
   // Check for last frame
   if (m_Buffers->GetIndex() == m_Buffers->GetCount() - 1)
   {
      if (m_bRecordOn)
      {
         m_bRecordOn = FALSE;
         KillTimer(1);
      }
      if (m_bPlayOn)
      {
         m_bPlayOn = FALSE;
         KillTimer(1);
      }
      UpdateMenu();
   } // End if.
} // End of the CGigESeqGrabDemoDlg::CheckForLastFrame method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnTimer
// Purpose   :
// Parameters:
//    nIDEvent
//==============================================================================
void CGigESeqGrabDemoDlg::OnTimer(UINT_PTR nIDEvent)
{
   if (nIDEvent == 1)
   {
      // Increase buffer index
      m_Buffers->Next();

      // Resfresh display
      m_View->Show();

      // Check if last frame is reached
      CheckForLastFrame();

      // Refresh controls
      PostMessage(WM_UPDATE_CONTROLS, 0, 0);
   } // End if.

   CDialog::OnTimer(nIDEvent);
} // End of the CGigESeqGrabDemoDlg::OnTimer method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnUpdateControls
// Purpose   : Update the control values.
// Parameters: None
//==============================================================================
LRESULT CGigESeqGrabDemoDlg::OnUpdateControls(WPARAM, LPARAM)
{
   // Update edit controls
   m_ActiveBuffer = m_Buffers->GetIndex() + 1;
   m_BufferCount = m_Buffers->GetCount();
   m_Slider = m_Buffers->GetIndex();

   BOOL bNoGrab = !m_bRecordOn && !m_bPlayOn;
   if (bNoGrab)
      m_LiveFrameRate = _T("N/A");

   m_BufferFrameRate = m_Buffers->GetFrameRate();

   m_Buffers->GetDeviceTimeStamp(&m_TimestampBuffer);
   if (m_Buffers->GetIndex())
   {
      UINT64 timestampBufferPrevious = 0;
      m_Buffers->GetDeviceTimeStamp(m_Buffers->GetIndex() - 1, &timestampBufferPrevious);
      m_TimestampBufferDelta = m_TimestampBuffer - timestampBufferPrevious;
   }
   else
      m_TimestampBufferDelta = 0;

   UpdateData(FALSE);
   return 0;
} // End of the CGigESeqGrabDemoDlg::OnUpdateControls method.

//*****************************************************************************************
//
//             Record Control
//
//*****************************************************************************************

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnRecord
// Purpose   : Start recording frames.
// Parameters: None
//==============================================================================
void CGigESeqGrabDemoDlg::OnRecord(void)
{
   // Reset source and destination indices
   m_Xfer->Init();

   // Reset the frame rate statistics ahead of each transfer stream
   SapXferFrameRateInfo* pStats = m_Xfer->GetFrameRateStatistics();
   pStats->Reset();

   // Acquire all frames
   if (m_Xfer->Snap(m_Buffers->GetCount()))
   {
      m_bRecordOn = TRUE;
      UpdateMenu();
   }
} // End of the CGigESeqGrabDemoDlg::OnRecord method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnPlay
// Purpose   : Play back the frames.
// Parameters: None
//==============================================================================
void CGigESeqGrabDemoDlg::OnPlay(void)
{
   // Initialize buffer index
   m_Buffers->SetIndex(0);

   // Start playback timer
   int frameTime = max(1, (int)(1000.0 / m_Buffers->GetFrameRate()));
   SetTimer(1, frameTime, NULL);

   m_bPlayOn = TRUE;
   UpdateMenu();
} // End of the CGigESeqGrabDemoDlg::OnPlay method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnPause
// Purpose   : Pause the recording or playing of frames.
// Parameters: None
//==============================================================================
void CGigESeqGrabDemoDlg::OnPause(void)
{
   if (!m_bPauseOn)
   {
      // Check if recording or playing
      if (m_bRecordOn)
      {
         KillTimer(1);
         // Stop current acquisition
         if (!m_Xfer->Freeze())
            return;

         if (CAbortDlg(this, m_Xfer).DoModal() != IDOK)
            m_Xfer->Abort();
      }
      else if (m_bPlayOn)
      {
         // Stop playback timer
         KillTimer(1);
      } // End if, else if.
   }
   else
   {
      // Check if recording or playing
      if (m_bRecordOn)
      {
         int frameTime = (int)(1000.0 / m_Buffers->GetFrameRate());
         SetTimer(1, frameTime, NULL);
         // Acquire remaining frames
         if (!m_Xfer->Snap(m_Buffers->GetCount() - m_Buffers->GetIndex() - 1))
            return;
      }
      else if (m_bPlayOn)
      {
         // Restart playback timer
         int frameTime = (int)(1000.0 / m_Buffers->GetFrameRate());
         SetTimer(1, frameTime, NULL);
      } // End if, else if.
   } // End if, else.

   m_bPauseOn = !m_bPauseOn;
   UpdateMenu();
} // End of the CGigESeqGrabDemoDlg::OnPause method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnStop
// Purpose   : Stop the recording or playing of frames.
// Parameters: None
//==============================================================================
void CGigESeqGrabDemoDlg::OnStop(void)
{
   // Check if recording or playing
   if (m_bRecordOn)
   {
      // Stop current acquisition
      if (!m_Xfer->Freeze())
         return;

      if (CAbortDlg(this, m_Xfer).DoModal() != IDOK)
         m_Xfer->Abort();

      m_bRecordOn = FALSE;
   }
   else if (m_bPlayOn)
   {
      // Stop playback timer
      KillTimer(1);
      m_bPlayOn = FALSE;
   } // End if, else if.

   m_bPauseOn = FALSE;
   UpdateMenu();
} // End of the CGigESeqGrabDemoDlg::OnStop method.

//*****************************************************************************************
//
//             General Options
//
//*****************************************************************************************

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnBufferOptions
// Purpose   : Change the number of buffers used.
// Parameters: None
//==============================================================================
void CGigESeqGrabDemoDlg::OnBufferOptions(void)
{
   CBufDlg dlg(this, m_Buffers, m_View->GetDisplay());
   if (dlg.DoModal() == IDOK)
   {
      CWaitCursor cur;

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
      } // End if.

      m_ImageWnd.Reset();
      InvalidateRect(NULL);
      UpdateWindow();
      UpdateMenu();
   } // End if.
} // End of the CGigESeqGrabDemoDlg::OnBufferOptions method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnLoadAcqConfig
// Purpose   : Select a new configuration file for the acquisition.
// Parameters: None
//==============================================================================
void CGigESeqGrabDemoDlg::OnLoadAcqDeviceConfig(void)
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
      } // End if.

      ReadCameraTimestamp();

      m_ImageWnd.Reset();
      InvalidateRect(NULL);
      UpdateWindow();
      UpdateMenu();
   } // End if, else.
} // End of the CGigESeqGrabDemoDlg::OnLoadAcqConfig method.

//*****************************************************************************************
//
//             File Options
//
//*****************************************************************************************

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnFileLoad
// Purpose   : Load a file to the buffers.
// Parameters: None
//==============================================================================
void CGigESeqGrabDemoDlg::OnFileLoad(void)
{
   if (m_Buffers->GetFormat() == SapFormatMono16)
   {
      MessageBox(_T("Sequence images in AVI format are sampled at 8-bit pixel depth.\nYou cannot load a sequence in the current configuration."));
      return;
   }

   if ((m_Buffers->GetFormat() == SapFormatRGBR888))
   {
      MessageBox(_T("Sequence images acquired in RGBR888 format (red first) were saved as RGB888 (blue first).\nYou cannot load a sequence in the current configuration."));
      return;
   }

   CLoadSaveDlg dlg(this, m_Buffers, TRUE, TRUE);
   if (dlg.DoModal() == IDOK)
   {
      InvalidateRect(NULL);
      UpdateWindow();
      UpdateMenu();
   } // End if.
} // End of the CGigESeqGrabDemoDlg::OnFileLoad method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnFileSave
// Purpose   : Save buffers to a file.
// Parameters: None
//==============================================================================
void CGigESeqGrabDemoDlg::OnFileSave(void)
{
   if (m_Buffers->GetFormat() == SapFormatMono16)
   {
      MessageBox(_T("Saving images in AVI format requires downsampling them to 8-bit pixel depth.\nYou will not be able to reload this sequence in this application unless you change the buffer format."));
   }

   if ((m_Buffers->GetFormat() == SapFormatRGBR888))
   {
      MessageBox(_T("Saving images in AVI format requires conversion to RGB888 format (blue first).\nYou will not be able to reload this sequence in this application unless you change the buffer format."));
   }

   CLoadSaveDlg dlg(this, m_Buffers, FALSE, TRUE);
   dlg.DoModal();
} // End of the CGigESeqGrabDemoDlg::OnFileSave method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnFileLoadCurrent
// Purpose   :
// Parameters: None
//==============================================================================
void CGigESeqGrabDemoDlg::OnFileLoadCurrent(void)
{
   CLoadSaveDlg dlg(this, m_Buffers, TRUE, FALSE);
   if (dlg.DoModal() == IDOK)
   {
      InvalidateRect(NULL);
      UpdateWindow();
      UpdateMenu();
   } // End if.
} // End of the CGigESeqGrabDemoDlg::OnFileLoadCurrent method.

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnFileSaveCurrent
// Purpose   :
// Parameters: None
//==============================================================================
void CGigESeqGrabDemoDlg::OnFileSaveCurrent(void)
{
   CLoadSaveDlg dlg(this, m_Buffers, FALSE, FALSE);
   dlg.DoModal();
} // End of the CGigESeqGrabDemoDlg::OnFileSaveCurrent method.
//==============================================================================

//==============================================================================
// Name      : CGigESeqGrabDemoDlg::OnHighFrameRate
// Purpose   :
// Parameters: None
//==============================================================================
#include "HighFrameRateDlg.h"

void CGigESeqGrabDemoDlg::OnHighFrameRate()
{
   CHighFrameRateDlg dlg(this, m_nFramesPerCallback, m_Xfer);

   if (dlg.DoModal() == IDOK)
   {
      CWaitCursor cursor;

      m_nFramesPerCallback = dlg.GetNFramesPerCallback();

      // Destroy transfer object
      m_Xfer->Destroy();

      // Recreate transfer object with new setting
      CreateObjects();
   }
}// End of the CGigESeqGrabDemoDlg::OnHighFrameRate method.
//==============================================================================

void CGigESeqGrabDemoDlg::ReadCameraTimestamp(void)
{
   BOOL bIsAvailable = FALSE;

   // Current feature name in SFNC
   if (m_AcqDevice->IsFeatureAvailable("TimestampLatch", &bIsAvailable) && bIsAvailable)
   {
      m_AcqDevice->SetFeatureValue("TimestampLatch", 1);
   }
   // Deprecated in SFNC
   else if (m_AcqDevice->IsFeatureAvailable("GevTimestampControlLatch", &bIsAvailable) && bIsAvailable)
   {
      m_AcqDevice->SetFeatureValue("GevTimestampControlLatch", 1);
   }
   // Specific to Teledyne DALSA
   else if (m_AcqDevice->IsFeatureAvailable("timestampControlLatch", &bIsAvailable) && bIsAvailable)
   {
      m_AcqDevice->SetFeatureValue("timestampControlLatch", 1);
   }

   UINT64 timestamp = 0;

   // Current feature name in SFNC
   if (m_AcqDevice->IsFeatureAvailable("TimestampLatchValue", &bIsAvailable) && bIsAvailable)
   {
      m_AcqDevice->GetFeatureValue("TimestampLatchValue", &timestamp);
   }
   // Deprecated in SFNC
   else if (m_AcqDevice->IsFeatureAvailable("GevTimestampValue", &bIsAvailable) && bIsAvailable)
   {
      m_AcqDevice->GetFeatureValue("GevTimestampValue", &timestamp);
   }
   // Specific to Teledyne DALSA
   else if (m_AcqDevice->IsFeatureAvailable("timestampValue", &bIsAvailable) && bIsAvailable)
   {
      m_AcqDevice->GetFeatureValue("timestampValue", &timestamp);
   }

   char strBuf[64];
   _ui64toa_s(timestamp, strBuf, sizeof(strBuf), 10);
   m_TimestampCurrent = CString(strBuf);
}

void CGigESeqGrabDemoDlg::OnBnClickedCameraTimestampUpdate()
{
   ReadCameraTimestamp();
   UpdateData(FALSE);
}


void CGigESeqGrabDemoDlg::OnBnClickedCameraTimestampReset()
{
   BOOL bIsAvailable = FALSE;

   // Current feature name in SFNC
   if (m_AcqDevice->IsFeatureAvailable("TimestampReset", &bIsAvailable) && bIsAvailable)
   {
      m_AcqDevice->SetFeatureValue("TimestampReset", 1);
   }
   // Deprecated in SFNC
   else if (m_AcqDevice->IsFeatureAvailable("GevTimestampControlReset", &bIsAvailable) && bIsAvailable)
   {
      m_AcqDevice->SetFeatureValue("GevTimestampControlReset", 1);
   }
   // Specific to Teledyne DALSA
   else if (m_AcqDevice->IsFeatureAvailable("timestampControlReset", &bIsAvailable) && bIsAvailable)
   {
      m_AcqDevice->SetFeatureValue("timestampControlReset", 1);
   }
   else
   {
      MessageBox(_T("Timestamp reset is not supported for this camera"));
      return;
   }

   OnBnClickedCameraTimestampUpdate();
}


BOOL CGigESeqGrabDemoDlg::PreTranslateMessage(MSG* pMsg)
{
   if (pMsg->message == WM_KEYDOWN)
   {
      if (pMsg->wParam == VK_RETURN || pMsg->wParam == VK_ESCAPE)
      {
         return TRUE;                // Do not process further
      }
   }

   return CDialog::PreTranslateMessage(pMsg);
}
