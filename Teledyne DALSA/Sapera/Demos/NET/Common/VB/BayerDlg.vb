Partial Public Class BayerDlg
   Inherits Form

   Private m_pBayer As SapBayer
   Private m_pXfer As SapTransfer
   Private m_pPro As SapProcessing
   Private m_pImageWnd As ImageBox
   Private m_Align As SapBayer.AlignMode
   Private m_Method As SapBayer.CalculationMethod

   Public Sub New(ByVal pBayer As SapBayer, ByVal pXfer As SapTransfer, ByVal pImageWnd As ImageBox, ByVal pPro As SapProcessing)
      InitializeComponent()

      m_pBayer = pBayer
      m_pXfer = pXfer
      m_pPro = pPro
      m_pImageWnd = pImageWnd
   End Sub

   Private Sub BayerDlg_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
      If m_pBayer Is Nothing Then
         MessageBox.Show("No Bayer object specified")
         Me.Close()
         Exit Sub
      End If
      m_Align = m_pBayer.Align
      m_Method = m_pBayer.Method
      m_pImageWnd.TrackerEnable = True
      UpdateInterface()
   End Sub

   Private Sub UpdateInterface()
      Dim AlignCap As SapBayer.AlignMode = m_pBayer.AvailAlign
      Dim MethodCap As SapBayer.CalculationMethod = m_pBayer.AvailMethod

      Select Case m_Align
         Case SapBayer.AlignMode.BGGR
            radioButton_Align_BG_GR.Checked = True
            Exit Select
         Case SapBayer.AlignMode.GBRG
            radioButton_Align_GB_RG.Checked = True
            Exit Select
         Case SapBayer.AlignMode.GRBG
            radioButton_Align_GR_BG.Checked = True
            Exit Select
         Case SapBayer.AlignMode.RGGB
            radioButton_Align_RG_GB.Checked = True
            Exit Select
      End Select

      ' Check which alignment is available
      radioButton_Align_GB_RG.Enabled = (AlignCap And SapBayer.AlignMode.GBRG) > 0
      radioButton_Align_BG_GR.Enabled = (AlignCap And SapBayer.AlignMode.BGGR) > 0
      radioButton_Align_GR_BG.Enabled = (AlignCap And SapBayer.AlignMode.GRBG) > 0
      radioButton_Align_RG_GB.Enabled = (AlignCap And SapBayer.AlignMode.RGGB) > 0

      Select Case m_Method
         Case SapBayer.CalculationMethod.Method1
            radioButton_Method1.Checked = True
            Exit Select
         Case SapBayer.CalculationMethod.Method2
            radioButton_Method2.Checked = True
            Exit Select
         Case SapBayer.CalculationMethod.Method3
            radioButton_Method3.Checked = True
            Exit Select
         Case SapBayer.CalculationMethod.Method4
            radioButton_Method4.Checked = True
            Exit Select
         Case SapBayer.CalculationMethod.Method5
            radioButton_Method5.Checked = True
            Exit Select
         Case SapBayer.CalculationMethod.Method6
            radioButton_Method6.Checked = True
            Exit Select
      End Select

      ' Check which interpolation method is available
      radioButton_Method1.Enabled = (MethodCap And SapBayer.CalculationMethod.Method1) > 0
      radioButton_Method2.Enabled = (MethodCap And SapBayer.CalculationMethod.Method2) > 0
      radioButton_Method3.Enabled = (MethodCap And SapBayer.CalculationMethod.Method3) > 0
      radioButton_Method4.Enabled = (MethodCap And SapBayer.CalculationMethod.Method4) > 0
      radioButton_Method5.Enabled = (MethodCap And SapBayer.CalculationMethod.Method5) > 0
      radioButton_Method6.Enabled = (MethodCap And SapBayer.CalculationMethod.Method6) > 0

      ' Initialize gain values
      Dim wbGain As SapDataFRGB = m_pBayer.WBGain

      textBox_Red_Gain.Text = wbGain.Red.ToString()
      textBox_Green_Gain.Text = wbGain.Green.ToString()
      textBox_Blue_Gain.Text = wbGain.Blue.ToString()

      button_AutoWhite.Enabled = m_pImageWnd IsNot Nothing

      ' Initialize Gamma correction factor
      textBox_Gamma.Text = m_pBayer.Gamma.ToString()
      checkBox_Gamma.Checked = m_pBayer.LutEnable

      ' Check if bayer decoder is enabled and if a lookup table is available
      textBox_Gamma.Enabled = m_pBayer.Enabled AndAlso checkBox_Gamma.Checked
      checkBox_Gamma.Enabled = m_pBayer.Enabled

      ' Disable Apply button until something change
      button_Apply.Enabled = False
   End Sub

   Private Sub textBox_Red_Gain_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles textBox_Red_Gain.TextChanged
      OnChange()
   End Sub

   Private Sub textBox_Green_Gain_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles textBox_Green_Gain.TextChanged
      OnChange()
   End Sub

   Private Sub textBox_Blue_Gain_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles textBox_Blue_Gain.TextChanged
      OnChange()
   End Sub

   Private Sub textBox_Gamma_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles textBox_Gamma.TextChanged
      OnChange()
   End Sub

   Private Sub radioButton_Method1_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles radioButton_Method1.CheckedChanged
      m_Method = SapBayer.CalculationMethod.Method1
      OnChange()
   End Sub

   Private Sub radioButton_Method2_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles radioButton_Method2.CheckedChanged
      m_Method = SapBayer.CalculationMethod.Method2
      OnChange()
   End Sub

   Private Sub radioButton_Method3_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles radioButton_Method3.CheckedChanged
      m_Method = SapBayer.CalculationMethod.Method3
      OnChange()
   End Sub

   Private Sub radioButton_Method4_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles radioButton_Method4.CheckedChanged
      m_Method = SapBayer.CalculationMethod.Method4
      OnChange()
   End Sub

   Private Sub radioButton_Method5_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles radioButton_Method5.CheckedChanged
      m_Method = SapBayer.CalculationMethod.Method5
      OnChange()
   End Sub

   Private Sub radioButton_Method6_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles radioButton_Method6.CheckedChanged
      m_Method = SapBayer.CalculationMethod.Method6
      OnChange()
   End Sub

   Private Sub radioButton_Align_GB_RG_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles radioButton_Align_GB_RG.CheckedChanged
      m_Align = SapBayer.AlignMode.GBRG
      OnChange()
   End Sub

   Private Sub radioButton_Align_BG_GR_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles radioButton_Align_BG_GR.CheckedChanged
      m_Align = SapBayer.AlignMode.BGGR
      OnChange()
   End Sub

   Private Sub radioButton_Align_RG_GB_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles radioButton_Align_RG_GB.CheckedChanged
      m_Align = SapBayer.AlignMode.RGGB
      OnChange()
   End Sub

   Private Sub radioButton_Align_GR_BG_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles radioButton_Align_GR_BG.CheckedChanged
      m_Align = SapBayer.AlignMode.GRBG
      OnChange()
   End Sub

   Private Sub OnChange()
      ' Something change so enable the Apply button
      button_Apply.Enabled = True
   End Sub

   Private Sub checkBox_Gamma_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles checkBox_Gamma.CheckedChanged
      textBox_Gamma.Enabled = m_pBayer.Enabled AndAlso checkBox_Gamma.Checked
      ' Something change so enable the Apply button
      button_Apply.Enabled = True
   End Sub

   Private Sub button_AutoWhite_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_AutoWhite.Click
      If m_pImageWnd Is Nothing Then
         Exit Sub
      End If

      If m_pImageWnd.IsTrackerEmpty Then
         MessageBox.Show("You must select a ROI containing white pixels")
         Exit Sub
      End If

      If m_pBayer.Enabled AndAlso Not m_pBayer.SoftwareConversion Then
         MessageBox.Show("White balance is not available when hardware Bayer conversion is enabled")
         Exit Sub
      End If

      Dim rect As Rectangle = m_pImageWnd.Tracker

      If rect.Width > 1 AndAlso rect.Height > 1 Then
         ' Compute new white balance factors from region of interest
         If m_pBayer.WhiteBalance(rect.Left, rect.Top, rect.Width, rect.Height) Then
            ' Update user interface
            UpdateInterface()

            ' Redraw the image
            UpdateView()
         End If
      End If
   End Sub

   Private Sub button_Apply_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Apply.Click
      ' Set alignment 
      m_pBayer.Align = m_Align

      ' Set interpolation method
      m_pBayer.Method = m_Method

      ' Set white balance's gain
      m_pBayer.WBGain = New SapDataFRGB(Single.Parse(textBox_Red_Gain.Text), Single.Parse(textBox_Green_Gain.Text), Single.Parse(textBox_Blue_Gain.Text))

      If checkBox_Gamma.Checked Then
         ' Set new gamma factor
         m_pBayer.Gamma = Integer.Parse(textBox_Gamma.Text)
      End If

      ' Enable/Disable lookup table
      m_pBayer.LutEnable = checkBox_Gamma.Checked

      ' Redraw the image
      UpdateView()

      ' Disable apply button
      button_Apply.Enabled = False
   End Sub

   Private Sub UpdateView()
      If m_pBayer.Enabled Then
         ' Check if we are operating on-line
         If m_pXfer IsNot Nothing AndAlso m_pXfer.Initialized Then
            ' Check if we are grabbing
            If m_pXfer.Grabbing Then
               Exit Sub
               ' The view will be automatically updated on the next acquired frame
            End If

            ' Check if we are using an hardware bayer decoder
            If Not m_pBayer.SoftwareConversion Then
               ' Acquire one frame
               m_pXfer.Snap()
               Exit Sub
            End If
         End If

         ' Else, apply bayer conversion to current buffer's content
         If m_pPro IsNot Nothing Then
            m_pPro.Execute()
         Else
            m_pBayer.Convert()
            If m_pImageWnd IsNot Nothing Then
               ' Redraw the bayer decoded image
               m_pImageWnd.Refresh()
            End If
         End If
      End If
   End Sub

   Private Sub BayerDlg_FormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs) Handles MyBase.FormClosed
      m_pImageWnd.TrackerEnable = False
   End Sub

   Private Sub button_Cancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Cancel.Click
      Me.Close()
   End Sub

End Class

