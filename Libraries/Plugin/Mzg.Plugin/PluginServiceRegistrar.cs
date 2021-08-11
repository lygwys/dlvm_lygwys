using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mzg.Infrastructure.Inject;

namespace Mzg.Plugin
{
    /// <summary>
    /// 插件模块服务注册
    /// </summary>
    public class PluginServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Plugin.IEntityPluginCreater, Plugin.EntityPluginCreater>();
            services.AddScoped<Plugin.IEntityPluginUpdater, Plugin.EntityPluginUpdater>();
            services.AddScoped<Plugin.IEntityPluginFinder, Plugin.EntityPluginFinder>();
            services.AddScoped<Plugin.IEntityPluginDeleter, Plugin.EntityPluginDeleter>();
            services.AddScoped<Plugin.IEntityPluginExecutor, Plugin.EntityPluginExecutor>();
            services.AddScoped<Plugin.IEntityPluginFileProvider, Plugin.EntityPluginFileProvider>();
            services.AddScoped<Plugin.Data.IEntityPluginRepository, Plugin.Data.EntityPluginRepository>();
            services.AddTransient(typeof(Plugin.IPluginExecutor<,>), typeof(Plugin.PluginExecutor<,>));
        }
    }
}