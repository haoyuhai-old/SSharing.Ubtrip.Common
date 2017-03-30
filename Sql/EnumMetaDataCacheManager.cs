using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSharing.Ubtrip.Common.Sql
{
    public class EnumMetaDataCacheManager
    {
        private static object _Lock = new object();
        private static Dictionary<Type, EnumStorageType> _Cache = new Dictionary<Type, EnumStorageType>();

        public static EnumStorageType GetEnumStorageType(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException("应为枚举类型", "enumType");

            EnumStorageType result;
            if (_Cache.TryGetValue(enumType, out  result))
            {
                return result;
            }

            lock (_Lock)
            {
                if (!_Cache.ContainsKey(enumType))
                {
                    EnumStorageType storageType = EnumStorageType.EnumName;

                    EnumStorageTypeAttribute[] attributes = (EnumStorageTypeAttribute[])enumType.GetCustomAttributes(typeof(EnumStorageTypeAttribute), false);
                    if (attributes.Length > 0)
                        storageType = attributes[0].StorageType;

                    _Cache.Add(enumType, storageType);
                }
                return _Cache[enumType];
            }
        }
    }
}
