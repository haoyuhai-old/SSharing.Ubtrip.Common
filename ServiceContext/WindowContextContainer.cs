using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ZhiBen.Framework.Service.Common
{
    internal class WindowContextContainer : IContextContainer
    {
        // Fields
        [ThreadStatic]
        private static IDictionary _Items;

        // Methods
        public WindowContextContainer()
        {
            if (_Items == null)
            {
                _Items = new Hashtable();
            }
        }

        public void Add(string key, object value)
        {
            _Items.Add(key, value);
        }

        public bool Contains(string key)
        {
            return _Items.Contains(key);
        }

        public void Remove(string key)
        {
            _Items.Remove(key);
        }

        // Properties
        public object this[string key]
        {
            get
            {
                return _Items[key];
            }
            set
            {
                _Items[key] = value;
            }
        }
    }


}
