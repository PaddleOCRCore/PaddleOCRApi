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
    var ocrConfig = builder.Configuration.GetSection("OCRConfig").Get<OCRConfig>();
    if (ocrConfig != null) builder.Services.AddSingleton(ocrConfig);
    //检测模型依赖注入
    builder.Services.AddSingleton<IOCRService, OCRService>();
    builder.Services.AddSingleton<OCREngine>();

    // UVDoc文档矫正服务依赖注入（条件注入）
    var uvdocConfig = builder.Configuration.GetSection("UVDocConfig").Get<UVDocConfig>();
    if (uvdocConfig != null)
    {
        builder.Services.AddSingleton(uvdocConfig);
        if (uvdocConfig.enabled)
        {
            logger.Info("UVDoc文档矫正服务已启用");
            // 注入UVDoc服务
            builder.Services.AddSingleton<IUVDocService>(sp =>
            {
                var uvdocService = new UVDocService();
                var parameter = new UVDocParameter
                {
                    enable_mkldnn = uvdocConfig.enable_mkldnn,
                    cpu_threads = uvdocConfig.cpu_threads,
                    use_gpu = uvdocConfig.use_gpu,
                    gpu_id = uvdocConfig.gpu_id,
                    gpu_mem = uvdocConfig.gpu_mem,
                    use_tensorrt = uvdocConfig.use_tensorrt
                };
                string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models", uvdocConfig.uvdoc_infer);
                if (uvdocService.Initialize(modelPath, parameter))
                {
                    logger.Info($"UVDoc引擎初始化成功 [GPU: {parameter.use_gpu}, CPU线程: {parameter.cpu_threads}]");
                }
                else
                {
                    logger.Error($"UVDoc引擎初始化失败: {uvdocService.GetLastError()}");
                }
                return uvdocService;
            });
        }
        else
        {
            logger.Info("UVDoc文档矫正服务未启用");
        }
    }

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
