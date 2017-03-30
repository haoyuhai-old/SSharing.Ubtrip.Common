using System;

namespace SSharing.Ubtrip.Common.Sql
{
    /// <summary>
    /// Oracle PL-SQL语句实现类
    /// </summary>
    public class OracleSql : BaseSql, ISql
    {
        /// <summary>
        /// 转换为字符串
        /// 前置条件：暂时仅支持整型，字符串，日期3种类型
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public override string ToString(object val)
        {
            if (val == null)
            {
                return "null";
            }

            // 运行1,000,000次,性能比较和选择

            // string 最快 
            if (val is System.String) // 0.8微秒
            {
                return string.Format("'{0}'", val.ToString().Replace("'", "''"));
            }
            else if (val is System.DateTime) // 2.0微秒
            {
                return string.Format("TO_DATE('{0}','yyyy-mm-dd hh24:mi:ss')", ((DateTime)val).ToString("yyyy-MM-dd hh:mm:ss"));
            }
            else if (val is Enum)
            {
                //枚举类型在数数据库中保存为字符串
                return string.Format("'{0}'", val.ToString());
            }
            else // long 0.2微秒
            {
                return val.ToString();
            }


            //较慢
            //if (val is System.String) // 0.9
            //{
            //    StringBuilder sql = new StringBuilder();
            //    return sql.AppendFormat("'{0}'", val.ToString().Replace("'", "''")).ToString();
            //}
            //else if (val is System.DateTime) // 2.2
            //{
            //    StringBuilder sql = new StringBuilder();
            //    return sql.AppendFormat("TO_DATE('{0}','yyyy-mm-dd hh24:mi:ss')", ((DateTime)val).ToString("yyyy-MM-dd hh:mm:ss"));
            //}
            //else // long 0.3
            //{
            //    return val.ToString();
            //}


            // 最慢
            //switch (val.GetType().FullName)
            //{
            //    case "System.String":      // 1.4
            //        return string.Format("'{0}'", val.ToString().Replace("'", "''"));
            //    case "System.DateTime":    // 4.0
            //        return string.Format("TO_DATE('{0}','yyyy-mm-dd hh24:mi:ss')", ((DateTime)val).ToString("yyyy-MM-dd hh:mm:ss"));
            //    default:                   // long 0.6
            //        return val.ToString();
            //}
        }

        /// <summary>
        /// 返回指定页数据的SQL
        /// </summary>
        /// <param name="sql">原始SQL语句</param>
        /// <param name="startIndex">起始记录</param>
        /// <param name="endIndex">截止记录</param>
        /// <returns></returns>
        public override string PageSql(string sql, string orderKey, int startIndex, int endIndex)
        {
            return string.Format("SELECT * FROM (SELECT ROWNUM AS {3},{4}.* FROM ({0}) {4} WHERE ROWNUM <={2}) WHERE {3} BETWEEN {1} AND {2}",
                sql, startIndex, endIndex, SqlReservedKeys.COL_ROWNUM, SqlReservedKeys.TAB_PAGESQL);
        }
    }
}
