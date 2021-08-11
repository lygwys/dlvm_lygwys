using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mzg.Infrastructure.Inject;

namespace Mzg.Business.FormStateRule
{
    /// <summary>
    /// 表单状态规则模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Business.FormStateRule.ISystemFormStateRuleService, Business.FormStateRule.SystemFormStateRuleService>();
            services.AddScoped<Business.FormStateRule.ISystemFormStatusSetter, Business.FormStateRule.SystemFormStatusSetter>();
            // 当对类库名称修改后要单独注册此类库下的服务,同时在startupExtensions.cs增加了注册方法
            // xmg
            // 202006101118
            services.AddScoped<Business.FormStateRule.Data.ISystemFormStateRuleRepository, Business.FormStateRule.Data.SystemFormStateRuleRepository>();


        }
    }
}