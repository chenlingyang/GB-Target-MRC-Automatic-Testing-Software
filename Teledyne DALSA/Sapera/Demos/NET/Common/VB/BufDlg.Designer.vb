<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BufDlg
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
        Me.textBox1_PixelDepth = New System.Windows.Forms.TextBox
        Me.label4 = New System.Windows.Forms.Label
        Me.groupBox1_Format = New System.Windows.Forms.GroupBox
        Me.comboBox1_Format = New System.Windows.Forms.ComboBox
        Me.groupBox1_Type = New System.Windows.Forms.GroupBox
        Me.radioButton1_Virtual = New System.Windows.Forms.RadioButton
        Me.radioButton1_Overlay = New System.Windows.Forms.RadioButton
        Me.radioButton1_Offscreen_Video = New System.Windows.Forms.RadioButton
        Me.radioButton1_ScatterGather = New System.Windows.Forms.RadioButton
        Me.radioButton1_Contiguous = New System.Windows.Forms.RadioButton
        Me.button1_Cancel = New System.Windows.Forms.Button
        Me.button1_OK = New System.Windows.Forms.Button
        Me.groupBox1_Count = New System.Windows.Forms.GroupBox
        Me.textBox1_Height = New System.Windows.Forms.TextBox
        Me.textBox1_Width = New System.Windows.Forms.TextBox
        Me.textBox1_Count = New System.Windows.Forms.TextBox
        Me.label3 = New System.Windows.Forms.Label
        Me.label2 = New System.Windows.Forms.Label
        Me.label1 = New System.Windows.Forms.Label
        Me.groupBox1_Format.SuspendLayout()
        Me.groupBox1_Type.SuspendLayout()
        Me.groupBox1_Count.SuspendLayout()
        Me.SuspendLayout()
        '
        'textBox1_PixelDepth
        '
        Me.textBox1_PixelDepth.Location = New System.Drawing.Point(226, 235)
        Me.textBox1_PixelDepth.Name = "textBox1_PixelDepth"
        Me.textBox1_PixelDepth.Size = New System.Drawing.Size(68, 20)
        Me.textBox1_PixelDepth.TabIndex = 13
        '
        'label4
        '
        Me.label4.AutoSize = True
        Me.label4.Location = New System.Drawing.Point(57, 238)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(142, 13)
        Me.label4.TabIndex = 12
        Me.label4.Text = "Pixel Depth (significant bits) :"
        '
        'groupBox1_Format
        '
        Me.groupBox1_Format.Controls.Add(Me.comboBox1_Format)
        Me.groupBox1_Format.Location = New System.Drawing.Point(18, 168)
        Me.groupBox1_Format.Name = "groupBox1_Format"
        Me.groupBox1_Format.Size = New System.Drawing.Size(289, 56)
        Me.groupBox1_Format.TabIndex = 11
        Me.groupBox1_Format.TabStop = False
        Me.groupBox1_Format.Text = "Format"
        '
        'comboBox1_Format
        '
        Me.comboBox1_Format.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comboBox1_Format.FormattingEnabled = True
        Me.comboBox1_Format.Location = New System.Drawing.Point(18, 23)
        Me.comboBox1_Format.Name = "comboBox1_Format"
        Me.comboBox1_Format.Size = New System.Drawing.Size(258, 21)
        Me.comboBox1_Format.TabIndex = 0
        '
        'groupBox1_Type
        '
        Me.groupBox1_Type.Controls.Add(Me.radioButton1_Virtual)
        Me.groupBox1_Type.Controls.Add(Me.radioButton1_Overlay)
        Me.groupBox1_Type.Controls.Add(Me.radioButton1_Offscreen_Video)
        Me.groupBox1_Type.Controls.Add(Me.radioButton1_ScatterGather)
        Me.groupBox1_Type.Controls.Add(Me.radioButton1_Contiguous)
        Me.groupBox1_Type.Location = New System.Drawing.Point(180, 6)
        Me.groupBox1_Type.Name = "groupBox1_Type"
        Me.groupBox1_Type.Size = New System.Drawing.Size(127, 145)
        Me.groupBox1_Type.TabIndex = 10
        Me.groupBox1_Type.TabStop = False
        Me.groupBox1_Type.Text = "Type"
        '
        'radioButton1_Virtual
        '
        Me.radioButton1_Virtual.AutoSize = True
        Me.radioButton1_Virtual.Location = New System.Drawing.Point(14, 122)
        Me.radioButton1_Virtual.Name = "radioButton1_Virtual"
        Me.radioButton1_Virtual.Size = New System.Drawing.Size(54, 17)
        Me.radioButton1_Virtual.TabIndex = 4
        Me.radioButton1_Virtual.TabStop = True
        Me.radioButton1_Virtual.Text = "Virtual"
        Me.radioButton1_Virtual.UseVisualStyleBackColor = True
        '
        'radioButton1_Overlay
        '
        Me.radioButton1_Overlay.AutoSize = True
        Me.radioButton1_Overlay.Location = New System.Drawing.Point(13, 97)
        Me.radioButton1_Overlay.Name = "radioButton1_Overlay"
        Me.radioButton1_Overlay.Size = New System.Drawing.Size(61, 17)
        Me.radioButton1_Overlay.TabIndex = 3
        Me.radioButton1_Overlay.TabStop = True
        Me.radioButton1_Overlay.Text = "Overlay"
        Me.radioButton1_Overlay.UseVisualStyleBackColor = True
        '
        'radioButton1_Offscreen_Video
        '
        Me.radioButton1_Offscreen_Video.AutoSize = True
        Me.radioButton1_Offscreen_Video.Location = New System.Drawing.Point(13, 73)
        Me.radioButton1_Offscreen_Video.Name = "radioButton1_Offscreen_Video"
        Me.radioButton1_Offscreen_Video.Size = New System.Drawing.Size(101, 17)
        Me.radioButton1_Offscreen_Video.TabIndex = 2
        Me.radioButton1_Offscreen_Video.TabStop = True
        Me.radioButton1_Offscreen_Video.Text = "Offscreen Video"
        Me.radioButton1_Offscreen_Video.UseVisualStyleBackColor = True
        '
        'radioButton1_ScatterGather
        '
        Me.radioButton1_ScatterGather.AutoSize = True
        Me.radioButton1_ScatterGather.Location = New System.Drawing.Point(13, 48)
        Me.radioButton1_ScatterGather.Name = "radioButton1_ScatterGather"
        Me.radioButton1_ScatterGather.Size = New System.Drawing.Size(94, 17)
        Me.radioButton1_ScatterGather.TabIndex = 1
        Me.radioButton1_ScatterGather.TabStop = True
        Me.radioButton1_ScatterGather.Text = "Scatter-Gather"
        Me.radioButton1_ScatterGather.UseVisualStyleBackColor = True
        '
        'radioButton1_Contiguous
        '
        Me.radioButton1_Contiguous.AutoSize = True
        Me.radioButton1_Contiguous.Location = New System.Drawing.Point(13, 24)
        Me.radioButton1_Contiguous.Name = "radioButton1_Contiguous"
        Me.radioButton1_Contiguous.Size = New System.Drawing.Size(78, 17)
        Me.radioButton1_Contiguous.TabIndex = 0
        Me.radioButton1_Contiguous.TabStop = True
        Me.radioButton1_Contiguous.Text = "Contiguous"
        Me.radioButton1_Contiguous.UseVisualStyleBackColor = True
        '
        'button1_Cancel
        '
        Me.button1_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.button1_Cancel.Location = New System.Drawing.Point(180, 277)
        Me.button1_Cancel.Name = "button1_Cancel"
        Me.button1_Cancel.Size = New System.Drawing.Size(75, 23)
        Me.button1_Cancel.TabIndex = 9
        Me.button1_Cancel.Text = "Cancel"
        Me.button1_Cancel.UseVisualStyleBackColor = True
        '
        'button1_OK
        '
        Me.button1_OK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.button1_OK.Location = New System.Drawing.Point(90, 277)
        Me.button1_OK.Name = "button1_OK"
        Me.button1_OK.Size = New System.Drawing.Size(75, 23)
        Me.button1_OK.TabIndex = 8
        Me.button1_OK.Text = "OK"
        Me.button1_OK.UseVisualStyleBackColor = True
        '
        'groupBox1_Count
        '
        Me.groupBox1_Count.Controls.Add(Me.textBox1_Height)
        Me.groupBox1_Count.Controls.Add(Me.textBox1_Width)
        Me.groupBox1_Count.Controls.Add(Me.textBox1_Count)
        Me.groupBox1_Count.Controls.Add(Me.label3)
        Me.groupBox1_Count.Controls.Add(Me.label2)
        Me.groupBox1_Count.Controls.Add(Me.label1)
        Me.groupBox1_Count.Location = New System.Drawing.Point(17, 6)
        Me.groupBox1_Count.Name = "groupBox1_Count"
        Me.groupBox1_Count.Size = New System.Drawing.Size(136, 145)
        Me.groupBox1_Count.TabIndex = 7
        Me.groupBox1_Count.TabStop = False
        Me.groupBox1_Count.Text = "Count and Size"
        '
        'textBox1_Height
        '
        Me.textBox1_Height.Location = New System.Drawing.Point(64, 96)
        Me.textBox1_Height.Name = "textBox1_Height"
        Me.textBox1_Height.Size = New System.Drawing.Size(56, 20)
        Me.textBox1_Height.TabIndex = 5
        '
        'textBox1_Width
        '
        Me.textBox1_Width.Location = New System.Drawing.Point(64, 64)
        Me.textBox1_Width.Name = "textBox1_Width"
        Me.textBox1_Width.Size = New System.Drawing.Size(56, 20)
        Me.textBox1_Width.TabIndex = 4
        '
        'textBox1_Count
        '
        Me.textBox1_Count.Location = New System.Drawing.Point(64, 28)
        Me.textBox1_Count.Name = "textBox1_Count"
        Me.textBox1_Count.Size = New System.Drawing.Size(56, 20)
        Me.textBox1_Count.TabIndex = 3
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.Location = New System.Drawing.Point(16, 99)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(44, 13)
        Me.label3.TabIndex = 2
        Me.label3.Text = "Height :"
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(16, 64)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(41, 13)
        Me.label2.TabIndex = 1
        Me.label2.Text = "Width :"
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(16, 36)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(41, 13)
        Me.label1.TabIndex = 0
        Me.label1.Text = "Count :"
        '
        'BufDlg
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(324, 307)
        Me.ControlBox = False
        Me.Controls.Add(Me.textBox1_PixelDepth)
        Me.Controls.Add(Me.label4)
        Me.Controls.Add(Me.groupBox1_Format)
        Me.Controls.Add(Me.groupBox1_Type)
        Me.Controls.Add(Me.button1_Cancel)
        Me.Controls.Add(Me.button1_OK)
        Me.Controls.Add(Me.groupBox1_Count)
        Me.Name = "BufDlg"
        Me.Text = "BufDlg"
        Me.groupBox1_Format.ResumeLayout(False)
        Me.groupBox1_Type.ResumeLayout(False)
        Me.groupBox1_Type.PerformLayout()
        Me.groupBox1_Count.ResumeLayout(False)
        Me.groupBox1_Count.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents textBox1_PixelDepth As System.Windows.Forms.TextBox
    Private WithEvents label4 As System.Windows.Forms.Label
    Private WithEvents groupBox1_Format As System.Windows.Forms.GroupBox
    Private WithEvents comboBox1_Format As System.Windows.Forms.ComboBox
    Private WithEvents groupBox1_Type As System.Windows.Forms.GroupBox
    Private WithEvents radioButton1_Virtual As System.Windows.Forms.RadioButton
    Private WithEvents radioButton1_Overlay As System.Windows.Forms.RadioButton
    Private WithEvents radioButton1_Offscreen_Video As System.Windows.Forms.RadioButton
    Private WithEvents radioButton1_ScatterGather As System.Windows.Forms.RadioButton
    Private WithEvents radioButton1_Contiguous As System.Windows.Forms.RadioButton
    Private WithEvents button1_Cancel As System.Windows.Forms.Button
    Private WithEvents button1_OK As System.Windows.Forms.Button
    Private WithEvents groupBox1_Count As System.Windows.Forms.GroupBox
    Private WithEvents textBox1_Height As System.Windows.Forms.TextBox
    Private WithEvents textBox1_Width As System.Windows.Forms.TextBox
    Private WithEvents textBox1_Count As System.Windows.Forms.TextBox
    Private WithEvents label3 As System.Windows.Forms.Label
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents label1 As System.Windows.Forms.Label
End Class
