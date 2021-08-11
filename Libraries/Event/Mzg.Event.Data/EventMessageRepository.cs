using Mzg.Core.Data;
using Mzg.Data;
using Mzg.Event.Domain;

namespace Mzg.Event.Data
{
    /// <summary>
    /// 事件消息仓储
    /// </summary>
    public class EventMessageRepository : DefaultRepository<EventMessage>, IEventMessageRepository
    {
        public EventMessageRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}