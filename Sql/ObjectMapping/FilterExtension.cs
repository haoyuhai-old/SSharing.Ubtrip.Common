using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSharing.Ubtrip.Common.Sql
{
    public static class FilterExtension
    {
        /// <summary>
        /// 获取用来查询的完整的Select字符串
        /// </summary>
        public static string GetSelectString(this Filter filter, Type type)
        {
            string condition = filter.GetConditionString();
            string orderKey = filter.OrderKey;

            TableMetaData table = MetaDataCacheManager.GetTableMetaData(type);

            StringBuilder sb = new StringBuilder("Select * From " + table.TableName);
            if (!string.IsNullOrEmpty(filter.WithOption))
            {
                sb.AppendFormat(" WITH({0})", filter.WithOption);
            }
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" Where ").Append(condition);
            }
            if (!string.IsNullOrEmpty(orderKey))
            {
                sb.Append(" Order By ").Append(orderKey);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取用来查询的完整的Select字符串
        /// </summary>
        public static string GetSelectString<T>(this Filter<T> filter)
        {
            return GetSelectString(filter, typeof(T));
        }
    }
}
