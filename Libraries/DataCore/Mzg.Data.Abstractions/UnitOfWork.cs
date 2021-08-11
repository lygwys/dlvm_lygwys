using Mzg.Core.Data;
using System;

namespace Mzg.Data.Abstractions
{
    /// <summary>
    /// 事务工作单元
    /// </summary>
    public class UnitOfWork : IDisposable
    {
        private readonly IDbContext _dbContext;

        //禁止用注入的方式实例化
        private UnitOfWork(IDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext.BeginTransaction();
        }

        public void Dispose()
        {
            if (!_dbContext.TransactionCancelled)
            {
                _dbContext.CompleteTransaction();
            }
        }

        public static UnitOfWork Build(IDbContext dbContext)
        {
            return new UnitOfWork(dbContext);
        }
    }
}