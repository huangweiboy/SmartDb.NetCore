using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Collections;
using System.Linq;
using SmartDb;
using SmartDb.NetCore;

namespace SmartDb.SqlServer.NetCore
{
   public class SqlServerBuilder:SqlBuilder
    {
        /// <summary>
        /// 单实体分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryColumns"></param>
        /// <param name="whereSql"></param>
        /// <param name="sortCriteria"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="whereObjParams"></param>
        /// <returns></returns>
        public override DbEntity QueryPageList<T>(string queryColumns, string whereSql, string sortColumn, string sortType, long pageSize, long pageIndex, object whereObjParams)
        {
            DbEntity dbEntity = null;
            if (string.IsNullOrEmpty(sortColumn) || string.IsNullOrEmpty(sortType))
            {
                return dbEntity;
            }
            if (pageSize <= 0 || pageIndex <= 0)
            {
                return dbEntity;
            }
            Type type = typeof(T);
            var attributeBuilder = new AttributeBuilder();
            var tableEntity = attributeBuilder.GetTableInfo(type);
            dbEntity = new DbEntity()
            {
                TableEntity = tableEntity
            };
            var startNum = pageSize * (pageIndex - 1)+1;
            var endNum = pageSize * pageIndex;
            var dbOperator = DbFactory.GetDbParamOperator();
            List<TableColumnAttribute> whereColumns = attributeBuilder.GetColumnInfos(whereObjParams);
            var dbParams = new List<IDbDataParameter>();

            //分页查询模板
            var queryTemplate = @"select  {queryColumns} from 
(
    select {sortColumn},ROW_NUMBER() over(order by {sortColumn} {sortType}) num from {tableName} {whereCriteria} order by {sortColumn} {sortType}
) 
a inner join {tableName} b on a.{sortColumn}=b.{sortColumn} and a.num between {startNum} and {endNum} order by b.{sortColumn} {sortType};";
            StringBuilder sqlBuild = new StringBuilder(queryTemplate);
            sqlBuild.Replace("{sortColumn}", sortColumn);
            sqlBuild.Replace("{tableName}", tableEntity.TableName);
            sqlBuild.Replace("{sortType}", sortType);
            sqlBuild.Replace("{startNum}", startNum.ToString());
            sqlBuild.Replace("{endNum}", endNum.ToString());
            sqlBuild.Replace("{pageSize}", pageSize.ToString());
            HandleQueryParam(queryColumns, "b", ref sqlBuild);
            HandleWhereParam(whereSql, whereColumns, ref sqlBuild, ref dbParams);
            dbEntity.CommandText = sqlBuild.ToString();
            dbEntity.DbParams = dbParams;
            return dbEntity;
        }
    }
}
