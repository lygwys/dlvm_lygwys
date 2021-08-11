using Mzg.Core.Data;
using Mzg.Data;
using Mzg.Organization.Domain;

namespace Mzg.Organization.Data
{
    /// <summary>
    /// 系统用户仓储
    /// </summary>
    public class SystemUserRepository : DefaultRepository<SystemUser>, ISystemUserRepository
    {
        public SystemUserRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}