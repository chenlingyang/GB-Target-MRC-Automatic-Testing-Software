using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.SapClassGui;

namespace DALSA.SaperaLT.Demos.NET.CSharp.ColorConvDemo
{
   public partial class ColorConvDemoDlg : Form
   {

      private static SapFormatInfo[] m_ColorConvOutputFormatValues = { 
											new SapFormatInfo(SapFormat.RGB888, "RGB 888"), 
											new SapFormatInfo(SapFormat.RGB8888, "RGB 8888"), 
											new SapFormatInfo(SapFormat.RGB101010, "RGB 101010"), 
											new SapFormatInfo(SapFormat.RGB16161616, "RGB 16161616") 
																  };

      private SapAcquisition m_Acquisition;
      private SapBuffer m_Buffers;
      private SapAcqToBuf m_Xfer;
      private SapView m_View;
      private SapColorConversion m_ColorConv;
      private SapProcessing m_Pro;
      private SapLocation m_ServerLocation;
      private String m_ConfigFileName;
      private String m_Executetime;
      private bool m_IsSignalDetected;
      private bool m_online;


      //System menu
      private SystemMenu m_SystemMenu;
      //index for "about this.." item im system menu
      private const int m_AboutID = 0x100;

      // Delegate to display number of frame acquired 
      // Delegate is needed because .NEt framework does not support cross thread control modification
      private delegate void DisplayFrameAcquired(int number, bool trash);

      static void xfer_XferNotify(object sender, SapXferNotifyEventArgs argsNotify)
      {
         ColorConvDemoDlg demo = argsNotify.Context as ColorConvDemoDlg;
         // If grabbing in trash buffer, do not display the image, update the
         // appropriate number of frames on the status bar instead
         if (argsNotify.Trash)
            demo.Invoke(new DisplayFrameAcquired(demo.ShowFrameNumber), argsNotify.EventCount, true);
         // Refresh view
         else
         {
            demo.Invoke(new DisplayFrameAcquired(demo.ShowFrameNumber), argsNotify.EventCount, false);
            demo.m_Pro.ExecuteNext();
         }
      }

      //
      // This function is called each time a buffer has been processed by the processing object
      //
      static void ProCallback(object sender, SapProcessingDoneEventArgs pInfo)
      {
         ColorConvDemoDlg demo = pInfo.Context as ColorConvDemoDlg;

         // Check if color conversion was enabled and if it's been done by software
         if (demo.m_ColorConv.SoftwareEnabled)
         {
            // Show current buffer index and execution time in millisecond
            demo.m_Executetime = String.Format("Color conversion = {0:0.00}", demo.m_Pro.Time);
         }
         else
            demo.m_Executetime = "";

         // Refresh view
         demo.m_View.Show();
      }

      //
      // This function is called each time a buffer has been shown by the view object
      //
      static void ViewCallback(object sender, SapDisplayDoneEventArgs pInfo)
      {
         ColorConvDemoDlg demo = pInfo.Context as ColorConvDemoDlg;

         if (!demo.m_ImageBox.IsTrackerEmpty)
            demo.m_ImageBox.DisplayTracker();
      }

      static void GetSignalStatus(object sender, SapSignalNotifyEventArgs argsSignal)
      {
         ColorConvDemoDlg demo = argsSignal.Context as ColorConvDemoDlg;
         SapAcquisition.AcqSignalStatus signalStatus = argsSignal.SignalStatus;

         demo.m_IsSignalDetected = (signalStatus != SapAcquisition.AcqSignalStatus.None);
         if (demo.m_IsSignalDetected == false)
            demo.StatusLabelInfo.Text = "Online... No camera signal detected";
         else demo.StatusLabelInfo.Text = "Online... Camera signal detected";
      }

      public ColorConvDemoDlg()
      {
         m_Acquisition = null;
         m_Buffers = null;
         m_Xfer = null;
         m_View = null;
         m_ColorConv = null;
         m_Pro = null;
         m_Executetime = "";

         InitializeComponent();

         // Note:
         //  The code to initialize m_ImageBox was originally in the InitializeComponent function
         //  called above. However, it has been moved to the dialog constructor as a workaround
         //  to a Visual Studio Designer error when loading the DALSA.SaperaLT.SapClassBasic
         //  assembly under 64-bit Windows.
         //  As a consequence, it is not possible to adjust the m_ImageBox properties
         //  automatically using the Designer anymore, this has to be done manually.
         // 
         this.m_ImageBox = new DALSA.SaperaLT.SapClassGui.ImageBox();
         this.m_ImageBox.Location = new System.Drawing.Point(254, 69);
         this.m_ImageBox.Name = "m_ImageBox";
         this.m_ImageBox.PixelValueDisplay = this.PixelDataValue;
         this.m_ImageBox.Size = new System.Drawing.Size(387, 356);
         this.m_ImageBox.SliderEnable = true;
         this.m_ImageBox.SliderMaximum = 10;
         this.m_ImageBox.SliderMinimum = 0;
         this.m_ImageBox.SliderValue = 0;
         this.m_ImageBox.SliderVisible = false;
         this.m_ImageBox.TabIndex = 23;
         this.m_ImageBox.TrackerEnable = false;
         this.m_ImageBox.View = null;
         this.Controls.Add(this.m_ImageBox);

         AcqConfigDlg acConfigDlg = new AcqConfigDlg(null, "", AcqConfigDlg.ServerCategory.ServerAcq);
         if (acConfigDlg.ShowDialog() == DialogResult.OK)
            m_online = true;
         else
            m_online = false;

         if (!CreateNewObjects(acConfigDlg, false))
            this.Close();

      }

      private void ShowFrameNumber(int number, bool trash)
      {
         String str;
         if (trash)
         {
            if (string.IsNullOrEmpty(m_Executetime))
            {
               str = String.Format("Frames acquired in trash buffer: {0}", number * m_Buffers.Count);
               this.StatusLabelInfoTrash.Text = str;
            }
         }
         else
         {

            if (!string.IsNullOrEmpty(m_Executetime))
               this.StatusLabelInfo.Text = m_Executetime;
            else
            {
               str = String.Format("Frames acquired :{0}", number * m_Buffers.Count);
               this.StatusLabelInfo.Text = str;
            }
         }
      }

      //*****************************************************************************************
      //
      //					Create and Destroy Object
      //
      //*****************************************************************************************

      // Create new objects with acquisition information
      public bool CreateNewObjects(AcqConfigDlg acConfigDlg, bool Restore)
      {
         if (m_online)
         {
            if (!Restore)
            {
               m_ServerLocation = acConfigDlg.ServerLocation;
               m_ConfigFileName = acConfigDlg.ConfigFile;
            }
            // define on-line object
            m_Acquisition = new SapAcquisition(m_ServerLocation, m_ConfigFileName);
            if (SapBuffer.IsBufferTypeSupported(m_ServerLocation, SapBuffer.MemoryType.ScatterGather))
               m_Buffers = new SapBufferWithTrash(2, m_Acquisition, SapBuffer.MemoryType.ScatterGather);
            else
               m_Buffers = new SapBufferWithTrash(2, m_Acquisition, SapBuffer.MemoryType.ScatterGatherPhysical);
            m_Xfer = new SapAcqToBuf(m_Acquisition, m_Buffers);
            m_ColorConv = new SapColorConversion(m_Acquisition, m_Buffers);


            //event for transfer
            m_Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            m_Xfer.XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
            m_Xfer.XferNotifyContext = this;

            // event for signal status
            m_Acquisition.SignalNotify += new SapSignalNotifyHandler(GetSignalStatus);
            m_Acquisition.SignalNotifyContext = this;
         }
         else
         {
            //define off-line object
            m_Buffers = new SapBuffer();
            m_ColorConv = new SapColorConversion(m_Buffers);
            StatusLabelInfo.Text = "offline... Load images";
         }

         m_View = new SapView(m_Buffers);
         //event for view  
         m_View.DisplayDone += new SapDisplayDoneHandler(ViewCallback);
         m_View.DisplayDoneContext = this;
         m_View.DisplayDoneEnable = true;
         m_Pro = new SapMyProcessing(m_Buffers, m_ColorConv, new SapProcessingDoneHandler(ProCallback), this);

         //Set SaperaView to imagebox control 
         m_ImageBox.View = m_View;

         if (!CreateObjects())
         {
            DisposeObjects();
            return false;
         }

         // Resize ImagBox to take into account the size of created sapview
         m_ImageBox.OnSize();
         EnableSignalStatus();
         UpdateControls();
         return true;
      }

      // Create new objects with acquisition information
      public bool CreateNewObjects()
      {
         if (m_online)
         {
            // define on-line object
            m_Acquisition = new SapAcquisition(m_ServerLocation, m_ConfigFileName);
            if (SapBuffer.IsBufferTypeSupported(m_ServerLocation, SapBuffer.MemoryType.ScatterGather))
               m_Buffers = new SapBufferWithTrash(2, m_Acquisition, SapBuffer.MemoryType.ScatterGather);
            else
               m_Buffers = new SapBufferWithTrash(2, m_Acquisition, SapBuffer.MemoryType.ScatterGatherPhysical);
            m_Xfer = new SapAcqToBuf(m_Acquisition, m_Buffers);
            m_ColorConv = new SapColorConversion(m_Acquisition, m_Buffers);


            //event for view
            m_Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            m_Xfer.XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
            m_Xfer.XferNotifyContext = this;

            // event for signal status
            m_Acquisition.SignalNotify += new SapSignalNotifyHandler(GetSignalStatus);
            m_Acquisition.SignalNotifyContext = this;
         }
         else
         {
            //define off-line object
            m_Buffers = new SapBuffer();
            m_ColorConv = new SapColorConversion(m_Buffers);
            StatusLabelInfo.Text = "offline... Load images";
         }

         m_View = new SapView(m_Buffers);
         //event for view 
         m_View.DisplayDone += new SapDisplayDoneHandler(ViewCallback);
         m_View.DisplayDoneContext = this;
         m_View.DisplayDoneEnable = true;
         m_Pro = new SapMyProcessing(m_Buffers, m_ColorConv, new SapProcessingDoneHandler(ProCallback), this);

         //Set SaperaView to imagebox control 
         m_ImageBox.View = m_View;

         if (!CreateObjects())
         {
            DisposeObjects();
            return false;
         }

         // Resize ImagBox to take into account the size of created sapview
         m_ImageBox.OnSize();
         EnableSignalStatus();
         UpdateControls();
         return true;
      }


      // Call Create Object 
      private bool CreateObjects()
      {
         // Create acquisition object
         if (m_Acquisition != null && !m_Acquisition.Initialized)
         {
            if (m_Acquisition.Create() == false)
            {
               DestroyObjects();
               return false;
            }
         }

         // Create buffer object
         if (m_Buffers != null && !m_Buffers.Initialized)
         {
            if (m_Buffers.Create() == false)
            {
               DestroyObjects();
               return false;
            }
            m_Buffers.Clear();
         }

         // Create color conversion object
         if (m_ColorConv != null && !m_ColorConv.Initialized)
         {
            if (m_ColorConv.Create() == false)
            {
               DestroyObjects();
               return false;
            }
         }

         // Create view object
         if (m_View != null && !m_View.Initialized)
         {
            if (m_ColorConv != null && m_ColorConv.Initialized)
            {
               // Set buffer to be viewed
               // When using hardware color decoder, view the acquired RGB buffer,
               // otherwise, for software color conversion, view the converted RGB buffer.
               SapBuffer convBuffer = m_ColorConv.OutputBuffer;
               if (convBuffer != null && convBuffer.Initialized)
                  m_View.Buffer = convBuffer;
               else
                  m_View.Buffer = m_Buffers;
            }

            if (m_View.Create() == false)
            {
               DestroyObjects();
               return false;
            }
         }
         // Create Xfer object
         if (m_Xfer != null && !m_Xfer.Initialized)
         {
            if (m_Xfer.Create() == false)
            {
               DestroyObjects();
               return false;
            }
            m_Xfer.AutoEmpty = false;
         }

         // Create processing object
         if (m_Pro != null && !m_Pro.Initialized)
         {
            if (!m_Pro.Create())
            {
               DestroyObjects();
               return false;
            }

            m_Pro.AutoEmpty = true;
         }
         return true;
      }

      private void DestroyObjects()
      {
         if (m_Xfer != null && m_Xfer.Initialized)
            m_Xfer.Destroy();
         if (m_Pro != null && m_Pro.Initialized)
            m_Pro.Destroy();
         if (m_View != null && m_View.Initialized)
            m_View.Destroy();
         if (m_ColorConv != null && m_ColorConv.Initialized)
            m_ColorConv.Destroy();
         if (m_Buffers != null && m_Buffers.Initialized)
            m_Buffers.Destroy();
         if (m_Acquisition != null && m_Acquisition.Initialized)
            m_Acquisition.Destroy();

      }

      private void DisposeObjects()
      {
         if (m_Xfer != null)
         { m_Xfer.Dispose(); m_Xfer = null; }
         if (m_Pro != null)
         { m_Pro.Dispose(); m_Pro = null; }
         if (m_View != null)
         { m_View.Dispose(); m_View = null; m_ImageBox.View = null; }
         if (m_ColorConv != null)
         { m_ColorConv.Dispose(); m_ColorConv = null; }
         if (m_Buffers != null)
         { m_Buffers.Dispose(); m_Buffers = null; }
         if (m_Acquisition != null)
         { m_Acquisition.Dispose(); m_Acquisition = null; }
      }


      //**********************************************************************************
      //
      //				Window related functions
      //
      //**********************************************************************************
      protected override void OnResize(EventArgs argsPaint)
      {
         if (m_ImageBox != null)
            m_ImageBox.OnSize();
         base.OnResize(argsPaint);
      }

      private void GigECameraDemoDlg_FormClosed(object sender, FormClosedEventArgs e)
      {
         DestroyObjects();
         DisposeObjects();
      }

      //*****************************************************************************************
      //
      //					General Function
      //
      //*****************************************************************************************

      // Updates the menu items enabling/disabling the proper items depending on the stateof the application
      void UpdateControls()
      {
         bool bAcqNoGrab = m_Xfer != null && m_Xfer.Initialized && !m_Xfer.Grabbing;
         bool bAcqGrab = m_Xfer != null && m_Xfer.Initialized && m_Xfer.Grabbing;
         bool bNoGrab = m_Xfer == null || !m_Xfer.Grabbing;
         bool isColorConvEnabled = m_ColorConv != null && m_ColorConv.Initialized && m_ColorConv.Enabled;
         bool isColorConvHWsupported = m_ColorConv != null && m_ColorConv.HardwareSupported;
         bool isColorConvSW = isColorConvEnabled && m_ColorConv.SoftwareEnabled;
         bool isColorConvSWsupported = m_ColorConv != null && m_ColorConv.SoftwareSupported;

         // Acquisition Control
         button_Grab.Enabled = bAcqNoGrab;
         button_Freeze.Enabled = bAcqGrab;
         button_Snap.Enabled = bAcqNoGrab;

         // Options
         button_LoadConfig.Enabled = bAcqNoGrab || bNoGrab;
         button_Buffers.Enabled = bNoGrab;
         button_View.Enabled = bNoGrab;

         // File Options
         button_New.Enabled = bNoGrab;
         button_Load_Raw.Enabled = bNoGrab && (isColorConvSW || !isColorConvEnabled);
         button_Save_Raw.Enabled = bNoGrab && (isColorConvSW || !isColorConvEnabled);
         button_Save_Converted.Enabled = bNoGrab && isColorConvEnabled;

         // Color Conversion Options
         radio_hw.Enabled = bNoGrab && isColorConvHWsupported;
         radio_hw.Checked = m_ColorConv.HardwareEnabled;
         radio_sw.Enabled = bNoGrab && isColorConvSWsupported;
         radio_sw.Checked = m_ColorConv.SoftwareEnabled;
         radio_disable.Enabled = bNoGrab;
         radio_disable.Checked = !m_ColorConv.Enabled;

         // Set selection for Color Conversion output format control
         for (int i = 0; i < m_ColorConvOutputFormatValues.Length; i++)
         {
            if (m_ColorConvOutputFormatValues[i].Value == m_ColorConv.OutputFormat)
            {
               comboBox_Output_Conversion.SelectedIndex = i;
               break;
            }
         }

         comboBox_Output_Conversion.Enabled = bNoGrab && isColorConvSW;
         button_Options.Enabled = bNoGrab && isColorConvEnabled;
      }


      private void button_Snap_Click(object sender, EventArgs e)
      {
         AbortDlg abort = new AbortDlg(m_Xfer);

         if (m_Xfer.Snap())
         {
            if (abort.ShowDialog() != DialogResult.OK)
               m_Xfer.Abort();
            UpdateControls();
         }
      }

      private void button_Grab_Click(object sender, EventArgs e)
      {
         if (m_Xfer.Grab())
         {
            UpdateControls();
         }
      }

      private void button_Freeze_Click(object sender, EventArgs e)
      {
         AbortDlg abort = new AbortDlg(m_Xfer);

         if (m_Xfer.Freeze())
         {
            if (abort.ShowDialog() != DialogResult.OK)
               m_Xfer.Abort();
            UpdateControls();
         }
      }



      //*****************************************************************************************
      //
      //					General Function
      //
      //*****************************************************************************************

      private void button_Exit_Click(object sender, EventArgs e)
      {
         Application.Exit();
      }

      private void ColorConvDemoDlg_FormClosed(object sender, FormClosedEventArgs e)
      {
         DestroyObjects();
         DisposeObjects();
      }

      private void EnableSignalStatus()
      {
         if (m_Acquisition != null)
         {
            m_IsSignalDetected = (m_Acquisition.SignalStatus != SapAcquisition.AcqSignalStatus.None);
            if (m_IsSignalDetected == false)
               StatusLabelInfo.Text = "Online... No camera signal detected";
            else
               StatusLabelInfo.Text = "Online... Camera signal detected";
            m_Acquisition.SignalNotifyEnable = true;
         }
      }

      private void SystemEvents_SessionEnded(object sender, SessionEndedEventArgs e)
      {
         // The FormClosed event is not invoked when logging off or shutting down,
         // so we need to clean up here too.
         DestroyObjects();
         DisposeObjects();
      }

      private void ColorConvDemoDlg_Load(object sender, EventArgs e)
      {
         // Add about this to the system menu
         m_SystemMenu = SystemMenu.FromForm(this);
         if (m_SystemMenu != null)
         {
            m_SystemMenu.AppendSeparator();
            m_SystemMenu.AppendMenu(m_AboutID, "About this...");
         }

         // Register event handler which is invoked when logging off or shutting down
         SystemEvents.SessionEnded += new SessionEndedEventHandler(SystemEvents_SessionEnded);
      }


      //Catch the WM_SYSCOMMAND message and process it.
      protected override void WndProc(ref Message msg)
      {
         if (msg.Msg == (int)WindowMessages.wmSysCommand)
            if (msg.WParam.ToInt32() == m_AboutID)
            {
               AboutBox dlg = new AboutBox();
               dlg.ShowDialog();
               Invalidate();
               Update();
            }
         // Call base class function
         base.WndProc(ref msg);
      }

      //*****************************************************************************************
      //
      //					General Options
      //
      //*****************************************************************************************

      private void button_LoadConfig_Click(object sender, EventArgs e)
      {
         // Set new acquisition parameters     
         AcqConfigDlg acConfigDlg = new AcqConfigDlg(null, "", AcqConfigDlg.ServerCategory.ServerAcq);
         if (acConfigDlg.ShowDialog() == DialogResult.OK)
         {
            bool m_onlineSave = m_online;
            m_online = true;
            // Save device location
            SapLocation m_ServerLocationSave = m_ServerLocation;
            String m_ConfigFileNameSave = m_ConfigFileName;
            //Destroy Object
            DestroyObjects();
            DisposeObjects();
            // Update objects with new acquisition
            if (!CreateNewObjects(acConfigDlg, false))
            {
               MessageBox.Show("New objects creation has failed. Restoring original object ");
               m_ServerLocation = m_ServerLocationSave;
               m_ConfigFileName = m_ConfigFileNameSave;
               m_online = m_onlineSave;
               // Recreate original objects
               if (!CreateNewObjects(null, true))
               {
                  MessageBox.Show("Original object creation has failed. Closing application ");
                  Application.Exit();
               }
            }
         }
         else
         {
            MessageBox.Show("No Modification in Acquisition");
         }
         m_ImageBox.Refresh();
      }

      private void button_Buffers_Click(object sender, EventArgs e)
      {
         // Set new buffer parameters   
         BufferDlg dlg = new BufferDlg(m_Buffers, m_View.Display, m_online);
         if (dlg.ShowDialog() == DialogResult.OK)
         {
            DestroyObjects();
            DisposeObjects();

            // Update objects with new buffer
            // Update objects with new acquisition
            if (!CreateNewObjects())
            {
               MessageBox.Show("New objects creation has failed. Restoring original object ");
               // Recreate original objects
               if (!CreateNewObjects(null, true))
               {
                  MessageBox.Show("Original object creation has failed. Closing application ");
                  Application.Exit();
               }
            }
         }
      }

      private void button_View_Click(object sender, EventArgs e)
      {
         ViewDlg viewDialog = new ViewDlg(m_View, m_ImageBox.ViewRectangle);

         if (viewDialog.ShowDialog() == DialogResult.OK)
            m_ImageBox.OnSize();
      }

      //*****************************************************************************************
      //
      //               File Options
      //
      //*****************************************************************************************

      private void button_New_Click(object sender, EventArgs e)
      {
         m_Buffers.Clear();
         SapBuffer pConvBuffer = m_ColorConv.OutputBuffer;
         if (pConvBuffer != null && pConvBuffer != m_Buffers)
         {
            pConvBuffer.Clear();
         }
         m_ImageBox.Refresh();

      }

      private void button_Load_Raw_Click(object sender, EventArgs e)
      {
         LoadSaveDlg newDialogLoad = new LoadSaveDlg(m_Buffers, true, false);
         if (newDialogLoad.ShowDialog() == DialogResult.OK)
         {
            if (m_ColorConv.SoftwareEnabled)
            {
               m_Pro.Execute();
            }
         }
         m_ImageBox.OnSize();
         newDialogLoad.Dispose();
      }

      private void button_Save_Raw_Click(object sender, EventArgs e)
      {
         LoadSaveDlg newDialogSave = new LoadSaveDlg(m_ColorConv.InputBuffer, false, false);
         // Show the dialog and process the result
         newDialogSave.ShowDialog();
         newDialogSave.Dispose();

      }

      private void button_Save_Converted_Click(object sender, EventArgs e)
      {
         LoadSaveDlg newDialogSave = new LoadSaveDlg(m_ColorConv.OutputBuffer, false, false);
         // Show the dialog and process the result
         newDialogSave.ShowDialog();
         newDialogSave.Dispose();
      }

      //**************************************************************************************
      //
      //         Color Conversion Options
      //
      //**************************************************************************************

      private void radio_disable_CheckedChanged(object sender, EventArgs e)
      {
         ColorConvChoiceChange();
      }

      private void radio_hw_CheckedChanged(object sender, EventArgs e)
      {
         ColorConvChoiceChange();
      }

      private void radio_sw_CheckedChanged(object sender, EventArgs e)
      {
         ColorConvChoiceChange();
      }

      private void ColorConvChoiceChange()
      {
         // change the data model state
         if (radio_sw.Checked)
            m_ColorConv.Enable(true, false);
         else if (radio_hw.Checked)
            m_ColorConv.Enable(true, true);
         else
            m_ColorConv.Enable(false, false);

         // Fill color conversion output format control
         UpdateOutputFormatList();

         // need to tell the view about the right buffer to render
         UpdateViewFormat();
         
         // execute the SW conversion if needed
         //if (m_ColorConv.SoftwareEnabled)
            //m_Pro.Execute();

         // update UI
         UpdateControls();
      }

      private void UpdateViewFormat()
      {
         m_View.Destroy();
         m_View.Buffer = m_ColorConv.OutputBuffer;
         m_View.Create();
      }

      private void UpdateOutputFormatList()
      {
         comboBox_Output_Conversion.Items.Clear();
         for (int i = 0; i < m_ColorConvOutputFormatValues.Length; i++)
         {
            if (m_ColorConv!= null && m_ColorConv.SoftwareEnabled)
            {
               if (!m_ColorConv.IsOutputFormatSupported(m_ColorConvOutputFormatValues[i].Value))
                  continue;
            }

            comboBox_Output_Conversion.Items.Add(m_ColorConvOutputFormatValues[i]);
         }
      }


      private void button_Options_Click(object sender, EventArgs e)
      {
         ColorConvDlg dlg = new ColorConvDlg(m_ColorConv, m_Xfer, m_ImageBox, m_Pro);
         dlg.Show();
      }

      private void comboBox_Output_Conversion_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (m_ColorConv != null || !m_ColorConv.Initialized)
            if(!m_ColorConv.Enabled)
               return;
         
         // Set new color decoder output format
         SapFormatInfo selectFormat = (SapFormatInfo)comboBox_Output_Conversion.SelectedItem;
         m_ColorConv.OutputFormat = selectFormat.Value;

         // need to tell the view about the right buffer to render
         UpdateViewFormat();

         UpdateControls();
      }

      // Format table
      public class SapFormatInfo
      {
         public SapFormatInfo(SapFormat format, String name)
         {
            m_Value = format;
            m_Name = name;
         }

         public SapFormat Value
         {
            get { return m_Value; }
            set { m_Value = value; }
         }

         public override string ToString()
         {
            return m_Name;
         }

         private SapFormat m_Value;
         private String m_Name;
      }

   }
}