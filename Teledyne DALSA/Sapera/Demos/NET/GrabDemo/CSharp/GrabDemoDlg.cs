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


namespace DALSA.SaperaLT.Demos.NET.CSharp.GrabDemo
{

    public partial class GrabDemoDlg : Form
    {

       // Delegate to display number of frame acquired 
       // Delegate is needed because .NEt framework does not support  cross thread control modification
       private delegate void DisplayFrameAcquired(int number, bool trash);

        static void xfer_XferNotify(object sender, SapXferNotifyEventArgs argsNotify)
        {
            GrabDemoDlg GrabDlg = argsNotify.Context as GrabDemoDlg;
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
            GrabDemoDlg GrabDlg = argsSignal.Context as GrabDemoDlg;
            SapAcquisition.AcqSignalStatus signalStatus = argsSignal.SignalStatus;

            GrabDlg.m_IsSignalDetected = (signalStatus != SapAcquisition.AcqSignalStatus.None);
            if (GrabDlg.m_IsSignalDetected == false)
                GrabDlg.StatusLabelInfo.Text = "Online... No camera signal detected";
            else GrabDlg.StatusLabelInfo.Text = "Online... Camera signal detected";
        }
       
        // Constructor
        public GrabDemoDlg()
        {
            m_Acquisition = null;
            m_Buffers = null;
            m_Xfer = null;
            m_View = null;
            m_IsSignalDetected = true;

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
                m_online = true;
            else
                m_online = false;
           
            if (!CreateNewObjects(acConfigDlg,false))
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
                // define on-line object
                m_Acquisition = new SapAcquisition(m_ServerLocation, m_ConfigFileName);
                if (SapBuffer.IsBufferTypeSupported (m_ServerLocation , SapBuffer.MemoryType .ScatterGather ))
                    m_Buffers = new SapBufferWithTrash(2, m_Acquisition, SapBuffer.MemoryType.ScatterGather);
                else
                    m_Buffers = new SapBufferWithTrash(2, m_Acquisition, SapBuffer.MemoryType.ScatterGatherPhysical);
                m_Xfer = new SapAcqToBuf(m_Acquisition, m_Buffers);
                m_View = new SapView(m_Buffers);
               

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
                m_View = new SapView(m_Buffers);
                StatusLabelInfo.Text = "offline... Load images";
            }

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

        // Create new object with Buffer information
        public bool  CreateNewObjects(BufferDlg BufDlg)
        {
            if (m_online)
            {
                // define on-line object
                m_Acquisition = new SapAcquisition(m_ServerLocation, m_ConfigFileName);
                if (BufDlg.Count > 1)
                    m_Buffers = new SapBufferWithTrash(BufDlg.Count, m_Acquisition, BufDlg.Type);
                else
                    m_Buffers = new SapBuffer(1, m_Acquisition, BufDlg.Type);
                m_Xfer = new SapAcqToBuf(m_Acquisition, m_Buffers);
                m_View = new SapView(m_Buffers);

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
                m_Buffers = new SapBuffer(BufDlg.Count,BufDlg.BWidth,BufDlg.BHeight,BufDlg.Format,BufDlg.Type);
                m_View = new SapView(m_Buffers);
                StatusLabelInfo.Text = "offline... Load images";

            }

            m_ImageBox.View = m_View;

            if (!CreateObjects())
            {
                DisposeObjects();
                return false;
            }

            m_ImageBox.OnSize();   
            EnableSignalStatus();
            return true;
        }

        // Call Create method  
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
                if (m_Xfer.Create() == false)
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
            { m_Acquisition.Dispose(); m_Acquisition = null;}

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

       private void GrabDemoDlg_FormClosed(object sender, FormClosedEventArgs e)
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
            m_Buffers.Clear();
            m_ImageBox.Refresh();
        }

        private void button_Load_Click(object sender, EventArgs e)
        {
            LoadSaveDlg newDialogLoad = new LoadSaveDlg(m_Buffers, true, false);
            // Show the dialog and process the result
            newDialogLoad.ShowDialog();
            newDialogLoad.Dispose();
            m_ImageBox.Refresh(); 
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            LoadSaveDlg newDialogSave = new LoadSaveDlg(m_Buffers, false, false);
            // Show the dialog and process the result
            newDialogSave.ShowDialog();
            newDialogSave.Dispose();
            m_ImageBox.Refresh();
        
        }


        //*****************************************************************************************
        //
        //					General Options
        //
        //*****************************************************************************************

        private void button_Buffer_Click(object sender, EventArgs e)
        {
           // Set new buffer parameters   
           BufferDlg dlg = new BufferDlg(m_Buffers,m_View.Display,m_online);      
           if (dlg.ShowDialog() == DialogResult.OK)
            {    
                DestroyObjects();                
                DisposeObjects();

                // Update objects with new buffer
                // Update objects with new acquisition
                if (!CreateNewObjects(dlg))
                {
                    MessageBox.Show("New objects creation has failed. Restoring original object ");
                    // Recreate original objects
                    if(!CreateNewObjects(null, true))
                    {
                        MessageBox.Show("Original object creation has failed. Closing application ");
                        Application.Exit();
                    }
                }
            }
            m_ImageBox.Refresh();
        }

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
        
        // Updates the menu items enabling/disabling the proper items depending on the state of the application
        void UpdateControls()
        {
            bool bAcqNoGrab = (m_Xfer != null) && (m_Xfer.Grabbing == false);
            bool bAcqGrab = (m_Xfer != null) && (m_Xfer.Grabbing == true);
            bool bNoGrab = (m_Xfer == null) || (m_Xfer.Grabbing == false);

            // Acquisition Control
            button_Grab.Enabled = bAcqNoGrab && m_online;
            button_Snap.Enabled = bAcqNoGrab && m_online;
            button_Freeze.Enabled = bAcqGrab && m_online;

            // File Options
            button_New.Enabled = bNoGrab;
            button_Load.Enabled = bNoGrab;
            button_Save.Enabled = bNoGrab;

            button_LoadConfig.Enabled = bAcqNoGrab || bNoGrab;
            button_Buffer.Enabled = bNoGrab;
        }

        //*****************************************************************************************
        //
        //					Acquisition Control
        //
        //*****************************************************************************************

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
           StatusLabelInfoTrash.Text = "";
           if (m_Xfer.Grab())
               UpdateControls();         
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
        //					System menu
        //
        //*****************************************************************************************

        private void GrabDemoDlg_Load(object sender, EventArgs e)
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