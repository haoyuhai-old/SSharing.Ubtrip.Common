using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace SSharing.Ubtrip.Common.Entity
{
	/// <summary>
    /// 类 编 号：
    /// 类 名 称：
    /// 内容摘要: 加强了分页参数的DataTable类
    ///			  该类只有属性，没有方法。
    /// 完成日期：2006-11-16
	/// </summary>	
	public class PagesTable : BasePage
	{
        private DataTable _DataTable = null;


        /// <summary>
        /// 当前页的DataTable数据
		/// </summary>
		public DataTable DataTable
		{
			get
			{
				return _DataTable;
			}
			set
			{
				_DataTable = value;
			}
		}


        /// <summary>
        /// 当前页记录数
        /// </summary>
        virtual public int CurPageCount
        {
            get
            {
                if (_DataTable != null)
                {
                    return _DataTable.Rows.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

	}

}
