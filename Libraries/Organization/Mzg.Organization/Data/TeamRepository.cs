using Mzg.Core.Data;
using Mzg.Data;
using Mzg.Organization.Domain;

namespace Mzg.Organization.Data
{
    /// <summary>
    /// 团队仓储
    /// </summary>
    public class TeamRepository : DefaultRepository<Team>, ITeamRepository
    {
        public TeamRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}