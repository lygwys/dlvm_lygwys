using Mzg.Core.Data;
using Mzg.Data;
using Mzg.DataMapping.Domain;

namespace Mzg.DataMapping.Data
{
    /// <summary>
    /// 实体转换字段映射仓储
    /// </summary>
    public class AttributeMapRepository : DefaultRepository<AttributeMap>, IAttributeMapRepository
    {
        public AttributeMapRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}