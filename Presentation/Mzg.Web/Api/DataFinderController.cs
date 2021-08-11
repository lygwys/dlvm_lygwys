﻿using Microsoft.AspNetCore.Mvc;
using Mzg.Infrastructure.Utility;
using Mzg.Schema.Attribute;
using Mzg.Schema.Entity;
using Mzg.Sdk.Abstractions.Query;
using Mzg.Sdk.Client;
using Mzg.Web.Api.Models;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 数据查询接口
    /// </summary>
    [Route("{org}/api/data/retrieve")]
    public class DataFinderController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IDataFinder _dataFinder;

        public DataFinderController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IDataFinder dataFinder)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _dataFinder = dataFinder;
        }

        [Description("查找引用记录")]
        [HttpGet("ReferencedRecord/{entityid}/{value}/{allcolumns?}")]
        public IActionResult RetrieveReferencedRecord([FromRoute]RetrieveReferencedRecordModel args)
        {
            if (args.EntityId.Equals(Guid.Empty) || args.Value.IsEmpty())
            {
                return NotFound();
            }
            var entity = _entityFinder.FindById(args.EntityId);
            if (entity == null)
            {
                return NotFound();
            }
            var entityName = entity.Name;
            QueryByAttribute qba = new QueryByAttribute(entityName, CurrentUser.UserSettings.LanguageId);
            if (args.AllColumns)
            {
                qba.ColumnSet.AllColumns = true;
            }
            else
            {
                qba.ColumnSet.AddColumns(entityName + "id", "name");
            }
            qba.Attributes.Add(entityName + "id");
            qba.Values.Add(args.Value);

            var result = _dataFinder.Retrieve(qba);
            if (result != null && result.Count > 0)
            {
                if (!args.AllColumns)
                {
                    result.AddIfNotContain("id", result[entityName + "id"]);
                    result.Remove(entityName + "id");
                }
                else
                {
                    result = DataHelper.WrapOptionName(_attributeFinder.FindByEntityId(args.EntityId), result);
                }
            }

            return JOk(result);
        }

        [Description("查找一条记录")]
        [HttpPost("single")]
        public IActionResult Retrieve(QueryExpression query)
        {
            if (query.EntityName.IsEmpty())
            {
                return JError("entityname is not specified");
            }
            var entity = _entityFinder.FindByName(query.EntityName);
            if (entity == null)
            {
                return JError("entityname is not found");
            }

            var result = _dataFinder.Retrieve(query);

            return JOk(result);
        }

        [Description("查找一条记录")]
        [HttpGet("{entityname}/{id}")]
        public IActionResult RetrieveById([FromRoute]RetrieveByIdModel args)
        {
            if (args.EntityName.IsEmpty())
            {
                return JError("entityname is not specified");
            }
            var entity = _entityFinder.FindByName(args.EntityName);
            if (entity == null)
            {
                return JError("entityname is not found");
            }

            var result = _dataFinder.RetrieveById(args.EntityName, args.Id);

            return JOk(result);
        }

        [Description("查找多条记录")]
        [HttpPost("Multiple")]
        public IActionResult RetrieveMultiple(RetrieveMultipleModel args)
        {
            if (args.Query.EntityName.IsEmpty())
            {
                return JError("entityname is not specified");
            }
            var entity = _entityFinder.FindByName(args.Query.EntityName);
            if (entity == null)
            {
                return JError("entityname is not found");
            }
            if (args.IsAll)
            {
                var result = _dataFinder.RetrieveAll(args.Query, args.IsPermission);
                return JOk(result);
            }
            else
            {
                var result = _dataFinder.RetrieveMultiple(args.Query, args.IsPermission);
                return JOk(result);
            }
        }

        /// <summary>
        /// 查找所有记录, 路由示例：/all/customer/name,leadid/name:asc
        /// </summary>
        /// <param name="entityName">实体名称</param>
        /// <param name="columns">以半角逗号分隔的查询列名，allcolumn代表查询所有列</param>
        /// <param name="orderby">排序方式, 字段:升序或降序</param>
        /// <returns></returns>
        [Description("查找所有记录")]
        [HttpGet("all/{entityname}/{columns}/{orderby?}")]
        public IActionResult RetrieveAll(string entityName, string columns, string orderby)
        {
            if (entityName.IsEmpty())
            {
                return JError("entityname is not specified");
            }
            var entity = _entityFinder.FindByName(entityName);
            if (entity == null)
            {
                return JError("entityname is not found");
            }
            List<string> columnNames = columns.IsNotEmpty() ? (columns.IsCaseInsensitiveEqual("allcolumn") ? null : columns.SplitSafe(",").ToList()) : null;
            OrderExpression order = null;
            if (orderby.IsNotEmpty())
            {
                var orderValues = orderby.SplitSafe(":");
                if (orderValues.Length > 0)
                {
                    order = new OrderExpression(orderValues[0], orderValues.Length > 1 && orderValues[1].IsCaseInsensitiveEqual("asc") ? OrderType.Ascending : OrderType.Descending);
                }
            }
            var result = _dataFinder.RetrieveAll(entityName, columnNames, order);
            return JOk(result);
        }
    }
}