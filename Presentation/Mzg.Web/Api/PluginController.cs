using Microsoft.AspNetCore.Mvc;
using Mzg.Core.Context;
using Mzg.Infrastructure.Utility;
using Mzg.Plugin;
using Mzg.Plugin.Abstractions;
using Mzg.Solution.Abstractions;
using Mzg.Web.Api.Models;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using System.ComponentModel;
using System.Linq;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 实体插件接口
    /// </summary>
    [Route("{org}/api/[controller]")]
    public class PluginController : ApiControllerBase
    {
        private readonly IEntityPluginFinder _entityPluginFinder;

        public PluginController(IWebAppContext appContext
            , IEntityPluginFinder entityPluginFinder)
            : base(appContext)
        {
            _entityPluginFinder = entityPluginFinder;
        }

        [Description("解决方案组件")]
        [HttpGet("SolutionComponents")]
        public IActionResult SolutionComponents([FromQuery]GetSolutionComponentsModel model)
        {
            var data = _entityPluginFinder.QueryPaged(x => x.Where(f => f.IsVisibled == true).Page(model.Page, model.PageSize), model.SolutionId, model.InSolution);
            if (data.Items.NotEmpty())
            {
                var result = data.Items.Select(x => (new SolutionComponentItem { ObjectId = x.EntityPluginId, Name = x.AssemblyName, LocalizedName = x.ClassName, ComponentTypeName = PluginDefaults.ModuleName, CreatedOn = x.CreatedOn })).ToList();
                return JOk(new PagedList<SolutionComponentItem>()
                {
                    CurrentPage = model.Page
                    ,
                    ItemsPerPage = model.PageSize
                    ,
                    Items = result
                    ,
                    TotalItems = data.TotalItems
                    ,
                    TotalPages = data.TotalPages
                });
            }
            return JOk(data);
        }
    }
}