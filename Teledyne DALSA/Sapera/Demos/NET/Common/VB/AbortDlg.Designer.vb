<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AbortDlg
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
        Me.Button_Abort = New System.Windows.Forms.Button
        Me.Timer = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'Button_Abort
        '
        Me.Button_Abort.Location = New System.Drawing.Point(89, 26)
        Me.Button_Abort.Name = "Button_Abort"
        Me.Button_Abort.Size = New System.Drawing.Size(100, 23)
        Me.Button_Abort.TabIndex = 0
        Me.Button_Abort.Text = "Abort"
        Me.Button_Abort.UseVisualStyleBackColor = True
        '
        'Timer
        '
        '
        'AbortDlg
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(278, 77)
        Me.ControlBox = False
        Me.Controls.Add(Me.Button_Abort)
        Me.Name = "AbortDlg"
        Me.Opacity = 0
        Me.Text = "Waiting for grab to terminate..."
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Button_Abort As System.Windows.Forms.Button
    Friend WithEvents Timer As System.Windows.Forms.Timer
End Class
