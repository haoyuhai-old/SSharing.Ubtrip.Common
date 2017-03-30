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

        #region ������
        private int mCurPage;
        private int mPageSize;
        private int mTotalCount;

        #endregion // ������ ����

        #region ������

        /// <summary>
        /// ��ʼ��ҳ��С�͵�ǰҳ��
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        virtual public void SetPage(int pageSize, int curPage)
        {
            PageSize = pageSize;
            CurPage = curPage;
        }

        #endregion // ������ ����

        #region ������
        
        /// <summary>
        /// ��ǰҳ��ҳ������1��ʼ
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
        /// ʵ��ÿҳ��ʾ�ļ�¼��
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
        /// �ܼ�¼��
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
        /// ��ҳ����ͨ��pageSize��totalCount����õ�
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
                //throw new Exception("��Ӧ������TotalPage");
            }
        }

        /// <summary>
        /// ��ǰҳ��ʼ��¼
        /// </summary>
        [DataMember]
        virtual public int StartRecord
        {
            get
            {
                // ���ҳ��¼��Ϊ-1,��ʾ����ҳ
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
                //throw new Exception("��Ӧ������StartRecord");
            }
        }

        /// <summary>
        /// ��ǰҳ��ֹ��¼
        /// </summary>
        [DataMember]
        virtual public int EndRecord
        {
            get
            {
                // ���ҳ��¼��Ϊ-1,��ʾ����ҳ
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
                //throw new Exception("��Ӧ������EndRecord");
            }
        }


        #endregion // ������ ����

    }
}
