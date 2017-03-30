using System;
using System.Collections.Generic;
using System.Text;

namespace SSharing.Ubtrip.Common.Util
{
    /// <summary>
    /// 关于数字比较，运算方面的辅助类
    /// </summary>
    public static class MathUtil
    {
        static double decision = 0.01;
        static decimal decimalDescison = 1m;

        #region 浮点数比较        
        /// <summary>
        /// 比较两个浮点值是否相等, 精度为0.01
        /// </summary>
        public static bool AreEqual(double amount1, double amount2)
        {
            return Math.Abs(amount1 - amount2) < decision;
        }

        /// <summary>
        /// 比较两个浮点值是否不相等, 精度为0.01
        /// </summary>
        public static bool NotEqual(double amount1, double amount2)
        {
            return !AreEqual(amount1, amount2);
        }

        /// <summary>
        /// 第一个浮点值是否大于第二个, 精度为0.01
        /// </summary>
        public static bool GreatThan(double amount1, double amount2)
        {
            return amount1 - amount2 > decision;
        }

        /// <summary>
        /// 第一个浮点值是否小于第二个, 精度为0.01
        /// </summary>
        public static bool LessThan(double amount1, double amount2)
        {
            return amount1 - amount2 < -decision;
        }

        /// <summary>
        /// 比较两个decimal是否相等, 精度为1
        /// </summary>
        public static bool AreEqual(decimal amount1, decimal amount2)
        {
            return Math.Abs(amount1 - amount2) < decimalDescison;
        }

        /// <summary>
        /// 比较两个decimal是否不相等, 精度为1
        /// </summary>
        public static bool NotEqual(decimal amount1, decimal amount2)
        {
            return !AreEqual(amount1, amount2);
        }
        #endregion

        #region 金额分配

        /// <summary>
        /// 将指定的金额进行n等份，不会丢失精度
        /// </summary>
        /// <param name="amount">金额</param>
        /// <param name="n">份数, n>=1</param>
        /// <returns>返回分配以后的结果，返回数组的长度等于n</returns>
        public static long[] Allocate(long amount, int n)
        {
            if (n <= 0) throw new ArgumentOutOfRangeException("n", "n必须大于0");

            long low = amount / n;
            long high = low + 1;

            long[] results = new long[n];
            int reaminder = (int)(amount % n);
            for (int i = 0; i < reaminder; i++)
            {
                results[i] = high;
            }
            for (int i = reaminder; i < n; i++)
            {
                results[i] = low;
            }
            return results;
        }

        /// <summary>
        /// 按指定的比例进行金额分配，不会丢失精度
        /// </summary>
        /// <param name="amount">将要进行分配的金额</param>
        /// <param name="ratios">分配的比例</param>
        /// <returns>返回分配后的结果，返回数组的长度等于传入的分配比例数组的长度</returns>
        public static long[] Allocate(long amount, long[] ratios)
        {
            //总分配点数
            long total = 0;
            for (int i = 0; i < ratios.Length; i++)
            {
                total += ratios[i];
            }

            long remainder = amount;
            long[] results = new long[ratios.Length];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = amount * ratios[i] / total;
                remainder -= results[i];
            }

            for (int i = 0; i < remainder; i++)
            {
                results[i] = results[i] + 1;
            }

            return results;
        }
        #endregion

        //public static bool LessThan(decimal d1, double d2)
        //{
        //    return LessThan(Convert.ToDouble(d1), d2);
        //}

        //public static bool GreatThan(double d1, decimal d2)
        //{
        //    return GreatThan(d1, Convert.ToDouble(d2));
        //}

        public static decimal ToDecimal(double d)
        {
            decimal result = Convert.ToDecimal(d);
            return decimal.Round(result, 2);
        }

        public static decimal ParseDecimal(string str)
        {
            decimal d;
            if (decimal.TryParse(str, out d))
            {
                return d;
            }
            else
            {
                return 0m;
            }
        }
    }
}
