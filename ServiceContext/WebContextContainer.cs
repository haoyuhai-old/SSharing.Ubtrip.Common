using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Messaging;

namespace ZhiBen.Framework.Service.Common
{
    internal class WebContextContainer : IContextContainer
    {
        // Methods
        public void Add(string key, object value)
        {
            this.CheckKey(key);
            if (CallContext.GetData(key) != null)
            {
                throw new ArgumentException(string.Format("相同键值{0}的元素已经存在", key), "key");
            }
            CallContext.SetData(key, value);
        }

        private void CheckKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key", "键值不能为null");
            }
        }

        public bool Contains(string key)
        {
            this.CheckKey(key);
            return (CallContext.GetData(key) != null);
        }

        public void Remove(string key)
        {
            this.CheckKey(key);
            CallContext.SetData(key, null);
        }

        // Properties
        public object this[string key]
        {
            get
            {
                this.CheckKey(key);
                return CallContext.GetData(key);
            }
            set
            {
                this.CheckKey(key);
                CallContext.SetData(key, value);
            }
        }
    }


}
