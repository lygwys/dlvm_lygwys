using Mzg.Localization.Abstractions;
using Mzg.Organization.Data;
using Mzg.Organization.Domain;
using Mzg.Security.Domain;
using System;
using System.Collections.Generic;

namespace Mzg.Identity
{
    /// <summary>
    /// 当前用户信息
    /// </summary>
    [Serializable]
    public class CurrentUser : ICurrentUser
    {
        private readonly ISystemUserSettingsRepository _systemUserSettingsRepository;

        public CurrentUser(ISystemUserSettingsRepository systemUserSettingsRepository)
        {
            _systemUserSettingsRepository = systemUserSettingsRepository;
        }

        public const string SESSION_KEY = "SESSION_USER";
        public Guid SystemUserId { get; set; }
        public string SessionId { get; set; }
        public string UserName { get; set; }
        public string LoginName { get; set; }
        public bool IsSuperAdmin { get; set; }
        public Guid BusinessUnitId { get; set; }
        public Guid PostId { get; set; }
        public string PostName { get; set; }
        public string BusinessUnitIdName { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public List<RoleObjectAccessPrivileges> Privileges { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public List<SystemUserRoles> Roles { get; set; }

        public Guid OrganizationId { get; set; }

        //[Newtonsoft.Json.JsonIgnore]
        public Organization.Domain.Organization OrgInfo { get; set; }

        private UserSettings _userSettings;

        private List<RoleObjectAccessEntityPermission> _roleObjectAccessEntityPermissions;

        [Newtonsoft.Json.JsonIgnore]
        public List<RoleObjectAccessEntityPermission> RoleObjectAccessEntityPermission
        {
            get
            {
                if (_roleObjectAccessEntityPermissions == null)
                {
                }
                return _roleObjectAccessEntityPermissions;
            }
            set { _roleObjectAccessEntityPermissions = value; }
        }

        /// <summary>
        /// 当前用户系统设置
        /// xmg
        /// 202006301807
        /// </summary>
        public UserSettings UserSettings
        {
            get
            {

                if (HasValue() && _userSettings == null)
                {
                    _userSettings = _systemUserSettingsRepository.FindById(SystemUserId);
                    if (_userSettings != null)
                    {
                        //以上取到登录人的默认枚举配置CHS中文
                        //以下是把枚举列表后判断登录人的个人选项设置重新写入语言类别（中文、英文）
                        // xmg
                        // 202006022001
                        foreach (int myCode in Enum.GetValues(typeof(LanguageCode)))
                        {
                            string strName = Enum.GetName(typeof(LanguageCode), myCode); //获取名称

                            if (myCode == _userSettings.LanguageUniqueId && !string.IsNullOrEmpty(strName))
                            {
                                // 把字符串转换为枚举
                                LanguageCode d = (LanguageCode)Enum.Parse(typeof(LanguageCode), strName);
                                _userSettings.LanguageId = d;
                            }
                        }

                    }


                }

                return _userSettings ?? new UserSettings();
            }
            set { _userSettings = value; }
        }

        public bool HasValue()
        {
            return !SystemUserId.Equals(Guid.Empty);
        }

        public bool Equals(ICurrentUser u)
        {
            return SystemUserId.Equals(u.SystemUserId);
        }
    }
}