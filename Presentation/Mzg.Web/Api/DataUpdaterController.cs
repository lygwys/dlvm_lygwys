using Microsoft.AspNetCore.Mvc;
using Mzg.Core.Data;
using Mzg.Infrastructure.Utility;
using Mzg.Schema.Abstractions;
using Mzg.Schema.Attribute;
using Mzg.Schema.Entity;
using Mzg.Schema.Extensions;
using Mzg.Sdk.Client;
using Mzg.Sdk.Extensions;
using Mzg.Web.Api.Models;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Framework.Mvc;
using Mzg.Web.Models;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 数据更新接口
    /// </summary>
    [Route("{org}/api/data/update")]
    public class DataUpdatererController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IDataUpdater _dataUpdater;

        public DataUpdatererController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IDataUpdater dataUpdater)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _dataUpdater = dataUpdater;
        }

        [Description("更新记录")]
        [HttpPost]
        public IActionResult Post(DataUpdateModel model)
        {
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
            var primaryAttr = childAttributes.Find(n => n.TypeIsPrimaryKey());
            if (model.Data.StartsWith("["))
            {
                //var details = new List<Entity>();
                var items = JArray.Parse(model.Data.UrlDecode());
                if (items.Count > 0)
                {
                    foreach (var c in items)
                    {
                        dynamic root = JObject.Parse(c.ToString());
                        Entity detail = new Entity(entityMeta.Name);
                        foreach (JProperty p in root)
                        {
                            if (p.Name.IsCaseInsensitiveEqual("id"))
                            {
                                detail.SetIdValue(Guid.Parse(p.Value.ToString()), primaryAttr.Name);
                            }
                            else
                            {
                                var attr = childAttributes.Find(n => n.Name.IsCaseInsensitiveEqual(p.Name));
                                if (attr != null)
                                {
                                    detail.SetAttributeValue(p.Name.ToString().ToLower(), detail.WrapAttributeValue(_entityFinder, attr, p.Value.ToString()));
                                }
                            }
                        }
                        //details.Add(detail);
                        _dataUpdater.Update(detail, model.IsPermission);
                    }
                }
                //_organizationServiceProxy.UpdateMany(details);
                return UpdateSuccess();
            }
            else
            {
                Entity detail = new Entity(entityMeta.Name);
                dynamic root = JObject.Parse(model.Data.UrlDecode());
                foreach (JProperty p in root)
                {
                    if (p.Name.IsCaseInsensitiveEqual("id"))
                    {
                        detail.SetIdValue(Guid.Parse(p.Value.ToString()), primaryAttr.Name);
                    }
                    else
                    {
                        var attr = childAttributes.Find(n => n.Name.IsCaseInsensitiveEqual(p.Name));
                        if (attr != null)
                        {
                            detail.SetAttributeValue(p.Name.ToString().ToLower(), detail.WrapAttributeValue(_entityFinder, attr, p.Value.ToString()));
                        }
                    }
                }
                //如果是多条件更新,
                if (model.Criteria != null)
                {   //如果设置了条件
                    if (model.Criteria.Conditions.Count > 0)
                    {
                        _dataUpdater.UpdateCri(detail, model.Criteria, model.IsPermission);
                    }
                    else
                        return NotFound();

                }
                else//如果是单一条件
                {
                    _dataUpdater.Update(detail, model.IsPermission);
                }
            }
            return UpdateSuccess();
        }

        [Description("更改记录状态")]
        [HttpPost("state")]
        public IActionResult State(SetEntityRecordStateModel model)
        {
            if (model.RecordId.IsEmpty())
            {
                return NotSpecifiedRecord();
            }
            var entityMeta = _entityFinder.FindById(model.EntityId);
            var primaryKey = _attributeFinder.Find(x => x.EntityId == model.EntityId && x.AttributeTypeName == AttributeTypeIds.PRIMARYKEY);
            var result = false;
            foreach (var item in model.RecordId)
            {
                Entity entity = new Entity(entityMeta.Name);
                entity.SetIdName(primaryKey.Name);
                entity.SetIdValue(item);
                entity.SetAttributeValue("statecode", (int)model.State);
                result = _dataUpdater.Update(entity);
            }
            return result.UpdateResult(T);
        }
    }
}