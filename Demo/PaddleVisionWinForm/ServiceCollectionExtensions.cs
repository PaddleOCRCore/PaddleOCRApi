using Microsoft.Extensions.DependencyInjection;
using PaddleOCRSDK;

namespace PaddleVisionWinForm
{
    /// <summary>
    /// 服务集合扩展方法
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加UVDoc服务（单例模式）
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns></returns>
        public static IServiceCollection AddUVDocService(this IServiceCollection services)
        {
            services.AddSingleton<IUVDocService, UVDocService>();
            return services;
        }

        /// <summary>
        /// 添加UVDoc服务（作用域模式）
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns></returns>
        public static IServiceCollection AddUVDocServiceScoped(this IServiceCollection services)
        {
            services.AddScoped<IUVDocService, UVDocService>();
            return services;
        }

        /// <summary>
        /// 添加UVDoc服务（瞬态模式）
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns></returns>
        public static IServiceCollection AddUVDocServiceTransient(this IServiceCollection services)
        {
            services.AddTransient<IUVDocService, UVDocService>();
            return services;
        }
    }
}
