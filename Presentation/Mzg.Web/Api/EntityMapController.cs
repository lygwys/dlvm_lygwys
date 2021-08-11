using Microsoft.AspNetCore.Mvc;
using Mzg.Core.Context;
using Mzg.DataMapping;
using Mzg.DataMapping.Abstractions;
using Mzg.Infrastructure.Utility;
using Mzg.Schema.Entity;
using Mzg.Solution.Abstractions;
using Mzg.Web.Api.Models;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using System.ComponentModel;
using System.Linq;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 单据转换规则接口
    /// </summary>
    [Route("{org}/api/entitymap")]
    public class EntityMapController : ApiControllerBase
    {
        private readonly IEntityMapFinder _entityMapFinder;
        private readonly IEntityFinder _entityFinder;

        public EntityMapController(IWebAppContext appContext
            , IEntityMapFinder entityMapFinder
            , IEntityFinder entityService)
            : base(appContext)
        {
            _entityMapFinder = entityMapFinder;
            _entityFinder = entityService;
        }

        [Description("解决方案组件")]
        [HttpGet("SolutionComponents")]
        public IActionResult SolutionComponents([FromQuery]GetSolutionComponentsModel model)
        {
            var data = _entityMapFinder.QueryPaged(x => x.Page(model.Page, model.PageSize), model.SolutionId, model.InSolution);
            if (data.Items.NotEmpty())
            {
                var entities = _entityFinder.FindAll();
                var result = data.Items.Select(x => (new SolutionComponentItem { ObjectId = x.EntityMapId, Name = x.SourceEnttiyName + "-" + x.TargetEnttiyName, LocalizedName = (entities.Find(e => e.EntityId == x.SourceEntityId).LocalizedName + "-" + entities.Find(e => e.EntityId == x.TargetEntityId).LocalizedName), ComponentTypeName = DataMappingDefaults.ModuleName, CreatedOn = x.CreatedOn })).ToList();
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