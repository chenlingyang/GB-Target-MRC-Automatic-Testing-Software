using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ImageCaptureApp.Modules
{
    public sealed class MrcProcessResult
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public string OutputDirectory { get; init; } = string.Empty;
        public string? LabeledImagePath { get; init; }
        public string? OverviewImagePath { get; init; }
        public string? ExcelPath { get; init; }
        public string? CurvePath { get; init; }
        public string? SummaryJsonPath { get; init; }
        public int? MinResolvableGroupId { get; init; }
        public double? MinResolvableCMean { get; init; }
    }

    /// <summary>
    /// MRC 算法执行模块：保存当前图像并调用 Python 脚本处理。
    /// </summary>
    public static class MrcProcessingModule
    {
        private static readonly string[] SupportedImageExtensions =
        {
            ".bmp", ".jpg", ".jpeg", ".png", ".tif", ".tiff"
        };

        public static async Task<MrcProcessResult> ProcessCurrentFrameAsync(
            Mat sourceImage,
            string? mappingPath,
            string outputRootDirectory)
        {
            if (sourceImage == null || sourceImage.IsEmpty)
            {
                return new MrcProcessResult { Success = false, Message = "当前没有可处理图像。" };
            }

            string? scriptPath = ResolveMrcScriptPath();
            if (string.IsNullOrWhiteSpace(scriptPath) || !File.Exists(scriptPath))
            {
                return new MrcProcessResult
                {
                    Success = false,
                    Message = "未找到 MRC 脚本，请确认 Python/MRC_final.py 已存在。"
                };
            }

            string? validMappingPath = ResolveMappingPath(mappingPath, scriptPath);
            if (string.IsNullOrWhiteSpace(validMappingPath) || !File.Exists(validMappingPath))
            {
                return new MrcProcessResult
                {
                    Success = false,
                    Message = "未找到映射表（xlsx）。请确认 Python/MappingTable.xlsx 存在。"
                };
            }

            string runFolder = Path.Combine(
                outputRootDirectory,
                "mrc_result",
                DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            Directory.CreateDirectory(runFolder);

            string inputPath = Path.Combine(runFolder, "input.png");
            sourceImage.Save(inputPath);

            return await ProcessImageFileAsync(inputPath, mappingPath, runFolder);
        }

        public static async Task<MrcProcessResult> ProcessImageFileAsync(
            string imagePath,
            string? mappingPath,
            string outputDirectory)
        {
            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
            {
                return new MrcProcessResult { Success = false, Message = "输入图像文件不存在。" };
            }

            Directory.CreateDirectory(outputDirectory);

            string stdOut;
            string stdErr;
            int exitCode;
            string? scriptPath = ResolveMrcScriptPath();
            if (string.IsNullOrWhiteSpace(scriptPath) || !File.Exists(scriptPath))
            {
                return new MrcProcessResult
                {
                    Success = false,
                    Message = "未找到 MRC 脚本，请确认 Python/MRC_final.py 已存在。"
                };
            }

            string? validMappingPath = ResolveMappingPath(mappingPath, scriptPath);
            if (string.IsNullOrWhiteSpace(validMappingPath) || !File.Exists(validMappingPath))
            {
                return new MrcProcessResult
                {
                    Success = false,
                    Message = "未找到映射表（xlsx）。请确认 Python/MappingTable.xlsx 存在。"
                };
            }

            try
            {
                (exitCode, stdOut, stdErr) = await RunPythonAsync(scriptPath, imagePath, outputDirectory, validMappingPath);
            }
            catch (Exception ex)
            {
                return new MrcProcessResult
                {
                    Success = false,
                    Message = $"调用 Python 失败：{ex.Message}",
                    OutputDirectory = outputDirectory
                };
            }

            if (exitCode != 0)
            {
                string err = BuildScriptFailureMessage(stdOut, stdErr);
                if (exitCode == 9009)
                {
                    err = BuildPythonMissingMessage();
                }

                return new MrcProcessResult
                {
                    Success = false,
                    Message = $"MRC 处理失败（退出码 {exitCode}）：{err}".Trim(),
                    OutputDirectory = outputDirectory
                };
            }

            string stem = Path.GetFileNameWithoutExtension(imagePath);
            string labeledPath = Path.Combine(outputDirectory, $"{stem}_labels.png");
            string overviewPath = Path.Combine(outputDirectory, $"{stem}_ov.png");
            string excelPath = Path.Combine(outputDirectory, $"{stem}_res.xlsx");
            string curvePath = Path.Combine(outputDirectory, $"{stem}_curve.png");
            string summaryJsonPath = Path.Combine(outputDirectory, $"{stem}_summary.json");

            bool hasLabeledImage = File.Exists(labeledPath);
            if (!hasLabeledImage)
            {
                string detail = BuildScriptFailureMessage(stdOut, stdErr);
                string message = IsLikelyNoRectangleFailure(stdOut, stdErr)
                    ? "未找到合适的图像（未检测到足够的矩形目标），请更换或调整图像后重试。"
                    : $"MRC 处理失败：脚本未生成标注结果图。{detail}";

                return new MrcProcessResult
                {
                    Success = false,
                    Message = message,
                    OutputDirectory = outputDirectory
                };
            }

            return new MrcProcessResult
            {
                Success = true,
                Message = "MRC 处理完成。",
                OutputDirectory = outputDirectory,
                LabeledImagePath = labeledPath,
                OverviewImagePath = File.Exists(overviewPath) ? overviewPath : null,
                ExcelPath = File.Exists(excelPath) ? excelPath : null,
                CurvePath = File.Exists(curvePath) ? curvePath : null,
                SummaryJsonPath = File.Exists(summaryJsonPath) ? summaryJsonPath : null,
                MinResolvableGroupId = ReadSummaryInt(summaryJsonPath, "min_resolvable_group_id"),
                MinResolvableCMean = ReadSummaryDouble(summaryJsonPath, "min_resolvable_c_mean")
            };
        }

        public static string[] CollectImageFiles(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                return Array.Empty<string>();
            }

            return Directory.EnumerateFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(p => SupportedImageExtensions.Contains(Path.GetExtension(p), StringComparer.OrdinalIgnoreCase))
                .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public sealed class BatchDistributionPlotResult
        {
            public bool Success { get; init; }
            public string Message { get; init; } = string.Empty;
            public string? PlotPath { get; init; }
            public int? BatchMinResolvableGroupId { get; init; }
        }

        public static async Task<BatchDistributionPlotResult> GenerateBatchDistributionPlotAsync(
            IEnumerable<int> groupIds,
            string outputDirectory,
            string plotFileName = "min_group_distribution.png")
        {
            int[] ids = groupIds?.Where(id => id > 0).ToArray() ?? Array.Empty<int>();
            if (ids.Length == 0)
            {
                return new BatchDistributionPlotResult
                {
                    Success = false,
                    Message = "没有有效的最小可分辨组号，无法生成分布图。"
                };
            }

            string? scriptPath = ResolveMrcScriptPath();
            if (string.IsNullOrWhiteSpace(scriptPath) || !File.Exists(scriptPath))
            {
                return new BatchDistributionPlotResult
                {
                    Success = false,
                    Message = "未找到 MRC 脚本，无法生成分布图。"
                };
            }

            Directory.CreateDirectory(outputDirectory);
            string plotPath = Path.Combine(outputDirectory, plotFileName);
            string groupIdsArg = string.Join(",", ids);

            try
            {
                (int exitCode, string stdOut, string stdErr) = await RunPythonBatchDistributionAsync(
                    scriptPath,
                    outputDirectory,
                    plotPath,
                    groupIdsArg);

                if (exitCode != 0)
                {
                    string err = BuildScriptFailureMessage(stdOut, stdErr);
                    return new BatchDistributionPlotResult
                    {
                        Success = false,
                        Message = $"生成分布图失败（退出码 {exitCode}）：{err}".Trim()
                    };
                }

                if (!File.Exists(plotPath))
                {
                    return new BatchDistributionPlotResult
                    {
                        Success = false,
                        Message = "Python 未输出分布图文件。"
                    };
                }

                int? batchGroup = TryReadBatchGroupFromStdout(stdOut);
                return new BatchDistributionPlotResult
                {
                    Success = true,
                    Message = "分布图已生成。",
                    PlotPath = plotPath,
                    BatchMinResolvableGroupId = batchGroup ?? MrcBatchSummaryHelper.PickBatchMinResolvableGroup(
                        ids.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count()))
                };
            }
            catch (Exception ex)
            {
                return new BatchDistributionPlotResult
                {
                    Success = false,
                    Message = $"调用 Python 生成分布图失败：{ex.Message}"
                };
            }
        }

        private static int? TryReadBatchGroupFromStdout(string stdOut)
        {
            if (string.IsNullOrWhiteSpace(stdOut))
            {
                return null;
            }

            try
            {
                string? jsonLine = stdOut
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Reverse()
                    .FirstOrDefault(line => line.TrimStart().StartsWith("{", StringComparison.Ordinal));

                if (string.IsNullOrWhiteSpace(jsonLine))
                {
                    return null;
                }

                using JsonDocument doc = JsonDocument.Parse(jsonLine);
                if (doc.RootElement.TryGetProperty("batch_min_resolvable_group_id", out JsonElement e)
                    && e.ValueKind == JsonValueKind.Number)
                {
                    return e.GetInt32();
                }
            }
            catch
            {
                // ignore parse errors
            }

            return null;
        }

        private static async Task<(int ExitCode, string StdOut, string StdErr)> RunPythonBatchDistributionAsync(
            string scriptPath,
            string outputDirectory,
            string plotPath,
            string groupIdsArg)
        {
            foreach (string pythonExe in EnumeratePythonExecutableCandidates())
            {
                (int code, string o, string e)? result = await TryRunPythonBatchDistributionCommandAsync(
                    pythonExe, null, scriptPath, outputDirectory, plotPath, groupIdsArg);
                if (result.HasValue && result.Value.code != 9009)
                {
                    return (result.Value.code, result.Value.o, result.Value.e);
                }
            }

            return (9009, string.Empty, BuildPythonMissingMessage());
        }

        private static async Task<(int code, string o, string e)?> TryRunPythonBatchDistributionCommandAsync(
            string fileName,
            string? prefixArg,
            string scriptPath,
            string outputDirectory,
            string plotPath,
            string groupIdsArg)
        {
            try
            {
                using Process process = new Process();
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                if (!string.IsNullOrWhiteSpace(prefixArg))
                {
                    process.StartInfo.ArgumentList.Add(prefixArg);
                }
                else if (string.Equals(fileName, "py", StringComparison.OrdinalIgnoreCase))
                {
                    process.StartInfo.ArgumentList.Add("-3");
                }

                ConfigurePythonProcessEnvironment(process.StartInfo, fileName);

                process.StartInfo.ArgumentList.Add(scriptPath);
                process.StartInfo.ArgumentList.Add("--batch-distribution");
                process.StartInfo.ArgumentList.Add("--output");
                process.StartInfo.ArgumentList.Add(outputDirectory);
                process.StartInfo.ArgumentList.Add("--plot-output");
                process.StartInfo.ArgumentList.Add(plotPath);
                process.StartInfo.ArgumentList.Add("--group-ids");
                process.StartInfo.ArgumentList.Add(groupIdsArg);

                process.Start();
                Task<string> readOut = process.StandardOutput.ReadToEndAsync();
                Task<string> readErr = process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                return (process.ExitCode, await readOut, await readErr);
            }
            catch
            {
                return null;
            }
        }

        private static string? ResolveMrcScriptPath()
        {
            string appBase = AppContext.BaseDirectory;
            string[] candidates =
            {
                Path.Combine(appBase, "Python", "MRC_final.py"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Python", "MRC_final.py"),
                Path.GetFullPath(Path.Combine(appBase, "..", "..", "..", "..", "Python", "MRC_final.py"))
            };

            return candidates.FirstOrDefault(File.Exists);
        }

        private static string? ResolveMappingPath(string? providedPath, string scriptPath)
        {
            if (!string.IsNullOrWhiteSpace(providedPath) && File.Exists(providedPath))
            {
                return providedPath;
            }

            string scriptDir = Path.GetDirectoryName(scriptPath) ?? string.Empty;
            if (!Directory.Exists(scriptDir))
            {
                return null;
            }

            string bundledPath = Path.Combine(scriptDir, "MappingTable.xlsx");
            if (File.Exists(bundledPath))
            {
                return bundledPath;
            }

            return Directory
                .GetFiles(scriptDir, "*.xlsx", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();
        }

        private static string BuildScriptFailureMessage(string stdOut, string stdErr)
        {
            string merged = string.Join(
                Environment.NewLine,
                new[] { stdErr?.Trim(), stdOut?.Trim() }.Where(s => !string.IsNullOrWhiteSpace(s)));

            if (string.IsNullOrWhiteSpace(merged))
            {
                return "未收到 Python 输出日志。";
            }

            string oneLine = merged.Replace("\r", " ").Replace("\n", " ").Trim();
            if (oneLine.Length > 260)
            {
                oneLine = oneLine[..260] + "...";
            }
            return $"日志：{oneLine}";
        }

        private static bool IsLikelyNoRectangleFailure(string stdOut, string stdErr)
        {
            string text = $"{stdErr}\n{stdOut}";
            string[] keywords =
            {
                "可用于编号的矩形不足",
                "候选矩形不足",
                "映射编号唯一匹配失败",
                "无法完成四角波峰波谷定向",
                "定向用映射匹配不足"
            };
            return keywords.Any(k => text.Contains(k, StringComparison.OrdinalIgnoreCase));
        }

        private static async Task<(int ExitCode, string StdOut, string StdErr)> RunPythonAsync(
            string scriptPath,
            string inputPath,
            string outputPath,
            string mappingPath)
        {
            foreach (string pythonExe in EnumeratePythonExecutableCandidates())
            {
                (int code, string o, string e)? result = await TryRunPythonCommandAsync(
                    pythonExe, null, scriptPath, inputPath, outputPath, mappingPath);
                if (result.HasValue && result.Value.code != 9009)
                {
                    return (result.Value.code, result.Value.o, result.Value.e);
                }
            }

            return (9009, string.Empty, BuildPythonMissingMessage());
        }

        private static IEnumerable<string> EnumeratePythonExecutableCandidates()
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            void TryAdd(string? path, List<string> bucket)
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    return;
                }

                string candidate = Path.IsPathRooted(path) ? Path.GetFullPath(path) : path;
                if (!seen.Add(candidate))
                {
                    return;
                }

                if (!Path.IsPathRooted(candidate) || File.Exists(candidate))
                {
                    bucket.Add(candidate);
                }
            }

            var ordered = new List<string>();
            string appBase = AppContext.BaseDirectory;
            TryAdd(Path.Combine(appBase, "Runtime", "Python", "python.exe"), ordered);
            TryAdd(Path.Combine(appBase, "Python", "python.exe"), ordered);

            string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            TryAdd(Path.Combine(local, "Programs", "Python", "Python312", "python.exe"), ordered);
            TryAdd(Path.Combine(local, "Programs", "Python", "Python311", "python.exe"), ordered);
            TryAdd(Path.Combine(local, "Programs", "Python", "Python310", "python.exe"), ordered);
            TryAdd("python", ordered);
            TryAdd("py", ordered);

            return ordered;
        }

        private static string BuildPythonMissingMessage()
        {
            string bundled = Path.Combine(AppContext.BaseDirectory, "Runtime", "Python", "python.exe");
            return "Python 启动失败：未找到可用的 Python 运行时。"
                + $" 请确认发布包内存在 {bundled}，或在本机安装 Python 3 并加入 PATH。";
        }

        private static void ConfigurePythonProcessEnvironment(ProcessStartInfo startInfo, string pythonExecutable)
        {
            if (!Path.IsPathRooted(pythonExecutable) || !File.Exists(pythonExecutable))
            {
                return;
            }

            string pythonHome = Path.GetDirectoryName(pythonExecutable) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(pythonHome))
            {
                return;
            }

            startInfo.Environment["PYTHONHOME"] = pythonHome;
            startInfo.Environment["PYTHONUTF8"] = "1";
            string scriptsDir = Path.Combine(pythonHome, "Scripts");
            string pathValue = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            startInfo.Environment["PATH"] = $"{pythonHome};{scriptsDir};{pathValue}";
        }

        private static int? ReadSummaryInt(string path, string key)
        {
            if (!File.Exists(path)) return null;
            try
            {
                using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(path, Encoding.UTF8));
                if (doc.RootElement.TryGetProperty(key, out JsonElement e) && e.ValueKind == JsonValueKind.Number)
                {
                    return e.GetInt32();
                }
            }
            catch
            {
                // ignore summary parsing errors
            }
            return null;
        }

        private static double? ReadSummaryDouble(string path, string key)
        {
            if (!File.Exists(path)) return null;
            try
            {
                using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(path, Encoding.UTF8));
                if (doc.RootElement.TryGetProperty(key, out JsonElement e) && e.ValueKind == JsonValueKind.Number)
                {
                    return e.GetDouble();
                }
            }
            catch
            {
                // ignore summary parsing errors
            }
            return null;
        }

        private static async Task<(int code, string o, string e)?> TryRunPythonCommandAsync(
            string fileName,
            string? prefixArg,
            string scriptPath,
            string inputPath,
            string outputPath,
            string mappingPath)
        {
            try
            {
                using Process process = new Process();
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                if (!string.IsNullOrWhiteSpace(prefixArg))
                {
                    process.StartInfo.ArgumentList.Add(prefixArg);
                }
                else if (string.Equals(fileName, "py", StringComparison.OrdinalIgnoreCase))
                {
                    process.StartInfo.ArgumentList.Add("-3");
                }

                ConfigurePythonProcessEnvironment(process.StartInfo, fileName);

                process.StartInfo.ArgumentList.Add(scriptPath);
                process.StartInfo.ArgumentList.Add("--input");
                process.StartInfo.ArgumentList.Add(inputPath);
                process.StartInfo.ArgumentList.Add("--output");
                process.StartInfo.ArgumentList.Add(outputPath);
                process.StartInfo.ArgumentList.Add("--mapping");
                process.StartInfo.ArgumentList.Add(mappingPath);

                process.Start();
                Task<string> readOut = process.StandardOutput.ReadToEndAsync();
                Task<string> readErr = process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                return (process.ExitCode, await readOut, await readErr);
            }
            catch
            {
                return null;
            }
        }
    }
}
