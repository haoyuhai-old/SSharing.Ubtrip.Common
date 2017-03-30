using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace SSharing.Ubtrip.Common.Entity
{
    
    /// <summary>
    /// �� �� �ţ�
    /// �� �� �ƣ�
    /// ����ժҪ: �������ݼ�����
    /// </summary>
    [Serializable]
    [DataContract]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PageList<T> : BasePage
    {

        #region ������

        private List<T> _PageListData = new List<T>();

        #endregion // ������ ����

        
        #region ������
        
        /// <summary>
        /// ���ӵ������󵽼��϶�����
        /// </summary>
        /// <param name="val">��Ҫ��ӵ����ϵĶ���</param>
        virtual public void Add(T val)
        {
            _PageListData.Add(val);
        }

        #endregion // ������ ����



        #region ������


        /// <summary>
        /// �����б�
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
        /// ͨ������ֵ�õ���������
        /// </summary>
        /// <param name="index">������</param>
        /// <returns>���Ͷ���</returns>
        virtual public T this[int index]
        {
            get
            {
                return _PageListData[index];
            }
        }

        /// <summary>
        /// ��ǰҳ��¼��
        /// </summary>        
        virtual public int CurPageCount
        {
            get
            {
                return _PageListData.Count;
            }
            set
            {
                //throw new Exception("��Ӧ������CurPageCount");
            }
        }

        #endregion // ������ ����


    } // �� PageList ����
} // �����ռ� ZTE.Fol.Common.Entity ����