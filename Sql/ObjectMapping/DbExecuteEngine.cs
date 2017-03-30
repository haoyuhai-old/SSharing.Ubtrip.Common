using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZhiBen.Framework.DataAccess;
using System.Data;
using System.Data.Common;
using System.Collections;
using SSharing.Ubtrip.Common.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace SSharing.Ubtrip.Common.Sql
{
    /// <summary>
    /// 封装执行数据库语句的逻辑
    /// </summary>
    class DbExecuteEngine
    {
        string _DatabaseName;
        DbInstance _DbInstance;
        SqlDialect _Dialect;

        public DbExecuteEngine(string databaseName)
        {
            _DatabaseName = databaseName;
            _DbInstance = string.IsNullOrEmpty(databaseName) ? DbManager.GetDbInstance() : DbManager.GetDbInstance(databaseName);

            Type dataBaseType = _DbInstance.GetDatabase().GetType();
            if (dataBaseType == typeof(SqlDatabase))
            {
                _Dialect = SqlServerDialect.Instance;
            }
            else if (dataBaseType == typeof(MySqlDatabase))
            {
                _Dialect = MySqlDialect.Instance;
            }            
            else
            {
                throw new SqlException("不支持的数据库类型:" + dataBaseType.Name);
            }
        }

        public Database GetDatabase()
        {
            return _DbInstance.GetDatabase();
        }

        #region Insert
        public void Insert(object obj, bool includeIdetityColumnInSql)
        {
            BaseEntity baseEntity = obj as BaseEntity;
            if (baseEntity != null)
            {
                if (string.IsNullOrEmpty(baseEntity.EnabledFlag))
                {
                    //插入时如果EnabledFlag为空，则默认为Y
                    baseEntity.EnabledFlag = "Y";
                }
            }

            #region 创建列集合
            TableMetaData table = MetaDataCacheManager.GetTableMetaData(obj.GetType());
            List<string> columnNames = new List<string>();
            List<object> columnValues = new List<object>();
            List<DbType> columnDbTypes = new List<DbType>();

            if (table.PKColumn != null)
            {
                if (table.PKColumn.ColumnCategory != Category.IdentityKey
                    || includeIdetityColumnInSql)
                {
                    //当存在非自动增长的主键，或者存在自动增长主键，但需要在insert语句中包含指定的列时
                    columnNames.Add(table.PKColumn.ColumnName);
                    columnValues.Add(table.PKColumn.PropertyAccessor.GetValue(obj));
                    columnDbTypes.Add(GetDbType(table.PKColumn.PropertyType));
                }
            }

            if (table.VersionColumn != null)
            {
                #region 处理版本控制列
                columnNames.Add(table.VersionColumn.ColumnName);

                //Version列赋初值
                object versionValue;
                if (table.VersionColumn.PropertyType == typeof(DateTime))
                {
                    DateTime dt = DateTime.Now;
                    versionValue = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
                }
                else if (table.VersionColumn.PropertyType == typeof(Int32))
                {
                    versionValue = 1;
                }
                else if (table.VersionColumn.PropertyType == typeof(Int64))
                {
                    versionValue = (long)1;
                }
                else
                {
                    throw new MappingException(string.Format("实体类型{0}中的Version列{1}的数据类型无效", obj.GetType().Name, table.VersionColumn.ColumnName));
                }
                table.VersionColumn.PropertyAccessor.SetValue(obj, versionValue);
                columnValues.Add(versionValue);
                columnDbTypes.Add(GetDbType(table.VersionColumn.PropertyType));
                #endregion
            }
                        
            foreach (ColumnMetaData column in table.NormalColumns)
            {
                columnNames.Add(column.ColumnName);
                columnValues.Add(column.PropertyAccessor.GetValue(obj));
                columnDbTypes.Add(GetDbType(column.PropertyType));
            }
            #endregion

            #region 生成Insert Command
            string format = "Insert Into {0}({1}) Values({2});";
            StringBuilder tempNames = new StringBuilder();
            StringBuilder tempParamNames = new StringBuilder();
            foreach (string columnName in columnNames)
            {
                tempNames.Append(columnName).Append(",");
                tempParamNames.Append("@").Append(columnName).Append(",");
            }
            tempNames.Remove(tempNames.Length - 1, 1);
            tempParamNames.Remove(tempParamNames.Length - 1, 1);

            DbCommand dbCommand = _DbInstance.GetSqlStringCommand(string.Format(format, table.TableName, tempNames.ToString(), tempParamNames.ToString()));

            for (int i = 0; i < columnNames.Count; i++)
            {
                _DbInstance.AddInParameter(dbCommand, "@" + columnNames[i], columnDbTypes[i], TransformToSqlParamValue(columnValues[i]));
            }
            #endregion

            //执行sql命令
            try
            {
                _DbInstance.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw new SqlException(ex.Message, LogHelper.GetSqlString(dbCommand), ex);
            }

            //取回标识值
            if (table.PKColumn != null && table.PKColumn.ColumnCategory == Category.IdentityKey && !includeIdetityColumnInSql)
            {
                object newIdValue = _DbInstance.ExecuteScalar(CommandType.Text, "SELECT CAST(@@IDENTITY as bigint) as value");
                table.PKColumn.PropertyAccessor.SetValue(obj, TransformToObjValueFromDb(newIdValue, table.PKColumn.PropertyType));
            }
        }
        #endregion

        #region Update
        public void Update(object obj, bool ignoreVersionCheck, List<string> updateColumnList)
        {            
            TableMetaData table = MetaDataCacheManager.GetTableMetaData(obj.GetType());
            if (table.PKColumn == null)
                throw new InvalidOperationException(string.Format("类型{0}未定义主键列，无法执行更新操作", obj.GetType().Name));

            List<string> columnNames = new List<string>();
            List<object> columnValues = new List<object>();
            List<DbType> columnDbTypes = new List<DbType>();
            object newVersion = null;
            object oldVersion = null;
            if (table.VersionColumn != null)
            {
                #region 处理标识列
                columnNames.Add(table.VersionColumn.ColumnName);
                
                if (table.VersionColumn.PropertyType == typeof(DateTime))
                {
                    DateTime oldDt = (DateTime)table.VersionColumn.PropertyAccessor.GetValue(obj);
                    if (!ignoreVersionCheck && oldDt == DateTime.MinValue)
                        throw new SqlException("更新对象时版本列的值为空");
                    oldVersion = oldDt;
                    DateTime dt = DateTime.Now;
                    newVersion = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);                    
                }
                else if (table.VersionColumn.PropertyType == typeof(Int32))
                {
                    oldVersion = table.VersionColumn.PropertyAccessor.GetValue(obj);
                    newVersion = (int)oldVersion + 1;
                }
                else if (table.VersionColumn.PropertyType == typeof(Int64))
                {
                    oldVersion = table.VersionColumn.PropertyAccessor.GetValue(obj);
                    newVersion = (long)oldVersion + 1;
                }
                else
                {
                    throw new MappingException(string.Format("实体类型{0}中的Version列{1}的数据类型无效", obj.GetType().Name, table.VersionColumn.ColumnName));
                }
                
                columnValues.Add(newVersion);
                columnDbTypes.Add(GetDbType(table.VersionColumn.PropertyType));
                #endregion
            }

            if (updateColumnList.Count == 0)
            {
                foreach (ColumnMetaData column in table.NormalColumns)
                {
                    if (column.ColumnCategory != Category.ReadOnly)
                    {
                        columnNames.Add(column.ColumnName);
                        columnValues.Add(column.PropertyAccessor.GetValue(obj));
                        columnDbTypes.Add(GetDbType(column.PropertyType));
                    }
                }
            }
            else
            {
                //更新指定的列
                foreach (string columnName in updateColumnList)
                {
                    ColumnMetaData column = FindColumn(columnName, table.NormalColumns);
                    if (column == null)
                        throw new SqlException("列名未找到或无效:" + columnName);

                    if (column.ColumnCategory != Category.ReadOnly)
                    {
                        columnNames.Add(column.ColumnName);
                        columnValues.Add(column.PropertyAccessor.GetValue(obj));
                        columnDbTypes.Add(GetDbType(column.PropertyType));
                    }
                }
            }

            string format = "Update {0} Set {1} Where {2};";            
            StringBuilder tempUpdateString = new StringBuilder();
            StringBuilder tempConditions = new StringBuilder();
            for (int i = 0; i < columnNames.Count; i++)
            {
                tempUpdateString.AppendFormat("{0}=@{0},", columnNames[i]);                
            }
            tempUpdateString.Remove(tempUpdateString.Length - 1, 1);

            //根据主键及版本列生成条件
            tempConditions.AppendFormat("{0}=@{0}", table.PKColumn.ColumnName);
            if (!ignoreVersionCheck && table.VersionColumn != null)
            {
                tempConditions.AppendFormat(" And {0}=@Old_{0}", table.VersionColumn.ColumnName);
            }
            
            DbCommand dbCommand = _DbInstance.GetSqlStringCommand(string.Format(format, table.TableName, tempUpdateString, tempConditions));
            for (int i = 0; i < columnNames.Count; i++)
            {
                _DbInstance.AddInParameter(dbCommand, "@" + columnNames[i], columnDbTypes[i], TransformToSqlParamValue(columnValues[i]));
            }
            _DbInstance.AddInParameter(dbCommand, "@" + table.PKColumn.ColumnName, GetDbType(table.PKColumn.PropertyType), table.PKColumn.PropertyAccessor.GetValue(obj));
            if (!ignoreVersionCheck && table.VersionColumn != null)
            {
                _DbInstance.AddInParameter(dbCommand, "@Old_" + table.VersionColumn.ColumnName, GetDbType(table.VersionColumn.PropertyType), oldVersion);
            }
            
            //执行sql命令
            int count;
            try
            {
                count = _DbInstance.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw new SqlException(ex.Message, LogHelper.GetSqlString(dbCommand), ex);
            }
            
            if (count == 0)
            {
                throw new StaleObjectStateException("在更新到数据库时，受影响的记录数为0，请检查更新的条件是否正确，或者对象的状态已过期。");
            }
            if (count > 1)
            {
                throw new SqlException("更新的记录超过一条");
            }

            if (table.VersionColumn != null)
            {
                table.VersionColumn.PropertyAccessor.SetValue(obj, newVersion);
            }
        }
        #endregion

        #region Delete
        public void Delete(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            TableMetaData table = MetaDataCacheManager.GetTableMetaData(obj.GetType());
            if (table.PKColumn == null)
            {
                throw new MappingException(string.Format("实体类型{0}未定义主键列", table.EntityType.Name));
            }
            object id = table.PKColumn.PropertyAccessor.GetValue(obj);

            if (table.PKColumn.PropertyType == typeof(Int32) || table.PKColumn.PropertyType == typeof(Int64))
            {
                long idValue = Convert.ToInt64(id);
                if (idValue == 0)
                    throw new SqlException("id为空");
            }
            else if (table.PKColumn.PropertyType == typeof(string))
            {
                string idValue = (string)id;
                if (string.IsNullOrEmpty(idValue))
                    throw new SqlException("id为空");
            }
            
            DeleteByCondition(obj.GetType(), Filter.CreateEq(table.PKColumn.ColumnName, id));
        }

        public void Delete<T>(object id)
        {
            TableMetaData table = MetaDataCacheManager.GetTableMetaData(typeof(T));
            if (table.PKColumn == null)
            {
                throw new MappingException(string.Format("实体类型{0}未定义主键列", table.EntityType.Name));
            }
            if (table.PKColumn.PropertyType == typeof(Int32) || table.PKColumn.PropertyType == typeof(Int64))
            {
                long idValue = Convert.ToInt64(id);
                if (idValue == 0)
                    throw new SqlException("id为空");
            }
            else if (table.PKColumn.PropertyType == typeof(string))
            {
                string idValue = (string)id;
                if (string.IsNullOrEmpty(idValue))
                    throw new SqlException("id为空");
            }

            DeleteByCondition(typeof(T), Filter.CreateEq(table.PKColumn.ColumnName, id));
        }

        public void DeleteByCondition(Type entityType, Filter filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            string condition = filter.GetConditionString();
            if (string.IsNullOrEmpty(condition))
                throw new SqlException("删除时未指定条件");

            TableMetaData table = MetaDataCacheManager.GetTableMetaData(entityType);

            DbCommand dbCommand = _DbInstance.GetSqlStringCommand(string.Format("Delete From {0} Where {1};", table.TableName, condition));
            //执行sql命令
            try
            {
                _DbInstance.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw new SqlException(ex.Message, LogHelper.GetSqlString(dbCommand), ex);
            }            
        }
        #endregion

        #region Get
        public T Get<T>(object id)
        {            
            TableMetaData table = MetaDataCacheManager.GetTableMetaData(typeof(T));
            if(table.PKColumn == null)
            {
                throw new MappingException(string.Format("实体类型{0}未定义主键列", table.EntityType.Name));
            }
            //if (table.PKColumn.PropertyType == typeof(Int32) || table.PKColumn.PropertyType == typeof(Int64))
            //{
            //    //long idValue = Convert.ToInt64(id);
            //    //if (idValue == 0)
            //    //    throw new SqlException("id为空");
            //}
            //else if (table.PKColumn.PropertyType == typeof(string))
            //{
            //    string idValue = (string)id;
            //    if(string.IsNullOrEmpty(idValue))
            //        throw new SqlException("id为空");
            //}
            
            List<T> list = GetList<T>(Filter.CreateEq(table.PKColumn.ColumnName, id));

            if (list.Count == 0)
            {
                return default(T);
            }
            else if (list.Count == 1)
            {
                return list[0];
            }
            else
            {
                throw new SqlException(string.Format("id为{0}的记录多于一条", id));
            }
        }
        #endregion

        #region GetList
        public List<T> GetList<T>(Filter filter)
        {
            if (filter == null) filter = Filter.Create<T>();
            return GetList<T>(filter.GetSelectString(typeof(T)));
        }

        public List<T> GetListByMappedSql<T>(string sqlKey, params object[] paramValues)
        {
            if (!IsMappedSql(sqlKey))
                throw new ArgumentException("调用GetListByMappedSql时必须指定映射的sql", "sqlKey");

            return GetList<T>(GetMappedSqlString(_DatabaseName, sqlKey, paramValues));
        }

        public List<T> GetList<T>(string selectString)
        {
            IList[] results = GetMultiList(new string[] { selectString }, new Type[] { typeof(T) });
            return (List<T>)results[0];
        }

        public IList[] GetMultiList(string[] selectStrings, Type[] returnTypes)
        {
            if (selectStrings == null || returnTypes == null) throw new ArgumentException("selectStrings或returnTypes为空");
            if (selectStrings.Length != returnTypes.Length) throw new ArgumentException("selectStrings与returnTypes的长度不同");

            if (selectStrings.Length == 0) return new IList[0];

            #region 得到实际执行的字符串
            string executeSql;
            if (selectStrings.Length == 1)
            {
                executeSql = selectStrings[0];
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (string item in selectStrings)
                {
                    sb.Append(item);
                    if (!item.EndsWith(";"))
                    {
                        sb.Append(";");
                    }
                }
                executeSql = sb.ToString();
            }
            #endregion

            //读取结果集
            IList[] results = new IList[selectStrings.Length];

            DbCommand dbCommand = _DbInstance.GetSqlStringCommand(executeSql);
            dbCommand.CommandTimeout = int.MaxValue;
            //执行sql命令
            IDataReader dr;
            try
            {
                dr = _DbInstance.ExecuteReader(dbCommand);
            }
            catch (Exception ex)
            {
                throw new SqlException(ex.Message, LogHelper.GetSqlString(dbCommand), ex);
            }
            try
            {
                for (int i = 0; i < selectStrings.Length; i++)
                {
                    string errorColumn = string.Empty;
                    string errorDbValue = string.Empty;
                    Type listType = typeof(List<>).MakeGenericType(returnTypes[i]);
                    IList list = (IList)Activator.CreateInstance(listType);
                    TableMetaData table = MetaDataCacheManager.GetTableMetaData(returnTypes[i]);
                    try
                    {
                        while (dr.Read())
                        {
                            list.Add(CreateObjectByDataReader(table, dr, out errorColumn, out errorDbValue));
                        }
                        results[i] = list;
                    }
                    catch (Exception ex)
                    {
                        throw new SqlException(string.Format("CreateObjectByDataReader error, table:{0}, column:{1}, dbValue:{2}. message:{3}", table.TableName, errorColumn, errorDbValue, ex.Message), "", ex);
                    }

                    dr.NextResult();
                }
            }
            finally
            {
                dr.Close();
            }            
            return results;
        }
        #endregion

        #region GetPageList

        public PageList<T> GetPageList<T>(Filter filter, PageInfo pageInfo)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            if (string.IsNullOrEmpty(pageInfo.OrderKey))
                throw new SqlException("在执行分页查询时必须在分页参数中指定排序字段");

            if (!string.IsNullOrEmpty(filter.OrderKey))
                throw new SqlException("在执行分页查询时请在分页参数中指定排序字段，而不要在Filter中指定");
            
            string sql = filter.GetSelectString(typeof(T));

            return GetPageList<T>(sql, pageInfo.OrderKey, pageInfo.PageSize, pageInfo.CurPage);
        }

        public PageList<T> GetPageListByMappedSql<T>(string sqlKey, PageInfo pageInfo, params object[] paramValues)
        {
            if (!IsMappedSql(sqlKey))
                throw new ArgumentException("调用GetPageListByMappedSql时必须指定映射的sql", "sqlKey");

            return new DbMgr(_DatabaseName).ExecutePageListObject<T>(sqlKey, pageInfo.OrderKey, pageInfo.PageSize, pageInfo.CurPage, paramValues);
        }

        public PageList<T> GetPageList<T>(string sql, string orderKey, int pageSize, int curPage)
        {
            PageList<T> pageList = new PageList<T>();
            pageList.SetPage(pageSize, curPage);
            
            string pageSql = _Dialect.GetPageSql(sql, orderKey, pageList.StartRecord, pageList.EndRecord);
            string executeSql;
            if (pageSize > 0)
            {
                string countSql = _Dialect.GetCountSql(sql);
                executeSql = countSql + "; " + pageSql;
            }
            else
            {
                executeSql = pageSql;
            }

            DbCommand dbCommand = _DbInstance.GetSqlStringCommand(executeSql);

            //执行sql命令
            IDataReader dr;
            try
            {
                dr = this._DbInstance.ExecuteReader(dbCommand);
            }
            catch (Exception ex)
            {
                throw new SqlException(ex.Message, LogHelper.GetSqlString(dbCommand), ex);
            }
            try
            {
                if (pageSize > 0)
                {
                    //读取记录条数
                    dr.Read();
                    pageList.TotalCount = (int)dr[0];
                    dr.NextResult();
                }

                string errorColumn = string.Empty;
                string errorDbValue = string.Empty;
                TableMetaData table = MetaDataCacheManager.GetTableMetaData(typeof(T));
                try
                {
                    while (dr.Read())
                    {
                        pageList.Add((T)CreateObjectByDataReader(table, dr, out errorColumn, out errorDbValue));
                    }
                }
                catch (Exception ex)
                {
                    throw new SqlException(string.Format("CreateObjectByDataReader error, table:{0}, column:{1}, dbValue:{2}. message:{3}", table.TableName, errorColumn, errorDbValue, ex.Message));
                }

                // 如果未分页，则设置总数为当前页记录数
                if (pageSize <= 0)
                {
                    pageList.TotalCount = pageList.CurPageCount;
                }
            }
            finally
            {
                dr.Close();
            }
            return pageList;
        }

        #endregion

        #region ExecuteNonQuery
        public void ExecuteNonQueryByMappedSql(string sqlKey, params object[] paramValues)
        {
            if (!IsMappedSql(sqlKey))
                throw new ArgumentException("调用ExecuteNonQueryByMappedSql时必须指定映射的sql", "sqlKey");

            new DbMgr(_DatabaseName).ExecuteNonQuery(sqlKey, paramValues);
        }

        public void ExecuteNonQuery(string sqlString)
        {
            //执行sql命令
            try
            {
                new DbMgr(_DatabaseName).ExecuteNonQuery(sqlString);
            }
            catch (Exception ex)
            {
                throw new SqlException(ex.Message, sqlString, ex);
            }            
        }
        #endregion

        public bool Exist<T>(Filter<T> filter)
        {
            string condition = (filter == null) ? string.Empty : filter.GetConditionString();
            
            TableMetaData table = MetaDataCacheManager.GetTableMetaData(typeof(T));            
            StringBuilder sb = new StringBuilder("Select Count(1) From " + table.TableName);
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" Where ").Append(condition);
            }

            object obj;
            try
            {
                obj = ExecuteScalar(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new SqlException(ex.Message, sb.ToString(), ex);
            }
            return Convert.ToInt32(obj) > 0;
        }

        #region ExecuteScalar
        public object ExecuteScalarByMappedSql(string sqlKey, params object[] paramValues)
        {
            if (!IsMappedSql(sqlKey))
                throw new ArgumentException("调用ExecuteScalarByMappedSql时必须指定映射的sql", "sqlKey");

            return new DbMgr(_DatabaseName).ExecuteScalar(sqlKey, paramValues);
        }

        public object ExecuteScalar(string sqlString)
        {
            //执行sql命令
            try
            {
                return new DbMgr(_DatabaseName).ExecuteScalar(sqlString);
            }
            catch (Exception ex)
            {
                throw new SqlException(ex.Message, sqlString, ex);
            }
        }
        #endregion

        #region private methods

        //根据dr读取的值创建对象，当出错时，out参数返回出错时对应的列，以便在外层抛出更明确的异常信息
        private static object CreateObjectByDataReader(TableMetaData table, IDataReader dr, out string errorColumn, out string errorDbValue)
        {
            errorColumn = string.Empty;
            errorDbValue = string.Empty;
            object entityObj = table.EntityType.Assembly.CreateInstance(table.EntityType.FullName);
            
            for (int i = 0; i < dr.FieldCount; i++)
            {
                string columnName = dr.GetName(i).ToUpper();
                errorColumn = columnName;

                PropertyMetaData propertyData;
                if (table.PropertiesDic.TryGetValue(columnName, out propertyData))
                {
                    object dbValue = dr.GetValue(i);
                    errorDbValue = dbValue.ToString();
                    if (dbValue != DBNull.Value)
                    {
                        object fieldValue = TransformToObjValueFromDb(dbValue, propertyData.PropertyType);
                        if(fieldValue != null)
                        {
                            propertyData.PropertyAccessor.SetValue(entityObj, fieldValue);
                        }
                    }
                }
                else
                {
                    //如果在对象中未找到对应的属性，则忽略该值.
                }
            }
            return entityObj;
        }

        private static DbType GetDbType(Type type)
        {
            return type == typeof(string) ? DbType.AnsiString :
                    type == typeof(DateTime) ? DbType.DateTime :
                    type == typeof(Byte) ? DbType.Byte :
                    type == typeof(Int16) ? DbType.Int16 :
                    type == typeof(Int32) ? DbType.Int32 :
                    type == typeof(Int64) ? DbType.Int64 :
                    type == typeof(float) ? DbType.Single :
                    type == typeof(double) ? DbType.Double :
                    type == typeof(decimal) ? DbType.Decimal :
                    type == typeof(Guid) ? DbType.Guid :
                    type == typeof(byte[]) ? DbType.Binary :
                    type.IsEnum ? GetEnumDbType(type) :
                    DbType.AnsiString;
        }
        
        private static object TransformToSqlParamValue(object objValue)
        {
            if (objValue == null) return DBNull.Value;

            Type type = objValue.GetType();
            if (type.IsEnum)
            {
                DbType dbType = GetEnumDbType(type);
                if (dbType == DbType.Int32)
                    return (int)objValue;
                else
                    return objValue.ToString();
            }
            else if (type == typeof(DateTime))
            {
                DateTime dt = (DateTime)objValue;
                if (dt == DateTime.MinValue)
                    return DBNull.Value;
                else
                    return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
            }
            return objValue;
        }

        private static object TransformToObjValueFromDb(object dbValue, Type objType)
        {
            if (dbValue == DBNull.Value)
            {
                return null;
            }
            else if(objType == typeof(Int32))
            {
                return Convert.ToInt32(dbValue);
            }
            else if (objType == typeof(Int64))
            {
                return Convert.ToInt64(dbValue);
            }
            else if (objType == typeof(decimal))
            {
                return Convert.ToDecimal(dbValue);
            }
            else if (objType == typeof(double))
            {
                return Convert.ToDouble(dbValue);
            }
            else if (objType.IsEnum)
            {
                string enumStr = dbValue.ToString();
                if (string.IsNullOrWhiteSpace(enumStr))
                    return null;

                return Enum.Parse(objType, enumStr, true);
            }
            else
            {
                return dbValue;
            }
        }

        private static ColumnMetaData FindColumn(string columnName, List<ColumnMetaData> list)
        {
            foreach (ColumnMetaData item in list)
            {
                if (string.Equals(item.ColumnName, columnName))
                {
                    return item;
                }
            }
            return null;
        }

        internal static DbType GetEnumDbType(Type enumType)
        {
            if (EnumMetaDataCacheManager.GetEnumStorageType(enumType) == EnumStorageType.EnumValue)
                return DbType.Int32;
            else
                return DbType.AnsiString;
        }

        private static bool IsMappedSql(string sqlName)
        {
            return !string.IsNullOrEmpty(sqlName) && sqlName.StartsWith("$");
        }

        internal static string GetMappedSqlString(string databaseName, string sqlKey, params object[] paramValues)
        {
            return SqlMap.GetSql(SqlManager.GetISql(databaseName), sqlKey, paramValues);
        }
        #endregion
    }
}
