using Mzg.Core.Context;
using Mzg.Core.Data;
using Mzg.Plugin.Domain;
using System;

namespace Mzg.Plugin.Data
{
    public interface IEntityPluginRepository : IRepository<EntityPlugin>
    {
        PagedList<EntityPlugin> QueryPaged(QueryDescriptor<EntityPlugin> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}