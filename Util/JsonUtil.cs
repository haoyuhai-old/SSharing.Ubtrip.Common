using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace SSharing.Ubtrip.Common.Util
{
    /// <summary>
    /// JSON对象加工类
    /// </summary>
    public static class JsonUtil
    {
        /// <summary>
        /// 转JSON字符串为对象
        /// </summary>
        /// <param name="typType">对象类型</param>
        /// <param name="strInput">JSON格式字符串</param>
        /// <param name="strFunCode">日志FunCode</param>
        /// <returns>返回--对象</returns>
        public static object doDeserialize(Type typType, string strInput, string strFunCode)
        {
            object objObject = null;
            try
            {
                JavaScriptSerializer jssScriptSerializer = new JavaScriptSerializer();
                jssScriptSerializer.MaxJsonLength = int.MaxValue;
                objObject = jssScriptSerializer.Deserialize(strInput, typType);
            }
            catch (Exception ex)
            {
                LogUtil.Write(ex.Message, LogLevel.Error, strFunCode);
                LogUtil.Write(ex.StackTrace, LogLevel.Error, strFunCode);
                throw new Exception(ex.Message);
            }

            return objObject;
        }

        /// <summary>
        /// 转对象为JSON字符串
        /// </summary>
        /// <param name="objObject">对象类型</param>
        /// <returns>返回--字符串</returns>
        public static string doSerialize(object objObject)
        {
            JavaScriptSerializer jssScriptSerializer = new JavaScriptSerializer();
            jssScriptSerializer.MaxJsonLength = int.MaxValue;
            return jssScriptSerializer.Serialize(objObject);
        }
    }
}
