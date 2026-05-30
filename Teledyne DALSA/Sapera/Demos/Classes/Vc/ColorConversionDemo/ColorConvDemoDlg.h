// ColorConvDemoDlg.h : header file
//

#if !defined(AFX_COLORCONVDEMODLG_H__82BFE149_F01E_11D1_AF74_00A0C91AC0FB__INCLUDED_)
#define AFX_COLORCONVDEMODLG_H__82BFE149_F01E_11D1_AF74_00A0C91AC0FB__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "SapClassBasic.h"
#include "SapClassGui.h"
#include "SapMyProcessing.h"

/////////////////////////////////////////////////////////////////////////////
// ColorConvDemoDlg dialog

class CColorConvDemoDlg : public CDialog
{
   // Construction
public:
   CColorConvDemoDlg(CWnd* pParent = NULL);   // standard constructor

   BOOL CreateObjects();
   BOOL DestroyObjects();

   void UpdateMenu( void);

   void GetSignalStatus();
   void GetSignalStatus(SapAcquisition::SignalStatus signalStatus);

   static void XferCallback(SapXferCallbackInfo *pInfo);
   static void ProCallback(SapProCallbackInfo *pInfo);
   static void SignalCallback(SapAcqCallbackInfo *pInfo);
   static void ViewCallback(SapViewCallbackInfo *pInfo);

   // Dialog Data
   //{{AFX_DATA(CColorConvDemoDlg)
   enum { IDD = IDD_COLORCONVDEMO_DIALOG };
   CComboBox   m_ColorConvOutputFormatCtrl;
   CScrollBar  m_verticalScr;
   CScrollBar  m_horizontalScr;
   CStatic  m_viewWnd;
   CStatic  m_statusWnd;
   enum
   {
      ColorConvChoiceNone,
      ColorConvChoiceHW,
      ColorConvChoiceSW
   };
   int m_ColorConvChoice;
   //}}AFX_DATA

   // ClassWizard generated virtual function overrides
   //{{AFX_VIRTUAL(CColorConvDemoDlg)
protected:
   virtual void DoDataExchange(CDataExchange* pDX);   // DDX/DDV support
   //}}AFX_VIRTUAL

   // Implementation
protected:
   HICON   m_hIcon;
   CString m_appTitle;

   SapAcquisition       *m_Acq;
   SapBuffer            *m_Buffers;
   SapColorConversion   *m_ColorConv;
   SapTransfer          *m_Xfer;
   SapView              *m_View;
   CImageWnd            *m_ImageWnd;
   SapMyProcessing      *m_Pro;

   CColorConversionOptionsDlg* m_pColorConvOptionsDlg;

   BOOL m_IsSignalDetected;   // TRUE if camera signal is detected
   BOOL m_IsInitialized;		// TRUE if bayer decoder enable control state has been initialized 

   static SapFormatInfo m_ColorConvOutputFormatValues[];

   void UpdateColorConvChoiceFromData();
   void UpdateOutputFormatList();
   void UpdateViewFormat();

   // Generated message map functions
   //{{AFX_MSG(CColorConvDemoDlg)
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
   afx_msg void OnColorConvChoiceChange();
   afx_msg void OnColorConvOptions();
   afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
   afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
   afx_msg void OnViewOptions();
   afx_msg void OnColorConvOutputFormat();
   afx_msg void OnEndSession(BOOL bEnding);
   afx_msg BOOL OnQueryEndSession();
   //}}AFX_MSG

   DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_COLORCONVDEMODLG_H__82BFE149_F01E_11D1_AF74_00A0C91AC0FB__INCLUDED_)
