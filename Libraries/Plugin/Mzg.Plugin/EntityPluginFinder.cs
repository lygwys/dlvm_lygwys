using Mzg.Context;
using Mzg.Core.Context;
using Mzg.Data.Provider;
using Mzg.Infrastructure.Utility;
using Mzg.Plugin.Abstractions;
using Mzg.Plugin.Data;
using Mzg.Plugin.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mzg.Plugin
{
    /// <summary>
    /// 实体插件查询服务
    /// </summary>
    public class EntityPluginFinder : IEntityPluginFinder
    {
        private readonly IEntityPluginRepository _entityPluginRepository;
        private readonly Caching.CacheManager<EntityPlugin> _cacheService;

        public EntityPluginFinder(IAppContext appContext
            , IEntityPluginRepository entityPluginRepository)
        {
            _entityPluginRepository = entityPluginRepository;
            _cacheService = new Caching.CacheManager<EntityPlugin>(EntityPluginCache.GetCacheKey(appContext), appContext.PlatformSettings.CacheEnabled);
        }

        public EntityPlugin FindById(Guid id)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("EntityPluginId", id.ToString());
            EntityPlugin entity = _cacheService.Get(dic, () =>
             {
                 return _entityPluginRepository.FindById(id);
             });
            return entity;
        }

        /// <summary>
        /// 对实体触发事件、插件触发事件进行查询处理后返回
        /// mzg
        /// 202011121726
        /// </summary>
        /// <param name="entityid"></param>
        /// <param name="eventName"></param>
        /// <param name="businessObjectId"></param>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public List<EntityPlugin> QueryByEntityId(Guid entityid, string eventName, Guid? businessObjectId = null, PlugInType typeCode = PlugInType.Entity)
        {
            List<EntityPlugin> entities = _cacheService.GetVersionItems(entityid + "/" + eventName, () =>
             {
                 return _entityPluginRepository.Query(x => x.EntityId == entityid)?.ToList();
             });
            if (entities.NotEmpty())
            {
                //if(eventName.Equals("Create"))
                //    return entities.OrderBy(x => x.ProcessOrder).ToList();
                //if (businessObjectId.HasValue&&eventName.Equals("Update"))
                //    return entities.OrderBy(x => x.ProcessOrder).ToList();

                if (businessObjectId.HasValue)
                {
                    entities.RemoveAll(x => !(x.EventName.IsCaseInsensitiveEqual(eventName) && x.BusinessObjectId == businessObjectId.Value && x.TypeCode == (int)typeCode));
                }
                else
                {
                    entities.RemoveAll(x => !(x.EventName.IsCaseInsensitiveEqual(eventName) && x.BusinessObjectId == Guid.Empty && x.TypeCode == (int)typeCode));
                }
            }
            return entities.OrderBy(x => x.ProcessOrder).ToList();
        }

        public PagedList<EntityPlugin> QueryPaged(Func<QueryDescriptor<EntityPlugin>, QueryDescriptor<EntityPlugin>> container)
        {
            QueryDescriptor<EntityPlugin> q = container(QueryDescriptorBuilder.Build<EntityPlugin>());
            return _entityPluginRepository.QueryPaged(q);
        }

        public PagedList<EntityPlugin> QueryPaged(Func<QueryDescriptor<EntityPlugin>, QueryDescriptor<EntityPlugin>> container, Guid solutionId, bool existInSolution)
        {
            QueryDescriptor<EntityPlugin> q = container(QueryDescriptorBuilder.Build<EntityPlugin>());
            var datas = _entityPluginRepository.QueryPaged(q, Module.Core.ModuleCollection.GetIdentity(PluginDefaults.ModuleName), solutionId, existInSolution);

            return datas;
        }

        public List<EntityPlugin> Query(Func<QueryDescriptor<EntityPlugin>, QueryDescriptor<EntityPlugin>> container)
        {
            QueryDescriptor<EntityPlugin> q = container(QueryDescriptorBuilder.Build<EntityPlugin>());
            return _entityPluginRepository.Query(q)?.ToList();
        }

        public List<EntityPlugin> FindAll()
        {
            var entities = _cacheService.GetVersionItems("all", () =>
             {
                 return PreCacheAll();
             });
            return entities;
        }

        private List<EntityPlugin> PreCacheAll()
        {
            return _entityPluginRepository.FindAll()?.ToList();
        }
    }
}