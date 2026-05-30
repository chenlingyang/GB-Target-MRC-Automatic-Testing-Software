// IOGioDlg.cpp : implementation file
//

#include "stdafx.h"
#include "IOGioDlg.h"
#include "IOGioGetServer.h"
#include "resource.h"


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
	enum { IDD = IDD_SCG_ABOUTBOX };
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
// CGioMainDlg dialog


CGioMainDlg::CGioMainDlg(CWnd* pParent)
	: CDialog(CGioMainDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CGioMainDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);

   memset(m_pDlgInput, 0, sizeof(m_pDlgInput));
   memset(m_pDlgOutput, 0, sizeof(m_pDlgOutput));
   memset(m_pDlgBidirectional, 0, sizeof(m_pDlgBidirectional));
   memset(m_pGio, 0, sizeof(m_pGio)); 
	m_gioCount = 0;
}


CGioMainDlg::~CGioMainDlg()
{
	for (UINT32 iDevice = 0; (iDevice < MAX_GIO_DEVICE) && (iDevice < m_gioCount); iDevice++)
	{
      if (m_pDlgBidirectional[iDevice]!=NULL)
		{
			delete m_pDlgBidirectional[iDevice];
			m_pDlgBidirectional[iDevice] = NULL;
		}
		if (m_pDlgOutput[iDevice]!=NULL)
		{
			delete m_pDlgOutput[iDevice];
			m_pDlgOutput[iDevice] = NULL;
		}
		if (m_pDlgInput[iDevice]!=NULL)
		{
			delete m_pDlgInput[iDevice];
			m_pDlgInput[iDevice] = NULL;
		}
	   if( m_pGio[iDevice] != NULL)
      {
         m_pGio[iDevice]->Destroy();
         delete m_pGio[iDevice];
         m_pGio[iDevice] = NULL;
      }
	}	
}

void CGioMainDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CGioMainDlg)
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CGioMainDlg, CDialog)
	//{{AFX_MSG_MAP(CGioMainDlg)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_WM_TIMER()
	ON_WM_CTLCOLOR()
	ON_BN_CLICKED(IDC_BT_CLOSE, OnExit)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CGioMainDlg message handlers

BOOL CGioMainDlg::CreateObjects()
{
	CWaitCursor wait;

	// Create Gio objects
	// Loop for all resources
	for (UINT32 iDevice = 0; (iDevice < MAX_GIO_DEVICE) && (iDevice < m_gioCount); iDevice++)
	{
		SapLocation location(m_ServerIndex, iDevice);
	
			// SapGio Instanciate
			m_pGio[iDevice] = new SapGio(location);					

			// SapGio Create
			if (m_pGio[iDevice] && !*m_pGio[iDevice] && !m_pGio[iDevice]->Create())
			   {
				  DestroyObjects();
				  return FALSE;
			   }
	}
	return TRUE;
}

BOOL CGioMainDlg::DestroyObjects()
{
	// Destroy Gio objects
	// Loop for all resources
	for (UINT32 iDevice = 0; (iDevice < MAX_GIO_DEVICE) && (iDevice < m_gioCount); iDevice++)
	{
		if (m_pGio[iDevice] && *m_pGio[iDevice]) m_pGio[iDevice]->Destroy();
	}
	return TRUE;
}


BOOL CGioMainDlg::OnInitDialog()
{
	UINT32 windowOffset=70;
	BOOL status;

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

	// Show Server Dialog 
	CGioServer dlg(this);
	if (dlg.DoModal() == IDOK)
	{

		m_ServerIndex = dlg.GetServerIndex();
		m_ServerName = dlg.GetServerName();

		if ( m_ServerIndex != -1)
		{
			// Get all resources from SapManager
			m_gioCount = SapManager::GetResourceCount(m_ServerIndex, SapManager::ResourceGio);

			// Create all objects
			if (!CreateObjects()) { EndDialog(TRUE); return FALSE; }
					
			UINT32 capInput=0,capOutput=0;
			CString str="";

			if (m_gioCount == 0)
			{
				AfxMessageBox(_T("There is no General IO on this server"));
				exit(1);
			}
			else
			{

			UpdateData(FALSE);

			//Loop for all resources
			for (UINT32 iDevice = 0; (iDevice < MAX_GIO_DEVICE) && (iDevice < m_gioCount); iDevice++)
			{
				
				if (m_pGio[iDevice] == NULL)
				{
					AfxMessageBox(_T("Could not instantiate a SapGio object !!!"));
					return FALSE;
				}

				
				if (m_pGio[iDevice]->IsCapabilityValid(CORGIO_CAP_DIR_OUTPUT))
					status = m_pGio[iDevice]->GetCapability(CORGIO_CAP_DIR_OUTPUT,&capOutput);

				if (m_pGio[iDevice]->IsCapabilityValid(CORGIO_CAP_DIR_INPUT))
				status = m_pGio[iDevice]->GetCapability(CORGIO_CAP_DIR_INPUT,&capInput);

				// PC2Vision Interrupt Module not supported
            if (iDevice == 2 && _tcscmp(m_ServerName,_T("PC2-Vision_1"))==0)
				{}
				else
				{
               //bidirectional
				   if (capOutput && capInput)
               {
                  m_pDlgBidirectional[iDevice] = new CGioBidirectionalDlg(this, iDevice, m_pGio[iDevice]);
                  if (m_pDlgBidirectional[iDevice] != NULL)
                  {
						   m_pDlgBidirectional[iDevice]->Create();
							windowOffset = m_pDlgBidirectional[iDevice]->MoveWindow(windowOffset);
							m_pDlgBidirectional[iDevice]->ShowWindow(SW_SHOW);
						}
               }
					else if (capOutput)
					{

						m_pDlgOutput[iDevice] = new CGioOutputDlg(this, iDevice, m_pGio[iDevice]);

							if (m_pDlgOutput[iDevice] != NULL)
							{
								m_pDlgOutput[iDevice]->Create();
								windowOffset = m_pDlgOutput[iDevice]->MoveWindow(windowOffset);
								m_pDlgOutput[iDevice]->ShowWindow(SW_SHOW);
							}
					}
               else if (capInput)    
					{
						m_pDlgInput[iDevice] = new CGioInputDlg(this, iDevice, m_pGio[iDevice]);

						if (m_pDlgInput[iDevice] != NULL)
						{
							m_pDlgInput[iDevice]->Create();
							windowOffset = m_pDlgInput[iDevice]->MoveWindow(windowOffset);
							m_pDlgInput[iDevice]->ShowWindow(SW_SHOW);
						}
					}
				}
			}//end for
			}//end else
		}//end else
	}//end if
	else
	{
		exit(1);
	}

			
	RECT rect;
	GetClientRect(&rect);
	rect.bottom=rect.bottom;
	rect.right=windowOffset;
	MoveWindow(&rect,TRUE);

	CWnd* pTemp;
	pTemp = GetDlgItem(IDC_BT_CLOSE);
	CRect tmp;
	pTemp->GetWindowRect(tmp);
	int lButton = tmp.right - tmp.left;
	tmp.left = (rect.right / 2) - (lButton / 2);
	tmp.right = tmp.left + lButton;
	
	ScreenToClient(tmp);
    pTemp->MoveWindow(tmp);


	SetTimer(0,800,NULL);

	return TRUE;  // return TRUE  unless you set the focus to a control
}


void CGioMainDlg::OnTimer(UINT_PTR nIDEvent) 
{
	CDialog::OnTimer(nIDEvent);
	SetTimer(0,800,NULL);

	for (UINT iDevice=0; iDevice<m_gioCount; iDevice++)
	{
		if (m_pDlgInput[iDevice])
			m_pDlgInput[iDevice]->Update();
		if (m_pDlgOutput[iDevice])
			m_pDlgOutput[iDevice]->Update();
      if (m_pDlgBidirectional[iDevice])
			m_pDlgBidirectional[iDevice]->Update();
	}
}


void CGioMainDlg::OnPaint() 
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, (WPARAM) dc.GetSafeHdc(), 0);

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


HBRUSH CGioMainDlg::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor) 
{
	HBRUSH hbr = CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
	switch ( nCtlColor )
	{
		case CTLCOLOR_DLG:
		return (HBRUSH)(m_bkBrush.GetSafeHandle());	
		default:
			break;
	}
	
	return hbr;

}

//**********************************************************************************
//
//				Window related functions
//
//**********************************************************************************

void CGioMainDlg::OnSysCommand(UINT nID, LPARAM lParam)
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

void CGioMainDlg::OnDestroy() 
{
	CDialog::OnDestroy();

	// Destroy all objects
	DestroyObjects();

	// Delete all objects
	for (UINT32 iDevice = 0; (iDevice < MAX_GIO_DEVICE) && (iDevice < m_gioCount); iDevice++)
	{
		if (m_pGio[iDevice])
		{
		   delete m_pGio[iDevice];
         m_pGio[iDevice] = NULL;
      }
	}
}

// The system calls this to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CGioMainDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}


void CGioMainDlg::OnExit() 
{
	EndDialog(TRUE);
}

