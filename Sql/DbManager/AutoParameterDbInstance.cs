using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Data;
using System.Data.Common;

using Microsoft.Practices.EnterpriseLibrary.Data;
using ZhiBen.Framework.Service.Common;

namespace ZhiBen.Framework.DataAccess
{
    public class AutoParameterDbInstance : DbInstance
    {
        // Fields
        private readonly string CurrentOperator;
        private readonly decimal CurrentOrganizationId;
        private static readonly string ORG_FIELD_NAME;
        private static readonly string ORG_PARAM_NAME;

        // Methods
        static AutoParameterDbInstance()
        {
            string str = ConfigurationManager.AppSettings["OrganizationIdFieldName"];
            if (string.IsNullOrEmpty(str))
            {
                str = "organization_id";
            }
            ORG_PARAM_NAME = "pn_" + str;
            ORG_FIELD_NAME = str;
        }

        internal AutoParameterDbInstance(Database database, DbTransaction transaction)
            : base(database, transaction)
        {
            this.CurrentOperator = ServiceContext.Current.User.UseId;
            this.CurrentOrganizationId = ServiceContext.Current.OrganizationId;
        }

        private void AddFixedParameter(DbCommand command)
        {
            if (!command.Parameters.Contains(ORG_PARAM_NAME))
            {
                base.AddInParameter(command, ORG_PARAM_NAME, DbType.Decimal, 0);
            }
        }

        private object[] AddFixedParameterValue(object[] parameterValues)
        {
            if (parameterValues == null)
            {
                return new object[] { this.CurrentOrganizationId };
            }
            object[] array = new object[parameterValues.Length + 1];
            parameterValues.CopyTo(array, 0);
            array[array.Length - 1] = this.CurrentOrganizationId;
            return array;
        }

        private static string[] AddFixedSourceColumn(string[] sourceColumns)
        {
            if (sourceColumns == null)
            {
                return new string[] { ORG_FIELD_NAME };
            }
            string[] array = new string[sourceColumns.Length + 1];
            sourceColumns.CopyTo(array, 0);
            array[array.Length - 1] = ORG_FIELD_NAME;
            return array;
        }

        public override DataSet ExecuteDataSet(DbCommand command)
        {
            this.AddFixedParameter(command);
            return base.ExecuteDataSet(command);
        }

        public override DataSet ExecuteDataSet(CommandType commandType, string commandText)
        {
            if (commandType == CommandType.StoredProcedure)
            {
                return this.ExecuteDataSet(base.GetStoredProcCommand(commandText));
            }
            return base.ExecuteDataSet(commandType, commandText);
        }

        public override DataSet ExecuteDataSet(string storedProcedureName, params object[] parameterValues)
        {
            return base.ExecuteDataSet(storedProcedureName, this.AddFixedParameterValue(parameterValues));
        }

        public override int ExecuteNonQuery(DbCommand command)
        {
            this.AddFixedParameter(command);
            return base.ExecuteNonQuery(command);
        }

        public override int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            if (commandType == CommandType.StoredProcedure)
            {
                return base.ExecuteNonQuery(base.GetStoredProcCommand(commandText));
            }
            return base.ExecuteNonQuery(commandType, commandText);
        }

        public override int ExecuteNonQuery(string storedProcedureName, params object[] parameterValues)
        {
            return base.ExecuteNonQuery(storedProcedureName, this.AddFixedParameterValue(parameterValues));
        }

        public override IDataReader ExecuteReader(DbCommand command)
        {
            this.AddFixedParameter(command);
            return base.ExecuteReader(command);
        }

        public override IDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            if (commandType == CommandType.StoredProcedure)
            {
                return this.ExecuteReader(commandText, new object[0]);
            }
            return base.ExecuteReader(commandType, commandText);
        }

        public override IDataReader ExecuteReader(string storedProcedureName, params object[] parameterValues)
        {
            return base.ExecuteReader(storedProcedureName, this.AddFixedParameterValue(parameterValues));
        }

        public override object ExecuteScalar(DbCommand command)
        {
            this.AddFixedParameter(command);
            return base.ExecuteScalar(command);
        }

        public override object ExecuteScalar(CommandType commandType, string commandText)
        {
            if (commandType == CommandType.StoredProcedure)
            {
                return this.ExecuteScalar(base.GetStoredProcCommand(commandText));
            }
            return base.ExecuteScalar(commandType, commandText);
        }

        public override object ExecuteScalar(string storedProcedureName, params object[] parameterValues)
        {
            return base.ExecuteScalar(storedProcedureName, this.AddFixedParameterValue(parameterValues));
        }

        public override DbCommand GetStoredProcCommand(string storedProcedureName, params object[] parameterValues)
        {
            return base.GetStoredProcCommand(storedProcedureName, this.AddFixedParameterValue(parameterValues));
        }

        public override DbCommand GetStoredProcCommandWithSourceColumns(string storedProcedureName, params string[] sourceColumns)
        {
            return base.GetStoredProcCommandWithSourceColumns(storedProcedureName, AddFixedSourceColumn(sourceColumns));
        }

        public override void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames)
        {
            this.AddFixedParameter(command);
            base.LoadDataSet(command, dataSet, tableNames);
        }

        public override void LoadDataSet(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            if (commandType == CommandType.StoredProcedure)
            {
                this.LoadDataSet(base.GetStoredProcCommand(commandText), dataSet, tableNames);
            }
            else
            {
                base.LoadDataSet(commandType, commandText, dataSet, tableNames);
            }
        }

        public override void LoadDataSet(string storedProcedureName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            base.LoadDataSet(storedProcedureName, dataSet, tableNames, this.AddFixedParameterValue(parameterValues));
        }

        private void SetDataOperator(DataSet dataSet, string tableName, object dataOperator)
        {
            DataTable table = dataSet.Tables[tableName];
            if (table.Columns.Contains("last_updated_by"))
            {
                object obj2 = (dataOperator != null) ? dataOperator : this.CurrentOperator;
                foreach (DataRow row in table.Rows)
                {
                    switch (row.RowState)
                    {
                        case DataRowState.Added:
                            row["created_by"] = obj2;
                            row["last_updated_by"] = obj2;
                            break;

                        case DataRowState.Modified:
                            goto Label_0076;
                    }
                    continue;
                Label_0076:
                    row["last_updated_by"] = obj2;
                }
            }
        }

        private void SetDataOrganizationId(DataSet dataSet, string tableName)
        {
            DataTable table = dataSet.Tables[tableName];
            if (table.Columns.Contains(ORG_FIELD_NAME))
            {
                foreach (DataRow row in table.Rows)
                {
                    if ((row.RowState == DataRowState.Added) || (row.RowState == DataRowState.Modified))
                    {
                        row[ORG_FIELD_NAME] = this.CurrentOrganizationId;
                    }
                }
            }
        }

        public override int UpdateDataSet(DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand, UpdateBehavior updateBehavior)
        {
            return this.UpdateDataSetCore(dataSet, tableName, insertCommand, updateCommand, deleteCommand, updateBehavior, null);
        }

        private int UpdateDataSetCore(DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand, UpdateBehavior updateBehavior, object dataOperator)
        {
            this.SetDataOperator(dataSet, tableName, dataOperator);
            this.SetDataOrganizationId(dataSet, tableName);
            if (insertCommand != null)
            {
                this.AddFixedParameter(insertCommand);
            }
            if (updateCommand != null)
            {
                this.AddFixedParameter(updateCommand);
            }
            if (deleteCommand != null)
            {
                this.AddFixedParameter(deleteCommand);
            }
            return base.UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, updateBehavior);
        }

        public void UpdateDataSetWithOperator(DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand, object dataOperator)
        {
            this.UpdateDataSetWithOperator(dataSet, tableName, insertCommand, updateCommand, deleteCommand, UpdateBehavior.Transactional, dataOperator);
        }

        public void UpdateDataSetWithOperator(DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand, UpdateBehavior updateBehavior, object dataOperator)
        {
            this.UpdateDataSetCore(dataSet, tableName, insertCommand, updateCommand, deleteCommand, updateBehavior, dataOperator);
        }
    }


}
