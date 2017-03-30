using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ComponentModel;

namespace SSharing.Ubtrip.Common
{
    /// <summary>
    /// 表示WebService响应的头信息
    /// </summary>
    [Serializable, DataContract]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class ResponseHead
    {
        public ResponseHead(bool success, string errorCode, string errorMessage)
        {
            this.Success = success;
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
            this.PerformanceRecords = new List<PerformanceRecord>();
        }

        /// <summary>
        /// 是否调用成功
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        [DataMember]
        public string ErrorCode { get; set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 性能记录项目列表
        /// </summary>
        [DataMember]
        public List<PerformanceRecord> PerformanceRecords { get; set; }
        
        public override string ToString()
        {
            return string.Format("ResponseHead: Success:{0}, ErrorCode->{1}, ErrorMessage->{2}", Success, ErrorCode, ErrorMessage);
        }
    }
}
