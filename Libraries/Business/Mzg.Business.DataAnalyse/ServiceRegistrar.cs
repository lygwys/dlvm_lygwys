using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mzg.Infrastructure.Inject;

namespace Mzg.Business.DataAnalyse
{
    /// <summary>
    /// 数据分析模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Business.DataAnalyse.Visualization.IChartCreater, Business.DataAnalyse.Visualization.ChartCreater>();
            services.AddScoped<Business.DataAnalyse.Visualization.IChartUpdater, Business.DataAnalyse.Visualization.ChartUpdater>();
            services.AddScoped<Business.DataAnalyse.Visualization.IChartFinder, Business.DataAnalyse.Visualization.ChartFinder>();
            services.AddScoped<Business.DataAnalyse.Visualization.IChartDeleter, Business.DataAnalyse.Visualization.ChartDeleter>();
            services.AddScoped<Business.DataAnalyse.Visualization.IChartBuilder, Business.DataAnalyse.Visualization.ChartBuilder>();
            services.AddScoped<Business.DataAnalyse.Visualization.IChartDependency, Business.DataAnalyse.Visualization.ChartDependency>();
            services.AddScoped<Business.DataAnalyse.Report.IReportService, Business.DataAnalyse.Report.ReportService>();
            // 当对类库名称修改后要单独注册此类库下的服务,同时在startupExtensions.cs增加了注册方法
            // xmg
            // 202006101108
            services.AddScoped<Business.DataAnalyse.Data.IChartRepository, Business.DataAnalyse.Data.ChartRepository>();
            services.AddScoped<Business.DataAnalyse.Data.IReportRepository, Business.DataAnalyse.Data.ReportRepository>();


        }
    }
}