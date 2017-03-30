using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using SSharing.Ubtrip.Common.Util;
using System.Reflection;
using System.IO;


namespace SSharing.Ubtrip.Common.Sql
{
    public class SqlMap
    {
        public const string TAG_KEY = "Key";
        public const string TAG_SQL = "Sql";
        public const string TAG_PARAM_CLASS = "ParamClass";
        public const string TAG_RETURN_CLASS = "ReturnClass";
        public const string TAG_CONDITON_SQL = "ConditionSql";
        public const string TAG_SQL_MAP_SOURCE = "SqlMapSource";

        private static Dictionary<string, SqlStringBuilder> _Ssbs = new Dictionary<string, SqlStringBuilder>();

        static SqlMap()
        {
            try
            {
                string fileName = ConfigurationManager.AppSettings["SqlMapFile"];
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = @"Config\SqlMap.config";
                }

                ParseSqlMapFile(fileName);
            }
            catch (Exception ex)
            {
                LogUtil.Write(ex.ToString(), LogLevel.Error);
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 检查xml节点是否包含指定的key值
        /// </summary>
        /// <param name="xn"></param>
        /// <param name="keys"></param>
        private static void Check(XmlNode xn, params string[] keys)
        {
            foreach (string key in keys)
            {
                if (xn.Attributes[key] == null)
                {
                    throw new Exception(string.Format("{0}属性异常.缺少属性:{1}", xn.Name, key));
                }
            }
        }

        /// <summary>
        /// 解析SQL语句并生成对应的SQL构造器SqlStringBuilder
        /// 前置条件：xnItem!=null,并且xnItem满足SQL语句格式
        /// </summary>
        /// <param name="xnItem"></param>
        private static void ParseMapItem(XmlNode xnItem)
        {
            string key;
            string sql;
            Type paramClassType;
            Type returnClassType;
            SqlStringBuilder ssb;

            // 检查mapitem是否包含TAG_KEY,TAG_SQL
            Check(xnItem, TAG_KEY);

            // 获取SQL语句key,并检查key是否有效
            key = "$" + xnItem.Attributes[TAG_KEY].Value;
            if (_Ssbs.ContainsKey(key))
            {
                throw new Exception(string.Format("SQL关键字重复异常.{0}", key));
            }

            // 获取主体SQL语句
            if (xnItem.Attributes[TAG_SQL] != null)
            {
                sql = xnItem.Attributes[TAG_SQL].Value;
            }
            else
            {
                // SQL语句
                sql = StringUtil.Suppress(xnItem.FirstChild.Value);
            }

            // 获取入口参数类
            if (xnItem.Attributes[TAG_PARAM_CLASS] == null)
            {
                paramClassType = null;
            }
            else
            {
                paramClassType = ReflectUtil.GetType(xnItem.Attributes[TAG_PARAM_CLASS].Value);
            }

            // 获取返回参数类
            if (xnItem.Attributes[TAG_RETURN_CLASS] == null)
            {
                returnClassType = null;
            }
            else
            {
                returnClassType = ReflectUtil.GetType(xnItem.Attributes[TAG_RETURN_CLASS].Value);
            }

            // 创建SQL构造器
            ssb = new SqlStringBuilder(sql, paramClassType, returnClassType);
            //bool bHasCondition = false;
            foreach (XmlNode subXn in xnItem.ChildNodes)
            {
                if (subXn.Name == TAG_CONDITON_SQL)
                {
                    ssb.AddConditionSql(StringUtil.Suppress(subXn.InnerText));
                    //bHasCondition = true;
                }
            }
            //if (bHasCondition)
            //{
            //    string lstText = xnItem.LastChild.InnerText;
            //    if (lstText.Length > 0 && lstText.ToLower().IndexOf("order by") != -1)
            //    {
            //        ssb.AddOrderBySql(lstText);
            //    }
            //}

            // 将SQL构造器纳入缓存管理
            _Ssbs.Add(key, ssb);
        }

        /// <summary>
        /// 装载分支SQL
        /// </summary>
        private static void ParseSqlMapFile(string fileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\" + fileName);

            XmlNodeList nodes = xmlDoc.SelectNodes("/SqlMap/MapItem");
            foreach (XmlNode node in nodes)
            {
                XmlAttribute attribute = node.Attributes["SqlMapSource"];
                if (attribute != null)
                {
                    ParseSqlMapFile(attribute.Value);
                }
                else
                {
                    attribute = node.Attributes["SqlMapAssembly"];
                    if (attribute != null)
                    {
                        ParseEmbeddedSqlMapFile(attribute.Value);
                    }
                    else
                    {
                        try
                        {
                            ParseMapItem(node);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(string.Format("解析config文件{0}时出错:{1}", fileName, ex.Message), ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 解析嵌入到程序集中的资源文件(必须以.config结尾)
        /// </summary>
        private static void ParseEmbeddedSqlMapFile(string assemblyString)
        {
            if (string.IsNullOrWhiteSpace(assemblyString)) return;
            Assembly assembly;
            try
            {
                assembly = Assembly.Load(assemblyString);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("加载程序集{0}时出错:{1}", assemblyString, ex.Message), ex);
            }

            string[] resNames = assembly.GetManifestResourceNames();
            foreach (string resName in resNames)
            {
                if (resName != null && resName.EndsWith(".config"))
                {
                    try
                    {
                        Stream stream = assembly.GetManifestResourceStream(resName);
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(stream);
                        foreach (XmlNode node in xmlDoc.SelectNodes("/SqlMap/MapItem"))
                        {
                            ParseMapItem(node);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("解析程序集{0}中的config文件{1}时出错:{2}", assemblyString, resName, ex.Message), ex);
                    }
                }
            }
        }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        public static string GetSql(ISql isql, string key, params object[] paramValues)
        {
            if (_Ssbs.ContainsKey(key) == false)
            {
                throw new Exception(string.Format("SQL语句key异常.找不到key:{0}", key));
            }

            return _Ssbs[key].ToString(isql, paramValues);
        }

        /// <summary>
        /// 获取Sql的返回结果集元素类型
        /// </summary>
        public static Type GetSqlReturnClass(string key)
        {
            if (_Ssbs.ContainsKey(key) == false)
            {
                throw new Exception(string.Format("SQL语句key异常.找不到key:{0}", key));
            }

            return _Ssbs[key].ReturnClass;
        }
    }
}
