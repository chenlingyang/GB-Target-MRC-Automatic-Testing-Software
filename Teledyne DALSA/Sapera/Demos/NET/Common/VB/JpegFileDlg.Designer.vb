<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class JPEGFileDlg
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
        Me.groupBox1 = New System.Windows.Forms.GroupBox
        Me.label1 = New System.Windows.Forms.Label
        Me.UpDown_compression_level = New System.Windows.Forms.NumericUpDown
        Me.groupBox1.SuspendLayout()
        CType(Me.UpDown_compression_level, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'button_Cancel
        '
        Me.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.button_Cancel.Location = New System.Drawing.Point(153, 125)
        Me.button_Cancel.Name = "button_Cancel"
        Me.button_Cancel.Size = New System.Drawing.Size(75, 23)
        Me.button_Cancel.TabIndex = 5
        Me.button_Cancel.Text = "Cancel"
        Me.button_Cancel.UseVisualStyleBackColor = True
        '
        'button_OK
        '
        Me.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.button_OK.Location = New System.Drawing.Point(72, 125)
        Me.button_OK.Name = "button_OK"
        Me.button_OK.Size = New System.Drawing.Size(75, 23)
        Me.button_OK.TabIndex = 4
        Me.button_OK.Text = "OK"
        Me.button_OK.UseVisualStyleBackColor = True
        '
        'groupBox1
        '
        Me.groupBox1.Controls.Add(Me.label1)
        Me.groupBox1.Controls.Add(Me.UpDown_compression_level)
        Me.groupBox1.Location = New System.Drawing.Point(23, 12)
        Me.groupBox1.Name = "groupBox1"
        Me.groupBox1.Size = New System.Drawing.Size(238, 93)
        Me.groupBox1.TabIndex = 3
        Me.groupBox1.TabStop = False
        Me.groupBox1.Text = "Compression level [1...99]"
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(46, 48)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(33, 13)
        Me.label1.TabIndex = 1
        Me.label1.Text = "Level"
        '
        'UpDown_compression_level
        '
        Me.UpDown_compression_level.Location = New System.Drawing.Point(99, 41)
        Me.UpDown_compression_level.Name = "UpDown_compression_level"
        Me.UpDown_compression_level.Size = New System.Drawing.Size(120, 20)
        Me.UpDown_compression_level.TabIndex = 0
        '
        'JPEGFileDlg
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(288, 164)
        Me.ControlBox = False
        Me.Controls.Add(Me.button_Cancel)
        Me.Controls.Add(Me.button_OK)
        Me.Controls.Add(Me.groupBox1)
        Me.Name = "JPEGFileDlg"
        Me.Text = "JPEGFileDlg"
        Me.groupBox1.ResumeLayout(False)
        Me.groupBox1.PerformLayout()
        CType(Me.UpDown_compression_level, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents button_Cancel As System.Windows.Forms.Button
    Private WithEvents button_OK As System.Windows.Forms.Button
    Private WithEvents groupBox1 As System.Windows.Forms.GroupBox
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents UpDown_compression_level As System.Windows.Forms.NumericUpDown
End Class
