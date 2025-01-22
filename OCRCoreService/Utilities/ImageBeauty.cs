using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace OCRCoreService
{
    /// <summary>
    /// 人像美化接口
    /// </summary>
    public static class ImageBeauty
    {
        /// <summary>
        /// 图片转为base64编码的文本
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static string ToBase64(Bitmap bmp)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                String strbaser64 = Convert.ToBase64String(arr);
                return strbaser64;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// base64编码的文本转为图片
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static Bitmap Base64StringToImage(string inputStr)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(inputStr);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    Bitmap bmp = new Bitmap(ms);
                    return bmp;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #region 图片转Base64
        public static string ImgToBase64String(Bitmap bmp)
        {
            string strbaser64 = "";
            try
            {
                using (Bitmap bitmap = new Bitmap(bmp))
                {
                    bitmap.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitmap.Save(ms, ImageFormat.Jpeg);
                        byte[] arr = new byte[ms.Length];
                        ms.Position = 0;
                        ms.Read(arr, 0, (int)ms.Length);
                        ms.Close();
                        strbaser64 = Convert.ToBase64String(arr);
                        return strbaser64;
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        #endregion

        #region 图片路径转Base64
        /// <summary>
        /// 图片路径转Base64
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        public static string GetBase64FromImage(string strPath)
        {
            string strbaser64 = "";
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(strPath, FileMode.Open)))
                {
                    FileInfo fi = new FileInfo(strPath);
                    byte[] bytes = reader.ReadBytes((int)fi.Length);
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        strbaser64 = Convert.ToBase64String(bytes);
                        return strbaser64;
                    }
                }
            }
            catch (Exception)
            {
                return strbaser64;
            }
        }
        #endregion

        #region 等比缩放
        /// <summary>
        /// 图片等比缩放
        /// </summary>
        /// <remarks></remarks>
        /// <param name="initImage">原图Stream对象</param>
        /// <param name="destWidth">指定的最大宽度</param>
        /// <param name="destHeight">指定的最大高度</param>
        public static Bitmap ZoomAuto(Bitmap initImage, Double destWidth, Double destHeight)
        {
            //缩略图宽、高计算
            double newWidth = destWidth;
            double newHeight = destHeight;
            //宽大于高或宽等于高（横图或正方）
            if (initImage.Width > initImage.Height || initImage.Width == initImage.Height)
            {
                //如果宽大于模版
                if (initImage.Width > destWidth)
                {
                    //宽按模版，高按比例缩放
                    newWidth = destWidth;
                    newHeight = initImage.Height * (destWidth / initImage.Width);
                }
            }
            //高大于宽（竖图）
            else
            {
                //如果高大于模版
                if (initImage.Height > destHeight)
                {
                    //高按模版，宽按比例缩放
                    newHeight = destHeight;
                    newWidth = initImage.Width * (destHeight / initImage.Height);
                }
            }
            //生成新图
            //新建一个bmp图片
            Bitmap newImage = new Bitmap((int)newWidth, (int)newHeight);
            newImage.SetResolution(initImage.HorizontalResolution, initImage.VerticalResolution);
            //新建一个画板
            Graphics gp = Graphics.FromImage(newImage);
            try
            {
                Rectangle destRect = new Rectangle(0, 0, newImage.Width, newImage.Height);
                gp.SmoothingMode = SmoothingMode.HighQuality;
                gp.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gp.CompositingQuality = CompositingQuality.HighQuality;
                gp.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gp.Clear(Color.White);
                gp.DrawImage(initImage, destRect,
                    new Rectangle(0, 0, initImage.Width, initImage.Height), GraphicsUnit.Pixel);
                //gp.DrawImage(initImage, destRect, 0, 0, initImage.Width, initImage.Height, GraphicsUnit.Pixel);
            }
            finally
            {
                gp.Dispose();
            }
            initImage.Dispose();
            return newImage;
        }
        #endregion
    }
}
