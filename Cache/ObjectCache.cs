using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSharing.Ubtrip.Common
{
    /// <summary>
    /// 单个对象的静态缓存实现基类
    /// </summary>
    /// <typeparam name="TEntity">缓存的实体对象的类型</typeparam>
    /// <typeparam name="TCache">缓存对象的类型(单实例)</typeparam>
    public abstract class ObjectCache<TEntity, TCache> : SingletonBase<TCache> 
        where TCache : ObjectCache<TEntity, TCache>
        where TEntity : class
    {
        private volatile bool _Initialized = false;
        private object _Lock = new object();
       
        private TEntity _Item;        

        protected ObjectCache()
        {            
        }
        
        /// <summary>
        /// 获取缓存项
        /// </summary>
        public TEntity GetItem()
        {
            AssertCacheItemLoaded();

            //这里直接将缓存的对象返回，存在被更改的风险！
            return _Item;
        }

        /// <summary>
        /// 清空缓存，下次将重新加载
        /// </summary>
        public void Refresh()
        {
            lock (_Lock)
            {
                _Initialized = false;
                _Item = default(TEntity);
            }
        }

        protected void AssertCacheItemLoaded()
        {
            if (!_Initialized)
            {
                lock (_Lock)
                {
                    if (!_Initialized)
                    {
                        _Item = LoadCacheItem();                        
                        _Initialized = true;
                    }
                }
            }
        }

        protected abstract TEntity LoadCacheItem();
    }
}
