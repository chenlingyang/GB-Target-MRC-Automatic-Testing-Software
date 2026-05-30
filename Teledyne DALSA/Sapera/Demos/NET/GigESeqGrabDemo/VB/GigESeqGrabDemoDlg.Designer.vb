<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class GigESeqGrabDemodlg
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(GigESeqGrabDemodlg))
        Me.button_Stop = New System.Windows.Forms.Button
        Me.button_Play = New System.Windows.Forms.Button
        Me.groupBox6 = New System.Windows.Forms.GroupBox
        Me.textBox_Maximum = New System.Windows.Forms.TextBox
        Me.textBox_Minimum = New System.Windows.Forms.TextBox
        Me.label5 = New System.Windows.Forms.Label
        Me.label4 = New System.Windows.Forms.Label
        Me.timer = New System.Windows.Forms.Timer(Me.components)
        Me.button_Exit = New System.Windows.Forms.Button
        Me.button_Pause = New System.Windows.Forms.Button
        Me.button_Record = New System.Windows.Forms.Button
        Me.groupBox3 = New System.Windows.Forms.GroupBox
        Me.button_High_Frame_Rate = New System.Windows.Forms.Button
        Me.button_Load_Config = New System.Windows.Forms.Button
        Me.button_Buffers = New System.Windows.Forms.Button
        Me.groupBox2 = New System.Windows.Forms.GroupBox
        Me.textBox_NumberOfBuffers = New System.Windows.Forms.TextBox
        Me.panel1 = New System.Windows.Forms.Panel
        Me.groupBox5 = New System.Windows.Forms.GroupBox
        Me.textBox_Frame_Rate = New System.Windows.Forms.TextBox
        Me.label3 = New System.Windows.Forms.Label
        Me.groupBox4 = New System.Windows.Forms.GroupBox
        Me.textBox_Displayed = New System.Windows.Forms.TextBox
        Me.label2 = New System.Windows.Forms.Label
        Me.label1 = New System.Windows.Forms.Label
        Me.pictureBox1 = New System.Windows.Forms.PictureBox
        Me.button_Save_Current = New System.Windows.Forms.Button
        Me.button_Load_Current = New System.Windows.Forms.Button
        Me.groupBox1 = New System.Windows.Forms.GroupBox
        Me.button_Save_Sequence = New System.Windows.Forms.Button
        Me.button_Load_Sequence = New System.Windows.Forms.Button
        Me.groupBox6.SuspendLayout()
        Me.groupBox3.SuspendLayout()
        Me.groupBox2.SuspendLayout()
        Me.panel1.SuspendLayout()
        Me.groupBox5.SuspendLayout()
        Me.groupBox4.SuspendLayout()
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.groupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'button_Stop
        '
        Me.button_Stop.Location = New System.Drawing.Point(35, 128)
        Me.button_Stop.Name = "button_Stop"
        Me.button_Stop.Size = New System.Drawing.Size(104, 23)
        Me.button_Stop.TabIndex = 3
        Me.button_Stop.Text = "Stop"
        Me.button_Stop.UseVisualStyleBackColor = True
        '
        'button_Play
        '
        Me.button_Play.Location = New System.Drawing.Point(35, 57)
        Me.button_Play.Name = "button_Play"
        Me.button_Play.Size = New System.Drawing.Size(104, 23)
        Me.button_Play.TabIndex = 1
        Me.button_Play.Text = "Play"
        Me.button_Play.UseVisualStyleBackColor = True
        '
        'groupBox6
        '
        Me.groupBox6.Controls.Add(Me.textBox_Maximum)
        Me.groupBox6.Controls.Add(Me.textBox_Minimum)
        Me.groupBox6.Controls.Add(Me.label5)
        Me.groupBox6.Controls.Add(Me.label4)
        Me.groupBox6.Location = New System.Drawing.Point(8, 52)
        Me.groupBox6.Name = "groupBox6"
        Me.groupBox6.Size = New System.Drawing.Size(143, 82)
        Me.groupBox6.TabIndex = 2
        Me.groupBox6.TabStop = False
        Me.groupBox6.Text = "Time between frame (ms)"
        '
        'textBox_Maximum
        '
        Me.textBox_Maximum.Enabled = False
        Me.textBox_Maximum.Location = New System.Drawing.Point(72, 45)
        Me.textBox_Maximum.Name = "textBox_Maximum"
        Me.textBox_Maximum.Size = New System.Drawing.Size(65, 20)
        Me.textBox_Maximum.TabIndex = 3
        '
        'textBox_Minimum
        '
        Me.textBox_Minimum.Enabled = False
        Me.textBox_Minimum.Location = New System.Drawing.Point(72, 19)
        Me.textBox_Minimum.Name = "textBox_Minimum"
        Me.textBox_Minimum.Size = New System.Drawing.Size(65, 20)
        Me.textBox_Minimum.TabIndex = 2
        '
        'label5
        '
        Me.label5.AutoSize = True
        Me.label5.Location = New System.Drawing.Point(10, 48)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(51, 13)
        Me.label5.TabIndex = 1
        Me.label5.Text = "Maximum"
        '
        'label4
        '
        Me.label4.AutoSize = True
        Me.label4.Location = New System.Drawing.Point(10, 22)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(48, 13)
        Me.label4.TabIndex = 0
        Me.label4.Text = "Minimum"
        '
        'timer
        '
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
        'button_Pause
        '
        Me.button_Pause.Location = New System.Drawing.Point(35, 93)
        Me.button_Pause.Name = "button_Pause"
        Me.button_Pause.Size = New System.Drawing.Size(104, 23)
        Me.button_Pause.TabIndex = 2
        Me.button_Pause.Text = "Pause"
        Me.button_Pause.UseVisualStyleBackColor = True
        '
        'button_Record
        '
        Me.button_Record.Location = New System.Drawing.Point(35, 22)
        Me.button_Record.Name = "button_Record"
        Me.button_Record.Size = New System.Drawing.Size(104, 23)
        Me.button_Record.TabIndex = 0
        Me.button_Record.Text = "Record"
        Me.button_Record.UseVisualStyleBackColor = True
        '
        'groupBox3
        '
        Me.groupBox3.Controls.Add(Me.button_High_Frame_Rate)
        Me.groupBox3.Controls.Add(Me.button_Load_Config)
        Me.groupBox3.Controls.Add(Me.button_Buffers)
        Me.groupBox3.Location = New System.Drawing.Point(6, 229)
        Me.groupBox3.Name = "groupBox3"
        Me.groupBox3.Size = New System.Drawing.Size(165, 120)
        Me.groupBox3.TabIndex = 1
        Me.groupBox3.TabStop = False
        Me.groupBox3.Text = "General Options"
        '
        'button_High_Frame_Rate
        '
        Me.button_High_Frame_Rate.Location = New System.Drawing.Point(34, 89)
        Me.button_High_Frame_Rate.Name = "button_High_Frame_Rate"
        Me.button_High_Frame_Rate.Size = New System.Drawing.Size(104, 23)
        Me.button_High_Frame_Rate.TabIndex = 2
        Me.button_High_Frame_Rate.Text = "High Frame Rate"
        Me.button_High_Frame_Rate.UseVisualStyleBackColor = True
        '
        'button_Load_Config
        '
        Me.button_Load_Config.Location = New System.Drawing.Point(34, 54)
        Me.button_Load_Config.Name = "button_Load_Config"
        Me.button_Load_Config.Size = New System.Drawing.Size(104, 23)
        Me.button_Load_Config.TabIndex = 1
        Me.button_Load_Config.Text = "Load Config"
        Me.button_Load_Config.UseVisualStyleBackColor = True
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
        'groupBox2
        '
        Me.groupBox2.Controls.Add(Me.button_Stop)
        Me.groupBox2.Controls.Add(Me.button_Pause)
        Me.groupBox2.Controls.Add(Me.button_Play)
        Me.groupBox2.Controls.Add(Me.button_Record)
        Me.groupBox2.Location = New System.Drawing.Point(5, 61)
        Me.groupBox2.Name = "groupBox2"
        Me.groupBox2.Size = New System.Drawing.Size(166, 162)
        Me.groupBox2.TabIndex = 0
        Me.groupBox2.TabStop = False
        Me.groupBox2.Text = "Record Control"
        '
        'textBox_NumberOfBuffers
        '
        Me.textBox_NumberOfBuffers.Enabled = False
        Me.textBox_NumberOfBuffers.Location = New System.Drawing.Point(106, 57)
        Me.textBox_NumberOfBuffers.Name = "textBox_NumberOfBuffers"
        Me.textBox_NumberOfBuffers.Size = New System.Drawing.Size(45, 20)
        Me.textBox_NumberOfBuffers.TabIndex = 3
        '
        'panel1
        '
        Me.panel1.Controls.Add(Me.button_Exit)
        Me.panel1.Controls.Add(Me.groupBox5)
        Me.panel1.Controls.Add(Me.groupBox4)
        Me.panel1.Controls.Add(Me.groupBox3)
        Me.panel1.Controls.Add(Me.groupBox2)
        Me.panel1.Location = New System.Drawing.Point(70, 5)
        Me.panel1.Name = "panel1"
        Me.panel1.Size = New System.Drawing.Size(183, 597)
        Me.panel1.TabIndex = 22
        '
        'groupBox5
        '
        Me.groupBox5.Controls.Add(Me.groupBox6)
        Me.groupBox5.Controls.Add(Me.textBox_Frame_Rate)
        Me.groupBox5.Controls.Add(Me.label3)
        Me.groupBox5.Location = New System.Drawing.Point(9, 458)
        Me.groupBox5.Name = "groupBox5"
        Me.groupBox5.Size = New System.Drawing.Size(162, 140)
        Me.groupBox5.TabIndex = 3
        Me.groupBox5.TabStop = False
        Me.groupBox5.Text = "Recording Statistics"
        '
        'textBox_Frame_Rate
        '
        Me.textBox_Frame_Rate.Location = New System.Drawing.Point(95, 21)
        Me.textBox_Frame_Rate.Name = "textBox_Frame_Rate"
        Me.textBox_Frame_Rate.Size = New System.Drawing.Size(53, 20)
        Me.textBox_Frame_Rate.TabIndex = 1
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.Location = New System.Drawing.Point(5, 23)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(84, 13)
        Me.label3.TabIndex = 0
        Me.label3.Text = "Frame Rate/sec"
        '
        'groupBox4
        '
        Me.groupBox4.Controls.Add(Me.textBox_NumberOfBuffers)
        Me.groupBox4.Controls.Add(Me.textBox_Displayed)
        Me.groupBox4.Controls.Add(Me.label2)
        Me.groupBox4.Controls.Add(Me.label1)
        Me.groupBox4.Location = New System.Drawing.Point(6, 355)
        Me.groupBox4.Name = "groupBox4"
        Me.groupBox4.Size = New System.Drawing.Size(165, 97)
        Me.groupBox4.TabIndex = 2
        Me.groupBox4.TabStop = False
        Me.groupBox4.Text = "Buffers"
        '
        'textBox_Displayed
        '
        Me.textBox_Displayed.Enabled = False
        Me.textBox_Displayed.Location = New System.Drawing.Point(106, 17)
        Me.textBox_Displayed.Name = "textBox_Displayed"
        Me.textBox_Displayed.Size = New System.Drawing.Size(45, 20)
        Me.textBox_Displayed.TabIndex = 2
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(8, 60)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(92, 13)
        Me.label2.TabIndex = 1
        Me.label2.Text = "Number of Buffers"
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(8, 20)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(53, 13)
        Me.label1.TabIndex = 0
        Me.label1.Text = "Displayed"
        '
        'pictureBox1
        '
        Me.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left
        Me.pictureBox1.Image = CType(resources.GetObject("pictureBox1.Image"), System.Drawing.Image)
        Me.pictureBox1.Location = New System.Drawing.Point(0, 0)
        Me.pictureBox1.Name = "pictureBox1"
        Me.pictureBox1.Size = New System.Drawing.Size(64, 609)
        Me.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pictureBox1.TabIndex = 21
        Me.pictureBox1.TabStop = False
        '
        'button_Save_Current
        '
        Me.button_Save_Current.Location = New System.Drawing.Point(409, 27)
        Me.button_Save_Current.Name = "button_Save_Current"
        Me.button_Save_Current.Size = New System.Drawing.Size(110, 23)
        Me.button_Save_Current.TabIndex = 3
        Me.button_Save_Current.Text = "Save Current "
        Me.button_Save_Current.UseVisualStyleBackColor = True
        '
        'button_Load_Current
        '
        Me.button_Load_Current.Location = New System.Drawing.Point(293, 27)
        Me.button_Load_Current.Name = "button_Load_Current"
        Me.button_Load_Current.Size = New System.Drawing.Size(110, 23)
        Me.button_Load_Current.TabIndex = 2
        Me.button_Load_Current.Text = "Load Current"
        Me.button_Load_Current.UseVisualStyleBackColor = True
        '
        'groupBox1
        '
        Me.groupBox1.Controls.Add(Me.button_Save_Current)
        Me.groupBox1.Controls.Add(Me.button_Load_Current)
        Me.groupBox1.Controls.Add(Me.button_Save_Sequence)
        Me.groupBox1.Controls.Add(Me.button_Load_Sequence)
        Me.groupBox1.Location = New System.Drawing.Point(259, 0)
        Me.groupBox1.Name = "groupBox1"
        Me.groupBox1.Size = New System.Drawing.Size(537, 58)
        Me.groupBox1.TabIndex = 27
        Me.groupBox1.TabStop = False
        Me.groupBox1.Text = "File Options"
        '
        'button_Save_Sequence
        '
        Me.button_Save_Sequence.Location = New System.Drawing.Point(135, 27)
        Me.button_Save_Sequence.Name = "button_Save_Sequence"
        Me.button_Save_Sequence.Size = New System.Drawing.Size(110, 23)
        Me.button_Save_Sequence.TabIndex = 1
        Me.button_Save_Sequence.Text = "Save Sequence"
        Me.button_Save_Sequence.UseVisualStyleBackColor = True
        '
        'button_Load_Sequence
        '
        Me.button_Load_Sequence.Location = New System.Drawing.Point(19, 27)
        Me.button_Load_Sequence.Name = "button_Load_Sequence"
        Me.button_Load_Sequence.Size = New System.Drawing.Size(110, 23)
        Me.button_Load_Sequence.TabIndex = 0
        Me.button_Load_Sequence.Text = "Load Sequence"
        Me.button_Load_Sequence.UseVisualStyleBackColor = True
        '
        'GigESeqGrabDemodlg
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(796, 609)
        Me.Controls.Add(Me.panel1)
        Me.Controls.Add(Me.pictureBox1)
        Me.Controls.Add(Me.groupBox1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "GigESeqGrabDemodlg"
        Me.Text = "GigE-Vision SeqGrabDemo .NET"
        Me.groupBox6.ResumeLayout(False)
        Me.groupBox6.PerformLayout()
        Me.groupBox3.ResumeLayout(False)
        Me.groupBox2.ResumeLayout(False)
        Me.panel1.ResumeLayout(False)
        Me.groupBox5.ResumeLayout(False)
        Me.groupBox5.PerformLayout()
        Me.groupBox4.ResumeLayout(False)
        Me.groupBox4.PerformLayout()
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.groupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
   Private WithEvents button_Stop As System.Windows.Forms.Button
   Private WithEvents button_Play As System.Windows.Forms.Button
   Private WithEvents groupBox6 As System.Windows.Forms.GroupBox
   Private WithEvents textBox_Maximum As System.Windows.Forms.TextBox
   Private WithEvents textBox_Minimum As System.Windows.Forms.TextBox
   Private WithEvents label5 As System.Windows.Forms.Label
   Private WithEvents label4 As System.Windows.Forms.Label
   Private WithEvents timer As System.Windows.Forms.Timer
   Private WithEvents button_Exit As System.Windows.Forms.Button
   Private WithEvents button_Pause As System.Windows.Forms.Button
   Private WithEvents button_Record As System.Windows.Forms.Button
   Private WithEvents groupBox3 As System.Windows.Forms.GroupBox
   Private WithEvents button_High_Frame_Rate As System.Windows.Forms.Button
   Private WithEvents button_Load_Config As System.Windows.Forms.Button
   Private WithEvents button_Buffers As System.Windows.Forms.Button
   Private WithEvents groupBox2 As System.Windows.Forms.GroupBox
   Private WithEvents textBox_NumberOfBuffers As System.Windows.Forms.TextBox
   Private WithEvents panel1 As System.Windows.Forms.Panel
   Private WithEvents groupBox5 As System.Windows.Forms.GroupBox
   Private WithEvents textBox_Frame_Rate As System.Windows.Forms.TextBox
   Private WithEvents label3 As System.Windows.Forms.Label
   Private WithEvents groupBox4 As System.Windows.Forms.GroupBox
   Private WithEvents textBox_Displayed As System.Windows.Forms.TextBox
   Private WithEvents label2 As System.Windows.Forms.Label
   Private WithEvents label1 As System.Windows.Forms.Label
   Private WithEvents pictureBox1 As System.Windows.Forms.PictureBox
   Private WithEvents button_Save_Current As System.Windows.Forms.Button
   Private WithEvents button_Load_Current As System.Windows.Forms.Button
   Private WithEvents groupBox1 As System.Windows.Forms.GroupBox
   Private WithEvents button_Save_Sequence As System.Windows.Forms.Button
   Private WithEvents button_Load_Sequence As System.Windows.Forms.Button
   Friend WithEvents m_ImageBox As DALSA.SaperaLT.Demos.NET.VB.GigESeqGrabDemo.ImageBox

End Class
