using Microsoft.AspNetCore.Mvc;
using Mzg.Core.Context;
using Mzg.Core.Data;
using Mzg.Data.Provider;
using Mzg.Infrastructure.Utility;
using Mzg.Schema;
using Mzg.Schema.Abstractions;
using Mzg.Schema.Attribute;
using Mzg.Schema.Entity;
using Mzg.Schema.Extensions;
using Mzg.Schema.OptionSet;
using Mzg.Schema.StringMap;
using Mzg.Sdk.Abstractions.Query;
using Mzg.Solution;
using Mzg.Web.Customize.Models;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Models;
using Mzg.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Mzg.Web.Customize.Controllers
{
    /// <summary>
    /// 字段管理控制器
    /// </summary>
    public class AttributeController : CustomizeBaseController
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IOptionSetFinder _optionSetFinder;
        private readonly IOptionSetDetailFinder _optionSetDetailFinder;
        private readonly IStringMapFinder _stringMapFinder;
        private readonly IAttributeCreater _attributeCreater;
        private readonly IAttributeDeleter _attributeDeleter;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IAttributeUpdater _attributeUpdater;
        private readonly IMetadataService _metadataService;

        public AttributeController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityFinder entityFinder
            , IOptionSetFinder optionSetFinder
            , IOptionSetDetailFinder optionSetDetailFinder
            , IStringMapFinder stringMapFinder
            , IAttributeCreater attributeCreater
            , IAttributeDeleter attributeDeleter
            , IAttributeFinder attributeFinder
            , IAttributeUpdater attributeUpdater
            , IMetadataService metadataService)
            : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;
            _optionSetFinder = optionSetFinder;
            _optionSetDetailFinder = optionSetDetailFinder;
            _stringMapFinder = stringMapFinder;
            _attributeCreater = attributeCreater;
            _attributeDeleter = attributeDeleter;
            _attributeFinder = attributeFinder;
            _attributeUpdater = attributeUpdater;
            _metadataService = metadataService;
        }

        [Description("字段列表")]
        public IActionResult Index(AttributeModel model)
        {

            if (model.EntityId.Equals(Guid.Empty))
            {
                return NotFound();
            }
            var entity = _entityFinder.FindById(model.EntityId);
            if (entity == null)
            {
                return NotFound();
            }
            model.Entity = entity;
            if (!model.LoadData)
            {
                return DynamicResult(model);
            }

            FilterContainer<Schema.Domain.Attribute> container = FilterContainerBuilder.Build<Schema.Domain.Attribute>();
            container.And(n => n.EntityId == model.EntityId);
            if (model.Name.IsNotEmpty())
            {
                container.And(n => n.Name.Like(model.Name));
            }
            if (model.AttributeTypeName != null && model.AttributeTypeName.Length > 0)
            {
                container.And(n => n.AttributeTypeName.In(model.AttributeTypeName));
            }
            if (model.FilterSysAttribute)
            {
                container.And(n => n.Name.NotIn(AttributeDefaults.SystemAttributes));
                container.And(n => n.AttributeTypeName != AttributeTypeIds.PRIMARYKEY);
            }
            if (!model.IsSortBySeted)
            {
                model.SortBy = "name";
                model.SortDirection = (int)SortDirection.Asc;
            }
            if (model.GetAll)
            {
                model.Page = 1;
                model.PageSize = WebContext.PlatformSettings.MaxFetchRecords;
            }
            else if (!model.PageSizeBySeted && CurrentUser.UserSettings.PagingLimit > 0)
            {
                model.PageSize = CurrentUser.UserSettings.PagingLimit;
            }
            model.PageSize = model.PageSize > WebContext.PlatformSettings.MaxFetchRecords ? WebContext.PlatformSettings.MaxFetchRecords : model.PageSize;
            PagedList<Schema.Domain.Attribute> result = _attributeFinder.QueryPaged(x => x
                .Page(model.Page, model.PageSize)
                .Where(container)
                .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                );

            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            model.SolutionId = SolutionId.Value;
            return DynamicResult(model);
        }

        [HttpGet]
        [Description("在前台显示的字段列表")]
        public IActionResult FieldList(Guid? id)
        {
            AttributeModel model = new AttributeModel();
            if (id.HasValue && !id.Equals(Guid.Empty))
            {
                Mzg.Schema.Domain.Attribute entity = _attributeFinder.FindById(id.Value);
                if (IsRequestJson)
                {
                    return JOk(entity);
                }
            }

            return JOk(null);
        }



        [Description("检查字段是否已存在")]
        public IActionResult Exists(Guid entityid, string name)
        {
            var isExists = _attributeFinder.IsExists(entityid, name);
            if (isExists)
            {
                return JError(T["ATTRIBUTE_NAME_EXISTS"]);
            }
            return JOk("");
        }

        [HttpGet]
        [Description("新建字段")]
        public IActionResult CreateAttribute(Guid entityid)
        {
            if (entityid.Equals(Guid.Empty))
            {
                return NotFound();
            }
            CreateAttributeModel model = new CreateAttributeModel();
            model.SolutionId = SolutionId.Value;
            model.Entity = _entityFinder.FindById(entityid);
            return View(model);
        }


        [ValidateAntiForgeryToken]
        [Description("新建字段")]
        public IActionResult CreateAttribute(CreateAttributeModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _entityFinder.FindById(model.EntityId);
                if (entity == null)
                {
                    return NotFound();
                }
                var attr = _attributeFinder.Find(model.EntityId, model.Name);
                if (attr != null)
                {
                    return JError(T["attribute_name_exists"]);
                }
                var attrInfo = new Schema.Domain.Attribute();
                //model.CopyTo(entity);
                attrInfo.EntityId = entity.EntityId;
                attrInfo.EntityName = entity.Name;
                attrInfo.Name = model.Name.Trim();
                attrInfo.LocalizedName = model.LocalizedName;
                attrInfo.AttributeId = Guid.NewGuid();
                attrInfo.IsNullable = model.IsNullable;
                attrInfo.IsRequired = model.IsRequired;
                attrInfo.LogEnabled = model.LogEnabled;
                attrInfo.IsCustomizable = true;
                attrInfo.IsCustomField = true;
                attrInfo.AuthorizationEnabled = model.AuthorizationEnabled;
                attrInfo.CreatedBy = CurrentUser.SystemUserId;
                attrInfo.Description = model.Description;
                attrInfo.ValueType = model.ValueType;
                switch (model.AttributeType)
                {
                    case AttributeTypeIds.NVARCHAR:
                        attrInfo.MaxLength = model.MaxLength.Value;
                        attrInfo.DataFormat = model.TextFormat;
                        attrInfo.DefaultValue = model.DefaultValue;
                        if (model.ValueType == 2)
                        {
                            attrInfo.FormulaExpression = model.FormulaExpression;
                        }
                        break;

                    case AttributeTypeIds.NTEXT:
                        attrInfo.DataFormat = model.NTextFormat;
                        attrInfo.DefaultValue = model.DefaultValue;
                        break;

                    case AttributeTypeIds.INT:
                        attrInfo.MinValue = model.IntMinValue.Value <= int.MinValue ? int.MinValue : model.IntMinValue.Value;
                        attrInfo.MaxValue = model.IntMaxValue.Value >= int.MaxValue ? int.MaxValue : model.IntMaxValue.Value;
                        attrInfo.DefaultValue = model.DefaultValue;
                        if (model.ValueType == 2)
                        {
                            attrInfo.FormulaExpression = model.FormulaExpression;
                        }
                        else if (model.ValueType == 3)
                        {
                            attrInfo.SummaryEntityId = model.SummaryEntityId;
                            attrInfo.SummaryExpression = model.SummaryExpression;
                        }
                        break;

                    case AttributeTypeIds.FLOAT:
                        attrInfo.Precision = model.FloatPrecision.Value;
                        attrInfo.MinValue = model.FloatMinValue.Value <= decimal.MinValue ? decimal.MinValue : model.FloatMinValue.Value;
                        attrInfo.MaxValue = model.FloatMaxValue.Value >= decimal.MaxValue ? decimal.MaxValue : model.FloatMaxValue.Value;
                        attrInfo.DefaultValue = model.DefaultValue;
                        if (model.ValueType == 2)
                        {
                            attrInfo.FormulaExpression = model.FormulaExpression;
                        }
                        else if (model.ValueType == 3)
                        {
                            attrInfo.SummaryEntityId = model.SummaryEntityId;
                            attrInfo.SummaryExpression = model.SummaryExpression;
                        }
                        break;

                    case AttributeTypeIds.MONEY:
                        attrInfo.Precision = model.MoneyPrecision.Value;
                        attrInfo.MinValue = decimal.Parse(model.MoneyMinValue.Value.ToString()) <= decimal.Parse("-922337203685477.5808") ? decimal.Parse("-922337203685477.5808") : decimal.Parse(model.MoneyMinValue.Value.ToString());
                        attrInfo.MaxValue = decimal.Parse(model.MoneyMaxValue.Value.ToString()) >= decimal.Parse("922337203685477.5807") ? decimal.Parse("922337203685477.5807") : decimal.Parse(model.MoneyMaxValue.Value.ToString());
                        attrInfo.DefaultValue = model.DefaultValue;
                        if (model.ValueType == 2)
                        {
                            attrInfo.FormulaExpression = model.FormulaExpression;
                        }
                        else if (model.ValueType == 3)
                        {
                            attrInfo.SummaryEntityId = model.SummaryEntityId;
                            attrInfo.SummaryExpression = model.SummaryExpression;
                        }
                        break;

                    case AttributeTypeIds.PICKLIST:
                        attrInfo.DisplayStyle = model.OptionSetType;
                        if (model.IsCommonOptionSet)
                        {
                            attrInfo.OptionSetId = model.CommonOptionSet.Value;
                        }
                        else
                        {
                            if (model.OptionSetName.IsEmpty())
                            {
                                return JError(T["attribute_options_empty"]);
                            }
                            //新建选项集
                            Schema.Domain.OptionSet os = new Schema.Domain.OptionSet();
                            os.OptionSetId = Guid.NewGuid();
                            os.Name = model.Name;
                            os.IsPublic = false;
                            List<Schema.Domain.OptionSetDetail> details = new List<Schema.Domain.OptionSetDetail>();
                            int i = 0;
                            foreach (var item in model.OptionSetName)
                            {
                                if (item.IsEmpty())
                                {
                                    continue;
                                }

                                Schema.Domain.OptionSetDetail osd = new Schema.Domain.OptionSetDetail();
                                osd.OptionSetDetailId = Guid.NewGuid();
                                osd.OptionSetId = os.OptionSetId;
                                osd.Name = item;
                                osd.Value = model.OptionSetValue[i];
                                osd.IsSelected = model.IsSelectedOption[i];
                                osd.DisplayOrder = i;
                                details.Add(osd);
                                i++;
                            }
                            attrInfo.OptionSetId = os.OptionSetId;
                            os.Items = details;
                            attrInfo.OptionSet = os;
                        }
                        break;

                    case AttributeTypeIds.BIT:
                        if (model.BitOptionName.IsEmpty())
                        {
                            return JError(T["attribute_options_empty"]);
                        }
                        //新建选项集
                        List<Schema.Domain.StringMap> pickListItems = new List<Schema.Domain.StringMap>();
                        int j = 0;
                        foreach (var item in model.BitOptionName)
                        {
                            Schema.Domain.StringMap s = new Schema.Domain.StringMap();
                            s.StringMapId = Guid.NewGuid();
                            s.Name = item;
                            s.Value = j == 0 ? 1 : 0;//第一项为true选项
                            s.DisplayOrder = j;
                            s.AttributeId = attrInfo.AttributeId;
                            s.EntityName = attrInfo.EntityName;
                            s.AttributeName = attrInfo.Name;
                            j++;
                            pickListItems.Add(s);
                        }
                        attrInfo.PickLists = pickListItems;
                        break;

                    case AttributeTypeIds.DATETIME:
                        attrInfo.DataFormat = model.DateTimeFormat;
                        break;

                    case AttributeTypeIds.PARTYLIST:
                    case AttributeTypeIds.LOOKUP://如果字段类型为引用
                        attrInfo.ReferencedEntityId = model.LookupEntity.Value;
                        attrInfo.DisplayStyle = model.LookupType;
                        attrInfo.DefaultValue = model.DefaultValue;
                        attrInfo.FilterExpression = model.FilterExpression;//引用条件
                        attrInfo.filedConditionsExp = model.filedConditionsExp;//引用字段
                        // 按引入字段的配置更新引入字段为显示字段,同时把默认的name字段去掉
                        // xmg
                        // 202009031512
                        if (!string.IsNullOrEmpty(attrInfo.filedConditionsExp))
                        {
                            //把之前的设置初始化
                            var attrInfoName = _attributeFinder.FindByEntityId(model.EntityId);

                            foreach (var attr2 in attrInfoName)
                            {
                                attr2.IsPrimaryField = false;
                                _attributeUpdater.Update(attr2);
                            }

                            if (!string.IsNullOrEmpty(attrInfo.filedConditionsExp))
                            {

                                attrInfo.IsPrimaryField = true;
                            }


                        }

                        break;


                }
                attrInfo.AttributeTypeName = model.AttributeType;
                _attributeCreater.Create(attrInfo);
                return CreateSuccess(new { id = attrInfo.AttributeId });
            }
            return JModelError(T["created_error"]);
        }

        [HttpGet]
        [Description("字段编辑")]
        public IActionResult EditAttribute(Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return NotFound();
            }
            EditAttributeModel model = new EditAttributeModel();
            var entity = _attributeFinder.FindById(id);
            if (entity == null)
            {
                return NotFound();
            }
            model.Entity = _entityFinder.FindById(entity.EntityId);
            if (model.Entity != null)
            {
                entity.CopyTo(model);
                if (entity.OptionSetId.HasValue)
                {
                    entity.OptionSet = _optionSetFinder.FindById(entity.OptionSetId.Value);
                    entity.OptionSet.Items = _optionSetDetailFinder.Query(n => n.Where(w => w.OptionSetId == entity.OptionSetId.Value).Sort(s => s.SortAscending(f => f.DisplayOrder)));

                    model.IsCommonOptionSet = entity.OptionSet.IsPublic;
                }
                if (entity.TypeIsBit() || entity.TypeIsState())
                {
                    entity.PickLists = _stringMapFinder.Query(n => n.Where(w => w.AttributeId == entity.AttributeId));
                }
                model.Attribute = entity;
                if (model.SummaryExpression.IsNotEmpty())
                {
                    model.AaExp = new AttributeAggregateExpression().DeserializeFromJson(model.SummaryExpression);
                }
                else
                {
                    model.AaExp = new AttributeAggregateExpression();
                }

                return View(model);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("字段编辑")]
        public IActionResult EditAttribute(EditAttributeModel model)
        {
            if (ModelState.IsValid)
            {
                var attrInfo = _attributeFinder.FindById(model.AttributeId.Value);
                if (attrInfo == null)
                {
                    return NotFound();
                }
                //model.CopyTo(attrInfo);
                //attrInfo.IsCustomizable = true;
                attrInfo.LocalizedName = model.LocalizedName;
                attrInfo.LogEnabled = model.LogEnabled;
                attrInfo.IsRequired = model.IsRequired;
                attrInfo.AuthorizationEnabled = model.AuthorizationEnabled;
                attrInfo.Description = model.Description;
                attrInfo.ValueType = model.ValueType;
                var attrTypeName = attrInfo.AttributeTypeName;
                if (attrTypeName == "state")
                {
                    attrTypeName = "bit";
                }
                else if (attrTypeName == "status")
                {
                    attrTypeName = "picklist";
                }

                switch (attrTypeName)
                {
                    case AttributeTypeIds.NVARCHAR:
                        attrInfo.MaxLength = model.MaxLength.Value;
                        attrInfo.DataFormat = model.TextFormat;
                        attrInfo.DefaultValue = model.DefaultValue;
                        if (model.ValueType == 2)
                        {
                            attrInfo.FormulaExpression = model.FormulaExpression;
                        }
                        break;

                    case AttributeTypeIds.NTEXT:
                        attrInfo.DataFormat = model.NTextFormat;
                        attrInfo.DefaultValue = model.DefaultValue;
                        break;

                    case AttributeTypeIds.INT:
                        attrInfo.MinValue = model.IntMinValue.Value <= int.MinValue ? int.MinValue : model.IntMinValue.Value;
                        attrInfo.MaxValue = model.IntMaxValue.Value >= int.MaxValue ? int.MaxValue : model.IntMaxValue.Value;

                        attrInfo.DefaultValue = model.DefaultValue;
                        if (model.ValueType == 0)//如果是无
                        {
                            attrInfo.DefaultValue = null; //清空默认值

                            attrInfo.FormulaExpression = null;//清空公式

                            attrInfo.SummaryEntityId = Guid.Empty;//清空汇总
                            attrInfo.SummaryExpression = null;
                        }
                        else if (model.ValueType == 1)//如果是默认值
                        {
                            attrInfo.FormulaExpression = null;//清空公式

                            attrInfo.SummaryEntityId = Guid.Empty;//清空汇总
                            attrInfo.SummaryExpression = null;
                        }
                        else if (model.ValueType == 2)//如果是公式
                        {
                            attrInfo.FormulaExpression = model.FormulaExpression;
                            attrInfo.DefaultValue = null; //清空默认值

                            attrInfo.SummaryEntityId = Guid.Empty;//清空汇总
                            attrInfo.SummaryExpression = null;
                        }
                        else if (model.ValueType == 3)//如果是汇总
                        {
                            attrInfo.SummaryEntityId = model.SummaryEntityId;
                            attrInfo.SummaryExpression = model.SummaryExpression;

                            attrInfo.DefaultValue = null; //清空默认值

                            attrInfo.FormulaExpression = null;//清空公式
                        }
                        break;

                    case AttributeTypeIds.FLOAT:
                        attrInfo.Precision = model.FloatPrecision.Value;
                        attrInfo.MinValue = model.FloatMinValue.Value <= decimal.MinValue ? decimal.MinValue : model.FloatMinValue.Value;
                        attrInfo.MaxValue = model.FloatMaxValue.Value >= decimal.MaxValue ? decimal.MaxValue : model.FloatMaxValue.Value;
                        attrInfo.DefaultValue = model.DefaultValue;
                        if (model.ValueType == 0)//如果是无
                        {
                            attrInfo.DefaultValue = null; //清空默认值

                            attrInfo.FormulaExpression = null;//清空公式

                            attrInfo.SummaryEntityId = Guid.Empty;//清空汇总
                            attrInfo.SummaryExpression = null;
                        }
                        else if (model.ValueType == 1)//如果是默认值
                        {
                            attrInfo.FormulaExpression = null;//清空公式

                            attrInfo.SummaryEntityId = Guid.Empty;//清空汇总
                            attrInfo.SummaryExpression = null;
                        }
                        else if (model.ValueType == 2)//如果是公式
                        {
                            attrInfo.FormulaExpression = model.FormulaExpression;
                            attrInfo.DefaultValue = null; //清空默认值

                            attrInfo.SummaryEntityId = Guid.Empty;//清空汇总
                            attrInfo.SummaryExpression = null;
                        }
                        else if (model.ValueType == 3)//如果是汇总
                        {
                            attrInfo.SummaryEntityId = model.SummaryEntityId;
                            attrInfo.SummaryExpression = model.SummaryExpression;

                            attrInfo.DefaultValue = null; //清空默认值

                            attrInfo.FormulaExpression = null;//清空公式
                        }

                        break;

                    case AttributeTypeIds.MONEY:
                        attrInfo.Precision = model.MoneyPrecision.Value;
                        attrInfo.MinValue = decimal.Parse(model.MoneyMinValue.Value.ToString()) <= decimal.Parse("-922337203685477.5808") ? decimal.Parse("-922337203685477.5808") : decimal.Parse(model.MoneyMinValue.Value.ToString());
                        attrInfo.MaxValue = decimal.Parse(model.MoneyMaxValue.Value.ToString()) >= decimal.Parse("922337203685477.5807") ? decimal.Parse("922337203685477.5807") : decimal.Parse(model.MoneyMaxValue.Value.ToString());
                        attrInfo.DefaultValue = model.DefaultValue;

                        if (model.ValueType == 0)//如果是无
                        {
                            attrInfo.DefaultValue = null; //清空默认值

                            attrInfo.FormulaExpression = null;//清空公式

                            attrInfo.SummaryEntityId = Guid.Empty;//清空汇总
                            attrInfo.SummaryExpression = null;
                        }
                        else if (model.ValueType == 1)//如果是默认值
                        {
                            attrInfo.FormulaExpression = null;//清空公式

                            attrInfo.SummaryEntityId = Guid.Empty;//清空汇总
                            attrInfo.SummaryExpression = null;
                        }
                        else if (model.ValueType == 2)//如果是公式
                        {
                            attrInfo.FormulaExpression = model.FormulaExpression;
                            attrInfo.DefaultValue = null; //清空默认值

                            attrInfo.SummaryEntityId = Guid.Empty;//清空汇总
                            attrInfo.SummaryExpression = null;
                        }
                        else if (model.ValueType == 3)//如果是汇总
                        {
                            attrInfo.SummaryEntityId = model.SummaryEntityId;
                            attrInfo.SummaryExpression = model.SummaryExpression;

                            attrInfo.DefaultValue = null; //清空默认值

                            attrInfo.FormulaExpression = null;//清空公式
                        }
                        break;

                    case AttributeTypeIds.PICKLIST:
                        attrInfo.DisplayStyle = model.OptionSetType;
                        attrInfo.OptionSet = _optionSetFinder.FindById(attrInfo.OptionSetId.Value);
                        if (!attrInfo.OptionSet.IsPublic)
                        {
                            if (model.OptionSetName.IsEmpty())
                            {
                                return JError(T["attribute_options_empty"]);
                            }
                            //选项集
                            List<Schema.Domain.OptionSetDetail> details = new List<Schema.Domain.OptionSetDetail>();
                            int i = 0;
                            foreach (var item in model.OptionSetName)
                            {
                                if (item.IsEmpty())
                                {
                                    continue;
                                }

                                Schema.Domain.OptionSetDetail osd = new Schema.Domain.OptionSetDetail();
                                osd.OptionSetDetailId = model.OptionSetDetailId[i];
                                osd.OptionSetId = attrInfo.OptionSetId.Value;
                                osd.Name = item;
                                osd.Value = model.OptionSetValue[i];
                                osd.IsSelected = model.IsSelectedOption[i];
                                osd.DisplayOrder = i;
                                details.Add(osd);

                                i++;
                            }
                            attrInfo.OptionSet.Items = details;
                        }
                        break;

                    case AttributeTypeIds.BIT:
                        //新建选项集
                        List<Schema.Domain.StringMap> pickListItems = new List<Schema.Domain.StringMap>();

                        int j = 0;
                        foreach (var item in model.BitOptionName)
                        {
                            Schema.Domain.StringMap s = new Schema.Domain.StringMap();
                            s.StringMapId = model.BitDetailId[j];
                            s.Name = item;
                            s.Value = j == 0 ? 1 : 0;//第一项为true选项，因为只有两项默认为第一个为1（是）
                            s.DisplayOrder = bool.Parse(model.CheckedValue[j].ToString()) ? 1 : 0;//此字段确定哪一个控件被选中
                                                                                                  // s.DisplayOrder = j;
                            s.AttributeId = attrInfo.AttributeId;
                            s.AttributeName = attrInfo.Name;
                            s.EntityName = attrInfo.EntityName;



                            j++;
                            pickListItems.Add(s);
                        }
                        attrInfo.PickLists = pickListItems;
                        break;

                    case AttributeTypeIds.DATETIME:
                        attrInfo.DataFormat = model.DateTimeFormat;
                        attrInfo.DefaultValue = model.DefaultValue;
                        break;

                    case AttributeTypeIds.PARTYLIST:
                    case AttributeTypeIds.LOOKUP:
                        attrInfo.DisplayStyle = model.LookupType;
                        //attrInfo.ReferencedEntityId = model.LookupEntity.Value;
                        attrInfo.DefaultValue = model.DefaultValue;
                        attrInfo.FilterExpression = model.FilterExpression;//引用条件
                        attrInfo.filedConditionsExp = model.filedConditionsExp;//引用字段
                        // 按引入字段的配置更新引入字段为显示字段,更新前要把之前的设置初始化
                        // xmg
                        // 202009032112
                        if (!string.IsNullOrEmpty(attrInfo.filedConditionsExp))
                        {

                            var ae = new AttributeAggregateExpression();
                            ae = ae.DeserializeFromJson(attrInfo.filedConditionsExp);
                            //把之前的设置初始化
                            var attrInfoName = _attributeFinder.FindByEntityId(Guid.Parse(model.ReferencedEntityId.ToString()));
                            foreach (var attr in attrInfoName)
                            {
                                attr.IsPrimaryField = false;
                                _attributeUpdater.Update(attr);
                            }

                            foreach (var id in ae.Filter.Conditions)
                            {
                                var attrInfo5 = _attributeFinder.Find(Guid.Parse(model.ReferencedEntityId.ToString()), id.AttributeName);
                                attrInfo5.IsPrimaryField = true;
                                _attributeUpdater.Update(attrInfo5);
                            }


                        }
                        //更新视图
                        _metadataService.AlterView(_entityFinder.FindById(model.EntityId));

                        break;


                }

                _attributeUpdater.Update(attrInfo);
                return UpdateSuccess(new { id = attrInfo.AttributeId });
            }
            return JModelError(T["saved_error"]);
        }

        [Description("删除字段")]
        [HttpPost]
        public IActionResult DeleteAttribute([FromBody]DeleteManyModel model)
        {
            return _attributeDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }
    }
}