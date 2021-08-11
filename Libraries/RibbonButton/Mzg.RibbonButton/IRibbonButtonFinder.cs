using Mzg.Core.Context;
using Mzg.RibbonButton.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mzg.RibbonButton
{
    public interface IRibbonButtonFinder
    {
        Domain.RibbonButton Find(Expression<Func<Domain.RibbonButton, bool>> predicate);

        Domain.RibbonButton FindById(Guid id);

        List<Domain.RibbonButton> Find(Guid entityId, RibbonButtonArea? area);

        List<Domain.RibbonButton> FindAll();

        List<Domain.RibbonButton> Query(Func<QueryDescriptor<Domain.RibbonButton>, QueryDescriptor<Domain.RibbonButton>> container);

        PagedList<Domain.RibbonButton> QueryPaged(Func<QueryDescriptor<Domain.RibbonButton>, QueryDescriptor<Domain.RibbonButton>> container);

        PagedList<Domain.RibbonButton> QueryPaged(Func<QueryDescriptor<Domain.RibbonButton>, QueryDescriptor<Domain.RibbonButton>> container, Guid solutionId, bool existInSolution);
    }
}