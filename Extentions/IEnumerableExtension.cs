using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSharing.Ubtrip.Common
{
    public static class IEnumerableExtension
    {
        /// <summary>
        /// 根据指定的键选择器函数，从 System.Collections.Generic.IEnumerable创建一个 System.Collections.Generic.Dictionary。当键值重复时只包含第一个值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TSource> ToDictionarySafe<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            Dictionary<TKey, TSource> result = new Dictionary<TKey, TSource>();
            foreach (var item in source)
            {
                TKey key = keySelector(item);
                if (key != null && !result.ContainsKey(key))
                {
                    result.Add(key, item);
                }
            }
            return result;
        }

        /// <summary>
        /// 从列表中获取分页的数据, curPage从1开始
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <returns></returns>
        public static List<T> TakePageList<T>(this IEnumerable<T> list, int pageSize, int curPage)
        {
            if (pageSize <= 0) throw new ArgumentOutOfRangeException("pageSize");
            if (curPage <= 0) throw new ArgumentOutOfRangeException("curPage");

            int startIndex = pageSize * (curPage - 1);
            int endIndex = startIndex + pageSize - 1;

            List<T> result = new List<T>();
            int index = 0;
            foreach (T item in list)
            {
                if (index >= startIndex && index <= endIndex)
                {
                    result.Add(item);
                }
                index++;
            }
            return result;
        }
    }
}
