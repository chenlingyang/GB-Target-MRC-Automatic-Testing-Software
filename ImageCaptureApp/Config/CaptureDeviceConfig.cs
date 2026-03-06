using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ImageCaptureApp.Config
{
    /// <summary>
    /// 采集设备配置类
    /// </summary>
    public class CaptureDeviceConfig
    {
        private static readonly string ConfigFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, 
            "Config", 
            "CaptureDeviceConfig.json");

        /// <summary>
        /// 设备设置
        /// </summary>
        [JsonPropertyName("DeviceSettings")]
        public DeviceSettings DeviceSettings { get; set; } = new DeviceSettings();

        /// <summary>
        /// 显示设置
        /// </summary>
        [JsonPropertyName("DisplaySettings")]
        public DisplaySettings DisplaySettings { get; set; } = new DisplaySettings();

        /// <summary>
        /// 存储设置
        /// </summary>
        [JsonPropertyName("StorageSettings")]
        public StorageSettings StorageSettings { get; set; } = new StorageSettings();

        /// <summary>
        /// 加载配置
        /// </summary>
        public static CaptureDeviceConfig Load()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    var config = JsonSerializer.Deserialize<CaptureDeviceConfig>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        ReadCommentHandling = JsonCommentHandling.Skip
                    });
                    return config ?? new CaptureDeviceConfig();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载配置失败: {ex.Message}");
            }

            // 如果加载失败，返回默认配置并保存
            var defaultConfig = new CaptureDeviceConfig();
            defaultConfig.Save();
            return defaultConfig;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public void Save()
        {
            try
            {
                string directory = Path.GetDirectoryName(ConfigFilePath) ?? "";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存配置失败: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 设备设置
    /// </summary>
    public class DeviceSettings
    {
        /// <summary>
        /// 设备索引（0表示第一个设备）
        /// </summary>
        [JsonPropertyName("DeviceIndex")]
        public int DeviceIndex { get; set; } = 0;

        /// <summary>
        /// 设备名称（用于标识，不影响功能）
        /// </summary>
        [JsonPropertyName("DeviceName")]
        public string DeviceName { get; set; } = "默认采集卡";

        /// <summary>
        /// API类型：DirectShow, MSMF, V4L2等
        /// </summary>
        [JsonPropertyName("API")]
        public string API { get; set; } = "DirectShow";

        /// <summary>
        /// 分辨率设置
        /// </summary>
        [JsonPropertyName("Resolution")]
        public Resolution Resolution { get; set; } = new Resolution();

        /// <summary>
        /// 帧率
        /// </summary>
        [JsonPropertyName("FrameRate")]
        public int FrameRate { get; set; } = 30;

        /// <summary>
        /// 是否自动启动采集
        /// </summary>
        [JsonPropertyName("AutoStart")]
        public bool AutoStart { get; set; } = false;
    }

    /// <summary>
    /// 分辨率设置
    /// </summary>
    public class Resolution
    {
        [JsonPropertyName("Width")]
        public int Width { get; set; } = 1280;

        [JsonPropertyName("Height")]
        public int Height { get; set; } = 720;
    }

    /// <summary>
    /// 显示设置
    /// </summary>
    public class DisplaySettings
    {
        [JsonPropertyName("DefaultZoom")]
        public double DefaultZoom { get; set; } = 1.0;

        [JsonPropertyName("MinZoom")]
        public double MinZoom { get; set; } = 0.1;

        [JsonPropertyName("MaxZoom")]
        public double MaxZoom { get; set; } = 10.0;
    }

    /// <summary>
    /// 存储设置
    /// </summary>
    public class StorageSettings
    {
        /// <summary>
        /// 默认格式：PNG, JPEG, BMP, TIFF
        /// </summary>
        [JsonPropertyName("DefaultFormat")]
        public string DefaultFormat { get; set; } = "PNG";

        /// <summary>
        /// 默认保存路径（空表示使用系统默认）
        /// </summary>
        [JsonPropertyName("DefaultSavePath")]
        public string DefaultSavePath { get; set; } = "";

        /// <summary>
        /// 是否自动保存
        /// </summary>
        [JsonPropertyName("AutoSave")]
        public bool AutoSave { get; set; } = false;
    }
}
