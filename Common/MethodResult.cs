using System;
using System.Collections.Generic;
using System.Text;


namespace SSharing.Ubtrip.Common
{
    /// <summary>
    /// 表示方法调用的结果, 包括是否调用成功以及相关的描述信息
    /// </summary>
    [Serializable]
    public class MethodResult
    {
        public static readonly MethodResult Success = new MethodResult(true, string.Empty);        
     
        public MethodResult(bool successful, string errorMsg)
        {
            m_Successful = successful;
            m_ErrorMsg = errorMsg;
            m_Data = null;
        }
        public MethodResult(bool successful, string errorMsg, object data)
        {
            m_Successful = successful;
            m_ErrorMsg = errorMsg;
            m_Data = data;
        }

        private bool m_Successful;
        /// <summary>
        /// 是否支付成功
        /// </summary>
        public bool Successful
        {
            get { return m_Successful; }
        }

        private string m_ErrorMsg;
        /// <summary>
        /// 错误的描述信息
        /// </summary>
        public string ErrorMsg
        {
            get { return m_ErrorMsg; }
        }

        private object m_Data;
        /// <summary>
        /// 返回的数据
        /// </summary>
        public object Data
        {
            get { return m_Data; }
        }
    }

    /// <summary>
    /// 表示具有两个指定类型属性的对象
    /// </summary>
    public class Pair<T1, T2>
    {
        public Pair(T1 first, T2 second)
        {
            m_First = first;
            m_Second = second;
        }

        private T1 m_First;
        public T1 First
        {
            get { return m_First; }
            set { m_First = value; }
        }

        private T2 m_Second;
        public T2 Second
        {
            get { return m_Second; }
            set { m_Second = value; }
        }
    }

}
