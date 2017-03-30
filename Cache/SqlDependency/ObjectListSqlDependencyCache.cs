using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicCache = SSharing.Ubtrip.Common.Cache.CacheManager;
using SSharing.Ubtrip.Common.Cache;

namespace SSharing.Ubtrip.Common
{
    [Serializable]
    class SqlObjectListCacheInner<TEntity, TKey>
    {
        public List<TEntity> Items { get; set; }
        public Dictionary<TKey, TEntity> ItemsDic { get; set; }
    }

    /// <summary>
    /// 集合对象Sql依赖缓存的实现基类
    /// </summary>
    /// <typeparam name="TEntity">缓存的实体对象的类型</typeparam>
    /// <typeparam name="TKey">对象Key的类型</typeparam>
    /// <typeparam name="TCache">缓存对象的类型(单实例)</typeparam>
    public abstract class ObjectListSqlDependencyCache<TEntity, TKey, TCache> : SingletonBase<TCache>
        where TCache : ObjectListSqlDependencyCache<TEntity, TKey, TCache>
        where TEntity : class
    {
        private static readonly string _CacheKey = "SqlCache_" + typeof(TCache).FullName;

        /// <summary>
        /// 最后一次加载数据的时间
        /// </summary>
        public DateTime LastLoadTime { get; private set; }

        protected ObjectListSqlDependencyCache()
        {
        }

        /// <summary>
        /// 根据指定的键值获取缓存项
        /// </summary>
        public virtual TEntity GetItem(TKey key)
        {
            if (key == null) return null;

            SqlObjectListCacheInner<TEntity, TKey> cacheObj = GetCacheObj();

            TEntity result;
            cacheObj.ItemsDic.TryGetValue(key, out result);
            return result;
        }
        
        /// <summary>
        /// 获取所有的缓存项集合
        /// </summary>
        public virtual List<TEntity> GetAll()
        {
            SqlObjectListCacheInner<TEntity, TKey> cacheObj = GetCacheObj();
            return cacheObj.Items.ToList();
        }
                
        /// <summary>
        /// 清除缓存项，下次调用时将重新加载
        /// </summary>
        public virtual void Refresh()
        {
            DynamicCache.Instance.Remove(_CacheKey);
        }

        private SqlObjectListCacheInner<TEntity, TKey> GetCacheObj()
        {
            SqlObjectListCacheInner<TEntity, TKey> cacheObj = DynamicCache.Instance.GetData<SqlObjectListCacheInner<TEntity, TKey>>(_CacheKey);
            if (cacheObj == null)
            {
                List<TEntity> items = LoadCacheItems();
                if (items == null)
                    items = new List<TEntity>();

                cacheObj = new SqlObjectListCacheInner<TEntity, TKey>();
                cacheObj.Items = items;
                cacheObj.ItemsDic = GetItemsDic(items);

                //加入动态缓存
                //if (this._SqlDependencies == null)
                //{
                //    this._SqlDependencies = this.GetSqlDependencies();
                //}
                //if (this._SqlDependencies == null)
                //{
                //    throw new Exception("GetSqlDependencies is null");
                //}                
                SqlDependencyExpiration[] sqlDependencies = GetSqlDependencies();
                DynamicCache.Instance.Add(CacheManagerName.Defalut, _CacheKey, cacheObj, sqlDependencies);

                this.LastLoadTime = DateTime.Now;
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
        protected abstract SqlDependencyExpiration[] GetSqlDependencies();
    }
}
