# MRC 自动测试软件 — 项目手册（总册）

| 项目 | 内容 |
|------|------|
| 软件名称 | MRC 自动测试软件（上位机：ImageCaptureApp） |
| 靶标类型 | GBA1 类 MRC 靶标（算法与映射表约定） |
| 文档类型 | 项目手册（涵盖工程范围、架构、模块、集成、配置、交付与运维要点） |
| 适用源码版本 | 仓库内 `ImageCaptureApp` 工程，目标框架 .NET 8.0（WPF） |
| 文档版本 | 1.0 |
| 发布日期 | 2026-05-08 |

---

## 修订记录

| 版本 | 日期 | 说明 |
|------|------|------|
| 1.0 | 2026-05-08 | 首版：整合上位机、MRC 算法集成、配置与交付结构，作为全项目总册 |

---

## 一、文档说明与阅读指引

### 1.1 编写目的

本手册从**工程项目整体**角度描述 MRC 自动测试软件：建设目标、功能边界、代码与目录结构、系统架构、核心模块职责、Python 算法集成方式、配置与数据流、输出物与汇总格式、已知限制及与其它文档的关系。适用于**项目负责人、开发与测试人员、运维与现场调试人员**在统一语义下理解本仓库。

### 1.2 与其它文档的分工

| 文档 | 定位 |
|------|------|
| `手册/MRC自动测试软件_项目手册.md`（本文） | **总册**：全项目重点一览 |
| `手册/项目手册.md` | **终端用户操作说明**（安装、界面、逐步操作、故障排除条目化） |
| `手册/MRC算法项目手册.md` | **算法维护手册**：`MRC_final.py` 流水线、公式与参数、与上位机副本差异 |
| `手册/软件设计文档.md` | 早期/通用上位机设计纲要（部分表述偏模板化，实现细节以源码与本文为准） |
| `手册/TeledyneDALSA_对接说明.md` | SaperaLT DLL、路径与对接补充说明 |
| `手册/VS调试运行说明.md` | Visual Studio 调试与运行注意事项 |

阅读顺序建议：**本文 →（操作层面）项目手册.md →（改算法）MRC算法项目手册.md →（对接采集）TeledyneDALSA_对接说明.md**。

### 1.3 术语与缩写

| 术语 | 含义 |
|------|------|
| 上位机 | 本仓库 Windows 桌面应用程序 `ImageCaptureApp` |
| SaperaLT | Teledyne DALSA 图像采集软件开发包/运行时 |
| CCF | 相机/采集配置文件（Sapera 常用文本配置） |
| MRC（本软件语境） | 针对 GBA1 靶标图像的最小可分辨对比度相关自动测试流水线，由 Python 脚本实现核心算法，上位机负责调度与展示 |
| EmguCV | OpenCV 的 .NET 封装，用于图像缓冲、读写与基础处理 |

---

## 二、项目概述

### 2.1 建设目标

在 Windows 平台上提供一套**可交付的图像采集与 MRC 自动分析一体化软件**：通过 **Teledyne DALSA SaperaLT** 从采集卡获取图像，实时预览与存储；对符合约定的靶标图像调用 **Python（OpenCV 等）** 完成几何摆正、矩形检测与映射、条纹剖面评价及最小可分辨组等指标汇总；支持**实时节流处理**与**文件夹批处理**，并生成结构化输出（标注图、Excel、曲线、JSON 摘要、批处理汇总 CSV）。

### 2.2 范围说明

**范围内**：SaperaLT 固定采集链路；单帧/批量存图；CCF 切换与配置持久化；灰度化显示、缩放平移；MRC 脚本调度与结果表格展示。

**范围外（当前主程序行为）**：不上线通用 DirectShow/USB 摄像头切换路径；采集源在下层仍保留 `ImageCaptureModule` 类供扩展，主界面逻辑固定为 `TeledyneDalsaSaperaLt`。

### 2.3 合规与依赖前提

软件正常运行依赖：**SaperaLT 及采集硬件**、**.NET 8 运行时**（发布时可自带）、**Python 3 可用（`python` 或 `py -3`）**、随程序发布的 **`Python/MRC_final.py` 与 `Python/MappingTable.xlsx`**。具体路径与排查见第七节及 `项目手册.md` 中的运维条目。

---

## 三、仓库与目录结构（重点）

```
（仓库根）
├── ImageCaptureApp/              # 上位机工程（C# WPF）
│   ├── MainWindow.xaml(.cs)      # 主界面与业务流程编排
│   ├── Config/
│   │   └── CaptureDeviceConfig.json
│   ├── Modules/
│   │   ├── ICaptureModule.cs
│   │   ├── TeledyneDalsaSaperaLtCaptureModule.cs   # 当前默认采集实现
│   │   ├── ImageCaptureModule.cs                   # 预留/其它采集实现
│   │   ├── ImageProcessingModule.cs
│   │   ├── ImageStorageModule.cs
│   │   └── MrcProcessingModule.cs                  # Python 调用与结果解析
│   ├── Controls/
│   │   └── ImageDisplayControl.xaml(.cs)
│   └── Python/
│       ├── MRC_final.py           # 与算法进度版同步的上位机副本（含 summary.json）
│       └── MappingTable.xlsx      # 10×10 映射表（构建时复制到输出目录）
├── mrc/MRC_progress/              # 算法研发目录（含进度版 MRC_final.py 等）
├── 手册/                          # 文档（本文档、用户手册、算法手册等）
├── Teledyne DALSA/               # 厂商 SDK 示例与头文件等（若存在于工作区）
└── mycamera.ccf                  # CCF 示例或默认搜索候选之一（依部署而定）
```

**要点**：发布包须包含与 `ImageCaptureApp.csproj` 一致的 **`Config`、`Python` 资源复制规则**（`CaptureDeviceConfig.json`、`MRC_final.py`、`MappingTable.xlsx` 均配置为 `CopyToOutputDirectory`）。

---

## 四、系统架构与技术栈

### 4.1 逻辑分层

1. **表现层（WPF）**：菜单、工具栏、参数卡片、双栏图像显示、MRC 结果表格、状态栏。  
2. **采集与显示编排**：`MainWindow` 订阅 `ICaptureModule` 帧事件，维护 `_originalImage` / `_currentDisplayImage`，驱动缩放与灰度切换。  
3. **采集实现层**：`TeledyneDalsaSaperaLtCaptureModule` 通过反射加载 `DALSA.SaperaLT.SapClassBasic.dll`，绑定 CCF，完成 Grab 与缓冲到 `Mat` 的转换。  
4. **图像处理与存储**：`ImageProcessingModule`（灰度等）、`ImageStorageModule`（格式枚举与保存）。  
5. **MRC 集成层**：`MrcProcessingModule` 启动进程调用 Python，解析退出码、`*_labels.png` 与可选 `*_summary.json`。  
6. **配置层**：`CaptureDeviceConfig` JSON 序列化，优先运行目录 `Config/CaptureDeviceConfig.json`，调试时可回退复制源码树配置。

### 4.2 技术栈摘要

| 类别 | 选型 |
|------|------|
| 框架 | .NET 8.0，WPF，可选 Windows Forms 互操作（`UseWindowsForms`） |
| 图像 | Emgu.CV 4.8.1（NuGet：`Emgu.CV`、`Emgu.CV.runtime.windows`） |
| 采集 | SaperaLT .NET（`DALSA.SaperaLT.SapClassBasic.dll`，反射加载） |
| MRC | Python 3 + OpenCV-Python、NumPy、openpyxl、matplotlib（由脚本侧依赖满足） |

---

## 五、功能规格总览

### 5.1 图像采集

- 固定使用 **SaperaLT** 采集链路；启动时强制 `DeviceSettings.CaptureSource = TeledyneDalsaSaperaLt`。  
- 初始化需有效 **CCF** 与 **Sapera .NET DLL**（路径见配置或自动探测）。  
- 支持 **开始/停止采集**、失败时弹窗与状态栏 **LastError** 摘要。  
- 实际分辨率以采集卡/缓冲为准；若小于配置分辨率，界面提示用户。

### 5.2 显示与交互

- 左侧实时预览；**Ctrl + 滚轮**缩放；**Shift + 左键**或**中键**平移；工具栏重置与缩放倍数显示。  
- **灰度化**切换影响当前用于保存/送 MRC 的图像语义（与界面逻辑一致）。  
- Sapera 模式下状态栏帧率可能为 **0**（当前实现未从驱动读取 FPS）。

### 5.3 存储

- **单帧保存**：快捷键与应用命令，格式由保存对话框决定。  
- **批量采集**：在已开始采集前提下，按设定张数与格式（PNG/JPG/TIF/RAW）写入指定目录，文件名带时间戳与序号。

### 5.4 CCF 管理

- 界面选择 `.ccf` 并应用：停止采集、释放模块、更新配置中的路径、重新 `InitializeCaptureModule`，必要时恢复采集。

### 5.5 MRC 分析

- **实时模式**：采集进行中开启「MRC处理」；两次触发间隔 **不少于约 1200 ms**（节流）；输出根目录优先为批量采集路径，否则回退至用户「图片」下的 `Captures`。每次运行在与输出根并列的逻辑下创建 `mrc_result\yyyyMMdd_HHmmss\`，写入临时 `input.png` 及脚本产出物。  
- **文件夹批处理**：对选定文件夹内单层图像逐个调用脚本；每图输出目录为 **`所选文件夹\mrc_result\<不含扩展名的文件名>\`**；全程结束后写入 **`所选文件夹\mrc_result\mrc_summary.csv`**。  
- **成功判定**：进程退出码为 0 且存在 **`{stem}_labels.png`**；否则解析 stdout/stderr 摘要提示用户；若日志命中「矩形不足 / 映射失败」等关键字，界面给出更明确的靶标相关提示。

---

## 六、软件模块说明（C#）

### 6.1 `ICaptureModule` / `TeledyneDalsaSaperaLtCaptureModule`

定义采集生命周期：`Initialize`、`StartCapture`、`StopCapture`、帧事件、`GetResolution`、`LastError`、`Dispose`。Sapera 实现负责 DLL 加载、CCF 解析、采集资源创建与图像拷贝到 `Mat`。

### 6.2 `ImageCaptureModule`

非 Sapera 采集路径的遗留/扩展实现；**当前主窗体创建工厂固定返回 Sapera 模块**。

### 6.3 `ImageProcessingModule`

提供灰度化、缩放、裁剪、亮度对比度等通用操作，供界面与保存链路使用。

### 6.4 `ImageStorageModule`

封装保存格式枚举与磁盘写入，与配置项 `StorageSettings.DefaultFormat` 及界面格式下拉联动。

### 6.5 `MrcProcessingModule`

- **脚本解析**：从 `AppContext.BaseDirectory\Python\MRC_final.py` 等多候选路径解析脚本。  
- **映射表**：优先用户传入路径，否则使用脚本同目录 `MappingTable.xlsx`，或该目录下首个 xlsx。  
- **调用参数**：`--input`、`--output`、`--mapping`（UTF-8 标准输出/错误重定向）。  
- **Python 解析顺序**：本地常见路径 `python.exe` 候选 → `python` → `py -3`；均不可用则等价 **9009** 类提示。  
- **结果填充**：`MrcProcessResult` 含输出目录、各附加文件路径、`min_resolvable_group_id` / `min_resolvable_c_mean`（自 `*_summary.json`）。  

### 6.6 `CaptureDeviceConfig`

加载优先级：**运行目录** `Config/CaptureDeviceConfig.json` →（调试）项目内配置复制到运行目录 → 默认新建并保存。字段语义与示例见 **`项目手册.md` 第七节**或源码 `DeviceSettings` 定义。

### 6.7 界面层 `MainWindow`

编排初始化、定时状态刷新、批量采集循环、MRC 实时 `Task.Run` 异步、结果表 `ObservableCollection` 更新及 CSV 汇总写出逻辑。

---

## 七、MRC 算法与 Python 集成

### 7.1 算法职责（摘要）

对输入 BGR 图像执行：**倾角估计与旋转摆正** →（可选）**四角定向消除 90° 歧义** → **色不变掩膜与矩形候选** → **100 矩形与 10×10 映射表匹配** → **每组 Rect1 条纹剖面与 C_mean** → **异常标记与最小可分辨组（上位机版 JSON）** → 写出标注图、Excel、曲线等。

完整步骤、公式与命令行参数见 **`手册/MRC算法项目手册.md`**。

### 7.2 双副本策略

| 路径 | 说明 |
|------|------|
| `mrc/MRC_progress/MRC_final.py` | 算法研发主副本，便于迭代 |
| `ImageCaptureApp/Python/MRC_final.py` | **上位机构建与发布所用副本**，含 `*_summary.json` 供 `MrcProcessingModule` 读取 |

二者应保持功能同步策略：**以发布目录脚本为准**；合并改动时需 regression 批处理与实时路径。

### 7.3 上位机与脚本契约

- 输入：上位机保证传入图像路径与输出目录可写。  
- 输出：必须生成 **`{stem}_labels.png`** 视为成功；`{stem}_summary.json` 为增强字段，缺失时表格中最小分辨组可为空。  
- JSON 键名（当前约定）：`min_resolvable_group_id`、`min_resolvable_c_mean`（与 `ReadSummaryInt` / `ReadSummaryDouble` 一致）。

---

## 八、配置与部署要点

### 8.1 构建与发布

- 使用 **Visual Studio 2022**，安装 **.NET 桌面开发**工作负载。  
- 发布输出需包含：`ImageCaptureApp.exe`、`Config\CaptureDeviceConfig.json`、`Python\` 下脚本与 xlsx、（可选）应用清单 `app.manifest`。

### 8.2 运行环境检查清单

1. SaperaLT 安装及服务正常；采集卡未被其它程序独占。  
2. `SaperaDotNetDllPath` 指向有效 `DALSA.SaperaLT.SapClassBasic.dll`，或 DLL 位于可探测路径。  
3. `SaperaCcfPath` 或通过界面应用有效 CCF。  
4. Python 可在命令行执行；批处理/实时前建议手工运行一次脚本验证依赖库。

---

## 九、数据流简述

**采集数据流**：硬件 → Sapera 缓冲 → `Mat` → UI 显示 / 保存 /（实时）克隆帧 → `MrcProcessingModule.ProcessCurrentFrameAsync`。

**批处理数据流**：磁盘图像路径列表 → `ProcessImageFileAsync` → `mrc_result\<stem>\` 多文件产出 → 聚合 `mrc_summary.csv`。

**配置数据流**：启动 `Load()` → 修正 `CaptureSource` → 初始化采集 → 用户修改格式/CCF 等 → `Save()` 写回运行目录 JSON。

---

## 十、输出物与汇总格式

### 10.1 单次 MRC 运行（脚本侧）

典型文件（stem 为输入文件名不含扩展名）：`*_a.*`、`*_labels.*`、`*_ov.*`、`*_corner_debug.*`（条件）、`*_res.xlsx`、`*_curve.png`、`*_summary.json`（上位机副本）。说明见算法手册第三节。

### 10.2 批处理汇总 CSV

路径：**`<图像文件夹>\mrc_result\mrc_summary.csv`**  

表头：

`image_name,success,min_resolvable_group_id,min_resolvable_c_mean,message,output_dir`

字段 `success`：`1` 成功，`0` 失败；含逗号或换行的字段按 CSV 引号规则转义。

---

## 十一、用户操作索引

详细逐步说明、快捷键与界面分区图式描述见 **`手册/项目手册.md`**。本总册仅列任务索引：

1. 启动与依赖检查  
2. 开始/停止采集与预览  
3. 缩放、平移、灰度化  
4. 单帧保存与批量采集  
5. CCF 选择与应用  
6. MRC 实时处理（注意节流与输出目录）  
7. MRC 文件夹批处理与结果表回看  

---

## 十二、开发与调试

- 调试前确认 **运行目录** 下是否已复制 `Config` 与 `Python`（首次 F5 若未复制，依 `CaptureDeviceConfig.Load` 回退逻辑可能从项目目录拉取配置）。  
- Sapera 相关异常优先查看 `TeledyneDalsaSaperaLtCaptureModule.LastError` 与弹窗文本。  
- Python 侧调试可在命令行直接调用 `MRC_final.py` 与上位机相同参数，对比退出码与生成文件。  
- 更细调试步骤见 **`手册/VS调试运行说明.md`**。

---

## 十三、已知限制与设计决策

1. **采集源固定为 SaperaLT**，简化现场配置与测试矩阵。  
2. **实时 MRC 1200 ms 节流**，避免 UI 线程阻塞与磁盘风暴。  
3. **帧率显示可能为 0**，不阻碍采集本身。  
4. **批处理不递归子目录**，避免误扫超大目录。  
5. **仓库根其它 README/快速开始** 若仍描述“默认可选 DirectShow”，属于历史文档；**以本文与 `项目手册.md` 为准**。

---

## 十四、故障排除（总览）

| 类别 | 典型现象 | 处理方向 |
|------|----------|----------|
| 采集初始化 | 找不到 DLL / CCF | 配置路径、安装 SaperaLT、核对 CCF 与硬件 |
| 采集运行 | 无图或断开 | 线缆、独占进程、CCF 与相机模式 |
| Python | 退出码 9009 | 安装 Python，PATH 或 `py -3` |
| MRC | 无 labels | 查看日志关键词；检查靶标与光照 |
| MRC | 缺 MappingTable | 恢复 `Python/MappingTable.xlsx` 发布 |
| 磁盘 | 批处理失败增多 | 权限、空间、杀毒软件拦截 |

条目化细则见 **`项目手册.md` 第九章**。

---

## 十五、附录：文档与源码索引

| 检索目标 | 位置 |
|----------|------|
| 主界面与 MRC 节流、CSV 汇总 | `ImageCaptureApp/MainWindow.xaml.cs` |
| Python 进程参数与路径解析 | `ImageCaptureApp/Modules/MrcProcessingModule.cs` |
| Sapera 采集实现 | `ImageCaptureApp/Modules/TeledyneDalsaSaperaLtCaptureModule.cs` |
| 配置加载与保存 | `ImageCaptureApp/Config/CaptureDeviceConfig.cs` |
| 资源复制 | `ImageCaptureApp/ImageCaptureApp.csproj` |
| 算法全流程 | `手册/MRC算法项目手册.md`、`ImageCaptureApp/Python/MRC_final.py` |

---

**文档结束**
