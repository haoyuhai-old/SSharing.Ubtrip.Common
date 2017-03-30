using System;
using System.Collections.Generic;
using System.Text;

namespace SSharing.Ubtrip.Common.Sql
{
    public static class SqlHelper
    {
        public static string GetSqlInConditions(List<long> ids)
        {
            StringBuilder sb = new StringBuilder();
            foreach (long id in ids)
            {
                sb.Append(id).Append(",");
            }
            sb.Remove(sb.Length - 1, 1); //移除最后的逗号
            return sb.ToString();
        }

        public static string GetSqlInConditions(List<string> codes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string code in codes)
            {
                sb.AppendFormat("'{0}',", code);
            }
            sb.Remove(sb.Length - 1, 1); //移除最后的逗号
            return sb.ToString();
        }
    }
}
