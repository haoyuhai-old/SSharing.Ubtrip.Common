using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Xml;

using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;

namespace ZhiBen.Framework.DataAccess
{
    public static class SqlMapper
    {
        // Fields
        private static readonly string _SqlMapperFile;
        private static readonly string _SqlMapperFileBaseDir;

        // Methods
        static SqlMapper()
        {
            string sqlMapFile = ConfigurationManager.AppSettings["SqlMapFile"];
            if (string.IsNullOrEmpty(sqlMapFile))
            {
                sqlMapFile = @"Config\SqlMap.config";
            }
            _SqlMapperFile = GetSqlMapFileAbsolutePath(sqlMapFile, AppDomain.CurrentDomain.BaseDirectory);
            _SqlMapperFileBaseDir = Path.GetDirectoryName(_SqlMapperFile);
        }

        private static Hashtable CreateSqlLookup()
        {
            //Hashtable sqlMapTable = new Hashtable(StringComparer.get_InvariantCultureIgnoreCase());
            Hashtable sqlMapTable = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
            ParseSqlMapFile(sqlMapTable, _SqlMapperFile);
            return sqlMapTable;
        }

        public static string GetSql(string key)
        {
            Hashtable sqlLookup = GetSqlLookup();
            if (!sqlLookup.ContainsKey(key))
            {
                throw new ArgumentException(string.Format("找不到{0}对应的SQL", key));
            }
            return (string)sqlLookup[key];
        }

        private static Hashtable GetSqlLookup()
        {
            ICacheManager cacheManager = CacheFactory.GetCacheManager();
            Hashtable data = (Hashtable)cacheManager.GetData("SqlMapper");
            if (data == null)
            {
                data = CreateSqlLookup();
                //cacheManager.Add("SqlMapper", data, 2, null, new ICacheItemExpiration[] { new FileDependency(_SqlMapperFile) });
                cacheManager.Add("SqlMapper", data,CacheItemPriority.Normal
                    , null, new ICacheItemExpiration[] { new FileDependency(_SqlMapperFile) });
            }
            return data;
        }

        private static string GetSqlMapFileAbsolutePath(string sqlMapFile, string baseDir)
        {
            if (Path.IsPathRooted(sqlMapFile))
            {
                return sqlMapFile;
            }
            return Path.Combine(baseDir, sqlMapFile);
        }

        private static void ParseSqlMapFile(Hashtable sqlMapTable, string sqlMapFile)
        {
            string sqlMapFileAbsolutePath = GetSqlMapFileAbsolutePath(sqlMapFile, _SqlMapperFileBaseDir);
            XmlDocument document = new XmlDocument();
            document.Load(sqlMapFileAbsolutePath);
            foreach (XmlNode node in document.SelectNodes("/SqlMap/MapItem"))
            {
                XmlAttribute attribute = node.Attributes["SqlMapSource"];
                if (attribute == null)
                {
                    string key = node.Attributes["Key"].Value;
                    string str3 = node.Attributes["Sql"].Value;
                    sqlMapTable.Add(key, str3);
                }
                else
                {
                    ParseSqlMapFile(sqlMapTable, attribute.Value);
                }
            }
        }
    }


}
