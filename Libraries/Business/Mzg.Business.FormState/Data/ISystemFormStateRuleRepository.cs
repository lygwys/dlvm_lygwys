using Mzg.Business.FormStateRule.Domain;
using Mzg.Core.Context;
using Mzg.Core.Data;
using System;

namespace Mzg.Business.FormStateRule.Data
{
    public interface ISystemFormStateRuleRepository : IRepository<SystemFormStateRule>
    {
        PagedList<SystemFormStateRule> QueryPaged(QueryDescriptor<SystemFormStateRule> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}