using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SSharing.Ubtrip.Common.Sql
{
    class ColumnMetaData
    {
        public Type PropertyType { get; set; }
        public DynamicPropertyAccessor PropertyAccessor { get; set; }

        public string ColumnName { get; set; }
        public Category ColumnCategory { get; set; }
    }
}
