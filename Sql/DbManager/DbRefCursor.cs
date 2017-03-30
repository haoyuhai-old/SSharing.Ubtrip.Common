using System;
using System.Collections.Generic;
using System.Text;

namespace ZhiBen.Framework.DataAccess
{
    public class DbRefCursor
    {
        // Properties
        public static DbRefCursor Value
        {
            get
            {
                return new DbRefCursor();
            }
        }
    }


}
