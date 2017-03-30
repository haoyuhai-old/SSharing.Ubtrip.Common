using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ZhiBen.Framework.DataAccess
{
    public class DbInstance
    {
        // Fields
        private Database _Database;
        private DbTransaction _Transaction;

        // Methods
        internal DbInstance(Database database, DbTransaction transaction)
        {
            this._Database = database;
            this._Transaction = transaction;
        }

        /// <summary>
        /// 返回底层数据库连接的相关信息(微软企业库的Database对象)
        /// </summary>
        public Database GetDatabase()
        {
            return _Database;
        }

        public void AddInParameter(DbCommand command, string name, DbType dbType)
        {
            this._Database.AddInParameter(command, name, dbType);
        }

        public void AddInParameter(DbCommand command, string name, DbType dbType, object value)
        {
            this._Database.AddInParameter(command, name, dbType, value);
        }

        public void AddInParameter(DbCommand command, string name, DbType dbType, string sourceColumn, DataRowVersion sourceVersion)
        {
            this._Database.AddInParameter(command, name, dbType, sourceColumn, sourceVersion);
        }

        public void AddOutParameter(DbCommand command, string name, DbType dbType, int size)
        {
            this._Database.AddOutParameter(command, name, dbType, size);
        }

        public void AddParameter(DbCommand command, string name, DbType dbType, ParameterDirection direction, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            this._Database.AddParameter(command, name, dbType, direction, sourceColumn, sourceVersion, value);
        }

        public void AddParameter(DbCommand command, string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            this._Database.AddParameter(command, name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
        }

        public virtual DataSet ExecuteDataSet(DbCommand command)
        {
            if (this.IsInTransaction())
            {
                return this._Database.ExecuteDataSet(command, this._Transaction);
            }
            return this._Database.ExecuteDataSet(command);
        }

        public virtual DataSet ExecuteDataSet(CommandType commandType, string commandText)
        {
            if (this.IsInTransaction())
            {
                return this._Database.ExecuteDataSet(this._Transaction, commandType, MapSql(commandText));
            }
            return this._Database.ExecuteDataSet(commandType, MapSql(commandText));
        }

        public virtual DataSet ExecuteDataSet(string storedProcedureName, params object[] parameterValues)
        {
            if (this.IsInTransaction())
            {
                return this._Database.ExecuteDataSet(this._Transaction, MapSql(storedProcedureName), parameterValues);
            }
            return this._Database.ExecuteDataSet(MapSql(storedProcedureName), parameterValues);
        }

        public virtual DataSet ExecuteDataSetBySql(string sql)
        {
            return this.ExecuteDataSet(CommandType.Text, sql);
        }

        public virtual DataTable ExecuteDataTable(DbCommand command)
        {
            return this.ExecuteDataSet(command).Tables[0];
        }

        public virtual DataTable ExecuteDataTable(CommandType commandType, string commandText)
        {
            return this.ExecuteDataSet(commandType, commandText).Tables[0];
        }

        public virtual DataTable ExecuteDataTable(string storedProcedureName, params object[] parameterValues)
        {
            return this.ExecuteDataSet(storedProcedureName, parameterValues).Tables[0];
        }

        public virtual DataTable ExecuteDataTableBySql(string sql)
        {
            return this.ExecuteDataSetBySql(sql).Tables[0];
        }

        public virtual int ExecuteNonQuery(DbCommand command)
        {
            if (this.IsInTransaction())
            {
                return this._Database.ExecuteNonQuery(command, this._Transaction);
            }
            return this._Database.ExecuteNonQuery(command);
        }

        public virtual int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            if (this.IsInTransaction())
            {
                return this._Database.ExecuteNonQuery(this._Transaction, commandType, MapSql(commandText));
            }
            return this._Database.ExecuteNonQuery(commandType, MapSql(commandText));
        }

        public virtual int ExecuteNonQuery(string storedProcedureName, params object[] parameterValues)
        {
            if (this.IsInTransaction())
            {
                return this._Database.ExecuteNonQuery(this._Transaction, MapSql(storedProcedureName), parameterValues);
            }
            return this._Database.ExecuteNonQuery(MapSql(storedProcedureName), parameterValues);
        }

        public virtual int ExecuteNonQueryBySql(string sql)
        {
            return this.ExecuteNonQuery(CommandType.Text, sql);
        }

        public virtual IDataReader ExecuteReader(DbCommand command)
        {
            if (this.IsInTransaction())
            {
                return this._Database.ExecuteReader(command, this._Transaction);
            }
            return this._Database.ExecuteReader(command);
        }

        public virtual IDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            if (this.IsInTransaction())
            {
                return this._Database.ExecuteReader(this._Transaction, commandType, MapSql(commandText));
            }
            return this._Database.ExecuteReader(commandType, MapSql(commandText));
        }

        public virtual IDataReader ExecuteReader(string storedProcedureName, params object[] parameterValues)
        {
            if (this.IsInTransaction())
            {
                return this._Database.ExecuteReader(this._Transaction, MapSql(storedProcedureName), parameterValues);
            }
            return this._Database.ExecuteReader(MapSql(storedProcedureName), parameterValues);
        }

        public virtual IDataReader ExecuteReaderBySql(string sql)
        {
            return this.ExecuteReader(CommandType.Text, sql);
        }

        public virtual object ExecuteScalar(DbCommand command)
        {
            if (this.IsInTransaction())
            {
                return this._Database.ExecuteScalar(command, this._Transaction);
            }
            return this._Database.ExecuteScalar(command);
        }

        public virtual object ExecuteScalar(CommandType commandType, string commandText)
        {
            if (this.IsInTransaction())
            {
                return this._Database.ExecuteScalar(this._Transaction, commandType, MapSql(commandText));
            }
            return this._Database.ExecuteScalar(commandType, MapSql(commandText));
        }

        public virtual object ExecuteScalar(string storedProcedureName, params object[] parameterValues)
        {
            if (this.IsInTransaction())
            {
                return this._Database.ExecuteScalar(this._Transaction, MapSql(storedProcedureName), parameterValues);
            }
            return this._Database.ExecuteScalar(MapSql(storedProcedureName), parameterValues);
        }

        public virtual object ExecuteScalarBySql(string sql)
        {
            return this.ExecuteScalar(CommandType.Text, sql);
        }

        public DbDataAdapter GetDataAdapter()
        {
            return this._Database.GetDataAdapter();
        }

        public object GetParameterValue(DbCommand command, string name)
        {
            return this._Database.GetParameterValue(command, name);
        }

        public DbCommand GetSqlStringCommand(string query)
        {
            return this._Database.GetSqlStringCommand(MapSql(query));
        }

        public DbCommand GetStoredProcCommand(string storedProcedureName)
        {
            return this._Database.GetStoredProcCommand(MapSql(storedProcedureName));
        }

        public virtual DbCommand GetStoredProcCommand(string storedProcedureName, params object[] parameterValues)
        {
            return this._Database.GetStoredProcCommand(MapSql(storedProcedureName), parameterValues);
        }

        public virtual DbCommand GetStoredProcCommandWithSourceColumns(string storedProcedureName, params string[] sourceColumns)
        {
            return this._Database.GetStoredProcCommandWithSourceColumns(MapSql(storedProcedureName), sourceColumns);
        }

        private bool IsInTransaction()
        {
            return (this._Transaction != null);
        }

        public virtual void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames)
        {
            if (this.IsInTransaction())
            {
                this._Database.LoadDataSet(command, dataSet, tableNames, this._Transaction);
            }
            else
            {
                this._Database.LoadDataSet(command, dataSet, tableNames);
            }
        }

        public void LoadDataSet(DbCommand command, DataSet dataSet, string tableName)
        {
            this.LoadDataSet(command, dataSet, new string[] { tableName });
        }

        public virtual void LoadDataSet(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            if (this.IsInTransaction())
            {
                this._Database.LoadDataSet(this._Transaction, commandType, MapSql(commandText), dataSet, tableNames);
            }
            else
            {
                this._Database.LoadDataSet(commandType, MapSql(commandText), dataSet, tableNames);
            }
        }

        public virtual void LoadDataSet(string storedProcedureName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            if (this.IsInTransaction())
            {
                this._Database.LoadDataSet(this._Transaction, MapSql(storedProcedureName), dataSet, tableNames, parameterValues);
            }
            else
            {
                this._Database.LoadDataSet(MapSql(storedProcedureName), dataSet, tableNames, parameterValues);
            }
        }

        public virtual void LoadDataSetBySql(string sql, DataSet dataSet, string[] tableNames)
        {
            this.LoadDataSet(CommandType.Text, sql, dataSet, tableNames);
        }

        private static string MapSql(string sql)
        {
            if (sql.StartsWith("$"))
            {
                return SqlMapper.GetSql(sql.Remove(0, 1));
            }
            return sql;
        }

        public void SetParameterValue(DbCommand command, string parameterName, object value)
        {
            this._Database.SetParameterValue(command, parameterName, value);
        }

        public int UpdateDataSet(DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand)
        {
            return this.UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, UpdateBehavior.Transactional);
        }

        public virtual int UpdateDataSet(DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand, UpdateBehavior updateBehavior)
        {
            if (this.IsInTransaction())
            {
                return this._Database.UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, this._Transaction);
            }
            return this._Database.UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, updateBehavior);
        }
    }
}