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

namespace DALSA.SaperaLT.Demos.NET.CSharp.GigEFlatFieldDemo
{
	public partial class GigEFlatFieldDemoDlg : Form
	{

		private const String g_ValueChangeEventName = "Feature Value Changed";
		private const String DEFAULT_FFC_FILENAME = "FFC.tif";
		private const String STANDARD_FILTER = "TIFF Files (*.tif)|*.tif||";

		private String g_CalibEnableFeatureName = "";
		private SapAcqDevice m_AcqDevice;
		private SapBuffer m_Buffers;
		private SapAcqDeviceToBuf m_Xfer;
		private SapView m_View;
		private SapFlatField m_FlatField;
		private SapMyProcessing m_Pro;
		private SapLocation m_ServerLocation;
		private String m_ConfigFileName, m_FlatField_ExecuteTime;
		private bool m_CalibEnableAvailable, m_CalibEnable, m_ValueChangeEventAvailable, m_isGenie;
		private bool m_bUseROI;

		//System menu
		private SystemMenu m_SystemMenu;
		//index for "about this.." item im system menu
		private const int m_AboutID = 0x100;

		// Delegate to display number of frame acquired 
		// Delegate is needed because .NEt framework does not support  cross thread control modification
		private delegate void DisplayFrameAcquired(int number, bool trash);


		static void xfer_XferNotify(object sender, SapXferNotifyEventArgs argsNotify)
		{
			GigEFlatFieldDemoDlg GigeDlg = argsNotify.Context as GigEFlatFieldDemoDlg;

			// Set the ROI if needed
			if (GigeDlg.m_bUseROI)
			{
				Rectangle roi = GigeDlg.m_ImageBox.Tracker;
				GigeDlg.m_FlatField.SetRegionOfInterest(roi.Left, roi.Top, roi.Width, roi.Height);
			}

			// If grabbing in trash buffer, do not display the image, update the
			// appropriate number of frames on the status bar instead
			if (argsNotify.Trash)
				GigeDlg.Invoke(new DisplayFrameAcquired(GigeDlg.ShowFrameNumber), argsNotify.EventCount, true);
			// Refresh view
			else
			{
				GigeDlg.Invoke(new DisplayFrameAcquired(GigeDlg.ShowFrameNumber), argsNotify.EventCount, false);
				GigeDlg.m_Pro.ExecuteNext();
			}
		}

		//
		// This function is called each time a buffer has been processed by the processing object
		//
		static void ProCallback(object sender, SapProcessingDoneEventArgs pInfo)
		{
			GigEFlatFieldDemoDlg GigeDlg = pInfo.Context as GigEFlatFieldDemoDlg;

			// Check if flat field correction was enabled and if it's been done by software
			if (GigeDlg.m_FlatField.Enabled && GigeDlg.m_FlatField.SoftwareCorrection)
			{
				// Show current buffer index and execution time in milliseconds
				GigeDlg.m_FlatField_ExecuteTime = String.Format("Flat field correction= {0:0.00} ms", GigeDlg.m_Pro.Time);
			}
			else
				GigeDlg.m_FlatField_ExecuteTime = "";

			// Refresh view
			GigeDlg.m_View.Show();
		}

		//
		// This function is called each time a buffer has been shown by the view object
		//
		static void ViewCallback(object sender, SapDisplayDoneEventArgs pInfo)
		{
			GigEFlatFieldDemoDlg GigeDlg = pInfo.Context as GigEFlatFieldDemoDlg;

			if (GigeDlg.m_bUseROI && !GigeDlg.m_ImageBox.IsTrackerEmpty)
				GigeDlg.m_ImageBox.DisplayTracker();
		}

		//
		// This function is called each time an acquisition device specific event happens,
		// for example, when the value of a feature changes
		//
		static void AcqDeviceCallback(object sender, SapAcqDeviceNotifyEventArgs pInfo)
		{
			// Uncomment to display events received from the camera
			//SapAcqDevice acq = pInfo.Context as SapAcqDevice;

			//int eventIndex, featureIndex;
			//String [] allEvent;
			//String [] allFeature;

			//eventIndex = pInfo.EventIndex;
			//allEvent = acq.EventNames;
			//String message = "Acquisition device event: ";
			//message += allEvent[eventIndex];
			//message += "\n";

			//featureIndex = pInfo.FeatureIndex;
			//if (allEvent[eventIndex] == g_ValueChangeEventName)
			//{
			//   allFeature = acq.FeatureNames;
			//   message += "Feature name: ";
			//   message += allFeature[featureIndex];
			//   message += "\n";
			//}

			//MessageBox.Show(message);
		}

		public GigEFlatFieldDemoDlg()
		{
			m_AcqDevice = null;
			m_Buffers = null;
			m_Xfer = null;
			m_View = null;
			m_FlatField = null;
			m_Pro = null;

			m_CalibEnableAvailable = false;
			m_CalibEnable = false;
			m_ValueChangeEventAvailable = false;
			m_FlatField_ExecuteTime = "";
			m_isGenie = false;

			AcqConfigDlg acConfigDlg = new AcqConfigDlg(null, "", AcqConfigDlg.ServerCategory.ServerAcqDevice);
			if (acConfigDlg.ShowDialog() == DialogResult.OK)
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
				this.m_ImageBox.Location = new System.Drawing.Point(261, 73);
				this.m_ImageBox.Name = "m_ImageBox";
				this.m_ImageBox.PixelValueDisplay = this.PixelDataValue;
				this.m_ImageBox.Size = new System.Drawing.Size(386, 357);
				this.m_ImageBox.SliderEnable = true;
				this.m_ImageBox.SliderMaximum = 10;
				this.m_ImageBox.SliderMinimum = 0;
				this.m_ImageBox.SliderValue = 0;
				this.m_ImageBox.SliderVisible = false;
				this.m_ImageBox.TabIndex = 37;
				this.m_ImageBox.TrackerEnable = false;
				this.m_ImageBox.View = null;
				this.Controls.Add(this.m_ImageBox);

				checkBox_Use_ROI.Checked = m_bUseROI = false;

				if (!CreateNewObjects(acConfigDlg, false))
					this.Close();
				else if (!m_isGenie && !m_CalibEnableAvailable)
				{
					MessageBox.Show("Hardware based flat-field correction is not supported on this camera.");
					this.Close();
				}
			}
			else
			{
				MessageBox.Show("No cameras found or selected");
				this.Close();
			}


		}

		private void ShowFrameNumber(int number, bool trash)
		{
			String str;
			if (trash)
			{
				if (string.IsNullOrEmpty(m_FlatField_ExecuteTime))
				{
					str = String.Format("Frames acquired in trash buffer: {0}", number);
					this.StatusLabelInfoTrash.Text = str;
				}
			}
			else
			{

				if (!string.IsNullOrEmpty(m_FlatField_ExecuteTime))
					this.StatusLabelInfo.Text = m_FlatField_ExecuteTime;
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
			if (!Restore)
			{
				m_ServerLocation = acConfigDlg.ServerLocation;
				m_ConfigFileName = acConfigDlg.ConfigFile;
			}
			m_AcqDevice = new SapAcqDevice(m_ServerLocation, m_ConfigFileName);
			if (SapBuffer.IsBufferTypeSupported(m_ServerLocation, SapBuffer.MemoryType.ScatterGather))
				m_Buffers = new SapBufferWithTrash(2, m_AcqDevice, SapBuffer.MemoryType.ScatterGather);
			else
				m_Buffers = new SapBufferWithTrash(2, m_AcqDevice, SapBuffer.MemoryType.ScatterGatherPhysical);
			m_Xfer = new SapAcqDeviceToBuf(m_AcqDevice, m_Buffers);

			m_View = new SapView(m_Buffers);
			//event for view  
			m_View.DisplayDone += new SapDisplayDoneHandler(ViewCallback);
			m_View.DisplayDoneContext = this;
			m_View.DisplayDoneEnable = true;

			m_FlatField = new SapFlatField(m_AcqDevice);
			m_Pro = new SapMyProcessing(m_Buffers, m_FlatField, ProCallback, this);
			m_ImageBox.View = m_View;

			m_Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
			m_Xfer.XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
			m_Xfer.XferNotifyContext = this;
			StatusLabelInfo.Text = "Online... Waiting grabbed images";

			if (!CreateObjects(true, true))
			{
				DisposeObjects();
				return false;
			}

			// Resize ImagBox to take into account the size of created sapview
			m_ImageBox.OnSize();
			UpdateControls();
			return true;
		}


		// Create new objects with acquisition information
		public bool CreateNewObjects(BufferDlg dlg)
		{
			m_AcqDevice = new SapAcqDevice(m_ServerLocation, m_ConfigFileName);
			if (dlg.Count > 1)
				m_Buffers = new SapBufferWithTrash(dlg.Count, m_AcqDevice, dlg.Type);
			else
				m_Buffers = new SapBuffer(1, m_AcqDevice, dlg.Type);
			m_Xfer = new SapAcqDeviceToBuf(m_AcqDevice, m_Buffers);

			m_View = new SapView(m_Buffers);
			//event for view 
			m_View.DisplayDone += new SapDisplayDoneHandler(ViewCallback);
			m_View.DisplayDoneContext = this;
			m_View.DisplayDoneEnable = true;

			m_FlatField = new SapFlatField(m_AcqDevice);
			m_Pro = new SapMyProcessing(m_Buffers, m_FlatField, ProCallback, this);
			m_ImageBox.View = m_View;

			m_Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
			m_Xfer.XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
			m_Xfer.XferNotifyContext = this;
			StatusLabelInfo.Text = "Online... Waiting grabbed images";

			if (!CreateObjects(true, true))
			{
				DisposeObjects();
				return false;
			}

			// Resize ImagBox to take into account the size of created sapview
			m_ImageBox.OnSize();
			UpdateControls();
			return true;
		}

		// Call Create Object 
		private bool CreateObjects(bool createAcq, bool createFlatField)
		{
			if (createAcq)
			{
				// Create acquisition object
				if (m_AcqDevice != null && !m_AcqDevice.Initialized)
				{
					if (!m_AcqDevice.Create())
					{
						DestroyObjects(true, true);
						return false;
					}
				}

				//GigE
				m_CalibEnableAvailable = m_AcqDevice.IsFeatureAvailable("flatfieldCorrectionMode");
				if (m_CalibEnableAvailable)
				{
					m_isGenie = false;
					g_CalibEnableFeatureName = "flatfieldCorrectionMode";
				}
				else
				{
					//Genie
					m_CalibEnableAvailable = m_AcqDevice.IsFeatureAvailable("FlatFieldCalibrationEnable");
					if (m_CalibEnableAvailable)
					{
						m_isGenie = true;
						g_CalibEnableFeatureName = "FlatFieldCalibrationEnable";
					}
				}


				m_ValueChangeEventAvailable = m_AcqDevice.IsEventAvailable(g_ValueChangeEventName);

				if (m_ValueChangeEventAvailable)
				{
					m_AcqDevice.EnableEvent(g_ValueChangeEventName);
					m_AcqDevice.AcqDeviceNotify += new SapAcqDeviceNotifyHandler(AcqDeviceCallback);
					m_AcqDevice.AcqDeviceNotifyContext = m_AcqDevice;

				}
			}

			// Create buffer object
			if (m_Buffers != null && !m_Buffers.Initialized)
			{
				if (m_Buffers.Create() == false)
				{
					DestroyObjects(true, true);
					return false;
				}
				m_Buffers.Clear();
			}
			// Create view object
			if (m_View != null && !m_View.Initialized)
			{
				if (m_View.Create() == false)
				{
					DestroyObjects(true, true);
					return false;
				}
			}
			// Create Xfer object
			if (m_Xfer != null && !m_Xfer.Initialized)
			{
				if (m_Xfer.Create() == false)
				{
					DestroyObjects(true, true);
					return false;
				}
			}

			// Create flat field object
			if (createFlatField)
			{
				if (m_FlatField != null && !m_FlatField.Initialized)
				{
					if (!m_FlatField.Create())
					{
						DestroyObjects(true, true);
						return false;
					}
				}
			}

			if (m_Pro != null && !m_Pro.Initialized)
			{
				if (!m_Pro.Create())
				{
					DestroyObjects(true, true);
					return false;
				}
				m_Pro.AutoEmpty = true;
				if (m_Xfer != null)
					m_Xfer.AutoEmpty = false;
			}

			return true;
		}

		private void DestroyObjects(bool destroyAcq, bool destroyFlatField)
		{
			// Destroy processing object
			if (m_Pro != null && m_Pro.Initialized)
				m_Pro.Destroy();

			// Destroy flat field object
			if (destroyFlatField)
			{
				if (m_FlatField != null && m_FlatField.Initialized)
					m_FlatField.Destroy();
				// Disable flat field correction
				checkBox_FlatField.Checked = false;
			}

			if (m_Xfer != null && m_Xfer.Initialized)
				m_Xfer.Destroy();
			if (m_View != null && m_View.Initialized)
				m_View.Destroy();
			if (m_Buffers != null && m_Buffers.Initialized)
				m_Buffers.Destroy();

			if (destroyAcq)
			{
				if (m_AcqDevice != null && m_AcqDevice.Initialized)
				{
					// Unregister acquisition callback
					if (m_ValueChangeEventAvailable)
					{
						m_AcqDevice.DisableEvent(g_ValueChangeEventName);
						m_AcqDevice.AcqDeviceNotify -= new SapAcqDeviceNotifyHandler(AcqDeviceCallback);
						m_AcqDevice.AcqDeviceNotifyContext = null;
					}

					// Disable flat-field calibration mode on the camera
					if (m_CalibEnableAvailable && m_CalibEnable)
					{
						if (m_isGenie)
							m_AcqDevice.SetFeatureValue(g_CalibEnableFeatureName, false);
						else
							m_AcqDevice.SetFeatureValue(g_CalibEnableFeatureName, "Off");
						m_CalibEnable = false;
					}

					// Destroy acquisition object
					m_AcqDevice.Destroy();
				}
			}
		}

		private void DisposeObjects()
		{
			if (m_Pro != null)
			{ m_Pro.Dispose(); m_Pro = null; }
			if (m_FlatField != null)
			{ m_FlatField.Dispose(); m_FlatField = null; }
			if (m_Xfer != null)
			{ m_Xfer.Dispose(); m_Xfer = null; }
			if (m_View != null)
			{ m_View.Dispose(); m_View = null; m_ImageBox.View = null; }
			if (m_Buffers != null)
			{ m_Buffers.Dispose(); m_Buffers = null; }
			if (m_AcqDevice != null)
			{ m_AcqDevice.Dispose(); m_AcqDevice = null; }
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

		private void GigEFlatFieldDemoDlg_FormClosed(object sender, FormClosedEventArgs e)
		{
			DestroyObjects(true, true);
			DisposeObjects();
		}

		//**************************************************************************************
		// Updates the menu items enabling/disabling the proper items depending on the state
		//  of the application
		//**************************************************************************************
		void UpdateControls()
		{
			bool acqNoGrab = m_Xfer != null && m_Xfer.Initialized && !m_Xfer.Grabbing;
			bool acqGrab = m_Xfer != null && m_Xfer.Initialized && m_Xfer.Grabbing;
			bool noGrab = m_Xfer == null || !m_Xfer.Grabbing;

			bool isFlatFieldEnabled = m_FlatField != null && m_FlatField.Initialized && m_FlatField.Enabled;
			bool isFlatFieldAvailable = m_AcqDevice != null && m_AcqDevice.Initialized && m_AcqDevice.FlatFieldAvailable;
			bool isFlatFieldSoftware = m_FlatField != null && m_FlatField.Initialized && m_FlatField.SoftwareCorrection;
			bool isFlatFieldPixelReplacement = m_FlatField != null && m_FlatField.Initialized && m_FlatField.PixelReplacement;

			// Acquisition Control
			button_Grab.Enabled = acqNoGrab;
			button_Freeze.Enabled = acqGrab;
			button_Snap.Enabled = acqNoGrab;

			// Acquisition Options
			button_LoadConfig.Enabled = acqNoGrab;

			// Flat field correction
			checkBox_FlatField.Enabled = noGrab;
			button_Calibrate.Enabled = noGrab && !isFlatFieldEnabled;
			button_Load_FF.Enabled = noGrab && !isFlatFieldEnabled;
			button_Save_FF.Enabled = noGrab;

			checkBox_FlatField.Checked = isFlatFieldEnabled;
			checkBox_Hardware_Correction.Checked = isFlatFieldAvailable && !isFlatFieldSoftware;
			checkBox_Hardware_Correction.Enabled = isFlatFieldAvailable && isFlatFieldEnabled && noGrab;

			checkBox_Pixels_Replacement.Checked = isFlatFieldPixelReplacement;
			checkBox_Pixels_Replacement.Enabled = isFlatFieldSoftware && isFlatFieldEnabled && noGrab;

			// File Options
			button_New.Enabled = noGrab;
			button_Load.Enabled = noGrab;
			button_Save.Enabled = noGrab;

			// General Options
			button_Buffers.Enabled = noGrab;
			button_View.Enabled = noGrab;

		}

		// Set the flat-field calibration mode of the camera
		private bool SetCalibEnable(bool newCalibEnable)
		{
            SapFeature calibFeature = new SapFeature(m_AcqDevice.Location);
            calibFeature.Create();
            m_AcqDevice.GetFeatureInfo(g_CalibEnableFeatureName, calibFeature);
            int enumCount = calibFeature.EnumCount;
            string[] enumText = calibFeature.EnumText;
            bool calibModeValueAvailable = false;

            for (int enumIndex = 0; enumIndex < enumCount; enumIndex++)
            {
                bool enumEnabled = calibFeature.get_EnumEnabled(enumIndex);
                if (enumText[enumIndex] == "Calibration" && enumEnabled)
                    calibModeValueAvailable = true;
            }

            calibFeature.Destroy();
            calibFeature.Dispose();

			if ((newCalibEnable != m_CalibEnable) && m_CalibEnableAvailable)
			{
   			bool success = false;
				DestroyObjects(false, false);
				if (m_isGenie)
					success = m_AcqDevice.SetFeatureValue(g_CalibEnableFeatureName, newCalibEnable);
				else
					if (newCalibEnable && calibModeValueAvailable)
						success = m_AcqDevice.SetFeatureValue(g_CalibEnableFeatureName, "Calibration");
					else
						success = m_AcqDevice.SetFeatureValue(g_CalibEnableFeatureName, "Off");

				if (success)
				{
					if (!CreateObjects(false, false))
					{
						if (m_isGenie)
							success = m_AcqDevice.SetFeatureValue(g_CalibEnableFeatureName, m_CalibEnable);
						else
							if (m_CalibEnable && calibModeValueAvailable)
								success = m_AcqDevice.SetFeatureValue(g_CalibEnableFeatureName, "Calibration");
							else
								success = m_AcqDevice.SetFeatureValue(g_CalibEnableFeatureName, "Off");
						CreateObjects(false, false);
						return false;
					}
					m_CalibEnable = newCalibEnable;
					if (m_ImageBox != null)
						m_ImageBox.OnSize();
				}
			}
			return true;
		}

		private bool CheckPixelFormat(String mode)
		{
			SapFormat format = m_Buffers.Format;

			if (format != SapFormat.Mono8 && format != SapFormat.Mono16)
			{
				String message;
				if (mode == "warning")
					message = "Pixel Format will be automatically changed to Raw Bayer for flat field calibration.";
				else
				{
					message = String.Format("Pixel format must be 8-bit or 16-bit monochrome for {0}\n", mode);
					message += "(for Genie color, this corresponds to Bayer Raw8 in CamExpert)";
				}
				MessageBox.Show(message);
				return false;
			}

			return true;
		}

		private void SystemEvents_SessionEnded(object sender, SessionEndedEventArgs e)
		{
			// The FormClosed event is not invoked when logging off or shutting down,
			// so we need to clean up here too.
			DestroyObjects(true, true);
			DisposeObjects();
		}


		//*****************************************************************************************
		//
		//               Acquisition Control
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
			this.StatusLabelInfo.Text = "";
			this.StatusLabelInfoTrash.Text = "";
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
		//               Flat field Options
		//
		//*****************************************************************************************

		private void button_Calibrate_Click(object sender, EventArgs e)
		{
         if (!m_isGenie && m_AcqDevice.FlatFieldAvailable)
         {
            ulong width, height, sensorWidth, sensorHeight;
            bool isInvalidSize;
            if (!m_AcqDevice.GetFeatureValue("Width", out width))
               return;
            if (!m_AcqDevice.GetFeatureValue("Height", out height))
               return;
            if (!m_AcqDevice.GetFeatureValue("SensorWidth", out sensorWidth))
               return;
            if (!m_AcqDevice.GetFeatureValue("SensorHeight", out sensorHeight))
               return;
            if (sensorHeight == 1)
               isInvalidSize = (width != sensorWidth);
            else
               isInvalidSize = (width != sensorWidth || height != sensorHeight);
            if (isInvalidSize)
            {
               MessageBox.Show("Flat-field calibration can only be performed on the full sensor size.\nMake sure that image cropping and binning are disabled.");
               return;
            }
         }

			CheckPixelFormat("warning");

			// Flat-field calibration mode must be enabled on the camera
			if (SetCalibEnable(true))
			{
				FlatFieldDlg dlg = new FlatFieldDlg(m_FlatField, m_Xfer, m_Buffers);
				dlg.ShowDialog();
				SetCalibEnable(false);
				UpdateControls();
			}
		}

		private void checkBox_FlatField_CheckedChanged(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;

			if (m_FlatField != null && m_FlatField.Initialized)
			{
				// To enable/disable flat field correction, the transfer object must first be disconnected from the hardware
				if (m_Xfer != null && m_Xfer.Initialized)
					m_Xfer.Destroy();

				bool success = true;

				// Check for invalid pixel format when using software correction
				if (checkBox_FlatField.Checked && !checkBox_Hardware_Correction.Checked)
					success = CheckPixelFormat("software correction");

				if (success)
					m_FlatField.Enable(checkBox_FlatField.Checked, checkBox_Hardware_Correction.Checked);


				if (m_Xfer != null && !m_Xfer.Initialized)
				{
					// Recreate the transfer object to reconnect it to the hardware
					m_Xfer.Create();
					m_Xfer.Init(true);
					m_Pro.Init();
				}
				if (checkBox_FlatField.Checked)
					StatusLabelInfoTrash.Text = "";
			}
			UpdateControls();
			Cursor = Cursors.Default;
		}

		private void checkBox_Use_ROI_CheckedChanged(object sender, EventArgs e)
		{
			m_bUseROI = checkBox_Use_ROI.Checked;
			m_ImageBox.TrackerEnable = m_bUseROI;

			if (m_bUseROI)
			{
				Rectangle roi = m_ImageBox.Tracker;
				m_FlatField.SetRegionOfInterest(roi.Left, roi.Top, roi.Width, roi.Height);
			}
			else
			{
				m_FlatField.ResetRegionOfInterest();
			}
		}

		private void button_Load_FF_Click(object sender, EventArgs e)
		{

			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Open Flat Field Correction";
			dlg.FileName = DEFAULT_FFC_FILENAME;
			dlg.Filter = STANDARD_FILTER;

			if (dlg.ShowDialog() == DialogResult.OK)
			{
				String PathName = dlg.FileName;
				// Load flat field correction file
				if (!m_FlatField.Load(PathName))
					return;
			}

			UpdateControls();
		}

		private void button_Save_FF_Click(object sender, EventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Title = "Save Flat Field Correction As";
			dlg.FileName = DEFAULT_FFC_FILENAME;
			dlg.Filter = STANDARD_FILTER;


			if (dlg.ShowDialog() == DialogResult.OK)
			{
				String PathName = dlg.FileName;
				// Save flat field correction file
				m_FlatField.Save(PathName);
			}

			UpdateControls();
		}

		private void checkBox_Hardware_Correction_CheckedChanged(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;

			if (m_FlatField != null && m_FlatField.Initialized)
			{
				// To enable/disable flat field correction, the transfer object must first be disconnected from the hardware
				if (m_Xfer != null && m_Xfer.Initialized)
					m_Xfer.Destroy();

				bool success = true;

				// Check for invalid pixel format when using software correction
				if (checkBox_FlatField.Checked && !checkBox_Hardware_Correction.Checked)
					success = CheckPixelFormat("software correction");

				// Enable/disable flat field correction
				if (success)
					m_FlatField.Enable(checkBox_FlatField.Checked, checkBox_Hardware_Correction.Checked);

				if (m_Xfer != null && !m_Xfer.Initialized)
				{
					// Recreate the transfer object to reconnect it to the hardware
					m_Xfer.Create();
					m_Xfer.Init(true);
					m_Pro.Init();
				}
			}
			UpdateControls();
			Cursor = Cursors.Default;
		}

		private void checkBox_Pixels_Replacement_CheckedChanged(object sender, EventArgs e)
		{
			// Enable/disable pixel replacement flat field correction
			m_FlatField.PixelReplacement = checkBox_Pixels_Replacement.Checked;
			UpdateControls();
		}

		private void button_Exit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void button_Buffers_Click(object sender, EventArgs e)
		{
			// Set new buffer parameters
			BufferDlg dlg = new BufferDlg(m_Buffers, m_View.Display, true);
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				DestroyObjects(true, true);
				DisposeObjects();

				//Update objects with new buffer
				if (!CreateNewObjects(dlg))
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
			m_ImageBox.Refresh();
		}

		private void button_View_Click(object sender, EventArgs e)
		{
			ViewDlg viewDialog = new ViewDlg(m_View, m_ImageBox.ViewRectangle);

			if (viewDialog.ShowDialog() == DialogResult.OK)
				m_ImageBox.OnSize();
			m_ImageBox.Refresh();

		}

		private void button_LoadConfig_Click(object sender, EventArgs e)
		{
			// Set new acquisition parameters
			AcqConfigDlg acConfigDlg = new AcqConfigDlg(null, "", AcqConfigDlg.ServerCategory.ServerAcqDevice);
			if (acConfigDlg.ShowDialog() == DialogResult.OK)
			{
				DestroyObjects(true, true);
				DisposeObjects();

				// Update objects with new acquisition
				if (!CreateNewObjects(acConfigDlg, false))
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
			else
			{
				MessageBox.Show("No Modification in Acquisition");
			}
			m_ImageBox.Refresh();

		}

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
		//					System menu
		//
		//*****************************************************************************************

		private void GigEFlatFieldDemoDlg_Load(object sender, EventArgs e)
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
	}
}