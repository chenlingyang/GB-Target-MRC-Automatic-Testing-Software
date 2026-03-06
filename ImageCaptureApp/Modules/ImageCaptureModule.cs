using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace ImageCaptureApp.Modules
{
    /// <summary>
    /// 图像采集模块 - 负责从图像采集卡获取图像数据
    /// </summary>
    public class ImageCaptureModule : IDisposable
    {
        private VideoCapture? _capture;
        private bool _isCapturing = false;
        private CancellationTokenSource? _cancellationTokenSource;
        private Mat? _currentFrame;
        private readonly object _frameLock = new object();
        private readonly object _readLock = new object(); // 新增：用于同步 _capture.Read 访问

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
        /// 初始化采集卡
        /// </summary>
        /// <param name="deviceIndex">设备索引，默认为0（第一个设备）</param>
        /// <returns>是否初始化成功</returns>
        public bool Initialize(int deviceIndex = 0)
        {
            try
            {
                _capture?.Dispose();
                _capture = new VideoCapture(deviceIndex, VideoCapture.API.DShow);

                if (!_capture.IsOpened)
                {
                    return false;
                }

                // 设置默认参数
                _capture.Set(CapProp.FrameWidth, 1280);
                _capture.Set(CapProp.FrameHeight, 720);
                _capture.Set(CapProp.Fps, 30);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"初始化采集卡失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 设置分辨率
        /// </summary>
        public void SetResolution(int width, int height)
        {
            if (_capture != null && _capture.IsOpened)
            {
                _capture.Set(CapProp.FrameWidth, width);
                _capture.Set(CapProp.FrameHeight, height);
            }
        }

        /// <summary>
        /// 设置帧率
        /// </summary>
        public void SetFrameRate(int fps)
        {
            if (_capture != null && _capture.IsOpened)
            {
                _capture.Set(CapProp.Fps, fps);
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
        public int BatchCaptureAndSave(string saveFolder, int count, string fileNamePrefix = "capture", int delayMs = 0, Action<int>? onProgress = null)
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

                // 生成文件名：前缀_时间戳_序号.png
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"{fileNamePrefix}_{timestamp}_{i + 1:D4}.png";
                string filePath = Path.Combine(saveFolder, fileName);

                // 调用存储模块保存（假设 ImageStorageModule 存在）
                if (ImageStorageModule.SaveImage(frame, filePath))
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