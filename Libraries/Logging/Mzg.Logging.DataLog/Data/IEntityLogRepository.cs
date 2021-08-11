using Mzg.Core.Data;
using Mzg.Logging.DataLog.Domain;
using System;

namespace Mzg.Logging.DataLog.Data
{
    public interface IEntityLogRepository : IRepository<EntityLog>
    {
        void Clear();

        void Clear(Guid entityId);
    }
}