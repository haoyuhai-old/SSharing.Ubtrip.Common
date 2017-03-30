using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace SSharing.Ubtrip.Common.Util
{
    /// <summary>
    /// ���ݿ�����ת��������
    /// </summary>
    public class DbUtil
    {
        public static readonly string LANG_ZHS = "ZHS";
        public static readonly string LANG_US = "US";

        ///// <summary>
        ///// ���к�
        ///// </summary>
        ///// <param name="sequenceName"></param>
        ///// <returns></returns>
        //public static long GetSequenceNext(DbMgr db, string sequenceName)
        //{
        //    string sql = string.Format("SELECT {0}.NEXTVAL FROM DUAL", sequenceName);
        //    return ParamUtil.getlong(db.ExecuteScalar(sql));
        //}

        /// <summary>
        /// ����ת��
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string SqlLanguage(string culture)
        {
            if (culture == null || culture == "zh-CN" || culture == "")
            {
                return LANG_ZHS;
            }
            else
            {
                return LANG_US;
            }
        }

        /// <summary>
        /// �ж��Ƿ�Ϊnullֵ
        /// �ж�Ϊnull����
        /// 1.�ַ��� ==""
        /// 2.����==0
        /// �������Ͳ�����Ϊnull;
        /// </summary>
        /// <param name="val"></param>
        /// <returns>true:null, false:��null</returns>
        public static bool isnull(object val)
        {
            if ((val == null) ||
                (val is System.String && val.ToString() == "") ||
                ((val is System.Int32 || val is System.Int64) && val.ToString() == "0") ||
                ((val is DateTime) && (DateTime)val == ParamUtil.GetEmptyDatetime()) ||
                ((val is Decimal) && (Decimal)val == 0m) ||
                ((val is double) && Math.Abs((double)val) < 0.01))
            {
                return true;
            }

            // Ĭ��Ϊ��null
            return false;
        }

        /// <summary>
        /// ��valueת��Ϊ����t��ֵ
        /// �ӿ쵽�������� string -> long64 -> double -> int32 -> DateTime 
        /// �������ܿ���,��֧������5����֮���������������
        /// 100������У�ƽ��1��ʱ���������0.2΢��, �ٶ���if�����Ⱥ�˳���й�
        /// ���ͱȽϺ��ַ����Ƚϣ�ʱ����� ,���ǵ�cpu
        /// </summary>
        /// <param name="t"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object convert(Type t, object value)
        {
            if (t == typeof(String))
            {
                return (String)(value == DBNull.Value ? string.Empty : value);
            }
            else if (t == typeof(Int64))
            {
                return Convert.ToInt64(value == DBNull.Value ? 0 : value);
            }
            else if (t == typeof(Double))
            {
                return Convert.ToDouble(value == DBNull.Value ? 0 : value);
            }
            else if (t == typeof(Decimal))
            {
                return Convert.ToDecimal(value == DBNull.Value ? 0 : value);
            }
            else if (t == typeof(Int32))
            {
                return Convert.ToInt32(value == DBNull.Value ? 0 : value);
            }
            else if (t== typeof(Byte))
            {
                return Convert.ToByte(value == DBNull.Value ? 0 : value);
            }
            else if (t == typeof(DateTime))
            {
                return (DateTime)(value == DBNull.Value ? DateTime.MinValue : value);
                //if(value==null || value.ToString()==string.Empty)
                // {
                //     return System.DateTime.MinValue;
                // }                      
            }
            else if (t == typeof(Boolean))
            {
                return value == DBNull.Value ? false : value.ToString() == "1" || value.ToString() == "Y" || string.Equals(value.ToString(), "true", StringComparison.CurrentCultureIgnoreCase);
            }
            else if (t.BaseType == typeof(Enum))
            {
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    return Enum.Parse(t, value.ToString(), true);
                }
                else
                {
                    return Enum.Parse(t, "0");
                }
            }
            else
            {
                throw new Exception(string.Format("���ݿ������쳣.��֧������{0}.", t));
            }
        }

        public static object convert(string t, object value)
        {
            if (t == "System.String")
            {
                return (System.String)value;
            }
            else if (t == "System.Int64")
            {
                return Convert.ToInt64(value);
            }
            else if (t == "System.Double")
            {
                return Convert.ToDouble(value);
            }
            else if (t == "System.Decimal")
            {
                return Convert.ToDecimal(value);
            }
            else if (t == "System.Int32")
            {
                return Convert.ToInt32(value);
            }
            else if (t == "System.DateTime")
            {
                return (System.DateTime)value;
            }
            else
            {
                throw new Exception(string.Format("���ݿ������쳣.��֧������{0}.", t));
            }
        }

    }// �����
}// �����ռ����
