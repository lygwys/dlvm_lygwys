using Mzg.Core.Data;
using Mzg.Data;

namespace Mzg.UserPersonalization.Data.Xms
{
    public class UserPersonalizationRepository : DefaultRepository<Domain.UserCustomization>, IUserPersonalizationRepository
    {
        public UserPersonalizationRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}