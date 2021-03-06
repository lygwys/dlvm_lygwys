using Microsoft.AspNetCore.Mvc;
using Mzg.Flow;
using Mzg.Flow.Abstractions;
using Mzg.Flow.Core;
using Mzg.Schema.Entity;
using Mzg.Sdk.Client;
using Mzg.Sdk.Extensions;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Models;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 审批流启动接口
    /// </summary>
    [Route("{org}/api/workflow/start")]
    public class WorkflowStarterController : ApiControllerBase //此处进行了继承的修改原来为ApiControllerBase，因为如果按原来的上传文件不好使。
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IWorkFlowFinder _workFlowFinder;
        private readonly IWorkFlowStarter _workFlowStarter;
        private readonly IDataFinder _dataFinder;

        public WorkflowStarterController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IWorkFlowFinder workFlowFinder
            , IWorkFlowStarter workFlowStarter
            , IDataFinder dataFinder
            )
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _workFlowFinder = workFlowFinder;
            _workFlowStarter = workFlowStarter;
            _dataFinder = dataFinder;
        }

        [HttpPost]
        [Description("启动审批")]
        public async Task<IActionResult> Post([FromForm]StartWorkFlowModel model)
        {
            if (ModelState.IsValid)
            {
                var files = Request.Form.Files;
                //实体元数据
                var entityMetas = _entityFinder.FindById(model.EntityId);
                //查找记录
                var entity = _dataFinder.RetrieveById(entityMetas.Name, model.RecordId);
                if (entity.GetIntValue("ProcessState") == (int)WorkFlowProcessState.Processing)
                {
                    return JError(T["workflow_processing_notallowtwice"]);
                }
                if (entity.GetIntValue("ProcessState") == (int)WorkFlowProcessState.Passed)
                {
                    return JError(T["workflow_stop_notallowtwice"]);
                }
                if (entity.GetIntValue("ProcessState", -1) != -1
                    && (entity.GetIntValue("ProcessState") == (int)WorkFlowProcessState.Waiting || entity.GetIntValue("ProcessState") == (int)WorkFlowProcessState.Disabled))
                {
                    return JError(T["workflow_state_notallowtwice"]);
                }

                //找到审批流程
                var workFlow = _workFlowFinder.FindById(model.WorkflowId);
                if (workFlow == null)
                {
                    return JError(T["workflow_notfound"]);
                }
                var result = await _workFlowStarter.StartAsync(new WorkFlowStartUpContext()
                {
                    User = CurrentUser
                    ,
                    ApplicantId = CurrentUser.SystemUserId
                    ,
                    Description = model.Description
                    ,
                    ObjectId = model.RecordId
                    ,
                    EntityMetaData = entityMetas
                    ,
                    WorkFlowMetaData = workFlow
                    ,
                    ObjectData = entity
                    ,
                    Attachments = model.Attachments != null ? model.Attachments.Count : 0
                    ,
                    AttachmentFiles = model.Attachments
                    ,
                    BusinessUnitId = entity.GetGuidValue("OwningBusinessUnit")
                }).ConfigureAwait(false);
                if (!result.IsSuccess)
                {
                    return JError(result.Message);
                }

                return JOk(T["workflow_start_success"]);
            }
            return JModelError(T["workflow_start_failure"]);
        }
    }
}