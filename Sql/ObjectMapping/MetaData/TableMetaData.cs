using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SSharing.Ubtrip.Common.Sql
{
    class TableMetaData
    {
        #region 属性
        public Type EntityType { get; private set; }

        public string TableName { get; private set; }

        public ColumnMetaData PKColumn { get; private set; }

        public ColumnMetaData VersionColumn { get; private set; }

        public List<ColumnMetaData> NormalColumns { get; private set; }

        /// <summary>
        /// 属性集合，其中Key为大写形式的属性名或列名(如果指定了不同的列名，集合中将包含两项),用来在从数据库中读取数据时对属性赋值
        /// </summary>
        public Dictionary<string, PropertyMetaData> PropertiesDic { get; set; }
        #endregion

        /// <summary>
        /// 解析实体类型的元数据(允许不定义TableAttribute)
        /// </summary>
        public static TableMetaData ParseTableMetaData(Type entityType)
        {
            TableMetaData table = new TableMetaData();
            table.EntityType = entityType;

            //表名
            TableAttribute[] tableAttributes = (TableAttribute[])entityType.GetCustomAttributes(typeof(TableAttribute), false);
            if (tableAttributes.Length > 0)
                table.TableName = string.IsNullOrEmpty(tableAttributes[0].TableName) ? entityType.Name : tableAttributes[0].TableName;

            //列集合
            table.NormalColumns = new List<ColumnMetaData>();
            table.PropertiesDic = new Dictionary<string, PropertyMetaData>();
            foreach (PropertyInfo pi in entityType.GetProperties())
            {
                string piNameUpper = pi.Name.ToUpper();
                if (table.PropertiesDic.ContainsKey(piNameUpper))
                    throw new MappingException(string.Format("实体类型{0}中的属性名或列名{1}重复", entityType.Name, pi.Name));

                PropertyMetaData propertyData = new PropertyMetaData();
                propertyData.PropertyType = pi.PropertyType;
                propertyData.PropertyAccessor = new DynamicPropertyAccessor(pi);
                table.PropertiesDic.Add(piNameUpper, propertyData);

                ColumnAttribute[] columnAttributes = (ColumnAttribute[])pi.GetCustomAttributes(typeof(ColumnAttribute), false);
                if (columnAttributes.Length > 0)
                {
                    #region 解析Column
                    ColumnMetaData column = new ColumnMetaData();
                    column.PropertyType = pi.PropertyType;
                    column.PropertyAccessor = new DynamicPropertyAccessor(pi);
                    column.ColumnName = string.IsNullOrEmpty(columnAttributes[0].ColumnName) ? pi.Name : columnAttributes[0].ColumnName;
                    column.ColumnCategory = columnAttributes[0].ColumnCategory;

                    switch (column.ColumnCategory)
                    {
                        case Category.Normal:
                        case Category.ReadOnly:
                            table.NormalColumns.Add(column);
                            break;

                        case Category.IdentityKey:
                        case Category.Key:                            
                            if(table.PKColumn != null)
                                throw new MappingException(string.Format("实体类型{0}中定义了超过一个主键列:{1}和{2}", entityType.Name, table.PKColumn.ColumnName, column.ColumnName));

                            table.PKColumn = column;                            
                            break;

                        case Category.Version:
                            if(column.PropertyType != typeof(DateTime) && column.PropertyType != typeof(Int32) && column.PropertyType != typeof(Int64))
                                throw new MappingException(string.Format("实体类型{0}中的Version列{1}的数据类型无效", entityType.Name, column.ColumnName));
                            if(table.VersionColumn != null)
                                throw new MappingException(string.Format("实体类型{0}中定义了超过一个Version列:{1}和{2}", entityType.Name, table.VersionColumn.ColumnName, column.ColumnName));

                            table.VersionColumn = column;
                            break;

                        default:
                            throw new NotSupportedException("无效的ColumnCategory:" + columnAttributes[0].ColumnCategory);
                    }

                    //如果列名与属性名不同，则添加两个属性到dic中                    
                    if (!string.IsNullOrEmpty(columnAttributes[0].ColumnName))                    
                    {
                        string columnNameUpper = columnAttributes[0].ColumnName.ToUpper();
                        if (columnNameUpper != piNameUpper)
                        {
                            if (table.PropertiesDic.ContainsKey(columnNameUpper))
                                throw new MappingException(string.Format("实体类型{0}中的属性名或列名{1}重复", entityType.Name, columnAttributes[0].ColumnName));

                            table.PropertiesDic.Add(columnNameUpper, propertyData);
                        }
                    }
                    #endregion
                }
            }

            if (!string.IsNullOrEmpty(table.TableName) && table.PKColumn == null && table.NormalColumns.Count == 0)
                throw new MappingException(string.Format("实体类型:{0}中定义了TableAttribute,但未找到定义ColumnAttribute的列", entityType.Name));

            return table;    
        }
    }

    [Serializable]
    public class MappingException : Exception
    {
        public MappingException()
        {
        }

        public MappingException(string message)
            : base(message)
        {
        }
    }
}
