using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mzg.Core;
using Mzg.Infrastructure.Inject;
using Mzg.Infrastructure.Utility;

namespace Mzg.Data
{
    /// <summary>
    /// 数据处理模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Abstractions.DataBaseOptions>(configuration.GetSection("DataBase"));
            //repositories
            services.AddScoped<Core.Data.IDbContext, Data.DbContext>();
            services.AddScoped<Data.Abstractions.IDataProviderOptions, Data.Abstractions.XmsDbConfiguration>();
            var repositories = AssemblyHelper.GetClassOfType(typeof(Core.Data.IRepository<>), "Mzg.*.dll");
            foreach (var repos in repositories)
            {
                if (repos.IsAbstract) continue;
                var it = repos.GetInterfaces();
                foreach (var i in it)
                {
                    if (i.Name == typeof(Core.Data.IRepository<>).Name) continue;
                    services.AddScoped(i, repos);
                }
            }
            //cascade delete
            services.RegisterScope(typeof(Core.Data.ICascadeDelete<>));
        }
    }
}