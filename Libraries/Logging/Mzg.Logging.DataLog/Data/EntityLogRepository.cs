using Mzg.Core.Data;
using Mzg.Data;
using Mzg.Infrastructure.Utility;
using Mzg.Logging.DataLog.Domain;
using System;

namespace Mzg.Logging.DataLog.Data
{
    /// <summary>
    /// 实体数据日志仓储
    /// </summary>
    public class EntityLogRepository : DefaultRepository<EntityLog>, IEntityLogRepository
    {
        public EntityLogRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region Implements

        public void Clear()
        {
            _repository.Execute("TRUNCATE TABLE [{0}]".FormatWith(TableName));
        }

        public void Clear(Guid entityId)
        {
            _repository.Execute("DELETE [{0}] WHERE [EntityId]=@0".FormatWith(TableName), entityId);
        }

        #endregion Implements
    }
}