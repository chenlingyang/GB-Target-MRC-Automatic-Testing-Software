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

namespace DALSA.SaperaLT.Demos.NET.CSharp.BayerDemo
{
    public partial class BayerDemoDlg : Form
    {

       private static SapFormatInfo[] m_BayerOutputFormatValues = { 
											new SapFormatInfo(SapFormat.RGB8888, "RGB 8888"), 
											new SapFormatInfo(SapFormat.RGB101010, "RGB 101010"), 
											new SapFormatInfo(SapFormat.RGB16161616, "RGB 16161616") 
																  };
       
       private SapAcquisition m_Acquisition;
       private SapBuffer m_Buffers;
       private SapAcqToBuf m_Xfer;
       private SapView m_View;
       private SapBayer m_Bayer;
       private SapProcessing  m_Pro;
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
          BayerDemoDlg BD = argsNotify.Context as BayerDemoDlg;
          // If grabbing in trash buffer, do not display the image, update the
          // appropriate number of frames on the status bar instead
          if (argsNotify.Trash)
             BD.Invoke(new DisplayFrameAcquired(BD.ShowFrameNumber), argsNotify.EventCount, true);  
          // Refresh view
          else
          {
             BD.Invoke(new DisplayFrameAcquired(BD.ShowFrameNumber), argsNotify.EventCount, false);
             BD.m_Pro.ExecuteNext();
          }
       }

       //
      // This function is called each time a buffer has been processed by the processing object
      //
       static void ProCallback(object sender, SapProcessingDoneEventArgs pInfo)
      {
         BayerDemoDlg BD = pInfo.Context as BayerDemoDlg;

         // Check if bayer conversion was enabled and if it's been done by software
         if (BD.m_Bayer.Enabled && BD.m_Bayer.SoftwareConversion)
         {
            // Show current buffer index and execution time in millisecond
            BD.m_Executetime = String.Format("Bayer conversion = {0:0.00}", BD.m_Pro.Time);
         }
         else
            BD.m_Executetime = "";
         
         // Refresh view
         BD.m_View.Show();
      }

      //
      // This function is called each time a buffer has been shown by the view object
      //
       static void ViewCallback(object sender, SapDisplayDoneEventArgs pInfo)
       {
         BayerDemoDlg BD = pInfo.Context as BayerDemoDlg;

         if( !BD.m_ImageBox.IsTrackerEmpty)
            BD.m_ImageBox.DisplayTracker();
       }

       static void GetSignalStatus(object sender, SapSignalNotifyEventArgs argsSignal)
       {
          BayerDemoDlg BD = argsSignal.Context as BayerDemoDlg;
          SapAcquisition.AcqSignalStatus signalStatus = argsSignal.SignalStatus;

          BD.m_IsSignalDetected = (signalStatus != SapAcquisition.AcqSignalStatus.None);
          if (BD.m_IsSignalDetected == false)
             BD.StatusLabelInfo.Text = "Online... No camera signal detected";
          else BD.StatusLabelInfo.Text = "Online... Camera signal detected";
       }
 
       public BayerDemoDlg()
       {
            m_Acquisition = null;
            m_Buffers = null;
            m_Xfer = null;
            m_View = null;
            m_Bayer = null;
            m_Pro = null;
            m_Executetime = "";


            // With the introduction of the new(est) Color Conversion Demo
            // this is now deprecated and not maintained. 
            if(MessageBox.Show("Please note the Bayer Demo is now deprecated.\nYou should use the Color Conversion Demo instead.\nDo you want to continue?",
               "Bayer Demo is deprecated", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
            {
               this.Close();
               return;
            }

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

            // Fill Bayer output format control
            for (int i = 0; i < m_BayerOutputFormatValues.Length; i++)
               comboBox_Output_Conversion.Items.Add(m_BayerOutputFormatValues[i]);

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
                str = String.Format("Frames acquired in trash buffer: {0}", number);
                this.StatusLabelInfoTrash.Text = str;
             }
          }
          else
          {

             if (!string.IsNullOrEmpty(m_Executetime))
                this.StatusLabelInfo.Text = m_Executetime;
             else
             {
                str = String.Format("Frames acquired :{0}", number);
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
                m_Bayer = new SapBayer(m_Acquisition, m_Buffers);

        
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
               m_Bayer = new SapBayer(m_Buffers);
               StatusLabelInfo.Text = "offline... Load images";
          }

          m_View = new SapView(m_Buffers);
           //event for view  
          m_View.DisplayDone += new SapDisplayDoneHandler(ViewCallback);
          m_View.DisplayDoneContext = this;
          m_View.DisplayDoneEnable = true;
          m_Pro = new SapMyProcessing(m_Buffers, m_Bayer, new SapProcessingDoneHandler(ProCallback), this);
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
              m_Bayer = new SapBayer(m_Acquisition, m_Buffers);


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
              m_Bayer = new SapBayer(m_Buffers);
              StatusLabelInfo.Text = "offline... Load images";
           }

           m_View = new SapView(m_Buffers);
           //event for view 
           m_View.DisplayDone += new SapDisplayDoneHandler(ViewCallback);
           m_View.DisplayDoneContext = this;
           m_View.DisplayDoneEnable = true;
           m_Pro = new SapMyProcessing(m_Buffers, m_Bayer, new SapProcessingDoneHandler(ProCallback), this);
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

            // Validate the video format of the configuration file
            if (m_Acquisition != null)
            {
               int videoType = 0;

               if (!m_Acquisition.GetParameter(SapAcquisition.Prm.VIDEO, out videoType))
               {
                  DestroyObjects();
                  return false;
               }

               if (videoType != (int)SapAcquisition.Val.VIDEO_BAYER)
               {
                  MessageBox.Show("The specified configuration file does not correspond to a Bayer camera");
                  DestroyObjects();
                  return false;
               }
            }

            // Enable/Disable bayer conversion
            // This call may require to modify the acquisition output format.
            // For this reason, it has to be done after creating the acquisition object but before
            // creating the output buffer object.
            if (m_Bayer != null && !m_Bayer.Enable(checkBox_Bayer_Conversion.Checked, checkBox_Hard_Conversion.Checked))
            {
               checkBox_Bayer_Conversion.Checked = false;
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

            // Create bayer object
            if (m_Bayer!= null && !m_Bayer.Initialized) 
            {
               if(m_Bayer.Create() == false)
               {
                  DestroyObjects();
                  return false;
               }
            }

            // Create view object
            if (m_View != null && !m_View.Initialized) 
            {
               if (m_Bayer != null && m_Bayer.Initialized)
               {
                  // Set buffer to be viewed
                  // When using hardware bayer decoder, view the acquired RGB buffer,
                  // otherwise, for software bayer conversion, view the converted RGB buffer.
                  SapBuffer bayerBuffer = m_Bayer.BayerBuffer;
                  if (bayerBuffer != null && bayerBuffer.Initialized)
                     m_View.Buffer = bayerBuffer;
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
	         if( m_Pro != null && !m_Pro.Initialized)
	         {
		         if( !m_Pro.Create())
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
            if (m_Bayer != null && m_Bayer.Initialized)
                m_Bayer.Destroy(); 
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
          if (m_Bayer != null)
          { m_Bayer.Dispose(); m_Bayer = null; }
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

         bool bAcqNoGrab			= m_Xfer !=null && m_Xfer.Initialized && !m_Xfer.Grabbing;
         bool bAcqGrab				= m_Xfer !=null && m_Xfer.Initialized && m_Xfer.Grabbing;
         bool bNoGrab				= m_Xfer == null || !m_Xfer.Grabbing;
         bool isBayerEnabled		= m_Bayer != null && m_Bayer.Initialized && m_Bayer.Enabled;
         bool isBayerAvailable	= m_Acquisition != null && m_Acquisition.Initialized && m_Acquisition.BayerAvailable;
         bool isBayerSoftware		= m_Bayer != null && m_Bayer.Initialized && m_Bayer.SoftwareConversion;

         // Acquisition Control
         button_Grab.Enabled = bAcqNoGrab;
         button_Freeze.Enabled = bAcqGrab;
         button_Snap.Enabled = bAcqNoGrab;
      
         // Options
         button_LoadConfig.Enabled = bAcqNoGrab || bNoGrab;
         button_Buffers.Enabled =  bNoGrab;
         button_View.Enabled = bNoGrab;
         
         // File Options
         button_New.Enabled = bNoGrab;
         button_Load_Raw.Enabled = bNoGrab && (isBayerSoftware || !isBayerEnabled);
         button_Save_Raw.Enabled = bNoGrab && (isBayerSoftware || !isBayerEnabled);
         button_Save_Converted.Enabled = bNoGrab && isBayerEnabled;
         
         // Bayer Options
         checkBox_Bayer_Conversion.Checked = isBayerEnabled;
         checkBox_Bayer_Conversion.Enabled = bNoGrab;
        
         checkBox_Hard_Conversion.Checked =  isBayerAvailable && !isBayerSoftware;
         checkBox_Hard_Conversion.Enabled = isBayerAvailable && isBayerEnabled && bNoGrab;
         
         // Set selection for Bayer output format control
         for (int i = 0; i < m_BayerOutputFormatValues.Length; i++)
         {
            if (m_BayerOutputFormatValues[i].Value == m_Bayer.OutputFormat)
            {
               comboBox_Output_Conversion.SelectedIndex = i;
               break;
            }
         }

         comboBox_Output_Conversion.Enabled = bNoGrab;
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

       private void BayerDemoDlg_FormClosed(object sender, FormClosedEventArgs e)
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

       private void BayerDemoDlg_Load(object sender, EventArgs e)
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
          SapBuffer pBayerBuffer = m_Bayer.BayerBuffer;
          if (pBayerBuffer != null && pBayerBuffer != m_Buffers)
          {
             pBayerBuffer.Clear();
          }
          m_ImageBox.Refresh();

       }

       private void button_Load_Raw_Click(object sender, EventArgs e)
       {
          LoadSaveDlg newDialogLoad = new LoadSaveDlg(m_Buffers, true, false);
          if (newDialogLoad.ShowDialog() == DialogResult.OK)
          {
             if (m_Bayer.Enabled && m_Bayer.SoftwareConversion)
             {
                m_Pro.Execute();
             } 
          }
          m_ImageBox.OnSize();
          newDialogLoad.Dispose();
       }

       private void button_Save_Raw_Click(object sender, EventArgs e)
       {
          LoadSaveDlg newDialogSave = new LoadSaveDlg(m_Bayer.Buffer, false, false);
         // Show the dialog and process the result
         newDialogSave.ShowDialog();
         newDialogSave.Dispose();

       }

       private void button_Save_Converted_Click(object sender, EventArgs e)
       {
          LoadSaveDlg newDialogSave = new LoadSaveDlg(m_Bayer.BayerBuffer, false, false);
         // Show the dialog and process the result
         newDialogSave.ShowDialog();
         newDialogSave.Dispose();
       }

       //**************************************************************************************
       //
       //         Bayer Options
       //
       //**************************************************************************************

       private void button_Options_Click(object sender, EventArgs e)
       {
          BayerDlg dlg = new BayerDlg(m_Bayer, m_Xfer, m_ImageBox, m_Pro);      
          dlg.Show();
       }

       private void checkBox_Bayer_Conversion_CheckedChanged(object sender, EventArgs e)
       {
          DestroyObjects();
          CreateObjects();
        
          if (m_Bayer.Enabled && m_Bayer.SoftwareConversion)
             m_Pro.Execute();
          
          UpdateControls();
       }

       private void checkBox_Hard_Conversion_CheckedChanged(object sender, EventArgs e)
       {
          // Destroy objects
          DestroyObjects();
          // Recreate objects
          CreateObjects();   
          UpdateControls();	   
       }

       private void comboBox_Output_Conversion_SelectedIndexChanged(object sender, EventArgs e)
       {
          // Retrieve current Bayer decoder output format
          SapFormat bayerOutputFormat = m_Bayer.OutputFormat;

          // Before changing Bayer decoder output format, we have to destroy Sapera objects
          DestroyObjects();

          // Set new Bayer decoder output format
          SapFormatInfo selectFormat = (SapFormatInfo)comboBox_Output_Conversion.SelectedItem;
          m_Bayer.OutputFormat = selectFormat.Value;

          // Recreate objects
          if (!CreateObjects())
          {
             // If ever it fail, try to go back to previous state

             // Set Bayer decoder output format to what it was before
             m_Bayer.OutputFormat = bayerOutputFormat;
             CreateObjects();
          }

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