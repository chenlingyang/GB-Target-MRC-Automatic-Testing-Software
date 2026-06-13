using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Microsoft.Win32;
using Emgu.CV;
using ImageCaptureApp.Modules;
using ImageCaptureApp.Config;
using System.Threading;
using System.Windows.Threading;

namespace ImageCaptureApp
{
    public partial class MainWindow : Window
    {
        private ICaptureModule? _captureModule;
        private Mat? _currentDisplayImage;
        private Mat? _originalImage;
        private bool _isGrayscaleMode = false;
        private bool _isMrcRealtimeEnabled = false;
        private bool _isMrcRealtimeProcessing = false;
        private DateTime _lastMrcRealtimeAt = DateTime.MinValue;
        private CancellationTokenSource? _mrcCts;
        private string SelectedMrcPipeline =>
            CmbMrcPipeline.SelectedItem is ComboBoxItem item && item.Tag is string tag ? tag : "550";
        private ImageStorageModule.ImageFormat _saveFormat = ImageStorageModule.ImageFormat.PNG;
        private DispatcherTimer? _statusUpdateTimer;
        private CaptureDeviceConfig? _config;
        private readonly ObservableCollection<MrcResultRow> _mrcResultRows = new();
        private string? _lastDistributionPlotPath;

        public MainWindow()
        {
            InitializeComponent();

            // 加载配置
            _config = CaptureDeviceConfig.Load();

            ApplyConfigToUi();
            InitializeCaptureModule();
            InitializeStatusTimer();

            // 设置快捷键命令
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, (s, e) => SaveCurrentImage()));
            TxtMrcFolder.Text = TxtSaveFolder.Text;
            TxtCcfPath.Text = _config?.DeviceSettings.SaperaCcfPath ?? string.Empty;
            MrcResultGrid.ItemsSource = _mrcResultRows;
        }

        private sealed class MrcResultRow
        {
            public string ImageName { get; set; } = string.Empty;
            public int? MinResolvableGroupId { get; set; }
            public string MinGroupDisplay { get; set; } = "--";
            public string CMeanDisplay { get; set; } = "--";
            public string AbnormalFilterDisplay { get; set; } = "已过滤";
            public string Status { get; set; } = "待处理";
            public string LabeledImagePath { get; set; } = string.Empty;
        }

        /// <summary>
        /// 初始化采集模块
        /// </summary>
        private void InitializeCaptureModule()
        {
            // 释放旧模块
            if (_captureModule != null)
            {
                _captureModule.FrameReceived -= CaptureModule_FrameReceived;
                _captureModule.CaptureStateChanged -= CaptureModule_CaptureStateChanged;
                _captureModule.Dispose();
                _captureModule = null;
            }

            if (_config == null)
            {
                StatusInfo.Text = "配置加载失败，已使用默认设置";
                _config = new CaptureDeviceConfig();
            }

            // 采集源固定为 Teledyne DALSA（SaperaLT），不再进行源切换与回退
            const string fixedSource = "TeledyneDalsaSaperaLt";
            _config.DeviceSettings.CaptureSource = fixedSource;
            _captureModule = CreateCaptureModule(fixedSource);
            _captureModule.FrameReceived += CaptureModule_FrameReceived;
            _captureModule.CaptureStateChanged += CaptureModule_CaptureStateChanged;

            bool ok = _captureModule.Initialize(_config.DeviceSettings);
            string? lastError = _captureModule.LastError;

            if (ok && _captureModule != null)
            {
                _config.Save();

                var (actualW, actualH) = _captureModule.GetResolution();
                StatusInfo.Text = $"采集卡已连接: {_config.DeviceSettings.DeviceName} ({actualW}x{actualH})";

                if (actualW < _config.DeviceSettings.Resolution.Width || actualH < _config.DeviceSettings.Resolution.Height)
                {
                    MessageBox.Show(
                        $"当前设备实际分辨率为 {actualW}x{actualH}，低于你配置的 {_config.DeviceSettings.Resolution.Width}x{_config.DeviceSettings.Resolution.Height}。\n" +
                        "这通常是设备/驱动可用分辨率限制，可尝试在相机驱动或采集卡工具里先设置输出模式。",
                        "分辨率提示",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                if (_config.DeviceSettings.AutoStart)
                {
                    _captureModule.StartCapture();
                }
            }
            else
            {
                if (_captureModule != null)
                {
                    _captureModule.FrameReceived -= CaptureModule_FrameReceived;
                    _captureModule.CaptureStateChanged -= CaptureModule_CaptureStateChanged;
                    _captureModule.Dispose();
                    _captureModule = null;
                }

                string err = lastError ?? "初始化失败";
                StatusInfo.Text = err.Replace("\n", " ");
                MessageBox.Show(err, "采集模块初始化失败", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ApplyConfigToUi()
        {
            if (_config == null) return;

            // 采集源固定为 Teledyne DALSA（SaperaLT）
            const string src = "TeledyneDalsaSaperaLt";
            _config.DeviceSettings.CaptureSource = src;
            for (int i = 0; i < CmbCaptureSource.Items.Count; i++)
            {
                if (CmbCaptureSource.Items[i] is System.Windows.Controls.ComboBoxItem item &&
                    item.Tag is string tag &&
                    string.Equals(tag, src, StringComparison.OrdinalIgnoreCase))
                {
                    CmbCaptureSource.SelectedIndex = i;
                    break;
                }
            }

            if (CmbCaptureSource.SelectedIndex < 0 && CmbCaptureSource.Items.Count > 0)
            {
                CmbCaptureSource.SelectedIndex = 0;
            }

            _saveFormat = ImageStorageModule.ParseFormatOrDefault(_config.StorageSettings.DefaultFormat);
            SelectSaveFormatInUi(_saveFormat);
            TxtCcfPath.Text = _config.DeviceSettings.SaperaCcfPath ?? string.Empty;
        }

        private void CmbCaptureSource_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // 采集源固定，不处理切换。
        }

        private void CmbSaveFormat_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!IsLoaded || _config == null) return;
            if (CmbSaveFormat.SelectedItem is not System.Windows.Controls.ComboBoxItem item) return;
            if (item.Tag is not string formatTag) return;

            _saveFormat = ImageStorageModule.ParseFormatOrDefault(formatTag, ImageStorageModule.ImageFormat.PNG);
            _config.StorageSettings.DefaultFormat = ImageStorageModule.ToConfigFormatString(_saveFormat);
            _config.Save();
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
                if (!isCapturing && _isMrcRealtimeEnabled)
                {
                    _isMrcRealtimeEnabled = false;
                    BtnMrcProcess.Content = "MRC处理";
                }
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

                    TryStartRealtimeMrcIfNeeded();
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
                        MessageBox.Show(
                            _captureModule.LastError ?? "启动采集失败，请检查采集卡连接。",
                            "错误",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }

        private static ICaptureModule CreateCaptureModule(string source)
        {
            return string.Equals(source, "TeledyneDalsaSaperaLt", StringComparison.Ordinal)
                ? new TeledyneDalsaSaperaLtCaptureModule()
                : new ImageCaptureModule();
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
        /// MRC处理按钮
        /// </summary>
        private void BtnMrcProcess_Click(object sender, RoutedEventArgs e)
        {
            if (_captureModule == null || !_captureModule.IsCapturing)
            {
                MessageBox.Show("请先开始采集，再开启MRC实时处理。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _isMrcRealtimeEnabled = !_isMrcRealtimeEnabled;
            BtnMrcProcess.Content = _isMrcRealtimeEnabled ? "停止MRC实时" : "MRC处理";
            StatusInfo.Text = _isMrcRealtimeEnabled ? "MRC实时处理中..." : "MRC实时处理已停止";
        }

        private void BtnMrcSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "选择待处理图像文件夹"
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TxtMrcFolder.Text = dialog.SelectedPath;
            }
        }

        private void BtnSelectCcf_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "选择相机 CCF 配置文件",
                Filter = "CCF文件 (*.ccf)|*.ccf|所有文件 (*.*)|*.*",
                CheckFileExists = true,
                Multiselect = false
            };
            if (dialog.ShowDialog() == true)
            {
                TxtCcfPath.Text = dialog.FileName;
            }
        }

        private void BtnApplyCcf_Click(object sender, RoutedEventArgs e)
        {
            if (_config == null)
            {
                MessageBox.Show("配置未加载，无法应用 CCF。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string ccfPath = TxtCcfPath.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(ccfPath) || !File.Exists(ccfPath))
            {
                MessageBox.Show("请选择有效的 CCF 文件。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            bool wasCapturing = _captureModule?.IsCapturing == true;
            try
            {
                _captureModule?.StopCapture();
                _config.DeviceSettings.SaperaCcfPath = ccfPath;
                _config.Save();
                InitializeCaptureModule();
                if (_captureModule == null)
                {
                    StatusInfo.Text = "CCF配置应用失败";
                    return;
                }

                if (wasCapturing && _captureModule != null && !_captureModule.IsCapturing)
                {
                    _captureModule.StartCapture();
                }

                StatusInfo.Text = "CCF配置已应用";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"应用 CCF 失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnMrcProcessFolder_Click(object sender, RoutedEventArgs e)
        {
            string folder = TxtMrcFolder.Text;
            if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
            {
                MessageBox.Show("请选择有效的 MRC 图像文件夹。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string[] imageFiles = MrcProcessingModule.CollectImageFiles(folder);
            if (imageFiles.Length == 0)
            {
                MessageBox.Show("目标文件夹中未找到可处理图像（bmp/jpg/png/tif）。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _mrcCts?.Cancel();
            _mrcCts = new CancellationTokenSource();
            CancellationToken token = _mrcCts.Token;

            BtnMrcProcessFolder.Visibility = Visibility.Collapsed;
            BtnMrcStop.Visibility = Visibility.Visible;
            BtnMrcSelectFolder.IsEnabled = false;
            BtnMrcProcess.IsEnabled = false;
            TxtMrcProgress.Text = $"处理中（{imageFiles.Length} 张图像，批量模式）...";
            StatusInfo.Text = "MRC文件夹处理中...";
            _mrcResultRows.Clear();
            BatchMrcSummaryPanel.Visibility = Visibility.Collapsed;
            TxtBatchMrcSummary.Text = string.Empty;
            _lastDistributionPlotPath = null;

            string outputFolder = Path.Combine(folder, "mrc_result");
            int totalImages = imageFiles.Length;

            try
            {
                // 一次 Python 调用处理全部图像（--input-dir 批量模式），实时推送进度
                MrcProcessingModule.FolderProcessResult batchResult =
                    await MrcProcessingModule.ProcessFolderAsync(
                        folder, null, outputFolder, SelectedMrcPipeline, token,
                        onProgress: (current, total, imageName) =>
                        {
                            Dispatcher.Invoke(() =>
                            {
                                TxtMrcProgress.Text = $"处理中 {current}/{total}: {imageName}";

                                // 实时读取 summary.json 更新结果表
                                string stem = Path.GetFileNameWithoutExtension(imageName);
                                var entry = MrcProcessingModule.TryReadImageSummary(outputFolder, stem);
                                if (entry != null)
                                {
                                    UpsertMrcResultRow(imageName, new MrcProcessResult
                                    {
                                        Success = true,
                                        Message = "完成",
                                        OutputDirectory = outputFolder,
                                        LabeledImagePath = entry.LabeledImagePath,
                                        OverviewImagePath = entry.OverviewImagePath,
                                        ExcelPath = entry.ExcelPath,
                                        CurvePath = entry.CurvePath,
                                        SummaryJsonPath = entry.SummaryJsonPath,
                                        MinResolvableGroupId = entry.MinResolvableGroupId,
                                        MinResolvableCMean = entry.MinResolvableCMean
                                    });
                                }
                                else
                                {
                                    // 还没生成 JSON（可能刚 [OK]），先加个处理中行
                                    UpsertMrcResultRow(imageName, new MrcProcessResult
                                    {
                                        Success = false,
                                        Message = "处理中...",
                                        OutputDirectory = outputFolder
                                    });
                                }
                            });
                        });

                if (!batchResult.Success)
                {
                    StatusInfo.Text = batchResult.Message;
                    TxtMrcProgress.Text = "处理失败";
                    MessageBox.Show(batchResult.Message, "MRC批处理失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int success = 0;
                int failed = 0;
                string? firstLabeledImage = null;

                foreach (var entry in batchResult.Entries)
                {
                    if (entry.Success)
                    {
                        success++;
                        if (firstLabeledImage == null && !string.IsNullOrWhiteSpace(entry.LabeledImagePath) && File.Exists(entry.LabeledImagePath))
                            firstLabeledImage = entry.LabeledImagePath;
                    }
                    else
                    {
                        failed++;
                    }

                    UpsertMrcResultRow(entry.ImageName, new MrcProcessResult
                    {
                        Success = entry.Success,
                        Message = entry.Message,
                        OutputDirectory = entry.OutputDirectory,
                        LabeledImagePath = entry.LabeledImagePath,
                        OverviewImagePath = entry.OverviewImagePath,
                        ExcelPath = entry.ExcelPath,
                        CurvePath = entry.CurvePath,
                        SummaryJsonPath = entry.SummaryJsonPath,
                        MinResolvableGroupId = entry.MinResolvableGroupId,
                        MinResolvableCMean = entry.MinResolvableCMean
                    });
                }

                if (!string.IsNullOrWhiteSpace(firstLabeledImage))
                    ShowResultImage(firstLabeledImage);

                await ApplyBatchDistributionSummaryAsync(outputFolder, batchResult.Entries.Count, success, failed, SelectedMrcPipeline);

                StatusInfo.Text = $"MRC文件夹处理完成：成功{success}，失败{failed}";
                TxtMrcProgress.Text = $"完成 成功{success}/失败{failed}";
                string batchConclusion = TxtBatchMrcSummary.Text;
                MessageBox.Show(
                    $"MRC文件夹处理完成。\n总数：{batchResult.Entries.Count}\n成功：{success}\n失败：{failed}\n{batchConclusion}\n汇总表：{batchResult.SummaryCsvPath}\n输出目录：{outputFolder}",
                    "MRC批处理完成",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (OperationCanceledException)
            {
                StatusInfo.Text = "MRC处理已取消";
                TxtMrcProgress.Text = "已取消";
            }
            catch (Exception ex)
            {
                StatusInfo.Text = "MRC文件夹处理异常";
                TxtMrcProgress.Text = "处理异常";
                MessageBox.Show($"处理文件夹时发生异常：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                BtnMrcProcessFolder.Visibility = Visibility.Visible;
                BtnMrcStop.Visibility = Visibility.Collapsed;
                BtnMrcSelectFolder.IsEnabled = true;
                BtnMrcProcess.IsEnabled = true;
                _mrcCts?.Dispose();
                _mrcCts = null;
            }
        }

        private void BtnMrcStop_Click(object sender, RoutedEventArgs e)
        {
            // 停止实时 MRC
            if (_isMrcRealtimeEnabled)
            {
                _isMrcRealtimeEnabled = false;
                BtnMrcProcess.Content = "MRC处理";
                StatusInfo.Text = "MRC实时处理已停止";
            }

            // 停止批量处理
            _mrcCts?.Cancel();
        }

        private void TryStartRealtimeMrcIfNeeded()
        {
            if (!_isMrcRealtimeEnabled || _isMrcRealtimeProcessing)
            {
                return;
            }
            if (_originalImage == null || _originalImage.IsEmpty)
            {
                return;
            }
            // 节流：避免每帧都跑一次 MRC 导致卡顿
            if ((DateTime.Now - _lastMrcRealtimeAt).TotalMilliseconds < 1200)
            {
                return;
            }

            _lastMrcRealtimeAt = DateTime.Now;
            _isMrcRealtimeProcessing = true;
            Mat sourceCopy = _originalImage.Clone();
            string outputRoot = Directory.Exists(TxtSaveFolder.Text)
                ? TxtSaveFolder.Text
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Captures");
            Directory.CreateDirectory(outputRoot);

            _ = Task.Run(async () =>
            {
                try
                {
                    using (sourceCopy)
                    {
                        MrcProcessResult result = await MrcProcessingModule.ProcessCurrentFrameAsync(sourceCopy, null, outputRoot, SelectedMrcPipeline);
                        await Dispatcher.InvokeAsync(() =>
                        {
                            _isMrcRealtimeProcessing = false;
                            if (!result.Success)
                            {
                                StatusInfo.Text = result.Message;
                                return;
                            }

                            if (!string.IsNullOrWhiteSpace(result.LabeledImagePath) && File.Exists(result.LabeledImagePath))
                            {
                                ShowResultImage(result.LabeledImagePath);
                            }

                            UpsertMrcResultRow($"实时帧 {DateTime.Now:HH:mm:ss}", result);
                            string minGroupText = result.MinResolvableGroupId.HasValue
                                ? $"最小分辨组: {result.MinResolvableGroupId} (C={result.MinResolvableCMean?.ToString("F4", CultureInfo.InvariantCulture)})"
                                : "最小分辨组: 未找到";
                            StatusInfo.Text = $"MRC实时更新 {DateTime.Now:HH:mm:ss} | {minGroupText}";
                        });
                    }
                }
                catch (Exception ex)
                {
                    await Dispatcher.InvokeAsync(() =>
                    {
                        _isMrcRealtimeProcessing = false;
                        StatusInfo.Text = $"MRC实时处理异常: {ex.Message}";
                    });
                }
            });
        }

        private static string Csv(string? value)
        {
            string text = value ?? string.Empty;
            if (!text.Contains(',') && !text.Contains('"') && !text.Contains('\n') && !text.Contains('\r'))
            {
                return text;
            }
            return $"\"{text.Replace("\"", "\"\"")}\"";
        }

        private void ShowResultImage(string labeledImagePath)
        {
            Mat show = CvInvoke.Imread(labeledImagePath, Emgu.CV.CvEnum.ImreadModes.Color);
            if (show.IsEmpty)
            {
                show.Dispose();
                return;
            }

            ResultImageDisplay.SetImage(show);
            show.Dispose();
        }

        private void ShowDistributionImage(string plotPath)
        {
            Mat show = CvInvoke.Imread(plotPath, Emgu.CV.CvEnum.ImreadModes.Color);
            if (show.IsEmpty)
            {
                show.Dispose();
                return;
            }

            DistributionImageDisplay.SetImage(show);
            show.Dispose();
            _lastDistributionPlotPath = plotPath;
            MrcResultTabs.SelectedItem = TabMrcDistribution;
        }

        private async Task ApplyBatchDistributionSummaryAsync(
            string outputFolder,
            int totalProcessed,
            int successCount,
            int failedCount,
            string pipeline = "1m6")
        {
            IEnumerable<int?> groupIds = _mrcResultRows
                .Where(row => string.Equals(row.Status, "完成", StringComparison.Ordinal))
                .Select(row => row.MinResolvableGroupId);

            MrcBatchSummaryHelper.BatchGroupDistribution distribution = MrcBatchSummaryHelper.Analyze(groupIds);
            string conclusion = MrcBatchSummaryHelper.FormatConclusion(distribution, totalProcessed, failedCount);

            if (distribution.ValidImageCount == 0)
            {
                BatchMrcSummaryPanel.Visibility = Visibility.Visible;
                TxtBatchMrcSummary.Text = conclusion;
                return;
            }

            int[] validGroupIds = groupIds
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToArray();

            MrcProcessingModule.BatchDistributionPlotResult plotResult =
                await MrcProcessingModule.GenerateBatchDistributionPlotAsync(validGroupIds, outputFolder, pipeline: pipeline);

            BatchMrcSummaryPanel.Visibility = Visibility.Visible;
            if (plotResult.Success && !string.IsNullOrWhiteSpace(plotResult.PlotPath) && File.Exists(plotResult.PlotPath))
            {
                ShowDistributionImage(plotResult.PlotPath);
                int? batchGroup = plotResult.BatchMinResolvableGroupId ?? distribution.BatchMinResolvableGroupId;
                if (batchGroup.HasValue)
                {
                    conclusion = MrcBatchSummaryHelper.FormatConclusion(
                        new MrcBatchSummaryHelper.BatchGroupDistribution
                        {
                            CountByGroupId = distribution.CountByGroupId,
                            ValidImageCount = distribution.ValidImageCount,
                            BatchMinResolvableGroupId = batchGroup,
                            PeakCount = distribution.CountByGroupId.TryGetValue(batchGroup.Value, out int peak)
                                ? peak
                                : distribution.PeakCount
                        },
                        totalProcessed,
                        failedCount);
                }
            }
            else if (!plotResult.Success)
            {
                conclusion += $"（分布图生成失败：{plotResult.Message}）";
            }

            TxtBatchMrcSummary.Text = conclusion;
            StatusInfo.Text = conclusion;
        }

        private void UpsertMrcResultRow(string imageName, MrcProcessResult result)
        {
            string key = imageName?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(key))
            {
                key = "未命名图像";
            }

            string minGroupDisplay = result.MinResolvableGroupId?.ToString() ?? "未找到";
            string cMeanDisplay = result.MinResolvableCMean?.ToString("F4", CultureInfo.InvariantCulture) ?? "--";
            string status = result.Success ? "完成" : $"失败: {result.Message}";
            string labeledPath = result.LabeledImagePath ?? string.Empty;
            if (status.Length > 90)
            {
                status = status[..90] + "...";
            }

            MrcResultRow? existing = _mrcResultRows.FirstOrDefault(x => string.Equals(x.ImageName, key, StringComparison.OrdinalIgnoreCase));
            if (existing == null)
            {
                _mrcResultRows.Insert(0, new MrcResultRow
                {
                    ImageName = key,
                    MinResolvableGroupId = result.MinResolvableGroupId,
                    MinGroupDisplay = minGroupDisplay,
                    CMeanDisplay = cMeanDisplay,
                    AbnormalFilterDisplay = "已过滤",
                    Status = status,
                    LabeledImagePath = labeledPath
                });
                return;
            }

            existing.MinResolvableGroupId = result.MinResolvableGroupId;
            existing.MinGroupDisplay = minGroupDisplay;
            existing.CMeanDisplay = cMeanDisplay;
            existing.AbnormalFilterDisplay = "已过滤";
            existing.Status = status;
            existing.LabeledImagePath = labeledPath;
            MrcResultGrid.Items.Refresh();
        }

        private void MrcResultGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MrcResultGrid.SelectedItem is not MrcResultRow row)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(row.LabeledImagePath) || !File.Exists(row.LabeledImagePath))
            {
                return;
            }

            ShowResultImage(row.LabeledImagePath);
            StatusInfo.Text = $"显示结果图: {row.ImageName}";
        }

        private async Task<MrcProcessResult> RunMrcWithCurrentImageAsync(string outputRoot)
        {
            if (_originalImage == null || _originalImage.IsEmpty)
            {
                return new MrcProcessResult { Success = false, Message = "当前没有可处理图像。" };
            }

            using Mat sourceCopy = _originalImage.Clone();
            return await MrcProcessingModule.ProcessCurrentFrameAsync(sourceCopy, null, outputRoot);
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
            if (ImageStorageModule.SaveImageWithDialog(_currentDisplayImage, fileName, _saveFormat))
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
                    TxtMrcFolder.Text = dialog.SelectedPath;
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
                        },
                        format: _saveFormat
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

        private void SelectSaveFormatInUi(ImageStorageModule.ImageFormat format)
        {
            string target = ImageStorageModule.ToConfigFormatString(format);
            for (int i = 0; i < CmbSaveFormat.Items.Count; i++)
            {
                if (CmbSaveFormat.Items[i] is System.Windows.Controls.ComboBoxItem item &&
                    item.Tag is string tag &&
                    string.Equals(tag, target, StringComparison.OrdinalIgnoreCase))
                {
                    CmbSaveFormat.SelectedIndex = i;
                    return;
                }
            }

            if (CmbSaveFormat.Items.Count > 0)
            {
                CmbSaveFormat.SelectedIndex = 0;
            }
        }
    }
}