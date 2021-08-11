using Microsoft.AspNetCore.Mvc;
using Mzg.Core.Context;
using Mzg.Data.Provider;
using Mzg.Logging.DataLog;
using Mzg.Logging.DataLog.Domain;
using Mzg.Schema.Attribute;
using Mzg.Schema.Entity;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 实体数据日志接口
    /// </summary>
    [Route("{org}/api/data/[action]")]
    public class DataLogController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IEntityLogService _entityLogService;

        public DataLogController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IEntityLogService entityLogService)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _entityLogService = entityLogService;
        }

        [Description("实体日志列表")]
        public IActionResult EntityLogs(EntityLogsModel model)
        {
            model.Attributes = _attributeFinder.FindByEntityId(model.EntityId);
            FilterContainer<EntityLog> container = FilterContainerBuilder.Build<EntityLog>();
            container.And(n => n.OrganizationId == CurrentUser.OrganizationId);
            container.And(n => n.EntityId == model.EntityId);
            if (model.OperationType.HasValue)
            {
                container.And(n => n.OperationType == model.OperationType.Value);
            }
            if (model.GetAll)
            {
                List<EntityLog> result = _entityLogService.Query(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(container)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                    );

                model.Items = result;
                model.TotalItems = result.Count;
            }
            else
            {
                PagedList<EntityLog> result = _entityLogService.QueryPaged(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(container)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                    );

                model.Items = result.Items;
                model.TotalItems = result.TotalItems;
            }
            return JOk(model);
        }

        [Description("清空实体日志")]
        [ValidateAntiForgeryToken]
        public IActionResult ClearEntityLogs(Guid entityId)
        {
            _entityLogService.Clear(entityId);
            return JOk(T["operation_success"]);
        }
    }
}