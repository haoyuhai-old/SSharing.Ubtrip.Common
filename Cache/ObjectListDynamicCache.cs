using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicCache = SSharing.Ubtrip.Common.Cache.CacheManager;

namespace SSharing.Ubtrip.Common
{
    [Serializable]
    class DynamicListCacheInner<TEntity, TKey>
    {
        public List<TEntity> Items { get; set; }
        public Dictionary<TKey, TEntity> ItemsDic { get; set; }
    }

    /// <summary>
    /// 集合对象动态缓存的实现基类
    /// </summary>
    /// <typeparam name="TEntity">缓存的实体对象的类型</typeparam>
    /// <typeparam name="TKey">对象Key的类型</typeparam>
    /// <typeparam name="TCache">缓存对象的类型(单实例)</typeparam>
    public abstract class ObjectListDynamicCache<TEntity, TKey, TCache> : SingletonBase<TCache> 
        where TCache : ObjectListDynamicCache<TEntity, TKey, TCache> 
        where TEntity : class 
    {
        private string _CacheKey = "DynamicCache_" + typeof(TCache).FullName;

        protected ObjectListDynamicCache()
        {            
        }

        /// <summary>
        /// 根据指定的键值获取缓存项
        /// </summary>
        public virtual TEntity GetItem(TKey key)
        {
            if (key == null) return null;

            DynamicListCacheInner<TEntity, TKey> cacheObj = GetCacheObj();

            TEntity result;
            cacheObj.ItemsDic.TryGetValue(key, out result);
            return result;
        }



        /// <summary>
        /// 获取所有的缓存项集合
        /// </summary>
        public virtual List<TEntity> GetAll()
        {
            DynamicListCacheInner<TEntity, TKey> cacheObj = GetCacheObj();
            return cacheObj.Items.ToList();
        }

        /// <summary>
        /// 清除缓存项，下次调用时将重新加载
        /// </summary>
        public virtual void Refresh()
        {
            DynamicCache.Instance.Remove(_CacheKey);
        }

        private DynamicListCacheInner<TEntity, TKey> GetCacheObj()
        {            
            DynamicListCacheInner<TEntity, TKey> cacheObj = DynamicCache.Instance.GetData<DynamicListCacheInner<TEntity, TKey>>(_CacheKey);
            if (cacheObj == null)
            {
                List<TEntity> items = LoadCacheItems();
                if (items == null) 
                    items = new List<TEntity>();

                cacheObj = new DynamicListCacheInner<TEntity, TKey>();
                cacheObj.Items = items;
                cacheObj.ItemsDic = GetItemsDic(items);
                
                //加入动态缓存
                DynamicCache.Instance.Add(_CacheKey, cacheObj, DateTime.Now.Add(GetCacheDuration()));
            }

            return cacheObj;
        }

        private Dictionary<TKey, TEntity> GetItemsDic(List<TEntity> items)
        {
            Dictionary<TKey, TEntity> dic = new Dictionary<TKey, TEntity>();
            foreach (var item in items)
            {
                TKey key = GetKey(item);
                if (!dic.ContainsKey(key))
                {
                    dic.Add(key, item);
                }
            }
            return dic;
        }

        protected abstract List<TEntity> LoadCacheItems();
        protected abstract TKey GetKey(TEntity item);
        protected abstract TimeSpan GetCacheDuration();
    }
}
