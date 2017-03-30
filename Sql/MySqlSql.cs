using System;
using SSharing.Ubtrip.Common.Util;
using System.Data;

namespace SSharing.Ubtrip.Common.Sql
{
    /// <summary>
    /// MySql T-SQL语句实现类
    /// </summary>
    public class MySqlSql : BaseSql, ISql
    {
        /// <summary>
        /// 转换为字符串
        /// 前置条件：暂时仅支持整型，字符串，日期3种类型
        /// </summary>
        public override string ToString(object val)
        {
            if (val == null) return "null";

            // 运行1,000,000次,性能比较和选择
            // string 最快 
            if (val is String) // 0.8微秒
            {
                string newVal = val.ToString();
                if (newVal.StartsWith("'") && newVal.EndsWith("'")) return newVal;

                return string.Format("'{0}'", newVal.Replace("'", "''"));
            }
            else if (val is DateTime) // 2.0微秒
            {
                if (ParamUtil.GetEmptyDatetime().Equals((DateTime)val))
                {
                    return "null";
                }
                else
                {
                    return string.Format("CONVERT(datetime,'{0}',120)", ((DateTime)val).ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            else if (val is Enum)
            {
                DbType dbType = DbExecuteEngine.GetEnumDbType(val.GetType());
                if (dbType == DbType.Int32)
                    return ((int)val).ToString();                   //取枚举整数值
                else
                    return string.Format("'{0}'", val.ToString());  //取枚举字符串
            }
            else if (val is Byte[])
            {
                return Convert.ToBase64String((Byte[])val, Base64FormattingOptions.None);
            }
            else // long 0.2微秒
            {
                return val.ToString();
            }
        }

        ///// <summary>
        ///// 返回指定页数据的SQL
        ///// </summary>
        ///// <param name="sql">原始SQL语句</param>
        ///// <param name="startIndex">起始记录</param>
        ///// <param name="endIndex">截止记录</param>
        ///// <returns></returns>
        //public override string PageSql(string sql, string orderKey, int startIndex, int endIndex)
        //{
        //    // 若有order by 则需要去掉
        //    //if (sql.ToLower().IndexOf("order by") != -1)
        //    //{
        //    //    int idx = sql.ToLower().IndexOf("order by");
        //    //    sql = sql.Substring(0, idx);
        //    //}
        //    return string.Format("SELECT * FROM (SELECT ROW_NUMBER() Over(ORDER BY {5}) AS {3},{4}.* FROM ({0}) {4}) tmpTable WHERE {3} >= {1} AND {3} <= {2}",
        //        sql, startIndex, endIndex, SqlReservedKeys.COL_ROWNUM, SqlReservedKeys.TAB_PAGESQL, orderKey);
        //}
    }
}
