// FlatFieldDemoDlg.cpp : implementation file
//

#include "stdafx.h"
#include "FlatFieldDemo.h"
#include "FlatFieldDemoDlg.h"

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
// CFlatFieldDemoDlg dialog

CFlatFieldDemoDlg::CFlatFieldDemoDlg(CWnd* pParent /*=NULL*/)
   : CDialog(CFlatFieldDemoDlg::IDD, pParent)
{
   //{{AFX_DATA_INIT(CFlatFieldDemoDlg)
   m_FlatFieldEnabled = FALSE;
   m_FlatFieldUseHardware = TRUE;
   m_FlatFieldPixelReplacement = TRUE;
   //}}AFX_DATA_INIT
   m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);

   m_Acq = NULL;
   m_Buffers = NULL;
   m_Bayer = NULL;
   m_Xfer = NULL;
   m_View = NULL;
   m_Pro = NULL;
   m_IsSignalDetected = TRUE;
   m_FlatField = NULL;
}

void CFlatFieldDemoDlg::DoDataExchange(CDataExchange* pDX)
{
   CDialog::DoDataExchange(pDX);
   //{{AFX_DATA_MAP(CFlatFieldDemoDlg)
   DDX_Control(pDX, IDC_STATUS, m_statusWnd);
   DDX_Control(pDX, IDC_VIEW_WND, m_ImageWnd);
   DDX_Check(pDX, IDC_FLATFIELD_ENABLE, m_FlatFieldEnabled);
   DDX_Check(pDX, IDC_FLATFIELD_USE_HARDWARE, m_FlatFieldUseHardware);
   DDX_Check(pDX, IDC_FLATFIELD_PIXEL_REPLACEMENT, m_FlatFieldPixelReplacement);
   //}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CFlatFieldDemoDlg, CDialog)
   //{{AFX_MSG_MAP(CFlatFieldDemoDlg)
   ON_WM_SYSCOMMAND()
   ON_WM_PAINT()
   ON_WM_QUERYDRAGICON()
   ON_WM_DESTROY()
   ON_WM_SIZE()
   ON_BN_CLICKED(IDC_SNAP, OnSnap)
   ON_BN_CLICKED(IDC_GRAB, OnGrab)
   ON_BN_CLICKED(IDC_FREEZE, OnFreeze)
   ON_BN_CLICKED(IDC_GENERAL_OPTIONS, OnGeneralOptions)
   ON_BN_CLICKED(IDC_LOAD_ACQ_CONFIG, OnLoadAcqConfig)
   ON_BN_CLICKED(IDC_BUFFER_OPTIONS, OnBufferOptions)
   ON_BN_CLICKED(IDC_VIEW_OPTIONS, OnViewOptions)
   ON_BN_CLICKED(IDC_FILE_LOAD, OnFileLoad)
   ON_BN_CLICKED(IDC_FILE_NEW, OnFileNew)
   ON_BN_CLICKED(IDC_FILE_SAVE, OnFileSave)
   ON_BN_CLICKED(IDC_EXIT, OnExit)
   ON_BN_CLICKED(IDC_FLATFIELD_ENABLE, OnFlatFieldEnable)
   ON_BN_CLICKED(IDC_FLATFIELD_CALIBRATE, OnFlatFieldCalibrate)
   ON_BN_CLICKED(IDC_FLATFIELD_LOAD, OnFlatFieldLoad)
   ON_BN_CLICKED(IDC_FLATFIELD_SAVE, OnFlatFieldSave)
   ON_BN_CLICKED(IDC_FLATFIELD_USE_HARDWARE, OnFlatFieldUseHardware)
   ON_BN_CLICKED(IDC_FLATFIELD_PIXEL_REPLACEMENT, OnFlatFieldPixelReplacement)
   ON_BN_CLICKED(IDC_BAYER_CHECKBOX, OnBnClickedBayerCheckbox)
   ON_WM_ENDSESSION()
   ON_WM_QUERYENDSESSION()
   //}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CFlatFieldDemoDlg message handlers

//
// This function is called each time an image has been transferred into system memory by the transfer object
//
void CFlatFieldDemoDlg::XferCallback(SapXferCallbackInfo *pInfo)
{
   CFlatFieldDemoDlg *pDlg = (CFlatFieldDemoDlg *)pInfo->GetContext();

   // If grabbing in trash buffer, do not display the image, update the
   // appropriate number of frames on the status bar instead
   if (pInfo->IsTrash())
   {
      CString str;
      str.Format(_T("Frames acquired in trash buffer: %d"), pInfo->GetEventCount());

      pDlg->m_statusWnd.SetWindowText(str);
   }
   else
   {
      // Process current buffer (see Run member function into the SapMyProcessing.cpp file)
      pDlg->m_Pro->ExecuteNext();
   }
}

//
// This function is called each time a buffer has been processed by the processing object
//
void CFlatFieldDemoDlg::ProCallback(SapProCallbackInfo *pInfo)
{
   CFlatFieldDemoDlg* pDlg = (CFlatFieldDemoDlg *)pInfo->GetContext();
   CString				 str;

   // Check if flat field correction was enabled and if it's been done by software
   if (pDlg->m_FlatField->IsEnabled() && pDlg->m_FlatField->IsSoftware())
   {
      // Show current buffer index and execution time in millisecond
      str.Format(_T("Flat field correction= %5.2f ms"), pDlg->m_Pro->GetTime());
   }

   // Refresh view
   pDlg->m_statusWnd.SetWindowText(str);
   pDlg->m_View->Show();
}

//
// This function is called each time the input signal connection status change
//
void CFlatFieldDemoDlg::SignalCallback(SapAcqCallbackInfo *pInfo)
{
   CFlatFieldDemoDlg *pDlg = (CFlatFieldDemoDlg *)pInfo->GetContext();

   pDlg->GetSignalStatus(pInfo->GetSignalStatus());
}

void CFlatFieldDemoDlg::PixelChanged(int x, int y)
{
   CString str = m_appTitle;
   str += "  " + m_ImageWnd.GetPixelString(CPoint(x, y));
   SetWindowText(str);
}

void CFlatFieldDemoDlg::RoiChanged()
{
   if (m_ImageWnd.IsRoiActive())
   {
      unsigned int left, top, width, height;
      m_ImageWnd.GetROI(left, top, width, height);
      m_FlatField->SetRegionOfInterest(left, top, width, height);
   }
   else
      m_FlatField->ResetRegionOfInterest();
}

BOOL CFlatFieldDemoDlg::OnInitDialog()
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
   SetIcon(m_hIcon, FALSE);	// Set small icon
   SetIcon(m_hIcon, TRUE);		// Set big icon

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
      m_FlatField = new SapFlatField( m_Acq);
      m_Bayer = new SapBayer(m_Acq, m_Buffers);
   }
   else
   {
      // Define off-line objects
      m_Buffers = new SapBuffer();
      m_FlatField = new SapFlatField( m_Buffers);
   }

   // Define other objects
	m_Pro  = new SapMyProcessing( m_Buffers, m_FlatField, ProCallback, this);
   m_View = new SapView(m_Buffers);

   // Attach sapview to image viewer
   m_ImageWnd.AttachSapView(m_View);

   // Create all objects
   if (!CreateObjects())
   {
      EndDialog(TRUE);
      return FALSE;
   }

   m_ImageWnd.AttachEventHandler(this);
   m_ImageWnd.Reset();
   m_ImageWnd.SupportROI();

   UpdateMenu();

   // Get current input signal connection status
   GetSignalStatus();

   return TRUE;  // return TRUE  unless you set the focus to a control
}

BOOL CFlatFieldDemoDlg::CreateObjects()
{
   CWaitCursor wait;

   // Create acquisition object
   if (m_Acq && !*m_Acq && !m_Acq->Create())
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

   // Create flat field object
   if (m_FlatField && !*m_FlatField && !m_FlatField->Create())
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

   // Create Bayer object
   if (m_Bayer && !*m_Bayer && !m_Bayer->Create())
   {
      DestroyObjects();
      return FALSE;
   }

   // Create transfer object
   if (m_Xfer && !*m_Xfer)
   {
      if (!m_Xfer->Create())
      {
         DestroyObjects();
         return FALSE;
      }
   }

	if( m_Pro && !*m_Pro)
	{
		if( !m_Pro->Create())
		{
			DestroyObjects();
			return FALSE;
		}

		m_Pro->SetAutoEmpty( TRUE);

		if( m_Xfer)
			m_Xfer->SetAutoEmpty( FALSE);
	}

   return TRUE;
}

BOOL CFlatFieldDemoDlg::DestroyObjects()
{
   // Destroy transfer object
   if (m_Xfer && *m_Xfer) m_Xfer->Destroy();

   if (m_Pro && *m_Pro) m_Pro->Destroy();

   if (m_Bayer && *m_Bayer) m_Bayer->Destroy();

   // Destroy view object
   if (m_View && *m_View) m_View->Destroy();

   // Destroy flat field object
   if (m_FlatField && *m_FlatField)	m_FlatField->Destroy();

   // Destroy buffer object
   if (m_Buffers && *m_Buffers) m_Buffers->Destroy();

   // Destroy acquisition object
   if (m_Acq && *m_Acq) m_Acq->Destroy();

   // Disable flat field correction
   m_FlatFieldEnabled = FALSE;

   UpdateData(FALSE);
   UpdateMenu();

   return TRUE;
}

//**********************************************************************************
//
//				Window related functions
//
//**********************************************************************************
void CFlatFieldDemoDlg::OnSysCommand(UINT nID, LPARAM lParam)
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
void CFlatFieldDemoDlg::OnPaint()
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
   }
}

void CFlatFieldDemoDlg::OnDestroy()
{
   // Destroy all objects
   DestroyObjects();

   // Delete all objects
   if (m_Xfer)			delete m_Xfer;
   if (m_Pro)			delete m_Pro;
   if (m_Bayer)		delete m_Bayer;
   if (m_View)			delete m_View;
   if (m_FlatField)  delete m_FlatField;
   if (m_Buffers)		delete m_Buffers;
   if (m_Acq)			delete m_Acq;

   CDialog::OnDestroy();
}


void CFlatFieldDemoDlg::OnSize(UINT nType, int cx, int cy)
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
HCURSOR CFlatFieldDemoDlg::OnQueryDragIcon()
{
   return (HCURSOR)m_hIcon;
}

void CFlatFieldDemoDlg::OnExit()
{
   EndDialog(TRUE);
}

void CFlatFieldDemoDlg::OnEndSession(BOOL bEnding)
{
   CDialog::OnEndSession(bEnding);

   if (bEnding)
   {
      // If ending the session, free the resources.
      OnDestroy();
   }
}

BOOL CFlatFieldDemoDlg::OnQueryEndSession()
{
   if (!CDialog::OnQueryEndSession())
      return FALSE;

   return TRUE;
}

//**************************************************************************************
// Updates the menu items enabling/disabling the proper items depending on the state
//  of the application
//**************************************************************************************
void CFlatFieldDemoDlg::UpdateMenu(void)
{
   BOOL bAcqNoGrab = m_Xfer && *m_Xfer && !m_Xfer->IsGrabbing();
   BOOL bAcqGrab = m_Xfer && *m_Xfer && m_Xfer->IsGrabbing();
   BOOL bNoGrab = !m_Xfer || !m_Xfer->IsGrabbing();

   BOOL isFlatFieldEnabled = m_FlatField && *m_FlatField && m_FlatField->IsEnabled();
   BOOL isFlatFieldAvailable = m_Acq && *m_Acq && m_Acq->IsFlatFieldAvailable();
   BOOL isFlatFieldSoftware = m_FlatField && *m_FlatField && m_FlatField->IsSoftware();
   BOOL isFlatFieldPixelReplacement = m_FlatField && *m_FlatField && m_FlatField->IsPixelReplacement();

   BOOL isBayerAvailable = m_Acq && *m_Acq && m_Acq->IsBayerAvailable();

   // Acquisition Control
   GetDlgItem(IDC_GRAB)->EnableWindow(bAcqNoGrab);
   GetDlgItem(IDC_SNAP)->EnableWindow(bAcqNoGrab);
   GetDlgItem(IDC_FREEZE)->EnableWindow(bAcqGrab);

   // Acquisition Options
   GetDlgItem(IDC_GENERAL_OPTIONS)->EnableWindow(bAcqNoGrab);
   GetDlgItem(IDC_LOAD_ACQ_CONFIG)->EnableWindow(bAcqNoGrab);

   // Flat field correction
   GetDlgItem(IDC_FLATFIELD_ENABLE)->EnableWindow(bNoGrab);
   GetDlgItem(IDC_FLATFIELD_CALIBRATE)->EnableWindow(bNoGrab);
   GetDlgItem(IDC_FLATFIELD_LOAD)->EnableWindow(bNoGrab);
   GetDlgItem(IDC_FLATFIELD_SAVE)->EnableWindow(bNoGrab);

   m_FlatFieldEnabled = isFlatFieldEnabled;
   ((CButton *)GetDlgItem(IDC_FLATFIELD_ENABLE))->SetCheck(m_FlatFieldEnabled);

   m_FlatFieldUseHardware = isFlatFieldAvailable && !isFlatFieldSoftware;

   BOOL bayerDecoder = FALSE;
   if (m_Acq && *m_Acq && m_Acq->IsParameterValid(CORACQ_PRM_BAYER_DECODER_ENABLE))
   {
      // Get Parameter of Bayer Decoder
      m_Acq->GetParameter(CORACQ_PRM_BAYER_DECODER_ENABLE, &bayerDecoder);
      if (isBayerAvailable && bayerDecoder && isFlatFieldAvailable)
         m_FlatFieldUseHardware = true;
   }

   ((CButton *)GetDlgItem(IDC_FLATFIELD_USE_HARDWARE))->SetCheck(m_FlatFieldUseHardware);

   m_FlatFieldPixelReplacement = isFlatFieldPixelReplacement;
   ((CButton *)GetDlgItem(IDC_FLATFIELD_PIXEL_REPLACEMENT))->SetCheck(m_FlatFieldPixelReplacement);

   if ((m_Acq && *m_Acq) && (isBayerAvailable && bayerDecoder))
      GetDlgItem(IDC_FLATFIELD_USE_HARDWARE)->EnableWindow(isBayerAvailable && bayerDecoder&& isFlatFieldAvailable);
   else
      GetDlgItem(IDC_FLATFIELD_USE_HARDWARE)->EnableWindow(isFlatFieldAvailable && isFlatFieldEnabled && bNoGrab);

   GetDlgItem(IDC_FLATFIELD_PIXEL_REPLACEMENT)->EnableWindow(isFlatFieldSoftware);


   //Bayer
   GetDlgItem(IDC_BAYER_CHECKBOX)->EnableWindow(isBayerAvailable);

   // File Options
   GetDlgItem(IDC_FILE_NEW)->EnableWindow(bNoGrab);
   GetDlgItem(IDC_FILE_LOAD)->EnableWindow(bNoGrab);
   GetDlgItem(IDC_FILE_SAVE)->EnableWindow(bNoGrab);

   // General Options
   GetDlgItem(IDC_BUFFER_OPTIONS)->EnableWindow(bNoGrab);
   GetDlgItem(IDC_VIEW_OPTIONS)->EnableWindow(bNoGrab);

   // If last control was disabled, set default focus
   if (!GetFocus())
      GetDlgItem(IDC_EXIT)->SetFocus();
}

BOOL CFlatFieldDemoDlg::CheckPixelFormat(char* mode)
{
   SapFormat format = m_Buffers->GetFormat();

   if (format != SapFormatMono8 && format != SapFormatMono16 && format != SapFormatMono8P2 && format != SapFormatMono8P3 && format != SapFormatMono8P4)
   {
      CString message;
      message.Format(_T("Pixel format must be 8-bit or 16-bit monochrome for %s\n"), (LPCTSTR)CString(mode));
      MessageBox(message);
      return FALSE;
   }

   return TRUE;
}


//*****************************************************************************************
//
//					Acquisition Control
//
//*****************************************************************************************
void CFlatFieldDemoDlg::OnFreeze()
{
   if (m_Xfer->Freeze())
   {
      if (CAbortDlg(this, m_Xfer).DoModal() != IDOK)
         m_Xfer->Abort();

      UpdateMenu();
   }
}

void CFlatFieldDemoDlg::OnGrab()
{
   m_statusWnd.SetWindowText(_T(""));

   if (m_Xfer->Grab())
   {
      UpdateMenu();
   }
}

void CFlatFieldDemoDlg::OnSnap()
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
//					Flat field Options
//
//*****************************************************************************************
#define DEFAULT_FFC_FILENAME			"FFC.tif"
#define STANDARD_FILTER					"TIFF Files (*.tif)|*.tif|CRC Files (*.crc)|*.crc||"

void CFlatFieldDemoDlg::OnFlatFieldEnable()
{
   CWaitCursor wait;

   UpdateData();

   // To enable/disable flat field correction, the transfer object must first be disconnected from the hardware
   if (m_Xfer && *m_Xfer)
      m_Xfer->Destroy();

   BOOL success = TRUE;

   // Check for invalid pixel format
   if (m_FlatFieldEnabled && !m_FlatFieldUseHardware)
      success = CheckPixelFormat("software correction");

   // Enable/disable flat field correction
   if (success)
      success = m_FlatField->Enable(m_FlatFieldEnabled, m_FlatFieldUseHardware);

   if (!success)
   {
      m_FlatFieldEnabled = !m_FlatFieldEnabled;
      UpdateData(FALSE);
   }

   if (m_Xfer && !*m_Xfer)
   {
      // Recreate the transfer object to reconnect it to the hardware
      if (!m_Xfer->Create() || !m_Xfer->Init(TRUE))
      {
         m_FlatFieldUseHardware = !m_FlatFieldUseHardware;
         m_FlatField->Enable(m_FlatFieldEnabled, m_FlatFieldUseHardware);
         m_Xfer->Create();
         m_Xfer->Init(TRUE);
         UpdateData(FALSE);
         if (!m_FlatFieldUseHardware)
            AfxMessageBox(_T("There was an error enabling hardware correction.  Software correction enabled in its place."));
      }
      m_Pro->Init();
   }

   UpdateMenu();
}

void CFlatFieldDemoDlg::OnFlatFieldUseHardware()
{
   CWaitCursor wait;

   UpdateData();

   // Get Parameter of Bayer Decoder
   int bayerDecoder;
   m_Acq->GetParameter(CORACQ_PRM_BAYER_DECODER_ENABLE, &bayerDecoder);

   if (m_Acq && *m_Acq && m_Acq->IsBayerAvailable())
   {
      if (bayerDecoder)
      {
         m_FlatFieldUseHardware = true;
         AfxMessageBox(_T("Hardware correction is always used when Bayer decoder is enabled."));
         UpdateData();
      }
   }

   OnFlatFieldEnable();

   UpdateMenu();
}

void CFlatFieldDemoDlg::OnFlatFieldPixelReplacement()
{
   UpdateData();

   // Enable/disable pixel replacement flat field correction
   m_FlatField->EnablePixelReplacement(m_FlatFieldPixelReplacement);

   UpdateMenu();
}

void CFlatFieldDemoDlg::OnFlatFieldCalibrate()
{
   if (!CheckPixelFormat("calibration"))
      return;

   CFlatFieldDlg dlg(this, m_FlatField, m_Xfer, m_Buffers);
   dlg.DoModal();
   UpdateMenu();
}

void CFlatFieldDemoDlg::OnFlatFieldLoad()
{
   CFileDialog dlgFFC(TRUE, _T(""), _T(DEFAULT_FFC_FILENAME), OFN_HIDEREADONLY | OFN_FILEMUSTEXIST, _T(STANDARD_FILTER), this);

   dlgFFC.m_ofn.lpstrTitle = _T("Open Flat Field Correction");
   if (dlgFFC.DoModal() == IDOK)
   {
      // Load flat field correction file
      if (!m_FlatField->Load(CStringA(dlgFFC.GetPathName())))
         return;
   }

   UpdateMenu();
}

void CFlatFieldDemoDlg::OnFlatFieldSave()
{
   CFileDialog dlgFFC(FALSE, _T(""), _T(DEFAULT_FFC_FILENAME), OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT, _T(STANDARD_FILTER), this);

   dlgFFC.m_ofn.lpstrTitle = _T("Save Flat Field Correction As");
   if (dlgFFC.DoModal() == IDOK)
   {
      // Save flat field correction file
      m_FlatField->Save(CStringA(dlgFFC.GetPathName()));
   }

   UpdateMenu();
}

//*****************************************************************************************
//
//					Acquisition Options
//
//*****************************************************************************************

void CFlatFieldDemoDlg::OnGeneralOptions()
{
   CAcqDlg dlg(this, m_Acq);
   dlg.DoModal();
}

void CFlatFieldDemoDlg::OnLoadAcqConfig()
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

      // Recreate objects
      if (!CreateObjects())
      {
         *m_Acq = acq;
         CreateObjects();
      }

      GetSignalStatus();

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

void CFlatFieldDemoDlg::OnBufferOptions()
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

void CFlatFieldDemoDlg::OnViewOptions()
{
   CViewDlg dlg(this, m_View);
   if (dlg.DoModal() == IDOK)
      m_ImageWnd.Refresh();
}

//*****************************************************************************************
//
//					File Options
//
//*****************************************************************************************

void CFlatFieldDemoDlg::OnFileNew()
{
   m_Buffers->Clear();
   InvalidateRect(NULL, FALSE);
}

void CFlatFieldDemoDlg::OnFileLoad()
{
   CLoadSaveDlg dlg(this, m_Buffers, TRUE);
   if (dlg.DoModal() == IDOK)
   {
      if (m_FlatField->IsEnabled() && m_FlatField->IsSoftware())
      {
         m_Pro->Execute();
      }

      InvalidateRect(NULL);
      UpdateWindow();
   }
}

void CFlatFieldDemoDlg::OnFileSave()
{
   CLoadSaveDlg dlg(this, m_Buffers, FALSE);

   dlg.DoModal();
}


//*****************************************************************************************
//
//					Status signal 
//
//*****************************************************************************************

void CFlatFieldDemoDlg::GetSignalStatus()
{
   SapAcquisition::SignalStatus signalStatus;

   if (m_Acq && m_Acq->IsSignalStatusAvailable())
   {
      if (m_Acq->GetSignalStatus(&signalStatus, SignalCallback, this))
         GetSignalStatus(signalStatus);
   }
}

void CFlatFieldDemoDlg::GetSignalStatus(SapAcquisition::SignalStatus signalStatus)
{
   m_IsSignalDetected = (signalStatus != SapAcquisition::SignalNone);

   if (m_IsSignalDetected)
   {
      SetWindowText(m_appTitle);
   }
   else
   {
      CString newTitle = m_appTitle;
      newTitle += " (No camera signal detected)";
      SetWindowText(newTitle);
   }
}



void CFlatFieldDemoDlg::OnBnClickedBayerCheckbox()
{
   //Bayer settings button
   BOOL bayerOutput = ((CButton *)GetDlgItem(IDC_BAYER_CHECKBOX))->GetCheck();

   if (bayerOutput)
   {
      m_FlatField->SetVideoType(SapAcquisition::VideoBayer);
   }
   else
   {
      m_FlatField->SetVideoType(SapAcquisition::VideoMono);
   }

}
