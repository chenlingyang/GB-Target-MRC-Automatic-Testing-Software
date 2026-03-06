using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        private Mat? _currentImage;
        private ScaleTransform? _scaleTransform;

        public ImageDisplayControl()
        {
            InitializeComponent();
            _scaleTransform = DisplayImage.RenderTransform as ScaleTransform;
            if (_scaleTransform == null)
            {
                _scaleTransform = new ScaleTransform(1.0, 1.0);
                DisplayImage.RenderTransform = _scaleTransform;
            }
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
            _zoomScale = 1.0;
            if (_scaleTransform != null)
            {
                _scaleTransform.ScaleX = 1.0;
                _scaleTransform.ScaleY = 1.0;
            }
            ScrollViewer.ScrollToHorizontalOffset(0);
            ScrollViewer.ScrollToVerticalOffset(0);
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
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
                ZoomScale *= zoomFactor;
                if (_scaleTransform != null)
                {
                    _scaleTransform.ScaleX = _zoomScale;
                    _scaleTransform.ScaleY = _zoomScale;
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// 鼠标移动事件（用于平移）
        /// </summary>
        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning && e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point currentPoint = e.GetPosition(this);
                Vector offset = currentPoint - _panStartPoint;
                
                ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset - offset.X);
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - offset.Y);
                
                _panStartPoint = currentPoint;
            }
        }

        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed || 
                (e.LeftButton == MouseButtonState.Pressed && Keyboard.Modifiers == ModifierKeys.Shift))
            {
                _isPanning = true;
                _panStartPoint = e.GetPosition(this);
                this.Cursor = Cursors.Hand;
            }
        }

        /// <summary>
        /// 鼠标释放事件
        /// </summary>
        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isPanning = false;
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// 鼠标离开事件
        /// </summary>
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            _isPanning = false;
            this.Cursor = Cursors.Arrow;
        }
    }
}
