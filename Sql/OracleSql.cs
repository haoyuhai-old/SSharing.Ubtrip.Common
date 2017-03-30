using System;

namespace SSharing.Ubtrip.Common.Sql
{
    /// <summary>
    /// Oracle PL-SQL���ʵ����
    /// </summary>
    public class OracleSql : BaseSql, ISql
    {
        /// <summary>
        /// ת��Ϊ�ַ���
        /// ǰ����������ʱ��֧�����ͣ��ַ���������3������
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public override string ToString(object val)
        {
            if (val == null)
            {
                return "null";
            }

            // ����1,000,000��,���ܱȽϺ�ѡ��

            // string ��� 
            if (val is System.String) // 0.8΢��
            {
                return string.Format("'{0}'", val.ToString().Replace("'", "''"));
            }
            else if (val is System.DateTime) // 2.0΢��
            {
                return string.Format("TO_DATE('{0}','yyyy-mm-dd hh24:mi:ss')", ((DateTime)val).ToString("yyyy-MM-dd hh:mm:ss"));
            }
            else if (val is Enum)
            {
                //ö�������������ݿ��б���Ϊ�ַ���
                return string.Format("'{0}'", val.ToString());
            }
            else // long 0.2΢��
            {
                return val.ToString();
            }


            //����
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


            // ����
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
        /// ����ָ��ҳ���ݵ�SQL
        /// </summary>
        /// <param name="sql">ԭʼSQL���</param>
        /// <param name="startIndex">��ʼ��¼</param>
        /// <param name="endIndex">��ֹ��¼</param>
        /// <returns></returns>
        public override string PageSql(string sql, string orderKey, int startIndex, int endIndex)
        {
            return string.Format("SELECT * FROM (SELECT ROWNUM AS {3},{4}.* FROM ({0}) {4} WHERE ROWNUM <={2}) WHERE {3} BETWEEN {1} AND {2}",
                sql, startIndex, endIndex, SqlReservedKeys.COL_ROWNUM, SqlReservedKeys.TAB_PAGESQL);
        }
    }
}
