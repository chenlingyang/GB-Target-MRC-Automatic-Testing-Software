图像采集卡上位机 - 便携发布版

使用方法：
1. 将整个 PublishOutput 文件夹复制到目标电脑任意位置
2. 双击 ImageCaptureApp.exe 运行
3. 无需安装 .NET / Python / 配置环境变量

目录说明：
- ImageCaptureApp.exe    主程序
- Runtime\Python\        内置 Python（MRC 算法用）
- Python\                MRC 脚本与映射表
- Config\                采集卡配置
- Teledyne DALSA\        Sapera .NET 组件（采集卡接口）
- mycamera.ccf           相机配置文件

相机相关：
- 发布包已内置 Sapera .NET 组件（DALSA.SaperaLT.SapClassBasic.dll）
- 目标电脑仍需安装 Teledyne DALSA 采集卡驱动 / Sapera LT 运行时（硬件通信）

系统要求：
- Windows 10/11 64 位
- 建议安装 Microsoft Visual C++ 2015-2022 运行库（x64）

重新打包（开发机）：
- 双击项目根目录 build-release.bat
- 或运行 ImageCaptureApp\publish.bat
