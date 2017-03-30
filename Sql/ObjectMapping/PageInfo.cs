using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSharing.Ubtrip.Common
{
    /// <summary>
    /// 表示分页信息: 每页记录数、当前第几页(从1开始)，排序列等
    /// </summary>
    public class PageInfo
    {
        public PageInfo(int pageSize, int curPage, string orderKey)
        {
            this.OrderKey = orderKey;
            this.PageSize = pageSize;
            this.CurPage = curPage;
        }

        public string OrderKey { get; set; }
        public int PageSize { get; set; }
        public int CurPage { get; set; }
    }
}
