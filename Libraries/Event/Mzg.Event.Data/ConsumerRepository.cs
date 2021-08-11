using Mzg.Core.Data;
using Mzg.Data;
using Mzg.Event.Domain;

namespace Mzg.Event.Data
{
    /// <summary>
    /// 事件消费者仓储
    /// </summary>
    public class ConsumerRepository : DefaultRepository<Consumer>, IConsumerRepository
    {
        public ConsumerRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}