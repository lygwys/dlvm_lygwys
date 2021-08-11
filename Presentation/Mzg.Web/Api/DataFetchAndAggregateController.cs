using Microsoft.AspNetCore.Mvc;
using Mzg.Authorization.Abstractions;
using Mzg.Core.Components.Platform;
using Mzg.Infrastructure.Utility;
using Mzg.QueryView;
using Mzg.QueryView.Abstractions;
using Mzg.Schema.Attribute;
using Mzg.Schema.RelationShip;
using Mzg.Sdk.Abstractions.Query;
using Mzg.Sdk.Client;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 查询视图数据
    /// xmg
    /// 202008211553
    /// </summary>
    [Route("{org}/api/data/fetchAndAggregate")]
    [ApiController]
    public class DataFetchAndAggregateController : ApiControllerBase
    {
        private readonly IQueryViewFinder _queryViewFinder;
        private readonly IRelationShipFinder _relationShipFinder;
        private readonly IFetchDataService _fetchService;
        private readonly IRoleObjectAccessService _roleObjectAccessService;
        private readonly IAggregateService _aggregateService;
        private readonly IAttributeFinder _attributeFinder;

        public DataFetchAndAggregateController(IWebAppContext appContext
            , IQueryViewFinder queryViewService
            , IRelationShipFinder relationShipFinder
            , IRoleObjectAccessService roleObjectAccessService
            , IFetchDataService fetchDataService
            , IAggregateService aggregateService
            , IAttributeFinder attributeFinder)
            : base(appContext)
        {
            _queryViewFinder = queryViewService;
            _relationShipFinder = relationShipFinder;
            _fetchService = fetchDataService;
            _roleObjectAccessService = roleObjectAccessService;
            _aggregateService = aggregateService;
            _attributeFinder = attributeFinder;
        }

        /// <summary>
        /// 查询视图数据和统计数据，子表数据的查询
        /// 通过pqgrid.dev.js中this.xhr = $.ajax(进入此方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description("查询视图数据和统计数据")]
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
                var fetchDatas = _fetchService.Execute(fetch);//查询数据服务在fetchdataservice.cs中
                object aggregateDatas = null;
                if (queryView.AggregateConfig.IsNotEmpty())
                {
                    var aggFields = new List<AggregateExpressionField>().DeserializeFromJson(queryView.AggregateConfig);
                    if (aggFields.NotEmpty())
                    {
                        var queryExp = new QueryExpression().DeserializeFromJson(queryView.FetchConfig);
                        if (model.Filter != null
                            && (model.Filter.Conditions.NotEmpty() || (model.Filter.Filters.NotEmpty() && model.Filter.Filters.First().Conditions.NotEmpty())))
                        {
                            queryExp.Criteria.AddFilter(model.Filter);
                        }
                        var aggExp = new AggregateExpression
                        {
                            ColumnSet = queryExp.ColumnSet,
                            Criteria = queryExp.Criteria,
                            EntityName = queryExp.EntityName,
                            LinkEntities = queryExp.LinkEntities,
                            AggregateFields = aggFields
                        };
                        var aggDatas = _aggregateService.Execute(aggExp);
                        var attributes = _attributeFinder.FindByName(queryView.EntityId, aggFields.Select(x => x.AttributeName).ToArray());
                        foreach (dynamic item in aggDatas)
                        {
                            var line = item as IDictionary<string, object>;
                            var attribute = attributes.Find(x => x.Name.IsCaseInsensitiveEqual(line.Keys.First()));
                            item.metadata = new { attribute.Name, attribute.LocalizedName, attribute.AttributeTypeName, attribute.EntityId, attribute.EntityName, attribute.EntityLocalizedName };
                            item.aggregatetype = aggFields.Find(x => x.AttributeName.IsCaseInsensitiveEqual(attribute.Name)).AggregateType;
                        }
                        aggregateDatas = new { View = new { queryView.QueryViewId, queryView.Name }, Data = aggDatas };
                    }
                }
                return Content(new { FetchData = fetchDatas, AggregateData = aggregateDatas }.SerializeToJson());
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