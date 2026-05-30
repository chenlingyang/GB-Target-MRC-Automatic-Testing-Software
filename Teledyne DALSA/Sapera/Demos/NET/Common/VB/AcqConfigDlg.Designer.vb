<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class AcqConfigDlg
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AcqConfigDlg))
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.ComboBox_Device = New System.Windows.Forms.ComboBox
        Me.ComboBox_Server = New System.Windows.Forms.ComboBox
        Me.Button_Cancel = New System.Windows.Forms.Button
        Me.Button_OK = New System.Windows.Forms.Button
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.Button_Browse = New System.Windows.Forms.Button
        Me.TextBox_configfile = New System.Windows.Forms.TextBox
        Me.ComboBox_configfile = New System.Windows.Forms.ComboBox
        Me.CheckBox_Configfile = New System.Windows.Forms.CheckBox
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.label4 = New System.Windows.Forms.Label
        Me.label3 = New System.Windows.Forms.Label
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.ComboBox_Device)
        Me.GroupBox1.Controls.Add(Me.ComboBox_Server)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 7)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(435, 91)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Location"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(230, 22)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(93, 13)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Acquistion Device"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(15, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(92, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Acquisition Server"
        '
        'ComboBox_Device
        '
        Me.ComboBox_Device.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox_Device.FormattingEnabled = True
        Me.ComboBox_Device.Location = New System.Drawing.Point(222, 41)
        Me.ComboBox_Device.Name = "ComboBox_Device"
        Me.ComboBox_Device.Size = New System.Drawing.Size(205, 21)
        Me.ComboBox_Device.TabIndex = 1
        '
        'ComboBox_Server
        '
        Me.ComboBox_Server.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox_Server.FormattingEnabled = True
        Me.ComboBox_Server.Location = New System.Drawing.Point(6, 41)
        Me.ComboBox_Server.Name = "ComboBox_Server"
        Me.ComboBox_Server.Size = New System.Drawing.Size(203, 21)
        Me.ComboBox_Server.TabIndex = 2
        '
        'Button_Cancel
        '
        Me.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button_Cancel.Location = New System.Drawing.Point(459, 51)
        Me.Button_Cancel.Name = "Button_Cancel"
        Me.Button_Cancel.Size = New System.Drawing.Size(83, 28)
        Me.Button_Cancel.TabIndex = 3
        Me.Button_Cancel.Text = "Cancel"
        Me.Button_Cancel.UseVisualStyleBackColor = True
        '
        'Button_OK
        '
        Me.Button_OK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Button_OK.Location = New System.Drawing.Point(459, 12)
        Me.Button_OK.Name = "Button_OK"
        Me.Button_OK.Size = New System.Drawing.Size(83, 28)
        Me.Button_OK.TabIndex = 0
        Me.Button_OK.Text = "OK"
        Me.Button_OK.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Button_Browse)
        Me.GroupBox2.Controls.Add(Me.TextBox_configfile)
        Me.GroupBox2.Controls.Add(Me.ComboBox_configfile)
        Me.GroupBox2.Controls.Add(Me.CheckBox_Configfile)
        Me.GroupBox2.Location = New System.Drawing.Point(12, 101)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(530, 86)
        Me.GroupBox2.TabIndex = 1
        Me.GroupBox2.TabStop = False
        '
        'Button_Browse
        '
        Me.Button_Browse.Location = New System.Drawing.Point(441, 20)
        Me.Button_Browse.Name = "Button_Browse"
        Me.Button_Browse.Size = New System.Drawing.Size(75, 23)
        Me.Button_Browse.TabIndex = 3
        Me.Button_Browse.Text = "Browse"
        Me.Button_Browse.UseVisualStyleBackColor = True
        '
        'TextBox_configfile
        '
        Me.TextBox_configfile.Location = New System.Drawing.Point(9, 51)
        Me.TextBox_configfile.Name = "TextBox_configfile"
        Me.TextBox_configfile.Size = New System.Drawing.Size(411, 20)
        Me.TextBox_configfile.TabIndex = 2
        '
        'ComboBox_configfile
        '
        Me.ComboBox_configfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox_configfile.FormattingEnabled = True
        Me.ComboBox_configfile.Location = New System.Drawing.Point(9, 23)
        Me.ComboBox_configfile.Name = "ComboBox_configfile"
        Me.ComboBox_configfile.Size = New System.Drawing.Size(411, 21)
        Me.ComboBox_configfile.TabIndex = 1
        '
        'CheckBox_Configfile
        '
        Me.CheckBox_Configfile.AutoSize = True
        Me.CheckBox_Configfile.Location = New System.Drawing.Point(18, 0)
        Me.CheckBox_Configfile.Name = "CheckBox_Configfile"
        Me.CheckBox_Configfile.Size = New System.Drawing.Size(107, 17)
        Me.CheckBox_Configfile.TabIndex = 0
        Me.CheckBox_Configfile.Text = "Configuration File"
        Me.CheckBox_Configfile.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.label4)
        Me.GroupBox3.Controls.Add(Me.label3)
        Me.GroupBox3.Location = New System.Drawing.Point(12, 194)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(530, 61)
        Me.GroupBox3.TabIndex = 2
        Me.GroupBox3.TabStop = False
        '
        'label4
        '
        Me.label4.AutoSize = True
        Me.label4.Location = New System.Drawing.Point(106, 35)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(329, 13)
        Me.label4.TabIndex = 9
        Me.label4.Text = "you must run the CamExpert utility to generate your Configuration file."
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.Location = New System.Drawing.Point(135, 16)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(252, 13)
        Me.label3.TabIndex = 8
        Me.label3.Text = "If no Configuration file exists for your board/camera, "
        '
        'AcqConfigDlg
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(554, 261)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.Button_Cancel)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Button_OK)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "AcqConfigDlg"
        Me.Text = "AcqConfigDlg"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Button_Cancel As System.Windows.Forms.Button
    Friend WithEvents Button_OK As System.Windows.Forms.Button
    Friend WithEvents CheckBox_Configfile As System.Windows.Forms.CheckBox
    Friend WithEvents Button_Browse As System.Windows.Forms.Button
    Private WithEvents label4 As System.Windows.Forms.Label
    Private WithEvents label3 As System.Windows.Forms.Label
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Private WithEvents ComboBox_Device As System.Windows.Forms.ComboBox
    Private WithEvents ComboBox_Server As System.Windows.Forms.ComboBox
    Private WithEvents ComboBox_configfile As System.Windows.Forms.ComboBox
    Private WithEvents TextBox_configfile As System.Windows.Forms.TextBox
End Class

