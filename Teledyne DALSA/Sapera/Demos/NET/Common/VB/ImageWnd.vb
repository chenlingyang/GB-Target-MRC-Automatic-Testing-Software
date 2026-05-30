Namespace DALSA.SaperaLT.SapClassGui
    Public Class ImageWnd
        Public Sub New(ByVal pView As SapView, ByVal pViewWnd As Control, ByVal pHorzScr As HScrollBar, ByVal pVertScr As VScrollBar, ByVal pAppWnd As Form)
            m_pView = pView
            m_pViewWnd = pViewWnd
            m_pHorzScr = pHorzScr
            m_pVertScr = pVertScr
            m_pAppWnd = pAppWnd

            m_Rightoffset = m_pAppWnd.ClientRectangle.Right - (m_pViewWnd.ClientRectangle.Right + m_pViewWnd.Location.X)
            m_Bottomoffset = m_pAppWnd.ClientRectangle.Bottom - (m_pViewWnd.ClientRectangle.Bottom + m_pViewWnd.Location.Y)

            m_roi = New Rectangle(0, 0, 0, 0)
            UpdateRectTracker()
            OnSize()
        End Sub

        Public Sub New(ByVal pView As SapView, ByVal pViewWnd As Control, ByVal pHorzScr As HScrollBar, ByVal pVertScr As VScrollBar, ByVal pAppWnd As Form, ByVal pSlider As TrackBar)
            m_pView = pView
            m_pViewWnd = pViewWnd
            m_pHorzScr = pHorzScr
            m_pVertScr = pVertScr
            m_pAppWnd = pAppWnd
            m_pSlider = pSlider

            m_Rightoffset = m_pAppWnd.ClientRectangle.Right - (m_pViewWnd.ClientRectangle.Right + m_pViewWnd.Location.X)
            m_Bottomoffset = m_pAppWnd.ClientRectangle.Bottom - (m_pViewWnd.ClientRectangle.Bottom + m_pViewWnd.Location.Y)

            m_roi = New Rectangle(0, 0, 0, 0)
            UpdateRectTracker()
            OnSize()
        End Sub


        Public Function GetPixelString(ByVal point As Point) As String
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
                     SapFormat.Int32, SapFormat.Uint32, SapFormat.Int64, SapFormat.Uint64
                        Dim dataMono As New SapDataMono()
                        m_pView.Buffer.ReadElement(pt.X, pt.Y, dataMono)
                        text = [String].Format("[ x= {0} y= {1} Value= {2} ]", pt.X, pt.Y, dataMono.Mono)
                        Exit Select
                    Case SapFormat.LAB, SapFormat.LABP8, SapFormat.LAB101010, SapFormat.LABP16
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
                    Case SapFormat.RGB161616, SapFormat.RGB101010, SapFormat.RGB565, SapFormat.RGB888, SapFormat.RGBR888
                        Dim dataRGB As New SapDataRGB()
                        m_pView.Buffer.ReadElement(pt.X, pt.Y, dataRGB)
                        text = [String].Format("[ x= {0} y= {1} R= {2} G= {3} B= {4} ]", pt.X, pt.Y, dataRGB.Red, dataRGB.Green, dataRGB.Blue)
                        Exit Select
                    Case SapFormat.RGB16161616, SapFormat.RGB8888
                        Dim dataRGBA As New SapDataRGBA()
                        m_pView.Buffer.ReadElement(pt.X, pt.Y, dataRGBA)
                        text = [String].Format("[ x= {0} y= {1} R= {2} G= {3} B= {4} ]", pt.X, pt.Y, dataRGBA.Red, dataRGBA.Green, dataRGBA.Blue)
                        Exit Select
                    Case Else
                        Exit Select
                End Select
                ' Append string to application title
                str = " " + text
            End If
            Return str
        End Function

        Public Sub OnMove()
            ' Call corresponding handler if application window is not being iconified
            If m_pView IsNot Nothing AndAlso m_pAppWnd IsNot Nothing AndAlso m_pAppWnd.WindowState <> FormWindowState.Minimized Then
                m_pView.OnMove()
            End If
        End Sub

        Public Sub OnPaint()
            ' Call corresponding handler if application window is not being iconified
            If m_pView IsNot Nothing AndAlso m_pAppWnd IsNot Nothing AndAlso m_pAppWnd.WindowState <> FormWindowState.Minimized Then
                m_pView.OnPaint()
            End If
        End Sub

        Public Sub OnSize()
            If m_pViewWnd Is Nothing Then
                Return
            End If

            ' If application window is being iconified, hide the current view
            If m_pAppWnd.WindowState = FormWindowState.Minimized Then
                If m_pView IsNot Nothing Then
                    m_pView.Hide()
                End If
                Return
            End If

            ' Get application rectangle
            Dim appRect As Rectangle
            appRect = m_pAppWnd.ClientRectangle

            ' Get view rectangle and offset
            Dim viewRect As Rectangle

            If m_pViewWnd IsNot Nothing Then
                viewRect = New Rectangle(m_pViewWnd.Location, m_pViewWnd.Size)
            Else
                viewRect = appRect
            End If

            If m_pViewWnd IsNot Nothing AndAlso m_pHorzScr IsNot Nothing AndAlso m_pVertScr IsNot Nothing Then

                ''''''''''''''''''''''''''''
                ' Adjust windows' position''
                ''''''''''''''''''''''''''''

                ' Get scroll bars rectangles
                Dim horzRect As Rectangle
                Dim vertRect As Rectangle
                vertRect = New Rectangle(m_pVertScr.Location, m_pVertScr.Size)
                horzRect = New Rectangle(m_pHorzScr.Location, m_pHorzScr.Size)

                ' Adjust view
                viewRect.Width = appRect.Right - (m_Rightoffset + viewRect.Left)
                If viewRect.Width > (m_pView.Buffer.Width * m_pView.ScalingZoomHorz) Then
                    viewRect.Width = CInt((m_pView.Buffer.Width * m_pView.ScalingZoomHorz))
                End If

                viewRect.Height = appRect.Bottom - (m_Bottomoffset + viewRect.Top)
                If viewRect.Height > (m_pView.Buffer.Height * m_pView.ScalingZoomVert) Then
                    viewRect.Height = CInt((m_pView.Buffer.Height * m_pView.ScalingZoomVert))
                End If

                m_pViewWnd.Size = New Size(viewRect.Width, viewRect.Height)

                ' Adjust Horizontal scrollbar
                horzRect.X = viewRect.Left
                horzRect.Y = viewRect.Top - horzRect.Height
                horzRect.Width = viewRect.Width
                m_pHorzScr.Location = New Point(horzRect.X, horzRect.Y)
                m_pHorzScr.Size = New Size(horzRect.Width, horzRect.Height)

                ' Adjust vertical scrollbar
                vertRect.X = viewRect.Left - vertRect.Width
                vertRect.Y = viewRect.Top
                vertRect.Height = viewRect.Height
                m_pVertScr.Location = New Point(vertRect.X, vertRect.Y)
                m_pVertScr.Size = New Size(vertRect.Width, vertRect.Height)

                ' Adjust Slider
                If m_pSlider IsNot Nothing Then
                    Dim SliderRect As New Rectangle(m_pSlider.Location, m_pSlider.Size)
                    SliderRect.X = viewRect.Left
                    SliderRect.Y = viewRect.Bottom
                    SliderRect.Width = viewRect.Width
                    m_pSlider.Location = New Point(SliderRect.X, SliderRect.Y)

                    m_pSlider.Size = New Size(SliderRect.Width, SliderRect.Height)
                End If
            End If

            If m_pView IsNot Nothing Then
                ' Call corresponding handler
                m_pView.OnSize()

                ' Update scroll bars' position and range
                UpdateScrollBars()

                ' Update view rectangle
                'm_ViewRect = viewRect;
                'm_ViewRect.X += m_pView.ScalingDestArea.Left;
                'm_ViewRect.Y += m_pView.ScalingDestArea.Top;
                'm_ViewRect.Width = m_pView.BufferAreaWidth;
                'm_ViewRect.Height = m_pView.BufferAreaHeight;

                ' Update view rectangle
                m_ViewRect.X = viewRect.X
                m_ViewRect.Y = viewRect.Y
                m_ViewRect.Width = appRect.Right - (m_Rightoffset + viewRect.Left)
                m_ViewRect.Height = appRect.Bottom - (m_Bottomoffset + viewRect.Top)

                ' TODO in phase 2 
                ' Update tracker limits
                'Rectangle limitRect = new Rectangle(0,0,m_pView.BufferAreaWidth,m_pView.BufferAreaHeight);
                'm_RectTracker.SetLimitRect(limitRect);

                ' TODO in phase 2 
                'If m_pViewWnd Is Nothing Then
                '    ' Check if unused region of the AppWnd should be repainted
                '    Dim viewWidth As Integer = m_pView.BufferAreaWidth
                '    Dim viewHeight As Integer = m_pView.BufferAreaHeight

                '    If appRect.Width > viewWidth Then
                '        Dim rect As Rectangle = appRect
                '        rect.X = viewWidth
                '        m_pAppWnd.Invalidate(rect, True)
                '    End If
                '    If appRect.Height > viewHeight Then
                '        Dim rect As Rectangle = appRect
                '        rect.Y = viewHeight
                '        m_pAppWnd.Invalidate(rect, True)
                '    End If
                'End If
            End If
        End Sub

        Public Sub Update()
            ' Redraw m_pViewWnd
            m_pViewWnd.Invalidate()
            m_pViewWnd.Update()
            ' Redraw m_pView
            m_pView.OnPaint()

        End Sub

        Public Sub OnHScroll(ByVal e As ScrollEventArgs)
            ' Update view and tracker
            If m_pView IsNot Nothing Then
                m_pView.OnHScroll(m_pHorzScr.Value)
            End If

            OnPaint()
            UpdateRectTracker()
        End Sub

        Public Sub OnVScroll(ByVal e As ScrollEventArgs)
            ' Update view and tracker
            If m_pView IsNot Nothing Then
                m_pView.OnVScroll(m_pVertScr.Value)
            End If

            OnPaint()
            UpdateRectTracker()
        End Sub

        Public Sub OnSliderScroll(ByVal e As EventArgs)
            If m_pView IsNot Nothing Then
                m_pView.Show(m_pSlider.Value)
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
                    If m_pView.Buffer IsNot Nothing Then
                        viewWidth = m_pView.Buffer.Width
                        viewHeight = m_pView.Buffer.Height
                    End If
                    ' pageWidth and pageHeight are already initialized correctly
                    Exit Select
                Case SapView.DisplayScalingMode.FitToWindow
                    ' viewWidth and viewHeight are already initialized correctly
                    ' pageWidth and pageHeight are already initialized correctly
                    Exit Select
                Case SapView.DisplayScalingMode.Zoom, SapView.DisplayScalingMode.UserDefined
                    If m_pView.Buffer IsNot Nothing Then
                        viewWidth = CInt((m_pView.Buffer.Width))
                        viewHeight = CInt((m_pView.Buffer.Height))
                    End If
                    pageWidth = CInt((m_pView.ScalingSrcArea.Width))
                    pageHeight = CInt((m_pView.ScalingSrcArea.Height))
                    Exit Select
            End Select
            ' Update tracker position
            UpdateRectTracker()

            If m_pHorzScr IsNot Nothing AndAlso m_pVertScr IsNot Nothing Then
                ' Size Horitontal scrollbar
                m_pHorzScr.Minimum = 0
                m_pHorzScr.Maximum = CInt((CSng((viewWidth + 0.5)) - 1))
                m_pHorzScr.LargeChange = pageWidth
                m_pHorzScr.Value = m_pView.HorzScrollPosition


                ' Size Vertical scrollbar
                m_pVertScr.Minimum = 0
                m_pVertScr.Maximum = CInt(((CSng(viewHeight) + 0.5) - 1))
                m_pVertScr.LargeChange = pageHeight
                m_pVertScr.Value = m_pView.VertScrollPosition

                ' Show/hide scroll bars

                If m_pView.HorzScrollRange > 0 Then
                    m_pHorzScr.Show()
                Else
                    m_pHorzScr.Hide()
                End If
                If m_pView.VertScrollRange > 0 Then
                    m_pVertScr.Show()
                Else
                    m_pVertScr.Hide()
                End If
            End If
        End Sub

        Private Function TranslatePos(ByVal point As Point) As Point
            Dim translatedPoint As Point = point

            If translatedPoint.X < 0 Then
                translatedPoint.X = 0
            End If

            If translatedPoint.Y < 0 Then
                translatedPoint.Y = 0
            End If

            translatedPoint.X += CInt((m_pHorzScr.Value * m_pView.ScalingZoomHorz))
            translatedPoint.Y += CInt((m_pVertScr.Value * m_pView.ScalingZoomVert))

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

            translatedPoint.X -= CInt((m_pHorzScr.Value * m_pView.ScalingZoomHorz))
            translatedPoint.Y -= CInt((m_pVertScr.Value * m_pView.ScalingZoomVert))

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

        Private Sub UpdateRectTracker()
            'TODO in phase 2 
            'm_RectTracker.m_rect.TopLeft() = UntranslatePos( new Point(m_roi.Left,m_roi.Top));
            'm_RectTracker.m_rect.BottomRight()= UntranslatePos( new Point(m_roi.Right,m_roi.Bottom));
        End Sub

        Public ReadOnly Property ViewRectangle() As Rectangle
            Get
                Return m_ViewRect
            End Get
        End Property

        Public Property View() As SapView
            Get
                Return m_pView
            End Get
            Set(ByVal value As SapView)
                m_pView = value
            End Set
        End Property

        Private m_pView As SapView
        Private m_pViewWnd As Control
        Private m_pHorzScr As HScrollBar
        Private m_pVertScr As VScrollBar
        Private m_pAppWnd As Form
        Private m_pSlider As TrackBar

        ' Other variables
        Private m_ViewRect As Rectangle
        Private m_roi As Rectangle
        Private m_Rightoffset As Integer
        Private m_Bottomoffset As Integer

        ' Variable for Phase 2
        'private RectTracker m_RectTracker;

    End Class
End Namespace