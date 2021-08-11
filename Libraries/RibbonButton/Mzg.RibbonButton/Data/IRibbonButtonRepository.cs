using Mzg.Core.Context;
using Mzg.Core.Data;
using System;

namespace Mzg.RibbonButton.Data
{
    public interface IRibbonButtonRepository : IRepository<Domain.RibbonButton>
    {
        PagedList<Domain.RibbonButton> QueryPaged(QueryDescriptor<Domain.RibbonButton> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}