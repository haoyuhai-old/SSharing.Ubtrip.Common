using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using ZhiBen.Framework.DataAccess;
using System.Data;
using System.IO;

namespace SSharing.Ubtrip.Common.Util
{
    public static class LogUtil
    {
        static LogLevel _LogLevel;

        static LogUtil()
        {
            _LogLevel = GetLogLevelInner();
        }

        /// <summary>
        /// 记录日志到数据库中,只适用于后台的日志记录, UI层的日志记录应调用WebUtil.LogInfoWrite()。
        /// </summary>
        public static void Write(string message, LogLevel level)
        {
            Write(message, level, "SYS");
        }

        public static void WriteData(object data, LogLevel level)
        {
            WriteData(data, level, "SYS");
        }               
        
        /// <summary>
        /// 记录日志到数据库中(将打开新的连接，不使用上下文事务),只适用于后台的日志记录, UI层的日志记录应调用WebUtil.LogInfoWrite()。
        /// </summary>
        public static void Write(string message, LogLevel level, string functionCode)
        {
            WriteData(message, level, functionCode);
        }

        /// <summary>
        /// 记录日志到数据库中(将打开新的连接，不使用上下文事务),只适用于后台的日志记录, UI层的日志记录应调用WebUtil.LogInfoWrite()。
        /// </summary>
        public static void WriteData(object data, LogLevel level, string functionCode)
        {
            try
            {
                LogLevel allowedLevels = _LogLevel;

                //只记录与配置相匹配的级别
                if ((allowedLevels & level) == level)
                {
                    string message = data != null ? data.ToString() : string.Empty;
                    message = message.Replace("'", "''");

                    //File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"), DateTime.Now.ToString() + message);

                    //这里直接获取底层的数据库对象，以执行独立的sql语句（独立于外部的上下文事务）
                    DbManager.GetDbInstance().GetDatabase().ExecuteNonQuery(CommandType.Text, string.Format(
                         @"INSERT INTO dbo.t_sys_logs
                           (created_by
                           ,creation_date
                           ,last_update_date
                           ,last_updated_by
                           ,login_id
                           ,enabled_flag
                           ,employee_id
                           ,function_code
                           ,log_type
                           ,operation_date
                           ,operation_desc
                           ,ip_address)
                     VALUES
                           (0
                           ,getdate()
                           ,getdate()
                           ,0
                           ,0
                           ,'Y'
                           ,0
                           ,'{2}'
                           ,'{0}'
                           ,getdate()
                           ,'{1}'
                           ,'')", level.ToString(), message, functionCode));
                }
            }
            catch
            {
                //防止写日志发生错误导致影响业务流程

            }            
        }

        public static void Write(string message, LogLevel level, string functionCode,long UserId)
        {
            LogLevel allowedLevels = _LogLevel;

            string isOk = "成功";
            if (level == LogLevel.Error)
            {
                isOk = "失败";
            }
            //只记录与配置相匹配的级别
            if ((allowedLevels & level) == level)
            {
                if (message != null) message = message.Replace("'", "''");

                //File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"), DateTime.Now.ToString() + message);

                //这里直接获取底层的数据库对象，以执行独立的sql语句（独立于外部的上下文事务）
                DbManager.GetDbInstance().GetDatabase().ExecuteNonQuery(CommandType.Text, string.Format(
                     @"INSERT INTO dbo.t_sys_logs
                           (created_by
                           ,creation_date
                           ,last_update_date
                           ,last_updated_by
                           ,login_id
                           ,enabled_flag
                           ,employee_id
                           ,function_code
                           ,log_type
                           ,operation_date
                           ,operation_desc
                           ,ip_address
                           ,module_code)
                     VALUES
                           ({3}
                           ,getdate()
                           ,getdate()
                           ,{3}
                           ,{3}
                           ,'Y'
                           ,{3}
                           ,'{2}'
                           ,'{0}'
                           ,getdate()
                           ,'{1}'
                           ,''
                           ,'{4}')", level.ToString(), message, functionCode, UserId, isOk));
            }
        }

        public static LogLevel GetLogLevel()
        {
            return _LogLevel;
        }

        private static LogLevel GetLogLevelInner()
        {
            LogLevel allowedLevels;
            string logLevelStr = ConfigurationManager.AppSettings["LogLevel"];
            if (string.IsNullOrEmpty(logLevelStr))
            {
                allowedLevels = LogLevel.Error;
            }
            else
            {
                allowedLevels = (LogLevel)Enum.Parse(typeof(LogLevel), logLevelStr);
            }
            return allowedLevels;
        }
    }

    [Flags]
    public enum LogLevel
    {
        /// <summary>
        /// 严重错误
        /// </summary>
        Critical = 1,

        /// <summary>
        /// 错误
        /// </summary>
        Error = 3,

        /// <summary>
        /// 警告
        /// </summary>
        Warn = 7,

        /// <summary>
        /// 提示信息
        /// </summary>
        Info = 15,

        /// <summary>
        /// 调试详细信息
        /// </summary>
        Verbose = 31,

        All = -1,

        Off = 0,
    }

    /// <summary>
    /// 自动计时跟踪类, 该类的对象将记录创建至释放时的性能日志(以LogLevel.Verbose级别)
    /// </summary>
    public class Tracer : IDisposable
    {
        private const string TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

        private string _TracerName;
        private DateTime _StartTime;
        private Stopwatch _Watch;

        /// <summary>
        /// 构造一个自动计时对象，在创建及释放对象时将记录性能日志
        /// </summary>
        public Tracer(string functionCode)
        {
            _TracerName = functionCode;
            _StartTime = DateTime.Now;            
            _Watch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _Watch.Stop();
            LogUtil.Write(string.Format("Tracer {0} Start at {1}, Stop at {2}, Eclapsed {3}ms", _TracerName, _StartTime.ToString(TIME_FORMAT), DateTime.Now.ToString(TIME_FORMAT), _Watch.ElapsedMilliseconds), LogLevel.Verbose, _TracerName);
        }
    }
}
