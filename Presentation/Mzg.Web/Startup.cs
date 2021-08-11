using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Security;
using DevExpress.XtraReports.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mzg.Module;
using Mzg.Web.Framework.Infrastructure;
using Mzg.Web.Services;

namespace Mzg.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //支持XtraReport中的scripts ediitor
            ScriptPermissionManager.GlobalInstance = new ScriptPermissionManager(ExecutionMode.Unrestricted);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // 使用此方法向容器添加服务
        public void ConfigureServices(IServiceCollection services)
        {
            //支持跨域
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy",
            //        builder => builder.AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowAnyHeader()
            //        .AllowCredentials());
            //});
            //支持XtraReport开始
            services.AddDevExpressControls();
            services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();
            services
                .AddMvc()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
            services.ConfigureReportingServices(configurator =>
            {
                configurator.ConfigureReportDesigner(designerConfigurator =>
                {
                    designerConfigurator.RegisterDataSourceWizardConfigFileConnectionStringsProvider();
                });
                configurator.ConfigureWebDocumentViewer(viewerConfigurator =>
                {
                    viewerConfigurator.UseCachedReportSourceBuilder();
                });
                configurator.ConfigureReportDesigner(designerConfigurator =>
                {
                    designerConfigurator.EnableCustomSql();
                });

            });
            //支持XtraReport结束
            services.AddWebDefaults(Configuration);
            services.AddSession();
            services.RegisterModules();

            //支持新的编译包
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // 使用此方法配置HTTP请求管道
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)//, IModuleRegistrar moduleRegistrar)
        {
            loggerFactory.AddLog4Net("log4net.config", true);

            //支持XtraReport**************************************开始
            DevExpress.XtraReports.Configuration.Settings.Default.UserDesignerOptions.DataBindingMode = DevExpress.XtraReports.UI.DataBindingMode.ExpressionsAdvanced;
            app.UseDevExpressControls();
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");
            //});
            //支持XtraReport**********************************结束
            //app.UseHttpsRedirection();
            app.UseXmsStaticFiles(env, Configuration);
            //app.UseCookiePolicy();
            app.UseSession();
            //app.UseAuthentication();
            //app.UseExceptionHandlerMiddleWare();
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(name: "org",
            //                    template: "{org}/{controller=Home}/{action=Index}");
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //    routes.MapRoute(
            //        "home",
            //        "{org}/index",
            //                      new { controller = "home", action = "index" });
            //    routes.MapRoute("error",
            //                    "error/{action}",
            //                    new { controller = "error", action = "index" });
            //    routes.MapRoute(
            //     name: "customize",
            //     template: "{org}/{area:exists}/{controller=Home}/{action=Index}/{id?}"
            //   );
            //    routes.MapRoute(
            //        "customize_home",
            //        "{org}/customize",
            //                      new { area = "customize", controller = "home", action = "index" });
            //    routes.MapRoute(
            //        "customize_home_index",
            //        "{org}/customize/index",
            //                      new { area = "customize", controller = "home", action = "index" });
            //});
            app.UseWebDefaults();


        }
    }
}