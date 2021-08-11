using Mzg.Core.Context;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mzg.Module
{
    public interface IModuleService
    {
        bool Create(Domain.Module entity);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        Domain.Module Find(Expression<Func<Domain.Module, bool>> predicate);

        Domain.Module FindById(Guid id);

        List<Domain.Module> Query(Func<QueryDescriptor<Domain.Module>, QueryDescriptor<Domain.Module>> container);

        PagedList<Domain.Module> QueryPaged(Func<QueryDescriptor<Domain.Module>, QueryDescriptor<Domain.Module>> container);

        bool Update(Domain.Module entity);
    }
}