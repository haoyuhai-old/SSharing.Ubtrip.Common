using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSharing.Ubtrip.Common
{
    /// <summary>
    /// 集合对象的静态缓存实现基类
    /// </summary>
    /// <typeparam name="TEntity">缓存的实体对象的类型</typeparam>
    /// <typeparam name="TKey">对象Key的类型</typeparam>
    public abstract class ObjectListCache<TEntity, TKey, TCache> : SingletonBase<TCache>
        where TCache : ObjectListCache<TEntity, TKey, TCache>
        where TEntity : class
    {
        private volatile bool _Initialized = false;
        private object _Lock = new object();
       
        private List<TEntity> _Items;
        private Dictionary<TKey, TEntity> _ItemsDic;

        protected ObjectListCache()
        {            
        }

        /// <summary>
        /// 根据指定的键值获取缓存项
        /// </summary>
        public virtual TEntity GetItem(TKey key)
        {
            if (key == null) return null;

            AssertCacheItemsLoaded();

            //线程不安全，为性能考虑，这里未进行线程同步！
            TEntity result;
            _ItemsDic.TryGetValue(key, out result);
            return result;
        }

        /// <summary>
        /// 获取所有的缓存项集合
        /// </summary>
        public virtual List<TEntity> GetAll()
        {
            AssertCacheItemsLoaded();

            //线程不安全，为性能考虑，这里未进行线程同步！
            return _Items.ToList();
        }

        /// <summary>
        /// 清空缓存，下次将重新加载
        /// </summary>
        public virtual void Refresh()
        {
            lock (_Lock)
            {
                _Initialized = false;
                _Items = null;
                _ItemsDic = null;
            }  
        }

        protected void AssertCacheItemsLoaded()
        {
            if (!_Initialized)
            {
                lock (_Lock)
                {
                    if (!_Initialized)
                    {
                        List<TEntity> items = LoadCacheItems();
                        
                        Dictionary<TKey, TEntity> dic = new Dictionary<TKey, TEntity>();
                        foreach (TEntity item in items)
                        {
                            TKey key = GetKey(item);
                            if (!dic.ContainsKey(key))
                            {
                                dic.Add(key, item);
                            }
                        }

                        _Items = items;
                        _ItemsDic = dic;
                        _Initialized = true;
                    }
                }
            }
        }

        protected abstract List<TEntity> LoadCacheItems();
        protected abstract TKey GetKey(TEntity item);           
    }
}
