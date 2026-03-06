using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Drawing;

namespace ImageCaptureApp.Modules
{
    /// <summary>
    /// 图像处理模块 - 提供各种图像处理功能
    /// </summary>
    public static class ImageProcessingModule
    {
        /// <summary>
        /// 将图像转换为灰度图
        /// </summary>
        /// <param name="source">源图像</param>
        /// <returns>灰度图像</returns>
        public static Mat ConvertToGrayscale(Mat source)
        {
            if (source == null || source.IsEmpty)
            {
                return new Mat();
            }

            Mat gray = new Mat();
            
            if (source.NumberOfChannels == 1)
            {
                // 已经是灰度图，直接返回副本
                source.CopyTo(gray);
            }
            else if (source.NumberOfChannels == 3)
            {
                // RGB转灰度
                CvInvoke.CvtColor(source, gray, ColorConversion.Bgr2Gray);
            }
            else if (source.NumberOfChannels == 4)
            {
                // BGRA转灰度
                CvInvoke.CvtColor(source, gray, ColorConversion.Bgra2Gray);
            }
            else
            {
                source.CopyTo(gray);
            }

            return gray;
        }

        /// <summary>
        /// 缩放图像（按比例）
        /// </summary>
        /// <param name="source">源图像</param>
        /// <param name="scale">缩放比例（1.0为原始大小，>1.0为放大，<1.0为缩小）</param>
        /// <param name="interpolation">插值方法</param>
        /// <returns>缩放后的图像</returns>
        public static Mat ResizeImage(Mat source, double scale, Inter interpolation = Inter.Linear)
        {
            if (source == null || source.IsEmpty || scale <= 0)
            {
                return new Mat();
            }

            int newWidth = (int)(source.Width * scale);
            int newHeight = (int)(source.Height * scale);

            return ResizeImage(source, newWidth, newHeight, interpolation);
        }

        /// <summary>
        /// 缩放图像（指定尺寸）
        /// </summary>
        /// <param name="source">源图像</param>
        /// <param name="width">目标宽度</param>
        /// <param name="height">目标高度</param>
        /// <param name="interpolation">插值方法</param>
        /// <returns>缩放后的图像</returns>
        public static Mat ResizeImage(Mat source, int width, int height, Inter interpolation = Inter.Linear)
        {
            if (source == null || source.IsEmpty || width <= 0 || height <= 0)
            {
                return new Mat();
            }

            Mat resized = new Mat();
            CvInvoke.Resize(source, resized, new Size(width, height), 0, 0, interpolation);
            return resized;
        }

        /// <summary>
        /// 裁剪图像
        /// </summary>
        /// <param name="source">源图像</param>
        /// <param name="region">裁剪区域</param>
        /// <returns>裁剪后的图像</returns>
        public static Mat CropImage(Mat source, Rectangle region)
        {
            if (source == null || source.IsEmpty)
            {
                return new Mat();
            }

            // 确保区域在图像范围内
            region.Intersect(new Rectangle(0, 0, source.Width, source.Height));

            if (region.Width <= 0 || region.Height <= 0)
            {
                return new Mat();
            }

            Mat cropped = new Mat(source, region);
            return cropped.Clone();
        }

        /// <summary>
        /// 调整图像亮度
        /// </summary>
        public static Mat AdjustBrightness(Mat source, int brightness)
        {
            if (source == null || source.IsEmpty)
            {
                return new Mat();
            }

            Mat result = new Mat();
            source.ConvertTo(result, DepthType.Cv8U, 1, brightness);
            return result;
        }

        /// <summary>
        /// 调整图像对比度
        /// </summary>
        public static Mat AdjustContrast(Mat source, double contrast)
        {
            if (source == null || source.IsEmpty)
            {
                return new Mat();
            }

            Mat result = new Mat();
            source.ConvertTo(result, DepthType.Cv8U, contrast, 0);
            return result;
        }
    }
}
