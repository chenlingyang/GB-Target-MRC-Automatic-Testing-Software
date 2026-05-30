
using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.SapClassGui;


namespace DALSA.SaperaLT.Demos.NET.CSharp.CameraCompressionDemo
{
   partial class CameraCompressionDemoDlg
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
           System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CameraCompressionDemoDlg));
           this.pictureBox1_Logo = new System.Windows.Forms.PictureBox();
           this.toolStrip1 = new System.Windows.Forms.ToolStrip();
           this.StatusLabel = new System.Windows.Forms.ToolStripLabel();
           this.StatusLabelInfo = new System.Windows.Forms.ToolStripLabel();
           this.StatusLabelInfoTrash = new System.Windows.Forms.ToolStripLabel();
           this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
           this.PixelLabel = new System.Windows.Forms.ToolStripLabel();
           this.PixelDataValue = new System.Windows.Forms.ToolStripLabel();
           this.panel1 = new System.Windows.Forms.Panel();
           this.groupBox_File_Control = new System.Windows.Forms.GroupBox();
           this.button_Save = new System.Windows.Forms.Button();
           this.button_Load = new System.Windows.Forms.Button();
           this.button_New = new System.Windows.Forms.Button();
           this.groupBox_General_Options = new System.Windows.Forms.GroupBox();
           this.button_Load_Config = new System.Windows.Forms.Button();
           this.button_View = new System.Windows.Forms.Button();
           this.button_Buffer = new System.Windows.Forms.Button();
           this.groupBox_Acquisition_Control = new System.Windows.Forms.GroupBox();
           this.numericUpDown_JpegQuality = new System.Windows.Forms.NumericUpDown();
           this.label_JpegQuality = new System.Windows.Forms.Label();
           this.checkBox_EnableJpeg = new System.Windows.Forms.CheckBox();
           this.button_Freeze = new System.Windows.Forms.Button();
           this.button_Grab = new System.Windows.Forms.Button();
           this.button_Snap = new System.Windows.Forms.Button();
           this.groupBox_JpegDecompStats = new System.Windows.Forms.GroupBox();
           this.label_PossibleFps = new System.Windows.Forms.Label();
           this.textBox_PossibleFramesPerSecond = new System.Windows.Forms.TextBox();
           this.textBox_JpegDecodeTimeAvg = new System.Windows.Forms.TextBox();
           this.textBox_JpegDecodeTime = new System.Windows.Forms.TextBox();
           this.label_DecAvg = new System.Windows.Forms.Label();
           this.label_DecCurrent = new System.Windows.Forms.Label();
           this.groupBox1 = new System.Windows.Forms.GroupBox();
           this.groupBox_CameraInformation = new System.Windows.Forms.GroupBox();
           this.textBox_NumImagesLostInCamera = new System.Windows.Forms.TextBox();
           this.textBox_NumFramesInCamera = new System.Windows.Forms.TextBox();
           this.label_InternalImagesLost = new System.Windows.Forms.Label();
           this.label_NumFramesCurCam = new System.Windows.Forms.Label();
           this.groupBox_ImageStatistics = new System.Windows.Forms.GroupBox();
           this.textBox_CurrentFramesPerSecond = new System.Windows.Forms.TextBox();
           this.label_CurrentFps = new System.Windows.Forms.Label();
           this.textBox_AverageBandwidth = new System.Windows.Forms.TextBox();
           this.textBox_MaxSpaceUsed = new System.Windows.Forms.TextBox();
           this.textBox_MinSpaceUsed = new System.Windows.Forms.TextBox();
           this.textBox_CurrentSpaceUsed = new System.Windows.Forms.TextBox();
           this.label_AvgBandwidth = new System.Windows.Forms.Label();
           this.label_MaxSize = new System.Windows.Forms.Label();
           this.label_MinSize = new System.Windows.Forms.Label();
           this.label_CurrentSize = new System.Windows.Forms.Label();
           this.StatusLabelInfoIncomplete = new System.Windows.Forms.ToolStripLabel();
           ((System.ComponentModel.ISupportInitialize)(this.pictureBox1_Logo)).BeginInit();
           this.toolStrip1.SuspendLayout();
           this.panel1.SuspendLayout();
           this.groupBox_File_Control.SuspendLayout();
           this.groupBox_General_Options.SuspendLayout();
           this.groupBox_Acquisition_Control.SuspendLayout();
           ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_JpegQuality)).BeginInit();
           this.groupBox_JpegDecompStats.SuspendLayout();
           this.groupBox_CameraInformation.SuspendLayout();
           this.groupBox_ImageStatistics.SuspendLayout();
           this.SuspendLayout();
           // 
           // pictureBox1_Logo
           // 
           this.pictureBox1_Logo.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1_Logo.Image")));
           this.pictureBox1_Logo.Location = new System.Drawing.Point(0, 0);
           this.pictureBox1_Logo.Name = "pictureBox1_Logo";
           this.pictureBox1_Logo.Size = new System.Drawing.Size(60, 500);
           this.pictureBox1_Logo.TabIndex = 0;
           this.pictureBox1_Logo.TabStop = false;
           // 
           // toolStrip1
           // 
           this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
           this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel,
            this.StatusLabelInfo,
            this.StatusLabelInfoTrash,
            this.StatusLabelInfoIncomplete,
            this.toolStripSeparator1,
            this.PixelLabel,
            this.PixelDataValue});
           this.toolStrip1.Location = new System.Drawing.Point(183, 12);
           this.toolStrip1.Name = "toolStrip1";
           this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
           this.toolStrip1.Size = new System.Drawing.Size(272, 25);
           this.toolStrip1.TabIndex = 10;
           this.toolStrip1.Text = "toolStrip1";
           // 
           // StatusLabel
           // 
           this.StatusLabel.Name = "StatusLabel";
           this.StatusLabel.Size = new System.Drawing.Size(45, 22);
           this.StatusLabel.Text = "Status :";
           // 
           // StatusLabelInfo
           // 
           this.StatusLabelInfo.Name = "StatusLabelInfo";
           this.StatusLabelInfo.Size = new System.Drawing.Size(49, 22);
           this.StatusLabelInfo.Text = "nothing";
           // 
           // StatusLabelInfoTrash
           // 
           this.StatusLabelInfoTrash.Name = "StatusLabelInfoTrash";
           this.StatusLabelInfoTrash.Size = new System.Drawing.Size(0, 22);
           // 
           // toolStripSeparator1
           // 
           this.toolStripSeparator1.Name = "toolStripSeparator1";
           this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
           // 
           // PixelLabel
           // 
           this.PixelLabel.Name = "PixelLabel";
           this.PixelLabel.Size = new System.Drawing.Size(37, 22);
           this.PixelLabel.Text = "Pixel :";
           // 
           // PixelDataValue
           // 
           this.PixelDataValue.Name = "PixelDataValue";
           this.PixelDataValue.Size = new System.Drawing.Size(92, 22);
           this.PixelDataValue.Text = "Data not avaible";
           // 
           // panel1
           // 
           this.panel1.Controls.Add(this.groupBox_File_Control);
           this.panel1.Controls.Add(this.groupBox_General_Options);
           this.panel1.Controls.Add(this.groupBox_Acquisition_Control);
           this.panel1.Location = new System.Drawing.Point(66, 12);
           this.panel1.Name = "panel1";
           this.panel1.Size = new System.Drawing.Size(114, 511);
           this.panel1.TabIndex = 11;
           // 
           // groupBox_File_Control
           // 
           this.groupBox_File_Control.Controls.Add(this.button_Save);
           this.groupBox_File_Control.Controls.Add(this.button_Load);
           this.groupBox_File_Control.Controls.Add(this.button_New);
           this.groupBox_File_Control.Location = new System.Drawing.Point(0, 5);
           this.groupBox_File_Control.Name = "groupBox_File_Control";
           this.groupBox_File_Control.Size = new System.Drawing.Size(112, 143);
           this.groupBox_File_Control.TabIndex = 11;
           this.groupBox_File_Control.TabStop = false;
           this.groupBox_File_Control.Text = "File Control";
           // 
           // button_Save
           // 
           this.button_Save.Location = new System.Drawing.Point(6, 103);
           this.button_Save.Name = "button_Save";
           this.button_Save.Size = new System.Drawing.Size(100, 25);
           this.button_Save.TabIndex = 2;
           this.button_Save.Text = "Save";
           this.button_Save.UseVisualStyleBackColor = true;
           this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
           // 
           // button_Load
           // 
           this.button_Load.Location = new System.Drawing.Point(6, 63);
           this.button_Load.Name = "button_Load";
           this.button_Load.Size = new System.Drawing.Size(100, 25);
           this.button_Load.TabIndex = 1;
           this.button_Load.Text = "Load";
           this.button_Load.UseVisualStyleBackColor = true;
           this.button_Load.Click += new System.EventHandler(this.button_Load_Click);
           // 
           // button_New
           // 
           this.button_New.Location = new System.Drawing.Point(6, 23);
           this.button_New.Name = "button_New";
           this.button_New.Size = new System.Drawing.Size(100, 25);
           this.button_New.TabIndex = 0;
           this.button_New.Text = "New";
           this.button_New.UseVisualStyleBackColor = true;
           this.button_New.Click += new System.EventHandler(this.button_New_Click);
           // 
           // groupBox_General_Options
           // 
           this.groupBox_General_Options.Controls.Add(this.button_Load_Config);
           this.groupBox_General_Options.Controls.Add(this.button_View);
           this.groupBox_General_Options.Controls.Add(this.button_Buffer);
           this.groupBox_General_Options.Location = new System.Drawing.Point(0, 367);
           this.groupBox_General_Options.Name = "groupBox_General_Options";
           this.groupBox_General_Options.Size = new System.Drawing.Size(112, 143);
           this.groupBox_General_Options.TabIndex = 12;
           this.groupBox_General_Options.TabStop = false;
           this.groupBox_General_Options.Text = "General Options";
           // 
           // button_Load_Config
           // 
           this.button_Load_Config.Location = new System.Drawing.Point(6, 102);
           this.button_Load_Config.Name = "button_Load_Config";
           this.button_Load_Config.Size = new System.Drawing.Size(100, 25);
           this.button_Load_Config.TabIndex = 0;
           this.button_Load_Config.Text = "Load CCF";
           this.button_Load_Config.UseVisualStyleBackColor = true;
           this.button_Load_Config.Click += new System.EventHandler(this.button_Load_Config_Click);
           // 
           // button_View
           // 
           this.button_View.Location = new System.Drawing.Point(6, 19);
           this.button_View.Name = "button_View";
           this.button_View.Size = new System.Drawing.Size(100, 25);
           this.button_View.TabIndex = 1;
           this.button_View.Text = "View";
           this.button_View.UseVisualStyleBackColor = true;
           this.button_View.Click += new System.EventHandler(this.button_View_Click);
           // 
           // button_Buffer
           // 
           this.button_Buffer.Location = new System.Drawing.Point(6, 60);
           this.button_Buffer.Name = "button_Buffer";
           this.button_Buffer.Size = new System.Drawing.Size(100, 25);
           this.button_Buffer.TabIndex = 0;
           this.button_Buffer.Text = "Buffer";
           this.button_Buffer.UseVisualStyleBackColor = true;
           this.button_Buffer.Click += new System.EventHandler(this.button_Buffer_Click);
           // 
           // groupBox_Acquisition_Control
           // 
           this.groupBox_Acquisition_Control.Controls.Add(this.numericUpDown_JpegQuality);
           this.groupBox_Acquisition_Control.Controls.Add(this.label_JpegQuality);
           this.groupBox_Acquisition_Control.Controls.Add(this.checkBox_EnableJpeg);
           this.groupBox_Acquisition_Control.Controls.Add(this.button_Freeze);
           this.groupBox_Acquisition_Control.Controls.Add(this.button_Grab);
           this.groupBox_Acquisition_Control.Controls.Add(this.button_Snap);
           this.groupBox_Acquisition_Control.Location = new System.Drawing.Point(0, 154);
           this.groupBox_Acquisition_Control.Name = "groupBox_Acquisition_Control";
           this.groupBox_Acquisition_Control.Size = new System.Drawing.Size(112, 207);
           this.groupBox_Acquisition_Control.TabIndex = 9;
           this.groupBox_Acquisition_Control.TabStop = false;
           this.groupBox_Acquisition_Control.Text = "Acquisition Control";
           // 
           // numericUpDown_JpegQuality
           // 
           this.numericUpDown_JpegQuality.Location = new System.Drawing.Point(48, 168);
           this.numericUpDown_JpegQuality.Name = "numericUpDown_JpegQuality";
           this.numericUpDown_JpegQuality.Size = new System.Drawing.Size(58, 20);
           this.numericUpDown_JpegQuality.TabIndex = 17;
           // 
           // label_JpegQuality
           // 
           this.label_JpegQuality.AutoSize = true;
           this.label_JpegQuality.Location = new System.Drawing.Point(3, 170);
           this.label_JpegQuality.Name = "label_JpegQuality";
           this.label_JpegQuality.Size = new System.Drawing.Size(39, 13);
           this.label_JpegQuality.TabIndex = 15;
           this.label_JpegQuality.Text = "Quality";
           // 
           // checkBox_EnableJpeg
           // 
           this.checkBox_EnableJpeg.AutoSize = true;
           this.checkBox_EnableJpeg.Location = new System.Drawing.Point(6, 145);
           this.checkBox_EnableJpeg.Name = "checkBox_EnableJpeg";
           this.checkBox_EnableJpeg.Size = new System.Drawing.Size(89, 17);
           this.checkBox_EnableJpeg.TabIndex = 14;
           this.checkBox_EnableJpeg.Text = "Enable JPEG";
           this.checkBox_EnableJpeg.UseVisualStyleBackColor = true;
           this.checkBox_EnableJpeg.Click += new System.EventHandler(this.checkBox_EnableJpeg_Click);
           // 
           // button_Freeze
           // 
           this.button_Freeze.Location = new System.Drawing.Point(6, 103);
           this.button_Freeze.Name = "button_Freeze";
           this.button_Freeze.Size = new System.Drawing.Size(100, 25);
           this.button_Freeze.TabIndex = 2;
           this.button_Freeze.Text = "Freeze";
           this.button_Freeze.UseVisualStyleBackColor = true;
           this.button_Freeze.Click += new System.EventHandler(this.button_Freeze_Click);
           // 
           // button_Grab
           // 
           this.button_Grab.Location = new System.Drawing.Point(6, 63);
           this.button_Grab.Name = "button_Grab";
           this.button_Grab.Size = new System.Drawing.Size(100, 25);
           this.button_Grab.TabIndex = 1;
           this.button_Grab.Text = "Grab";
           this.button_Grab.UseVisualStyleBackColor = true;
           this.button_Grab.Click += new System.EventHandler(this.button_Grab_Click);
           // 
           // button_Snap
           // 
           this.button_Snap.Location = new System.Drawing.Point(6, 23);
           this.button_Snap.Name = "button_Snap";
           this.button_Snap.Size = new System.Drawing.Size(100, 25);
           this.button_Snap.TabIndex = 0;
           this.button_Snap.Text = "Snap";
           this.button_Snap.UseVisualStyleBackColor = true;
           this.button_Snap.Click += new System.EventHandler(this.button_Snap_Click);
           // 
           // groupBox_JpegDecompStats
           // 
           this.groupBox_JpegDecompStats.Controls.Add(this.label_PossibleFps);
           this.groupBox_JpegDecompStats.Controls.Add(this.textBox_PossibleFramesPerSecond);
           this.groupBox_JpegDecompStats.Controls.Add(this.textBox_JpegDecodeTimeAvg);
           this.groupBox_JpegDecompStats.Controls.Add(this.textBox_JpegDecodeTime);
           this.groupBox_JpegDecompStats.Controls.Add(this.label_DecAvg);
           this.groupBox_JpegDecompStats.Controls.Add(this.label_DecCurrent);
           this.groupBox_JpegDecompStats.Location = new System.Drawing.Point(186, 40);
           this.groupBox_JpegDecompStats.Name = "groupBox_JpegDecompStats";
           this.groupBox_JpegDecompStats.Size = new System.Drawing.Size(489, 49);
           this.groupBox_JpegDecompStats.TabIndex = 12;
           this.groupBox_JpegDecompStats.TabStop = false;
           this.groupBox_JpegDecompStats.Text = "JPEG Decompression Statistics";
           // 
           // label_PossibleFps
           // 
           this.label_PossibleFps.AutoSize = true;
           this.label_PossibleFps.Location = new System.Drawing.Point(308, 23);
           this.label_PossibleFps.Name = "label_PossibleFps";
           this.label_PossibleFps.Size = new System.Drawing.Size(109, 13);
           this.label_PossibleFps.TabIndex = 9;
           this.label_PossibleFps.Text = "Maximum possible fps";
           // 
           // textBox_PossibleFramesPerSecond
           // 
           this.textBox_PossibleFramesPerSecond.Location = new System.Drawing.Point(423, 20);
           this.textBox_PossibleFramesPerSecond.Name = "textBox_PossibleFramesPerSecond";
           this.textBox_PossibleFramesPerSecond.Size = new System.Drawing.Size(56, 20);
           this.textBox_PossibleFramesPerSecond.TabIndex = 4;
           // 
           // textBox_JpegDecodeTimeAvg
           // 
           this.textBox_JpegDecodeTimeAvg.Location = new System.Drawing.Point(240, 20);
           this.textBox_JpegDecodeTimeAvg.Name = "textBox_JpegDecodeTimeAvg";
           this.textBox_JpegDecodeTimeAvg.Size = new System.Drawing.Size(56, 20);
           this.textBox_JpegDecodeTimeAvg.TabIndex = 3;
           // 
           // textBox_JpegDecodeTime
           // 
           this.textBox_JpegDecodeTime.Location = new System.Drawing.Point(87, 20);
           this.textBox_JpegDecodeTime.Name = "textBox_JpegDecodeTime";
           this.textBox_JpegDecodeTime.Size = new System.Drawing.Size(56, 20);
           this.textBox_JpegDecodeTime.TabIndex = 2;
           // 
           // label_DecAvg
           // 
           this.label_DecAvg.AutoSize = true;
           this.label_DecAvg.Location = new System.Drawing.Point(154, 23);
           this.label_DecAvg.Name = "label_DecAvg";
           this.label_DecAvg.Size = new System.Drawing.Size(80, 13);
           this.label_DecAvg.TabIndex = 1;
           this.label_DecAvg.Text = "Average (in ms)";
           // 
           // label_DecCurrent
           // 
           this.label_DecCurrent.AutoSize = true;
           this.label_DecCurrent.Location = new System.Drawing.Point(7, 23);
           this.label_DecCurrent.Name = "label_DecCurrent";
           this.label_DecCurrent.Size = new System.Drawing.Size(74, 13);
           this.label_DecCurrent.TabIndex = 0;
           this.label_DecCurrent.Text = "Current (in ms)";
           // 
           // groupBox1
           // 
           this.groupBox1.Location = new System.Drawing.Point(0, 0);
           this.groupBox1.Name = "groupBox1";
           this.groupBox1.Size = new System.Drawing.Size(200, 100);
           this.groupBox1.TabIndex = 0;
           this.groupBox1.TabStop = false;
           // 
           // groupBox_CameraInformation
           // 
           this.groupBox_CameraInformation.Controls.Add(this.textBox_NumImagesLostInCamera);
           this.groupBox_CameraInformation.Controls.Add(this.textBox_NumFramesInCamera);
           this.groupBox_CameraInformation.Controls.Add(this.label_InternalImagesLost);
           this.groupBox_CameraInformation.Controls.Add(this.label_NumFramesCurCam);
           this.groupBox_CameraInformation.Location = new System.Drawing.Point(681, 40);
           this.groupBox_CameraInformation.Name = "groupBox_CameraInformation";
           this.groupBox_CameraInformation.Size = new System.Drawing.Size(433, 49);
           this.groupBox_CameraInformation.TabIndex = 13;
           this.groupBox_CameraInformation.TabStop = false;
           this.groupBox_CameraInformation.Text = "Camera Information";
           // 
           // textBox_NumImagesLostInCamera
           // 
           this.textBox_NumImagesLostInCamera.Location = new System.Drawing.Point(364, 20);
           this.textBox_NumImagesLostInCamera.Name = "textBox_NumImagesLostInCamera";
           this.textBox_NumImagesLostInCamera.Size = new System.Drawing.Size(56, 20);
           this.textBox_NumImagesLostInCamera.TabIndex = 3;
           // 
           // textBox_NumFramesInCamera
           // 
           this.textBox_NumFramesInCamera.Location = new System.Drawing.Point(194, 20);
           this.textBox_NumFramesInCamera.Name = "textBox_NumFramesInCamera";
           this.textBox_NumFramesInCamera.Size = new System.Drawing.Size(56, 20);
           this.textBox_NumFramesInCamera.TabIndex = 2;
           // 
           // label_InternalImagesLost
           // 
           this.label_InternalImagesLost.AutoSize = true;
           this.label_InternalImagesLost.Location = new System.Drawing.Point(256, 23);
           this.label_InternalImagesLost.Name = "label_InternalImagesLost";
           this.label_InternalImagesLost.Size = new System.Drawing.Size(102, 13);
           this.label_InternalImagesLost.TabIndex = 1;
           this.label_InternalImagesLost.Text = "Internal Images Lost";
           // 
           // label_NumFramesCurCam
           // 
           this.label_NumFramesCurCam.AutoSize = true;
           this.label_NumFramesCurCam.Location = new System.Drawing.Point(6, 23);
           this.label_NumFramesCurCam.Name = "label_NumFramesCurCam";
           this.label_NumFramesCurCam.Size = new System.Drawing.Size(182, 13);
           this.label_NumFramesCurCam.TabIndex = 0;
           this.label_NumFramesCurCam.Text = "Number of frames currently in camera";
           // 
           // groupBox_ImageStatistics
           // 
           this.groupBox_ImageStatistics.Controls.Add(this.textBox_CurrentFramesPerSecond);
           this.groupBox_ImageStatistics.Controls.Add(this.label_CurrentFps);
           this.groupBox_ImageStatistics.Controls.Add(this.textBox_AverageBandwidth);
           this.groupBox_ImageStatistics.Controls.Add(this.textBox_MaxSpaceUsed);
           this.groupBox_ImageStatistics.Controls.Add(this.textBox_MinSpaceUsed);
           this.groupBox_ImageStatistics.Controls.Add(this.textBox_CurrentSpaceUsed);
           this.groupBox_ImageStatistics.Controls.Add(this.label_AvgBandwidth);
           this.groupBox_ImageStatistics.Controls.Add(this.label_MaxSize);
           this.groupBox_ImageStatistics.Controls.Add(this.label_MinSize);
           this.groupBox_ImageStatistics.Controls.Add(this.label_CurrentSize);
           this.groupBox_ImageStatistics.Location = new System.Drawing.Point(186, 95);
           this.groupBox_ImageStatistics.Name = "groupBox_ImageStatistics";
           this.groupBox_ImageStatistics.Size = new System.Drawing.Size(928, 47);
           this.groupBox_ImageStatistics.TabIndex = 14;
           this.groupBox_ImageStatistics.TabStop = false;
           this.groupBox_ImageStatistics.Text = "Image Statistics";
           // 
           // textBox_CurrentFramesPerSecond
           // 
           this.textBox_CurrentFramesPerSecond.Location = new System.Drawing.Point(689, 20);
           this.textBox_CurrentFramesPerSecond.Name = "textBox_CurrentFramesPerSecond";
           this.textBox_CurrentFramesPerSecond.Size = new System.Drawing.Size(56, 20);
           this.textBox_CurrentFramesPerSecond.TabIndex = 9;
           // 
           // label_CurrentFps
           // 
           this.label_CurrentFps.AutoSize = true;
           this.label_CurrentFps.Location = new System.Drawing.Point(625, 23);
           this.label_CurrentFps.Name = "label_CurrentFps";
           this.label_CurrentFps.Size = new System.Drawing.Size(58, 13);
           this.label_CurrentFps.TabIndex = 8;
           this.label_CurrentFps.Text = "Current fps";
           // 
           // textBox_AverageBandwidth
           // 
           this.textBox_AverageBandwidth.Location = new System.Drawing.Point(537, 20);
           this.textBox_AverageBandwidth.Name = "textBox_AverageBandwidth";
           this.textBox_AverageBandwidth.Size = new System.Drawing.Size(56, 20);
           this.textBox_AverageBandwidth.TabIndex = 7;
           // 
           // textBox_MaxSpaceUsed
           // 
           this.textBox_MaxSpaceUsed.Location = new System.Drawing.Point(327, 20);
           this.textBox_MaxSpaceUsed.Name = "textBox_MaxSpaceUsed";
           this.textBox_MaxSpaceUsed.Size = new System.Drawing.Size(56, 20);
           this.textBox_MaxSpaceUsed.TabIndex = 6;
           // 
           // textBox_MinSpaceUsed
           // 
           this.textBox_MinSpaceUsed.Location = new System.Drawing.Point(207, 20);
           this.textBox_MinSpaceUsed.Name = "textBox_MinSpaceUsed";
           this.textBox_MinSpaceUsed.Size = new System.Drawing.Size(56, 20);
           this.textBox_MinSpaceUsed.TabIndex = 5;
           // 
           // textBox_CurrentSpaceUsed
           // 
           this.textBox_CurrentSpaceUsed.Location = new System.Drawing.Point(75, 20);
           this.textBox_CurrentSpaceUsed.Name = "textBox_CurrentSpaceUsed";
           this.textBox_CurrentSpaceUsed.Size = new System.Drawing.Size(56, 20);
           this.textBox_CurrentSpaceUsed.TabIndex = 4;
           // 
           // label_AvgBandwidth
           // 
           this.label_AvgBandwidth.AutoSize = true;
           this.label_AvgBandwidth.Location = new System.Drawing.Point(437, 23);
           this.label_AvgBandwidth.Name = "label_AvgBandwidth";
           this.label_AvgBandwidth.Size = new System.Drawing.Size(97, 13);
           this.label_AvgBandwidth.TabIndex = 3;
           this.label_AvgBandwidth.Text = "AverageBandwidth";
           // 
           // label_MaxSize
           // 
           this.label_MaxSize.AutoSize = true;
           this.label_MaxSize.Location = new System.Drawing.Point(270, 23);
           this.label_MaxSize.Name = "label_MaxSize";
           this.label_MaxSize.Size = new System.Drawing.Size(48, 13);
           this.label_MaxSize.TabIndex = 2;
           this.label_MaxSize.Text = "Max size";
           // 
           // label_MinSize
           // 
           this.label_MinSize.AutoSize = true;
           this.label_MinSize.Location = new System.Drawing.Point(153, 23);
           this.label_MinSize.Name = "label_MinSize";
           this.label_MinSize.Size = new System.Drawing.Size(45, 13);
           this.label_MinSize.TabIndex = 1;
           this.label_MinSize.Text = "Min size";
           // 
           // label_CurrentSize
           // 
           this.label_CurrentSize.AutoSize = true;
           this.label_CurrentSize.Location = new System.Drawing.Point(7, 23);
           this.label_CurrentSize.Name = "label_CurrentSize";
           this.label_CurrentSize.Size = new System.Drawing.Size(62, 13);
           this.label_CurrentSize.TabIndex = 0;
           this.label_CurrentSize.Text = "Current size";
           // 
           // StatusLabelInfoIncomplete
           // 
           this.StatusLabelInfoIncomplete.Name = "StatusLabelInfoIncomplete";
           this.StatusLabelInfoIncomplete.Size = new System.Drawing.Size(0, 22);
           // 
           // CameraCompressionDemoDlg
           // 
           this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
           this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
           this.ClientSize = new System.Drawing.Size(1121, 532);
           this.Controls.Add(this.groupBox_ImageStatistics);
           this.Controls.Add(this.groupBox_CameraInformation);
           this.Controls.Add(this.groupBox_JpegDecompStats);
           this.Controls.Add(this.toolStrip1);
           this.Controls.Add(this.pictureBox1_Logo);
           this.Controls.Add(this.panel1);
           this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
           this.Name = "CameraCompressionDemoDlg";
           this.Text = "GigE-Vision Camera Demo .NET";
           this.Load += new System.EventHandler(this.CameraCompressionDemoDlg_Load);
           this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CameraCompressionDemoDlg_FormClosed);
           ((System.ComponentModel.ISupportInitialize)(this.pictureBox1_Logo)).EndInit();
           this.toolStrip1.ResumeLayout(false);
           this.toolStrip1.PerformLayout();
           this.panel1.ResumeLayout(false);
           this.groupBox_File_Control.ResumeLayout(false);
           this.groupBox_General_Options.ResumeLayout(false);
           this.groupBox_Acquisition_Control.ResumeLayout(false);
           this.groupBox_Acquisition_Control.PerformLayout();
           ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_JpegQuality)).EndInit();
           this.groupBox_JpegDecompStats.ResumeLayout(false);
           this.groupBox_JpegDecompStats.PerformLayout();
           this.groupBox_CameraInformation.ResumeLayout(false);
           this.groupBox_CameraInformation.PerformLayout();
           this.groupBox_ImageStatistics.ResumeLayout(false);
           this.groupBox_ImageStatistics.PerformLayout();
           this.ResumeLayout(false);
           this.PerformLayout();

        }

        #endregion

      private System.Windows.Forms.PictureBox pictureBox1_Logo;
      private System.Windows.Forms.ToolStrip toolStrip1;
      private System.Windows.Forms.ToolStripLabel StatusLabel;
      private System.Windows.Forms.ToolStripLabel StatusLabelInfo;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
      private System.Windows.Forms.ToolStripLabel PixelLabel;
      private System.Windows.Forms.ToolStripLabel PixelDataValue;
      private System.Windows.Forms.ToolStripLabel StatusLabelInfoTrash;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.GroupBox groupBox_File_Control;
      private System.Windows.Forms.Button button_Save;
      private System.Windows.Forms.Button button_Load;
      private System.Windows.Forms.Button button_New;
      private System.Windows.Forms.GroupBox groupBox_General_Options;
      private System.Windows.Forms.Button button_View;
      private System.Windows.Forms.Button button_Buffer;
      private System.Windows.Forms.Button button_Load_Config;
      private System.Windows.Forms.GroupBox groupBox_Acquisition_Control;
      private System.Windows.Forms.Button button_Freeze;
      private System.Windows.Forms.Button button_Grab;
      private System.Windows.Forms.Button button_Snap;
      private ImageBox m_ImageBox;

      private SapAcqDevice m_AcqDevice;
      private SapBuffer m_Buffers;
      private SapBuffer m_BuffersView;
      private SapAcqDeviceToBuf m_Xfer;
      private SapView m_View;
      private SapLocation m_ServerLocation;
      private SapMyProcessing m_Pro;
      private JpegProcessing m_JpegProcessing;
      private string m_ConfigFileName;

      private bool m_CompressedImageAvailable;
      private bool m_JpegEnabled;

      //System menu
      private SystemMenu m_SystemMenu;
      //index for "about this.." item im system menu
      private const int m_AboutID = 0x100;
      private System.Windows.Forms.CheckBox checkBox_EnableJpeg;
      private System.Windows.Forms.Label label_JpegQuality;
      private System.Windows.Forms.NumericUpDown numericUpDown_JpegQuality;
      private System.Windows.Forms.GroupBox groupBox_JpegDecompStats;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.GroupBox groupBox_CameraInformation;
      private System.Windows.Forms.GroupBox groupBox_ImageStatistics;
      private System.Windows.Forms.Label label_DecAvg;
      private System.Windows.Forms.Label label_DecCurrent;
      private System.Windows.Forms.Label label_NumFramesCurCam;
      private System.Windows.Forms.Label label_InternalImagesLost;
      private System.Windows.Forms.Label label_MaxSize;
      private System.Windows.Forms.Label label_MinSize;
      private System.Windows.Forms.Label label_CurrentSize;
      private System.Windows.Forms.Label label_AvgBandwidth;
      private System.Windows.Forms.TextBox textBox_JpegDecodeTimeAvg;
      private System.Windows.Forms.TextBox textBox_JpegDecodeTime;
      private System.Windows.Forms.TextBox textBox_PossibleFramesPerSecond;
      private System.Windows.Forms.TextBox textBox_NumFramesInCamera;
      private System.Windows.Forms.TextBox textBox_NumImagesLostInCamera;
      private System.Windows.Forms.TextBox textBox_CurrentSpaceUsed;
      private System.Windows.Forms.TextBox textBox_MaxSpaceUsed;
      private System.Windows.Forms.TextBox textBox_MinSpaceUsed;
      private System.Windows.Forms.TextBox textBox_AverageBandwidth;
      private System.Windows.Forms.Label label_PossibleFps;
      private System.Windows.Forms.TextBox textBox_CurrentFramesPerSecond;
      private System.Windows.Forms.Label label_CurrentFps;
      private System.Windows.Forms.ToolStripLabel StatusLabelInfoIncomplete;
   }
}
