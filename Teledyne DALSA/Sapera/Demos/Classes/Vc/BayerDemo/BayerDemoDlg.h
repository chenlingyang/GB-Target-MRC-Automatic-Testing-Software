// BayerDemoDlg.h : header file
//

#if !defined(AFX_BAYERDEMODLG_H__82BFE149_F01E_11D1_AF74_00A0C91AC0FB__INCLUDED_)
#define AFX_BAYERDEMODLG_H__82BFE149_F01E_11D1_AF74_00A0C91AC0FB__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "SapClassBasic.h"
#include "SapClassGui.h"
#include "SapMyProcessing.h"

/////////////////////////////////////////////////////////////////////////////
// CBayerDemoDlg dialog

class CBayerDemoDlg : public CDialog
{
// Construction
public:
   CBayerDemoDlg(CWnd* pParent = NULL);   // standard constructor

   BOOL CreateObjects();
   BOOL DestroyObjects();

   void UpdateMenu( void);
   void UpdateTitleBar();

   void GetSignalStatus();
   void GetSignalStatus(SapAcquisition::SignalStatus signalStatus);

   static void XferCallback(SapXferCallbackInfo *pInfo);
   static void ProCallback(SapProCallbackInfo *pInfo);
   static void SignalCallback(SapAcqCallbackInfo *pInfo);
	static void ViewCallback(SapViewCallbackInfo *pInfo);

// Dialog Data
   //{{AFX_DATA(CBayerDemoDlg)
	enum { IDD = IDD_BAYERDEMO_DIALOG };
	CComboBox	m_BayerOutputFormatCtrl;
   CScrollBar   m_verticalScr;
   CScrollBar   m_horizontalScr;
   CStatic   m_viewWnd;
   CStatic   m_statusWnd;
   BOOL   m_BayerEnabled;
	BOOL	m_BayerUseHardware;
	//}}AFX_DATA

   // ClassWizard generated virtual function overrides
   //{{AFX_VIRTUAL(CBayerDemoDlg)
   protected:
   virtual void DoDataExchange(CDataExchange* pDX);   // DDX/DDV support
   //}}AFX_VIRTUAL

// Implementation
protected:
   HICON   m_hIcon;
   CString m_appTitle;

   SapAcquisition *m_Acq;
   SapBuffer      *m_Buffers;
   SapBayer       *m_Bayer;
   SapTransfer    *m_Xfer;
   SapView        *m_View;
   CImageWnd      *m_ImageWnd;
   SapMyProcessing*m_Pro;

	CBayerOptionsDlg*		m_pBayerOptionsDlg;

   BOOL m_IsSignalDetected;   // TRUE if camera signal is detected
	BOOL m_IsInitialized;		// TRUE if bayer decoder enable control state has been initialized 

	static SapFormatInfo m_BayerOutputFormatValues[];

   // Generated message map functions
   //{{AFX_MSG(CBayerDemoDlg)
   virtual BOOL OnInitDialog();
   afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
   afx_msg void OnPaint();
   afx_msg HCURSOR OnQueryDragIcon();
   afx_msg void OnDestroy();
   afx_msg void OnMouseMove(UINT nFlags, CPoint point);
   afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
   afx_msg void OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
   afx_msg void OnMove(int x, int y);
   afx_msg void OnSize(UINT nType, int cx, int cy);
   afx_msg void OnSnap();
   afx_msg void OnGrab();
   afx_msg void OnFreeze();
   afx_msg void OnLoadAcqConfig();
   afx_msg void OnBufferOptions();
   afx_msg void OnFileLoad();
   afx_msg void OnFileNew();
   afx_msg void OnFileSave();
   afx_msg void OnFileSaveConverted();
   afx_msg void OnExit();
   afx_msg void OnBayerEnable();
   afx_msg void OnWhiteBalance();
   afx_msg void OnBayerOptions();
   afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
   afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
   afx_msg void OnViewOptions();
	afx_msg void OnBayerUseHardware();
	afx_msg void OnBayerOutputFormat();
   afx_msg void OnEndSession(BOOL bEnding);
   afx_msg BOOL OnQueryEndSession();
	//}}AFX_MSG

   DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_BAYERDEMODLG_H__82BFE149_F01E_11D1_AF74_00A0C91AC0FB__INCLUDED_)
