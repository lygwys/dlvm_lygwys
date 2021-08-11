using Mzg.Core.Context;
using Mzg.Organization.Domain;
using System;
using System.Collections.Generic;

namespace Mzg.Organization
{
    /// <summary>
    /// 业务部门服务
    /// xmg
    /// 202006301803
    /// </summary>
    public interface IBusinessUnitService
    {
        bool Create(BusinessUnit entity);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        BusinessUnit FindById(Guid id);

        List<BusinessUnit> GetChilds(Guid parentId);

        bool IsChild(Guid parentId, Guid businessUnitId);

        List<BusinessUnit> Query(Func<QueryDescriptor<BusinessUnit>, QueryDescriptor<BusinessUnit>> container);

        PagedList<BusinessUnit> QueryPaged(Func<QueryDescriptor<BusinessUnit>, QueryDescriptor<BusinessUnit>> container);

        bool Update(BusinessUnit entity);

        bool Update(Func<UpdateContext<BusinessUnit>, UpdateContext<BusinessUnit>> context);
        string Build(Func<QueryDescriptor<BusinessUnit>, QueryDescriptor<BusinessUnit>> container, bool nameLower = true);
        List<dynamic> Build(List<BusinessUnit> BusinessUnitList, Guid parentId);
    }
}