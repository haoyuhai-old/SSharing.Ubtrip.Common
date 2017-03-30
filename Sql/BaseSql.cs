using System;
using System.Collections.Generic;
using System.Text;

using SSharing.Ubtrip.Common.Util;

namespace SSharing.Ubtrip.Common.Sql
{
    abstract public class BaseSql : ISql
    {
        /// <summary>
        /// ת��Ϊ�ַ���
        /// ע��: ��������BaseSql.ToString()����
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        virtual public string ToString(object value)
        {
            throw new Exception("��������BaseSql.ToString()����");
        }

        /// <summary>
        /// ���ؼ����ܼ�¼����SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        virtual public string CountSql(string sql)
        {
            // ����order by ����Ҫȥ��
            //if (sql.ToLower().IndexOf("order by") != -1)
            //{
            //    int idx = sql.ToLower().IndexOf("order by");
            //    sql = sql.Substring(0, idx);
            //}
            return string.Format("SELECT COUNT(1) FROM ({0}) tmpTable", sql);
        }

        /// <summary>
        /// ����ָ��ҳ���ݵ�SQL
        /// </summary>
        /// <param name="sql">ԭʼSQL���</param>
        /// <param name="startIndex">��ʼ��¼</param>
        /// <param name="endIndex">��ֹ��¼</param>
        /// <returns></returns>
        virtual public string PageSql(string sql,string idKey, int startIndex, int endIndex)
        {
            throw new Exception("��������BaseSql.PageSql()����");
        }


    }
}
