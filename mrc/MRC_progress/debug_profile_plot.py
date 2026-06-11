#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
MRC_550 剖面调试可视化：对指定组的 rect1 绘制剖面线 + 波峰/波谷标注。
用法：python debug_profile_plot.py --input <图像路径> --groups 21,22,23,24,25
"""
import argparse
import os
import sys

import cv2
import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
import numpy as np

# 加载 MRC_550 模块
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from MRC_550 import MRC550Processor


def plot_profile_debug(processor, image_bgr, mapped, group_ids, out_dir, stem):
    """对指定 group_id 的 rect1 绘制剖面调试图。"""
    mask = processor._build_color_invariant_mask(image_bgr)
    expected = processor._default_expected_stripe_counts()
    h_img, w_img = image_bgr.shape[:2]
    os.makedirs(out_dir, exist_ok=True)

    rect1_list = [m for m in mapped if int(m["rect_id"]) == 1]

    for gid in group_ids:
        match = [m for m in rect1_list if int(m["group_id"]) == gid]
        if not match:
            print(f"  [WARN] 未找到 group {gid} 的 rect1")
            continue
        m = match[0]

        # --- 提取原始剖面（完全复现 _extract_profile_metrics 流程）---
        roi_xyxy, metrics = processor._profile_for_rect1(
            image_bgr, mask, m, profile_roi_pad=2,
            expected_stripes=expected.get(gid),
        )
        x1, y1, x2, y2 = roi_xyxy
        roi = image_bgr[y1:y2, x1:x2]

        # 重复 _extract_profile_metrics 里的步骤，拿到每一步中间结果
        roi_gray = cv2.cvtColor(roi, cv2.COLOR_BGR2GRAY).astype(np.float32)
        row_idx = int(np.argmax(np.std(roi_gray, axis=1)))

        # 原始单行
        line_single = roi_gray[row_idx, :].astype(np.float32)

        # 多行平均（21+）
        if gid >= processor.PROFILE_DENSE_GROUP_MIN:
            line_raw = processor._average_profile_rows(roi_gray, row_idx, num_rows=3)
        else:
            line_raw = roi_gray[row_idx, :].astype(np.float32)

        # 亮区裁边前
        line_before_trim = line_raw.copy()

        if gid >= processor.PROFILE_DENSE_MID_GROUP_MIN:
            line_raw, left, right = processor._trim_profile_line_bright_core(line_raw)
        else:
            left, right = 0, int(line_raw.size)

        # 平滑后
        sigma = processor._profile_smooth_sigma(gid)
        line_smooth = processor._smooth_profile_line(line_raw, sigma)

        # 峰谷检测
        peaks, valleys = processor._detect_peaks_valleys(
            line_smooth, group_id=gid, expected_stripes=expected.get(gid),
        )

        exp_n = expected.get(gid, 0)
        got_n = len(peaks) + len(valleys)

        # --- 绘图 ---
        fig, axes = plt.subplots(2, 1, figsize=(14, 9))
        fig.suptitle(
            f"{stem}  Group {gid}  Rect1 Profile Debug  "
            f"(expected={exp_n}  detected={got_n}  {'OK' if got_n == exp_n else 'MISMATCH'})",
            fontsize=13, fontweight="bold",
        )

        # ---- 上子图：原始 ROI + 多行平均后的剖面 ----
        ax = axes[0]
        x_single = np.arange(line_single.size)
        ax.plot(x_single, line_single, color="#cccccc", lw=0.8, alpha=0.7, label="single row (max-var)")
        if gid >= processor.PROFILE_DENSE_GROUP_MIN:
            x_avg = np.arange(line_before_trim.size)
            ax.plot(x_avg, line_before_trim, color="#1f77b4", lw=1.2, alpha=0.8, label="3-row average (before trim)")
        ax.axhline(np.mean(line_single), color="#888888", ls=":", lw=0.8)
        ax.set_ylabel("Gray Value")
        ax.set_title("Step 1: Raw Profile (single row vs multi-row average)")
        ax.legend(loc="best", fontsize=8)
        ax.grid(True, alpha=0.25)

        # ---- 下子图：平滑后 + 峰谷标注 ----
        ax = axes[1]
        x_line = np.arange(line_smooth.size)
        ax.plot(x_line, line_smooth, color="#333333", lw=1.0, label=f"smoothed (σ={sigma})")

        # 标注波峰（绿）
        if peaks:
            ax.scatter(
                [int(p) for p in peaks],
                [float(line_smooth[int(p)]) for p in peaks],
                s=60, c="#2ca02c", marker="^", edgecolors="#1b5e20",
                linewidths=1.0, zorder=5, label=f"peaks ({len(peaks)})",
            )
        # 标注波谷（红）
        if valleys:
            ax.scatter(
                [int(v) for v in valleys],
                [float(line_smooth[int(v)]) for v in valleys],
                s=60, c="#d62728", marker="v", edgecolors="#8b0000",
                linewidths=1.0, zorder=5, label=f"valleys ({len(valleys)})",
            )

        # 裁边区域着色
        if left > 0 or right < int(line_smooth.size):
            ax.axvspan(0, left, color="red", alpha=0.06)
            ax.axvspan(right, int(line_smooth.size), color="red", alpha=0.06)
            ax.text(left + 2, ax.get_ylim()[1] * 0.95, "trim←", fontsize=7, color="red", va="top")
            ax.text(right - 2, ax.get_ylim()[1] * 0.95, "→trim", fontsize=7, color="red", va="top", ha="right")

        ax.set_xlabel("Column Index (px)")
        ax.set_ylabel("Gray Value")
        ax.set_title(
            f"Step 2: Smoothed Profile + Detected Peaks/Valleys  "
            f"(sigma={sigma}, peaks={len(peaks)}, valleys={len(valleys)}, total={got_n})"
        )
        ax.legend(loc="best", fontsize=8)
        ax.grid(True, alpha=0.25)

        plt.tight_layout()
        out_path = os.path.join(out_dir, f"{stem}_g{gid:02d}_profile.png")
        plt.savefig(out_path, dpi=150, bbox_inches="tight")
        plt.close(fig)
        print(f"  [OK] Group {gid}: expected={exp_n} got={got_n}  →  {out_path}")


def main():
    plt.rcParams["font.sans-serif"] = ["Microsoft YaHei", "SimHei", "SimSun", "Noto Sans CJK SC", "DejaVu Sans"]
    plt.rcParams["axes.unicode_minus"] = False

    parser = argparse.ArgumentParser(description="MRC_550 剖面峰谷可视化调试")
    parser.add_argument("--input", required=True, help="输入图像路径")
    parser.add_argument("--output", default="", help="输出目录（默认：图像旁 debug_profile）")
    parser.add_argument(
        "--groups", default="19,20,21,22,23,24,25",
        help="要绘制的组号，逗号分隔",
    )
    args = parser.parse_args()

    if not os.path.exists(args.input):
        raise FileNotFoundError(f"图像不存在: {args.input}")

    group_ids = [int(x.strip()) for x in args.groups.split(",") if x.strip()]

    processor = MRC550Processor()
    image = processor._imread_unicode(args.input)
    if image is None:
        raise RuntimeError(f"无法读取图像: {args.input}")

    # 倾角校正
    angle_deg = processor._measure_side_angle(image)
    border_value = processor._estimate_dark_border_value(image)
    aligned, _, _ = processor._refine_rotation_with_verification(image, angle_deg, border_value)
    work_bgr = aligned

    # 矩形检测 + 编号
    rect_mask = processor._build_color_invariant_mask(work_bgr)
    rect_candidates = processor._detect_rectangles(work_bgr, rect_mask)
    mapped = processor._assign_labels_5x5_grid(rect_candidates, image_bgr=work_bgr)

    stem = os.path.splitext(os.path.basename(args.input))[0]
    out_dir = args.output or os.path.join(os.path.dirname(args.input), f"debug_{stem}")
    os.makedirs(out_dir, exist_ok=True)

    print(f"Generating profile plots for groups: {group_ids}")
    plot_profile_debug(processor, work_bgr, mapped, group_ids, out_dir, stem)
    print(f"\nDone! Output: {out_dir}")


if __name__ == "__main__":
    main()
