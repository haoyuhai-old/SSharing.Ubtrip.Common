using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;


namespace SSharing.Ubtrip.Common.Util
{
    /// <summary>
    /// ���乤����
    /// </summary>
    public class ReflectUtil
    {
        /// <summary>
        /// ��ȡ����,������Ͳ��ڻ�����,����뻺��
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetType(string typeName)
        {
            return GetType(typeName,typeName);
        }

        /// <summary>
        /// ��ȡ����,������Ͳ��ڻ�����,����뻺��
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetType(string typeName, string keyName)
        {
            // �ȴӻ����л�ȡ������
            Type objType = (Type)CacheUtil.GetCache(keyName);

            if (objType == null)
            {
                // ���û���ڻ������ҵ�����ʹ��Ĭ������,��д�뻺��
                objType = Type.GetType(typeName, true);
                CacheUtil.SetCache(keyName, objType);
            }

            return objType;
        }


        /// <summary>
        /// ��ȡ����ֵ
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropertyValue(object obj, string propertyName)
        {
            // Լ0.8΢�� string��죬int����,Date����
            return obj.GetType().InvokeMember(propertyName, BindingFlags.GetProperty, null, obj, null);
        }


        /// <summary>
        /// ��������ֵ
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

    }// �����
}// �����ռ����
