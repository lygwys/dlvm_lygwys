using Mzg.Core.Context;
using Mzg.Core.Data;
using Mzg.Data;
using Mzg.Infrastructure.Utility;
using System;

namespace Mzg.RibbonButton.Data
{
    /// <summary>
    /// 按钮仓储
    /// </summary>
    public class RibbonButtonRepository : DefaultRepository<Domain.RibbonButton>, IRibbonButtonRepository
    {
        public RibbonButtonRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        public PagedList<Domain.RibbonButton> QueryPaged(QueryDescriptor<Domain.RibbonButton> q, int solutionComponentType, Guid solutionId, bool existInSolution)
        {
            if (q.QueryText.IsNotEmpty())
            {
                q.QueryText += " AND ";
            }
            q.QueryText += MetaData.TableInfo.PrimaryKey + " " + (existInSolution ? "" : "NOT") + " IN(SELECT ObjectId FROM SolutionComponent WHERE SolutionId=@" + q.Parameters.Count;
            q.Parameters.Add(new QueryParameter("@" + q.Parameters.Count, solutionId));
            q.QueryText += " and ComponentType = @" + q.Parameters.Count;
            q.Parameters.Add(new QueryParameter("@" + q.Parameters.Count, solutionComponentType));
            q.QueryText += ")";
            return base.QueryPaged(q);
        }
    }
}