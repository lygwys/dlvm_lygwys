using Microsoft.AspNetCore.Mvc;
using Mzg.Flow;
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

namespace Mzg.Web.Controllers
{
    /// <summary>
    /// 审批流执行控制器,此文件在controllers目录下,点击审批处理按钮后首先执行到这里和api目录下的同名此文不同
    /// 一个是post一个是Post前的点击,以下注册的服务在post前要首先运行.
    /// xmg
    /// 202006172002
    /// </summary>
    [Route("{org}/flow/[action]")]
    public class WorkFlowExecuterController : AuthenticatedControllerBase
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

        /// <summary>
        /// 由enity.js文件中的WorkFlowProcessing方法中到此处
        /// xmg
        /// 202101132030
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description("审批处理")]
        public IActionResult WorkFlowProcessing(WorkFlowProcessingModel model)
        {
            if (model.EntityId.Equals(Guid.Empty) || model.RecordId.Equals(Guid.Empty))
            {
                return NotFound();
            }
            var entityMetas = _entityFinder.FindById(model.EntityId);
            var entity = _dataFinder.RetrieveById(entityMetas.Name, model.RecordId);
            var instances = _workFlowInstanceService.Query(n => n.Take(1).Where(f => f.EntityId == model.EntityId && f.ObjectId == model.RecordId).Sort(s => s.SortDescending(f => f.CreatedOn)));
            WorkFlowInstance instance = null;
            if (instances.NotEmpty())
            {
                instance = instances.First();
            }
            if (instance == null)
            {
                return NotFound();
            }

            var nextI = model.StepOrder + 1;
            List<WorkFlowProcess> nextSteps = _workFlowProcessFinder.Query(n => n.Where(f => f.WorkFlowInstanceId == instance.WorkFlowInstanceId && f.StepOrder == nextI));
            //如果不是最后一个环节则进行错误提示
            if (!nextSteps.IsEmpty())
            {

                var processInfo = _workFlowProcessFinder.GetCurrentStep(instance.WorkFlowInstanceId, CurrentUser.SystemUserId, CurrentUser.PostId, CurrentUser.Roles.Select(n => n.RoleId).ToArray());

                if (processInfo == null)
                {
                    if (_workFlowProcessFinder.GetLastHandledStep(model.StepOrder, instance.WorkFlowInstanceId, CurrentUser.SystemUserId, CurrentUser.PostId, CurrentUser.Roles.Select(n => n.RoleId).ToArray()) != null)
                    {
                        return JError("您已处理!");
                    }
                    return JError(T["workflow_nopermission"]);
                }
                model.ProcessInfo = processInfo;
            }
            else
            {
                //如果是最后一个人进行一次或二次审批时，则查询状态为1或2的
                var processInfo = _workFlowProcessFinder.GetCurrentStepLast(model.StepOrder, instance.WorkFlowInstanceId, CurrentUser.SystemUserId, CurrentUser.PostId, CurrentUser.Roles.Select(n => n.RoleId).ToArray());
                model.ProcessInfo = processInfo;
            }

            model.InstanceInfo = instance;

            model.ProcessList = _workFlowProcessFinder.Query(n => n.Where(f => f.WorkFlowInstanceId == instance.WorkFlowInstanceId).Sort(s => s.SortAscending(f => f.StepOrder)));


            return View($"~/Views/Flow/{WebContext.ActionName}.cshtml", model);
        }
    }
}