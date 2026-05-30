using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageCaptureApp.Modules
{
  /// <summary>
  /// 批量 MRC 结果汇总：统计最小可分辨组号分布，并判定本次批量的代表组号。
  /// </summary>
  public static class MrcBatchSummaryHelper
  {
    public sealed class BatchGroupDistribution
    {
      public IReadOnlyDictionary<int, int> CountByGroupId { get; init; } =
        new Dictionary<int, int>();

      public int ValidImageCount { get; init; }

      public int? BatchMinResolvableGroupId { get; init; }

      public int PeakCount { get; init; }
    }

    /// <summary>
    /// 统计有效组号分布。无效或未找到的组号会被忽略。
    /// </summary>
    public static BatchGroupDistribution Analyze(IEnumerable<int?> groupIds)
    {
      var counts = new Dictionary<int, int>();
      int validCount = 0;

      foreach (int? groupId in groupIds)
      {
        if (!groupId.HasValue)
        {
          continue;
        }

        validCount++;
        int key = groupId.Value;
        counts.TryGetValue(key, out int current);
        counts[key] = current + 1;
      }

      int? batchGroup = PickBatchMinResolvableGroup(counts);
      int peakCount = batchGroup.HasValue ? counts[batchGroup.Value] : 0;

      return new BatchGroupDistribution
      {
        CountByGroupId = counts,
        ValidImageCount = validCount,
        BatchMinResolvableGroupId = batchGroup,
        PeakCount = peakCount
      };
    }

    /// <summary>
    /// 取出现次数最多的组号；若并列，取较小的组号（向下兼容）。
    /// </summary>
    public static int? PickBatchMinResolvableGroup(IReadOnlyDictionary<int, int> countByGroupId)
    {
      if (countByGroupId.Count == 0)
      {
        return null;
      }

      int maxCount = countByGroupId.Values.Max();
      return countByGroupId
        .Where(kv => kv.Value == maxCount)
        .Select(kv => kv.Key)
        .Min();
    }

    public static string FormatConclusion(BatchGroupDistribution distribution, int totalProcessed, int failedCount)
    {
      if (distribution.ValidImageCount == 0)
      {
        return $"批量处理完成：共 {totalProcessed} 张，有效组号 0 张，失败 {failedCount} 张，无法判定最小可分辨组。";
      }

      if (!distribution.BatchMinResolvableGroupId.HasValue)
      {
        return $"批量处理完成：共 {totalProcessed} 张，有效组号 {distribution.ValidImageCount} 张，失败 {failedCount} 张，无法判定最小可分辨组。";
      }

      int group = distribution.BatchMinResolvableGroupId.Value;
      return
        $"本次批量最小可分辨组：{group}（出现 {distribution.PeakCount}/{distribution.ValidImageCount} 张；" +
        $"共处理 {totalProcessed} 张，失败 {failedCount} 张；并列时取较小组号）";
    }
  }
}
