
using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.SapClassGui;


namespace DALSA.SaperaLT.Demos.NET.CSharp.GigECameraDemo
{
    partial class GigECameraDemoDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GigECameraDemoDlg));
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
            this.button_View = new System.Windows.Forms.Button();
            this.button_Buffer = new System.Windows.Forms.Button();
            this.groupBox_Acquisition_Options = new System.Windows.Forms.GroupBox();
            this.button_Load_Config = new System.Windows.Forms.Button();
            this.groupBox_Acquisition_Control = new System.Windows.Forms.GroupBox();
            this.button_Freeze = new System.Windows.Forms.Button();
            this.button_Grab = new System.Windows.Forms.Button();
            this.button_Snap = new System.Windows.Forms.Button();
            this.button_Exit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1_Logo)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox_File_Control.SuspendLayout();
            this.groupBox_General_Options.SuspendLayout();
            this.groupBox_Acquisition_Options.SuspendLayout();
            this.groupBox_Acquisition_Control.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1_Logo
            // 
            this.pictureBox1_Logo.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1_Logo.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1_Logo.Image")));
            this.pictureBox1_Logo.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1_Logo.Name = "pictureBox1_Logo";
            this.pictureBox1_Logo.Size = new System.Drawing.Size(60, 531);
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
            this.toolStrip1.Location = new System.Drawing.Point(60, 506);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(727, 25);
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
            this.PixelLabel.Name = "PixelLabel";
            this.PixelLabel.Size = new System.Drawing.Size(36, 22);
            this.PixelLabel.Text = "Pixel :";
            // 
            // PixelDataValue
            // 
            this.PixelDataValue.Name = "PixelDataValue";
            this.PixelDataValue.Size = new System.Drawing.Size(86, 22);
            this.PixelDataValue.Text = "Data not avaible";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox_File_Control);
            this.panel1.Controls.Add(this.groupBox_General_Options);
            this.panel1.Controls.Add(this.groupBox_Acquisition_Options);
            this.panel1.Controls.Add(this.groupBox_Acquisition_Control);
            this.panel1.Controls.Add(this.button_Exit);
            this.panel1.Location = new System.Drawing.Point(66, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(169, 503);
            this.panel1.TabIndex = 11;
            // 
            // groupBox_File_Control
            // 
            this.groupBox_File_Control.Controls.Add(this.button_Save);
            this.groupBox_File_Control.Controls.Add(this.button_Load);
            this.groupBox_File_Control.Controls.Add(this.button_New);
            this.groupBox_File_Control.Location = new System.Drawing.Point(17, 203);
            this.groupBox_File_Control.Name = "groupBox_File_Control";
            this.groupBox_File_Control.Size = new System.Drawing.Size(142, 132);
            this.groupBox_File_Control.TabIndex = 11;
            this.groupBox_File_Control.TabStop = false;
            this.groupBox_File_Control.Text = "File Control";
            // 
            // button_Save
            // 
            this.button_Save.Location = new System.Drawing.Point(22, 97);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(104, 23);
            this.button_Save.TabIndex = 2;
            this.button_Save.Text = "Save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // button_Load
            // 
            this.button_Load.Location = new System.Drawing.Point(22, 60);
            this.button_Load.Name = "button_Load";
            this.button_Load.Size = new System.Drawing.Size(104, 23);
            this.button_Load.TabIndex = 1;
            this.button_Load.Text = "Load";
            this.button_Load.UseVisualStyleBackColor = true;
            this.button_Load.Click += new System.EventHandler(this.button_Load_Click);
            // 
            // button_New
            // 
            this.button_New.Location = new System.Drawing.Point(22, 24);
            this.button_New.Name = "button_New";
            this.button_New.Size = new System.Drawing.Size(104, 23);
            this.button_New.TabIndex = 0;
            this.button_New.Text = "New";
            this.button_New.UseVisualStyleBackColor = true;
            this.button_New.Click += new System.EventHandler(this.button_New_Click);
            // 
            // groupBox_General_Options
            // 
            this.groupBox_General_Options.Controls.Add(this.button_View);
            this.groupBox_General_Options.Controls.Add(this.button_Buffer);
            this.groupBox_General_Options.Location = new System.Drawing.Point(17, 341);
            this.groupBox_General_Options.Name = "groupBox_General_Options";
            this.groupBox_General_Options.Size = new System.Drawing.Size(142, 88);
            this.groupBox_General_Options.TabIndex = 12;
            this.groupBox_General_Options.TabStop = false;
            this.groupBox_General_Options.Text = "General Options";
            // 
            // button_View
            // 
            this.button_View.Location = new System.Drawing.Point(22, 53);
            this.button_View.Name = "button_View";
            this.button_View.Size = new System.Drawing.Size(104, 23);
            this.button_View.TabIndex = 1;
            this.button_View.Text = "View";
            this.button_View.UseVisualStyleBackColor = true;
            this.button_View.Click += new System.EventHandler(this.button_View_Click);
            // 
            // button_Buffer
            // 
            this.button_Buffer.Location = new System.Drawing.Point(22, 20);
            this.button_Buffer.Name = "button_Buffer";
            this.button_Buffer.Size = new System.Drawing.Size(104, 23);
            this.button_Buffer.TabIndex = 0;
            this.button_Buffer.Text = "Buffer";
            this.button_Buffer.UseVisualStyleBackColor = true;
            this.button_Buffer.Click += new System.EventHandler(this.button_Buffer_Click);
            // 
            // groupBox_Acquisition_Options
            // 
            this.groupBox_Acquisition_Options.Controls.Add(this.button_Load_Config);
            this.groupBox_Acquisition_Options.Location = new System.Drawing.Point(17, 435);
            this.groupBox_Acquisition_Options.Name = "groupBox_Acquisition_Options";
            this.groupBox_Acquisition_Options.Size = new System.Drawing.Size(142, 54);
            this.groupBox_Acquisition_Options.TabIndex = 13;
            this.groupBox_Acquisition_Options.TabStop = false;
            this.groupBox_Acquisition_Options.Text = "Acquisition Options";
            // 
            // button_Load_Config
            // 
            this.button_Load_Config.Location = new System.Drawing.Point(22, 20);
            this.button_Load_Config.Name = "button_Load_Config";
            this.button_Load_Config.Size = new System.Drawing.Size(104, 23);
            this.button_Load_Config.TabIndex = 0;
            this.button_Load_Config.Text = "Load Config";
            this.button_Load_Config.UseVisualStyleBackColor = true;
            this.button_Load_Config.Click += new System.EventHandler(this.button_Load_Config_Click);
            // 
            // groupBox_Acquisition_Control
            // 
            this.groupBox_Acquisition_Control.Controls.Add(this.button_Freeze);
            this.groupBox_Acquisition_Control.Controls.Add(this.button_Grab);
            this.groupBox_Acquisition_Control.Controls.Add(this.button_Snap);
            this.groupBox_Acquisition_Control.Location = new System.Drawing.Point(17, 53);
            this.groupBox_Acquisition_Control.Name = "groupBox_Acquisition_Control";
            this.groupBox_Acquisition_Control.Size = new System.Drawing.Size(142, 144);
            this.groupBox_Acquisition_Control.TabIndex = 9;
            this.groupBox_Acquisition_Control.TabStop = false;
            this.groupBox_Acquisition_Control.Text = "Acquisition Control";
            // 
            // button_Freeze
            // 
            this.button_Freeze.Location = new System.Drawing.Point(22, 104);
            this.button_Freeze.Name = "button_Freeze";
            this.button_Freeze.Size = new System.Drawing.Size(104, 23);
            this.button_Freeze.TabIndex = 2;
            this.button_Freeze.Text = "Freeze";
            this.button_Freeze.UseVisualStyleBackColor = true;
            this.button_Freeze.Click += new System.EventHandler(this.button_Freeze_Click);
            // 
            // button_Grab
            // 
            this.button_Grab.Location = new System.Drawing.Point(22, 62);
            this.button_Grab.Name = "button_Grab";
            this.button_Grab.Size = new System.Drawing.Size(104, 23);
            this.button_Grab.TabIndex = 1;
            this.button_Grab.Text = "Grab";
            this.button_Grab.UseVisualStyleBackColor = true;
            this.button_Grab.Click += new System.EventHandler(this.button_Grab_Click);
            // 
            // button_Snap
            // 
            this.button_Snap.Location = new System.Drawing.Point(22, 23);
            this.button_Snap.Name = "button_Snap";
            this.button_Snap.Size = new System.Drawing.Size(104, 23);
            this.button_Snap.TabIndex = 0;
            this.button_Snap.Text = "Snap";
            this.button_Snap.UseVisualStyleBackColor = true;
            this.button_Snap.Click += new System.EventHandler(this.button_Snap_Click);
            // 
            // button_Exit
            // 
            this.button_Exit.ForeColor = System.Drawing.Color.Black;
            this.button_Exit.Location = new System.Drawing.Point(39, 11);
            this.button_Exit.Name = "button_Exit";
            this.button_Exit.Size = new System.Drawing.Size(104, 35);
            this.button_Exit.TabIndex = 10;
            this.button_Exit.Text = "Exit";
            this.button_Exit.UseVisualStyleBackColor = true;
            this.button_Exit.Click += new System.EventHandler(this.button_Exit_Click);
            // 
            // GigECameraDemoDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 531);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.pictureBox1_Logo);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GigECameraDemoDlg";
            this.Text = "GigE-Vision Camera Demo .NET";
            this.Load += new System.EventHandler(this.GigECameraDemoDlg_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GigECameraDemoDlg_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1_Logo)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox_File_Control.ResumeLayout(false);
            this.groupBox_General_Options.ResumeLayout(false);
            this.groupBox_Acquisition_Options.ResumeLayout(false);
            this.groupBox_Acquisition_Control.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox groupBox_Acquisition_Options;
        private System.Windows.Forms.Button button_Load_Config;
        private System.Windows.Forms.GroupBox groupBox_Acquisition_Control;
        private System.Windows.Forms.Button button_Freeze;
        private System.Windows.Forms.Button button_Grab;
        private System.Windows.Forms.Button button_Snap;
        private System.Windows.Forms.Button button_Exit;
        private ImageBox m_ImageBox;

        private SapAcqDevice m_AcqDevice;
        private SapBuffer m_Buffers;
        private SapAcqDeviceToBuf m_Xfer;
        private SapView m_View;
        private SapLocation m_ServerLocation;
        private string m_ConfigFileName;

        //System menu
        private SystemMenu m_SystemMenu;
        //index for "about this.." item im system menu
        private const int m_AboutID = 0x100;
      

  
       

    }
}

