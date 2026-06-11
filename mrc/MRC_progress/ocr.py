#!/usr/bin/env python3
# -*- coding: utf-8 -*-
import argparse
import os
from typing import Optional, Tuple

import cv2
import numpy as np


def imread_unicode(path: str) -> Optional[np.ndarray]:
    try:
        data = np.fromfile(path, dtype=np.uint8)
        if data.size == 0:
            return None
        return cv2.imdecode(data, cv2.IMREAD_COLOR)
    except Exception:
        return None


def imwrite_unicode(path: str, image: np.ndarray) -> bool:
    ext = os.path.splitext(path)[1] or ".png"
    ok, buf = cv2.imencode(ext, image)
    if not ok:
        return False
    try:
        buf.tofile(path)
        return True
    except Exception:
        return False


def build_gray_preview(image_bgr: np.ndarray) -> np.ndarray:
    gray = cv2.cvtColor(image_bgr, cv2.COLOR_BGR2GRAY)
    clahe = cv2.createCLAHE(clipLimit=1.2, tileGridSize=(8, 8))
    local = clahe.apply(gray)
    background = cv2.GaussianBlur(local, (0, 0), 5.0)
    detail = cv2.subtract(local, background)
    detail = cv2.normalize(detail, None, 0, 255, cv2.NORM_MINMAX).astype(np.uint8)
    return cv2.addWeighted(local, 0.82, detail, 0.18, 0.0)


def build_structure_mask(image_bgr: np.ndarray) -> np.ndarray:
    gray_local = build_gray_preview(image_bgr)
    background = cv2.GaussianBlur(gray_local, (0, 0), 11.0)
    highpass = cv2.subtract(gray_local, background)
    highpass = cv2.normalize(highpass, None, 0, 255, cv2.NORM_MINMAX)
    tophat = cv2.morphologyEx(gray_local, cv2.MORPH_TOPHAT, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (19, 19)))
    grad_x = cv2.Sobel(gray_local, cv2.CV_32F, 1, 0, ksize=3)
    grad_y = cv2.Sobel(gray_local, cv2.CV_32F, 0, 1, ksize=3)
    grad_mag = cv2.magnitude(grad_x, grad_y)
    grad_mag = cv2.GaussianBlur(grad_mag, (3, 3), 0)
    grad_mag = cv2.normalize(grad_mag, None, 0, 255, cv2.NORM_MINMAX).astype(np.uint8)
    _, m1 = cv2.threshold(highpass.astype(np.uint8), 0, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)
    _, m2 = cv2.threshold(tophat, 0, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)
    _, m3 = cv2.threshold(grad_mag, 0, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)
    mask = cv2.bitwise_or(cv2.bitwise_or(m1, m2), m3)
    mask = cv2.morphologyEx(mask, cv2.MORPH_OPEN, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (3, 3)))
    mask = cv2.morphologyEx(mask, cv2.MORPH_CLOSE, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (5, 5)))
    return mask


def collect_central_points(mask: np.ndarray) -> Tuple[np.ndarray, np.ndarray]:
    ys, xs = np.where(mask > 0)
    if len(xs) < 100:
        raise RuntimeError("亮结构太少，无法建立参考矩形。")
    center_x, center_y = float(np.mean(xs)), float(np.mean(ys))
    bbox_w, bbox_h = float(xs.max() - xs.min() + 1), float(ys.max() - ys.min() + 1)
    radius = 0.22 * max(bbox_w, bbox_h)
    num_labels, labels, stats, centroids = cv2.connectedComponentsWithStats(mask, connectivity=8)
    selected_mask = np.zeros_like(mask)
    points = []
    for idx in range(1, num_labels):
        if int(stats[idx, cv2.CC_STAT_AREA]) < 20:
            continue
        cx, cy = centroids[idx]
        if float(np.hypot(cx - center_x, cy - center_y)) > radius:
            continue
        ys_i, xs_i = np.where(labels == idx)
        if len(xs_i) == 0:
            continue
        selected_mask[ys_i, xs_i] = 255
        points.append(np.column_stack([xs_i.astype(np.float32), ys_i.astype(np.float32)]))
    if not points:
        raise RuntimeError("中心参考矩形提取失败。")
    return np.vstack(points), selected_mask


def normalize_angle_to_45(angle_deg: float) -> float:
    while angle_deg <= -45.0:
        angle_deg += 90.0
    while angle_deg > 45.0:
        angle_deg -= 90.0
    return angle_deg


def fit_reference_rectangle(points: np.ndarray) -> Tuple[np.ndarray, float]:
    hull = cv2.convexHull(points.reshape(-1, 1, 2))
    rect = cv2.minAreaRect(hull)
    box = cv2.boxPoints(rect).astype(np.float32)
    best_angle, best_abs = None, 1e9
    for idx in range(4):
        p1, p2 = box[idx], box[(idx + 1) % 4]
        angle = float(np.degrees(np.arctan2(float(p2[1] - p1[1]), float(p2[0] - p1[0]))))
        angle = normalize_angle_to_45(angle)
        if abs(angle) < best_abs:
            best_abs, best_angle = abs(angle), angle
    if best_angle is None:
        raise RuntimeError("参考矩形角度计算失败。")
    return np.int32(np.round(box)), float(best_angle)


def measure_side_angle(image_bgr: np.ndarray) -> Tuple[float, np.ndarray, np.ndarray, np.ndarray]:
    mask = build_structure_mask(image_bgr)
    points, selected_mask = collect_central_points(mask)
    box, angle_deg = fit_reference_rectangle(points)
    return angle_deg, mask, selected_mask, box


def estimate_dark_border_value(image: np.ndarray) -> Tuple[int, int, int]:
    h, w = image.shape[:2]
    bw, bh = max(8, w // 30), max(8, h // 30)
    strips = [image[:bh, :, :].reshape(-1, 3), image[-bh:, :, :].reshape(-1, 3), image[:, :bw, :].reshape(-1, 3), image[:, -bw:, :].reshape(-1, 3)]
    value = np.percentile(np.vstack(strips), 15, axis=0)
    return tuple(int(np.clip(v, 0, 255)) for v in value)


def rotate_keep_size(image: np.ndarray, delta_deg: float, border_value: Tuple[int, int, int]) -> np.ndarray:
    h, w = image.shape[:2]
    matrix = cv2.getRotationMatrix2D((w / 2.0, h / 2.0), delta_deg, 1.0)
    return cv2.warpAffine(image, matrix, (w, h), flags=cv2.INTER_LINEAR, borderMode=cv2.BORDER_CONSTANT, borderValue=border_value)


def refine_rotation_with_verification(image_bgr: np.ndarray, initial_angle_deg: float, border_value: Tuple[int, int, int]) -> Tuple[np.ndarray, float, float]:
    candidates = []
    for signed_angle in (-initial_angle_deg, initial_angle_deg):
        rotated = rotate_keep_size(image_bgr, signed_angle, border_value)
        try:
            residual, _, _, _ = measure_side_angle(rotated)
        except Exception:
            residual = 1e9
        candidates.append((abs(residual), rotated, signed_angle, residual))
    candidates.sort(key=lambda item: item[0])
    best_abs, best_image, applied_rotation_deg, residual = candidates[0]
    for _ in range(5):
        if best_abs <= 0.35:
            break
        updated = rotate_keep_size(best_image, -residual, border_value)
        try:
            new_residual, _, _, _ = measure_side_angle(updated)
        except Exception:
            break
        if abs(new_residual) >= best_abs:
            break
        best_image, applied_rotation_deg, residual, best_abs = updated, applied_rotation_deg - residual, new_residual, abs(new_residual)
    return best_image, applied_rotation_deg, residual


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="OCR standalone: 检测倾角并摆正，输出摆正图")
    parser.add_argument("--input", default=r"F:\研究生\项目\MRC_progress\mrc_test.png")
    parser.add_argument("--output", default=os.path.join(os.path.dirname(os.path.abspath(__file__)), "result_ocr"))
    args = parser.parse_args()

    image = imread_unicode(args.input)
    if image is None:
        raise FileNotFoundError(f"无法读取图像: {args.input}")

    border0 = estimate_dark_border_value(image)
    angle_deg, _, _, _ = measure_side_angle(image)
    aligned, _, _ = refine_rotation_with_verification(image, angle_deg, border0)

    os.makedirs(args.output, exist_ok=True)
    stem = os.path.splitext(os.path.basename(args.input))[0]
    ext = os.path.splitext(args.input)[1] or ".png"
    out_a = os.path.join(args.output, f"{stem}_a{ext}")
    if not imwrite_unicode(out_a, aligned):
        raise IOError(f"保存失败: {out_a}")

    print(f"[OK] {stem}")
