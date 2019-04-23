using System;
using System.Collections.Generic;
using System.Reflection;

namespace SmartDb.NetCore
{
    public class AttributeBuilder
    {
        /// <summary>
        /// 获取表信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public TableAttribute GetTableInfo(Type type)
        {
            TableAttribute tableEntity = new TableAttribute();
            tableEntity.TableName = type.Name;
            object[] customAttributes = type.GetCustomAttributes(typeof(TableAttribute), true);
            if (customAttributes == null)
            {
                return tableEntity;
            }
            if (customAttributes.Length <= 0)
            {
                return tableEntity;
            }
            tableEntity = customAttributes[0] as TableAttribute;
            return tableEntity;
        }

        /// <summary>
        /// 根据实体类型、实体对象实例获取表字段列表信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public List<TableColumnAttribute> GetColumnInfos(Type type,object entity)
        {
            List<TableColumnAttribute> columnEntityList = null;
            PropertyInfo[] propertys = type.GetProperties();
            if (propertys == null)
            {
                return columnEntityList;
            }
            if (propertys.Length <= 0)
            {
                return columnEntityList;
            }
            columnEntityList = new List<TableColumnAttribute>();
            foreach (PropertyInfo proInfo in propertys)
            {
                TableColumnAttribute systemColumn = GetProperInfo(proInfo, entity);
                TableColumnAttribute selftDefineColumn =GetCustomAttributeInfo(proInfo, entity);
                if (selftDefineColumn == null)
                {
                    columnEntityList.Add(systemColumn);
                    continue;
                }
                columnEntityList.Add(selftDefineColumn);
            }
            return columnEntityList;
       }

        /// <summary>
        /// 根据参数获取表字段列表
        /// </summary>
        /// <param name="param">参数对象,例:new {Uid=1,Uname="joyet"}</param>
        /// <returns></returns>
        public List<TableColumnAttribute> GetColumnInfos(object param)
        {
            List<TableColumnAttribute> columnEntityList = null;
            if (param == null)
            {
                return columnEntityList;
            }
            Type type = param.GetType();
            PropertyInfo[] props = type.GetProperties();
            if (props == null)
            {
                return columnEntityList;
            }
            if (props.Length == 0)
            {
                return columnEntityList;
            }
            columnEntityList = new List<TableColumnAttribute>();
            foreach (PropertyInfo proInfo in props)
            {
                TableColumnAttribute systemColumn = GetProperInfo(proInfo, param);
                columnEntityList.Add(systemColumn);
            }
            return columnEntityList;
        }

        /// <summary>
        /// 获取表主键字段信息
        /// </summary>
        /// <returns></returns>
        public TableColumnAttribute GetPkColumnInfo(Type type)
        {
            TableColumnAttribute pkColumn = null;
            PropertyInfo[] props = type.GetProperties();
            if (props == null)
            {
                return pkColumn;
            }
            if (props.Length == 0)
            {
                return pkColumn;
            }
            foreach (PropertyInfo proInfo in props)
            {
                TableColumnAttribute systemColumn = GetProperInfo(proInfo, null);
                if (systemColumn == null)
                {
                    return pkColumn;
                }
                TableColumnAttribute selftDefineColumn = GetCustomAttributeInfo(proInfo, null);
                if (selftDefineColumn == null)
                {
                    return pkColumn;
                }
                if (selftDefineColumn.IsPrimaryKey)
                {
                    pkColumn = selftDefineColumn;
                    break;
                }
            }
            return pkColumn;
        }

        /// <summary>
        /// 获取单个属性信息
        /// </summary>
        /// <param name="proInfo"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private TableColumnAttribute GetProperInfo(PropertyInfo proInfo, object entity)
       {
            TableColumnAttribute columnEntity = null;
            if (proInfo == null)
            {
                return columnEntity;
            }
            columnEntity = new TableColumnAttribute()
            {
                ColumnName = proInfo.Name
            };
            if (entity != null)
            {
                object attrValue = proInfo.GetValue(entity, null);
                if (attrValue != null)
                {
                    columnEntity.ColumnValue = attrValue;
                }
            }
            return columnEntity;
       }

        /// <summary>
        /// 获取单个属性的自定义特性信息
        /// </summary>
        /// <param name="proInfo"></param>
        /// <param name="param"></param>
        /// <returns></returns>
       private  TableColumnAttribute GetCustomAttributeInfo(PropertyInfo proInfo, object param)
       {
           TableColumnAttribute columnEntity = null;
           if (proInfo == null)
           {
                return columnEntity;
            }
            object[] customAttributes = proInfo.GetCustomAttributes(typeof(TableColumnAttribute), true);
            if (customAttributes == null)
            {
                return columnEntity;
            }
            if (customAttributes.Length<=0)
            {
                return columnEntity;
            }
            columnEntity = (TableColumnAttribute)customAttributes[0];
            if (columnEntity == null)
            {
                return columnEntity;
            }
            columnEntity.ColumnName = string.IsNullOrEmpty(columnEntity.ColumnName) ? proInfo.Name : columnEntity.ColumnName;
            if (param != null)
            {
                object attrValue = proInfo.GetValue(param, null);
                if (attrValue != null)
                {
                    columnEntity.ColumnValue = attrValue;
                }
            }
            return columnEntity;
       }

    }
}
