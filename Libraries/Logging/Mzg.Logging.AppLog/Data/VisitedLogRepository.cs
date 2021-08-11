using Mzg.Core.Data;
using Mzg.Data;
using Mzg.Infrastructure.Utility;

namespace Mzg.Logging.AppLog.Data
{
    /// <summary>
    /// 访问日志仓储
    /// </summary>
    public class AppLogRepository : DefaultRepository<Domain.VisitedLog>, IAppLogRepository
    {
        public AppLogRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region Implements

        public void Clear()
        {
            _repository.Execute("TRUNCATE TABLE [{0}]".FormatWith(TableName));
        }

        #endregion Implements
    }
}