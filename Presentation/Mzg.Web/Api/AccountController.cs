using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Mzg.Core;
using Mzg.Identity;
using Mzg.Organization;
using Mzg.Security.Verify;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Framework.Mvc;
using Mzg.Web.Models;
using System;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 登录接口
    /// </summary>
    [Route("{org}/api/account")]
    [ApiController]//默认从formbody中取值。如果没有此句可以手动定义从哪里取如（[FromForm]、[FromBody]等）
    public class AccountController : XmsControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ISystemUserService _systemUserService;
        private readonly IVerifyProvider _verifyProvider;
        private readonly IMemoryCache _cache;

        public AccountController(IWebAppContext appContext
            , IAuthenticationService authenticationService
            , ISystemUserService systemUserService
            , IVerifyProvider verifyProvider
            ,IMemoryCache cache
           )
            : base(appContext)
        {
            _authenticationService = authenticationService;
            _systemUserService = systemUserService;
            _verifyProvider = verifyProvider;
            _cache = cache;
        }

        [HttpPost]
        public IActionResult Post([FromForm]SignInModel model)
        {
            var flag = false;
            var msg = string.Empty;
            if (ModelState.IsValid)
            {
                if (WebContext.PlatformSettings.VerifyCodeEnabled && !_verifyProvider.IsValid(model.VerifyCode))
                {
                    flag = false;
                    msg = "验证码不正确";
                    ModelState.AddModelError("verifycode", msg);
                }
                else
                {
                    var orgInfo = WebContext.Org;
                    var u = _systemUserService.GetUserByLoginName(model.LoginName);
                    if (u == null)
                    {
                        flag = false;
                        msg = "帐号不存在";
                        ModelState.AddModelError("loginname", msg);
                    }
                    else if (u.StateCode == RecordState.Disabled)
                    {
                        flag = false;
                        msg = "帐号已禁用";
                        ModelState.AddModelError("loginname", msg);
                    }
                    else
                    {
                        //是否存在其他相同登录名的人员
                        if (_systemUserService.ExistsLoginName(model.LoginName, u.SystemUserId))
                        {
                            flag = false;
                            msg = "存在相同的登录名，请您使用手机号码登录。";
                            ModelState.AddModelError("existsname", msg);
                        }
                        else
                        {
                            //if (_systemUserService.IsValidePassword(model.Password, u.Salt, u.Password))
                            //{
                            var usr = _systemUserService.GetUserByLoginNameAndPassword(model.LoginName, model.Password);
                            if (usr != null)
                            {
                                //lyb改,勾选“记住我”则存入cookies缓存中
                                if (model.RememberMe)
                                {
                                    _cache.Set("username_cache", model.LoginName,TimeSpan.FromDays(7));
                                    _cache.Set("password_cache", model.Password, TimeSpan.FromDays(7));
                                }
                                //登录状态记录
                                _authenticationService.SignIn(u, true);
                                //获取用户个性化信息

                                //更新最后登录时间
                                _systemUserService.Update(n => n.Set(f => f.LastLoginTime, DateTime.Now)//.Set(f => f.IsLogin, true)
                                .Where(f => f.SystemUserId == u.SystemUserId));
                                msg = "登录成功";
                                flag = true;
                            }
                            //}
                            else
                            {
                                flag = false;
                                msg = "密码有误!";
                                ModelState.AddModelError("password", msg);
                            }
                        }
                    }
                }
                if (flag)
                {
                    return JResult.Ok(msg);
                }
            }
            return JResult.Error(GetModelErrors());
        }
    }
}