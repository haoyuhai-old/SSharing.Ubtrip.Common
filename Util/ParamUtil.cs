using System;
using System.Collections.Generic;
using System.Text;

namespace SSharing.Ubtrip.Common.Util
{
    /// <summary>
    /// 参数工具类
    /// </summary>
    public class ParamUtil
    {
        private ParamUtil()
        {
        }

        /// <summary>
        /// 获取bool值
        /// </summary>
        public static bool getbool(object val)
        {
            if (val == null)
                return false;
            else
            {
                if (val.ToString() == "1" || val.ToString().ToUpper() == "Y" || val.ToString().ToUpper() == "TRUE")
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 获取string值
        /// </summary>
        public static string getstring(object val)
        {
            if (val == null)
                return "";
            else
                return val.ToString();
        }

        /// <summary>
        /// 获取long值
        /// </summary>
        public static long getlong(object val)
        {
            if (val == null || val.ToString() == "")
                return 0;
            else
                return Convert.ToInt64(val);
        }

        /// <summary>
        /// 获取double值
        /// </summary>
        public static double getdouble(object val)
        {
            if (val == null || val.ToString() == "")
                return 0;
            else
                return Convert.ToDouble(val);
        }

        /// <summary>
        /// 获取int值
        /// </summary>
        public static int getint(object val)
        {
            if (val == null || val.ToString() == "")
                return 0;
            else
                return Convert.ToInt32(val);
        }

        /// <summary>
        /// 得到DateTime值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static System.DateTime getdatetime(object val)
        {
            try
            {
                if (val == null || val.ToString() == "")
                    return DateTime.Parse("0001-1-1 0:00:00");
                else
                    return DateTime.Parse(val.ToString());
            }
            catch
            {
                return DateTime.Parse("0001-1-1 0:00:00");
            }
        }

        /// <summary>
        /// 得到空日期值
        /// </summary>
        /// <returns></returns>
        public static System.DateTime GetEmptyDatetime()
        {
            return DateTime.Parse("0001-1-1 0:00:00");
        }

        /// <summary>
        /// 获取金额字符串格式：12,345,678 返回字符串
        /// </summary>
        /// <returns></returns>
        public static string getAmtFormat(object val)
        {
            if (val == null || val.ToString() == "")
            {
                return "0";
            }
            else
            {
                string str = string.Format("{0:N2}", decimal.Parse(val.ToString()));
                return str.Replace(".00", string.Empty);
            }
        }

        /// <summary>
        /// 获取金额字符串格式：12,345,678 返回字符串
        /// <para>isCent:金额单位为分，输出结果为元</para>
        /// </summary>
        /// <returns></returns>
        public static string getAmtFormat(object val, bool isCent)
        {
            if (val == null || val.ToString() == "") return "0";
            
            return string.Format("{0:N}", Math.Round(decimal.Parse(val.ToString()) / (isCent ? 100 : 1), 2)).Replace(".00", string.Empty);
        }

        /// <summary>
        /// 转换为保留两位有效数字的百分数
        /// </summary>
        /// <returns></returns>
        public static string getPercentFormat(object val)
        {
            if (val == null || val.ToString() == ""|| val.ToString() == "0")
                return "0.00%";
            else
                return (Math.Round(decimal.Parse(val.ToString()), 4) * 100).ToString("0.00") + "%";
        }
    }// 类结束
}// 命名空间结束

