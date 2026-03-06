# 图像采集卡上位机软件

基于 C# WPF 和 EmguCV 开发的图像采集卡上位机软件，支持实时图像采集、显示、处理和存储。

## 功能特性

- ✅ **图像采集**: 支持 DirectShow 接口的图像采集卡
- ✅ **图像显示**: 实时显示采集的图像
- ✅ **图像缩放**: 支持放大、缩小，鼠标滚轮缩放
- ✅ **图像平移**: 支持鼠标拖拽平移
- ✅ **图像灰度化**: 一键转换为灰度图像
- ✅ **图像保存**: 支持保存为多种格式（PNG、JPEG、BMP、TIFF）

## 系统要求

- Windows 10/11
- .NET 6.0 或更高版本
- Visual Studio 2022（开发环境）
- 图像采集卡设备（支持 DirectShow）

## 安装依赖

### 方法一：使用 NuGet（推荐）

项目已配置 NuGet 包引用，在 Visual Studio 中打开项目后会自动还原依赖：

- Emgu.CV (4.8.1.5350)
- Emgu.CV.runtime.windows (4.8.1.5350)

### 方法二：手动安装

如果自动还原失败，可以在 Visual Studio 的 NuGet 包管理器中手动安装：

```
Install-Package Emgu.CV -Version 4.8.1.5350
Install-Package Emgu.CV.runtime.windows -Version 4.8.1.5350
```

## 编译和运行

1. 使用 Visual Studio 2022 打开 `ImageCaptureApp.sln`
2. 还原 NuGet 包（如果未自动还原）
3. 按 F5 或点击"启动"按钮运行项目

## 使用说明

### 基本操作

1. **开始采集**: 点击工具栏的"开始采集"按钮
2. **停止采集**: 点击工具栏的"停止采集"按钮
3. **保存图像**: 点击"保存"按钮，选择保存路径和格式
4. **灰度化**: 点击"灰度化"按钮切换彩色/灰度模式
5. **缩放图像**: 
   - 使用工具栏的"放大"/"缩小"按钮
   - 或使用 Ctrl + 鼠标滚轮
6. **平移图像**: 
   - Shift + 左键拖拽
   - 或中键拖拽

### 快捷键

- `Ctrl + S`: 保存图像
- `Ctrl + +`: 放大
- `Ctrl + -`: 缩小
- `Ctrl + 0`: 重置视图

## 项目结构

```
ImageCaptureApp/
├── Modules/                    # 核心模块
│   ├── ImageCaptureModule.cs   # 图像采集模块
│   ├── ImageProcessingModule.cs # 图像处理模块
│   └── ImageStorageModule.cs   # 图像存储模块
├── Controls/                    # 自定义控件
│   ├── ImageDisplayControl.xaml # 图像显示控件
│   └── ImageDisplayControl.xaml.cs
├── MainWindow.xaml              # 主窗口
├── MainWindow.xaml.cs           # 主窗口逻辑
├── App.xaml                     # 应用程序入口
└── App.xaml.cs
```

## 支持的图像采集卡

软件使用 DirectShow 接口，支持所有符合 DirectShow 标准的图像采集设备，包括：

- USB 摄像头
- PCI/PCIe 图像采集卡（需安装 DirectShow 驱动）
- 其他支持 DirectShow 的视频设备

## 常见问题

### Q: 提示"未检测到采集卡设备"
A: 请确保：
1. 图像采集卡已正确连接
2. 已安装采集卡的驱动程序
3. 设备在 Windows 设备管理器中显示正常

### Q: 图像显示卡顿
A: 可以尝试：
1. 降低采集分辨率
2. 降低帧率
3. 关闭其他占用资源的程序

### Q: 保存图像失败
A: 请检查：
1. 保存路径是否有写入权限
2. 磁盘空间是否充足
3. 文件是否被其他程序占用

## 开发说明

### 扩展功能

如需添加新的图像处理功能，可以在 `ImageProcessingModule.cs` 中添加新的静态方法。

### 修改采集参数

在 `ImageCaptureModule.cs` 的 `Initialize` 方法中修改默认分辨率、帧率等参数。

## 许可证

本项目仅供学习和研究使用。

## 更新日志

### v1.0 (2026-02-17)
- 初始版本发布
- 实现图像采集功能
- 实现图像缩放和平移
- 实现图像灰度化
- 实现图像保存功能
