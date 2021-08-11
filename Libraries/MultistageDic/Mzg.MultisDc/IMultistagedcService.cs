using Mzg.Core.Context;
using Mzg.MultisDc.Domain;
using System;
using System.Collections.Generic;

namespace Mzg.MultisDc
{
    /// <summary>
    /// 第四步：定义多级字典服务接口、
    /// xmg
    /// 202007181001
    /// </summary>
    public interface IMultistagedcService
    {
        List<MultistageDc> AllMultistagedcs { get; }

        bool Create(MultistageDc entity);

        bool DeleteById(Guid id);

        bool DeleteById(IEnumerable<Guid> ids);

        MultistageDc Find(Func<QueryDescriptor<MultistageDc>, QueryDescriptor<MultistageDc>> container);

        MultistageDc Find(string systemName, string className, string methodName);

        MultistageDc Find(string url);

        MultistageDc FindById(Guid id);

        bool IsExists(MultistageDc entity);

        int Move(Guid moveid, Guid targetid, Guid parentid, string position);

        List<MultistageDc> Query(Func<QueryDescriptor<MultistageDc>, QueryDescriptor<MultistageDc>> container);

        PagedList<MultistageDc> QueryPaged(Func<QueryDescriptor<MultistageDc>, QueryDescriptor<MultistageDc>> container);

        bool Update(MultistageDc entity);

        bool UpdateAuthorization(bool isAuthorization, params Guid[] id);
    }
}
