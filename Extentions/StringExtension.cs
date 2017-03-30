using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSharing.Ubtrip.Common
{
    public static class StringExtension
    {
        public static string TrimStart(this string target, string trimString)
        {
            string result = target;
            while (result.StartsWith(trimString))
            {
                result = result.Substring(trimString.Length);
            }

            return result;
        }

        public static string TrimEnd(this string target, string trimString)
        {
            string result = target;
            while (result.EndsWith(trimString))
            {
                result = result.Substring(0, result.Length - trimString.Length);
            }

            return result;
        }

        /// <summary>
        /// 当前的字符串是否为null或空？ eg: string s = null; if(s.IsNullOrEmpty()) { ...}
        /// </summary>
        public static bool IsNullOrEmpty(this String s)
        {
            return String.IsNullOrEmpty(s);
        }

        /// <summary>
        /// 当前的字符串是否即不为null也不为空？ eg: string s = null; if(s.IsNotNullOrEmpty()) { ...}
        /// </summary>
        public static bool IsNotNullOrEmpty(this String s)
        {
            return !String.IsNullOrEmpty(s);
        }

        public static double ParseDoubleOrDefault(this String s)
        {
            double d;
            if (double.TryParse(s, out d))
            {
                return d;
            }
            else
            {
                return 0;
            }
        }

        public static decimal ParseDecimalOrDefault(this String s)
        {
            decimal d;
            if (decimal.TryParse(s, out d))
            {
                return d;
            }
            else
            {
                return 0;
            }
        }

        public static int ParseIntOrDefault(this String s)
        {
            int d;
            if (int.TryParse(s, out d))
            {
                return d;
            }
            else
            {
                return 0;
            }
        }

        public static bool ParseBoolOrDefault(this String s)
        {
            if (s == null) return false;
            s = s.Trim();

            return string.Equals(s, "True", StringComparison.CurrentCultureIgnoreCase)
                || string.Equals(s, "Y", StringComparison.CurrentCultureIgnoreCase)
                || s == "1";            
        }

        public static DateTime ParseDateTimeOrDefault(this String s)
        {
            DateTime dt;
            if (DateTime.TryParse(s, out dt))
            {
                return dt;
            }
            else
            {
                return default(DateTime);
            }
        }
    }
}
