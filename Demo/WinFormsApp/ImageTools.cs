using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp
{
    public static class ImageTools
    {
        #region 读取图片到image中
        /// <summary>
        /// 读取图片到image中
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        public static Image LoadImage(string strPath)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(strPath, FileMode.Open)))
            {
                FileInfo fi = new FileInfo(strPath);
                byte[] bytes = reader.ReadBytes((int)fi.Length);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    return Image.FromStream(ms);
                }
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
    }
}
