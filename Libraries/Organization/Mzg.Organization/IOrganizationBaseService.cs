using Mzg.Core.Context;
using Mzg.Organization.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mzg.Organization
{
    public interface IOrganizationBaseService
    {
        bool Create(OrganizationBase entity);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        OrganizationBase Find(Expression<Func<OrganizationBase, bool>> predicate);

        OrganizationBase FindById(Guid id);

        OrganizationBase FindByUniqueName(string uniqueName);

        List<OrganizationBase> Query(Func<QueryDescriptor<OrganizationBase>, QueryDescriptor<OrganizationBase>> container);

        PagedList<OrganizationBase> QueryPaged(Func<QueryDescriptor<OrganizationBase>, QueryDescriptor<OrganizationBase>> container);

        bool Update(OrganizationBase entity);
    }
}