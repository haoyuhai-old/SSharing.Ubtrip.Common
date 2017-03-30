using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicCache = SSharing.Ubtrip.Common.Cache.CacheManager;
using SSharing.Ubtrip.Common.Cache;

namespace SSharing.Ubtrip.Common
{
    [Serializable]
    class SqlObjectCacheInner<TEntity>
    {
        public TEntity Item { get; set; }
    }

    /// <summary>
    /// 单个对象Sql依赖缓存的实现基类
    /// </summary>
    /// <typeparam name="TEntity">缓存的实体对象的类型</typeparam>    
    /// <typeparam name="TCache">缓存对象的类型(单实例)</typeparam>
    public abstract class ObjectSqlDependencyCache<TEntity, TCache> : SingletonBase<TCache>
        where TCache : ObjectSqlDependencyCache<TEntity, TCache>
        where TEntity : class
    {
        private static readonly string _CacheKey = "SqlCache_" + typeof(TCache).FullName;

        /// <summary>
        /// 最后一次加载数据的时间
        /// </summary>
        public DateTime LastLoadTime { get; private set; }

        protected ObjectSqlDependencyCache()
        {
        }

        /// <summary>
        /// 获取缓存项
        /// </summary>
        public TEntity GetItem()
        {
            SqlObjectCacheInner<TEntity> cacheObj = GetCacheObj();
            return cacheObj.Item;
        }

        /// <summary>
        /// 清除缓存项，下次调用时将重新加载
        /// </summary>
        public void Refresh()
        {
            DynamicCache.Instance.Remove(_CacheKey);
        }

        private SqlObjectCacheInner<TEntity> GetCacheObj()
        {
            SqlObjectCacheInner<TEntity> cacheObj = DynamicCache.Instance.GetData<SqlObjectCacheInner<TEntity>>(_CacheKey);
            if (cacheObj == null)
            {
                cacheObj = new SqlObjectCacheInner<TEntity>();
                cacheObj.Item = LoadCacheItem();

                //加入动态缓存
                DynamicCache.Instance.Add(CacheManagerName.Defalut, _CacheKey, cacheObj, this.GetSqlDependencies());

                this.LastLoadTime = DateTime.Now;
            }

            return cacheObj;
        }

        protected abstract TEntity LoadCacheItem();
        protected abstract SqlDependencyExpiration[] GetSqlDependencies();
    }
}
