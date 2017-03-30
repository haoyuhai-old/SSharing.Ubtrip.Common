using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SSharing.Ubtrip.Common
{
    /// <summary>
    /// 自定义ListItem，兼容System.Web.UI.WebControls.ListItem，因为它不支持序列化
    /// </summary>
    [Serializable]
    [DataContract]
    public class UbtripListItem
    {
        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public bool Selected { get; set; }
    }
}
