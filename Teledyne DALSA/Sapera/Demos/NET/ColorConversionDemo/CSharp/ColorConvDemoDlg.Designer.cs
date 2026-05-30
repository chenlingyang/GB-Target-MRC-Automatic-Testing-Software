namespace DALSA.SaperaLT.Demos.NET.CSharp.ColorConvDemo
{
    partial class ColorConvDemoDlg
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorConvDemoDlg));
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.button_Freeze = new System.Windows.Forms.Button();
         this.button_Grab = new System.Windows.Forms.Button();
         this.button_Snap = new System.Windows.Forms.Button();
         this.button_LoadConfig = new System.Windows.Forms.Button();
         this.groupBox3 = new System.Windows.Forms.GroupBox();
         this.button_View = new System.Windows.Forms.Button();
         this.button_Buffers = new System.Windows.Forms.Button();
         this.button_Exit = new System.Windows.Forms.Button();
         this.panel1 = new System.Windows.Forms.Panel();
         this.groupBox4 = new System.Windows.Forms.GroupBox();
         this.label1 = new System.Windows.Forms.Label();
         this.comboBox_Output_Conversion = new System.Windows.Forms.ComboBox();
         this.button_Options = new System.Windows.Forms.Button();
         this.button_New = new System.Windows.Forms.Button();
         this.button_Load_Raw = new System.Windows.Forms.Button();
         this.button_Save_Raw = new System.Windows.Forms.Button();
         this.button_Save_Converted = new System.Windows.Forms.Button();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.pictureBox1 = new System.Windows.Forms.PictureBox();
         this.StatusLabelInfo = new System.Windows.Forms.ToolStripLabel();
         this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
         this.FrameLostCount = new System.Windows.Forms.ToolStripLabel();
         this.toolStrip1 = new System.Windows.Forms.ToolStrip();
         this.StatusLabel = new System.Windows.Forms.ToolStripLabel();
         this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
         this.StatusLabelInfoTrash = new System.Windows.Forms.ToolStripLabel();
         this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
         this.PixelLabel = new System.Windows.Forms.ToolStripLabel();
         this.PixelDataValue = new System.Windows.Forms.ToolStripLabel();
         this.radio_disable = new System.Windows.Forms.RadioButton();
         this.radio_hw = new System.Windows.Forms.RadioButton();
         this.radio_sw = new System.Windows.Forms.RadioButton();
         this.groupBox2.SuspendLayout();
         this.groupBox3.SuspendLayout();
         this.panel1.SuspendLayout();
         this.groupBox4.SuspendLayout();
         this.groupBox1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
         this.toolStrip1.SuspendLayout();
         this.SuspendLayout();
         // 
         // groupBox2
         // 
         this.groupBox2.Controls.Add(this.button_Freeze);
         this.groupBox2.Controls.Add(this.button_Grab);
         this.groupBox2.Controls.Add(this.button_Snap);
         this.groupBox2.Controls.Add(this.button_LoadConfig);
         this.groupBox2.Location = new System.Drawing.Point(5, 69);
         this.groupBox2.Name = "groupBox2";
         this.groupBox2.Size = new System.Drawing.Size(166, 135);
         this.groupBox2.TabIndex = 0;
         this.groupBox2.TabStop = false;
         this.groupBox2.Text = "Acquisition control";
         // 
         // button_Freeze
         // 
         this.button_Freeze.Location = new System.Drawing.Point(20, 97);
         this.button_Freeze.Name = "button_Freeze";
         this.button_Freeze.Size = new System.Drawing.Size(104, 23);
         this.button_Freeze.TabIndex = 3;
         this.button_Freeze.Text = "Freeze";
         this.button_Freeze.UseVisualStyleBackColor = true;
         this.button_Freeze.Click += new System.EventHandler(this.button_Freeze_Click);
         // 
         // button_Grab
         // 
         this.button_Grab.Location = new System.Drawing.Point(20, 71);
         this.button_Grab.Name = "button_Grab";
         this.button_Grab.Size = new System.Drawing.Size(104, 23);
         this.button_Grab.TabIndex = 2;
         this.button_Grab.Text = "Grab";
         this.button_Grab.UseVisualStyleBackColor = true;
         this.button_Grab.Click += new System.EventHandler(this.button_Grab_Click);
         // 
         // button_Snap
         // 
         this.button_Snap.Location = new System.Drawing.Point(20, 45);
         this.button_Snap.Name = "button_Snap";
         this.button_Snap.Size = new System.Drawing.Size(104, 23);
         this.button_Snap.TabIndex = 1;
         this.button_Snap.Text = "Snap";
         this.button_Snap.UseVisualStyleBackColor = true;
         this.button_Snap.Click += new System.EventHandler(this.button_Snap_Click);
         // 
         // button_LoadConfig
         // 
         this.button_LoadConfig.Location = new System.Drawing.Point(20, 19);
         this.button_LoadConfig.Name = "button_LoadConfig";
         this.button_LoadConfig.Size = new System.Drawing.Size(104, 23);
         this.button_LoadConfig.TabIndex = 0;
         this.button_LoadConfig.Text = "LoadConfig";
         this.button_LoadConfig.UseVisualStyleBackColor = true;
         this.button_LoadConfig.Click += new System.EventHandler(this.button_LoadConfig_Click);
         // 
         // groupBox3
         // 
         this.groupBox3.Controls.Add(this.button_View);
         this.groupBox3.Controls.Add(this.button_Buffers);
         this.groupBox3.Location = new System.Drawing.Point(6, 421);
         this.groupBox3.Name = "groupBox3";
         this.groupBox3.Size = new System.Drawing.Size(165, 94);
         this.groupBox3.TabIndex = 1;
         this.groupBox3.TabStop = false;
         this.groupBox3.Text = "General Options";
         // 
         // button_View
         // 
         this.button_View.Location = new System.Drawing.Point(19, 57);
         this.button_View.Name = "button_View";
         this.button_View.Size = new System.Drawing.Size(104, 23);
         this.button_View.TabIndex = 2;
         this.button_View.Text = "View";
         this.button_View.UseVisualStyleBackColor = true;
         this.button_View.Click += new System.EventHandler(this.button_View_Click);
         // 
         // button_Buffers
         // 
         this.button_Buffers.Location = new System.Drawing.Point(19, 28);
         this.button_Buffers.Name = "button_Buffers";
         this.button_Buffers.Size = new System.Drawing.Size(104, 23);
         this.button_Buffers.TabIndex = 0;
         this.button_Buffers.Text = "Buffers";
         this.button_Buffers.UseVisualStyleBackColor = true;
         this.button_Buffers.Click += new System.EventHandler(this.button_Buffers_Click);
         // 
         // button_Exit
         // 
         this.button_Exit.Location = new System.Drawing.Point(40, 13);
         this.button_Exit.Name = "button_Exit";
         this.button_Exit.Size = new System.Drawing.Size(104, 42);
         this.button_Exit.TabIndex = 4;
         this.button_Exit.Text = "Exit";
         this.button_Exit.UseVisualStyleBackColor = true;
         this.button_Exit.Click += new System.EventHandler(this.button_Exit_Click);
         // 
         // panel1
         // 
         this.panel1.Controls.Add(this.groupBox4);
         this.panel1.Controls.Add(this.button_Exit);
         this.panel1.Controls.Add(this.groupBox3);
         this.panel1.Controls.Add(this.groupBox2);
         this.panel1.Location = new System.Drawing.Point(64, 0);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(183, 545);
         this.panel1.TabIndex = 16;
         // 
         // groupBox4
         // 
         this.groupBox4.Controls.Add(this.radio_sw);
         this.groupBox4.Controls.Add(this.radio_hw);
         this.groupBox4.Controls.Add(this.radio_disable);
         this.groupBox4.Controls.Add(this.label1);
         this.groupBox4.Controls.Add(this.comboBox_Output_Conversion);
         this.groupBox4.Controls.Add(this.button_Options);
         this.groupBox4.Location = new System.Drawing.Point(6, 210);
         this.groupBox4.Name = "groupBox4";
         this.groupBox4.Size = new System.Drawing.Size(165, 205);
         this.groupBox4.TabIndex = 23;
         this.groupBox4.TabStop = false;
         this.groupBox4.Text = "Color Conversion";
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(16, 149);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(74, 13);
         this.label1.TabIndex = 3;
         this.label1.Text = "Output Format";
         // 
         // comboBox_Output_Conversion
         // 
         this.comboBox_Output_Conversion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.comboBox_Output_Conversion.FormattingEnabled = true;
         this.comboBox_Output_Conversion.Location = new System.Drawing.Point(19, 165);
         this.comboBox_Output_Conversion.Name = "comboBox_Output_Conversion";
         this.comboBox_Output_Conversion.Size = new System.Drawing.Size(121, 21);
         this.comboBox_Output_Conversion.TabIndex = 2;
         this.comboBox_Output_Conversion.SelectedIndexChanged += new System.EventHandler(this.comboBox_Output_Conversion_SelectedIndexChanged);
         // 
         // button_Options
         // 
         this.button_Options.Location = new System.Drawing.Point(19, 108);
         this.button_Options.Name = "button_Options";
         this.button_Options.Size = new System.Drawing.Size(83, 24);
         this.button_Options.TabIndex = 1;
         this.button_Options.Text = "Options";
         this.button_Options.UseVisualStyleBackColor = true;
         this.button_Options.Click += new System.EventHandler(this.button_Options_Click);
         // 
         // button_New
         // 
         this.button_New.Location = new System.Drawing.Point(19, 27);
         this.button_New.Name = "button_New";
         this.button_New.Size = new System.Drawing.Size(110, 23);
         this.button_New.TabIndex = 0;
         this.button_New.Text = "New";
         this.button_New.UseVisualStyleBackColor = true;
         this.button_New.Click += new System.EventHandler(this.button_New_Click);
         // 
         // button_Load_Raw
         // 
         this.button_Load_Raw.Location = new System.Drawing.Point(146, 27);
         this.button_Load_Raw.Name = "button_Load_Raw";
         this.button_Load_Raw.Size = new System.Drawing.Size(110, 23);
         this.button_Load_Raw.TabIndex = 1;
         this.button_Load_Raw.Text = "Load Raw";
         this.button_Load_Raw.UseVisualStyleBackColor = true;
         this.button_Load_Raw.Click += new System.EventHandler(this.button_Load_Raw_Click);
         // 
         // button_Save_Raw
         // 
         this.button_Save_Raw.Location = new System.Drawing.Point(278, 27);
         this.button_Save_Raw.Name = "button_Save_Raw";
         this.button_Save_Raw.Size = new System.Drawing.Size(110, 23);
         this.button_Save_Raw.TabIndex = 2;
         this.button_Save_Raw.Text = "Save Raw";
         this.button_Save_Raw.UseVisualStyleBackColor = true;
         this.button_Save_Raw.Click += new System.EventHandler(this.button_Save_Raw_Click);
         // 
         // button_Save_Converted
         // 
         this.button_Save_Converted.Location = new System.Drawing.Point(409, 27);
         this.button_Save_Converted.Name = "button_Save_Converted";
         this.button_Save_Converted.Size = new System.Drawing.Size(110, 23);
         this.button_Save_Converted.TabIndex = 3;
         this.button_Save_Converted.Text = "Save Converted";
         this.button_Save_Converted.UseVisualStyleBackColor = true;
         this.button_Save_Converted.Click += new System.EventHandler(this.button_Save_Converted_Click);
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.button_Save_Converted);
         this.groupBox1.Controls.Add(this.button_Save_Raw);
         this.groupBox1.Controls.Add(this.button_Load_Raw);
         this.groupBox1.Controls.Add(this.button_New);
         this.groupBox1.Location = new System.Drawing.Point(253, 0);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(545, 64);
         this.groupBox1.TabIndex = 21;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "File Options";
         // 
         // pictureBox1
         // 
         this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
         this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
         this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
         this.pictureBox1.Location = new System.Drawing.Point(0, 0);
         this.pictureBox1.Name = "pictureBox1";
         this.pictureBox1.Size = new System.Drawing.Size(64, 545);
         this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
         this.pictureBox1.TabIndex = 15;
         this.pictureBox1.TabStop = false;
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
         // toolStrip1
         // 
         this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabelInfo,
            this.StatusLabel,
            this.toolStripLabel1,
            this.StatusLabelInfoTrash,
            this.toolStripSeparator2,
            this.PixelLabel,
            this.PixelDataValue,
            this.toolStripSeparator1,
            this.FrameLostCount});
         this.toolStrip1.Location = new System.Drawing.Point(64, 520);
         this.toolStrip1.Name = "toolStrip1";
         this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
         this.toolStrip1.Size = new System.Drawing.Size(752, 25);
         this.toolStrip1.TabIndex = 22;
         this.toolStrip1.Text = "toolStrip1";
         // 
         // StatusLabel
         // 
         this.StatusLabel.Name = "StatusLabel";
         this.StatusLabel.Size = new System.Drawing.Size(45, 22);
         this.StatusLabel.Text = "Status :";
         // 
         // toolStripLabel1
         // 
         this.toolStripLabel1.Name = "toolStripLabel1";
         this.toolStripLabel1.Size = new System.Drawing.Size(0, 22);
         // 
         // StatusLabelInfoTrash
         // 
         this.StatusLabelInfoTrash.Name = "StatusLabelInfoTrash";
         this.StatusLabelInfoTrash.Size = new System.Drawing.Size(0, 22);
         // 
         // toolStripSeparator2
         // 
         this.toolStripSeparator2.Name = "toolStripSeparator2";
         this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
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
         // radio_disable
         // 
         this.radio_disable.AutoSize = true;
         this.radio_disable.Location = new System.Drawing.Point(19, 30);
         this.radio_disable.Name = "radio_disable";
         this.radio_disable.Size = new System.Drawing.Size(60, 17);
         this.radio_disable.TabIndex = 4;
         this.radio_disable.TabStop = true;
         this.radio_disable.Text = "Disable";
         this.radio_disable.UseVisualStyleBackColor = true;
         this.radio_disable.CheckedChanged += new System.EventHandler(this.radio_disable_CheckedChanged);
         // 
         // radio_hw
         // 
         this.radio_hw.AutoSize = true;
         this.radio_hw.Location = new System.Drawing.Point(19, 53);
         this.radio_hw.Name = "radio_hw";
         this.radio_hw.Size = new System.Drawing.Size(107, 17);
         this.radio_hw.TabIndex = 5;
         this.radio_hw.TabStop = true;
         this.radio_hw.Text = "Enable Hardware";
         this.radio_hw.UseVisualStyleBackColor = true;
         this.radio_hw.CheckedChanged += new System.EventHandler(this.radio_hw_CheckedChanged);
         // 
         // radio_sw
         // 
         this.radio_sw.AutoSize = true;
         this.radio_sw.Location = new System.Drawing.Point(19, 76);
         this.radio_sw.Name = "radio_sw";
         this.radio_sw.Size = new System.Drawing.Size(103, 17);
         this.radio_sw.TabIndex = 6;
         this.radio_sw.TabStop = true;
         this.radio_sw.Text = "Enable Software";
         this.radio_sw.UseVisualStyleBackColor = true;
         this.radio_sw.CheckedChanged += new System.EventHandler(this.radio_sw_CheckedChanged);
         // 
         // ColorConvDemoDlg
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(816, 545);
         this.Controls.Add(this.toolStrip1);
         this.Controls.Add(this.pictureBox1);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.panel1);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "ColorConvDemoDlg";
         this.Text = "Color Conversion Demo .NET";
         this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ColorConvDemoDlg_FormClosed);
         this.Load += new System.EventHandler(this.ColorConvDemoDlg_Load);
         this.groupBox2.ResumeLayout(false);
         this.groupBox3.ResumeLayout(false);
         this.panel1.ResumeLayout(false);
         this.groupBox4.ResumeLayout(false);
         this.groupBox4.PerformLayout();
         this.groupBox1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
         this.toolStrip1.ResumeLayout(false);
         this.toolStrip1.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_Freeze;
        private System.Windows.Forms.Button button_Grab;
        private System.Windows.Forms.Button button_Snap;
        private System.Windows.Forms.Button button_LoadConfig;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button_View;
        private System.Windows.Forms.Button button_Buffers;
        private System.Windows.Forms.Button button_Exit;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button_New;
        private System.Windows.Forms.Button button_Load_Raw;
        private System.Windows.Forms.Button button_Save_Raw;
        private System.Windows.Forms.Button button_Save_Converted;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripLabel StatusLabelInfo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel FrameLostCount;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox comboBox_Output_Conversion;
        private System.Windows.Forms.Button button_Options;
        private System.Windows.Forms.Label label1;
        private DALSA.SaperaLT.SapClassGui.ImageBox m_ImageBox;
        private System.Windows.Forms.ToolStripLabel StatusLabel;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel PixelLabel;
        private System.Windows.Forms.ToolStripLabel PixelDataValue;
        private System.Windows.Forms.ToolStripLabel StatusLabelInfoTrash;
        private System.Windows.Forms.RadioButton radio_sw;
        private System.Windows.Forms.RadioButton radio_hw;
        private System.Windows.Forms.RadioButton radio_disable;
    }
}

