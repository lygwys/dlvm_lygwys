using Microsoft.AspNetCore.Mvc;
using Mzg.Sdk.Abstractions;
using Mzg.Sdk.Client;
using Mzg.Web.Api.Models;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Framework.Infrastructure;
using System.ComponentModel;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 实体数据分派接口
    /// </summary>
    [Route("{org}/api/data/assign")]
    public class DataAssignController : ApiControllerBase
    {
        private readonly IDataAssigner _dataAssigner;

        public DataAssignController(IWebAppContext appContext
            , IDataAssigner dataAssigner)
            : base(appContext)
        {
            _dataAssigner = dataAssigner;
        }

        /// <summary>
        /// 分派操作
        /// xmg
        /// 202006031402
        /// </summary>
        /// <param name="model">户端如果是提交json数据时候建议都加上[FromBody]</param>
        /// <returns></returns>
        [Description("分派记录")]
        [HttpPost]
        public IActionResult Post([FromBody]AssignedModel model)
        {
            if (model == null || !Arguments.HasValue(model.ObjectId))
            {
                return NotSpecifiedRecord();
            }
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
    }
}