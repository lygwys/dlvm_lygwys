using Microsoft.AspNetCore.Mvc;
using Mzg.Flow;
using Mzg.Flow.Abstractions;
using Mzg.Flow.Core;
using Mzg.Flow.Domain;
using Mzg.Infrastructure.Utility;
using Mzg.Schema.Entity;
using Mzg.Sdk.Client;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 审批流执行接口，审批处理：Post到这里，以下注册的服务在post前要首先运行
    /// xmg
    /// 202006171958
    /// </summary>
    [Route("{org}/api/workflow/execute")]
    public class WorkFlowExecuterController : WebControllerBase //此处进行了继承的修改原来为ApiControllerBase，因为如果按原来的上传文件不好使。
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IWorkFlowProcessService _workFlowProcessService;
        private readonly IWorkFlowProcessFinder _workFlowProcessFinder;
        private readonly IWorkFlowInstanceService _workFlowInstanceService;
        private readonly IWorkFlowExecuter _workFlowExecuter;
        private readonly IDataFinder _dataFinder;

        public WorkFlowExecuterController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IWorkFlowProcessService workFlowProcessService
            , IWorkFlowProcessFinder workFlowProcessFinder
            , IWorkFlowInstanceService workFlowInstanceService
            , IWorkFlowExecuter workFlowExecuter
            , IDataFinder dataFinder)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _workFlowProcessService = workFlowProcessService;
            _workFlowProcessFinder = workFlowProcessFinder;
            _workFlowInstanceService = workFlowInstanceService;
            _workFlowExecuter = workFlowExecuter;
            _dataFinder = dataFinder;
        }

        [HttpPost]
        [Description("审批处理")]
        public async Task<IActionResult> Post(WorkFlowProcessedModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ProcessState == WorkFlowProcessState.UnPassed && model.Description.IsEmpty())
                {
                    return JError(T["workflow_unpassed_needreason"]);
                }
                var processInfo = _workFlowProcessFinder.FindById(model.WorkFlowProcessId);
                if (processInfo == null)
                {
                    return NotFound();
                }
                var nextI = processInfo.StepOrder + 1;
                List<WorkFlowProcess> nextSteps = _workFlowProcessFinder.Query(n => n.Where(f => f.WorkFlowInstanceId == processInfo.WorkFlowInstanceId && f.StepOrder == nextI));
                //如果不是最后一个环节则进行错误提示
                if (!nextSteps.IsEmpty())
                {
                    if (processInfo.StateCode != WorkFlowProcessState.Processing)
                    {
                        return JError(T["workflow_alreadyhandled"]);
                    }
                }
                //审批人的岗位、人员编号、角色三种条件都不在此单据中的审批人员列表中
                if ((processInfo.HandlerId != CurrentUser.SystemUserId) && (processInfo.Handlers.IndexOf((CurrentUser.PostId).ToString()) == -1) && Array.IndexOf((CurrentUser.Roles.Select(n => n.RoleId).ToArray()), processInfo.HandlerId) == -1)
                {
                    return JError(T["workflow_youarenothandler"]);
                }
                var instance = _workFlowInstanceService.FindById(processInfo.WorkFlowInstanceId);
                if (instance == null)
                {
                    return NotFound();
                }
                var entityMeta = _entityFinder.FindById(instance.EntityId);
                if (entityMeta == null)
                {
                    return NotFound();
                }
                //执行工作流
                var result = await _workFlowExecuter.ExecuteAsync(new WorkFlowExecutionContext()
                {
                    Attachments = model.Attachments != null ? model.Attachments.Count() : 0
                    ,
                    AttachmentFiles = model.Attachments
                    ,
                    Description = model.Description
                    ,
                    ProcessInfo = processInfo
                    ,
                    InstanceInfo = instance
                    ,
                    EntityMetaData = entityMeta
                    ,
                    ProcessState = model.ProcessState
                }).ConfigureAwait(false);
                if (!result.IsSuccess)
                {
                    return JError(result.Message);
                }
                return JOk(T["operation_success"]);
            }
            return JError(T["operation_error"] + ": " + GetModelErrors(ModelState));
        }
    }
}