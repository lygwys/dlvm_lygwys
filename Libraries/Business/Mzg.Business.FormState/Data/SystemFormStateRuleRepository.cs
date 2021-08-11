using Mzg.Business.FormStateRule.Domain;
using Mzg.Core.Context;
using Mzg.Core.Data;
using Mzg.Data;
using Mzg.Infrastructure.Utility;
using System;

namespace Mzg.Business.FormStateRule.Data
{
    /// <summary>
    /// 表单状态控制规则仓储
    /// </summary>
    public class SystemFormStateRuleRepository : DefaultRepository<SystemFormStateRule>, ISystemFormStateRuleRepository
    {
        public SystemFormStateRuleRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region implements

        public PagedList<SystemFormStateRule> QueryPaged(QueryDescriptor<SystemFormStateRule> q, int solutionComponentType, Guid solutionId, bool existInSolution)
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

        #endregion implements
    }
}