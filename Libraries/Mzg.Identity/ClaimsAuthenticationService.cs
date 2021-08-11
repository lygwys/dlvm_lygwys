using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Mzg.AppSetting;
using Mzg.Core.Data;
using Mzg.Data;
using Mzg.Infrastructure.Utility;
using Mzg.Organization;
using Mzg.Organization.Domain;
using Mzg.Session;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Mzg.Identity
{
    /// <summary>
    /// 身份认证服务,在此加入了登录超时的调用 IGetAppsettingJson
    /// xmg
    /// 20200522
    /// </summary>
    public class ClaimAuthenticationService : IAuthenticationService
    {
        private readonly HttpContext _httpContext;
        private readonly ISystemUserService _userService;
        private long _expiration = 30;
        private readonly ISessionService _sessionService;
        private ICurrentUser _cachedUser;
        private readonly IGetAppsettingJson _getAppsetting;
        private readonly IBusinessUnitService _businessuitService;
        private readonly IDbContext _dbContext;

        public ClaimAuthenticationService(IHttpContextAccessor httpContext
            , ISessionService sessionService
            , ISystemUserService userService
            , ICurrentUser currentUser
            , IGetAppsettingJson getAppsetting
            , IBusinessUnitService BusinessUnitService
            , IDbContext dbContext
            )
        {
            _httpContext = httpContext.HttpContext;
            _sessionService = sessionService;
            _userService = userService;
            _cachedUser = currentUser;
            _getAppsetting = getAppsetting;
            _businessuitService = BusinessUnitService;
            _dbContext = dbContext;

        }

        /// <summary>
        ///要用Cookie代表一个通过验证的主体，必须包含Claim, ClaimsIdentity, ClaimsPrincipal这三个信息，以一个持有合法驾照的人做比方，
        /// ClaimsPrincipal就是持有证件的人，ClaimsIdentity就是证件，"Basic"就是证件类型（这里假设是驾照），Claim就是驾照中的信息。
        /// ASP.NET Core 的验证模型是 claims-based authentication.
        /// xmg
        /// 20200523
        /// </summary>
        public virtual async void SignIn(SystemUser user, bool persistent = true)
        {
            var claims = new[] {
                new Claim("Name", user.LoginName, ClaimValueTypes.String, XmsAuthenticationDefaults.ClaimsIssuer)
                ,new Claim("Org", user.OrganizationId.ToString(), ClaimValueTypes.String, XmsAuthenticationDefaults.ClaimsIssuer)
                ,new Claim("OrgUniqueName", user.UniqueName.ToString(), ClaimValueTypes.String, XmsAuthenticationDefaults.ClaimsIssuer)
            };
            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal userPrincipal = new ClaimsPrincipal(userIdentity);

            _expiration = long.Parse(_getAppsetting.TimeOut);
            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = persistent
                ,
                IssuedUtc = DateTime.UtcNow.AddHours(8)
                ,
                ExpiresUtc = DateTimeOffset.Now.AddMinutes(_expiration).AddHours(8)
            };
            await _httpContext.SignInAsync(
            XmsAuthenticationDefaults.AuthenticationScheme,
            userPrincipal, authenticationProperties);

            _cachedUser.LoginName = user.LoginName;
            _cachedUser.SystemUserId = user.SystemUserId;
            _cachedUser.UserName = user.Name;
            _cachedUser.OrganizationId = user.OrganizationId;
            _cachedUser.BusinessUnitId = user.BusinessUnitId;
            _cachedUser.PostId = user.PostId;
            Sql s = Sql.Builder.Append("select name from Post")
                .Append("where postid=@0", user.PostId);
            List<dynamic> dataww = new DataRepositoryBase<dynamic>(_dbContext).ExecuteQuery(s);
            _cachedUser.PostName = "";
            _cachedUser.BusinessUnitIdName = _businessuitService.FindById(user.BusinessUnitId).Name;
            _cachedUser.SessionId = _sessionService.GetId();
            //把登录人的信息存入session中以Session_key为KEY
            _sessionService.Set(CurrentUser.SESSION_KEY, _cachedUser);
        }

        public virtual async void SignOut()
        {
            _cachedUser = null;
            _httpContext.Session.Clear();
            await _httpContext.SignOutAsync(XmsAuthenticationDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// 得到用户信息，有缓存在缓存中得到没有缓存则重新获取
        /// 202009120635
        /// </summary>
        /// <returns></returns>
        public virtual ICurrentUser GetAuthenticatedUser()
        {
            if (_cachedUser != null && _cachedUser.HasValue())
            {
                return _cachedUser;
            }

            if (_httpContext?.User == null || _httpContext?.User?.Identity == null || !_httpContext.User.Identity.IsAuthenticated)
            {
                return null;
            }
            var claims = _httpContext.User.Claims.ToList();
            if (claims.Count() < 3)
            {
                return null;
            }
            var authenticateResult = _httpContext.AuthenticateAsync(XmsAuthenticationDefaults.AuthenticationScheme).Result;
            if (!authenticateResult.Succeeded)
            {
                return null;
            }
            var nameClaim = authenticateResult.Principal.FindFirst(claim => claim.Type.IsCaseInsensitiveEqual("Name"));
            //&& claim.Issuer.Equals(XmsAuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase)
            if (nameClaim == null)
            {
                return null;
            }
            var groupClaim = authenticateResult.Principal.Claims.FirstOrDefault(claim => claim.Type == "Org");
            //&& claim.Issuer.Equals(XmsAuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase)
            if (groupClaim == null)
            {
                return null;
            }
            //在此如果session中有值则取没有则
            return GetAuthenticatedUserFromSession(nameClaim, groupClaim) ?? GetAuthenticatedUserFromTicket(nameClaim, groupClaim);
        }

        /// <summary>
        /// 在session中得到用户的基本信息
        /// </summary>
        /// <param name="nameClaim"></param>
        /// <param name="groupClaim"></param>
        /// <returns></returns>
        private ICurrentUser GetAuthenticatedUserFromSession(Claim nameClaim, Claim groupClaim)
        {
            var loginName = nameClaim.Value;
            var organizationId = groupClaim.Value;
            //在session中得到用户的基本信息
            var user = _sessionService.GetValue<CurrentUser>(CurrentUser.SESSION_KEY);

            if (user != null && user.OrganizationId.ToString().IsCaseInsensitiveEqual(organizationId) && user.LoginName.IsCaseInsensitiveEqual(loginName))
            {
                return user;
            }

            return null;
        }

        /// <summary>
        /// 重新在数据库中取到各字段的值
        /// </summary>
        /// <param name="nameClaim"></param>
        /// <param name="groupClaim"></param>
        /// <returns></returns>
        private ICurrentUser GetAuthenticatedUserFromTicket(Claim nameClaim, Claim groupClaim)
        {
            var loginName = nameClaim.Value;
            //从数据库中取值
            var user = _userService.GetUserByLoginName(loginName);
            if (user == null)
            {
                return null;
            }
            //密码已更改
            //if (!user.Password.IsCaseInsensitiveEqual(password))
            //{
            //    return null;
            //}
            //已删除或禁用
            if (user.IsDeleted || user.StateCode == 0)
            {
                return null;
            }
            _cachedUser.LoginName = user.LoginName;
            _cachedUser.SystemUserId = user.SystemUserId;
            _cachedUser.UserName = user.Name;
            _cachedUser.OrganizationId = user.OrganizationId;
            _cachedUser.BusinessUnitId = user.BusinessUnitId;
            _cachedUser.SessionId = _sessionService.GetId();
            _cachedUser.PostId = user.PostId;
            _cachedUser.BusinessUnitIdName = _businessuitService.FindById(user.BusinessUnitId).Name;
            _sessionService.Set(CurrentUser.SESSION_KEY, _cachedUser);
            return _cachedUser;
        }
    }

}