// HighFrameRateDlg.cpp : implementation file
//

#include "stdafx.h"
#include "gigeseqgrabdemo.h"
#include "HighFrameRateDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CHighFrameRateDlg dialog


CHighFrameRateDlg::CHighFrameRateDlg(CWnd* pParent /* = NULL */, DWORD nFramesPerCallback /* = 1 */, SapTransfer* phXfer)
	: CDialog(CHighFrameRateDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CHighFrameRateDlg)
	m_nFramesPerCallback = nFramesPerCallback;
	//}}AFX_DATA_INIT

	m_phXfer= phXfer;
}

void CHighFrameRateDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CHighFrameRateDlg)
	DDX_Control(pDX, IDC_SPIN_FRAMES_PER_CALLBACK, m_nFramesPerCallbackSpinCtrl);
	DDX_Text(pDX, IDC_FRAMES_PER_CALLBACK, m_nFramesPerCallback);
	DDV_MinMaxDWord(pDX, m_nFramesPerCallback, 1, 32767);
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CHighFrameRateDlg, CDialog)
	//{{AFX_MSG_MAP(CHighFrameRateDlg)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CHighFrameRateDlg message handlers

BOOL CHighFrameRateDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();

	m_nFramesPerCallbackSpinCtrl.SetRange( 1, 32767);

	if( m_phXfer && *m_phXfer)
	{
		// Check if on-board buffers is supported
		UINT32 capIntBuffers = CORXFER_VAL_NB_INT_BUFFERS_NONE;

      if (m_phXfer->IsCapabilityValid(CORXFER_CAP_NB_INT_BUFFERS))
         m_phXfer->GetCapability( CORXFER_CAP_NB_INT_BUFFERS, &capIntBuffers);

		if( ! (capIntBuffers & CORXFER_VAL_NB_INT_BUFFERS_AUTO))
		{
			// This feature is not supported; disable control
			//m_nFramesOnBoard= 2;
			//GetDlgItem( IDC_NFRAMES_ONBOARD)->EnableWindow( FALSE);
		}
	}

	UpdateData( FALSE);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CHighFrameRateDlg::OnOK() 
{
	UpdateData( TRUE);
	
	CDialog::OnOK();
}

void CHighFrameRateDlg::OnCancel() 
{
	CDialog::OnCancel();
}
