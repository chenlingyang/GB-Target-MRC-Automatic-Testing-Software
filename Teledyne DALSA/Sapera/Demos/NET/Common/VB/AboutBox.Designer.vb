<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AboutBox
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AboutBox))
        Me.tableLayoutPanel = New System.Windows.Forms.TableLayoutPanel
        Me.logoPictureBox = New System.Windows.Forms.PictureBox
        Me.labelDescription = New System.Windows.Forms.Label
        Me.labelVersion = New System.Windows.Forms.Label
        Me.labelCopyright = New System.Windows.Forms.Label
        Me.okButton = New System.Windows.Forms.Button
        Me.tableLayoutPanel.SuspendLayout()
        CType(Me.logoPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tableLayoutPanel
        '
        Me.tableLayoutPanel.ColumnCount = 2
        Me.tableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.0!))
        Me.tableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67.0!))
        Me.tableLayoutPanel.Controls.Add(Me.logoPictureBox, 0, 0)
        Me.tableLayoutPanel.Controls.Add(Me.labelDescription, 1, 0)
        Me.tableLayoutPanel.Controls.Add(Me.labelVersion, 1, 1)
        Me.tableLayoutPanel.Controls.Add(Me.labelCopyright, 1, 2)
        Me.tableLayoutPanel.Controls.Add(Me.okButton, 1, 3)
        Me.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tableLayoutPanel.Location = New System.Drawing.Point(9, 9)
        Me.tableLayoutPanel.Name = "tableLayoutPanel"
        Me.tableLayoutPanel.RowCount = 4
        Me.tableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 21.24531!))
        Me.tableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 21.24531!))
        Me.tableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24.28035!))
        Me.tableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.22904!))
        Me.tableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.tableLayoutPanel.Size = New System.Drawing.Size(384, 127)
        Me.tableLayoutPanel.TabIndex = 1
        '
        'logoPictureBox
        '
        Me.logoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.logoPictureBox.Image = CType(resources.GetObject("logoPictureBox.Image"), System.Drawing.Image)
        Me.logoPictureBox.Location = New System.Drawing.Point(3, 3)
        Me.logoPictureBox.Name = "logoPictureBox"
        Me.tableLayoutPanel.SetRowSpan(Me.logoPictureBox, 4)
        Me.logoPictureBox.Size = New System.Drawing.Size(120, 121)
        Me.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.logoPictureBox.TabIndex = 12
        Me.logoPictureBox.TabStop = False
        '
        'labelDescription
        '
        Me.labelDescription.Dock = System.Windows.Forms.DockStyle.Fill
        Me.labelDescription.Location = New System.Drawing.Point(132, 0)
        Me.labelDescription.Margin = New System.Windows.Forms.Padding(6, 0, 3, 0)
        Me.labelDescription.MaximumSize = New System.Drawing.Size(0, 17)
        Me.labelDescription.Name = "labelDescription"
        Me.labelDescription.Size = New System.Drawing.Size(249, 17)
        Me.labelDescription.TabIndex = 19
        Me.labelDescription.Text = "Description"
        Me.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'labelVersion
        '
        Me.labelVersion.Dock = System.Windows.Forms.DockStyle.Fill
        Me.labelVersion.Location = New System.Drawing.Point(132, 26)
        Me.labelVersion.Margin = New System.Windows.Forms.Padding(6, 0, 3, 0)
        Me.labelVersion.MaximumSize = New System.Drawing.Size(0, 17)
        Me.labelVersion.Name = "labelVersion"
        Me.labelVersion.Size = New System.Drawing.Size(249, 17)
        Me.labelVersion.TabIndex = 0
        Me.labelVersion.Text = "Version"
        Me.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'labelCopyright
        '
        Me.labelCopyright.Dock = System.Windows.Forms.DockStyle.Fill
        Me.labelCopyright.Location = New System.Drawing.Point(132, 52)
        Me.labelCopyright.Margin = New System.Windows.Forms.Padding(6, 0, 3, 0)
        Me.labelCopyright.MaximumSize = New System.Drawing.Size(0, 17)
        Me.labelCopyright.Name = "labelCopyright"
        Me.labelCopyright.Size = New System.Drawing.Size(249, 17)
        Me.labelCopyright.TabIndex = 21
        Me.labelCopyright.Text = "Copyright"
        Me.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'okButton
        '
        Me.okButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.okButton.Location = New System.Drawing.Point(306, 103)
        Me.okButton.Name = "okButton"
        Me.okButton.Size = New System.Drawing.Size(75, 21)
        Me.okButton.TabIndex = 24
        Me.okButton.Text = "&OK"
        '
        'AboutBox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(402, 145)
        Me.Controls.Add(Me.tableLayoutPanel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "AboutBox"
        Me.Padding = New System.Windows.Forms.Padding(9)
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "AboutBox"
        Me.tableLayoutPanel.ResumeLayout(False)
        CType(Me.logoPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents tableLayoutPanel As System.Windows.Forms.TableLayoutPanel
    Private WithEvents logoPictureBox As System.Windows.Forms.PictureBox
    Private WithEvents labelDescription As System.Windows.Forms.Label
    Private WithEvents labelVersion As System.Windows.Forms.Label
    Private WithEvents labelCopyright As System.Windows.Forms.Label
    Private WithEvents okButton As System.Windows.Forms.Button

End Class
