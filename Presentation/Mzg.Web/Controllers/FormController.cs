using Microsoft.AspNetCore.Mvc;
using Mzg.Authorization.Abstractions;
using Mzg.Business.FormStateRule;
using Mzg.Business.SerialNumber;
using Mzg.Core;
using Mzg.Flow;
using Mzg.Flow.Abstractions;
using Mzg.Flow.Domain;
using Mzg.Form;
using Mzg.Form.Abstractions;
using Mzg.Form.Abstractions.Component;
using Mzg.Form.Domain;
using Mzg.Infrastructure.Utility;
using Mzg.QueryView;
using Mzg.RibbonButton;
using Mzg.RibbonButton.Abstractions;
using Mzg.Schema.Abstractions;
using Mzg.Schema.Attribute;
using Mzg.Schema.Entity;
using Mzg.Sdk.Client;
using Mzg.Sdk.Extensions;
using Mzg.Security.Principal;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Mzg.Web.Controllers
{
    /// <summary>
    /// 表单控制器,打开修改页面
    /// xmg
    /// 20200821
    /// </summary>
    [Route("{org}/entity/[action]")]
    public class FormController : AuthenticatedControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly ISystemFormFinder _systemFormFinder;
        private readonly IQueryViewFinder _queryViewFinder;
        private readonly IRibbonButtonFinder _ribbonbuttonFinder;
        private readonly IWorkFlowProcessFinder _workFlowProcessFinder;
        private readonly IWorkFlowInstanceService _workFlowInstanceService;
        private readonly ISystemFormStatusSetter _systemFormStatusSetter;
        private readonly ISerialNumberRuleFinder _serialNumberRuleFinder;
        private readonly IRoleObjectAccessEntityPermissionService _roleObjectAccessEntityPermissionService;
        private readonly ISystemUserPermissionService _systemUserPermissionService;
        private readonly IRibbonButtonStatusSetter _ribbonButtonStatusSetter;
        private readonly IFormService _formService;
        private readonly IDataFinder _dataFinder;

        public FormController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , ISystemFormFinder systemFormFinder
            , IQueryViewFinder queryViewFinder
            , IRibbonButtonFinder ribbonbuttonFinder
            , IRibbonButtonStatusSetter ribbonButtonStatusSetter
            , IWorkFlowProcessFinder workFlowProcessFinder
            , IWorkFlowInstanceService workFlowInstanceService
            , ISystemFormStatusSetter systemFormStatusSetter
            , IRoleObjectAccessEntityPermissionService roleObjectAccessEntityPermissionService
            , ISystemUserPermissionService systemUserPermissionService
            , ISerialNumberRuleFinder serialNumberRuleFinder
            , IFormService formService
            , IDataFinder dataFinder
            )
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _systemFormFinder = systemFormFinder;
            _queryViewFinder = queryViewFinder;
            _ribbonbuttonFinder = ribbonbuttonFinder;
            _ribbonButtonStatusSetter = ribbonButtonStatusSetter;
            _workFlowProcessFinder = workFlowProcessFinder;
            _workFlowInstanceService = workFlowInstanceService;
            _systemFormStatusSetter = systemFormStatusSetter;
            _roleObjectAccessEntityPermissionService = roleObjectAccessEntityPermissionService;
            _systemUserPermissionService = systemUserPermissionService;
            _serialNumberRuleFinder = serialNumberRuleFinder;
            _formService = formService;
            _dataFinder = dataFinder;
        }
        /// <summary>
        /// 修改页面表单的生成，修改页面入口cs
        /// xmg
        /// 202012021838
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("表单生成")]
        [Route("{entityname?}")]//特性配置路由
        public IActionResult Create(EntityFormModel args)
        {
            if (args.EntityId.Equals(Guid.Empty) && args.EntityName.IsEmpty())
            {
                return NotFound();
            }
            var entity = args.EntityId.Equals(Guid.Empty) ? _entityFinder.FindByName(args.EntityName) : _entityFinder.FindById(args.EntityId);
            if (entity == null)
            {
                return NotFound();
            }
            args.EntityId = entity.EntityId;
            args.EntityName = entity.Name;
            EditRecordModel m = new EditRecordModel
            {
                EntityMetaData = entity,
                EntityId = args.EntityId,
                RelationShipName = args.RelationShipName,
                ReferencedRecordId = args.ReferencedRecordId
            };

            if (args.RecordId.HasValue && !args.RecordId.Value.Equals(Guid.Empty))//如果是修改页面
            {
                var record = _dataFinder.RetrieveById(entity.Name, args.RecordId.Value);
                if (record == null || record.Count == 0)
                {
                    return NotFound();
                }
                var fileAttributes = _attributeFinder.FindByEntityId(entity.EntityId).Where(n => n.DataFormat.IsCaseInsensitiveEqual("fileupload"));
                foreach (var item in fileAttributes)//如果此字段可以上传文件
                {
                    if (record.GetStringValue(item.Name).IsNotEmpty())
                    {
                        record[item.Name] = string.Empty;
                    }
                    else
                    {
                        record.Remove(item.Name);
                    }
                }
                m.Entity = record;
                m.RecordId = args.RecordId;
                //var SubEntity = _entityFinder.FindByParentId(entity.EntityId);
                //foreach (var suben in SubEntity)//子实体列表
                //{
                //    var QueryViewIds = _queryViewFinder.FindByEntityId(suben.EntityId);
                //    foreach (var item in QueryViewIds)//默认视图id，是默认
                //    {
                //        if (item.IsDefault)
                //        {
                m.SubQueryId = entity.EntityId;
                //        }
                //    }
                //}
                m.FormState = FormState.Update;
                if (m.Entity.GetIntValue("statecode", -1) == 0)
                {
                    m.FormState = FormState.Disabled;
                    //model.ReadOnly = true;
                }
                if (entity.ParentEntityId != null)//如果是单据体要取主实体的id
                {
                    var fileConfig = _attributeFinder.FindByEntityIdSum(Guid.Parse(entity.ParentEntityId.ToString()));
                    if (fileConfig.Count > 0)
                    {
                        foreach (var item in fileConfig)// 把多个字段的汇总配置传到前台
                        {
                            if (item.SummaryExpression != null)
                                ViewData["FieldConfig"] += item.SerializeToJson() + ",";
                        }

                        ViewData["FieldConfig"] = "[" + ViewData["FieldConfig"].ToString().Substring(0, ViewData["FieldConfig"].ToString().Length - 1) + "]";
                    }
                    else
                    {
                        ViewData["FieldConfig"] = "[]";
                    }
                }
                else
                {
                    var fileConfig = _attributeFinder.FindByEntityIdSum(Guid.Parse(entity.EntityId.ToString()));
                    if (fileConfig.Count > 0)
                    {
                        foreach (var item in fileConfig)// 把多个字段的汇总配置传到前台
                        {
                            if (item.SummaryExpression != null)
                                ViewData["FieldConfig"] += item.SerializeToJson() + ",";
                        }

                        ViewData["FieldConfig"] = "[" + ViewData["FieldConfig"].ToString().Substring(0, ViewData["FieldConfig"].ToString().Length - 1) + "]";
                    }
                    else
                    {
                        ViewData["FieldConfig"] = "[]";
                    }
                }
                ViewData["duplicate"] = "{}";//修改时也要设置此值因为要传到前台页面
            }
            else if (args.CopyId.HasValue && !args.CopyId.Value.Equals(Guid.Empty))//如果为新增页面
            {
                var record = _dataFinder.RetrieveById(entity.Name, args.CopyId.Value);
                if (record == null || record.Count == 0)
                {
                    return NotFound();
                }
                var fileAttributes = _attributeFinder.FindByEntityId(entity.EntityId).Where(n => n.DataFormat.IsCaseInsensitiveEqual("fileupload"));
                foreach (var item in fileAttributes)//如果此字段可以上传文件
                {
                    record.Remove(item.Name);
                }
                record.RemoveKeys(AttributeDefaults.SystemAttributes);
                m.Entity = record;
                //m.RecordId = model.RecordId;
                m.FormState = FormState.Create;
                ViewData["duplicate"] = "{}";//也要设置此值因为要传到前台页面
            }
            //在新增页面取到字段对应的默认值后传到前台在entity.create.js中调用
            //xmg 202012041555
            else if (m.FormState == FormState.Create)
            {
                //_attributeFinder.FindByEntityId(entity.EntityId).Where(n => n.DataFormat.IsCaseInsensitiveEqual("fileupload"));

                //以上方式不能用于ViewData进行传送
                var fileAttributes = _attributeFinder.FindByEntityIdDefv(entity.EntityId);
                ViewData["duplicate"] = fileAttributes.SerializeToJson();
                //把字段的汇总配置传到前台
                if (entity.ParentEntityId != null)//如果是单据体要取主实体的id
                {
                    var fileConfig = _attributeFinder.FindByEntityIdSum(Guid.Parse(entity.ParentEntityId.ToString()));
                    if (fileConfig.Count > 0)
                    {
                        foreach (var item in fileConfig)// 把多个字段的汇总配置传到前台
                        {
                            if (item.SummaryExpression != null)
                                ViewData["FieldConfig"] += item.SerializeToJson() + ",";
                        }

                        ViewData["FieldConfig"] = "[" + ViewData["FieldConfig"].ToString().Substring(0, ViewData["FieldConfig"].ToString().Length - 1) + "]";
                    }
                    else
                    {
                        ViewData["FieldConfig"] = "[]";
                    }
                }
                else
                {
                    var fileConfig = _attributeFinder.FindByEntityIdSum(Guid.Parse(entity.EntityId.ToString()));
                    if (fileConfig.Count > 0)
                    {
                        foreach (var item in fileConfig)// 把多个字段的汇总配置传到前台
                        {
                            if (item.SummaryExpression != null)
                                ViewData["FieldConfig"] += item.SerializeToJson() + ",";
                        }

                        ViewData["FieldConfig"] = "[" + ViewData["FieldConfig"].ToString().Substring(0, ViewData["FieldConfig"].ToString().Length - 1) + "]";
                    }
                    else
                    {
                        ViewData["FieldConfig"] = "[]";
                    }
                }

            }
            else
            {
                ViewData["duplicate"] = "{}";//也要设置此值因为要传到前台页面
                m.FormState = FormState.Create;
            }
            m.ReadOnly = args.ReadOnly;
            m.StepOrder = args.StepOrder;//把审批步骤排序id传到model
            var isCreate = !args.RecordId.HasValue || args.RecordId.Value.Equals(Guid.Empty);
            SystemForm formEntity = null;
            //workflow
            if (!isCreate && m.EntityMetaData.WorkFlowEnabled && m.Entity.GetGuidValue("workflowid").Equals(Guid.Empty))
            {
                var processState = m.Entity.GetIntValue("processstate", -1);
                //单据在处理中、完成审批时要隐藏新增、保存、启动审批等按钮
                if (processState == (int)WorkFlowProcessState.Processing || processState == (int)WorkFlowProcessState.Passed)
                    m.ReadOnly = true;

                var nextI = args.StepOrder + 1;
                List<WorkFlowProcess> nextSteps = _workFlowProcessFinder.Query(n => n.Where(f => f.WorkFlowInstanceId == args.WorkFlowInstanceId && f.StepOrder == nextI));
                //如果到了最后一个环节
                if (nextSteps.IsEmpty())
                {
                    m.LastStep = true;

                }
                if (processState == (int)WorkFlowProcessState.Processing || processState == (int)WorkFlowProcessState.Passed)
                {

                    m.FormState = FormState.ReadOnly;
                    var instances = _workFlowInstanceService.Top(n => n.Take(1).Where(f => f.EntityId == m.EntityId.Value && f.ObjectId == m.RecordId.Value).Sort(s => s.SortDescending(f => f.CreatedOn)));
                    WorkFlowInstance instance = null;
                    if (instances.NotEmpty())
                    {
                        instance = instances.First();
                    }
                    if (instance != null)
                    {
                        var processInfo = _workFlowProcessFinder.GetCurrentStep(instance.WorkFlowInstanceId, CurrentUser.SystemUserId, CurrentUser.PostId, CurrentUser.Roles.Select(n => n.RoleId).ToArray());
                        if (processInfo != null)
                        {
                            if (!processInfo.FormId.Equals(Guid.Empty))
                            {
                                formEntity = _systemFormFinder.FindById(processInfo.FormId);
                            }
                        }
                    }

                }
                m.WorkFlowProcessState = processState;
            }
            if (formEntity == null)
            {
                if (args.FormId.HasValue && !args.FormId.Value.Equals(Guid.Empty))
                {
                    formEntity = _systemFormFinder.FindById(args.FormId.Value);
                    if (formEntity.StateCode != RecordState.Enabled)
                    {
                        formEntity = null;
                    }
                }
                else
                {
                    //获取实体默认表单,每个实体的表单都在此表中
                    formEntity = _systemFormFinder.FindEntityDefaultForm(args.EntityId);
                }
            }
            if (formEntity == null)
            {
                return PromptView(T["notfound_defaultform"]);
            }
            m.FormInfo = formEntity;
            m.FormId = formEntity.SystemFormId;
            //调用表单生成器把json数据反序列化
            FormBuilder formBuilder = new FormBuilder(formEntity.FormConfig);

            _formService.Init(formEntity);
            //表单可用状态
            if (!isCreate && m.FormState != FormState.Disabled && formBuilder.Form.FormRules.NotEmpty())
            {
                if (_systemFormStatusSetter.IsDisabled(formBuilder.Form.FormRules, m.Entity))
                {
                    m.FormState = FormState.Disabled;
                }
            }
            //获取所有字段信息
            m.AttributeList = _formService.AttributeMetaDatas;
            //获取字段中启用了权限控制的字段列表
            if (!CurrentUser.IsSuperAdmin && m.AttributeList.Count(n => n.AuthorizationEnabled) > 0)
            {
                var securityFields = m.AttributeList.Where(n => n.AuthorizationEnabled).Select(f => f.AttributeId)?.ToList();
                if (securityFields.NotEmpty())
                {
                    //无权限的字段
                    var noneRead = _systemUserPermissionService.GetNoneReadFields(CurrentUser.SystemUserId, securityFields);
                    var noneEdit = _systemUserPermissionService.GetNoneEditFields(CurrentUser.SystemUserId, securityFields);
                    //移除无读取权限的字段内容
                    if (m.Entity.NotEmpty())
                    {
                        if (noneRead.IsEmpty()) return JError("您还未对启用权控制的字段进行权限分配!");
                        foreach (var item in noneRead)
                        {
                            m.Entity.Remove(m.AttributeList.Find(n => n.AttributeId == item).Name);
                        }
                    }
                    var obj = new { noneread = noneRead, noneedit = noneEdit };
                    ViewData["NonePermissionFields"] = obj.SerializeToJson();
                }
            }
            else
            {
                ViewData["NonePermissionFields"] = "[]";
            }
            var _form = formBuilder.Form;
            m.Form = _form;
            //把默认表单传到前端cshtml中以方便在js中调用
            ViewData["form"] = _formService.Form.SerializeToJson(false);
            //buttons
            var buttons = _ribbonbuttonFinder.Find(m.EntityId.Value, RibbonButtonArea.Form);
            if (formEntity.IsCustomButton && formEntity.CustomButtons.IsNotEmpty())
            {
                List<Guid> buttonid = new List<Guid>();
                buttonid = buttonid.DeserializeFromJson(formEntity.CustomButtons);
                buttons.RemoveAll(x => !buttonid.Contains(x.RibbonButtonId));
            }
            if (buttons.NotEmpty())
            {
                buttons = buttons.OrderBy(x => x.DisplayOrder).ToList();
                m.RibbonButtons = buttons;
                _ribbonButtonStatusSetter.Set(m.RibbonButtons, m.FormState, m.Entity);
            }
            if (isCreate)
            {
                var rep = _roleObjectAccessEntityPermissionService.FindUserPermission(m.EntityMetaData.Name, CurrentUser.LoginName, AccessRightValue.Create);
                m.HasBasePermission = rep != null && rep.AccessRightsMask != EntityPermissionDepth.None;
            }
            else
            {
                var rep = _roleObjectAccessEntityPermissionService.FindUserPermission(m.EntityMetaData.Name, CurrentUser.LoginName, AccessRightValue.Update);
                m.HasBasePermission = rep != null && rep.AccessRightsMask != EntityPermissionDepth.None;
            }
            m.SnRule = _serialNumberRuleFinder.FindByEntityId(args.EntityId);
            if (m.SnRule != null && m.Entity.NotEmpty() && args.CopyId.HasValue)
            {
                m.Entity.SetAttributeValue(m.SnRule.AttributeName, null);
            }
            ViewData["record"] = m.Entity.SerializeToJson();
            m.StageId = args.StageId;
            m.BusinessFlowId = args.BusinessFlowId;
            m.BusinessFlowInstanceId = args.BusinessFlowInstanceId;

            return View($"~/Views/Entity/Create.cshtml", m);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Description("新建记录")]
        //public IActionResult Create(EditRecordModel _recordModel, string child = null)
        //{
        //    Guid entityid = _recordModel.EntityId.Value;
        //    Guid formid = _recordModel.FormId.Value;
        //    Guid? recordid = _recordModel.RecordId;
        //    bool isNew = !(recordid.HasValue && !recordid.Value.Equals(Guid.Empty));
        //    if (entityid.Equals(Guid.Empty))
        //    {
        //        return NotFound();
        //    }
        //    var entityMetaData = _entityFinder.FindById(entityid);
        //    if (entityMetaData == null)
        //    {
        //        return NotFound();
        //    }
        //    _recordModel.EntityMetaData = entityMetaData;
        //    _recordModel.EntityId = entityid;
        //    var formEntity = DomainCreator.Get<SystemForm>();
        //    if (!formid.Equals(Guid.Empty))
        //    {
        //        formEntity = _systemFormFinder.FindById(formid);
        //    }
        //    else
        //    {
        //        //获取实体默认表单
        //        formEntity = _systemFormFinder.FindEntityDefaultForm(entityid);
        //    }
        //    if (formEntity == null)
        //    {
        //        return NotFound();
        //    }
        //    _recordModel.FormInfo = formEntity;
        //    //FormBuilder formBuilder = new FormBuilder(formEntity);
        //    _formService.Init(formEntity);
        //    //内容已更改的字段信息
        //    List<string> attributeChangedList = null;
        //    if (!isNew && _recordModel.AttributeChanged.IsNotEmpty())
        //    {
        //        attributeChangedList = new List<string>();
        //        attributeChangedList.AddRange(_recordModel.AttributeChanged.Split(','));
        //    }
        //    var headHasChanged = attributeChangedList.NotEmpty();
        //    Core.Data.Entity entity = new Core.Data.Entity(entityMetaData.Name);
        //    if (isNew || (!isNew && headHasChanged))
        //    {
        //        foreach (var attr in _formService.AttributeMetaDatas)
        //        {
        //            var k = attr.Name;
        //            if (headHasChanged && !attributeChangedList.Exists(n => n.IsCaseInsensitiveEqual(k)))
        //            {
        //                continue;
        //            }
        //            object v = Request.Form[k];
        //            if (v != null)
        //            {
        //                v = entity.WrapAttributeValue(_entityFinder, attr, v);
        //                entity[k] = v;
        //            }
        //        }
        //    }
        //    //包含单据体时，启用事务
        //    if (child.IsNotEmpty())
        //    {
        //        //_organizationServiceProxy.BeginTransaction();
        //    }
        //    var thisId = Guid.Empty;
        //    try
        //    {
        //        if (isNew)
        //        {
        //            if (_recordModel.RelationShipName.IsNotEmpty() && _recordModel.ReferencedRecordId.HasValue)//如果存在关联关系
        //            {
        //                var relationShipMetas = _relationShipFinder.FindByName(_recordModel.RelationShipName);
        //                if (null != relationShipMetas && relationShipMetas.ReferencingEntityId == _recordModel.EntityId && entity.GetStringValue(relationShipMetas.ReferencingAttributeName).IsEmpty())
        //                {
        //                    //设置当前记录关联字段的值
        //                    entity.SetAttributeValue(relationShipMetas.ReferencingAttributeName, new EntityReference(relationShipMetas.ReferencedEntityName, _recordModel.ReferencedRecordId.Value));
        //                }
        //            }
        //            if (!_recordModel.StageId.Equals(Guid.Empty))//业务流程的阶段
        //            {
        //                entity.SetAttributeValue("StageId", _recordModel.StageId);
        //            }
        //            thisId = _dataCreater.Create(entity);
        //            if (!_recordModel.StageId.Equals(Guid.Empty))//业务流程的阶段
        //            {
        //                _businessProcessFlowInstanceUpdater.UpdateForward(_recordModel.BusinessFlowId, _recordModel.BusinessFlowInstanceId, _recordModel.StageId, thisId);
        //            }
        //        }
        //        else
        //        {
        //            thisId = recordid.Value;
        //            entity.SetIdValue(recordid.Value);
        //            if (headHasChanged)
        //            {
        //                _dataUpdater.Update(entity);
        //            }
        //        }
        //        //单据体
        //        if (child.IsNotEmpty())
        //        {
        //            var childs = JArray.Parse(child.UrlDecode());
        //            if (childs.Count > 0)
        //            {
        //                List<Core.Data.Entity> childEntities = new List<Core.Data.Entity>();
        //                List<string> entityNames = new List<string>();
        //                foreach (var c in childs)
        //                {
        //                    dynamic root = JObject.Parse(c.ToString());
        //                    string name = root.name, relationshipname = root.relationshipname, refname = string.Empty;
        //                    if (!entityNames.Exists(n => n.IsCaseInsensitiveEqual(name)))
        //                    {
        //                        entityNames.Add(name);
        //                    }

        //                    var data = root.data;
        //                    var childAttributes = _attributeFinder.FindByEntityName(name);
        //                    if (relationshipname.IsNotEmpty())
        //                    {
        //                        var relationShipMetas = _relationShipFinder.FindByName(relationshipname);
        //                        if (null != relationShipMetas && relationShipMetas.ReferencedEntityId == _recordModel.EntityId)
        //                        {
        //                            refname = relationShipMetas.ReferencingAttributeName;
        //                        }
        //                    }
        //                    Core.Data.Entity detail = new Core.Data.Entity(name);
        //                    foreach (JProperty p in data)
        //                    {
        //                        var attr = childAttributes.Find(n => n.Name.IsCaseInsensitiveEqual(p.Name));
        //                        if (attr != null && p.Value != null)
        //                        {
        //                            detail.SetAttributeValue(p.Name.ToString().ToLower(), detail.WrapAttributeValue(_entityFinder, attr, p.Value.ToString()));
        //                        }
        //                    }
        //                    //关联主记录ID
        //                    if (refname.IsNotEmpty())
        //                    {
        //                        detail.SetAttributeValue(refname, new EntityReference(_recordModel.EntityMetaData.Name, thisId));
        //                    }
        //                    childEntities.Add(detail);
        //                }
        //                //批量创建记录
        //                if (childEntities.NotEmpty())
        //                {
        //                    foreach (var item in entityNames)
        //                    {
        //                        var items = childEntities.Where(n => n.Name.IsCaseInsensitiveEqual(item)).ToList();
        //                        var creatingRecords = items.Where(n => n.Name.IsCaseInsensitiveEqual(item) && n.GetIdValue().Equals(Guid.Empty)).ToList();
        //                        if (creatingRecords.NotEmpty())
        //                        {
        //                            _dataCreater.CreateMany(creatingRecords);
        //                        }
        //                        if (!isNew)
        //                        {
        //                            foreach (var updItem in items.Where(n => n.Name.IsCaseInsensitiveEqual(item) && !n.GetIdValue().Equals(Guid.Empty)))
        //                            {
        //                                _dataUpdater.Update(updItem);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        //_organizationServiceProxy.CommitTransaction();
        //    }
        //    catch (Exception ex)
        //    {
        //        //_organizationServiceProxy.RollBackTransaction();
        //        return JsonError(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        //    }
        //    if (isNew)
        //    {
        //        return CreateSuccess(new { id = thisId });
        //    }
        //    return UpdateSuccess(new { id = thisId });
        //}

        [HttpGet]
        [Description("更新记录打开修改表单")]
        [Route("{entityname?}")]
        //xmg
        //202008201216
        public IActionResult Edit(Guid entityid, Guid recordid, Guid? formid, Guid? copyid)
        {
            return Create(new EntityFormModel { EntityId = entityid, RecordId = recordid, FormId = formid, CopyId = copyid });
            //return RedirectToAction("create", new { entityid = entityid, recordid = recordid, formid = formid });
        }
    }
}