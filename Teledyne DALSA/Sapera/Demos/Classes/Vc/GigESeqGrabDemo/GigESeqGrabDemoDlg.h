//==============================================================================
// Module : GigESeqAcqDemoDlg.h
// Project: Sapera demos.
// Purpose: Main header file for the GigESeqAcqDemoDlg application.
//==============================================================================
// Include files.


#ifndef _GigESeqAcqDemoDlgH
#define _GigESeqAcqDemoDlgH

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "SapClassBasic.h"
#include "SapClassGui.h"

//==============================================================================
// #define constants.

#define WM_UPDATE_CONTROLS    (WM_USER + 1)  // User message for updating controls
#define MAX_BUFFER            15

//==============================================================================
// CGigESeqGrabDemoDlg dialog

class CGigESeqGrabDemoDlg : public CDialog, public CImageExWndEventHandler
{
   // Construction
public:
   CGigESeqGrabDemoDlg(CWnd* pParent = NULL); // standard constructor

   BOOL CreateObjects(void);
   BOOL DestroyObjects(void);
   void UpdateMenu(void);
   void UpdateFrameRate(void);
   void CheckForLastFrame(void);
   void ReadCameraTimestamp();

   void PixelChanged(int x, int y); // override CImageExWndEventHandler::PixelChanged

   static void XferCallback(SapXferCallbackInfo *pInfo);

   // Dialog Data
   //{{AFX_DATA(CGigESeqGrabDemoDlg)
   enum { IDD = IDD_GIGESEQGRABDEMO_DIALOG };
   CButton	m_stopButton;
   CButton	m_pauseButton;
   CButton	m_playButton;
   CButton	m_recordButton;
   CSliderCtrl m_SliderCtrl;
   CScrollBar  m_verticalScr;
   CScrollBar  m_horizontalScr;
   CString     m_LiveFrameRate;
   float       m_BufferFrameRate;
   float       m_MaxTime;
   float       m_MinTime;
   int         m_ActiveBuffer;
   int         m_BufferCount;
   int         m_Slider;
   CString m_TimestampCurrent;
   UINT64 m_TimestampBuffer;
   UINT64 m_TimestampBufferDelta;
   //}}AFX_DATA

   // ClassWizard generated virtual function overrides
   //{{AFX_VIRTUAL(CGigESeqGrabDemoDlg)
protected:
   virtual void DoDataExchange(CDataExchange* pDX);   // DDX/DDV support
   //}}AFX_VIRTUAL

   // Implementation
protected:
   HICON          m_hIcon;
   CString        m_appTitle;

   CImageExWnd    m_ImageWnd;
   SapAcqDevice   *m_AcqDevice;
   SapBuffer      *m_Buffers;
   SapTransfer    *m_Xfer;
   SapView        *m_View;

   BOOL m_IsSignalDetected;   // TRUE if camera signal is detected

   // Button flags
   BOOL           m_bRecordOn;      // TRUE if recording
   BOOL           m_bPlayOn;        // TRUE if playing back
   BOOL           m_bPauseOn;       // TRUE if pausing

   int 		 m_nFramesPerCallback;

   // Generated message map functions
   //{{AFX_MSG(CGigESeqGrabDemoDlg)
   virtual BOOL OnInitDialog(void);
   virtual BOOL PreTranslateMessage(MSG* pMsg);
   afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
   afx_msg void OnPaint(void);
   afx_msg HCURSOR OnQueryDragIcon(void);
   afx_msg void OnDestroy(void);
   afx_msg void OnSize(UINT nType, int cx, int cy);
   afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
   afx_msg void OnTimer(UINT_PTR nIDEvent);
   afx_msg void OnRecord(void);
   afx_msg void OnPlay(void);
   afx_msg void OnPause(void);
   afx_msg void OnStop(void);
   afx_msg void OnBufferOptions(void);
   afx_msg void OnLoadAcqDeviceConfig(void);
   afx_msg void OnFileLoad(void);
   afx_msg void OnFileSave(void);
   afx_msg void OnKillfocusBufferFrameRate(void);
   afx_msg void OnFileLoadCurrent(void);
   afx_msg void OnFileSaveCurrent(void);
   afx_msg void OnHighFrameRate();
   afx_msg void OnEndSession(BOOL bEnding);
   afx_msg BOOL OnQueryEndSession();
   afx_msg void OnBnClickedCameraTimestampUpdate();
   afx_msg void OnBnClickedCameraTimestampReset();
   //}}AFX_MSG
   DECLARE_MESSAGE_MAP()

   // User messages
   afx_msg LRESULT OnUpdateControls(WPARAM, LPARAM);
   
};

//==============================================================================
//{{AFX_INSERT_LOCATION}}
#endif
