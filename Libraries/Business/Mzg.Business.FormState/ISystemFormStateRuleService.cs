using Mzg.Business.FormStateRule.Domain;
using Mzg.Core.Context;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mzg.Business.FormStateRule
{
    public interface ISystemFormStateRuleService
    {
        bool Create(SystemFormStateRule entity);

        bool CreateMany(List<SystemFormStateRule> entities);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        SystemFormStateRule Find(Expression<Func<SystemFormStateRule, bool>> predicate);

        SystemFormStateRule FindById(Guid id);

        List<SystemFormStateRule> Query(Func<QueryDescriptor<SystemFormStateRule>, QueryDescriptor<SystemFormStateRule>> container);

        PagedList<SystemFormStateRule> QueryPaged(Func<QueryDescriptor<SystemFormStateRule>, QueryDescriptor<SystemFormStateRule>> container);

        PagedList<SystemFormStateRule> QueryPaged(Func<QueryDescriptor<SystemFormStateRule>, QueryDescriptor<SystemFormStateRule>> container, Guid solutionId, bool existInSolution);

        bool Update(Func<UpdateContext<SystemFormStateRule>, UpdateContext<SystemFormStateRule>> context);

        bool Update(SystemFormStateRule entity);
    }
}