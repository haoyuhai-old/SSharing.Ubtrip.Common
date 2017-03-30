using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq.Expressions;

namespace SSharing.Ubtrip.Common.Sql
{
    /// <summary>
    /// 表示查询条件, 可以从该类派生子类以添加自定义的查询条件
    /// </summary>
    [Serializable, DataContract]
    public class Filter
    {
        private const string _DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        [DataMember]
        public List<FilterItem> FilterItems { get; private set; }
        [DataMember]
        public string OrderKey { get; private set; }

        /// <summary>
        /// 设置表查询选项(NoLock, ReadPast等)
        /// </summary>
        [DataMember]
        public string WithOption { get; set; }

        protected Filter()
        {
            FilterItems = new List<FilterItem>();
        }

        #region 公有方法

        /// <summary>
        /// 创建空实例
        /// </summary>
        public static Filter Create()
        {
            return new Filter();
        }

        /// <summary>
        /// 创建范型版的空实例
        /// </summary>
        public static Filter<T> Create<T>()
        {
            return new Filter<T>();
        }

        /// <summary>
        /// 根据linq表达式创建范型版的实例
        /// </summary>
        public static Filter<T> Create<T>(Expression<Func<T, bool>> exp)
        {
            return LinqParser<T>.GetSql(exp);
        }

        /// <summary>
        /// 创建一个新实例，并用一个等于条件初始化
        /// </summary>
        public static Filter CreateEq(string columnName, object columnValue)
        {
            return new Filter().Eq(columnName, columnValue);
        }

        /// <summary>
        /// 创建一个新实例，并用一个等于条件初始化
        /// </summary>
        public static Filter<T> CreateEq<T>(Expression<GetProperty<T>> property, object columnValue)
        {
            return new Filter<T>().Eq(property, columnValue);
        }

        /// <summary>
        /// 添加一个'等于'条件
        /// </summary>
        public Filter Eq(string columnName, object columnValue)
        {
            this.FilterItems.Add(new FilterItem(columnName, Operator.Eq, columnValue));
            return this;
        }
                
        /// <summary>
        /// 添加一个'不等于'条件
        /// </summary>
        public Filter NotEq(string columnName, object columnValue)
        {
            this.FilterItems.Add(new FilterItem(columnName, Operator.NotEq, columnValue));
            return this;
        }

        /// <summary>
        /// 添加一个'大于'条件
        /// </summary>
        public Filter Gt(string columnName, object columnValue)
        {
            this.FilterItems.Add(new FilterItem(columnName, Operator.Gt, columnValue));
            return this;
        }

        /// <summary>
        /// 添加一个'大于等于'条件
        /// </summary>
        public Filter Ge(string columnName, object columnValue)
        {
            this.FilterItems.Add(new FilterItem(columnName, Operator.Ge, columnValue));
            return this;
        }

        /// <summary>
        /// 添加一个'小于'条件
        /// </summary>
        public Filter Lt(string columnName, object columnValue)
        {
            this.FilterItems.Add(new FilterItem(columnName, Operator.Lt, columnValue));
            return this;
        }

        /// <summary>
        /// 添加一个'小于等于'条件
        /// </summary>
        public Filter Le(string columnName, object columnValue)
        {
            this.FilterItems.Add(new FilterItem(columnName, Operator.Le, columnValue));
            return this;
        }               

        /// <summary>
        /// 添加一个'Like'条件
        /// </summary>
        public Filter Like(string columnName, object columnValue)
        {
            this.FilterItems.Add(new FilterItem(columnName, Operator.Like, columnValue));
            return this;
        }

        /// <summary>
        /// 添加一个左匹配(like 'content%')条件
        /// </summary>
        public Filter LeftLike(string columnName, object columnValue)
        {
            this.FilterItems.Add(new FilterItem(columnName, Operator.LeftLike, columnValue));
            return this;
        }

        /// <summary>
        /// 添加一个右匹配(like '%content')条件
        /// </summary>
        public Filter RightLike(string columnName, object columnValue)
        {
            this.FilterItems.Add(new FilterItem(columnName, Operator.RightLike, columnValue));
            return this;
        }

        /// <summary>
        /// 添加自定义的查询条件，此条件将被原样置于Where条件中
        /// </summary>
        public Filter Custom(string customCondition)
        {
            this.FilterItems.Add(new FilterItem(customCondition));
            return this;
        }

        /// <summary>
        /// 设置OrderBy子句,如有多个字段，用','分隔(比如'Name ASC, Code DESC')
        /// </summary>
        public Filter OrderBy(string orderKey)
        {
            this.OrderKey = orderKey;
            return this;
        }

        /// <summary>
        /// 获取lamda表达式中指定属性定义的列名(比如: string colName = Filter.ColumnName(e => e.Name))
        /// </summary>
        public static string GetColName<T>(Expression<GetProperty<T>> property)
        {
            return MyReflection.GetColName(property);            
        }

        /// <summary>
        /// 获取条件字符串(WHERE后面的部分,不包括Order By)
        /// </summary>
        public string GetConditionString()
        {
            StringBuilder sb = new StringBuilder(" 1=1 ");

            foreach (PropertyInfo pi in this.GetType().GetProperties())
            {
                FilterColumnAttribute[] attrs = (FilterColumnAttribute[])pi.GetCustomAttributes(typeof(FilterColumnAttribute), true);
                if (attrs.Length > 0)
                {
                    string condition = GetCondition(GetDataType(pi.PropertyType), pi.GetValue(this, null), attrs[0].ColumnName, attrs[0].Operator);
                    if (!String.IsNullOrEmpty(condition))
                    {
                        sb.Append(" And " + condition);
                    }
                }
            }

            foreach (FilterItem filterItem in this.FilterItems)
            {
                if (!string.IsNullOrEmpty(filterItem.CustomCondition))
                {
                    sb.Append(" And (" + filterItem.CustomCondition + ")");
                }
                else
                {
                    string condition = GetCondition(GetDataType(filterItem.ColumnValue.GetType()), filterItem.ColumnValue, filterItem.ColumnName, filterItem.Operator);
                    if (!String.IsNullOrEmpty(condition))
                    {
                        sb.Append(" And " + condition);
                    }
                }
            }

            string str = sb.ToString();
            return str == " 1=1 " ? string.Empty : str;
        }

        #endregion

        #region 私有方法

        private static DataQueryType GetDataType(Type type)
        {
            return type == typeof(string) ? DataQueryType.String :
                type == typeof(DateTime) ? DataQueryType.DateTime :
                type == typeof(Byte) ? DataQueryType.Number :
                type == typeof(Int16) ? DataQueryType.Number :
                type == typeof(Int32) ? DataQueryType.Number :
                type == typeof(Int64) ? DataQueryType.Number :
                type == typeof(float) ? DataQueryType.Number :
                type == typeof(double) ? DataQueryType.Number :
                type == typeof(decimal) ? DataQueryType.Number :
                type.IsEnum ? DataQueryType.Enum : 
                DataQueryType.String;
        }

        public static string GetSqlInConditions(IEnumerable<long> ids)
        {
            if (ids == null) return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (long id in ids)
            {
                sb.Append(id).Append(",");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1); //移除最后的逗号
            }
            return sb.ToString();
        }

        public static string GetSqlInConditions(IEnumerable<int> ids)
        {
            if (ids == null) return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (int id in ids)
            {
                sb.Append(id).Append(",");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1); //移除最后的逗号
            }
            return sb.ToString();
        }

        public static string GetSqlInConditions(IEnumerable<string> codes)
        {
            if (codes == null) return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (string code in codes)
            {
                sb.AppendFormat("'{0}',", code);
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1); //移除最后的逗号
            }
            return sb.ToString();
        }

        private static string GetCondition(DataQueryType dataQueryType, object propertyValue, string columnName, Operator optr)
        {
            if (propertyValue == null) return string.Empty;

            if (optr == Operator.In)
            {
                #region In
                if (propertyValue is IList<long>)
                {
                    return string.Format("{0} IN ({1})", columnName, GetSqlInConditions((IEnumerable<long>)propertyValue));
                }
                else if (propertyValue is IList<int>)
                {
                    return string.Format("{0} IN ({1})", columnName, GetSqlInConditions((IEnumerable<int>)propertyValue));
                }
                else if (propertyValue is IList<string>)
                {
                    return string.Format("{0} IN ({1})", columnName, GetSqlInConditions((IEnumerable<string>)propertyValue));
                }
                else
                {
                    return string.Empty;
                }
                #endregion
            }

            if (optr == Operator.NotIn)
            {
                #region Not In
                if (propertyValue is IList<long>)
                {
                    return string.Format("{0} Not IN ({1})", columnName, GetSqlInConditions((IEnumerable<long>)propertyValue));
                }
                else if (propertyValue is IList<int>)
                {
                    return string.Format("{0} Not IN ({1})", columnName, GetSqlInConditions((IEnumerable<int>)propertyValue));
                }
                else if (propertyValue is IList<string>)
                {
                    return string.Format("{0} Not IN ({1})", columnName, GetSqlInConditions((IEnumerable<string>)propertyValue));
                }
                else
                {
                    return string.Empty;
                }
                #endregion
            }

            if (dataQueryType == DataQueryType.String)
            {
                #region
                string value = (string)propertyValue;
                value = value.Replace(@"\", @"\\");
                if (string.IsNullOrEmpty(value)) return string.Empty;                
                switch (optr)
                {
                    case Operator.Eq:
                        return string.Format("{0}='{1}'",  columnName, value);

                    case Operator.NotEq:
                        return string.Format("{0}!='{1}'", columnName, value);

                    case Operator.Like:
                        return string.Format("{0} like '%{1}%'", columnName, value);                        

                    case Operator.LeftLike:
                        return string.Format("{0} like '{1}%'", columnName, value);                        

                    case Operator.RightLike:
                        return string.Format("{0} like '%{1}'", columnName, value);                        

                    default:
                        throw new Exception(string.Format("当属性类型是string时,操作符{0}无效", optr));
                }
                #endregion
            }            
            else if (dataQueryType == DataQueryType.DateTime)
            {
                #region
                DateTime value = (DateTime)propertyValue;
                if (value == DateTime.MinValue) return string.Empty;
                switch (optr)
                {
                    case Operator.Eq:
                        return string.Format("{0}='{1}'", columnName, value.ToString(_DateTimeFormat));

                    case Operator.NotEq:
                        return string.Format("{0}!='{1}'", columnName, value.ToString(_DateTimeFormat));                        

                    case Operator.Gt:
                        return string.Format("{0}>'{1}'", columnName, value.ToString(_DateTimeFormat));

                    case Operator.Ge:
                        return string.Format("{0}>='{1}'", columnName, value.ToString(_DateTimeFormat));

                    case Operator.Lt:
                        return string.Format("{0}<'{1}'", columnName, value.ToString(_DateTimeFormat));

                    case Operator.Le:
                        return string.Format("{0}<='{1}'", columnName, value.ToString(_DateTimeFormat));

                    default:
                        throw new Exception(string.Format("当属性类型是DateTime时,操作符{0}无效", optr));
                }

                #endregion
            }
            else if (dataQueryType == DataQueryType.Number)
            {
                #region                
                switch (optr)
                {
                    case Operator.Eq:
                        return string.Format("{0}={1}", columnName, propertyValue.ToString());

                    case Operator.NotEq:
                        return string.Format("{0}!={1}", columnName, propertyValue.ToString());

                    case Operator.Gt:
                        return string.Format("{0}>{1}", columnName, propertyValue.ToString());

                    case Operator.Ge:
                        return string.Format("{0}>={1}", columnName, propertyValue.ToString());

                    case Operator.Lt:
                        return string.Format("{0}<{1}", columnName, propertyValue.ToString());

                    case Operator.Le:
                        return string.Format("{0}<={1}", columnName, propertyValue.ToString());

                    default:
                        throw new Exception(string.Format("当属性类型是整数时,操作符{0}无效", optr));
                }

                #endregion
            }
            else if (dataQueryType == DataQueryType.Enum)
            {
                #region
                if (EnumMetaDataCacheManager.GetEnumStorageType(propertyValue.GetType()) == EnumStorageType.EnumName)
                {
                    string value = propertyValue.ToString();
                    switch (optr)
                    {
                        case Operator.Eq:
                            return string.Format("{0}='{1}'", columnName, value);

                        case Operator.NotEq:
                            return string.Format("{0}!='{1}'", columnName, value);
                        default:
                            throw new Exception(string.Format("当属性类型是枚举时,操作符{0}无效", optr));
                    }
                }
                else
                {
                    int value = (int)propertyValue;
                    switch (optr)
                    {
                        case Operator.Eq:
                            return string.Format("{0}={1}", columnName, value);

                        case Operator.NotEq:
                            return string.Format("{0}!={1}", columnName, value);
                        default:
                            throw new Exception(string.Format("当属性类型是枚举时,操作符{0}无效", optr));
                    }
                }
                #endregion
            }
            else
            {
                throw new Exception(string.Format("查询条件的属性类型{0}无效", dataQueryType));
            }
        }

        public override string ToString()
        {
            return GetConditionString();
        }
        #endregion        
    }

    /// <summary>
    /// 表示范型版的查询条件
    /// </summary>
    [Serializable, DataContract]
    public class Filter<T> : Filter
    {
        internal protected Filter()
        {
        }

        /// <summary>
        /// 添加一个'等于'条件
        /// </summary>
        public Filter<T> Eq(Expression<GetProperty<T>> property, object columnValue)
        {
            string columnName = Filter.GetColName<T>(property);
            this.FilterItems.Add(new FilterItem(columnName, Operator.Eq, columnValue));
            return this;
        }

        /// <summary>
        /// 添加一个'不等于'条件
        /// </summary>
        public Filter<T> NotEq(Expression<GetProperty<T>> property, object columnValue)
        {
            string columnName = Filter.GetColName<T>(property);
            this.FilterItems.Add(new FilterItem(columnName, Operator.NotEq, columnValue));
            return this;
        }

        /// <summary>
        /// 添加一个'大于'条件
        /// </summary>
        public Filter<T> Gt(Expression<GetProperty<T>> property, object columnValue)
        {
            string columnName = Filter.GetColName<T>(property);
            this.FilterItems.Add(new FilterItem(columnName, Operator.Gt, columnValue));
            return this;
        }

        /// <summary>
        /// 添加一个'大于等于'条件
        /// </summary>
        public Filter<T> Ge(Expression<GetProperty<T>> property, object columnValue)
        {
            string columnName = Filter.GetColName<T>(property);
            this.FilterItems.Add(new FilterItem(columnName, Operator.Ge, columnValue));
            return this;
        }

        /// <summary>
        /// 添加一个'小于'条件
        /// </summary>
        public Filter<T> Lt(Expression<GetProperty<T>> property, object columnValue)
        {
            string columnName = Filter.GetColName<T>(property);
            this.FilterItems.Add(new FilterItem(columnName, Operator.Lt, columnValue));
            return this;
        }

        /// <summary>
        /// 添加一个'小于等于'条件
        /// </summary>
        public Filter<T> Le(Expression<GetProperty<T>> property, object columnValue)
        {
            string columnName = Filter.GetColName<T>(property);
            this.FilterItems.Add(new FilterItem(columnName, Operator.Le, columnValue));
            return this;
        }

        /// <summary>
        /// 添加一个'Like'条件
        /// </summary>
        public Filter<T> Like(Expression<GetProperty<T>> property, object columnValue)
        {
            string columnName = Filter.GetColName<T>(property);
            this.FilterItems.Add(new FilterItem(columnName, Operator.Like, columnValue));
            return this;
        }

        /// <summary>
        /// 添加一个左匹配(like 'content%')条件
        /// </summary>
        public Filter<T> LeftLike(Expression<GetProperty<T>> property, object columnValue)
        {
            string columnName = Filter.GetColName<T>(property);
            this.FilterItems.Add(new FilterItem(columnName, Operator.LeftLike, columnValue));
            return this;
        }

        /// <summary>
        /// 添加一个右匹配(like '%content')条件
        /// </summary>
        public Filter<T> RightLike(Expression<GetProperty<T>> property, object columnValue)
        {
            string columnName = Filter.GetColName<T>(property);
            this.FilterItems.Add(new FilterItem(columnName, Operator.RightLike, columnValue));
            return this;
        }

        /// <summary>
        /// 添加一个In条件
        /// </summary>
        public Filter<T> In(Expression<GetProperty<T>> property, IEnumerable<long> numbers)
        {
            return In_Internal(property, numbers);
        }

        /// <summary>
        /// 添加一个In条件
        /// </summary>
        public Filter<T> In(Expression<GetProperty<T>> property, IEnumerable<int> numbers)
        {
            return In_Internal(property, numbers);
        }

        /// <summary>
        /// 添加一个In条件,内部使用
        /// </summary>
        public Filter<T> In(Expression<GetProperty<T>> property, IEnumerable<string> strings)
        {
            return In_Internal(property, strings);
        }

        private Filter<T> In_Internal(Expression<GetProperty<T>> property, object value)
        {
            string columnName = Filter.GetColName<T>(property);
            this.FilterItems.Add(new FilterItem(columnName, Operator.In, value));
            return this;
        }  

        /// <summary>
        /// 添加一个NotIn条件
        /// </summary>
        public Filter<T> NotIn(Expression<GetProperty<T>> property, IEnumerable<long> numbers)
        {
            return NotIn_Internal(property, numbers);
        }

        /// <summary>
        /// 添加一个NotIn条件
        /// </summary>
        public Filter<T> NotIn(Expression<GetProperty<T>> property, IEnumerable<int> numbers)
        {
            return NotIn_Internal(property, numbers);
        }

        /// <summary>
        /// 添加一个NotIn条件,内部使用
        /// </summary>
        public Filter<T> NotIn(Expression<GetProperty<T>> property, IEnumerable<string> strings)
        {
            return NotIn_Internal(property, strings);
        }


        private Filter<T> NotIn_Internal(Expression<GetProperty<T>> property, object value)
        {
            string columnName = Filter.GetColName<T>(property);
            this.FilterItems.Add(new FilterItem(columnName, Operator.NotIn, value));
            return this;
        }

        /// <summary>
        /// 设置OrderBy子句,如有多个字段，用','分隔(比如'Name ASC, Code DESC')
        /// </summary>
        public new Filter<T> OrderBy(string orderKey)
        {
            base.OrderBy(orderKey);
            return this;
        }

        /// <summary>
        /// 设置OrderBy子句,如有多个字段，用','分隔(比如'Name ASC, Code DESC')
        /// </summary>
        public Filter<T> OrderBy(Expression<GetProperty<T>> property)
        {
            string columnName = Filter.GetColName<T>(property);
            base.OrderBy(columnName);
            return this;
        }

        /// <summary>
        /// 获取数据库的列名
        /// </summary>
        public string GetDbColumnName(Expression<GetProperty<T>> property)
        {
            return MyReflection.GetColName(property);            
        }
    }

    [Serializable, DataContract]
    public class FilterItem
    {
        public FilterItem(string customCondition)
        {
            if (string.IsNullOrEmpty(customCondition))
                throw new ArgumentNullException("customCondition");

            CustomCondition = customCondition;
        }

        public FilterItem(string columnName, Operator optr, object columnValue)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentNullException("columnName");
            if (columnValue == null)
                throw new ArgumentNullException("columnValue");

            ColumnName = columnName;
            ColumnValue = columnValue;
            Operator = optr;
        }

        [DataMember]
        public string ColumnName { get; set; }
        [DataMember]
        public object ColumnValue { get; set; }
        [DataMember]
        public Operator Operator { get; set; }

        [DataMember]
        public string CustomCondition { get; set; }
    }

    public delegate object GetProperty<T>(T declaringType);

    public enum DataQueryType
    {
        String,
        DateTime,
        Number,
        Enum,
    }
}
