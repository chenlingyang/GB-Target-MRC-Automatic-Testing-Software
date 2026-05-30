Public Class ViewDlg

    Private m_pView As SapView
    Private m_ViewArea As Rectangle
    Private m_Range As Integer
    Private m_RangeInit As Integer
    Private m_bLockAspectRatio As Boolean

    Public Sub New(ByVal pView As SapView, ByVal ViewArea As Rectangle)

        InitializeComponent()

        m_pView = pView
        Slider_Range.Maximum = m_pView.RangeMax
        Slider_Range.Minimum = m_pView.RangeMin

        m_Range = m_pView.Range
        m_RangeInit = m_Range
        Slider_Range.Value = m_Range
        textBox_Range.Text = m_Range.ToString()

        NUpDown_height_scalor.Value = CDec((m_pView.ScalingZoomVert * 100.0F))
        NUpDown_width_scalor.Value = CDec((m_pView.ScalingZoomHorz * 100.0F))

        NUpDown_height.Value = [Decimal].Floor(CDec(((m_pView.Buffer.Height * CSng(NUpDown_height_scalor.Value) / 100) + 0.5F)))
        NUpDown_width.Value = [Decimal].Floor(CDec(((m_pView.Buffer.Width * CSng(NUpDown_width_scalor.Value) / 100) + 0.5F)))

        m_bLockAspectRatio = checkBox_lock.Checked
        m_ViewArea = ViewArea

        Initialize_ViewFormat_Combo()

        LoadSettings()
    End Sub
    Private Sub Initialize_ViewFormat_Combo()
        Dim buffer As SapBuffer = m_pView.Buffer

        If buffer.MultiFormat Then
            Dim numPages As Integer = buffer.NumPages
            Dim pageFormats() As SapFormat = buffer.PageFormats
            Dim index As Integer
            For index = 0 To numPages - 1
                comboBox1_ViewFormat.Items.Add(pageFormats(index).ToString & " (page " & index & ")")
            Next
            Dim currentPage As Integer = buffer.Page
            comboBox1_ViewFormat.SelectedIndex = currentPage
            comboBox1_ViewFormat.Enabled = True
        Else
            comboBox1_ViewFormat.Enabled = False
        End If
    End Sub

    Private Sub LoadSettings()
        ' Read file name
        Dim sAspectRatio As String = ""
        Dim KeyPath As String = "Software\Teledyne DALSA\" + Application.ProductName + "\SapView"
        Dim regkey As RegistryKey = Registry.CurrentUser.OpenSubKey(KeyPath)

        If regkey IsNot Nothing Then
            ' Read view settings
            If regkey.GetValue("Aspect Ratio") IsNot Nothing Then
                sAspectRatio = regkey.GetValue("Aspect Ratio", "False").ToString()
            End If

            If sAspectRatio = "True" Then
                checkBox_lock.Checked = True
            Else
                checkBox_lock.Checked = False
            End If
        End If
    End Sub

    Private Sub SaveSettings()
        ' Write file name
        Dim keyPath As String = "Software\Teledyne DALSA\" + Application.ProductName + "\SapView"
        Dim regKeyAppRoot As RegistryKey = Registry.CurrentUser.CreateSubKey(keyPath)
        regKeyAppRoot.SetValue("Aspect Ratio", checkBox_lock.Checked)
    End Sub

    Private Sub SetRange()
        m_pView.Range = m_Range
    End Sub

    Private Sub NUpDown_width_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NUpDown_width.ValueChanged
        NUpDown_width_scalor.Value = CDec(((100.0F * CSng(NUpDown_width.Value) / m_pView.Buffer.Width)))

        If m_bLockAspectRatio Then
            NUpDown_height_scalor.Value = NUpDown_width_scalor.Value
            NUpDown_height.Value = [Decimal].Floor(CDec((CSng(NUpDown_height_scalor.Value) * m_pView.Buffer.Height / 100 + 0.5F)))
        End If
    End Sub

    Private Sub NUpDown_width_scalor_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NUpDown_width_scalor.ValueChanged
        NUpDown_width.Value = [Decimal].Floor(CDec((CSng(NUpDown_width_scalor.Value) * m_pView.Buffer.Width / 100 + 0.5F)))

        If m_bLockAspectRatio Then
            NUpDown_height_scalor.Value = NUpDown_width_scalor.Value
            NUpDown_height.Value = [Decimal].Floor(CDec((CSng(NUpDown_height_scalor.Value) * m_pView.Buffer.Height / 100 + 0.5F)))
        End If
    End Sub

    Private Sub NUpDown_height_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NUpDown_height.ValueChanged
        NUpDown_height_scalor.Value = CDec((100.0F * CSng(NUpDown_height.Value) / m_pView.Buffer.Height))

        If m_bLockAspectRatio Then
            NUpDown_height_scalor.Value = NUpDown_height_scalor.Value
            NUpDown_width.Value = [Decimal].Floor(CDec((CSng(NUpDown_height_scalor.Value) * m_pView.Buffer.Width / 100 + 0.5F)))
        End If
    End Sub

    Private Sub NUpDown_height_scalor_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NUpDown_height_scalor.ValueChanged
        NUpDown_height.Value = [Decimal].Floor(CDec((CSng(NUpDown_height_scalor.Value) * m_pView.Buffer.Height / 100 + 0.5F)))

        If m_bLockAspectRatio Then
            NUpDown_height_scalor.Value = NUpDown_height_scalor.Value
            NUpDown_width.Value = [Decimal].Floor(CDec((CSng(NUpDown_height_scalor.Value) * m_pView.Buffer.Width / 100 + 0.5F)))
        End If
    End Sub

    Private Sub button_Fit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_Fit.Click
      Dim WidthScalor As Decimal = CDec((100.0F * m_ViewArea.Width / m_pView.Buffer.Width))
      Dim HeightScalor As Decimal = CDec((100.0F * m_ViewArea.Height / m_pView.Buffer.Height))

      If m_bLockAspectRatio Then
         If WidthScalor < HeightScalor Then
            HeightScalor = WidthScalor
         Else
            WidthScalor = HeightScalor
         End If
      End If

      NUpDown_width_scalor.Value = WidthScalor
      NUpDown_height_scalor.Value = HeightScalor

      NUpDown_width.Value = [Decimal].Floor(CDec((m_pView.Buffer.Width * CSng(NUpDown_width_scalor.Value) / 100 + 0.5F)))
      NUpDown_height.Value = [Decimal].Floor(CDec((m_pView.Buffer.Height * CSng(NUpDown_height_scalor.Value) / 100 + 0.5F)))
    End Sub

    Private Sub button_No_Scaling_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_No_Scaling.Click
        NUpDown_width_scalor.Value = CDec(100.0F)
        NUpDown_height_scalor.Value = CDec(100.0F)

        NUpDown_width.Value = [Decimal].Floor(CDec((m_pView.Buffer.Width * CSng(NUpDown_width_scalor.Value) / 100 + 0.5F)))
        NUpDown_height.Value = [Decimal].Floor(CDec((m_pView.Buffer.Height * CSng(NUpDown_height_scalor.Value) / 100 + 0.5F)))
    End Sub

    Private Sub checkBox_lock_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles checkBox_lock.CheckedChanged
        m_bLockAspectRatio = checkBox_lock.Checked

        If m_bLockAspectRatio Then
            NUpDown_height_scalor.Value = NUpDown_width_scalor.Value
            NUpDown_height.Value = [Decimal].Floor(CDec((CSng(NUpDown_height_scalor.Value) * m_pView.Buffer.Height / 100 + 0.5F)))
        End If
    End Sub

    Private Sub button_OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_OK.Click
        m_pView.SetScalingMode(CSng(NUpDown_width_scalor.Value) / 100.0F, CSng(NUpDown_height_scalor.Value) / 100.0F)
        SaveSettings()
    End Sub

    Private Sub button_Apply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_Apply.Click
        m_pView.SetScalingMode(CSng(NUpDown_width_scalor.Value) / 100.0F, CSng(NUpDown_height_scalor.Value) / 100.0F)
        SaveSettings()
    End Sub

    Private Sub button_Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_Cancel.Click
        m_Range = m_RangeInit
        SetRange()
    End Sub

    Private Sub Slider_Range_Scroll_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Slider_Range.Scroll
        m_Range = Slider_Range.Value
        textBox_Range.Text = m_Range.ToString()
    End Sub

    Private Sub textBox_Range_TextChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles textBox_Range.TextChanged
        Dim oldRange As Integer = m_Range

        m_Range = Integer.Parse(textBox_Range.Text)
        If m_Range < m_pView.RangeMin Or m_Range > m_pView.RangeMax Then
            m_Range = oldRange
        End If

        SetRange()
    End Sub
    Private Sub comboBox1_ViewFormat_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles comboBox1_ViewFormat.SelectedIndexChanged
        Dim index As Integer = comboBox1_ViewFormat.SelectedIndex
        If index <> -1 Then
            m_pView.Buffer.AllPage = index
        End If
    End Sub
End Class