// GigeCameraDemoDlg.h : header file
//

#if !defined(AFX_GIGECAMERADEMODLG_H__82BFE149_F01E_11D1_AF74_00A0C91AC0FB__INCLUDED_)
#define AFX_GIGECAMERADEMODLG_H__82BFE149_F01E_11D1_AF74_00A0C91AC0FB__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "SapClassBasic.h"
#include "SapClassGui.h"

/////////////////////////////////////////////////////////////////////////////
// CGigeCameraDemoDlg dialog

class CGigeCameraDemoDlg : public CDialog, public CImageExWndEventHandler
{
// Construction
public:
	CGigeCameraDemoDlg(CWnd* pParent = NULL);	// standard constructor

	BOOL CreateObjects();
	BOOL DestroyObjects();
	void UpdateMenu();
	static void XferCallback(SapXferCallbackInfo *pInfo);

   void PixelChanged(int x, int y);

// Dialog Data
	//{{AFX_DATA(CGigeCameraDemoDlg)
	enum { IDD = IDD_GIGECAMERADEMO_DIALOG };
	CStatic	m_statusWnd;
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CGigeCameraDemoDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	HICON		m_hIcon;
	CString  m_appTitle;

   CImageExWnd    m_ImageWnd;
	SapAcqDevice	*m_AcqDevice;
	SapBuffer		*m_Buffers;
	SapTransfer		*m_Xfer;
	SapView        *m_View;

	// Generated message map functions
	//{{AFX_MSG(CGigeCameraDemoDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnDestroy();
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnSnap();
	afx_msg void OnGrab();
	afx_msg void OnFreeze();
	afx_msg void OnLoadAcqConfig();
	afx_msg void OnBufferOptions();
	afx_msg void OnViewOptions();
	afx_msg void OnFileLoad();
	afx_msg void OnFileNew();
	afx_msg void OnFileSave();
	afx_msg void OnExit();
   afx_msg void OnEndSession(BOOL bEnding);
   afx_msg BOOL OnQueryEndSession();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_GIGECAMERADEMODLG_H__82BFE149_F01E_11D1_AF74_00A0C91AC0FB__INCLUDED_)
