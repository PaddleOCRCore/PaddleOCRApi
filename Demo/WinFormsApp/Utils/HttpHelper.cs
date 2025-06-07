using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;

namespace WinFormsApp.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpActionResult
    {
        /// <summary>
        /// 
        /// </summary>
        public HttpStatusCode Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage { get; set; }
    }
    public class HttpHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string Post(string url, IDictionary<Object, Object> dic)
        {
            ServicePointManager.DefaultConnectionLimit = 5000;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
                {
                    return true; //总是接受  
                });
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |
                    SecurityProtocolType.Tls11;
            }
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Accept = "application/json";
            request.Timeout = 150000;
            request.AllowAutoRedirect = true;

            string responseStr = null;

            try
            {
                if (dic.Count > 0)
                {
                    string param = DictionaryToJson(dic);
                    StreamWriter requestStream = new StreamWriter(request.GetRequestStream());
                    requestStream.Write(param);
                    requestStream.Close();
                }
                // 发送请求并获取响应
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseStr = reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                HttpActionResult actionResult = new HttpActionResult();
                actionResult.Status = HttpStatusCode.BadRequest;
                actionResult.ErrorMessage = ex.Message;
                return JsonConvert.SerializeObject(actionResult);
            }
            catch (Exception ex)
            {
                HttpActionResult actionResult = new HttpActionResult();
                actionResult.Status = HttpStatusCode.BadRequest;
                actionResult.ErrorMessage = ex.Message;
                return JsonConvert.SerializeObject(actionResult);
            }
            return responseStr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public static string PostFile(string url, string imagePath)
        {
            ServicePointManager.DefaultConnectionLimit = 5000;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
                {
                    return true; //总是接受  
                });
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |
                    SecurityProtocolType.Tls11;
            }
            string responseStr = null;
            // 构建multipart/form-data请求体
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endBoundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

            // 创建HttpWebRequest对象
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            // 打开文件并读取为字节数组
            byte[] fileBytes = File.ReadAllBytes(imagePath);
            using (MemoryStream ms = new MemoryStream())
            {
                // 写入表单数据
                ms.Write(boundaryBytes, 0, boundaryBytes.Length);
                string header = $"Content-Disposition: form-data; name=\"request\"; filename=\"{Path.GetFileName(imagePath)}\"\r\nContent-Type: image/jpeg\r\n\r\n";
                byte[] headerBytes = Encoding.UTF8.GetBytes(header);
                ms.Write(headerBytes, 0, headerBytes.Length);
                ms.Write(fileBytes, 0, fileBytes.Length);
                ms.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                // 设置请求体内容长度
                request.ContentLength = ms.Length;
                // 获取请求流并写入数据
                using (Stream requestStream = request.GetRequestStream())
                {
                    ms.WriteTo(requestStream);
                }
            }
            try
            {
                // 发送请求并获取响应
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseStr = reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                HttpActionResult actionResult = new HttpActionResult();
                actionResult.Status = HttpStatusCode.BadRequest;
                actionResult.ErrorMessage = ex.Message;
                return JsonConvert.SerializeObject(actionResult);
            }
            catch (Exception ex)
            {
                HttpActionResult actionResult = new HttpActionResult();
                actionResult.Status = HttpStatusCode.BadRequest;
                actionResult.ErrorMessage = ex.Message;
                return JsonConvert.SerializeObject(actionResult);
            }
            return responseStr;
        }
        /// <summary>
        /// Dictionary转JSON
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string DictionaryToJson(IDictionary<Object, Object> dic)
        {
            StringBuilder JsonString = new StringBuilder();
            JsonString.Append("{");
            foreach (string key in dic.Keys)
            {
                JsonString.AppendFormat("\"{0}\":", key);
                Object value = dic[key];
                string valueType = dic[key].GetType().ToString();
                switch (valueType)
                {
                    case "System.Int32":
                    case "System.Double":
                    case "System.Single":
                    case "System.Boolean":
                        JsonString.AppendFormat("{0}", dic[key].ToString().ToLower());
                        break;
                    case "System.DateTime":
                        JsonString.AppendFormat("\"{0}\"", Convert.ToDateTime(dic[key]).ToString("yyyy-MM-dd HH:mm:ss"));
                        break;
                    default:
                        JsonString.AppendFormat("\"{0}\"", dic[key]);
                        break;
                }
                JsonString.Append(",");

            }
            JsonString.Append(",}");
            return JsonString.ToString().Replace(",,}", "}");
        }
    }
}
