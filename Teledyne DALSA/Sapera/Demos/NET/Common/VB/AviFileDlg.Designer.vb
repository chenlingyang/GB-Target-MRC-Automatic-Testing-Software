<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AviFileDlg
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
        Me.button_Cancel = New System.Windows.Forms.Button
        Me.button_OK = New System.Windows.Forms.Button
        Me.groupBox2 = New System.Windows.Forms.GroupBox
        Me.label3 = New System.Windows.Forms.Label
        Me.label2 = New System.Windows.Forms.Label
        Me.comboBox_Compression_type = New System.Windows.Forms.ComboBox
        Me.UpDown_Compression_level = New System.Windows.Forms.NumericUpDown
        Me.groupBox1 = New System.Windows.Forms.GroupBox
        Me.label1 = New System.Windows.Forms.Label
        Me.UpDown_FirstFrame = New System.Windows.Forms.NumericUpDown
        Me.groupBox2.SuspendLayout()
        CType(Me.UpDown_Compression_level, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.groupBox1.SuspendLayout()
        CType(Me.UpDown_FirstFrame, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'button_Cancel
        '
        Me.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.button_Cancel.Location = New System.Drawing.Point(153, 231)
        Me.button_Cancel.Name = "button_Cancel"
        Me.button_Cancel.Size = New System.Drawing.Size(75, 23)
        Me.button_Cancel.TabIndex = 7
        Me.button_Cancel.Text = "Cancel"
        Me.button_Cancel.UseVisualStyleBackColor = True
        '
        'button_OK
        '
        Me.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.button_OK.Location = New System.Drawing.Point(72, 231)
        Me.button_OK.Name = "button_OK"
        Me.button_OK.Size = New System.Drawing.Size(75, 23)
        Me.button_OK.TabIndex = 6
        Me.button_OK.Text = "OK"
        Me.button_OK.UseVisualStyleBackColor = True
        '
        'groupBox2
        '
        Me.groupBox2.Controls.Add(Me.label3)
        Me.groupBox2.Controls.Add(Me.label2)
        Me.groupBox2.Controls.Add(Me.comboBox_Compression_type)
        Me.groupBox2.Controls.Add(Me.UpDown_Compression_level)
        Me.groupBox2.Location = New System.Drawing.Point(10, 105)
        Me.groupBox2.Name = "groupBox2"
        Me.groupBox2.Size = New System.Drawing.Size(292, 100)
        Me.groupBox2.TabIndex = 5
        Me.groupBox2.TabStop = False
        Me.groupBox2.Text = "Save options"
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.Location = New System.Drawing.Point(28, 56)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(97, 13)
        Me.label3.TabIndex = 3
        Me.label3.Text = "Compression Type:"
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(28, 31)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(129, 13)
        Me.label2.TabIndex = 2
        Me.label2.Text = "Compression Level [1..99]"
        '
        'comboBox_Compression_type
        '
        Me.comboBox_Compression_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comboBox_Compression_type.FormattingEnabled = True
        Me.comboBox_Compression_type.Items.AddRange(New Object() {"No compression"})
        Me.comboBox_Compression_type.Location = New System.Drawing.Point(163, 56)
        Me.comboBox_Compression_type.Name = "comboBox_Compression_type"
        Me.comboBox_Compression_type.Size = New System.Drawing.Size(121, 21)
        Me.comboBox_Compression_type.TabIndex = 1
        '
        'UpDown_Compression_level
        '
        Me.UpDown_Compression_level.Location = New System.Drawing.Point(163, 29)
        Me.UpDown_Compression_level.Maximum = New Decimal(New Integer() {99, 0, 0, 0})
        Me.UpDown_Compression_level.Name = "UpDown_Compression_level"
        Me.UpDown_Compression_level.Size = New System.Drawing.Size(120, 20)
        Me.UpDown_Compression_level.TabIndex = 0
        '
        'groupBox1
        '
        Me.groupBox1.Controls.Add(Me.label1)
        Me.groupBox1.Controls.Add(Me.UpDown_FirstFrame)
        Me.groupBox1.Location = New System.Drawing.Point(10, 12)
        Me.groupBox1.Name = "groupBox1"
        Me.groupBox1.Size = New System.Drawing.Size(292, 79)
        Me.groupBox1.TabIndex = 4
        Me.groupBox1.TabStop = False
        Me.groupBox1.Text = "Load options"
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(25, 32)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(61, 13)
        Me.label1.TabIndex = 1
        Me.label1.Text = "First Frame:"
        '
        'UpDown_FirstFrame
        '
        Me.UpDown_FirstFrame.Location = New System.Drawing.Point(98, 32)
        Me.UpDown_FirstFrame.Name = "UpDown_FirstFrame"
        Me.UpDown_FirstFrame.Size = New System.Drawing.Size(120, 20)
        Me.UpDown_FirstFrame.TabIndex = 0
        '
        'AviFileDlg
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(310, 271)
        Me.ControlBox = False
        Me.Controls.Add(Me.button_Cancel)
        Me.Controls.Add(Me.button_OK)
        Me.Controls.Add(Me.groupBox2)
        Me.Controls.Add(Me.groupBox1)
        Me.Name = "AviFileDlg"
        Me.Text = "AviFileDlg"
        Me.groupBox2.ResumeLayout(False)
        Me.groupBox2.PerformLayout()
        CType(Me.UpDown_Compression_level, System.ComponentModel.ISupportInitialize).EndInit()
        Me.groupBox1.ResumeLayout(False)
        Me.groupBox1.PerformLayout()
        CType(Me.UpDown_FirstFrame, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents button_Cancel As System.Windows.Forms.Button
    Private WithEvents button_OK As System.Windows.Forms.Button
    Private WithEvents groupBox2 As System.Windows.Forms.GroupBox
    Private WithEvents label3 As System.Windows.Forms.Label
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents comboBox_Compression_type As System.Windows.Forms.ComboBox
    Private WithEvents UpDown_Compression_level As System.Windows.Forms.NumericUpDown
    Private WithEvents groupBox1 As System.Windows.Forms.GroupBox
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents UpDown_FirstFrame As System.Windows.Forms.NumericUpDown
End Class
