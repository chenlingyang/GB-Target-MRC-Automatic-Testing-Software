<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FlatFieldDlg
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
      Me.groupBox1 = New System.Windows.Forms.GroupBox()
      Me.textBox_Max_Dev = New System.Windows.Forms.TextBox()
      Me.textBox_Vert_Offset = New System.Windows.Forms.TextBox()
      Me.textBox_Frame_Avg = New System.Windows.Forms.TextBox()
      Me.textBox_Line_Avg = New System.Windows.Forms.TextBox()
      Me.label7 = New System.Windows.Forms.Label()
      Me.label6 = New System.Windows.Forms.Label()
      Me.label5 = New System.Windows.Forms.Label()
      Me.label4 = New System.Windows.Forms.Label()
      Me.label3 = New System.Windows.Forms.Label()
      Me.comboBox_Correction_Type = New System.Windows.Forms.ComboBox()
      Me.comboBox_Video_Type = New System.Windows.Forms.ComboBox()
      Me.label2 = New System.Windows.Forms.Label()
      Me.label1 = New System.Windows.Forms.Label()
      Me.label8 = New System.Windows.Forms.Label()
      Me.comboBox_Calibration_Index = New System.Windows.Forms.ComboBox()
      Me.label9 = New System.Windows.Forms.Label()
      Me.label_darkImage1 = New System.Windows.Forms.Label()
      Me.label_darkImage2 = New System.Windows.Forms.Label()
      Me.label_brightImage2 = New System.Windows.Forms.Label()
      Me.label_brightImage1 = New System.Windows.Forms.Label()
      Me.label14 = New System.Windows.Forms.Label()
      Me.button_Acq_Dark = New System.Windows.Forms.Button()
      Me.button_Acq_Bright = New System.Windows.Forms.Button()
      Me.LogMessageBox = New System.Windows.Forms.ListBox()
      Me.button_OK = New System.Windows.Forms.Button()
      Me.button_Cancel = New System.Windows.Forms.Button()
      Me.ClippedCoefsDefects_checkbox = New System.Windows.Forms.CheckBox()
      Me.label10 = New System.Windows.Forms.Label()
      Me.button_Save_And_Upload = New System.Windows.Forms.Button()
      Me.Label11 = New System.Windows.Forms.Label()
      Me.saveLabel = New System.Windows.Forms.Label()
      Me.comboBox_FlatField_Selector = New System.Windows.Forms.ComboBox()
      Me.label_flatField = New System.Windows.Forms.Label()
      Me.button_load_cluster = New System.Windows.Forms.Button()
      Me.label_load_cluster = New System.Windows.Forms.Label()
      Me.groupBox1.SuspendLayout()
      Me.SuspendLayout()
      '
      'groupBox1
      '
      Me.groupBox1.Controls.Add(Me.label_load_cluster)
      Me.groupBox1.Controls.Add(Me.button_load_cluster)
      Me.groupBox1.Controls.Add(Me.textBox_Max_Dev)
      Me.groupBox1.Controls.Add(Me.textBox_Vert_Offset)
      Me.groupBox1.Controls.Add(Me.textBox_Frame_Avg)
      Me.groupBox1.Controls.Add(Me.textBox_Line_Avg)
      Me.groupBox1.Controls.Add(Me.label7)
      Me.groupBox1.Controls.Add(Me.label6)
      Me.groupBox1.Controls.Add(Me.label5)
      Me.groupBox1.Controls.Add(Me.label4)
      Me.groupBox1.Controls.Add(Me.label3)
      Me.groupBox1.Controls.Add(Me.comboBox_Correction_Type)
      Me.groupBox1.Controls.Add(Me.comboBox_Video_Type)
      Me.groupBox1.Controls.Add(Me.label2)
      Me.groupBox1.Controls.Add(Me.label1)
      Me.groupBox1.Location = New System.Drawing.Point(12, 12)
      Me.groupBox1.Name = "groupBox1"
      Me.groupBox1.Size = New System.Drawing.Size(458, 226)
      Me.groupBox1.TabIndex = 0
      Me.groupBox1.TabStop = False
      Me.groupBox1.Text = "Parameters"
      '
      'textBox_Max_Dev
      '
      Me.textBox_Max_Dev.Location = New System.Drawing.Point(289, 141)
      Me.textBox_Max_Dev.Name = "textBox_Max_Dev"
      Me.textBox_Max_Dev.Size = New System.Drawing.Size(117, 20)
      Me.textBox_Max_Dev.TabIndex = 12
      '
      'textBox_Vert_Offset
      '
      Me.textBox_Vert_Offset.Location = New System.Drawing.Point(18, 142)
      Me.textBox_Vert_Offset.Name = "textBox_Vert_Offset"
      Me.textBox_Vert_Offset.Size = New System.Drawing.Size(109, 20)
      Me.textBox_Vert_Offset.TabIndex = 11
      '
      'textBox_Frame_Avg
      '
      Me.textBox_Frame_Avg.Location = New System.Drawing.Point(289, 89)
      Me.textBox_Frame_Avg.Name = "textBox_Frame_Avg"
      Me.textBox_Frame_Avg.Size = New System.Drawing.Size(117, 20)
      Me.textBox_Frame_Avg.TabIndex = 10
      '
      'textBox_Line_Avg
      '
      Me.textBox_Line_Avg.Location = New System.Drawing.Point(18, 89)
      Me.textBox_Line_Avg.Name = "textBox_Line_Avg"
      Me.textBox_Line_Avg.Size = New System.Drawing.Size(109, 20)
      Me.textBox_Line_Avg.TabIndex = 9
      '
      'label7
      '
      Me.label7.AutoSize = True
      Me.label7.Location = New System.Drawing.Point(286, 125)
      Me.label7.Name = "label7"
      Me.label7.Size = New System.Drawing.Size(86, 13)
      Me.label7.TabIndex = 8
      Me.label7.Text = "mean pixel value"
      '
      'label6
      '
      Me.label6.AutoSize = True
      Me.label6.Location = New System.Drawing.Point(286, 112)
      Me.label6.Name = "label6"
      Me.label6.Size = New System.Drawing.Size(120, 13)
      Me.label6.TabIndex = 7
      Me.label6.Text = "Maximum deviation from"
      '
      'label5
      '
      Me.label5.AutoSize = True
      Me.label5.Location = New System.Drawing.Point(15, 124)
      Me.label5.Name = "label5"
      Me.label5.Size = New System.Drawing.Size(112, 13)
      Me.label5.TabIndex = 6
      Me.label5.Text = "Vertical offset from top"
      '
      'label4
      '
      Me.label4.AutoSize = True
      Me.label4.Location = New System.Drawing.Point(286, 73)
      Me.label4.Name = "label4"
      Me.label4.Size = New System.Drawing.Size(139, 13)
      Me.label4.TabIndex = 5
      Me.label4.Text = "Number of frame to average"
      '
      'label3
      '
      Me.label3.AutoSize = True
      Me.label3.Location = New System.Drawing.Point(15, 73)
      Me.label3.Name = "label3"
      Me.label3.Size = New System.Drawing.Size(129, 13)
      Me.label3.TabIndex = 4
      Me.label3.Text = "Number of line to average"
      '
      'comboBox_Correction_Type
      '
      Me.comboBox_Correction_Type.FormattingEnabled = True
      Me.comboBox_Correction_Type.Location = New System.Drawing.Point(18, 32)
      Me.comboBox_Correction_Type.Name = "comboBox_Correction_Type"
      Me.comboBox_Correction_Type.Size = New System.Drawing.Size(159, 21)
      Me.comboBox_Correction_Type.TabIndex = 3
      '
      'comboBox_Video_Type
      '
      Me.comboBox_Video_Type.FormattingEnabled = True
      Me.comboBox_Video_Type.Location = New System.Drawing.Point(289, 33)
      Me.comboBox_Video_Type.Name = "comboBox_Video_Type"
      Me.comboBox_Video_Type.Size = New System.Drawing.Size(153, 21)
      Me.comboBox_Video_Type.TabIndex = 2
      '
      'label2
      '
      Me.label2.AutoSize = True
      Me.label2.Location = New System.Drawing.Point(303, 17)
      Me.label2.Name = "label2"
      Me.label2.Size = New System.Drawing.Size(61, 13)
      Me.label2.TabIndex = 1
      Me.label2.Text = "Video Type"
      '
      'label1
      '
      Me.label1.AutoSize = True
      Me.label1.Location = New System.Drawing.Point(15, 16)
      Me.label1.Name = "label1"
      Me.label1.Size = New System.Drawing.Size(82, 13)
      Me.label1.TabIndex = 0
      Me.label1.Text = "Correction Type"
      '
      'label8
      '
      Me.label8.AutoSize = True
      Me.label8.Location = New System.Drawing.Point(9, 259)
      Me.label8.Name = "label8"
      Me.label8.Size = New System.Drawing.Size(84, 13)
      Me.label8.TabIndex = 1
      Me.label8.Text = "Calibration index"
      '
      'comboBox_Calibration_Index
      '
      Me.comboBox_Calibration_Index.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
      Me.comboBox_Calibration_Index.FormattingEnabled = True
      Me.comboBox_Calibration_Index.Location = New System.Drawing.Point(99, 258)
      Me.comboBox_Calibration_Index.Name = "comboBox_Calibration_Index"
      Me.comboBox_Calibration_Index.Size = New System.Drawing.Size(100, 21)
      Me.comboBox_Calibration_Index.TabIndex = 2
      '
      'label9
      '
      Me.label9.AutoSize = True
      Me.label9.Location = New System.Drawing.Point(11, 304)
      Me.label9.Name = "label9"
      Me.label9.Size = New System.Drawing.Size(44, 13)
      Me.label9.TabIndex = 3
      Me.label9.Text = "Step 1 :"
      '
      'label_darkImage1
      '
      Me.label_darkImage1.AutoSize = True
      Me.label_darkImage1.Location = New System.Drawing.Point(50, 304)
      Me.label_darkImage1.Name = "label_darkImage1"
      Me.label_darkImage1.Size = New System.Drawing.Size(107, 13)
      Me.label_darkImage1.TabIndex = 4
      Me.label_darkImage1.Text = "Acquire a dark image"
      '
      'label_darkImage2
      '
      Me.label_darkImage2.AutoSize = True
      Me.label_darkImage2.Location = New System.Drawing.Point(50, 319)
      Me.label_darkImage2.Name = "label_darkImage2"
      Me.label_darkImage2.Size = New System.Drawing.Size(203, 13)
      Me.label_darkImage2.TabIndex = 5
      Me.label_darkImage2.Text = "Pixels mean under 20 level recommanded"
      '
      'label_brightImage2
      '
      Me.label_brightImage2.AutoSize = True
      Me.label_brightImage2.Location = New System.Drawing.Point(50, 363)
      Me.label_brightImage2.Name = "label_brightImage2"
      Me.label_brightImage2.Size = New System.Drawing.Size(215, 13)
      Me.label_brightImage2.TabIndex = 8
      Me.label_brightImage2.Text = "Pixels mean around 128 level recommanded"
      '
      'label_brightImage1
      '
      Me.label_brightImage1.AutoSize = True
      Me.label_brightImage1.Location = New System.Drawing.Point(50, 347)
      Me.label_brightImage1.Name = "label_brightImage1"
      Me.label_brightImage1.Size = New System.Drawing.Size(180, 13)
      Me.label_brightImage1.TabIndex = 7
      Me.label_brightImage1.Text = "Acquire a non-saturated bright image"
      '
      'label14
      '
      Me.label14.AutoSize = True
      Me.label14.Location = New System.Drawing.Point(11, 347)
      Me.label14.Name = "label14"
      Me.label14.Size = New System.Drawing.Size(44, 13)
      Me.label14.TabIndex = 6
      Me.label14.Text = "Step 2 :"
      '
      'button_Acq_Dark
      '
      Me.button_Acq_Dark.Location = New System.Drawing.Point(297, 304)
      Me.button_Acq_Dark.Name = "button_Acq_Dark"
      Me.button_Acq_Dark.Size = New System.Drawing.Size(168, 21)
      Me.button_Acq_Dark.TabIndex = 9
      Me.button_Acq_Dark.Text = "Acquire Dark Image"
      Me.button_Acq_Dark.UseVisualStyleBackColor = True
      '
      'button_Acq_Bright
      '
      Me.button_Acq_Bright.Location = New System.Drawing.Point(297, 347)
      Me.button_Acq_Bright.Name = "button_Acq_Bright"
      Me.button_Acq_Bright.Size = New System.Drawing.Size(168, 21)
      Me.button_Acq_Bright.TabIndex = 10
      Me.button_Acq_Bright.Text = "Acquire Bright Image"
      Me.button_Acq_Bright.UseVisualStyleBackColor = True
      '
      'LogMessageBox
      '
      Me.LogMessageBox.FormattingEnabled = True
      Me.LogMessageBox.Location = New System.Drawing.Point(12, 496)
      Me.LogMessageBox.Name = "LogMessageBox"
      Me.LogMessageBox.Size = New System.Drawing.Size(453, 108)
      Me.LogMessageBox.TabIndex = 12
      '
      'button_OK
      '
      Me.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK
      Me.button_OK.Location = New System.Drawing.Point(131, 626)
      Me.button_OK.Name = "button_OK"
      Me.button_OK.Size = New System.Drawing.Size(75, 23)
      Me.button_OK.TabIndex = 13
      Me.button_OK.Text = "OK"
      Me.button_OK.UseVisualStyleBackColor = True
      '
      'button_Cancel
      '
      Me.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
      Me.button_Cancel.Location = New System.Drawing.Point(276, 626)
      Me.button_Cancel.Name = "button_Cancel"
      Me.button_Cancel.Size = New System.Drawing.Size(75, 23)
      Me.button_Cancel.TabIndex = 14
      Me.button_Cancel.Text = "Cancel"
      Me.button_Cancel.UseVisualStyleBackColor = True
      '
      'ClippedCoefsDefects_checkbox
      '
      Me.ClippedCoefsDefects_checkbox.AutoSize = True
      Me.ClippedCoefsDefects_checkbox.Location = New System.Drawing.Point(447, 266)
      Me.ClippedCoefsDefects_checkbox.Name = "ClippedCoefsDefects_checkbox"
      Me.ClippedCoefsDefects_checkbox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.ClippedCoefsDefects_checkbox.Size = New System.Drawing.Size(15, 14)
      Me.ClippedCoefsDefects_checkbox.TabIndex = 15
      Me.ClippedCoefsDefects_checkbox.UseVisualStyleBackColor = True
      '
      'label10
      '
      Me.label10.Location = New System.Drawing.Point(283, 255)
      Me.label10.Name = "label10"
      Me.label10.Size = New System.Drawing.Size(160, 41)
      Me.label10.TabIndex = 16
      Me.label10.Text = "Consider as defective pixel with offset or gain coefficient that reaches the hard" & _
    "ware limit" & Global.Microsoft.VisualBasic.ChrW(10)
      '
      'button_Save_And_Upload
      '
      Me.button_Save_And_Upload.Location = New System.Drawing.Point(297, 393)
      Me.button_Save_And_Upload.Name = "button_Save_And_Upload"
      Me.button_Save_And_Upload.Size = New System.Drawing.Size(168, 21)
      Me.button_Save_And_Upload.TabIndex = 17
      Me.button_Save_And_Upload.Text = "Save and Upload"
      Me.button_Save_And_Upload.UseVisualStyleBackColor = True
      '
      'Label11
      '
      Me.Label11.AutoSize = True
      Me.Label11.Location = New System.Drawing.Point(11, 393)
      Me.Label11.Name = "Label11"
      Me.Label11.Size = New System.Drawing.Size(44, 13)
      Me.Label11.TabIndex = 18
      Me.Label11.Text = "Step 3 :"
      '
      'saveLabel
      '
      Me.saveLabel.AutoSize = True
      Me.saveLabel.Location = New System.Drawing.Point(50, 393)
      Me.saveLabel.Name = "saveLabel"
      Me.saveLabel.Size = New System.Drawing.Size(226, 13)
      Me.saveLabel.TabIndex = 19
      Me.saveLabel.Text = "Save Calibration offset and gain files (Optional)"
      '
      'comboBox_FlatField_Selector
      '
      Me.comboBox_FlatField_Selector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
      Me.comboBox_FlatField_Selector.FormattingEnabled = True
      Me.comboBox_FlatField_Selector.Location = New System.Drawing.Point(297, 456)
      Me.comboBox_FlatField_Selector.Name = "comboBox_FlatField_Selector"
      Me.comboBox_FlatField_Selector.Size = New System.Drawing.Size(168, 21)
      Me.comboBox_FlatField_Selector.TabIndex = 20
      '
      'label_flatField
      '
      Me.label_flatField.AutoSize = True
      Me.label_flatField.Location = New System.Drawing.Point(300, 432)
      Me.label_flatField.Name = "label_flatField"
      Me.label_flatField.Size = New System.Drawing.Size(140, 13)
      Me.label_flatField.TabIndex = 23
      Me.label_flatField.Text = "to user Flat Field Destination"
      '
      'button_load_cluster
      '
      Me.button_load_cluster.Location = New System.Drawing.Point(18, 181)
      Me.button_load_cluster.Name = "button_load_cluster"
      Me.button_load_cluster.Size = New System.Drawing.Size(109, 23)
      Me.button_load_cluster.TabIndex = 13
      Me.button_load_cluster.Text = "Load cluster map"
      Me.button_load_cluster.UseVisualStyleBackColor = True
      '
      'label_load_cluster
      '
      Me.label_load_cluster.AutoSize = True
      Me.label_load_cluster.Location = New System.Drawing.Point(133, 186)
      Me.label_load_cluster.Name = "label_load_cluster"
      Me.label_load_cluster.Size = New System.Drawing.Size(116, 13)
      Me.label_load_cluster.TabIndex = 14
      Me.label_load_cluster.Text = "No defined cluster map"
      '
      'FlatFieldDlg
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(482, 675)
      Me.ControlBox = False
      Me.Controls.Add(Me.label_flatField)
      Me.Controls.Add(Me.comboBox_FlatField_Selector)
      Me.Controls.Add(Me.saveLabel)
      Me.Controls.Add(Me.Label11)
      Me.Controls.Add(Me.button_Save_And_Upload)
      Me.Controls.Add(Me.label10)
      Me.Controls.Add(Me.ClippedCoefsDefects_checkbox)
      Me.Controls.Add(Me.button_Cancel)
      Me.Controls.Add(Me.button_OK)
      Me.Controls.Add(Me.LogMessageBox)
      Me.Controls.Add(Me.button_Acq_Bright)
      Me.Controls.Add(Me.button_Acq_Dark)
      Me.Controls.Add(Me.label_brightImage2)
      Me.Controls.Add(Me.label_brightImage1)
      Me.Controls.Add(Me.label14)
      Me.Controls.Add(Me.label_darkImage2)
      Me.Controls.Add(Me.label_darkImage1)
      Me.Controls.Add(Me.label9)
      Me.Controls.Add(Me.comboBox_Calibration_Index)
      Me.Controls.Add(Me.label8)
      Me.Controls.Add(Me.groupBox1)
      Me.Name = "FlatFieldDlg"
      Me.Text = "FlatFieldDlg"
      Me.groupBox1.ResumeLayout(False)
      Me.groupBox1.PerformLayout()
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub


    Private WithEvents groupBox1 As System.Windows.Forms.GroupBox
    Private WithEvents label6 As System.Windows.Forms.Label
   Private WithEvents label5 As System.Windows.Forms.Label
   Private WithEvents label4 As System.Windows.Forms.Label
   Private WithEvents label3 As System.Windows.Forms.Label
   Private WithEvents comboBox_Correction_Type As System.Windows.Forms.ComboBox
   Private WithEvents comboBox_Video_Type As System.Windows.Forms.ComboBox
   Private WithEvents label2 As System.Windows.Forms.Label
   Private WithEvents label1 As System.Windows.Forms.Label
   Private WithEvents textBox_Max_Dev As System.Windows.Forms.TextBox
   Private WithEvents textBox_Vert_Offset As System.Windows.Forms.TextBox
   Private WithEvents textBox_Frame_Avg As System.Windows.Forms.TextBox
   Private WithEvents textBox_Line_Avg As System.Windows.Forms.TextBox
   Private WithEvents label7 As System.Windows.Forms.Label
   Private WithEvents label8 As System.Windows.Forms.Label
   Private WithEvents comboBox_Calibration_Index As System.Windows.Forms.ComboBox
   Private WithEvents label9 As System.Windows.Forms.Label
   Private WithEvents label_darkImage1 As System.Windows.Forms.Label
   Private WithEvents label_darkImage2 As System.Windows.Forms.Label
   Private WithEvents label_brightImage2 As System.Windows.Forms.Label
   Private WithEvents label_brightImage1 As System.Windows.Forms.Label
   Private WithEvents label14 As System.Windows.Forms.Label
   Private WithEvents button_Acq_Dark As System.Windows.Forms.Button
   Private WithEvents button_Acq_Bright As System.Windows.Forms.Button
   Private WithEvents LogMessageBox As System.Windows.Forms.ListBox
   Private WithEvents button_OK As System.Windows.Forms.Button
   Private WithEvents button_Cancel As System.Windows.Forms.Button
   Private WithEvents ClippedCoefsDefects_checkbox As System.Windows.Forms.CheckBox
    Private WithEvents label10 As System.Windows.Forms.Label
    Private WithEvents button_Save_And_Upload As System.Windows.Forms.Button
    Private WithEvents Label11 As System.Windows.Forms.Label
    Private WithEvents Label12 As System.Windows.Forms.Label
    Private WithEvents comboBox_FlatField_Selector As System.Windows.Forms.ComboBox
    Private WithEvents label_flatField As System.Windows.Forms.Label
   Private WithEvents saveLabel As System.Windows.Forms.Label
   Friend WithEvents label_load_cluster As System.Windows.Forms.Label
   Friend WithEvents button_load_cluster As System.Windows.Forms.Button
End Class

