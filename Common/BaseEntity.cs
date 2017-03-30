using System;
using System.Collections.Generic;
using System.Text;
using SSharing.Ubtrip.Common.Sql;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SSharing.Ubtrip.Common.Entity
{
    [JsonObject(MemberSerialization.OptOut)]//�л���Newtonsoft.json��ʹ������û�м�[DataMember]���ֶ�Ҳ�����л�������֮ǰ��JavaScriptSerializer
    [Serializable, DataContract]
    public abstract class BaseEntity
    {
        #region ��׼�ֶ�

        private long mCreatedBy;                
        private System.DateTime mCreationDate;  
        private System.DateTime mLastUpdateDate;
        private long mLastUpdatedBy;                
        private long mLoginID;                  
        private string mEnabledFlag = string.Empty;            

        #endregion 

        #region ��׼����

        /// <summary>
        /// �������ƣ�CreatedBy
        /// �������ͣ�long
        /// ����ժҪ�������˱��
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
        /// �������ƣ�CreationDate
        /// �������ͣ�System.DateTime
        /// ����ժҪ������ʱ��
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
        /// �������ƣ�UpdateDate
        /// �������ͣ�System.DateTime
        /// ����ժҪ���޸�ʱ��
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
        /// �������ƣ�UpdatedBy
        /// �������ͣ�long
        /// ����ժҪ���޸��˱��
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
        /// �������ƣ�LoginId
        /// �������ͣ�long
        /// ����ժҪ���޸�ʱ�ĵ�¼LoginID
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
        /// �������ƣ�EnabledFlag
        /// �������ͣ�string
        /// ����ժҪ����Ч���(����Y/����N)
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
