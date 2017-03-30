using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSharing.Ubtrip.Common.Sql
{
    /// <summary>
    /// 指定枚举类型在数据库中保存的方式,如果不指定则按名称保存
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
    public class EnumStorageTypeAttribute : Attribute
    {
        public EnumStorageTypeAttribute()
        {
            this.StorageType = EnumStorageType.EnumName;
        }

        public EnumStorageType StorageType { get; set; }

        public EnumStorageTypeAttribute(EnumStorageType storageType)
        {
            this.StorageType = storageType;
        }

    }

    /// <summary>
    /// 枚举类型在数据库中保存的方式
    /// </summary>
    public enum EnumStorageType
    {
        /// <summary>
        /// 以枚举定义时的文本保存
        /// </summary>
        EnumName,

        /// <summary>
        /// 以枚举定义时的数值保存
        /// </summary>
        EnumValue,
    }
}
