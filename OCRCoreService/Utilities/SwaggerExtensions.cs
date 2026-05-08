using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;


namespace OCRCoreService
{
    /// <summary>
    /// Swagger 扩展方法
    /// </summary>
    internal static class SwaggerExtensions
    {
        /// <summary>
        /// 注入 Swagger 服务到容器内
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            string headerName = configuration["ApiAuth:header_name"] ?? "PaddleOCR-Api-Key";
            if (string.IsNullOrWhiteSpace(headerName))
            {
                headerName = "PaddleOCR-Api-Key";
            }

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "WebAPI接口"
                });
                options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "请输入API Key",
                    Name = headerName,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiKeyScheme"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                // 启用XML注释（需生成XML文档）
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //options.IncludeXmlComments(xmlPath);
            });
        }
        /// <summary>
        /// Swagger 中间件
        /// </summary>
        /// <param name="app"></param>
        /// <param name="pathBase"></param>
        public static void UseSwaggerApp(this IApplicationBuilder app, string pathBase)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{pathBase}/swagger/v1/swagger.json", "PaddleOCRApi V1");
            });
        }
    }
}
