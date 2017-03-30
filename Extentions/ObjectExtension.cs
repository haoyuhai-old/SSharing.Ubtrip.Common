using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SSharing.Ubtrip.Common
{
    public static class ObjectExtension
    {
        /// <summary>
        /// 将对象序列化为Xml
        /// </summary>
        public static string ToXmlString(this object obj)
        {
            if(obj == null) return string.Empty;
                        
            using (StringWriter sw = new StringWriter())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());            
                xmlSerializer.Serialize(sw, obj);

                return sw.ToString();
            }            
        }

        /// <summary>
        /// 克隆一个新的对象(深拷贝,采用二进制格式化的方式)
        /// </summary>
        public static object Clone(this object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(ms, obj);
                ms.Position = 0;
                return bformatter.Deserialize(ms);
            }
        }

        public static List<T> ToList<T>(this string str, char split, Converter<string, T> convertHandler)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new List<T>();
            }
            else
            {
                string[] arr = str.Split(split);
                T[] tArr = Array.ConvertAll(arr, convertHandler);
                return new List<T>(tArr);
            }
        }
    }
}
