using SmartDb.NetCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartDb.MySql.NetCore
{
    public class MySqlBuilder : SqlBuilder
    {
        /// <summary>
        /// 单表分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryColumns"></param>
        /// <param name="whereSql"></param>
        /// <param name="sortCriteria"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="whereObjParams"></param>
        /// <returns></returns>
        public override DbEntity QueryPageList<T>(string  queryColumns, string whereSql, string sortColumn, string sortType, long pageSize, long pageIndex, object whereObjParams)
        {
            DbEntity dbEntity = base.QueryPageList<T>(queryColumns,whereSql,sortColumn,sortType,pageSize,pageIndex,whereObjParams);
            if (dbEntity == null)
            {
                return dbEntity;
            }
            Type type = typeof(T);
            var attributeBuilder = new AttributeBuilder();
            var tableEntity = attributeBuilder.GetTableInfo(type);
            dbEntity.TableEntity = tableEntity;
            var startNum = pageSize * (pageIndex - 1);
            var dbOperator = DbFactory.GetDbParamOperator();
            List<TableColumnAttribute> whereColumns = attributeBuilder.GetColumnInfos(whereObjParams);
            var dbParams = new List<IDbDataParameter>();

            //分页查询模板
            var queryTemplate = @"select  {queryColumns} from 
(
	select {sortColumn} from {tableName} {whereCriteria} order by {sortColumn} {sortType} limit {startNum},{pageSize}
) 
a inner join {tableName} b on a.{sortColumn}=b.{sortColumn} order by b.{sortColumn} {sortType};";
            StringBuilder sqlBuild = new StringBuilder(queryTemplate);
            sqlBuild.Replace("{sortColumn}", sortColumn);
            sqlBuild.Replace("{tableName}", tableEntity.TableName);
            sqlBuild.Replace("{sortType}", sortType);
            sqlBuild.Replace("{startNum}", startNum.ToString());
            sqlBuild.Replace("{pageSize}", pageSize.ToString());
            HandleQueryParam(queryColumns, "b", ref sqlBuild);
            HandleWhereParam(whereSql, whereColumns, ref sqlBuild, ref dbParams);
            dbEntity.CommandText = sqlBuild.ToString();
            dbEntity.DbParams = dbParams;
            return dbEntity;
        }
    }
}
