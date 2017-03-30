using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace SSharing.Ubtrip.Common.Sql
{
    /// <summary>
    /// 列名的集合，用来指定要更新的列.
    /// </summary>
    public class ColumnList
    {
        protected List<string> _ColumnNames = new List<string>();
        public List<string> GetColumnNames()
        {
            return _ColumnNames;
        }

        public ColumnList Add(string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentNullException("columnName");

            this._ColumnNames.Add(columnName);
            return this;
        }
    }

    /// <summary>
    /// 列名的集合，用来指定要更新的列.
    /// </summary>
    public class ColumnList<T> : ColumnList
    {
        public ColumnList()
        { 
        }

        public ColumnList(params Expression<GetProperty<T>>[] propertys)
        {
            foreach (Expression<GetProperty<T>> property in propertys)
            {
                this.Add(property);
            }
        }        

        public ColumnList<T> Add(Expression<GetProperty<T>> property)
        {
            return (ColumnList<T>)Add(DbService.GetColName<T>(property));
            DbMgr db = new DbMgr();
        }
    }
}
