<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RawFileDlg
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
        Me.comboBox_RawFormat = New System.Windows.Forms.ComboBox
        Me.textBox_Offset = New System.Windows.Forms.TextBox
        Me.textBox_Height = New System.Windows.Forms.TextBox
        Me.textBox_Width = New System.Windows.Forms.TextBox
        Me.button_Cancel = New System.Windows.Forms.Button
        Me.button_OK = New System.Windows.Forms.Button
        Me.Label4 = New System.Windows.Forms.Label
        Me.label3 = New System.Windows.Forms.Label
        Me.label2 = New System.Windows.Forms.Label
        Me.label1 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'comboBox_RawFormat
        '
        Me.comboBox_RawFormat.FormattingEnabled = True
        Me.comboBox_RawFormat.Location = New System.Drawing.Point(69, 101)
        Me.comboBox_RawFormat.Name = "comboBox_RawFormat"
        Me.comboBox_RawFormat.Size = New System.Drawing.Size(144, 21)
        Me.comboBox_RawFormat.TabIndex = 17
        '
        'textBox_Offset
        '
        Me.textBox_Offset.Location = New System.Drawing.Point(113, 68)
        Me.textBox_Offset.Name = "textBox_Offset"
        Me.textBox_Offset.Size = New System.Drawing.Size(77, 20)
        Me.textBox_Offset.TabIndex = 15
        '
        'textBox_Height
        '
        Me.textBox_Height.Location = New System.Drawing.Point(113, 39)
        Me.textBox_Height.Name = "textBox_Height"
        Me.textBox_Height.Size = New System.Drawing.Size(77, 20)
        Me.textBox_Height.TabIndex = 14
        '
        'textBox_Width
        '
        Me.textBox_Width.Location = New System.Drawing.Point(113, 9)
        Me.textBox_Width.Name = "textBox_Width"
        Me.textBox_Width.Size = New System.Drawing.Size(77, 20)
        Me.textBox_Width.TabIndex = 13
        '
        'button_Cancel
        '
        Me.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.button_Cancel.Location = New System.Drawing.Point(113, 136)
        Me.button_Cancel.Name = "button_Cancel"
        Me.button_Cancel.Size = New System.Drawing.Size(62, 23)
        Me.button_Cancel.TabIndex = 12
        Me.button_Cancel.Text = "Cancel"
        Me.button_Cancel.UseVisualStyleBackColor = True
        '
        'button_OK
        '
        Me.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.button_OK.Location = New System.Drawing.Point(43, 136)
        Me.button_OK.Name = "button_OK"
        Me.button_OK.Size = New System.Drawing.Size(61, 23)
        Me.button_OK.TabIndex = 11
        Me.button_OK.Text = "OK"
        Me.button_OK.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(12, 109)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(45, 13)
        Me.Label4.TabIndex = 16
        Me.Label4.Text = "Format: "
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.Location = New System.Drawing.Point(10, 71)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(80, 13)
        Me.label3.TabIndex = 10
        Me.label3.Text = "Offset (in bytes)"
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(10, 39)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(84, 13)
        Me.label2.TabIndex = 9
        Me.label2.Text = "Height (in pixels)"
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(12, 9)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(82, 13)
        Me.label1.TabIndex = 8
        Me.label1.Text = "Width (in Pixels)"
        '
        'RawFileDlg
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(225, 171)
        Me.ControlBox = False
        Me.Controls.Add(Me.comboBox_RawFormat)
        Me.Controls.Add(Me.textBox_Offset)
        Me.Controls.Add(Me.textBox_Height)
        Me.Controls.Add(Me.textBox_Width)
        Me.Controls.Add(Me.button_Cancel)
        Me.Controls.Add(Me.button_OK)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.label3)
        Me.Controls.Add(Me.label2)
        Me.Controls.Add(Me.label1)
        Me.Name = "RawFileDlg"
        Me.Text = "RawFileDlg"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents comboBox_RawFormat As System.Windows.Forms.ComboBox
    Private WithEvents textBox_Offset As System.Windows.Forms.TextBox
    Private WithEvents textBox_Height As System.Windows.Forms.TextBox
    Private WithEvents textBox_Width As System.Windows.Forms.TextBox
    Private WithEvents button_Cancel As System.Windows.Forms.Button
    Private WithEvents button_OK As System.Windows.Forms.Button
    Private WithEvents Label4 As System.Windows.Forms.Label
    Private WithEvents label3 As System.Windows.Forms.Label
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents label1 As System.Windows.Forms.Label
End Class
