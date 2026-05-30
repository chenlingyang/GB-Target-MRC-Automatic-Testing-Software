using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;

using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.SapClassGui;

namespace DALSA.SaperaLT.Demos.NET.CSharp.SeqGrabDemo
{
   public partial class SeqGrabDemoDlg : Form
   {
      // Delegate to display number of frame acquired 
      // Delegate is needed because .NEt framework does not support cross thread control modification
      private delegate void RefreshControlDelegate();
      private delegate void CheckForLastFrameDelegate();

      static void xfer_XferNotify(object sender, SapXferNotifyEventArgs argsNotify)
      {
         SeqGrabDemoDlg SeqGDDlg = argsNotify.Context as SeqGrabDemoDlg;

         // Check if last frame is reached
         SeqGDDlg.Invoke(new CheckForLastFrameDelegate(SeqGDDlg.CheckForLastFrame));

         // Refresh view
         SeqGDDlg.m_View.Show();

         // Refresh controls
         SeqGDDlg.Invoke(new RefreshControlDelegate(SeqGDDlg.RefreshTextBox));
      }

      static void GetSignalStatus(object sender, SapSignalNotifyEventArgs argsSignal)
      {
         SeqGrabDemoDlg SeqGDDlg = argsSignal.Context as SeqGrabDemoDlg;
         SapAcquisition.AcqSignalStatus signalStatus = argsSignal.SignalStatus;

         SeqGDDlg.m_IsSignalDetected = (signalStatus != SapAcquisition.AcqSignalStatus.None);
         if (SeqGDDlg.m_IsSignalDetected == false)
            SeqGDDlg.StatusLabelInfo.Text = "Online... No camera signal detected";
         else SeqGDDlg.StatusLabelInfo.Text = "Online... Camera signal detected";
      }

      static void AcqCallback(object sender, SapAcqNotifyEventArgs argsSignal)
      {
         SeqGrabDemoDlg SeqGDDlg = argsSignal.Context as SeqGrabDemoDlg;
         SeqGDDlg.FrameLostCount.Text = "Frame Lost : " + argsSignal.EventCount.ToString();
      }

      public SeqGrabDemoDlg()
      {
         m_Acquisition = null;
         m_Buffers = null;
         m_Xfer = null;
         m_View = null;
         m_bRecordOn = false;
         m_bPlayOn = false;
         m_bPauseOn = false;

         m_nFramesPerCallback = 1;
         m_nFramesOnBoard = 4096;

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
         this.m_ImageBox.Location = new System.Drawing.Point(260, 71);
         this.m_ImageBox.Name = "m_ImageBox";
         this.m_ImageBox.PixelValueDisplay = null;
         this.m_ImageBox.Size = new System.Drawing.Size(386, 406);
         this.m_ImageBox.SliderEnable = false;
         this.m_ImageBox.SliderMaximum = 10;
         this.m_ImageBox.SliderMinimum = 0;
         this.m_ImageBox.SliderValue = 0;
         this.m_ImageBox.SliderVisible = true;
         this.m_ImageBox.TabIndex = 15;
         this.m_ImageBox.TrackerEnable = false;
         this.m_ImageBox.View = null;
         this.Controls.Add(this.m_ImageBox);

         AcqConfigDlg acConfigDlg = new AcqConfigDlg(null, "", AcqConfigDlg.ServerCategory.ServerAcq);
         if (acConfigDlg.ShowDialog() == DialogResult.OK)
         {
            m_online = true;
         }
         else
         {
            m_online = false;
         }

         if (!CreateNewObjects(Default_Nbr_buffers, acConfigDlg))
            this.Close();
      }

      private void RefreshTextBox()
      {
         UpdateTextBox();
         UpdateFrameRate();
      }

      //*****************************************************************************************
      //
      //					Create and Destroy Object
      //
      //*****************************************************************************************

      // Create new objects with acquisition information
      public bool CreateNewObjects(int Nbr_Buffers, AcqConfigDlg acConfigDlg)
      {
         if (m_online)
         {
            if (acConfigDlg != null)
            {
               m_ServerLocation = acConfigDlg.ServerLocation;
               m_ConfigFileName = acConfigDlg.ConfigFile;
            }
            // define on-line object
            m_Acquisition = new SapAcquisition(m_ServerLocation, m_ConfigFileName);
            if (SapBuffer.IsBufferTypeSupported(m_ServerLocation, SapBuffer.MemoryType.ScatterGather))
               m_Buffers = new SapBuffer(Nbr_Buffers, m_Acquisition, SapBuffer.MemoryType.ScatterGather);
            else
               m_Buffers = new SapBuffer(Nbr_Buffers, m_Acquisition, SapBuffer.MemoryType.ScatterGatherPhysical);
            m_Xfer = new SapAcqToBuf(m_Acquisition, m_Buffers);
            m_View = new SapView(m_Buffers);
            //event for view
            m_Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            m_Xfer.XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
            m_Xfer.XferNotifyContext = this;

            // event fot Acqcallback (Frame lost)
            m_Acquisition.EventType = SapAcquisition.AcqEventType.FrameLost;
            m_Acquisition.AcqNotify += new SapAcqNotifyHandler(AcqCallback);
            m_Acquisition.AcqNotifyContext = this;

            // event for signal status
            m_Acquisition.SignalNotify += new SapSignalNotifyHandler(GetSignalStatus);
            m_Acquisition.SignalNotifyContext = this;
         }
         else
         {
            //define off-line object
            m_Buffers = new SapBuffer();
            m_Buffers.Count = Nbr_Buffers;
            m_View = new SapView(m_Buffers);
         }

         m_ImageBox.View = m_View;

         if (!CreateObjects())
         {
            DisposeObjects();
            return false;
         }

         m_ImageBox.OnSize();
         UpdateControls();
         UpdateTextBox();
         EnableSignalStatus();
         return true;
      }

      // Create new objects with acquisition information
      public bool CreateNewObjects(BufferDlg dlg)
      {

         if (m_online)
         {
            m_Acquisition = new SapAcquisition(m_ServerLocation, m_ConfigFileName);
            m_Buffers = new SapBufferWithTrash(Math.Max(2, dlg.Count), m_Acquisition, dlg.Type);
            m_Xfer = new SapAcqToBuf(m_Acquisition, m_Buffers);
            m_View = new SapView(m_Buffers);

            m_Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            m_Xfer.XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
            m_Xfer.XferNotifyContext = this;

            // event fot Acqcallback (Frame lost)
            m_Acquisition.EventType = SapAcquisition.AcqEventType.FrameLost;
            m_Acquisition.AcqNotify += new SapAcqNotifyHandler(AcqCallback);
            m_Acquisition.AcqNotifyContext = this;

            // event for signal status
            m_Acquisition.SignalNotify += new SapSignalNotifyHandler(GetSignalStatus);
            m_Acquisition.SignalNotifyContext = this;

         }
         else
         {
            m_Buffers = new SapBuffer(dlg.Count, dlg.BWidth, dlg.BHeight, dlg.Format, dlg.Type);
            m_View = new SapView(m_Buffers);
         }

         m_ImageBox.View = m_View;

         if (CreateObjects() == false)
         {
            DisposeObjects();
            return false;
         }

         m_ImageBox.OnSize();
         UpdateControls();
         UpdateTextBox();
         EnableSignalStatus();
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
         // Create view object
         if (m_View != null && !m_View.Initialized)
         {
            if (m_View.Create() == false)
            {
               DestroyObjects();
               return false;
            }
         }
         // Create Xfer object
         if (m_Xfer != null && !m_Xfer.Initialized)
         {

            //Number of frames per callback retreived
            int nFramesPerCallback;

            // Set number of onboard buffers
            m_Xfer.Pairs[0].FramesOnBoard = m_nFramesOnBoard;

            // Set number of frames per callback
            m_Xfer.Pairs[0].FramesPerCallback = m_nFramesPerCallback;

            // If there is a large number of buffers, temporarily boost the command timeout value,
            // since the call to Create may take a long time to complete.
            // As a safe rule of thumb, use 100 milliseconds per buffer.
            int oldCommandTimeout = SapManager.CommandTimeout;
            int newCommandTimeout = 100 * m_Buffers.Count;

            if (newCommandTimeout < oldCommandTimeout)
               newCommandTimeout = oldCommandTimeout;

            SapManager.CommandTimeout = newCommandTimeout;

            // Create transfer object
            if (!m_Xfer.Create())
            {
               DestroyObjects();
               return false;
            }

            // Restore original command timeout value
            SapManager.CommandTimeout = oldCommandTimeout;
            // initialize tranfer object and reset source/destination index

            m_Xfer.Init(true);

            // Retrieve number of frames per callback
            // It may be less than what we have asked for.
            nFramesPerCallback = m_Xfer.Pairs[0].FramesPerCallback;
            if (m_nFramesPerCallback > nFramesPerCallback)
            {
               m_nFramesPerCallback = nFramesPerCallback;
               MessageBox.Show("No memory");
            }

            m_nFramesOnBoard = m_Xfer.Pairs[0].FramesOnBoard;
         }
         return true;
      }

      private void DestroyObjects()
      {
         if (m_Xfer != null && m_Xfer.Initialized)
            m_Xfer.Destroy();
         if (m_View != null && m_View.Initialized)
            m_View.Destroy();
         if (m_Buffers != null && m_Buffers.Initialized)
            m_Buffers.Destroy();
         if (m_Acquisition != null && m_Acquisition.Initialized)
            m_Acquisition.Destroy();
      }

      private void DisposeObjects()
      {
         if (m_Xfer != null)
         { m_Xfer.Dispose(); m_Xfer = null; }
         if (m_View != null)
         { m_View.Dispose(); m_View = null; m_ImageBox.View = null; }
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

      private void SeqGrabDemoDlg_FormClosed(object sender, FormClosedEventArgs e)
      {
         DestroyObjects();
         DisposeObjects();
      }


      //**********************************************************************************
      //
      //				Timer related functions
      //
      //**********************************************************************************

      private void timer_Tick(object sender, EventArgs e)
      {
         // Increase buffer index
         m_Buffers.Next();
         // Resfresh display
         m_View.Show();
         // Refresh controls
         UpdateTextBox();
         // Check if last frame is reached
         CheckForLastFrame();
      }

      //**********************************************************************************
      //
      //				Frame rate related functions
      //
      //**********************************************************************************

      private void CheckForLastFrame()
      {
         // Check for last frame
         if (m_Buffers.Index == m_Buffers.Count - 1)
         {
            if (m_bRecordOn)
            {
               m_bRecordOn = false;
            }
            if (m_bPlayOn)
            {
               m_bPlayOn = false;
               timer.Enabled = false;
            }
            UpdateControls();
         }
      }

      private void UpdateFrameRate()
      {
         if (m_Xfer.UpdateFrameRateStatistics())
         {
            float framerate = 0.0f;
            SapXferFrameRateInfo stats = m_Xfer.FrameRateStatistics;

            if (stats.IsBufferFrameRateAvailable)
               framerate = stats.BufferFrameRate;
            else if (stats.IsLiveFrameRateAvailable && !stats.IsLiveFrameRateStalled)
               framerate = stats.LiveFrameRate;

            m_Buffers.FrameRate = framerate;

            m_MinTime = stats.MinTimePerFrame;
            m_MaxTime = stats.MaxTimePerFrame;
         }
      }

      //**********************************************************************************
      //
      //				General functions
      //
      //**********************************************************************************


      private void UpdateControls()
      {

         bool bAcqNoGrab = m_online && !m_bRecordOn && !m_bPlayOn;
         bool bNoGrab = !m_bRecordOn && !m_bPlayOn;

         // Record Control
         button_Record.Enabled = bAcqNoGrab;
         button_Play.Enabled = bNoGrab;
         button_Pause.Enabled = !bNoGrab;
         button_Stop.Enabled = !bNoGrab;
         button_Pause.Text = m_bPauseOn ? "Continue" : "Pause";

         // General Options
         button_Buffers.Enabled = bNoGrab;
         button_Load_Config.Enabled = bNoGrab;
         button_High_Frame_Rate.Enabled = bNoGrab;

         // File Options
         button_Load_Current.Enabled = bNoGrab;
         button_Load_Sequence.Enabled = bNoGrab;
         button_Save_Current.Enabled = bNoGrab;
         button_Save_Sequence.Enabled = bNoGrab;

         // Recording statistics
         textBox_Frame_Rate.Enabled = bNoGrab;

         // Slider
         m_ImageBox.SliderEnable = bNoGrab || (m_bPlayOn && m_bPauseOn);
         m_ImageBox.SliderMinimum = 0;
         m_ImageBox.SliderMaximum = m_Buffers.Count - 1;
      }

      private void UpdateTextBox()
      {
         textBox_Displayed.Text = Convert.ToString(m_Buffers.Index + 1);
         textBox_NumberOfBuffers.Text = m_Buffers.Count.ToString();
         textBox_Minimum.Text = m_MinTime.ToString();
         textBox_Maximum.Text = m_MaxTime.ToString();
         m_ImageBox.SliderValue = m_Buffers.Index;
         if (!m_bPlayOn)
            textBox_Frame_Rate.Text = m_Buffers.FrameRate.ToString();
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

      private void button_Exit_Click(object sender, EventArgs e)
      {
         Application.Exit();
      }

      private void SystemEvents_SessionEnded(object sender, SessionEndedEventArgs e)
      {
         // The FormClosed event is not invoked when logging off or shutting down,
         // so we need to clean up here too.
         DestroyObjects();
         DisposeObjects();
      }

      //*****************************************************************************************
      //
      //					General Options
      //
      //*****************************************************************************************

      private void button_Load_Config_Click(object sender, EventArgs e)
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
            if (!CreateNewObjects(Default_Nbr_buffers, acConfigDlg))
            {
               MessageBox.Show("New objects creation has failed. Restoring original object ");
               m_ServerLocation = m_ServerLocationSave;
               m_ConfigFileName = m_ConfigFileNameSave;
               m_online = m_onlineSave;
               // Recreate original objects
               if (!CreateNewObjects(Default_Nbr_buffers, null))
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
         BufferDlg dlg = new BufferDlg(m_Buffers, m_View.Display, true);
         if (dlg.ShowDialog() == DialogResult.OK)
         {
            DestroyObjects();
            DisposeObjects();

            // Update objects with new buffer
            if (!CreateNewObjects(dlg))
            {
               MessageBox.Show("New objects creation has failed. Restoring original object ");
               // Recreate original objects
               if (!CreateNewObjects(Default_Nbr_buffers, null))
               {
                  MessageBox.Show("Original object creation has failed. Closing application ");
                  Application.Exit();
               }
            }
         }
         m_ImageBox.Refresh();
      }

      private void button_High_Frame_Rate_Click(object sender, EventArgs e)
      {
         HighFrameDlg dlg = new HighFrameDlg(m_nFramesPerCallback, m_nFramesOnBoard, m_Xfer);
         if (dlg.ShowDialog() == DialogResult.OK)
         {
            m_nFramesPerCallback = dlg.Frame_Per_Callback;
            m_nFramesOnBoard = dlg.Frame_On_Board;
            int Nbr_Buffer = m_Buffers.Count;
            DestroyObjects();
            DisposeObjects();
            CreateNewObjects(Nbr_Buffer, null);
         }
         m_ImageBox.Refresh();
      }


      //*****************************************************************************************
      //
      //					File Options
      //
      //*****************************************************************************************


      private void button_Load_Sequence_Click(object sender, EventArgs e)
      {
         if (m_Buffers.Format == SapFormat.Mono16 ||
             m_Buffers.Format == SapFormat.RGB101010 ||
             m_Buffers.Format == SapFormat.RGB161616 ||
             m_Buffers.Format == SapFormat.RGB16161616)
         {
            MessageBox.Show("Sequence images in AVI format are sampled at 8-bit pixel depth.\nYou cannot load a sequence in the current configuration.");
            return;
         }

         if (m_Buffers.Format == SapFormat.RGBR888)
         {
            MessageBox.Show("Sequence images acquired in RGBR888 format (red first) were saved as RGB888 (blue first).\nYou cannot load a sequence in the current configuration.");
            return;
         }

         LoadSaveDlg dlg = new LoadSaveDlg(m_Buffers, true, true);
         dlg.ShowDialog();
         dlg.Dispose();
         m_ImageBox.Refresh();
      }

      private void button_Save_Sequence_Click(object sender, EventArgs e)
      {
         if (m_Buffers.Format == SapFormat.Mono16 ||
             m_Buffers.Format == SapFormat.RGB101010 ||
             m_Buffers.Format == SapFormat.RGB161616 ||
             m_Buffers.Format == SapFormat.RGB16161616)
         {
            MessageBox.Show("Saving images in AVI format requires downsampling them to 8-bit pixel depth.\nYou will not be able to reload this sequence in this application unless you change the buffer format.");
         }

         if (m_Buffers.Format == SapFormat.RGBR888)
         {
            MessageBox.Show("Saving images in AVI format requires conversion to RGB888 format (blue first).\nYou will not be able to reload this sequence in this application unless you change the buffer format.");
         }

         LoadSaveDlg dlg = new LoadSaveDlg(m_Buffers, false, true);
         dlg.ShowDialog();
         dlg.Dispose();
         m_ImageBox.Refresh();
      }

      private void button_Load_Current_Click(object sender, EventArgs e)
      {
         LoadSaveDlg dlg = new LoadSaveDlg(m_Buffers, true, false);
         // Show the dialog and process the result
         dlg.ShowDialog();
         dlg.Dispose();
         m_ImageBox.Refresh();
      }

      private void button_Save_Current_Click(object sender, EventArgs e)
      {
         LoadSaveDlg dlg = new LoadSaveDlg(m_Buffers, false, false);
         // Show the dialog and process the result
         dlg.ShowDialog();
         dlg.Dispose();
         m_ImageBox.Refresh();
      }

      //*****************************************************************************************
      //
      //					Acquisition Control
      //
      //*****************************************************************************************

      private void button_Record_Click(object sender, EventArgs e)
      {
         FrameLostCount.Text = "";

         // Reset source and destination indices
         m_Xfer.Init(true);

         // Reset the frame rate statistics ahead of each transfer stream
         SapXferFrameRateInfo stats = m_Xfer.FrameRateStatistics;
         stats.Reset();

         // Acquire all frames
         if (m_Xfer.Snap(m_Buffers.Count))
         {
            m_bRecordOn = true;
            UpdateControls();
         }
         else
         {
            m_bRecordOn = false;
         }
      }

      private void button_Play_Click(object sender, EventArgs e)
      {
         // Initialize buffer index
         m_Buffers.Index = 0;

         // Start playback timer
         timer.Interval = (int)(1000.0 / float.Parse(textBox_Frame_Rate.Text));
         timer.Enabled = true;

         m_bPlayOn = true;
         UpdateControls();
      }

      private void button_Pause_Click(object sender, EventArgs e)
      {
         if (!m_bPauseOn)
         {
            if (m_bRecordOn)
            {
               // Stop current acquisition
               if (!m_Xfer.Freeze())
                  return;

               AbortDlg dlg = new AbortDlg(m_Xfer);
               if (dlg.ShowDialog() != DialogResult.OK)
                  m_Xfer.Abort();
            }
            else if (m_bPlayOn)
            {
               // Stop playback timer
               timer.Enabled = false;
            }
         }
         else
         {
            // Check if recording or playing
            if (m_bRecordOn)
            {
               // Acquire remaining frames
               if (!m_Xfer.Snap(m_Buffers.Count - m_Buffers.Index - 1))
                  return;
            }
            else if (m_bPlayOn)
            {
               // Restart playback timer
               timer.Enabled = true;
            }
         }

         m_bPauseOn = !m_bPauseOn;
         UpdateControls();

      }

      private void button_Stop_Click(object sender, EventArgs e)
      {
         // Check if recording or playing
         if (m_bRecordOn)
         {
            // Stop current acquisition
            if (!m_Xfer.Freeze())
               return;

            AbortDlg dlg = new AbortDlg(m_Xfer);
            if (dlg.ShowDialog() != DialogResult.OK)
               m_Xfer.Abort();

            m_bRecordOn = false;
         }
         else if (m_bPlayOn)
         {
            // Stop playback timer
            timer.Enabled = false;
            m_bPlayOn = false;
         }
         m_bPauseOn = false;
         UpdateControls();
      }
      //*****************************************************************************************
      //
      //					System menu
      //
      //*****************************************************************************************

      private void SeqGrabDemoDlg_Load(object sender, EventArgs e)
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
   }
}