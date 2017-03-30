using System;
using System.Collections.Generic;
using System.Text;

namespace SSharing.Ubtrip.Common.Util
{
    /// <summary>
    /// ����������
    /// </summary>
    public class ParamUtil
    {
        private ParamUtil()
        {
        }

        /// <summary>
        /// ��ȡboolֵ
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
        /// ��ȡstringֵ
        /// </summary>
        public static string getstring(object val)
        {
            if (val == null)
                return "";
            else
                return val.ToString();
        }

        /// <summary>
        /// ��ȡlongֵ
        /// </summary>
        public static long getlong(object val)
        {
            if (val == null || val.ToString() == "")
                return 0;
            else
                return Convert.ToInt64(val);
        }

        /// <summary>
        /// ��ȡdoubleֵ
        /// </summary>
        public static double getdouble(object val)
        {
            if (val == null || val.ToString() == "")
                return 0;
            else
                return Convert.ToDouble(val);
        }

        /// <summary>
        /// ��ȡintֵ
        /// </summary>
        public static int getint(object val)
        {
            if (val == null || val.ToString() == "")
                return 0;
            else
                return Convert.ToInt32(val);
        }

        /// <summary>
        /// �õ�DateTimeֵ
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
        /// �õ�������ֵ
        /// </summary>
        /// <returns></returns>
        public static System.DateTime GetEmptyDatetime()
        {
            return DateTime.Parse("0001-1-1 0:00:00");
        }

        /// <summary>
        /// ��ȡ����ַ�����ʽ��12,345,678 �����ַ���
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
        /// ��ȡ����ַ�����ʽ��12,345,678 �����ַ���
        /// <para>isCent:��λΪ�֣�������ΪԪ</para>
        /// </summary>
        /// <returns></returns>
        public static string getAmtFormat(object val, bool isCent)
        {
            if (val == null || val.ToString() == "") return "0";
            
            return string.Format("{0:N}", Math.Round(decimal.Parse(val.ToString()) / (isCent ? 100 : 1), 2)).Replace(".00", string.Empty);
        }

        /// <summary>
        /// ת��Ϊ������λ��Ч���ֵİٷ���
        /// </summary>
        /// <returns></returns>
        public static string getPercentFormat(object val)
        {
            if (val == null || val.ToString() == ""|| val.ToString() == "0")
                return "0.00%";
            else
                return (Math.Round(decimal.Parse(val.ToString()), 4) * 100).ToString("0.00") + "%";
        }
    }// �����
}// �����ռ����

