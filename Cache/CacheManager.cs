using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;

namespace SSharing.Ubtrip.Common.Cache
{
    /// <summary>
    /// ��ҵ���CacheManager�İ�װ��
    /// </summary>
    public class CacheManager /*: ICacheManager*/
    {
        #region ���캯��
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CacheManager"/> class.
        /// </summary>
        protected CacheManager()
        {
        }
        #endregion

        #region ICacheManager ʵ��

        public static readonly CacheManager Instance = new CacheManager();

        #endregion

        #region ʵ��ICacheManager
        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="obj">The obj.</param>
        public void Add(string key, Object obj)
        {
            Add(CacheManagerName.Defalut, key, obj);
        }

        /// <summary>
        /// ��ӻ����������ָ����ʱ�����
        /// </summary>
        public void Add(string key, object obj, DateTime absoluteTime)
        {
            Add(CacheManagerName.Defalut, key, obj, absoluteTime);
        }

        /// <summary>
        /// Adds the specified cache manager name.
        /// </summary>
        /// <param name="cacheManagerName">Name of the cache manager.</param>
        /// <param name="key">The key.</param>
        /// <param name="obj">The obj.</param>
        public void Add(string cacheManagerName, string key, Object obj)
        {
            Microsoft.Practices.EnterpriseLibrary.Caching.ICacheManager
                cacheManager = CacheFactory.GetCacheManager(cacheManagerName);

            cacheManager.Add(key, obj);
        }

        /// <summary>
        /// Adds the specified cache manager name.
        /// </summary>
        /// <param name="cacheManagerName">Name of the cache manager.</param>
        /// <param name="key">The key.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="duration">��������ʱ�䣬����Ϊ��λ</param>
        public void Add(string cacheManagerName, string key, Object obj, int slidingExpirationSeconds)
        {
            Microsoft.Practices.EnterpriseLibrary.Caching.ICacheManager
                cacheManager = CacheFactory.GetCacheManager(cacheManagerName);

            cacheManager.Add(
                key, obj, CacheItemPriority.Normal, null, new SlidingTime(TimeSpan.FromSeconds(slidingExpirationSeconds)));
        }

        /// <summary>
        /// ��ӻ����������ָ����ʱ�����
        /// </summary>
        public void Add(string cacheManagerName, string key, object obj, DateTime absoluteTime)
        {
            Microsoft.Practices.EnterpriseLibrary.Caching.ICacheManager
                cacheManager = CacheFactory.GetCacheManager(cacheManagerName);

            cacheManager.Add(key, obj, CacheItemPriority.Normal, null, new AbsoluteTime(absoluteTime));
        }

        public void Add(string cacheManagerName, string key, object obj, params SqlDependencyExpiration[] expirations)
        {
            Microsoft.Practices.EnterpriseLibrary.Caching.ICacheManager
                cacheManager = CacheFactory.GetCacheManager(cacheManagerName);

            cacheManager.Add(key, obj, CacheItemPriority.Normal, null, expirations);
        }

        //private SqlDependencyExpiration GetSqlDependencyExpiration(string sqlDependencyProcName, Dictionary<string, object> parameters)
        //{
        //    SqlDependencyExpiration expiration = new SqlDependencyExpiration(sqlDependencyProcName, parameters);
        //    //expiration.Expired += delegate
        //    //{
        //    //    MessageBox.Show("Cache has expired!");
        //    //};
        //    return expiration;
        //}

        /// <summary>
        /// Adds the specified cache manager name.
        /// </summary>
        /// <param name="cacheManagerName">Name of the cache manager.</param>
        /// <param name="key">The key.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="file">The file.</param>
        public void Add(string cacheManagerName, string key, Object obj, string file)
        {
            Microsoft.Practices.EnterpriseLibrary.Caching.ICacheManager
                cacheManager = CacheFactory.GetCacheManager(cacheManagerName);

            cacheManager.Add(key, obj, CacheItemPriority.Normal, null, new FileDependency(file));
        }

        /// <summary>
        /// Adds the specified cache manager name and never expired.
        /// </summary>
        /// <param name="cacheManagerName">Name of the cache manager.</param>
        /// <param name="key">The key.</param>
        /// <param name="obj">The obj.</param>
        public void AddNeverExpired(string cacheManagerName, string key, Object obj)
        {
            Microsoft.Practices.EnterpriseLibrary.Caching.ICacheManager
                cacheManager = CacheFactory.GetCacheManager(cacheManagerName);

            cacheManager.Add(
                key, obj, CacheItemPriority.Normal, null, new NeverExpired());
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(string key)
        {
            Remove(CacheManagerName.Defalut, key);
        }

        /// <summary>
        /// Removes the specified cache manager name.
        /// </summary>
        /// <param name="cacheManagerName">Name of the cache manager.</param>
        /// <param name="key">The key.</param>
        public void Remove(string cacheManagerName, string key)
        {
            Microsoft.Practices.EnterpriseLibrary.Caching.ICacheManager
                cacheManager = CacheFactory.GetCacheManager(cacheManagerName);
            cacheManager.Remove(key);
        }


        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public void Flush()
        {
            Flush(CacheManagerName.Defalut);
        }

        /// <summary>
        /// Flushes the specified cache manager name.
        /// </summary>
        /// <param name="cacheManagerName">Name of the cache manager.</param>
        public void Flush(string cacheManagerName)
        {
            Microsoft.Practices.EnterpriseLibrary.Caching.ICacheManager
                cacheManager = CacheFactory.GetCacheManager(cacheManagerName);
            cacheManager.Flush();
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Object GetData(string key)
        {
            return GetData(CacheManagerName.Defalut, key);
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T GetData<T>(string key)
        {
            return GetData<T>(CacheManagerName.Defalut, key);
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="cacheManagerName">Name of the cache manager.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Object GetData(string cacheManagerName, string key)
        {
            Microsoft.Practices.EnterpriseLibrary.Caching.ICacheManager
                cacheManager = CacheFactory.GetCacheManager(cacheManagerName);
            return cacheManager.GetData(key);
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="cacheManagerName">Name of the cache manager.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T GetData<T>(string cacheManagerName, string key)
        {
            Microsoft.Practices.EnterpriseLibrary.Caching.ICacheManager
                cacheManager = CacheFactory.GetCacheManager(cacheManagerName);
            T obj = (T)cacheManager.GetData(key);
            return obj;
        }


        #endregion
    }// �����
}// �����ռ����
