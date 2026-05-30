<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MultiBoardSyncGrabDemoDlg
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MultiBoardSyncGrabDemoDlg))
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.Button_View = New System.Windows.Forms.Button
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.Button_New = New System.Windows.Forms.Button
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.Button_Freeze = New System.Windows.Forms.Button
        Me.Button_Grab = New System.Windows.Forms.Button
        Me.Button_Snap = New System.Windows.Forms.Button
        Me.Button_Exit = New System.Windows.Forms.Button
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip
        Me.StatusLabel = New System.Windows.Forms.ToolStripLabel
        Me.StatusLabelInfo = New System.Windows.Forms.ToolStripLabel
        Me.StatusLabelInfoTrash = New System.Windows.Forms.ToolStripLabel
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.PixelLabel = New System.Windows.Forms.ToolStripLabel
        Me.PixelDataValue = New System.Windows.Forms.ToolStripLabel
        Me.Panel1.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.GroupBox3)
        Me.Panel1.Controls.Add(Me.GroupBox2)
        Me.Panel1.Controls.Add(Me.GroupBox1)
        Me.Panel1.Controls.Add(Me.Button_Exit)
        Me.Panel1.Location = New System.Drawing.Point(65, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(149, 320)
        Me.Panel1.TabIndex = 8
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Button_View)
        Me.GroupBox3.Location = New System.Drawing.Point(9, 259)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(130, 52)
        Me.GroupBox3.TabIndex = 3
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "General Options"
        '
        'Button_View
        '
        Me.Button_View.Location = New System.Drawing.Point(21, 20)
        Me.Button_View.Name = "Button_View"
        Me.Button_View.Size = New System.Drawing.Size(94, 23)
        Me.Button_View.TabIndex = 1
        Me.Button_View.Text = "View"
        Me.Button_View.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Button_New)
        Me.GroupBox2.Location = New System.Drawing.Point(9, 194)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(130, 52)
        Me.GroupBox2.TabIndex = 2
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "File Control"
        '
        'Button_New
        '
        Me.Button_New.Location = New System.Drawing.Point(21, 19)
        Me.Button_New.Name = "Button_New"
        Me.Button_New.Size = New System.Drawing.Size(94, 23)
        Me.Button_New.TabIndex = 0
        Me.Button_New.Text = "New"
        Me.Button_New.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Button_Freeze)
        Me.GroupBox1.Controls.Add(Me.Button_Grab)
        Me.GroupBox1.Controls.Add(Me.Button_Snap)
        Me.GroupBox1.Location = New System.Drawing.Point(9, 51)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(131, 132)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Acquisition Control"
        '
        'Button_Freeze
        '
        Me.Button_Freeze.Location = New System.Drawing.Point(21, 93)
        Me.Button_Freeze.Name = "Button_Freeze"
        Me.Button_Freeze.Size = New System.Drawing.Size(94, 23)
        Me.Button_Freeze.TabIndex = 2
        Me.Button_Freeze.Text = "Freeze"
        Me.Button_Freeze.UseVisualStyleBackColor = True
        '
        'Button_Grab
        '
        Me.Button_Grab.Location = New System.Drawing.Point(21, 58)
        Me.Button_Grab.Name = "Button_Grab"
        Me.Button_Grab.Size = New System.Drawing.Size(94, 23)
        Me.Button_Grab.TabIndex = 1
        Me.Button_Grab.Text = "Grab"
        Me.Button_Grab.UseVisualStyleBackColor = True
        '
        'Button_Snap
        '
        Me.Button_Snap.Location = New System.Drawing.Point(21, 22)
        Me.Button_Snap.Name = "Button_Snap"
        Me.Button_Snap.Size = New System.Drawing.Size(94, 23)
        Me.Button_Snap.TabIndex = 0
        Me.Button_Snap.Text = "Snap"
        Me.Button_Snap.UseVisualStyleBackColor = True
        '
        'Button_Exit
        '
        Me.Button_Exit.Location = New System.Drawing.Point(30, 9)
        Me.Button_Exit.Name = "Button_Exit"
        Me.Button_Exit.Size = New System.Drawing.Size(94, 36)
        Me.Button_Exit.TabIndex = 0
        Me.Button_Exit.Text = "Exit"
        Me.Button_Exit.UseVisualStyleBackColor = True
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(1, 0)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(58, 396)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 6
        Me.PictureBox1.TabStop = False
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusLabel, Me.StatusLabelInfo, Me.StatusLabelInfoTrash, Me.ToolStripSeparator1, Me.PixelLabel, Me.PixelDataValue})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 436)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.ToolStrip1.Size = New System.Drawing.Size(982, 25)
        Me.ToolStrip1.TabIndex = 9
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'StatusLabel
        '
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusLabel.Size = New System.Drawing.Size(45, 22)
        Me.StatusLabel.Text = "Status :"
        '
        'StatusLabelInfo
        '
        Me.StatusLabelInfo.Name = "StatusLabelInfo"
        Me.StatusLabelInfo.Size = New System.Drawing.Size(49, 22)
        Me.StatusLabelInfo.Text = "nothing"
        '
        'StatusLabelInfoTrash
        '
        Me.StatusLabelInfoTrash.Name = "StatusLabelInfoTrash"
        Me.StatusLabelInfoTrash.Size = New System.Drawing.Size(0, 22)
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'PixelLabel
        '
        Me.PixelLabel.Name = "PixelLabel"
        Me.PixelLabel.Size = New System.Drawing.Size(37, 22)
        Me.PixelLabel.Text = "Pixel :"
        '
        'PixelDataValue
        '
        Me.PixelDataValue.Name = "PixelDataValue"
        Me.PixelDataValue.Size = New System.Drawing.Size(92, 22)
        Me.PixelDataValue.Text = "Data not avaible"
        '
        'MultiBoardSyncGrabDemoDlg
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(982, 461)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "MultiBoardSyncGrabDemoDlg"
        Me.Text = "Multi-board Sync Acquisition Demo .NET"
        Me.Panel1.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
   Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents Button_View As System.Windows.Forms.Button
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Button_New As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Button_Freeze As System.Windows.Forms.Button
    Friend WithEvents Button_Grab As System.Windows.Forms.Button
    Friend WithEvents Button_Snap As System.Windows.Forms.Button
    Friend WithEvents Button_Exit As System.Windows.Forms.Button
   Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents StatusLabel As System.Windows.Forms.ToolStripLabel
    Friend WithEvents StatusLabelInfo As System.Windows.Forms.ToolStripLabel
    Friend WithEvents StatusLabelInfoTrash As System.Windows.Forms.ToolStripLabel
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents PixelLabel As System.Windows.Forms.ToolStripLabel
   Friend WithEvents PixelDataValue As System.Windows.Forms.ToolStripLabel
   Friend WithEvents m_ImageBox As DALSA.SaperaLT.Demos.NET.VB.MultiBoardSyncGrabDemo.ImageBox

End Class
