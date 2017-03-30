using System;
using System.Collections.Generic;
using System.Text;
using ZhiBen.Framework.DataAccess;
using System.Data;

namespace SSharing.Ubtrip.Common
{
    /// <summary>
    /// 表示一个事务范围，其中的代码将在事务中执行
    /// </summary>
    public sealed class TransactionScope : IDisposable
    {
        private string m_DatabaseName;
        private bool m_IsInnerTransaction; //是否内部事务
        private bool m_Completed = false;
        
        public TransactionScope() : this(DbManager.DefaultDatabaseName)
        {
        }

        public TransactionScope(string databaseName) : this(databaseName, IsolationLevel.ReadCommitted)
        {            
        }

        public TransactionScope(string databaseName, IsolationLevel isolationLevel)
        {
            m_DatabaseName = databaseName;
            if (DbManager.IsInTransaction(m_DatabaseName))
            {
                //如果是内部事务，则什么也不做
                m_IsInnerTransaction = true;
            }
            else
            {
                m_IsInnerTransaction = false;
                DbManager.BeginTransaction(m_DatabaseName, isolationLevel);
            }
        }

        /// <summary>
        /// 指示事务范围内的操作都已成功完成
        /// </summary>
        public void Complete()
        {
            m_Completed = true;
        }

        /// <summary>
        /// 结束事务范围，如果操作都成功完成就提交事务，否则回滚事务
        /// </summary>
        public void Dispose()
        {
            //如果是内部事务，则什么也不做
            if (!m_IsInnerTransaction)
            {
                if (m_Completed)
                {
                    DbManager.Commit(m_DatabaseName);
                }
                else
                {
                    try
                    {
                        DbManager.Rollback(m_DatabaseName);
                    }
                    catch (InvalidOperationException)
                    {
                        //事务已提交或回滚。- 或 -连接已断开。
                    }
                }
            }
        }
    }
}
