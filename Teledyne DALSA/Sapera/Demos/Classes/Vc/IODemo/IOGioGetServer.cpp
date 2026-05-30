// IOGioGetServer.cpp : implementation file
//

#include "stdafx.h"
#include "SapClassBasic.h"
#include "SapClassGui.h"
#include "IOGioGetServer.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif


/////////////////////////////////////////////////////////////////////////////
// CGioServer dialog

CGioServer::CGioServer(CWnd* pParent /*=NULL*/)
	: CDialog(CGioServer::IDD, pParent)
{
	//{{AFX_DATA_INIT(CGioServer)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void CGioServer::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CGioServer)
	DDX_Control(pDX, IDC_SCG_COMBO_SERVER, m_Combo_Server);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CGioServer, CDialog)
	//{{AFX_MSG_MAP(CGioServer)
	ON_CBN_SELCHANGE(IDC_SCG_COMBO_SERVER, OnSelchangeComboServer)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CGioServer message handlers

BOOL CGioServer::OnInitDialog() 
{
	CDialog::OnInitDialog();	
	int i=0;
	int nbDevices=0;

	// fill board list
	for (i=1;i<SapManager::GetServerCount();i++)
	{
		char serverName[64];
		int index;
		int count = SapManager::GetResourceCount(i, SapManager::ResourceGio);
		SapManager::GetServerName(i,serverName,sizeof(serverName));
		if (count != 0) {
			index = m_Combo_Server.AddString(CString(serverName));
			nbDevices++;
			m_Combo_Server.SetItemData(index, i);
		}
	}

	// No GIO module for all servers detected
	if (nbDevices == 0)
	{
		AfxMessageBox(_T("No I/O device available."));
		exit(1);
	}

	UpdateData(FALSE);
	m_Combo_Server.SetCurSel(0);
	OnSelchangeComboServer();
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CGioServer::OnSelchangeComboServer() 
{
	int iSelection = m_Combo_Server.GetCurSel();
	m_Combo_Server.GetLBText(iSelection,m_ServerName);
   m_ServerIndex = (int)m_Combo_Server.GetItemData(iSelection);
}
