using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
                                                               
using SSharing.Ubtrip.Common.Util;

namespace SSharing.Ubtrip.Common.Sql
{
    //术语
    // .SQL:标准SQL语句(Ansi SQL)
    // .SQL语句:参数格式:#[参数编号:]参数名称#, 其中,
    //          参数编号范围必须在1..99之间;
    //          参数名称必须是a-z,a-z,0-9,以首字母开始,长度不能超过30;
    //          双#之间不能有空格类字符.
    //          另外,#作为参数起止标识,不能作为非sql参数字符使用.
    // .SQL参数: 对应类型为SqlParam,包括参数名称, 参数编号, 参数SQL等属性, 为SQL的重要组成元素, 在SQL中的格式为:#[参数编号:]参数名称#
    // .SqlStringBuilder: SQL构造器
    // .入口参数类: 
    // .标准参数: 带显式参数编号的SQL参数(显式编号范围1-99)
    // .属性参数: 不带显式参数编号的SQL参数(内部编号为0)
    // .NULL参数: 参数中的sql值有效，参数名称为空，用户SQL的最后一个SQL参数后面的字符串
    // .条件参数: 参数的sql值中包含参数值占位符，如:AND language={0}, 生成的SQL需要将占位符替换为参数值
    // .正则参数: SQL与正则表达式匹配的字符串



    //public class SqlParamType
    //{
    //    public const string PT_DATE = "dat";
    //    public const string PT_INT = "int";
    //    public const string PT_CHAR = "chr";
    //}

    public class SqlParam
    {
        public string ParamName;    // 参数名称
        public int ParamSeq;        // 参数编号
        public string ParamSql;     // 参数SQL
        public bool IsConditionParam; // 是否条件参数
        

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

        #region 构造函数
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



        #region 私有函数

        /// <summary>
        /// 设置SQL参数
        /// </summary>
        /// <param name="fullParamName">参数全名称,有基础类型、类属性和末尾sql等三种参数
        /// 基础类型格式: #[1-99]:ParamName#, 第一个字符表示参数顺序号,值范围数字1-99,第二个字符为冒号分隔符,后面是实际参数名
        /// 类属性格式:#ParamName#,ParamName就是实际参数名,必须与参数类的某个公开属性严格匹配
        /// 末尾sql格式:fullParamName为空字符串,此时只有ParamSql属性有意义
        /// </param>
        /// <param name="sql"></param>
        /// <param name="isConditionParam">是否条件参数</param>
        /// <returns>SQL参数</returns>
        private SqlParam SetSqlParam(string fullParamName, string sql, bool isConditionParam)
        {
            string paramName = "";
            int paramSeq = 0;
            int paramNameIndex;

            // 0.fullParamName=="": 表示最后一个参数,只有SQL有效,参数名和类型无效,参数编号为-1
            // 1.(fullParamName.IndexOf(":")<0):  表示基础类型,参数名中包含参数顺序号,格式: #[1-99]:ParamName#
            // 2.(fullParamName.IndexOf(":")>=0): 表示对象类,参数名中为类属性,格式: 类属性名
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

            // 条件参数：需要将参数字符转换为{0},简化ToString()的处理
            if (isConditionParam == true)
            {
                sql = " " + sql.Replace(fullParamName, "{0}");
            }

            // 返回参数
            return new SqlParam(paramName, paramSeq, sql, isConditionParam);

        }

        /// <summary>
        /// 构造SQL参数
        /// 1.参数按顺序存放在mSqlParams中.
        /// 2.参数名称,格式: #ParamName#, 如sql="select * from talbe1 where key=#param1# and language='ZHS'",则第一个参数名称="param1"
        /// 3.参数SQL, 参数的前缀SQL片段,对于上例,参数param1对应的sql为"select * from talbe1 where key="
        /// 4.最后一个[参数名称,参数SQL]=[""," and language='ZHS'"]
        /// </summary>
        private void BuildParameter()
        {
            int lastIndex = 0;

            // 应用SQL格式对应的正则表达式,获取所有有效的参数,并构建其参数结构
            Regex r = new Regex(mRegexPatern);
            MatchCollection mc = r.Matches(mSql);

            foreach (Match m in mc)
            {
                mSqlParams.Add(SetSqlParam(m.Value, mSql.Substring(lastIndex, m.Index - lastIndex), false));
                lastIndex = m.Index + m.Length;
            }

            // 末尾sql参数,参数名称="",只有sql有效
            mSqlParams.Add(SetSqlParam("", mSql.Substring(lastIndex), false));
        }


        /// <summary>
        /// 检验参数是否合理
        /// 建议在Debug模式下使用
        /// </summary>
        private void CheckSql()
        {

            // 检查参数数量是否正确匹配
            // 如果#的数量!=2*(参数数量-1)(末尾参数不计算),则检查不通过
            Regex rc = new Regex("#");
            MatchCollection mcc = rc.Matches(mSql);
            if (mcc.Count != 2 * (mSqlParams.Count - 1))
            {
                throw new Exception(string.Format(
                                      @"SQL参数解析异常.
                                      参数格式:#[参数编号:]参数名称#, 其中,
                                      参数编号范围必须在1..99之间;
                                      参数名称必须是a-z,A-Z,0-9,以首字母开始,长度不能超过30;
                                      双#之间不能有空格类字符.

                                      另外,#作为SQL参数起止标识,不能作为非SQL参数字符使用.{0}!=2*({1}-1)
                                      SQL:
                                      {2}", mcc.Count, mSqlParams.Count, mSql));
            }

            // 检查sql属性参数的数目
            // 如果SQL类未指定,但是存在属性参数,则抛出异常信息
            int c = 0;
            foreach (SqlParam p in mSqlParams)
            {
                if (p.ParamSeq == 0 && p.ParamName != "")
                {
                    // 属性参数的数量+1
                    c++;
                    if (mParamClass == null)
                    {
                        throw new Exception(
                            string.Format("入口参数类null异常.必须指定入口参数类,才能使用属性参数.ParamName={0}, SQL:{1}", 
                            p.ParamName, mSql));
                    }
                }
            }

            // 检查标准参数的有效性

            // 设置标准参数数量=总参数数量-属性参数数量-1(末尾参数)
            c = mSqlParams.Count - c - 1;

            List<int> li = new List<int>();
            foreach (SqlParam p in mSqlParams)
            {
                if (p.ParamSeq != 0)
                {
                    if (p.ParamSeq > c)
                    {
                        throw new Exception(string.Format("标准参数编号溢出异常.标准参数编号范围在1..99之间,且小于等于标准参数数量.{0}<={1}, SQL:{2}", p.ParamSeq, c, mSql));
                    }
                    if (li.IndexOf(p.ParamSeq) >= 0)
                    {
                        throw new Exception(string.Format("标准参数编号出现重复异常.标准参数编号不能重复.{0}, SQL:{1}", p.ParamSeq, mSql));
                    }
                    li.Add(p.ParamSeq);
                }
            }

        }



        #endregion


        #region 公有方法

        /// <summary>
        /// 增加条件SQL
        /// </summary>
        /// <param name="sql"></param>
        public void AddConditionSql(string sql)
        {
            mSql += " " + sql;

            // 应用SQL格式对应的正则表达式,获取所有有效的参数,并构建其参数结构
            Regex r = new Regex(mRegexPatern);
            MatchCollection mc = r.Matches(sql);

            if (mc.Count != 1)
            {
                throw new Exception(string.Format("条件参数异常.一条件sql有且只有一个参数.{0},SQL:{1}", sql, mSql));
            }

            mSqlParams.Add(SetSqlParam(mc[0].Value, sql, true));

        }

        /// <summary>
        /// 输出SQL语句
        /// 1.各sql参数对应实例化后拼起来就构成SqlString
        /// </summary>
        /// <param name="paramValues">第一个参数值必须是入口参数类对象,如果入口参数类为null,第一个参数填null.标准参数编号从1开始到9</param>
        /// <returns></returns>
        public string ToString(ISql isql, params object[] paramValues)
        {
            StringBuilder sql = new StringBuilder();
            object paramValue = null;
            
            // 1.标准参数(mSqlParams[i].ParamSeq > 0),参数值按顺序
            // 2.属性参数(mSqlParams[i].ParamSeq == 0),参数值按属性名称
            // 3.末尾参数(mSqlParams[i].ParamSeq == -1),无参数
            // 4.条件参数处理
            //   如果不是空参数值，需要将参数值替换参数sql字段的参数占位符{0}
            //   如果是空参数，则不作处理
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
            // 排序
            sql.Append(mOrderBy);

            return sql.ToString();
        }

        
        /// <summary>
        /// 获取Sql的返回结果集元素类型
        /// </summary>
        public Type ReturnClass
        {
            get
            {
                return mReturnClass;
            }
        }


        /// <summary>
        /// 增加排序SQL
        /// </summary>
        /// <param name="sql"></param>
        public void AddOrderBySql(string sql)
        {
            mOrderBy = " " + sql;
        }

        #endregion

    }
}
