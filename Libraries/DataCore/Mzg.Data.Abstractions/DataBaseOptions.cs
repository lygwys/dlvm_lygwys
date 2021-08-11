using Microsoft.Extensions.Options;

namespace Mzg.Data.Abstractions
{
    /// <summary>
    /// 基础数据库参数,查询数据的最大时间为3分钟
    /// xmg
    /// 202007061905
    /// </summary>
    public class DataBaseOptions : IOptions<DataBaseOptions>, IDataProviderOptions
    {
        public string DbType { get; set; }
        public int CommandTimeOut { get; set; } = 180;
        public string ConnectionString { get; set; }
        public string ProviderName { get; set; } = "System.Data.SqlClient";

        public DataBaseOptions Value => this;
    }
}