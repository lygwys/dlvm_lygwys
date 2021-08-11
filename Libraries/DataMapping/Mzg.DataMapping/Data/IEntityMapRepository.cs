using Mzg.Core.Context;
using Mzg.Core.Data;
using Mzg.DataMapping.Domain;
using System;

namespace Mzg.DataMapping.Data
{
    public interface IEntityMapRepository : IRepository<EntityMap>
    {
        PagedList<EntityMap> QueryPaged(QueryDescriptor<EntityMap> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}