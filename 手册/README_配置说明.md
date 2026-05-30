# 图像采集卡配置快速指南

## 快速更换采集卡

### 方法一：修改配置文件（最简单）

1. **找到配置文件**
   - 编译后：`bin\Debug\net8.0-windows\Config\CaptureDeviceConfig.json`
   - 源代码：`ImageCaptureApp\Config\CaptureDeviceConfig.json`

2. **打开配置文件**，修改以下内容：

```json
{
  "DeviceSettings": {
    "DeviceIndex": 0,        // ← 修改这里：0=第一个设备，1=第二个，2=第三个...
    "DeviceName": "我的采集卡", // ← 修改这里：设备名称（仅用于显示）
    "Resolution": {
      "Width": 640,          // ← 修改这里：图像宽度
      "Height": 480          // ← 修改这里：图像高度
    },
    "FrameRate": 30         // ← 修改这里：帧率
  }
}
```

3. **保存文件并重新运行程序**

### 方法二：通过代码修改

**文件**: `MainWindow.xaml.cs`

**找到这个方法**（约第31行）:
```csharp
private void InitializeCaptureModule()
{
    ...
    int deviceIndex = _config.DeviceSettings.DeviceIndex;  // ← 修改这里
    ...
}
```

**直接修改**:
```csharp
int deviceIndex = 1;  // 改为你的设备索引
```

---

## 常见采集卡配置示例

### USB摄像头（通常索引为0）
```json
{
  "DeviceSettings": {
    "DeviceIndex": 0,
    "DeviceName": "USB摄像头",
    "Resolution": { "Width": 640, "Height": 480 },
    "FrameRate": 30
  }
}
```

### PCI采集卡（可能需要索引1或2）
```json
{
  "DeviceSettings": {
    "DeviceIndex": 1,
    "DeviceName": "PCI采集卡",
    "Resolution": { "Width": 1920, "Height": 1080 },
    "FrameRate": 30
  }
}
```

### 高分辨率采集卡
```json
{
  "DeviceSettings": {
    "DeviceIndex": 0,
    "DeviceName": "4K采集卡",
    "Resolution": { "Width": 3840, "Height": 2160 },
    "FrameRate": 15
  }
}
```

---

## 如何确定设备索引

如果不知道你的采集卡是哪个索引，可以：

1. **逐个尝试**: 修改 `DeviceIndex` 为 0, 1, 2, 3... 直到找到正确的
2. **查看设备管理器**: 
   - 按 Win+X → 设备管理器
   - 查看"图像设备"或"声音、视频和游戏控制器"
   - 通常第一个设备索引为0，第二个为1，以此类推

---

## 分辨率设置建议

### 常见分辨率
- **640x480**: 标准VGA，适合预览
- **1280x720**: HD，平衡质量和性能
- **1920x1080**: Full HD，高质量
- **3840x2160**: 4K，需要高性能

### 注意事项
- 分辨率越高，处理速度越慢
- 确保采集卡支持你设置的分辨率
- 如果卡顿，降低分辨率或帧率

---

## 其他配置项说明

### AutoStart（自动启动采集）
```json
"AutoStart": true  // true=程序启动时自动开始采集，false=需要手动点击"开始采集"
```

### DefaultFormat（默认保存格式）
```json
"StorageSettings": {
  "DefaultFormat": "PNG"  // PNG, JPEG, BMP, TIFF
}
```

### DefaultZoom（默认缩放）
```json
"DisplaySettings": {
  "DefaultZoom": 1.0  // 1.0=原始大小，2.0=放大2倍，0.5=缩小一半
}
```

---

## 故障排除

### 问题：修改配置后没有生效
- **解决**: 确保修改的是运行目录下的配置文件（`bin\Debug\net8.0-windows\Config\`）
- **或者**: 重新编译项目

### 问题：找不到配置文件
- **解决**: 首次运行程序会自动创建默认配置文件
- **位置**: 程序运行目录下的 `Config` 文件夹

### 问题：配置格式错误
- **解决**: 确保JSON格式正确，注意逗号和引号
- **验证**: 使用在线JSON验证工具检查格式

---

## 配置文件完整示例

```json
{
  "DeviceSettings": {
    "DeviceIndex": 0,
    "DeviceName": "我的采集卡",
    "API": "DirectShow",
    "Resolution": {
      "Width": 1920,
      "Height": 1080
    },
    "FrameRate": 30,
    "AutoStart": false
  },
  "DisplaySettings": {
    "DefaultZoom": 1.0,
    "MinZoom": 0.1,
    "MaxZoom": 10.0
  },
  "StorageSettings": {
    "DefaultFormat": "PNG",
    "DefaultSavePath": "",
    "AutoSave": false
  }
}
```

---

**提示**: 修改配置后，重启程序即可生效！
