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
using System.Threading;

namespace DALSA.SaperaLT.Demos.NET.CSharp.GigEMetadataDemo
{
    public partial class GigEMetadataDemoDlg : Form
    {
        // Delegate to display number of frame acquired
        // Delegate is needed because .NEt framework does not support  cross thread control modification
        private delegate void RefreshControlDelegate();
        private delegate void CheckForLastFrameDelegate();

        static void xfer_XferNotify(object sender, SapXferNotifyEventArgs argsNotify)
        {
            GigEMetadataDemoDlg GigeDlg = argsNotify.Context as GigEMetadataDemoDlg;

            SapBuffer.DataState bufState = SapBuffer.DataState.Empty;
            int bufIndex = GigeDlg.m_Buffers.Index;

            bufState = GigeDlg.m_Buffers.get_State(bufIndex);
            GigeDlg.m_BufferIsValid[bufIndex] = (bufState == SapBuffer.DataState.Full);

            // Check if last frame is reached
            GigeDlg.Invoke(new CheckForLastFrameDelegate(GigeDlg.CheckForLastFrame));

            // Refresh view
            GigeDlg.m_View.Show();

            // Refresh controls
            GigeDlg.Invoke(new RefreshControlDelegate(GigeDlg.RefreshTextBox));
        }

        public GigEMetadataDemoDlg()
        {
            m_AcqDevice = null;
            m_Feature = null;
            m_Buffers = null;
            m_Xfer = null;
            m_View = null;
            m_Metadata = null;
            m_MetadataType = SapMetadata.MetadataType.Unknown;

            m_bRecordOn = false;
            m_bPlayOn = false;
            m_bPauseOn = false;

            m_XferDisconnectDuringSetup = false;
            m_IsSelectAvailable = false;
            m_bIsAreaScan = true;

            m_nFramesPerCallback = 1;
            m_bLastRecordActiveMode = false;

            m_BufferIsValid = null;

            InitializeComponent();

            richTextBox_Metadata.SelectionTabs = new int[1];
            int[] tabstops = { 170 };
            richTextBox_Metadata.SelectionTabs = tabstops;

            checkBox_EnableMetadata.Checked = false;

            m_originalAppTitle = this.Text;

            // Note:
            //  The code to initialize m_ImageBox was originally in the InitializeComponent function
            //  called above. However, it has been moved to the dialog constructor as a workaround
            //  to a Visual Studio Designer error when loading the DALSA.SaperaLT.SapClassBasic
            //  assembly under 64-bit Windows.
            //  As a consequence, it is not possible to adjust the m_ImageBox properties
            //  automatically using the Designer anymore, this has to be done manually.
            // 
            this.m_ImageBox = new MyImageBox(this);
            this.m_ImageBox.Location = new System.Drawing.Point(249 + 124, 71 + 86);
            this.m_ImageBox.Name = "m_ImageBox";
            this.m_ImageBox.PixelValueDisplay = null;
            this.m_ImageBox.Size = new System.Drawing.Size(386, 406);
            this.m_ImageBox.SliderEnable = true;
            this.m_ImageBox.SliderMaximum = 10;
            this.m_ImageBox.SliderMinimum = 0;
            this.m_ImageBox.SliderValue = 0;
            this.m_ImageBox.SliderVisible = true;
            this.m_ImageBox.TabIndex = 7;
            this.m_ImageBox.TrackerEnable = true;
            this.m_ImageBox.View = null;
            this.Controls.Add(this.m_ImageBox);

            AcqConfigDlg acqConfigDlg = new AcqConfigDlg(null, "", AcqConfigDlg.ServerCategory.ServerAcqDevice);
            if (acqConfigDlg.ShowDialog() == DialogResult.OK)
            {
                if (CreateNewObjects(Default_Nbr_buffers, acqConfigDlg) == false)
                {
                    this.Close();
                    return;
                }
            }
            else
            {
                MessageBox.Show("No cameras found or selected");
                this.Close();
                return;
            }
        }

        public class MyImageBox : ImageBox
        {
            private GigEMetadataDemoDlg m_GigEMetadataDemoDlg = null;
            public MyImageBox(GigEMetadataDemoDlg gigEMetadataDemoDlg)
            {
                m_GigEMetadataDemoDlg = gigEMetadataDemoDlg;
            }

            protected override void Slider_Scroll(object sender, EventArgs e)
            {
                if ((m_pView != null) && (m_pView.Initialized == true))
                {
                    m_pView.Buffer.Index = Slider.Value;
                    m_pView.Show();

                    m_GigEMetadataDemoDlg.UpdateMetadataList();
                    m_GigEMetadataDemoDlg.UpdateTextBox();
                }
            }

        }

        private void RefreshTextBox()
        {
            UpdateFrameRate();
            UpdateTextBox();
        }

        //*****************************************************************************************
        //
        //             Create and Destroy Object
        //
        //*****************************************************************************************

        // Create new objects with acquisition information
        public bool CreateNewObjects(int Nbr_Buffers, AcqConfigDlg acqConfigDlg)
        {
            if (acqConfigDlg != null)
            {
                m_ServerLocation = acqConfigDlg.ServerLocation;
                m_ConfigFileName = acqConfigDlg.ConfigFile;
            }

            m_AcqDevice = new SapAcqDevice(m_ServerLocation, m_ConfigFileName);
            m_Feature = new SapFeature(m_ServerLocation);
            m_Buffers = new SapBufferWithTrash(Nbr_Buffers);
            m_Metadata = new SapMetadata(m_AcqDevice, m_Buffers);
            m_Xfer = new SapAcqDeviceToBuf(m_AcqDevice, m_Buffers);
            m_View = new SapView(m_Buffers);

            m_ImageBox.View = m_View;

            m_Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            m_Xfer.XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
            m_Xfer.XferNotifyContext = this;

            if (!CreateObjects())
            {
                DisposeObjects();
                return false;
            }

            // Resize ImageBox to take into account the size of created sapview
            m_ImageBox.OnSize();
            UpdateControls();
            UpdateTextBox();

            return true;
        }

        // Call Create Object
        private bool CreateObjects()
        {
            return CreateObjects(true);
        }

        private bool CreateObjects(bool createAcqDevice)
        {
            // Create acquisition object
            if ((createAcqDevice == true) && (m_AcqDevice != null))
            {
                if (m_AcqDevice.Create() == false)
                {
                    DestroyObjects();
                    return false;
                }
            }

            // Make sure the acq device supports mandatory metadata features
            if (!SapMetadata.IsMetadataSupported(m_AcqDevice))
            {
                MessageBox.Show("This demo only supports Teledyne Dalsa and Teledyne Lumenera cameras with metadata features", "Incompatible camera",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                DestroyObjects();
                return false;
            }

            // Create feature object
            if ((m_Feature != null) && (m_Feature.Initialized == false))
            {
                if (m_Feature.Create() == false)
                {
                    DestroyObjects();
                    return false;
                }
            }

            // Create metadata object
            if ((m_Metadata != null) && (m_Metadata.Initialized == false))
            {
                if (m_Metadata.Create() == false)
                {
                    DestroyObjects();
                    return false;
                }
            }

            m_bIsAreaScan = (m_Metadata.Type == SapMetadata.MetadataType.PerFrame);

            // Check if metadata selectors can be enabled through application code
            m_IsSelectAvailable = false;
            string featurename = m_bIsAreaScan ? "ChunkEnable" : "endOfLineMetadataContentActivationMode";
            if (m_AcqDevice.GetFeatureInfo(featurename, m_Feature) == true)
            {
                m_IsSelectAvailable = (m_Feature.DataAccessMode == SapFeature.AccessMode.RW);
            }

            // Check if the transfer needs to remain disconnected when enabling / disabling / selecting metadata
            m_XferDisconnectDuringSetup = false;
            featurename = m_bIsAreaScan ? "ChunkModeActive" : "endOfLineMetadataMode";
            if (m_AcqDevice.GetFeatureInfo(featurename, m_Feature) == true)
            {
                m_XferDisconnectDuringSetup = (m_Feature.DataWriteMode == SapFeature.WriteMode.NotConnected);
            }

            // Create buffer object
            if ((m_Buffers != null) && (m_Buffers.Initialized == false))
            {
                if (m_Buffers.Create() == false)
                {
                    DestroyObjects();
                    return false;
                }
                m_Buffers.Clear();
                m_BufferIsValid = new bool[m_Buffers.Count];
            }

            // Create view object
            if ((m_View != null) && (m_View.Initialized == false))
            {
                if (m_View.Create() == false)
                {
                    DestroyObjects();
                    return false;
                }
            }

            // Create Xfer object
            if ((m_Xfer != null) && (m_Xfer.Initialized == false))
            {
                //Number of frames per callback retreived
                int nFramesPerCallback;

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
                if (m_Xfer.Create() == false)
                {
                    DestroyObjects();
                    return false;
                }

                // Restore original command timeout value
                SapManager.CommandTimeout = oldCommandTimeout;

                // Initialize tranfer object and reset source/destination index

                m_Xfer.Init(true);

                // Retrieve number of frames per callback
                // It may be less than what we have asked for.
                nFramesPerCallback = m_Xfer.Pairs[0].FramesPerCallback;
                if (m_nFramesPerCallback > nFramesPerCallback)
                {
                    m_nFramesPerCallback = nFramesPerCallback;
                    MessageBox.Show("No memory");
                }
            }

            // Show type of metadata in title bar
            m_appTitle = m_originalAppTitle;
            m_MetadataType = m_Metadata.Type;
            switch (m_MetadataType)
            {
                case SapMetadata.MetadataType.PerFrame:
                    m_appTitle += " (Per-Frame Metadata)";
                    break;
                case SapMetadata.MetadataType.PerLine:
                    m_appTitle += " (Per-Line Metadata)";
                    labelLineIndex.Text = "Line Index (0.." + (int)(m_Buffers.Height - 1) + "): ";
                    labelLineIndex.Visible = true;
                    textBoxLineIndex.Visible = true;
                    break;
                default:
                    m_appTitle += " -- Acquisition device doesn't support Metadata";
                    break;
            }

            this.Text = m_appTitle;

            ReadCameraTimestamp();

            if (m_XferDisconnectDuringSetup == true)
                m_Xfer.Disconnect();

            // Feed the list of selectors
            checkedListBox_MetadataItemsEnabled.Items.Clear();
            checkedListBox_MetadataItemsEnabled.ItemCheck -= checkedListBox_MetadataItemsEnabled_ItemCheck;
            for (int selectorIndex = 0; selectorIndex < m_Metadata.SelectorCount; selectorIndex++)
            {
                string name;
                m_Metadata.GetSelectorName(selectorIndex, out name);
                bool bIsSelected = m_Metadata.IsSelected(selectorIndex);
                checkedListBox_MetadataItemsEnabled.Items.Add(name, bIsSelected);
            }
            checkedListBox_MetadataItemsEnabled.ItemCheck += checkedListBox_MetadataItemsEnabled_ItemCheck;

            if (m_XferDisconnectDuringSetup == true)
                m_Xfer.Connect();

            // Clear extracted chunks in their display listboxes
            richTextBox_Metadata.Clear();

            m_DeviceModelName = "Unknown";
            m_DeviceUserID = "Unknown";
            m_AcqDevice.GetFeatureValue("DeviceModelName", out m_DeviceModelName);
            m_AcqDevice.GetFeatureValue("DeviceUserID", out m_DeviceUserID);

            m_ImageBox.OnSize();
            UpdateControls();
            UpdateTextBox();

            return true;
        }

        private void DestroyObjects()
        {
            DestroyObjects(true);
        }

        private void DestroyObjects(bool destroyAcqDevice)
        {
            if ((m_Xfer != null) && (m_Xfer.Initialized == true))
                m_Xfer.Destroy();
            if ((m_View != null) && (m_View.Initialized == true))
                m_View.Destroy();
            if ((m_Buffers != null) && (m_Buffers.Initialized == true))
                m_Buffers.Destroy();
            if ((m_Metadata != null) && (m_Metadata.Initialized == true))
                m_Metadata.Destroy();
            if ((m_Feature != null) && (m_Feature.Initialized == true))
                m_Feature.Destroy();
            if ((destroyAcqDevice == true) && (m_AcqDevice != null) && (m_AcqDevice.Initialized == true))
                m_AcqDevice.Destroy();
        }

        private void DisposeObjects()
        {
            if (m_Xfer != null)
            {
                m_Xfer.Dispose();
                m_Xfer = null;
            }

            if (m_View != null)
            {
                m_View.Dispose();
                m_View = null;
                m_ImageBox.View = null;
            }

            if (m_Buffers != null)
            {
                m_Buffers.Dispose();
                m_Buffers = null;
            }

            if (m_Metadata != null)
            {
                m_Metadata.Dispose();
                m_Metadata = null;
            }

            if (m_Feature != null)
            {
                m_Feature.Dispose();
                m_Feature = null;
            }

            if (m_AcqDevice != null)
            {
                m_AcqDevice.Dispose();
                m_AcqDevice = null;
            }
        }

        //**********************************************************************************
        //
        //          Window related functions
        //
        //**********************************************************************************


        protected override void OnResize(EventArgs argsPaint)
        {
            if (m_ImageBox != null)
                m_ImageBox.OnSize();
            base.OnResize(argsPaint);
        }

        private void GigEMetadataDemoDlg_FormClosed(object sender, FormClosedEventArgs e)
        {
            DestroyObjects();
            DisposeObjects();
        }

        //**********************************************************************************
        //
        //             Timer related functions
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
        //             General functions
        //
        //**********************************************************************************


        private void UpdateControls()
        {
            bool bAcqNoGrab = (m_Xfer != null) && !m_bRecordOn && !m_bPlayOn;
            bool bNoGrab = !m_bRecordOn && !m_bPlayOn;
            bool bMetadataEnabled = m_Metadata.Enable;

            // Record Control
            button_Record.Enabled = bAcqNoGrab;
            button_Play.Enabled = bNoGrab;
            button_Pause.Enabled = !bNoGrab;
            button_Stop.Enabled = !bNoGrab;
            button_Pause.Text = (m_bPauseOn == true) ? "Continue" : "Pause";

            // General Options
            button_Buffers.Enabled = bNoGrab;
            button_LoadAcqConfig.Enabled = bNoGrab;
            button_HighFrameRate.Enabled = bNoGrab;

            // File Options
            button_LoadCurrent.Enabled = bNoGrab;
            button_LoadSequence.Enabled = bNoGrab;
            button_SaveCurrent.Enabled = bNoGrab;
            button_SaveSequence.Enabled = bNoGrab;

            // Metadata Options
            checkBox_EnableMetadata.Enabled = bAcqNoGrab;
            checkBox_EnableMetadata.Checked = bMetadataEnabled;
            checkedListBox_MetadataItemsEnabled.Enabled = bAcqNoGrab && bMetadataEnabled && m_IsSelectAvailable;
            richTextBox_Metadata.Enabled = bAcqNoGrab && bMetadataEnabled;
            button_SaveMetadata.Enabled = bAcqNoGrab && bMetadataEnabled;

            // Slider
            m_ImageBox.SliderEnable = bNoGrab || (m_bPlayOn && m_bPauseOn);
            m_ImageBox.SliderMinimum = 0;
            m_ImageBox.SliderMaximum = m_Buffers.Count - 1;
        }

        private void UpdateTextBox()
        {
            textBox_BufferDisplayed.Text = Convert.ToString(m_Buffers.Index + 1);
            textBox_NumberOfBuffers.Text = m_Buffers.Count.ToString();

            if (m_MinTime != float.MaxValue)
                textBox_MinTimeBetweenFrames.Text = m_MinTime.ToString();
            else
                textBox_MinTimeBetweenFrames.Text = "0";
            textBox_MaxTimeBetweenFrames.Text = m_MaxTime.ToString();

            m_ImageBox.SliderValue = m_Buffers.Index;
            if (m_bPlayOn == false)
                textBox_FrameRate.Text = m_Buffers.FrameRate.ToString();

            ulong timestampBufferCurrent = m_Buffers.get_DeviceTimeStamp(m_Buffers.Index);
            textBox_CurrentBufferTimestamp.Text = timestampBufferCurrent.ToString();

            if (m_Buffers.Index == 0)
                textBox_DeltaTimestamp.Text = string.Empty;
            else
            {
                ulong timestampBufferPrevious = m_Buffers.get_DeviceTimeStamp(m_Buffers.Index - 1);
                if (timestampBufferCurrent == timestampBufferPrevious)
                    textBox_DeltaTimestamp.Text = string.Empty;
                else
                    textBox_DeltaTimestamp.Text = (timestampBufferCurrent - timestampBufferPrevious).ToString();
            }
        }

        private void SystemEvents_SessionEnded(object sender, SessionEndedEventArgs e)
        {
            // The FormClosed event is not invoked when logging off or shutting down,
            // so we need to clean up here too.
            DestroyObjects();
            DisposeObjects();
        }

        //**********************************************************************************
        //
        //             Frame rate related functions
        //
        //**********************************************************************************


        private void CheckForLastFrame()
        {
            // Check for last frame
            if (m_Buffers.Index == m_Buffers.Count - 1)
            {
                if (m_bRecordOn == true)
                {
                    m_bRecordOn = false;
                }
                if (m_bPlayOn == true)
                {
                    m_bPlayOn = false;
                    timer.Enabled = false;
                }
                UpdateControls();
                UpdateMetadataList();
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

        //*****************************************************************************************
        //
        //             General Options
        //
        //*****************************************************************************************

        private void button_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button_Load_Config_Click(object sender, EventArgs e)
        {
            // Set new acquisition parameters
            AcqConfigDlg acqConfigDlg = new AcqConfigDlg(AcqConfigDlg.ServerCategory.ServerAcqDevice);
            if (acqConfigDlg.ShowDialog() == DialogResult.OK)
            {
                // Destroy objects
                DestroyObjects();

                // Backup
                SapLocation backupLocation = m_AcqDevice.Location;
                string backupConfigFileName = m_AcqDevice.ConfigFileName;

                // Update object
                m_AcqDevice.Location = acqConfigDlg.ServerLocation;
                m_AcqDevice.ConfigFileName = acqConfigDlg.ConfigFile;
                m_Feature.Location = acqConfigDlg.ServerLocation;

                // Recreate objects
                if (CreateObjects() == false)
                {
                    // Restore backup
                    m_AcqDevice.Location = backupLocation;
                    m_AcqDevice.ConfigFileName = backupConfigFileName;
                    m_Feature.Location = backupLocation;
                    CreateObjects();
                    MessageBox.Show("Failed to load config: restored old config.");
                }

                ReadCameraTimestamp();
                UpdateTextBox();
            }
            else
                MessageBox.Show("No Modification in Acquisition");

            m_ImageBox.Refresh();
        }

        private void button_Buffers_Click(object sender, EventArgs e)
        {
            // Set new buffer parameters
            BufferDlg dlg = new BufferDlg(m_Buffers, m_View.Display, true);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // Destroy objects
                DestroyObjects();

                // Backup
                int backupCount = m_Buffers.Count;

                // Update object
                m_Buffers.Count = dlg.Count;

                // Recreate objects
                if (CreateObjects() == false)
                {
                    // Restore backup
                    m_Buffers.Count = backupCount;
                    CreateObjects();
                    MessageBox.Show("Failed to update buffer frames count: restored old count.");
                }

                UpdateMetadataList();
                UpdateControls();
                UpdateTextBox();
            }
            else
                MessageBox.Show("No Modification in Buffer");

            m_ImageBox.Refresh();
        }

        private void button_High_Frame_Rate_Click(object sender, EventArgs e)
        {
            HighFrameDlg dlg = new HighFrameDlg(m_nFramesPerCallback, 0, null);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // Destroy objects
                DestroyObjects();

                // Backup
                int backupFramesPerCallback = m_nFramesPerCallback;

                // Update object
                m_nFramesPerCallback = dlg.Frame_Per_Callback;

                // Recreate objects
                if (CreateObjects() == false)
                {
                    // Restore backup
                    m_nFramesPerCallback = backupFramesPerCallback;
                    CreateObjects();
                    MessageBox.Show("Failed to update frames per callback count: restored old count.");
                }
            }
            else
                MessageBox.Show("No Modification in Frames per callback count");

            m_ImageBox.Refresh();
        }

        //*****************************************************************************************
        //
        //             Acquisition Control
        //
        //*****************************************************************************************

        private void button_Record_Click(object sender, EventArgs e)
        {
            // Reset source and destination indices
            m_Xfer.Init(true);

            // Reset the frame rate statistics ahead of each transfer stream
            SapXferFrameRateInfo stats = m_Xfer.FrameRateStatistics;
            stats.Reset();

            // Acquire all frames
            if (m_Xfer.Snap(m_Buffers.Count))
            {
                m_bRecordOn = true;
                m_bLastRecordActiveMode = checkBox_EnableMetadata.Checked;
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
            timer.Interval = Math.Max(1, (int)(1000.0 / float.Parse(textBox_FrameRate.Text)));
            timer.Enabled = true;

            m_bPlayOn = true;
            UpdateControls();
        }

        private void button_Pause_Click(object sender, EventArgs e)
        {
            if (m_bPauseOn == false)
            {
                if (m_bRecordOn == true)
                {
                    // Stop current acquisition
                    if (m_Xfer.Freeze() == false)
                        return;

                    AbortDlg dlg = new AbortDlg(m_Xfer);
                    if (dlg.ShowDialog() != DialogResult.OK)
                        m_Xfer.Abort();
                }
                else if (m_bPlayOn == true)
                {
                    // Stop playback timer
                    timer.Enabled = false;
                }
            }
            else
            {
                // Check if recording or playing
                if (m_bRecordOn == true)
                {
                    //int frameTime = (int)(1000.0 / m_Buffers->GetFrameRate());
                    //SetTimer(1, frameTime, NULL);
                    // Acquire remaining frames
                    if (m_Xfer.Snap(m_Buffers.Count - m_Buffers.Index - 1) == false)
                        return;
                }
                else if (m_bPlayOn == true)
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
            if (m_bRecordOn == true)
            {
                // Stop current acquisition
                if (m_Xfer.Freeze() == false)
                    return;

                AbortDlg dlg = new AbortDlg(m_Xfer);
                if (dlg.ShowDialog() != DialogResult.OK)
                    m_Xfer.Abort();

                m_bRecordOn = false;
            }
            else if (m_bPlayOn == true)
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
        //             File Control
        //
        //*****************************************************************************************

        private void button_Load_Sequence_Click(object sender, EventArgs e)
        {
            if (m_Buffers.Format == SapFormat.Mono16)
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
            if (m_Buffers.Format == SapFormat.Mono16)
                MessageBox.Show("Saving images in AVI format requires downsampling them to 8-bit pixel depth.\nYou will not be able to reload this sequence in this application unless you change the buffer format.");

            if (m_Buffers.Format == SapFormat.RGBR888)
                MessageBox.Show("Saving images in AVI format requires conversion to RGB888 format (blue first).\nYou will not be able to reload this sequence in this application unless you change the buffer format.");

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
        //             System menu
        //
        //*****************************************************************************************

        private void GigEMetadataDemoDlg_Load(object sender, EventArgs e)
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

        private void UpdateMetadataList()
        {
            richTextBox_Metadata.ResetText();

            if (!m_Metadata.Enable)
                return;

            if (!m_BufferIsValid[m_Buffers.Index])
                return;

            if (m_bLastRecordActiveMode == true)
            {
                if (m_MetadataType == SapMetadata.MetadataType.PerFrame)
                {
                    if (!m_Metadata.Extract(m_Buffers.Index))
                        return;
                }
                else
                {
                    int lineIndex = 0;
                    int.TryParse(textBoxLineIndex.Text, out lineIndex);
                    if (!m_Metadata.Extract(m_Buffers.Index, lineIndex))
                        return;
                }


                for (int resultIndex = 0; resultIndex < m_Metadata.ExtractedResultCount; resultIndex++)
                {
                    string name, value;
                    m_Metadata.GetExtractedResult(resultIndex, out name, out value);
                    richTextBox_Metadata.AppendText(name);
                    richTextBox_Metadata.AppendText("\t");
                    richTextBox_Metadata.AppendText(value);
                    richTextBox_Metadata.AppendText("\n");
                }
            }
        }

        private void ReadCameraTimestamp()
        {
            // Current feature name in SFNC
            if (m_AcqDevice.IsFeatureAvailable("TimestampLatch"))
            {
                m_AcqDevice.SetFeatureValue("TimestampLatch", 1);
            }
            // Deprecated in SFNC
            else if (m_AcqDevice.IsFeatureAvailable("GevTimestampControlLatch"))
            {
                m_AcqDevice.SetFeatureValue("GevTimestampControlLatch", 1);
            }
            // Specific to Teledyne DALSA
            else if (m_AcqDevice.IsFeatureAvailable("timestampControlLatch"))
            {
                m_AcqDevice.SetFeatureValue("timestampControlLatch", 1);
            }

            UInt64 timestamp = 0;

            // Current feature name in SFNC
            if (m_AcqDevice.IsFeatureAvailable("TimestampLatchValue"))
            {
                m_AcqDevice.GetFeatureValue("TimestampLatchValue", out timestamp);
            }
            // Deprecated in SFNC
            else if (m_AcqDevice.IsFeatureAvailable("GevTimestampValue"))
            {
                m_AcqDevice.GetFeatureValue("GevTimestampValue", out timestamp);
            }
            // Specific to Teledyne DALSA
            else if (m_AcqDevice.IsFeatureAvailable("timestampValue"))
            {
                m_AcqDevice.GetFeatureValue("timestampValue", out timestamp);
            }

            textBox_CameraTimestamp.Text = timestamp.ToString();
        }

        private void button_Update_Click(object sender, EventArgs e)
        {
            ReadCameraTimestamp();
        }

        private void button_Reset_Click(object sender, EventArgs e)
        {
            // Current feature name in SFNC
            if (m_AcqDevice.IsFeatureAvailable("TimestampReset"))
            {
                m_AcqDevice.SetFeatureValue("TimestampReset", 1);
            }
            // Deprecated in SFNC
            else if (m_AcqDevice.IsFeatureAvailable("GevTimestampControlReset"))
            {
                m_AcqDevice.SetFeatureValue("GevTimestampControlReset", 1);
            }
            // Specific to Teledyne DALSA
            else if (m_AcqDevice.IsFeatureAvailable("timestampControlReset"))
            {
                m_AcqDevice.SetFeatureValue("timestampControlReset", 1);
            }
            else
            {
                MessageBox.Show("Timestamp reset is not supported for this camera");
                return;
            }

            ReadCameraTimestamp();
        }

        private void checkBox_EnableMetadata_CheckedChanged(object sender, EventArgs e)
        {
            if (m_XferDisconnectDuringSetup == true)
                m_Xfer.Disconnect();

            m_Metadata.Enable = checkBox_EnableMetadata.Checked;

            // Check if metadata selectors can be enabled through application code
            m_IsSelectAvailable = false;
            string featurename = m_bIsAreaScan ? "ChunkEnable" : "endOfLineMetadataContentActivationMode";
            if (m_AcqDevice.GetFeatureInfo(featurename, m_Feature) == true)
            {
                m_IsSelectAvailable = (m_Feature.DataAccessMode == SapFeature.AccessMode.RW);
            }

            if (m_XferDisconnectDuringSetup == true)
                m_Xfer.Connect();

            UpdateControls();
        }

        private void checkedListBox_MetadataItemsEnabled_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int selectorIndex = e.Index;
            bool bIsSelected = (e.NewValue == CheckState.Checked);

            // Update selected item's checkbox
            m_Metadata.Select(selectorIndex, bIsSelected);

            // Update all items checkboxes, in case another one changed because of the itemIndex one
            checkedListBox_MetadataItemsEnabled.ItemCheck -= checkedListBox_MetadataItemsEnabled_ItemCheck;
            for (selectorIndex = 0; selectorIndex < m_Metadata.SelectorCount; selectorIndex++)
            {
                bIsSelected = m_Metadata.IsSelected(selectorIndex);
                checkedListBox_MetadataItemsEnabled.SetItemChecked(selectorIndex, bIsSelected);
            }
            checkedListBox_MetadataItemsEnabled.ItemCheck += checkedListBox_MetadataItemsEnabled_ItemCheck;

        }

        private void button_SaveMetadata_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "csv files (*.csv)|*.csv";
            dlg.FileName = "metadata.csv";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (m_Metadata.SaveToCSV(dlg.FileName))
                    MessageBox.Show("The metadata has been saved.", "We are done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("We failed to save the metadata!", "Oops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxLineIndex_TextChanged(object sender, EventArgs e)
        {
            UpdateMetadataList();
        }
    }
}
