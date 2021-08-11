using Microsoft.AspNetCore.Http;

namespace Mzg.Identity
{
    public static class XmsAuthenticationDefaults
    {
        public static string AuthenticationScheme => "Authentication";
        public static string ExternalAuthenticationScheme => "ExternalAuthentication";
        public static string ClaimsIssuer => "mzg";

        public static PathString LoginPath => new PathString("/account/signin");

        public static PathString LogoutPath => new PathString("/account/signout");
        public static PathString AccessDeniedPath => new PathString("/error");

        public static PathString InitializationPath => new PathString("/initialization/initialization");
    }
    /// <summary>
    /// 手动修改于
    /// 20200527
    /// xmg
    /// </summary>
    public static class XmsCookieDefaults
    {
        public static string Prefix => ".Mzg";

        public static string UserCookie => ".User";

        public static string AntiforgeryCookie => ".Antiforgery";

        public static string SessionCookie => ".Session";
        public static string AuthenticationCookie => ".Authentication";

        public static string ExternalAuthenticationCookie => ".ExternalAuthentication";
    }
}