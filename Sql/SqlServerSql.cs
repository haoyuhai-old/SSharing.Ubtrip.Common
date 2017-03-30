using System;
using SSharing.Ubtrip.Common.Util;
using System.Data;

namespace SSharing.Ubtrip.Common.Sql
{
    /// <summary>
    /// SqlServer T-SQL���ʵ����
    /// </summary>
    public class SqlServerSql : BaseSql, ISql
    {
        /// <summary>
        /// ת��Ϊ�ַ���
        /// ǰ����������ʱ��֧�����ͣ��ַ���������3������
        /// </summary>
        public override string ToString(object val)
        {
            if (val == null) return "null";

            // ����1,000,000��,���ܱȽϺ�ѡ��
            // string ��� 
            if (val is String) // 0.8΢��
            {
                string newVal = val.ToString();
                if (newVal.StartsWith("'") && newVal.EndsWith("'")) return newVal;

                return string.Format("'{0}'", newVal.Replace("'", "''"));
            }
            else if (val is DateTime) // 2.0΢��
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
                    return ((int)val).ToString();                   //ȡö������ֵ
                else
                    return string.Format("'{0}'", val.ToString());  //ȡö���ַ���
            }
            else if (val is Byte[])
            {
                return Convert.ToBase64String((Byte[])val, Base64FormattingOptions.None);
            }
            else // long 0.2΢��
            {
                return val.ToString();
            }
        }

        /// <summary>
        /// ����ָ��ҳ���ݵ�SQL
        /// </summary>
        /// <param name="sql">ԭʼSQL���</param>
        /// <param name="startIndex">��ʼ��¼</param>
        /// <param name="endIndex">��ֹ��¼</param>
        /// <returns></returns>
        public override string PageSql(string sql, string orderKey, int startIndex, int endIndex)
        {
            // ����order by ����Ҫȥ��
            //if (sql.ToLower().IndexOf("order by") != -1)
            //{
            //    int idx = sql.ToLower().IndexOf("order by");
            //    sql = sql.Substring(0, idx);
            //}
            return string.Format("SELECT * FROM (SELECT ROW_NUMBER() Over(ORDER BY {5}) AS {3},{4}.* FROM ({0}) {4}) tmpTable WHERE {3} >= {1} AND {3} <= {2}",
                sql, startIndex, endIndex, SqlReservedKeys.COL_ROWNUM, SqlReservedKeys.TAB_PAGESQL, orderKey);
        }
    }// ��
}
