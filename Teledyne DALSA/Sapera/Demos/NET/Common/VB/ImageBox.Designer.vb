Partial Class ImageBox
   ''' <summary> 
   ''' Required designer variable.
   ''' </summary>
   Private components As System.ComponentModel.IContainer = Nothing

   ''' <summary> 
   ''' Clean up any resources being used.
   ''' </summary>
   ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
   Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
      If disposing AndAlso (components IsNot Nothing) Then
         components.Dispose()
      End If
      MyBase.Dispose(disposing)
   End Sub

   ''' <summary> 
   ''' Required method for Designer support - do not modify 
   ''' the contents of this method with the code editor.
   ''' </summary>
   Private Sub InitializeComponent()
      Me.picBox = New System.Windows.Forms.PictureBox()
      Me.v_Scroll = New System.Windows.Forms.VScrollBar()
      Me.h_Scroll = New System.Windows.Forms.HScrollBar()
      Me.Slider = New System.Windows.Forms.TrackBar()
      DirectCast((Me.picBox), System.ComponentModel.ISupportInitialize).BeginInit()
      DirectCast((Me.Slider), System.ComponentModel.ISupportInitialize).BeginInit()
      Me.SuspendLayout()
      ' 
      ' picBox
      ' 
      Me.picBox.Location = New System.Drawing.Point(0, 0)
      Me.picBox.Name = "picBox"
      Me.picBox.Size = New System.Drawing.Size(366, 340)
      Me.picBox.TabIndex = 0
      Me.picBox.TabStop = False
      ' 
      ' vScroll
      ' 
      Me.v_Scroll.Location = New System.Drawing.Point(368, 0)
      Me.v_Scroll.Name = "vScroll"
      Me.v_Scroll.Size = New System.Drawing.Size(17, 340)
      Me.v_Scroll.TabIndex = 1
      ' 
      ' hScroll
      ' 
      Me.h_Scroll.Location = New System.Drawing.Point(0, 340)
      Me.h_Scroll.Name = "hScroll"
      Me.h_Scroll.Size = New System.Drawing.Size(362, 17)
      Me.h_Scroll.TabIndex = 2
      ' 
      ' Slider
      ' 
      Me.Slider.Location = New System.Drawing.Point(3, 360)
      Me.Slider.Name = "Slider"
      Me.Slider.Size = New System.Drawing.Size(382, 42)
      Me.Slider.TabIndex = 7
      ' 
      ' ImageBox
      ' 
      Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0F, 13.0F)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.Controls.Add(Me.Slider)
      Me.Controls.Add(Me.h_Scroll)
      Me.Controls.Add(Me.v_Scroll)
      Me.Controls.Add(Me.picBox)
      Me.Name = "ImageBox"
      Me.Size = New System.Drawing.Size(386, 406)
      DirectCast((Me.picBox), System.ComponentModel.ISupportInitialize).EndInit()
      DirectCast((Me.Slider), System.ComponentModel.ISupportInitialize).EndInit()
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub
End Class



