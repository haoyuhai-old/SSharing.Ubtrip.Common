using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;

namespace SSharing.Ubtrip.Common.Sql
{
    public static class SqlManager
    {
        static string _DefaultDataBase = "";
        static Dictionary<string, ISql> _ISqls = new Dictionary<string, ISql>();


        /// <summary>
        /// 默认构造函数
        /// </summary>
        static SqlManager()
        {
            foreach (ConnectionStringSettings css in ConfigurationManager.ConnectionStrings)
            {
                switch (css.ProviderName)
                {
                    case DbProviderMapping.DefaultOracleProviderName:
                        _ISqls.Add(css.Name, new OracleSql());
                        break;
                    case DbProviderMapping.DefaultSqlProviderName:
                        _ISqls.Add(css.Name, new SqlServerSql());
                        break;
                    case "MySql.Data.MySqlClient":
                        _ISqls.Add(css.Name, new MySqlSql());
                        break;
                    case "System.Data.OleDb":
                        _ISqls.Add(css.Name, new SqlServerSql());
                        break;
                    default:
                        throw new Exception(string.Format("数据库类型异常.暂不支持数据库{0}:{1}",css.Name,css.ProviderName));
                }
            }

            DatabaseSettings ds = (DatabaseSettings)ConfigurationManager.GetSection(DatabaseSettings.SectionName);
            _DefaultDataBase = ds.DefaultDatabase;
        }


        /// <summary>
        /// 获取默认设置
        /// </summary>
        /// <returns></returns>
        public static ISql GetISql()
        {
            return GetISql(_DefaultDataBase);
        }

        /// <summary>
        /// 获取指定配置名称数据库ISQL
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static ISql GetISql(string dbName)
        {
            if (_ISqls.ContainsKey(dbName))
            {
                return _ISqls[dbName];
            }
            else
            {
                throw new Exception(string.Format("数据库名称异常.找不到数据库:{0}", dbName));
            }
        }
    }
}
