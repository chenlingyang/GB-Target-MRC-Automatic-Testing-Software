<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BayerDemoDlg
   Inherits System.Windows.Forms.Form

   ''' <summary>
   ''' Required designer variable.
   ''' </summary>
   Private components As System.ComponentModel.IContainer = Nothing

   ''' <summary>
   ''' Clean up any resources being used.
   ''' </summary>
   ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
   Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
      If disposing AndAlso (components IsNot Nothing) Then
         components.Dispose()
      End If
      MyBase.Dispose(disposing)
   End Sub


   ''' <summary>
   ''' Required method for Designer support - do not modify
   ''' the contents of this method with the code editor.
   ''' </summary>
   Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BayerDemoDlg))
        Me.groupBox2 = New System.Windows.Forms.GroupBox
        Me.button_Freeze = New System.Windows.Forms.Button
        Me.button_Grab = New System.Windows.Forms.Button
        Me.button_Snap = New System.Windows.Forms.Button
        Me.button_LoadConfig = New System.Windows.Forms.Button
        Me.groupBox3 = New System.Windows.Forms.GroupBox
        Me.button_View = New System.Windows.Forms.Button
        Me.button_Buffers = New System.Windows.Forms.Button
        Me.button_Exit = New System.Windows.Forms.Button
        Me.panel1 = New System.Windows.Forms.Panel
        Me.groupBox4 = New System.Windows.Forms.GroupBox
        Me.checkBox_Hard_Conversion = New System.Windows.Forms.CheckBox
        Me.label1 = New System.Windows.Forms.Label
        Me.comboBox_Output_Conversion = New System.Windows.Forms.ComboBox
        Me.button_Options = New System.Windows.Forms.Button
        Me.checkBox_Bayer_Conversion = New System.Windows.Forms.CheckBox
        Me.button_New = New System.Windows.Forms.Button
        Me.button_Load_Raw = New System.Windows.Forms.Button
        Me.button_Save_Raw = New System.Windows.Forms.Button
        Me.button_Save_Converted = New System.Windows.Forms.Button
        Me.groupBox1 = New System.Windows.Forms.GroupBox
        Me.pictureBox1 = New System.Windows.Forms.PictureBox
        Me.StatusLabelInfo = New System.Windows.Forms.ToolStripLabel
        Me.toolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.FrameLostCount = New System.Windows.Forms.ToolStripLabel
        Me.toolStrip1 = New System.Windows.Forms.ToolStrip
        Me.StatusLabel = New System.Windows.Forms.ToolStripLabel
        Me.toolStripLabel1 = New System.Windows.Forms.ToolStripLabel
        Me.StatusLabelInfoTrash = New System.Windows.Forms.ToolStripLabel
        Me.toolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
        Me.PixelLabel = New System.Windows.Forms.ToolStripLabel
        Me.PixelDataValue = New System.Windows.Forms.ToolStripLabel
        Me.groupBox2.SuspendLayout()
        Me.groupBox3.SuspendLayout()
        Me.panel1.SuspendLayout()
        Me.groupBox4.SuspendLayout()
        Me.groupBox1.SuspendLayout()
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.toolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'groupBox2
        '
        Me.groupBox2.Controls.Add(Me.button_Freeze)
        Me.groupBox2.Controls.Add(Me.button_Grab)
        Me.groupBox2.Controls.Add(Me.button_Snap)
        Me.groupBox2.Controls.Add(Me.button_LoadConfig)
        Me.groupBox2.Location = New System.Drawing.Point(5, 69)
        Me.groupBox2.Name = "groupBox2"
        Me.groupBox2.Size = New System.Drawing.Size(166, 162)
        Me.groupBox2.TabIndex = 0
        Me.groupBox2.TabStop = False
        Me.groupBox2.Text = "Acquisition control"
        '
        'button_Freeze
        '
        Me.button_Freeze.Location = New System.Drawing.Point(35, 128)
        Me.button_Freeze.Name = "button_Freeze"
        Me.button_Freeze.Size = New System.Drawing.Size(104, 23)
        Me.button_Freeze.TabIndex = 3
        Me.button_Freeze.Text = "Freeze"
        Me.button_Freeze.UseVisualStyleBackColor = True
        '
        'button_Grab
        '
        Me.button_Grab.Location = New System.Drawing.Point(35, 93)
        Me.button_Grab.Name = "button_Grab"
        Me.button_Grab.Size = New System.Drawing.Size(104, 23)
        Me.button_Grab.TabIndex = 2
        Me.button_Grab.Text = "Grab"
        Me.button_Grab.UseVisualStyleBackColor = True
        '
        'button_Snap
        '
        Me.button_Snap.Location = New System.Drawing.Point(35, 57)
        Me.button_Snap.Name = "button_Snap"
        Me.button_Snap.Size = New System.Drawing.Size(104, 23)
        Me.button_Snap.TabIndex = 1
        Me.button_Snap.Text = "Snap"
        Me.button_Snap.UseVisualStyleBackColor = True
        '
        'button_LoadConfig
        '
        Me.button_LoadConfig.Location = New System.Drawing.Point(35, 22)
        Me.button_LoadConfig.Name = "button_LoadConfig"
        Me.button_LoadConfig.Size = New System.Drawing.Size(104, 23)
        Me.button_LoadConfig.TabIndex = 0
        Me.button_LoadConfig.Text = "LoadConfig"
        Me.button_LoadConfig.UseVisualStyleBackColor = True
        '
        'groupBox3
        '
        Me.groupBox3.Controls.Add(Me.button_View)
        Me.groupBox3.Controls.Add(Me.button_Buffers)
        Me.groupBox3.Location = New System.Drawing.Point(6, 421)
        Me.groupBox3.Name = "groupBox3"
        Me.groupBox3.Size = New System.Drawing.Size(165, 94)
        Me.groupBox3.TabIndex = 1
        Me.groupBox3.TabStop = False
        Me.groupBox3.Text = "General Options"
        '
        'button_View
        '
        Me.button_View.Location = New System.Drawing.Point(34, 57)
        Me.button_View.Name = "button_View"
        Me.button_View.Size = New System.Drawing.Size(104, 23)
        Me.button_View.TabIndex = 2
        Me.button_View.Text = "View"
        Me.button_View.UseVisualStyleBackColor = True
        '
        'button_Buffers
        '
        Me.button_Buffers.Location = New System.Drawing.Point(34, 19)
        Me.button_Buffers.Name = "button_Buffers"
        Me.button_Buffers.Size = New System.Drawing.Size(104, 23)
        Me.button_Buffers.TabIndex = 0
        Me.button_Buffers.Text = "Buffers"
        Me.button_Buffers.UseVisualStyleBackColor = True
        '
        'button_Exit
        '
        Me.button_Exit.Location = New System.Drawing.Point(40, 13)
        Me.button_Exit.Name = "button_Exit"
        Me.button_Exit.Size = New System.Drawing.Size(104, 42)
        Me.button_Exit.TabIndex = 4
        Me.button_Exit.Text = "Exit"
        Me.button_Exit.UseVisualStyleBackColor = True
        '
        'panel1
        '
        Me.panel1.Controls.Add(Me.groupBox4)
        Me.panel1.Controls.Add(Me.button_Exit)
        Me.panel1.Controls.Add(Me.groupBox3)
        Me.panel1.Controls.Add(Me.groupBox2)
        Me.panel1.Location = New System.Drawing.Point(64, 0)
        Me.panel1.Name = "panel1"
        Me.panel1.Size = New System.Drawing.Size(183, 545)
        Me.panel1.TabIndex = 16
        '
        'groupBox4
        '
        Me.groupBox4.Controls.Add(Me.checkBox_Hard_Conversion)
        Me.groupBox4.Controls.Add(Me.label1)
        Me.groupBox4.Controls.Add(Me.comboBox_Output_Conversion)
        Me.groupBox4.Controls.Add(Me.button_Options)
        Me.groupBox4.Controls.Add(Me.checkBox_Bayer_Conversion)
        Me.groupBox4.Location = New System.Drawing.Point(6, 246)
        Me.groupBox4.Name = "groupBox4"
        Me.groupBox4.Size = New System.Drawing.Size(165, 156)
        Me.groupBox4.TabIndex = 23
        Me.groupBox4.TabStop = False
        Me.groupBox4.Text = "Bayer Conversion"
        '
        'checkBox_Hard_Conversion
        '
        Me.checkBox_Hard_Conversion.AutoSize = True
        Me.checkBox_Hard_Conversion.Checked = True
        Me.checkBox_Hard_Conversion.CheckState = System.Windows.Forms.CheckState.Checked
        Me.checkBox_Hard_Conversion.Location = New System.Drawing.Point(16, 131)
        Me.checkBox_Hard_Conversion.Name = "checkBox_Hard_Conversion"
        Me.checkBox_Hard_Conversion.Size = New System.Drawing.Size(128, 17)
        Me.checkBox_Hard_Conversion.TabIndex = 4
        Me.checkBox_Hard_Conversion.Text = "Hardware Conversion"
        Me.checkBox_Hard_Conversion.UseVisualStyleBackColor = True
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(16, 79)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(74, 13)
        Me.label1.TabIndex = 3
        Me.label1.Text = "Output Format"
        '
        'comboBox_Output_Conversion
        '
        Me.comboBox_Output_Conversion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comboBox_Output_Conversion.FormattingEnabled = True
        Me.comboBox_Output_Conversion.Location = New System.Drawing.Point(16, 99)
        Me.comboBox_Output_Conversion.Name = "comboBox_Output_Conversion"
        Me.comboBox_Output_Conversion.Size = New System.Drawing.Size(121, 21)
        Me.comboBox_Output_Conversion.TabIndex = 2
        '
        'button_Options
        '
        Me.button_Options.Location = New System.Drawing.Point(34, 47)
        Me.button_Options.Name = "button_Options"
        Me.button_Options.Size = New System.Drawing.Size(83, 24)
        Me.button_Options.TabIndex = 1
        Me.button_Options.Text = "Options"
        Me.button_Options.UseVisualStyleBackColor = True
        '
        'checkBox_Bayer_Conversion
        '
        Me.checkBox_Bayer_Conversion.AutoSize = True
        Me.checkBox_Bayer_Conversion.Checked = True
        Me.checkBox_Bayer_Conversion.CheckState = System.Windows.Forms.CheckState.Checked
        Me.checkBox_Bayer_Conversion.Location = New System.Drawing.Point(16, 19)
        Me.checkBox_Bayer_Conversion.Name = "checkBox_Bayer_Conversion"
        Me.checkBox_Bayer_Conversion.Size = New System.Drawing.Size(59, 17)
        Me.checkBox_Bayer_Conversion.TabIndex = 0
        Me.checkBox_Bayer_Conversion.Text = "Enable"
        Me.checkBox_Bayer_Conversion.UseVisualStyleBackColor = True
        '
        'button_New
        '
        Me.button_New.Location = New System.Drawing.Point(19, 27)
        Me.button_New.Name = "button_New"
        Me.button_New.Size = New System.Drawing.Size(110, 23)
        Me.button_New.TabIndex = 0
        Me.button_New.Text = "New"
        Me.button_New.UseVisualStyleBackColor = True
        '
        'button_Load_Raw
        '
        Me.button_Load_Raw.Location = New System.Drawing.Point(146, 27)
        Me.button_Load_Raw.Name = "button_Load_Raw"
        Me.button_Load_Raw.Size = New System.Drawing.Size(110, 23)
        Me.button_Load_Raw.TabIndex = 1
        Me.button_Load_Raw.Text = "Load Raw"
        Me.button_Load_Raw.UseVisualStyleBackColor = True
        '
        'button_Save_Raw
        '
        Me.button_Save_Raw.Location = New System.Drawing.Point(278, 27)
        Me.button_Save_Raw.Name = "button_Save_Raw"
        Me.button_Save_Raw.Size = New System.Drawing.Size(110, 23)
        Me.button_Save_Raw.TabIndex = 2
        Me.button_Save_Raw.Text = "Save Raw"
        Me.button_Save_Raw.UseVisualStyleBackColor = True
        '
        'button_Save_Converted
        '
        Me.button_Save_Converted.Location = New System.Drawing.Point(409, 27)
        Me.button_Save_Converted.Name = "button_Save_Converted"
        Me.button_Save_Converted.Size = New System.Drawing.Size(110, 23)
        Me.button_Save_Converted.TabIndex = 3
        Me.button_Save_Converted.Text = "Save Converted"
        Me.button_Save_Converted.UseVisualStyleBackColor = True
        '
        'groupBox1
        '
        Me.groupBox1.Controls.Add(Me.button_Save_Converted)
        Me.groupBox1.Controls.Add(Me.button_Save_Raw)
        Me.groupBox1.Controls.Add(Me.button_Load_Raw)
        Me.groupBox1.Controls.Add(Me.button_New)
        Me.groupBox1.Location = New System.Drawing.Point(253, 0)
        Me.groupBox1.Name = "groupBox1"
        Me.groupBox1.Size = New System.Drawing.Size(545, 64)
        Me.groupBox1.TabIndex = 21
        Me.groupBox1.TabStop = False
        Me.groupBox1.Text = "File Options"
        '
        'pictureBox1
        '
        Me.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left
        Me.pictureBox1.Image = CType(resources.GetObject("pictureBox1.Image"), System.Drawing.Image)
        Me.pictureBox1.Location = New System.Drawing.Point(0, 0)
        Me.pictureBox1.Name = "pictureBox1"
        Me.pictureBox1.Size = New System.Drawing.Size(64, 545)
        Me.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pictureBox1.TabIndex = 15
        Me.pictureBox1.TabStop = False
        '
        'StatusLabelInfo
        '
        Me.StatusLabelInfo.Name = "StatusLabelInfo"
        Me.StatusLabelInfo.Size = New System.Drawing.Size(0, 22)
        '
        'toolStripSeparator1
        '
        Me.toolStripSeparator1.Name = "toolStripSeparator1"
        Me.toolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'FrameLostCount
        '
        Me.FrameLostCount.Name = "FrameLostCount"
        Me.FrameLostCount.Size = New System.Drawing.Size(0, 22)
        '
        'toolStrip1
        '
        Me.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.toolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusLabelInfo, Me.StatusLabel, Me.toolStripLabel1, Me.StatusLabelInfoTrash, Me.toolStripSeparator2, Me.PixelLabel, Me.PixelDataValue, Me.toolStripSeparator1, Me.FrameLostCount})
        Me.toolStrip1.Location = New System.Drawing.Point(64, 520)
        Me.toolStrip1.Name = "toolStrip1"
        Me.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.toolStrip1.Size = New System.Drawing.Size(752, 25)
        Me.toolStrip1.TabIndex = 22
        Me.toolStrip1.Text = "toolStrip1"
        '
        'StatusLabel
        '
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusLabel.Size = New System.Drawing.Size(45, 22)
        Me.StatusLabel.Text = "Status :"
        '
        'toolStripLabel1
        '
        Me.toolStripLabel1.Name = "toolStripLabel1"
        Me.toolStripLabel1.Size = New System.Drawing.Size(0, 22)
        '
        'StatusLabelInfoTrash
        '
        Me.StatusLabelInfoTrash.Name = "StatusLabelInfoTrash"
        Me.StatusLabelInfoTrash.Size = New System.Drawing.Size(0, 22)
        '
        'toolStripSeparator2
        '
        Me.toolStripSeparator2.Name = "toolStripSeparator2"
        Me.toolStripSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'PixelLabel
        '
        Me.PixelLabel.Name = "PixelLabel"
        Me.PixelLabel.Size = New System.Drawing.Size(36, 22)
        Me.PixelLabel.Text = "Pixel :"
        '
        'PixelDataValue
        '
        Me.PixelDataValue.Name = "PixelDataValue"
        Me.PixelDataValue.Size = New System.Drawing.Size(86, 22)
        Me.PixelDataValue.Text = "Data not avaible"
        '
        'BayerDemoDlg
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(816, 545)
        Me.Controls.Add(Me.toolStrip1)
        Me.Controls.Add(Me.pictureBox1)
        Me.Controls.Add(Me.groupBox1)
        Me.Controls.Add(Me.panel1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "BayerDemoDlg"
        Me.Text = "Bayer Demo .NET"
        Me.groupBox2.ResumeLayout(False)
        Me.groupBox3.ResumeLayout(False)
        Me.panel1.ResumeLayout(False)
        Me.groupBox4.ResumeLayout(False)
        Me.groupBox4.PerformLayout()
        Me.groupBox1.ResumeLayout(False)
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.toolStrip1.ResumeLayout(False)
        Me.toolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub


   Private WithEvents groupBox2 As System.Windows.Forms.GroupBox
   Private WithEvents button_Freeze As System.Windows.Forms.Button
   Private WithEvents button_Grab As System.Windows.Forms.Button
   Private WithEvents button_Snap As System.Windows.Forms.Button
   Private WithEvents button_LoadConfig As System.Windows.Forms.Button
   Private WithEvents groupBox3 As System.Windows.Forms.GroupBox
   Private WithEvents button_View As System.Windows.Forms.Button
   Private WithEvents button_Buffers As System.Windows.Forms.Button
   Private WithEvents button_Exit As System.Windows.Forms.Button
   Private WithEvents panel1 As System.Windows.Forms.Panel
   Private WithEvents button_New As System.Windows.Forms.Button
   Private WithEvents button_Load_Raw As System.Windows.Forms.Button
   Private WithEvents button_Save_Raw As System.Windows.Forms.Button
   Private WithEvents button_Save_Converted As System.Windows.Forms.Button
   Private WithEvents groupBox1 As System.Windows.Forms.GroupBox
   Private WithEvents pictureBox1 As System.Windows.Forms.PictureBox
   Private WithEvents StatusLabelInfo As System.Windows.Forms.ToolStripLabel
   Private WithEvents toolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
   Private WithEvents FrameLostCount As System.Windows.Forms.ToolStripLabel
   Private WithEvents toolStrip1 As System.Windows.Forms.ToolStrip
   Private WithEvents groupBox4 As System.Windows.Forms.GroupBox
   Private WithEvents comboBox_Output_Conversion As System.Windows.Forms.ComboBox
   Private WithEvents button_Options As System.Windows.Forms.Button
   Private WithEvents checkBox_Bayer_Conversion As System.Windows.Forms.CheckBox
   Private WithEvents checkBox_Hard_Conversion As System.Windows.Forms.CheckBox
   Private WithEvents label1 As System.Windows.Forms.Label
   Private WithEvents m_ImageBox As DALSA.SaperaLT.Demos.NET.VB.BayerDemo.ImageBox
   Private WithEvents StatusLabel As System.Windows.Forms.ToolStripLabel
   Private WithEvents toolStripLabel1 As System.Windows.Forms.ToolStripLabel
   Private WithEvents toolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
   Private WithEvents PixelLabel As System.Windows.Forms.ToolStripLabel
   Private WithEvents PixelDataValue As System.Windows.Forms.ToolStripLabel
   Private WithEvents StatusLabelInfoTrash As System.Windows.Forms.ToolStripLabel

End Class
