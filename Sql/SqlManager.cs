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
        /// Ĭ�Ϲ��캯��
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
                        throw new Exception(string.Format("���ݿ������쳣.�ݲ�֧�����ݿ�{0}:{1}",css.Name,css.ProviderName));
                }
            }

            DatabaseSettings ds = (DatabaseSettings)ConfigurationManager.GetSection(DatabaseSettings.SectionName);
            _DefaultDataBase = ds.DefaultDatabase;
        }


        /// <summary>
        /// ��ȡĬ������
        /// </summary>
        /// <returns></returns>
        public static ISql GetISql()
        {
            return GetISql(_DefaultDataBase);
        }

        /// <summary>
        /// ��ȡָ�������������ݿ�ISQL
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
                throw new Exception(string.Format("���ݿ������쳣.�Ҳ������ݿ�:{0}", dbName));
            }
        }
    }
}
