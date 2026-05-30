//==============================================================================
// Module : GigEMetaDataDemoDlg.cpp
// Project: Sapera samples.
// Purpose: This module contains the methods that define the CGigEMetaDataDemoDlg class.
//==============================================================================
// Include files.

#include "stdafx.h"
#include "GigEMetaDataDemo.h"
#include "GigEMetaDataDemoDlg.h"
#include "GigEMetaDataDemoAbout.h"
#include "float.h"



//==============================================================================
// Name      : CGigEMetaDataDemoDlg::CGigEMetaDataDemoDlg
// Purpose   : Class constructor.
// Parameters:
//    pCParent                Parent window.
//==============================================================================
CGigEMetaDataDemoDlg::CGigEMetaDataDemoDlg(CWnd* pParent)
   : CDialog(CGigEMetaDataDemoDlg::IDD, pParent)
{
   //{{AFX_DATA_INIT(CGigEMetaDataDemoDlg)
   m_LiveFrameRate = _T("");
   m_BufferFrameRate = 0.0f;
   m_MaxTime = 0.0f;
   m_MinTime = 0.0f;
   m_ActiveBuffer = 0;
   m_BufferCount = 0;
   m_Slider = 0;
   //}}AFX_DATA_INIT

   // Note that LoadIcon does not require a subsequent DestroyIcon in Win32
   m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);

   m_AcqDevice = NULL;
   m_Feature = NULL;
   m_Buffers = NULL;
   m_Xfer = NULL;
   m_View = NULL;
   m_Metadata = NULL;

   m_metadataType = SapMetadata::MetadataUnknown;

   m_IsSignalDetected = TRUE;
   m_IsAreaScan = TRUE;

   m_bRecordOn = FALSE;
   m_bPlayOn = FALSE;
   m_bPauseOn = FALSE;

   m_XferDisconnectDuringSetup = FALSE;
   m_IsSelectAvailable = FALSE;

   m_nFramesPerCallback = 1;
   m_MetadataFileIndex = 1;

   m_BufferIsValid = NULL;

   m_TimestampCurrent = _T("");
   m_TimestampBuffer = 0;
   m_TimestampBufferDelta = 0;
   m_PlayLastClock = 0;
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::DoDataExchange
// Purpose   : Data exchange.
// Parameters:
//    pDX
//==============================================================================
void CGigEMetaDataDemoDlg::DoDataExchange(CDataExchange* pDX)
{
   CDialog::DoDataExchange(pDX);
   //{{AFX_DATA_MAP(CGigEMetaDataDemoDlg)
   DDX_Control(pDX, IDC_STOP, m_stopButton);
   DDX_Control(pDX, IDC_PAUSE, m_pauseButton);
   DDX_Control(pDX, IDC_PLAY, m_playButton);
   DDX_Control(pDX, IDC_RECORD, m_recordButton);
   DDX_Control(pDX, IDC_SLIDER, m_SliderCtrl);
   DDX_Control(pDX, IDC_VIEW_WND, m_ImageWnd);
   DDX_Text(pDX, IDC_LIVE_FRAME_RATE, m_LiveFrameRate);
   DDX_Text(pDX, IDC_BUFFER_FRAME_RATE, m_BufferFrameRate);
   DDX_Text(pDX, IDC_MAX_TIME, m_MaxTime);
   DDX_Text(pDX, IDC_MIN_TIME, m_MinTime);
   DDX_Text(pDX, IDC_ACTIVE, m_ActiveBuffer);
   DDX_Text(pDX, IDC_AVAILABLE, m_BufferCount);
   DDX_Slider(pDX, IDC_SLIDER, m_Slider);
   DDX_Control(pDX, IDC_CHECKLIST_METADATA, m_checklistMetadata);
   DDX_Control(pDX, IDC_METADATA_ACTIVE_MODE, m_checkEnable);
   DDX_Control(pDX, IDC_LIST_METADATA_VIEW, m_listMetadataView);
   DDX_Control(pDX, IDC_LIST_METADATA_VIEW2, m_listMetadataView2);
   DDX_Text(pDX, IDC_TIMESTAMP_CURRENT, m_TimestampCurrent);
   DDX_Text(pDX, IDC_TIMESTAMP_BUFFER, m_TimestampBuffer);
   DDX_Text(pDX, IDC_TIMESTAMP_BUFFER_DELTA, m_TimestampBufferDelta);
   //}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CGigEMetaDataDemoDlg, CDialog)
   //{{AFX_MSG_MAP(CGigEMetaDataDemoDlg)
   ON_WM_SYSCOMMAND()
   ON_WM_PAINT()
   ON_WM_QUERYDRAGICON()
   ON_WM_DESTROY()
   ON_WM_MOVE()
   ON_WM_SIZE()
   ON_WM_HSCROLL()
   ON_WM_VSCROLL()
   ON_WM_TIMER()
   ON_BN_CLICKED(IDC_RECORD, OnBnClickedRecord)
   ON_BN_CLICKED(IDC_PLAY, OnBnClickedPlay)
   ON_BN_CLICKED(IDC_PAUSE, OnBnClickedPause)
   ON_BN_CLICKED(IDC_STOP, OnBnClickedStop)
   ON_BN_CLICKED(IDC_BUFFER_OPTIONS, OnBnClickedBufferOptions)
   ON_BN_CLICKED(IDC_LOAD_ACQ_CONFIG, OnBnClickedLoadAcqConfig)
   ON_BN_CLICKED(IDC_FILE_LOAD, OnBnClickedFileLoad)
   ON_BN_CLICKED(IDC_FILE_SAVE, OnBnClickedFileSave)
   ON_EN_KILLFOCUS(IDC_BUFFER_FRAME_RATE, OnKillfocusBufferFrameRate)
   ON_BN_CLICKED(IDC_FILE_LOAD_CURRENT, OnBnClickedFileLoadCurrent)
   ON_BN_CLICKED(IDC_FILE_SAVE_CURRENT, OnBnClickedFileSaveCurrent)
   ON_MESSAGE(WM_UPDATE_CONTROLS, OnUpdateControls)
   ON_BN_CLICKED(IDC_HIGH_FRAME_RATE, OnBnClickedHighFrameRate)
   ON_WM_ENDSESSION()
   ON_WM_QUERYENDSESSION()
   ON_BN_CLICKED(IDC_SAVE_METADATA, &CGigEMetaDataDemoDlg::OnBnClickedSaveMetadata)
   ON_BN_CLICKED(IDC_METADATA_ACTIVE_MODE, &CGigEMetaDataDemoDlg::OnBnClickedMetadataActiveMode)
   ON_CLBN_CHKCHANGE(IDC_CHECKLIST_METADATA, &CGigEMetaDataDemoDlg::OnClBnChkChangeCheckListMetadata)
   ON_BN_CLICKED(IDC_READ_CURRENT_TIMESTAMP, &CGigEMetaDataDemoDlg::OnBnClickedReadCurrentTimestamp)
   ON_BN_CLICKED(IDC_RESET_TIMESTAMP, &CGigEMetaDataDemoDlg::OnBnClickedResetTimestamp)
   //}}AFX_MSG_MAP
END_MESSAGE_MAP()

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::XferCallback
// Purpose   : Callback function for the transfer.
// Parameters: None
//==============================================================================
void CGigEMetaDataDemoDlg::XferCallback(SapXferCallbackInfo* pInfo)
{
   CGigEMetaDataDemoDlg* pDlg = (CGigEMetaDataDemoDlg*)pInfo->GetContext();

   SapBuffer::State bufState = SapBuffer::StateEmpty;
   int bufIndex = pDlg->m_Buffers->GetIndex();

   pDlg->m_Buffers->GetState(bufIndex, &bufState);
   pDlg->m_BufferIsValid[bufIndex] = (bufState == SapBuffer::StateFull);

   // Measure real frame time
   pDlg->UpdateFrameRate();

   // Check if last frame is reached
   pDlg->CheckForLastFrame();

   // Refresh view
   pDlg->m_View->Show();

   // Refresh controls
   pDlg->PostMessage(WM_UPDATE_CONTROLS, 0, 0);
}

#pragma region image viewer event handlers

void CGigEMetaDataDemoDlg::PixelChanged(int x, int y)
{
   CString str = m_appTitle;
   str += "  " + m_ImageWnd.GetPixelString(CPoint(x, y));
   SetWindowText(str);
}

void CGigEMetaDataDemoDlg::MarkerChanged()
{
   uint markerLine = m_ImageWnd.GetMarkerLine();
   CString str;
   str.Format(_T("    [ marker at line %d ]"), markerLine);
   SetWindowText(m_appTitle + str);

   UpdateMetadataList();
}

void CGigEMetaDataDemoDlg::InteractiveMarkerChanged(int lineIndex)
{
   CString str;
   str.Format(_T("    [ interactive marker at line %d ]"), lineIndex);
   SetWindowText(m_appTitle + str);
}

#pragma endregion

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnInitDialog
// Purpose   : Dialog initialization.
// Parameters: None
//==============================================================================
BOOL CGigEMetaDataDemoDlg::OnInitDialog(void)
{
   CDialog::OnInitDialog();
   CDialog::SetDefID(IDC_METADATA_ACTIVE_MODE);

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
   SetIcon(m_hIcon, TRUE);       // Set big icon
   SetIcon(m_hIcon, FALSE);      // Set small icon

   // Initialize variables
   GetWindowText(m_originalAppTitle);

   // We must be operating on-line
   CAcqConfigDlg dlg(this, CAcqConfigDlg::ServerAcqDevice);
   if (dlg.DoModal() != IDOK)
   {
      MessageBox(_T("No device found"));
      EndDialog(TRUE);
      return FALSE;
   }

   // Define objects
   m_AcqDevice = new SapAcqDevice(dlg.GetLocation(), dlg.GetConfigFile());
   m_Feature = new SapFeature(dlg.GetLocation());
   m_Buffers = new SapBufferWithTrash(MAX_BUFFER);
   m_Metadata = new SapMetadata(m_AcqDevice, m_Buffers);
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

   // Create image window
   m_ImageWnd.AttachEventHandler(this);
   m_ImageWnd.Reset();

   UpdateMenu();

   return TRUE;
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::CreateObjects
// Purpose   : Create sapera objects.
// Parameters: None
//==============================================================================
BOOL CGigEMetaDataDemoDlg::CreateObjects(bool createAcqDevice)
{
   //Number of frames per callback retreived
   int nFramesPerCallback;

   // Create acquisition object
   if (createAcqDevice && m_AcqDevice && !*m_AcqDevice && !m_AcqDevice->Create())
   {
      DestroyObjects();
      return FALSE;
   }

   // Make sure the acq device supports mandatory metadata features
   if (!SapMetadata::IsMetadataSupported(m_AcqDevice))
   {
      MessageBox(_T("This demo only supports Teledyne Dalsa and Teledyne Lumenera cameras with metadata features"),
                 _T("Incompatible camera"), MB_ICONERROR);
      DestroyObjects();
      return FALSE;
   }

   // Create feature object
   if (m_Feature && !*m_Feature && !m_Feature->Create())
   {
      DestroyObjects();
      return FALSE;
   }

   // Create metadata object
   if (m_Metadata && !*m_Metadata && !m_Metadata->Create())
   {
      DestroyObjects();
      return FALSE;
   }

   m_IsAreaScan = (m_Metadata->GetMetadataType() == SapMetadata::MetadataPerFrame);

   // Check if metadata selectors can be enabled through application code
   m_IsSelectAvailable = FALSE;
   char* featurename = m_IsAreaScan ? "ChunkEnable" : "endOfLineMetadataContentActivationMode";
   if (m_AcqDevice->GetFeatureInfo(featurename, m_Feature))
   {
      SapFeature::AccessMode accessMode;
      if (m_Feature->GetAccessMode(&accessMode))
         m_IsSelectAvailable = (accessMode == SapFeature::AccessRW);
   }

   // Check if the transfer needs to remain disconnected when enabling / disabling / selecting metadata
   m_XferDisconnectDuringSetup = FALSE;
   featurename = m_IsAreaScan ? "ChunkModeActive" : "endOfLineMetadataMode";
   if (m_AcqDevice->GetFeatureInfo(featurename, m_Feature))
   {
      SapFeature::WriteMode writeMode;
      if (m_Feature->GetWriteMode(&writeMode))
         m_XferDisconnectDuringSetup = (writeMode == SapFeature::WriteNotConnected);
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

      m_BufferIsValid = new BOOL[m_Buffers->GetCount()];
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

   // Show type of metadata in title bar
   m_appTitle = m_originalAppTitle;
   m_metadataType = m_Metadata->GetMetadataType();
   switch (m_metadataType)
   {
   case SapMetadata::MetadataPerFrame:
      m_appTitle += " (Per-Frame Metadata)";
      break;
   case SapMetadata::MetadataPerLine:
      m_appTitle += " (Per-Line Metadata)";
      m_ImageWnd.EnableMarker();
      SetDlgItemText(IDC_STATIC_METADATA_LIST_HEADER, _T("Metadata of the displayed buffer and marker"));
      break;
   default:
      m_appTitle += " -- Acquisition device doesn't support Metadata";
      break;
   }
   SetWindowText(m_appTitle);

   ReadCameraTimestamp();

   if (m_XferDisconnectDuringSetup)
      m_Xfer->Disconnect();

   // Feed the list of selectors
   m_checklistMetadata.ResetContent();
   UINT selectorCount = m_Metadata->GetSelectorCount();
   char selectorName[MAX_PATH] = { 0 };
   for (UINT selectorIndex = 0; selectorIndex < selectorCount; selectorIndex++)
   {
      if (m_Metadata->GetSelectorName(selectorIndex, selectorName, MAX_PATH))
      {
         int pos = m_checklistMetadata.AddString(CString(selectorName));
         m_checklistMetadata.SetItemData(pos, selectorIndex);
         m_checklistMetadata.SetCheck(pos, m_Metadata->IsSelected(selectorIndex) ? BST_CHECKED : BST_UNCHECKED);
      }
   }

   if (m_XferDisconnectDuringSetup)
      m_Xfer->Connect();

   // Clear extracted chunks in their display listboxes
   m_listMetadataView.ResetContent();
   m_listMetadataView2.ResetContent();

   UpdateMenu();

   return TRUE;
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::DestryObjects
// Purpose   : Destroy sapera objects.
// Parameters: None
//==============================================================================
BOOL CGigEMetaDataDemoDlg::DestroyObjects(bool destroyAcqDevice)
{
   // Destroy transfer object
   if (m_Xfer && *m_Xfer)
      m_Xfer->Destroy();

   // Destroy view object
   if (m_View && *m_View)
      m_View->Destroy();

   // Destroy buffer object
   if (m_BufferIsValid != NULL)
   {
      delete [] m_BufferIsValid;
      m_BufferIsValid = NULL;
   }

   if (m_Buffers && *m_Buffers)
      m_Buffers->Destroy();

   // Destroy metadata object
   if (m_Metadata && *m_Metadata)
      m_Metadata->Destroy();

   // Destroy feature object
   if (m_Feature && *m_Feature)
      m_Feature->Destroy();

   // Destroy acquisition object
   if (destroyAcqDevice && m_AcqDevice && *m_AcqDevice)
      m_AcqDevice->Destroy();

   return TRUE;
}

#pragma region metadata related code

void CGigEMetaDataDemoDlg::OnBnClickedSaveMetadata()
{
   CFileDialog fileDlg(FALSE, _T("csv"), _T("metadata.csv"), OFN_HIDEREADONLY | OFN_CREATEPROMPT | OFN_OVERWRITEPROMPT, _T("Metadata Files (*.csv)|*.csv||"), this);
   if (fileDlg.DoModal() == IDOK)
   {
      CStringA pathName = (CStringA)fileDlg.GetPathName();
      CWaitCursor cur;
      if (m_Metadata->SaveToCSV(pathName))
         MessageBox(_T("The metadata has been succesfully saved to the file"), _T("We are done"), MB_ICONINFORMATION);
      else
         MessageBox(_T("Failed to save the metadata to the file"), _T("Failed to save"), MB_ICONERROR);
   }
}

void CGigEMetaDataDemoDlg::OnBnClickedMetadataActiveMode()
{
   if (m_XferDisconnectDuringSetup)
      m_Xfer->Disconnect();

   m_Metadata->Enable(m_checkEnable.GetCheck() == BST_CHECKED);

   // Check if metadata selectors can be enabled through application code
   m_IsSelectAvailable = FALSE;
   char* featurename = m_IsAreaScan ? "ChunkEnable" : "endOfLineMetadataContentActivationMode";
   if (m_AcqDevice->GetFeatureInfo(featurename, m_Feature))
   {
      SapFeature::AccessMode accessMode;
      if (m_Feature->GetAccessMode(&accessMode))
         m_IsSelectAvailable = (accessMode == SapFeature::AccessRW);
   }

   if (m_XferDisconnectDuringSetup)
      m_Xfer->Connect();

   UpdateMenu();
}

void CGigEMetaDataDemoDlg::OnClBnChkChangeCheckListMetadata()
{
   int itemIndex = m_checklistMetadata.GetCaretIndex();
   BOOL itemIndexChecked = (m_checklistMetadata.GetCheck(itemIndex) != 0);

   // Update selected item's checkbox
   m_Metadata->Select(itemIndex, itemIndexChecked);

   // Update all items checkboxes, in case another one changed because of the itemIndex one
   UINT selectorCount = m_Metadata->GetSelectorCount();
   for (UINT selectorIndex = 0; selectorIndex < selectorCount; selectorIndex++)
   {
      BOOL bIsSelected = m_Metadata->IsSelected(selectorIndex);
      m_checklistMetadata.SetCheck(selectorIndex, bIsSelected ? BST_CHECKED : BST_UNCHECKED);
   }
}

void CGigEMetaDataDemoDlg::UpdateMetadataList()
{
   m_listMetadataView.ResetContent();
   m_listMetadataView2.ResetContent();

   if (!m_Metadata->IsEnabled())
      return;

   if (!m_BufferIsValid[m_Buffers->GetIndex()])
      return;

   BOOL bExtractStatus = FALSE;

   if (m_metadataType == SapMetadata::MetadataPerFrame)
      bExtractStatus = m_Metadata->Extract(m_Buffers->GetIndex());
   else if (m_metadataType == SapMetadata::MetadataPerLine)
   {
      uint markerLine = m_ImageWnd.GetMarkerLine();
      bExtractStatus = m_Metadata->Extract(m_Buffers->GetIndex(), markerLine);
   }
   else
      return;

   if (bExtractStatus)
   {
      UINT resultCount = m_Metadata->GetExtractedResultCount();

      for (UINT resultIndex = 0; resultIndex < resultCount; resultIndex++)
      {
         char sResultName[MAX_PATH] = { 0 };
         char sResultValue[MAX_PATH] = { 0 };
         if (m_Metadata->GetExtractedResult(resultIndex, sResultName, MAX_PATH, sResultValue, MAX_PATH))
         {
            m_listMetadataView.AddString(CString(sResultName));
            m_listMetadataView2.AddString(CString(sResultValue));
         }
      }
   }
}

#pragma endregion

#pragma region MFC message handlers
//==============================================================================
// Name      : CGigEMetaDataDemoDlg::DestryObjects
// Purpose   : Handle commands from system, only handles about box.
// Parameters:
//    nID
//    lParam
//==============================================================================
void CGigEMetaDataDemoDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
   if ((nID & 0xFFF0) == IDM_ABOUTBOX)
   {
      CAboutDlg dlgAbout;
      dlgAbout.DoModal();
   }
   else
      CDialog::OnSysCommand(nID, lParam);
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnPaint
// Purpose   : If you add a minimize button to your dialog, you will need the code
//             below to draw the icon.
// Parameters: None
//==============================================================================
void CGigEMetaDataDemoDlg::OnPaint(void)
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
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnQueryDragIcon
// Purpose   : The system calls this to obtain the cursor to display while the
//             user drags the minimized window.
// Parameters: None
//==============================================================================
HCURSOR CGigEMetaDataDemoDlg::OnQueryDragIcon(void)
{
   return (HCURSOR)m_hIcon;
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnDestroy
// Purpose   : Handle the destroy message.
// Parameters: None
//==============================================================================
void CGigEMetaDataDemoDlg::OnDestroy(void)
{
   CDialog::OnDestroy();

   // Destroy all objects
   DestroyObjects();

   // Delete all objects
   if (m_View)			delete m_View;
   if (m_Xfer)			delete m_Xfer;
   if (m_Buffers)		delete m_Buffers;
   if (m_Metadata)   delete m_Metadata;
   if (m_Feature)	   delete m_Feature;
   if (m_AcqDevice)	delete m_AcqDevice;
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnSize
// Purpose   : Handle the size event.
// Parameters:
//    cx                      New X size.
//    cy                      New Y size.
//==============================================================================
void CGigEMetaDataDemoDlg::OnSize(UINT nType, int cx, int cy)
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
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnHScroll
// Purpose   : Handle the horizontal scroll bar events.
// Parameters:
//    nSBCode                 Scroll bar code.
//    nPos                    New scroll bar position.
//    pScrollBar              Scroll bar object.
//==============================================================================
void CGigEMetaDataDemoDlg::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
   if (pScrollBar->GetDlgCtrlID() == IDC_SLIDER)
   {
      // Get slider position
      UpdateData(TRUE);

      // Update buffer index
      m_Buffers->SetIndex(m_Slider);

      // Refresh controls
      OnUpdateControls(0, 0);

      UpdateMetadataList();

      // Resfresh display
      m_View->Show();
   }

   CDialog::OnHScroll(nSBCode, nPos, pScrollBar);
}

void CGigEMetaDataDemoDlg::OnEndSession(BOOL bEnding)
{
   CDialog::OnEndSession(bEnding);

   if (bEnding)
      OnDestroy(); // If ending the session, free the resources.
}

BOOL CGigEMetaDataDemoDlg::OnQueryEndSession()
{
   if (!CDialog::OnQueryEndSession())
      return FALSE;

   return TRUE;
}

#pragma endregion

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnKillfocusBufferFrameRate
// Purpose   : Handle the
// Parameters: None
//==============================================================================
void CGigEMetaDataDemoDlg::OnKillfocusBufferFrameRate(void)
{
   UpdateData(TRUE);
   m_Buffers->SetFrameRate(m_BufferFrameRate);
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::UpdateMenu
// Purpose   : Updates the menu items enabling/disabling the proper items
//             depending on the state of the application.
// Parameters: None
//==============================================================================
void CGigEMetaDataDemoDlg::UpdateMenu(void)
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

   // Metadata controls
   BOOL bIsMetadataEnabled = m_Metadata->IsEnabled();

   m_checkEnable.SetCheck(bIsMetadataEnabled ? BST_CHECKED : BST_UNCHECKED);
   m_checkEnable.EnableWindow(bAcqNoGrab);
   m_checklistMetadata.EnableWindow(bAcqNoGrab && bIsMetadataEnabled && m_IsSelectAvailable);
   m_listMetadataView.EnableWindow(bAcqNoGrab && bIsMetadataEnabled);
   m_listMetadataView2.EnableWindow(bAcqNoGrab && bIsMetadataEnabled);
   if (!bIsMetadataEnabled)
   {
      m_listMetadataView.ResetContent();
      m_listMetadataView2.ResetContent();
   }

   // Slider
   m_SliderCtrl.EnableWindow(bNoGrab || (m_bPlayOn && m_bPauseOn));
   m_SliderCtrl.SetRange(0, m_Buffers->GetCount() - 1, TRUE);

   // If last control was disabled, set default focus
   if (!GetFocus())
      GetDlgItem(IDC_BUFFER_OPTIONS)->SetFocus();

   // Update control values
   PostMessage(WM_UPDATE_CONTROLS, 0, 0);
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::UpdateFrameRate
// Purpose   : Calculate statistics on frame rate.
// Parameters: None
//==============================================================================
void CGigEMetaDataDemoDlg::UpdateFrameRate(void)
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
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::CheckForLastFrame
// Purpose   : Check if the last frame needed has been acquired.
// Parameters: None
//==============================================================================
void CGigEMetaDataDemoDlg::CheckForLastFrame(void)
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

      UpdateMetadataList();
   }
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnTimer
// Purpose   :
// Parameters:
//    nIDEvent
//==============================================================================
void CGigEMetaDataDemoDlg::OnTimer(UINT_PTR nIDEvent)
{
   if (nIDEvent == 1)
   {
      // Increase buffer index
      m_Buffers->Next();

      // Calculate the normal frame count in the interval
      clock_t curClock = clock();
      float elapsedSeconds = (curClock - m_PlayLastClock) / (float)CLOCKS_PER_SEC;
      m_PlayLastClock = curClock;
      int normalFrameCountInInterval = (int)(elapsedSeconds * m_BufferFrameRate);

      // Skip some frame if we lose some time since last been here (or since OnBnClickedPlay)
      int i;
      for (i = 1; (i < normalFrameCountInInterval) && (m_Buffers->GetIndex() < m_Buffers->GetCount() - 1); i++)
         m_Buffers->Next();

      // Resfresh display
      m_View->Show();

      // Check if last frame is reached
      CheckForLastFrame();

      // Refresh controls
      PostMessage(WM_UPDATE_CONTROLS, 0, 0);
   }

   CDialog::OnTimer(nIDEvent);
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnUpdateControls
// Purpose   : Update the control values.
// Parameters: None
//==============================================================================
LRESULT CGigEMetaDataDemoDlg::OnUpdateControls(WPARAM, LPARAM)
{
   if (m_Buffers)
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
   }
   return 0;
}

//*****************************************************************************************
//
//             Record Control
//
//*****************************************************************************************

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnBnClickedRecord
// Purpose   : Start recording frames.
// Parameters: None
//==============================================================================
void CGigEMetaDataDemoDlg::OnBnClickedRecord(void)
{
   // Reset source and destination indices
   m_Xfer->Init();

   // Reset the frame rate statistics ahead of each transfer stream
   SapXferFrameRateInfo* pStats = m_Xfer->GetFrameRateStatistics();
   pStats->Reset();

   // Make all buffers valid for metadata (may be set as invalid in transfer callback)
   int bufCount = m_Buffers->GetCount();
   for (int bufIndex = 0; bufIndex < bufCount; bufIndex++)
      m_BufferIsValid[bufIndex] = TRUE;

   // Acquire all frames
   if (m_Xfer->Snap(m_Buffers->GetCount()))
   {
      m_bRecordOn = TRUE;
      UpdateMenu();
   }
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnBnClickedPlay
// Purpose   : Play back the frames.
// Parameters: None
//==============================================================================
void CGigEMetaDataDemoDlg::OnBnClickedPlay(void)
{
   m_Buffers->SetIndex(0); // Initialize buffer index

   // Start playback timer
   m_PlayLastClock = clock();
   int frameTime = max(1, (int)(1000.0 / m_Buffers->GetFrameRate()));
   SetTimer(1, frameTime, NULL);

   m_bPlayOn = TRUE;
   UpdateMenu();
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnBnClickedPause
// Purpose   : Pause the recording or playing of frames.
// Parameters: None
//==============================================================================
void CGigEMetaDataDemoDlg::OnBnClickedPause(void)
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
         KillTimer(1); // Stop playback timer
   }
   else
   {
      // Check if recording or playing
      if (m_bRecordOn)
      {
         int frameTime = max(1, (int)(1000.0 / m_Buffers->GetFrameRate()));
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
      }
   }

   m_bPauseOn = !m_bPauseOn;
   UpdateMenu();
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnBnClickedStop
// Purpose   : Stop the recording or playing of frames.
// Parameters: None
//==============================================================================
void CGigEMetaDataDemoDlg::OnBnClickedStop(void)
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
   }

   m_bPauseOn = FALSE;
   UpdateMenu();
}

//*****************************************************************************************
//
//             General Options
//
//*****************************************************************************************

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnBnClickedBufferOptions
// Purpose   : Change the number of buffers used.
// Parameters: None
//==============================================================================
void CGigEMetaDataDemoDlg::OnBnClickedBufferOptions(void)
{
   CBufDlg dlg(this, m_Buffers, m_View->GetDisplay());
   if (dlg.DoModal() == IDOK)
   {
      CWaitCursor cur;

      // Destroy objects
      DestroyObjects(false);

      // Update buffer object
      SapBuffer buf = *m_Buffers;
      *m_Buffers = dlg.GetBuffer();

      // Recreate objects
      if (!CreateObjects(false))
      {
         *m_Buffers = buf;
         CreateObjects(false);
      }

      // empty list of metadata
      m_listMetadataView.ResetContent();
      m_listMetadataView2.ResetContent();

      m_ImageWnd.Reset();
      InvalidateRect(NULL);
      UpdateWindow();
      UpdateMenu();
   }
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnBnClickedLoadAcqConfig
// Purpose   : Select a new configuration file for the acquisition.
// Parameters: None
//==============================================================================
void CGigEMetaDataDemoDlg::OnBnClickedLoadAcqConfig(void)
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
      m_Feature->SetLocation(dlg.GetLocation());

      // Recreate objects
      if (!CreateObjects())
      {
         m_AcqDevice->SetLocation(loc);
         m_AcqDevice->SetConfigFile(configFile);
         m_Feature->SetLocation(loc);
         CreateObjects();
      }

      ReadCameraTimestamp();

      m_ImageWnd.Reset();
      InvalidateRect(NULL);
      UpdateWindow();
      UpdateMenu();
   }
}

//*****************************************************************************************
//
//             File Options
//
//*****************************************************************************************

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnBnClickedFileLoad
// Purpose   : Load a file to the buffers.
// Parameters: None
//==============================================================================
void CGigEMetaDataDemoDlg::OnBnClickedFileLoad(void)
{
   if (m_Buffers->GetFormat() == SapFormatMono16)
   {
      MessageBox(_T("Sequence images in AVI format are sampled at 8-bit pixel depth.\nYou cannot load a sequence in the current configuration."));
      return;
   }

   if (m_Buffers->GetFormat() == SapFormatRGBR888)
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
   }
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnBnClickedFileSave
// Purpose   : Save buffers to a file.
// Parameters: None
//==============================================================================
void CGigEMetaDataDemoDlg::OnBnClickedFileSave(void)
{
   if (m_Buffers->GetFormat() == SapFormatMono16)
      MessageBox(_T("Saving images in AVI format requires downsampling them to 8-bit pixel depth.\nYou will not be able to reload this sequence in this application unless you change the buffer format."));

   if (m_Buffers->GetFormat() == SapFormatRGBR888)
      MessageBox(_T("Saving images in AVI format requires conversion to RGB888 format (blue first).\nYou will not be able to reload this sequence in this application unless you change the buffer format."));

   CLoadSaveDlg dlg(this, m_Buffers, FALSE, TRUE);
   dlg.DoModal();
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnBnClickedFileLoadCurrent
// Purpose   :
// Parameters: None
//==============================================================================
void CGigEMetaDataDemoDlg::OnBnClickedFileLoadCurrent(void)
{
   CLoadSaveDlg dlg(this, m_Buffers, TRUE, FALSE);
   if (dlg.DoModal() == IDOK)
   {
      InvalidateRect(NULL);
      UpdateWindow();
      UpdateMenu();
   }
}

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnBnClickedFileSaveCurrent
// Purpose   :
// Parameters: None
//==============================================================================
void CGigEMetaDataDemoDlg::OnBnClickedFileSaveCurrent(void)
{
   CLoadSaveDlg dlg(this, m_Buffers, FALSE, FALSE);
   dlg.DoModal();
}

//==============================================================================

//==============================================================================
// Name      : CGigEMetaDataDemoDlg::OnBnClickedHighFrameRate
// Purpose   :
// Parameters: None
//==============================================================================
#include "HighFrameRateDlg.h"

void CGigEMetaDataDemoDlg::OnBnClickedHighFrameRate()
{
   CHighFrameRateDlg dlg(this, m_nFramesPerCallback, m_Xfer);

   if (dlg.DoModal() == IDOK)
   {
      CWaitCursor cursor;

      m_nFramesPerCallback = dlg.GetNFramesPerCallback();

      m_Xfer->Destroy();

      CreateObjects();
   }
}

//==============================================================================

void CGigEMetaDataDemoDlg::ReadCameraTimestamp(void)
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

void CGigEMetaDataDemoDlg::OnBnClickedReadCurrentTimestamp()
{
   ReadCameraTimestamp();

   UpdateData(FALSE);
}

void CGigEMetaDataDemoDlg::OnBnClickedResetTimestamp()
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

   OnBnClickedReadCurrentTimestamp();
}


BOOL CGigEMetaDataDemoDlg::PreTranslateMessage(MSG* pMsg)
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
