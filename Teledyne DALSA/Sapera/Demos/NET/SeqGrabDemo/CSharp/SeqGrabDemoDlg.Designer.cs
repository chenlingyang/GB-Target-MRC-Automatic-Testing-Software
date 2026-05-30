using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.SapClassGui;

namespace DALSA.SaperaLT.Demos.NET.CSharp.SeqGrabDemo
{
   partial class SeqGrabDemoDlg
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SeqGrabDemoDlg));
         this.pictureBox1 = new System.Windows.Forms.PictureBox();
         this.textBox_Maximum = new System.Windows.Forms.TextBox();
         this.groupBox6 = new System.Windows.Forms.GroupBox();
         this.textBox_Minimum = new System.Windows.Forms.TextBox();
         this.label5 = new System.Windows.Forms.Label();
         this.label4 = new System.Windows.Forms.Label();
         this.textBox_NumberOfBuffers = new System.Windows.Forms.TextBox();
         this.textBox_Displayed = new System.Windows.Forms.TextBox();
         this.textBox_Frame_Rate = new System.Windows.Forms.TextBox();
         this.label3 = new System.Windows.Forms.Label();
         this.button_Save_Current = new System.Windows.Forms.Button();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.button_Load_Current = new System.Windows.Forms.Button();
         this.button_Save_Sequence = new System.Windows.Forms.Button();
         this.button_Load_Sequence = new System.Windows.Forms.Button();
         this.panel1 = new System.Windows.Forms.Panel();
         this.button_Exit = new System.Windows.Forms.Button();
         this.groupBox5 = new System.Windows.Forms.GroupBox();
         this.groupBox4 = new System.Windows.Forms.GroupBox();
         this.label2 = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.groupBox3 = new System.Windows.Forms.GroupBox();
         this.button_High_Frame_Rate = new System.Windows.Forms.Button();
         this.button_Load_Config = new System.Windows.Forms.Button();
         this.button_Buffers = new System.Windows.Forms.Button();
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.button_Stop = new System.Windows.Forms.Button();
         this.button_Pause = new System.Windows.Forms.Button();
         this.button_Play = new System.Windows.Forms.Button();
         this.button_Record = new System.Windows.Forms.Button();
         this.timer = new System.Windows.Forms.Timer(this.components);
         this.toolStrip1 = new System.Windows.Forms.ToolStrip();
         this.StatusLabelInfo = new System.Windows.Forms.ToolStripLabel();
         this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
         this.FrameLostCount = new System.Windows.Forms.ToolStripLabel();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
         this.groupBox6.SuspendLayout();
         this.groupBox1.SuspendLayout();
         this.panel1.SuspendLayout();
         this.groupBox5.SuspendLayout();
         this.groupBox4.SuspendLayout();
         this.groupBox3.SuspendLayout();
         this.groupBox2.SuspendLayout();
         this.toolStrip1.SuspendLayout();
         this.SuspendLayout();
         // 
         // pictureBox1
         // 
         this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
         this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
         this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
         this.pictureBox1.Location = new System.Drawing.Point(0, 0);
         this.pictureBox1.Name = "pictureBox1";
         this.pictureBox1.Size = new System.Drawing.Size(64, 625);
         this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
         this.pictureBox1.TabIndex = 7;
         this.pictureBox1.TabStop = false;
         // 
         // textBox_Maximum
         // 
         this.textBox_Maximum.Enabled = false;
         this.textBox_Maximum.Location = new System.Drawing.Point(72, 45);
         this.textBox_Maximum.Name = "textBox_Maximum";
         this.textBox_Maximum.Size = new System.Drawing.Size(65, 20);
         this.textBox_Maximum.TabIndex = 3;
         // 
         // groupBox6
         // 
         this.groupBox6.Controls.Add(this.textBox_Maximum);
         this.groupBox6.Controls.Add(this.textBox_Minimum);
         this.groupBox6.Controls.Add(this.label5);
         this.groupBox6.Controls.Add(this.label4);
         this.groupBox6.Location = new System.Drawing.Point(8, 52);
         this.groupBox6.Name = "groupBox6";
         this.groupBox6.Size = new System.Drawing.Size(143, 82);
         this.groupBox6.TabIndex = 2;
         this.groupBox6.TabStop = false;
         this.groupBox6.Text = "Time between frame (ms)";
         // 
         // textBox_Minimum
         // 
         this.textBox_Minimum.Enabled = false;
         this.textBox_Minimum.Location = new System.Drawing.Point(72, 19);
         this.textBox_Minimum.Name = "textBox_Minimum";
         this.textBox_Minimum.Size = new System.Drawing.Size(65, 20);
         this.textBox_Minimum.TabIndex = 2;
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
         // textBox_NumberOfBuffers
         // 
         this.textBox_NumberOfBuffers.Enabled = false;
         this.textBox_NumberOfBuffers.Location = new System.Drawing.Point(106, 57);
         this.textBox_NumberOfBuffers.Name = "textBox_NumberOfBuffers";
         this.textBox_NumberOfBuffers.Size = new System.Drawing.Size(45, 20);
         this.textBox_NumberOfBuffers.TabIndex = 3;
         // 
         // textBox_Displayed
         // 
         this.textBox_Displayed.Enabled = false;
         this.textBox_Displayed.Location = new System.Drawing.Point(106, 17);
         this.textBox_Displayed.Name = "textBox_Displayed";
         this.textBox_Displayed.Size = new System.Drawing.Size(45, 20);
         this.textBox_Displayed.TabIndex = 2;
         // 
         // textBox_Frame_Rate
         // 
         this.textBox_Frame_Rate.Location = new System.Drawing.Point(95, 21);
         this.textBox_Frame_Rate.Name = "textBox_Frame_Rate";
         this.textBox_Frame_Rate.Size = new System.Drawing.Size(53, 20);
         this.textBox_Frame_Rate.TabIndex = 1;
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
         // button_Save_Current
         // 
         this.button_Save_Current.Location = new System.Drawing.Point(409, 27);
         this.button_Save_Current.Name = "button_Save_Current";
         this.button_Save_Current.Size = new System.Drawing.Size(110, 23);
         this.button_Save_Current.TabIndex = 3;
         this.button_Save_Current.Text = "Save Current ";
         this.button_Save_Current.UseVisualStyleBackColor = true;
         this.button_Save_Current.Click += new System.EventHandler(this.button_Save_Current_Click);
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.button_Save_Current);
         this.groupBox1.Controls.Add(this.button_Load_Current);
         this.groupBox1.Controls.Add(this.button_Save_Sequence);
         this.groupBox1.Controls.Add(this.button_Load_Sequence);
         this.groupBox1.Location = new System.Drawing.Point(259, 0);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(548, 64);
         this.groupBox1.TabIndex = 13;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "File Options";
         // 
         // button_Load_Current
         // 
         this.button_Load_Current.Location = new System.Drawing.Point(293, 27);
         this.button_Load_Current.Name = "button_Load_Current";
         this.button_Load_Current.Size = new System.Drawing.Size(110, 23);
         this.button_Load_Current.TabIndex = 2;
         this.button_Load_Current.Text = "Load Current";
         this.button_Load_Current.UseVisualStyleBackColor = true;
         this.button_Load_Current.Click += new System.EventHandler(this.button_Load_Current_Click);
         // 
         // button_Save_Sequence
         // 
         this.button_Save_Sequence.Location = new System.Drawing.Point(135, 27);
         this.button_Save_Sequence.Name = "button_Save_Sequence";
         this.button_Save_Sequence.Size = new System.Drawing.Size(110, 23);
         this.button_Save_Sequence.TabIndex = 1;
         this.button_Save_Sequence.Text = "Save Sequence";
         this.button_Save_Sequence.UseVisualStyleBackColor = true;
         this.button_Save_Sequence.Click += new System.EventHandler(this.button_Save_Sequence_Click);
         // 
         // button_Load_Sequence
         // 
         this.button_Load_Sequence.Location = new System.Drawing.Point(19, 27);
         this.button_Load_Sequence.Name = "button_Load_Sequence";
         this.button_Load_Sequence.Size = new System.Drawing.Size(110, 23);
         this.button_Load_Sequence.TabIndex = 0;
         this.button_Load_Sequence.Text = "Load Sequence";
         this.button_Load_Sequence.UseVisualStyleBackColor = true;
         this.button_Load_Sequence.Click += new System.EventHandler(this.button_Load_Sequence_Click);
         // 
         // panel1
         // 
         this.panel1.Controls.Add(this.button_Exit);
         this.panel1.Controls.Add(this.groupBox5);
         this.panel1.Controls.Add(this.groupBox4);
         this.panel1.Controls.Add(this.groupBox3);
         this.panel1.Controls.Add(this.groupBox2);
         this.panel1.Location = new System.Drawing.Point(70, 0);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(183, 599);
         this.panel1.TabIndex = 8;
         // 
         // button_Exit
         // 
         this.button_Exit.Location = new System.Drawing.Point(40, 13);
         this.button_Exit.Name = "button_Exit";
         this.button_Exit.Size = new System.Drawing.Size(104, 37);
         this.button_Exit.TabIndex = 4;
         this.button_Exit.Text = "Exit";
         this.button_Exit.UseVisualStyleBackColor = true;
         this.button_Exit.Click += new System.EventHandler(this.button_Exit_Click);
         // 
         // groupBox5
         // 
         this.groupBox5.Controls.Add(this.groupBox6);
         this.groupBox5.Controls.Add(this.textBox_Frame_Rate);
         this.groupBox5.Controls.Add(this.label3);
         this.groupBox5.Location = new System.Drawing.Point(9, 458);
         this.groupBox5.Name = "groupBox5";
         this.groupBox5.Size = new System.Drawing.Size(162, 140);
         this.groupBox5.TabIndex = 3;
         this.groupBox5.TabStop = false;
         this.groupBox5.Text = "Recording Statistics";
         // 
         // groupBox4
         // 
         this.groupBox4.Controls.Add(this.textBox_NumberOfBuffers);
         this.groupBox4.Controls.Add(this.textBox_Displayed);
         this.groupBox4.Controls.Add(this.label2);
         this.groupBox4.Controls.Add(this.label1);
         this.groupBox4.Location = new System.Drawing.Point(6, 352);
         this.groupBox4.Name = "groupBox4";
         this.groupBox4.Size = new System.Drawing.Size(165, 97);
         this.groupBox4.TabIndex = 2;
         this.groupBox4.TabStop = false;
         this.groupBox4.Text = "Buffers";
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(8, 60);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(92, 13);
         this.label2.TabIndex = 1;
         this.label2.Text = "Number of Buffers";
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(8, 20);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(53, 13);
         this.label1.TabIndex = 0;
         this.label1.Text = "Displayed";
         // 
         // groupBox3
         // 
         this.groupBox3.Controls.Add(this.button_High_Frame_Rate);
         this.groupBox3.Controls.Add(this.button_Load_Config);
         this.groupBox3.Controls.Add(this.button_Buffers);
         this.groupBox3.Location = new System.Drawing.Point(6, 226);
         this.groupBox3.Name = "groupBox3";
         this.groupBox3.Size = new System.Drawing.Size(165, 120);
         this.groupBox3.TabIndex = 1;
         this.groupBox3.TabStop = false;
         this.groupBox3.Text = "General Options";
         // 
         // button_High_Frame_Rate
         // 
         this.button_High_Frame_Rate.Location = new System.Drawing.Point(34, 89);
         this.button_High_Frame_Rate.Name = "button_High_Frame_Rate";
         this.button_High_Frame_Rate.Size = new System.Drawing.Size(104, 23);
         this.button_High_Frame_Rate.TabIndex = 2;
         this.button_High_Frame_Rate.Text = "High Frame Rate";
         this.button_High_Frame_Rate.UseVisualStyleBackColor = true;
         this.button_High_Frame_Rate.Click += new System.EventHandler(this.button_High_Frame_Rate_Click);
         // 
         // button_Load_Config
         // 
         this.button_Load_Config.Location = new System.Drawing.Point(34, 54);
         this.button_Load_Config.Name = "button_Load_Config";
         this.button_Load_Config.Size = new System.Drawing.Size(104, 23);
         this.button_Load_Config.TabIndex = 1;
         this.button_Load_Config.Text = "Load Config";
         this.button_Load_Config.UseVisualStyleBackColor = true;
         this.button_Load_Config.Click += new System.EventHandler(this.button_Load_Config_Click);
         // 
         // button_Buffers
         // 
         this.button_Buffers.Location = new System.Drawing.Point(34, 19);
         this.button_Buffers.Name = "button_Buffers";
         this.button_Buffers.Size = new System.Drawing.Size(104, 23);
         this.button_Buffers.TabIndex = 0;
         this.button_Buffers.Text = "Buffers";
         this.button_Buffers.UseVisualStyleBackColor = true;
         this.button_Buffers.Click += new System.EventHandler(this.button_Buffers_Click);
         // 
         // groupBox2
         // 
         this.groupBox2.Controls.Add(this.button_Stop);
         this.groupBox2.Controls.Add(this.button_Pause);
         this.groupBox2.Controls.Add(this.button_Play);
         this.groupBox2.Controls.Add(this.button_Record);
         this.groupBox2.Location = new System.Drawing.Point(5, 58);
         this.groupBox2.Name = "groupBox2";
         this.groupBox2.Size = new System.Drawing.Size(166, 162);
         this.groupBox2.TabIndex = 0;
         this.groupBox2.TabStop = false;
         this.groupBox2.Text = "Record Control";
         // 
         // button_Stop
         // 
         this.button_Stop.Location = new System.Drawing.Point(35, 128);
         this.button_Stop.Name = "button_Stop";
         this.button_Stop.Size = new System.Drawing.Size(104, 23);
         this.button_Stop.TabIndex = 3;
         this.button_Stop.Text = "Stop";
         this.button_Stop.UseVisualStyleBackColor = true;
         this.button_Stop.Click += new System.EventHandler(this.button_Stop_Click);
         // 
         // button_Pause
         // 
         this.button_Pause.Location = new System.Drawing.Point(35, 93);
         this.button_Pause.Name = "button_Pause";
         this.button_Pause.Size = new System.Drawing.Size(104, 23);
         this.button_Pause.TabIndex = 2;
         this.button_Pause.Text = "Pause";
         this.button_Pause.UseVisualStyleBackColor = true;
         this.button_Pause.Click += new System.EventHandler(this.button_Pause_Click);
         // 
         // button_Play
         // 
         this.button_Play.Location = new System.Drawing.Point(35, 57);
         this.button_Play.Name = "button_Play";
         this.button_Play.Size = new System.Drawing.Size(104, 23);
         this.button_Play.TabIndex = 1;
         this.button_Play.Text = "Play";
         this.button_Play.UseVisualStyleBackColor = true;
         this.button_Play.Click += new System.EventHandler(this.button_Play_Click);
         // 
         // button_Record
         // 
         this.button_Record.Location = new System.Drawing.Point(35, 22);
         this.button_Record.Name = "button_Record";
         this.button_Record.Size = new System.Drawing.Size(104, 23);
         this.button_Record.TabIndex = 0;
         this.button_Record.Text = "Record";
         this.button_Record.UseVisualStyleBackColor = true;
         this.button_Record.Click += new System.EventHandler(this.button_Record_Click);
         // 
         // timer
         // 
         this.timer.Tick += new System.EventHandler(this.timer_Tick);
         // 
         // toolStrip1
         // 
         this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabelInfo,
            this.toolStripSeparator1,
            this.FrameLostCount});
         this.toolStrip1.Location = new System.Drawing.Point(64, 600);
         this.toolStrip1.Name = "toolStrip1";
         this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
         this.toolStrip1.Size = new System.Drawing.Size(748, 25);
         this.toolStrip1.TabIndex = 14;
         this.toolStrip1.Text = "toolStrip1";
         // 
         // StatusLabelInfo
         // 
         this.StatusLabelInfo.Name = "StatusLabelInfo";
         this.StatusLabelInfo.Size = new System.Drawing.Size(0, 22);
         // 
         // toolStripSeparator1
         // 
         this.toolStripSeparator1.Name = "toolStripSeparator1";
         this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
         // 
         // FrameLostCount
         // 
         this.FrameLostCount.Name = "FrameLostCount";
         this.FrameLostCount.Size = new System.Drawing.Size(0, 22);
         // 
         // SeqGrabDemoDlg
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(812, 625);
         this.Controls.Add(this.toolStrip1);
         this.Controls.Add(this.pictureBox1);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.panel1);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "SeqGrabDemoDlg";
         this.Text = "SeqGrabDemo .NET";
         this.Load += new System.EventHandler(this.SeqGrabDemoDlg_Load);
         this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SeqGrabDemoDlg_FormClosed);
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
         this.groupBox6.ResumeLayout(false);
         this.groupBox6.PerformLayout();
         this.groupBox1.ResumeLayout(false);
         this.panel1.ResumeLayout(false);
         this.groupBox5.ResumeLayout(false);
         this.groupBox5.PerformLayout();
         this.groupBox4.ResumeLayout(false);
         this.groupBox4.PerformLayout();
         this.groupBox3.ResumeLayout(false);
         this.groupBox2.ResumeLayout(false);
         this.toolStrip1.ResumeLayout(false);
         this.toolStrip1.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.PictureBox pictureBox1;
      private System.Windows.Forms.TextBox textBox_Maximum;
      private System.Windows.Forms.GroupBox groupBox6;
      private System.Windows.Forms.TextBox textBox_Minimum;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.TextBox textBox_NumberOfBuffers;
      private System.Windows.Forms.TextBox textBox_Displayed;
      private System.Windows.Forms.TextBox textBox_Frame_Rate;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Button button_Save_Current;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.Button button_Load_Current;
      private System.Windows.Forms.Button button_Save_Sequence;
      private System.Windows.Forms.Button button_Load_Sequence;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.Button button_Exit;
      private System.Windows.Forms.GroupBox groupBox5;
      private System.Windows.Forms.GroupBox groupBox4;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.GroupBox groupBox3;
      private System.Windows.Forms.Button button_High_Frame_Rate;
      private System.Windows.Forms.Button button_Load_Config;
      private System.Windows.Forms.Button button_Buffers;
      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.Button button_Stop;
      private System.Windows.Forms.Button button_Pause;
      private System.Windows.Forms.Button button_Play;
      private System.Windows.Forms.Button button_Record;
      private System.Windows.Forms.Timer timer;
      private System.Windows.Forms.ToolStrip toolStrip1;
      private System.Windows.Forms.ToolStripLabel StatusLabelInfo;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
      private System.Windows.Forms.ToolStripLabel FrameLostCount;

      private SapAcquisition m_Acquisition;
      private SapBuffer m_Buffers;
      private SapAcqToBuf m_Xfer;
      private SapView m_View;
      private bool m_IsSignalDetected;
      private bool m_online;
      private SapLocation m_ServerLocation;
      private string m_ConfigFileName;
      private bool m_bRecordOn;
      private bool m_bPlayOn;
      private bool m_bPauseOn;
      private int m_nFramesPerCallback;
      private int m_nFramesOnBoard;
      private float m_MinTime;
      private float m_MaxTime;

      //System menu
      private SystemMenu m_SystemMenu;
      //index for "about this.." item im system menu
      private const int m_AboutID = 0x100;
      private const int Default_Nbr_buffers = 15;
      private ImageBox m_ImageBox;


   }
}

