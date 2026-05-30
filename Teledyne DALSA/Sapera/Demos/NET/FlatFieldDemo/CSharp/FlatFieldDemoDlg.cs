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

namespace DALSA.SaperaLT.Demos.NET.CSharp.FlatFieldDemo
{
	public partial class FlatFieldDemoDlg : Form
	{
		private const String DEFAULT_FFC_FILENAME = "FFC.tif";
		private const String STANDARD_FILTER = "TIFF Files (*.tif)|*.tif||";

		private SapAcquisition m_Acquisition;
		private SapBuffer m_Buffers;
		private SapTransfer m_Xfer;
		private SapView m_View;
		private SapFlatField m_FlatField;
		private SapMyProcessing m_Pro;
		private SapLocation m_ServerLocation;
		private String m_FlatField_ExecuteTime;
		private String m_ConfigFileName;

		private bool m_IsSignalDetected;
		private bool m_online;
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
			FlatFieldDemoDlg FlatDlg = argsNotify.Context as FlatFieldDemoDlg;

			// Set the ROI if needed
			if (FlatDlg.m_bUseROI)
			{
				Rectangle roi = FlatDlg.m_ImageBox.Tracker;
				FlatDlg.m_FlatField.SetRegionOfInterest(roi.Left, roi.Top, roi.Width, roi.Height);
			}

			// If grabbing in trash buffer, do not display the image, update the
			// appropriate number of frames on the status bar instead
			if (argsNotify.Trash)
				FlatDlg.Invoke(new DisplayFrameAcquired(FlatDlg.ShowFrameNumber), argsNotify.EventCount, true);
			// Refresh view
			else
			{
				FlatDlg.Invoke(new DisplayFrameAcquired(FlatDlg.ShowFrameNumber), argsNotify.EventCount, false);
				FlatDlg.m_Pro.ExecuteNext();
			}
		}

		//
		// This function is called each time a buffer has been processed by the processing object
		//
		static void ProCallback(object sender, SapProcessingDoneEventArgs pInfo)
		{
			FlatFieldDemoDlg FlatDlg = pInfo.Context as FlatFieldDemoDlg;

			// Check if flat field correction was enabled and if it's been done by software
			if (FlatDlg.m_FlatField.Enabled && FlatDlg.m_FlatField.SoftwareCorrection)
			{
				// Show current buffer index and execution time in milliseconds
				FlatDlg.m_FlatField_ExecuteTime = String.Format("Flat field correction= {0:0.00} ms", FlatDlg.m_Pro.Time);
			}
			else
				FlatDlg.m_FlatField_ExecuteTime = "";

			// Refresh view
			FlatDlg.m_View.Show();
		}

		//
		// This function is called each time a buffer has been shown by the view object
		//
		static void ViewCallback(object sender, SapDisplayDoneEventArgs pInfo)
		{
			FlatFieldDemoDlg FlatDlg = pInfo.Context as FlatFieldDemoDlg;

			if (FlatDlg.m_bUseROI && !FlatDlg.m_ImageBox.IsTrackerEmpty)
				FlatDlg.m_ImageBox.DisplayTracker();
		}

		static void GetSignalStatus(object sender, SapSignalNotifyEventArgs argsSignal)
		{
			FlatFieldDemoDlg FlatDlg = argsSignal.Context as FlatFieldDemoDlg;
			SapAcquisition.AcqSignalStatus signalStatus = argsSignal.SignalStatus;

			FlatDlg.m_IsSignalDetected = (signalStatus != SapAcquisition.AcqSignalStatus.None);
			if (FlatDlg.m_IsSignalDetected == false)
				FlatDlg.StatusLabelInfo.Text = "Online... No camera signal detected";
			else FlatDlg.StatusLabelInfo.Text = "Online... Camera signal detected";
		}

		public FlatFieldDemoDlg()
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
			this.m_ImageBox.Location = new System.Drawing.Point(262, 74);
			this.m_ImageBox.Name = "m_ImageBox";
			this.m_ImageBox.PixelValueDisplay = this.PixelDataValue;
			this.m_ImageBox.Size = new System.Drawing.Size(386, 357);
			this.m_ImageBox.SliderEnable = true;
			this.m_ImageBox.SliderMaximum = 10;
			this.m_ImageBox.SliderMinimum = 0;
			this.m_ImageBox.SliderValue = 0;
			this.m_ImageBox.SliderVisible = false;
			this.m_ImageBox.TabIndex = 30;
			this.m_ImageBox.TrackerEnable = false;
			this.m_ImageBox.View = null;
			this.Controls.Add(this.m_ImageBox);

			AcqConfigDlg acConfigDlg = new AcqConfigDlg(null, "", AcqConfigDlg.ServerCategory.ServerAcq);
			if (acConfigDlg.ShowDialog() == DialogResult.OK)
				m_online = true;
			else
				m_online = false;

			checkBox_Use_ROI.Checked = m_bUseROI = false;

			if (!CreateNewObjects(acConfigDlg, false))
				this.Close();
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
				m_FlatField = new SapFlatField(m_Acquisition);


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
				m_FlatField = new SapFlatField(m_Buffers);
				StatusLabelInfo.Text = "offline... Load images";
			}

			m_View = new SapView(m_Buffers);
			//event for view  
			m_View.DisplayDone += new SapDisplayDoneHandler(ViewCallback);
			m_View.DisplayDoneContext = this;
			m_View.DisplayDoneEnable = true;

			m_Pro = new SapMyProcessing(m_Buffers, m_FlatField, new SapProcessingDoneHandler(ProCallback), this);

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
				m_FlatField = new SapFlatField(m_Acquisition);


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
				m_FlatField = new SapFlatField(m_Buffers);
				StatusLabelInfo.Text = "offline... Load images";
			}

			m_View = new SapView(m_Buffers);
			//event for view 
			m_View.DisplayDone += new SapDisplayDoneHandler(ViewCallback);
			m_View.DisplayDoneContext = this;
			m_View.DisplayDoneEnable = true;

			m_Pro = new SapMyProcessing(m_Buffers, m_FlatField, new SapProcessingDoneHandler(ProCallback), this);

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
				if (!m_Acquisition.Create())
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

			// Create flat field object
			if (m_FlatField != null && !m_FlatField.Initialized)
			{
				if (!m_FlatField.Create())
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
			// Create Xfer object
			if (m_Xfer != null && !m_Xfer.Initialized)
			{
				if (m_Xfer.Create() == false)
				{
					DestroyObjects();
					return false;
				}
			}

			if (m_Pro != null && !m_Pro.Initialized)
			{
				if (!m_Pro.Create())
				{
					DestroyObjects();
					return false;
				}
				m_Pro.AutoEmpty = true;
				if (m_Xfer != null)
					m_Xfer.AutoEmpty = false;
			}
			return true;
		}

		private void DestroyObjects()
		{
			if (m_Xfer != null && m_Xfer.Initialized)
				m_Xfer.Destroy();

			// Destroy processing object
			if (m_Pro != null && m_Pro.Initialized)
				m_Pro.Destroy();

			if (m_View != null && m_View.Initialized)
				m_View.Destroy();

			if (m_FlatField != null && m_FlatField.Initialized)
			{
				m_FlatField.Destroy();
				// Disable flat field correction
				checkBox_FaltField.Checked = false;
				checkBox_Hardware_Correction.Checked = false;
			}

			if (m_Buffers != null && m_Buffers.Initialized)
				m_Buffers.Destroy();

			if (m_Acquisition != null && m_Acquisition.Initialized)
				m_Acquisition.Destroy();
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
			if (m_Acquisition != null)
			{ m_Acquisition.Dispose(); m_Acquisition = null; }
		}


		//**************************************************************************************
		// Updates the menu items enabling/disabling the proper items depending on the state
		//  of the application
		//**************************************************************************************
		private void UpdateControls()
		{
			bool bAcqNoGrab = m_Xfer != null && m_Xfer.Initialized && !m_Xfer.Grabbing;
			bool bAcqGrab = m_Xfer != null && m_Xfer.Initialized && m_Xfer.Grabbing;
			bool bNoGrab = m_Xfer == null || !m_Xfer.Grabbing;

			bool isFlatFieldEnabled = m_FlatField != null && m_FlatField.Initialized && m_FlatField.Enabled;
			bool isFlatFieldAvailable = m_Acquisition != null && m_Acquisition.Initialized && m_Acquisition.FlatFieldAvailable;
			bool isFlatFieldSoftware = m_FlatField != null && m_FlatField.Initialized && m_FlatField.SoftwareCorrection;
			bool isFlatFieldPixelReplacement = m_FlatField != null && m_FlatField.Initialized && m_FlatField.PixelReplacement;

			bool isBayerAvailable = m_Acquisition != null && m_Acquisition.Initialized && m_Acquisition.BayerAvailable;

			// Acquisition Control
			button_Grab.Enabled = bAcqNoGrab;
			button_Snap.Enabled = bAcqNoGrab;
			button_Freeze.Enabled = bAcqGrab;

			// Acquisition Options
			button_LoadConfig.Enabled = bAcqNoGrab;

			// Flat field correction
			checkBox_FaltField.Enabled = bNoGrab;
			button_Calibrate.Enabled = bNoGrab && !isFlatFieldEnabled;
			button_Load_FF.Enabled = bNoGrab && !isFlatFieldEnabled;
			button_Save_FF.Enabled = bNoGrab;

			checkBox_FaltField.Checked = isFlatFieldEnabled;
			checkBox_Hardware_Correction.Checked = isFlatFieldAvailable && !isFlatFieldSoftware;

			bool bayerDecoder = false;
			if (m_Acquisition != null && m_Acquisition.Initialized && m_Acquisition.IsParameterAvailable(SapAcquisition.Prm.BAYER_DECODER_ENABLE))
			{
				// Get Parameter of Bayer Decoder
				int value;
				m_Acquisition.GetParameter(SapAcquisition.Prm.BAYER_DECODER_ENABLE, out value);
				if (value > 0)
					bayerDecoder = true;
				if (isBayerAvailable && bayerDecoder && isFlatFieldAvailable)
					checkBox_Hardware_Correction.Checked = true;
			}

			checkBox_Pixels_Replacement.Checked = isFlatFieldPixelReplacement;

			if ((m_Acquisition != null && m_Acquisition.Initialized) && (isBayerAvailable && bayerDecoder))
				checkBox_Hardware_Correction.Enabled = isBayerAvailable && bayerDecoder && isFlatFieldAvailable;
			else
				checkBox_Hardware_Correction.Enabled = isFlatFieldAvailable && isFlatFieldEnabled && bNoGrab;

			checkBox_Pixels_Replacement.Enabled = isFlatFieldEnabled && isFlatFieldSoftware;

			// File Options
			button_New.Enabled = bNoGrab;
			button_Load.Enabled = bNoGrab;
			button_Save.Enabled = bNoGrab;

			// General Options
			button_Buffers.Enabled = bNoGrab;
			button_View.Enabled = bNoGrab;
		}


		bool CheckPixelFormat(String mode)
		{
			SapFormat format = m_Buffers.Format;

			if (format != SapFormat.Mono8 && format != SapFormat.Mono16)
			{
				String message = String.Format("Pixel format must be 8-bit or 16-bit monochrome for {0}\n", mode); ;
				MessageBox.Show(message);
				return false;
			}

			return true;
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

		//*****************************************************************************************
		//
		//					General Function
		//
		//*****************************************************************************************

		private void button_Exit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void FlatFieldDemoDlg_FormClosed(object sender, FormClosedEventArgs e)
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

		private void FlatFieldDemoDlg_Load(object sender, EventArgs e)
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
		//					Flat field Options
		//
		//*****************************************************************************************

		private void button_Calibrate_Click(object sender, EventArgs e)
		{
			if (!CheckPixelFormat("calibration"))
				return;

			FlatFieldDlg dlg = new FlatFieldDlg(m_FlatField, m_Xfer, m_Buffers);
			dlg.ShowDialog();
			UpdateControls();

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

		private void checkBox_FaltField_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;

			if (m_FlatField != null && m_FlatField.Initialized)
			{
				// To enable/disable flat field correction, the transfer object must first be disconnected from the hardware
				if (m_Xfer != null && m_Xfer.Initialized)
					m_Xfer.Destroy();

				bool success = true;

				// Check for invalid pixel format
				if (checkBox_FaltField.Checked && !checkBox_Hardware_Correction.Checked)
					success = CheckPixelFormat("software correction");

				// Enable/disable flat field correction
				if (success)
					m_FlatField.Enable(checkBox_FaltField.Checked, checkBox_Hardware_Correction.Checked);

				if (m_Xfer != null && !m_Xfer.Initialized)
				{
					// Recreate the transfer object to reconnect it to the hardware
					m_Xfer.Create();
					m_Xfer.Init(true);
					m_Pro.Init();
				}

				UpdateControls();
			}
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

		private void checkBox_Hardware_Correction_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;
			if (m_FlatField != null && m_FlatField.Initialized)
			{
				// To enable/disable flat field correction, the transfer object must first be disconnected from the hardware
				if (m_Xfer != null && m_Xfer.Initialized)
					m_Xfer.Destroy();

				// Get Parameter of Bayer Decoder
				int bayerDecoder;
				if (m_Acquisition != null && m_Acquisition.Initialized && m_Acquisition.BayerAvailable)
				{
					m_Acquisition.GetParameter(SapAcquisition.Prm.BAYER_DECODER_ENABLE, out bayerDecoder);
					if (bayerDecoder > 0)
					{
						checkBox_Hardware_Correction.Checked = true;
						MessageBox.Show("Hardware correction is always used when Bayer decoder is enabled.");
					}
				}

				bool success = true;

				// Check for invalid pixel format when using software correction
				if (checkBox_FaltField.Checked && !checkBox_Hardware_Correction.Checked)
					success = CheckPixelFormat("software correction");

				// Enable/disable flat field correction
				if (success)
				{
					m_FlatField.Enable(checkBox_FaltField.Checked, checkBox_Hardware_Correction.Checked);

				}

				if (m_Xfer != null && !m_Xfer.Initialized)
				{
					// Recreate the transfer object to reconnect it to the hardware
					m_Xfer.Create();
					m_Xfer.Init(true);
					m_Pro.Init();
				}

				UpdateControls();
			}
			Cursor = Cursors.Default;
		}

		private void checkBox_Pixels_Replacement_Click(object sender, EventArgs e)
		{
			// Enable/disable pixel replacement flat field correction
			m_FlatField.PixelReplacement = checkBox_Pixels_Replacement.Checked;

			UpdateControls();
		}


		//*****************************************************************************************
		//
		//					General Options
		//
		//*****************************************************************************************

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

		//*****************************************************************************************
		//
		//					File Options
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
			if (newDialogLoad.ShowDialog() == DialogResult.OK)
			{
				if (m_FlatField.Enabled && m_FlatField.SoftwareCorrection)
				{
					m_Pro.Execute();
				}
			}
			newDialogLoad.Dispose();
			m_ImageBox.OnSize();
		}

		private void button_Save_Click(object sender, EventArgs e)
		{
			LoadSaveDlg newDialogSave = new LoadSaveDlg(m_Buffers, false, false);
			// Show the dialog and process the result
			newDialogSave.ShowDialog();
			newDialogSave.Dispose();
		}
	}
}