using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZhiBen.Framework.DataAccess;
using System.Linq.Expressions;
using System.Collections;
using System.Data;
using SSharing.Ubtrip.Common.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace SSharing.Ubtrip.Common.Sql
{
    /// <summary>
    /// 提供对特定数据库访问的快捷方法
    /// </summary>
    public class DbServiceInstance
    {
        private string _DatabaseName;
        
        internal DbServiceInstance(string databaseName)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                _DatabaseName = DbManager.DefaultDatabaseName;
            }
            else
            {
                _DatabaseName = databaseName;
            }
        }

        public Database GetDatabase()
        {
            return new DbExecuteEngine(_DatabaseName).GetDatabase();
        }

        public string GetDatabaseName()
        {
            Database db = GetDatabase();
            using (System.Data.Common.DbConnection con = db.CreateConnection())
            {
                return con.Database;
            }
        }

        #region Insert
        /// <summary>
        /// 插入对象到数据库
        /// </summary>
        public void Insert(object obj)
        {
            Insert(obj, false);
        }

        /// <summary>
        /// 插入对象到数据库,并在生成的Sql语句中包含标识列的值
        /// </summary>
        public void InsertWithAssignedIdetityValue(object obj)
        {
            Insert(obj, true);
        }

        private void Insert(object obj, bool includeIdetityColumnInSql)
        {
            if (!DbManager.IsInTransaction(_DatabaseName))
            {
                using (TransactionScope ts = new TransactionScope(_DatabaseName))
                {
                    new DbExecuteEngine(_DatabaseName).Insert(obj, includeIdetityColumnInSql);
                    ts.Complete();
                }
            }
            else
            {
                new DbExecuteEngine(_DatabaseName).Insert(obj, includeIdetityColumnInSql);
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新对象的状态到数据库, 并检查数据的版本
        /// </summary>
        public void Update(object obj)
        {
            Update(obj, false, new List<string>());
        }

        /// <summary>
        /// 更新对象的状态到数据库, 忽略数据的版本检查(正常情况下不建议使用)
        /// </summary>
        public void UpdateIgnoreVersion(object obj)
        {
            Update(obj, true, new List<string>());
        }

        /// <summary>
        /// 更新对象指定列的数据到数据库,不检测版本
        /// </summary>
        public void UpdateSpecifyColumns<T>(T obj, params Expression<GetProperty<T>>[] properties)
        {
            if (properties.Length == 0)
                throw new ArgumentNullException("properties");

            ColumnList<T> columnList = new ColumnList<T>(properties);
            UpdateSpecifyColumns(obj, columnList, true);
        }

        /// <summary>
        /// 更新对象指定列的数据到数据库,检测版本列
        /// </summary>
        public void UpdateSpecifyColumnsWithVersionCheck<T>(T obj, params Expression<GetProperty<T>>[] properties)
        {
            if (properties.Length == 0)
                throw new ArgumentNullException("properties");

            ColumnList<T> columnList = new ColumnList<T>(properties);
            UpdateSpecifyColumns(obj, columnList, false);
        }

        /// <summary>
        /// 更新对象指定列的数据到数据库,不检测版本
        /// </summary>
        public void UpdateSpecifyColumns(object obj, ColumnList columnList)
        {
            UpdateSpecifyColumns(obj, columnList, true);
        }

        /// <summary>
        /// 更新对象指定列的数据到数据库
        /// </summary>
        public void UpdateSpecifyColumns(object obj, ColumnList columnList, bool ignoreVersion)
        {
            Update(obj, ignoreVersion, columnList.GetColumnNames());
        }
        
        private void Update(object obj, bool ignoreVersionCheck, List<string> updateColumnList)
        {
            if (!DbManager.IsInTransaction(_DatabaseName))
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    new DbExecuteEngine(_DatabaseName).Update(obj, ignoreVersionCheck, updateColumnList);
                    ts.Complete();
                }
            }
            else
            {
                new DbExecuteEngine(_DatabaseName).Update(obj, ignoreVersionCheck, updateColumnList);
            }
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除对象(根据主键生成条件)
        /// </summary>
        public void Delete(object obj)
        {
            if (!DbManager.IsInTransaction(_DatabaseName))
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    new DbExecuteEngine(_DatabaseName).Delete(obj);
                    ts.Complete();
                }
            }
            else
            {
                new DbExecuteEngine(_DatabaseName).Delete(obj);
            }
        }

        /// <summary>
        /// 根据主键删除记录
        /// </summary>
        public void Delete<T>(object id)
        {
            if (!DbManager.IsInTransaction(_DatabaseName))
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    new DbExecuteEngine(_DatabaseName).Delete<T>(id);
                    ts.Complete();
                }
            }
            else
            {
                new DbExecuteEngine(_DatabaseName).Delete<T>(id);
            }
        }

        /// <summary>
        /// 根据指定的条件删除记录
        /// </summary>
        public void DeleteByCondition<T>(Expression<GetProperty<T>> property, Operator optr, object value)
        {
            Filter filter = Filter.Create();
            filter.FilterItems.Add(new FilterItem(DbService.GetColName<T>(property), optr, value));
            this.DeleteByCondition<T>(filter);
        }

        /// <summary>
        /// 根据linq表达式删除记录
        /// </summary>
        public void DeleteByCondition<T>(Expression<Func<T, bool>> exp)
        {
            Filter<T> filter = Filter.Create<T>(exp);            
            this.DeleteByCondition<T>(filter);
        }

        /// <summary>
        /// 根据指定的条件删除记录
        /// </summary>
        public void DeleteByCondition<T>(Filter filter)
        {
            if (!DbManager.IsInTransaction(_DatabaseName))
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    new DbExecuteEngine(_DatabaseName).DeleteByCondition(typeof(T), filter);
                    ts.Complete();
                }
            }
            else
            {
                new DbExecuteEngine(_DatabaseName).DeleteByCondition(typeof(T), filter);
            }
        }
        #endregion

        #region Get
        /// <summary>
        /// 通过主键获取对象
        /// </summary>
        public T Get<T>(object id)
        {
            return new DbExecuteEngine(_DatabaseName).Get<T>(id);
        }

        /// <summary>
        /// 根据指定的条件获取对象,如果获取的对象多于一个，将抛出异常
        /// </summary>
        public T GetUnique<T>(Filter filter)
        {
            List<T> list = GetList<T>(filter);
            if (list.Count == 0)
                return default(T);
            else if (list.Count == 1)
                return list[0];
            else
                throw new SqlException("查询结果多于一个, 查询条件:" + filter.GetConditionString());
        }

        /// <summary>
        /// 根据指定的条件获取对象,如果获取的对象多于一个，将抛出异常
        /// </summary>
        public T GetUnique<T>(Expression<GetProperty<T>> property, Operator optr, object value)
        {
            string columnName = DbService.GetColName<T>(property);
            Filter filter = Filter.Create();
            filter.FilterItems.Add(new FilterItem(columnName, optr, value));
            return GetUnique<T>(filter);
        }
        
        /// <summary>
        /// 根据指定的linq表达式获取对象,如果获取的对象多于一个，将抛出异常
        /// </summary>
        public T GetUnique<T>(Expression<Func<T, bool>> exp)
        {
            Filter<T> filter = Filter.Create<T>(exp);
            return GetUnique<T>(filter);
        }
        #endregion

        #region Exist

        /// <summary>
        /// 是否存在符合条件的记录
        /// </summary>
        public bool Exist<T>(Expression<GetProperty<T>> property, Operator optr, object value)
        {
            Filter<T> filter = Filter.Create<T>();
            filter.FilterItems.Add(new FilterItem(DbService.GetColName<T>(property), optr, value));

            return Exist(filter);
        }

        /// <summary>
        /// 根据指定的linq表达式判断是否存在符合条件的记录
        /// </summary>
        public bool Exist<T>(Expression<Func<T, bool>> exp)
        {
            Filter<T> filter = Filter.Create<T>(exp);
            return Exist(filter);
        }

        /// <summary>
        /// 是否存在符合条件的记录
        /// </summary>
        public bool Exist<T>(Filter<T> filter)
        {
            return new DbExecuteEngine(_DatabaseName).Exist(filter);
        }

        #endregion

        #region GetList
        /// <summary>
        /// 获取所有对象
        /// </summary>
        public List<T> GetAll<T>()
        {
            return new DbExecuteEngine(_DatabaseName).GetList<T>(Filter.Create());
        }

        /// <summary>
        /// 根据指定的条件获取对象集合
        /// </summary>
        public List<T> GetList<T>(Filter filter)
        {
            return new DbExecuteEngine(_DatabaseName).GetList<T>(filter);
        }

        /// <summary>
        /// 根据映射的sql及参数获取对象集合
        /// </summary>
        public List<T> GetListByMappedSql<T>(string sqlKey, params object[] paramValues)
        {
            return new DbExecuteEngine(_DatabaseName).GetListByMappedSql<T>(sqlKey, paramValues);
        }

        /// <summary>
        /// 根据指定的select语句获取对象集合
        /// </summary>
        public List<T> GetList<T>(string selectString)
        {
            return new DbExecuteEngine(_DatabaseName).GetList<T>(selectString);
        }

        /// <summary>
        /// 根据指定的条件获取对象集合
        /// </summary>
        public List<T> GetList<T>(string columnName, Operator optr, object value)
        {
            Filter filter = Filter.Create();
            filter.FilterItems.Add(new FilterItem(columnName, optr, value));
            return GetList<T>(filter);
        }

        /// <summary>
        /// 根据指定的条件获取对象集合
        /// </summary>
        public List<T> GetList<T>(Expression<GetProperty<T>> property, Operator optr, object value)
        {
            string columnName = DbService.GetColName<T>(property);
            return GetList<T>(columnName, optr, value);
        }

        /// <summary>
        /// 根据linq表达式获取对象集合
        /// </summary>
        public List<T> GetList<T>(Expression<Func<T, bool>> exp)
        {
            Filter<T> filter = Filter.Create<T>(exp);            
            return GetList<T>(filter);
        }

        /// <summary>
        /// 在一次数据库查询中获取多个结果集
        /// </summary>
        public IList[] GetMultiList(string[] selectStrings, Type[] returnTypes)
        {
            return new DbExecuteEngine(_DatabaseName).GetMultiList(selectStrings, returnTypes);
        }

        /// <summary>
        /// 在一次数据库查询中获取两个结果集
        /// </summary>
        public void GetMultiList<T1, T2>(Filter<T1> filter1, Filter<T2> filter2, out List<T1> list1, out List<T2> list2)
        {
            IList[] lists = GetMultiList(
                new string[] { 
                    filter1.GetSelectString(), 
                    filter2.GetSelectString() 
                },
                new Type[] { 
                    typeof(T1), 
                    typeof(T2) 
                });

            list1 = (List<T1>)lists[0];
            list2 = (List<T2>)lists[1];
        }

        /// <summary>
        /// 在一次数据库查询中获取三个结果集
        /// </summary>
        public void GetMultiList<T1, T2, T3>(Filter<T1> filter1, Filter<T2> filter2, Filter<T3> filter3, out List<T1> list1, out List<T2> list2, out List<T3> list3)
        {
            IList[] lists = GetMultiList(
                new string[] { 
                    filter1.GetSelectString(), 
                    filter2.GetSelectString(),
                    filter3.GetSelectString() 
                },
                new Type[] { 
                    typeof(T1), 
                    typeof(T2),
                    typeof(T3) 
                });

            list1 = (List<T1>)lists[0];
            list2 = (List<T2>)lists[1];
            list3 = (List<T3>)lists[2];
        }

        #endregion

        #region GetPageList

        /// <summary>
        /// 根据条件获取分页的对象集合(自动生成Sql语句而不使用配置, 请设置pageInfo.OrderKey,而不要设置Filter.OrderBy)
        /// </summary>
        public PageList<T> GetPageList<T>(Filter filter, PageInfo pageInfo)
        {
            return new DbExecuteEngine(_DatabaseName).GetPageList<T>(filter, pageInfo);
        }

        public PageList<T> GetPageList<T>(string sql, string orderKey, int pageSize, int curPage)
        {
            return new DbExecuteEngine(_DatabaseName).GetPageList<T>(sql, orderKey, pageSize, curPage);
        }

        /// <summary>
        /// 获取分页的对象集合(sqlKey必须是映射的sql语句，且必须带前缀'$')
        /// </summary>
        public PageList<T> GetPageListByMappedSql<T>(string sqlKey, PageInfo pageInfo, params object[] paramValues)
        {
            return new DbExecuteEngine(_DatabaseName).GetPageListByMappedSql<T>(sqlKey, pageInfo, paramValues);
        }
        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// 执行非查询SQL语句(sqlKey必须是映射的sql语句，且必须带前缀'$')
        /// </summary>
        public void ExecuteNonQueryByMappedSql(string sqlKey, params object[] paramValues)
        {
            if (!DbManager.IsInTransaction(_DatabaseName))
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    new DbExecuteEngine(_DatabaseName).ExecuteNonQueryByMappedSql(sqlKey, paramValues);
                    ts.Complete();
                }
            }
            else
            {
                new DbExecuteEngine(_DatabaseName).ExecuteNonQueryByMappedSql(sqlKey, paramValues);
            }
        }

        /// <summary>
        /// 执行非查询SQL语句
        /// </summary>
        public void ExecuteNonQuery(string sqlString)
        {
            if (!DbManager.IsInTransaction(_DatabaseName))
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    new DbExecuteEngine(_DatabaseName).ExecuteNonQuery(sqlString);
                    ts.Complete();
                }
            }
            else
            {
                new DbExecuteEngine(_DatabaseName).ExecuteNonQuery(sqlString);             
            }
        }
        #endregion

        #region ExecuteScalar

        /// <summary>
        /// 返回结果集中第一行的第一个值(sqlKey必须是映射的sql语句，且必须带前缀'$')
        /// </summary>
        public object ExecuteScalarByMappedSql(string sqlKey, params object[] paramValues)
        {
            return new DbExecuteEngine(_DatabaseName).ExecuteScalarByMappedSql(sqlKey, paramValues);
        }

        /// <summary>
        /// 返回结果集中第一行的第一个值
        /// </summary>
        public object ExecuteScalar(string sqlString)
        {
            return new DbExecuteEngine(_DatabaseName).ExecuteScalar(sqlString);
        }
        #endregion

        /// <summary>
        /// 获得映射的sql语句,sqlKey必须带'$'前缀
        /// </summary>
        public string GetMappedSqlString(string sqlKey, params object[] paramValues)
        {
            return DbExecuteEngine.GetMappedSqlString(_DatabaseName, sqlKey, paramValues);
        }

        /// <summary>
        /// 获取当前事务中新增记录的Id值,必须与insert语句执行在同一事务中
        /// </summary>        
        public int GetNewIndentity()
        {
            return (int)this.ExecuteScalar("SELECT CAST(@@IDENTITY as int) as value");
        }
        
    }
}
