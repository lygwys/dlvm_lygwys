using Mzg.Core.Data;
using Mzg.MultisDc.Domain;
using System;

namespace Mzg.MultisDc.Data
{
    /// <summary>
    /// 第二步：设置数据仓储接口，实现第一步中的数据表接口
    /// xmg
    /// 202007180955
    /// </summary>
    public interface IMultistagedcRepository : IRepository<MultistageDc>
    {
        int MoveNode(Guid moveid, Guid targetid, Guid parentid, string position);

        string GetMultistageDcXml(Guid solutionId);
    }
}
