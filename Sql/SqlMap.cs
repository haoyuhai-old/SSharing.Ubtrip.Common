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
        /// ���xml�ڵ��Ƿ����ָ����keyֵ
        /// </summary>
        /// <param name="xn"></param>
        /// <param name="keys"></param>
        private static void Check(XmlNode xn, params string[] keys)
        {
            foreach (string key in keys)
            {
                if (xn.Attributes[key] == null)
                {
                    throw new Exception(string.Format("{0}�����쳣.ȱ������:{1}", xn.Name, key));
                }
            }
        }

        /// <summary>
        /// ����SQL��䲢���ɶ�Ӧ��SQL������SqlStringBuilder
        /// ǰ��������xnItem!=null,����xnItem����SQL����ʽ
        /// </summary>
        /// <param name="xnItem"></param>
        private static void ParseMapItem(XmlNode xnItem)
        {
            string key;
            string sql;
            Type paramClassType;
            Type returnClassType;
            SqlStringBuilder ssb;

            // ���mapitem�Ƿ����TAG_KEY,TAG_SQL
            Check(xnItem, TAG_KEY);

            // ��ȡSQL���key,�����key�Ƿ���Ч
            key = "$" + xnItem.Attributes[TAG_KEY].Value;
            if (_Ssbs.ContainsKey(key))
            {
                throw new Exception(string.Format("SQL�ؼ����ظ��쳣.{0}", key));
            }

            // ��ȡ����SQL���
            if (xnItem.Attributes[TAG_SQL] != null)
            {
                sql = xnItem.Attributes[TAG_SQL].Value;
            }
            else
            {
                // SQL���
                sql = StringUtil.Suppress(xnItem.FirstChild.Value);
            }

            // ��ȡ��ڲ�����
            if (xnItem.Attributes[TAG_PARAM_CLASS] == null)
            {
                paramClassType = null;
            }
            else
            {
                paramClassType = ReflectUtil.GetType(xnItem.Attributes[TAG_PARAM_CLASS].Value);
            }

            // ��ȡ���ز�����
            if (xnItem.Attributes[TAG_RETURN_CLASS] == null)
            {
                returnClassType = null;
            }
            else
            {
                returnClassType = ReflectUtil.GetType(xnItem.Attributes[TAG_RETURN_CLASS].Value);
            }

            // ����SQL������
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

            // ��SQL���������뻺�����
            _Ssbs.Add(key, ssb);
        }

        /// <summary>
        /// װ�ط�֧SQL
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
                            throw new Exception(string.Format("����config�ļ�{0}ʱ����:{1}", fileName, ex.Message), ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ����Ƕ�뵽�����е���Դ�ļ�(������.config��β)
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
                throw new Exception(string.Format("���س���{0}ʱ����:{1}", assemblyString, ex.Message), ex);
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
                        throw new Exception(string.Format("��������{0}�е�config�ļ�{1}ʱ����:{2}", assemblyString, resName, ex.Message), ex);
                    }
                }
            }
        }

        /// <summary>
        /// ��ȡSQL���
        /// </summary>
        public static string GetSql(ISql isql, string key, params object[] paramValues)
        {
            if (_Ssbs.ContainsKey(key) == false)
            {
                throw new Exception(string.Format("SQL���key�쳣.�Ҳ���key:{0}", key));
            }

            return _Ssbs[key].ToString(isql, paramValues);
        }

        /// <summary>
        /// ��ȡSql�ķ��ؽ����Ԫ������
        /// </summary>
        public static Type GetSqlReturnClass(string key)
        {
            if (_Ssbs.ContainsKey(key) == false)
            {
                throw new Exception(string.Format("SQL���key�쳣.�Ҳ���key:{0}", key));
            }

            return _Ssbs[key].ReturnClass;
        }
    }
}
