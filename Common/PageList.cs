using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace SSharing.Ubtrip.Common.Entity
{
    
    /// <summary>
    /// 类 编 号：
    /// 类 名 称：
    /// 内容摘要: 公共数据集合类
    /// </summary>
    [Serializable]
    [DataContract]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PageList<T> : BasePage
    {

        #region 变量区

        private List<T> _PageListData = new List<T>();

        #endregion // 变量区 结束

        
        #region 方法区
        
        /// <summary>
        /// 增加单个对象到集合对象中
        /// </summary>
        /// <param name="val">需要添加到集合的对象</param>
        virtual public void Add(T val)
        {
            _PageListData.Add(val);
        }

        #endregion // 方法区 结束



        #region 属性区


        /// <summary>
        /// 数据列表
        /// </summary>
        [DataMember]
        public List<T> Data
        {
            get
            {
                return _PageListData;
            }
            set
            {
                _PageListData = value;
            }
        }
        
        /// <summary>
        /// 通过索引值得到单个对象
        /// </summary>
        /// <param name="index">索引号</param>
        /// <returns>范型对象</returns>
        virtual public T this[int index]
        {
            get
            {
                return _PageListData[index];
            }
        }

        /// <summary>
        /// 当前页记录数
        /// </summary>        
        virtual public int CurPageCount
        {
            get
            {
                return _PageListData.Count;
            }
            set
            {
                //throw new Exception("不应该设置CurPageCount");
            }
        }

        #endregion // 属性区 结束


    } // 类 PageList 结束
} // 命名空间 ZTE.Fol.Common.Entity 结束