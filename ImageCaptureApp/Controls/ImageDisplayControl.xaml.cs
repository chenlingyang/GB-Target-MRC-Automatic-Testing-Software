using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Emgu.CV;
using Emgu.CV.CvEnum;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageCaptureApp.Controls
{
    /// <summary>
    /// 图像显示控件 - 支持缩放和平移
    /// </summary>
    public partial class ImageDisplayControl : UserControl
    {
        private double _zoomScale = 1.0;
        private System.Windows.Point _panStartPoint;
        private bool _isPanning = false;
        private bool _isFittedToView = false;
        private Mat? _currentImage;
        private ScaleTransform? _scaleTransform;
        private TranslateTransform? _translateTransform;

        public ImageDisplayControl()
        {
            InitializeComponent();
            _scaleTransform = DisplayImage.RenderTransform as ScaleTransform;
            if (_scaleTransform == null)
            {
                _scaleTransform = new ScaleTransform(1.0, 1.0);
            }

            _translateTransform = new TranslateTransform(0.0, 0.0);
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(_scaleTransform);
            transformGroup.Children.Add(_translateTransform);
            DisplayImage.RenderTransform = transformGroup;

            if (_translateTransform == null)
            {
                _translateTransform = new TranslateTransform(0.0, 0.0);
            }

            // 关键：PreviewMouseWheel 优先于 ScrollViewer 处理，确保 Ctrl+滚轮缩放可用
            this.PreviewMouseWheel += UserControl_PreviewMouseWheel;
            // handledEventsToo=true，防止子控件先处理后本控件收不到事件
            AddHandler(MouseWheelEvent, new MouseWheelEventHandler(UserControl_MouseWheel), true);
            // 拖拽平移使用预览事件，确保左键按住拖动在子控件（Image/ScrollViewer）上也稳定触发
            AddHandler(PreviewMouseDownEvent, new MouseButtonEventHandler(UserControl_MouseDown), true);
            AddHandler(PreviewMouseMoveEvent, new MouseEventHandler(UserControl_MouseMove), true);
            AddHandler(PreviewMouseUpEvent, new MouseButtonEventHandler(UserControl_MouseUp), true);
            Loaded += ImageDisplayControl_Loaded;
            SizeChanged += ImageDisplayControl_SizeChanged;
        }

        /// <summary>
        /// 当前缩放比例
        /// </summary>
        public double ZoomScale
        {
            get => _zoomScale;
            set
            {
                _zoomScale = Math.Max(0.1, Math.Min(10.0, value));
                UpdateDisplay();
            }
        }

        /// <summary>
        /// 设置显示的图像
        /// </summary>
        public void SetImage(Mat? image)
        {
            if (image == null || image.IsEmpty)
            {
                DisplayImage.Source = null;
                _currentImage?.Dispose();
                _currentImage = null;
                return;
            }

            _currentImage?.Dispose();
            _currentImage = image.Clone();
            UpdateDisplay();

            // 首次显示时自动适配到可视区域，尽量铺满
            if (!_isFittedToView)
            {
                _isFittedToView = FitToViewport();
            }
        }

        /// <summary>
        /// 更新显示
        /// </summary>
        private void UpdateDisplay()
        {
            if (_currentImage == null || _currentImage.IsEmpty)
            {
                DisplayImage.Source = null;
                return;
            }

            try
            {
                // 转换为BitmapSource（不在这里缩放，使用RenderTransform）
                BitmapSource? bitmapSource = ConvertMatToBitmapSource(_currentImage);
                if (bitmapSource != null)
                {
                    DisplayImage.Source = bitmapSource;
                    DisplayImage.Width = bitmapSource.PixelWidth;
                    DisplayImage.Height = bitmapSource.PixelHeight;

                    // 应用缩放变换
                    if (_scaleTransform != null)
                    {
                        _scaleTransform.ScaleX = _zoomScale;
                        _scaleTransform.ScaleY = _zoomScale;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"更新显示失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 将Mat转换为BitmapSource
        /// </summary>
        private BitmapSource? ConvertMatToBitmapSource(Mat mat)
        {
            if (mat == null || mat.IsEmpty)
            {
                return null;
            }

            try
            {
                // 直接从 Mat 数据创建 BitmapSource
                System.Windows.Media.PixelFormat pixelFormat;
                int bytesPerPixel;

                // 根据通道数确定像素格式
                if (mat.NumberOfChannels == 1)
                {
                    pixelFormat = PixelFormats.Gray8;
                    bytesPerPixel = 1;
                }
                else if (mat.NumberOfChannels == 3)
                {
                    pixelFormat = PixelFormats.Bgr24;
                    bytesPerPixel = 3;
                }
                else if (mat.NumberOfChannels == 4)
                {
                    pixelFormat = PixelFormats.Bgra32;
                    bytesPerPixel = 4;
                }
                else
                {
                    // 默认转换为 BGR24
                    Mat converted = new Mat();
                    CvInvoke.CvtColor(mat, converted, ColorConversion.Bgra2Bgr);
                    pixelFormat = PixelFormats.Bgr24;
                    bytesPerPixel = 3;

                    BitmapSource? result = CreateBitmapSourceFromMat(converted, pixelFormat, bytesPerPixel);
                    converted.Dispose();
                    return result;
                }

                return CreateBitmapSourceFromMat(mat, pixelFormat, bytesPerPixel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"转换Mat到BitmapSource失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 从 Mat 创建 BitmapSource
        /// </summary>
        private BitmapSource? CreateBitmapSourceFromMat(Mat mat, System.Windows.Media.PixelFormat pixelFormat, int bytesPerPixel)
        {
            if (mat == null || mat.IsEmpty)
            {
                return null;
            }

            try
            {
                int stride = mat.Width * bytesPerPixel;
                int size = stride * mat.Height;

                // 复制数据到字节数组
                byte[] buffer = new byte[size];
                System.Runtime.InteropServices.Marshal.Copy(mat.DataPointer, buffer, 0, size);

                var bitmapSource = BitmapSource.Create(
                    mat.Width,
                    mat.Height,
                    96, // DPI
                    96, // DPI
                    pixelFormat,
                    null,
                    buffer,
                    stride);

                bitmapSource.Freeze();
                return bitmapSource;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"创建BitmapSource失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 将System.Drawing.Bitmap转换为BitmapSource
        /// </summary>
        private BitmapSource ConvertBitmapToBitmapSource(System.Drawing.Bitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bitmap.PixelFormat);

            try
            {
                // 根据像素格式选择合适的 WPF 像素格式
                System.Windows.Media.PixelFormat pixelFormat = PixelFormats.Bgr24;

                if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                {
                    pixelFormat = PixelFormats.Bgr24;
                }
                else if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppRgb ||
                         bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                {
                    pixelFormat = PixelFormats.Bgra32;
                }
                else if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    pixelFormat = PixelFormats.Gray8;
                }

                var bitmapSource = BitmapSource.Create(
                    bitmapData.Width,
                    bitmapData.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    pixelFormat,
                    null,
                    bitmapData.Scan0,
                    bitmapData.Stride * bitmapData.Height,
                    bitmapData.Stride);

                // 冻结以提升性能并允许跨线程访问
                bitmapSource.Freeze();
                return bitmapSource;
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        /// <summary>
        /// 重置视图
        /// </summary>
        public void ResetView()
        {
            _isFittedToView = FitToViewport();
            UpdateDisplay();
        }

        /// <summary>
        /// 放大
        /// </summary>
        public void ZoomIn()
        {
            ZoomScale *= 1.2;
            if (_scaleTransform != null)
            {
                _scaleTransform.ScaleX = _zoomScale;
                _scaleTransform.ScaleY = _zoomScale;
            }
        }

        /// <summary>
        /// 缩小
        /// </summary>
        public void ZoomOut()
        {
            ZoomScale /= 1.2;
            if (_scaleTransform != null)
            {
                _scaleTransform.ScaleX = _zoomScale;
                _scaleTransform.ScaleY = _zoomScale;
            }
        }

        /// <summary>
        /// 鼠标滚轮事件
        /// </summary>
        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
            var mousePoint = e.GetPosition(ScrollViewer);
            ZoomAt(mousePoint, zoomFactor);
            e.Handled = true;
        }

        // 按你的要求：使用 PreviewMouseWheel 抢先处理，避免 ScrollViewer 吞掉 Ctrl+滚轮
        private void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
            var mousePoint = e.GetPosition(ScrollViewer);
            ZoomAt(mousePoint, zoomFactor);
            e.Handled = true; // 阻止 ScrollViewer 再滚动
        }

        private void ZoomAt(System.Windows.Point pointInScrollViewer, double zoomFactor)
        {
            if (_currentImage == null || _currentImage.IsEmpty)
            {
                return;
            }

            double oldScale = _zoomScale;
            double newScale = Math.Max(0.1, Math.Min(10.0, oldScale * zoomFactor));
            if (Math.Abs(newScale - oldScale) < 0.0001)
            {
                return;
            }

            // 缩放前该点对应的内容坐标
            double oldTx = _translateTransform?.X ?? 0.0;
            double oldTy = _translateTransform?.Y ?? 0.0;

            _zoomScale = newScale;
            if (_scaleTransform != null)
            {
                _scaleTransform.ScaleX = _zoomScale;
                _scaleTransform.ScaleY = _zoomScale;
            }
            double ratio = newScale / oldScale;
            if (_translateTransform != null)
            {
                _translateTransform.X = pointInScrollViewer.X - (pointInScrollViewer.X - oldTx) * ratio;
                _translateTransform.Y = pointInScrollViewer.Y - (pointInScrollViewer.Y - oldTy) * ratio;
            }

            UpdateDisplay();
        }

        private bool FitToViewport()
        {
            if (_currentImage == null || _currentImage.IsEmpty)
            {
                _zoomScale = 1.0;
                return false;
            }

            double viewportW = ScrollViewer.ViewportWidth;
            double viewportH = ScrollViewer.ViewportHeight;

            if (viewportW <= 1 || viewportH <= 1)
            {
                viewportW = Math.Max(1, ActualWidth - 20);
                viewportH = Math.Max(1, ActualHeight - 20);
            }

            if (viewportW <= 1 || viewportH <= 1)
            {
                return false;
            }

            double sx = viewportW / _currentImage.Width;
            double sy = viewportH / _currentImage.Height;
            double fit = Math.Min(sx, sy);

            // 允许缩小和放大，保证整幅图尽量占据显示框大部分区域并居中
            _zoomScale = Math.Max(0.1, Math.Min(10.0, fit));

            if (_scaleTransform != null)
            {
                _scaleTransform.ScaleX = _zoomScale;
                _scaleTransform.ScaleY = _zoomScale;
            }

            UpdateDisplay();
            Dispatcher.BeginInvoke(new Action(CenterImageInViewport), DispatcherPriority.Background);
            return true;
        }

        private void CenterImageInViewport()
        {
            if (_currentImage == null || _currentImage.IsEmpty || _translateTransform == null)
            {
                return;
            }

            double viewportW = ScrollViewer.ViewportWidth > 1 ? ScrollViewer.ViewportWidth : Math.Max(1, ActualWidth - 20);
            double viewportH = ScrollViewer.ViewportHeight > 1 ? ScrollViewer.ViewportHeight : Math.Max(1, ActualHeight - 20);
            double shownW = _currentImage.Width * _zoomScale;
            double shownH = _currentImage.Height * _zoomScale;

            _translateTransform.X = (viewportW - shownW) / 2.0;
            _translateTransform.Y = (viewportH - shownH) / 2.0;
        }

        private void ImageDisplayControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isFittedToView && _currentImage != null && !_currentImage.IsEmpty)
            {
                _isFittedToView = FitToViewport();
            }
        }

        private void ImageDisplayControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 首帧尚未完成适配时，窗口尺寸变化后再尝试一次，避免初次显示很小
            if (!_isFittedToView && _currentImage != null && !_currentImage.IsEmpty)
            {
                _isFittedToView = FitToViewport();
            }
        }

        /// <summary>
        /// 鼠标移动事件（用于平移）
        /// </summary>
        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning && (e.LeftButton == MouseButtonState.Pressed || e.MiddleButton == MouseButtonState.Pressed || IsMouseCaptured))
            {
                System.Windows.Point currentPoint = e.GetPosition(this);
                Vector offset = currentPoint - _panStartPoint;
                if (_translateTransform != null)
                {
                    _translateTransform.X += offset.X;
                    _translateTransform.Y += offset.Y;
                }

                _panStartPoint = currentPoint;
            }
        }

        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Middle)
            {
                _isPanning = true;
                _panStartPoint = e.GetPosition(this);
                CaptureMouse();
                this.Cursor = Cursors.Hand;
                e.Handled = true;
            }
        }

        /// <summary>
        /// 鼠标释放事件
        /// </summary>
        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isPanning)
            {
                return;
            }

            if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Middle)
            {
                _isPanning = false;
                if (IsMouseCaptured)
                {
                    ReleaseMouseCapture();
                }
                this.Cursor = Cursors.Arrow;
                e.Handled = true;
            }
        }

        /// <summary>
        /// 鼠标离开事件
        /// </summary>
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            // 拖拽时允许鼠标移出控件，直到 MouseUp 才结束平移
            if (IsMouseCaptured)
            {
                return;
            }

            _isPanning = false;
            this.Cursor = Cursors.Arrow;
        }
    }
}