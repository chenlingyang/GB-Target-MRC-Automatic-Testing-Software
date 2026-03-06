using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Emgu.CV;
using ImageCaptureApp.Modules;
using ImageCaptureApp.Config;
using System.Windows.Threading;

namespace ImageCaptureApp
{
    public partial class MainWindow : Window
    {
        private ImageCaptureModule? _captureModule;
        private Mat? _currentDisplayImage;
        private Mat? _originalImage;
        private bool _isGrayscaleMode = false;
        private DispatcherTimer? _statusUpdateTimer;
        private CaptureDeviceConfig? _config;

        public MainWindow()
        {
            InitializeComponent();

            // 加载配置
            _config = CaptureDeviceConfig.Load();

            InitializeCaptureModule();
            InitializeStatusTimer();

            // 设置快捷键命令
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, (s, e) => SaveCurrentImage()));
        }

        /// <summary>
        /// 初始化采集模块
        /// </summary>
        private void InitializeCaptureModule()
        {
            _captureModule = new ImageCaptureModule();
            _captureModule.FrameReceived += CaptureModule_FrameReceived;
            _captureModule.CaptureStateChanged += CaptureModule_CaptureStateChanged;

            // 从配置文件读取设备设置
            if (_config != null)
            {
                int deviceIndex = _config.DeviceSettings.DeviceIndex;

                if (_captureModule.Initialize(deviceIndex))
                {
                    // 应用配置的分辨率和帧率
                    _captureModule.SetResolution(
                        _config.DeviceSettings.Resolution.Width,
                        _config.DeviceSettings.Resolution.Height);
                    _captureModule.SetFrameRate(_config.DeviceSettings.FrameRate);

                    StatusInfo.Text = $"采集卡已连接: {_config.DeviceSettings.DeviceName}";

                    // 如果配置了自动启动，则开始采集
                    if (_config.DeviceSettings.AutoStart)
                    {
                        _captureModule.StartCapture();
                    }
                }
                else
                {
                    StatusInfo.Text = $"未检测到采集卡设备 (索引: {deviceIndex})";
                }
            }
            else
            {
                // 使用默认设置
                if (_captureModule.Initialize(0))
                {
                    StatusInfo.Text = "采集卡已连接";
                }
                else
                {
                    StatusInfo.Text = "未检测到采集卡设备";
                }
            }
        }

        /// <summary>
        /// 初始化状态更新定时器
        /// </summary>
        private void InitializeStatusTimer()
        {
            _statusUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _statusUpdateTimer.Tick += StatusUpdateTimer_Tick;
            _statusUpdateTimer.Start();
        }

        /// <summary>
        /// 采集状态变化事件
        /// </summary>
        private void CaptureModule_CaptureStateChanged(object? sender, bool isCapturing)
        {
            Dispatcher.Invoke(() =>
            {
                BtnStart.IsEnabled = !isCapturing;
                BtnStop.IsEnabled = isCapturing;
                StatusInfo.Text = isCapturing ? "正在采集..." : "已停止采集";
            });
        }

        /// <summary>
        /// 新帧到达事件
        /// </summary>
        private void CaptureModule_FrameReceived(object? sender, Mat frame)
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    // 保存原始图像
                    _originalImage?.Dispose();
                    _originalImage = frame.Clone();

                    // 根据模式处理图像
                    Mat displayImage = _isGrayscaleMode
                        ? ImageProcessingModule.ConvertToGrayscale(_originalImage)
                        : _originalImage.Clone();

                    // 更新显示
                    _currentDisplayImage?.Dispose();
                    _currentDisplayImage = displayImage;
                    ImageDisplay.SetImage(_currentDisplayImage);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"处理帧失败: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// 状态更新定时器事件
        /// </summary>
        private void StatusUpdateTimer_Tick(object? sender, EventArgs e)
        {
            if (_captureModule != null)
            {
                var (width, height) = _captureModule.GetResolution();
                double fps = _captureModule.GetFrameRate();

                StatusResolution.Text = $"分辨率: {width}x{height}";
                StatusFrameRate.Text = $"帧率: {fps:F1} fps";
            }

            StatusZoom.Text = $"缩放: {(int)(ImageDisplay.ZoomScale * 100)}%";
        }

        /// <summary>
        /// 开始采集按钮
        /// </summary>
        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_captureModule != null)
            {
                if (!_captureModule.IsCapturing)
                {
                    if (_captureModule.StartCapture())
                    {
                        StatusInfo.Text = "开始采集";
                    }
                    else
                    {
                        MessageBox.Show("启动采集失败，请检查采集卡连接。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// 停止采集按钮
        /// </summary>
        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            _captureModule?.StopCapture();
        }

        /// <summary>
        /// 保存按钮
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentImage();
        }

        /// <summary>
        /// 灰度化按钮
        /// </summary>
        private void BtnGrayscale_Click(object sender, RoutedEventArgs e)
        {
            ToggleGrayscale();
        }

        /// <summary>
        /// 放大按钮
        /// </summary>
        private void BtnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ImageDisplay.ZoomIn();
        }

        /// <summary>
        /// 缩小按钮
        /// </summary>
        private void BtnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ImageDisplay.ZoomOut();
        }

        /// <summary>
        /// 重置按钮
        /// </summary>
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ImageDisplay.ResetView();
        }

        /// <summary>
        /// 菜单 - 保存
        /// </summary>
        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentImage();
        }

        /// <summary>
        /// 菜单 - 退出
        /// </summary>
        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 菜单 - 放大
        /// </summary>
        private void MenuZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ImageDisplay.ZoomIn();
        }

        /// <summary>
        /// 菜单 - 缩小
        /// </summary>
        private void MenuZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ImageDisplay.ZoomOut();
        }

        /// <summary>
        /// 菜单 - 重置视图
        /// </summary>
        private void MenuResetView_Click(object sender, RoutedEventArgs e)
        {
            ImageDisplay.ResetView();
        }

        /// <summary>
        /// 菜单 - 灰度化
        /// </summary>
        private void MenuGrayscale_Click(object sender, RoutedEventArgs e)
        {
            ToggleGrayscale();
        }

        /// <summary>
        /// 菜单 - 关于
        /// </summary>
        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "图像采集卡上位机软件\n\n" +
                "版本: 1.0\n" +
                "功能:\n" +
                "- 图像采集\n" +
                "- 图像缩放\n" +
                "- 图像灰度化\n" +
                "- 图像保存\n" +
                "- 批量采集\n\n" +
                "使用说明:\n" +
                "- Ctrl+滚轮: 缩放图像\n" +
                "- Shift+左键拖拽: 平移图像\n" +
                "- 中键拖拽: 平移图像",
                "关于",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        /// <summary>
        /// 窗口按键事件
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Add || e.Key == Key.OemPlus)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    ImageDisplay.ZoomIn();
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    ImageDisplay.ZoomOut();
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.D0)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    ImageDisplay.ResetView();
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// 保存当前图像
        /// </summary>
        private void SaveCurrentImage()
        {
            if (_currentDisplayImage == null || _currentDisplayImage.IsEmpty)
            {
                MessageBox.Show("没有可保存的图像。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string fileName = $"image_{DateTime.Now:yyyyMMdd_HHmmss}";
            if (ImageStorageModule.SaveImageWithDialog(_currentDisplayImage, fileName))
            {
                StatusInfo.Text = "图像已保存";
            }
        }

        /// <summary>
        /// 切换灰度化模式
        /// </summary>
        private void ToggleGrayscale()
        {
            _isGrayscaleMode = !_isGrayscaleMode;

            MenuGrayscale.IsChecked = _isGrayscaleMode;
            BtnGrayscale.Content = _isGrayscaleMode ? "恢复彩色" : "灰度化";

            // 如果有原始图像，重新处理
            if (_originalImage != null && !_originalImage.IsEmpty)
            {
                Mat displayImage = _isGrayscaleMode
                    ? ImageProcessingModule.ConvertToGrayscale(_originalImage)
                    : _originalImage.Clone();

                _currentDisplayImage?.Dispose();
                _currentDisplayImage = displayImage;
                ImageDisplay.SetImage(_currentDisplayImage);
            }
        }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            _captureModule?.StopCapture();
            _captureModule?.Dispose();
            _currentDisplayImage?.Dispose();
            _originalImage?.Dispose();
            _statusUpdateTimer?.Stop();

            // 保存配置
            _config?.Save();

            base.OnClosed(e);
        }

        // ==================== 新增批量采集相关代码 ====================

        /// <summary>
        /// 浏览文件夹按钮
        /// </summary>
        private void BtnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = "选择保存文件夹";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    TxtSaveFolder.Text = dialog.SelectedPath;
                }
            }
        }

        /// <summary>
        /// 批量采集按钮
        /// </summary>
        private async void BtnBatchCapture_Click(object sender, RoutedEventArgs e)
        {
            // 参数验证
            if (!int.TryParse(TxtBatchCount.Text, out int count) || count <= 0)
            {
                MessageBox.Show("请输入有效的采集数量（正整数）", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string folder = TxtSaveFolder.Text;
            if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
            {
                MessageBox.Show("请选择有效的保存文件夹", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 检查采集模块是否已初始化并正在采集
            if (_captureModule == null || !_captureModule.IsCapturing)
            {
                MessageBox.Show("请先开始采集", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 禁用按钮，更新状态
            BtnBatchCapture.IsEnabled = false;
            BtnSelectFolder.IsEnabled = false;
            TxtBatchCount.IsEnabled = false;
            TxtBatchProgress.Text = "采集中...";

            try
            {
                // 在后台线程执行批量采集，避免阻塞UI
                int saved = await Task.Run(() =>
                {
                    return _captureModule.BatchCaptureAndSave(
                        folder,
                        count,
                        fileNamePrefix: "capture",
                        delayMs: 0,  // 连续采集，可根据需要设置间隔（毫秒）
                        onProgress: (progress) =>
                        {
                            // 通过 Dispatcher 更新UI进度
                            Dispatcher.Invoke(() =>
                            {
                                TxtBatchProgress.Text = $"已保存: {progress}/{count}";
                            });
                        }
                    );
                });

                TxtBatchProgress.Text = $"完成，成功保存 {saved} 张";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"批量采集出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                TxtBatchProgress.Text = "出错";
            }
            finally
            {
                // 恢复按钮
                BtnBatchCapture.IsEnabled = true;
                BtnSelectFolder.IsEnabled = true;
                TxtBatchCount.IsEnabled = true;
            }
        }
    }
}