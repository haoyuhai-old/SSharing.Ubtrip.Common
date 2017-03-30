using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace SSharing.Ubtrip.Common
{
    public class SqlDependencyExpiration : ICacheItemExpiration
    {
        //private static List<string> StartedConnectionNames = new List<string>();

        private static readonly CommandType DefaultComamndType = CommandType.StoredProcedure;

        public event EventHandler Expired;

        public bool HasChanged { get; set; }

        public string ConnectionName { get; set; }
        
        public SqlDependencyExpiration(string commandText, IDictionary<string, object> parameters) :
            this(commandText, DefaultComamndType, string.Empty, parameters)
        { }

        public SqlDependencyExpiration(string commandText, string connectionStringName, IDictionary<string, object> parameters) :
            this(commandText, DefaultComamndType, connectionStringName, parameters)
        { }

        public SqlDependencyExpiration(string commandText, CommandType commandType, IDictionary<string, object> parameters) :
            this(commandText, commandType, string.Empty, parameters)
        { }
        
        public SqlDependencyExpiration(string commandText, CommandType commandType, string connectionStringName, IDictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(connectionStringName))
            {
                this.ConnectionName = DatabaseSettings.GetDatabaseSettings(ConfigurationSourceFactory.Create()).DefaultDatabase;
            }
            else
            {
                this.ConnectionName = connectionStringName;
            }

            //if (!StartedConnectionNames.Contains(this.ConnectionName))
            //{
                bool success = SqlDependency.Start(ConfigurationManager.ConnectionStrings[this.ConnectionName].ConnectionString);
            //    if (success)
            //    {
            //        StartedConnectionNames.Add(this.ConnectionName);
            //    }
            //    else
            //    {
            //        throw new Exception("已存在兼容的侦听器：" + this.ConnectionName);
            //    }
            //}
            using (SqlConnection sqlConnection = DatabaseFactory.CreateDatabase(this.ConnectionName).CreateConnection() as SqlConnection)
            {
                SqlCommand command = new SqlCommand(commandText, sqlConnection);
                command.CommandType = commandType;
                if (parameters != null)
                {
                    this.AddParameters(command, parameters);
                }
                SqlDependency dependency = new SqlDependency(command);
                dependency.OnChange += delegate
                {
                    this.HasChanged = true;
                    if (this.Expired != null)
                    {
                        this.Expired(this, new EventArgs());
                    }
                };
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }
                command.ExecuteNonQuery();
            }
        }

        private void AddParameters(SqlCommand command, IDictionary<string, object> parameters)
        {
            command.Parameters.Clear();
            foreach (var parameter in parameters)
            {
                string parameterName = parameter.Key;
                if (!parameter.Key.StartsWith("@"))
                {
                    parameterName = "@" + parameterName;
                }

                command.Parameters.Add(new SqlParameter(parameterName, parameter.Value));
            }
        }

        #region ICacheItemExpiration Members

        public bool HasExpired()
        {
            bool indicator = this.HasChanged;
            this.HasChanged = false;
            return indicator;
        }

        public void Initialize(CacheItem owningCacheItem)
        {

        }

        public void Notify()
        {

        }

        #endregion
    }
}
