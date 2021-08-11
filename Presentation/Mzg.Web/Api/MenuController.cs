using Microsoft.AspNetCore.Mvc;
using Mzg.Infrastructure.Utility;
using Mzg.SiteMap;
using Mzg.Web.Api.Models;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Framework.Infrastructure;
using System.ComponentModel;
using System.Linq;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 菜单接口
    /// </summary>
    [Route("{org}/api/[controller]")]
    public class MenuController : ApiControllerBase
    {
        private readonly IPrivilegeService _privilegeService;
        private readonly IPrivilegeTreeBuilder _privilegeTreeBuilder;

        public MenuController(IWebAppContext appContext
            , IPrivilegeService privilegeService
            , IPrivilegeTreeBuilder privilegeTreeBuilder)
            : base(appContext)
        {
            _privilegeService = privilegeService;
            _privilegeTreeBuilder = privilegeTreeBuilder;
        }

        [Description("查询菜单权限资源")]
        [HttpGet("PrivilegeResource")]
        public IActionResult PrivilegeResource()
        {
            var result = _privilegeTreeBuilder.Build(x => x
            .Where(f => f.OrganizationId == CurrentUser.OrganizationId)
            .Sort(s => s.SortAscending(ss => ss.DisplayOrder))
            );
            return JOk(result);
        }

        [Description("启用菜单权限")]
        [HttpPost("AuthorizationEnabled")]
        public IActionResult AuthorizationEnabled(UpdateAuthorizationStateModel model)
        {
            var authorizations = _privilegeService.Query(x => x.Where(w => w.AuthorizationEnabled == true));
            if (authorizations.NotEmpty())
            {
                _privilegeService.UpdateAuthorization(false, authorizations.Select(x => x.PrivilegeId).ToArray());
            }
            if (Arguments.HasValue(model.ObjectId))
            {
                _privilegeService.UpdateAuthorization(true, model.ObjectId);
            }
            return SaveSuccess();
        }
    }
}