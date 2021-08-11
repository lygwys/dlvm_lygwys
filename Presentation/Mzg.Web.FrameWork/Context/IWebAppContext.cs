using Mzg.Configuration.Domain;
using Mzg.Context;
using Mzg.Core.Org;
using Mzg.Identity;
using Mzg.Localization.Abstractions;
using Mzg.SiteMap.Domain;
using System.Collections.Generic;

namespace Mzg.Web.Framework.Context
{
    public interface IWebAppContext : IAppContext
    {
        /// <summary>
        /// 组织信息
        /// </summary>
        Organization.Domain.Organization Org { get; set; }

        /// <summary>
        /// 平台参数
        /// </summary>
        PlatformSetting PlatformSettings { get; set; }

        /// <summary>
        /// 本地化标签服务
        /// </summary>
        ILocalizedTextProvider T { get; set; }

        /// <summary>
        /// 当前用户信息
        /// </summary>
        ICurrentUser CurrentUser { get; set; }

        /// <summary>
        /// 组织数据存储参数
        /// </summary>
        IOrgDataServer OrgDataServer { get; }


        /// <summary>
        /// 获取点击页面的路径信息(菜单树结构数据)
        /// 如点击开发平台再点流程再点编辑后生成一个list分别为[0]开发平台[1]流程[2]编辑业务流程
        /// xmg
        /// 202005232302
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        List<Privilege> PrivilegeTree { get; }




    }
}