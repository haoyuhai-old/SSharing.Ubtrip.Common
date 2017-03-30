using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace SSharing.Ubtrip.Common
{
    /// <summary>
    /// 表示WebService请求的头信息
    /// </summary>
    [Serializable, DataContract]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class RequestHead
    {
        public RequestHead(string userKey, string password)
        {
            this.UserKey = userKey;
            this.Password = password;
        }

        [DataMember]
        public string UserKey { get; set; }

        [DataMember]
        public string Password { get; set; }

        public override string ToString()
        {
            return string.Format("RequestHead: UserId->{0}, Password->{1}", UserKey, Password);
        }
    }
}
