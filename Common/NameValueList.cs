using System;
using System.Collections.Generic;
using System.Text;

namespace SSharing.Ubtrip.Common.Entity
{
    public class NameValueList: System.Collections.DictionaryBase
    {
        /// <summary>
        /// 增加单个对象到集合对象中
        /// </summary>	
        public virtual void Add(string key, string val)
        {
            this.Dictionary.Add(key, val);
        }

        /// <summary>
        /// 从集合对象中移除单个对象
        /// </summary>
        public virtual void Remove(string key)
        {
            this.Dictionary.Remove(key);
        }

        /// <summary>
        /// 得到指定关键字的对象
        /// </summary>
        public virtual string this[string key]
        {
            get { return (string)Dictionary[key]; }
            set { Dictionary[key] = value; }
        }

        /// <summary>
        /// 判断是否包含指定关键字的单个对象
        /// </summary>
        public virtual bool Contains(string key)
        {
            return this.Dictionary.Contains(key);
        }	
    }
}
