using Microsoft.AspNetCore.Mvc;
using Mzg.Core;
using Mzg.Infrastructure.Utility;
using Mzg.Schema.Entity;
using Mzg.Security.Abstractions;
using Mzg.Security.DataAuthorization;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 实体权限接口
    /// </summary>
    [Route("{org}/api/[controller]")]
    [ApiController]
    public class EntityPermissionController : ApiControllerBase
    {
        private readonly IEntityPermissionService _entityPermissionService;
        private readonly IEntityFinder _entityFinder;

        public EntityPermissionController(IWebAppContext appContext
            , IEntityPermissionService entityPermissionService
            , IEntityFinder entityService)
            : base(appContext)
        {
            _entityPermissionService = entityPermissionService;
            _entityFinder = entityService;
        }

        [Description("查询实体权限资源")]
        [HttpGet("PrivilegeResource")]
        public IActionResult PrivilegeResource()
        {
            var data = _entityPermissionService.Query(x => x.Select(s => new { s.EntityPermissionId, s.AccessRight, s.EntityId }));
            if (data.NotEmpty())
            {
                var result = new List<PrivilegeResourceItem>();
                var entities = _entityFinder.FindAll().Where(x => x.AuthorizationEnabled);
                foreach (var item in entities)
                {
                    var views = data.Where(x => x.EntityId == item.EntityId);
                    if (!views.Any())
                    {
                        continue;
                    }
                    var group1 = new PrivilegeResourceItem();
                    group1.Label = item.LocalizedName;
                    group1.Children = views.Select(x => (new PrivilegeResourceItem { Id = x.EntityPermissionId, Label = T["security_" + Enum.GetName(typeof(AccessRightValue), x.AccessRight)], AuthorizationEnabled = true })).ToList();
                    result.Add(group1);
                }
                return JOk(result);
            }
            return JOk();
        }
    }
}