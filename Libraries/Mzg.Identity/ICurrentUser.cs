using Mzg.Core.Context;
using Mzg.Organization.Domain;
using Mzg.Security.Domain;
using System.Collections.Generic;

namespace Mzg.Identity
{
    /// <summary>
    /// 当前用户信息
    /// xmg
    /// 202006301809
    /// </summary>
    public interface ICurrentUser : IUserContext
    {
        List<RoleObjectAccessEntityPermission> RoleObjectAccessEntityPermission { get; set; }
        Organization.Domain.Organization OrgInfo { get; set; }
        List<RoleObjectAccessPrivileges> Privileges { get; set; }
        List<SystemUserRoles> Roles { get; set; }
        string SessionId { get; set; }

        /// <summary>
        /// 当前用户系统设置
        /// xmg
        /// 202006301807
        /// </summary>
        UserSettings UserSettings { get; set; }

        bool Equals(ICurrentUser u);

        bool HasValue();
    }
}