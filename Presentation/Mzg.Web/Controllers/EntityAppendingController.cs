using Microsoft.AspNetCore.Mvc;
using Mzg.Core;
using Mzg.Core.Data;
using Mzg.DataMapping;
using Mzg.Infrastructure.Utility;
using Mzg.Schema.Entity;
using Mzg.Sdk.Client;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Models;
using System;
using System.ComponentModel;
using System.Linq;

namespace Mzg.Web.Controllers
{
    /// <summary>
    /// 实体数据映射控制器
    /// </summary>
    [Route("{org}/entity/[action]")]
    public class EntityAppendingController : AuthenticatedControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IEntityMapFinder _entityMapFinder;
        private readonly IDataCreater _dataCreater;

        public EntityAppendingController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IEntityMapFinder entityMapFinder
            , IDataCreater dataCreater)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _entityMapFinder = entityMapFinder;
            _dataCreater = dataCreater;
        }

        [Description("下推单据")]
        public IActionResult AppendRecord(Guid entityid, Guid recordid)
        {
            AppendRecordModel model = new AppendRecordModel
            {
                EntityId = entityid,
                ObjectId = recordid,
                EntityMaps = _entityMapFinder.Query(n => n.Where(f => f.SourceEntityId == entityid && f.StateCode == RecordState.Enabled))
            };
            if (model.EntityMaps.NotEmpty())
            {
                var targetIds = model.EntityMaps.Select(n => n.TargetEntityId);
                model.TargetEntityMetas = _entityFinder.Query(n => n.Where(f => f.EntityId.In(targetIds)));
            }
            return View($"~/Views/Entity/{WebContext.ActionName}.cshtml", model);
        }
    }
}