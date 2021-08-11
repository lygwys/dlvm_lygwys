using Microsoft.AspNetCore.Mvc;
using Mzg.Infrastructure.Utility;
using Mzg.Schema.Entity;
using Mzg.Sdk.Abstractions;
using Mzg.Sdk.Client;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Models;
using System;
using System.ComponentModel;

namespace Mzg.Web.Controllers
{
    /// <summary>
    /// 实体数据分派控制器
    /// </summary>
    [Route("{org}/entity/[action]")]
    public class EntityAssigningController : AuthenticatedControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IDataAssigner _dataAssigner;

        public EntityAssigningController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IDataAssigner dataAssigner)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _dataAssigner = dataAssigner;
        }

        #region 分派

        [Description("分派记录")]
        [HttpPost]
        public IActionResult Assigning([FromBody]AssignModel model)
        {
            if (model.ObjectId.IsEmpty())
            {
                return NotSpecifiedRecord();
            }
            model.EntityMetaData = _entityFinder.FindById(model.EntityId);
            return View($"~/Views/Entity/{WebContext.ActionName}.cshtml", model);
        }

        [Description("分派记录")]
        [HttpPost]
        public IActionResult Assigned(AssignModel model)
        {
            foreach (var item in model.ObjectId)
            {
                OwnerObject owner = null;
                if (model.OwnerIdType == 1) //assign to me
                {
                    owner = new OwnerObject(OwnerTypes.SystemUser, CurrentUser.SystemUserId);
                }
                else if (model.OwnerIdType == 2)
                {
                    owner = new OwnerObject(OwnerTypes.SystemUser, model.OwnerId);
                }
                else if (model.OwnerIdType == 3)
                {
                    owner = new OwnerObject(OwnerTypes.Team, model.OwnerId);
                }
                _dataAssigner.Assign(model.EntityId, item, owner);
            }
            return JOk(T["assign_success"]);
        }

        [Description("重新分派记录")]
        public IActionResult AssignUserAllRecords(Guid userId, Guid? targetId)
        {
            if (targetId.HasValue && targetId.Value.Equals(Guid.Empty))
            {
                return JOk(T["assign_success"]);
            }
            else
            {
                AssignUserAllRecordsModel model = new AssignUserAllRecordsModel();
                model.UserId = userId;
                return View($"~/Views/Entity/{WebContext.ActionName}.cshtml", model);
            }
        }

        #endregion 分派
    }
}