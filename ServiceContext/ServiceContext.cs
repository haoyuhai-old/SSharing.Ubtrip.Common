using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;

namespace ZhiBen.Framework.Service.Common
{
    public class ServiceContext
    {
        // Fields
        private static ServiceContext _Current = new ServiceContext();

        // Methods
        public void Add(string key, object value)
        {
            this.ContextContainer.Add(key, value);
        }

        public bool Contains(string key)
        {
            return this.ContextContainer.Contains(key);
        }

        public void Remove(string key)
        {
            this.ContextContainer.Remove(key);
        }

        // Properties
        private IContextContainer ContextContainer
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return new WindowContextContainer();
                }
                return new WebContextContainer();
            }
        }

        public static ServiceContext Current
        {
            get
            {
                return _Current;
            }
        }

        public object this[string key]
        {
            get
            {
                return this.ContextContainer[key];
            }
            set
            {
                this.ContextContainer[key] = value;
            }
        }

        public long OrganizationId
        {
            get
            {
                if (!this.ContextContainer.Contains("org@ZhiBen.framework.context"))
                {
                    throw new InvalidOperationException("组织ID还未初始化。");
                }
                return (long)this.ContextContainer["org@ZhiBen.framework.context"];
            }
            set
            {
                this.ContextContainer.Add("org@ZhiBen.framework.context", value);
            }
        }

        public UserInfo User
        {
            get
            {
                return (this.ContextContainer["user@ZhiBen.framework.context"] as UserInfo);
            }
            set
            {
                this.ContextContainer.Add("user@ZhiBen.framework.context", value);
            }
        }
    }


}
