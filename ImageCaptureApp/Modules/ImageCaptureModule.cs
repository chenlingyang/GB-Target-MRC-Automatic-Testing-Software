using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using ImageCaptureApp.Config;

namespace ImageCaptureApp.Modules
{
    /// <summary>
    /// 图像采集模块 - 负责从图像采集卡获取图像数据
    /// </summary>
    public class ImageCaptureModule : IDisposable, ICaptureModule
    {
        private VideoCapture? _capture;
        private bool _isCapturing = false;
        private CancellationTokenSource? _cancellationTokenSource;
        private Mat? _currentFrame;
        private readonly object _frameLock = new object();
        private readonly object _readLock = new object(); // 新增：用于同步 _capture.Read 访问
        private int _requestedWidth = 1280;
        private int _requestedHeight = 720;
        private int _requestedFps = 30;

        public string? LastError { get; private set; }

        /// <summary>
        /// 新帧到达事件
        /// </summary>
        public event EventHandler<Mat>? FrameReceived;

        /// <summary>
        /// 采集状态变化事件
        /// </summary>
        public event EventHandler<bool>? CaptureStateChanged;

        /// <summary>
        /// 当前是否正在采集
        /// </summary>
        public bool IsCapturing => _isCapturing;

        /// <summary>
        /// 当前帧
        /// </summary>
        public Mat? CurrentFrame
        {
            get
            {
                lock (_frameLock)
                {
                    return _currentFrame?.Clone();
                }
            }
        }

        /// <summary>
        /// 初始化采集模块（从配置初始化）
        /// </summary>
        public bool Initialize(DeviceSettings settings)
        {
            LastError = null;
            if (settings == null)
            {
                LastError = "配置为空";
                return false;
            }

            _requestedWidth = Math.Max(160, settings.Resolution.Width);
            _requestedHeight = Math.Max(120, settings.Resolution.Height);
            _requestedFps = Math.Max(1, settings.FrameRate);

            if (!Initialize(settings.DeviceIndex))
            {
                if (string.IsNullOrWhiteSpace(LastError))
                {
                    LastError = $"初始化默认采集卡失败 (DeviceIndex={settings.DeviceIndex})";
                }
                return false;
            }

            // 统一在初始化阶段尽量把分辨率拉到配置值，避免部分设备默认落到 160x120
            TryApplyPreferredVideoSettings();
            return true;
        }

        /// <summary>
        /// 初始化采集卡（默认采集：OpenCV VideoCapture + DirectShow）
        /// </summary>
        /// <param name="deviceIndex">设备索引，默认为0（第一个设备）</param>
        /// <returns>是否初始化成功</returns>
        public bool Initialize(int deviceIndex = 0)
        {
            LastError = null;
            try
            {
                _capture?.Dispose();
                _capture = new VideoCapture(deviceIndex, VideoCapture.API.DShow);

                if (!_capture.IsOpened)
                {
                    LastError = $"无法打开设备 (DeviceIndex={deviceIndex})";
                    return false;
                }

                // 设置默认参数
                _capture.Set(CapProp.FrameWidth, _requestedWidth);
                _capture.Set(CapProp.FrameHeight, _requestedHeight);
                _capture.Set(CapProp.Fps, _requestedFps);

                return true;
            }
            catch (Exception ex)
            {
                LastError = $"初始化采集卡失败: {ex.Message}";
                System.Diagnostics.Debug.WriteLine(LastError);
                return false;
            }
        }

        /// <summary>
        /// 设置分辨率
        /// </summary>
        public void SetResolution(int width, int height)
        {
            _requestedWidth = Math.Max(160, width);
            _requestedHeight = Math.Max(120, height);
            if (_capture != null && _capture.IsOpened)
            {
                _capture.Set(CapProp.FrameWidth, _requestedWidth);
                _capture.Set(CapProp.FrameHeight, _requestedHeight);
            }
        }

        /// <summary>
        /// 设置帧率
        /// </summary>
        public void SetFrameRate(int fps)
        {
            _requestedFps = Math.Max(1, fps);
            if (_capture != null && _capture.IsOpened)
            {
                _capture.Set(CapProp.Fps, _requestedFps);
            }
        }

        /// <summary>
        /// 尽量应用用户偏好的视频参数（优先 1280x720+，避免设备默认 160x120）。
        /// 某些摄像头/采集卡需要 MJPG 才能输出高分辨率，这里做一次兼容设置。
        /// </summary>
        private void TryApplyPreferredVideoSettings()
        {
            if (_capture == null || !_capture.IsOpened)
            {
                return;
            }

            try
            {
                // 先尝试 MJPG，可显著提高很多设备在 DShow 下可用分辨率
                _capture.Set(CapProp.FourCC, VideoWriter.Fourcc('M', 'J', 'P', 'G'));
            }
            catch
            {
                // 某些设备不支持设置 FourCC，忽略即可
            }

            // 优先尝试配置值，然后兜底常用分辨率
            var candidates = new (int w, int h)[]
            {
                (_requestedWidth, _requestedHeight),
                (1920, 1080),
                (1280, 720),
                (1024, 768),
                (800, 600),
                (640, 480),
            };

            foreach (var (w, h) in candidates)
            {
                _capture.Set(CapProp.FrameWidth, w);
                _capture.Set(CapProp.FrameHeight, h);
                _capture.Set(CapProp.Fps, _requestedFps);

                Thread.Sleep(60);
                var (actualW, actualH) = GetResolution();
                if (actualW >= _requestedWidth && actualH >= _requestedHeight)
                {
                    return;
                }
            }

            // 仍不满足时记录信息，方便排查（可能为设备/驱动本身限制）
            var (finalW, finalH) = GetResolution();
            if (finalW < _requestedWidth || finalH < _requestedHeight)
            {
                LastError = $"当前设备实际分辨率为 {finalW}x{finalH}，低于目标 {_requestedWidth}x{_requestedHeight}（可能是设备或驱动限制）";
            }
        }

        /// <summary>
        /// 获取当前分辨率
        /// </summary>
        public (int width, int height) GetResolution()
        {
            if (_capture != null && _capture.IsOpened)
            {
                int width = (int)_capture.Get(CapProp.FrameWidth);
                int height = (int)_capture.Get(CapProp.FrameHeight);
                return (width, height);
            }
            return (0, 0);
        }

        /// <summary>
        /// 获取当前帧率
        /// </summary>
        public double GetFrameRate()
        {
            if (_capture != null && _capture.IsOpened)
            {
                return _capture.Get(CapProp.Fps);
            }
            return 0;
        }

        /// <summary>
        /// 开始采集
        /// </summary>
        public bool StartCapture()
        {
            if (_capture == null || !_capture.IsOpened)
            {
                return false;
            }

            if (_isCapturing)
            {
                return true;
            }

            _isCapturing = true;
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => CaptureLoop(_cancellationTokenSource.Token));

            CaptureStateChanged?.Invoke(this, true);
            return true;
        }

        /// <summary>
        /// 停止采集
        /// </summary>
        public void StopCapture()
        {
            if (!_isCapturing)
            {
                return;
            }

            _isCapturing = false;
            _cancellationTokenSource?.Cancel();

            CaptureStateChanged?.Invoke(this, false);
        }

        /// <summary>
        /// 采集循环
        /// </summary>
        private void CaptureLoop(CancellationToken cancellationToken)
        {
            Mat frame = new Mat();

            while (_isCapturing && !cancellationToken.IsCancellationRequested)
            {
                if (_capture != null && _capture.IsOpened)
                {
                    bool readSuccess;
                    lock (_readLock) // 加锁防止与 GetFrame 冲突
                    {
                        readSuccess = _capture.Read(frame);
                    }

                    if (readSuccess && !frame.IsEmpty)
                    {
                        lock (_frameLock)
                        {
                            _currentFrame?.Dispose();
                            _currentFrame = frame.Clone();
                        }

                        FrameReceived?.Invoke(this, _currentFrame!);
                    }
                }

                Thread.Sleep(33); // 约30fps
            }

            frame.Dispose();
        }

        /// <summary>
        /// 获取一帧图像（同步方式）
        /// </summary>
        public Mat? GetFrame()
        {
            if (_capture == null || !_capture.IsOpened)
            {
                return null;
            }

            Mat frame = new Mat();
            bool readSuccess;
            lock (_readLock) // 加锁防止与 CaptureLoop 冲突
            {
                readSuccess = _capture.Read(frame);
            }

            if (readSuccess && !frame.IsEmpty)
            {
                return frame;
            }

            frame.Dispose();
            return null;
        }

        /// <summary>
        /// 批量采集并自动保存到文件夹
        /// </summary>
        /// <param name="saveFolder">保存文件夹路径</param>
        /// <param name="count">要采集的图像数量</param>
        /// <param name="fileNamePrefix">文件名前缀（默认为 "capture"）</param>
        /// <param name="delayMs">采集间隔（毫秒），0表示连续采集</param>
        /// <param name="onProgress">进度回调（参数为已保存数量）</param>
        /// <returns>成功保存的数量</returns>
        public int BatchCaptureAndSave(
            string saveFolder,
            int count,
            string fileNamePrefix = "capture",
            int delayMs = 0,
            Action<int>? onProgress = null,
            ImageStorageModule.ImageFormat format = ImageStorageModule.ImageFormat.PNG)
        {
            if (count <= 0) return 0;
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            int savedCount = 0;
            for (int i = 0; i < count; i++)
            {
                Mat? frame = GetFrame(); // 使用同步方式获取帧
                if (frame == null || frame.IsEmpty)
                {
                    // 采集失败，跳过此帧
                    continue;
                }

                // 生成文件名：前缀_时间戳_序号.ext
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string extension = format switch
                {
                    ImageStorageModule.ImageFormat.JPEG => ".jpg",
                    ImageStorageModule.ImageFormat.TIFF => ".tif",
                    ImageStorageModule.ImageFormat.RAW => ".raw",
                    _ => ".png"
                };
                string fileName = $"{fileNamePrefix}_{timestamp}_{i + 1:D4}{extension}";
                string filePath = Path.Combine(saveFolder, fileName);

                if (ImageStorageModule.SaveImage(frame, filePath, format))
                {
                    savedCount++;
                }

                frame.Dispose(); // 释放资源

                // 回调进度
                onProgress?.Invoke(savedCount);

                // 间隔延时
                if (delayMs > 0 && i < count - 1)
                    Thread.Sleep(delayMs);
            }

            return savedCount;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            StopCapture();

            lock (_frameLock)
            {
                _currentFrame?.Dispose();
                _currentFrame = null;
            }

            _capture?.Dispose();
            _capture = null;
            _cancellationTokenSource?.Dispose();
        }
    }
}