using Mzg.Core.Context;
using Mzg.Core.Data;
using Mzg.Data;
using Mzg.Data.Abstractions;
using Mzg.DataMapping.Domain;
using Mzg.Infrastructure.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mzg.DataMapping.Data
{
    /// <summary>
    /// 实体映射仓储
    /// </summary>
    public class EntityMapRepository : DefaultRepository<EntityMap>, IEntityMapRepository
    {
        public EntityMapRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region implements

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool DeleteById(Guid id)
        {
            var result = false;
            using (UnitOfWork.Build(DbContext))
            {
                _repository.Execute("DELETE " + TableName + " WHERE ParentEntityMapId=@0", id);
                result = _repository.Delete(id);
            }
            return result;
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteById(List<Guid> ids)
        {
            List<object> values = ids.Select(n => (object)n).ToList();
            var result = false;
            using (UnitOfWork.Build(DbContext))
            {
                _repository.Execute("DELETE " + TableName + " WHERE ParentEntityMapId IN(@0)", values.ToArray());
                result = _repository.DeleteMany(ids);
            }
            return result;
        }

        public PagedList<EntityMap> QueryPaged(QueryDescriptor<EntityMap> q, int solutionComponentType, Guid solutionId, bool existInSolution)
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