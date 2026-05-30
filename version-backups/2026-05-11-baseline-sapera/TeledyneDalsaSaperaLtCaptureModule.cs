using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using ImageCaptureApp.Config;

namespace ImageCaptureApp.Modules
{
    /// <summary>
    /// Teledyne DALSA 采集模块（SaperaLT）。
    ///
    /// 设计说明：
    /// - 使用反射加载 DALSA.SaperaLT.SapClassBasic.dll，避免项目在缺少 SDK 的机器上编译失败。
    /// - 初始化采用 Sapera C# 示例中的对象链：SapLocation -> SapAcquisition -> SapBuffer -> SapAcqToBuf。
    /// - 抓帧方式：采集中周期性将 SapBuffer 导出为 BMP，再由 OpenCV 读取为 Mat 发送到 UI。
    ///   该方式优先保证可用性（后续可升级为直接内存拷贝以提升性能）。
    /// </summary>
    public sealed class TeledyneDalsaSaperaLtCaptureModule : ICaptureModule
    {
        public event EventHandler<Mat>? FrameReceived;
        public event EventHandler<bool>? CaptureStateChanged;

        public bool IsCapturing { get; private set; }
        public string? LastError { get; private set; }

        private Assembly? _sapAsm;
        private object? _sapLocation;
        private object? _sapAcquisition;
        private object? _sapBuffer;
        private object? _sapTransfer;
        private CancellationTokenSource? _captureCts;
        private string _snapshotPath = string.Empty;
        private string _ccfPath = string.Empty;
        private int _width;
        private int _height;

        public bool Initialize(DeviceSettings settings)
        {
            LastError = null;
            IsCapturing = false;
            _width = settings.Resolution.Width;
            _height = settings.Resolution.Height;

            string dllPath = ResolveSaperaDllPath(settings.SaperaDotNetDllPath);
            if (string.IsNullOrWhiteSpace(dllPath) || !File.Exists(dllPath))
            {
                LastError = "未找到 SaperaLT .NET 组件 DALSA.SaperaLT.SapClassBasic.dll";
                return false;
            }

            _ccfPath = ResolveCcfPath(settings.SaperaCcfPath);
            if (string.IsNullOrWhiteSpace(_ccfPath) || !File.Exists(_ccfPath))
            {
                LastError = "未找到 CCF 文件。请在配置中设置 SaperaCcfPath，或把 mycamera.ccf 放在项目根目录。";
                return false;
            }

            try
            {
                _sapAsm = Assembly.LoadFrom(dllPath);
                string ns = "DALSA.SaperaLT.SapClassBasic";

                Type sapManagerType = RequireType(ns + ".SapManager");
                Type sapLocationType = RequireType(ns + ".SapLocation");
                Type sapAcquisitionType = RequireType(ns + ".SapAcquisition");
                Type sapBufferType = RequireType(ns + ".SapBuffer");
                Type sapAcqToBufType = RequireType(ns + ".SapAcqToBuf");

                object resourceTypeAcq = ParseNestedEnum(sapManagerType, "ResourceType", "Acq");
                string serverName = ResolveServerName(sapManagerType, _ccfPath, resourceTypeAcq) ?? string.Empty;
                if (string.IsNullOrWhiteSpace(serverName))
                {
                    LastError = "未找到可用 Sapera 服务器（采集卡）。请确认驱动和采集卡状态。";
                    return false;
                }

                int acqCount = Convert.ToInt32(
                    sapManagerType.GetMethod("GetResourceCount", new[] { typeof(string), resourceTypeAcq.GetType() })!
                        .Invoke(null, new[] { serverName, resourceTypeAcq }),
                    CultureInfo.InvariantCulture);
                if (acqCount <= 0)
                {
                    LastError = $"服务器 {serverName} 下未发现采集资源（Acq）。";
                    return false;
                }

                int resourceIndex = ResolveAcquisitionResourceIndex(
                    sapManagerType,
                    resourceTypeAcq,
                    serverName,
                    acqCount,
                    _ccfPath);

                bool built = TryBuildSaperaObjects(
                    sapLocationType,
                    sapAcquisitionType,
                    sapBufferType,
                    sapAcqToBufType,
                    serverName,
                    resourceIndex);
                if (!built)
                {
                    for (int tryIdx = 0; tryIdx < acqCount; tryIdx++)
                    {
                        if (tryIdx == resourceIndex)
                        {
                            continue;
                        }

                        CleanupSaperaObjects();
                        built = TryBuildSaperaObjects(
                            sapLocationType,
                            sapAcquisitionType,
                            sapBufferType,
                            sapAcqToBufType,
                            serverName,
                            tryIdx);
                        if (built)
                        {
                            resourceIndex = tryIdx;
                            break;
                        }
                    }
                }

                if (!built)
                {
                    string acqList = FormatAcqResourceList(sapManagerType, resourceTypeAcq, serverName, acqCount);
                    LastError =
                        $"Sapera 初始化失败，请检查 CCF 对应服务器与设备索引。Server={serverName}, Index={resourceIndex}" +
                        (string.IsNullOrEmpty(acqList) ? "" : $" 本机 Acq: {acqList}");
                    return false;
                }

                object memTypeScatter = ParseNestedEnum(sapBufferType, "MemoryType", "ScatterGather");
                _sapBuffer ??= Activator.CreateInstance(sapBufferType, 1, _sapAcquisition, memTypeScatter);
                _sapTransfer ??= Activator.CreateInstance(sapAcqToBufType, _sapAcquisition, _sapBuffer);

                if (_sapAcquisition == null || _sapBuffer == null || _sapTransfer == null)
                {
                    LastError = "Sapera 对象实例化失败（Acquisition/Buffer/Transfer）。";
                    return false;
                }

                if (!InvokeBool(_sapAcquisition, "Create") ||
                    !InvokeBool(_sapBuffer, "Create") ||
                    !InvokeBool(_sapTransfer, "Create"))
                {
                    LastError = "Sapera 对象创建失败（Acquisition/Buffer/Transfer）。";
                    return false;
                }

                _width = ReadIntProperty(_sapBuffer, "Width", _width);
                _height = ReadIntProperty(_sapBuffer, "Height", _height);
                _snapshotPath = Path.Combine(Path.GetTempPath(), "image_captureapp_sapera_snapshot.bmp");
                return true;
            }
            catch (Exception ex)
            {
                LastError = "Sapera 初始化失败: " + ex.Message;
                CleanupSaperaObjects();
                return false;
            }
        }

        private bool TryBuildSaperaObjects(
            Type sapLocationType,
            Type sapAcquisitionType,
            Type sapBufferType,
            Type sapAcqToBufType,
            string serverName,
            int resourceIndex)
        {
            try
            {
                _sapLocation = Activator.CreateInstance(sapLocationType, serverName, resourceIndex);
                _sapAcquisition = Activator.CreateInstance(sapAcquisitionType, _sapLocation, _ccfPath);
                object memTypeScatter = ParseNestedEnum(sapBufferType, "MemoryType", "ScatterGather");
                _sapBuffer = Activator.CreateInstance(sapBufferType, 1, _sapAcquisition, memTypeScatter);
                _sapTransfer = Activator.CreateInstance(sapAcqToBufType, _sapAcquisition, _sapBuffer);
                return _sapAcquisition != null && _sapBuffer != null && _sapTransfer != null;
            }
            catch
            {
                return false;
            }
        }

        public bool StartCapture()
        {
            if (_sapTransfer == null || _sapBuffer == null)
            {
                LastError = "Sapera 尚未初始化成功。";
                CaptureStateChanged?.Invoke(this, false);
                return false;
            }

            try
            {
                if (!InvokeBool(_sapTransfer, "Grab"))
                {
                    LastError = "Sapera Grab 启动失败。";
                    CaptureStateChanged?.Invoke(this, false);
                    return false;
                }

                IsCapturing = true;
                _captureCts = new CancellationTokenSource();
                Task.Run(() => SnapshotLoop(_captureCts.Token));
                CaptureStateChanged?.Invoke(this, true);
                return true;
            }
            catch (Exception ex)
            {
                LastError = "启动 Sapera 采集失败: " + ex.Message;
                CaptureStateChanged?.Invoke(this, false);
                return false;
            }
        }

        public void StopCapture()
        {
            IsCapturing = false;
            try
            {
                _captureCts?.Cancel();
            }
            catch { }

            if (_sapTransfer != null)
            {
                try
                {
                    InvokeBool(_sapTransfer, "Freeze");
                    InvokeBool(_sapTransfer, "Wait", 2000);
                }
                catch
                {
                    try { InvokeBool(_sapTransfer, "Abort"); } catch { }
                }
            }

            CaptureStateChanged?.Invoke(this, false);
        }

        public void SetResolution(int width, int height)
        {
            // Sapera 模式下分辨率由 CCF 决定，这里仅更新显示用值
            _width = Math.Max(1, width);
            _height = Math.Max(1, height);
        }

        public void SetFrameRate(int fps)
        {
            // Sapera 模式下帧率通常由相机/触发配置决定，当前版本不强制改写
        }

        public (int width, int height) GetResolution() => (_width, _height);
        public double GetFrameRate() => 0;

        public int BatchCaptureAndSave(
            string saveFolder,
            int count,
            string fileNamePrefix = "capture",
            int delayMs = 0,
            Action<int>? onProgress = null,
            ImageStorageModule.ImageFormat format = ImageStorageModule.ImageFormat.PNG)
        {
            if (!IsCapturing || count <= 0 || _sapBuffer == null)
            {
                return 0;
            }

            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }

            int saved = 0;
            for (int i = 0; i < count; i++)
            {
                string snapBmp = Path.Combine(saveFolder, $"{fileNamePrefix}_{DateTime.Now:yyyyMMdd_HHmmss}_{i + 1:D4}_tmp.bmp");
                if (TrySaveCurrentBuffer(snapBmp))
                {
                    try
                    {
                        using Mat frame = CvInvoke.Imread(snapBmp, ImreadModes.AnyColor);
                        if (!frame.IsEmpty)
                        {
                            string extension = format switch
                            {
                                ImageStorageModule.ImageFormat.JPEG => ".jpg",
                                ImageStorageModule.ImageFormat.TIFF => ".tif",
                                ImageStorageModule.ImageFormat.RAW => ".raw",
                                _ => ".png"
                            };
                            string outPath = Path.Combine(saveFolder, $"{fileNamePrefix}_{DateTime.Now:yyyyMMdd_HHmmss}_{i + 1:D4}{extension}");
                            if (ImageStorageModule.SaveImage(frame, outPath, format))
                            {
                                saved++;
                                onProgress?.Invoke(saved);
                            }
                        }
                    }
                    finally
                    {
                        try { if (File.Exists(snapBmp)) File.Delete(snapBmp); } catch { }
                    }
                }

                if (delayMs > 0 && i < count - 1)
                {
                    Thread.Sleep(delayMs);
                }
            }
            return saved;
        }

        public void Dispose()
        {
            StopCapture();
            CleanupSaperaObjects();
        }

        private void SnapshotLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested && IsCapturing)
            {
                try
                {
                    if (TrySaveCurrentBuffer(_snapshotPath))
                    {
                        using Mat frame = CvInvoke.Imread(_snapshotPath, ImreadModes.AnyColor);
                        if (!frame.IsEmpty)
                        {
                            _width = frame.Width;
                            _height = frame.Height;
                            FrameReceived?.Invoke(this, frame.Clone());
                        }
                    }
                }
                catch (Exception ex)
                {
                    LastError = "Sapera 采集循环异常: " + ex.Message;
                }

                Thread.Sleep(30);
            }
        }

        private bool TrySaveCurrentBuffer(string filePath)
        {
            if (_sapBuffer == null)
            {
                return false;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? ".");

            // 尝试常见 Save 重载
            // 1) Save(string filePath, string options)
            if (InvokeBool(_sapBuffer, "Save", filePath, "-format bmp"))
            {
                return File.Exists(filePath);
            }

            // 2) Save(string filePath)
            if (InvokeBool(_sapBuffer, "Save", filePath))
            {
                return File.Exists(filePath);
            }

            // 3) 兜底为 tif
            string tifPath = Path.ChangeExtension(filePath, ".tif");
            if (InvokeBool(_sapBuffer, "Save", tifPath, "-format tif"))
            {
                if (File.Exists(tifPath))
                {
                    File.Copy(tifPath, filePath, true);
                    return true;
                }
            }

            return false;
        }

        private static string ResolveSaperaDllPath(string configuredPath)
        {
            if (!string.IsNullOrWhiteSpace(configuredPath) && File.Exists(configuredPath))
            {
                return configuredPath;
            }

            string root = AppDomain.CurrentDomain.BaseDirectory;
            string[] guesses =
            {
                Path.Combine(root, "Teledyne DALSA", "Sapera", "Components", "NET", "Bin", "DALSA.SaperaLT.SapClassBasic.dll"),
                Path.Combine(root, "..", "..", "..", "..", "Teledyne DALSA", "Sapera", "Components", "NET", "Bin", "DALSA.SaperaLT.SapClassBasic.dll"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Teledyne DALSA", "Sapera", "Components", "NET", "Bin", "DALSA.SaperaLT.SapClassBasic.dll"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Teledyne DALSA", "Sapera", "Components", "NET", "Bin", "DALSA.SaperaLT.SapClassBasic.dll"),
            };

            return guesses.FirstOrDefault(File.Exists) ?? string.Empty;
        }

        private static string ResolveCcfPath(string configuredPath)
        {
            if (!string.IsNullOrWhiteSpace(configuredPath) && File.Exists(configuredPath))
            {
                return configuredPath;
            }

            string root = AppDomain.CurrentDomain.BaseDirectory;
            string[] guesses =
            {
                Path.Combine(root, "mycamera.ccf"),
                Path.Combine(root, "..", "..", "..", "..", "mycamera.ccf"),
                Path.Combine(root, "..", "..", "..", "..", "Teledyne DALSA", "Sapera", "CamFiles", "User", "mycamera.ccf"),
            };

            return guesses.FirstOrDefault(File.Exists) ?? string.Empty;
        }

        private static string? ResolveServerName(Type sapManagerType, string ccfPath, object resourceTypeAcq)
        {
            string? fromCcf = ReadCcfValue(ccfPath, "Board", "Server Name");
            MethodInfo? getResourceCount = sapManagerType.GetMethod("GetResourceCount", new[] { typeof(string), resourceTypeAcq.GetType() });
            MethodInfo? getServerCount = sapManagerType.GetMethod("GetServerCount", Type.EmptyTypes);
            MethodInfo? getServerName = sapManagerType.GetMethod("GetServerName", new[] { typeof(int) });
            if (getResourceCount == null || getServerCount == null || getServerName == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(fromCcf))
            {
                int ccfAcqCount = Convert.ToInt32(
                    getResourceCount.Invoke(null, new[] { fromCcf, resourceTypeAcq }),
                    CultureInfo.InvariantCulture);
                if (ccfAcqCount > 0)
                {
                    return fromCcf;
                }
            }

            int count = Convert.ToInt32(getServerCount.Invoke(null, null), CultureInfo.InvariantCulture);
            if (count <= 0)
            {
                return null;
            }

            // 与 Sapera 示例一致：GetServerName / GetServerCount 使用 0-based 服务器索引
            for (int i = 0; i < count; i++)
            {
                string? name = getServerName.Invoke(null, new object[] { i }) as string;
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                int acqCount = Convert.ToInt32(
                    getResourceCount.Invoke(null, new[] { name, resourceTypeAcq }),
                    CultureInfo.InvariantCulture);
                if (acqCount > 0)
                {
                    return name;
                }
            }

            return null;
        }

        private static int ResolveResourceIndex(string ccfPath)
        {
            string? idx = ReadCcfValue(ccfPath, "Board", "Device Index");
            if (int.TryParse(idx, out int value))
            {
                return value;
            }
            return 0;
        }

        /// <summary>
        /// 优先用 CCF 中 [Board] Device Name 与 SapManager.GetResourceName 对齐（与 CamExpert 一致），
        /// 避免 Device Index 在本机 Acq 数量变化或枚举顺序不同时越界。
        /// </summary>
        private static int ResolveAcquisitionResourceIndex(
            Type sapManagerType,
            object resourceTypeAcq,
            string serverName,
            int acqCount,
            string ccfPath)
        {
            string? deviceNameFromCcf = ReadCcfValue(ccfPath, "Board", "Device Name");
            MethodInfo? getResourceName = sapManagerType.GetMethod(
                "GetResourceName",
                new[] { typeof(string), resourceTypeAcq.GetType(), typeof(int) });

            if (!string.IsNullOrWhiteSpace(deviceNameFromCcf) && getResourceName != null)
            {
                string target = deviceNameFromCcf.Trim();
                for (int i = 0; i < acqCount; i++)
                {
                    try
                    {
                        object? nm = getResourceName.Invoke(null, new[] { serverName, resourceTypeAcq, i });
                        if (nm is string rn && string.Equals(rn.Trim(), target, StringComparison.OrdinalIgnoreCase))
                        {
                            return i;
                        }
                    }
                    catch
                    {
                        // 忽略单个索引查询异常
                    }
                }
            }

            int fromFile = ResolveResourceIndex(ccfPath);
            if (fromFile < 0 || fromFile >= acqCount)
            {
                return 0;
            }

            return fromFile;
        }

        private static string FormatAcqResourceList(
            Type sapManagerType,
            object resourceTypeAcq,
            string serverName,
            int acqCount)
        {
            MethodInfo? getResourceName = sapManagerType.GetMethod(
                "GetResourceName",
                new[] { typeof(string), resourceTypeAcq.GetType(), typeof(int) });
            if (getResourceName == null)
            {
                return string.Empty;
            }

            string sep = "";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < acqCount; i++)
            {
                try
                {
                    object? nm = getResourceName.Invoke(null, new[] { serverName, resourceTypeAcq, i });
                    sb.Append(sep).Append('[').Append(i.ToString(CultureInfo.InvariantCulture)).Append("]=").Append(nm);
                    sep = "; ";
                }
                catch
                {
                    sb.Append(sep).Append('[').Append(i.ToString(CultureInfo.InvariantCulture)).Append("]=?");
                    sep = "; ";
                }
            }

            return sb.ToString();
        }

        private static string? ReadCcfValue(string ccfPath, string section, string key)
        {
            if (!File.Exists(ccfPath))
            {
                return null;
            }

            string current = string.Empty;
            foreach (var raw in File.ReadLines(ccfPath))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line.StartsWith(";", StringComparison.Ordinal))
                {
                    continue;
                }

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    current = line.Substring(1, line.Length - 2).Trim();
                    continue;
                }

                if (!string.Equals(current, section, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                int eq = line.IndexOf('=');
                if (eq <= 0)
                {
                    continue;
                }

                string k = line.Substring(0, eq).Trim();
                if (string.Equals(k, key, StringComparison.OrdinalIgnoreCase))
                {
                    return line.Substring(eq + 1).Trim();
                }
            }
            return null;
        }

        private Type RequireType(string fullName)
        {
            Type? t = _sapAsm?.GetType(fullName);
            if (t == null)
            {
                throw new InvalidOperationException("类型不存在: " + fullName);
            }
            return t;
        }

        private static object ParseNestedEnum(Type ownerType, string nestedEnumName, string enumValue)
        {
            Type? enumType = ownerType.GetNestedType(nestedEnumName, BindingFlags.Public);
            if (enumType == null)
            {
                throw new InvalidOperationException($"枚举类型不存在: {ownerType.FullName}+{nestedEnumName}");
            }
            return Enum.Parse(enumType, enumValue);
        }

        private static bool InvokeBool(object target, string methodName, params object[] args)
        {
            MethodInfo[] methods = target.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => string.Equals(m.Name, methodName, StringComparison.Ordinal))
                .ToArray();

            foreach (var method in methods)
            {
                var ps = method.GetParameters();
                if (ps.Length != args.Length)
                {
                    continue;
                }

                try
                {
                    object? ret = method.Invoke(target, args);
                    if (ret is bool b)
                    {
                        return b;
                    }
                }
                catch
                {
                    // 尝试下一个重载
                }
            }

            return false;
        }

        private static int ReadIntProperty(object target, string propertyName, int fallback)
        {
            try
            {
                PropertyInfo? p = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (p?.GetValue(target) is object v)
                {
                    return Convert.ToInt32(v, CultureInfo.InvariantCulture);
                }
            }
            catch { }
            return fallback;
        }

        private void CleanupSaperaObjects()
        {
            DestroyObject(_sapTransfer);
            DestroyObject(_sapBuffer);
            DestroyObject(_sapAcquisition);
            _sapTransfer = null;
            _sapBuffer = null;
            _sapAcquisition = null;
            _sapLocation = null;
            _sapAsm = null;
        }

        private static void DestroyObject(object? obj)
        {
            if (obj == null)
            {
                return;
            }

            try
            {
                InvokeBool(obj, "Destroy");
            }
            catch { }

            try
            {
                if (obj is IDisposable d)
                {
                    d.Dispose();
                }
            }
            catch { }
        }
    }
}

