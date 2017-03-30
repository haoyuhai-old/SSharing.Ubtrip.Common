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
    /// 提供对默认数据库访问的快捷方法
    /// </summary>
    public static class DbService
    {
        #region 辅助方法
        /// <summary>
        /// 获取指定的数据库,参数可以为空
        /// </summary>
        public static DbServiceInstance GetDb(string databaseName)
        {
            return new DbServiceInstance(databaseName);
        }

        public static Database GetDatabase()
        {
            return new DbServiceInstance(null).GetDatabase();
        }

        public static string GetDatabaseName()
        {
            return new DbServiceInstance(null).GetDatabaseName();
        }

        /// <summary>
        /// 获取lamda表达式中指定属性定义的列名(比如: string colName = DbService.GetColName(c => c.Name))
        /// </summary>
        public static string GetColName<T>(Expression<GetProperty<T>> property)
        {
            return MyReflection.GetColName(property);
        }

        /// <summary>
        /// 获取类型对应的表名
        /// </summary>
        public static string GetTableName(Type type)
        {
            TableMetaData table = MetaDataCacheManager.GetTableMetaData(type);
            return table.TableName;
        }

        /// <summary>
        /// 获得映射的sql语句,sqlKey必须带'$'前缀
        /// </summary>
        public static string GetMappedSqlString(string sqlKey, params object[] paramValues)
        {
            return new DbServiceInstance(null).GetMappedSqlString(sqlKey, paramValues);
        }

        /// <summary>
        /// 获取当前事务中新增记录的Id值,必须与insert语句执行在同一事务中
        /// </summary>        
        public static int GetNewIndentity()
        {
            return new DbServiceInstance(null).GetNewIndentity();
        }
        
        #endregion

        #region Insert
        /// <summary>
        /// 插入对象到数据库
        /// </summary>
        public static void Insert(object obj)
        {
            new DbServiceInstance(null).Insert(obj);
        }
        /// <summary>
        /// 插入对象到数据库,并在生成的Sql语句中包含标识列的值。
        /// 调用此方法前需调用'SET IDENTITY_INSERT table_name ON'以禁用自增功能
        /// 调用此方法后再调用'SET IDENTITY_INSERT table_name OFF'以启用自增功能
        /// </summary>
        public static void InsertWithAssignedIdetityValue(object obj)
        {
            new DbServiceInstance(null).InsertWithAssignedIdetityValue(obj);
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新对象的状态到数据库, 并检查数据的版本
        /// </summary>
        public static void Update(object obj)
        {
            new DbServiceInstance(null).Update(obj);
        }

        /// <summary>
        /// 更新对象的状态到数据库, 忽略数据的版本检查(正常情况下不建议使用)
        /// </summary>
        public static void UpdateIgnoreVersion(object obj)
        {
            new DbServiceInstance(null).UpdateIgnoreVersion(obj);
        }

        /// <summary>
        /// 更新对象指定列的数据到数据库,不检测版本
        /// </summary>
        public static void UpdateSpecifyColumns<T>(T obj, params Expression<GetProperty<T>>[] properties)
        {
            new DbServiceInstance(null).UpdateSpecifyColumns(obj, properties);
        }

        /// <summary>
        /// 更新对象指定列的数据到数据库,检测版本列
        /// </summary>
        public static void UpdateSpecifyColumnsWithVersionCheck<T>(T obj, params Expression<GetProperty<T>>[] properties)
        {
            new DbServiceInstance(null).UpdateSpecifyColumnsWithVersionCheck(obj, properties);
        }

        /// <summary>
        /// 更新对象指定列的数据到数据库,不检测版本
        /// </summary>
        public static void UpdateSpecifyColumns(object obj, ColumnList columnList)
        {
            new DbServiceInstance(null).UpdateSpecifyColumns(obj, columnList);
        }

        /// <summary>
        /// 更新对象指定列的数据到数据库
        /// </summary>
        public static void UpdateSpecifyColumns(object obj, ColumnList columnList, bool ignoreVersion)
        {
            new DbServiceInstance(null).UpdateSpecifyColumns(obj, columnList, ignoreVersion);
        }                
        #endregion

        #region Delete
        /// <summary>
        /// 删除对象(根据主键生成条件)
        /// </summary>
        public static void Delete(object obj)
        {
            new DbServiceInstance(null).Delete(obj);
        }

        /// <summary>
        /// 根据主键删除记录
        /// </summary>
        public static void Delete<T>(object id)
        {
            new DbServiceInstance(null).Delete<T>(id);
        }

        /// <summary>
        /// 根据指定的条件删除记录
        /// </summary>
        public static void DeleteByCondition<T>(Filter filter)
        {
            new DbServiceInstance(null).DeleteByCondition<T>(filter);
        }

        /// <summary>
        /// 根据指定的条件删除记录
        /// </summary>
        public static void DeleteByCondition<T>(Expression<GetProperty<T>> property, Operator optr, object value)
        {
            Filter filter = Filter.Create();
            filter.FilterItems.Add(new FilterItem(DbService.GetColName<T>(property), optr, value));
            new DbServiceInstance(null).DeleteByCondition<T>(filter);
        }

        /// <summary>
        /// 根据linq表达式删除记录
        /// </summary>
        public static void DeleteByCondition<T>(Expression<Func<T, bool>> exp)
        {
            new DbServiceInstance(null).DeleteByCondition<T>(exp);
        }
        #endregion

        #region Get
        /// <summary>
        /// 通过主键获取对象
        /// </summary>
        public static T Get<T>(object id)
        {
            return new DbServiceInstance(null).Get<T>(id);
        }        

        /// <summary>
        /// 根据指定的条件获取对象,如果获取的对象多于一个，将抛出异常
        /// </summary>
        public static T GetUnique<T>(Filter filter)
        {
            return new DbServiceInstance(null).GetUnique<T>(filter);
        }

        /// <summary>
        /// 根据指定的条件获取对象,如果获取的对象多于一个，将抛出异常
        /// </summary>
        public static T GetUnique<T>(Expression<GetProperty<T>> property, Operator optr, object value)
        {
            return new DbServiceInstance(null).GetUnique<T>(property, optr, value);
        }

        /// <summary>
        /// 根据指定的linq表达式获取对象,如果获取的对象多于一个，将抛出异常
        /// </summary>
        public static T GetUnique<T>(Expression<Func<T, bool>> exp)
        {
            return new DbServiceInstance(null).GetUnique<T>(exp);
        }
        #endregion

        #region Exist

        /// <summary>
        /// 是否存在符合条件的记录
        /// </summary>
        public static bool Exist<T>(Expression<GetProperty<T>> property, Operator optr, object value)
        {
            return new DbServiceInstance(null).Exist<T>(property, optr, value);
        }
         
        /// <summary>
        /// 根据指定的linq表达式判断是否存在符合条件的记录
        /// </summary>
        public static bool Exist<T>(Expression<Func<T, bool>> exp)
        {
            return new DbServiceInstance(null).Exist<T>(exp);
        }

        /// <summary>
        /// 是否存在符合条件的记录
        /// </summary>
        public static bool Exist<T>(Filter<T> filter)
        {
            return new DbServiceInstance(null).Exist(filter);
        }

        #endregion

        #region GetList

        /// <summary>
        /// 获取所有对象
        /// </summary>
        public static List<T> GetAll<T>()
        {
            return new DbServiceInstance(null).GetAll<T>();
        }

        /// <summary>
        /// 根据指定的条件获取对象集合
        /// </summary>
        public static List<T> GetList<T>(Filter filter)
        {
            return new DbServiceInstance(null).GetList<T>(filter);
        }

        /// <summary>
        /// 根据映射的sql及参数获取对象集合
        /// </summary>
        public static List<T> GetListByMappedSql<T>(string sqlKey, params object[] paramValues)
        {
            return new DbServiceInstance(null).GetListByMappedSql<T>(sqlKey, paramValues);
        }

        /// <summary>
        /// 根据指定的select语句获取对象集合
        /// </summary>
        public static List<T> GetList<T>(string selectString)
        {
            return new DbServiceInstance(null).GetList<T>(selectString);
        }

        /// <summary>
        /// 根据指定的条件获取对象集合
        /// </summary>
        public static List<T> GetList<T>(string columnName, Operator optr, object value)
        {
            return new DbServiceInstance(null).GetList<T>(columnName, optr, value);
        }

        /// <summary>
        /// 根据指定的条件获取对象集合
        /// </summary>
        public static List<T> GetList<T>(Expression<GetProperty<T>> property, Operator optr, object value)
        {
            return new DbServiceInstance(null).GetList<T>(property, optr, value);
        }

        /// <summary>
        /// 根据linq表达式获取对象集合
        /// </summary>
        public static List<T> GetList<T>(Expression<Func<T, bool>> exp)
        {
            return new DbServiceInstance(null).GetList<T>(exp);
        }

        /// <summary>
        /// 在一次数据库查询中获取多个结果集
        /// </summary>
        public static IList[] GetMultiList(string[] selectStrings, Type[] returnTypes)
        {
            return new DbServiceInstance(null).GetMultiList(selectStrings, returnTypes);
        }

        /// <summary>
        /// 在一次数据库查询中获取两个结果集
        /// </summary>
        public static void GetMultiList<T1, T2>(Filter<T1> filter1, Filter<T2> filter2, out List<T1> list1, out List<T2> list2)
        {
            new DbServiceInstance(null).GetMultiList(filter1, filter2, out list1, out list2);
        }

        /// <summary>
        /// 在一次数据库查询中获取三个结果集
        /// </summary>
        public static void GetMultiList<T1, T2, T3>(Filter<T1> filter1, Filter<T2> filter2, Filter<T3> filter3, out List<T1> list1, out List<T2> list2, out List<T3> list3)
        {
            new DbServiceInstance(null).GetMultiList(filter1, filter2, filter3, out list1, out list2, out list3);
        }
        #endregion

        #region GetPageList

        /// <summary>
        /// 根据条件获取分页的对象集合(自动生成Sql语句而不使用配置, 请设置pageInfo.OrderKey,而不要设置Filter.OrderBy)
        /// </summary>
        public static PageList<T> GetPageList<T>(Filter filter, PageInfo pageInfo)
        {
            return new DbServiceInstance(null).GetPageList<T>(filter, pageInfo);
        }

        public static PageList<T> GetPageList<T>(string sql, string orderKey, int pageSize, int curPage)
        {
            return new DbServiceInstance(null).GetPageList<T>(sql, orderKey, pageSize, curPage);
        }

        /// <summary>
        /// 获取分页的对象集合(sqlKey必须是映射的sql语句，且必须带前缀'$')
        /// </summary>
        public static PageList<T> GetPageListByMappedSql<T>(string sqlKey, PageInfo pageInfo, params object[] paramValues)
        {
            return new DbServiceInstance(null).GetPageListByMappedSql<T>(sqlKey, pageInfo, paramValues);
        }
        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// 执行非查询SQL语句(sqlKey必须是映射的sql语句，且必须带前缀'$')
        /// </summary>
        public static void ExecuteNonQueryByMappedSql(string sqlKey, params object[] paramValues)
        {
            new DbServiceInstance(null).ExecuteNonQueryByMappedSql(sqlKey, paramValues);
        }

        /// <summary>
        /// 执行非查询SQL语句
        /// </summary>
        public static void ExecuteNonQuery(string sqlString)
        {
            new DbServiceInstance(null).ExecuteNonQuery(sqlString);
        }
        #endregion

        #region ExecuteScalar

        /// <summary>
        /// 返回结果集中第一行的第一个值(sqlKey必须是映射的sql语句，且必须带前缀'$')
        /// </summary>
        public static object ExecuteScalarByMappedSql(string sqlKey, params object[] paramValues)
        {
            return new DbServiceInstance(null).ExecuteScalarByMappedSql(sqlKey, paramValues);
        }

        /// <summary>
        /// 返回结果集中第一行的第一个值
        /// </summary>
        public static object ExecuteScalar(string sqlString)
        {
            return new DbServiceInstance(null).ExecuteScalar(sqlString);
        }
        #endregion
    }
}
