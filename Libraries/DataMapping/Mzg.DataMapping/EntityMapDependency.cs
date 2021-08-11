using Mzg.DataMapping.Abstractions;
using Mzg.Dependency;
using Mzg.Schema.Abstractions;
using System;

namespace Mzg.DataMapping
{
    /// <summary>
    /// 实体映射依赖服务
    /// </summary>
    public class EntityMapDependency : IEntityMapDependency
    {
        private readonly IDependencyService _dependencyService;

        public EntityMapDependency(IDependencyService dependencyService)
        {
            _dependencyService = dependencyService;
        }

        public bool Create(Domain.EntityMap entity)
        {
            //依赖于实体
            return _dependencyService.Create(DataMappingDefaults.ModuleName, entity.EntityMapId, EntityDefaults.ModuleName, entity.TargetEntityId);
        }

        public bool Update(Domain.EntityMap entity)
        {
            //依赖于实体
            return _dependencyService.Update(DataMappingDefaults.ModuleName, entity.EntityMapId, AttributeDefaults.ModuleName, entity.TargetEntityId);
        }

        public bool Delete(params Guid[] id)
        {
            return _dependencyService.DeleteByDependentId(DataMappingDefaults.ModuleName, id); ;
        }
    }
}