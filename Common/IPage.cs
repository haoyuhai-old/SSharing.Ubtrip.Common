using System;
using System.Collections.Generic;
using System.Text;

namespace SSharing.Ubtrip.Common.Entity
{
    /// <summary>
    /// 接口编号：
    /// 接口名称：
    /// 内容摘要: 分页接口
    /// </summary>
    public interface IPage
    {
        /// <summary>
        /// 当前页，页计数从1开始
        /// </summary>
        int CurPage
        {
            get;
            set;
        }
        /// <summary>
        /// 实际每页显示的记录数
        /// </summary>
        int PageSize
        {
            get;
            set;
        }
        /// <summary>
        /// 总记录数
        /// </summary>
        int TotalCount
        {
            get;
            set;
        }
        /// <summary>
        /// 总页数。通过pageSize、totalCount计算得到
        /// </summary>
        int TotalPage
        {
            get;
        }

        /// <summary>
        /// 当前页起始记录
        /// </summary>
        int StartRecord
        {
            get;
        }

        /// <summary>
        /// 当前页截止记录
        /// </summary>
        int EndRecord
        {
            get;
        }


    }

}
