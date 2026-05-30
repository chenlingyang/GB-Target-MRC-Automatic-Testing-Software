# Visual Studio 调试运行说明

## 如何通过F5调试运行

### 步骤1：打开项目
1. 双击 `ImageCaptureApp.sln` 文件
2. 等待 Visual Studio 打开项目
3. 等待 NuGet 包自动还原完成（首次可能需要几分钟）

### 步骤2：设置启动项目
- 如果解决方案中有多个项目，右键点击 `ImageCaptureApp` 项目
- 选择 **"设为启动项目"**
- 或者直接在解决方案资源管理器中右键项目 → **"设为启动项目"**

### 步骤3：运行项目
- **按 F5**：开始调试运行（带断点调试）
- **按 Ctrl+F5**：开始运行（不调试，更快）
- **点击工具栏绿色播放按钮**：开始调试运行

### 步骤4：调试功能
- **设置断点**：在代码行号左侧点击，出现红点
- **查看变量**：鼠标悬停在变量上，或使用"局部变量"窗口
- **单步调试**：
  - F10：逐过程（Step Over）
  - F11：逐语句（Step Into）
  - Shift+F11：跳出（Step Out）
- **停止调试**：Shift+F5 或点击停止按钮

---

## 项目配置说明

### 启动对象
项目已配置 `StartupObject` 为 `ImageCaptureApp.App`，这是 WPF 应用程序的标准入口点。

### 启动URI
在 `App.xaml` 中配置了 `StartupUri="MainWindow.xaml"`，程序启动时会自动打开主窗口。

### 输出类型
项目类型为 `WinExe`（Windows可执行文件），可以直接运行。

---

## 常见问题

### Q: 按F5没有反应
**解决方案**:
1. 确保项目已设置为启动项目（项目名称应为粗体）
2. 检查是否有编译错误（查看"错误列表"窗口）
3. 尝试重新生成解决方案：**生成** → **重新生成解决方案**

### Q: 提示"无法启动程序"
**解决方案**:
1. 检查输出目录：`bin\Debug\net8.0-windows\ImageCaptureApp.exe` 是否存在
2. 尝试清理并重新生成：**生成** → **清理解决方案**，然后**生成解决方案**

### Q: 程序启动但立即关闭
**解决方案**:
1. 使用 Ctrl+F5 运行（不调试），查看错误信息
2. 检查"输出"窗口的错误信息
3. 确保已连接图像采集设备（如果AutoStart=true）

### Q: 找不到配置文件
**解决方案**:
1. 首次运行会自动创建配置文件
2. 配置文件位置：`bin\Debug\net8.0-windows\Config\CaptureDeviceConfig.json`
3. 如果不存在，手动创建 `Config` 文件夹并复制配置文件

---

## 调试技巧

### 1. 查看输出信息
- **视图** → **输出** → 选择"调试"或"生成"
- 可以看到程序的调试输出和错误信息

### 2. 使用断点
```csharp
// 在关键位置设置断点
private void InitializeCaptureModule()
{
    _captureModule = new ImageCaptureModule();  // ← 在这里设置断点
    // ...
}
```

### 3. 条件断点
- 右键断点 → **条件**
- 设置条件，例如：`deviceIndex == 1`

### 4. 监视窗口
- **调试** → **窗口** → **监视** → **监视1**
- 添加要监视的变量或表达式

### 5. 调用堆栈
- **调试** → **窗口** → **调用堆栈**
- 查看函数调用顺序

---

## 项目启动流程

```
1. Visual Studio 按F5
   ↓
2. 编译项目（如果代码有变化）
   ↓
3. 启动 ImageCaptureApp.exe
   ↓
4. App.xaml.cs 的 App 类初始化
   ↓
5. 加载 MainWindow.xaml
   ↓
6. MainWindow.xaml.cs 构造函数执行
   ↓
7. 加载配置文件
   ↓
8. 初始化采集模块
   ↓
9. 显示主窗口
   ↓
10. 如果 AutoStart=true，自动开始采集
```

---

## 性能优化调试

### 检查内存使用
- **调试** → **窗口** → **诊断工具**
- 查看内存和CPU使用情况

### 检查图像处理性能
在代码中添加时间测量：
```csharp
var stopwatch = System.Diagnostics.Stopwatch.StartNew();
// ... 处理代码 ...
stopwatch.Stop();
System.Diagnostics.Debug.WriteLine($"处理时间: {stopwatch.ElapsedMilliseconds}ms");
```

---

## 配置文件调试

### 查看加载的配置
在 `MainWindow.xaml.cs` 的 `InitializeCaptureModule()` 方法中添加：
```csharp
System.Diagnostics.Debug.WriteLine($"设备索引: {_config.DeviceSettings.DeviceIndex}");
System.Diagnostics.Debug.WriteLine($"分辨率: {_config.DeviceSettings.Resolution.Width}x{_config.DeviceSettings.Resolution.Height}");
```

### 验证配置文件格式
如果配置文件格式错误，程序会使用默认配置。检查输出窗口是否有错误信息。

---

**提示**: 首次运行建议使用 Ctrl+F5（不调试），这样可以看到所有错误信息！
