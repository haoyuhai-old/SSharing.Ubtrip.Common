using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSharing.Ubtrip.Common.Sql
{
    class PropertyMetaData
    {
        public Type PropertyType { get; set; }
        public DynamicPropertyAccessor PropertyAccessor { get; set; }
    }
}
