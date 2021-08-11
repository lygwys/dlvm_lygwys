using Microsoft.AspNetCore.Mvc;
using Mzg.Infrastructure.Utility;
using Mzg.Sdk.Abstractions.Query;
using Mzg.Sdk.Client;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using System;
using System.ComponentModel;
using System.Linq;

namespace Mzg.Web.Controllers
{
    /// <summary>
    /// 审批流附件控制器
    /// </summary>
    [Route("{org}/flow/[action]")]
    public class WorkflowAttachmentController : AuthenticatedControllerBase
    {
        private readonly IDataFinder _dataFinder;

        public WorkflowAttachmentController(IWebAppContext appContext
            , IDataFinder dataFinder)
            : base(appContext)
        {
            _dataFinder = dataFinder;
        }

        [Description("下载流程附件")]
        public IActionResult WorkFlowAttachments(Guid processId, bool preview = false)
        {
            QueryExpression query = new QueryExpression("attachment", CurrentUser.UserSettings.LanguageId);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("objectid", ConditionOperator.Equal, processId);
            var data = _dataFinder.RetrieveAll(query);
            if (data.IsEmpty())
            {
                return NotFound();
            }
            return Redirect("/" + WebContext.OrganizationUniqueName + "/file/download?id=" + data.First().GetIdValue() + "&sid=" + CurrentUser.SessionId + "&preview=" + preview);
        }
    }
}