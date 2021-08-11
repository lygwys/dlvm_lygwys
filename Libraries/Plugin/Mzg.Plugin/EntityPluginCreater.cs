using Microsoft.AspNetCore.Http;
using Mzg.Context;
using Mzg.Data.Abstractions;
using Mzg.Dependency;
using Mzg.Identity;
using Mzg.Plugin.Abstractions;
using Mzg.Plugin.Data;
using Mzg.Plugin.Domain;
using Mzg.Schema.Abstractions;
using Mzg.Solution;
using Mzg.Solution.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mzg.Plugin
{
    /// <summary>
    /// 实体插件创建服务
    /// </summary>
    public class EntityPluginCreater : IEntityPluginCreater
    {
        private readonly IEntityPluginRepository _entityPluginRepository;
        private readonly IEntityPluginFileProvider _entityPluginFileProvider;
        private readonly ISolutionComponentService _solutionComponentService;
        private readonly IDependencyService _dependencyService;
        private readonly Caching.CacheManager<EntityPlugin> _cacheService;
        private readonly IAppContext _appContext;

        public EntityPluginCreater(IAppContext appContext
            , IEntityPluginRepository entityPluginRepository
            , ISolutionComponentService solutionComponentService
            , IEntityPluginFileProvider entityPluginFileProvider
            , IDependencyService dependencyService)
        {
            _appContext = appContext;
            _entityPluginRepository = entityPluginRepository;
            _solutionComponentService = solutionComponentService;
            _entityPluginFileProvider = entityPluginFileProvider;
            _dependencyService = dependencyService;
            _cacheService = new Caching.CacheManager<EntityPlugin>(EntityPluginCache.GetCacheKey(appContext), _appContext.PlatformSettings.CacheEnabled);
        }

        public async Task<bool> Create(EntityPlugin entity, IFormFile file)
        {
            if (file != null)
            {
                await _entityPluginFileProvider.Save(file).ConfigureAwait(false);
            }
            return Create(entity);
        }

        public bool Create(EntityPlugin entity, string fileName)
        {
            string savePath = _entityPluginFileProvider.Save(fileName);
            if (!string.IsNullOrWhiteSpace(savePath))
            {
                return Create(entity);
            }
            return false;
        }

        public async Task<List<PluginAnalysis>> BeforehandLoad(IFormFile file)
        {
            if (file != null)
            {
                return await _entityPluginFileProvider.BeforehandLoad(file).ConfigureAwait(false);
            }
            return null;
        }

        public List<PluginAnalysis> BeforehandLoad(string fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                return _entityPluginFileProvider.BeforehandLoad(fileName);
            }
            return null;
        }

        public bool Create(EntityPlugin entity)
        {
            entity.SolutionId = SolutionDefaults.DefaultSolutionId;//组件属于默认解决方案
            entity.CreatedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
            entity.OrganizationId = _appContext.OrganizationId;
            var result = true;
            using (UnitOfWork.Build(_entityPluginRepository.DbContext))
            {
                result = _entityPluginRepository.Create(entity);
                //solution component
                _solutionComponentService.Create(entity.SolutionId, entity.EntityPluginId, PluginDefaults.ModuleName);
                //依赖于实体
                _dependencyService.Create(PluginDefaults.ModuleName, entity.EntityPluginId, EntityDefaults.ModuleName, entity.EntityId);
                //add to cache
                _cacheService.SetEntity(entity);
            }
            return result;
        }
    }
}