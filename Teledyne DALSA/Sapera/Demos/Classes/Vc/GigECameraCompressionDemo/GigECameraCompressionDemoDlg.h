// CameraCompressionDemoDlg.h : header file
//

#if !defined(AFX_CAMERACOMPRESSIONDEMODLG_H__82BFE149_F01E_11D1_AF74_00A0C91AC0FB__INCLUDED_)
#define AFX_CAMERACOMPRESSIONDEMODLG_H__82BFE149_F01E_11D1_AF74_00A0C91AC0FB__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "SapClassBasic.h"
#include "SapClassGui.h"
#include "SapMyProcessing.h"
#include "TemperatureLog.h"

/////////////////////////////////////////////////////////////////////////////
// CCameraCompressionDemoDlg dialog

class CCameraCompressionDemoDlg : public CDialog
{
// Construction
public:
   CCameraCompressionDemoDlg(CWnd* pParent = NULL); // standard constructor
   ~CCameraCompressionDemoDlg();

   BOOL CreateObjects(BOOL updateViewFormat = FALSE, SapFormat viewFormat = SapFormatUnknown);
   BOOL DestroyObjects();
   void UpdateMenu();
   void UpdateTitleBar();
   void UpdateViewBuffersViewFormat(SapFormat viewFormat);
   static void XferCallback(SapXferCallbackInfo *pInfo);
   static void ProCallback(SapProCallbackInfo*);
   static void ViewCallback(SapViewCallbackInfo*);

   // Dialog Data
   //{{AFX_DATA(CCameraCompressionDemoDlg)
   enum { IDD = IDD_CAMERACOMPRESSIONDEMO_DIALOG };
   CStatic     m_statusWnd;
   CScrollBar  m_verticalScr;
   CScrollBar  m_horizontalScr;
   CStatic     m_viewWnd;
   //}}AFX_DATA

   // ClassWizard generated virtual function overrides
   //{{AFX_VIRTUAL(CCameraCompressionDemoDlg)
protected:
   virtual void DoDataExchange(CDataExchange* pDX); // DDX/DDV support
   //}}AFX_VIRTUAL

   // Implementation
   HICON    m_hIcon;
   CString  m_appTitle;

   CImageWnd*     m_ImageWnd;
   SapAcqDevice*  m_AcqDevice;
   SapBuffer*     m_Buffers;
   SapBuffer*     m_BuffersView;
   SapTransfer*   m_Xfer;
   SapView*       m_View;
   SapMyProcessing*  m_Pro;
   JpegProcessing*   m_JpegProcessing;
   BOOL           m_CompressedImageAvailable;

   // Generated message map functions
   //{{AFX_MSG(CCameraCompressionDemoDlg)
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
   afx_msg void OnViewOptions();
   afx_msg void OnFileLoad();
   afx_msg void OnFileSave();
   afx_msg void OnEndSession(BOOL bEnding);
   afx_msg BOOL OnQueryEndSession();
   afx_msg void OnBnClickedCheckJpegEnable();
   afx_msg void OnEnKillfocusEditImageCompressionQuality();
   //}}AFX_MSG
   DECLARE_MESSAGE_MAP()

public:
   BOOL    m_JpegPresent;
   BOOL32  m_JpegEnabled;
   CButton m_JpegEnableButton;
   CSpinButtonCtrl m_SpinImageCompressionQuality;
   CEdit m_EditImageCompressionQuality;
   int   m_EditImageCompressionQualityValue;
   char m_TemperatureLogFilename[512];
   TemperatureLog* m_temperatureLog;
};


/////////////////////////////////////////////////////////////////////////////
// CLoadSaveHwJpegDlg dialog
class CLoadSaveHwJpegDlg : public CLoadSaveDlg
{
public:
   CLoadSaveHwJpegDlg(CWnd* pParent, SapBuffer *pBuffer, BOOL isFileOpen = TRUE, BOOL isSequence = FALSE)
      : CLoadSaveDlg(pParent, NULL, isFileOpen, isSequence)
   {
      m_pBuffersView = pBuffer;
   }

   // Operations
   // Only use INT_PTR starting with Visual Studio .NET 2003, since its presence depends on the
   // Platform SDK version with Visual Studio 6, and it is only needed for 64-bit compatibility anyway.
#if defined(_MSC_VER) && _MSC_VER >= 1300
   virtual INT_PTR DoModal()
#else
   virtual int DoModal();
#endif
   {
      // Set name and type
      m_ofn.lpstrFile = m_PathName;
      SetFilterIndex();

      // Call base class method
      int ret = (int) CFileDialog::DoModal();
      if (ret != IDOK) return ret;

      // Update type from filter index
      if (m_bSequence)
         m_Type = m_SequenceTypes[m_ofn.nFilterIndex - 1];
      else
         m_Type = m_StandardTypes[m_ofn.nFilterIndex - 1];

      // Update file object, only if JPEG is not selected
      if (m_Type != 4)
      {
         // Re-add buffer info that was omitted in the overloaded constructor
         m_pBuffer = m_pBuffersView;

         // Removing this line in JPEG will not start the JPEG quality dialog
         if (!UpdateBuffer()) return IDABORT;
      }

      // Save parameters to registry
      SaveSettings();

      return IDOK;
   }

   SapBuffer *m_pBuffersView;
};


//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CAMERACOMPRESSIONDEMODLG_H__82BFE149_F01E_11D1_AF74_00A0C91AC0FB__INCLUDED_)
