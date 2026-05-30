// FlatFieldDemoDlg.h : header file
//

#if !defined(AFX_FLATFIELDDEMODLG_H__78628807_E420_4FAF_AB4E_B33FC80BC1DB__INCLUDED_)
#define AFX_FLATFIELDDEMODLG_H__78628807_E420_4FAF_AB4E_B33FC80BC1DB__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "SapClassBasic.h"
#include "SapClassGui.h"
#include "SapMyProcessing.h"

class SapFlatField;

/////////////////////////////////////////////////////////////////////////////
// CFlatFieldDemoDlg dialog

class CFlatFieldDemoDlg : public CDialog, public CImageExWndEventHandler
{
// Construction
public:
	CFlatFieldDemoDlg(CWnd* pParent = NULL);	// standard constructor

	BOOL CreateObjects();
	BOOL DestroyObjects();

	void UpdateMenu();
   BOOL CheckPixelFormat(char* mode);

	void GetSignalStatus();
	void GetSignalStatus(SapAcquisition::SignalStatus signalStatus);

	void FlatFieldEnable();

	static void SignalCallback(SapAcqCallbackInfo *pInfo);
	static void XferCallback(SapXferCallbackInfo *pInfo);
	static void ProCallback( SapProCallbackInfo *pInfo);

   void PixelChanged(int x, int y);
   void RoiChanged();

// Dialog Data
	//{{AFX_DATA(CFlatFieldDemoDlg)
	enum { IDD = IDD_FLATFIELDDEMO_DIALOG };
	CStatic	m_statusWnd;
	BOOL	m_FlatFieldEnabled;
	BOOL	m_FlatFieldUseHardware;
	BOOL	m_FlatFieldPixelReplacement;
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CFlatFieldDemoDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	HICON						m_hIcon;
	CString					m_appTitle;

   CImageExWnd          m_ImageWnd;
	SapAcquisition*		m_Acq;
	SapBuffer*				m_Buffers;
	SapTransfer*			m_Xfer;
	SapView*					m_View;
	SapFlatField*			m_FlatField;
   SapBayer*            m_Bayer;

	SapMyProcessing*		m_Pro;

   BOOL						m_IsSignalDetected;   // TRUE if camera signal is detected

	// Generated message map functions
	//{{AFX_MSG(CFlatFieldDemoDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnDestroy();
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnSnap();
	afx_msg void OnGrab();
	afx_msg void OnFreeze();
	afx_msg void OnGeneralOptions();
	afx_msg void OnLoadAcqConfig();
	afx_msg void OnBufferOptions();
	afx_msg void OnViewOptions();
	afx_msg void OnFileLoad();
	afx_msg void OnFileNew();
	afx_msg void OnFileSave();
	afx_msg void OnExit();
	afx_msg void OnFlatFieldEnable();
	afx_msg void OnFlatFieldCalibrate();
	afx_msg void OnFlatFieldLoad();
	afx_msg void OnFlatFieldSave();
	afx_msg void OnFlatFieldUseHardware();
	afx_msg void OnFlatFieldPixelReplacement();
   afx_msg void OnEndSession(BOOL bEnding);
   afx_msg BOOL OnQueryEndSession();
   afx_msg void OnBnClickedBayerCheckbox();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_FLATFIELDDEMODLG_H__78628807_E420_4FAF_AB4E_B33FC80BC1DB__INCLUDED_)
