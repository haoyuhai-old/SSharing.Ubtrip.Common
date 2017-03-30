using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SSharing.Ubtrip.Common.Util
{
    /// <summary>
    /// 关于序列化相关的辅助方法
    /// </summary>
    public static class SerializeUtil
    {
        /// <summary>
        /// 对象的深克隆方法(使用BinaryFormatter的方式)
        /// </summary>
        public static object Clone(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(ms, obj);
                ms.Position = 0;
                return bformatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// 将对象二进制序列化为字节数组
        /// </summary>
        public static byte[] BinarySerialize(object obj)
        {
            if (obj == null)
            {
                return new byte[] { };
            }

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 将二进制数组反序列化为原来的对象
        /// </summary>
        public static object BinaryDeSerialize(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                return bformatter.Deserialize(ms);
            }
        }
    }
}
