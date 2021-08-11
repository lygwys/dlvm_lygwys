using Mzg.Core.Data;
using Mzg.Data;
using Mzg.Organization.Domain;

namespace Mzg.Organization.Data
{
    /// <summary>
    /// 团队成员仓储
    /// </summary>
    public class TeamMembershipRepository : DefaultRepository<TeamMembership>, ITeamMembershipRepository
    {
        public TeamMembershipRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}