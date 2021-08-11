using Mzg.Core.Data;
using Mzg.Data;
using Mzg.Event.Domain;

namespace Mzg.Event.Data
{
    /// <summary>
    /// 事件主题仓储
    /// </summary>
    public class TopicRepository : DefaultRepository<Topic>, ITopicRepository
    {
        public TopicRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}