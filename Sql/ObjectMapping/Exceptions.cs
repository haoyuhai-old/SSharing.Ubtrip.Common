using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace SSharing.Ubtrip.Common.Sql
{
    /// <summary>
    /// 表示在更新时，对象的版本已过期
    /// </summary>
    [Serializable]
    public class StaleObjectStateException : Exception
    {
        public StaleObjectStateException()
        {
        }

        public StaleObjectStateException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// 表示数据访问层执行时发生的异常
    /// </summary>
    [Serializable]
    public class SqlException : Exception
    {
        public string Sql { get; private set; }

        public SqlException()
        {
        }

        public SqlException(string message)
            : base(message)
        {
        }

        public SqlException(string message, string sql, Exception innerException)
            : base(message, innerException)
        {
            this.Sql = sql;
        }

        //public SqlException(string message, Exception innerException)
        //    : base(message, innerException)
        //{
        //}

        public override string ToString()
        {
            string message = this.Message;
            string str2 = base.GetType().ToString() + " (0x" + base.HResult.ToString("X8", CultureInfo.InvariantCulture) + ")";
            if (!string.IsNullOrEmpty(message))
            {
                str2 = str2 + ": " + message;
            }
            if (!string.IsNullOrEmpty(this.Sql))
            {
                str2 += " SQL:" + this.Sql;
            }
            Exception innerException = base.InnerException;
            if (innerException != null)
            {
                str2 = str2 + " ---> " + innerException.ToString();
            }
            if (this.StackTrace != null)
            {
                str2 = str2 + Environment.NewLine + this.StackTrace;
            }
            return str2;
        }
    }

}
