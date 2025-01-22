using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;


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
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "WebAPI接口"
                });
                //Set the comments path for the swagger json and ui.  
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, "OCRCoreService.xml");
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
                c.SwaggerEndpoint($"{pathBase}/swagger/v1/swagger.json", "FaceCore API V1");
            });
        }
    }
}
