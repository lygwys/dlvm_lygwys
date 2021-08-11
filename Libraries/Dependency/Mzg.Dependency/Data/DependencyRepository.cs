using Mzg.Core.Data;
using Mzg.Data;

namespace Mzg.Dependency.Data
{
    /// <summary>
    /// 依赖项仓储
    /// </summary>
    public class DependencyRepository : DefaultRepository<Domain.Dependency>, IDependencyRepository
    {
        public DependencyRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}