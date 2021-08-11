using Mzg.Context;
using Mzg.Plugin.Domain;

namespace Mzg.Plugin
{
    /// <summary>
    /// 实体插件缓存服务
    /// </summary>
    public class EntityPluginCache
    {
        public static string BuildKey(EntityPlugin entity)
        {
            return entity.EntityId + "/" + entity.EventName + "/" + entity.EntityPluginId + "/";
        }

        public static string GetCacheKey(IAppContext appContext)
        {
            return appContext.OrganizationUniqueName + ":entityplugins";
        }
    }
}