using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mzg.Context;
using Mzg.Core;
using Mzg.Core.Org;
using Mzg.Identity;
using Mzg.Infrastructure;
using Mzg.Infrastructure.Inject;
using Mzg.Infrastructure.Utility;
using Mzg.Organization;
using Mzg.Web.Framework.Context;

namespace Mzg.Web.Framework.Infrastructure
{
    /// <summary>
    /// 服务注入
    /// </summary>
    public class DefaultServicesRegister : IServiceRegistrar
    {
        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            //服务解析器
            services.AddScoped<IServiceResolver, ServiceResolver>();
            //web上下文
            services.AddScoped<IAppContext, WebAppContext>();
            services.AddScoped<IWebAppContext, WebAppContext>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IOrgDataServer, OrgDataServer>();
            services.AddScoped<IWebHelper, WebHelper>();

            //exception handler
            services.AddScoped<IExceptionHandlerFactory, ExceptionHandlerFactory>();
            services.RegisterScope(typeof(IExceptionHandler<>));
        }

        public int Order => 0;
    }
}