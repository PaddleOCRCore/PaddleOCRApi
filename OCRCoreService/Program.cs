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

using OCRCoreService.Authorization;
using OCRCoreService;
using Microsoft.AspNetCore.HttpOverrides;
using NLog;
using NLog.Web;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using PaddleOCRSDK;
using OCRCoreService.Services;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseNLog();
    // Add services to the container.
    //builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    //检测模型依赖注入
    builder.Services.AddSingleton<IOCRService, OCRService>();
    builder.Services.AddSingleton<OCREngine>();

    // 网页显示中文
    builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    //使用本地缓存必须添加
    builder.Services.AddMemoryCache();
    //添加Api全局过滤
    builder.Services.AddControllersWithViews(options =>
    {
        //options.Filters.Add<WebApiActionAttribute>();//改为在接口中单独引用，上传文件接口无法使用此方法
        options.Filters.Add<ApiErrorHandleAttribute>();
    });
    builder.Services.AddSwagger();


    var app = builder.Build();

    var fordwardedHeaderOptions = new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    };
    fordwardedHeaderOptions.KnownNetworks.Clear();
    fordwardedHeaderOptions.KnownProxies.Clear();
    app.UseForwardedHeaders(fordwardedHeaderOptions);

    if (builder.Configuration.GetValue("UseHttps", true)) app.UseHttpsRedirection();
    var pathBase = builder.Configuration["SwaggerPathBase"]?.TrimEnd('/') ?? "";
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseSwaggerApp(pathBase);
    }
    else
    {
        app.UseDeveloperExceptionPage();
        app.UseSwaggerApp(pathBase);
    }
    app.UseStaticFiles();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapDefaultControllerRoute();
    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
