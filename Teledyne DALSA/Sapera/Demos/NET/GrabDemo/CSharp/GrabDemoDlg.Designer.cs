
using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.SapClassGui;

namespace DALSA.SaperaLT.Demos.NET.CSharp.GrabDemo
{
    partial class GrabDemoDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GrabDemoDlg));
            this.pictureBox1_Logo = new System.Windows.Forms.PictureBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripLabel();
            this.StatusLabelInfo = new System.Windows.Forms.ToolStripLabel();
            this.StatusLabelInfoTrash = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.PixelLabel = new System.Windows.Forms.ToolStripLabel();
            this.PixelDataValue = new System.Windows.Forms.ToolStripLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1_General = new System.Windows.Forms.GroupBox();
            this.button_View = new System.Windows.Forms.Button();
            this.button_Buffer = new System.Windows.Forms.Button();
            this.groupBox1_FileControl = new System.Windows.Forms.GroupBox();
            this.button_Save = new System.Windows.Forms.Button();
            this.button_Load = new System.Windows.Forms.Button();
            this.button_New = new System.Windows.Forms.Button();
            this.groupBox1_Acq_Options = new System.Windows.Forms.GroupBox();
            this.button_LoadConfig = new System.Windows.Forms.Button();
            this.button_Exit = new System.Windows.Forms.Button();
            this.groupBox1_Acquisition = new System.Windows.Forms.GroupBox();
            this.button_Freeze = new System.Windows.Forms.Button();
            this.button_Grab = new System.Windows.Forms.Button();
            this.button_Snap = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1_Logo)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1_General.SuspendLayout();
            this.groupBox1_FileControl.SuspendLayout();
            this.groupBox1_Acq_Options.SuspendLayout();
            this.groupBox1_Acquisition.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1_Logo
            // 
            this.pictureBox1_Logo.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1_Logo.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1_Logo.Image")));
            this.pictureBox1_Logo.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1_Logo.Name = "pictureBox1_Logo";
            this.pictureBox1_Logo.Size = new System.Drawing.Size(60, 534);
            this.pictureBox1_Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1_Logo.TabIndex = 0;
            this.pictureBox1_Logo.TabStop = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel,
            this.StatusLabelInfo,
            this.StatusLabelInfoTrash,
            this.toolStripSeparator1,
            this.PixelLabel,
            this.PixelDataValue});
            this.toolStrip1.Location = new System.Drawing.Point(60, 509);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(741, 25);
            this.toolStrip1.TabIndex = 11;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Font = new System.Drawing.Font("Tahoma", 9F);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(50, 22);
            this.StatusLabel.Text = "Status :";
            // 
            // StatusLabelInfo
            // 
            this.StatusLabelInfo.Name = "StatusLabelInfo";
            this.StatusLabelInfo.Size = new System.Drawing.Size(43, 22);
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
            this.PixelLabel.Font = new System.Drawing.Font("Tahoma", 9F);
            this.PixelLabel.Name = "PixelLabel";
            this.PixelLabel.Size = new System.Drawing.Size(39, 22);
            this.PixelLabel.Text = "Pixel :";
            // 
            // PixelDataValue
            // 
            this.PixelDataValue.Name = "PixelDataValue";
            this.PixelDataValue.Size = new System.Drawing.Size(85, 22);
            this.PixelDataValue.Text = "data not avaible";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1_General);
            this.panel1.Controls.Add(this.groupBox1_FileControl);
            this.panel1.Controls.Add(this.groupBox1_Acq_Options);
            this.panel1.Controls.Add(this.button_Exit);
            this.panel1.Controls.Add(this.groupBox1_Acquisition);
            this.panel1.Location = new System.Drawing.Point(66, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(166, 506);
            this.panel1.TabIndex = 12;
            // 
            // groupBox1_General
            // 
            this.groupBox1_General.Controls.Add(this.button_View);
            this.groupBox1_General.Controls.Add(this.button_Buffer);
            this.groupBox1_General.Location = new System.Drawing.Point(12, 360);
            this.groupBox1_General.Name = "groupBox1_General";
            this.groupBox1_General.Size = new System.Drawing.Size(144, 86);
            this.groupBox1_General.TabIndex = 12;
            this.groupBox1_General.TabStop = false;
            this.groupBox1_General.Text = "General Options";
            // 
            // button_View
            // 
            this.button_View.Location = new System.Drawing.Point(21, 52);
            this.button_View.Name = "button_View";
            this.button_View.Size = new System.Drawing.Size(104, 23);
            this.button_View.TabIndex = 1;
            this.button_View.Text = "View";
            this.button_View.UseVisualStyleBackColor = true;
            this.button_View.Click += new System.EventHandler(this.button_View_Click);
            // 
            // button_Buffer
            // 
            this.button_Buffer.Location = new System.Drawing.Point(22, 21);
            this.button_Buffer.Name = "button_Buffer";
            this.button_Buffer.Size = new System.Drawing.Size(103, 23);
            this.button_Buffer.TabIndex = 0;
            this.button_Buffer.Text = "Buffer";
            this.button_Buffer.UseVisualStyleBackColor = true;
            this.button_Buffer.Click += new System.EventHandler(this.button_Buffer_Click);
            // 
            // groupBox1_FileControl
            // 
            this.groupBox1_FileControl.Controls.Add(this.button_Save);
            this.groupBox1_FileControl.Controls.Add(this.button_Load);
            this.groupBox1_FileControl.Controls.Add(this.button_New);
            this.groupBox1_FileControl.Location = new System.Drawing.Point(12, 233);
            this.groupBox1_FileControl.Name = "groupBox1_FileControl";
            this.groupBox1_FileControl.Size = new System.Drawing.Size(143, 121);
            this.groupBox1_FileControl.TabIndex = 11;
            this.groupBox1_FileControl.TabStop = false;
            this.groupBox1_FileControl.Text = "File Control";
            // 
            // button_Save
            // 
            this.button_Save.Location = new System.Drawing.Point(20, 81);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(105, 23);
            this.button_Save.TabIndex = 2;
            this.button_Save.Text = "Save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // button_Load
            // 
            this.button_Load.Location = new System.Drawing.Point(20, 51);
            this.button_Load.Name = "button_Load";
            this.button_Load.Size = new System.Drawing.Size(105, 23);
            this.button_Load.TabIndex = 1;
            this.button_Load.Text = "Load";
            this.button_Load.UseVisualStyleBackColor = true;
            this.button_Load.Click += new System.EventHandler(this.button_Load_Click);
            // 
            // button_New
            // 
            this.button_New.Location = new System.Drawing.Point(20, 19);
            this.button_New.Name = "button_New";
            this.button_New.Size = new System.Drawing.Size(105, 23);
            this.button_New.TabIndex = 0;
            this.button_New.Text = "New";
            this.button_New.UseVisualStyleBackColor = true;
            this.button_New.Click += new System.EventHandler(this.button_New_Click);
            // 
            // groupBox1_Acq_Options
            // 
            this.groupBox1_Acq_Options.Controls.Add(this.button_LoadConfig);
            this.groupBox1_Acq_Options.Location = new System.Drawing.Point(12, 452);
            this.groupBox1_Acq_Options.Name = "groupBox1_Acq_Options";
            this.groupBox1_Acq_Options.Size = new System.Drawing.Size(144, 52);
            this.groupBox1_Acq_Options.TabIndex = 13;
            this.groupBox1_Acq_Options.TabStop = false;
            this.groupBox1_Acq_Options.Text = "Acquisition Options";
            // 
            // button_LoadConfig
            // 
            this.button_LoadConfig.Location = new System.Drawing.Point(20, 23);
            this.button_LoadConfig.Name = "button_LoadConfig";
            this.button_LoadConfig.Size = new System.Drawing.Size(105, 23);
            this.button_LoadConfig.TabIndex = 0;
            this.button_LoadConfig.Text = "Load Config";
            this.button_LoadConfig.UseVisualStyleBackColor = true;
            this.button_LoadConfig.Click += new System.EventHandler(this.button_LoadConfig_Click);
            // 
            // button_Exit
            // 
            this.button_Exit.Location = new System.Drawing.Point(33, 12);
            this.button_Exit.Name = "button_Exit";
            this.button_Exit.Size = new System.Drawing.Size(104, 33);
            this.button_Exit.TabIndex = 10;
            this.button_Exit.Text = "Exit";
            this.button_Exit.UseVisualStyleBackColor = true;
            this.button_Exit.Click += new System.EventHandler(this.button_Exit_Click);
            // 
            // groupBox1_Acquisition
            // 
            this.groupBox1_Acquisition.Controls.Add(this.button_Freeze);
            this.groupBox1_Acquisition.Controls.Add(this.button_Grab);
            this.groupBox1_Acquisition.Controls.Add(this.button_Snap);
            this.groupBox1_Acquisition.Location = new System.Drawing.Point(12, 63);
            this.groupBox1_Acquisition.Name = "groupBox1_Acquisition";
            this.groupBox1_Acquisition.Size = new System.Drawing.Size(143, 164);
            this.groupBox1_Acquisition.TabIndex = 9;
            this.groupBox1_Acquisition.TabStop = false;
            this.groupBox1_Acquisition.Text = "Acquisition Control";
            // 
            // button_Freeze
            // 
            this.button_Freeze.Location = new System.Drawing.Point(20, 115);
            this.button_Freeze.Name = "button_Freeze";
            this.button_Freeze.Size = new System.Drawing.Size(105, 24);
            this.button_Freeze.TabIndex = 2;
            this.button_Freeze.Text = "Freeze";
            this.button_Freeze.UseVisualStyleBackColor = true;
            this.button_Freeze.Click += new System.EventHandler(this.button_Freeze_Click);
            // 
            // button_Grab
            // 
            this.button_Grab.Location = new System.Drawing.Point(20, 72);
            this.button_Grab.Name = "button_Grab";
            this.button_Grab.Size = new System.Drawing.Size(105, 24);
            this.button_Grab.TabIndex = 1;
            this.button_Grab.Text = "Grab";
            this.button_Grab.UseVisualStyleBackColor = true;
            this.button_Grab.Click += new System.EventHandler(this.button_Grab_Click);
            // 
            // button_Snap
            // 
            this.button_Snap.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Snap.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_Snap.Location = new System.Drawing.Point(20, 29);
            this.button_Snap.Name = "button_Snap";
            this.button_Snap.Size = new System.Drawing.Size(105, 25);
            this.button_Snap.TabIndex = 0;
            this.button_Snap.Text = "Snap";
            this.button_Snap.UseVisualStyleBackColor = true;
            this.button_Snap.Click += new System.EventHandler(this.button_Snap_Click);
            // 
            // GrabDemoDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 534);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.pictureBox1_Logo);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GrabDemoDlg";
            this.Text = "Grab Demo .NET";
            this.Load += new System.EventHandler(this.GrabDemoDlg_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GrabDemoDlg_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1_Logo)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox1_General.ResumeLayout(false);
            this.groupBox1_FileControl.ResumeLayout(false);
            this.groupBox1_Acq_Options.ResumeLayout(false);
            this.groupBox1_Acquisition.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

       private System.Windows.Forms.PictureBox pictureBox1_Logo;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel StatusLabel;
        private System.Windows.Forms.ToolStripLabel StatusLabelInfo;
        private System.Windows.Forms.ToolStripLabel PixelLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel PixelDataValue;
        private System.Windows.Forms.ToolStripLabel StatusLabelInfoTrash;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1_General;
        private System.Windows.Forms.Button button_View;
        private System.Windows.Forms.Button button_Buffer;
        private System.Windows.Forms.GroupBox groupBox1_FileControl;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.Button button_Load;
        private System.Windows.Forms.Button button_New;
        private System.Windows.Forms.GroupBox groupBox1_Acq_Options;
        private System.Windows.Forms.Button button_LoadConfig;
        private System.Windows.Forms.Button button_Exit;
        private System.Windows.Forms.GroupBox groupBox1_Acquisition;
        private System.Windows.Forms.Button button_Freeze;
        private System.Windows.Forms.Button button_Grab;
        private System.Windows.Forms.Button button_Snap;

        private SapAcquisition      m_Acquisition;
        private SapBuffer           m_Buffers;
        private SapAcqToBuf         m_Xfer;
        private SapView             m_View;
        private bool m_IsSignalDetected;
        private bool m_online;
        private SapLocation m_ServerLocation;
        private string m_ConfigFileName;


        //System menu
        private SystemMenu m_SystemMenu;
        //index for "about this.." item im system menu
        private const int m_AboutID = 0x100;
       private ImageBox m_ImageBox;
        


    }
}

