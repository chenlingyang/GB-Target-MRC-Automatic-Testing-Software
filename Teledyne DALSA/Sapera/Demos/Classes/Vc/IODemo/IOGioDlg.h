#if !defined(AFX_IOGIODLG_H__83AFC729_1070_45DC_9F53_D97A4A1307E1__INCLUDED_)
#define AFX_IOGIODLG_H__83AFC729_1070_45DC_9F53_D97A4A1307E1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// IOGioDlg.h : header file
//

#include "SapClassBasic.h"
#include "SapClassGui.h"
#include "resource.h"


#define MAX_GIO_DEVICE	12
#define DEVICE_SPACE	10

/////////////////////////////////////////////////////////////////////////////
// CGioMainDlg dialog

class CGioMainDlg : public CDialog
{

// Construction
public:
	CGioMainDlg::CGioMainDlg(CWnd* pParent = NULL);// standard constructor
	~CGioMainDlg();	// standard destructor

	int		m_ServerIndex;
	int		m_ResourceIndex;

	BOOL CreateObjects();
	BOOL DestroyObjects();

	// Dialog Data
	//{{AFX_DATA(CGioMainDlg)
	enum { IDD = IDD_SCG_GIO };
		// NOTE: the ClassWizard will add data members here
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CGioMainDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	CBrush m_bkBrush; 
	HICON		m_hIcon;
	CString  m_appTitle;

	SapGio				      *m_pGio[MAX_GIO_DEVICE]; // General IO objects
	CGioInputDlg		      *m_pDlgInput[MAX_GIO_DEVICE];
	CGioOutputDlg		      *m_pDlgOutput[MAX_GIO_DEVICE];
   CGioBidirectionalDlg    *m_pDlgBidirectional[MAX_GIO_DEVICE];
	UINT32 m_gioCount;
	const TCHAR* m_ServerName;

	// Generated message map functions
	//{{AFX_MSG(CGioMainDlg)
		// NOTE: the ClassWizard will add member functions here
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg void OnBnCloseClicked();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnDestroy();
	afx_msg void OnExit();
	afx_msg void OnTimer(UINT_PTR nIDEvent);
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_IOGIODLG_H__83AFC729_1070_45DC_9F53_D97A4A1307E1__INCLUDED_)
