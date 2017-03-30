using System;
using System.Collections.Generic;
using System.Text;

namespace ZhiBen.Framework.Service.Common
{
    internal interface IContextContainer
    {
        // Methods
        void Add(string key, object value);
        bool Contains(string key);
        void Remove(string key);

        // Properties
        object this[string key] { get; set; }
    }


}
