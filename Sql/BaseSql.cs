using System;
using System.Collections.Generic;
using System.Text;

using SSharing.Ubtrip.Common.Util;

namespace SSharing.Ubtrip.Common.Sql
{
    abstract public class BaseSql : ISql
    {
        /// <summary>
        /// 转换为字符串
        /// 注意: 必须重载BaseSql.ToString()方法
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        virtual public string ToString(object value)
        {
            throw new Exception("必须重载BaseSql.ToString()方法");
        }

        /// <summary>
        /// 返回计算总记录数的SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        virtual public string CountSql(string sql)
        {
            // 若有order by 则需要去掉
            //if (sql.ToLower().IndexOf("order by") != -1)
            //{
            //    int idx = sql.ToLower().IndexOf("order by");
            //    sql = sql.Substring(0, idx);
            //}
            return string.Format("SELECT COUNT(1) FROM ({0}) tmpTable", sql);
        }

        /// <summary>
        /// 返回指定页数据的SQL
        /// </summary>
        /// <param name="sql">原始SQL语句</param>
        /// <param name="startIndex">起始记录</param>
        /// <param name="endIndex">截止记录</param>
        /// <returns></returns>
        virtual public string PageSql(string sql,string idKey, int startIndex, int endIndex)
        {
            throw new Exception("必须重载BaseSql.PageSql()方法");
        }


    }
}
