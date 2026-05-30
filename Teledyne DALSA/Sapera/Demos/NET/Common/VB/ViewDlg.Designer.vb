<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ViewDlg
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
        Me.button_Apply = New System.Windows.Forms.Button
        Me.button_Cancel = New System.Windows.Forms.Button
        Me.button_OK = New System.Windows.Forms.Button
        Me.groupBox1_Scaling = New System.Windows.Forms.GroupBox
        Me.button_No_Scaling = New System.Windows.Forms.Button
        Me.button_Fit = New System.Windows.Forms.Button
        Me.checkBox_lock = New System.Windows.Forms.CheckBox
        Me.label6 = New System.Windows.Forms.Label
        Me.NUpDown_height_scalor = New System.Windows.Forms.NumericUpDown
        Me.label5 = New System.Windows.Forms.Label
        Me.NUpDown_width_scalor = New System.Windows.Forms.NumericUpDown
        Me.label4 = New System.Windows.Forms.Label
        Me.label3 = New System.Windows.Forms.Label
        Me.NUpDown_height = New System.Windows.Forms.NumericUpDown
        Me.NUpDown_width = New System.Windows.Forms.NumericUpDown
        Me.label2 = New System.Windows.Forms.Label
        Me.label1 = New System.Windows.Forms.Label
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.textBox_Range = New System.Windows.Forms.TextBox
        Me.Slider_Range = New System.Windows.Forms.TrackBar
        Me.ViewFormatBox = New System.Windows.Forms.GroupBox
        Me.Label10 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.comboBox1_ViewFormat = New System.Windows.Forms.ComboBox
        Me.groupBox1_Scaling.SuspendLayout()
        CType(Me.NUpDown_height_scalor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NUpDown_width_scalor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NUpDown_height, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NUpDown_width, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        CType(Me.Slider_Range, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ViewFormatBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'button_Apply
        '
        Me.button_Apply.Location = New System.Drawing.Point(140, 409)
        Me.button_Apply.Name = "button_Apply"
        Me.button_Apply.Size = New System.Drawing.Size(75, 23)
        Me.button_Apply.TabIndex = 7
        Me.button_Apply.Text = "Apply"
        Me.button_Apply.UseVisualStyleBackColor = True
        '
        'button_Cancel
        '
        Me.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.button_Cancel.Location = New System.Drawing.Point(221, 409)
        Me.button_Cancel.Name = "button_Cancel"
        Me.button_Cancel.Size = New System.Drawing.Size(75, 23)
        Me.button_Cancel.TabIndex = 6
        Me.button_Cancel.Text = "Cancel"
        Me.button_Cancel.UseVisualStyleBackColor = True
        '
        'button_OK
        '
        Me.button_OK.Cursor = System.Windows.Forms.Cursors.Default
        Me.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.button_OK.Location = New System.Drawing.Point(59, 409)
        Me.button_OK.Name = "button_OK"
        Me.button_OK.Size = New System.Drawing.Size(75, 23)
        Me.button_OK.TabIndex = 5
        Me.button_OK.Text = "OK"
        Me.button_OK.UseVisualStyleBackColor = True
        '
        'groupBox1_Scaling
        '
        Me.groupBox1_Scaling.Controls.Add(Me.button_No_Scaling)
        Me.groupBox1_Scaling.Controls.Add(Me.button_Fit)
        Me.groupBox1_Scaling.Controls.Add(Me.checkBox_lock)
        Me.groupBox1_Scaling.Controls.Add(Me.label6)
        Me.groupBox1_Scaling.Controls.Add(Me.NUpDown_height_scalor)
        Me.groupBox1_Scaling.Controls.Add(Me.label5)
        Me.groupBox1_Scaling.Controls.Add(Me.NUpDown_width_scalor)
        Me.groupBox1_Scaling.Controls.Add(Me.label4)
        Me.groupBox1_Scaling.Controls.Add(Me.label3)
        Me.groupBox1_Scaling.Controls.Add(Me.NUpDown_height)
        Me.groupBox1_Scaling.Controls.Add(Me.NUpDown_width)
        Me.groupBox1_Scaling.Controls.Add(Me.label2)
        Me.groupBox1_Scaling.Controls.Add(Me.label1)
        Me.groupBox1_Scaling.Cursor = System.Windows.Forms.Cursors.Default
        Me.groupBox1_Scaling.Location = New System.Drawing.Point(6, 8)
        Me.groupBox1_Scaling.Name = "groupBox1_Scaling"
        Me.groupBox1_Scaling.Size = New System.Drawing.Size(351, 159)
        Me.groupBox1_Scaling.TabIndex = 4
        Me.groupBox1_Scaling.TabStop = False
        Me.groupBox1_Scaling.Text = "Scaling"
        '
        'button_No_Scaling
        '
        Me.button_No_Scaling.Location = New System.Drawing.Point(256, 122)
        Me.button_No_Scaling.Name = "button_No_Scaling"
        Me.button_No_Scaling.Size = New System.Drawing.Size(89, 23)
        Me.button_No_Scaling.TabIndex = 13
        Me.button_No_Scaling.Text = "No scaling"
        Me.button_No_Scaling.UseVisualStyleBackColor = True
        '
        'button_Fit
        '
        Me.button_Fit.Location = New System.Drawing.Point(163, 122)
        Me.button_Fit.Name = "button_Fit"
        Me.button_Fit.Size = New System.Drawing.Size(87, 23)
        Me.button_Fit.TabIndex = 12
        Me.button_Fit.Text = "Fit to Window"
        Me.button_Fit.UseVisualStyleBackColor = True
        '
        'checkBox_lock
        '
        Me.checkBox_lock.AutoSize = True
        Me.checkBox_lock.Location = New System.Drawing.Point(33, 126)
        Me.checkBox_lock.Name = "checkBox_lock"
        Me.checkBox_lock.Size = New System.Drawing.Size(108, 17)
        Me.checkBox_lock.TabIndex = 11
        Me.checkBox_lock.Text = "Lock aspect ratio"
        Me.checkBox_lock.UseVisualStyleBackColor = True
        '
        'label6
        '
        Me.label6.AutoSize = True
        Me.label6.Location = New System.Drawing.Point(316, 82)
        Me.label6.Name = "label6"
        Me.label6.Size = New System.Drawing.Size(15, 13)
        Me.label6.TabIndex = 10
        Me.label6.Text = "%"
        '
        'NUpDown_height_scalor
        '
        Me.NUpDown_height_scalor.Cursor = System.Windows.Forms.Cursors.Default
        Me.NUpDown_height_scalor.DecimalPlaces = 2
        Me.NUpDown_height_scalor.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.NUpDown_height_scalor.Location = New System.Drawing.Point(242, 77)
        Me.NUpDown_height_scalor.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
        Me.NUpDown_height_scalor.Name = "NUpDown_height_scalor"
        Me.NUpDown_height_scalor.Size = New System.Drawing.Size(71, 20)
        Me.NUpDown_height_scalor.TabIndex = 9
        '
        'label5
        '
        Me.label5.AutoSize = True
        Me.label5.Location = New System.Drawing.Point(316, 34)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(15, 13)
        Me.label5.TabIndex = 8
        Me.label5.Text = "%"
        '
        'NUpDown_width_scalor
        '
        Me.NUpDown_width_scalor.DecimalPlaces = 2
        Me.NUpDown_width_scalor.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.NUpDown_width_scalor.Location = New System.Drawing.Point(242, 29)
        Me.NUpDown_width_scalor.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
        Me.NUpDown_width_scalor.Name = "NUpDown_width_scalor"
        Me.NUpDown_width_scalor.Size = New System.Drawing.Size(71, 20)
        Me.NUpDown_width_scalor.TabIndex = 7
        '
        'label4
        '
        Me.label4.AutoSize = True
        Me.label4.Location = New System.Drawing.Point(165, 84)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(28, 13)
        Me.label4.TabIndex = 6
        Me.label4.Text = "lines"
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.Location = New System.Drawing.Point(165, 36)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(33, 13)
        Me.label3.TabIndex = 5
        Me.label3.Text = "pixels"
        '
        'NUpDown_height
        '
        Me.NUpDown_height.Location = New System.Drawing.Point(81, 77)
        Me.NUpDown_height.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
        Me.NUpDown_height.Name = "NUpDown_height"
        Me.NUpDown_height.Size = New System.Drawing.Size(82, 20)
        Me.NUpDown_height.TabIndex = 4
        '
        'NUpDown_width
        '
        Me.NUpDown_width.Location = New System.Drawing.Point(81, 30)
        Me.NUpDown_width.Maximum = New Decimal(New Integer() {1000000, 0, 0, 0})
        Me.NUpDown_width.Name = "NUpDown_width"
        Me.NUpDown_width.Size = New System.Drawing.Size(82, 20)
        Me.NUpDown_width.TabIndex = 3
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(33, 84)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(38, 13)
        Me.label2.TabIndex = 1
        Me.label2.Text = "Height"
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(33, 36)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(35, 13)
        Me.label1.TabIndex = 0
        Me.label1.Text = "Width"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label8)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.textBox_Range)
        Me.GroupBox1.Controls.Add(Me.Slider_Range)
        Me.GroupBox1.Location = New System.Drawing.Point(6, 173)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(351, 110)
        Me.GroupBox1.TabIndex = 8
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Range"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(18, 84)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(107, 13)
        Me.Label8.TabIndex = 3
        Me.Label8.Text = "are not to be viewed."
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(18, 66)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(264, 13)
        Me.Label7.TabIndex = 2
        Me.Label7.Text = "Range: specifies how many of the most significant bits "
        '
        'textBox_Range
        '
        Me.textBox_Range.Location = New System.Drawing.Point(279, 20)
        Me.textBox_Range.Name = "textBox_Range"
        Me.textBox_Range.Size = New System.Drawing.Size(52, 20)
        Me.textBox_Range.TabIndex = 1
        '
        'Slider_Range
        '
        Me.Slider_Range.Location = New System.Drawing.Point(6, 21)
        Me.Slider_Range.Name = "Slider_Range"
        Me.Slider_Range.Size = New System.Drawing.Size(255, 45)
        Me.Slider_Range.TabIndex = 0
        '
        'ViewFormatBox
        '
        Me.ViewFormatBox.Controls.Add(Me.Label10)
        Me.ViewFormatBox.Controls.Add(Me.Label9)
        Me.ViewFormatBox.Controls.Add(Me.comboBox1_ViewFormat)
        Me.ViewFormatBox.Location = New System.Drawing.Point(5, 294)
        Me.ViewFormatBox.Name = "ViewFormatBox"
        Me.ViewFormatBox.Size = New System.Drawing.Size(351, 100)
        Me.ViewFormatBox.TabIndex = 9
        Me.ViewFormatBox.TabStop = False
        Me.ViewFormatBox.Text = "View Format"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(15, 71)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(127, 13)
        Me.Label10.TabIndex = 2
        Me.Label10.Text = "buffers (like RGB-IR 8-bit)"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(14, 58)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(256, 13)
        Me.Label9.TabIndex = 1
        Me.Label9.Text = "View Format: specifies what to display for multi-format"
        '
        'comboBox1_ViewFormat
        '
        Me.comboBox1_ViewFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comboBox1_ViewFormat.FormattingEnabled = True
        Me.comboBox1_ViewFormat.Location = New System.Drawing.Point(11, 22)
        Me.comboBox1_ViewFormat.Name = "comboBox1_ViewFormat"
        Me.comboBox1_ViewFormat.Size = New System.Drawing.Size(334, 21)
        Me.comboBox1_ViewFormat.TabIndex = 0
        '
        'ViewDlg
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(369, 448)
        Me.ControlBox = False
        Me.Controls.Add(Me.ViewFormatBox)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.button_Apply)
        Me.Controls.Add(Me.button_Cancel)
        Me.Controls.Add(Me.button_OK)
        Me.Controls.Add(Me.groupBox1_Scaling)
        Me.Name = "ViewDlg"
        Me.Text = "ViewDlg"
        Me.groupBox1_Scaling.ResumeLayout(False)
        Me.groupBox1_Scaling.PerformLayout()
        CType(Me.NUpDown_height_scalor, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NUpDown_width_scalor, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NUpDown_height, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NUpDown_width, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.Slider_Range, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ViewFormatBox.ResumeLayout(False)
        Me.ViewFormatBox.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents button_Apply As System.Windows.Forms.Button
    Private WithEvents button_Cancel As System.Windows.Forms.Button
    Private WithEvents button_OK As System.Windows.Forms.Button
    Private WithEvents groupBox1_Scaling As System.Windows.Forms.GroupBox
    Private WithEvents button_No_Scaling As System.Windows.Forms.Button
    Private WithEvents button_Fit As System.Windows.Forms.Button
    Private WithEvents checkBox_lock As System.Windows.Forms.CheckBox
    Private WithEvents label6 As System.Windows.Forms.Label
    Private WithEvents NUpDown_height_scalor As System.Windows.Forms.NumericUpDown
    Private WithEvents label5 As System.Windows.Forms.Label
    Private WithEvents NUpDown_width_scalor As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
    Private WithEvents label4 As System.Windows.Forms.Label
    Private WithEvents label3 As System.Windows.Forms.Label
    Private WithEvents NUpDown_height As System.Windows.Forms.NumericUpDown
    Private WithEvents NUpDown_width As System.Windows.Forms.NumericUpDown
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents label1 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents textBox_Range As System.Windows.Forms.TextBox
    Friend WithEvents Slider_Range As System.Windows.Forms.TrackBar
    Friend WithEvents ViewFormatBox As System.Windows.Forms.GroupBox
    Friend WithEvents comboBox1_ViewFormat As System.Windows.Forms.ComboBox
End Class
