using System;
using System.Collections.Generic;
using System.Text;

namespace SSharing.Ubtrip.Common.Util
{
    /// <summary>
    /// 做为静态缓存使用
    /// 需要动态装载请使用cache缓存块
    /// </summary>
    public static class CacheUtil
    {
        private static Dictionary<string, object> _objs;
        private static object _lockObj;

        static CacheUtil()
        {
            _objs = new Dictionary<string, object>();
            _lockObj = new object();
        }

        /// <summary>
        /// 获取当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <returns></returns>
        public static object GetCache(string cacheKey)
        {
            //System.Web.Caching.Cache objCache = System.Web.HttpRuntime.Cache;
            //return objCache[cacheKey];
            if (_objs.ContainsKey(cacheKey) == true)
            {
                return _objs[cacheKey];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="objObject"></param>
        public static void SetCache(string cacheKey, object objObject)
        {
            //System.Web.Caching.Cache objCache = System.Web.HttpRuntime.Cache;
            //objCache.Insert(cacheKey, objObject);
            if (_objs.ContainsKey(cacheKey) == false)
            {
                lock (_lockObj)
                {
                    if (_objs.ContainsKey(cacheKey) == false)
                    {
                        _objs.Add(cacheKey, objObject);
                    }
                }
            }

        }

    }// 类结束
}// 命名空间结束
