<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class HighFrameDlg
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
        Me.UpDown_Frame = New System.Windows.Forms.NumericUpDown
        Me.label1 = New System.Windows.Forms.Label
        Me.button_Cancel = New System.Windows.Forms.Button
        Me.button_OK = New System.Windows.Forms.Button
        Me.UpDown_Frame_Onboard = New System.Windows.Forms.NumericUpDown
        Me.label2 = New System.Windows.Forms.Label
        CType(Me.UpDown_Frame, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.UpDown_Frame_Onboard, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'UpDown_Frame
        '
        Me.UpDown_Frame.Location = New System.Drawing.Point(180, 8)
        Me.UpDown_Frame.Name = "UpDown_Frame"
        Me.UpDown_Frame.Size = New System.Drawing.Size(65, 20)
        Me.UpDown_Frame.TabIndex = 7
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(14, 11)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(156, 13)
        Me.label1.TabIndex = 6
        Me.label1.Text = "Number of Frame per Callback :"
        '
        'button_Cancel
        '
        Me.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.button_Cancel.Location = New System.Drawing.Point(290, 34)
        Me.button_Cancel.Name = "button_Cancel"
        Me.button_Cancel.Size = New System.Drawing.Size(75, 23)
        Me.button_Cancel.TabIndex = 5
        Me.button_Cancel.Text = "Cancel"
        Me.button_Cancel.UseVisualStyleBackColor = True
        '
        'button_OK
        '
        Me.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.button_OK.Location = New System.Drawing.Point(290, 5)
        Me.button_OK.Name = "button_OK"
        Me.button_OK.Size = New System.Drawing.Size(75, 23)
        Me.button_OK.TabIndex = 4
        Me.button_OK.Text = "OK"
        Me.button_OK.UseVisualStyleBackColor = True
        '
        'UpDown_Frame_Onboard
        '
        Me.UpDown_Frame_Onboard.Location = New System.Drawing.Point(180, 37)
        Me.UpDown_Frame_Onboard.Maximum = New Decimal(New Integer() {32767, 0, 0, 0})
        Me.UpDown_Frame_Onboard.Minimum = New Decimal(New Integer() {2, 0, 0, 0})
        Me.UpDown_Frame_Onboard.Name = "UpDown_Frame_Onboard"
        Me.UpDown_Frame_Onboard.Size = New System.Drawing.Size(65, 20)
        Me.UpDown_Frame_Onboard.TabIndex = 9
        Me.UpDown_Frame_Onboard.Value = New Decimal(New Integer() {2, 0, 0, 0})
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(14, 40)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(140, 13)
        Me.label2.TabIndex = 8
        Me.label2.Text = "Number of Frame on Board :"
        '
        'HighFrameDlg
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(379, 72)
        Me.ControlBox = False
        Me.Controls.Add(Me.UpDown_Frame_Onboard)
        Me.Controls.Add(Me.label2)
        Me.Controls.Add(Me.UpDown_Frame)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.button_Cancel)
        Me.Controls.Add(Me.button_OK)
        Me.Name = "HighFrameDlg"
        Me.Text = "High Frame Rate Setting"
        CType(Me.UpDown_Frame, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.UpDown_Frame_Onboard, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents UpDown_Frame As System.Windows.Forms.NumericUpDown
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents button_Cancel As System.Windows.Forms.Button
    Private WithEvents button_OK As System.Windows.Forms.Button
    Private WithEvents UpDown_Frame_Onboard As System.Windows.Forms.NumericUpDown
    Private WithEvents label2 As System.Windows.Forms.Label
End Class
