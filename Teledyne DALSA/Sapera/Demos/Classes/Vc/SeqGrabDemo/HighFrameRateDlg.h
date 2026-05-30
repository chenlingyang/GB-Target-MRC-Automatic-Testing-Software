#if !defined(AFX_HIGHFRAMERATEDLG_H__C3450AD4_33A7_40E8_A0D5_56DBF2618697__INCLUDED_)
#define AFX_HIGHFRAMERATEDLG_H__C3450AD4_33A7_40E8_A0D5_56DBF2618697__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// HighFrameRateDlg.h : header file
//

#include "SapClassBasic.h"

/////////////////////////////////////////////////////////////////////////////
// CHighFrameRateDlg dialog

class CHighFrameRateDlg : public CDialog
{
// Construction
public:
	CHighFrameRateDlg(CWnd* pParent = NULL, DWORD nFramesPerCallback= 1, DWORD nFramesOnBoard= 2, SapTransfer* phXfer= NULL);   // standard constructor

	DWORD GetNFramesPerCallback() { return m_nFramesPerCallback;}
	DWORD GetNFramesOnBoard()     { return m_nFramesOnBoard;}

// Dialog Data
	//{{AFX_DATA(CHighFrameRateDlg)
	enum { IDD = IDD_HIGH_FRAME_RATE };
	CSpinButtonCtrl	m_nFramesOnBoardSpinCtrl;
	CSpinButtonCtrl	m_nFramesPerCallbackSpinCtrl;
	DWORD	m_nFramesPerCallback;
	DWORD	m_nFramesOnBoard;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CHighFrameRateDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CHighFrameRateDlg)
	virtual BOOL OnInitDialog();
	virtual void OnOK();
	virtual void OnCancel();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

private:
	SapTransfer* m_phXfer;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_HIGHFRAMERATEDLG_H__C3450AD4_33A7_40E8_A0D5_56DBF2618697__INCLUDED_)
