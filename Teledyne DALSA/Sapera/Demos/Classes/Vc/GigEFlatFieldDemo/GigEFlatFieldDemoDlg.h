// GigEFlatFieldDemoDlg.h : header file
//

#if !defined(AFX_GIGEFLATFIELDDEMODLG_H__78628807_E420_4FAF_AB4E_B33FC80BC1DB__INCLUDED_)
#define AFX_GIGEFLATFIELDDEMODLG_H__78628807_E420_4FAF_AB4E_B33FC80BC1DB__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "SapClassBasic.h"
#include "SapClassGui.h"
#include "SapMyProcessing.h"

class SapFlatField;


/////////////////////////////////////////////////////////////////////////////
// CGigEFlatFieldDemoDlg dialog

class CGigEFlatFieldDemoDlg : public CDialog
{
// Construction
public:
   CGigEFlatFieldDemoDlg(CWnd* pParent = NULL);

   BOOL CreateObjects(BOOL createAcq = TRUE, BOOL createFlatField = TRUE);
   BOOL DestroyObjects(BOOL destroyAcq = TRUE, BOOL destroyFlatField = TRUE);

   void UpdateMenu();
   void UpdateTitleBar();
   BOOL SetCalibEnable(BOOL newCalibEnable);
   BOOL CheckPixelFormat(char* mode);

   static void XferCallback(SapXferCallbackInfo* pInfo);
   static void ProCallback( SapProCallbackInfo* pInfo);
   static void AcqDeviceCallback(SapAcqDeviceCallbackInfo* pInfo);
   static void ViewCallback(SapViewCallbackInfo *pInfo);

// Dialog Data
   //{{AFX_DATA(CGigEFlatFieldDemoDlg)
   enum { IDD = IDD_GIGEFLATFIELDDEMO_DIALOG };
   CStatic m_statusWnd;
   CScrollBar m_verticalScr;
   CScrollBar m_horizontalScr;
   CStatic m_viewWnd;
   BOOL m_FlatFieldEnabled;
   BOOL m_FlatFieldUseROI;
   BOOL m_FlatFieldUseHardware;
   BOOL m_FlatFieldPixelReplacement;
   //}}AFX_DATA

   // ClassWizard generated virtual function overrides
   //{{AFX_VIRTUAL(CGigEFlatFieldDemoDlg)
   protected:
   virtual void DoDataExchange(CDataExchange* pDX);   // DDX/DDV support
   //}}AFX_VIRTUAL

// Implementation
protected:
   HICON m_hIcon;
   CString m_appTitle;

   SapAcqDevice* m_AcqDevice;
   SapBuffer* m_Buffers;
   SapTransfer* m_Xfer;
   SapView* m_View;
   SapFlatField* m_FlatField;
   SapMyProcessing* m_Pro;
   CImageWnd* m_ImageWnd;

   BOOL m_CalibEnableAvailable;
   BOOL m_CalibEnable;
   BOOL m_ValueChangeEventAvailable;
   BOOL m_isGenie;

   // Generated message map functions
   //{{AFX_MSG(CGigEFlatFieldDemoDlg)
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
   afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
   afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
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
   afx_msg void OnFlatFieldEnable();
   afx_msg void OnFlatFieldCalibrate();
   afx_msg void OnFlatFieldLoad();
   afx_msg void OnFlatFieldSave();
   afx_msg void OnFlatFieldUseROI();
   afx_msg void OnFlatFieldUseHardware();
   afx_msg void OnFlatFieldPixelReplacement();
   afx_msg void OnEndSession(BOOL bEnding);
   afx_msg BOOL OnQueryEndSession();
   //}}AFX_MSG
   DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_GIGEFLATFIELDDEMODLG_H__78628807_E420_4FAF_AB4E_B33FC80BC1DB__INCLUDED_)
