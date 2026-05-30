using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.SapClassGui;

namespace DALSA.SaperaLT.Demos.NET.CSharp.GigEMetadataDemo
{
   partial class GigEMetadataDemoDlg
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GigEMetadataDemoDlg));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_Stop = new System.Windows.Forms.Button();
            this.button_Pause = new System.Windows.Forms.Button();
            this.button_Play = new System.Windows.Forms.Button();
            this.button_Record = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button_HighFrameRate = new System.Windows.Forms.Button();
            this.button_LoadAcqConfig = new System.Windows.Forms.Button();
            this.button_Buffers = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBox_DeltaTimestamp = new System.Windows.Forms.TextBox();
            this.textBox_CurrentBufferTimestamp = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_NumberOfBuffers = new System.Windows.Forms.TextBox();
            this.textBox_BufferDisplayed = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.textBox_MaxTimeBetweenFrames = new System.Windows.Forms.TextBox();
            this.textBox_MinTimeBetweenFrames = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_FrameRate = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_SaveCurrent = new System.Windows.Forms.Button();
            this.button_SaveSequence = new System.Windows.Forms.Button();
            this.button_LoadSequence = new System.Windows.Forms.Button();
            this.button_LoadCurrent = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.button_Reset = new System.Windows.Forms.Button();
            this.button_Update = new System.Windows.Forms.Button();
            this.textBox_CameraTimestamp = new System.Windows.Forms.TextBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.textBoxLineIndex = new System.Windows.Forms.TextBox();
            this.button_SaveMetadata = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.richTextBox_Metadata = new System.Windows.Forms.RichTextBox();
            this.checkedListBox_MetadataItemsEnabled = new System.Windows.Forms.CheckedListBox();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBox_EnableMetadata = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelLineIndex = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(60, 776);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_Stop);
            this.groupBox2.Controls.Add(this.button_Pause);
            this.groupBox2.Controls.Add(this.button_Play);
            this.groupBox2.Controls.Add(this.button_Record);
            this.groupBox2.Location = new System.Drawing.Point(372, 92);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(412, 60);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Record Control";
            // 
            // button_Stop
            // 
            this.button_Stop.Location = new System.Drawing.Point(310, 19);
            this.button_Stop.Name = "button_Stop";
            this.button_Stop.Size = new System.Drawing.Size(95, 33);
            this.button_Stop.TabIndex = 3;
            this.button_Stop.Text = "Stop";
            this.button_Stop.UseVisualStyleBackColor = true;
            this.button_Stop.Click += new System.EventHandler(this.button_Stop_Click);
            // 
            // button_Pause
            // 
            this.button_Pause.Location = new System.Drawing.Point(209, 19);
            this.button_Pause.Name = "button_Pause";
            this.button_Pause.Size = new System.Drawing.Size(95, 33);
            this.button_Pause.TabIndex = 2;
            this.button_Pause.Text = "Pause";
            this.button_Pause.UseVisualStyleBackColor = true;
            this.button_Pause.Click += new System.EventHandler(this.button_Pause_Click);
            // 
            // button_Play
            // 
            this.button_Play.Location = new System.Drawing.Point(108, 19);
            this.button_Play.Name = "button_Play";
            this.button_Play.Size = new System.Drawing.Size(95, 33);
            this.button_Play.TabIndex = 1;
            this.button_Play.Text = "Play";
            this.button_Play.UseVisualStyleBackColor = true;
            this.button_Play.Click += new System.EventHandler(this.button_Play_Click);
            // 
            // button_Record
            // 
            this.button_Record.Location = new System.Drawing.Point(7, 19);
            this.button_Record.Name = "button_Record";
            this.button_Record.Size = new System.Drawing.Size(95, 33);
            this.button_Record.TabIndex = 0;
            this.button_Record.Text = "Record";
            this.button_Record.UseVisualStyleBackColor = true;
            this.button_Record.Click += new System.EventHandler(this.button_Record_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button_HighFrameRate);
            this.groupBox3.Controls.Add(this.button_LoadAcqConfig);
            this.groupBox3.Controls.Add(this.button_Buffers);
            this.groupBox3.Location = new System.Drawing.Point(475, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(309, 60);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "General Options";
            // 
            // button_HighFrameRate
            // 
            this.button_HighFrameRate.Location = new System.Drawing.Point(107, 19);
            this.button_HighFrameRate.Name = "button_HighFrameRate";
            this.button_HighFrameRate.Size = new System.Drawing.Size(95, 33);
            this.button_HighFrameRate.TabIndex = 2;
            this.button_HighFrameRate.Text = "High Frame Rate";
            this.button_HighFrameRate.UseVisualStyleBackColor = true;
            this.button_HighFrameRate.Click += new System.EventHandler(this.button_High_Frame_Rate_Click);
            // 
            // button_LoadAcqConfig
            // 
            this.button_LoadAcqConfig.Location = new System.Drawing.Point(6, 19);
            this.button_LoadAcqConfig.Name = "button_LoadAcqConfig";
            this.button_LoadAcqConfig.Size = new System.Drawing.Size(95, 33);
            this.button_LoadAcqConfig.TabIndex = 1;
            this.button_LoadAcqConfig.Text = "Load Acq Config";
            this.button_LoadAcqConfig.UseVisualStyleBackColor = true;
            this.button_LoadAcqConfig.Click += new System.EventHandler(this.button_Load_Config_Click);
            // 
            // button_Buffers
            // 
            this.button_Buffers.Location = new System.Drawing.Point(208, 19);
            this.button_Buffers.Name = "button_Buffers";
            this.button_Buffers.Size = new System.Drawing.Size(95, 33);
            this.button_Buffers.TabIndex = 0;
            this.button_Buffers.Text = "Buffers";
            this.button_Buffers.UseVisualStyleBackColor = true;
            this.button_Buffers.Click += new System.EventHandler(this.button_Buffers_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBox_DeltaTimestamp);
            this.groupBox4.Controls.Add(this.textBox_CurrentBufferTimestamp);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.textBox_NumberOfBuffers);
            this.groupBox4.Controls.Add(this.textBox_BufferDisplayed);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(66, 78);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(297, 111);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Buffers";
            // 
            // textBox_DeltaTimestamp
            // 
            this.textBox_DeltaTimestamp.Location = new System.Drawing.Point(203, 81);
            this.textBox_DeltaTimestamp.Name = "textBox_DeltaTimestamp";
            this.textBox_DeltaTimestamp.Size = new System.Drawing.Size(88, 20);
            this.textBox_DeltaTimestamp.TabIndex = 7;
            // 
            // textBox_CurrentBufferTimestamp
            // 
            this.textBox_CurrentBufferTimestamp.Location = new System.Drawing.Point(152, 51);
            this.textBox_CurrentBufferTimestamp.Name = "textBox_CurrentBufferTimestamp";
            this.textBox_CurrentBufferTimestamp.Size = new System.Drawing.Size(139, 20);
            this.textBox_CurrentBufferTimestamp.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 84);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(178, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Delta timestamp from previous buffer";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Current Buffer Timestamp";
            // 
            // textBox_NumberOfBuffers
            // 
            this.textBox_NumberOfBuffers.Enabled = false;
            this.textBox_NumberOfBuffers.Location = new System.Drawing.Point(246, 17);
            this.textBox_NumberOfBuffers.Name = "textBox_NumberOfBuffers";
            this.textBox_NumberOfBuffers.Size = new System.Drawing.Size(45, 20);
            this.textBox_NumberOfBuffers.TabIndex = 3;
            // 
            // textBox_BufferDisplayed
            // 
            this.textBox_BufferDisplayed.Enabled = false;
            this.textBox_BufferDisplayed.Location = new System.Drawing.Point(67, 17);
            this.textBox_BufferDisplayed.Name = "textBox_BufferDisplayed";
            this.textBox_BufferDisplayed.Size = new System.Drawing.Size(45, 20);
            this.textBox_BufferDisplayed.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(148, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Number of Buffers";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Displayed";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.groupBox6);
            this.groupBox5.Controls.Add(this.textBox_FrameRate);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Location = new System.Drawing.Point(790, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(162, 140);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Recording Statistics";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.textBox_MaxTimeBetweenFrames);
            this.groupBox6.Controls.Add(this.textBox_MinTimeBetweenFrames);
            this.groupBox6.Controls.Add(this.label5);
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Location = new System.Drawing.Point(8, 52);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(143, 82);
            this.groupBox6.TabIndex = 2;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Time between frame (ms)";
            // 
            // textBox_MaxTimeBetweenFrames
            // 
            this.textBox_MaxTimeBetweenFrames.Enabled = false;
            this.textBox_MaxTimeBetweenFrames.Location = new System.Drawing.Point(72, 45);
            this.textBox_MaxTimeBetweenFrames.Name = "textBox_MaxTimeBetweenFrames";
            this.textBox_MaxTimeBetweenFrames.Size = new System.Drawing.Size(65, 20);
            this.textBox_MaxTimeBetweenFrames.TabIndex = 3;
            // 
            // textBox_MinTimeBetweenFrames
            // 
            this.textBox_MinTimeBetweenFrames.Enabled = false;
            this.textBox_MinTimeBetweenFrames.Location = new System.Drawing.Point(72, 19);
            this.textBox_MinTimeBetweenFrames.Name = "textBox_MinTimeBetweenFrames";
            this.textBox_MinTimeBetweenFrames.Size = new System.Drawing.Size(65, 20);
            this.textBox_MinTimeBetweenFrames.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Maximum";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Minimum";
            // 
            // textBox_FrameRate
            // 
            this.textBox_FrameRate.Location = new System.Drawing.Point(95, 21);
            this.textBox_FrameRate.Name = "textBox_FrameRate";
            this.textBox_FrameRate.Size = new System.Drawing.Size(53, 20);
            this.textBox_FrameRate.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Frame Rate/sec";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_SaveCurrent);
            this.groupBox1.Controls.Add(this.button_SaveSequence);
            this.groupBox1.Controls.Add(this.button_LoadSequence);
            this.groupBox1.Controls.Add(this.button_LoadCurrent);
            this.groupBox1.Location = new System.Drawing.Point(66, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(403, 60);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File Options";
            // 
            // button_SaveCurrent
            // 
            this.button_SaveCurrent.Location = new System.Drawing.Point(303, 19);
            this.button_SaveCurrent.Name = "button_SaveCurrent";
            this.button_SaveCurrent.Size = new System.Drawing.Size(93, 33);
            this.button_SaveCurrent.TabIndex = 3;
            this.button_SaveCurrent.Text = "Save Current ";
            this.button_SaveCurrent.UseVisualStyleBackColor = true;
            this.button_SaveCurrent.Click += new System.EventHandler(this.button_Save_Current_Click);
            // 
            // button_SaveSequence
            // 
            this.button_SaveSequence.Location = new System.Drawing.Point(105, 19);
            this.button_SaveSequence.Name = "button_SaveSequence";
            this.button_SaveSequence.Size = new System.Drawing.Size(93, 33);
            this.button_SaveSequence.TabIndex = 1;
            this.button_SaveSequence.Text = "Save Sequence";
            this.button_SaveSequence.UseVisualStyleBackColor = true;
            this.button_SaveSequence.Click += new System.EventHandler(this.button_Save_Sequence_Click);
            // 
            // button_LoadSequence
            // 
            this.button_LoadSequence.Location = new System.Drawing.Point(6, 19);
            this.button_LoadSequence.Name = "button_LoadSequence";
            this.button_LoadSequence.Size = new System.Drawing.Size(95, 33);
            this.button_LoadSequence.TabIndex = 0;
            this.button_LoadSequence.Text = "Load Sequence";
            this.button_LoadSequence.UseVisualStyleBackColor = true;
            this.button_LoadSequence.Click += new System.EventHandler(this.button_Load_Sequence_Click);
            // 
            // button_LoadCurrent
            // 
            this.button_LoadCurrent.Location = new System.Drawing.Point(204, 19);
            this.button_LoadCurrent.Name = "button_LoadCurrent";
            this.button_LoadCurrent.Size = new System.Drawing.Size(93, 33);
            this.button_LoadCurrent.TabIndex = 2;
            this.button_LoadCurrent.Text = "Load Current";
            this.button_LoadCurrent.UseVisualStyleBackColor = true;
            this.button_LoadCurrent.Click += new System.EventHandler(this.button_Load_Current_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.button_Reset);
            this.groupBox7.Controls.Add(this.button_Update);
            this.groupBox7.Controls.Add(this.textBox_CameraTimestamp);
            this.groupBox7.Location = new System.Drawing.Point(958, 12);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(111, 140);
            this.groupBox7.TabIndex = 7;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Camera Timestamp";
            // 
            // button_Reset
            // 
            this.button_Reset.Location = new System.Drawing.Point(6, 95);
            this.button_Reset.Name = "button_Reset";
            this.button_Reset.Size = new System.Drawing.Size(99, 33);
            this.button_Reset.TabIndex = 2;
            this.button_Reset.Text = "Reset";
            this.button_Reset.UseVisualStyleBackColor = true;
            this.button_Reset.Click += new System.EventHandler(this.button_Reset_Click);
            // 
            // button_Update
            // 
            this.button_Update.Location = new System.Drawing.Point(6, 56);
            this.button_Update.Name = "button_Update";
            this.button_Update.Size = new System.Drawing.Size(99, 33);
            this.button_Update.TabIndex = 1;
            this.button_Update.Text = "Update";
            this.button_Update.UseVisualStyleBackColor = true;
            this.button_Update.Click += new System.EventHandler(this.button_Update_Click);
            // 
            // textBox_CameraTimestamp
            // 
            this.textBox_CameraTimestamp.Location = new System.Drawing.Point(6, 20);
            this.textBox_CameraTimestamp.Name = "textBox_CameraTimestamp";
            this.textBox_CameraTimestamp.Size = new System.Drawing.Size(99, 20);
            this.textBox_CameraTimestamp.TabIndex = 0;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.labelLineIndex);
            this.groupBox8.Controls.Add(this.textBoxLineIndex);
            this.groupBox8.Controls.Add(this.button_SaveMetadata);
            this.groupBox8.Controls.Add(this.label9);
            this.groupBox8.Controls.Add(this.richTextBox_Metadata);
            this.groupBox8.Controls.Add(this.checkedListBox_MetadataItemsEnabled);
            this.groupBox8.Controls.Add(this.label8);
            this.groupBox8.Controls.Add(this.checkBox_EnableMetadata);
            this.groupBox8.Location = new System.Drawing.Point(66, 195);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(297, 569);
            this.groupBox8.TabIndex = 0;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Metadata";
            // 
            // textBoxLineIndex
            // 
            this.textBoxLineIndex.Location = new System.Drawing.Point(121, 492);
            this.textBoxLineIndex.Name = "textBoxLineIndex";
            this.textBoxLineIndex.Size = new System.Drawing.Size(78, 20);
            this.textBoxLineIndex.TabIndex = 14;
            this.textBoxLineIndex.Text = "0";
            this.textBoxLineIndex.Visible = false;
            this.textBoxLineIndex.TextChanged += new System.EventHandler(this.textBoxLineIndex_TextChanged);
            // 
            // button_SaveMetadata
            // 
            this.button_SaveMetadata.Location = new System.Drawing.Point(6, 530);
            this.button_SaveMetadata.Name = "button_SaveMetadata";
            this.button_SaveMetadata.Size = new System.Drawing.Size(285, 33);
            this.button_SaveMetadata.TabIndex = 1;
            this.button_SaveMetadata.Text = "Save Metadata To File";
            this.button_SaveMetadata.UseVisualStyleBackColor = true;
            this.button_SaveMetadata.Click += new System.EventHandler(this.button_SaveMetadata_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(64, 206);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(159, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "Metadata of the displayed buffer";
            // 
            // richTextBox_Metadata
            // 
            this.richTextBox_Metadata.DetectUrls = false;
            this.richTextBox_Metadata.Location = new System.Drawing.Point(6, 222);
            this.richTextBox_Metadata.Name = "richTextBox_Metadata";
            this.richTextBox_Metadata.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTextBox_Metadata.Size = new System.Drawing.Size(285, 266);
            this.richTextBox_Metadata.TabIndex = 12;
            this.richTextBox_Metadata.Text = "";
            this.richTextBox_Metadata.WordWrap = false;
            // 
            // checkedListBox_MetadataItemsEnabled
            // 
            this.checkedListBox_MetadataItemsEnabled.CheckOnClick = true;
            this.checkedListBox_MetadataItemsEnabled.FormattingEnabled = true;
            this.checkedListBox_MetadataItemsEnabled.Location = new System.Drawing.Point(6, 55);
            this.checkedListBox_MetadataItemsEnabled.Name = "checkedListBox_MetadataItemsEnabled";
            this.checkedListBox_MetadataItemsEnabled.Size = new System.Drawing.Size(285, 139);
            this.checkedListBox_MetadataItemsEnabled.TabIndex = 11;
            this.checkedListBox_MetadataItemsEnabled.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox_MetadataItemsEnabled_ItemCheck);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(90, 39);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(94, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "Metadata Selector";
            // 
            // checkBox_EnableMetadata
            // 
            this.checkBox_EnableMetadata.AutoSize = true;
            this.checkBox_EnableMetadata.Location = new System.Drawing.Point(9, 19);
            this.checkBox_EnableMetadata.Name = "checkBox_EnableMetadata";
            this.checkBox_EnableMetadata.Size = new System.Drawing.Size(107, 17);
            this.checkBox_EnableMetadata.TabIndex = 8;
            this.checkBox_EnableMetadata.Text = "Enable Metadata";
            this.checkBox_EnableMetadata.UseVisualStyleBackColor = true;
            this.checkBox_EnableMetadata.CheckedChanged += new System.EventHandler(this.checkBox_EnableMetadata_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Location = new System.Drawing.Point(581, 92);
            this.panel1.MaximumSize = new System.Drawing.Size(0, 1000);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(0, 0);
            this.panel1.TabIndex = 2;
            // 
            // labelLineIndex
            // 
            this.labelLineIndex.AutoSize = true;
            this.labelLineIndex.Location = new System.Drawing.Point(17, 495);
            this.labelLineIndex.Name = "labelLineIndex";
            this.labelLineIndex.Size = new System.Drawing.Size(98, 13);
            this.labelLineIndex.TabIndex = 15;
            this.labelLineIndex.Text = "Line Index (0..100):";
            this.labelLineIndex.Visible = false;
            // 
            // GigEMetadataDemoDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1076, 776);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GigEMetadataDemoDlg";
            this.Text = "GigE-Vision MetadataDemo .NET";
            this.Load += new System.EventHandler(this.GigEMetadataDemoDlg_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GigEMetadataDemoDlg_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

      private System.Windows.Forms.PictureBox pictureBox1;
      private System.Windows.Forms.Timer timer;
      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.Button button_Stop;
      private System.Windows.Forms.Button button_Pause;
      private System.Windows.Forms.Button button_Play;
      private System.Windows.Forms.Button button_Record;
      private System.Windows.Forms.GroupBox groupBox3;
      private System.Windows.Forms.Button button_HighFrameRate;
      private System.Windows.Forms.Button button_LoadAcqConfig;
      private System.Windows.Forms.Button button_Buffers;
      private System.Windows.Forms.GroupBox groupBox4;
      private System.Windows.Forms.TextBox textBox_NumberOfBuffers;
      private System.Windows.Forms.TextBox textBox_BufferDisplayed;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.GroupBox groupBox5;
      private System.Windows.Forms.GroupBox groupBox6;
      private System.Windows.Forms.TextBox textBox_MaxTimeBetweenFrames;
      private System.Windows.Forms.TextBox textBox_MinTimeBetweenFrames;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.TextBox textBox_FrameRate;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.Button button_SaveCurrent;
      private System.Windows.Forms.Button button_LoadCurrent;
      private System.Windows.Forms.Button button_SaveSequence;
      private System.Windows.Forms.Button button_LoadSequence;

      private SapAcqDevice m_AcqDevice;
      private SapFeature m_Feature;
      private SapBuffer m_Buffers;
      private SapAcqDeviceToBuf m_Xfer;
      private SapView m_View;
      private SapLocation m_ServerLocation;
      private SapMetadata m_Metadata;
      private SapMetadata.MetadataType m_MetadataType;
      private string m_ConfigFileName;
      private bool m_bRecordOn;
      private bool m_bPlayOn;
      private bool m_bPauseOn;
      private bool m_XferDisconnectDuringSetup;
      private bool m_IsSelectAvailable;
      private bool m_bIsAreaScan;
      private int m_nFramesPerCallback;
      private bool m_bLastRecordActiveMode;
      private float m_MinTime;
      private float m_MaxTime;
      private string m_originalAppTitle;
      private string m_appTitle;
      private string m_DeviceModelName;
      private string m_DeviceUserID;
      private bool[] m_BufferIsValid;

      //System menu
      private SystemMenu m_SystemMenu;
      //index for "about this.." item im system menu
      private const int m_AboutID = 0x100;
      private const int Default_Nbr_buffers = 5;
      private MyImageBox m_ImageBox;
      private System.Windows.Forms.GroupBox groupBox7;
      private System.Windows.Forms.TextBox textBox_CameraTimestamp;
      private System.Windows.Forms.Button button_Reset;
      private System.Windows.Forms.Button button_Update;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.TextBox textBox_DeltaTimestamp;
      private System.Windows.Forms.TextBox textBox_CurrentBufferTimestamp;
      private System.Windows.Forms.GroupBox groupBox8;
      private System.Windows.Forms.Button button_SaveMetadata;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.RichTextBox richTextBox_Metadata;
      private System.Windows.Forms.CheckedListBox checkedListBox_MetadataItemsEnabled;
      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.CheckBox checkBox_EnableMetadata;
      private System.Windows.Forms.Panel panel1;
       private System.Windows.Forms.TextBox textBoxLineIndex;
       private System.Windows.Forms.Label labelLineIndex;
   }
}
