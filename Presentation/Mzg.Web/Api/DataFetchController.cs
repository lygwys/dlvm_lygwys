using Microsoft.AspNetCore.Mvc;
using Mzg.Authorization.Abstractions;
using Mzg.Core.Components.Platform;
using Mzg.Infrastructure.Utility;
using Mzg.QueryView;
using Mzg.QueryView.Abstractions;
using Mzg.Schema.RelationShip;
using Mzg.Sdk.Abstractions.Query;
using Mzg.Sdk.Client;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Models;
using System;
using System.ComponentModel;
using System.Linq;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 数据查询接口
    /// </summary>
    [Route("{org}/api/data/fetch")]
    public class DataFetchController : ApiControllerBase
    {
        private readonly IQueryViewFinder _queryViewFinder;
        private readonly IRelationShipFinder _relationShipFinder;
        private readonly IFetchDataService _fetchService;
        private readonly IRoleObjectAccessService _roleObjectAccessService;

        public DataFetchController(IWebAppContext appContext
            , IQueryViewFinder queryViewService
            , IRelationShipFinder relationShipFinder
            , IRoleObjectAccessService roleObjectAccessService
            , IFetchDataService fetchDataService)
            : base(appContext)
        {
            _queryViewFinder = queryViewService;
            _relationShipFinder = relationShipFinder;
            _fetchService = fetchDataService;
            _roleObjectAccessService = roleObjectAccessService;
        }

        /// <summary>
        /// 查询视图数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description("查询视图数据")]
        [HttpPost]
        public IActionResult Post([FromBody]EntityGridModel model)
        {
            QueryView.Domain.QueryView queryView = null;
            if (model.QueryViewId.HasValue && !model.QueryViewId.Equals(Guid.Empty))
            {
                queryView = _queryViewFinder.FindById(model.QueryViewId.Value);
            }
            else if (model.EntityId.HasValue && !model.EntityId.Value.Equals(Guid.Empty))
            {
                queryView = _queryViewFinder.FindEntityDefaultView(model.EntityId.Value);
            }
            else if (model.EntityName.IsNotEmpty())
            {
                queryView = _queryViewFinder.FindEntityDefaultView(model.EntityName);
            }
            else
            {
                return NotFound();
            }
            if (queryView == null)
            {
                return NotFound();
            }
            if (!queryView.IsDefault && queryView.AuthorizationEnabled)
            {
                if (!_roleObjectAccessService.Exists(queryView.QueryViewId, QueryViewDefaults.ModuleName, CurrentUser.Roles.Select(n => n.RoleId).ToArray()))
                {
                    return Unauthorized();
                }
            }
            model.QueryView = queryView;
            model.EntityId = queryView.EntityId;

            FetchDescriptor fetch = new FetchDescriptor
            {
                Page = model.Page,
                PageSize = model.PageSize,
                FetchConfig = queryView.FetchConfig,
                GetAll = !model.PagingEnabled
            };
            if (model.IsSortBySeted)
            {
                QueryColumnSortInfo sort = new QueryColumnSortInfo(model.SortBy, model.SortDirection == 0);
                fetch.Sort = sort;
            }
            //查询关键字
            if (model.Q.IsNotEmpty())
            {
                fetch.Keyword = model.Q;
                fetch.Field = model.QField;
            }
            var canFetch = true;
            //filter by relationship
            if (model.RelationShipName.IsNotEmpty())
            {
                var relationship = _relationShipFinder.FindByName(model.RelationShipName);
                if (relationship != null && model.ReferencedRecordId.HasValue && !model.ReferencedRecordId.Value.Equals(Guid.Empty))
                {
                    if (model.Filter == null)
                    {
                        model.Filter = new FilterExpression();
                    }
                    var condition = new ConditionExpression(relationship.ReferencingAttributeName, ConditionOperator.Equal, model.ReferencedRecordId);
                    model.Filter.AddCondition(condition);
                }
                model.RelationShipMeta = relationship;
            }
            if (model.RelationShipName.IsNotEmpty() && (!model.ReferencedRecordId.HasValue || model.ReferencedRecordId.Value.Equals(Guid.Empty)))
            {
                //如果被引用ID为空，则不查询数据
                canFetch = false;
            }
            fetch.Filter = model.Filter;
            if (canFetch)
            {
                fetch.User = CurrentUser;
                var datas = _fetchService.Execute(fetch);
                return Content(datas.SerializeToJson());
            }
            if (!model.IsSortBySeted)
            {
                if (_fetchService.QueryExpression.Orders.NotEmpty())
                {
                    model.SortBy = _fetchService.QueryExpression.Orders[0].AttributeName;
                    model.SortDirection = (int)_fetchService.QueryExpression.Orders[0].OrderType;
                }
            }
            return View($"~/Views/Entity/{WebContext.ActionName}.cshtml", model);
        }
    }
}