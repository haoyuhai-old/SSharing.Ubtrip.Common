using System;
using System.Collections.Generic;
using System.Text;
using SSharing.Ubtrip.Common.Sql;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SSharing.Ubtrip.Common.Entity
{
    [JsonObject(MemberSerialization.OptOut)]//切换到Newtonsoft.json后，使子类中没有加[DataMember]的字段也被序列化，兼容之前的JavaScriptSerializer
    [Serializable, DataContract]
    public abstract class BaseEntity
    {
        #region 标准字段

        private long mCreatedBy;                
        private System.DateTime mCreationDate;  
        private System.DateTime mLastUpdateDate;
        private long mLastUpdatedBy;                
        private long mLoginID;                  
        private string mEnabledFlag = string.Empty;            

        #endregion 

        #region 标准属性

        /// <summary>
        /// 属性名称：CreatedBy
        /// 属性类型：long
        /// 内容摘要：创建人编号
        /// </summary>
        [DataMember]
        [Column("created_by", Category.ReadOnly)]
        public long CreatedBy
        {
            get
            {
                return mCreatedBy;
            }
            set
            {
                mCreatedBy = value;
            }
        }// end CreatedBy

        /// <summary>
        /// 属性名称：CreationDate
        /// 属性类型：System.DateTime
        /// 内容摘要：创建时间
        /// </summary>
        [Column("creation_date", Category.ReadOnly)]
        [DataMember]
        public DateTime CreationDate
        {
            get
            {
                return mCreationDate;
            }
            set
            {
                mCreationDate = value;
            }
        }// end CreationDate

        /// <summary>
        /// 属性名称：UpdateDate
        /// 属性类型：System.DateTime
        /// 内容摘要：修改时间
        /// </summary>
        [Column("last_update_date", Category.Version)]
        [DataMember]
        public DateTime LastUpdateDate
        {
            get
            {
                return mLastUpdateDate;
            }
            set
            {
                mLastUpdateDate = value;
            }
        }// end UpdateDate

        /// <summary>
        /// 属性名称：UpdatedBy
        /// 属性类型：long
        /// 内容摘要：修改人编号
        /// </summary>
        [Column("last_updated_by")]
        [DataMember]
        public long LastUpdatedBy
        {
            get
            {
                return mLastUpdatedBy;
            }
            set
            {
                mLastUpdatedBy = value;
            }
        }// end UpdatedBy

        /// <summary>
        /// 属性名称：LoginId
        /// 属性类型：long
        /// 内容摘要：修改时的登录LoginID
        /// </summary>
        [Column("login_id")]
        [DataMember]
        public long LoginId
        {
            get
            {
                return mLoginID;
            }
            set
            {
                mLoginID = value;
            }
        }// end LoginID

        /// <summary>
        /// 属性名称：EnabledFlag
        /// 属性类型：string
        /// 内容摘要：有效标记(启用Y/禁用N)
        /// </summary>
        [Column("enabled_flag")]
        [DataMember]
        public string EnabledFlag
        {
            get
            {
                return mEnabledFlag;
            }
            set
            {
                mEnabledFlag = value;
            }
        }

        #endregion
    }
}
