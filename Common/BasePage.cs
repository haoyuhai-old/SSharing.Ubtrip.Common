using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using SSharing.Ubtrip.Common.Util;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace SSharing.Ubtrip.Common.Entity
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DataContract]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BasePage : IPage
    {

        #region 变量区
        private int mCurPage;
        private int mPageSize;
        private int mTotalCount;

        #endregion // 变量区 结束

        #region 方法区

        /// <summary>
        /// 初始化页大小和当前页号
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        virtual public void SetPage(int pageSize, int curPage)
        {
            PageSize = pageSize;
            CurPage = curPage;
        }

        #endregion // 方法区 结束

        #region 属性区
        
        /// <summary>
        /// 当前页，页计数从1开始
        /// </summary>
        [DataMember]
        virtual public int CurPage
        {
            get
            {
                return mCurPage;
            }
            set
            {
                if (mCurPage < 0)
                    mCurPage = 1;
                else
                    mCurPage = value;
            }
        }

        /// <summary>
        /// 实际每页显示的记录数
        /// </summary>
        [DataMember]
        virtual public int PageSize
        {
            get
            {
                return mPageSize;
            }
            set
            {
                mPageSize = value;
            }
        }

        /// <summary>
        /// 总记录数
        /// </summary>
        [DataMember]
        virtual public int TotalCount
        {
            get
            {
                return mTotalCount;
            }
            set
            {
                mTotalCount = value;
                //if (CurPage > TotalPage)
                //{
                //    CurPage = TotalPage;
                //}
            }
        }

        /// <summary>
        /// 总页数。通过pageSize、totalCount计算得到
        /// </summary>
        [DataMember]
        virtual public int TotalPage
        {
            get
            {
                if (mPageSize > 0)
                {
                    return ((mTotalCount - 1) / mPageSize + 1);
                }
                else
                {
                    return 1;
                }
            }
            set
            {
                //throw new Exception("不应该设置TotalPage");
            }
        }

        /// <summary>
        /// 当前页起始记录
        /// </summary>
        [DataMember]
        virtual public int StartRecord
        {
            get
            {
                // 如果页记录数为-1,表示不分页
                if (PageSize == -1)
                {
                    return 1;
                }
                else
                {
                    return PageSize * (CurPage - 1) + 1;
                }
            }
            set
            {
                //throw new Exception("不应该设置StartRecord");
            }
        }

        /// <summary>
        /// 当前页截止记录
        /// </summary>
        [DataMember]
        virtual public int EndRecord
        {
            get
            {
                // 如果页记录数为-1,表示不分页
                if (PageSize == -1)
                {
                    return int.MaxValue;
                }
                else
                {
                    return StartRecord + PageSize - 1;
                }
            }
            set
            {
                //throw new Exception("不应该设置EndRecord");
            }
        }


        #endregion // 属性区 结束

    }
}
