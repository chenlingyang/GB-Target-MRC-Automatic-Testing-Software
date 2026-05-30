// GigEFlatFieldDemoDlg.cpp : implementation file
//

#include "stdafx.h"
#include "GigEFlatFieldDemo.h"
#include "GigEFlatFieldDemoDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

static char *g_CalibEnableFeatureName = "";
static char *g_ValueChangeEventName = "Feature Value Changed";


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
// CGigEFlatFieldDemoDlg dialog

CGigEFlatFieldDemoDlg::CGigEFlatFieldDemoDlg(CWnd* pParent)
: CDialog(CGigEFlatFieldDemoDlg::IDD, pParent)
{
   //{{AFX_DATA_INIT(CGigEFlatFieldDemoDlg)
   m_FlatFieldEnabled = FALSE;
   m_FlatFieldUseROI = FALSE;
   m_FlatFieldUseHardware = TRUE;
   m_FlatFieldPixelReplacement = TRUE;
   //}}AFX_DATA_INIT
   m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);

   m_AcqDevice = NULL;
   m_Buffers = NULL;
   m_Xfer = NULL;
   m_View = NULL;
   m_FlatField = NULL;
   m_Pro = NULL;
   m_ImageWnd = NULL;

   m_isGenie = FALSE;
   m_CalibEnableAvailable = FALSE;
   m_CalibEnable = FALSE;
   m_ValueChangeEventAvailable = FALSE;
}

void CGigEFlatFieldDemoDlg::DoDataExchange(CDataExchange* pDX)
{
   CDialog::DoDataExchange(pDX);
   //{{AFX_DATA_MAP(CGigEFlatFieldDemoDlg)
   DDX_Control(pDX, IDC_STATUS, m_statusWnd);
   DDX_Control(pDX, IDC_VERT_SCROLLBAR, m_verticalScr);
   DDX_Control(pDX, IDC_HORZ_SCROLLBAR, m_horizontalScr);
   DDX_Control(pDX, IDC_VIEW_WND, m_viewWnd);
   DDX_Check(pDX, IDC_FLATFIELD_ENABLE, m_FlatFieldEnabled);
   DDX_Check(pDX, IDC_FLATFIELD_USE_ROI, m_FlatFieldUseROI);
   DDX_Check(pDX, IDC_FLATFIELD_USE_HARDWARE, m_FlatFieldUseHardware);
   DDX_Check(pDX, IDC_FLATFIELD_PIXEL_REPLACEMENT, m_FlatFieldPixelReplacement);
   //}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CGigEFlatFieldDemoDlg, CDialog)
   //{{AFX_MSG_MAP(CGigEFlatFieldDemoDlg)
   ON_WM_SYSCOMMAND()
   ON_WM_PAINT()
   ON_WM_QUERYDRAGICON()
   ON_WM_DESTROY()
   ON_WM_MOUSEMOVE()
   ON_WM_HSCROLL()
   ON_WM_VSCROLL()
   ON_WM_MOVE()
   ON_WM_SIZE()
   ON_WM_LBUTTONDOWN()
   ON_WM_SETCURSOR()
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
   ON_BN_CLICKED(IDC_FLATFIELD_ENABLE, OnFlatFieldEnable)
   ON_BN_CLICKED(IDC_FLATFIELD_CALIBRATE, OnFlatFieldCalibrate)
   ON_BN_CLICKED(IDC_FLATFIELD_LOAD, OnFlatFieldLoad)
   ON_BN_CLICKED(IDC_FLATFIELD_SAVE, OnFlatFieldSave)
   ON_BN_CLICKED(IDC_FLATFIELD_USE_ROI, OnFlatFieldUseROI)
   ON_BN_CLICKED(IDC_FLATFIELD_USE_HARDWARE, OnFlatFieldUseHardware)
   ON_BN_CLICKED(IDC_FLATFIELD_PIXEL_REPLACEMENT, OnFlatFieldPixelReplacement)
   ON_WM_ENDSESSION()
   ON_WM_QUERYENDSESSION()
   //}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CGigEFlatFieldDemoDlg message handlers

//
// This function is called each time an image has been transferred into system memory by the transfer object
//
void CGigEFlatFieldDemoDlg::XferCallback(SapXferCallbackInfo* pInfo)
{
   CGigEFlatFieldDemoDlg* pDlg = (CGigEFlatFieldDemoDlg*)pInfo->GetContext();

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
      pDlg->UpdateTitleBar();
   }
}

//
// This function is called each time a buffer has been processed by the processing object
//
void CGigEFlatFieldDemoDlg::ProCallback(SapProCallbackInfo* pInfo)
{
   CGigEFlatFieldDemoDlg* pDlg = (CGigEFlatFieldDemoDlg*)pInfo->GetContext();
   CString str;

   // Check if flat field correction was enabled and if it's been done by software
   if (pDlg->m_FlatField->IsEnabled() && pDlg->m_FlatField->IsSoftware())
   {
      // Show current buffer index and execution time in milliseconds
      str.Format(_T("Flat field correction= %5.2f ms"), pDlg->m_Pro->GetTime());
   }

   // Refresh view
   pDlg->m_statusWnd.SetWindowText(str);
   pDlg->m_View->Show();
}

//
// This function is called each time a buffer has been shown by the view object
//
void CGigEFlatFieldDemoDlg::ViewCallback(SapViewCallbackInfo *pInfo)
{
   CGigEFlatFieldDemoDlg *pDlg = (CGigEFlatFieldDemoDlg *)pInfo->GetContext();

   if (pDlg->m_FlatFieldUseROI)
      pDlg->m_ImageWnd->DisplayRoiTracker();
   else
      pDlg->m_ImageWnd->HideRoiTracker();
}

//
// This function is called each time an acquisition device specific event happens,
// for example, when the value of a feature changes
//
void CGigEFlatFieldDemoDlg::AcqDeviceCallback(SapAcqDeviceCallbackInfo* pInfo)
{
   // Uncomment to display events received from the camera
   /*
      int eventIndex, featureIndex;
      char eventName[80], featureName[80];

      if (pInfo->GetEventIndex(&eventIndex))
      {
      if (pInfo->GetAcqDevice()->GetEventNameByIndex(eventIndex, eventName, sizeof(eventName)))
      {
      CString message = "Acquisition device event: ";
      message += eventName;
      message += "\n";

      if (CorStricmp(eventName, g_ValueChangeEventName) == 0 && pInfo->GetFeatureIndex(&featureIndex))
      {
      if (pInfo->GetAcqDevice()->GetFeatureNameByIndex(featureIndex, featureName, sizeof(featureName)))
      {
      message += "Feature name: ";
      message += featureName;
      message += "\n";
      }

      }

      AfxMessageBox(message);
      }
      }
      */
}


//***********************************************************************************
// Initialize Demo Dialog based application
//***********************************************************************************
BOOL CGigEFlatFieldDemoDlg::OnInitDialog()
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
   SetIcon(m_hIcon, FALSE);      // Set small icon
   SetIcon(m_hIcon, TRUE);       // Set big icon

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
   m_View = new SapView(m_Buffers, m_viewWnd.GetSafeHwnd(), ViewCallback, this);

   // Create all objects
   if (!CreateObjects())
   {
      EndDialog(TRUE);
      return FALSE;
   }

   if (!m_isGenie && !m_CalibEnableAvailable)
   {
      MessageBox(_T("Hardware based flat-field correction is not supported on this camera."));
   }

   // Create an image window object
   m_ImageWnd = new CImageWnd(m_View, &m_viewWnd, &m_horizontalScr, &m_verticalScr, this);

   UpdateMenu();
   return TRUE;  // return TRUE  unless you set the focus to a control
}

BOOL CGigEFlatFieldDemoDlg::CreateObjects(BOOL createAcq, BOOL createFlatField)
{
   CWaitCursor wait;

   if (createAcq)
   {
      // Create acquisition object
      if (m_AcqDevice && !*m_AcqDevice && !m_AcqDevice->Create())
      {
         DestroyObjects();
         return FALSE;
      }

      //GigE
      if (!m_AcqDevice->IsFeatureAvailable("flatfieldCorrectionMode", &m_CalibEnableAvailable))
      {
         DestroyObjects();
         return FALSE;
      }

      if (m_CalibEnableAvailable)
      {
         m_isGenie = FALSE;
         g_CalibEnableFeatureName = "flatfieldCorrectionMode";
      }
      else
      {
         //Genie
         if (!m_AcqDevice->IsFeatureAvailable("FlatFieldCalibrationEnable", &m_CalibEnableAvailable))
         {
            DestroyObjects();
            return FALSE;
         }

         if (m_CalibEnableAvailable)
         {
            m_isGenie = TRUE;
            g_CalibEnableFeatureName = "FlatFieldCalibrationEnable";
         }
      }

      if (!m_AcqDevice->IsEventAvailable(g_ValueChangeEventName, &m_ValueChangeEventAvailable))
      {
         DestroyObjects();
         return FALSE;
      }

      if (m_ValueChangeEventAvailable)
      {
         if (!m_AcqDevice->RegisterCallback(g_ValueChangeEventName, AcqDeviceCallback, this))
         {
            DestroyObjects();
            return FALSE;
         }
      }
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

   // Create transfer object
   if (m_Xfer && !*m_Xfer)
   {
      if (!m_Xfer->Create())
      {
         DestroyObjects();
         return FALSE;
      }
   }

   // Create flat field object
   if (createFlatField)
   {
      delete m_FlatField;
      if (m_AcqDevice && *m_AcqDevice && m_AcqDevice->IsFlatFieldAvailable())
         m_FlatField = new SapFlatField(m_AcqDevice);
      else
         m_FlatField = new SapFlatField(m_Buffers);

      if (m_FlatField && !*m_FlatField && !m_FlatField->Create())
      {
         DestroyObjects();
         return FALSE;
      }
   }

   // Define processing
   delete m_Pro;
   m_Pro = new SapMyProcessing(m_Buffers, m_FlatField, ProCallback, this);
   if (m_Pro && !*m_Pro)
   {
      if (!m_Pro->Create())
      {
         DestroyObjects();
         return FALSE;
      }

      m_Pro->SetAutoEmpty(TRUE);
      if (m_Xfer)
         m_Xfer->SetAutoEmpty(FALSE);
   }

   return TRUE;
}

BOOL CGigEFlatFieldDemoDlg::DestroyObjects(BOOL destroyAcq, BOOL destroyFlatField)
{
   if (m_Xfer && m_Xfer->IsGrabbing())
      OnFreeze();

   // Destroy processing object
   if (m_Pro && *m_Pro)
      m_Pro->Destroy();

   // Destroy flat field object
   if (destroyFlatField)
   {
      if (m_FlatField && *m_FlatField)
         m_FlatField->Destroy();
   }

   // Destroy transfer object
   if (m_Xfer && *m_Xfer)
      m_Xfer->Destroy();

   // Destroy view object
   if (m_View && *m_View)
      m_View->Destroy();

   // Destroy buffer object
   if (m_Buffers && *m_Buffers)
      m_Buffers->Destroy();

   if (destroyAcq)
   {
      if (m_AcqDevice && *m_AcqDevice)
      {
         // Unregister acquisition callback
         if (m_ValueChangeEventAvailable)
            m_AcqDevice->UnregisterCallback(g_ValueChangeEventName);

         // Disable flat-field calibration mode on the camera
         if (m_CalibEnableAvailable && m_CalibEnable)
         {
            m_AcqDevice->SetFeatureValue(g_CalibEnableFeatureName, m_isGenie ? FALSE : "Off");
            m_CalibEnable = FALSE;
         }

         // Destroy acquisition object
         m_AcqDevice->Destroy();
      }
   }

   // Disable flat field correction
   m_FlatFieldEnabled = FALSE;

   UpdateData(FALSE);
   UpdateMenu();

   return TRUE;
}

//**********************************************************************************
//
//            Window related functions
//
//**********************************************************************************
void CGigEFlatFieldDemoDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
   if ((nID & 0xFFF0) == IDM_ABOUTBOX)
   {
      CAboutDlg dlgAbout;
      dlgAbout.DoModal();
   }
   else
      CDialog::OnSysCommand(nID, lParam);
}


// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.
void CGigEFlatFieldDemoDlg::OnPaint()
{
   if (IsIconic())
   {
      CPaintDC dc(this);   // Device context for painting

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

void CGigEFlatFieldDemoDlg::OnDestroy()
{
   // Destroy all objects
   DestroyObjects();

   // Delete all objects
   if (m_ImageWnd)
   {
      delete m_ImageWnd;
      m_ImageWnd = NULL;
   }

   if (m_Pro)
   {
      delete m_Pro;
      m_Pro = NULL;
   }

   if (m_FlatField)
   {
      delete m_FlatField;
      m_FlatField = NULL;
   }

   if (m_Xfer)
   {
      delete m_Xfer;
      m_Xfer = NULL;
   }

   if (m_View)
   {
      delete m_View;
      m_View = NULL;
   }

   if (m_Buffers)
   {
      delete m_Buffers;
      m_Buffers = NULL;
   }

   if (m_AcqDevice)
   {
      delete m_AcqDevice;
      m_AcqDevice = NULL;
   }

   CDialog::OnDestroy();
}

void CGigEFlatFieldDemoDlg::OnMouseMove(UINT nFlags, CPoint point)
{
   CString str = m_appTitle;

   if (!nFlags)
      str += "  " + m_ImageWnd->GetPixelString(point);

   SetWindowText(str);

   CDialog::OnMouseMove(nFlags, point);
}

void CGigEFlatFieldDemoDlg::OnMove(int x, int y)
{
   CDialog::OnMove(x, y);

   if (m_ImageWnd)
      m_ImageWnd->OnMove();
}

void CGigEFlatFieldDemoDlg::OnSize(UINT nType, int cx, int cy)
{
   CDialog::OnSize(nType, cx, cy);

   if (m_ImageWnd)
      m_ImageWnd->OnSize();
}

void CGigEFlatFieldDemoDlg::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
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

void CGigEFlatFieldDemoDlg::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
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
HCURSOR CGigEFlatFieldDemoDlg::OnQueryDragIcon()
{
   return (HCURSOR)m_hIcon;
}

void CGigEFlatFieldDemoDlg::OnExit()
{
   EndDialog(TRUE);
}

void CGigEFlatFieldDemoDlg::OnEndSession(BOOL bEnding)
{
   CDialog::OnEndSession(bEnding);

   if (bEnding)
   {
      // If ending the session, free the resources.
      OnDestroy();
   }
}

BOOL CGigEFlatFieldDemoDlg::OnQueryEndSession()
{
   if (!CDialog::OnQueryEndSession())
      return FALSE;

   return TRUE;
}

//**************************************************************************************
// Updates the menu items enabling/disabling the proper items depending on the state
//  of the application
//**************************************************************************************
void CGigEFlatFieldDemoDlg::UpdateMenu(void)
{
   BOOL acqNoGrab = m_Xfer && *m_Xfer && !m_Xfer->IsGrabbing();
   BOOL acqGrab = m_Xfer && *m_Xfer && m_Xfer->IsGrabbing();
   BOOL noGrab = !m_Xfer || !m_Xfer->IsGrabbing();

   BOOL isFlatFieldEnabled = m_FlatField && *m_FlatField && m_FlatField->IsEnabled();
   BOOL isFlatFieldAvailable = m_AcqDevice && *m_AcqDevice && m_AcqDevice->IsFlatFieldAvailable();
   BOOL isFlatFieldSoftware = m_FlatField && *m_FlatField && m_FlatField->IsSoftware();
   BOOL isFlatFieldPixelReplacement = m_FlatField && *m_FlatField && m_FlatField->IsPixelReplacement();

   // Acquisition Control
   GetDlgItem(IDC_GRAB)->EnableWindow(acqNoGrab);
   GetDlgItem(IDC_SNAP)->EnableWindow(acqNoGrab);
   GetDlgItem(IDC_FREEZE)->EnableWindow(acqGrab);

   // Acquisition Options
   GetDlgItem(IDC_LOAD_ACQ_CONFIG)->EnableWindow(acqNoGrab);

   // Flat field correction
   GetDlgItem(IDC_FLATFIELD_ENABLE)->EnableWindow(noGrab);
   GetDlgItem(IDC_FLATFIELD_CALIBRATE)->EnableWindow(noGrab && !isFlatFieldEnabled);
   GetDlgItem(IDC_FLATFIELD_LOAD)->EnableWindow(noGrab && !isFlatFieldEnabled);
   GetDlgItem(IDC_FLATFIELD_SAVE)->EnableWindow(noGrab);

   m_FlatFieldEnabled = isFlatFieldEnabled;
   ((CButton *)GetDlgItem(IDC_FLATFIELD_ENABLE))->SetCheck(m_FlatFieldEnabled);

   m_FlatFieldUseHardware = isFlatFieldAvailable && !isFlatFieldSoftware;
   ((CButton *)GetDlgItem(IDC_FLATFIELD_USE_HARDWARE))->SetCheck(m_FlatFieldUseHardware);
   GetDlgItem(IDC_FLATFIELD_USE_HARDWARE)->EnableWindow(isFlatFieldAvailable && isFlatFieldEnabled && noGrab);

   m_FlatFieldPixelReplacement = isFlatFieldPixelReplacement;
   ((CButton *)GetDlgItem(IDC_FLATFIELD_PIXEL_REPLACEMENT))->SetCheck(m_FlatFieldPixelReplacement);
   GetDlgItem(IDC_FLATFIELD_PIXEL_REPLACEMENT)->EnableWindow(isFlatFieldSoftware);

   // File Options
   GetDlgItem(IDC_FILE_NEW)->EnableWindow(noGrab);
   GetDlgItem(IDC_FILE_LOAD)->EnableWindow(noGrab);
   GetDlgItem(IDC_FILE_SAVE)->EnableWindow(noGrab);

   // General Options
   GetDlgItem(IDC_BUFFER_OPTIONS)->EnableWindow(noGrab);
   GetDlgItem(IDC_VIEW_OPTIONS)->EnableWindow(noGrab);

   // If last control was disabled, set default focus
   if (!GetFocus())
      GetDlgItem(IDC_EXIT)->SetFocus();
}

void CGigEFlatFieldDemoDlg::UpdateTitleBar()
{
   // Update pixel information
   CString str = m_appTitle;
   CPoint point;
   GetCursorPos(&point);
   ScreenToClient(&point);
   str += "  " + m_ImageWnd->GetPixelString(point);

   SetWindowText(str);
}


// Set the flat-field calibration mode of the camera
BOOL CGigEFlatFieldDemoDlg::SetCalibEnable(BOOL newCalibEnable)
{
   BOOL success = TRUE;
   int enumIndex, enumCount = 0;
   char enumString[MAX_PATH];
   BOOL enumEnabled;
   BOOL calibModeValueAvailable = FALSE;

   if (m_CalibEnableAvailable)
   {
      SapFeature calibFeature(m_AcqDevice->GetLocation());
      calibFeature.Create();
      m_AcqDevice->GetFeatureInfo(g_CalibEnableFeatureName, &calibFeature);
      calibFeature.GetEnumCount(&enumCount);

      for (enumIndex = 0; enumIndex < enumCount; enumIndex++)
      {
         memset(enumString, 0, sizeof(enumString));
         enumEnabled = FALSE;
         calibFeature.GetEnumString(enumIndex, enumString, sizeof(enumString) - 1);
         calibFeature.IsEnumEnabled(enumIndex, &enumEnabled);
         if (CorStricmp(enumString, "Calibration") == 0 && enumEnabled)
            calibModeValueAvailable = TRUE;
      }

      calibFeature.Destroy();
   }

   if (newCalibEnable != m_CalibEnable && m_CalibEnableAvailable)
   {
      DestroyObjects(FALSE, FALSE);

      if (m_isGenie)
         success = m_AcqDevice->SetFeatureValue(g_CalibEnableFeatureName, newCalibEnable);
      else
         if (newCalibEnable && calibModeValueAvailable)
            success = m_AcqDevice->SetFeatureValue(g_CalibEnableFeatureName, "Calibration");
         else
            success = m_AcqDevice->SetFeatureValue(g_CalibEnableFeatureName, "Off");

      if (success)
      {
         success = CreateObjects(FALSE, FALSE);
         if (!success)
         {
            if (m_isGenie)
               success = m_AcqDevice->SetFeatureValue(g_CalibEnableFeatureName, m_CalibEnable);
            else
               if (m_CalibEnable && calibModeValueAvailable)
                  success = m_AcqDevice->SetFeatureValue(g_CalibEnableFeatureName, "Calibration");
               else
                  success = m_AcqDevice->SetFeatureValue(g_CalibEnableFeatureName, "Off");
            CreateObjects(FALSE, FALSE);
         }
      }

      if (success)
         m_CalibEnable = newCalibEnable;

      if (m_ImageWnd)
         m_ImageWnd->OnSize();
   }

   return success;
}

BOOL CGigEFlatFieldDemoDlg::CheckPixelFormat(char* mode)
{
   SapFormat format = m_Buffers->GetFormat();

   if (format != SapFormatMono8 && format != SapFormatMono16)
   {
      CString message;
      if (strcmp(mode, "warning") == 0)
      {
         message.Format(_T("Pixel Format will be automatically changed to Raw Bayer for flat field calibration."));
      }
      else
      {
         message.Format(_T("Pixel format must be 8-bit or 16-bit monochrome for %s\n")
            _T("(for Genie color, this corresponds to Bayer Raw8 in CamExpert)"), (LPCTSTR)CString(mode));
      }
      MessageBox(message);
      return FALSE;
   }

   return TRUE;
}


//*****************************************************************************************
//
//               Acquisition Control
//
//*****************************************************************************************
void CGigEFlatFieldDemoDlg::OnFreeze()
{
   if (m_Xfer && *m_Xfer)
   {
      if (m_Xfer->Freeze())
      {
         if (CAbortDlg(this, m_Xfer).DoModal() != IDOK)
            m_Xfer->Abort();

         UpdateMenu();
      }
   }
}

void CGigEFlatFieldDemoDlg::OnGrab()
{
   m_statusWnd.SetWindowText(_T(""));

   if (m_Xfer->Grab())
      UpdateMenu();
}

void CGigEFlatFieldDemoDlg::OnSnap()
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
//               Flat field Options
//
//*****************************************************************************************

#define DEFAULT_FFC_FILENAME  "FFC.tif"
#define STANDARD_FILTER       "TIFF Files (*.tif)|*.tif||"

void CGigEFlatFieldDemoDlg::OnFlatFieldEnable()
{
   CWaitCursor wait;

   UpdateData();

   // To enable/disable flat field correction, the transfer object must first be disconnected from the hardware
   if (m_Xfer && *m_Xfer)
      m_Xfer->Destroy();

   BOOL success = TRUE;

   // Check for invalid pixel format when using software correction
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
      m_Xfer->Create();
      m_Xfer->Init(TRUE);
      m_Pro->Init();
   }

   UpdateMenu();
}

void CGigEFlatFieldDemoDlg::OnFlatFieldUseROI()
{
   UpdateData();

   m_FlatField->ResetRegionOfInterest();

   if (!m_FlatFieldUseROI)
      m_ImageWnd->HideRoiTracker();

   m_ImageWnd->OnPaint();
}

void CGigEFlatFieldDemoDlg::OnFlatFieldUseHardware()
{
   CWaitCursor wait;

   UpdateData();

   // To enable/disable flat field correction, the transfer object must first be disconnected from the hardware
   if (m_Xfer && *m_Xfer)
      m_Xfer->Destroy();

   BOOL success = TRUE;

   // Check for invalid pixel format when using software correction
   if (m_FlatFieldEnabled && !m_FlatFieldUseHardware)
      success = CheckPixelFormat("software correction");

   // Enable/disable flat field correction
   if (success)
      success = m_FlatField->Enable(m_FlatFieldEnabled, m_FlatFieldUseHardware);

   if (!success)
   {
      m_FlatFieldUseHardware = !m_FlatFieldUseHardware;
      UpdateData(FALSE);
   }

   if (m_Xfer && !*m_Xfer)
   {
      // Recreate the transfer object to reconnect it to the hardware
      m_Xfer->Create();
      m_Xfer->Init(TRUE);
      m_Pro->Init();
   }

   UpdateMenu();
}

void CGigEFlatFieldDemoDlg::OnFlatFieldPixelReplacement()
{
   UpdateData();

   // Enable/disable pixel replacement flat field correction
   m_FlatField->EnablePixelReplacement(m_FlatFieldPixelReplacement);

   UpdateMenu();
}

void CGigEFlatFieldDemoDlg::OnFlatFieldCalibrate()
{
   if (!m_isGenie && m_AcqDevice->IsFlatFieldAvailable())
   {
      UINT64 width, height, sensorWidth, sensorHeight;
      BOOL isInvalidSize;
      if (!m_AcqDevice->GetFeatureValue("Width", &width))
         return;
      if (!m_AcqDevice->GetFeatureValue("Height", &height))
         return;
      if (!m_AcqDevice->GetFeatureValue("SensorWidth", &sensorWidth))
         return;
      if (!m_AcqDevice->GetFeatureValue("SensorHeight", &sensorHeight))
         return;
      if (sensorHeight == 1)
         isInvalidSize = (width != sensorWidth);
      else
         isInvalidSize = (width != sensorWidth || height != sensorHeight);
      if (isInvalidSize)
      {
         MessageBox(_T("Flat-field calibration can only be performed on the full sensor size.\nMake sure that image cropping and binning are disabled."));
         return;
      }
   }

   CheckPixelFormat("warning");

   // Flat-field calibration mode must be enabled on the camera
   if (SetCalibEnable(TRUE))
   {
      CFlatFieldDlg dlg(this, m_FlatField, m_Xfer, m_Buffers);
      dlg.DoModal();
      SetCalibEnable(FALSE);
      UpdateMenu();
   }
}

void CGigEFlatFieldDemoDlg::OnFlatFieldLoad()
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

void CGigEFlatFieldDemoDlg::OnFlatFieldSave()
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
//               Acquisition Options
//
//*****************************************************************************************

void CGigEFlatFieldDemoDlg::OnLoadAcqConfig()
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

      m_ImageWnd->OnSize();
      InvalidateRect(NULL);
      UpdateWindow();
      UpdateMenu();
   }
}


//*****************************************************************************************
//
//               General Options
//
//*****************************************************************************************

void CGigEFlatFieldDemoDlg::OnBufferOptions()
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

void CGigEFlatFieldDemoDlg::OnViewOptions()
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

void CGigEFlatFieldDemoDlg::OnFileNew()
{
   m_Buffers->Clear();
   InvalidateRect(NULL, FALSE);
}

void CGigEFlatFieldDemoDlg::OnFileLoad()
{
   CLoadSaveDlg dlg(this, m_Buffers, TRUE);

   if (dlg.DoModal() == IDOK)
   {
      if (m_FlatField->IsEnabled() && m_FlatField->IsSoftware())
         m_Pro->Execute();

      InvalidateRect(NULL);
      UpdateWindow();
   }
}

void CGigEFlatFieldDemoDlg::OnFileSave()
{
   CLoadSaveDlg dlg(this, m_Buffers, FALSE);
   dlg.DoModal();
}

//**************************************************************************************
//
//         ROI tracking
//
//**************************************************************************************


void CGigEFlatFieldDemoDlg::OnLButtonDown(UINT nFlags, CPoint point)
{
   if (!m_FlatFieldUseROI)
      return;

   if (m_ImageWnd && m_ImageWnd->OnLButtonDown(point))
   {
      CRect roi = m_ImageWnd->GetSelectedRoi();
      m_FlatField->SetRegionOfInterest(roi.left, roi.top, roi.Width(), roi.Height());

      // Update controls
      InvalidateRect(NULL, FALSE);
      UpdateMenu();
   }

   CDialog::OnLButtonDown(nFlags, point);
}


BOOL CGigEFlatFieldDemoDlg::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message)
{
   if (m_FlatFieldUseROI && m_ImageWnd && m_ImageWnd->OnSetCursor(nHitTest))
      return TRUE;

   return CDialog::OnSetCursor(pWnd, nHitTest, message);
}
