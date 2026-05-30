using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.SapClassGui;


namespace DALSA.SaperaLT.Demos.NET.CSharp.MultiBoardSyncGrabDemo
{
    public partial class MultiBoardSyncGrabDemoDlg : Form
    {

       // Delegate to display number of frame acquired 
       // Delegate is needed because .NEt framework does not support  cross thread control modification
       private delegate void DisplayFrameAcquired(int number, bool trash);

        static void xfer_XferNotify(object sender, SapXferNotifyEventArgs argsNotify)
        {
            MultiBoardSyncGrabDemoDlg GrabDlg = argsNotify.Context as MultiBoardSyncGrabDemoDlg;
            // If grabbing in trash buffer, do not display the image, update the
            // appropriate number of frames on the status bar instead
            if (argsNotify.Trash)
               GrabDlg.Invoke(new DisplayFrameAcquired(GrabDlg.ShowFrameNumber), argsNotify.EventCount, true);  
            // Refresh view
            else
            {
               GrabDlg.Invoke(new DisplayFrameAcquired(GrabDlg.ShowFrameNumber), argsNotify.EventCount, false);  
               GrabDlg.m_View.Show();
            }
        }

        static void GetSignalStatus(object sender,SapSignalNotifyEventArgs argsSignal)
        {
            MultiBoardSyncGrabDemoDlg GrabDlg = argsSignal.Context as MultiBoardSyncGrabDemoDlg;
            SapAcquisition.AcqSignalStatus signalStatus = argsSignal.SignalStatus;

            GrabDlg.m_IsSignalDetected = (signalStatus != SapAcquisition.AcqSignalStatus.None);
            if (GrabDlg.m_IsSignalDetected == false)
                GrabDlg.StatusLabelInfo.Text = "Online... No camera signal detected";
            else GrabDlg.StatusLabelInfo.Text = "Online... Camera signal detected";
        }
       
        // Constructor
        public MultiBoardSyncGrabDemoDlg()
        {
            m_Acquisition = null;
            m_Buffers = null;
            m_Xfer = null;
            m_View = null;
            m_IsSignalDetected = true;

            InitializeComponent();

            m_Acquisition   = new SapAcquisition[2];
            m_Buffers       = new SapBufferRoi[2];
            m_Xfer          = new SapAcqToBuf[2];

            // Note:
            //  The code to initialize m_ImageBox was originally in the InitializeComponent function
            //  called above. However, it has been moved to the dialog constructor as a workaround
            //  to a Visual Studio Designer error when loading the DALSA.SaperaLT.SapClassBasic
            //  assembly under 64-bit Windows.
            //  As a consequence, it is not possible to adjust the m_ImageBox properties
            //  automatically using the Designer anymore, this has to be done manually.
            // 
            this.m_ImageBox = new DALSA.SaperaLT.SapClassGui.ImageBox();
            this.m_ImageBox.Location = new System.Drawing.Point(241, 4);
            this.m_ImageBox.Name = "m_ImageBox";
            this.m_ImageBox.PixelValueDisplay = this.PixelDataValue;
            this.m_ImageBox.Size = new System.Drawing.Size(386, 406);
            this.m_ImageBox.SliderEnable = false;
            this.m_ImageBox.SliderMaximum = 10;
            this.m_ImageBox.SliderMinimum = 0;
            this.m_ImageBox.SliderValue = 0;
            this.m_ImageBox.SliderVisible = false;
            this.m_ImageBox.TabIndex = 13;
            this.m_ImageBox.TrackerEnable = false;
            this.m_ImageBox.View = null;
            this.Controls.Add(this.m_ImageBox);

            AcqConfigDlg acConfigDlg = new AcqConfigDlg(null, "", AcqConfigDlg.ServerCategory.ServerAcq);
            if (acConfigDlg.ShowDialog() == DialogResult.OK)
            {
               m_Acquisition[0] = new SapAcquisition(acConfigDlg.ServerLocation, acConfigDlg.ConfigFile);
               // Create acquisition object
               if (m_Acquisition[0] != null && !m_Acquisition[0].Initialized)
               {
                  if (m_Acquisition[0].Create() == false)
                  {
                     DestroyObjects();
                  }
               }
            }
            else
            {
               MessageBox.Show("No board found or selected !");
               this.Close();
               return;
            }


            AcqConfigDlg acConfigDlg2 = new AcqConfigDlg(null, "", AcqConfigDlg.ServerCategory.ServerAcq);
            if (acConfigDlg2.ShowDialog() == DialogResult.OK)
            {
                m_Acquisition[1] = new SapAcquisition(acConfigDlg2.ServerLocation, acConfigDlg2.ConfigFile);
                // Create acquisition object
                if (m_Acquisition[1] != null && !m_Acquisition[1].Initialized)
                {
                    if (m_Acquisition[1].Create() == false)
                    {
                        DestroyObjects();
                    }
                }
            }
            else
            {
               MessageBox.Show("No board found or selected !");
               this.Close();
               return;
            }

            m_online = true;

            //check to see if both acquision devices support scatter gather.
            bool acq0SupportSG = SapBuffer.IsBufferTypeSupported(m_Acquisition[0].Location, SapBuffer.MemoryType.ScatterGather);
            bool acq1SupportSG = SapBuffer.IsBufferTypeSupported(m_Acquisition[1].Location, SapBuffer.MemoryType.ScatterGather);
   

            if (!acq0SupportSG || !acq1SupportSG)
            {
                // check if they support scatter gather physical
                bool acq0SupportSGP = SapBuffer.IsBufferTypeSupported(m_Acquisition[0].Location, SapBuffer.MemoryType.ScatterGatherPhysical);
                bool acq1SupportSGP = SapBuffer.IsBufferTypeSupported(m_Acquisition[1].Location, SapBuffer.MemoryType.ScatterGatherPhysical);

                if (!(!acq0SupportSG && !acq1SupportSG && acq0SupportSGP && acq1SupportSGP))
                {
                    String message;
                    message = String.Format("The chosen acquisition devices\n\n-{0}\n-{1}\n\ndo not support similar buffer types.", m_Acquisition[0].Location.ServerName, m_Acquisition[1].Location.ServerName);
                    MessageBox.Show(message, "Buffer Type Error");
                    m_online = false;
                }
            }

            if (!CreateNewObjects(acConfigDlg2, false))
                this.Close();
        }

       private void ShowFrameNumber(int number, bool trash)
       {
          String str;
          if (trash)
          {
             str = String.Format("Frames acquired in trash buffer: {0}", number);
             this.StatusLabelInfoTrash.Text = str;
          }
          else
          {
             str = String.Format("Frames acquired :{0}", number);
             this.StatusLabelInfo.Text = str;
          }
       }

        //*****************************************************************************************
        //
        //					Create and Destroy Object
        //
        //*****************************************************************************************

        // Create new object with acquisition information 
        public bool CreateNewObjects(AcqConfigDlg acConfigDlg, bool Restore)
        {
            if (m_online)
            {

                if (!Restore)
                {
                    m_ServerLocation = acConfigDlg.ServerLocation;
                    m_ConfigFileName = acConfigDlg.ConfigFile;
                }

                if (SapBuffer.IsBufferTypeSupported(m_ServerLocation, SapBuffer.MemoryType.ScatterGather))
                {
                    m_Buffer = new SapBufferWithTrash();
                    m_Buffers[0] = new SapBufferRoi(m_Buffer);
                    m_Buffers[1] = new SapBufferRoi(m_Buffer);
                }
                else
                {
                    m_Buffer = new SapBufferWithTrash();
                    m_Buffers[0] = new SapBufferRoi(m_Buffer);
                    m_Buffers[1] = new SapBufferRoi(m_Buffer);
                }

                m_Xfer[0] = new SapAcqToBuf(m_Acquisition[0], m_Buffers[0]);
                m_Xfer[1] = new SapAcqToBuf(m_Acquisition[1], m_Buffers[1]);
                m_View = new SapView(m_Buffer);
               

                //event for view
                m_Xfer[0].Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
                m_Xfer[0].XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
                m_Xfer[0].XferNotifyContext = this;

                m_Xfer[1].Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
                m_Xfer[1].XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
                m_Xfer[1].XferNotifyContext = this;
                
                // event for signal status
                m_Acquisition[0].SignalNotify += new SapSignalNotifyHandler(GetSignalStatus);
                m_Acquisition[0].SignalNotifyContext = this;

                m_Acquisition[1].SignalNotify += new SapSignalNotifyHandler(GetSignalStatus);
                m_Acquisition[1].SignalNotifyContext = this;
            }
            else
            {
                //define off-line object
                m_Buffer = new SapBuffer();
                m_View = new SapView(m_Buffer);
                StatusLabelInfo.Text = "Offline...Please load 2 acquisition devices";
            }

            m_View.SetScalingMode(0.6F, 0.6F);
            m_ImageBox.View = m_View;

            if (!CreateObjects())
            {
                DisposeObjects();
                return false;
            }

            m_ImageBox.OnSize();
            EnableSignalStatus();
            UpdateControls();
            return true;          
        }


        // Call Create method  
        private bool CreateObjects()
        {
            
            if (m_online)
            {
                m_Buffer.Count = 1;
                m_Buffer.Width = 2 * m_Acquisition[0].XferParams.Width;
                m_Buffer.Height = m_Acquisition[0].XferParams.Height;
                m_Buffer.Format = m_Acquisition[0].XferParams.Format;
                m_Buffer.PixelDepth = m_Acquisition[0].XferParams.PixelDepth;
            }
            // Create buffer object
            if (m_Buffer != null && !m_Buffer.Initialized)
            {
                if (m_Buffer.Create() == false)
                {
                    DestroyObjects();
                    return false;
                }
                m_Buffer.Clear();
            }

            if (m_online)
            {
                m_Buffers[0].SetRoi(0, 0, m_Acquisition[0].XferParams.Width, m_Acquisition[0].XferParams.Height);
                if (m_Buffers[0] != null && !m_Buffers[0].Initialized)
                {
                    if (m_Buffers[0].Create() == false)
                    {
                        DestroyObjects();
                        return false;
                    }
                }

                m_Buffers[1].SetRoi(m_Acquisition[0].XferParams.Width, 0, m_Acquisition[0].XferParams.Width, m_Acquisition[0].XferParams.Height);
                if (m_Buffers[1] != null && !m_Buffers[1].Initialized)
                {
                    if (m_Buffers[1].Create() == false)
                    {
                        DestroyObjects();
                        return false;
                    }
                }
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
            if (m_Xfer[0] != null && !m_Xfer[0].Initialized)
            {
                if (m_Xfer[0].Create() == false)
                {
                    DestroyObjects();
                    return false;
                }
            }
            if (m_Xfer[1] != null && !m_Xfer[1].Initialized)
            {
                if (m_Xfer[1].Create() == false)
                {
                    DestroyObjects();
                    return false;
                }
            }     
            return true;
        }

        //Call Destroy method
        private void DestroyObjects()
        {
            //stop grabbing
            if (m_Xfer[0] != null && m_Xfer[0].Grabbing)
                m_Xfer[0].Abort();
            if (m_Xfer[1] != null && m_Xfer[1].Grabbing)
                m_Xfer[1].Abort();

            if (m_Xfer[0] != null && m_Xfer[0].Initialized)
                m_Xfer[0].Destroy();
            if (m_Xfer[1] != null && m_Xfer[1].Initialized)
                m_Xfer[1].Destroy();
            if (m_View != null && m_View.Initialized)
                m_View.Destroy();
            if (m_Buffers[0] != null && m_Buffers[0].Initialized)
                m_Buffers[0].Destroy();
            if (m_Buffers[1] != null && m_Buffers[1].Initialized)
                m_Buffers[1].Destroy();
            if (m_Buffer != null && m_Buffer.Initialized)
                m_Buffer.Destroy();
            if (m_Acquisition[0] != null && m_Acquisition[0].Initialized)
                m_Acquisition[0].Destroy();
            if (m_Acquisition[1] != null && m_Acquisition[1].Initialized)
                m_Acquisition[1].Destroy();
        }

        private void DisposeObjects()
        {
            if (m_Xfer[1] != null)
            { m_Xfer[1].Dispose();}
            if (m_Xfer[0] != null)
            { m_Xfer[0].Dispose(); m_Xfer = null; }

            if (m_View != null)
            { m_View.Dispose(); m_View = null; m_ImageBox.View = null; }

            if (m_Buffers[1] != null)
            { m_Buffers[1].Dispose();}
            if (m_Buffers[0] != null)
            { m_Buffers[0].Dispose(); m_Buffers = null; }

            if (m_Buffer != null)
            { m_Buffer.Dispose(); m_Buffer = null; }

            if (m_Acquisition[1] != null)
            { m_Acquisition[1].Dispose(); }
            if (m_Acquisition[0] != null)
            { m_Acquisition[0].Dispose(); m_Acquisition = null; }

        }

        //**********************************************************************************
        //
        //				Window related functions
        //
        //**********************************************************************************


        protected override void OnResize(EventArgs argsResize)
        {
           if (m_ImageBox != null)
              m_ImageBox.OnSize();
           base.OnResize(argsResize);
        }

       private void MultiBoardSyncGrabDemoDlg_FormClosed(object sender, FormClosedEventArgs e)
       {
          DestroyObjects();
          DisposeObjects();
       }

        //*****************************************************************************************
        //
        //					File Control
        //
        //*****************************************************************************************

        private void button_New_Click(object sender, EventArgs e)
        {
            m_Buffer.Clear();
            m_ImageBox.Refresh();
        }

           

        private void button_View_Click(object sender, EventArgs e)
        {
           ViewDlg viewDialog = new ViewDlg(m_View, m_ImageBox.ViewRectangle);

            if (viewDialog.ShowDialog() == DialogResult.OK)
               m_ImageBox.OnSize();
            m_ImageBox.Refresh();            
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

        private void EnableSignalStatus()
        {
            if (m_Acquisition[0] != null)
            {
                m_IsSignalDetected = (m_Acquisition[0].SignalStatus != SapAcquisition.AcqSignalStatus.None);
                if (m_IsSignalDetected == false)        
                    StatusLabelInfo.Text = "Online... No camera signal detected";
                else 
                    StatusLabelInfo.Text = "Online... Camera signal detected";
                m_Acquisition[0].SignalNotifyEnable = true;  
            }  
        }

        private void SystemEvents_SessionEnded(object sender, SessionEndedEventArgs e)
        {
            // The FormClosed event is not invoked when logging off or shutting down,
            // so we need to clean up here too.
            DestroyObjects();
            DisposeObjects();
        }
        
        // Updates the menu items enabling/disabling the proper items depending on the state of the application
        void UpdateControls()
        {
            bool bAcqNoGrab = (m_Xfer[0] != null) && (m_Xfer[0].Grabbing == false);
            bool bAcqGrab = (m_Xfer[0] != null) && (m_Xfer[0].Grabbing == true);
            bool bNoGrab = (m_Xfer[0] == null) || (m_Xfer[0].Grabbing == false);

            // Acquisition Control
            button_Grab.Enabled = bAcqNoGrab && m_online;
            button_Snap.Enabled = bAcqNoGrab && m_online;
            button_Freeze.Enabled = bAcqGrab && m_online;

          }

        //*****************************************************************************************
        //
        //					Acquisition Control
        //
        //*****************************************************************************************

        private void button_Snap_Click(object sender, EventArgs e)
        {
            AbortDlg abort0 = new AbortDlg(m_Xfer[0]);
            AbortDlg abort1 = new AbortDlg(m_Xfer[1]);
            if (m_Xfer[0].Snap() && m_Xfer[1].Snap())
            {
                if (abort0.ShowDialog() != DialogResult.OK && abort1.ShowDialog() != DialogResult.OK)
                {
                    m_Xfer[0].Abort();
                    m_Xfer[1].Abort();
                }
                UpdateControls();
            }
        }

        private void button_Grab_Click(object sender, EventArgs e)
        {
           StatusLabelInfoTrash.Text = "";
           if (m_Xfer[0].Grab() && m_Xfer[1].Grab())
               UpdateControls();         
        }

        private void button_Freeze_Click(object sender, EventArgs e)
        {
            AbortDlg abort0 = new AbortDlg(m_Xfer[0]);
            AbortDlg abort1 = new AbortDlg(m_Xfer[1]);
            if (m_Xfer[0].Freeze() && m_Xfer[1].Freeze())
            {
                if (abort0.ShowDialog() != DialogResult.OK && abort1.ShowDialog() != DialogResult.OK)
                {
                    m_Xfer[0].Abort();
                    m_Xfer[1].Abort();
                }
                UpdateControls();
            }
        }

        
        //Catch the WM_SYSCOMMAND message and process it.
        protected override void WndProc ( ref Message msg )
        {   
              if(msg.Msg == (int)WindowMessages.wmSysCommand)
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