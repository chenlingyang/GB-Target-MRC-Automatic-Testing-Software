# Teledyne DALSA 采集卡对接说明（SaperaLT）

本项目已在界面中加入 **采集源选择框**：
- 默认采集卡（DirectShow / OpenCV `VideoCapture`）
- Teledyne DALSA（SaperaLT）

选择会写入配置文件：`ImageCaptureApp/Config/CaptureDeviceConfig.json` 的 `DeviceSettings.CaptureSource`。

---

## 你现在还差的关键文件

Teledyne DALSA 的 C# 示例工程引用了：
- `DALSA.SaperaLT.SapClassBasic.dll`

但你当前工作区的 `Teledyne DALSA` 资料文件夹内 **未包含任何 .dll**（只有示例源码/头文件/说明），因此：
- 本项目 **可以编译运行**
- 但 **Teledyne DALSA 采集源目前只做了“环境检测 + 友好提示 + 自动回退”**
- 还不能真正抓帧

要真正抓帧，需要你在本机安装/提供 **SaperaLT .NET 组件**（通常来自 SaperaLT 安装包）。

---

## 快速定位 DLL（含 C++ 的也能找出来）

项目根目录下有一个脚本 **`查找Sapera的DLL.ps1`**，会在本机搜索 Sapera/DALSA 相关文件：

- **.NET 程序集（.dll）**：C# 可直接引用，用于本项目采集源。
- **本地 DLL（C++ 用）**：脚本也会列出来；若只有 C++ SDK，C# 可通过 P/Invoke 或 C++/CLI 封装调用，我也可以按找到的 DLL 帮你设计接口。
- **.lib 文件**：C++ 链接库，脚本一并列出，便于确认 SDK 是否安装完整。

**用法**：在项目根目录 `vs软件` 下，右键 `查找Sapera的DLL.ps1` → “使用 PowerShell 运行”；或在 PowerShell 中执行：

```powershell
cd "F:\研究生\项目\vs软件"
.\查找Sapera的DLL.ps1
```

把脚本输出的 **.NET 程序集** 路径填到下面配置里；若只有 **C++ DLL**，把输出贴给我，我可以按 C++ API 做 P/Invoke 或封装方案。

---

## 第一步：准备 SaperaLT .NET DLL

请在你机器上找到（或安装后得到）：
- `DALSA.SaperaLT.SapClassBasic.dll`

通常位置在 SaperaLT 安装目录的类似路径（示例工程的 HintPath）：
- `Sapera\Components\NET\Bin\DALSA.SaperaLT.SapClassBasic.dll`

如果你找到了该 DLL，把它的**完整路径**填到配置文件里。

---

## 第二步：配置项目使用 DALSA 采集源

编辑运行目录的配置文件（优先改运行目录的那份）：
- `ImageCaptureApp/bin/Debug/net8.0-windows/Config/CaptureDeviceConfig.json`

关键字段：

```json
{
  "DeviceSettings": {
    "CaptureSource": "TeledyneDalsaSaperaLt",
    "SaperaDotNetDllPath": "D:\\Sapera\\Components\\NET\\Bin\\DALSA.SaperaLT.SapClassBasic.dll",
    "SaperaCcfPath": ""
  }
}
```

说明：
- **CaptureSource**：切换采集源
- **SaperaDotNetDllPath**：指向 `DALSA.SaperaLT.SapClassBasic.dll` 的完整路径
- **SaperaCcfPath**：CameraLink 采集常用的 `.ccf` 配置文件（如果你的采集链路需要）

---

## 第三步：运行验证

1. 用 VS 按 **F5** 启动
2. 在工具栏下拉框选择 **Teledyne DALSA（SaperaLT）**

如果 DLL 配置不正确，程序会弹出明确提示并自动回退到默认采集源。

---

## 接下来我会做什么（拿到 DLL 后）

当你提供/安装好 SaperaLT .NET 组件后，我会把真实抓帧逻辑接入 `TeledyneDalsaSaperaLtCaptureModule`，实现：
- 枚举服务器/相机（SapManager / SapAcqDevice）
- 建立采集链（SapAcquisition + SapBuffer + SapAcqToBuf）
- EndOfFrame 回调取帧并转成 `Emgu.CV.Mat`
- 接入现有显示/灰度/缩放/保存/批量采集流程

参考资料（你目录内已有源码）：
- `Teledyne DALSA/Sapera/Examples/NET/GrabConsole/CSharp/GrabConsole.cs`
- `Teledyne DALSA/Sapera/Examples/NET/GrabCameraLink/CSharp/GrabCameraLink.cs`

---

## 你需要发我什么信息（我才能把抓帧做完整）

请告诉我（或贴截图/文件名）：
- 采集卡型号（比如 Xtium-CL / Xcelera-CL 等）
- 相机接口（CameraLink / GigE Vision / USB3 Vision）
- 是否有 `.ccf` 配置文件（如果有，路径和文件名）
- SaperaLT 是否已安装（能否运行 Sapera 自带 Demo）

