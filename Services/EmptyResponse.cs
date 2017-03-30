using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace SSharing.Ubtrip.Common
{
    /// <summary>
    /// 表示一个空的服务响应(仅返回服务成功与否的消息)
    /// </summary>
    [Serializable, DataContract]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class EmptyResponse : Response
    {
        public static readonly EmptyResponse OK = new EmptyResponse();
    }
}
