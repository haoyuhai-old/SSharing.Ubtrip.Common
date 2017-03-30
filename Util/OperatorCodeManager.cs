using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZhiBen.Framework.Service.Common;

namespace SSharing.Ubtrip.Common.Util
{
    /// <summary>
    /// 表示一个运营商的上下文环境，在此上下文中可以获取运营商代码
    /// </summary>
    public sealed class OperatorContext : IDisposable
    {
        private const string OPT_CODE_CONTEXT_KEY = "OPT_CODE_CONTEXT_KEY";

        private bool m_IsInnerContext; //是否内部上下文

        /// <summary>
        /// 构造一个运营商的上下文环境，在此上下文中可以获取运营商代码。此构造函数必须在using块中调用。
        /// </summary>
        public OperatorContext(string optCode)
        {
            if (string.IsNullOrEmpty(optCode))
            {
                throw new ArgumentNullException("optCode", "运营商代码为空。");
            }

            if (IsInOperatorContext())
            {
                //如果是内部上下文，则什么也不做，嵌套时只有外部的有效
                m_IsInnerContext = true;
            }
            else
            {
                m_IsInnerContext = false;
                ServiceContext.Current.Add(OPT_CODE_CONTEXT_KEY, optCode);
            }
        }

        public void Dispose()
        {
            if (!m_IsInnerContext)
            {
                ServiceContext.Current.Remove(OPT_CODE_CONTEXT_KEY);
            }
        }

        /// <summary>
        /// 当前执行代码是否在某个运营商的上下文环境中执行？
        /// </summary>
        public static bool IsInOperatorContext()
        {
            return ServiceContext.Current.Contains(OPT_CODE_CONTEXT_KEY);
        }

        /// <summary>
        /// 如果当前执行代码在某个运营商的上下文环境中执行，则获取上下文中的运营商代码
        /// </summary>
        public static string GetOptCode()
        {
            if (IsInOperatorContext())
            {
                return (string)ServiceContext.Current[OPT_CODE_CONTEXT_KEY];
            }
            else
            {
                throw new Exception("当前上下文不包含运营商代码。");
            }
        }
    }
}
