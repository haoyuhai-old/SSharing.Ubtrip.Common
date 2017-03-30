using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace SSharing.Ubtrip.Common
{
    /// <summary>
    /// 表示一个空的请求参数(即服务不需要任何参数)
    /// </summary>
    [Serializable, DataContract]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class EmptyRequest : Request
    {
        public static readonly EmptyRequest Default = new EmptyRequest();
    }
}
