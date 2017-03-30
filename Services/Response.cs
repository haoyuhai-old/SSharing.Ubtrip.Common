using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace SSharing.Ubtrip.Common
{
    /// <summary>
    /// 作为所有WebService响应的基类
    /// </summary>
    [Serializable, DataContract]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class Response
    {
        protected Response()
        {
            this.Head = new ResponseHead(true, string.Empty, string.Empty);
        }

        /// <summary>
        /// 响应的头信息
        /// </summary>
        [DataMember]
        public ResponseHead Head { get; set; }

        /// <summary>
        /// 服务调用是否成功
        /// </summary>
        [IgnoreDataMember]
        public bool Success
        {
            get { return this.Head.Success; }
            set { this.Head.Success = value; }
        }
                
        /// <summary>
        /// 错误代码
        /// </summary>
        [IgnoreDataMember]
        public string ErrorCode 
        {
            get { return this.Head.ErrorCode; }
            set { this.Head.ErrorCode = value; }
        }

        /// <summary>
        /// 错误描述
        /// </summary>
        [IgnoreDataMember]
        public string ErrorMessage
        {
            get { return this.Head.ErrorMessage; }
            set { this.Head.ErrorMessage = value; }
        }

        /// <summary>
        /// 获取性能记录项目列表
        /// </summary>
        [IgnoreDataMember]
        public List<PerformanceRecord> PerformanceRecords 
        {
            get { return this.Head.PerformanceRecords; }
        }

        public void AddPerformanceRecord(string operation, DateTime time)
        {
            AddPerformanceRecord(new PerformanceRecord(operation, time));
        }

        public void AddPerformanceRecord(PerformanceRecord performanceRecord)
        {
            this.Head.PerformanceRecords.Add(performanceRecord);
        }

        public void AddPerformanceRecords(IEnumerable<PerformanceRecord> performanceRecords)
        {
            this.Head.PerformanceRecords.AddRange(performanceRecords);
        }

        public override string ToString()
        {
            return string.Format("ResponseType:{0}: Success->{1}, ErrorCode->{2}, ErrorMessage->{3}", this.GetType().Name, Success, ErrorCode, ErrorMessage);
        }
    }
}
