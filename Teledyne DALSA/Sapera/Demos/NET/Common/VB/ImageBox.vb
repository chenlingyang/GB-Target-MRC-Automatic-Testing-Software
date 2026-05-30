Partial Public Class ImageBox
   Inherits UserControl

   Private picBox As PictureBox
   Private v_Scroll As VScrollBar
   Private h_Scroll As HScrollBar
   Private Slider As TrackBar
   Private m_pView As SapView
   Private m_tracker As Rectangle
   Private recTracker As Rectangle
   Private mouse_Down As Boolean = False
   Private useRoi As Boolean = False
   Private useSlider As Boolean = False
   Private StartPoint As Point
   Private EndPoint As Point
   Private pixelValueDispaly As ToolStripLabel
   Private Lastv_ScrollValue As Integer, Lasth_ScrollValue As Integer
   Private SliderOffset As Integer, RightOffset As Integer, BottomOffset As Integer

   Public Sub New()
      InitializeComponent()
      AddHandler picBox.MouseDown, AddressOf Me.picBox_mouse_Down
      AddHandler picBox.MouseUp, AddressOf Me.picBox_MouseUp
      AddHandler picBox.MouseMove, AddressOf Me.picBox_MouseMove
      AddHandler v_Scroll.Scroll, AddressOf Me.v_Scroll_Scroll
      AddHandler h_Scroll.Scroll, AddressOf Me.h_Scroll_Scroll
      AddHandler Slider.Scroll, AddressOf Me.Slider_Scroll

      mouse_Down = False
      useRoi = False
      useSlider = False
      ' Set offset Value 
      RightOffset = 5
      BottomOffset = 5
      SliderOffset = 0
      ' Default : Slider is hide
      Slider.Hide()
      ' Constant size's part of scrollbar 
      h_Scroll.Height = 15
      v_Scroll.Width = 15
   End Sub

   Protected Overloads Overrides Sub OnPaint(ByVal e As PaintEventArgs)
      If m_pView IsNot Nothing AndAlso m_pView.Initialized Then
         FitImageBoxToBottomRight()
         UpdateScrollBars()
         m_pView.OnPaint()
         DisplayTracker()
      End If
      MyBase.OnPaint(e)
   End Sub

   Private Sub picBox_mouse_Down(ByVal sender As Object, ByVal e As MouseEventArgs)
      If useRoi Then
         mouse_Down = True
         StartPoint = New Point(e.X, e.Y)
      End If
   End Sub

   Private Sub picBox_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
      If useRoi Then
         If mouse_Down Then
            EndPoint = New Point(e.X, e.Y)
            If StartPoint.X > EndPoint.X Then
               If StartPoint.Y > EndPoint.Y Then
                  m_tracker = New Rectangle(EndPoint.X, EndPoint.Y, StartPoint.X - EndPoint.X, StartPoint.Y - EndPoint.Y)
               Else
                  m_tracker = New Rectangle(EndPoint.X, StartPoint.Y, StartPoint.X - EndPoint.X, EndPoint.Y - StartPoint.Y)
               End If
            Else
               If StartPoint.Y > EndPoint.Y Then
                  m_tracker = New Rectangle(StartPoint.X, EndPoint.Y, EndPoint.X - StartPoint.X, StartPoint.Y - EndPoint.Y)
               Else
                  m_tracker = New Rectangle(StartPoint.X, StartPoint.Y, EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y)
               End If
            End If
            Me.Refresh()
         End If
      End If
      ' Pixel value over the cursor
      If pixelValueDispaly IsNot Nothing Then
         pixelValueDispaly.Text = GetPixelString(New Point(e.X, e.Y))
      End If

   End Sub

   Private Sub picBox_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
      If useRoi Then
         EndPoint = New Point(e.X, e.Y)
         If StartPoint.X > EndPoint.X Then
            If StartPoint.Y > EndPoint.Y Then
               m_tracker = New Rectangle(EndPoint.X, EndPoint.Y, StartPoint.X - EndPoint.X, StartPoint.Y - EndPoint.Y)
            Else
               m_tracker = New Rectangle(EndPoint.X, StartPoint.Y, StartPoint.X - EndPoint.X, EndPoint.Y - StartPoint.Y)
            End If
         Else
            If StartPoint.Y > EndPoint.Y Then
               m_tracker = New Rectangle(StartPoint.X, EndPoint.Y, EndPoint.X - StartPoint.X, StartPoint.Y - EndPoint.Y)
            Else
               m_tracker = New Rectangle(StartPoint.X, StartPoint.Y, EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y)
            End If
         End If

         ' If drawn tracker is not empty copy to recTracker 
         If m_tracker.Height <> 0 AndAlso m_tracker.Width <> 0 Then
            recTracker = New Rectangle(m_tracker.X + h_Scroll.Value, m_tracker.Y + v_Scroll.Value, m_tracker.Width, m_tracker.Height)
         Else
            recTracker = New Rectangle(0, 0, 0, 0)
         End If

         mouse_Down = False

         ' Save current position of scroll bar
         Lasth_ScrollValue = h_Scroll.Value
         Lastv_ScrollValue = v_Scroll.Value
         Me.Refresh()
      End If
   End Sub

   Private Sub v_Scroll_Scroll(ByVal sender As Object, ByVal e As ScrollEventArgs)
      ' Update view and tracker
      If m_pView IsNot Nothing AndAlso m_pView.Initialized Then
         m_pView.OnVScroll(v_Scroll.Value)
      End If
      If useRoi Then
         UpdateTracker()
      End If
      Me.Refresh()
   End Sub

   Private Sub h_Scroll_Scroll(ByVal sender As Object, ByVal e As ScrollEventArgs)
      ' Update view and tracker
      If m_pView IsNot Nothing AndAlso m_pView.Initialized Then
         m_pView.OnHScroll(h_Scroll.Value)
      End If
      If useRoi Then
         UpdateTracker()
      End If
      Me.Refresh()
   End Sub

   Private Sub Slider_Scroll(ByVal sender As Object, ByVal e As EventArgs)
      If m_pView IsNot Nothing AndAlso m_pView.Initialized Then
         m_pView.Buffer.Index = Slider.Value
         m_pView.Show()
      End If
   End Sub

   Private Function GetPixelString(ByVal point As Point) As String
      Dim str As String = "[ Pixel data not available ]"

      ' if there is no buffer to display, return right away
      If m_pView Is Nothing OrElse m_pView.Buffer Is Nothing OrElse Not m_pView.Buffer.Mapped Then
         Return str
      End If
      If m_pView.Buffer IsNot Nothing Then
         Dim pt As Point = TranslatePos(point)
         ' Get pixel value at cursor's position and create string according to pixel format
         Dim text As String = ""
         Dim format As SapFormat = m_pView.Buffer.Format

         Select Case format
                Case SapFormat.Uint8, SapFormat.Int8, SapFormat.Int16, SapFormat.Uint16, SapFormat.Int24, SapFormat.Uint24, _
                 SapFormat.Int32, SapFormat.Uint32, SapFormat.Int64, SapFormat.Uint64, SapFormat.Mono8P2, SapFormat.Mono8P3, _
                 SapFormat.Mono8P4, SapFormat.Mono8P5, SapFormat.Mono8P6, SapFormat.Mono8P7, SapFormat.Mono8P8, _
                 SapFormat.Mono8P9, SapFormat.Mono8P10, SapFormat.Mono16P2, SapFormat.Mono16P3, SapFormat.Mono16P4, _
                 SapFormat.Mono16P5, SapFormat.Mono16P6, SapFormat.Mono16P7, SapFormat.Mono16P8, SapFormat.Mono16P9, _
                 SapFormat.Mono16P10
                    Dim dataMono As New SapDataMono()
                    m_pView.Buffer.ReadElement(pt.X, pt.Y, dataMono)
                    text = [String].Format("[ x= {0} y= {1} Value= {2} ]", pt.X, pt.Y, dataMono.Mono)
                    Exit Select
                Case SapFormat.LAB, SapFormat.LAB101010
                    Dim dataLAB As New SapDataLAB()
                    m_pView.Buffer.ReadElement(pt.X, pt.Y, dataLAB)
                    text = [String].Format("[ x= {0} y= {1} L= (2) A= (3) B= (4) ]", pt.X, pt.Y, dataLAB.L, dataLAB.A, dataLAB.B)
                    Exit Select
            Case SapFormat.LAB16161616
               Dim dataLABA As New SapDataLABA()
               m_pView.Buffer.ReadElement(pt.X, pt.Y, dataLABA)
               text = [String].Format("[ x= {0} y= {1} L= (2) A= (3) B= (4) ]", pt.X, pt.Y, dataLABA.L, dataLABA.A, dataLABA.B)
               Exit Select
            Case SapFormat.HSI, SapFormat.HSIP8
               Dim dataHSI As New SapDataHSI()
               m_pView.Buffer.ReadElement(pt.X, pt.Y, dataHSI)
               text = [String].Format("[ x= {0} y= {1} H= {2} S= {3} I= {4} ]", pt.X, pt.Y, dataHSI.H, dataHSI.S, dataHSI.I)
               Exit Select
            Case SapFormat.HSV
               Dim dataHSV As New SapDataHSV()
               m_pView.Buffer.ReadElement(pt.X, pt.Y, dataHSV)
               text = [String].Format("[x= {0} y= {1} H= {2} S= {3} V= {4}]", pt.X, pt.Y, dataHSV.H, dataHSV.S, dataHSV.V)
               Exit Select
            Case SapFormat.YUV
               Dim dataYUV As New SapDataYUV()
               m_pView.Buffer.ReadElement(pt.X, pt.Y, dataYUV)
               text = [String].Format("[ x= {0} y= {1} Y= {2} U= {3} V= {4} ]", pt.X, pt.Y, dataYUV.Y, dataYUV.U, dataYUV.V)
                    Exit Select
                Case SapFormat.RGB161616, SapFormat.RGB101010, SapFormat.RGB565, SapFormat.RGB888, SapFormat.RGBR888, SapFormat.RGB8888, SapFormat.BICOLOR88, SapFormat.BICOLOR1616, SapFormat.BICOLOR1212
                    Dim dataRGB As New SapDataRGB()
                    m_pView.Buffer.ReadElement(pt.X, pt.Y, dataRGB)
                    text = [String].Format("[ x= {0} y= {1} R= {2} G= {3} B= {4} ]", pt.X, pt.Y, dataRGB.Red, dataRGB.Green, dataRGB.Blue)
                    Exit Select
            Case SapFormat.RGB16161616
               Dim dataRGBA As New SapDataRGBA()
               m_pView.Buffer.ReadElement(pt.X, pt.Y, dataRGBA)
               text = [String].Format("[ x= {0} y= {1} R= {2} G= {3} B= {4} ]", pt.X, pt.Y, dataRGBA.Red, dataRGBA.Green, dataRGBA.Blue)
               Exit Select
                Case SapFormat.RGB888_MONO8
                Case SapFormat.RGB161616_MONO16
                Case SapFormat.RGBAP8
                Case SapFormat.RGBAP16
                    Dim dataMulti As New SapDataRGBA()
                    Dim currentPage As Integer = m_pView.Buffer.Page
                    m_pView.Buffer.ReadElement(pt.X, pt.Y, dataMulti)
                    If currentPage = 0 Then
                        text = [String].Format("[ x= {0} y= {1} R= {2} G= {3} B= {4} ]", pt.X, pt.Y, dataMulti.Red, dataMulti.Green, dataMulti.Blue)
                    Else
                        text = [String].Format("[ x= {0} y= {1} Mono= {2} ]", pt.X, pt.Y, dataMulti.Alpha)
                    End If
                    Exit Select
                Case SapFormat.RGBP8
                Case SapFormat.RGBP16
                Case SapFormat.LABP8
                Case SapFormat.LABP16
                    Dim dataRGB As New SapDataRGB()
                    m_pView.Buffer.ReadElement(pt.X, pt.Y, dataRGB)

                    Dim formatStr As String
                    If format = SapFormat.RGBP8 Or format = SapFormat.LABP8 Then
                        formatStr = "[ x= %03ld y= %03ld Value= %03d ]"
                    Else
                        formatStr = "[ x= %03ld y= %03ld Value= %04X ]"
                    End If

                    Dim page As Integer = m_pView.Buffer.Page
                    If page = 0 Then
                        text = String.Format(formatStr, pt.X, pt.Y, dataRGB.Red)
                    ElseIf page = 1 Then
                        text = String.Format(formatStr, pt.X, pt.Y, dataRGB.Green)
                    ElseIf page = 2 Then
                        text = String.Format(formatStr, pt.X, pt.Y, dataRGB.Blue)
                    End If
                    Exit Select

                Case Else
                    Exit Select
            End Select
         ' Append string to application title
         str = "  " & text
      End If
      Return str
   End Function

   Private Function TranslatePos(ByVal point As Point) As Point
      Dim translatedPoint As Point = point

      If translatedPoint.X < 0 Then
         translatedPoint.X = 0
      End If

      If translatedPoint.Y < 0 Then
         translatedPoint.Y = 0
      End If

      translatedPoint.X += CInt((h_Scroll.Value * m_pView.ScalingZoomHorz))
      translatedPoint.Y += CInt((v_Scroll.Value * m_pView.ScalingZoomVert))

      translatedPoint.X = CInt((translatedPoint.X / m_pView.ScalingZoomHorz))
      translatedPoint.Y = CInt((translatedPoint.Y / m_pView.ScalingZoomVert))

      If m_pView IsNot Nothing AndAlso m_pView.Buffer IsNot Nothing Then
         If translatedPoint.X >= m_pView.Buffer.Width Then
            translatedPoint.X = m_pView.Buffer.Width - 1
         End If

         If translatedPoint.Y >= m_pView.Buffer.Height Then
            translatedPoint.Y = m_pView.Buffer.Height - 1
         End If
      End If
      Return translatedPoint
   End Function

   Private Function UntranslatePos(ByVal point As Point) As Point
      Dim translatedPoint As Point = point

      If translatedPoint.X < 0 Then
         translatedPoint.X = 0
      End If

      If translatedPoint.Y < 0 Then
         translatedPoint.Y = 0
      End If

      translatedPoint.X = CInt((translatedPoint.X * m_pView.ScalingZoomHorz))
      translatedPoint.Y = CInt((translatedPoint.Y * m_pView.ScalingZoomVert))

      translatedPoint.X -= CInt((h_Scroll.Value * m_pView.ScalingZoomHorz))
      translatedPoint.Y -= CInt((v_Scroll.Value * m_pView.ScalingZoomVert))

      If m_pView IsNot Nothing Then
         If translatedPoint.X >= m_pView.BufferAreaWidth Then
            translatedPoint.X = m_pView.BufferAreaWidth - 1
         End If

         If translatedPoint.Y >= m_pView.BufferAreaHeight Then
            translatedPoint.Y = m_pView.BufferAreaHeight - 1
         End If
      End If

      Return translatedPoint
   End Function

   Private Sub UpdateTracker()
      m_tracker.X += (Lasth_ScrollValue - h_Scroll.Value)
      m_tracker.Y += (Lastv_ScrollValue - v_Scroll.Value)
      Lasth_ScrollValue = h_Scroll.Value
      Lastv_ScrollValue = v_Scroll.Value
   End Sub


   ' Fit picture box and scroll bars to the bottom right corner
   ' of the application's form
   Private Sub FitImageBoxToBottomRight()
      Dim frm As Form = Me.ParentForm
      If frm IsNot Nothing Then
         If frm.WindowState = FormWindowState.Minimized Then
            Exit Sub
         End If

         ' Set ImageBox size
         Me.Width = frm.ClientRectangle.Width - (Me.Left + RightOffset)
         Me.Height = frm.ClientRectangle.Height - (Me.Top + BottomOffset)

         Dim width As Integer = Me.ClientRectangle.Width - (picBox.Left + v_Scroll.Width)
         If width < (m_pView.Buffer.Width * m_pView.ScalingZoomHorz) Then
            picBox.Width = width
         Else
            picBox.Width = CInt((m_pView.Buffer.Width * m_pView.ScalingZoomHorz))
         End If

         Dim height As Integer = Me.ClientRectangle.Height - (picBox.Top + h_Scroll.Height + SliderOffset)
         If height < (m_pView.Buffer.Height * m_pView.ScalingZoomVert) Then
            picBox.Height = height
         Else
            picBox.Height = CInt((m_pView.Buffer.Height * m_pView.ScalingZoomVert))
         End If

         ' Set Scroll bar size and position
         h_Scroll.Top = picBox.Top + picBox.Height
         h_Scroll.Width = picBox.Left + picBox.Width - h_Scroll.Left
         v_Scroll.Left = picBox.Left + picBox.Width
         v_Scroll.Height = picBox.Top + picBox.Height - v_Scroll.Top

         'Set Slider width and position
         If useSlider Then
            Slider.Show()
            Slider.Top = picBox.Top + picBox.Height + h_Scroll.Height
            Slider.Width = picBox.Left + picBox.Width - h_Scroll.Left
         Else
            Slider.Hide()
         End If
      End If
   End Sub

   Public Sub DisplayTracker()
      If useRoi AndAlso Not m_tracker.IsEmpty Then
         Dim g As Graphics = m_pView.GetGraphics()
         g.DrawRectangle(New Pen(Color.Gainsboro, 2), m_tracker)
      End If
   End Sub

   Public Sub OnSize()
      If m_pView IsNot Nothing AndAlso m_pView.Initialized Then
         FitImageBoxToBottomRight()
         m_pView.OnSize()
         UpdateScrollBars()
      End If
   End Sub

   Private Sub UpdateScrollBars()

      ' Note: the view pointer has already been validated by the caller (OnSize method)
      Dim viewWidth As Integer = m_pView.BufferAreaWidth
      Dim viewHeight As Integer = m_pView.BufferAreaHeight
      Dim pageWidth As Integer = m_pView.BufferAreaWidth
      Dim pageHeight As Integer = m_pView.BufferAreaHeight

      Select Case m_pView.ScalingMode
         Case SapView.DisplayScalingMode.None
            If True Then
               If m_pView.Buffer IsNot Nothing Then
                  viewWidth = m_pView.Buffer.Width
                  viewHeight = m_pView.Buffer.Height
               End If
               Exit Select
            End If
            ' pageWidth and pageHeight are already initialized correctly
         Case SapView.DisplayScalingMode.FitToWindow
            If True Then
               ' viewWidth and viewHeight are already initialized correctly
               ' pageWidth and pageHeight are already initialized correctly
               Exit Select
            End If
         Case SapView.DisplayScalingMode.Zoom, SapView.DisplayScalingMode.UserDefined
            If True Then
               If m_pView.Buffer IsNot Nothing Then
                  viewWidth = CInt((m_pView.Buffer.Width))
                  viewHeight = CInt((m_pView.Buffer.Height))
               End If
               pageWidth = CInt((m_pView.ScalingSrcArea.Width))
               pageHeight = CInt((m_pView.ScalingSrcArea.Height))
               Exit Select
            End If
      End Select


      If h_Scroll IsNot Nothing AndAlso v_Scroll IsNot Nothing Then
         ' Size Horitontal scrollbar
         h_Scroll.Minimum = 0
         h_Scroll.Maximum = CInt((CSng((viewWidth + 0.5)) - 1))
         h_Scroll.LargeChange = pageWidth
         h_Scroll.Value = m_pView.HorzScrollPosition


         ' Size Vertical scrollbar
         v_Scroll.Minimum = 0
         v_Scroll.Maximum = CInt(((CSng(viewHeight) + 0.5) - 1))
         v_Scroll.LargeChange = pageHeight
         v_Scroll.Value = m_pView.VertScrollPosition

         ' Show/hide scroll bars

         If m_pView.HorzScrollRange > 0 Then
            h_Scroll.Show()
         Else
            h_Scroll.Hide()
         End If
         If m_pView.VertScrollRange > 0 Then
            v_Scroll.Show()
         Else
            v_Scroll.Hide()
         End If
      End If
   End Sub

   ' Properties
   Public Property View() As SapView
      Get
         Return m_pView
      End Get
      Set(ByVal value As SapView)
         m_pView = value
         If m_pView IsNot Nothing Then
            m_pView.Window = picBox
         End If
      End Set
   End Property

   Public Property PixelValueDisplay() As ToolStripLabel
      Get
         Return pixelValueDispaly
      End Get
      Set(ByVal value As ToolStripLabel)
         pixelValueDispaly = value
         If pixelValueDispaly IsNot Nothing Then
            BottomOffset = pixelValueDispaly.Height + 5
         Else
            BottomOffset = 5
         End If
      End Set
   End Property

   Public ReadOnly Property ViewRectangle() As Rectangle
      Get
         Dim ViewSize As New Size(Me.Width - v_Scroll.Width, Me.Height - h_Scroll.Height)
         Return New Rectangle(Me.Location, ViewSize)
      End Get
   End Property

   Public ReadOnly Property Tracker() As Rectangle
      Get
         If useRoi Then
            Return recTracker
         Else
            Return New Rectangle(0, 0, 0, 0)
         End If
      End Get
   End Property

   Public ReadOnly Property IsTrackerEmpty() As Boolean
      Get
         If recTracker.Height = 0 OrElse recTracker.Width = 0 Then
            Return True
         Else
            Return False
         End If
      End Get
   End Property

   Public Property TrackerEnable() As Boolean
      Get
         Return useRoi
      End Get
      Set(ByVal value As Boolean)
         useRoi = value
         recTracker = New Rectangle(0, 0, 0, 0)
         m_tracker = New Rectangle(0, 0, 0, 0)
         Me.Refresh()
      End Set
   End Property

   Public Property SliderVisible() As Boolean
      Get
         Return useSlider
      End Get
      Set(ByVal value As Boolean)
         useSlider = value
         If useSlider Then
            SliderOffset = 42
         Else
            SliderOffset = 0
         End If
         Me.OnSize()
      End Set
   End Property

   Public Property SliderEnable() As Boolean
      Get
         Return Slider.Enabled
      End Get
      Set(ByVal value As Boolean)
         Slider.Enabled = value
      End Set
   End Property

   Public Property SliderMinimum() As Integer
      Get
         Return Slider.Minimum
      End Get
      Set(ByVal value As Integer)
         Slider.Minimum = value
      End Set
   End Property

   Public Property SliderMaximum() As Integer
      Get
         Return Slider.Maximum
      End Get
      Set(ByVal value As Integer)
         Slider.Maximum = value
      End Set
   End Property

   Public Property SliderValue() As Integer
      Get
         Return Slider.Value
      End Get
      Set(ByVal value As Integer)
         Slider.Value = value
      End Set
   End Property
End Class






