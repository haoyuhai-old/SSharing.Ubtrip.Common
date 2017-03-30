using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
                                                               
using SSharing.Ubtrip.Common.Util;

namespace SSharing.Ubtrip.Common.Sql
{
    //����
    // .SQL:��׼SQL���(Ansi SQL)
    // .SQL���:������ʽ:#[�������:]��������#, ����,
    //          ������ŷ�Χ������1..99֮��;
    //          �������Ʊ�����a-z,a-z,0-9,������ĸ��ʼ,���Ȳ��ܳ���30;
    //          ˫#֮�䲻���пո����ַ�.
    //          ����,#��Ϊ������ֹ��ʶ,������Ϊ��sql�����ַ�ʹ��.
    // .SQL����: ��Ӧ����ΪSqlParam,������������, �������, ����SQL������, ΪSQL����Ҫ���Ԫ��, ��SQL�еĸ�ʽΪ:#[�������:]��������#
    // .SqlStringBuilder: SQL������
    // .��ڲ�����: 
    // .��׼����: ����ʽ������ŵ�SQL����(��ʽ��ŷ�Χ1-99)
    // .���Բ���: ������ʽ������ŵ�SQL����(�ڲ����Ϊ0)
    // .NULL����: �����е�sqlֵ��Ч����������Ϊ�գ��û�SQL�����һ��SQL����������ַ���
    // .��������: ������sqlֵ�а�������ֵռλ������:AND language={0}, ���ɵ�SQL��Ҫ��ռλ���滻Ϊ����ֵ
    // .�������: SQL��������ʽƥ����ַ���



    //public class SqlParamType
    //{
    //    public const string PT_DATE = "dat";
    //    public const string PT_INT = "int";
    //    public const string PT_CHAR = "chr";
    //}

    public class SqlParam
    {
        public string ParamName;    // ��������
        public int ParamSeq;        // �������
        public string ParamSql;     // ����SQL
        public bool IsConditionParam; // �Ƿ���������
        

        public SqlParam(string paramName, int paramSeq, string paramSql, bool isConditionParam)
        {
            ParamName = paramName;
            ParamSeq = paramSeq;
            ParamSql = paramSql;
            IsConditionParam = isConditionParam;
        }
    }


    public class SqlStringBuilder
    {
        private static string mRegexPatern = "#([0-9]{1,2}:)?([a-zA-Z_])[a-zA-Z_0-9]{0,29}#";

        private string mSql;
        private Type mParamClass;
        private List<SqlParam> mSqlParams;
        private Type mReturnClass;
        private string mOrderBy = string.Empty;

        #region ���캯��
        public SqlStringBuilder(string sql) : this(sql, null, null)
        {
        }

        public SqlStringBuilder(string sql, Type paramClass):this(sql, paramClass, null)
        {
        }

        public SqlStringBuilder(string sql, Type paramClass, Type returnClass)
        {
            mSqlParams = new List<SqlParam>();
            mSql = sql;
            mParamClass = paramClass;
            mReturnClass = returnClass;
            BuildParameter();
        }

        #endregion



        #region ˽�к���

        /// <summary>
        /// ����SQL����
        /// </summary>
        /// <param name="fullParamName">����ȫ����,�л������͡������Ժ�ĩβsql�����ֲ���
        /// �������͸�ʽ: #[1-99]:ParamName#, ��һ���ַ���ʾ����˳���,ֵ��Χ����1-99,�ڶ����ַ�Ϊð�ŷָ���,������ʵ�ʲ�����
        /// �����Ը�ʽ:#ParamName#,ParamName����ʵ�ʲ�����,������������ĳ�����������ϸ�ƥ��
        /// ĩβsql��ʽ:fullParamNameΪ���ַ���,��ʱֻ��ParamSql����������
        /// </param>
        /// <param name="sql"></param>
        /// <param name="isConditionParam">�Ƿ���������</param>
        /// <returns>SQL����</returns>
        private SqlParam SetSqlParam(string fullParamName, string sql, bool isConditionParam)
        {
            string paramName = "";
            int paramSeq = 0;
            int paramNameIndex;

            // 0.fullParamName=="": ��ʾ���һ������,ֻ��SQL��Ч,��������������Ч,�������Ϊ-1
            // 1.(fullParamName.IndexOf(":")<0):  ��ʾ��������,�������а�������˳���,��ʽ: #[1-99]:ParamName#
            // 2.(fullParamName.IndexOf(":")>=0): ��ʾ������,��������Ϊ������,��ʽ: ��������
            if (fullParamName == "")
            {
                paramSeq = -1;
            }
            else
            {
                paramNameIndex = fullParamName.IndexOf(":");
                if (paramNameIndex < 0)
                {
                    paramName = fullParamName.Substring(1, fullParamName.Length - 2);
                }
                else
                {
                    paramName = fullParamName.Substring(paramNameIndex + 1, fullParamName.Length - paramNameIndex - 2);
                    paramSeq = ParamUtil.getint(fullParamName.Substring(1, paramNameIndex - 1));
                }
            }

            // ������������Ҫ�������ַ�ת��Ϊ{0},��ToString()�Ĵ���
            if (isConditionParam == true)
            {
                sql = " " + sql.Replace(fullParamName, "{0}");
            }

            // ���ز���
            return new SqlParam(paramName, paramSeq, sql, isConditionParam);

        }

        /// <summary>
        /// ����SQL����
        /// 1.������˳������mSqlParams��.
        /// 2.��������,��ʽ: #ParamName#, ��sql="select * from talbe1 where key=#param1# and language='ZHS'",���һ����������="param1"
        /// 3.����SQL, ������ǰ׺SQLƬ��,��������,����param1��Ӧ��sqlΪ"select * from talbe1 where key="
        /// 4.���һ��[��������,����SQL]=[""," and language='ZHS'"]
        /// </summary>
        private void BuildParameter()
        {
            int lastIndex = 0;

            // Ӧ��SQL��ʽ��Ӧ��������ʽ,��ȡ������Ч�Ĳ���,������������ṹ
            Regex r = new Regex(mRegexPatern);
            MatchCollection mc = r.Matches(mSql);

            foreach (Match m in mc)
            {
                mSqlParams.Add(SetSqlParam(m.Value, mSql.Substring(lastIndex, m.Index - lastIndex), false));
                lastIndex = m.Index + m.Length;
            }

            // ĩβsql����,��������="",ֻ��sql��Ч
            mSqlParams.Add(SetSqlParam("", mSql.Substring(lastIndex), false));
        }


        /// <summary>
        /// ��������Ƿ����
        /// ������Debugģʽ��ʹ��
        /// </summary>
        private void CheckSql()
        {

            // �����������Ƿ���ȷƥ��
            // ���#������!=2*(��������-1)(ĩβ����������),���鲻ͨ��
            Regex rc = new Regex("#");
            MatchCollection mcc = rc.Matches(mSql);
            if (mcc.Count != 2 * (mSqlParams.Count - 1))
            {
                throw new Exception(string.Format(
                                      @"SQL���������쳣.
                                      ������ʽ:#[�������:]��������#, ����,
                                      ������ŷ�Χ������1..99֮��;
                                      �������Ʊ�����a-z,A-Z,0-9,������ĸ��ʼ,���Ȳ��ܳ���30;
                                      ˫#֮�䲻���пո����ַ�.

                                      ����,#��ΪSQL������ֹ��ʶ,������Ϊ��SQL�����ַ�ʹ��.{0}!=2*({1}-1)
                                      SQL:
                                      {2}", mcc.Count, mSqlParams.Count, mSql));
            }

            // ���sql���Բ�������Ŀ
            // ���SQL��δָ��,���Ǵ������Բ���,���׳��쳣��Ϣ
            int c = 0;
            foreach (SqlParam p in mSqlParams)
            {
                if (p.ParamSeq == 0 && p.ParamName != "")
                {
                    // ���Բ���������+1
                    c++;
                    if (mParamClass == null)
                    {
                        throw new Exception(
                            string.Format("��ڲ�����null�쳣.����ָ����ڲ�����,����ʹ�����Բ���.ParamName={0}, SQL:{1}", 
                            p.ParamName, mSql));
                    }
                }
            }

            // ����׼��������Ч��

            // ���ñ�׼��������=�ܲ�������-���Բ�������-1(ĩβ����)
            c = mSqlParams.Count - c - 1;

            List<int> li = new List<int>();
            foreach (SqlParam p in mSqlParams)
            {
                if (p.ParamSeq != 0)
                {
                    if (p.ParamSeq > c)
                    {
                        throw new Exception(string.Format("��׼�����������쳣.��׼������ŷ�Χ��1..99֮��,��С�ڵ��ڱ�׼��������.{0}<={1}, SQL:{2}", p.ParamSeq, c, mSql));
                    }
                    if (li.IndexOf(p.ParamSeq) >= 0)
                    {
                        throw new Exception(string.Format("��׼������ų����ظ��쳣.��׼������Ų����ظ�.{0}, SQL:{1}", p.ParamSeq, mSql));
                    }
                    li.Add(p.ParamSeq);
                }
            }

        }



        #endregion


        #region ���з���

        /// <summary>
        /// ��������SQL
        /// </summary>
        /// <param name="sql"></param>
        public void AddConditionSql(string sql)
        {
            mSql += " " + sql;

            // Ӧ��SQL��ʽ��Ӧ��������ʽ,��ȡ������Ч�Ĳ���,������������ṹ
            Regex r = new Regex(mRegexPatern);
            MatchCollection mc = r.Matches(sql);

            if (mc.Count != 1)
            {
                throw new Exception(string.Format("���������쳣.һ����sql����ֻ��һ������.{0},SQL:{1}", sql, mSql));
            }

            mSqlParams.Add(SetSqlParam(mc[0].Value, sql, true));

        }

        /// <summary>
        /// ���SQL���
        /// 1.��sql������Ӧʵ������ƴ�����͹���SqlString
        /// </summary>
        /// <param name="paramValues">��һ������ֵ��������ڲ��������,�����ڲ�����Ϊnull,��һ��������null.��׼������Ŵ�1��ʼ��9</param>
        /// <returns></returns>
        public string ToString(ISql isql, params object[] paramValues)
        {
            StringBuilder sql = new StringBuilder();
            object paramValue = null;
            
            // 1.��׼����(mSqlParams[i].ParamSeq > 0),����ֵ��˳��
            // 2.���Բ���(mSqlParams[i].ParamSeq == 0),����ֵ����������
            // 3.ĩβ����(mSqlParams[i].ParamSeq == -1),�޲���
            // 4.������������
            //   ������ǿղ���ֵ����Ҫ������ֵ�滻����sql�ֶεĲ���ռλ��{0}
            //   ����ǿղ�������������
            for (int i = 0; i < mSqlParams.Count; i++)
            {
                if (mSqlParams[i].ParamSeq != -1)
                {
                    if (mSqlParams[i].ParamSeq == 0)
                    {
                        paramValue = ReflectUtil.GetPropertyValue(paramValues[0], mSqlParams[i].ParamName);
                    }
                    else //if (mSqlParams[i].ParamSeq > 0)
                    {
                        paramValue = paramValues[mSqlParams[i].ParamSeq];
                    }


                    if (mSqlParams[i].IsConditionParam == true)
                    {
                        if (DbUtil.isnull(paramValue) == false)
                        {
                            sql.AppendFormat(mSqlParams[i].ParamSql, isql.ToString(paramValue));
                        }
                    }
                    else
                    {
                        sql.Append(mSqlParams[i].ParamSql).Append(isql.ToString(paramValue));
                    }
                }
                else
                {
                    sql.Append(mSqlParams[i].ParamSql);
                }
            }
            // ����
            sql.Append(mOrderBy);

            return sql.ToString();
        }

        
        /// <summary>
        /// ��ȡSql�ķ��ؽ����Ԫ������
        /// </summary>
        public Type ReturnClass
        {
            get
            {
                return mReturnClass;
            }
        }


        /// <summary>
        /// ��������SQL
        /// </summary>
        /// <param name="sql"></param>
        public void AddOrderBySql(string sql)
        {
            mOrderBy = " " + sql;
        }

        #endregion

    }
}
