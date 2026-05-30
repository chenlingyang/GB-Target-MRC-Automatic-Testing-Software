本目录用于保存可手工回退的源码快照。

如何回退到某一快照（例如 2026-05-11-baseline-sapera）：
1. 关闭 Visual Studio / 停止运行中的程序。
2. 将快照文件夹内对应文件覆盖回项目：
   ImageCaptureApp\Modules\TeledyneDalsaSaperaLtCaptureModule.cs
   ImageCaptureApp\MainWindow.xaml.cs（若快照中有）
   ImageCaptureApp\App.xaml.cs（若快照中有）
   ImageCaptureApp\Config\CaptureDeviceConfig.cs（若快照中有）
3. 重新编译解决方案。

若已使用 Git，也可用 git checkout / git stash 管理版本；本目录作为无 Git 时的兜底。
