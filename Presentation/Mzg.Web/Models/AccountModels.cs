using Mzg.Organization.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mzg.Web.Models
{
    public class SignInModel
    {
        [Required(ErrorMessage = "帐号不能为空")] public string LoginName { get; set; } = "";

        [Required(ErrorMessage = "密码不能为空")] public string Password { get; set; } = "";

        //[Required(ErrorMessage = "验证码不能为空")]
        public string VerifyCode { get; set; }

        public string ReturnUrl { get; set; }

        public bool RememberMe { get; set; } = false;

        [Required(ErrorMessage = "组织代码不能为空")]
        public string OrgUniqueName { get; set; }

        public string OrgName { get; set; }

        public List<OrganizationBase> OrgList { get; set; }
    }
}