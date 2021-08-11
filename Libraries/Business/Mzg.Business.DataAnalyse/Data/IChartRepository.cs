using Mzg.Business.DataAnalyse.Domain;
using Mzg.Core.Context;
using Mzg.Core.Data;
using Mzg.Sdk.Abstractions.Query;
using Mzg.Sdk.Query;
using System;
using System.Collections.Generic;

namespace Mzg.Business.DataAnalyse.Data
{
    public interface IChartRepository : IRepository<Chart>
    {
        List<dynamic> GetChartDataSource(ChartDataDescriptor chartData, QueryExpression query, IQueryResolver queryResolver);

        PagedList<Domain.Chart> QueryPaged(QueryDescriptor<Domain.Chart> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}