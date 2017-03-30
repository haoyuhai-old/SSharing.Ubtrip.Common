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
    /// ���ݷ���ͨ�ù�����
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
        /// ִ�е�ֵSQL
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
        /// ִ�зǲ�ѯSQL
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
        /// ִ�в�ѯSQL,����DataTable
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

            // �����ȡ�ļ�¼��ŷ�Χ(��ǰҳ)
            CalcRecordRange(pageSize, curPage, ref recordStart, ref recordEnd);

            try
            {
                // ִ�в�ѯ
                if (IsMappedSql(sqlName) == false)
                {
                    idr = _db.ExecuteReaderBySql(sqlName);
                }
                else
                {
                    idr = _db.ExecuteReaderBySql(SqlMap.GetSql(_isql, sqlName, paramValues));
                }
                // ��ȡ��ѯ����ĽṹDataTable
                dt = new DataTable();
                for (int k = 0; k < idr.FieldCount; k++)
                {
                    dt.Columns.Add(idr.GetName(k), idr.GetFieldType(k));
                }
                vals = new object[dt.Columns.Count];

                i = 0;
                // ��ȡָ����Χ������
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
        /// ִ�в�ѯSQL,�����б�object
        /// ǰ������: ������ӳ���SQL���,�ұ���ָ��SQL���ķ��ز�����
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

            // ���δָ��SQL���ķ��ز�����, ���׳��쳣
            returnClass = SqlMap.GetSqlReturnClass(sqlName);
            if (returnClass == null)
            {
                throw new Exception(string.Format("δָ��SQL���ķ��ز������쳣:{0}", sqlName));
            }

            // �����ȡ�ļ�¼��ŷ�Χ(��ǰҳ)
            CalcRecordRange(pageSize, curPage, ref recordStart, ref recordEnd);

            try
            {
                // ִ�в�ѯ
                string sql = SqlMap.GetSql(_isql, sqlName, paramValues);
                idr = _db.ExecuteReaderBySql(sql);

                // ��ȡָ����Χ������
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
        /// ִ�в�ѯSQL,����object
        /// ǰ������: ������ӳ���SQL���,�ұ���ָ��SQL���ķ��ز�����
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
        /// ִ�в�ѯSQL,�����б�object
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <param name="returnClass">����Ϊnull</param>
        /// <returns></returns>
        public List<T> ExecuteListObjectByType<T>(string sql, int pageSize, int curPage, Type returnClass)
        {
            int i;
            int recordStart = 0;
            int recordEnd = 0;
            List<T> objs = new List<T>();
            T obj;
            IDataReader idr = null;

            // �����ȡ�ļ�¼��ŷ�Χ(��ǰҳ)
            CalcRecordRange(pageSize, curPage, ref recordStart, ref recordEnd);

            try
            {
                // ִ�в�ѯ
                idr = _db.ExecuteReaderBySql(sql);

                // ��ȡָ����Χ������
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
        /// ִ�в�ѯSQL,����object
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="returnClass">����Ϊnull</param>
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
        /// ִ�в�ѯSQL,�����б�object
        /// ǰ������: ������ӳ���SQL���,�ұ���ָ��SQL���ķ��ز�����
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public PageList<T> ExecutePageListObject<T>(string sqlName,string orderKey, int pageSize, int curPage, params object[] paramValues)
        {
            // ���δָ��SQL���ķ��ز�����, ���׳��쳣
            Type returnClass = SqlMap.GetSqlReturnClass(sqlName);
            if (returnClass == null)
            {
                throw new Exception(string.Format("δָ��SQL���ķ��ز������쳣:{0}", sqlName));
            }
            return ExecutePageListObjectByType<T>(SqlMap.GetSql(_isql, sqlName, paramValues), orderKey, pageSize, curPage, returnClass);
        }

        /// <summary>
        /// ִ�в�ѯSQL,�����б�object
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <param name="returnClass">����Ϊnull</param>
        /// <returns></returns>
        public PageList<T> ExecutePageListObjectByType<T>(string sql,string orderKey, int pageSize, int curPage, Type returnClass)
        {
            PageList<T> objs = new PageList<T>();
            T obj;
            IDataReader idr = null;

            // �����ȡ�ļ�¼��ŷ�Χ(��ǰҳ)
            objs.SetPage(pageSize, curPage);

            try
            {
                // ��ȡ�ܼ�¼��
                if (pageSize != -1)
                {
                    objs.TotalCount = ParamUtil.getint(_db.ExecuteScalarBySql(_isql.CountSql(sql)));
                }
                string pageSql = _isql.PageSql(sql, orderKey, objs.StartRecord, objs.EndRecord);
                // ִ�в�ѯ
                idr = _db.ExecuteReaderBySql(pageSql);

                // ��ȡָ����Χ������
                while (idr.Read())
                {
                    obj = (T)ReflectUtil.CreateInstance(returnClass);

                    // ��һ��ΪCOL_ROWNUM(��¼��),����������ֵ
                    for (int i = 1; i < idr.FieldCount; i++)
                    {
                        ReflectUtil.SetPropertyValue(obj, idr.GetName(i), idr.GetValue(i));
                    }
                    objs.Add(obj);
                }

                // ���δ��ҳ������������Ϊ��ǰҳ��¼��
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
        /// ��ȡ������¼��IDֵ
        /// </summary>
        /// <returns>long</returns>
        public long GetIndentityValue()
        {
            return ParamUtil.getlong(_db.ExecuteScalarBySql("SELECT CAST(@@IDENTITY as int) as value"));
        }

        #region ˽�з���

        /// <summary>
        /// �ж��Ƿ�Ϊӳ��SQL
        /// </summary>
        /// <param name="sqlName"></param>
        /// <returns></returns>
        private bool IsMappedSql(string sqlName)
        {
            return (sqlName.IndexOf("$") >= 0);
        }

        /// <summary>
        /// ���㵱ǰҳ�ļ�¼��ż���
        /// </summary>
        /// <param name="pageSize">ҳ��¼��,���Ϊ-1,��ʾ����ҳ</param>
        /// <param name="curPage">����ҳ��,��1��ʼ</param>
        /// <param name="recordStart">�������,��ʼ��¼���</param>
        /// <param name="recordEnd">�������,������¼���</param>
        private void CalcRecordRange(int pageSize, int curPage, ref int recordStart, ref int recordEnd)
        {
            // ���ҳ��¼��Ϊ-1,��ʾ����ҳ
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


    }// �����
} // �����ռ����
