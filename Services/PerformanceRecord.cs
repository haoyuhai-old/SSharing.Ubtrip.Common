using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace SSharing.Ubtrip.Common
{
    /// <summary>
    /// 性能测试记录项，表示在某个时间点发生了什么事情
    /// </summary>
    [Serializable, DataContract]
    public class PerformanceRecord
    {
        private const string TimeFormat = "HH:mm:ss.f";

        public PerformanceRecord()
        {
        }

        public PerformanceRecord(string operation, DateTime time)
        {
            this.Event = operation;
            this.Time = time;
        }

        /// <summary>
        /// 事件发生时间点描述
        /// </summary>
        [DataMember]
        public string Event { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        [DataMember]
        [ScriptIgnore]
        public DateTime Time { get; set; }

        public string TimeS
        {
            get { return Time.ToString(TimeFormat); }
        }
        
        public static string GetPerformanceRecordsString(IEnumerable<PerformanceRecord> records)
        {
            string str = "";
            if (records != null)
            {
                foreach (var item in records)
                {
                    str += item.ToString() + "|";
                }
            }
            return str;
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", this.Event, this.Time.ToString(TimeFormat));
        }
    }
}
