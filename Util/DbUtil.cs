using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace SSharing.Ubtrip.Common.Util
{
    /// <summary>
    /// 数据库类型转换工具类
    /// </summary>
    public class DbUtil
    {
        public static readonly string LANG_ZHS = "ZHS";
        public static readonly string LANG_US = "US";

        ///// <summary>
        ///// 序列号
        ///// </summary>
        ///// <param name="sequenceName"></param>
        ///// <returns></returns>
        //public static long GetSequenceNext(DbMgr db, string sequenceName)
        //{
        //    string sql = string.Format("SELECT {0}.NEXTVAL FROM DUAL", sequenceName);
        //    return ParamUtil.getlong(db.ExecuteScalar(sql));
        //}

        /// <summary>
        /// 语言转换
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string SqlLanguage(string culture)
        {
            if (culture == null || culture == "zh-CN" || culture == "")
            {
                return LANG_ZHS;
            }
            else
            {
                return LANG_US;
            }
        }

        /// <summary>
        /// 判定是否为null值
        /// 判定为null规则：
        /// 1.字符串 ==""
        /// 2.整型==0
        /// 其他类型不允许为null;
        /// </summary>
        /// <param name="val"></param>
        /// <returns>true:null, false:非null</returns>
        public static bool isnull(object val)
        {
            if ((val == null) ||
                (val is System.String && val.ToString() == "") ||
                ((val is System.Int32 || val is System.Int64) && val.ToString() == "0") ||
                ((val is DateTime) && (DateTime)val == ParamUtil.GetEmptyDatetime()) ||
                ((val is Decimal) && (Decimal)val == 0m) ||
                ((val is double) && Math.Abs((double)val) < 0.01))
            {
                return true;
            }

            // 默认为非null
            return false;
        }

        /// <summary>
        /// 将value转换为类型t的值
        /// 从快到慢依次是 string -> long64 -> double -> int32 -> DateTime 
        /// 出于性能考虑,不支持以上5类型之外的其他数据类型
        /// 100万次运行，平均1次时间均不超过0.2微秒, 速度与if语句的先后顺序有关
        /// 类型比较和字符串比较，时间相近 ,都是单cpu
        /// </summary>
        /// <param name="t"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object convert(Type t, object value)
        {
            if (t == typeof(String))
            {
                return (String)(value == DBNull.Value ? string.Empty : value);
            }
            else if (t == typeof(Int64))
            {
                return Convert.ToInt64(value == DBNull.Value ? 0 : value);
            }
            else if (t == typeof(Double))
            {
                return Convert.ToDouble(value == DBNull.Value ? 0 : value);
            }
            else if (t == typeof(Decimal))
            {
                return Convert.ToDecimal(value == DBNull.Value ? 0 : value);
            }
            else if (t == typeof(Int32))
            {
                return Convert.ToInt32(value == DBNull.Value ? 0 : value);
            }
            else if (t== typeof(Byte))
            {
                return Convert.ToByte(value == DBNull.Value ? 0 : value);
            }
            else if (t == typeof(DateTime))
            {
                return (DateTime)(value == DBNull.Value ? DateTime.MinValue : value);
                //if(value==null || value.ToString()==string.Empty)
                // {
                //     return System.DateTime.MinValue;
                // }                      
            }
            else if (t == typeof(Boolean))
            {
                return value == DBNull.Value ? false : value.ToString() == "1" || value.ToString() == "Y" || string.Equals(value.ToString(), "true", StringComparison.CurrentCultureIgnoreCase);
            }
            else if (t.BaseType == typeof(Enum))
            {
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    return Enum.Parse(t, value.ToString(), true);
                }
                else
                {
                    return Enum.Parse(t, "0");
                }
            }
            else
            {
                throw new Exception(string.Format("数据库类型异常.不支持类型{0}.", t));
            }
        }

        public static object convert(string t, object value)
        {
            if (t == "System.String")
            {
                return (System.String)value;
            }
            else if (t == "System.Int64")
            {
                return Convert.ToInt64(value);
            }
            else if (t == "System.Double")
            {
                return Convert.ToDouble(value);
            }
            else if (t == "System.Decimal")
            {
                return Convert.ToDecimal(value);
            }
            else if (t == "System.Int32")
            {
                return Convert.ToInt32(value);
            }
            else if (t == "System.DateTime")
            {
                return (System.DateTime)value;
            }
            else
            {
                throw new Exception(string.Format("数据库类型异常.不支持类型{0}.", t));
            }
        }

    }// 类结束
}// 命名空间结束
