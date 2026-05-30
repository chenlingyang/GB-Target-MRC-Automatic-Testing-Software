using System;
using Emgu.CV;
using ImageCaptureApp.Config;

namespace ImageCaptureApp.Modules
{
    public interface ICaptureModule : IDisposable
    {
        event EventHandler<Mat>? FrameReceived;
        event EventHandler<bool>? CaptureStateChanged;

        bool IsCapturing { get; }

        /// <summary>
        /// 初始化采集模块（从配置初始化）。
        /// 返回 false 表示初始化失败，可读取 LastError。
        /// </summary>
        bool Initialize(DeviceSettings settings);

        bool StartCapture();
        void StopCapture();

        void SetResolution(int width, int height);
        void SetFrameRate(int fps);

        (int width, int height) GetResolution();
        double GetFrameRate();

        /// <summary>
        /// 批量采集并保存（同步方法，内部可回调进度）。
        /// 返回成功保存的张数。
        /// </summary>
        int BatchCaptureAndSave(
            string saveFolder,
            int count,
            string fileNamePrefix = "capture",
            int delayMs = 0,
            Action<int>? onProgress = null,
            ImageStorageModule.ImageFormat format = ImageStorageModule.ImageFormat.PNG);

        /// <summary>
        /// 初始化/运行失败时的错误信息（可空）。
        /// </summary>
        string? LastError { get; }
    }
}

