using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
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
            string outputRootDirectory,
            string pipeline = "550")
        {
            if (sourceImage == null || sourceImage.IsEmpty)
            {
                return new MrcProcessResult { Success = false, Message = "当前没有可处理图像。" };
            }

            string? scriptPath = ResolveMrcScriptPath(pipeline);
            if (string.IsNullOrWhiteSpace(scriptPath) || !File.Exists(scriptPath))
            {
                return new MrcProcessResult
                {
                    Success = false,
                    Message = $"未找到 MRC 脚本，请确认 Python/{PipelineToScript(pipeline)} 已存在。"
                };
            }

            // 550 光管不需要映射表
            if (pipeline != "550")
            {
                string? validMappingPath = ResolveMappingPath(mappingPath, scriptPath);
                if (string.IsNullOrWhiteSpace(validMappingPath) || !File.Exists(validMappingPath))
                {
                    return new MrcProcessResult
                    {
                        Success = false,
                        Message = "未找到映射表（xlsx）。请确认 Python/MappingTable.xlsx 存在。"
                    };
                }
            }

            string runFolder = Path.Combine(
                outputRootDirectory,
                "mrc_result",
                DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            Directory.CreateDirectory(runFolder);

            string inputPath = Path.Combine(runFolder, "input.png");
            sourceImage.Save(inputPath);

            return await ProcessImageFileAsync(inputPath, mappingPath, runFolder, pipeline);
        }

        public static async Task<MrcProcessResult> ProcessImageFileAsync(
            string imagePath,
            string? mappingPath,
            string outputDirectory,
            string pipeline = "1m6")
        {
            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
            {
                return new MrcProcessResult { Success = false, Message = "输入图像文件不存在。" };
            }

            Directory.CreateDirectory(outputDirectory);

            string stdOut;
            string stdErr;
            int exitCode;
            string? scriptPath = ResolveMrcScriptPath(pipeline);
            if (string.IsNullOrWhiteSpace(scriptPath) || !File.Exists(scriptPath))
            {
                return new MrcProcessResult
                {
                    Success = false,
                    Message = $"未找到 MRC 脚本，请确认 Python/{PipelineToScript(pipeline)} 已存在。"
                };
            }

            string? mappingArg = null;
            if (pipeline != "550")
            {
                mappingArg = ResolveMappingPath(mappingPath, scriptPath);
                if (string.IsNullOrWhiteSpace(mappingArg) || !File.Exists(mappingArg))
                {
                    return new MrcProcessResult
                    {
                        Success = false,
                        Message = "未找到映射表（xlsx）。请确认 Python/MappingTable.xlsx 存在。"
                    };
                }
            }

            try
            {
                (exitCode, stdOut, stdErr) = await RunPythonAsync(scriptPath, imagePath, outputDirectory, mappingArg);
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
            string imgExt = Path.GetExtension(imagePath);
            if (string.IsNullOrWhiteSpace(imgExt))
                imgExt = ".png";
            string labeledPath = Path.Combine(outputDirectory, $"{stem}_labels{imgExt}");
            string overviewPath = Path.Combine(outputDirectory, $"{stem}_ov{imgExt}");
            string cornerDebugPath = Path.Combine(outputDirectory, $"{stem}_corner_debug{imgExt}");
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

        public sealed class FolderProcessResult
        {
            public bool Success { get; init; }
            public string Message { get; init; } = string.Empty;
            public string OutputDirectory { get; init; } = string.Empty;
            public List<ImageProcessEntry> Entries { get; init; } = new();
            public string? SummaryCsvPath { get; init; }
        }

        public sealed class ImageProcessEntry
        {
            public string ImageName { get; set; } = string.Empty;
            public bool Success { get; set; }
            public int? MinResolvableGroupId { get; set; }
            public double? MinResolvableCMean { get; set; }
            public string Message { get; set; } = string.Empty;
            public string OutputDirectory { get; set; } = string.Empty;
            public string? LabeledImagePath { get; set; }
            public string? OverviewImagePath { get; set; }
            public string? ExcelPath { get; set; }
            public string? CurvePath { get; set; }
            public string? SummaryJsonPath { get; set; }
        }

        /// <summary>
        /// 批量处理整个文件夹：一次 Python 调用处理全部图像（--input-dir），避免逐张启动进程。
        /// onProgress: (current, total, imageName) — 每处理完一张图像时回调。
        /// </summary>
        public static async Task<FolderProcessResult> ProcessFolderAsync(
            string folderPath,
            string? mappingPath,
            string outputDirectory,
            string pipeline,
            CancellationToken cancellationToken,
            Action<int, int, string>? onProgress = null)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                return new FolderProcessResult { Success = false, Message = "输入文件夹不存在。" };
            }

            string[] imageFiles = CollectImageFiles(folderPath);
            if (imageFiles.Length == 0)
            {
                return new FolderProcessResult { Success = false, Message = "文件夹中未找到可处理图像。" };
            }

            string? scriptPath = ResolveMrcScriptPath(pipeline);
            if (string.IsNullOrWhiteSpace(scriptPath) || !File.Exists(scriptPath))
            {
                return new FolderProcessResult
                {
                    Success = false,
                    Message = $"未找到 MRC 脚本，请确认 Python/{PipelineToScript(pipeline)} 已存在。"
                };
            }

            string? mappingArg = null;
            if (pipeline != "550")
            {
                mappingArg = ResolveMappingPath(mappingPath, scriptPath);
                if (string.IsNullOrWhiteSpace(mappingArg) || !File.Exists(mappingArg))
                {
                    return new FolderProcessResult
                    {
                        Success = false,
                        Message = "未找到映射表（xlsx）。请确认 Python/MappingTable.xlsx 存在。"
                    };
                }
            }

            Directory.CreateDirectory(outputDirectory);

            string stdOut;
            string stdErr;
            int exitCode;

            try
            {
                (exitCode, stdOut, stdErr) = await RunPythonFolderAsync(
                    scriptPath, folderPath, outputDirectory, mappingArg, imageFiles.Length,
                    cancellationToken, onProgress);
            }
            catch (OperationCanceledException)
            {
                return new FolderProcessResult
                {
                    Success = false,
                    Message = "MRC 批量处理已被用户取消。",
                    OutputDirectory = outputDirectory
                };
            }
            catch (Exception ex)
            {
                return new FolderProcessResult
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
                    err = BuildPythonMissingMessage();
                return new FolderProcessResult
                {
                    Success = false,
                    Message = $"MRC 批量处理失败（退出码 {exitCode}）：{err}".Trim(),
                    OutputDirectory = outputDirectory
                };
            }

            // 收集所有 *_summary.json，构建每张图像的结果
            var entries = new List<ImageProcessEntry>();
            string[] summaryFiles = Directory.GetFiles(outputDirectory, "*_summary.json", SearchOption.TopDirectoryOnly);

            foreach (string summaryPath in summaryFiles)
            {
                string jsonStem = Path.GetFileNameWithoutExtension(summaryPath);  // e.g. "image_summary"
                string imageStem = jsonStem.Replace("_summary", "");               // e.g. "image"
                int? groupId = ReadSummaryInt(summaryPath, "min_resolvable_group_id");
                double? cMean = ReadSummaryDouble(summaryPath, "min_resolvable_c_mean");

                // 查找对应的标注图（可能多种扩展名）
                string? labeledPath = FindOutputFile(outputDirectory, imageStem, "_labels");
                string? overviewPath = FindOutputFile(outputDirectory, imageStem, "_ov");
                string? excelPath = FindOutputFile(outputDirectory, imageStem, "_res", ".xlsx");
                string? curvePath = FindOutputFile(outputDirectory, imageStem, "_curve", ".png");

                entries.Add(new ImageProcessEntry
                {
                    ImageName = imageStem,
                    Success = groupId.HasValue,
                    MinResolvableGroupId = groupId,
                    MinResolvableCMean = cMean,
                    Message = groupId.HasValue ? "完成" : "未找到有效结果",
                    OutputDirectory = outputDirectory,
                    LabeledImagePath = labeledPath,
                    OverviewImagePath = overviewPath,
                    ExcelPath = excelPath,
                    CurvePath = curvePath,
                    SummaryJsonPath = summaryPath
                });
            }

            // 写 CSV 汇总
            string csvPath = Path.Combine(outputDirectory, "mrc_summary.csv");
            var csvLines = new List<string>
            {
                "image_name,success,min_resolvable_group_id,min_resolvable_c_mean,message,output_dir"
            };
            foreach (var e in entries)
            {
                csvLines.Add(string.Join(",",
                    CsvCell(e.ImageName),
                    e.Success ? "1" : "0",
                    e.MinResolvableGroupId?.ToString() ?? "",
                    e.MinResolvableCMean?.ToString("F6", CultureInfo.InvariantCulture) ?? "",
                    CsvCell(e.Message),
                    CsvCell(e.OutputDirectory)));
            }
            File.WriteAllLines(csvPath, csvLines, Encoding.UTF8);

            return new FolderProcessResult
            {
                Success = true,
                Message = $"批量处理完成：成功 {entries.Count(e => e.Success)}/{entries.Count}",
                OutputDirectory = outputDirectory,
                Entries = entries,
                SummaryCsvPath = csvPath
            };
        }

        private static string? FindOutputFile(string directory, string stem, string suffix, string? fixedExt = null)
        {
            if (fixedExt != null)
            {
                string path = Path.Combine(directory, $"{stem}{suffix}{fixedExt}");
                if (File.Exists(path)) return path;
            }

            foreach (string ext in SupportedImageExtensions)
            {
                string path = Path.Combine(directory, $"{stem}{suffix}{ext}");
                if (File.Exists(path)) return path;
            }

            return null;
        }

        private static string CsvCell(string? value)
        {
            string text = value ?? string.Empty;
            if (!text.Contains(',') && !text.Contains('"') && !text.Contains('\n') && !text.Contains('\r'))
                return text;
            return $"\"{text.Replace("\"", "\"\"")}\"";
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
            string plotFileName = "min_group_distribution.png",
            string pipeline = "1m6")
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

            string? scriptPath = ResolveMrcScriptPath(pipeline);
            if (string.IsNullOrWhiteSpace(scriptPath) || !File.Exists(scriptPath))
            {
                return new BatchDistributionPlotResult
                {
                    Success = false,
                    Message = $"未找到 MRC 脚本（{PipelineToScript(pipeline)}），无法生成分布图。"
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

        private static string PipelineToScript(string pipeline)
        {
            return pipeline == "550" ? "MRC_550.py" : "MRC_final.py";
        }

        private static string? ResolveMrcScriptPath(string pipeline = "1m6")
        {
            string scriptName = PipelineToScript(pipeline);
            string appBase = AppContext.BaseDirectory;
            string[] candidates =
            {
                Path.Combine(appBase, "Python", scriptName),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Python", scriptName),
                Path.GetFullPath(Path.Combine(appBase, "..", "..", "..", "..", "Python", scriptName))
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
            string? mappingPath)
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

        private static async Task<(int ExitCode, string StdOut, string StdErr)> RunPythonFolderAsync(
            string scriptPath,
            string folderPath,
            string outputPath,
            string? mappingPath,
            int totalImages,
            CancellationToken cancellationToken,
            Action<int, int, string>? onProgress = null)
        {
            foreach (string pythonExe in EnumeratePythonExecutableCandidates())
            {
                (int code, string o, string e)? result = await TryRunPythonFolderCommandAsync(
                    pythonExe, null, scriptPath, folderPath, outputPath, mappingPath,
                    totalImages, cancellationToken, onProgress);
                if (result.HasValue && result.Value.code != 9009)
                {
                    return (result.Value.code, result.Value.o, result.Value.e);
                }
            }

            return (9009, string.Empty, BuildPythonMissingMessage());
        }

        private static async Task<(int code, string o, string e)?> TryRunPythonFolderCommandAsync(
            string fileName,
            string? prefixArg,
            string scriptPath,
            string folderPath,
            string outputPath,
            string? mappingPath,
            int totalImages,
            CancellationToken cancellationToken,
            Action<int, int, string>? onProgress = null)
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
                process.StartInfo.ArgumentList.Add("--input-dir");
                process.StartInfo.ArgumentList.Add(folderPath);
                process.StartInfo.ArgumentList.Add("--output");
                process.StartInfo.ArgumentList.Add(outputPath);
                if (!string.IsNullOrWhiteSpace(mappingPath))
                {
                    process.StartInfo.ArgumentList.Add("--mapping");
                    process.StartInfo.ArgumentList.Add(mappingPath);
                }

                process.Start();

                // 逐行读取 stdout，解析 [OK]/[FAIL] 推送进度
                var stdOutBuilder = new StringBuilder();
                int processed = 0;
                var readOutTask = Task.Run(async () =>
                {
                    using var reader = process.StandardOutput;
                    while (true)
                    {
                        string? line = await reader.ReadLineAsync();
                        if (line == null) break;
                        stdOutBuilder.AppendLine(line);

                        if (line.StartsWith("[OK]") || line.StartsWith("[FAIL]"))
                        {
                            processed++;
                            string imageName = line.StartsWith("[OK]")
                                ? line.Substring(4).Trim()
                                : line.Substring(6).Trim();
                            // 去掉 ": error_message" 后缀
                            int colonIdx = imageName.IndexOf(':');
                            if (colonIdx > 0 && line.StartsWith("[FAIL]"))
                                imageName = imageName.Substring(0, colonIdx).Trim();
                            onProgress?.Invoke(processed, totalImages, imageName);
                        }
                    }
                });

                Task<string> readErrTask = process.StandardError.ReadToEndAsync();

                using (cancellationToken.Register(() =>
                {
                    try { if (!process.HasExited) process.Kill(); } catch { }
                }))
                {
                    await process.WaitForExitAsync(cancellationToken);
                }

                await readOutTask;
                return (process.ExitCode, stdOutBuilder.ToString(), await readErrTask);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                return null;
            }
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
            // 关掉输出缓冲：print() 立即发送到管道，C# 端才能逐行读到进度
            startInfo.Environment["PYTHONUNBUFFERED"] = "1";

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

        /// <summary>
        /// 实时读取单张图像的 MRC 结果（供进度回调使用）。
        /// </summary>
        public static ImageProcessEntry? TryReadImageSummary(string outputDirectory, string imageStem)
        {
            string summaryPath = Path.Combine(outputDirectory, $"{imageStem}_summary.json");
            int? groupId = ReadSummaryInt(summaryPath, "min_resolvable_group_id");
            double? cMean = ReadSummaryDouble(summaryPath, "min_resolvable_c_mean");

            if (!groupId.HasValue)
                return null;

            return new ImageProcessEntry
            {
                ImageName = imageStem,
                Success = true,
                MinResolvableGroupId = groupId,
                MinResolvableCMean = cMean,
                Message = "完成",
                OutputDirectory = outputDirectory,
                LabeledImagePath = FindOutputFile(outputDirectory, imageStem, "_labels"),
                OverviewImagePath = FindOutputFile(outputDirectory, imageStem, "_ov"),
                ExcelPath = FindOutputFile(outputDirectory, imageStem, "_res", ".xlsx"),
                CurvePath = FindOutputFile(outputDirectory, imageStem, "_curve", ".png"),
                SummaryJsonPath = summaryPath
            };
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
            string? mappingPath)
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
                if (!string.IsNullOrWhiteSpace(mappingPath))
                {
                    process.StartInfo.ArgumentList.Add("--mapping");
                    process.StartInfo.ArgumentList.Add(mappingPath);
                }

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
