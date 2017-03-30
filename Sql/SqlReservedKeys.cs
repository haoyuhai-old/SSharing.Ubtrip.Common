using System;
using System.Collections.Generic;
using System.Text;

namespace SSharing.Ubtrip.Common.Sql
{

    /// <summary>
    /// 
    /// </summary>
    static public class SqlReservedKeys
    {
        /// <summary></summary>
        public const string TAB_PAGESQL = "tab_pagesql";
        /// <summary></summary>
        public const string COL_ROWNUM = "col_rownum";

        private static List<string> keys;

        /// <summary>
        /// 
        /// </summary>
        static SqlReservedKeys()
        {
            keys = new List<string>();
            keys.Add(TAB_PAGESQL);
            keys.Add(COL_ROWNUM);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ContainKey(string key)
        {
            return keys.Contains(key);
        }
    }
}
