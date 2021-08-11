﻿using Microsoft.Extensions.Options;
using Mzg.Core.Data;
using Mzg.Data.Abstractions;
using System.Data;

namespace Mzg.Data
{
    public class DataVisitor
    {
        private readonly DataRepositoryBase<dynamic> _repository;

        public DataVisitor(IOptionsMonitor<DataBaseOptions> options)
        {
            _repository = new DataRepositoryBase<dynamic>(options.CurrentValue);
        }

        public DataVisitor(IDbContext context)
        {
            _repository = new DataRepositoryBase<dynamic>(context);
        }

        /// <summary>
        /// 查询数据集
        /// </summary>
        /// <param name="s"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public DataSet ExecuteQueryDataSet(string s, params object[] args)
        {
            return ((DbContext)_repository.GetDbContext()).ExecuteQuery(s, args);
        }
    }
}