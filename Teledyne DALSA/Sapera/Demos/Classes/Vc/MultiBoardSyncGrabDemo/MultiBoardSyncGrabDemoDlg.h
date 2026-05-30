// MultiBoardSyncGrabDemoDlg.h : header file
//

#if !defined(AFX_MULTIBOARDSYNCGRABDEMODLG_H__82BFE149_F01E_11D1_AF74_00A0C91AC0FB__INCLUDED_)
#define AFX_MULTIBOARDSYNCGRABDEMODLG_H__82BFE149_F01E_11D1_AF74_00A0C91AC0FB__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "SapClassBasic.h"
#include "SapClassGui.h"

/////////////////////////////////////////////////////////////////////////////
// CMultiBoardSyncGrabDemoDlg dialog
/////////////////////////////////////////////////////////////////////////////
// C2Acq1xFerDlg dialog
// Derived class allows us to correctly retrieve the trash buffer handle
// for a SapBufferRoi object
class MySapBufferRoi : public SapBufferRoi
{
public:
   MySapBufferRoi(SapBuffer *pParent, int xmin=0, int ymin=0, int width=-1, int height=-1)
      : SapBufferRoi(pParent, xmin, ymin, width, height) {}
   virtual CORBUFFER GetTrash() const { return m_hTrashChild; }
};


class CMultiBoardSyncGrabDemoDlg : public CDialog
{
// Construction
public:
	CMultiBoardSyncGrabDemoDlg(CWnd* pParent = NULL);	// standard constructor

	BOOL CreateObjects();
	BOOL DestroyObjects();
	void UpdateMenu();
   void UpdateTitleBar();
	static void XferCallback(SapXferCallbackInfo *pInfo);
	static void SignalCallback(SapAcqCallbackInfo *pInfo);
   void GetSignalStatus();
   void GetSignalStatus(SapAcquisition::SignalStatus signalStatus);

// Dialog Data
	//{{AFX_DATA(CMultiBoardSyncGrabDemoDlg)
	enum { IDD = IDD_MULTIBOARDSYNCGRABDEMO_DIALOG };
	CStatic	m_statusWnd;
	CScrollBar	m_verticalScr;
	CScrollBar	m_horizontalScr;
	CStatic	m_viewWnd;
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMultiBoardSyncGrabDemoDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
   HICON		m_hIcon;
   CString	m_appTitle;
   UINT32   m_minWidth;
   UINT32   m_minHeight;
   UINT32   m_bufferWidth;
   UINT32   m_bufferHeight;
   UINT32   m_bufferFormat;

   CImageWnd*        m_ImageWnd;
   SapAcquisition*   m_Acq[2];
   
   MySapBufferRoi*   m_Buffers[2];
   SapBuffer*        m_Buffer ; 
      
   SapTransfer*      m_Xfer[2];
   SapView*          m_View;

	
   BOOL m_IsSignalDetected;   // TRUE if camera signal is detected

	// Generated message map functions
	//{{AFX_MSG(CMultiBoardSyncGrabDemoDlg)
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
	afx_msg void OnViewOptions();
	afx_msg void OnFileNew();
	afx_msg void OnExit();
   afx_msg void OnEndSession(BOOL bEnding);
   afx_msg BOOL OnQueryEndSession();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MULTIBOARDSYNCGRABDEMODLG_H__82BFE149_F01E_11D1_AF74_00A0C91AC0FB__INCLUDED_)
