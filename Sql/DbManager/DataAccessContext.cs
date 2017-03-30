using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

using Microsoft.Practices.EnterpriseLibrary.Data;
namespace ZhiBen.Framework.DataAccess
{
    internal class DataAccessContext
    {
        // Fields
        private DbConnection _Connection;
        private Database _Database;
        private string _DatabaseName;
        private DbTransaction _Transaction;

        // Methods
        public DataAccessContext(string databaseName, Database database)
        {
            this._DatabaseName = databaseName;
            this._Database = database;
        }

        public void BeginTransaction()
        {
            BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            this._Connection = this._Database.CreateConnection();
            this._Connection.Open();
            this._Transaction = this._Connection.BeginTransaction(isolationLevel);
        }

        public void CloseConnection()
        {
            //if (this._Connection.get_State() == ConnectionState.Open)
            if (this._Connection.State  == ConnectionState.Open)
            {
                this._Connection.Close();
            }
        }

        public void Commit()
        {
            this._Transaction.Commit();
        }

        public void Rollback()
        {
            this._Transaction.Rollback();
        }

        // Properties
        public Database Database
        {
            get
            {
                return this._Database;
            }
        }

        public DbTransaction Transaction
        {
            get
            {
                return this._Transaction;
            }
        }
    }
}
