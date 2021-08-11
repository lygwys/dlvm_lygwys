﻿using Mzg.Core.Data;
using Mzg.Data;
using Mzg.Infrastructure.Utility;
using Mzg.MultisDc.Domain;
using PetaPoco;
using System;

namespace Mzg.MultisDc.Data
{
    /// <summary>
    /// 第三步：实现多级字典仓储接口，及黙认仓储（第一步中的数据表）
    /// xmg
    /// 202007180958
    /// </summary>
    public class MultistagedcRepository : DefaultRepository<MultistageDc>, IMultistagedcRepository
    {
        public MultistagedcRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region Implements

        /// <summary>
        /// 创建记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override bool Create(MultistageDc entity)
        {
            var max = _repository.Find("SELECT ISNULL(MAX(DisplayOrder),1)+1 displayorder FROM MultistageDc WHERE ParentMultistageDcId = @0", entity.ParentMultistagedcId);
            entity.DisplayOrder = max.DisplayOrder;
            return _repository.CreateObject(entity);
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool DeleteById(Guid id)
        {
            var deleted = _repository.FindById(id);
            var flag = _repository.Delete(id);
            Sql s = Sql.Builder;
            //重新排序
            s.Append("SELECT IDENTITY(INT,1,1) AS displayorder2,MultistageDcId INTO #tmp");
            s.Append("FROM [MultistageDc] WHERE ParentMultistageDcId=@0 ORDER BY displayorder;", deleted.ParentMultistagedcId);
            s.Append("UPDATE a SET DisplayOrder = b.displayorder2 FROM [MultistageDc] a");
            s.Append("INNER JOIN #tmp b ON a.MultistageDcId=b.MultistageDcId;");
            s.Append("DROP TABLE #tmp;");
            _repository.Execute(s);
            return flag;
        }

        /// <summary>
        /// 移动节点
        /// </summary>
        /// <param name="moveid"></param>
        /// <param name="targetid"></param>
        /// <param name="parentid"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public int MoveNode(Guid moveid, Guid targetid, Guid parentid, string position)
        {
            int result = 0;
            var moveNode = FindById(moveid);
            var targetNode = FindById(targetid);
            Sql s = Sql.Builder;
            switch (position)
            {
                case "after":
                    if (moveNode.ParentMultistagedcId == targetNode.ParentMultistagedcId)
                    {
                        //先把目标节点后面的节点序号+1
                        s.Append("UPDATE MultistageDc SET DisplayOrder=DisplayOrder+1 WHERE ParentMultistageDcId=@0 AND MultistageDcId<>@1 AND DisplayOrder>@2;", targetNode.ParentMultistagedcId, moveid, targetNode.DisplayOrder);
                        //移动节点序号等于目标节点的序号+1
                        s.Append("UPDATE MultistageDc SET DisplayOrder=" + (targetNode.DisplayOrder + 1) + " WHERE MultistageDcId=@0;", moveid);
                    }
                    break;

                case "inside":
                    if (moveNode.ParentMultistagedcId == targetid)
                    {
                        //移动节点排第一，其它的排序+1
                        s.Append("UPDATE MultistageDc SET DisplayOrder=0 WHERE MultistageDcId=@0;", moveid);
                        //重新排序
                        s.Append("SELECT IDENTITY(INT,1,1) AS displayorder2,MultistageDcId INTO #tmp");
                        s.Append("FROM [MultistageDc] WHERE ParentMultistageDcId=@0 AND MultistageDcId<>@1 ORDER BY displayorder;", moveNode.ParentMultistagedcId, moveid);
                        s.Append("UPDATE a SET DisplayOrder = b.displayorder2 FROM [MultistageDc] a");
                        s.Append("INNER JOIN #tmp b ON a.MultistageDcId=b.MultistageDcId;");
                        s.Append("DROP TABLE #tmp;");
                    }
                    break;

                default:
                    result = -1;
                    break;
            }
            if (s.SQL.IsNotEmpty())
            {
                //重新排序
                //s.Append("SELECT IDENTITY(INT,1,1) AS displayorder,MultistageDcId INTO #tmp");
                //s.Append("FROM [MultistageDc] WHERE ParentMultistageDcId=@0 ORDER BY displayorder", moveNode.ParentMultistageDcId);
                //s.Append("UPDATE a SET DisplayOrder = b.displayorder FROM [MultistageDc] a");
                //s.Append("INNER JOIN #tmp b ON a.MultistageDcId=b.MultistageDcId");
                //s.Append("DROP TABLE #tmp");
                _repository.Execute(s);
                result = 1;
            }
            return result;
            //SqlParameter[] ps = new SqlParameter[]{
            //    new SqlParameter() { SqlDbType = SqlDbType.Int,Value = moveid }
            //    ,new SqlParameter() { SqlDbType = SqlDbType.Int, Value = targetid }
            //    ,new SqlParameter() { SqlDbType = SqlDbType.NVarChar,Value = position }
            //    ,new SqlParameter() { SqlDbType = SqlDbType.Int,Value = 3 }
            //    ,new SqlParameter() { Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int }
            //};

            //((Database)_repository.Client).Execute("EXEC [usp_Security_UpdMultistageDcNode] @0,@1,@2,@3,@4 OUTPUT", ps);

            //return (int)ps[4].Value;
        }

        public string GetMultistageDcXml(Guid solutionId)
        {
            //var result = new DataRepositoryBase<object>(DbContext).Find(@"select convert(xml,(select MultistageDcId,DisplayName,SystemName,ClassName,MethodName,ParentMultistageDcId,Url,OpenTarget,DisplayOrder,IsEnable,IsShowAsMenu,Description,SmallIcon,BigIcon,Level
            //from MultistageDc
            //where exists (select 1 from SolutionComponent where SolutionId = @0 and ComponentType = @1)
            //FOR XML PATH('MultistageDc'),ROOT('MultistageDc')) )
            //", solutionId, SolutionComponentType.MultistageDc);
            //if (result != null)
            //{
            //    var data = (result as IDictionary<string, object>).ToList();
            //    if (data.NotEmpty() && data[0].Value != null)
            //    {
            //        return data[0].Value.ToString();
            //    }
            //}
            return string.Empty;
        }

        #endregion Implements
    }
}