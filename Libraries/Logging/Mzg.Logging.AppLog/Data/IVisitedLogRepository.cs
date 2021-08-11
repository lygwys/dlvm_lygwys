using Mzg.Core.Data;

namespace Mzg.Logging.AppLog.Data
{
    public interface IAppLogRepository : IRepository<Domain.VisitedLog>
    {
        void Clear();
    }
}