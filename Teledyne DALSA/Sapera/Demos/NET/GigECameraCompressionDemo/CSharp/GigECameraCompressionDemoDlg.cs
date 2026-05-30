using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;

using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.SapClassGui;


namespace DALSA.SaperaLT.Demos.NET.CSharp.CameraCompressionDemo
{
    public partial class CameraCompressionDemoDlg : Form
    {
        // Delegate to display number of frame acquired 
        // Delegate is needed because .NEt framework does not support  cross thread control modification
        private delegate void DisplayFrameAcquired(int number, bool trash);
        private delegate void RefreshControl();

        //
        // This function is called each time an image has been transferred into system memory by the transfer object
        //
        static void XferCallback(object sender, SapXferNotifyEventArgs argsNotify)
        {
            CameraCompressionDemoDlg dlg = argsNotify.Context as CameraCompressionDemoDlg;

            // If grabbing in trash buffer, do not display the image, update the
            // appropriate number of frames on the status bar instead
            if (argsNotify.Trash)
                dlg.Invoke(new DisplayFrameAcquired(dlg.ShowFrameNumber), argsNotify.EventCount, true);
            else
            {
                dlg.Invoke(new DisplayFrameAcquired(dlg.ShowFrameNumber), argsNotify.EventCount, false);

                // Can't use SapXferPair.CycleMode.NextEmpty and SapXferPair.CycleMode.NextWithTrash in conjunction with ExecuteNext(), use SapXferPair.CycleMode.WithTrash instead.
                dlg.m_Pro.ExecuteNext();
                dlg.m_View.Show();
            }
        }

        public CameraCompressionDemoDlg()
        {
            m_AcqDevice = null;
            m_Buffers = null;
            m_BuffersView = null;
            m_Xfer = null;
            m_View = null;
            m_Pro = null;
            m_JpegProcessing = null;
            m_CompressedImageAvailable = false;

            AcqConfigDlg acqConfigDlg = new AcqConfigDlg(null, "", AcqConfigDlg.ServerCategory.ServerAcqDevice);
            if (acqConfigDlg.ShowDialog() == DialogResult.OK)
            {
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
                this.m_ImageBox.Location = new System.Drawing.Point(186, 144);
                this.m_ImageBox.Name = "m_ImageBox";
                this.m_ImageBox.PixelValueDisplay = this.PixelDataValue;
                this.m_ImageBox.Size = new System.Drawing.Size(386, 229);
                this.m_ImageBox.SliderEnable = true;
                this.m_ImageBox.SliderMaximum = 10;
                this.m_ImageBox.SliderMinimum = 0;
                this.m_ImageBox.SliderValue = 0;
                this.m_ImageBox.SliderVisible = false;
                this.m_ImageBox.TabIndex = 12;
                this.m_ImageBox.TrackerEnable = false;
                this.m_ImageBox.View = null;
                this.Controls.Add(this.m_ImageBox);

                if (!CreateNewObjects(acqConfigDlg, false))
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

            m_JpegEnabled = m_JpegProcessing.IsImageCompressionModeJpeg();

            if (m_JpegEnabled && (m_Buffers.Format == SapFormat.YUY2))
                UpdateViewBuffersViewFormat(SapFormat.RGB8888);

            checkBox_EnableJpeg.Checked = m_JpegEnabled;
            numericUpDown_JpegQuality.Value = m_JpegEnabled ? m_JpegProcessing.GetImageCompressionQuality() : 0;
            numericUpDown_JpegQuality.Enabled = true;

            numericUpDown_JpegQuality.Maximum = m_JpegProcessing.GetImageCompressionQualityMax();

            m_Pro.SetDecompressionEnabled(m_JpegEnabled);

            // We want each buffer frames be set to empty after processing
            m_Xfer.AutoEmpty = false;
            m_Pro.AutoEmpty = true;
            m_Pro.Init(); // Set SapProcessing index to current buffer index (required if buffer info changed)

            m_Pro.InitProTimeStatistics();
            m_Pro.InitProImageStatistics();

            Update_numericUpDown_JpegQuality();
        }

        private void Update_numericUpDown_JpegQuality()
        {
            if (m_JpegEnabled)
            {
                numericUpDown_JpegQuality.Minimum = m_JpegProcessing.GetImageCompressionQualityMin();
                numericUpDown_JpegQuality.Value = m_JpegProcessing.GetImageCompressionQuality();
            }
            else
            {
                numericUpDown_JpegQuality.Minimum = 0;
                numericUpDown_JpegQuality.Value = 0;
            }
        }

        private void ShowFrameNumber(int number, bool trash)
        {
            String str;
            if (trash)
            {
                str = String.Format("Skipped frames count (processing queue full): {0}", number);
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
        //             Create and Destroy Object
        //
        //*****************************************************************************************

        // Create new objects with acquisition information
        public bool CreateNewObjects(AcqConfigDlg acqConfigDlg, bool Restore)
        {
            if (!Restore)
            {
                m_ServerLocation = acqConfigDlg.ServerLocation;
                m_ConfigFileName = acqConfigDlg.ConfigFile;
            }
            m_AcqDevice = new SapAcqDevice(m_ServerLocation, m_ConfigFileName);
            if (SapBuffer.IsBufferTypeSupported(m_ServerLocation, SapBuffer.MemoryType.ScatterGather))
            {
                m_Buffers = new SapBufferWithTrash(4, m_AcqDevice, SapBuffer.MemoryType.ScatterGather);
                m_BuffersView = new SapBuffer(1, 0, 0, SapFormat.Unknown, SapBuffer.MemoryType.ScatterGather); // w, h and format will be set in CreateObjects(true); No need to have more than 1 view buffer
            }
            else
            {
                m_Buffers = new SapBufferWithTrash(4, m_AcqDevice, SapBuffer.MemoryType.ScatterGatherPhysical);
                m_BuffersView = new SapBuffer(1, m_Buffers.Width, m_Buffers.Height, m_Buffers.Format, SapBuffer.MemoryType.ScatterGatherPhysical); // w, h and format will be set in CreateObjects(true); No need to have more than 1 view buffer
            }

            m_JpegProcessing = new JpegProcessing(m_AcqDevice);
            m_Xfer = new SapAcqDeviceToBuf(m_AcqDevice, m_Buffers);
            m_Pro = new SapMyProcessing(m_Buffers, m_BuffersView, m_JpegProcessing, new SapProcessingDoneHandler(ProCallback), this);
            m_View = new SapView(m_BuffersView);
            m_ImageBox.View = m_View;

            m_Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            m_Xfer.XferNotify += new SapXferNotifyHandler(XferCallback);
            m_Xfer.XferNotifyContext = this;

            if (!CreateObjects(true, m_BuffersView.Format))
            {
                DisposeObjects();
                return false;
            }

            StatusLabelInfo.Text = "Online... Waiting grabbed images";

            // Resize ImageBox to take into account the size of created sapview
            m_ImageBox.OnSize();
            UpdateControls();

            return true;
        }


        // Call Create Object
        private bool CreateObjects(bool updateViewFormat, SapFormat viewFormat)
        {
            // Create acquisition object
            if (m_AcqDevice != null && !m_AcqDevice.Initialized)
            {
                if (m_AcqDevice.Create() == false)
                {
                    DestroyObjects();
                    return false;
                }
            }

            // Create the Jpeg processing object (uses the acq object).
            if (m_JpegProcessing != null && m_JpegProcessing.Initialized() == false)
            {
                if (m_JpegProcessing.Create() == false)
                {
                    DestroyObjects();
                    return false;
                }
            }

            // Check the device supports compression. If not, tell the user and terminate.
            if (!m_JpegProcessing.JpegAvailable())
            {
                MessageBox.Show("This demo only supports image compression enabled devices.");
                DestroyObjects();
                return false;
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

            if (updateViewFormat == true)
            {
                SapFormat newViewFormat = viewFormat == SapFormat.Unknown ? m_Buffers.Format : viewFormat;

                m_BuffersView.Width = m_Buffers.Width;
                m_BuffersView.Height = m_Buffers.Height;
                m_BuffersView.Format = newViewFormat;
            }

            if (m_BuffersView != null && !m_BuffersView.Initialized)
            {
                if (m_BuffersView.Create() == false)
                {
                    DestroyObjects();
                    return false;
                }

                m_BuffersView.Clear(); // Clear all buffers
            }

            // Create processing object
            if (m_Pro != null && !m_Pro.Initialized)
            {
                if (m_Pro.Create() == false)
                {
                    DestroyObjects();
                    return false;
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

            if (m_Xfer != null && m_Xfer.Pairs[0] != null)
            {
                // Can't use SapXferPair.CycleMode.NextEmpty and SapXferPair.CycleMode.NextWithTrash in conjunction with ExecuteNext(), use SapXferPair.CycleMode.WithTrash instead.
                m_Xfer.Pairs[0].Cycle = SapXferPair.CycleMode.WithTrash;
                if (m_Xfer.Pairs[0].Cycle != SapXferPair.CycleMode.WithTrash)
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

        private void DestroyObjects()
        {
            if (m_Xfer != null && m_Xfer.Initialized)
                m_Xfer.Destroy();
            if (m_JpegProcessing != null && m_JpegProcessing.Initialized())
                m_JpegProcessing.Destroy();
            if (m_View != null && m_View.Initialized)
                m_View.Destroy();
            if (m_Buffers != null && m_Buffers.Initialized)
                m_Buffers.Destroy();
            if (m_BuffersView != null && m_BuffersView.Initialized)
                m_BuffersView.Destroy();
            if (m_AcqDevice != null && m_AcqDevice.Initialized)
                m_AcqDevice.Destroy();
        }

        private void DisposeObjects()
        {
            if (m_View != null)
            { m_View.Dispose(); m_View = null; m_ImageBox.View = null; }
            if (m_Pro != null)
            { m_Pro.Dispose(); m_Pro = null; }
            if (m_Xfer != null)
            { m_Xfer.Dispose(); m_Xfer = null; }
            if (m_JpegProcessing != null)
            { m_JpegProcessing.Dispose(); m_JpegProcessing = null; }
            if (m_BuffersView != null)
            { m_BuffersView.Dispose(); m_BuffersView = null; }
            if (m_Buffers != null)
            { m_Buffers.Dispose(); m_Buffers = null; }
            if (m_AcqDevice != null)
            { m_AcqDevice.Dispose(); m_AcqDevice = null; }
        }

        //
        // This function is called each time a buffer has been processed by the processing object
        //
        static void ProCallback(object sender, SapProcessingDoneEventArgs pInfo)
        {
            // While in ProCallback, you cannot access m_pBuffers information since it has been set as empty
            // and may already have been updated by a newer frame. If you need this info, put the code in
            // SapMyProcessing.Run() function.

            CameraCompressionDemoDlg dlg = pInfo.Context as CameraCompressionDemoDlg;

            dlg.m_CompressedImageAvailable = dlg.m_JpegProcessing.Initialized() ? dlg.m_JpegProcessing.IsLastImageJpegDecodable() : false;

            // Display last processed image
            dlg.m_View.Show(dlg.m_BuffersView.Index);

            // Refresh controls
            dlg.Invoke(new RefreshControl(dlg.RefreshTextBoxes));
        }

        private void RefreshTextBoxes()
        {
            textBox_JpegDecodeTime.Text = string.Format("{0:0.00}", m_Pro.GetProTime());

            textBox_JpegDecodeTimeAvg.Text = string.Format("{0:0.00}", m_Pro.GetProTimeAvg());

            int incompleteFrameCount = this.m_Pro.GetProIncompleteFrameCount();
            if (incompleteFrameCount > 0)
                this.StatusLabelInfoIncomplete.Text = String.Format("Incomplete buffer frames count: {0}", incompleteFrameCount);

            int numFramesInCamera;
            m_AcqDevice.GetFeatureValue("transferQueueCurrentBlockCount", out numFramesInCamera);
            textBox_NumFramesInCamera.Text = Convert.ToString(numFramesInCamera);

            int numImagesLostInCamera;
            m_AcqDevice.GetFeatureValue("eventStatisticCount", out numImagesLostInCamera);
            textBox_NumImagesLostInCamera.Text = Convert.ToString(numImagesLostInCamera);

            float minSpaceUsedKB = (float)m_Pro.GetProMinSpaceUsed() / 1024;
            float minSpaceUsedMB = minSpaceUsedKB / 1024;
            if ((int)minSpaceUsedMB > 0)
                textBox_MinSpaceUsed.Text = string.Format("{0:0.0}MB", minSpaceUsedMB);
            else
                textBox_MinSpaceUsed.Text = string.Format("{0}KB", (int)minSpaceUsedKB);

            float maxSpaceUsedKB = (float)m_Pro.GetProMaxSpaceUsed() / 1024;
            float maxSpaceUsedMB = maxSpaceUsedKB / 1024;
            if ((int)maxSpaceUsedMB > 0)
                textBox_MaxSpaceUsed.Text = string.Format("{0:0.0}MB", maxSpaceUsedMB);
            else
                textBox_MaxSpaceUsed.Text = string.Format("{0}KB", (int)maxSpaceUsedKB);

            // Display frame's JPEG encoded size
            float curSpaceUsedKB = (float)m_Pro.GetProCurSpaceUsed() / 1024;
            float curSpaceUsedMB = curSpaceUsedKB / 1024;
            if ((int)curSpaceUsedMB > 0)
                textBox_CurrentSpaceUsed.Text = string.Format("{0:0.0}MB", curSpaceUsedMB);
            else
                textBox_CurrentSpaceUsed.Text = string.Format("{0}KB", (int)curSpaceUsedKB);

            // Display data bandwidth
            if (m_Pro.GetProBandwidth() == 0)
            {
                textBox_AverageBandwidth.Text = "";
                textBox_CurrentFramesPerSecond.Text = "";
                textBox_PossibleFramesPerSecond.Text = "";
            }
            else
            {
                float bandwidthAvgKB = m_Pro.GetProBandwidthAvg() / 1024;
                float bandwidthAvgMB = bandwidthAvgKB / 1024;
                if ((int)bandwidthAvgMB > 0)
                    textBox_AverageBandwidth.Text = string.Format("{0:0.0}MB/s", bandwidthAvgMB);
                else
                    textBox_AverageBandwidth.Text = string.Format("{0}KB/s", (int)bandwidthAvgKB);

                textBox_CurrentFramesPerSecond.Text = string.Format("{0}", m_Pro.GetProFrameRate());

                textBox_PossibleFramesPerSecond.Text = string.Format("{0:0.0}", 1000 / m_Pro.GetProTimeAvg());
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

        private void CameraCompressionDemoDlg_FormClosed(object sender, FormClosedEventArgs e)
        {
            DestroyObjects();
            DisposeObjects();
        }

        //*****************************************************************************************
        //
        //             Acquisition Control
        //
        //*****************************************************************************************

        private void UpdateCompressionQuality()
        {
            if (checkBox_EnableJpeg.Checked == true)
            {
                uint imageCompressionQuality = (uint)numericUpDown_JpegQuality.Value;//m_EditImageCompressionQualityValue;
                uint oldImageCompressionQuality = m_JpegProcessing.GetImageCompressionQuality();
                if (imageCompressionQuality != oldImageCompressionQuality)
                {
                    m_JpegProcessing.SetImageCompressionQuality(imageCompressionQuality);

                    // Update the dialog.
                    numericUpDown_JpegQuality.Value = m_JpegProcessing.GetImageCompressionQuality();
                }
            }
        }

        private void button_Snap_Click(object sender, EventArgs e)
        {
            UpdateCompressionQuality();

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
            UpdateCompressionQuality();

            this.StatusLabelInfo.Text = "";
            this.StatusLabelInfoTrash.Text = "";

            if (m_Xfer.Grab())
            {
                m_Pro.InitProTimeStatistics();
                m_Pro.InitProImageStatistics();
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
        //             General Options
        //
        //*****************************************************************************************

        private void button_Buffer_Click(object sender, EventArgs e)
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
                if (!CreateObjects(false, SapFormat.Unknown))
                {
                    // Restore backup
                    m_Buffers.Count = backupCount;
                    CreateObjects(false, SapFormat.Unknown);
                    MessageBox.Show("Failed to update buffer frames count: restored old count.");
                }

                m_Pro.Init(); // Set SapProcessing index to current buffer index (required if buffer info changed)
            }
            else
                MessageBox.Show("No Modification in Buffer");

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
        //             Acquisition Options
        //
        //*****************************************************************************************

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

                // Recreate objects
                if (!CreateObjects(false, SapFormat.Unknown))
                {
                    // Restore backup
                    m_AcqDevice.Location = backupLocation;
                    m_AcqDevice.ConfigFileName = backupConfigFileName;
                    CreateObjects(false, SapFormat.Unknown);
                    MessageBox.Show("Failed to load config: restored old config.");
                }
            }
            else
                MessageBox.Show("No Modification in Acquisition");

            m_ImageBox.Refresh();
        }

        //*****************************************************************************************
        //
        //             General Function
        //
        //*****************************************************************************************

        // Updates the menu items enabling/disabling the proper items depending on the stateof the application
        void UpdateControls()
        {
            bool bAcqNoGrab = (m_Xfer != null) && (m_Xfer.Grabbing == false);
            bool bAcqGrab = (m_Xfer != null) && (m_Xfer.Grabbing == true);
            bool bNoGrab = (m_Xfer == null) || (m_Xfer.Grabbing == false);

            // Acquisition Control
            button_Grab.Enabled = bAcqNoGrab;
            button_Snap.Enabled = bAcqNoGrab;
            checkBox_EnableJpeg.Enabled = bAcqNoGrab;
            numericUpDown_JpegQuality.Enabled = bAcqNoGrab;
            button_Freeze.Enabled = bAcqGrab;

            // File Options
            button_New.Enabled = bNoGrab;
            button_Load.Enabled = bNoGrab;
            button_Save.Enabled = bNoGrab;

            button_Load_Config.Enabled = bAcqNoGrab;
            button_Buffer.Enabled = bNoGrab;
        }

        private void UpdateViewBuffersViewFormat(SapFormat viewFormat)
        {
            m_View.Destroy();
            m_BuffersView.Destroy();

            m_BuffersView.Format = viewFormat;

            m_BuffersView.Create();
            m_View.Create();

            m_BuffersView.Clear();
        }

        private void SystemEvents_SessionEnded(object sender, SessionEndedEventArgs e)
        {
            // The FormClosed event is not invoked when logging off or shutting down,
            // so we need to clean up here too.
            DestroyObjects();
            DisposeObjects();
        }

        private void button_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //*****************************************************************************************
        //
        //             File Control
        //
        //*****************************************************************************************

        private void button_New_Click(object sender, EventArgs e)
        {
            m_CompressedImageAvailable = false;
            m_BuffersView.Clear();
            m_ImageBox.Refresh();
        }

        private void button_Load_Click(object sender, EventArgs e)
        {
            LoadSaveDlg newDialogLoad = new LoadSaveDlg(m_BuffersView, true, false);
            // Show the dialog and process the result
            newDialogLoad.ShowDialog();
            newDialogLoad.Dispose();
            m_ImageBox.Refresh();
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            if (m_CompressedImageAvailable)
            {
                LoadSaveHwJpegDlg newDialogSave = new LoadSaveHwJpegDlg(m_BuffersView, false, false);

                // Show the dialog and process the result
                newDialogSave.ShowDialog();
                SapBuffer.FileFormat fileFormat = newDialogSave.GetSapBufferFileFormat();

                if (fileFormat == SapBuffer.FileFormat.JPEG)
                {
                    // If "Jpeg" was selected, nothing will have been saved
                    // since we must save as a raw with the .jpg extension

                    string pathName = newDialogSave.PathName;

                    int dataPrmSize = m_Buffers.get_SpaceUsed(m_Buffers.Index);
                    {
                        // Allocate temp buffer for saving
                        SapBuffer saveJpegBuffers = new SapBuffer(1, dataPrmSize, 1, SapFormat.Mono8, SapBuffer.MemoryType.Default);
                        saveJpegBuffers.Count = 1;
                        saveJpegBuffers.Width = dataPrmSize;
                        saveJpegBuffers.Height = 1;
                        saveJpegBuffers.Create();

                        IntPtr saveJpegBuffersAddress;
                        saveJpegBuffers.GetAddress(out saveJpegBuffersAddress);

                        IntPtr bufferAddress;
                        m_Buffers.GetAddress(out bufferAddress);

                        byte[] saveData = new byte[dataPrmSize];
                        Marshal.Copy(bufferAddress, saveData, 0, dataPrmSize);
                        Marshal.Copy(saveData, 0, saveJpegBuffersAddress, dataPrmSize);


                        saveJpegBuffers.Save(pathName, "-format raw");

                        saveJpegBuffers.Destroy();
                        saveJpegBuffers.Dispose();
                    }
                }

                newDialogSave.Dispose();
            }
            else
            {
                LoadSaveDlg newDialogSave = new LoadSaveDlg(m_BuffersView, false, false);
                newDialogSave.ShowDialog();
                newDialogSave.Dispose();
            }

            m_ImageBox.Refresh();
        }

        //*****************************************************************************************
        //
        //             System menu
        //
        //*****************************************************************************************

        private void CameraCompressionDemoDlg_Load(object sender, EventArgs e)
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
            {
                if (msg.WParam.ToInt32() == m_AboutID)
                {
                    AboutBox dlg = new AboutBox();
                    dlg.ShowDialog();
                    Invalidate();
                    Update();
                }
            }
            // Call base class function
            base.WndProc(ref msg);
        }

        private void checkBox_EnableJpeg_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            m_JpegEnabled = !m_JpegProcessing.IsImageCompressionModeJpeg();
            checkBox_EnableJpeg.Checked = m_JpegEnabled;

            // To enable/disable JPEG, the transfer object must first be disconnected from the hardware
            if (m_Xfer != null && m_Xfer.Initialized)
                m_Xfer.Disconnect();

            // Initialize acquisition and processing
            m_Pro.SetDecompressionEnabled(m_JpegEnabled);

            // Enable/disable the Jpeg procesing. (Xfer must be disconnected).
            bool pixelFormatChanged = false;
            m_JpegProcessing.EnableJpegProcessing(m_JpegEnabled, out pixelFormatChanged);
            if (pixelFormatChanged == true)
            {
                // Destroy objects
                DestroyObjects();

                // Update object
                m_AcqDevice.ConfigFileName = null;

                // Recreate objects
                CreateObjects(false, SapFormat.Unknown);

                m_CompressedImageAvailable = false;
            }

            Update_numericUpDown_JpegQuality();

            SapFormat newViewFormat = SapFormat.Unknown;
            SapFormat acqFormat = m_Buffers.Format;
            if (m_JpegEnabled)
            {
                // If the acq buffer is YUY2, color JPEG decompression can only be done to RGB8888, so
                // we change it to this (one less conversion to make)
                if (acqFormat == SapFormat.YUY2)
                    newViewFormat = SapFormat.RGB8888;
            }
            else
            {
                // If the acq buffer format is different from the view buffer, make it the same
                if (m_BuffersView.Format != acqFormat)
                    newViewFormat = acqFormat;
            }

            if (newViewFormat != SapFormat.Unknown)
                UpdateViewBuffersViewFormat(newViewFormat);

            if (m_Xfer != null)
                m_Xfer.Connect();

            m_Pro.InitProTimeStatistics();

            Cursor = Cursors.Default;
        }
    }
}
