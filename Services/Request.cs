using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace SSharing.Ubtrip.Common
{
    /// <summary>
    /// 作为所有WebService请求的基类
    /// </summary>
    [Serializable, DataContract]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class Request
    {
        private static string DefaultUserKey = string.Empty;
        private static string DefaultPassword = string.Empty;

        protected Request() : this(DefaultUserKey, DefaultPassword)
        {
        }

        protected Request(string userKey, string password)
        {
            this.Head = new RequestHead(userKey, password);
        }
    
        /// <summary>
        /// 请求的头信息
        /// </summary>
        [DataMember]
        public RequestHead Head { get; set; }

        /// <summary>
        /// 设置默认的请求认证信息
        /// </summary>
        public static void SetDefaultAuthenticateInfo(string defaultUserKey, string defaultPassword)
        {
            DefaultUserKey = defaultUserKey;
            DefaultPassword = defaultPassword;
        }

        public override string ToString()
        {
            return string.Format("RequestType:{0}", this.GetType().Name);
        }
    }
}
