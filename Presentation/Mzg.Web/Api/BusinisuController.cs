using Microsoft.AspNetCore.Mvc;
using Mzg.Organization;
using Mzg.Web.Api.Models;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using System.ComponentModel;

namespace Mzg.Web.Api
{    /// <summary>
     /// 部门接口
     /// </summary>
    [Route("{org}/api/[controller]")]
    public class BusinisuController : ApiControllerBase
    {
        private readonly IBusinessUnitService _businessTreeBuilder;


        public BusinisuController(IWebAppContext appContext
            , IBusinessUnitService BusinessUnitService)
            : base(appContext)
        {

            _businessTreeBuilder = BusinessUnitService;
        }

        [Description("查询部门权限资源")]
        [HttpGet("BusinisuResource")]
        public IActionResult BusinisuResource()
        {
            var result = _businessTreeBuilder.Build(x => x
            .Where(f => f.OrganizationId == CurrentUser.OrganizationId)
            .Sort(s => s.SortAscending(ss => ss.Name))
            );
            return JOk(result);
        }

        [Description("启用部门权限")]
        [HttpPost("AuthorizationEnabled")]
        public IActionResult AuthorizationEnabled(UpdateAuthorizationStateModel model)
        {
            //var authorizations = _businessTreeBuilder.Query(x => x.Where(w => w.AuthorizationEnabled == true));
            //if (authorizations.NotEmpty())
            //{
            //    _businessTreeBuilder.UpdateAuthorization(false, authorizations.Select(x => x.PrivilegeId).ToArray());
            //}
            //if (Arguments.HasValue(model.ObjectId))
            //{
            //    _businessTreeBuilder.UpdateAuthorization(true, model.ObjectId);
            //}
            return SaveSuccess();
        }

    }
}
