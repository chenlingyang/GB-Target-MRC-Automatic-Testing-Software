using Emgu.CV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Emgu.CV.CvEnum;

namespace ImageCaptureApp.Modules
{
    /// <summary>
    /// 图像存储模块 - 负责图像的保存和管理
    /// </summary>
    public static class ImageStorageModule
    {
        /// <summary>
        /// 图像格式枚举
        /// </summary>
        public enum ImageFormat
        {
            JPEG,
            PNG,
            TIFF,
            RAW
        }

        /// <summary>
        /// 保存图像到文件
        /// </summary>
        /// <param name="image">要保存的图像</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="format">图像格式</param>
        /// <returns>是否保存成功</returns>
        public static bool SaveImage(Mat image, string filePath, ImageFormat format = ImageFormat.PNG)
        {
            if (image == null || image.IsEmpty)
            {
                return false;
            }

            try
            {
                // 确保目录存在
                string? directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (format == ImageFormat.RAW)
                {
                    return SaveRaw(image, filePath);
                }

                var parameters = GetSaveParameters(format);
                return CvInvoke.Imwrite(filePath, image, parameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存图像失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 保存图像（带对话框选择路径）
        /// </summary>
        /// <param name="image">要保存的图像</param>
        /// <param name="defaultFileName">默认文件名</param>
        /// <returns>是否保存成功</returns>
        public static bool SaveImageWithDialog(Mat image, string defaultFileName = "image", ImageFormat defaultFormat = ImageFormat.PNG)
        {
            if (image == null || image.IsEmpty)
            {
                return false;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "PNG图像|*.png|JPEG图像|*.jpg;*.jpeg|TIFF图像|*.tiff;*.tif|RAW原始数据|*.raw|所有文件|*.*";
                dialog.FilterIndex = GetFilterIndex(defaultFormat);
                dialog.FileName = defaultFileName;
                dialog.DefaultExt = GetExtension(defaultFormat).TrimStart('.');

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ImageFormat format = GetFormatFromExtension(Path.GetExtension(dialog.FileName));
                    return SaveImage(image, dialog.FileName, format);
                }
            }

            return false;
        }

        /// <summary>
        /// 批量保存图像
        /// </summary>
        /// <param name="images">图像列表</param>
        /// <param name="directory">保存目录</param>
        /// <param name="format">图像格式</param>
        /// <param name="prefix">文件名前缀</param>
        /// <returns>成功保存的数量</returns>
        public static int SaveImages(List<Mat> images, string directory, ImageFormat format = ImageFormat.PNG, string prefix = "image")
        {
            if (images == null || images.Count == 0)
            {
                return 0;
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            int successCount = 0;
            string extension = GetExtension(format);

            for (int i = 0; i < images.Count; i++)
            {
                string fileName = $"{prefix}_{DateTime.Now:yyyyMMdd_HHmmss}_{i:D4}{extension}";
                string filePath = Path.Combine(directory, fileName);

                if (SaveImage(images[i], filePath, format))
                {
                    successCount++;
                }
            }

            return successCount;
        }

        /// <summary>
        /// 获取保存参数
        /// </summary>
        private static KeyValuePair<ImwriteFlags, int>[] GetSaveParameters(ImageFormat format)
        {
            return format switch
            {
                ImageFormat.JPEG => new KeyValuePair<ImwriteFlags, int>[]
                {
                    new KeyValuePair<ImwriteFlags, int>(ImwriteFlags.JpegQuality, 95)
                },
                ImageFormat.PNG => new KeyValuePair<ImwriteFlags, int>[]
                {
                    new KeyValuePair<ImwriteFlags, int>(ImwriteFlags.PngCompression, 3)
                },
                _ => Array.Empty<KeyValuePair<ImwriteFlags, int>>()
            };
        }

        /// <summary>
        /// 根据扩展名获取格式
        /// </summary>
        private static ImageFormat GetFormatFromExtension(string extension)
        {
            return extension.ToLower() switch
            {
                ".jpg" or ".jpeg" => ImageFormat.JPEG,
                ".png" => ImageFormat.PNG,
                ".tiff" or ".tif" => ImageFormat.TIFF,
                ".raw" => ImageFormat.RAW,
                _ => ImageFormat.PNG
            };
        }

        /// <summary>
        /// 获取格式对应的扩展名
        /// </summary>
        private static string GetExtension(ImageFormat format)
        {
            return format switch
            {
                ImageFormat.JPEG => ".jpg",
                ImageFormat.PNG => ".png",
                ImageFormat.TIFF => ".tiff",
                ImageFormat.RAW => ".raw",
                _ => ".png"
            };
        }

        public static ImageFormat ParseFormatOrDefault(string? value, ImageFormat fallback = ImageFormat.PNG)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            return value.Trim().ToUpperInvariant() switch
            {
                "PNG" => ImageFormat.PNG,
                "JPG" or "JPEG" => ImageFormat.JPEG,
                "TIF" or "TIFF" => ImageFormat.TIFF,
                "RAW" => ImageFormat.RAW,
                _ => fallback
            };
        }

        public static string ToConfigFormatString(ImageFormat format)
        {
            return format switch
            {
                ImageFormat.PNG => "PNG",
                ImageFormat.JPEG => "JPG",
                ImageFormat.TIFF => "TIF",
                ImageFormat.RAW => "RAW",
                _ => "PNG"
            };
        }

        private static int GetFilterIndex(ImageFormat format)
        {
            return format switch
            {
                ImageFormat.PNG => 1,
                ImageFormat.JPEG => 2,
                ImageFormat.TIFF => 3,
                ImageFormat.RAW => 4,
                _ => 1
            };
        }

        private static bool SaveRaw(Mat image, string filePath)
        {
            int rows = image.Rows;
            int cols = image.Cols;
            int elemSize = image.ElementSize;
            int bytesPerRow = cols * elemSize;
            int totalBytes = bytesPerRow * rows;
            if (bytesPerRow <= 0 || totalBytes <= 0)
            {
                return false;
            }

            byte[] raw = new byte[totalBytes];
            IntPtr dataPtr = image.DataPointer;
            long step = image.Step;
            for (int r = 0; r < rows; r++)
            {
                IntPtr src = IntPtr.Add(dataPtr, (int)(r * step));
                Marshal.Copy(src, raw, r * bytesPerRow, bytesPerRow);
            }

            File.WriteAllBytes(filePath, raw);
            WriteRawMetadata(filePath, image, bytesPerRow, totalBytes);
            return true;
        }

        private static void WriteRawMetadata(string rawPath, Mat image, int bytesPerRow, int totalBytes)
        {
            string metaPath = Path.ChangeExtension(rawPath, ".txt");
            string[] lines =
            {
                "format=RAW",
                $"file={Path.GetFileName(rawPath)}",
                $"width={image.Cols}",
                $"height={image.Rows}",
                $"channels={image.NumberOfChannels}",
                $"depth={image.Depth}",
                $"element_size_bytes={image.ElementSize}",
                $"row_stride_bytes={image.Step}",
                $"row_data_bytes={bytesPerRow}",
                $"total_bytes={totalBytes}",
                "byte_order=little_endian",
                "pixel_order=interleaved"
            };
            File.WriteAllLines(metaPath, lines, Encoding.UTF8);
        }
    }
}
