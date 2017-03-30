using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicCache = SSharing.Ubtrip.Common.Cache.CacheManager;

namespace SSharing.Ubtrip.Common
{
    [Serializable]
    class DynamicCacheInner<TEntity>
    {
        public TEntity Item { get; set; }        
    }

    /// <summary>
    /// 单个对象动态缓存的实现基类
    /// </summary>
    /// <typeparam name="TEntity">缓存的实体对象的类型</typeparam>    
    /// <typeparam name="TCache">缓存对象的类型(单实例)</typeparam>
    public abstract class ObjectDynamicCache<TEntity, TCache> : SingletonBase<TCache> 
        where TCache : ObjectDynamicCache<TEntity, TCache>
        where TEntity : class
    {
        private string _CacheKey = "DynamicCache_" + typeof(TCache).FullName;

        protected ObjectDynamicCache()
        {            
        }

        /// <summary>
        /// 获取缓存项
        /// </summary>
        public TEntity GetItem()
        {
            DynamicCacheInner<TEntity> cacheObj = GetCacheObj();
            return cacheObj.Item;
        }

        /// <summary>
        /// 清除缓存项，下次调用时将重新加载
        /// </summary>
        public void Refresh()
        {
            DynamicCache.Instance.Remove(_CacheKey);
        }

        private DynamicCacheInner<TEntity> GetCacheObj()
        {
            DynamicCacheInner<TEntity> cacheObj = DynamicCache.Instance.GetData<DynamicCacheInner<TEntity>>(_CacheKey);
            if (cacheObj == null)
            {
                cacheObj = new DynamicCacheInner<TEntity>();
                cacheObj.Item = LoadCacheItem();
                
                //加入动态缓存
                DynamicCache.Instance.Add(_CacheKey, cacheObj, DateTime.Now.Add(GetCacheDuration()));
            }

            return cacheObj;
        }

        protected abstract TEntity LoadCacheItem();        
        protected abstract TimeSpan GetCacheDuration();
    }
}
