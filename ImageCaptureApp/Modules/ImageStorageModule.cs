using Emgu.CV;
using System;
using System.Collections.Generic;
using System.IO;
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
            BMP,
            JPEG,
            PNG,
            TIFF
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

                // 根据格式设置参数
                var parameters = new KeyValuePair<ImwriteFlags, int>[]
                {
                    new KeyValuePair<ImwriteFlags, int>(ImwriteFlags.JpegQuality, 95)
                };

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
        public static bool SaveImageWithDialog(Mat image, string defaultFileName = "image")
        {
            if (image == null || image.IsEmpty)
            {
                return false;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "PNG图像|*.png|JPEG图像|*.jpg;*.jpeg|BMP图像|*.bmp|TIFF图像|*.tiff;*.tif|所有文件|*.*";
                dialog.FilterIndex = 1;
                dialog.FileName = defaultFileName;
                dialog.DefaultExt = "png";

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
        private static KeyValuePair<int, int>[] GetSaveParameters(ImageFormat format)
        {
            return format switch
            {
                ImageFormat.JPEG => new KeyValuePair<int, int>[]
                {
                    new KeyValuePair<int, int>((int)ImwriteFlags.JpegQuality, 95)
                },
                ImageFormat.PNG => new KeyValuePair<int, int>[]
                {
                    new KeyValuePair<int, int>((int)ImwriteFlags.PngCompression, 3)
                },
                _ => Array.Empty<KeyValuePair<int, int>>()
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
                ".bmp" => ImageFormat.BMP,
                ".tiff" or ".tif" => ImageFormat.TIFF,
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
                ImageFormat.BMP => ".bmp",
                ImageFormat.TIFF => ".tiff",
                _ => ".png"
            };
        }
    }
}
