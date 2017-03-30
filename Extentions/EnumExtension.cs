using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SSharing.Ubtrip.Common
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class AttachDataAttribute : Attribute
    {
        public AttachDataAttribute(object key, object value)
        {
            this.Key = key;
            this.Value = value;
        }

        public object Key { get; private set; }

        public object Value { get; private set; }
    }

    /// <summary>
    /// 表示枚举项的中文显示文本
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class CnTextAttribute : AttachDataAttribute
    {
        public CnTextAttribute(string cnText)
            : base("CnText", cnText)
        {
        }
    }

    /// <summary>
    /// 表示枚举项的英文显示文本
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class EnTextAttribute : AttachDataAttribute
    {
        public EnTextAttribute(string enText)
            : base("EnText", enText)
        {
        }
    }

    /// <summary>
    /// 表示枚举项的值
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class EnumValueAttribute : AttachDataAttribute
    {
        public EnumValueAttribute(string enumValue)
            : base("EnumValue", enumValue)
        {
        }
    }

    public static class AttachDataExtensions
    {
        /// <summary>
        /// 获取枚举项的中文显示文本
        /// </summary>
        public static string GetCnText(this Enum value)
        {
            return GetEnumAttachedText(value, "CnText");
        }

        /// <summary>
        /// 获取枚举项的英文显示文本
        /// </summary>
        public static string GetEnText(this Enum value)
        {
            return GetEnumAttachedText(value, "EnText");
        }

        /// <summary>
        /// 获取枚举项的显示文本,如果当前请求的线程是英文版，则显示EnText, 否则显示CnText(如果未定义英文文字，也会显示中文)
        /// </summary>
        public static string GetTextByCulture(this Enum value)
        {
            //if (Lang.IsEnglish)
            //{
            //    string str = GetEnumAttachedText(value, "EnText");
            //    if (str.IsNotNullOrEmpty())
            //    {
            //        return str;
            //    }
            //}

            return GetEnumAttachedText(value, "CnText");
        }

        /// <summary>
        /// 获取枚举项的值(如果定义了EnumValue标记，则返回EnumValue定义的值，否则返回枚举项的字符串定义)
        /// </summary>
        public static string GetEnumValue(this Enum value)
        {
            string result = GetEnumAttachedText(value, "EnumValue");
            return string.IsNullOrEmpty(result) ? value.ToString() : result;
        }

        private static string GetEnumAttachedText(Enum value, string key)
        {
            FieldInfo enumField = value.GetType().GetField(value.ToString());
            if (enumField == null)
                throw new ArgumentException(string.Format("参数{0}不是枚举类型{1}的有效值", value.ToString(), value.GetType()), "value");

            var attributes = (AttachDataAttribute[])enumField.GetCustomAttributes(typeof(AttachDataAttribute), false);
            foreach (AttachDataAttribute item in attributes)
            {
                if (item.Key.Equals(key))
                    return (string)item.Value;
            }
            return string.Empty;
        }
    }

    public static class EnumUtil
    {
        /// <summary>
        /// 通过枚举的值("EnumValue标记的值")来获取枚举对象,如果给定的字符串无效，则抛出异常
        /// </summary>
        public static T ParseByEnumValue<T>(string enumValue) where T : struct
        {
            foreach (int i in Enum.GetValues(typeof(T)))
            {
                object enumObj = Enum.ToObject(typeof(T), i);

                if (((Enum)enumObj).GetEnumValue() == enumValue)
                {
                    return (T)enumObj;
                }
            }
            throw new ArgumentOutOfRangeException("enumValue", "无效的enumValue:" + enumValue);
        }

        /// <summary>
        /// 通过枚举CnText标记的值来获取枚举对象,如果给定的字符串无效，则抛出异常
        /// </summary>
        public static T ParseByCnText<T>(string enumCnText) where T : struct
        {
            foreach (int i in Enum.GetValues(typeof(T)))
            {
                object enumObj = Enum.ToObject(typeof(T), i);

                if (((Enum)enumObj).GetCnText() == enumCnText)
                {
                    return (T)enumObj;
                }
            }
            throw new ArgumentOutOfRangeException("enumCnText", "无效的enumCnText:" + enumCnText);
        }

        /// <summary>
        /// 通过枚举的名称("Enum的名称或数字值的字符串")来获取枚举对象,如果给定的字符串无效，则抛出异常
        /// </summary>
        public static T Parse<T>(string enumNameOrNumber) where T : struct
        {
            return (T)Enum.Parse(typeof(T), enumNameOrNumber);
        }

        /// <summary>
        /// 通过枚举的名称("Enum的名称或数字值的字符串")来获取枚举对象,如果给定的字符串无效，则返回default(T)
        /// </summary>
        public static T ParseOrDefault<T>(string enumNameOrNumber) where T : struct
        {
            if (string.IsNullOrWhiteSpace(enumNameOrNumber)) return default(T);

            T result;
            if (Enum.TryParse<T>(enumNameOrNumber, true, out result))
            {
                return result;
            }
            else
            {
                return default(T);
            }
        }
    }
}
