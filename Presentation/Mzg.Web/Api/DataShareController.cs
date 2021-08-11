using Microsoft.AspNetCore.Mvc;
using Mzg.Authorization.Abstractions;
using Mzg.Infrastructure.Utility;
using Mzg.Sdk.Client;
using Mzg.Security.Domain;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 实体数据共享接口
    /// </summary>
    [Route("{org}/api/data/share")]
    public class DataShareController : ApiControllerBase
    {
        private readonly IPrincipalObjectAccessService _principalObjectAccessService;
        private readonly IDataSharer _dataSharer;

        public DataShareController(IWebAppContext appContext
            , IPrincipalObjectAccessService principalObjectAccessService
            , IDataSharer dataSharer)
            : base(appContext)
        {
            _principalObjectAccessService = principalObjectAccessService;
            _dataSharer = dataSharer;
        }

        [Description("共享对象列表")]
        [HttpGet("SharedPrincipals")]
        public IActionResult SharedPrincipals(SharedPrincipalsModel model)
        {
            var result = _principalObjectAccessService.Query(n => n.Where(f => f.EntityId == model.EntityId && f.ObjectId == model.ObjectId));

            return JOk(result);
        }

        [HttpPost]
        [Description("共享记录")]
        public IActionResult Post(SharedModel model)
        {
            if (model.EntityId.Equals(Guid.Empty) || model.ObjectId.Equals(Guid.Empty))
            {
                return JError(T["parameter_error"]);
            }
            List<PrincipalObjectAccess> list = null;
            if (model.PrincipalsJson.IsNotEmpty())
            {
                list = list.DeserializeFromJson(model.PrincipalsJson.UrlDecode());
            }
            var flag = _dataSharer.Share(model.EntityId, model.ObjectId, list);
            if (flag)
            {
                return JOk(T["operation_success"]);
            }
            return JError(T["operation_error"]);
        }
    }
}