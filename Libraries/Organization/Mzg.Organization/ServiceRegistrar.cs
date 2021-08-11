using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mzg.Infrastructure.Inject;

namespace Mzg.Organization
{
    /// <summary>
    /// 组织模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Organization.Data.IOrganizationBaseRepository, Organization.Data.OrganizationBaseRepository>();
            services.AddScoped<Organization.Data.ISystemUserSettingsRepository, Organization.Data.SystemUserSettingsRepository>();
            services.AddScoped<Organization.Data.ITeamMembershipRepository, Organization.Data.TeamMembershipRepository>();
            services.AddScoped<Organization.Data.IBusinessUnitRepository, Organization.Data.BusinessUnitRepository>();
            services.AddScoped<Organization.Data.IOrganizationRepository, Organization.Data.OrganizationRepository>();
            services.AddScoped<Organization.Data.ISystemUserRepository, Organization.Data.SystemUserRepository>();
            services.AddScoped<Organization.Data.ITeamRepository, Organization.Data.TeamRepository>();
            services.AddScoped<Organization.IOrganizationBaseService, Organization.OrganizationBaseService>();
            services.AddScoped<Organization.IOrganizationService, Organization.OrganizationService>();
            services.AddScoped<Organization.IBusinessUnitService, Organization.BusinessUnitService>();
            services.AddScoped<Organization.ISystemUserService, Organization.SystemUserService>();
        }
    }
}