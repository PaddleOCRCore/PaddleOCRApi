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
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OCRCoreService.Authorization;

/// <summary>
/// API Key认证中间件
/// </summary>
public class ApiKeyAuthMiddleware
{
    private const string DefaultHeaderName = "PaddleOCR-Api-Key";
    private readonly RequestDelegate next;
    private readonly ILogger<ApiKeyAuthMiddleware> logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_next"></param>
    /// <param name="_logger"></param>
    public ApiKeyAuthMiddleware(RequestDelegate _next, ILogger<ApiKeyAuthMiddleware> _logger)
    {
        next = _next;
        logger = _logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context, ApiAuthConfig config)
    {
        if (!config.enabled || !IsProtectedControllerAction(context))
        {
            await next(context);
            return;
        }

        string headerName = string.IsNullOrWhiteSpace(config.header_name) ? DefaultHeaderName : config.header_name;
        if (string.IsNullOrWhiteSpace(config.api_key))
        {
            logger.LogError("API认证已启用，但ApiAuth:api_key未配置");
            await WriteUnauthorizedAsync(context, "认证失败:服务端未配置API Key！");
            return;
        }

        if (!context.Request.Headers.TryGetValue(headerName, out var headerValues))
        {
            await WriteUnauthorizedAsync(context, "认证失败:缺少API Key！");
            return;
        }

        string? requestApiKey = headerValues.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(requestApiKey) || !ApiKeyEquals(config.api_key, requestApiKey))
        {
            await WriteUnauthorizedAsync(context, "认证失败:API Key无效！");
            return;
        }

        ClaimsIdentity identity = new ClaimsIdentity("ApiKey");
        identity.AddClaim(new Claim(ClaimTypes.Name, "ApiKeyClient"));
        context.User = new ClaimsPrincipal(identity);

        await next(context);
    }

    private static bool IsProtectedControllerAction(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var descriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (descriptor == null)
        {
            return false;
        }

        return !string.Equals(descriptor.ControllerName, "Home", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ApiKeyEquals(string expected, string actual)
    {
        byte[] expectedBytes = Encoding.UTF8.GetBytes(expected);
        byte[] actualBytes = Encoding.UTF8.GetBytes(actual);
        return expectedBytes.Length == actualBytes.Length
            && CryptographicOperations.FixedTimeEquals(expectedBytes, actualBytes);
    }

    private static Task WriteUnauthorizedAsync(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json; charset=utf-8";

        ApiResult result = new ApiResult
        {
            Status = HttpStatusCode.Unauthorized,
            Data = "",
            ErrorMessage = message
        };

        return context.Response.WriteAsJsonAsync(result);
    }
}
