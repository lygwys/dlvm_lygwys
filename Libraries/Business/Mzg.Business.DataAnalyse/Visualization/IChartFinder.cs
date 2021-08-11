using Mzg.Business.DataAnalyse.Domain;
using Mzg.Core.Context;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mzg.Business.DataAnalyse.Visualization
{
    public interface IChartFinder
    {
        Chart Find(Expression<Func<Chart, bool>> predicate);

        Chart FindById(Guid id);

        List<Chart> Query(Func<QueryDescriptor<Chart>, QueryDescriptor<Chart>> container);

        PagedList<Chart> QueryPaged(Func<QueryDescriptor<Chart>, QueryDescriptor<Chart>> container);

        PagedList<Chart> QueryPaged(Func<QueryDescriptor<Chart>, QueryDescriptor<Chart>> container, Guid solutionId, bool existInSolution);
    }
}