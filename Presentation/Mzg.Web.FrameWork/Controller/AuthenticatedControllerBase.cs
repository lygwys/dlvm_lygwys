using Microsoft.AspNetCore.Mvc;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Filters;

namespace Mzg.Web.Framework.Controller
{
    /// <summary>
    /// 登录状态验证控制器基类
    /// </summary>
    [TypeFilter(typeof(InitializationFilterAttribute), Order = 0)]
    [TypeFilter(typeof(IdentityFilterAttribute), Order = 1)]
    public class AuthenticatedControllerBase : WebControllerBase
    {
        protected AuthenticatedControllerBase(IWebAppContext appContext) : base(appContext)
        {
        }
    }
}