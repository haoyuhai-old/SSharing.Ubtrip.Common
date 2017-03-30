using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace SSharing.Ubtrip.Common
{
    public static class ICommunicationObjectExtension
    {
        /// <summary>
        /// 安全地关闭当前对象(当状态为Faulted时调用Abort,否则调用Close)
        /// </summary>
        public static void CloseSafe(this ICommunicationObject communicationObject)
        {
            if (communicationObject.State == CommunicationState.Faulted)
                communicationObject.Abort();
            else
                communicationObject.Close();
        }
    }
}
