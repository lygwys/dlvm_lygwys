using Mzg.Core.Data;
using Mzg.Organization.Domain;
using System;
using System.Collections.Generic;

namespace Mzg.Organization.Data
{
    public interface IBusinessUnitRepository : IRepository<BusinessUnit>
    {
        List<BusinessUnit> GetChilds(Guid parentId);

        bool IsChild(Guid parentId, Guid businessUnitId);
    }
}