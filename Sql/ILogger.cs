using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSharing.Ubtrip.Common.Util;
using System.Data.Common;

namespace SSharing.Ubtrip.Common.Sql
{
    static class LogHelper
    {
        //public static void Log(DbCommand dbCommand)
        //{            
        //    LogUtil.WriteData(new DbCommandLogData(dbCommand), LogLevel.Verbose, "DbAccess");
        //}

        //public static void Log(string message)
        //{
        //    LogUtil.Write(message, LogLevel.Verbose, "DbAccess");
        //}

        internal static string GetSqlString(DbCommand dbCommand)
        {
            return new DbCommandLogData(dbCommand).ToString();
        }
    }

    class DbCommandLogData
    {
        public DbCommandLogData(DbCommand dbCommand)
        {
            m_DbCommand = dbCommand;
        }

        public DbCommand m_DbCommand { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}", m_DbCommand.CommandText);
            if (m_DbCommand.Parameters.Count > 0)
            {
                sb.AppendLine("Parameters:");
                foreach (DbParameter item in m_DbCommand.Parameters)
                {
                    sb.AppendFormat("[{0}]={1}, ", item.ParameterName, item.Value.ToString());
                }
            }
            return sb.ToString();
        }
    }
}
