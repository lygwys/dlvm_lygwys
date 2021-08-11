using Microsoft.AspNetCore.Mvc;
using Mzg.Flow;
using Mzg.Flow.Abstractions;
using Mzg.Flow.Domain;
using Mzg.Infrastructure.Utility;
using Mzg.Organization;
using Mzg.Schema.Entity;
using Mzg.Sdk.Abstractions.Query;
using Mzg.Sdk.Client;
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
    /// 审批流控制器
    /// </summary>
    public class FlowController : AuthenticatedControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IWorkFlowFinder _workFlowFinder;
        private readonly IWorkFlowProcessFinder _workFlowProcessFinder;
        private readonly IWorkFlowInstanceService _workFlowInstanceService;
        private readonly IWorkFlowProcessLogService _workFlowProcessLogService;
        private readonly IDataFinder _dataFinder;
        private readonly IWorkFlowStepService _workFlowStepService;
        private readonly ISystemUserService _systemUserService;

        public FlowController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IWorkFlowFinder workFlowFinder
            , IWorkFlowProcessFinder workFlowProcessFinder
            , IWorkFlowInstanceService workFlowInstanceService
            , IWorkFlowProcessLogService workFlowProcessLogService
            , IDataFinder dataFinder
            , IWorkFlowStepService workFlowStepService
            , ISystemUserService SystemUserService)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _workFlowFinder = workFlowFinder;
            _workFlowInstanceService = workFlowInstanceService;
            _workFlowProcessFinder = workFlowProcessFinder;
            _workFlowProcessLogService = workFlowProcessLogService;
            _dataFinder = dataFinder;
            _workFlowStepService = workFlowStepService;
            _systemUserService = SystemUserService;
        }

        #region 工作流

        [Description("任务移交")]
        [ValidateAntiForgeryToken]
        public IActionResult AssignHandler(Guid processId, string userName)
        {
            //查找用户
            var query = new QueryExpression("systemuser", CurrentUser.UserSettings.LanguageId);
            query.ColumnSet.AddColumns("systemuserid", "name");
            query.Criteria.FilterOperator = LogicalOperator.Or;
            query.Criteria.AddCondition("loginname", ConditionOperator.Equal, userName);
            query.Criteria.AddCondition("usernumber", ConditionOperator.Equal, userName);
            var user = _dataFinder.Retrieve(query);
            if (user == null || user.Count == 0)
            {
                return JError(T["workflow_nomatchuser"]);
            }
            Guid handlerId = user.GetIdValue();
            //当前步骤
            var processInfo = _workFlowProcessFinder.FindById(processId);
            if (processInfo == null)
            {
                return NotFound();
            }
            if (handlerId == CurrentUser.SystemUserId)
            {
                return JError(T["workflow_notallowtome"]);
            }
            var instance = _workFlowInstanceService.FindById(processInfo.WorkFlowInstanceId);
            if (handlerId == instance.ApplicantId)
            {
                return JError(T["workflow_notallowtoapplier"]);
            }
            //验证是否有移交权限
            //...
            var log = new WorkFlowProcessLog();
            log.CreatedOn = DateTime.Now;
            log.OperatorId = CurrentUser.SystemUserId;
            log.Title = T["workflow_assignto"];
            log.WorkFlowProcessId = Guid.Empty;
            log.WorkFlowProcessLogId = Guid.NewGuid();
            log.WorkFlowInstanceId = Guid.Empty;
            _workFlowProcessLogService.Create(log);
            return JOk(T["operation_success"]);
        }

        [Description("流程执行详情")]
        public IActionResult WorkFlowInstanceDetail(Guid entityid, Guid recordid)
        {
            var instances = _workFlowInstanceService.Query(n => n
                .Where(f => f.EntityId == entityid && f.ObjectId == recordid)
                .Sort(s => s.SortDescending(f => f.CreatedOn))
            );

            if (instances.IsEmpty())
            {
                return NotFound();
            }
            var entityMeta = _entityFinder.FindById(entityid);
            if (entityMeta == null)
            {
                return NotFound();
            }
            //记录岗位中的审批人员信息
            List<WorkFlowProcess> newsteps = new List<WorkFlowProcess>();
            foreach (var instance in instances)
            {
                int i = 0;
                var steps = _workFlowProcessFinder.Query(n => n
                .Where(f => f.WorkFlowInstanceId == instance.WorkFlowInstanceId && f.StateCode != WorkFlowProcessState.Disabled)
                .Sort(s => s.SortAscending(f => f.StepOrder)).Sort(s => s.SortAscending(f => f.StateCode)));

                //如果是按岗位审批：审批人对这个实体的这条记录有无读取权限，如果有读取权限则显示审批人姓名
                //xmg
                //202009282041
                foreach (var step in steps)//对每个审批层涉及的多个步骤（岗位、人员）进行循环操作
                {
                    List<WorkFlowStepHandler> handlerObjs = new List<WorkFlowStepHandler>().DeserializeFromJson(step.Handlers);
                    //如果审批结点不是发起人即是按岗位进行审批（step.Handlers的值不为空）
                    if (handlerObjs.Count > 0)
                    {
                        foreach (var item in handlerObjs)
                        {
                            if (item.Type == WorkFlowStepHandlerType.Post)//如果是按岗位审批
                            {
                                //去重复
                                if (newsteps.IndexOf(steps[i]) == -1)
                                {
                                    //找到这个岗位的人员
                                    var sysUserEntity = _systemUserService.Query(n => n.Where(f => f.PostId == step.HandlerId));
                                    //返回这个人是否有对这个实体的此部门数据的读取权限的人员姓名,无此部门数据的读取权限的人员不做记录
                                    var sprList = _workFlowStepService.RoleHasDeptPermission("2", entityMeta, sysUserEntity);
                                    if (sprList.Count > 0)//如果在此岗位并有此部门数据的读取权限的人员
                                    {
                                        foreach (var spr in sprList)//累加所有在此岗位有此部门数据的读取权限的人员姓名
                                        {
                                            step.HandlerIdName += spr + ",";


                                        }
                                        step.HandlerIdName = step.HandlerIdName.Substring(0, step.HandlerIdName.Length - 1);
                                        newsteps.Add(steps[i]);
                                    }
                                }

                            }
                            else
                            {
                                //去重复
                                if (newsteps.IndexOf(steps[i]) == -1)
                                    newsteps.Add(steps[i]);
                            }


                        }
                    }
                    else//如果审批结点是发起人即不是按岗位进行审批（step.Handlers的值不为空）
                    {
                        //去重复
                        if (newsteps.IndexOf(steps[i]) == -1)
                        {
                            //找到这个人员
                            var sysUserEntity = _systemUserService.Query(n => n.Where(f => f.SystemUserId == step.HandlerId));
                            //返回这个人是否有对这个实体的此部门数据的读取权限的人员姓名,无此部门数据的读取权限的人员不做记录
                            var sprList = _workFlowStepService.RoleHasDeptPermission("2", entityMeta, sysUserEntity);
                            if (sprList.Count > 0)//如果在此岗位并有此部门数据的读取权限的人员
                            {

                                newsteps.Add(steps[i]);
                            }
                        }

                    }

                    i++;
                }
                instance.Steps = newsteps;

            }

            WorkFlowInstanceDetailModel model = new WorkFlowInstanceDetailModel();
            model.Items = instances;
            var workFlowId = instances.First().WorkFlowId;
            model.FlowInfo = _workFlowFinder.FindById(workFlowId);

            var allSteps = _workFlowStepService.Query(n => n.Where(f => f.WorkFlowId == workFlowId).Sort(s => s.SortAscending(f => f.StepOrder)));

            model.Steps = allSteps;
            return View(model);
        }

        [Description("审批任务")]
        public IActionResult WorkFlowStateList(WorkFlowStateListModel model)
        {
            if (model.StateCode == 1)
            {
                var result = _workFlowProcessFinder.QueryHandlingList(CurrentUser.SystemUserId, model.Page, model.PageSize, model.EntityId, CurrentUser.PostId, CurrentUser.Roles.Select(n => n.RoleId).ToArray());
                model.Items = result.Items;
                model.TotalItems = result.TotalItems;
            }
            else if (model.StateCode == 2)
            {
                var result = _workFlowProcessFinder.QueryHandledList(CurrentUser.SystemUserId, model.Page, model.PageSize, model.EntityId, CurrentUser.PostId, CurrentUser.Roles.Select(n => n.RoleId).ToArray());
                model.Items = result.Items;
                model.TotalItems = result.TotalItems;
            }
            else if (model.StateCode == 0)
            {
                var result = _workFlowProcessFinder.QueryApplyHandlingList(CurrentUser.SystemUserId, model.Page, model.PageSize, model.EntityId);
                model.Items = result.Items;
                model.TotalItems = result.TotalItems;
            }
            model.HandledCount = _workFlowProcessFinder.QueryHandledCount(CurrentUser.SystemUserId, model.EntityId, CurrentUser.PostId, CurrentUser.Roles.Select(n => n.RoleId).ToArray());
            model.HandlingCount = _workFlowProcessFinder.QueryHandlingCount(CurrentUser.SystemUserId, model.EntityId, CurrentUser.PostId, CurrentUser.Roles.Select(n => n.RoleId).ToArray());
            model.ApplyHandlingCount = _workFlowProcessFinder.QueryApplyHandlingCount(CurrentUser.SystemUserId, model.EntityId);
            return DynamicResult(model);
        }

        #endregion 工作流
    }
}