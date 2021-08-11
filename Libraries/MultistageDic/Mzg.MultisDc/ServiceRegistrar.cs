using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mzg.Infrastructure.Inject;

namespace Mzg.MultisDc
{
    /// <summary>
    /// 菜单安全模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<MultisDc.IMultistagedcService, MultisDc.MultistagedcService>();
            services.AddScoped<MultisDc.IMultistagedcTreeBuilder, MultisDc.MultistagedcTreeBuilder>();
            services.AddScoped<MultisDc.IMultistagedcFinder, MultisDc.MultistagedcFinder>();
            services.AddScoped<MultisDc.Data.IMultistagedcRepository, MultisDc.Data.MultistagedcRepository>();
        }
    }
}
