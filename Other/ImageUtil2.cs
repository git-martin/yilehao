using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AutoOrderSender
{
    /// <summary>
    /// 水印位置
    /// </summary>
    public enum ImagePosition
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 1,
        /// <summary>
        /// 左上
        /// </summary>
        LeftTop = 2,
        /// <summary>
        /// 左下
        /// </summary>
        LeftBottom = 3,
        /// <summary>
        /// 右上
        /// </summary>
        RightTop = 4,
        /// <summary>
        /// 右下
        /// </summary>
        RigthBottom = 5,
        //TopMiddle,     //顶部居中
        //BottomMiddle, //底部居中
        //Center           //中心
    }

    /// <summary>
    /// ImageUtil 的摘要说明。
    /// </summary>
    public class ImageUtil2
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ImageUtil2()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        ///  生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>	
        /// <param name="isaddwatermark">是否添加水印</param>	
        /// <param name="quality">图片品质</param>	
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode = "Cut", bool isaddwatermark = false, int quality = 75)
        {
            MakeThumbnail(originalImagePath, thumbnailPath, width, height, mode, isaddwatermark, ImagePosition.Default, null, quality);
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>	
        /// <param name="isaddwatermark">是否添加水印</param>	
        /// <param name="quality">图片品质</param>	
        /// <param name="imagePosition">水印位置</param>	
        /// <param name="waterImage">水印图片名称</param>	
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode, bool isaddwatermark, ImagePosition imagePosition, string waterImage = null, int quality = 75)
        {
            Image originalImage = Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）				
                    break;
                case "W"://指定宽，高按比例					
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）				
                    if (originalImage.Width >= towidth && originalImage.Height >= toheight)
                    {
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                        else
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                    }
                    else
                    {
                        x = (originalImage.Width - towidth) / 2;
                        y = (originalImage.Height - toheight) / 2;
                        ow = towidth;
                        oh = toheight;
                    }
                    break;
                case "Fit"://不超出尺寸，比它小就不截了，不留白，大就缩小到最佳尺寸，主要为手机用
                    if (originalImage.Width > towidth && originalImage.Height > toheight)
                    {
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                            toheight = originalImage.Height * width / originalImage.Width;
                        else
                            towidth = originalImage.Width * height / originalImage.Height;
                    }
                    else if (originalImage.Width > towidth)
                    {
                        toheight = originalImage.Height * width / originalImage.Width;
                    }
                    else if (originalImage.Height > toheight)
                    {
                        towidth = originalImage.Width * height / originalImage.Height;
                    }
                    else
                    {
                        towidth = originalImage.Width;
                        toheight = originalImage.Height;
                        ow = towidth;
                        oh = toheight;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(Color.White);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);

            //加图片水印
            if (isaddwatermark)
            {
                if (string.IsNullOrEmpty(waterImage))
                    waterImage = "watermarker.png";
                Image copyImage = System.Drawing.Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, waterImage));
                //g.DrawImage(copyImage, new Rectangle(bitmap.Width-copyImage.Width, bitmap.Height-copyImage.Height, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, GraphicsUnit.Pixel);
                int xPosOfWm;
                int yPosOfWm;
                int wmHeight = copyImage.Height;
                int wmWidth = copyImage.Width;
                int phHeight = toheight;
                int phWidth = towidth;
                switch (imagePosition)
                {
                    case ImagePosition.LeftBottom:
                        xPosOfWm = 70;
                        yPosOfWm = phHeight - wmHeight - 70;
                        break;
                    case ImagePosition.LeftTop:
                        xPosOfWm = 70;
                        yPosOfWm = 0 - 70;
                        break;
                    case ImagePosition.RightTop:
                        xPosOfWm = phWidth - wmWidth - 70;
                        yPosOfWm = 0 - 70;
                        break;
                    case ImagePosition.RigthBottom:
                        xPosOfWm = phWidth - wmWidth - 70;
                        yPosOfWm = phHeight - wmHeight - 70;
                        break;
                    default:
                        xPosOfWm = 10;
                        yPosOfWm = 0;
                        break;
                }
                g.DrawImage(copyImage, new Rectangle(xPosOfWm, yPosOfWm, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, GraphicsUnit.Pixel);
            }

            //文字水印
            if ("亿乐豪商城" != "")
            {
                using (System.Drawing.Graphics gWater = System.Drawing.Graphics.FromImage(bitmap))
                {
                    System.Drawing.Font fontWater = new Font("宋体", 10);
                    System.Drawing.Brush brushWater = new SolidBrush(Color.White);
                    gWater.DrawString("亿乐豪商城", fontWater, brushWater, 10, 10);
                    gWater.Dispose();
                }
            }


            // 以下代码为保存图片时,设置压缩质量
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityArray = new long[1];
            qualityArray[0] = quality;
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityArray);
            encoderParams.Param[0] = encoderParam;
            //获得包含有关内置图像编码解码器的信息的ImageCodecInfo 对象.
            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo jpegICI = null;
            for (int i = 0; i < arrayICI.Length; i++)
            {
                if (arrayICI[i].FormatDescription.Equals("JPEG"))
                {
                    jpegICI = arrayICI[i];
                    //设置JPEG编码
                    break;
                }
            }

            try
            {
                if (jpegICI != null)
                {
                    bitmap.Save(thumbnailPath, jpegICI, encoderParams);
                }
                else
                {
                    //以jpg格式保存缩略图
                    bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }

        #region 图片旋转函数


        /// <summary>  

        /// 以逆时针为方向对图像进行旋转  

        /// </summary>  

        /// <param name="b">位图流</param>  

        /// <param name="angle">旋转角度[0,360](前台给的)</param>  

        /// <returns></returns>  

        public static Image RotateImg(Image b, int angle, string targetFileName)
        {

            angle = angle % 360;


            //弧度转换  

            double radian = angle * Math.PI / 180.0;

            double cos = Math.Cos(radian);

            double sin = Math.Sin(radian);


            //原图的宽和高  

            int w = b.Width;

            int h = b.Height;

            int W = (int)(Math.Max(Math.Abs(w * cos - h * sin), Math.Abs(w * cos + h * sin)));

            int H = (int)(Math.Max(Math.Abs(w * sin - h * cos), Math.Abs(w * sin + h * cos)));


            //目标位图  

            Bitmap dsImage = new Bitmap(W, H);

            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(dsImage);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;


            //计算偏移量  

            Point Offset = new Point((W - w) / 2, (H - h) / 2);


            //构造图像显示区域：让图像的中心与窗口的中心点一致  

            Rectangle rect = new Rectangle(Offset.X, Offset.Y, w, h);

            Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);


            g.TranslateTransform(center.X, center.Y);

            g.RotateTransform(360 - angle);


            //恢复图像在水平和垂直方向的平移  

            g.TranslateTransform(-center.X, -center.Y);

            g.DrawImage(b, rect);


            //重至绘图的所有变换  

            g.ResetTransform();


            g.Save();

            g.Dispose();

            //保存旋转后的图片  

            b.Dispose();

            dsImage.Save(targetFileName, System.Drawing.Imaging.ImageFormat.Jpeg);

            return dsImage;

        }


        public static Image RotateImg(string filename, int angle, string targetFileName)
        {

            return RotateImg(GetSourceImg(filename), angle, targetFileName);

        }


        public static Image GetSourceImg(string filename)
        {

            Image img;



            img = Bitmap.FromFile(filename);


            return img;

        }


        #endregion 图片旋转函数
    }
}
