using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace SSharing.Ubtrip.Common.Entity
{
	/// <summary>
    /// �� �� �ţ�
    /// �� �� �ƣ�
    /// ����ժҪ: ��ǿ�˷�ҳ������DataTable��
    ///			  ����ֻ�����ԣ�û�з�����
    /// ������ڣ�2006-11-16
	/// </summary>	
	public class PagesTable : BasePage
	{
        private DataTable _DataTable = null;


        /// <summary>
        /// ��ǰҳ��DataTable����
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
        /// ��ǰҳ��¼��
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
