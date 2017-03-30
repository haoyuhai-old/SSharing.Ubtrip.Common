using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


using ZhiBen.Framework.DataAccess;
using SSharing.Ubtrip.Common.Util;
using SSharing.Ubtrip.Common.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace SSharing.Ubtrip.Common.Sql
{
    /// <summary>
    /// 数据访问通用管理类
    /// </summary>
    public class DbMgr
    {
        DbInstance _db = null;
        ISql _isql = null;

        public DbMgr()
        {
            _db = DbManager.GetDbInstance();
            _isql = SqlManager.GetISql();
        }

        public DbMgr(string dbName)
        {
            _db = DbManager.GetDbInstance(dbName);
            _isql = SqlManager.GetISql(dbName);
        }

        public Database GetDatabase()
        {
            return _db.GetDatabase();
        }

        /// <summary>
        /// 执行单值SQL
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sqlName, params object[] paramValues)
        {
            if (IsMappedSql(sqlName) == false)
            {
                return _db.ExecuteScalarBySql(sqlName);
            }
            else
            {
                return _db.ExecuteScalarBySql(SqlMap.GetSql(_isql, sqlName, paramValues));
            }
        }
        public object ExecuteScalar(string sqlName)
        {
            return ExecuteScalar(sqlName, null);
        }



        /// <summary>
        /// 执行非查询SQL
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="paramValues"></param>
        public void ExecuteNonQuery(string sqlName,params object[] paramValues)
        {
            string sqlString;
            if (IsMappedSql(sqlName))
            {
                sqlString = SqlMap.GetSql(_isql, sqlName, paramValues);
            }
            else
            {
                sqlString = sqlName;
            }

            try
            {
                _db.ExecuteNonQueryBySql(sqlString);
            }
            catch (Exception ex)
            {
                throw new SqlException(ex.Message, sqlString, ex);
            }
        }
        public void ExecuteNonQuery(string sqlName)
        {
            ExecuteNonQuery(sqlName, null);
        }


        /// <summary>
        /// 执行查询SQL,返回DataTable
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sqlName, int pageSize, int curPage, params object[] paramValues)
        {
            int i;
            int recordStart = 0;
            int recordEnd = 0;
            DataTable dt = null;
            //DataRow dr;
            IDataReader idr = null;
            object[] vals = null;

            // 计算获取的记录编号范围(当前页)
            CalcRecordRange(pageSize, curPage, ref recordStart, ref recordEnd);

            try
            {
                // 执行查询
                if (IsMappedSql(sqlName) == false)
                {
                    idr = _db.ExecuteReaderBySql(sqlName);
                }
                else
                {
                    idr = _db.ExecuteReaderBySql(SqlMap.GetSql(_isql, sqlName, paramValues));
                }
                // 获取查询结果的结构DataTable
                dt = new DataTable();
                for (int k = 0; k < idr.FieldCount; k++)
                {
                    dt.Columns.Add(idr.GetName(k), idr.GetFieldType(k));
                }
                vals = new object[dt.Columns.Count];

                i = 0;
                // 获取指定范围的数据
                while (idr.Read())
                {
                    i++;
                    if (i < recordStart)
                    {
                    }
                    else if (i <= recordEnd)
                    {
                        idr.GetValues(vals);
                        dt.Rows.Add(vals);
                    }
                    else //if (i > recordEnd)
                    {
                        break;
                    }

                }
            }
            finally
            {
                if (idr != null) idr.Close();
            }

            return dt;
        }
        public DataTable ExecuteDataTable(string sqlName, int pageSize, int curPage)
        {
            return ExecuteDataTable(sqlName, pageSize, curPage, null);
        }
        public DataTable ExecuteDataTable(string sqlName)
        {
            return ExecuteDataTable(sqlName, -1, 0, null);
        }


        /// <summary>
        /// 执行查询SQL,返回列表object
        /// 前置条件: 必须是映射的SQL语句,且必须指定SQL语句的返回参数类
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public List<T> ExecuteListObject<T>(string sqlName, int pageSize, int curPage, params object[] paramValues)
        {
            int i;
            int recordStart = 0;
            int recordEnd = 0;
            List<T> objs = new List<T>();
            T obj;
            IDataReader idr = null;
            Type returnClass = null;

            // 如果未指定SQL语句的返回参数类, 将抛出异常
            returnClass = SqlMap.GetSqlReturnClass(sqlName);
            if (returnClass == null)
            {
                throw new Exception(string.Format("未指定SQL语句的返回参数类异常:{0}", sqlName));
            }

            // 计算获取的记录编号范围(当前页)
            CalcRecordRange(pageSize, curPage, ref recordStart, ref recordEnd);

            try
            {
                // 执行查询
                string sql = SqlMap.GetSql(_isql, sqlName, paramValues);
                idr = _db.ExecuteReaderBySql(sql);

                // 获取指定范围的数据
                i = 0;
                while (idr.Read())
                {
                    i++;
                    if (i < recordStart)
                    {
                    }
                    else if (i <= recordEnd)
                    {
                        obj = (T)ReflectUtil.CreateInstance(returnClass);
                        for (int j = 0; j < idr.FieldCount; j++)
                        {
                            ReflectUtil.SetPropertyValue(obj, idr.GetName(j), idr.GetValue(j));
                        }
                        objs.Add(obj);
                    }
                    else //if (i > recordEnd)
                    {
                        break;
                    }
                }
            }
            finally
            {
                if (idr != null) idr.Close();
            }

            return objs;
        }

        /// <summary>
        /// 执行查询SQL,返回object
        /// 前置条件: 必须是映射的SQL语句,且必须指定SQL语句的返回参数类
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public T ExecuteObject<T>(string sqlName, params object[] paramValues)
        {
            List<T> objs = ExecuteListObject<T>(sqlName, -1, 0, paramValues);
            if (objs.Count > 0)
            {
                return objs[0];
            }
            else
            {
                return default(T);
            }
        }


        /// <summary>
        /// 执行查询SQL,返回列表object
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <param name="returnClass">不能为null</param>
        /// <returns></returns>
        public List<T> ExecuteListObjectByType<T>(string sql, int pageSize, int curPage, Type returnClass)
        {
            int i;
            int recordStart = 0;
            int recordEnd = 0;
            List<T> objs = new List<T>();
            T obj;
            IDataReader idr = null;

            // 计算获取的记录编号范围(当前页)
            CalcRecordRange(pageSize, curPage, ref recordStart, ref recordEnd);

            try
            {
                // 执行查询
                idr = _db.ExecuteReaderBySql(sql);

                // 获取指定范围的数据
                i = 0;
                while (idr.Read())
                {
                    i++;
                    if (i < recordStart)
                    {
                    }
                    else if (i <= recordEnd)
                    {
                        obj = (T)ReflectUtil.CreateInstance(returnClass);

                        for (int j = 0; j < idr.FieldCount; j++)
                        {
                            ReflectUtil.SetPropertyValue(obj, idr.GetName(j), idr.GetValue(j));
                        }
                        objs.Add(obj);
                    }
                    else // (i > reocrdEnd)
                    {
                        break;
                    }
                }
            }
            finally
            {
                if (idr != null) idr.Close();
            }

            return objs;
        }

        /// <summary>
        /// 执行查询SQL,返回object
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="returnClass">不能为null</param>
        /// <returns></returns>
        public T ExecuteListObjectByType<T>(string sql, Type returnClass)
        {
            List<T> objs = ExecuteListObjectByType<T>(sql, -1, 0, returnClass);
            if (objs.Count > 0)
            {
                return objs[0];
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 执行查询SQL,返回列表object
        /// 前置条件: 必须是映射的SQL语句,且必须指定SQL语句的返回参数类
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public PageList<T> ExecutePageListObject<T>(string sqlName,string orderKey, int pageSize, int curPage, params object[] paramValues)
        {
            // 如果未指定SQL语句的返回参数类, 将抛出异常
            Type returnClass = SqlMap.GetSqlReturnClass(sqlName);
            if (returnClass == null)
            {
                throw new Exception(string.Format("未指定SQL语句的返回参数类异常:{0}", sqlName));
            }
            return ExecutePageListObjectByType<T>(SqlMap.GetSql(_isql, sqlName, paramValues), orderKey, pageSize, curPage, returnClass);
        }

        /// <summary>
        /// 执行查询SQL,返回列表object
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <param name="returnClass">不能为null</param>
        /// <returns></returns>
        public PageList<T> ExecutePageListObjectByType<T>(string sql,string orderKey, int pageSize, int curPage, Type returnClass)
        {
            PageList<T> objs = new PageList<T>();
            T obj;
            IDataReader idr = null;

            // 计算获取的记录编号范围(当前页)
            objs.SetPage(pageSize, curPage);

            try
            {
                // 获取总记录数
                if (pageSize != -1)
                {
                    objs.TotalCount = ParamUtil.getint(_db.ExecuteScalarBySql(_isql.CountSql(sql)));
                }
                string pageSql = _isql.PageSql(sql, orderKey, objs.StartRecord, objs.EndRecord);
                // 执行查询
                idr = _db.ExecuteReaderBySql(pageSql);

                // 获取指定范围的数据
                while (idr.Read())
                {
                    obj = (T)ReflectUtil.CreateInstance(returnClass);

                    // 第一列为COL_ROWNUM(记录号),不设置属性值
                    for (int i = 1; i < idr.FieldCount; i++)
                    {
                        ReflectUtil.SetPropertyValue(obj, idr.GetName(i), idr.GetValue(i));
                    }
                    objs.Add(obj);
                }

                // 如果未分页，则设置总数为当前页记录数
                if (pageSize == -1)
                {
                    objs.TotalCount = objs.CurPageCount;
                }
            }
            finally
            {
                if(idr != null)idr.Close();
            }

            return objs;
        }

        /// <summary>
        /// 获取新增记录的ID值
        /// </summary>
        /// <returns>long</returns>
        public long GetIndentityValue()
        {
            return ParamUtil.getlong(_db.ExecuteScalarBySql("SELECT CAST(@@IDENTITY as int) as value"));
        }

        #region 私有方法

        /// <summary>
        /// 判定是否为映射SQL
        /// </summary>
        /// <param name="sqlName"></param>
        /// <returns></returns>
        private bool IsMappedSql(string sqlName)
        {
            return (sqlName.IndexOf("$") >= 0);
        }

        /// <summary>
        /// 计算当前页的纪录编号集合
        /// </summary>
        /// <param name="pageSize">页记录数,如果为-1,表示不分页</param>
        /// <param name="curPage">返回页号,从1开始</param>
        /// <param name="recordStart">输出参数,起始记录编号</param>
        /// <param name="recordEnd">输出参数,截至记录编号</param>
        private void CalcRecordRange(int pageSize, int curPage, ref int recordStart, ref int recordEnd)
        {
            // 如果页记录数为-1,表示不分页
            if (pageSize == -1)
            {
                recordStart = 1;
                recordEnd = int.MaxValue;
            }
            else
            {
                recordStart = pageSize * (curPage - 1) + 1;
                recordEnd = recordStart + pageSize - 1;
            }
        }

        #endregion 


    }// 类结束
} // 命名空间结束
