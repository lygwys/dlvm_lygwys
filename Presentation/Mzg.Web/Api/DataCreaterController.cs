using Microsoft.AspNetCore.Mvc;
using Mzg.Core.Data;
using Mzg.Infrastructure.Utility;
using Mzg.Schema.Attribute;
using Mzg.Schema.Entity;
using Mzg.Sdk.Client;
using Mzg.Sdk.Extensions;
using Mzg.Web.Api.Models;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Framework.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 数据新增接口
    /// </summary>
    [Route("{org}/api/data/create")]
    public class DataCreaterController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IDataCreater _dataCreater;
        private readonly IDataMapper _dataMapper;

        public DataCreaterController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IDataCreater dataCreater
            , IDataMapper dataMapper)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _dataCreater = dataCreater;
            _dataMapper = dataMapper;
        }

        [Description("创建记录")]
        [HttpPost]
        public IActionResult Post(CreateRecordModel model)
        {
            if (model.Data.IsEmpty())
            {
                return JError("data is empty");
            }
            Schema.Domain.Entity entityMeta = null;
            if (model.EntityId.HasValue && !model.EntityId.Value.Equals(Guid.Empty))
            {
                entityMeta = _entityFinder.FindById(model.EntityId.Value);
            }
            else if (model.EntityName.IsNotEmpty())
            {
                entityMeta = _entityFinder.FindByName(model.EntityName);
            }
            if (entityMeta == null)
            {
                return NotFound();
            }
            var childAttributes = _attributeFinder.FindByEntityName(entityMeta.Name);
            if (model.Data.StartsWith("["))
            {
                var details = new List<Entity>();
                var items = JArray.Parse(model.Data.UrlDecode());
                if (items.Count > 0)
                {
                    foreach (var c in items)
                    {
                        dynamic root = JObject.Parse(c.ToString());
                        Entity detail = new Entity(entityMeta.Name);
                        foreach (JProperty p in root)
                        {
                            var attr = childAttributes.Find(n => n.Name.IsCaseInsensitiveEqual(p.Name));
                            if (attr != null && p.Value != null)
                            {
                                detail.SetAttributeValue(p.Name.ToString().ToLower(), detail.WrapAttributeValue(_entityFinder, attr, p.Value.ToString()));
                            }
                        }
                        details.Add(detail);
                    }
                }
                return _dataCreater.CreateMany(details, model.IsPermission).CreateResult(T);
            }
            else
            {
                dynamic root = JObject.Parse(model.Data.UrlDecode());
                Entity detail = new Entity(entityMeta.Name);
                foreach (JProperty p in root)
                {
                    var attr = childAttributes.Find(n => n.Name.IsCaseInsensitiveEqual(p.Name));
                    if (attr != null)
                    {
                        detail.SetAttributeValue(p.Name.ToString().ToLower(), detail.WrapAttributeValue(_entityFinder, attr, p.Value.ToString()));
                    }
                }
                var id = _dataCreater.Create(detail, model.IsPermission);
                return CreateSuccess(new { id = id });
            }
        }

        /// <summary>
        /// 单据转换设置完成后，在此进行赋值映射
        /// xmg
        /// 202005311221
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Description("从实体映射新建记录")]
        [HttpPost("map")]
        public IActionResult Map(CreateFromMapModel args)
        {
            if (!args.SourceEntityId.HasValue || args.SourceEntityId.Value.Equals(Guid.Empty))
            {
                if (args.SourceEntityName.IsNotEmpty())
                {
                    var entityMeta = _entityFinder.FindByName(args.SourceEntityName);
                    args.SourceEntityId = entityMeta.EntityId;
                }
            }
            if (!args.TargetEntityId.HasValue || args.TargetEntityId.Value.Equals(Guid.Empty))
            {
                if (args.TargetEntityName.IsNotEmpty())
                {
                    var entityMeta = _entityFinder.FindByName(args.TargetEntityName);
                    args.TargetEntityId = entityMeta.EntityId;
                }
            }
            if (!args.SourceEntityId.HasValue || !args.TargetEntityId.HasValue || args.SourceRecordId.Equals(Guid.Empty))
            {
                return NotSpecifiedRecord();
            }
            var newId = _dataMapper.Create(args.SourceEntityId.Value, args.TargetEntityId.Value, args.SourceRecordId);

            return CreateSuccess(new { entityid = args.TargetEntityId, id = newId });
        }
    }
}