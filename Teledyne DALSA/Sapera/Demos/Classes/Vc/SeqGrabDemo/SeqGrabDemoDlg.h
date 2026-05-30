//==============================================================================
// Module : SeqAcqDemoDlg.h
// Project: Sapera demos.
// Purpose: Main header file for the SeqAcqDemoDlg application.
//==============================================================================
// Include files.


#ifndef _SeqAcqDemoDlgH
#define _SeqAcqDemoDlgH

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
// CSeqGrabDemoDlg dialog

class CSeqGrabDemoDlg : public CDialog, public CImageExWndEventHandler
{
   // Construction
public:
   CSeqGrabDemoDlg(CWnd* pParent = NULL); // standard constructor

   BOOL CreateObjects(void);
   BOOL DestroyObjects(void);
   void UpdateMenu(void);
   void UpdateFrameRate(void);
   void CheckForLastFrame(void);
   static void AcqCallback(SapAcqCallbackInfo *pInfo);
   static void XferCallback(SapXferCallbackInfo *pInfo);
   static void SignalCallback(SapAcqCallbackInfo *pInfo);
   void GetSignalStatus();
   void GetSignalStatus(SapAcquisition::SignalStatus signalStatus);

   void PixelChanged(int x, int y); // override CImageExWndEventHandler::PixelChanged

   // Dialog Data
   //{{AFX_DATA(CSeqGrabDemoDlg)
   enum { IDD = IDD_SEQGRABDEMO_DIALOG };
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
   unsigned int m_BufferCounterStamp;
   int         m_Slider;
   CString	m_Msg;
   //}}AFX_DATA

   // ClassWizard generated virtual function overrides
   //{{AFX_VIRTUAL(CSeqGrabDemoDlg)
protected:
   virtual void DoDataExchange(CDataExchange* pDX);   // DDX/DDV support
   //}}AFX_VIRTUAL

   // Implementation
protected:
   HICON          m_hIcon;
   CString        m_appTitle;

   CImageExWnd    m_ImageWnd;
   SapAcquisition *m_Acq;
   SapBuffer      *m_Buffers;
   SapTransfer		*m_Xfer;
   SapView        *m_View;

   BOOL m_IsSignalDetected;   // TRUE if camera signal is detected

   // Button flags
   BOOL           m_bRecordOn;      // TRUE if recording
   BOOL           m_bPlayOn;        // TRUE if playing back
   BOOL           m_bPauseOn;       // TRUE if pausing

   int 				m_nFramesPerCallback;
   int				m_nFramesOnBoard;
   int            m_nFramesLost;

   // Generated message map functions
   //{{AFX_MSG(CSeqGrabDemoDlg)
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
   afx_msg void OnLoadAcqConfig(void);
   afx_msg void OnFileLoad(void);
   afx_msg void OnFileSave(void);
   afx_msg void OnKillfocusBufferFrameRate(void);
   afx_msg void OnFileLoadCurrent(void);
   afx_msg void OnFileSaveCurrent(void);
   afx_msg void OnHighFrameRate();
   afx_msg void OnEndSession(BOOL bEnding);
   afx_msg BOOL OnQueryEndSession();
   //}}AFX_MSG
   DECLARE_MESSAGE_MAP()

   // User messages
   afx_msg LRESULT OnUpdateControls(WPARAM, LPARAM);
   
};

//==============================================================================
//{{AFX_INSERT_LOCATION}}
#endif
