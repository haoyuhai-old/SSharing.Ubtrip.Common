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
                throw new ArgumentException(string.Format("��ͬ��ֵ{0}��Ԫ���Ѿ�����", key), "key");
            }
            CallContext.SetData(key, value);
        }

        private void CheckKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key", "��ֵ����Ϊnull");
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
