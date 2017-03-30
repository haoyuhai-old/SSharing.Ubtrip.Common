using System;
using System.Collections.Generic;
using System.Text;

namespace SSharing.Ubtrip.Common.Entity
{
    /// <summary>
    /// �ӿڱ�ţ�
    /// �ӿ����ƣ�
    /// ����ժҪ: ��ҳ�ӿ�
    /// </summary>
    public interface IPage
    {
        /// <summary>
        /// ��ǰҳ��ҳ������1��ʼ
        /// </summary>
        int CurPage
        {
            get;
            set;
        }
        /// <summary>
        /// ʵ��ÿҳ��ʾ�ļ�¼��
        /// </summary>
        int PageSize
        {
            get;
            set;
        }
        /// <summary>
        /// �ܼ�¼��
        /// </summary>
        int TotalCount
        {
            get;
            set;
        }
        /// <summary>
        /// ��ҳ����ͨ��pageSize��totalCount����õ�
        /// </summary>
        int TotalPage
        {
            get;
        }

        /// <summary>
        /// ��ǰҳ��ʼ��¼
        /// </summary>
        int StartRecord
        {
            get;
        }

        /// <summary>
        /// ��ǰҳ��ֹ��¼
        /// </summary>
        int EndRecord
        {
            get;
        }


    }

}
