using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;


namespace SSharing.Ubtrip.Common.Util
{
    /// <summary>
    /// 反射工具类
    /// </summary>
    public class ReflectUtil
    {
        /// <summary>
        /// 获取类型,如果类型不在缓存中,则加入缓存
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetType(string typeName)
        {
            return GetType(typeName,typeName);
        }

        /// <summary>
        /// 获取类型,如果类型不在缓存中,则加入缓存
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetType(string typeName, string keyName)
        {
            // 先从缓存中获取该类型
            Type objType = (Type)CacheUtil.GetCache(keyName);

            if (objType == null)
            {
                // 如果没有在缓存中找到，则使用默认类型,并写入缓存
                objType = Type.GetType(typeName, true);
                CacheUtil.SetCache(keyName, objType);
            }

            return objType;
        }


        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropertyValue(object obj, string propertyName)
        {
            // 约0.8微秒 string最快，int稍慢,Date最慢
            return obj.GetType().InvokeMember(propertyName, BindingFlags.GetProperty, null, obj, null);
        }


        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void SetPropertyValue(object obj, string propertyName, object value)
        {
            PropertyInfo pi = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
            if (pi == null)
                throw new Exception(string.Format("property name '{0}' not found", propertyName));
            if(!pi.CanWrite)
                throw new Exception(string.Format("property name '{0}' can not write", propertyName));


            pi.SetValue(obj, DbUtil.convert(pi.PropertyType,value), null);
            
            //obj.GetType().InvokeMember(propertyName, 
            //    BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.SetProperty,
            //    null, obj, new object[] { DbUtil.convert(mi[0].DeclaringType, obj) });
        }

        public static object CreateInstance(Type t)
        {
            return Activator.CreateInstance(t);
        }

    }// 类结束
}// 命名空间结束
