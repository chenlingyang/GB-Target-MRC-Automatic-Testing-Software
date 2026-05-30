<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FlatFieldDemoDlg
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

#Region "Windows Form Designer generated code"

   ''' <summary>
   ''' Required method for Designer support - do not modify
   ''' the contents of this method with the code editor.
   ''' </summary>
   Private Sub InitializeComponent()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FlatFieldDemoDlg))
		Me.pictureBox1 = New System.Windows.Forms.PictureBox()
		Me.button_Save = New System.Windows.Forms.Button()
		Me.button_Load = New System.Windows.Forms.Button()
		Me.button_New = New System.Windows.Forms.Button()
		Me.FrameLostCount = New System.Windows.Forms.ToolStripLabel()
		Me.toolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
		Me.StatusLabelInfo = New System.Windows.Forms.ToolStripLabel()
		Me.groupBox1 = New System.Windows.Forms.GroupBox()
		Me.button_Freeze = New System.Windows.Forms.Button()
		Me.button_Grab = New System.Windows.Forms.Button()
		Me.groupBox2 = New System.Windows.Forms.GroupBox()
		Me.button_LoadConfig = New System.Windows.Forms.Button()
		Me.button_Snap = New System.Windows.Forms.Button()
		Me.groupBox3 = New System.Windows.Forms.GroupBox()
		Me.button_View = New System.Windows.Forms.Button()
		Me.button_Buffers = New System.Windows.Forms.Button()
		Me.toolStrip1 = New System.Windows.Forms.ToolStrip()
		Me.PixelLabel = New System.Windows.Forms.ToolStripLabel()
		Me.PixelDataValue = New System.Windows.Forms.ToolStripLabel()
		Me.StatusLabel = New System.Windows.Forms.ToolStripLabel()
		Me.toolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
		Me.toolStripLabel2 = New System.Windows.Forms.ToolStripLabel()
		Me.StatusLabelInfoTrash = New System.Windows.Forms.ToolStripLabel()
		Me.toolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
		Me.panel1 = New System.Windows.Forms.Panel()
		Me.groupBox5 = New System.Windows.Forms.GroupBox()
		Me.groupBox4 = New System.Windows.Forms.GroupBox()
		Me.button_Save_FF = New System.Windows.Forms.Button()
		Me.button_Load_FF = New System.Windows.Forms.Button()
		Me.button_Calibrate = New System.Windows.Forms.Button()
		Me.checkBox_Pixels_Replacement = New System.Windows.Forms.CheckBox()
		Me.checkBox_Hardware_Correction = New System.Windows.Forms.CheckBox()
		Me.checkBox_FaltField = New System.Windows.Forms.CheckBox()
		Me.button_Exit = New System.Windows.Forms.Button()
		Me.CheckBox_Use_ROI = New System.Windows.Forms.CheckBox()
		CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.groupBox1.SuspendLayout()
		Me.groupBox2.SuspendLayout()
		Me.groupBox3.SuspendLayout()
		Me.toolStrip1.SuspendLayout()
		Me.panel1.SuspendLayout()
		Me.groupBox5.SuspendLayout()
		Me.groupBox4.SuspendLayout()
		Me.SuspendLayout()
		'
		'pictureBox1
		'
		Me.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
		Me.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left
		Me.pictureBox1.Image = CType(resources.GetObject("pictureBox1.Image"), System.Drawing.Image)
		Me.pictureBox1.Location = New System.Drawing.Point(0, 0)
		Me.pictureBox1.Name = "pictureBox1"
		Me.pictureBox1.Size = New System.Drawing.Size(64, 554)
		Me.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
		Me.pictureBox1.TabIndex = 23
		Me.pictureBox1.TabStop = False
		'
		'button_Save
		'
		Me.button_Save.Location = New System.Drawing.Point(375, 23)
		Me.button_Save.Name = "button_Save"
		Me.button_Save.Size = New System.Drawing.Size(110, 23)
		Me.button_Save.TabIndex = 2
		Me.button_Save.Text = "Save"
		Me.button_Save.UseVisualStyleBackColor = True
		'
		'button_Load
		'
		Me.button_Load.Location = New System.Drawing.Point(211, 23)
		Me.button_Load.Name = "button_Load"
		Me.button_Load.Size = New System.Drawing.Size(110, 23)
		Me.button_Load.TabIndex = 1
		Me.button_Load.Text = "Load "
		Me.button_Load.UseVisualStyleBackColor = True
		'
		'button_New
		'
		Me.button_New.Location = New System.Drawing.Point(35, 23)
		Me.button_New.Name = "button_New"
		Me.button_New.Size = New System.Drawing.Size(110, 23)
		Me.button_New.TabIndex = 0
		Me.button_New.Text = "New"
		Me.button_New.UseVisualStyleBackColor = True
		'
		'FrameLostCount
		'
		Me.FrameLostCount.Name = "FrameLostCount"
		Me.FrameLostCount.Size = New System.Drawing.Size(0, 22)
		'
		'toolStripSeparator1
		'
		Me.toolStripSeparator1.Name = "toolStripSeparator1"
		Me.toolStripSeparator1.Size = New System.Drawing.Size(6, 25)
		'
		'StatusLabelInfo
		'
		Me.StatusLabelInfo.Name = "StatusLabelInfo"
		Me.StatusLabelInfo.Size = New System.Drawing.Size(0, 22)
		'
		'groupBox1
		'
		Me.groupBox1.Controls.Add(Me.button_Save)
		Me.groupBox1.Controls.Add(Me.button_New)
		Me.groupBox1.Controls.Add(Me.button_Load)
		Me.groupBox1.Location = New System.Drawing.Point(259, 3)
		Me.groupBox1.Name = "groupBox1"
		Me.groupBox1.Size = New System.Drawing.Size(545, 64)
		Me.groupBox1.TabIndex = 28
		Me.groupBox1.TabStop = False
		Me.groupBox1.Text = "File Options"
		'
		'button_Freeze
		'
		Me.button_Freeze.Location = New System.Drawing.Point(23, 94)
		Me.button_Freeze.Name = "button_Freeze"
		Me.button_Freeze.Size = New System.Drawing.Size(104, 23)
		Me.button_Freeze.TabIndex = 3
		Me.button_Freeze.Text = "Freeze"
		Me.button_Freeze.UseVisualStyleBackColor = True
		'
		'button_Grab
		'
		Me.button_Grab.Location = New System.Drawing.Point(23, 56)
		Me.button_Grab.Name = "button_Grab"
		Me.button_Grab.Size = New System.Drawing.Size(104, 23)
		Me.button_Grab.TabIndex = 2
		Me.button_Grab.Text = "Grab"
		Me.button_Grab.UseVisualStyleBackColor = True
		'
		'groupBox2
		'
		Me.groupBox2.Controls.Add(Me.button_LoadConfig)
		Me.groupBox2.Location = New System.Drawing.Point(8, 69)
		Me.groupBox2.Name = "groupBox2"
		Me.groupBox2.Size = New System.Drawing.Size(166, 51)
		Me.groupBox2.TabIndex = 0
		Me.groupBox2.TabStop = False
		Me.groupBox2.Text = "Acquisition Option"
		'
		'button_LoadConfig
		'
		Me.button_LoadConfig.Location = New System.Drawing.Point(23, 19)
		Me.button_LoadConfig.Name = "button_LoadConfig"
		Me.button_LoadConfig.Size = New System.Drawing.Size(104, 23)
		Me.button_LoadConfig.TabIndex = 0
		Me.button_LoadConfig.Text = "LoadConfig"
		Me.button_LoadConfig.UseVisualStyleBackColor = True
		'
		'button_Snap
		'
		Me.button_Snap.Location = New System.Drawing.Point(23, 19)
		Me.button_Snap.Name = "button_Snap"
		Me.button_Snap.Size = New System.Drawing.Size(104, 23)
		Me.button_Snap.TabIndex = 1
		Me.button_Snap.Text = "Snap"
		Me.button_Snap.UseVisualStyleBackColor = True
		'
		'groupBox3
		'
		Me.groupBox3.Controls.Add(Me.button_View)
		Me.groupBox3.Controls.Add(Me.button_Buffers)
		Me.groupBox3.Location = New System.Drawing.Point(6, 456)
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
		'toolStrip1
		'
		Me.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.toolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FrameLostCount, Me.PixelLabel, Me.PixelDataValue, Me.toolStripSeparator1, Me.StatusLabel, Me.toolStripLabel1, Me.toolStripLabel2, Me.StatusLabelInfo, Me.StatusLabelInfoTrash, Me.toolStripSeparator2})
		Me.toolStrip1.Location = New System.Drawing.Point(0, 554)
		Me.toolStrip1.Name = "toolStrip1"
		Me.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
		Me.toolStrip1.Size = New System.Drawing.Size(808, 25)
		Me.toolStrip1.TabIndex = 29
		Me.toolStrip1.Text = "toolStrip1"
		'
		'PixelLabel
		'
		Me.PixelLabel.Name = "PixelLabel"
		Me.PixelLabel.Size = New System.Drawing.Size(37, 22)
		Me.PixelLabel.Text = "Pixel :"
		'
		'PixelDataValue
		'
		Me.PixelDataValue.Name = "PixelDataValue"
		Me.PixelDataValue.Size = New System.Drawing.Size(92, 22)
		Me.PixelDataValue.Text = "Data not avaible"
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
		'toolStripLabel2
		'
		Me.toolStripLabel2.Name = "toolStripLabel2"
		Me.toolStripLabel2.Size = New System.Drawing.Size(0, 22)
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
		'panel1
		'
		Me.panel1.Controls.Add(Me.groupBox5)
		Me.panel1.Controls.Add(Me.groupBox4)
		Me.panel1.Controls.Add(Me.button_Exit)
		Me.panel1.Controls.Add(Me.groupBox3)
		Me.panel1.Controls.Add(Me.groupBox2)
		Me.panel1.Location = New System.Drawing.Point(70, 3)
		Me.panel1.Name = "panel1"
		Me.panel1.Size = New System.Drawing.Size(183, 551)
		Me.panel1.TabIndex = 24
		'
		'groupBox5
		'
		Me.groupBox5.Controls.Add(Me.button_Freeze)
		Me.groupBox5.Controls.Add(Me.button_Grab)
		Me.groupBox5.Controls.Add(Me.button_Snap)
		Me.groupBox5.Location = New System.Drawing.Point(8, 126)
		Me.groupBox5.Name = "groupBox5"
		Me.groupBox5.Size = New System.Drawing.Size(163, 130)
		Me.groupBox5.TabIndex = 6
		Me.groupBox5.TabStop = False
		Me.groupBox5.Text = "Acquisitioin Control"
		'
		'groupBox4
		'
		Me.groupBox4.Controls.Add(Me.CheckBox_Use_ROI)
		Me.groupBox4.Controls.Add(Me.button_Save_FF)
		Me.groupBox4.Controls.Add(Me.button_Load_FF)
		Me.groupBox4.Controls.Add(Me.button_Calibrate)
		Me.groupBox4.Controls.Add(Me.checkBox_Pixels_Replacement)
		Me.groupBox4.Controls.Add(Me.checkBox_Hardware_Correction)
		Me.groupBox4.Controls.Add(Me.checkBox_FaltField)
		Me.groupBox4.Location = New System.Drawing.Point(7, 262)
		Me.groupBox4.Name = "groupBox4"
		Me.groupBox4.Size = New System.Drawing.Size(164, 188)
		Me.groupBox4.TabIndex = 5
		Me.groupBox4.TabStop = False
		Me.groupBox4.Text = "Flat Field Correction"
		'
		'button_Save_FF
		'
		Me.button_Save_FF.Location = New System.Drawing.Point(24, 98)
		Me.button_Save_FF.Name = "button_Save_FF"
		Me.button_Save_FF.Size = New System.Drawing.Size(104, 23)
		Me.button_Save_FF.TabIndex = 5
		Me.button_Save_FF.Text = "Save"
		Me.button_Save_FF.UseVisualStyleBackColor = True
		'
		'button_Load_FF
		'
		Me.button_Load_FF.Location = New System.Drawing.Point(24, 70)
		Me.button_Load_FF.Name = "button_Load_FF"
		Me.button_Load_FF.Size = New System.Drawing.Size(104, 23)
		Me.button_Load_FF.TabIndex = 4
		Me.button_Load_FF.Text = "Load"
		Me.button_Load_FF.UseVisualStyleBackColor = True
		'
		'button_Calibrate
		'
		Me.button_Calibrate.Location = New System.Drawing.Point(24, 42)
		Me.button_Calibrate.Name = "button_Calibrate"
		Me.button_Calibrate.Size = New System.Drawing.Size(104, 23)
		Me.button_Calibrate.TabIndex = 3
		Me.button_Calibrate.Text = "Calibrate"
		Me.button_Calibrate.UseVisualStyleBackColor = True
		'
		'checkBox_Pixels_Replacement
		'
		Me.checkBox_Pixels_Replacement.AutoSize = True
		Me.checkBox_Pixels_Replacement.Location = New System.Drawing.Point(18, 165)
		Me.checkBox_Pixels_Replacement.Name = "checkBox_Pixels_Replacement"
		Me.checkBox_Pixels_Replacement.Size = New System.Drawing.Size(114, 17)
		Me.checkBox_Pixels_Replacement.TabIndex = 2
		Me.checkBox_Pixels_Replacement.Text = "Pixel Replacement"
		Me.checkBox_Pixels_Replacement.UseVisualStyleBackColor = True
		'
		'checkBox_Hardware_Correction
		'
		Me.checkBox_Hardware_Correction.AutoSize = True
		Me.checkBox_Hardware_Correction.Location = New System.Drawing.Point(18, 146)
		Me.checkBox_Hardware_Correction.Name = "checkBox_Hardware_Correction"
		Me.checkBox_Hardware_Correction.Size = New System.Drawing.Size(123, 17)
		Me.checkBox_Hardware_Correction.TabIndex = 1
		Me.checkBox_Hardware_Correction.Text = "Hardware Correction"
		Me.checkBox_Hardware_Correction.UseVisualStyleBackColor = True
		'
		'checkBox_FaltField
		'
		Me.checkBox_FaltField.AutoSize = True
		Me.checkBox_FaltField.Location = New System.Drawing.Point(18, 19)
		Me.checkBox_FaltField.Name = "checkBox_FaltField"
		Me.checkBox_FaltField.Size = New System.Drawing.Size(59, 17)
		Me.checkBox_FaltField.TabIndex = 0
		Me.checkBox_FaltField.Text = "Enable"
		Me.checkBox_FaltField.UseVisualStyleBackColor = True
		'
		'button_Exit
		'
		Me.button_Exit.Location = New System.Drawing.Point(32, 13)
		Me.button_Exit.Name = "button_Exit"
		Me.button_Exit.Size = New System.Drawing.Size(104, 42)
		Me.button_Exit.TabIndex = 4
		Me.button_Exit.Text = "Exit"
		Me.button_Exit.UseVisualStyleBackColor = True
		'
		'CheckBox_Use_ROI
		'
		Me.CheckBox_Use_ROI.AutoSize = True
		Me.CheckBox_Use_ROI.Location = New System.Drawing.Point(18, 126)
		Me.CheckBox_Use_ROI.Name = "CheckBox_Use_ROI"
		Me.CheckBox_Use_ROI.Size = New System.Drawing.Size(76, 17)
		Me.CheckBox_Use_ROI.TabIndex = 6
		Me.CheckBox_Use_ROI.Text = "Use a ROI"
		Me.CheckBox_Use_ROI.UseVisualStyleBackColor = True
		'
		'FlatFieldDemoDlg
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(808, 579)
		Me.Controls.Add(Me.pictureBox1)
		Me.Controls.Add(Me.groupBox1)
		Me.Controls.Add(Me.toolStrip1)
		Me.Controls.Add(Me.panel1)
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.Name = "FlatFieldDemoDlg"
		Me.Text = "Flat-Field  Demo .NET "
		CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
		Me.groupBox1.ResumeLayout(False)
		Me.groupBox2.ResumeLayout(False)
		Me.groupBox3.ResumeLayout(False)
		Me.toolStrip1.ResumeLayout(False)
		Me.toolStrip1.PerformLayout()
		Me.panel1.ResumeLayout(False)
		Me.groupBox5.ResumeLayout(False)
		Me.groupBox4.ResumeLayout(False)
		Me.groupBox4.PerformLayout()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

#End Region

   Private WithEvents pictureBox1 As System.Windows.Forms.PictureBox
   Private WithEvents button_Save As System.Windows.Forms.Button
   Private WithEvents button_Load As System.Windows.Forms.Button
   Private WithEvents button_New As System.Windows.Forms.Button
   Private WithEvents FrameLostCount As System.Windows.Forms.ToolStripLabel
   Private WithEvents toolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
   Private WithEvents StatusLabelInfo As System.Windows.Forms.ToolStripLabel
   Private WithEvents groupBox1 As System.Windows.Forms.GroupBox
   Private WithEvents button_Freeze As System.Windows.Forms.Button
   Private WithEvents button_Grab As System.Windows.Forms.Button
   Private WithEvents groupBox2 As System.Windows.Forms.GroupBox
   Private WithEvents button_Snap As System.Windows.Forms.Button
   Private WithEvents button_LoadConfig As System.Windows.Forms.Button
   Private WithEvents groupBox3 As System.Windows.Forms.GroupBox
   Private WithEvents button_View As System.Windows.Forms.Button
   Private WithEvents button_Buffers As System.Windows.Forms.Button
   Private WithEvents toolStrip1 As System.Windows.Forms.ToolStrip
   Private WithEvents panel1 As System.Windows.Forms.Panel
   Private WithEvents button_Exit As System.Windows.Forms.Button
   Private WithEvents groupBox4 As System.Windows.Forms.GroupBox
   Private WithEvents checkBox_Pixels_Replacement As System.Windows.Forms.CheckBox
   Private WithEvents checkBox_Hardware_Correction As System.Windows.Forms.CheckBox
   Private WithEvents checkBox_FaltField As System.Windows.Forms.CheckBox
   Private WithEvents button_Calibrate As System.Windows.Forms.Button
   Private WithEvents button_Save_FF As System.Windows.Forms.Button
   Private WithEvents button_Load_FF As System.Windows.Forms.Button
   Private WithEvents groupBox5 As System.Windows.Forms.GroupBox
   Private WithEvents m_ImageBox As DALSA.SaperaLT.Demos.NET.VB.FlatFieldDemo.ImageBox
   Private WithEvents PixelLabel As System.Windows.Forms.ToolStripLabel
   Private WithEvents PixelDataValue As System.Windows.Forms.ToolStripLabel
   Private WithEvents StatusLabel As System.Windows.Forms.ToolStripLabel
   Private WithEvents toolStripLabel1 As System.Windows.Forms.ToolStripLabel
   Private WithEvents toolStripLabel2 As System.Windows.Forms.ToolStripLabel
   Private WithEvents StatusLabelInfoTrash As System.Windows.Forms.ToolStripLabel
	Private WithEvents toolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents CheckBox_Use_ROI As System.Windows.Forms.CheckBox
End Class


