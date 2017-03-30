using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSharing.Ubtrip.Common.Sql
{
    /// <summary>
    /// 查询条件列
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class FilterColumnAttribute : Attribute
    {
        public FilterColumnAttribute(string columnName)
            : this(columnName, Operator.Eq)
        {
        }

        public FilterColumnAttribute(string columnName, Operator queryOperator)
        {
            this.ColumnName = columnName;
            this.Operator = queryOperator;
        }

        public string ColumnName { get; set; }

        public Operator Operator { get; set; }
    }


    /// <summary>
    /// 查询条件操作符
    /// </summary>
    public enum Operator
    {
        /// <summary>
        /// 等于
        /// </summary>
        Eq,

        /// <summary>
        /// 不等于
        /// </summary>
        NotEq,

        /// <summary>
        /// 大于
        /// </summary>
        Gt,

        /// <summary>
        /// 大于等于
        /// </summary>
        Ge,

        /// <summary>
        /// 小于
        /// </summary>
        Lt,

        /// <summary>
        /// 小于等于
        /// </summary>
        Le,

        /// <summary>
        /// 左匹配(like 'content%')
        /// </summary>
        LeftLike,

        /// <summary>
        /// 右匹配(like '%content')
        /// </summary>
        RightLike,

        /// <summary>
        /// 包含(like '%content%')
        /// </summary>
        Like,

        /// <summary>
        /// In
        /// </summary>
        In,

        /// <summary>
        /// In
        /// </summary>
        NotIn,
    }
}
