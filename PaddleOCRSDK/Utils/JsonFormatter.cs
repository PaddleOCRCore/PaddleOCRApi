using Newtonsoft.Json;

namespace PaddleOCRSDK
{
    public class JsonFormatter
    {
        /// <summary>
        /// 格式化 JSON 字符串，添加缩进和换行以提高可读性。
        /// </summary>
        /// <param name="str">原始 JSON 字符串</param>
        /// <returns>格式化后的 JSON 字符串</returns>
        public static string ConvertJsonString(string str)
        {
            // 尝试反序列化原始字符串为对象
            object obj = JsonConvert.DeserializeObject(str);
            if (obj != null)
            {
                // 使用 Formatting.Indented 选项进行格式化
                return JsonConvert.SerializeObject(obj, Formatting.Indented);
            }
            else
            {
                // 如果反序列化失败，返回原字符串
                return str;
            }
        }
    }
}
