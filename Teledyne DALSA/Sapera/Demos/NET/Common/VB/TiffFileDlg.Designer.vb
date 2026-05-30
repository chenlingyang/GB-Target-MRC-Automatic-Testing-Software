<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TiffFileDlg
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
        Me.label3 = New System.Windows.Forms.Label
        Me.label2 = New System.Windows.Forms.Label
        Me.UpDown_compression_level = New System.Windows.Forms.NumericUpDown
        Me.label1 = New System.Windows.Forms.Label
        Me.comboBox_compression_type = New System.Windows.Forms.ComboBox
        Me.button_CANCEL = New System.Windows.Forms.Button
        Me.button_OK = New System.Windows.Forms.Button
        CType(Me.UpDown_compression_level, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.Location = New System.Drawing.Point(40, 92)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(62, 13)
        Me.label3.TabIndex = 13
        Me.label3.Text = "(JPEG only)"
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(37, 75)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(132, 13)
        Me.label2.TabIndex = 12
        Me.label2.Text = "Compression Level [1...99]"
        '
        'UpDown_compression_level
        '
        Me.UpDown_compression_level.Location = New System.Drawing.Point(183, 69)
        Me.UpDown_compression_level.Maximum = New Decimal(New Integer() {99, 0, 0, 0})
        Me.UpDown_compression_level.Name = "UpDown_compression_level"
        Me.UpDown_compression_level.Size = New System.Drawing.Size(97, 20)
        Me.UpDown_compression_level.TabIndex = 11
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(11, 24)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(97, 13)
        Me.label1.TabIndex = 10
        Me.label1.Text = "Compression Type:"
        '
        'comboBox_compression_type
        '
        Me.comboBox_compression_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comboBox_compression_type.FormattingEnabled = True
        Me.comboBox_compression_type.Items.AddRange(New Object() {"None", "RLE", "LZW", "JPEG"})
        Me.comboBox_compression_type.Location = New System.Drawing.Point(117, 20)
        Me.comboBox_compression_type.Name = "comboBox_compression_type"
        Me.comboBox_compression_type.Size = New System.Drawing.Size(164, 21)
        Me.comboBox_compression_type.TabIndex = 9
        '
        'button_CANCEL
        '
        Me.button_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.button_CANCEL.Location = New System.Drawing.Point(165, 137)
        Me.button_CANCEL.Name = "button_CANCEL"
        Me.button_CANCEL.Size = New System.Drawing.Size(75, 23)
        Me.button_CANCEL.TabIndex = 8
        Me.button_CANCEL.Text = "Cancel"
        Me.button_CANCEL.UseVisualStyleBackColor = True
        '
        'button_OK
        '
        Me.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.button_OK.Location = New System.Drawing.Point(84, 137)
        Me.button_OK.Name = "button_OK"
        Me.button_OK.Size = New System.Drawing.Size(75, 23)
        Me.button_OK.TabIndex = 7
        Me.button_OK.Text = "OK"
        Me.button_OK.UseVisualStyleBackColor = True
        '
        'TiffFileDlg
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(292, 181)
        Me.ControlBox = False
        Me.Controls.Add(Me.label3)
        Me.Controls.Add(Me.label2)
        Me.Controls.Add(Me.UpDown_compression_level)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.comboBox_compression_type)
        Me.Controls.Add(Me.button_CANCEL)
        Me.Controls.Add(Me.button_OK)
        Me.Name = "TiffFileDlg"
        Me.Text = "TiffFileDlg"
        CType(Me.UpDown_compression_level, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents label3 As System.Windows.Forms.Label
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents UpDown_compression_level As System.Windows.Forms.NumericUpDown
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents comboBox_compression_type As System.Windows.Forms.ComboBox
    Private WithEvents button_CANCEL As System.Windows.Forms.Button
    Private WithEvents button_OK As System.Windows.Forms.Button
End Class
