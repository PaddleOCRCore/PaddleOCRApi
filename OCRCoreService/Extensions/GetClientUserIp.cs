using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http
{
    /// <summary>
    /// 
    /// </summary>
    public static class GetClientUserIp
    {
        /// <summary>
        /// 获取客户Ip
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetClientIp(this HttpContext context)
        {
            var ip = "";
            try
            {
                var xFrowarded = context.Request.Headers["X-Forwarded-For"];

                if (xFrowarded.Count() > 0)
                {
                    ip = xFrowarded.FirstOrDefault();
                }
                if (string.IsNullOrEmpty(ip))
                {
                    var remoteIp = context.Connection.RemoteIpAddress;
                    ip = remoteIp != null ? remoteIp.MapToIPv4().ToString() : "-";
                }
                if (ip.Equals("::1"))
                    ip = "127.0.0.1";
            }
            catch (Exception)
            {
                throw;
            }
            return ip ?? "";
        }
        private static bool IsIPAddress(string str1)
        {
            if (str1 == null || str1 == string.Empty || str1.Length < 7 || str1.Length > 15) return false;
            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(regformat, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return regex.IsMatch(str1);
        }
    }
    
}
