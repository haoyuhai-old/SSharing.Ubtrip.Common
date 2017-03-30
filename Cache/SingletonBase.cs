using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSharing.Ubtrip.Common
{
    /// <summary>
    /// Singleton实现的基类
    /// </summary>
    /// <typeparam name="T">必须具有无参构造函数，可以为私有的</typeparam>
    public abstract class SingletonBase<T>
    {
        /// <summary>
        /// 获取对象的全局唯一实例
        /// </summary>
        public static T Instance
        {
            get { return SingletonCreator.instance; }
        }

        class SingletonCreator
        {
            internal static readonly T instance = (T)Activator.CreateInstance(typeof(T), true);
        }
    }
}
