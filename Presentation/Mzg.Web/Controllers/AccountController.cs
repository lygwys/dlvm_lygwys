using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Mzg.Identity;
using Mzg.Infrastructure.Utility;
using Mzg.Organization;
using Mzg.Organization.Domain;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Models;
using System.Collections.Generic;

namespace Mzg.Web.Controllers
{
    /// <summary>
    /// 登录
    /// </summary>
    [AllowAnonymous]
    public class AccountController : XmsControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IOrganizationBaseService _organizationBaseService;
        private readonly IMemoryCache _cache;

        public AccountController(IWebAppContext appContext
            , IAuthenticationService authenticationService
             , IOrganizationBaseService organizationBaseService
            , IMemoryCache cache
            )
            : base(appContext)
        {
            _authenticationService = authenticationService;
            _organizationBaseService = organizationBaseService;
            _cache = cache;
        }

        /// <summary>
        /// 注销
        /// </summary>
        public IActionResult SignOut()
        {
            _authenticationService.SignOut();
            return Redirect("/");
        }

        /// <summary>
        /// 未登录显示登录页面
        /// 登录过则记录上次的登录地址，再次登录后跳到此页面
        /// </summary>
        public IActionResult SignIn()
        {
            if (WebContext.IsSignIn)
            {
                if (WebContext.UrlReferrer.IsNotEmpty() && !WebContext.UrlReferrer.IsCaseInsensitiveEqual(WebContext.Url))
                {
                    return Redirect(WebContext.UrlReferrer);
                }
                return Redirect("~/" + WebContext.OrganizationUniqueName + "/home/index");
            }
            else
            {
                List<OrganizationBase> orglist = _organizationBaseService.Query(n => n.Where(x => x.State == 1));

                SignInModel model = new SignInModel
                {
                    ReturnUrl = HttpContext.GetRouteOrQueryString("returnurl"),
                    OrgUniqueName = WebContext.OrganizationUniqueName,
                    OrgName = WebContext.OrganizationName
                };
                if (model.ReturnUrl.IsNotEmpty() && (model.ReturnUrl.ToLower().IndexOf("/signout") >= 0 || model.ReturnUrl.ToLower().IndexOf("/signin") >= 0))
                {
                    model.ReturnUrl = string.Empty;
                }
                model.OrgList = orglist;
                //lyb改，记住我功能
                model.LoginName = _cache.Get<string>("username_cache");
                model.Password = _cache.Get<string>("password_cache");
                if (model.LoginName.HasValue())
                {
                    model.VerifyCode = "cookies";
                }
                //获取主页选择：深邃还是明亮
                string homepage = HttpContext.Request.Cookies["homepage"];
                if (homepage == "light")
                {
                    return View($"~/Views/Account/{WebContext.ActionName}2.cshtml", model);                  
                }
                else
                {
                    return View(model);
                }
            }
        }
    }
}