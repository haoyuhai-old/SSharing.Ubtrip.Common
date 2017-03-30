using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSharing.Ubtrip.Common.Sql
{
    class MetaDataCacheManager
    {
        private static object _Lock = new object();
        private static Dictionary<Type, TableMetaData> _Cache = new Dictionary<Type, TableMetaData>();

        public static TableMetaData GetTableMetaData(Type entityType)
        {
            TableMetaData result;
            if (_Cache.TryGetValue(entityType, out  result))
            {
                return result;
            }

            lock (_Lock)
            {
                if (!_Cache.ContainsKey(entityType))
                {
                    TableMetaData tableMetaData = TableMetaData.ParseTableMetaData(entityType);
                    _Cache.Add(entityType, tableMetaData);
                }
                return _Cache[entityType];
            }
        }
    }
}
