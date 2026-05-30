namespace DALSA.SaperaLT.Demos.NET.CSharp.GigEFlatFieldDemo
{
    partial class GigEFlatFieldDemoDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GigEFlatFieldDemoDlg));
			this.button_Load_FF = new System.Windows.Forms.Button();
			this.button_View = new System.Windows.Forms.Button();
			this.button_LoadConfig = new System.Windows.Forms.Button();
			this.button_Snap = new System.Windows.Forms.Button();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.button_Freeze = new System.Windows.Forms.Button();
			this.button_Grab = new System.Windows.Forms.Button();
			this.button_Buffers = new System.Windows.Forms.Button();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.StatusLabel = new System.Windows.Forms.ToolStripLabel();
			this.StatusLabelInfo = new System.Windows.Forms.ToolStripLabel();
			this.FrameLostCount = new System.Windows.Forms.ToolStripLabel();
			this.StatusLabelInfoTrash = new System.Windows.Forms.ToolStripLabel();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.PixelLabel = new System.Windows.Forms.ToolStripLabel();
			this.PixelDataValue = new System.Windows.Forms.ToolStripLabel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.button_Save_FF = new System.Windows.Forms.Button();
			this.button_Calibrate = new System.Windows.Forms.Button();
			this.checkBox_Pixels_Replacement = new System.Windows.Forms.CheckBox();
			this.checkBox_Hardware_Correction = new System.Windows.Forms.CheckBox();
			this.checkBox_FlatField = new System.Windows.Forms.CheckBox();
			this.button_Exit = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.button_Save = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.button_New = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.button_Load = new System.Windows.Forms.Button();
			this.checkBox_Use_ROI = new System.Windows.Forms.CheckBox();
			this.groupBox5.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Load_FF
			// 
			this.button_Load_FF.Location = new System.Drawing.Point(24, 70);
			this.button_Load_FF.Name = "button_Load_FF";
			this.button_Load_FF.Size = new System.Drawing.Size(104, 23);
			this.button_Load_FF.TabIndex = 4;
			this.button_Load_FF.Text = "Load";
			this.button_Load_FF.UseVisualStyleBackColor = true;
			this.button_Load_FF.Click += new System.EventHandler(this.button_Load_FF_Click);
			// 
			// button_View
			// 
			this.button_View.Location = new System.Drawing.Point(34, 57);
			this.button_View.Name = "button_View";
			this.button_View.Size = new System.Drawing.Size(104, 23);
			this.button_View.TabIndex = 2;
			this.button_View.Text = "View";
			this.button_View.UseVisualStyleBackColor = true;
			this.button_View.Click += new System.EventHandler(this.button_View_Click);
			// 
			// button_LoadConfig
			// 
			this.button_LoadConfig.Location = new System.Drawing.Point(23, 19);
			this.button_LoadConfig.Name = "button_LoadConfig";
			this.button_LoadConfig.Size = new System.Drawing.Size(104, 23);
			this.button_LoadConfig.TabIndex = 0;
			this.button_LoadConfig.Text = "LoadConfig";
			this.button_LoadConfig.UseVisualStyleBackColor = true;
			this.button_LoadConfig.Click += new System.EventHandler(this.button_LoadConfig_Click);
			// 
			// button_Snap
			// 
			this.button_Snap.Location = new System.Drawing.Point(23, 19);
			this.button_Snap.Name = "button_Snap";
			this.button_Snap.Size = new System.Drawing.Size(104, 23);
			this.button_Snap.TabIndex = 1;
			this.button_Snap.Text = "Snap";
			this.button_Snap.UseVisualStyleBackColor = true;
			this.button_Snap.Click += new System.EventHandler(this.button_Snap_Click);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.button_Freeze);
			this.groupBox5.Controls.Add(this.button_Grab);
			this.groupBox5.Controls.Add(this.button_Snap);
			this.groupBox5.Location = new System.Drawing.Point(8, 126);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(163, 130);
			this.groupBox5.TabIndex = 6;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Acquisitioin Control";
			// 
			// button_Freeze
			// 
			this.button_Freeze.Location = new System.Drawing.Point(23, 94);
			this.button_Freeze.Name = "button_Freeze";
			this.button_Freeze.Size = new System.Drawing.Size(104, 23);
			this.button_Freeze.TabIndex = 3;
			this.button_Freeze.Text = "Freeze";
			this.button_Freeze.UseVisualStyleBackColor = true;
			this.button_Freeze.Click += new System.EventHandler(this.button_Freeze_Click);
			// 
			// button_Grab
			// 
			this.button_Grab.Location = new System.Drawing.Point(23, 56);
			this.button_Grab.Name = "button_Grab";
			this.button_Grab.Size = new System.Drawing.Size(104, 23);
			this.button_Grab.TabIndex = 2;
			this.button_Grab.Text = "Grab";
			this.button_Grab.UseVisualStyleBackColor = true;
			this.button_Grab.Click += new System.EventHandler(this.button_Grab_Click);
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
			// toolStrip1
			// 
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel,
            this.StatusLabelInfo,
            this.FrameLostCount,
            this.StatusLabelInfoTrash,
            this.toolStripSeparator1,
            this.PixelLabel,
            this.PixelDataValue});
			this.toolStrip1.Location = new System.Drawing.Point(64, 551);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip1.Size = new System.Drawing.Size(752, 25);
			this.toolStrip1.TabIndex = 36;
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
			this.StatusLabelInfo.Size = new System.Drawing.Size(0, 22);
			// 
			// FrameLostCount
			// 
			this.FrameLostCount.Name = "FrameLostCount";
			this.FrameLostCount.Size = new System.Drawing.Size(0, 22);
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
			this.panel1.Controls.Add(this.groupBox5);
			this.panel1.Controls.Add(this.groupBox4);
			this.panel1.Controls.Add(this.button_Exit);
			this.panel1.Controls.Add(this.groupBox3);
			this.panel1.Controls.Add(this.groupBox2);
			this.panel1.Location = new System.Drawing.Point(70, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(183, 557);
			this.panel1.TabIndex = 31;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.checkBox_Use_ROI);
			this.groupBox4.Controls.Add(this.button_Save_FF);
			this.groupBox4.Controls.Add(this.button_Load_FF);
			this.groupBox4.Controls.Add(this.button_Calibrate);
			this.groupBox4.Controls.Add(this.checkBox_Pixels_Replacement);
			this.groupBox4.Controls.Add(this.checkBox_Hardware_Correction);
			this.groupBox4.Controls.Add(this.checkBox_FlatField);
			this.groupBox4.Location = new System.Drawing.Point(7, 262);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(164, 188);
			this.groupBox4.TabIndex = 5;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Flat Field Correction";
			// 
			// button_Save_FF
			// 
			this.button_Save_FF.Location = new System.Drawing.Point(24, 98);
			this.button_Save_FF.Name = "button_Save_FF";
			this.button_Save_FF.Size = new System.Drawing.Size(104, 23);
			this.button_Save_FF.TabIndex = 5;
			this.button_Save_FF.Text = "Save";
			this.button_Save_FF.UseVisualStyleBackColor = true;
			this.button_Save_FF.Click += new System.EventHandler(this.button_Save_FF_Click);
			// 
			// button_Calibrate
			// 
			this.button_Calibrate.Location = new System.Drawing.Point(24, 42);
			this.button_Calibrate.Name = "button_Calibrate";
			this.button_Calibrate.Size = new System.Drawing.Size(104, 23);
			this.button_Calibrate.TabIndex = 3;
			this.button_Calibrate.Text = "Calibrate";
			this.button_Calibrate.UseVisualStyleBackColor = true;
			this.button_Calibrate.Click += new System.EventHandler(this.button_Calibrate_Click);
			// 
			// checkBox_Pixels_Replacement
			// 
			this.checkBox_Pixels_Replacement.AutoSize = true;
			this.checkBox_Pixels_Replacement.Location = new System.Drawing.Point(18, 165);
			this.checkBox_Pixels_Replacement.Name = "checkBox_Pixels_Replacement";
			this.checkBox_Pixels_Replacement.Size = new System.Drawing.Size(114, 17);
			this.checkBox_Pixels_Replacement.TabIndex = 2;
			this.checkBox_Pixels_Replacement.Text = "Pixel Replacement";
			this.checkBox_Pixels_Replacement.UseVisualStyleBackColor = true;
			this.checkBox_Pixels_Replacement.CheckedChanged += new System.EventHandler(this.checkBox_Pixels_Replacement_CheckedChanged);
			// 
			// checkBox_Hardware_Correction
			// 
			this.checkBox_Hardware_Correction.AutoSize = true;
			this.checkBox_Hardware_Correction.Location = new System.Drawing.Point(18, 146);
			this.checkBox_Hardware_Correction.Name = "checkBox_Hardware_Correction";
			this.checkBox_Hardware_Correction.Size = new System.Drawing.Size(123, 17);
			this.checkBox_Hardware_Correction.TabIndex = 1;
			this.checkBox_Hardware_Correction.Text = "Hardware Correction";
			this.checkBox_Hardware_Correction.UseVisualStyleBackColor = true;
			this.checkBox_Hardware_Correction.CheckedChanged += new System.EventHandler(this.checkBox_Hardware_Correction_CheckedChanged);
			// 
			// checkBox_FlatField
			// 
			this.checkBox_FlatField.AutoSize = true;
			this.checkBox_FlatField.Location = new System.Drawing.Point(18, 19);
			this.checkBox_FlatField.Name = "checkBox_FlatField";
			this.checkBox_FlatField.Size = new System.Drawing.Size(59, 17);
			this.checkBox_FlatField.TabIndex = 0;
			this.checkBox_FlatField.Text = "Enable";
			this.checkBox_FlatField.UseVisualStyleBackColor = true;
			this.checkBox_FlatField.CheckedChanged += new System.EventHandler(this.checkBox_FlatField_CheckedChanged);
			// 
			// button_Exit
			// 
			this.button_Exit.Location = new System.Drawing.Point(32, 13);
			this.button_Exit.Name = "button_Exit";
			this.button_Exit.Size = new System.Drawing.Size(104, 42);
			this.button_Exit.TabIndex = 4;
			this.button_Exit.Text = "Exit";
			this.button_Exit.UseVisualStyleBackColor = true;
			this.button_Exit.Click += new System.EventHandler(this.button_Exit_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.button_View);
			this.groupBox3.Controls.Add(this.button_Buffers);
			this.groupBox3.Location = new System.Drawing.Point(6, 456);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(165, 94);
			this.groupBox3.TabIndex = 1;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "General Options";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.button_LoadConfig);
			this.groupBox2.Location = new System.Drawing.Point(8, 74);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(166, 51);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Acquisition Option";
			// 
			// button_Save
			// 
			this.button_Save.Location = new System.Drawing.Point(375, 23);
			this.button_Save.Name = "button_Save";
			this.button_Save.Size = new System.Drawing.Size(110, 23);
			this.button_Save.TabIndex = 2;
			this.button_Save.Text = "Save";
			this.button_Save.UseVisualStyleBackColor = true;
			this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(64, 576);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 30;
			this.pictureBox1.TabStop = false;
			// 
			// button_New
			// 
			this.button_New.Location = new System.Drawing.Point(35, 23);
			this.button_New.Name = "button_New";
			this.button_New.Size = new System.Drawing.Size(110, 23);
			this.button_New.TabIndex = 0;
			this.button_New.Text = "New";
			this.button_New.UseVisualStyleBackColor = true;
			this.button_New.Click += new System.EventHandler(this.button_New_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.button_Save);
			this.groupBox1.Controls.Add(this.button_New);
			this.groupBox1.Controls.Add(this.button_Load);
			this.groupBox1.Location = new System.Drawing.Point(259, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(545, 64);
			this.groupBox1.TabIndex = 35;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "File Options";
			// 
			// button_Load
			// 
			this.button_Load.Location = new System.Drawing.Point(211, 23);
			this.button_Load.Name = "button_Load";
			this.button_Load.Size = new System.Drawing.Size(110, 23);
			this.button_Load.TabIndex = 1;
			this.button_Load.Text = "Load ";
			this.button_Load.UseVisualStyleBackColor = true;
			this.button_Load.Click += new System.EventHandler(this.button_Load_Click);
			// 
			// checkBox_Use_ROI
			// 
			this.checkBox_Use_ROI.AutoSize = true;
			this.checkBox_Use_ROI.Location = new System.Drawing.Point(18, 126);
			this.checkBox_Use_ROI.Name = "checkBox_Use_ROI";
			this.checkBox_Use_ROI.Size = new System.Drawing.Size(76, 17);
			this.checkBox_Use_ROI.TabIndex = 6;
			this.checkBox_Use_ROI.Text = "Use a ROI";
			this.checkBox_Use_ROI.UseVisualStyleBackColor = true;
			this.checkBox_Use_ROI.CheckedChanged += new System.EventHandler(this.checkBox_Use_ROI_CheckedChanged);
			// 
			// GigEFlatFieldDemoDlg
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(816, 576);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.groupBox1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "GigEFlatFieldDemoDlg";
			this.Text = "GigEFlatFieldDemo .NET";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GigEFlatFieldDemoDlg_FormClosed);
			this.Load += new System.EventHandler(this.GigEFlatFieldDemoDlg_Load);
			this.groupBox5.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Load_FF;
        private System.Windows.Forms.Button button_View;
        private System.Windows.Forms.Button button_LoadConfig;
        private System.Windows.Forms.Button button_Snap;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button button_Freeze;
        private System.Windows.Forms.Button button_Grab;
        private System.Windows.Forms.Button button_Buffers;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel StatusLabelInfo;
        private System.Windows.Forms.ToolStripLabel FrameLostCount;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button_Save_FF;
        private System.Windows.Forms.Button button_Calibrate;
        private System.Windows.Forms.CheckBox checkBox_Pixels_Replacement;
        private System.Windows.Forms.CheckBox checkBox_Hardware_Correction;
        private System.Windows.Forms.CheckBox checkBox_FlatField;
        private System.Windows.Forms.Button button_Exit;
        private System.Windows.Forms.GroupBox groupBox3;
       private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.PictureBox pictureBox1;
       private System.Windows.Forms.Button button_New;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_Load;
       private System.Windows.Forms.ToolStripLabel StatusLabel;
       private System.Windows.Forms.ToolStripLabel StatusLabelInfoTrash;
       private System.Windows.Forms.ToolStripLabel PixelLabel;
       private System.Windows.Forms.ToolStripLabel PixelDataValue;
       private DALSA.SaperaLT.SapClassGui.ImageBox m_ImageBox;
	   private System.Windows.Forms.CheckBox checkBox_Use_ROI;
    }
}

