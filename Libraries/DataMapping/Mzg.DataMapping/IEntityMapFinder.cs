using Mzg.Core.Context;
using Mzg.DataMapping.Domain;
using System;
using System.Collections.Generic;

namespace Mzg.DataMapping
{
    public interface IEntityMapFinder
    {
        EntityMap Find(Guid sourceEntityId, Guid targetEntityId);

        EntityMap FindById(Guid id);

        EntityMap FindByParentId(Guid parentid);

        List<EntityMap> FindAll();

        PagedList<EntityMap> QueryPaged(Func<QueryDescriptor<EntityMap>, QueryDescriptor<EntityMap>> container);

        List<EntityMap> Query(Func<QueryDescriptor<EntityMap>, QueryDescriptor<EntityMap>> container);

        PagedList<EntityMap> QueryPaged(Func<QueryDescriptor<EntityMap>, QueryDescriptor<EntityMap>> container, Guid solutionId, bool existInSolution);
    }
}