using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mzg.Infrastructure.Inject;

namespace Mzg.UserPersonalization
{
    /// <summary>
    /// UserPersonalization服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Data.IUserPersonalizationRepository, Data.XmsUserPersonalizationRepository>();
            services.AddScoped<Data.Xms.IUserPersonalizationRepository, Data.Xms.UserPersonalizationRepository>();
            services.AddScoped<UserPersonalization.IUserPersonalizationService, UserPersonalization.UserPersonalizationService>();
        }
    }
}