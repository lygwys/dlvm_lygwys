using Mzg.Core.Context;
using Mzg.Core.Data;
using Mzg.Data.Provider;
using Mzg.Dependency.Data;
using Mzg.Infrastructure;
using Mzg.Infrastructure.Utility;
using Mzg.Module.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Mzg.Dependency
{
    /// <summary>
    /// 依赖项服务
    /// </summary>
    public class DependencyService : IDependencyService
    {
        private readonly IDependencyRepository _dependencyRepository;

        public DependencyService(IDependencyRepository dependencyRepository)
        {
            _dependencyRepository = dependencyRepository;
        }

        public bool Create(Domain.Dependency entity)
        {
            if (_dependencyRepository.Exists(x => x.DependentComponentType == entity.DependentComponentType && x.DependentObjectId == entity.DependentObjectId
             && x.RequiredComponentType == entity.RequiredComponentType && x.RequiredObjectId == entity.RequiredObjectId))
            {
                return false;
            }
            return _dependencyRepository.Create(entity);
        }

        /// <summary>
        /// 创建依赖关系
        /// xmg
        /// 202101270750
        /// </summary>
        /// <param name="dependentComponentType"></param>
        /// <param name="dependentObjectId"></param>
        /// <param name="requiredComponentType"></param>
        /// <param name="requiredObjectId"></param>
        /// <returns></returns>
        public bool Create(int dependentComponentType, Guid dependentObjectId, int requiredComponentType, params Guid[] requiredObjectId)
        {
            Guard.NotEmpty(requiredObjectId, nameof(requiredObjectId));
            var existEntities = _dependencyRepository.Query(x => x.DependentComponentType == dependentComponentType && x.DependentObjectId == dependentObjectId
            && x.RequiredComponentType == requiredComponentType && x.RequiredObjectId.In(requiredObjectId))?.ToList();
            List<Domain.Dependency> entities = new List<Domain.Dependency>();
            foreach (var item in requiredObjectId)
            {
                if (item.Equals(Guid.Empty))
                {
                    continue;
                }
                if (existEntities != null && existEntities.Exists(x => x.DependentComponentType == dependentComponentType && x.DependentObjectId == dependentObjectId
             && x.RequiredComponentType == requiredComponentType && x.RequiredObjectId == item))
                {
                    continue;
                }
                var entity = new Domain.Dependency();
                entity.DependencyId = Guid.NewGuid();
                entity.DependentComponentType = dependentComponentType;
                entity.DependentObjectId = dependentObjectId;
                entity.RequiredComponentType = requiredComponentType;
                entity.RequiredObjectId = item;
                entities.Add(entity);
            }
            if (entities.NotEmpty())
            {
                return CreateMany(entities);
            }
            return false;
        }

        public bool Create(string dependentComponentName, Guid dependentObjectId, string requiredComponentName, params Guid[] requiredObjectId)
        {
            return this.Create(ModuleCollection.GetIdentity(dependentComponentName), dependentObjectId, ModuleCollection.GetIdentity(requiredComponentName), requiredObjectId);
        }

        public bool CreateMany(List<Domain.Dependency> entities)
        {
            Guard.NotEmpty(entities, nameof(entities));

            return _dependencyRepository.CreateMany(entities);
        }

        public bool Update(Domain.Dependency entity)
        {
            return _dependencyRepository.Update(entity);
        }

        public bool Update(int dependentComponentType, Guid dependentObjectId, int requiredComponentType, params Guid[] requiredObjectId)
        {
            Guard.NotEmpty(dependentObjectId, nameof(dependentObjectId));
            //先删除原有条件是表单、按钮、单据体各删各的
            DeleteByDependentId(dependentComponentType, requiredComponentType, dependentObjectId);
            if (requiredObjectId.NotEmpty())
            {
                return Create(dependentComponentType, dependentObjectId, requiredComponentType, requiredObjectId);
            }
            return true;
        }

        public bool Update(string dependentComponentName, Guid dependentObjectId, string requiredComponentName, params Guid[] requiredObjectId)
        {
            return this.Update(ModuleCollection.GetIdentity(dependentComponentName), dependentObjectId, ModuleCollection.GetIdentity(requiredComponentName), requiredObjectId);
        }

        public Domain.Dependency FindById(Guid id)
        {
            return _dependencyRepository.FindById(id);
        }

        public Domain.Dependency Find(Expression<Func<Domain.Dependency, bool>> predicate)
        {
            return _dependencyRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            return _dependencyRepository.DeleteById(id);
        }

        public bool DeleteById(IEnumerable<Guid> ids)
        {
            var flag = true;
            foreach (var id in ids)
            {
                flag = this.DeleteById(id);
            }
            return flag;
        }

        public bool DeleteByDependentId(int dependentComponentType, int requiredComponentType, params Guid[] dependentId)
        {
            Guard.NotEmpty(dependentId, nameof(dependentId));
            return _dependencyRepository.DeleteMany(x => x.DependentObjectId.In(dependentId) && x.DependentComponentType == dependentComponentType && x.RequiredComponentType == requiredComponentType);
        }

        public bool DeleteByDependentId(string dependentComponentName, params Guid[] requiredId)
        {
            return this.DeleteByRequiredId(ModuleCollection.GetIdentity(dependentComponentName), requiredId);
        }

        public bool DeleteByRequiredId(int requiredComponentType, params Guid[] requiredId)
        {
            Guard.NotEmpty(requiredId, nameof(requiredId));
            return _dependencyRepository.DeleteMany(x => x.RequiredObjectId.In(requiredId) && x.RequiredComponentType == requiredComponentType);
        }

        public bool DeleteByRequiredId(string requiredComponentName, params Guid[] requiredId)
        {
            return this.DeleteByRequiredId(ModuleCollection.GetIdentity(requiredComponentName), requiredId);
        }

        public PagedList<Domain.Dependency> QueryPaged(Func<QueryDescriptor<Domain.Dependency>, QueryDescriptor<Domain.Dependency>> container)
        {
            QueryDescriptor<Domain.Dependency> q = container(QueryDescriptorBuilder.Build<Domain.Dependency>());
            var datas = _dependencyRepository.QueryPaged(q);

            return datas;
        }

        public List<Domain.Dependency> Query(Func<QueryDescriptor<Domain.Dependency>, QueryDescriptor<Domain.Dependency>> container)
        {
            QueryDescriptor<Domain.Dependency> q = container(QueryDescriptorBuilder.Build<Domain.Dependency>());
            var datas = _dependencyRepository.Query(q)?.ToList();

            return datas;
        }
    }
}