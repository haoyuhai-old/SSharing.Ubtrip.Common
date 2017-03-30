using System;
using System.Collections.Generic;
using System.Text;

namespace SSharing.Ubtrip.Common.Cache
{
    /// <summary>
    /// 缓存服务的基本接口，定义缓存服务的基本功能
    /// </summary>
    public interface ICacheManager
    {
        void Add(string cacheManagerName, string key, Object obj, int duration);
        void Add(string cacheManagerName, string key, Object obj, string file);
        void AddNeverExpired(string cacheManagerName, string key, Object obj);
        void Add(string key, Object obj);
        void Add(string cacheManagerName, string key, Object obj);
        void Add(string key, object obj, DateTime absoluteTime);
        void Add(string cacheManagerName, string key, object obj, DateTime absoluteTime);
        void Flush();
        void Flush(string cacheManagerName);
        Object GetData(string key);
        Object GetData(string cacheManagerName, string key);
        T GetData<T>(string key);
        T GetData<T>(string cacheManagerName, string key);
        void Remove(string cacheManagerName, string key);
        void Remove(string key);
    }// 类结束
}// 命名空间结束
