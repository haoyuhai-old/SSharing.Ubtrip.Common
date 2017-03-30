using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SSharing.Ubtrip.Common.Util
{
    /// <summary>
    /// ����Ƚ�ʱʹ�õ���Ϣ��
    /// </summary>
    public struct SortInfo
    {
        /// <summary>
        /// �Ƚϵķ������£�
        /// ASC������
        /// DESC������
        /// </summary>
        public enum Direction
        {
            ASC = 0,
            DESC,
        };

        public enum Target
        {
            CUSTOMER = 0,
            FORM,
            FIELD,
            SERVER,
        };

        public string name;
        public Direction direction;
        public Target target;
    }
    /// <summary>
    /// �Զ��������ࣺ�̳�IComparer<T>�ӿڣ�ʵ��ͬһ�Զ������͡�����Ƚ�
    /// </summary>
    public class SortUtil<T> : IComparer<T>
    {
        private Type type = null;
        private SortInfo info;

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="type">���бȽϵ�������</param>
        /// <param name="name">���бȽ϶������������</param>
        /// <param name="direction">�ȽϷ���(����/����)</param>
        public SortUtil(Type type, string name, SortInfo.Direction direction)
        {
            this.type = type;
            this.info.name = name;
            if (direction != SortInfo.Direction.ASC)
                this.info.direction = direction;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="className">���бȽϵ�������</param>
        /// <param name="name">���бȽ϶������������</param>
        /// <param name="direction">�ȽϷ���(����/����)</param>
        public SortUtil(string className, string name, SortInfo.Direction direction)
        {
            try
            {
                this.type = Type.GetType(className, true);
                this.info.name = name;
                this.info.direction = direction;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="t">���бȽϵ����͵�ʵ��</param>
        /// <param name="name">���бȽ϶������������</param>
        /// <param name="direction">�ȽϷ���(����/����)</param>
        public SortUtil(T t, string name, SortInfo.Direction direction)
        {
            this.type = t.GetType();
            this.info.name = name;
            this.info.direction = direction;
        }

        /// <summary>
        /// ���룡ʵ��IComparer<T>�ıȽϷ�����
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        int IComparer<T>.Compare(T t1, T t2)
        {
            object x = this.type.InvokeMember(this.info.name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, t1, null);
            object y = this.type.InvokeMember(this.info.name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, t2, null);
            if (this.info.direction != SortInfo.Direction.ASC)
                Swap(ref x, ref y);
            return (new CaseInsensitiveComparer()).Compare(x, y);
        }

        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void Swap(ref object x, ref object y)
        {
            object temp = null;
            temp = x;
            x = y;
            y = temp;
        }

    }// �����
}// �����ռ����
