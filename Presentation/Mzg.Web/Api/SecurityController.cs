﻿using Microsoft.AspNetCore.Mvc;
using Mzg.Security.Resource;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using System.ComponentModel;
using System.Linq;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 权限安全类接口
    /// </summary>
    [Route("{org}/api/security")]
    public class SecurityController : ApiControllerBase
    {
        private readonly IResourceOwnerService _resourceOwnerService;

        public SecurityController(IWebAppContext appContext
            , IResourceOwnerService resourceOwnerService)
            : base(appContext)
        {
            _resourceOwnerService = resourceOwnerService;
        }

        [Description("获取权限资源类型")]
        [HttpGet("ResourceOwners")]
        public IActionResult GetResourceOwners()
        {
            var resourceOwners = _resourceOwnerService.FindAll();
            foreach (var item in resourceOwners)
            {
                item.ModuleLocalizedName = Module.Core.ModuleCollection.GetDescriptor(item.ModuleName).LocalizedName;
            }
            return JOk(resourceOwners.OrderBy(x => x.ModuleName).ToList());
        }
    }
}