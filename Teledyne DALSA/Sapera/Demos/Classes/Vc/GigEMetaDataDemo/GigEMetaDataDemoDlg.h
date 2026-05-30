//==============================================================================
// Module : GigEMetaDataDemoDlg.h
// Project: Sapera demos.
// Purpose: Main header file for the GigEMetaDataDemoDlg application.
//==============================================================================
// Include files.


#ifndef _GigEMetaDataDemoDlgH
#define _GigEMetaDataDemoDlgH

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "SapClassGui.h"

//==============================================================================
// #define constants.

#define WM_UPDATE_CONTROLS    (WM_USER + 1)  // User message for updating controls
#define MAX_BUFFER            5

//==============================================================================
// CGigEMetaDataDemoDlg dialog

class CGigEMetaDataDemoDlg : public CDialog, public CImageExWndEventHandler
{
public:
   CGigEMetaDataDemoDlg(CWnd* pParent = NULL);

   static void XferCallback(SapXferCallbackInfo* pInfo);

   BOOL CreateObjects(bool createAcqDevice = true);
   BOOL DestroyObjects(bool destroyAcqDevice = true);

   void UpdateMenu(void);
   void UpdateFrameRate(void);
   void CheckForLastFrame(void);
   void ReadCameraTimestamp();

   void PixelChanged(int x, int y);
   void MarkerChanged();
   void InteractiveMarkerChanged(int lineIndex);

   void UpdateMetadataList();

   enum { IDD = IDD_GIGEMETADATADEMO_DIALOG };
   CButton     m_stopButton;
   CButton     m_pauseButton;
   CButton     m_playButton;
   CButton     m_recordButton;
   CSliderCtrl m_SliderCtrl;
   CString     m_LiveFrameRate;
   float       m_BufferFrameRate;
   float       m_MaxTime;
   float       m_MinTime;
   CString     m_TimestampCurrent;
   UINT64      m_TimestampBuffer;
   UINT64      m_TimestampBufferDelta;
   int         m_ActiveBuffer;
   int         m_BufferCount;
   int         m_Slider;
   int         m_strLenMax;

protected:
   virtual BOOL OnInitDialog();
   virtual void DoDataExchange(CDataExchange* pDX);


   // Implementation
protected:
   HICON          m_hIcon;
   CString        m_originalAppTitle;
   CString        m_appTitle;

   CImageExWnd    m_ImageWnd;
   SapAcqDevice*  m_AcqDevice;
   SapFeature*    m_Feature;
   SapMetadata*   m_Metadata;
   SapBuffer*     m_Buffers;
   SapTransfer*   m_Xfer;
   SapView*       m_View;
   SapXferFrameRateInfo *m_FrameRateInfo;

   SapMetadata::MetadataType m_metadataType;

   BOOL m_IsSignalDetected;   // TRUE if camera signal is detected
   BOOL m_IsAreaScan;

   // Button flags
   BOOL           m_bRecordOn;      // TRUE if recording
   BOOL           m_bPlayOn;        // TRUE if playing back
   BOOL           m_bPauseOn;       // TRUE if pausing

   BOOL m_XferDisconnectDuringSetup;
   BOOL m_IsSelectAvailable;

   int   m_nFramesPerCallback;

   int   m_MetadataFileIndex;

   BOOL* m_BufferIsValid;


public:
   CButton m_checkEnable;
   CCheckListBox m_checklistMetadata;
   CListBox m_listMetadataView;
   CListBox m_listMetadataView2;
   clock_t m_PlayLastClock;

   // Generated message map functions
   DECLARE_MESSAGE_MAP()

   afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
   afx_msg void OnPaint();
   afx_msg HCURSOR OnQueryDragIcon();
   afx_msg void OnDestroy();
   afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
   afx_msg void OnSize(UINT nType, int cx, int cy);
   afx_msg void OnTimer(UINT_PTR nIDEvent);
   afx_msg LRESULT OnUpdateControls(WPARAM, LPARAM);
   afx_msg void OnBnClickedFileLoad();
   afx_msg void OnBnClickedFileSave();
   afx_msg void OnBnClickedFileLoadCurrent();
   afx_msg void OnBnClickedFileSaveCurrent();
   afx_msg void OnBnClickedLoadAcqConfig();
   afx_msg void OnBnClickedHighFrameRate();
   afx_msg void OnBnClickedBufferOptions();
   afx_msg void OnKillfocusBufferFrameRate();
   afx_msg void OnBnClickedReadCurrentTimestamp();
   afx_msg void OnBnClickedResetTimestamp();
   afx_msg void OnBnClickedRecord();
   afx_msg void OnBnClickedPlay();
   afx_msg void OnBnClickedPause();
   afx_msg void OnBnClickedStop();
   afx_msg void OnBnClickedMetadataActiveMode();
   afx_msg void OnClBnChkChangeCheckListMetadata();
   afx_msg void OnBnClickedSaveMetadata();
   afx_msg void OnEndSession(BOOL bEnding);
   afx_msg BOOL OnQueryEndSession();
   virtual BOOL PreTranslateMessage(MSG* pMsg);
};

//==============================================================================
//{{AFX_INSERT_LOCATION}}
#endif
