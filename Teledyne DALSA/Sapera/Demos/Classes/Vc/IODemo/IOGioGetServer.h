#if !defined(AFX_IOGIOGETSERVER_H__4E623B83_9A8A_4764_A9E5_49C2B5612061__INCLUDED_)
#define AFX_IOGIOGETSERVER_H__4E623B83_9A8A_4764_A9E5_49C2B5612061__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// IOGioGetServer.h : header file
//

#include "resource.h"

/////////////////////////////////////////////////////////////////////////////
// CGioServer dialog

class CGioServer : public CDialog
{
// Construction
public:
	CGioServer(CWnd* pParent);   // standard constructor
	int GetServerIndex() {return m_ServerIndex;};
	const TCHAR* GetServerName() {return m_ServerName;};

// Dialog Data
	//{{AFX_DATA(CGioServer)
	enum { IDD = IDD_SCG_GIO_DIALOG_SERVER };
	CComboBox	m_Combo_Server;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CGioServer)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	int m_ServerIndex;
	TCHAR m_ServerName[64]; 


// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CGioServer)
	virtual BOOL OnInitDialog();
	afx_msg void OnSelchangeComboServer();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_IOGIOSERVER_H__4E623B83_9A8A_4764_A9E5_49C2B5612061__INCLUDED_)
