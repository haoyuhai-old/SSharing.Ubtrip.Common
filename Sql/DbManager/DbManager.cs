using System;
using System.Collections.Generic;
using System.Text;
using ZhiBen.Framework.Service.Common;
using System.Data;
using System.Data.Common;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;

namespace ZhiBen.Framework.DataAccess
{
    public class DbManager
    {
        // Fields
        private static Dictionary<string, Database> _DatabaseLookup = new Dictionary<string, Database>(StringComparer.InvariantCultureIgnoreCase);//StringComparer.get_InvariantCultureIgnoreCase()
        private static object _LockObject = new object();
        public static readonly string DefaultDatabaseName = DatabaseSettings.GetDatabaseSettings(ConfigurationSourceFactory.Create()).DefaultDatabase;
        private const string KEY_SUFFIX = "@ZhiBen.framework.dataaccess.context";

        // Methods
        public static void BeginTransaction()
        {
            BeginTransaction(DefaultDatabaseName, IsolationLevel.ReadCommitted);
        }

        public static void BeginTransaction(string databaseName)
        {
            BeginTransaction(databaseName, IsolationLevel.ReadCommitted);
        }

        public static void BeginTransaction(string databaseName, IsolationLevel isolationLevel)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentNullException("databaseName", "数据库名称不能为空。");
            }
            string key = GenerateKey(databaseName);
            if (ServiceContext.Current.Contains(key))
            {
                throw new InvalidOperationException(string.Format("数据库{0}的上下文环境已经存在。", databaseName));
            }
            DataAccessContext context = new DataAccessContext(databaseName, GetDatabase(databaseName));
            context.BeginTransaction(isolationLevel);
            ServiceContext.Current.Add(key, context);
        }

        public static bool IsInTransaction(string databaseName)
        {
            string key = GenerateKey(databaseName);
            return ServiceContext.Current.Contains(key);
        }

        public static void Commit()
        {
            Commit(DefaultDatabaseName);
        }

        public static void Commit(string databaseName)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentNullException(databaseName, "数据库名称不能为空。");
            }
            string key = GenerateKey(databaseName);
            if (!ServiceContext.Current.Contains(key))
            {
                throw new InvalidOperationException(string.Format("数据库{0}的上下文环境不存在。", databaseName));
            }
            DataAccessContext context = (DataAccessContext)ServiceContext.Current[key];
            try
            {
                context.Commit();
            }
            catch
            {
                context.Rollback();
                throw;
            }
            finally
            {
                context.CloseConnection();
                ServiceContext.Current.Remove(key);
            }
        }

        private static DbInstance CreateDbInstance(DbInstanceType type, Database database, DbTransaction transaction)
        {
            if (type == DbInstanceType.Default)
            {
                return new DbInstance(database, transaction);
            }
            return new AutoParameterDbInstance(database, transaction);
        }

        private static string GenerateKey(string databaseName)
        {
            return (databaseName + "@ZhiBen.framework.dataaccess.context");
        }

        public static AutoParameterDbInstance GetAutoParameterDbInstance()
        {
            return GetAutoParameterDbInstance(DefaultDatabaseName);
        }

        public static AutoParameterDbInstance GetAutoParameterDbInstance(string databaseName)
        {
            return (AutoParameterDbInstance)GetDbInstance(DbInstanceType.AutoParameter, databaseName);
        }

        private static Database GetDatabase(string databaseName)
        {
            if (!_DatabaseLookup.ContainsKey(databaseName))
            {
                lock (_LockObject)
                {
                    if (!_DatabaseLookup.ContainsKey(databaseName))
                    {
                        _DatabaseLookup.Add(databaseName, DatabaseFactory.CreateDatabase(databaseName));
                    }
                }
            }
            Database dbRet = null;
            _DatabaseLookup.TryGetValue(databaseName,out dbRet);
            return dbRet;
            //return _DatabaseLookup.get_Item(databaseName);
        }

        public static DbInstance GetDbInstance()
        {
            return GetDbInstance(DefaultDatabaseName);
        }

        public static DbInstance GetDbInstance(string databaseName)
        {
            return GetDbInstance(DbInstanceType.Default, databaseName);
        }

        private static DbInstance GetDbInstance(DbInstanceType type, string databaseName)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentNullException("databaseName", "数据库名称不能为空。");
            }
            string key = GenerateKey(databaseName);
            if (ServiceContext.Current.Contains(key))
            {
                DataAccessContext context = (DataAccessContext)ServiceContext.Current[key];
                return CreateDbInstance(type, context.Database, context.Transaction);
            }
            return CreateDbInstance(type, GetDatabase(databaseName), null);
        }

        public static void Rollback()
        {
            Rollback(DefaultDatabaseName);
        }

        public static void Rollback(string databaseName)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentNullException("databaseName", "数据库名称不能为空。");
            }
            string key = GenerateKey(databaseName);
            if (ServiceContext.Current.Contains(key))
            {
                DataAccessContext context = (DataAccessContext)ServiceContext.Current[key];
                try
                {
                    context.Rollback();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    context.CloseConnection();
                    ServiceContext.Current.Remove(key);
                }
            }
        }
    }


}
