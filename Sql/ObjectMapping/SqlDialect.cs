using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel;
using Microsoft.Practices.EnterpriseLibrary.Data.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace SSharing.Ubtrip.Common.Sql
{
    /// <summary>
    /// 表示Sql方言(描述各种关系数据库特有的使用特征)
    /// </summary>
    public abstract class SqlDialect
    {        
        /// <summary>
        /// 根据原来的SelectString得到计算总记录数的Sql字符串
        /// </summary>
        public virtual string GetCountSql(string selectString)
        {
            return string.Format("SELECT COUNT(1) FROM ({0}) tmpTable", selectString);
        }

        /// <summary>
        /// 获取分页的Sql语句
        /// </summary>
        /// <param name="selectString">原始SQL语句</param>
        /// <param name="startIndex">起始记录,从1开始</param>
        /// <param name="endIndex">截止记录,从1开始</param>
        /// <returns></returns>
        public abstract string GetPageSql(string selectString, string orderKey, int startIndex, int endIndex);
    }

    public class SqlServerDialect : SqlDialect
    {
        public static readonly SqlServerDialect Instance = new SqlServerDialect();

        public override string GetPageSql(string selectString, string orderKey, int startIndex, int endIndex)
        {
            return string.Format(
                "SELECT * FROM " +
                    "(SELECT ROW_NUMBER() Over(ORDER BY {3}) AS row_num, org_sql.* " +
                    "FROM ({0}) org_sql) tmpTable " +
                "WHERE row_num >= {1} AND row_num <= {2}",
                selectString, startIndex, endIndex, orderKey);
        }
    }

    public class MySqlDialect : SqlDialect
    {
        public static readonly MySqlDialect Instance = new MySqlDialect();

        public override string GetPageSql(string selectString, string orderKey, int startIndex, int endIndex)
        {
            throw new NotImplementedException();
            //return string.Format(
            //    "SELECT * FROM " +
            //        "(SELECT ROW_NUMBER() Over(ORDER BY {3}) AS row_num, org_sql.* " +
            //        "FROM ({0}) org_sql) tmpTable " +
            //    "WHERE row_num >= {1} AND row_num <= {2}",
            //    selectString, startIndex, endIndex, orderKey);
        }
    }

    //[DatabaseAssembler(typeof(OleDbDatabaseAssembler))]
    //public class OleDbDatabase : GenericDatabase
    //{
    //    public OleDbDatabase(string connectionString, DbProviderFactory dbProviderFactory)
    //        : base(connectionString, dbProviderFactory)
    //    {
    //    }

    //    public override string BuildParameterName(string name)
    //    {
    //        return "?";
    //    }
    //}

    //public class OleDbDatabaseAssembler : IDatabaseAssembler
    //{
    //    public Database Assemble(string name, ConnectionStringSettings connectionStringSettings, IConfigurationSource configurationSource)
    //    {
    //        return new OleDbDatabase(connectionStringSettings.ConnectionString, DbProviderFactories.GetFactory(connectionStringSettings.ProviderName));
    //    }
    //}

    [ConfigurationElementType(typeof(MySqlDatabaseData))]
    public class MySqlDatabase : Database
    {
        public MySqlDatabase(string connectionString, DbProviderFactory dbProviderFactory)
            : base(connectionString, dbProviderFactory)
        {            
        }

        public MySqlDatabase(string connectionString, DbProviderFactory dbProviderFactory,
                                IDataInstrumentationProvider instrumentationProvider)
            : base(connectionString, dbProviderFactory, instrumentationProvider)
        {
        }
        
        protected override void DeriveParameters(DbCommand discoveryCommand)
        {            
        }
    }

    public class MySqlDatabaseData : DatabaseData
    {
        public MySqlDatabaseData(ConnectionStringSettings connectionStringSettings, IConfigurationSource configurationSource)
            : base(connectionStringSettings, configurationSource)
        {
        }

        public override IEnumerable<TypeRegistration> GetRegistrations()
        {
            yield return new TypeRegistration<Database>(
                () => new MySqlDatabase(
                    ConnectionString, DbProviderFactories.GetFactory(ConnectionStringSettings.ProviderName)))
            {
                Name = Name,
                Lifetime = TypeRegistrationLifetime.Transient
            };
        }
        //public Database Assemble(string name, ConnectionStringSettings connectionStringSettings, IConfigurationSource configurationSource)
        //{
        //    return new MySqlDatabase(connectionStringSettings.ConnectionString, DbProviderFactories.GetFactory(connectionStringSettings.ProviderName));
        //}
    }
}
