using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSharing.Ubtrip.Common
{
    public static class MoneyUtil
    {
        /// <summary>
        /// 将以元为单位的金额转换为以分为单位
        /// </summary>
        public static long GetCents(double amountOfYuan)
        {
            return Convert.ToInt64(amountOfYuan * 100);
        }
                
        /// <summary>
        /// 将以元为单位的金额转换为以分为单位
        /// </summary>
        public static long GetCents(decimal amountOfYuan)
        {
            return Convert.ToInt64(amountOfYuan * 100);
        }

        /// <summary>
        /// 将以分为单位的金额转换为以元为单位
        /// </summary>
        public static double GetMoney(long cents)
        {
            return cents / 100.0;
        }

        /// <summary>
        /// 将以分为单位的金额转换为以元为单位
        /// </summary>
        public static decimal GetMoneyDecimal(long cents)
        {
            return Convert.ToDecimal(cents) / 100m;
        }

        /// <summary>
        /// 获取金额的默认表示形式（以元为单位，格式0.##）
        /// </summary>
        public static string GetString(long cents)
        {
            return (cents / 100.0).ToString("0.##");
        }

        /// <summary>
        /// 获取金额的默认表示形式（以元为单位，格式0.##）
        /// </summary>
        public static string GetStringOfDouble(double amountOfYuan)
        {
            return amountOfYuan.ToString("0.##");
        }
    }
}
