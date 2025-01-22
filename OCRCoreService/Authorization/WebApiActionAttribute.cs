// Copyright (c) 2025 PaddleOCRCore All Rights Reserved.
// https://github.com/PaddleOCRCore/PaddleOCRApi.git
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace OCRCoreService.Authorization;

/// <summary>
/// 
/// </summary>
//表示所有操作-筛选器特性的基类。
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class WebApiActionAttribute : ActionFilterAttribute
{
    private readonly ILogger<WebApiActionAttribute> logger;
    private readonly IServiceProvider provider;
    private readonly IConfiguration configuration;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_logger"></param>
    /// <param name="_provider"></param>
    /// <param name="_configuration"></param>
    public WebApiActionAttribute(ILogger<WebApiActionAttribute> _logger, IServiceProvider _provider,
            IConfiguration _configuration)
    {
        configuration = _configuration;
        logger = _logger;
        provider = _provider;
    }
    /// <summary>
    /// 获取配置文件
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private string GetValue(string key)
    {
        return configuration?[key] ?? "";
    }
    /// <summary>
    /// 在Action执行之后由 MVC 框架调用。
    /// </summary>
    /// <param name="context"></param>
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        base.OnActionExecuted(context);
        //string actionName= context.ActionDescriptor.
    }
    /// <summary>
    /// 在Action执行之前由 MVC 框架调用。
    /// </summary>
    /// <param name="context"></param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        try
        {
            //获取10位当前时间戳
            long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            //获取13位时间戳
            //var UninTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            //10位时间戳转换为DateTime
            //var DateTimeUnix = DateTimeOffset.FromUnixTimeSeconds(UninTimeStamp); 
            //13位时间戳转换为DateTime
            //var DateTimeUnix = DateTimeOffset.FromUnixTimeMilliseconds(UninTimeStamp); 
            HttpRequest request = context.HttpContext.Request;
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            //获取action名称
            string actionName = descriptor.ActionName;
            //获取Controller 名称
            string controllerName = descriptor.ControllerName;
            //获取访问的ip 及 端口
            string LoginIP = context.HttpContext.GetClientIp();
            //获取完整请求地址
            string urlReferrer = new StringBuilder()
            .Append(request.Scheme)
            .Append("://")
            .Append(request.Host)
            .Append(request.PathBase)
            .Append(request.Path)
            .Append(request.QueryString)
            .ToString();
            var authValue = request.Headers.Authorization;
            if (authValue.Count() > 0)
            {
            }
            //获取request提交的参数
            string param = string.Empty;
            string globalParam = string.Empty;
            foreach (var arg in context.ActionArguments)
            {
                string value = Newtonsoft.Json.JsonConvert.SerializeObject(arg.Value);
                param += $"{arg.Key} : {value} \r\n";
                //globalParam += value;
            }
            //globalParam = QueryString(request);
            StringBuilder SqlBuilder = new StringBuilder();
            SqlBuilder.Append($"\r\nUrl:{urlReferrer}\r\n");
            SqlBuilder.Append($"IP:{LoginIP}\r\n");
            SqlBuilder.Append($"Param:{param}\r\n");
            //SqlBuilder.Append($"GlobalParam:{globalParam}\r\n");
            logger.LogTrace(SqlBuilder.ToString());
        }
        catch (Exception ex)
        {
            logger.LogTrace("\r\nExceptionMessage:" + ex.Message + "\r\nStackTrace:" + ex.StackTrace + "\r\n");
        }
        base.OnActionExecuting(context);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private static string QueryString(HttpRequest request)
    {
        string str="";
        if (request.Method.ToLower().Equals("post"))
        {
            try
            {
                request.Body.Position = 0;
                using var requestReader = new StreamReader(request.Body);
                str = requestReader.ReadToEnd();
                request.Body.Position = 0;
            }
            catch
            {
            }
        }
        else
        {
            str= request.QueryString.Value ?? "";
        }
        return str;
    }
}
