#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
MRC_550 — 550 图案靶标（1280×1024）：5×5 组编号 1–25，每组 4 矩形。
独立脚本，不依赖 MRC_final.py。
"""
import argparse
import glob
import json
import math
import os
from typing import Any, Dict, List, Optional, Tuple

import cv2
import matplotlib
import numpy as np
import openpyxl

matplotlib.use("Agg")
import matplotlib.pyplot as plt
from matplotlib import font_manager
from matplotlib.ticker import FuncFormatter

RectInfo = Dict[str, Any]


class MRC550Processor:
    """550 图案：25 组（5 行 × 5 列），组号 1–25 从左到右、从上到下；每组 4 个矩形。"""

    GROUP_ROWS = 5
    GROUP_COLS = 5
    RECTS_PER_GROUP = 4
    MIN_RECTS_FOR_LABEL = 100
    TARGET_RECT_COUNT = 100
    MASK_OPEN_SIZE = 5
    # 基准剖面：单行最大方差 + σ 平滑后差分（25组统一处理）
    PROFILE_LINE_SMOOTH_SIGMA = 1.5
    FRONT_GROUP_TRUST_MAX = 19
    PROFILE_ROI_SEARCH_L_RATIO = 0.10
    PROFILE_ROI_SEARCH_L_MAX = 12
    # 第 1 组 1-1：轮廓 bbox 常略偏右，向左多搜一段，仍用掩膜裁掉黑边
    PROFILE_G1_SEARCH_L_RATIO = 0.35
    PROFILE_G1_SEARCH_L_MAX = 28
    PROFILE_G1_SEARCH_L_MIN = 4
    # 粘连拆分：面积 > 中位数 × 此比例则沿长轴一分为二（第 20 组常见 ~1.5×）
    FIELD_MERGE_SPLIT_RATIO = 1.48
    # 近方形且宽高均 ~2× 典型小格 → 按组内 2×2 四等分
    FIELD_MERGE_QUAD_ASPECT_MIN = 0.82
    FIELD_MERGE_QUAD_ASPECT_MAX = 1.22
    FIELD_MERGE_QUAD_SCALE_MIN = 1.65
    # 水平两格粘连：切分位置在典型格宽附近微调（±18% 掩膜谷值）
    FIELD_MERGE_HPAIR_WIDTH_SCALE = 1.45
    FIELD_MERGE_REF_WIDTH_TOL = 0.18
    # 单格宽高已接近典型 cell 时不再二次切分（四等分后 ~68px 仍可能 area>阈值）
    FIELD_MERGE_SINGLE_CELL_MAX_SCALE = 1.28
    # 方案 1/2 仅用于第 19–20 组粘连区；其余组仍用基础二分，避免误拆 1–15 组
    FIELD_MERGE_ADVANCED_GROUP_IDS = (19, 20)
    FIELD_GROUP_Y_GAP = 45.0
    FIELD_SUBROW_Y_GAP = 26.0
    FIELD_ROW_STRIPE_MIN_RATIO = 0.42
    CORNER_RECT_LABELS = {"TL": "1-1", "TR": "5-1", "BL": "21-1", "BR": "25-1"}
    # 原图已摆正时：0° 与最优方向差距不大则不再旋转
    ORIENT_PREFER_ZERO_MISMATCH_MARGIN = 20
    # 测得倾角低于此值时不旋转（避免裁切靶标边缘）
    SIDE_ANGLE_APPLY_MIN_DEG = 0.6

    @staticmethod
    def _imread_unicode(path: str) -> Optional[np.ndarray]:
        try:
            data = np.fromfile(path, dtype=np.uint8)
            if data.size == 0:
                return None
            return cv2.imdecode(data, cv2.IMREAD_COLOR)
        except Exception:
            return None

    @staticmethod
    def _imwrite_unicode(path: str, image: np.ndarray) -> bool:
        ext = os.path.splitext(path)[1] or ".png"
        ok, buf = cv2.imencode(ext, image)
        if not ok:
            return False
        try:
            buf.tofile(path)
            return True
        except Exception:
            return False

    @classmethod
    def _pick_corner_profile_rects(cls, mapped: List[Dict[str, Any]]) -> Dict[str, Dict[str, Any]]:
        """四角剖面矩形：固定取 1-1、5-1、21-1、25-1。"""
        by_label = {str(m["label"]): m for m in mapped}
        picked: Dict[str, Dict[str, Any]] = {}
        for name, label in cls.CORNER_RECT_LABELS.items():
            m = by_label.get(label)
            if m is not None:
                picked[name] = m
        if len(picked) < 4:
            missing = [lab for lab in cls.CORNER_RECT_LABELS.values() if lab not in by_label]
            raise RuntimeError(f"四角剖面矩形不足，缺少: {missing}")
        return picked

    @staticmethod
    def _assign_rects_to_grid(
        rects_use: List[RectInfo],
        grid_points: List[Tuple[int, int, float, float]],
        *,
        min_matches: int,
    ) -> List[Dict[str, Any]]:
        pairs: List[Tuple[float, int, int]] = []
        for i, r in enumerate(rects_use):
            cx, cy = r["center"]
            for j, (_gid, _rid, gx, gy) in enumerate(grid_points):
                d = (float(cx) - gx) ** 2 + (float(cy) - gy) ** 2
                pairs.append((d, i, j))
        pairs.sort(key=lambda x: x[0])

        used_rect: set = set()
        used_grid: set = set()
        assign: Dict[int, int] = {}
        for _d, i, j in pairs:
            if i in used_rect or j in used_grid:
                continue
            used_rect.add(i)
            used_grid.add(j)
            assign[i] = j
            if len(assign) == len(rects_use):
                break
        if len(assign) < min_matches:
            raise RuntimeError(f"编号唯一匹配失败，已匹配: {len(assign)} / {len(rects_use)}")

        mapped: List[Dict[str, Any]] = []
        for i, r in enumerate(rects_use):
            if i not in assign:
                continue
            gid, rid, _gx, _gy = grid_points[assign[i]]
            label = f"{gid}-{rid}"
            mapped.append(
                {
                    "label": label,
                    "group_id": int(gid),
                    "rect_id": int(rid),
                    "center": (float(r["center"][0]), float(r["center"][1])),
                    "bbox": tuple(int(v) for v in r["bbox"]),
                    "rotated_box": r["rotated_box"],
                }
            )
        mapped.sort(key=lambda x: (x["group_id"], x["rect_id"]))
        return mapped

    @staticmethod
    def _rect_info_from_bbox(bbox: Tuple[int, int, int, int], source: Optional[RectInfo] = None) -> RectInfo:
        x, y, w, h = (int(v) for v in bbox)
        cx, cy = float(x + w / 2.0), float(y + h / 2.0)
        area = float(w * h)
        aspect = float(max(w, h) / max(min(w, h), 1))
        box = np.array([[x, y], [x + w, y], [x + w, y + h], [x, y + h]], dtype=np.float32)
        info: RectInfo = {
            "bbox": (x, y, w, h),
            "center": (cx, cy),
            "area": area if source is None else float(source.get("area", area)),
            "aspect": aspect,
            "rotated_box": np.int32(np.round(box)).tolist(),
        }
        return info

    @staticmethod
    def _split_rect_info_once(r: RectInfo) -> List[RectInfo]:
        x, y, w, h = (int(v) for v in r["bbox"])
        if w < 4 or h < 4:
            return [r]
        if w >= h:
            w1 = max(2, w // 2)
            w2 = max(2, w - w1)
            return [
                MRC550Processor._rect_info_from_bbox((x, y, w1, h)),
                MRC550Processor._rect_info_from_bbox((x + w1, y, w2, h)),
            ]
        h1 = max(2, h // 2)
        h2 = max(2, h - h1)
        return [
            MRC550Processor._rect_info_from_bbox((x, y, w, h1)),
            MRC550Processor._rect_info_from_bbox((x, y + h1, w, h2)),
        ]

    @staticmethod
    def _split_rect_quadrants(r: RectInfo) -> List[RectInfo]:
        """组内 2×2 粘连：按 bbox 四等分。

        TODO(550-g19-20): 四等分后子框仍为几何等分 bbox，未按掩膜亮区重贴合；
        第 20 组在色散/透视下可能仍略偏；后续应对每象限用 mask 裁 tight bbox。
        """
        x, y, w, h = (int(v) for v in r["bbox"])
        if w < 8 or h < 8:
            return MRC550Processor._split_rect_info_once(r)
        w1 = max(2, w // 2)
        w2 = max(2, w - w1)
        h1 = max(2, h // 2)
        h2 = max(2, h - h1)
        return [
            MRC550Processor._rect_info_from_bbox((x, y, w1, h1)),
            MRC550Processor._rect_info_from_bbox((x + w1, y, w2, h1)),
            MRC550Processor._rect_info_from_bbox((x, y + h1, w1, h2)),
            MRC550Processor._rect_info_from_bbox((x + w1, y + h1, w2, h2)),
        ]

    @classmethod
    def _reference_cell_size(cls, candidates: List[RectInfo], med_area: float) -> Tuple[float, float]:
        """从正常小轮廓估计单格宽/高（方案 2 参考）。"""
        split_ratio = float(cls.FIELD_MERGE_SPLIT_RATIO)
        normal = [r for r in candidates if float(r["area"]) <= med_area * split_ratio]
        pool = normal if len(normal) >= 8 else list(candidates)
        widths = [float(r["bbox"][2]) for r in pool]
        heights = [float(r["bbox"][3]) for r in pool]
        ref_w = float(np.median(widths)) if widths else max(20.0, float(np.sqrt(med_area)))
        ref_h = float(np.median(heights)) if heights else ref_w
        return ref_w, ref_h

    @classmethod
    def _looks_like_group_quad_merge(cls, r: RectInfo, ref_w: float, ref_h: float) -> bool:
        w, h = (int(v) for v in r["bbox"][2:])
        if ref_w < 4.0 or ref_h < 4.0 or w < 8 or h < 8:
            return False
        aspect = float(w) / max(float(h), 1.0)
        return (
            float(w) >= ref_w * float(cls.FIELD_MERGE_QUAD_SCALE_MIN)
            and float(h) >= ref_h * float(cls.FIELD_MERGE_QUAD_SCALE_MIN)
            and float(cls.FIELD_MERGE_QUAD_ASPECT_MIN) <= aspect <= float(cls.FIELD_MERGE_QUAD_ASPECT_MAX)
        )

    @classmethod
    def _pick_horizontal_split_col(
        cls,
        proj: np.ndarray,
        w: int,
        ref_w: float,
    ) -> int:
        """方案 2：以典型格宽为切分点，在附近窗口内用掩膜谷值微调。"""
        split_local = int(round(ref_w))
        split_local = max(2, min(w - 2, split_local))
        tol = float(cls.FIELD_MERGE_REF_WIDTH_TOL)
        lo = max(1, int(round(ref_w * (1.0 - tol))))
        hi = min(w - 1, int(round(ref_w * (1.0 + tol))))
        if hi <= lo or proj.size < w:
            return split_local
        split_local = lo + int(np.argmin(proj[lo:hi]))
        return max(2, min(w - 2, split_local))

    @classmethod
    def _split_rect_by_mask(
        cls,
        r: RectInfo,
        mask: np.ndarray,
        ref_cell_w: float = 0.0,
        ref_cell_h: float = 0.0,
    ) -> List[RectInfo]:
        """掩膜谷值切分；ref_cell_w<=0 时仅用中段窗口（基础模式）。"""
        x, y, w, h = (int(v) for v in r["bbox"])
        if w < 6 or h < 6:
            return cls._split_rect_info_once(r)
        y1, y2 = max(0, y), min(mask.shape[0], y + h)
        x1, x2 = max(0, x), min(mask.shape[1], x + w)
        roi = mask[y1:y2, x1:x2]
        if roi.size == 0:
            return cls._split_rect_info_once(r)

        use_ref = ref_cell_w > 4.0 and ref_cell_h > 4.0
        h_pair = use_ref and float(w) >= ref_cell_w * float(cls.FIELD_MERGE_HPAIR_WIDTH_SCALE) and float(h) < ref_cell_h * 1.35

        if w >= h or h_pair:
            proj = np.count_nonzero(roi, axis=0).astype(np.float32)
            if use_ref and (h_pair or float(w) >= ref_cell_w * 1.25):
                split_local = cls._pick_horizontal_split_col(proj, w, ref_cell_w)
            else:
                lo, hi = max(1, w // 4), max(2, (3 * w) // 4)
                if hi <= lo:
                    return cls._split_rect_info_once(r)
                split_local = lo + int(np.argmin(proj[lo:hi]))
                split_local = max(2, min(w - 2, split_local))
            return [
                cls._rect_info_from_bbox((x, y, split_local, h)),
                cls._rect_info_from_bbox((x + split_local, y, w - split_local, h)),
            ]

        proj = np.count_nonzero(roi, axis=1).astype(np.float32)
        if use_ref:
            split_local = int(round(ref_cell_h))
            split_local = max(2, min(h - 2, split_local))
            tol = float(cls.FIELD_MERGE_REF_WIDTH_TOL)
            lo = max(1, int(round(ref_cell_h * (1.0 - tol))))
            hi = min(h - 1, int(round(ref_cell_h * (1.0 + tol))))
            if hi > lo and proj.size >= h:
                split_local = lo + int(np.argmin(proj[lo:hi]))
                split_local = max(2, min(h - 2, split_local))
        else:
            lo, hi = max(1, h // 4), max(2, (3 * h) // 4)
            if hi <= lo:
                return cls._split_rect_info_once(r)
            split_local = lo + int(np.argmin(proj[lo:hi]))
            split_local = max(2, min(h - 2, split_local))
        return [
            cls._rect_info_from_bbox((x, y, w, split_local)),
            cls._rect_info_from_bbox((x, y + split_local, w, h - split_local)),
        ]

    @staticmethod
    def _rect_center_in_field_groups(
        r: RectInfo,
        bounds: Tuple[float, float, float, float],
        group_ids: Tuple[int, ...],
    ) -> bool:
        x0, y0, x1, y1 = bounds
        cx, cy = (float(r["center"][0]), float(r["center"][1]))
        fw = max(float(x1 - x0), 1.0)
        fh = max(float(y1 - y0), 1.0)
        for gid in group_ids:
            gr = (int(gid) - 1) // MRC550Processor.GROUP_COLS
            gc = (int(gid) - 1) % MRC550Processor.GROUP_COLS
            gx0 = x0 + gc * fw / MRC550Processor.GROUP_COLS
            gx1 = x0 + (gc + 1) * fw / MRC550Processor.GROUP_COLS
            gy0 = y0 + gr * fh / MRC550Processor.GROUP_ROWS
            gy1 = y0 + (gr + 1) * fh / MRC550Processor.GROUP_ROWS
            if gx0 <= cx <= gx1 and gy0 <= cy <= gy1:
                return True
        return False

    @classmethod
    def _already_single_cell(cls, r: RectInfo, ref_w: float, ref_h: float) -> bool:
        w, h = (int(v) for v in r["bbox"][2:])
        scale = float(cls.FIELD_MERGE_SINGLE_CELL_MAX_SCALE)
        return float(w) <= ref_w * scale and float(h) <= ref_h * scale

    def _expand_merged_candidates(
        self,
        candidates: List[RectInfo],
        mask: Optional[np.ndarray] = None,
        field_bounds: Optional[Tuple[float, float, float, float]] = None,
    ) -> List[RectInfo]:
        """粘连拆分：方案 1/2 仅限第 19–20 组区域；其余组用基础二分/掩膜谷值。"""
        if not candidates:
            return []
        areas = np.array([float(r["area"]) for r in candidates], dtype=np.float32)
        med = float(np.median(areas))
        if med < 1.0:
            return list(candidates)

        ref_w, ref_h = self._reference_cell_size(candidates, med)
        advanced_groups = tuple(int(g) for g in self.FIELD_MERGE_ADVANCED_GROUP_IDS)
        expanded: List[RectInfo] = []
        split_ratio = float(self.FIELD_MERGE_SPLIT_RATIO)
        for r in candidates:
            if float(r["area"]) <= med * split_ratio:
                expanded.append(r)
                continue
            if self._already_single_cell(r, ref_w, ref_h):
                expanded.append(r)
                continue
            use_advanced = (
                field_bounds is not None
                and self._rect_center_in_field_groups(r, field_bounds, advanced_groups)
            )
            if use_advanced and self._looks_like_group_quad_merge(r, ref_w, ref_h):
                expanded.extend(self._split_rect_quadrants(r))
            elif use_advanced and mask is not None:
                expanded.extend(self._split_rect_by_mask(r, mask, ref_w, ref_h))
            elif mask is not None:
                expanded.extend(self._split_rect_by_mask(r, mask))
            else:
                expanded.extend(self._split_rect_info_once(r))
        return expanded

    @staticmethod
    def _area_inlier_rects(candidates: List[RectInfo], *, min_count: int = 40) -> List[RectInfo]:
        areas = np.array([float(r["area"]) for r in candidates], dtype=np.float32)
        med = float(np.median(areas))
        inliers = [
            r
            for r in candidates
            if med * 0.18 <= float(r["area"]) <= med * 3.2 and float(r["aspect"]) <= 2.6
        ]
        return inliers if len(inliers) >= min_count else list(candidates)

    # ---------- 靶标外框：10 行 × 10 列核心场 ----------
    @staticmethod
    def _split_rects_by_y_gap(rects: List[RectInfo], gap: float) -> List[List[RectInfo]]:
        if not rects:
            return []
        sorted_r = sorted(rects, key=lambda r: float(r["center"][1]))
        groups: List[List[RectInfo]] = [[sorted_r[0]]]
        for r in sorted_r[1:]:
            if float(r["center"][1]) - float(groups[-1][-1]["center"][1]) > gap:
                groups.append([r])
            else:
                groups[-1].append(r)
        return groups

    @classmethod
    def _build_physical_row_groups(cls, rects: List[RectInfo]) -> List[List[RectInfo]]:
        """先按组带(大间隙)再按子行(小间隙)切分，得到物理行。"""
        rows: List[List[RectInfo]] = []
        for band in cls._split_rects_by_y_gap(rects, cls.FIELD_GROUP_Y_GAP):
            if len(band) < 6:
                continue
            rows.extend(cls._split_rects_by_y_gap(band, cls.FIELD_SUBROW_Y_GAP))
        return rows

    @staticmethod
    def _merge_sparse_row_groups(rows: List[List[RectInfo]], max_n: int = 4) -> List[List[RectInfo]]:
        if not rows:
            return []
        merged: List[List[RectInfo]] = [list(rows[0])]
        for row in rows[1:]:
            if len(row) <= max_n:
                merged[-1].extend(row)
            else:
                merged.append(list(row))
        return merged

    @staticmethod
    def _row_stripe_strength(image_bgr: np.ndarray, row_rects: List[RectInfo]) -> float:
        """行内矩形条纹强度：下方空白区通常接近 0。"""
        if image_bgr is None or not row_rects:
            return 0.0
        h_img, w_img = image_bgr.shape[:2]
        scores: List[float] = []
        for r in row_rects:
            x, y, w, h = (int(v) for v in r["bbox"])
            x1, y1 = max(0, x), max(0, y)
            x2, y2 = min(w_img, x + w), min(h_img, y + h)
            if x2 - x1 < 4 or y2 - y1 < 4:
                continue
            gray = cv2.cvtColor(image_bgr[y1:y2, x1:x2], cv2.COLOR_BGR2GRAY)
            scores.append(float(np.max(np.std(gray, axis=1))))
        return float(np.median(np.array(scores, dtype=np.float32))) if scores else 0.0

    @classmethod
    def _pick_ten_rects_in_row(cls, row_rects: List[RectInfo]) -> List[RectInfo]:
        if len(row_rects) <= cls.GROUP_COLS * 2:
            return list(row_rects)
        sorted_r = sorted(row_rects, key=lambda r: float(r["center"][0]))
        need = cls.GROUP_COLS * 2
        best = sorted_r[:need]
        best_var = float("inf")
        for i in range(len(sorted_r) - need + 1):
            window = sorted_r[i : i + need]
            xs = np.array([float(r["center"][0]) for r in window], dtype=np.float32)
            var = float(np.std(np.diff(xs))) if xs.size >= 2 else 0.0
            if var < best_var:
                best_var = var
                best = window
        return best

    @classmethod
    def _score_ten_row_window(
        cls,
        rows10: List[List[RectInfo]],
        image_bgr: Optional[np.ndarray] = None,
    ) -> float:
        counts = [len(r) for r in rows10]
        score = float(sum(abs(c - cls.GROUP_COLS * 2) for c in counts))
        score += float(sum(max(0, cls.GROUP_COLS * 2 - 1 - c) * 8.0 for c in counts))
        ys = np.array([float(np.median([float(x["center"][1]) for x in row])) for row in rows10], dtype=np.float32)
        if ys.size >= 2:
            dy = np.diff(ys)
            dy_med = float(np.median(dy))
            score += float(np.std(dy)) / max(dy_med, 1.0) * 12.0
        if image_bgr is not None:
            strengths = [cls._row_stripe_strength(image_bgr, row) for row in rows10]
            if strengths:
                ref = float(np.percentile(strengths, 75))
                if ref > 1e-3:
                    for s in strengths:
                        if s < ref * cls.FIELD_ROW_STRIPE_MIN_RATIO:
                            score += 80.0
        return score

    @classmethod
    def _select_core_field_rows(
        cls,
        rects: List[RectInfo],
        image_bgr: Optional[np.ndarray] = None,
    ) -> List[List[RectInfo]]:
        """从候选里选出最像 10 行 × 10 列的物理行（排除下方无条纹区域）。"""
        inliers = cls._area_inlier_rects(rects)
        physical = cls._merge_sparse_row_groups(cls._build_physical_row_groups(inliers))
        if image_bgr is not None and physical:
            strengths = [cls._row_stripe_strength(image_bgr, row) for row in physical]
            ref = float(np.percentile(strengths, 75)) if strengths else 0.0
            if ref > 1e-3:
                physical = [
                    row
                    for row, s in zip(physical, strengths)
                    if s >= ref * cls.FIELD_ROW_STRIPE_MIN_RATIO
                ]

        need_rows = cls.GROUP_ROWS * 2
        if len(physical) < need_rows:
            sorted_r = sorted(inliers, key=lambda r: float(r["center"][1]))
            order_y = np.argsort([float(r["center"][1]) for r in sorted_r])
            return [
                [sorted_r[int(i)] for i in split]
                for split in np.array_split(order_y, need_rows)
                if len(split) > 0
            ]

        best_score = float("inf")
        best_start = 0
        for start in range(len(physical) - need_rows + 1):
            window = physical[start : start + need_rows]
            score = cls._score_ten_row_window(window, image_bgr=image_bgr)
            if score < best_score - 1e-6:
                best_score = score
                best_start = start
            elif abs(score - best_score) <= 5.0 and start < best_start:
                best_start = start

        return [list(r) for r in physical[best_start : best_start + need_rows]]

    @classmethod
    def _select_core_field_rects(
        cls,
        candidates: List[RectInfo],
        image_bgr: Optional[np.ndarray] = None,
    ) -> Tuple[List[RectInfo], List[List[RectInfo]]]:
        inliers = cls._area_inlier_rects(candidates)
        row_groups = cls._select_core_field_rows(inliers, image_bgr=image_bgr)
        core: List[RectInfo] = []
        for row in row_groups:
            core.extend(cls._pick_ten_rects_in_row(row))
        if len(core) < 80:
            core = list(inliers)
        return core, row_groups

    @classmethod
    def _resolve_field_geometry(
        cls,
        candidates: List[RectInfo],
        image_bgr: Optional[np.ndarray] = None,
    ) -> Tuple[Tuple[float, float, float, float], List[List[RectInfo]]]:
        """两遍精修：选 10 行核心场 → 外框 → 再筛框外离群 → 再估外框。"""
        _core, row_groups = cls._select_core_field_rects(candidates, image_bgr=image_bgr)
        bounds = cls._estimate_field_bounds(candidates, image_bgr=image_bgr, row_groups=row_groups)
        inliers = cls._filter_candidates_in_field(candidates, bounds)
        if len(inliers) >= cls.MIN_RECTS_FOR_LABEL - 5:
            _core, row_groups = cls._select_core_field_rects(inliers, image_bgr=image_bgr)
            bounds = cls._estimate_field_bounds(inliers, image_bgr=image_bgr, row_groups=row_groups)
        return bounds, row_groups

    @classmethod
    def _estimate_field_bounds(
        cls,
        candidates: List[RectInfo],
        image_bgr: Optional[np.ndarray] = None,
        row_groups: Optional[List[List[RectInfo]]] = None,
    ) -> Tuple[float, float, float, float]:
        """
        100 矩形整体外框：先锁定 10 行物理行（排除下方无条纹误检），再按行延长线定界。
        """
        inliers = cls._area_inlier_rects(candidates)
        if len(inliers) < 40:
            raise RuntimeError(f"估计靶标外框的矩形过少: {len(inliers)}")

        if row_groups is None:
            _, row_groups = cls._select_core_field_rects(inliers, image_bgr=image_bgr)

        n_rect_rows = cls.GROUP_ROWS * 2
        n_rect_cols = cls.GROUP_COLS * 2

        row_x_mins: List[float] = []
        row_x_maxs: List[float] = []
        row_y_refs: List[float] = []
        dx_samples: List[float] = []
        row_clusters: List[np.ndarray] = []

        for row in row_groups:
            if not row:
                continue
            pts = np.array([r["center"] for r in row], dtype=np.float32)
            if pts.shape[0] < 2:
                continue
            row_clusters.append(pts)
            picked = cls._pick_ten_rects_in_row(row)
            row_x_mins.append(float(np.min(pts[:, 0])))
            row_x_maxs.append(float(np.max(pts[:, 0])))
            fit = cv2.fitLine(pts.reshape(-1, 1, 2).astype(np.float32), cv2.DIST_L2, 0, 0.01, 0.01)
            row_y_refs.append(float(fit.flatten()[3]))

            xs = np.sort(np.array([float(r["center"][0]) for r in picked], dtype=np.float32))
            if xs.size >= 2:
                dx_samples.extend(np.diff(xs).astype(float).tolist())

        if len(row_y_refs) < 3:
            centers = np.array([r["center"] for r in inliers], dtype=np.float32)
            xs, ys = centers[:, 0], centers[:, 1]
            x0, x1 = float(np.percentile(xs, 8)), float(np.percentile(xs, 92))
            y0, y1 = float(np.percentile(ys, 8)), float(np.percentile(ys, 92))
            pad_x = max(4.0, (x1 - x0) * 0.008)
            pad_y = max(4.0, (y1 - y0) * 0.008)
            return x0 - pad_x, y0 - pad_y, x1 + pad_x, y1 + pad_y

        x0 = float(np.median(row_x_mins))
        x1 = float(np.median(row_x_maxs))
        y0 = float(row_y_refs[0])
        y1 = float(row_y_refs[-1])

        dy = float(np.median(np.diff(row_y_refs))) if len(row_y_refs) >= 2 else max(8.0, (y1 - y0) / max(n_rect_rows - 1, 1))
        dx = float(np.median(dx_samples)) if dx_samples else dy

        half_w = dx * 0.52
        half_h = dy * 0.52
        pad_x = max(2.0, dx * 0.05)
        pad_y = max(2.0, dy * 0.05)

        x_left = x0 - half_w - pad_x
        x_right = x1 + half_w + pad_x
        y_top = y0 - half_h - pad_y
        y_bot = y1 + half_h + pad_y

        core_centers_list: List[RectInfo] = []
        for row in row_groups:
            core_centers_list.extend(cls._pick_ten_rects_in_row(row))
        if len(core_centers_list) >= 80:
            core_centers = np.array([r["center"] for r in core_centers_list], dtype=np.float32)
            order_x = np.argsort(core_centers[:, 0])
            col_clusters = [core_centers[split] for split in np.array_split(order_x, n_rect_cols) if len(split) > 0]
        else:
            all_c = np.array([r["center"] for r in inliers], dtype=np.float32)
            order_x = np.argsort(all_c[:, 0])
            col_clusters = [all_c[split] for split in np.array_split(order_x, n_rect_cols) if len(split) > 0]

        if len(col_clusters) >= n_rect_cols // 2:
            col_y_mins = [float(np.min(pts[:, 1])) for pts in col_clusters]
            col_y_maxs = [float(np.max(pts[:, 1])) for pts in col_clusters]
            y_top = max(y_top, float(np.median(col_y_mins)) - half_h)
            y_bot = min(y_bot, float(np.median(col_y_maxs)) + half_h)

        row_lines: List[Tuple[float, float, float, float]] = []
        for pts in row_clusters:
            fit = cv2.fitLine(pts.reshape(-1, 1, 2).astype(np.float32), cv2.DIST_L2, 0, 0.01, 0.01)
            vx, vy, cx, cy = (float(v) for v in fit.flatten())

            def _y_on_line(x: float, vx: float = vx, vy: float = vy, cx: float = cx, cy: float = cy) -> float:
                if abs(vx) < 1e-6:
                    return cy
                return cy + (vy / vx) * (x - cx)

            row_lines.append((x_left, _y_on_line(x_left), x_right, _y_on_line(x_right)))

        cls._last_field_row_lines = row_lines
        return x_left, y_top, x_right, y_bot

    @classmethod
    def _field_row_extension_lines(cls) -> List[Tuple[float, float, float, float]]:
        return list(getattr(cls, "_last_field_row_lines", []))

    @staticmethod
    def _filter_candidates_in_field(
        candidates: List[RectInfo],
        bounds: Tuple[float, float, float, float],
        *,
        margin_ratio: float = 0.01,
    ) -> List[RectInfo]:
        x0, y0, x1, y1 = bounds
        w, h = max(x1 - x0, 1.0), max(y1 - y0, 1.0)
        mx, my = w * margin_ratio, h * margin_ratio
        x0 -= mx
        x1 += mx
        y0 -= my
        y1 += my
        kept: List[RectInfo] = []
        for r in candidates:
            cx, cy = r["center"]
            if x0 <= float(cx) <= x1 and y0 <= float(cy) <= y1:
                kept.append(r)
        return kept

    @staticmethod
    def _build_field_slot_grid(bounds: Tuple[float, float, float, float]) -> List[Tuple[int, int, float, float]]:
        """在大矩形范围内均匀划分 5×5 组 × 4 矩形 = 100 个槽位。"""
        x0, y0, x1, y1 = bounds
        fw, fh = float(x1 - x0), float(y1 - y0)
        slots: List[Tuple[int, int, float, float]] = []
        for gr in range(MRC550Processor.GROUP_ROWS):
            for gc in range(MRC550Processor.GROUP_COLS):
                gid = gr * MRC550Processor.GROUP_COLS + gc + 1
                gx0 = x0 + gc * fw / MRC550Processor.GROUP_COLS
                gx1 = x0 + (gc + 1) * fw / MRC550Processor.GROUP_COLS
                gy0 = y0 + gr * fh / MRC550Processor.GROUP_ROWS
                gy1 = y0 + (gr + 1) * fh / MRC550Processor.GROUP_ROWS
                gw, gh = gx1 - gx0, gy1 - gy0
                for sr in range(2):
                    for sc in range(2):
                        rid = sr * 2 + sc + 1
                        cx = gx0 + (sc + 0.5) * gw / 2.0
                        cy = gy0 + (sr + 0.5) * gh / 2.0
                        slots.append((gid, rid, float(cx), float(cy)))
        return slots

    def _assign_labels_by_field_grid(
        self,
        candidates: List[RectInfo],
        bounds: Tuple[float, float, float, float],
        *,
        min_matches: int = 95,
    ) -> List[Dict[str, Any]]:
        """在整体靶标外框内，用 100 个均匀槽位做唯一匹配编号。"""
        inliers = self._filter_candidates_in_field(candidates, bounds)
        if len(inliers) < min_matches:
            raise RuntimeError(f"靶标范围内的矩形不足 {min_matches} 个，当前: {len(inliers)}")

        areas = np.array([float(r["area"]) for r in inliers], dtype=np.float32)
        med = float(np.median(areas))
        band = [r for r in inliers if med * 0.15 <= float(r["area"]) <= med * 3.5]
        pool = band if len(band) >= min_matches else inliers

        if len(pool) > self.TARGET_RECT_COUNT + 15:
            med = float(np.median([float(r["area"]) for r in pool]))
            for r in pool:
                r["quality"] = abs(float(r["area"]) - med)
            pool = sorted(pool, key=lambda x: float(x["quality"]))[: self.TARGET_RECT_COUNT + 10]

        slots = self._build_field_slot_grid(bounds)
        return self._assign_rects_to_grid(pool, slots, min_matches=min_matches)

    def _prepare_field_candidates(
        self,
        mask: np.ndarray,
        image_bgr: Optional[np.ndarray] = None,
    ) -> Tuple[List[RectInfo], Tuple[float, float, float, float]]:
        raw = self._collect_rect_candidates_from_mask(mask, min_area=120.0, max_aspect_ratio=2.6)
        expanded_basic = self._expand_merged_candidates(raw, mask=mask)
        bounds, _row_groups = self._resolve_field_geometry(expanded_basic, image_bgr=image_bgr)
        expanded = self._expand_merged_candidates(raw, mask=mask, field_bounds=bounds)
        inliers = self._filter_candidates_in_field(expanded, bounds)
        return inliers, bounds

    # ---------- 倾角校正 ----------
    def _collect_outer_boundary_points(self, image_bgr: np.ndarray) -> np.ndarray:
        """测倾角：用掩膜轮廓外接点拟合参考框（不依赖编号结果）。"""
        mask = self._build_color_invariant_mask(image_bgr)
        raw = self._expand_merged_candidates(
            self._collect_rect_candidates_from_mask(mask, min_area=120.0, max_aspect_ratio=2.6),
            mask=mask,
        )
        if len(raw) >= 40:
            pts_list: List[List[float]] = []
            for r in raw:
                for xy in r["rotated_box"]:
                    pts_list.append([float(xy[0]), float(xy[1])])
            return np.array(pts_list, dtype=np.float32)

        ys, xs = np.where(mask > 0)
        if len(xs) < 100:
            raise RuntimeError("亮结构太少，无法建立外框参考。")
        points = np.column_stack([xs.astype(np.float32), ys.astype(np.float32)])
        if points.shape[0] > 8000:
            idx = np.linspace(0, points.shape[0] - 1, 8000, dtype=np.int32)
            points = points[idx]
        return points

    @staticmethod
    def _normalize_angle_to_45(angle_deg: float) -> float:
        while angle_deg <= -45.0:
            angle_deg += 90.0
        while angle_deg > 45.0:
            angle_deg -= 90.0
        return angle_deg

    @staticmethod
    def _fit_reference_rectangle(points: np.ndarray) -> Tuple[np.ndarray, float]:
        hull = cv2.convexHull(points.reshape(-1, 1, 2))
        rect = cv2.minAreaRect(hull)
        box = cv2.boxPoints(rect).astype(np.float32)

        best_angle = None
        best_abs = 1e9
        for idx in range(4):
            p1 = box[idx]
            p2 = box[(idx + 1) % 4]
            angle = float(np.degrees(np.arctan2(float(p2[1] - p1[1]), float(p2[0] - p1[0]))))
            angle = MRC550Processor._normalize_angle_to_45(angle)
            if abs(angle) < best_abs:
                best_abs = abs(angle)
                best_angle = angle

        if best_angle is None:
            raise RuntimeError("参考矩形角度计算失败。")
        return np.int32(np.round(box)), float(best_angle)

    def _measure_side_angle(self, image_bgr: np.ndarray) -> float:
        points = self._collect_outer_boundary_points(image_bgr)
        _box, angle_deg = self._fit_reference_rectangle(points)
        return float(angle_deg)

    @staticmethod
    def _estimate_dark_border_value(image: np.ndarray) -> Tuple[int, int, int]:
        h, w = image.shape[:2]
        bw = max(8, w // 30)
        bh = max(8, h // 30)
        strips = [
            image[:bh, :, :].reshape(-1, 3),
            image[-bh:, :, :].reshape(-1, 3),
            image[:, :bw, :].reshape(-1, 3),
            image[:, -bw:, :].reshape(-1, 3),
        ]
        pixels = np.vstack(strips)
        value = np.percentile(pixels, 15, axis=0)
        return tuple(int(np.clip(v, 0, 255)) for v in value)

    @staticmethod
    def _rotate_keep_size_with_pad(
        image: np.ndarray, delta_deg: float, border_value: Tuple[int, int, int], pad_ratio: float = 0.18
    ) -> np.ndarray:
        """扩边后旋转再裁回原尺寸，减轻大角度时边缘裁切丢目标（用于 90° 定向等）。"""
        h, w = image.shape[:2]
        pad = int(round(max(h, w) * pad_ratio))
        padded = cv2.copyMakeBorder(image, pad, pad, pad, pad, cv2.BORDER_CONSTANT, value=border_value)
        ph, pw = padded.shape[:2]
        matrix = cv2.getRotationMatrix2D((pw / 2.0, ph / 2.0), delta_deg, 1.0)
        rotated = cv2.warpAffine(
            padded,
            matrix,
            (pw, ph),
            flags=cv2.INTER_LINEAR,
            borderMode=cv2.BORDER_CONSTANT,
            borderValue=border_value,
        )
        y1 = max(0, (ph - h) // 2)
        x1 = max(0, (pw - w) // 2)
        return rotated[y1 : y1 + h, x1 : x1 + w]

    def _refine_rotation_with_verification(
        self,
        image_bgr: np.ndarray,
        initial_angle_deg: float,
        border_value: Tuple[int, int, int],
        max_steps: int = 3,
        tolerance_deg: float = 0.45,
    ) -> Tuple[np.ndarray, float, float]:
        """550：外框测角；倾角很小时不旋转，避免靶标被裁到画面外。"""
        min_apply = float(self.SIDE_ANGLE_APPLY_MIN_DEG)
        if abs(float(initial_angle_deg)) < min_apply:
            return image_bgr, 0.0, float(initial_angle_deg)

        def rotate_small(img: np.ndarray, delta: float) -> np.ndarray:
            if abs(delta) >= 8.0:
                return self._rotate_keep_size_with_pad(img, delta, border_value, pad_ratio=0.12)
            return self._rotate_keep_size_with_pad(img, delta, border_value, pad_ratio=0.06)

        candidates = []
        for signed_angle in (-float(initial_angle_deg), float(initial_angle_deg)):
            rotated = rotate_small(image_bgr, signed_angle)
            try:
                residual_angle = self._measure_side_angle(rotated)
            except Exception:
                residual_angle = 1e9
            candidates.append((abs(residual_angle), rotated, signed_angle, residual_angle))

        candidates.sort(key=lambda item: item[0])
        best_abs, best_image, applied_rotation_deg, residual = candidates[0]

        steps = 0
        while best_abs > tolerance_deg and steps < max_steps:
            correction = -float(residual)
            if abs(correction) < min_apply:
                break
            updated = rotate_small(best_image, correction)
            try:
                new_residual = self._measure_side_angle(updated)
            except Exception:
                break
            if abs(new_residual) >= best_abs:
                break
            best_image = updated
            applied_rotation_deg += correction
            residual = new_residual
            best_abs = abs(new_residual)
            steps += 1

        if abs(applied_rotation_deg) < min_apply:
            return image_bgr, 0.0, float(initial_angle_deg)
        return best_image, float(applied_rotation_deg), float(residual)

    # ---------- 矩形检测与编号 ----------
    @staticmethod
    def _contour_to_rect_info(cnt: np.ndarray) -> Optional[RectInfo]:
        area = float(cv2.contourArea(cnt))
        if area < 1.0:
            return None
        rect = cv2.minAreaRect(cnt)
        rw, rh = rect[1]
        if rw < 1.0 or rh < 1.0:
            return None
        aspect = max(rw, rh) / min(rw, rh)
        x, y, w, h = cv2.boundingRect(cnt)
        box = cv2.boxPoints(rect)
        return {
            "bbox": (int(x), int(y), int(w), int(h)),
            "center": (float(x + w / 2.0), float(y + h / 2.0)),
            "area": area,
            "aspect": float(aspect),
            "rotated_box": np.int32(np.round(box)).tolist(),
        }

    def _collect_rect_candidates_from_mask(
        self,
        mask: np.ndarray,
        *,
        min_area: float,
        max_aspect_ratio: float,
    ) -> List[RectInfo]:
        contours, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
        rects: List[RectInfo] = []
        for cnt in contours:
            info = self._contour_to_rect_info(cnt)
            if info is None:
                continue
            if float(info["area"]) < float(min_area):
                continue
            if float(info["aspect"]) > float(max_aspect_ratio):
                continue
            rects.append(info)
        return rects

    @staticmethod
    def _build_color_invariant_mask(image_bgr: np.ndarray) -> np.ndarray:
        lab = cv2.cvtColor(image_bgr, cv2.COLOR_BGR2LAB)
        a = lab[:, :, 1].astype(np.float32) - 128.0
        b = lab[:, :, 2].astype(np.float32) - 128.0
        chroma = np.sqrt(a * a + b * b)
        chroma = cv2.GaussianBlur(chroma, (3, 3), 0)
        chroma_u8 = (255 * (chroma / (np.max(chroma) + 1e-6))).astype(np.uint8)
        _, mask_chroma = cv2.threshold(chroma_u8, 0, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)

        gray = cv2.cvtColor(image_bgr, cv2.COLOR_BGR2GRAY)
        clahe = cv2.createCLAHE(clipLimit=2.0, tileGridSize=(8, 8))
        gray_enh = clahe.apply(gray)
        tophat = cv2.morphologyEx(
            gray_enh,
            cv2.MORPH_TOPHAT,
            cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (21, 21)),
        )
        mask_gray = cv2.adaptiveThreshold(
            tophat,
            255,
            cv2.ADAPTIVE_THRESH_GAUSSIAN_C,
            cv2.THRESH_BINARY,
            31,
            -2,
        )

        mask = mask_chroma if int(np.count_nonzero(mask_chroma)) >= 3000 else cv2.bitwise_or(mask_chroma, mask_gray)
        open_k = int(MRC550Processor.MASK_OPEN_SIZE)
        if open_k > 0:
            mask = cv2.morphologyEx(
                mask,
                cv2.MORPH_OPEN,
                cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (open_k, open_k)),
                iterations=1,
            )
        return cv2.morphologyEx(
            mask,
            cv2.MORPH_CLOSE,
            cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (3, 3)),
            iterations=1,
        )

    def _detect_rectangles(self, image_bgr: np.ndarray, mask: np.ndarray) -> List[RectInfo]:
        inliers, _bounds = self._prepare_field_candidates(mask, image_bgr=image_bgr)
        return inliers

    def _assign_labels_5x5_grid(
        self,
        rects: List[RectInfo],
        image_bgr: Optional[np.ndarray] = None,
    ) -> List[Dict[str, Any]]:
        """550：先估计 100 矩形整体外框，再在外框内均匀分 100 格编号。"""
        # rects 已由 _prepare_field_candidates 完成粘连拆分，勿二次 expand（会误切单格导致框偏右）
        bounds, _row_groups = self._resolve_field_geometry(list(rects), image_bgr=image_bgr)
        return self._assign_labels_by_field_grid(list(rects), bounds, min_matches=self.MIN_RECTS_FOR_LABEL)

    def _compute_stripe_mismatch_score(
        self,
        work_bgr: np.ndarray,
        profile_roi_pad: int = 2,
    ) -> Tuple[int, List[Dict[str, Any]]]:
        expected = self._default_expected_stripe_counts()
        mask = self._build_color_invariant_mask(work_bgr)
        rects = self._detect_rectangles(work_bgr, mask)
        mapped = self._assign_labels_5x5_grid(rects, image_bgr=work_bgr)
        mismatch = 0
        for m in mapped:
            if int(m.get("rect_id", -1)) != 1:
                continue
            gid = int(m["group_id"])
            _, metrics = self._profile_for_rect1(
                work_bgr, mask, m, profile_roi_pad, expected_stripes=expected.get(gid)
            )
            stripe_n = int(len(metrics["peaks"]) + len(metrics["valleys"]))
            mismatch += abs(stripe_n - int(expected.get(gid, stripe_n)))
        return int(mismatch), mapped

    def _draw_corner_profile_debug(
        self,
        image_bgr: np.ndarray,
        mapped: List[Dict[str, Any]],
        profile_roi_pad: int = 2,
        mask: Optional[np.ndarray] = None,
    ) -> Tuple[Dict[str, int], np.ndarray]:
        """在 1-1 / 5-1 / 21-1 / 25-1 上绘制四角剖面条纹数（输入已摆正时使用）。"""
        if mask is None:
            mask = self._build_color_invariant_mask(image_bgr)
        debug = image_bgr.copy()
        counts: Dict[str, int] = {}
        corner_rects = self._pick_corner_profile_rects(mapped)
        expected = self._default_expected_stripe_counts()
        for name, m in corner_rects.items():
            roi_xyxy, metrics = self._profile_for_rect1(
                image_bgr, mask, m, profile_roi_pad, expected_stripes=expected.get(int(m["group_id"]))
            )
            x1, y1, x2, y2 = roi_xyxy
            stripe_n = int(len(metrics["peaks"]) + len(metrics["valleys"]))
            counts[name] = stripe_n
            color = (0, 255, 0) if name == "TL" else (255, 0, 0)
            cv2.rectangle(debug, (x1, y1), (x2, y2), color, 2)
            cv2.putText(
                debug,
                f"{m['label']}:{stripe_n}",
                (x1, max(12, y1 - 4)),
                cv2.FONT_HERSHEY_SIMPLEX,
                0.45,
                color,
                1,
                cv2.LINE_AA,
            )
        return counts, debug

    def _apply_orientation_550(
        self,
        aligned_bgr: np.ndarray,
        border_value: Tuple[int, int, int],
        profile_roi_pad: int = 2,
    ) -> Tuple[np.ndarray, int, Dict[str, int], np.ndarray]:
        """0/90/180/270 中选条纹数最接近预期表的方向；原图已摆正时优先 0°。"""
        best_mismatch = 10**9
        rot0_mismatch = 10**9
        best_image = aligned_bgr
        best_rot = 0
        best_counts: Dict[str, int] = {}
        best_debug = aligned_bgr.copy()

        for rot_i in range(4):
            work = aligned_bgr if rot_i == 0 else self._rotate_keep_size_with_pad(
                aligned_bgr, 90.0 * rot_i, border_value
            )
            try:
                mismatch, mapped = self._compute_stripe_mismatch_score(work, profile_roi_pad=profile_roi_pad)
            except Exception:
                continue
            if rot_i == 0:
                rot0_mismatch = int(mismatch)
            if int(mismatch) >= best_mismatch:
                continue
            try:
                work_mask = self._build_color_invariant_mask(work)
                counts, debug = self._draw_corner_profile_debug(
                    work, mapped, profile_roi_pad=profile_roi_pad, mask=work_mask
                )
            except RuntimeError:
                continue
            best_mismatch = int(mismatch)
            best_rot = rot_i
            best_image = work
            best_counts = counts
            best_debug = debug

        if best_mismatch >= 10**9:
            raise RuntimeError("550 条纹一致性定向失败：四个方向均无法完成编号。")

        margin = int(self.ORIENT_PREFER_ZERO_MISMATCH_MARGIN)
        if best_rot != 0 and rot0_mismatch <= best_mismatch + margin:
            best_rot = 0
            best_image = aligned_bgr
            try:
                _, mapped0 = self._compute_stripe_mismatch_score(aligned_bgr, profile_roi_pad=profile_roi_pad)
                aligned_mask = self._build_color_invariant_mask(aligned_bgr)
                best_counts, best_debug = self._draw_corner_profile_debug(
                    aligned_bgr, mapped0, profile_roi_pad=profile_roi_pad, mask=aligned_mask
                )
            except Exception:
                pass

        return best_image, best_rot, best_counts, best_debug

    def _draw_field_overlay(
        self,
        image_bgr: np.ndarray,
        candidates: List[RectInfo],
    ) -> np.ndarray:
        out = image_bgr.copy()
        try:
            mask = self._build_color_invariant_mask(image_bgr)
            expanded_basic = self._expand_merged_candidates(candidates, mask=mask)
            bounds, _ = self._resolve_field_geometry(expanded_basic, image_bgr=image_bgr)
            x0, y0, x1, y1 = (int(v) for v in bounds)
            cv2.rectangle(out, (x0, y0), (x1, y1), (255, 255, 0), 2)
            for lx0, ly, lx1, _ly in self._field_row_extension_lines():
                cv2.line(out, (int(lx0), int(ly)), (int(lx1), int(ly)), (0, 255, 255), 1, cv2.LINE_AA)
            cv2.putText(
                out,
                "100-rect field",
                (x0 + 4, max(20, y0 - 6)),
                cv2.FONT_HERSHEY_SIMPLEX,
                0.55,
                (255, 255, 0),
                1,
                cv2.LINE_AA,
            )
        except Exception:
            pass
        return out

    @staticmethod
    def _draw_results_mapped(
        image_bgr: np.ndarray,
        mapped_rects: List[Dict[str, Any]],
        profile_rois: Optional[Dict[str, Tuple[int, int, int, int]]] = None,
    ) -> np.ndarray:
        out = image_bgr.copy()
        prof = profile_rois or {}
        for m in mapped_rects:
            box = np.array(m["rotated_box"], dtype=np.int32).reshape((-1, 1, 2))
            cv2.polylines(out, [box], isClosed=True, color=(0, 0, 255), thickness=2)
            label = str(m["label"])
            if label in prof:
                x1, y1, x2, y2 = prof[label]
                cv2.rectangle(out, (x1, y1), (x2, y2), (255, 255, 0), 1, cv2.LINE_AA)
            box_pts = np.array(m["rotated_box"], dtype=np.int32)
            x = int(np.min(box_pts[:, 0]))
            y = int(np.min(box_pts[:, 1]))
            cv2.putText(
                out,
                label,
                (x, max(18, y - 4)),
                cv2.FONT_HERSHEY_SIMPLEX,
                0.45,
                (0, 0, 255),
                1,
                cv2.LINE_AA,
            )
        return out

    @staticmethod
    def _build_overview_panel(original: np.ndarray, aligned: np.ndarray, mask: np.ndarray, final: np.ndarray) -> np.ndarray:
        h, w = original.shape[:2]
        half_w, half_h = w // 2, h // 2
        p1 = cv2.resize(original, (half_w, half_h), interpolation=cv2.INTER_AREA)
        p2 = cv2.resize(aligned, (half_w, half_h), interpolation=cv2.INTER_AREA)
        p3 = cv2.cvtColor(mask, cv2.COLOR_GRAY2BGR)
        p3 = cv2.resize(p3, (half_w, half_h), interpolation=cv2.INTER_NEAREST)
        p4 = cv2.resize(final, (half_w, half_h), interpolation=cv2.INTER_AREA)
        cv2.putText(p1, "Original", (12, 28), cv2.FONT_HERSHEY_SIMPLEX, 0.8, (0, 255, 255), 2, cv2.LINE_AA)
        cv2.putText(p2, "Aligned", (12, 28), cv2.FONT_HERSHEY_SIMPLEX, 0.8, (0, 255, 255), 2, cv2.LINE_AA)
        cv2.putText(p3, "Rectangle Mask", (12, 28), cv2.FONT_HERSHEY_SIMPLEX, 0.8, (0, 255, 255), 2, cv2.LINE_AA)
        cv2.putText(p4, "Labeled 5x5 Groups", (12, 28), cv2.FONT_HERSHEY_SIMPLEX, 0.8, (0, 255, 255), 2, cv2.LINE_AA)
        return np.vstack([np.hstack([p1, p2]), np.hstack([p3, p4])])

    # ---------- 剖面分析与 MRC 判定 ----------
    @staticmethod
    def _empty_profile_metrics() -> Dict[str, Any]:
        return {
            "line_raw": np.array([], dtype=np.float32),
            "row_idx": -1,
            "peaks": [],
            "valleys": [],
            "pair_n": 0,
            "c_mean": 0.0,
            "peak_vals_raw": [],
            "valley_vals_raw": [],
        }

    @staticmethod
    def _smooth_profile_line(line_raw: np.ndarray, sigma: float) -> np.ndarray:
        if sigma <= 0 or line_raw.size < 8:
            return line_raw.astype(np.float32)
        k = int(max(3, round(float(sigma) * 6))) | 1
        return cv2.GaussianBlur(line_raw.reshape(1, -1), (k, 1), float(sigma))[0]

    @staticmethod
    def _diff_peaks_valleys(line: np.ndarray) -> Tuple[List[int], List[int]]:
        d = np.diff(line)
        peaks: List[int] = []
        valleys: List[int] = []
        for i in range(1, len(d)):
            if d[i - 1] > 0 and d[i] <= 0:
                peaks.append(i)
            if d[i - 1] < 0 and d[i] >= 0:
                valleys.append(i)
        return peaks, valleys

    @classmethod
    def _detect_peaks_valleys(
        cls,
        line: np.ndarray,
        group_id: Optional[int] = None,
        expected_stripes: Optional[int] = None,
    ) -> Tuple[List[int], List[int]]:
        """统一用差分法检测峰谷（25 组一致处理）。"""
        return cls._diff_peaks_valleys(line)

    @classmethod
    def _pair_michelson_c_mean(
        cls,
        peak_vals: List[float],
        valley_vals: List[float],
    ) -> Tuple[int, float, List[float], List[float]]:
        pair_n = min(len(peak_vals), len(valley_vals))
        c_vals: List[float] = []
        for i in range(pair_n):
            p, v = peak_vals[i], valley_vals[i]
            den = p + v
            if den > 1e-6 and p > v:
                c_vals.append((p - v) / den)
        c_mean = float(np.mean(np.array(c_vals, dtype=np.float32))) if c_vals else 0.0
        return pair_n, c_mean, peak_vals, valley_vals

    @classmethod
    def _extract_profile_metrics(
        cls,
        roi_color: np.ndarray,
        group_id: Optional[int] = None,
        expected_stripes: Optional[int] = None,
    ) -> Dict[str, Any]:
        """最大方差行 → σ=1.5 高斯平滑 → 差分峰谷检测 → C_mean（25 组统一处理）。"""
        if roi_color.size == 0 or roi_color.shape[1] < 8:
            return cls._empty_profile_metrics()

        roi_gray = cv2.cvtColor(roi_color, cv2.COLOR_BGR2GRAY).astype(np.float32)
        row_idx = int(np.argmax(np.std(roi_gray, axis=1)))
        line_raw = roi_gray[row_idx, :].astype(np.float32)

        line_smooth = cls._smooth_profile_line(line_raw, float(cls.PROFILE_LINE_SMOOTH_SIGMA))
        peaks, valleys = cls._detect_peaks_valleys(line_smooth, group_id, expected_stripes)
        peak_vals = [float(line_smooth[i]) for i in peaks]
        valley_vals = [float(line_smooth[i]) for i in valleys]
        pair_n, c_mean, peak_vals, valley_vals = cls._pair_michelson_c_mean(peak_vals, valley_vals)

        return {
            "line_raw": line_smooth,
            "row_idx": row_idx,
            "peaks": peaks,
            "valleys": valleys,
            "pair_n": pair_n,
            "c_mean": c_mean,
            "peak_vals_raw": peak_vals,
            "valley_vals_raw": valley_vals,
        }

    def _profile_for_rect1(
        self,
        image_bgr: np.ndarray,
        mask: np.ndarray,
        mapped: Dict[str, Any],
        profile_roi_pad: int,
        expected_stripes: Optional[int] = None,
    ) -> Tuple[Tuple[int, int, int, int], Dict[str, Any]]:
        """统一：rect1 剖面 ROI + metrics。"""
        h, w = image_bgr.shape[:2]
        gid = int(mapped["group_id"])
        roi_xyxy = self._profile_roi_xyxy(
            image_bgr, mask, mapped["bbox"], profile_roi_pad, w, h, group_id=gid
        )
        x1, y1, x2, y2 = roi_xyxy
        exp = expected_stripes if expected_stripes is not None else self._default_expected_stripe_counts().get(gid)
        metrics = self._extract_profile_metrics(
            image_bgr[y1:y2, x1:x2], group_id=gid, expected_stripes=exp
        )
        return roi_xyxy, metrics

    @staticmethod
    def _save_group_results_excel(rows: List[Dict[str, Any]], excel_path: str) -> None:
        wb = openpyxl.Workbook()
        ws = wb.active
        ws.title = "group_rect1_results"
        ws.append(
            [
                "group_id",
                "label",
                "peak_gray_values",
                "valley_gray_values",
                "valid_pair_count",
                "stripe_count_n",
                "expected_stripe_count_n",
                "is_abnormal",
                "result_c_mean",
            ]
        )
        for row in rows:
            ws.append(
                [
                    int(row["group_id"]),
                    str(row["label"]),
                    ",".join(f"{float(v):.3f}" for v in row["peak_vals_raw"]),
                    ",".join(f"{float(v):.3f}" for v in row["valley_vals_raw"]),
                    int(row["pair_n"]),
                    int(row["stripe_n"]),
                    int(row["expected_stripe_n"]),
                    int(row["is_abnormal"]),
                    float(row["c_mean"]),
                ]
            )
        for col_cells in ws.columns:
            max_len = 0
            letter = col_cells[0].column_letter
            for cell in col_cells:
                max_len = max(max_len, len("" if cell.value is None else str(cell.value)))
            ws.column_dimensions[letter].width = min(max(12, max_len + 2), 80)
        wb.save(excel_path)

    @staticmethod
    def _save_group_curve_plot(rows: List[Dict[str, Any]], target: float, plot_path: str) -> None:
        if not rows:
            raise RuntimeError("无可用于绘图的组数据。")
        normal_rows = [r for r in rows if int(r.get("is_abnormal", 0)) == 0]
        abnormal_rows = [r for r in rows if int(r.get("is_abnormal", 0)) == 1]
        group_ids = [int(r["group_id"]) for r in rows]
        c_means = [float(r["c_mean"]) for r in rows]
        fig, ax = plt.subplots(figsize=(11, 5))
        ax.plot(group_ids, c_means, lw=1.2, color="#1f77b4", alpha=0.9, label="Rect1 result")
        if normal_rows:
            ax.scatter(
                [int(r["group_id"]) for r in normal_rows],
                [float(r["c_mean"]) for r in normal_rows],
                s=42,
                c="#1f77b4",
                marker="o",
                edgecolors="#1f77b4",
                linewidths=1.0,
                zorder=3,
                label="Normal",
            )
        if abnormal_rows:
            ax.scatter(
                [int(r["group_id"]) for r in abnormal_rows],
                [float(r["c_mean"]) for r in abnormal_rows],
                s=62,
                facecolors="none",
                edgecolors="#d62728",
                marker="o",
                linewidths=2.0,
                zorder=4,
                label="Abnormal (hollow)",
            )
        ax.axhline(target, color="#ff0000", lw=2.8, ls="--", label=f"Standard={target:.3f}")
        ax.fill_between(group_ids, target - 0.005, target + 0.005, color="#ff0000", alpha=0.10)
        ax.text(
            group_ids[-1],
            target + 0.006,
            f"  Standard line: {target:.3f}",
            color="#ff0000",
            fontsize=10,
            fontweight="bold",
            ha="right",
            va="bottom",
        )
        ax.set_xlabel("Group ID")
        ax.set_ylabel("Result value (C_mean)")
        ax.set_title("Group Rect1 Result Curve (with 0.03 standard)")
        ax.grid(True, alpha=0.3)
        ax.set_xticks(group_ids)
        ax.legend(loc="best", fontsize=9)
        plt.tight_layout()
        plt.savefig(plot_path, dpi=150, bbox_inches="tight")
        plt.close(fig)

    @staticmethod
    def _default_expected_stripe_counts() -> Dict[int, int]:
        # 550 图案：每组 rect1 预期明暗条纹总数（波峰+波谷）
        return {
            1: 7, 2: 7, 3: 7,
            4: 9, 5: 9, 6: 9,
            7: 11, 8: 11, 9: 11,
            10: 13, 11: 13,
            12: 15, 13: 15,
            14: 17, 15: 17,
            16: 19,
            17: 21, 18: 21,
            19: 23,
            20: 25,
            21: 27, 22: 27,
            23: 29,
            24: 31, 25: 31,
        }

    @staticmethod
    def _parse_expected_pairs(spec: str) -> Dict[int, int]:
        expected: Dict[int, int] = {}
        if not spec.strip():
            return expected
        for token in spec.split(","):
            item = token.strip()
            if not item:
                continue
            if ":" not in item:
                raise ValueError(f"expected-pairs格式错误: {item}")
            k, v = item.split(":", 1)
            gid = int(k.strip())
            cnt = int(v.strip())
            expected[gid] = cnt
        return expected

    @staticmethod
    def _bbox_pad_xyxy(
        bbox: Tuple[int, int, int, int],
        right_pad: int,
        img_w: int,
        img_h: int,
    ) -> Tuple[int, int, int, int]:
        x, y, w, h = (int(v) for v in bbox)
        pad = max(0, int(right_pad))
        return max(0, x), max(0, y), min(img_w, x + w + pad), min(img_h, y + h)

    @classmethod
    def _profile_roi_xyxy(
        cls,
        image_bgr: np.ndarray,
        mask: np.ndarray,
        bbox: Tuple[int, int, int, int],
        right_pad: int,
        img_w: int,
        img_h: int,
        group_id: Optional[int] = None,
    ) -> Tuple[int, int, int, int]:
        """
        剖面 ROI：在 bbox 附近按亮区掩膜裁出条纹 span，去掉左右黑边；
        不扩大到均匀槽位，避免把背景算进剖面。
        """
        x, y, w, h = (int(v) for v in bbox)
        pad = max(0, int(right_pad))
        y1, y2 = max(0, y), min(img_h, y + h)
        if y2 <= y1 or w < 4:
            return cls._bbox_pad_xyxy(bbox, right_pad, img_w, img_h)

        if group_id is not None and int(group_id) == 1:
            search_l = max(
                int(cls.PROFILE_G1_SEARCH_L_MIN),
                min(int(w * float(cls.PROFILE_G1_SEARCH_L_RATIO)), int(cls.PROFILE_G1_SEARCH_L_MAX)),
            )
        else:
            search_l = max(2, min(int(w * float(cls.PROFILE_ROI_SEARCH_L_RATIO)), int(cls.PROFILE_ROI_SEARCH_L_MAX)))
        x_search1 = max(0, x - search_l)
        x_search2 = min(img_w, x + w + pad)
        roi_mask = mask[y1:y2, x_search1:x_search2]
        if roi_mask.size == 0:
            return cls._bbox_pad_xyxy(bbox, right_pad, img_w, img_h)

        col_occ = np.count_nonzero(roi_mask, axis=0).astype(np.float32)
        mx = float(col_occ.max())
        if mx < 1.0:
            return cls._bbox_pad_xyxy(bbox, right_pad, img_w, img_h)

        active = col_occ >= max(1.0, mx * 0.12)
        idx = np.where(active)[0]
        if idx.size < 4:
            return cls._bbox_pad_xyxy(bbox, right_pad, img_w, img_h)

        left_i = int(idx[0])
        right_i = int(idx[-1]) + 1
        x1 = x_search1 + left_i
        x2 = min(img_w, x_search1 + right_i + pad)
        return x1, y1, x2, y2

    @staticmethod
    def _front_groups_all_normal(rows: List[Dict[str, Any]], max_group: int) -> bool:
        for row in rows:
            gid = int(row["group_id"])
            if gid > int(max_group):
                continue
            if int(row.get("is_abnormal", 1)) != 0:
                return False
        return True

    @staticmethod
    def _front_abnormal_group_ids(rows: List[Dict[str, Any]], max_group: int) -> List[int]:
        bad: List[int] = []
        for row in rows:
            gid = int(row["group_id"])
            if gid > int(max_group):
                continue
            if int(row.get("is_abnormal", 0)) != 0:
                bad.append(gid)
        return bad

    @staticmethod
    def _attach_abnormal_flags(
        rows: List[Dict[str, Any]],
        expected_pairs: Dict[int, int],
        pair_tolerance: int,
        c_mean_threshold: float = 0.03,
    ) -> None:
        for row in rows:
            gid = int(row["group_id"])
            expected = int(expected_pairs.get(gid, int(row["stripe_n"])))
            stripe_n = int(row["stripe_n"])
            c_mean = float(row.get("c_mean", 0.0))
            row["expected_stripe_n"] = expected
            stripe_bad = int(abs(stripe_n - expected) > max(0, int(pair_tolerance)))
            contrast_bad = int(c_mean < float(c_mean_threshold))
            row["is_abnormal"] = int(stripe_bad or contrast_bad)

    @staticmethod
    def _pick_min_resolvable_group(rows: List[Dict[str, Any]]) -> Tuple[Optional[int], Optional[float]]:
        """1. 前19组全正常→取全部组最大无异常号; 2. 否则→取前19组最大无异常号"""
        front_max = 19
        rows_sorted = sorted(rows, key=lambda x: int(x.get("group_id", 0)))

        # 检查前 19 组是否全部正常
        front_all_normal = all(
            int(r.get("is_abnormal", 1)) == 0
            for r in rows_sorted
            if int(r.get("group_id", 0)) <= front_max
        )

        if front_all_normal:
            # 全部组（1-25）中最大的无异常组
            valid = [r for r in rows_sorted if int(r.get("is_abnormal", 1)) == 0]
        else:
            # 仅前 19 组中最大的无异常组
            valid = [
                r for r in rows_sorted
                if int(r.get("group_id", 0)) <= front_max and int(r.get("is_abnormal", 1)) == 0
            ]

        if not valid:
            return None, None
        best = max(valid, key=lambda x: int(x.get("group_id", 0)))
        return int(best["group_id"]), float(best.get("c_mean", 0.0))

    def process_image(
        self,
        image_path: str,
        output_dir: str,
        target_value: float = 0.03,
        expected_pairs_spec: str = "",
        pair_tolerance: int = 0,
        profile_roi_pad: int = 2,
        enable_corner_orientation: bool = False,
    ) -> Dict[str, Any]:
        image = self._imread_unicode(image_path)
        if image is None:
            raise FileNotFoundError(f"无法读取图像: {image_path}")

        angle_deg = self._measure_side_angle(image)
        border_value = self._estimate_dark_border_value(image)
        aligned, applied_rotation_deg, residual_angle = self._refine_rotation_with_verification(
            image, angle_deg, border_value
        )
        # 550 不做矩形网格二次微旋转
        work_bgr = aligned
        corner_rot_90_count = 0
        if enable_corner_orientation:
            work_bgr, corner_rot_90_count, _, _ = self._apply_orientation_550(
                aligned, border_value, profile_roi_pad=profile_roi_pad
            )

        rect_mask = self._build_color_invariant_mask(work_bgr)
        rect_candidates = self._detect_rectangles(work_bgr, rect_mask)
        mapped_rects = self._assign_labels_5x5_grid(rect_candidates, image_bgr=work_bgr)
        corner_counts, corner_debug = self._draw_corner_profile_debug(
            work_bgr, mapped_rects, profile_roi_pad=profile_roi_pad, mask=rect_mask
        )
        expected_pairs = self._parse_expected_pairs(expected_pairs_spec) or self._default_expected_stripe_counts()
        profile_rois: Dict[str, Tuple[int, int, int, int]] = {}
        group_rect1_rows: List[Dict[str, Any]] = []
        for m in sorted([x for x in mapped_rects if int(x["rect_id"]) == 1], key=lambda x: int(x["group_id"])):
            gid = int(m["group_id"])
            roi_xyxy, metrics = self._profile_for_rect1(
                work_bgr, rect_mask, m, profile_roi_pad, expected_stripes=expected_pairs.get(gid)
            )
            profile_rois[str(m["label"])] = roi_xyxy
            group_rect1_rows.append(
                {
                    "group_id": gid,
                    "label": str(m["label"]),
                    "peak_vals_raw": list(metrics["peak_vals_raw"]),
                    "valley_vals_raw": list(metrics["valley_vals_raw"]),
                    "pair_n": int(metrics["pair_n"]),
                    "stripe_n": int(len(metrics["peaks"]) + len(metrics["valleys"])),
                    "c_mean": float(metrics["c_mean"]),
                }
            )

        rect_candidate_count = len(mapped_rects)
        final_img = self._draw_field_overlay(
            self._draw_results_mapped(work_bgr, mapped_rects, profile_rois=profile_rois),
            rect_candidates,
        )
        overview = self._build_overview_panel(image, aligned, rect_mask, final_img)

        os.makedirs(output_dir, exist_ok=True)
        stem = os.path.splitext(os.path.basename(image_path))[0]
        ext = os.path.splitext(image_path)[1] or ".png"
        out_aligned = os.path.join(output_dir, f"{stem}_a{ext}")
        out_output = os.path.join(output_dir, f"{stem}_labels{ext}")
        out_overview = os.path.join(output_dir, f"{stem}_ov{ext}")
        out_group_excel = os.path.join(output_dir, f"{stem}_res.xlsx")
        out_group_curve = os.path.join(output_dir, f"{stem}_curve.png")
        out_corner_debug = os.path.join(output_dir, f"{stem}_corner_debug{ext}")

        for p, img in [
            (out_aligned, aligned),
            (out_output, final_img),
            (out_overview, overview),
            (out_corner_debug, corner_debug),
        ]:
            if not self._imwrite_unicode(p, img):
                raise IOError(f"保存失败: {p}")

        self._attach_abnormal_flags(group_rect1_rows, expected_pairs=expected_pairs, pair_tolerance=pair_tolerance, c_mean_threshold=target_value)
        front_ok = self._front_groups_all_normal(group_rect1_rows, self.FRONT_GROUP_TRUST_MAX)
        front_bad = self._front_abnormal_group_ids(group_rect1_rows, self.FRONT_GROUP_TRUST_MAX)
        self._save_group_results_excel(group_rect1_rows, out_group_excel)
        self._save_group_curve_plot(group_rect1_rows, target_value, out_group_curve)
        min_group_id, min_group_c_mean = self._pick_min_resolvable_group(group_rect1_rows)

        out_summary_json = os.path.join(output_dir, f"{stem}_summary.json")
        summary = {
            "image_name": os.path.basename(image_path),
            "min_resolvable_group_id": min_group_id,
            "min_resolvable_c_mean": min_group_c_mean,
            "threshold": float(target_value),
            "valid_rule": "前19组全正常→取全部组最大无异常号; 否则→取前19组最大无异常号",
            "front_groups_valid": bool(front_ok),
            "front_group_trust_max": int(self.FRONT_GROUP_TRUST_MAX),
            "front_abnormal_group_ids": front_bad,
        }
        with open(out_summary_json, "w", encoding="utf-8") as f:
            json.dump(summary, f, ensure_ascii=False, indent=2)

        out: Dict[str, Any] = {
            "candidate_count": rect_candidate_count,
            "mapped_count": len(mapped_rects),
            "side_angle_deg": angle_deg,
            "applied_rotation_deg": applied_rotation_deg,
            "residual_angle_deg": residual_angle,
            "corner_orientation_enabled": bool(enable_corner_orientation),
            "corner_rot_90_count": int(corner_rot_90_count),
            "corner_stripe_counts": dict(corner_counts) if corner_counts else {},
            "mapped_rects": mapped_rects,
            "aligned_path": out_aligned,
            "output_path": out_output,
            "overview_path": out_overview,
            "group_excel_path": out_group_excel,
            "group_curve_path": out_group_curve,
            "summary_json_path": out_summary_json,
            "min_resolvable_group_id": min_group_id,
            "min_resolvable_c_mean": min_group_c_mean,
            "front_groups_valid": bool(front_ok),
            "front_abnormal_group_ids": front_bad,
            "corner_debug_path": out_corner_debug,
        }
        return out


def collect_images(folder: str) -> List[str]:
    exts = ["*.bmp", "*.jpg", "*.jpeg", "*.png", "*.tif", "*.tiff"]
    paths: List[str] = []
    for ext in exts:
        paths.extend(glob.glob(os.path.join(folder, ext)))
    filtered = []
    derived_suffixes = ("_a", "_labels", "_ov", "_curve", "_corner_debug")
    for p in paths:
        stem = os.path.splitext(os.path.basename(p))[0].lower()
        if stem.endswith(derived_suffixes):
            continue
        filtered.append(p)
    return sorted(list(set(filtered)))


def pick_batch_min_resolvable_group(group_ids: List[int]) -> Optional[int]:
    """批量判定：出现次数最多的组号；并列时取较小组号（向下兼容）。"""
    if not group_ids:
        return None
    counts: Dict[int, int] = {}
    for gid in group_ids:
        counts[gid] = counts.get(gid, 0) + 1
    max_count = max(counts.values())
    tied = [gid for gid, cnt in counts.items() if cnt == max_count]
    return min(tied)


def _nice_y_tick_step(max_count: int) -> int:
    """根据最大频次自动选择整数纵轴刻度步长（1/2/5/10/20…）。"""
    if max_count <= 0:
        return 1
    if max_count <= 5:
        return 1
    if max_count <= 12:
        return 2
    if max_count <= 30:
        return 5
    if max_count <= 60:
        return 10
    if max_count <= 150:
        return 20
    if max_count <= 300:
        return 50

    target_ticks = 6
    raw = max_count / target_ticks
    exp = 10 ** math.floor(math.log10(raw))
    fraction = raw / exp
    if fraction <= 1:
        nice = 1
    elif fraction <= 2:
        nice = 2
    elif fraction <= 5:
        nice = 5
    else:
        nice = 10
    return max(1, int(nice * exp))


def _build_integer_y_axis(max_count: int) -> Tuple[int, List[int]]:
    """构建覆盖峰值且刻度均为整数的纵轴范围。"""
    step = _nice_y_tick_step(max_count)
    y_top = int(math.ceil(max_count / step) * step)
    if y_top <= max_count:
        y_top += step
    ticks = list(range(0, y_top + 1, step))
    return y_top, ticks


def _smooth_distribution_curve(
    groups: List[int],
    values: List[float],
    num_points: int = 400,
) -> Tuple[np.ndarray, np.ndarray]:
    """在离散组号之间插值，生成平滑的频率分布曲线。"""
    x_arr = np.asarray(groups, dtype=float)
    y_arr = np.asarray(values, dtype=float)

    if len(x_arr) == 1:
        pad = 0.6
        x_smooth = np.linspace(x_arr[0] - pad, x_arr[0] + pad, num_points)
        y_smooth = np.full_like(x_smooth, y_arr[0], dtype=float)
        return x_smooth, y_smooth

    x_min, x_max = float(x_arr.min()), float(x_arr.max())
    span = max(x_max - x_min, 1.0)
    x_pad_min = x_min - span * 0.04
    x_pad_max = x_max + span * 0.04
    x_dense = np.linspace(x_pad_min, x_pad_max, num_points)

    try:
        from scipy.interpolate import PchipInterpolator

        interpolator = PchipInterpolator(x_arr, y_arr, extrapolate=False)
        y_dense = interpolator(x_dense)
        y_dense = np.clip(y_dense, 0.0, None)
        y_dense = np.nan_to_num(y_dense, nan=0.0)
    except Exception:
        y_dense = np.interp(x_dense, x_arr, y_arr)
        y_dense = np.clip(y_dense, 0.0, None)

    return x_dense, y_dense


def _ensure_plot_fonts() -> None:
    plt.rcParams["font.sans-serif"] = ["Microsoft YaHei", "SimHei", "SimSun", "Noto Sans CJK SC", "DejaVu Sans"]
    plt.rcParams["axes.unicode_minus"] = False


def save_batch_group_distribution_plot(
    group_ids: List[int],
    plot_path: str,
    batch_group_id: Optional[int] = None,
) -> Optional[int]:
    """绘制批量最小可分辨组号频率分布曲线图。"""
    if not group_ids:
        raise RuntimeError("没有可用于绘制分布图的有效组号。")

    _ensure_plot_fonts()

    counts: Dict[int, int] = {}
    for gid in group_ids:
        counts[gid] = counts.get(gid, 0) + 1

    if batch_group_id is None:
        batch_group_id = pick_batch_min_resolvable_group(group_ids)

    groups = sorted(counts.keys())
    values = [float(counts[g]) for g in groups]
    total = len(group_ids)
    max_count = int(max(values))
    y_top, y_ticks = _build_integer_y_axis(max_count)

    x_smooth, y_smooth = _smooth_distribution_curve(groups, values)

    fig, ax = plt.subplots(figsize=(10.5, 5.8))
    fig.patch.set_facecolor("#fafafa")
    ax.set_facecolor("#ffffff")

    ax.fill_between(x_smooth, y_smooth, 0, color="#4a90d9", alpha=0.22, zorder=1)
    ax.plot(
        x_smooth,
        y_smooth,
        color="#1f5a99",
        lw=2.4,
        alpha=0.95,
        zorder=2,
        label="频率分布曲线",
    )
    ax.scatter(
        groups,
        values,
        s=72,
        c="#ffffff",
        edgecolors="#1f5a99",
        linewidths=2.0,
        zorder=4,
        label="实测频数",
    )

    peak_group = batch_group_id if batch_group_id is not None else groups[int(np.argmax(values))]
    peak_count = counts.get(peak_group, max_count)

    if batch_group_id is not None and batch_group_id in counts:
        ax.axvline(
            batch_group_id,
            color="#c0392b",
            lw=1.8,
            ls="--",
            alpha=0.85,
            zorder=3,
            label=f"判定组号 {batch_group_id}",
        )
        ax.scatter(
            [batch_group_id],
            [peak_count],
            s=120,
            c="#e74c3c",
            edgecolors="#922b21",
            linewidths=1.6,
            zorder=5,
        )
        ax.annotate(
            f"判定组 {batch_group_id}\n{peak_count}/{total} 张",
            xy=(batch_group_id, peak_count),
            xytext=(18, 22),
            textcoords="offset points",
            fontsize=10,
            color="#922b21",
            fontweight="bold",
            bbox=dict(boxstyle="round,pad=0.35", fc="#fff8f0", ec="#e0c4b0", alpha=0.95),
            arrowprops=dict(arrowstyle="->", color="#c0392b", lw=1.3),
        )

    ax.set_xlabel("最小可分辨组号", fontsize=11)
    ax.set_ylabel("图像数量（张）", fontsize=11)
    ax.set_title(
        f"批量最小可分辨组号频率分布（有效图像 {total} 张）",
        fontsize=12,
        fontweight="bold",
        pad=12,
    )

    ax.set_ylim(0, y_top)
    ax.set_yticks(y_ticks)
    ax.yaxis.set_major_formatter(FuncFormatter(lambda v, _: f"{int(round(v))}"))

    if len(groups) > 1:
        span = groups[-1] - groups[0]
        pad = max(0.5, span * 0.06)
        ax.set_xlim(groups[0] - pad, groups[-1] + pad)
    else:
        ax.set_xlim(groups[0] - 0.8, groups[0] + 0.8)

    ax.set_xticks(groups)
    ax.grid(True, which="major", axis="both", alpha=0.28, linestyle="-", linewidth=0.6)
    ax.spines["top"].set_visible(False)
    ax.spines["right"].set_visible(False)
    ax.legend(loc="upper right", fontsize=9, framealpha=0.92)

    plt.tight_layout()
    plt.savefig(plot_path, dpi=160, bbox_inches="tight", facecolor=fig.get_facecolor())
    plt.close(fig)
    return batch_group_id


def run_batch_distribution_mode(group_ids: List[int], output_path: str) -> Dict[str, Any]:
    batch_group_id = save_batch_group_distribution_plot(group_ids, output_path)
    counts: Dict[int, int] = {}
    for gid in group_ids:
        counts[gid] = counts.get(gid, 0) + 1
    return {
        "batch_min_resolvable_group_id": batch_group_id,
        "valid_image_count": len(group_ids),
        "count_by_group_id": counts,
        "plot_path": output_path,
    }


if __name__ == "__main__":
    _ensure_plot_fonts()
    _ = font_manager.findfont("DejaVu Sans")

    # 默认批量处理目录（直接运行脚本、不传 --input 时使用）
    DEFAULT_INPUT_DIR = r"C:\Users\lenovo\Desktop\550\图像采集文件夹"
    DEFAULT_OUTPUT_DIR = os.path.join(DEFAULT_INPUT_DIR, "mrc_result")

    parser = argparse.ArgumentParser(
        description="MRC_550: 550 新图案（5×5 组编号 1–25，不用映射表）"
    )
    parser.add_argument("--input", default="", help="输入图像路径（单张，可选）")
    parser.add_argument(
        "--input-dir",
        default=DEFAULT_INPUT_DIR,
        help=f"输入图像文件夹（批量，默认：{DEFAULT_INPUT_DIR}）",
    )
    parser.add_argument(
        "--output",
        default=DEFAULT_OUTPUT_DIR,
        help=f"输出目录（默认：{DEFAULT_OUTPUT_DIR}）",
    )
    parser.add_argument("--target", type=float, default=0.03, help="结果曲线标准线，默认0.03")
    parser.add_argument(
        "--expected-pairs",
        default="",
        help="每组预期明暗条数，如 1:3,2:3,3:3；为空时自动按邻域趋势估计",
    )
    parser.add_argument("--pair-tol", type=int, default=0, help="条数容差，默认0（不允许偏差）")
    parser.add_argument(
        "--profile-roi-pad",
        type=int,
        default=2,
        help="1号矩形剖面 ROI：仅向右多取列数，默认2；与 MRC_bofeng 一致",
    )
    parser.add_argument(
        "--orient",
        action="store_true",
        help="开启 90° 四向定向（默认关闭；输入图已摆正时无需开启）",
    )
    parser.add_argument(
        "--batch-distribution",
        action="store_true",
        help="批量模式：根据多张图像的最小可分辨组号绘制分布图",
    )
    parser.add_argument(
        "--group-ids",
        default="",
        help="批量分布模式下的组号列表，逗号分隔，如 20,21,21,21,22",
    )
    parser.add_argument(
        "--plot-output",
        default="",
        help="批量分布图输出路径（默认：<output>/min_group_distribution.png）",
    )
    args = parser.parse_args()

    if args.batch_distribution:
        raw = [x.strip() for x in str(args.group_ids).split(",") if x.strip()]
        if not raw:
            raise SystemExit("批量分布模式需要 --group-ids 参数。")
        try:
            group_ids = [int(x) for x in raw]
        except ValueError as exc:
            raise SystemExit(f"--group-ids 格式错误: {exc}") from exc

        output_dir = os.path.abspath(args.output)
        os.makedirs(output_dir, exist_ok=True)
        plot_path = args.plot_output.strip() or os.path.join(output_dir, "min_group_distribution.png")
        plot_path = os.path.abspath(plot_path)
        os.makedirs(os.path.dirname(plot_path), exist_ok=True)

        result = run_batch_distribution_mode(group_ids, plot_path)
        print(json.dumps(result, ensure_ascii=False))
        raise SystemExit(0)

    output_dir = os.path.abspath(args.output)
    os.makedirs(output_dir, exist_ok=True)

    if args.input:
        image_paths = [args.input]
    else:
        input_dir = os.path.abspath(args.input_dir)
        if not os.path.isdir(input_dir):
            raise FileNotFoundError(f"输入文件夹不存在: {input_dir}")
        image_paths = collect_images(input_dir)
        if not image_paths:
            raise FileNotFoundError(f"文件夹内未找到可处理图像: {input_dir}")

    processor = MRC550Processor()
    batch_group_ids: List[int] = []
    for img_path in image_paths:
        name = os.path.basename(img_path)
        try:
            result = processor.process_image(
                img_path,
                output_dir=output_dir,
                target_value=args.target,
                expected_pairs_spec=args.expected_pairs,
                pair_tolerance=args.pair_tol,
                profile_roi_pad=args.profile_roi_pad,
                enable_corner_orientation=bool(args.orient),
            )
            gid = result.get("min_resolvable_group_id")
            if gid is not None:
                batch_group_ids.append(int(gid))
            print(f"[OK] {name}")
        except Exception as e:
            print(f"[FAIL] {name}: {e}")

    if len(batch_group_ids) >= 2:
        plot_path = args.plot_output.strip() or os.path.join(output_dir, "min_group_distribution.png")
        plot_path = os.path.abspath(plot_path)
        os.makedirs(os.path.dirname(plot_path), exist_ok=True)
        batch_result = run_batch_distribution_mode(batch_group_ids, plot_path)
        print(json.dumps(batch_result, ensure_ascii=False))
