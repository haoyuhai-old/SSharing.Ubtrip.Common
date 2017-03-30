using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSharing.Ubtrip.Common.Util
{
    public class OptCodeAndOrderId
    {
        public OptCodeAndOrderId(string optCode, long orderId)
        {
            this.OptCode = optCode;
            this.OrderId = orderId;
        }

        public string OptCode { get; set; }

        public long OrderId { get; set; }
    }
}
