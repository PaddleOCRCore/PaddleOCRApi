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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Text;

namespace OCRCoreService.Authorization;

/// <summary>
/// 
/// </summary>
public class ApiErrorHandleAttribute : ExceptionFilterAttribute
{
    private readonly ILogger<ApiErrorHandleAttribute> logger;
    private readonly IServiceProvider provider;
    private readonly IConfiguration configuration;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_logger"></param>
    /// <param name="_provider"></param>
    /// <param name="_configuration"></param>
    public ApiErrorHandleAttribute(ILogger<ApiErrorHandleAttribute> _logger, IServiceProvider _provider,
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
    /// 重写基类的异常处理方法
    /// </summary>
    /// <param name="context"></param>
    public override void OnException(ExceptionContext context)
    {
        base.OnException(context);
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
        string openid = "";
        if (context.HttpContext.User.Identity!.IsAuthenticated)
        {
            openid = context.HttpContext.User.Identity.Name ?? "";
        }
        // 取得发生例外时的错误讯息
        var errorMessage = context.Exception.Message;
        logger.LogError("\r\nUrl:" + urlReferrer + "\r\nOpenid:" + openid + "\r\nIPAddress:" + LoginIP + "\r\nMessage:" + errorMessage + "\r\nStackTrace：" + context.Exception.StackTrace, context.Exception);
        ApiResult result = new ApiResult();
        result.Status = HttpStatusCode.BadRequest;
        result.ErrorMessage = errorMessage;
        context.Result = new ObjectResult(result);
    }
}
