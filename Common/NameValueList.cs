using System;
using System.Collections.Generic;
using System.Text;

namespace SSharing.Ubtrip.Common.Entity
{
    public class NameValueList: System.Collections.DictionaryBase
    {
        /// <summary>
        /// ���ӵ������󵽼��϶�����
        /// </summary>	
        public virtual void Add(string key, string val)
        {
            this.Dictionary.Add(key, val);
        }

        /// <summary>
        /// �Ӽ��϶������Ƴ���������
        /// </summary>
        public virtual void Remove(string key)
        {
            this.Dictionary.Remove(key);
        }

        /// <summary>
        /// �õ�ָ���ؼ��ֵĶ���
        /// </summary>
        public virtual string this[string key]
        {
            get { return (string)Dictionary[key]; }
            set { Dictionary[key] = value; }
        }

        /// <summary>
        /// �ж��Ƿ����ָ���ؼ��ֵĵ�������
        /// </summary>
        public virtual bool Contains(string key)
        {
            return this.Dictionary.Contains(key);
        }	
    }
}
