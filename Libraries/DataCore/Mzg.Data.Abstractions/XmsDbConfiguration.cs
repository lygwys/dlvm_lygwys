namespace Mzg.Data.Abstractions
{
    /// <summary>
    /// 基础数据库参数,查询数据的最大时间为3分钟
    /// xmg
    /// 202007061905
    /// </summary>
    public class XmsDbConfiguration : IDataProviderOptions
    {
        public XmsDbConfiguration(Core.Org.IOrgDataServer orgDataServer)
        {
            if (orgDataServer != null)
            {
                ConnectionString = string.Format("Data Source={0};User ID={1};Password={2};Initial Catalog={3};Pooling=true;max pool size=512;{4};MultipleActiveResultSets=true;"//MultipleActiveResultSets=true;
                    , orgDataServer.DataServerName, orgDataServer.DataAccountName, orgDataServer.DataPassword, orgDataServer.DatabaseName, CommandTimeOut > 0 ? string.Format("connect timeout={0};", CommandTimeOut) : "");
            }
        }

        public int CommandTimeOut { get; set; } = 180;
        public string ConnectionString { get; set; }
        public string ProviderName { get; set; } = "System.Data.SqlClient";
        public string DbType { get; set; }
    }
}